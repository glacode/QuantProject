/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerBiasedPVO.cs
Copyright (C) 2006 
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
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.WeightedPVO.WeightedBalancedPVO;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.BiasedPVO
{
	/// <summary>
  /// Implements MarketOpenEventHandler and MarketCloseEventHandler
  /// These handlers contain the core strategy for the Biased Portfolio Value
  /// Oscillator
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerBiasedPVO : EndOfDayTimerHandlerPVO
  {
    protected int numOfDifferentGenomesToEvaluateOutOfSample;
    protected double minimumAcceptableGain;
    protected int currentGenomeIndex = 0;
    protected double currentTickersGainOrLoss = 0.0;
    protected Hashtable genomesCollector;
    protected bool takeProfitConditionReached;
    protected string[,] bestGenomesChosenTickers;
    protected double[,] bestGenomesChosenTickersPortfolioWeights;
    
    new protected double[] currentOversoldThreshold;
    new protected double[] currentOverboughtThreshold;
     
    
    public EndOfDayTimerHandlerBiasedPVO(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
                                Account account,                                
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark,
                                int numOfDifferentGenomesToEvaluateOutOfSample,
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
                                numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
                                account,                                
                                generationNumberForGeneticOptimizer,
                                populationSizeForGeneticOptimizer,
                                benchmark,
                                numDaysForOscillatingPeriod,
                                minLevelForOversoldThreshold,
                                maxLevelForOversoldThreshold,
                                minLevelForOverboughtThreshold,
                                maxLevelForOverboughtThreshold,
                                divisorForThresholdComputation,
                                symmetricalThresholds,
                                overboughtMoreThanOversoldForFixedPortfolio,
                                numDaysBetweenEachOptimization,
                                portfolioType, maxAcceptableCloseToCloseDrawdown)
    {
      
      this.numOfDifferentGenomesToEvaluateOutOfSample = numOfDifferentGenomesToEvaluateOutOfSample;
      this.minimumAcceptableGain = minimumAcceptableGain;
      this.bestGenomesChosenTickers = new string[numOfDifferentGenomesToEvaluateOutOfSample, numberOfTickersToBeChosen];
      this.bestGenomesChosenTickersPortfolioWeights = new double[numOfDifferentGenomesToEvaluateOutOfSample, numberOfTickersToBeChosen];
      this.currentOversoldThreshold = new double[numOfDifferentGenomesToEvaluateOutOfSample];
      this.currentOverboughtThreshold = new double[numOfDifferentGenomesToEvaluateOutOfSample];
      this.genomesCollector = new Hashtable();
    }
	

    #region MarketCloseEventHandler
    
    protected virtual double getCurrentChosenTickersGainOrLoss(IndexBasedEndOfDayTimer timer,
                                                int indexForChosenTickers)
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
      	string[] tickers = new string[this.numberOfTickersToBeChosen];
        double[] tickerWeights = new double[this.numberOfTickersToBeChosen];
        for(int i = 0; i < this.numberOfTickersToBeChosen; i++)
        {
          tickers[i] = this.bestGenomesChosenTickers[indexForChosenTickers,i];
          tickerWeights[i] = this.bestGenomesChosenTickersPortfolioWeights[indexForChosenTickers,i];
        }
        returnValue =
	      	 SignedTicker.GetCloseToClosePortfolioReturn(
	      	     tickers, tickerWeights,
	      	     initialDate,finalDate) + 1.0;
      }
    	catch(MissingQuotesException ex)
    	{
    		ex = ex;
    	}
    	return returnValue;
    }   

    private void marketCloseEventHandler_reverseOrClose(IndexBasedEndOfDayTimer timer)
    {
      double currentChosenTickersGainOrLoss = 
        this.getCurrentChosenTickersGainOrLoss(timer, this.currentGenomeIndex);
      this.marketCloseEventHandler_updateStopLossAndTakeProfitConditions();
      if(currentChosenTickersGainOrLoss != 999.0)
        //currentChosenTickersValue has been properly computed
      {
    		if(currentChosenTickersGainOrLoss >= 1.0 + currentOverboughtThreshold[this.currentGenomeIndex] &&
    	    this.portfolioHasBeenOversold)
    		//open positions derive from an overSold period but now
    		//the overbought threshold has been reached
    		{
    			this.reversePositions();
    			this.portfolioHasBeenOversold = false;
    			this.portfolioHasBeenOverbought = true;
    		}
    		else if(currentChosenTickersGainOrLoss <= 1.0 - currentOversoldThreshold[this.currentGenomeIndex] &&
    	    			this.portfolioHasBeenOverbought)
    		//open positions derive from an overBought period but now
    		//the overSold threshold has been reached
    		{
    			this.reversePositions();
    			this.portfolioHasBeenOversold = true;
    			this.portfolioHasBeenOverbought = false;
    		}
      	else if(this.stopLossConditionReached ||
         	 			this.takeProfitConditionReached ||
        	 			this.numDaysElapsedSinceLastOptimization + 1 == this.numDaysBetweenEachOptimization )
      	//reversal conditions have not been reached but 
      	//stop loss or take profit conditions yes
      	//or after the close it is necessary to run
      	//another optimization
      	{
	        base.closePositions();
          this.orders.Clear();
	        this.portfolioHasBeenOverbought = false;
	        this.portfolioHasBeenOversold = false;
      	}
      }
    }

    private void marketCloseEventHandler_closeIfItIsTimeToClose()
    {
      this.marketCloseEventHandler_updateStopLossAndTakeProfitConditions();
      if(this.stopLossConditionReached ||
          this.takeProfitConditionReached ||
          this.numDaysElapsedSinceLastOptimization + 1 == this.numDaysBetweenEachOptimization )
          //stop loss or take profit conditions yes
          //or after the next close it is necessary to run
          //another optimization
      {
        base.closePositions();
        this.orders.Clear();
        this.portfolioHasBeenOverbought = false;
        this.portfolioHasBeenOversold = false;
      }
    }

    //sets currentGenomeIndex with the genome's index that crosses an overbought/oversold threshold with the
    //highest degree and sets currentTickersGainOrLoss accordingly
    private void marketCloseEventHandler_openPositions_chooseBestGenome(IndexBasedEndOfDayTimer timer)
    {
      //default index is the first
      //genome (with the highest plain fitness), if no other genome
      //has crossed the threshold with a higher degree than the first genome
      double currentMaxDegreeOfCrossingThreshold = 0.0;
      double currentDegreeOfCrossingThreshold = 0.0;
      for(int i = 0; i < this.numOfDifferentGenomesToEvaluateOutOfSample; i++)
      {
        double currentChosenTickersGainOrLoss = 
                 this.getCurrentChosenTickersGainOrLoss(timer, i);
        if(currentChosenTickersGainOrLoss != 999.0)
          //currentChosenTickersValue has been properly computed
        {
          //computing degree of crossing threshold 
          if(currentChosenTickersGainOrLoss >= 1.0 + this.currentOverboughtThreshold[i])
          {
            currentDegreeOfCrossingThreshold = 
              (currentChosenTickersGainOrLoss - 1.0 - this.currentOverboughtThreshold[i])/
            		(1 + this.currentOverboughtThreshold[i]);
 
          }
          else if (currentChosenTickersGainOrLoss <= 1.0 - this.currentOversoldThreshold[i])
          {
            currentDegreeOfCrossingThreshold = 
              (1.0 - this.currentOversoldThreshold[i] - currentChosenTickersGainOrLoss)/
            		(1.0 - this.currentOversoldThreshold[i]);
          }
          if(currentDegreeOfCrossingThreshold > currentMaxDegreeOfCrossingThreshold)
          {
          	currentMaxDegreeOfCrossingThreshold = currentDegreeOfCrossingThreshold;
          	this.currentGenomeIndex = i;
          	this.currentTickersGainOrLoss = currentChosenTickersGainOrLoss;
          }
        }
      }
   }


    private void marketCloseEventHandler_openPositions(IndexBasedEndOfDayTimer timer)
    {
      this.currentTickersGainOrLoss = 999.0; //
    	this.marketCloseEventHandler_openPositions_chooseBestGenome(timer);
      if(this.currentTickersGainOrLoss != 999.0)
    	//currentChosenTickersValue has been properly computed
    	{
        string[] tickers = new string[this.numberOfTickersToBeChosen];
        double[] tickersWeights = new double[this.numberOfTickersToBeChosen];
        for(int i = 0; i < this.numberOfTickersToBeChosen; i++)
        {
          tickers[i] = this.bestGenomesChosenTickers[this.currentGenomeIndex,i];
          tickersWeights[i] = this.bestGenomesChosenTickersPortfolioWeights[this.currentGenomeIndex,i];
        }
          if(this.currentTickersGainOrLoss >= 1.0 + currentOverboughtThreshold[this.currentGenomeIndex] &&
           this.portfolioType == PortfolioType.ShortAndLong)
    		{
          SignedTicker.ChangeSignOfEachTicker(tickers);
          base.openPositions(tickers, tickersWeights);
          this.portfolioHasBeenOverbought = true;
          this.portfolioHasBeenOversold = false;
        }
        else if (this.currentTickersGainOrLoss <= 1.0 - currentOversoldThreshold[this.currentGenomeIndex])
    		{
          base.openPositions(tickers, tickersWeights);
          this.portfolioHasBeenOverbought = false;
          this.portfolioHasBeenOversold = true;
        }
        this.currentAccountValue = this.account.GetMarketValue();
    	}
    }
        
    private void marketCloseEventHandler_updateStopLossAndTakeProfitConditions()
    {
      this.previousAccountValue = this.currentAccountValue;
      this.currentAccountValue = this.account.GetMarketValue();
      double portfolioGainOrLoss = (this.currentAccountValue - this.previousAccountValue)
           													/this.previousAccountValue;
      
      if( portfolioGainOrLoss < -this.maxAcceptableCloseToCloseDrawdown )
      {
        this.stopLossConditionReached = true;
        this.takeProfitConditionReached = false;
      }
      else if (portfolioGainOrLoss >= this.minimumAcceptableGain)
               
      {
        this.stopLossConditionReached = false;
        this.takeProfitConditionReached = true;
      }
      else
      {
      	this.stopLossConditionReached = false;
        this.takeProfitConditionReached = false;
      }
    }   

    public override void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
     	if(this.account.Portfolio.Count > 0)
    	 	//this.marketCloseEventHandler_reverseOrClose((IndexBasedEndOfDayTimer)sender);
     	  this.marketCloseEventHandler_closeIfItIsTimeToClose();
      else if ( this.account.Portfolio.Count == 0 &&
    	         this.bestGenomesChosenTickers[0,0] != null )
			//portfolio is empty and optimization has been already launched    		
    		this.marketCloseEventHandler_openPositions((IndexBasedEndOfDayTimer)sender);
    }

    #endregion
    

    #region OneHourAfterMarketCloseEventHandler
        
    private void setTickers_updateTickersWeightsAndThresholdsAndAddGenomesForLog(GeneticOptimizer GO,
                                                                                 int eligibleTickersForGO)
    {
    	int addedGenomes = 0;
      int counter = 0;
      Genome currentGenome = null;
      this.genomesCollector.Clear();
      while(addedGenomes < this.numOfDifferentGenomesToEvaluateOutOfSample && 
            counter < GO.PopulationSize)
      {
      	currentGenome = (Genome)GO.CurrentGeneration[GO.PopulationSize - 1 - counter];
      	if(   counter == 0 || 
      	      !this.genomesCollector.ContainsKey(
              ( (GenomeMeaning)currentGenome.Meaning ).HashCodeForTickerComposition   )   )
//								currentGenome.Fitness)   )
				//it is the first genome to be added or no genome with the current
      	// fitness has been added to the hashtable yet
      	{
      		for(int i = 0; i<this.numberOfTickersToBeChosen; i++)
      		{
      			this.bestGenomesChosenTickers[addedGenomes,i] = 
      				((GenomeMeaningPVO)currentGenome.Meaning).Tickers[i];
      			this.bestGenomesChosenTickersPortfolioWeights[addedGenomes,i] =
      				((GenomeMeaningPVO)currentGenome.Meaning).TickersPortfolioWeights[i];
      		}
      		this.currentOversoldThreshold[addedGenomes] = 
      				((GenomeMeaningPVO)currentGenome.Meaning).OversoldThreshold;
      		this.currentOverboughtThreshold[addedGenomes] = 
      				((GenomeMeaningPVO)currentGenome.Meaning).OverboughtThreshold;
      		
      		this.genomesCollector.Add(
      		 ( (GenomeMeaning)currentGenome.Meaning ).HashCodeForTickerComposition, null);
//						currentGenome.Fitness,null);
      		this.addPVOGenomeToBestGenomes(currentGenome,((GenomeManagerForEfficientPortfolio)this.iGenomeManager).FirstQuoteDate,
          ((GenomeManagerForEfficientPortfolio)this.iGenomeManager).LastQuoteDate, eligibleTickersForGO,
           this.numDaysForOscillatingPeriod, this.portfolioType, GO.GenerationCounter,
           this.currentOversoldThreshold[addedGenomes], this.currentOverboughtThreshold[addedGenomes]);
      	  addedGenomes ++ ;
      	}
      	counter ++ ;
      }  
    }
    
    protected override void setTickers(DateTime currentDate,
      bool setGenomeCounter)
    {
      DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
      this.iGenomeManager =
          new GenomeManagerWeightedBalancedPVO(setOfTickersToBeOptimized,
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
      
      GO.MutationRate = 0.1;
      GO.CrossoverRate = 0.85;
      GO.Run(false);
       
      this.setTickers_updateTickersWeightsAndThresholdsAndAddGenomesForLog(GO, setOfTickersToBeOptimized.Rows.Count);
           	
    }
    
    #endregion

  }
}
