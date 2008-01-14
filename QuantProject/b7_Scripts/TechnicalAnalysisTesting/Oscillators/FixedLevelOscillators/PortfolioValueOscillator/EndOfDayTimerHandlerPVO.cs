/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerPVO.cs
Copyright (C) 2003 
Marco Milletti

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/
using System;
using System.Data;
using System.Collections;

using QuantProject.ADT;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies.TickersRelationships;
using QuantProject.Data;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
//using QuantProject.Data.Selectors.ByLinearDipendence;
using QuantProject.Data.DataTables;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.WalkForwardTesting.LinearCombination;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator
{
	/// <summary>
  /// Implements MarketOpenEventHandler and MarketCloseEventHandler
  /// These handlers contain the core strategy for the Portfolio Value
  /// Oscillator
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerPVO : EndOfDayTimerHandler
  {
    protected int minLevelForOversoldThreshold;
    protected int maxLevelForOversoldThreshold;
    protected int minLevelForOverboughtThreshold;
    protected int maxLevelForOverboughtThreshold;
    protected int divisorForThresholdComputation;
    protected int numDaysForOscillatingPeriod;
    protected double currentOversoldThreshold;
    protected double currentOverboughtThreshold;
    protected double maxAcceptableCloseToCloseDrawdown;
    protected double minimumAcceptableGain;
    protected DateTime lastCloseDate;
    protected IGenomeManager iGenomeManager;
    protected int seedForRandomGenerator;
    protected bool portfolioHasBeenOverbought;
    protected bool portfolioHasBeenOversold;
    protected bool symmetricalThresholds;
    protected bool overboughtMoreThanOversoldForFixedPortfolio;
    protected HistoricalAdjustedQuoteProvider historicalQuoteProvider;
        
    public EndOfDayTimerHandlerPVO(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
                                Account account,                                
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark,
                                int numDaysForOscillatingPeriod,
                                int minLevelForOversoldThreshold,
                                int maxLevelForOversoldThreshold,
                                int minLevelForOverboughtThreshold,
                                int maxLevelForOverboughtThreshold,
                                int divisorForThresholdComputation,
                                bool symmetricalThresholds,
                                bool overboughtMoreThanOversoldForFixedPortfolio,
                                int numDaysBetweenEachOptimization,
                                PortfolioType portfolioType, double maxAcceptableCloseToCloseDrawdown,
                                double minimumAcceptableGain):
    														base(tickerGroupID, numberOfEligibleTickers, 
                                numberOfTickersToBeChosen, numDaysForOptimizationPeriod, account,
                                generationNumberForGeneticOptimizer,
                                populationSizeForGeneticOptimizer,
                                benchmark, 0.0,
                                portfolioType)
    {
      this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
    	this.minLevelForOversoldThreshold  = minLevelForOversoldThreshold;
      this.maxLevelForOversoldThreshold = maxLevelForOversoldThreshold;
      this.minLevelForOverboughtThreshold = minLevelForOverboughtThreshold;
      this.maxLevelForOverboughtThreshold = maxLevelForOverboughtThreshold;
      this.divisorForThresholdComputation = divisorForThresholdComputation;
      this.symmetricalThresholds = symmetricalThresholds;
      this.overboughtMoreThanOversoldForFixedPortfolio = overboughtMoreThanOversoldForFixedPortfolio;
      this.maxAcceptableCloseToCloseDrawdown = maxAcceptableCloseToCloseDrawdown;
      this.minimumAcceptableGain = minimumAcceptableGain;
      this.stopLossConditionReached = false;
      this.currentAccountValue = 0.0;
      this.previousAccountValue = 0.0;
      this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
      this.seedForRandomGenerator = ConstantsProvider.SeedForRandomGenerator;
      this.portfolioHasBeenOverbought = false;
      this.portfolioHasBeenOversold = false;
      this.historicalQuoteProvider = new HistoricalAdjustedQuoteProvider();
    }
	
    public override void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
    	;
    }

    #region MarketCloseEventHandler
    
    protected virtual double getCurrentChosenWeightedPositionsReturn(IndexBasedEndOfDayTimer timer)
    {
      double returnValue = 999.0;
      try
      {
				DateTime firstDayOfOscillatingPeriod = 
					(DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition - this.numDaysForOscillatingPeriod]["quDate"];
				DateTime today = 
					(DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition]["quDate"];
				ReturnsManager returnsManager = new ReturnsManager(new CloseToCloseIntervals(
					new EndOfDayDateTime(firstDayOfOscillatingPeriod,
					EndOfDaySpecificTime.MarketClose) , 
					new EndOfDayDateTime(today,
					EndOfDaySpecificTime.MarketClose) ,
					this.benchmark , this.numDaysForOscillatingPeriod ) ,
					this.historicalQuoteProvider );
				returnValue = this.chosenWeightedPositions.GetReturn(0,returnsManager);
      }
    	catch(MissingQuotesException ex)
    	{
    		ex = ex;
    	}
    	return returnValue;
    }   

    private void marketCloseEventHandler_reverseIfNeeded(IndexBasedEndOfDayTimer timer)
    {
      double currentChosenWeightedPositionsReturn = 
        this.getCurrentChosenWeightedPositionsReturn(timer);
      if(currentChosenWeightedPositionsReturn != 999.0 &&
         this.portfolioType == PortfolioType.ShortAndLong)
        //currentChosenWeightedPositionsReturn has been
      	//properly computedand it is possible to reverse positions
      {
    		if(currentChosenWeightedPositionsReturn >= currentOverboughtThreshold &&
      	   this.portfolioHasBeenOversold)
    		//open positions derive from an overSold period but now
    		//the overbought threshold has been reached
    		{
    			this.reversePositions();
    			this.portfolioHasBeenOversold = false;
    			this.portfolioHasBeenOverbought = true;
    			this.previousAccountValue = this.account.GetMarketValue();
    		}
    		if(currentChosenWeightedPositionsReturn <= - currentOversoldThreshold &&
      	   this.portfolioHasBeenOverbought)
    		//open positions derive from an overBought period but now
    		//the overSold threshold has been reached
    		{
    			this.reversePositions();
    			this.portfolioHasBeenOversold = true;
    			this.portfolioHasBeenOverbought = false;
    			this.previousAccountValue = this.account.GetMarketValue();
    		}	
      }
    }

    protected void marketCloseEventHandler_openPositions(IndexBasedEndOfDayTimer timer)
    {
      if(this.account.CashAmount == 0.0 && this.account.Transactions.Count == 0)
				this.account.AddCash(15000);
			double currentChosenWeightedPositionsReturn = 
      		this.getCurrentChosenWeightedPositionsReturn(timer);
    	if(currentChosenWeightedPositionsReturn != 999.0)
    	//currentChosenTickersValue has been properly computed
    	{
    		if(currentChosenWeightedPositionsReturn >= currentOverboughtThreshold &&
           this.portfolioType == PortfolioType.ShortAndLong)
    		{
    			this.chosenWeightedPositions.Reverse();
          try
          {
            AccountManager.OpenPositions( this.chosenWeightedPositions,
          	                            	this.account );
            this.portfolioHasBeenOverbought = true;
          	this.portfolioHasBeenOversold = false;
          	this.previousAccountValue = this.account.GetMarketValue();
          }
          catch(Exception ex)
          {
            ex = ex;
          }
          finally
          {
            this.chosenWeightedPositions.Reverse();
          }
    		}
        else if (currentChosenWeightedPositionsReturn <= - currentOversoldThreshold)
    		{
          AccountManager.OpenPositions( this.chosenWeightedPositions,
          	                            this.account );
          this.portfolioHasBeenOverbought = false;
          this.portfolioHasBeenOversold = true;
          this.previousAccountValue = this.account.GetMarketValue();
        }
     	}
    }
    
    protected virtual void marketCloseEventHandler_closePositionsIfNeeded()
    {
      if(this.stopLossConditionReached ||
    	   this.takeProfitConditionReached ||
         this.numDaysElapsedSinceLastOptimization + 1 == this.numDaysBetweenEachOptimization )
      {    
    		AccountManager.ClosePositions(this.account);
        this.portfolioHasBeenOverbought = false;
        this.portfolioHasBeenOversold = false;
      }
    }    
    
    protected virtual void marketCloseEventHandler_updateStopLossAndTakeProfitConditions()
    {
      //this.previousAccountValue has been set at opening positions
      this.currentAccountValue = this.account.GetMarketValue();
      double portfolioGainOrLoss = (this.currentAccountValue - this.previousAccountValue)
           													/this.previousAccountValue;
      
      if( portfolioGainOrLoss <= -this.maxAcceptableCloseToCloseDrawdown )
      {
        this.stopLossConditionReached = true;
        this.takeProfitConditionReached = false;
      }
      else if (portfolioGainOrLoss >= this.minimumAcceptableGain)
               
      {
        this.stopLossConditionReached = false;
        this.takeProfitConditionReached = true;
      }
    }
    
    public override void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      this.marketCloseEventHandler_updateStopLossAndTakeProfitConditions();  
      this.marketCloseEventHandler_closePositionsIfNeeded();
      if(this.chosenWeightedPositions != null)
      //tickers to buy have been chosen by the optimizer
      {
      	if(this.account.Portfolio.Count == 0)
      			this.marketCloseEventHandler_openPositions((IndexBasedEndOfDayTimer)sender);
        		//positions are opened only if thresholds are reached
        else//there are some opened positions
            this.marketCloseEventHandler_reverseIfNeeded((IndexBasedEndOfDayTimer)sender);
      }
    }

    #endregion
    
    #region OneHourAfterMarketCloseEventHandler
  
    protected virtual DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
    {
			SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID, currentDate);
      DataTable tickersFromGroup = temporizedGroup.GetTableOfSelectedTickers();
      int numOfTickersInGroupAtCurrentDate = tickersFromGroup.Rows.Count;
      
      SelectorByAverageRawOpenPrice byPrice =
      		new SelectorByAverageRawOpenPrice(tickersFromGroup,false,currentDate.AddDays(-15),
      	                                  currentDate,
      	                                  numOfTickersInGroupAtCurrentDate,
      	                                  30,3000, 0.0001,100);
 
      SelectorByLiquidity mostLiquidSelector =
      	new SelectorByLiquidity(byPrice.GetTableOfSelectedTickers(),
        false,currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
        this.numberOfEligibleTickers);
      
//      SelectorByCloseToCloseCorrelationToBenchmark byCorrelationToBenchmark =
//      	new SelectorByCloseToCloseCorrelationToBenchmark(mostLiquidSelector.GetTableOfSelectedTickers(),
//      	                                                 "^GSPC",false,
//      	                                                 currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
//      	                                                 this.numberOfEligibleTickers/2,false);
//      
      
      SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromLastSelection = 
        new SelectorByQuotationAtEachMarketDay(mostLiquidSelector.GetTableOfSelectedTickers(),
        false, currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
        this.numberOfEligibleTickers, this.benchmark);
//      SelectorByMaxLinearDipendence dipendentTickers = 
//				new SelectorByMaxLinearDipendence(quotedAtEachMarketDayFromLastSelection.GetTableOfSelectedTickers(),
//						currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
//						this.numberOfEligibleTickers/2,2,1000, this.benchmark);
//			return dipendentTickers.GetTableOfSelectedTickers();
				
//				DataTable quotedAtEachMarketDay = 
//					quotedAtEachMarketDayFromLastSelection.GetTableOfSelectedTickers();
//				
//				CloseToCloseCorrelationProvider correlationProvider = 
//					new CloseToCloseCorrelationProvider(
//							QuantProject.ADT.ExtendedDataTable.GetArrayOfStringFromColumn(
//							quotedAtEachMarketDay, 0 ),
//              currentDate.AddDays(-this.numDaysForOptimizationPeriod),
//              currentDate,1,0.005,this.benchmark);

//			SelectorByCloseToCloseCorrelationToBenchmark byCorrelationToBenchmark =
//				new SelectorByCloseToCloseCorrelationToBenchmark(
//					quotedAtEachMarketDayFromLastSelection.GetTableOfSelectedTickers(),
//					this.benchmark, false,
//					currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
//					this.numberOfEligibleTickers / 2, false);
//			return byCorrelationToBenchmark.GetTableOfSelectedTickers();
      
			return quotedAtEachMarketDayFromLastSelection.GetTableOfSelectedTickers();
    	//for debug
//      SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromLastSelection = 
//        new SelectorByQuotationAtEachMarketDay(temporizedGroup.GetTableOfSelectedTickers(),
//        false, currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
//        this.numberOfEligibleTickers, this.benchmark);
//      return quotedAtEachMarketDayFromLastSelection.GetTableOfSelectedTickers();
    }
    protected void addPVOGenomeToBestGenomes(Genome genome,
                                              DateTime firstOptimizationDate,
                                              DateTime secondOptimizationDate,
                                              int eligibleTickers, int daysForOscillatingPeriod,
                                              PortfolioType portfolioType,
                                              int createdGenerations,
                                              double oversoldThreshold,
                                              double overboughtThreshold)
    {
      if(this.bestGenomes == null)
        this.bestGenomes = new ArrayList();
      
      this.bestGenomes.Add(new GenomeRepresentation(genome,
                              firstOptimizationDate,
                              secondOptimizationDate,
                              genome.Generation,
                              eligibleTickers,
                              daysForOscillatingPeriod, portfolioType, createdGenerations,
                              oversoldThreshold, overboughtThreshold));
    }
    
    protected virtual void setTickers(DateTime currentDate,
      bool setGenomeCounter)
    {
      DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
      if(setOfTickersToBeOptimized.Rows.Count > this.numberOfTickersToBeChosen*2)
        //the optimization process is meaningful only if the initial set of tickers is 
        //larger than the number of tickers to be chosen                     
      
      {
        this.iGenomeManager =
          new GenomeManagerPVO(setOfTickersToBeOptimized,
          currentDate.AddDays(-this.numDaysForOptimizationPeriod), 
          currentDate, this.numberOfTickersToBeChosen,
          this.numDaysForOscillatingPeriod,
          this.minLevelForOversoldThreshold,
          this.maxLevelForOversoldThreshold,
          this.minLevelForOverboughtThreshold,
          this.maxLevelForOverboughtThreshold,
          this.divisorForThresholdComputation,
          this.symmetricalThresholds,
          this.overboughtMoreThanOversoldForFixedPortfolio,
          this.portfolioType,this.benchmark);
        GeneticOptimizer GO = new GeneticOptimizer(this.iGenomeManager,
          this.populationSizeForGeneticOptimizer, 
          this.generationNumberForGeneticOptimizer,
          this.seedForRandomGenerator);
        if(setGenomeCounter)
          this.genomeCounter = new GenomeCounter(GO);
        GO.MutationRate = 0.1;
        GO.Run(false);
        
        this.chosenWeightedPositions = new WeightedPositions( ((GenomeMeaningPVO)GO.BestGenome.Meaning).TickersPortfolioWeights,
					new SignedTickers( ((GenomeMeaningPVO)GO.BestGenome.Meaning).Tickers) );
        this.currentOversoldThreshold = ((GenomeMeaningPVO)GO.BestGenome.Meaning).OversoldThreshold;
        this.currentOverboughtThreshold = ((GenomeMeaningPVO)GO.BestGenome.Meaning).OverboughtThreshold;
        this.addPVOGenomeToBestGenomes(GO.BestGenome,((GenomeManagerForEfficientPortfolio)this.iGenomeManager).FirstQuoteDate,
          ((GenomeManagerForEfficientPortfolio)this.iGenomeManager).LastQuoteDate, setOfTickersToBeOptimized.Rows.Count,
           this.numDaysForOscillatingPeriod, this.portfolioType, GO.GenerationCounter,
           this.currentOversoldThreshold, this.currentOverboughtThreshold);
      }
      //else it will be buyed again the previous optimized portfolio
      //that's it the actual chosenTickers member
    }
    
    /// <summary>
    /// Handles a "One hour after market close" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public override void OneHourAfterMarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      this.lastCloseDate = endOfDayTimingEventArgs.EndOfDayDateTime.DateTime;
      this.seedForRandomGenerator++;
      this.numDaysElapsedSinceLastOptimization++;
      if((this.numDaysElapsedSinceLastOptimization == 
            this.numDaysBetweenEachOptimization))
      //num days without optimization has elapsed
      {
        this.setTickers(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime, false);
        //sets tickers to be chosen next Market Close event
        this.numDaysElapsedSinceLastOptimization = 0;
      }
		      
    }
		#endregion
  }
}
