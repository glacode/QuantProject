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
using QuantProject.Data;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
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
    protected bool stopLossConditionReached;
    protected double currentAccountValue;
    protected double previousAccountValue;
    protected int numDaysBetweenEachOptimization;
    protected int numDaysElapsedSinceLastOptimization;
    protected DateTime lastCloseDate;
    protected IGenomeManager iGenomeManager;
    protected int seedForRandomGenerator;
    protected bool portfolioHasBeenOverbought;
    protected bool portfolioHasBeenOversold;
    protected bool symmetricalThresholds;
    protected bool overboughtMoreThanOversoldForFixedPortfolio;
        
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
                                PortfolioType portfolioType, double maxAcceptableCloseToCloseDrawdown):
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
      this.stopLossConditionReached = false;
      this.currentAccountValue = 0.0;
      this.previousAccountValue = 0.0;
      this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
      this.seedForRandomGenerator = ConstantsProvider.SeedForRandomGenerator;
      this.portfolioHasBeenOverbought = false;
      this.portfolioHasBeenOversold = false;
    }
	

    #region MarketCloseEventHandler
    
    protected void marketCloseEventHandler_updateStopLossCondition()
    {
      this.previousAccountValue = this.currentAccountValue;
      this.currentAccountValue = this.account.GetMarketValue();
      if((this.currentAccountValue - this.previousAccountValue)
           /this.previousAccountValue < -this.maxAcceptableCloseToCloseDrawdown)
      {
        this.stopLossConditionReached = true;
      }
      else
      {
        this.stopLossConditionReached = false;
      }
    }    

    protected virtual double getCurrentChosenTickersValue(IndexBasedEndOfDayTimer timer)
    {
      double returnValue = 999.0;
      try
      {
		    DateTime initialDate = 
	          (DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition - this.numDaysForOscillatingPeriod + 2]["quDate"];
	      //so to replicate exactly in sample scheme, where only numOscillatingDay - 1 returns
        //are computed
        DateTime finalDate = 
	        (DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition]["quDate"];
      	returnValue =
	      	 SignedTicker.GetCloseToClosePortfolioReturn(
	      	     this.chosenTickers, this.chosenTickersPortfolioWeights,
	      	     initialDate,finalDate) + 1.0;
      }
    	catch(MissingQuotesException ex)
    	{
    		ex = ex;
    	}
    	return returnValue;
    }   

    private void marketCloseEventHandler_reverseIfNeeded(IndexBasedEndOfDayTimer timer)
    {
      double currentChosenTickersValue = 
        this.getCurrentChosenTickersValue(timer);
      if(currentChosenTickersValue != 999.0)
        //currentChosenTickersValue has been properly computed
      {
      	if(this.portfolioType == PortfolioType.ShortAndLong)
      	//it is possible to reverse positions
      	{
      		if(currentChosenTickersValue >= 1.0 + currentOverboughtThreshold &&
      	    this.portfolioHasBeenOversold)
      		//open positions derive from an overSold period but now
      		//an the overbought threshold has been reached
      		{
      			this.reversePositions();
      			this.portfolioHasBeenOversold = false;
      			this.portfolioHasBeenOverbought = true;
      		}
      		if(currentChosenTickersValue <= 1.0 - currentOversoldThreshold &&
      	    this.portfolioHasBeenOverbought)
      		//open positions derive from an overSold period but now
      		//an the overbought threshold has been reached
      		{
      			this.reversePositions();
      			this.portfolioHasBeenOversold = true;
      			this.portfolioHasBeenOverbought = false;
      		}	
      	}
      }
    }

    private void marketCloseEventHandler_openPositions(IndexBasedEndOfDayTimer timer)
    {
      double currentChosenTickersValue = 
      		this.getCurrentChosenTickersValue(timer);
    	if(currentChosenTickersValue != 999.0)
    	//currentChosenTickersValue has been properly computed
    	{
    		if(currentChosenTickersValue >= 1.0 + currentOverboughtThreshold &&
           this.portfolioType == PortfolioType.ShortAndLong)
    		{
          SignedTicker.ChangeSignOfEachTicker(this.chosenTickers);
         
          //short the portfolio
          try
          {
            base.openPositions(this.chosenTickers);
            this.portfolioHasBeenOverbought = true;
          	this.portfolioHasBeenOversold = false;
          }
          catch(Exception ex)
          {
            ex = ex;
          }
          finally
          {
            SignedTicker.ChangeSignOfEachTicker(this.chosenTickers);
          }
    		}
        else if (currentChosenTickersValue <= 1.0 - currentOversoldThreshold)
    		{
          base.openPositions(this.chosenTickers);
          this.portfolioHasBeenOverbought = false;
          this.portfolioHasBeenOversold = true;
        }
    	}
    }
    
    protected virtual void marketCloseEventHandler_closePositionsIfNeeded()
    {
      if(this.stopLossConditionReached ||
        this.numDaysElapsedSinceLastOptimization + 1 == this.numDaysBetweenEachOptimization )
      {    
        base.closePositions();
        //a new optimization is needed, now
        this.chosenTickers[0] = null;
        //when positions are closed, these parameters
        //have to be reset to false
        this.portfolioHasBeenOverbought = false;
        this.portfolioHasBeenOversold = false;
      }
    }    
    
    public override void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      //this.marketCloseEventHandler_updateStopLossCondition();  
      this.marketCloseEventHandler_closePositionsIfNeeded();
      if(this.chosenTickers[0] != null)
      //tickers to buy have been chosen by the optimizer
      {
      	if(this.account.Portfolio.Count == 0)
      			this.marketCloseEventHandler_openPositions((IndexBasedEndOfDayTimer)sender);
        		//positions are opened only if thresholds are reached
        else//there are some opened positions
            this.marketCloseEventHandler_reverseIfNeeded((IndexBasedEndOfDayTimer)sender);;
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
      		new SelectorByAverageRawOpenPrice(tickersFromGroup,false,currentDate.AddDays(-30),
      	                                  currentDate,
      	                                  numOfTickersInGroupAtCurrentDate,
      	                                  20,500, 0.0001,100);
 
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
      if(setOfTickersToBeOptimized.Rows.Count > this.chosenTickers.Length*2)
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
          this.portfolioType);
        GeneticOptimizer GO = new GeneticOptimizer(this.iGenomeManager,
          this.populationSizeForGeneticOptimizer, 
          this.generationNumberForGeneticOptimizer,
          this.seedForRandomGenerator);
        if(setGenomeCounter)
          this.genomeCounter = new GenomeCounter(GO);
        GO.MutationRate = 0.2;
        GO.Run(false);
        
        this.chosenTickers = ((GenomeMeaningPVO)GO.BestGenome.Meaning).Tickers;
        this.chosenTickersPortfolioWeights = ((GenomeMeaningPVO)GO.BestGenome.Meaning).TickersPortfolioWeights;
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
      this.orders.Clear();
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
