/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerBiasedOTC_PVONoThresholds.cs
Copyright (C) 2007 
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

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.BiasedPVO.BiasedOTC_PVONoThresholds
{
	/// <summary>
  /// Implements MarketOpenEventHandler and MarketCloseEventHandler
  /// These handlers contain the core strategy for the Biased Portfolio Value
  /// Oscillator, with no thresholds and including quotes at open
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerBiasedOTC_PVONoThresholds : EndOfDayTimerHandlerPVO
  {
    protected int numOfDifferentGenomesToEvaluateOutOfSample;
    protected int currentGenomeIndex = 0;
    protected double currentWeightedPositionsGainOrLoss = 0.0;
    protected Hashtable genomesCollector;
    
		protected WeightedPositions[] weightedPositionsToEvaluateOutOfSample;
        
    public EndOfDayTimerHandlerBiasedOTC_PVONoThresholds(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
                                Account account,                                
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark,
                                int numOfDifferentGenomesToEvaluateOutOfSample,
                                int numDaysBetweenEachOptimization,
                                PortfolioType portfolioType):
    														base(tickerGroupID, numberOfEligibleTickers, 
                                numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
                                account,                                
                                generationNumberForGeneticOptimizer,
                                populationSizeForGeneticOptimizer,
                                benchmark,
                                2,
                                0,
                                0,
                                0,
                                0,
                                1,
                                true,
                                false,
                                numDaysBetweenEachOptimization,
                                portfolioType, 0.0)
    {
      this.numOfDifferentGenomesToEvaluateOutOfSample = numOfDifferentGenomesToEvaluateOutOfSample;
			this.weightedPositionsToEvaluateOutOfSample = new WeightedPositions[numOfDifferentGenomesToEvaluateOutOfSample];
      this.genomesCollector = new Hashtable();
    }
  
    protected virtual double getCurrentWeightedPositionsGainOrLoss(IndexBasedEndOfDayTimer timer,
                                                int indexForChosenWeightedPositions)
    {
      double returnValue = 999.0;
      try
      {
		    DateTime today = 
	          (DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition]["quDate"];
        DateTime lastMarketDay = 
          (DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition - 1]["quDate"];
        returnValue =
	      	 this.weightedPositionsToEvaluateOutOfSample[indexForChosenWeightedPositions].GetLastNightReturn(
	      	      	     lastMarketDay, today);
      }
    	catch(MissingQuotesException ex)
    	{
    		ex = ex;
    	}
    	return returnValue;
    }   

	  //sets currentGenomeIndex with the genome's index that crosses an overbought/oversold threshold with the
    //highest degree and sets currentTickersGainOrLoss accordingly
    protected virtual void openPositions_chooseBestGenome(IndexBasedEndOfDayTimer timer)
    {
      //default genome index is the first
      //genome (with the highest plain fitness), if no other genome
      //presents a gain or a loss greater than the first genome
      double currentWeightedPositionsGainOrLoss = 0.0;
      double currentMaxAbsoluteMove = 0.0;
      double currentAbsoluteMove = 0.0;
      for(int i = 0; i < this.numOfDifferentGenomesToEvaluateOutOfSample; i++)
      {
        currentWeightedPositionsGainOrLoss = this.getCurrentWeightedPositionsGainOrLoss(timer, i);
      	currentAbsoluteMove =
        	Math.Abs( currentWeightedPositionsGainOrLoss );
        if(currentAbsoluteMove != 999.0 && 
           currentAbsoluteMove > currentMaxAbsoluteMove)
          //currentAbsoluteMove has been properly computed and it is
        	//greater than the current greatest move up to date
        {
          currentMaxAbsoluteMove = currentAbsoluteMove;
        	this.currentGenomeIndex = i;
        	this.currentWeightedPositionsGainOrLoss = currentWeightedPositionsGainOrLoss;
        }
      }
    }
    //no thresholds
		protected virtual void openPositions(IndexBasedEndOfDayTimer timer)
    {
      this.currentWeightedPositionsGainOrLoss = 999.0;
    	this.openPositions_chooseBestGenome(timer);
      if(this.currentWeightedPositionsGainOrLoss != 999.0)
    	//currentWeightedPositionsGainOrLoss has been properly computed
    	{
        if(this.currentWeightedPositionsGainOrLoss > 0.0)
    		{
          this.weightedPositionsToEvaluateOutOfSample[this.currentGenomeIndex].Reverse();
					try
					{
						AccountManager.OpenPositions(this.weightedPositionsToEvaluateOutOfSample[this.currentGenomeIndex],
																				 this.account);
					}
					catch(Exception ex)
					{ex=ex;}
					finally
					{
						this.weightedPositionsToEvaluateOutOfSample[this.currentGenomeIndex].Reverse();
					}
        }
        else //(currentChosenTickersGainOrLoss < 0.0)
    		{
					AccountManager.OpenPositions(this.weightedPositionsToEvaluateOutOfSample[this.currentGenomeIndex],
						this.account);
        }
    	}
    }
    
    public override void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      if(this.account.Portfolio.Count == 0 &&
         this.genomesCollector.Count > 0)
            this.openPositions((IndexBasedEndOfDayTimer)sender);
    }

    public override void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      AccountManager.ClosePositions(this.account);
    }

    #region OneHourAfterMarketCloseEventHandler
        
    protected void setTickers_updateTickersListAndAddGenomesForLog(GeneticOptimizer GO,
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
      	if(counter == 0 || 
      	   !this.genomesCollector.ContainsKey(currentGenome.Fitness) )
      	//it is the first genome to be added or no genome with the current
      	// fitness has been added to the hashtable yet
      	{
    				this.weightedPositionsToEvaluateOutOfSample[addedGenomes] = new WeightedPositions(
      				((GenomeMeaningPVO)currentGenome.Meaning).TickersPortfolioWeights, 
							new SignedTickers( ((GenomeMeaningPVO)currentGenome.Meaning).Tickers ) );
      		this.genomesCollector.Add(currentGenome.Fitness, null);
      		this.addPVOGenomeToBestGenomes(currentGenome,((GenomeManagerForEfficientPortfolio)this.iGenomeManager).FirstQuoteDate,
          ((GenomeManagerForEfficientPortfolio)this.iGenomeManager).LastQuoteDate, eligibleTickersForGO,
           2, this.portfolioType, GO.GenerationCounter,
           0.0, 0.0);
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
          new GenomeManagerBiasedOTC_PVONoThresholds(setOfTickersToBeOptimized,
          currentDate.AddDays(-this.numDaysForOptimizationPeriod), 
          currentDate, this.numberOfTickersToBeChosen,
          this.portfolioType);
      GeneticOptimizer GO = new GeneticOptimizer(this.iGenomeManager,
          this.populationSizeForGeneticOptimizer, 
          this.generationNumberForGeneticOptimizer,
          this.seedForRandomGenerator);
      if(setGenomeCounter)
        this.genomeCounter = new GenomeCounter(GO);
      GO.MutationRate = 0.1;
      GO.Run(false);
       
      this.setTickers_updateTickersListAndAddGenomesForLog(GO, setOfTickersToBeOptimized.Rows.Count);
    }
    
    #endregion

  }
}
