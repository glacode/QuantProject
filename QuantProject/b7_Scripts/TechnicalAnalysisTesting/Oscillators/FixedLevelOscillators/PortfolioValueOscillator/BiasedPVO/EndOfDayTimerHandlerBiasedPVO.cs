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
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.BiasedPVO.BiasedPVONoThresholds;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.BiasedPVO
{
	/// <summary>
  /// Implements MarketOpenEventHandler and MarketCloseEventHandler
  /// These handlers contain the core strategy for the Biased Portfolio Value
  /// Oscillator
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerBiasedPVO : EndOfDayTimerHandlerBiasedPVONoThresholds
  {
    new protected double[] currentOverboughtThreshold;
		new protected double[] currentOversoldThreshold;

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
																account, generationNumberForGeneticOptimizer,
																populationSizeForGeneticOptimizer, benchmark,
																numOfDifferentGenomesToEvaluateOutOfSample,
																numDaysBetweenEachOptimization,
																portfolioType, maxAcceptableCloseToCloseDrawdown,
																minimumAcceptableGain)
    {
			this.currentOverboughtThreshold = new double[numOfDifferentGenomesToEvaluateOutOfSample];
			this.currentOversoldThreshold = new double[numOfDifferentGenomesToEvaluateOutOfSample];
			this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
      this.minLevelForOversoldThreshold = minLevelForOversoldThreshold;
      this.maxLevelForOversoldThreshold = maxLevelForOversoldThreshold;
      this.minLevelForOverboughtThreshold = minLevelForOverboughtThreshold;
      this.maxLevelForOverboughtThreshold = maxLevelForOverboughtThreshold;
			this.divisorForThresholdComputation = divisorForThresholdComputation;
			this.symmetricalThresholds =symmetricalThresholds;
      this.overboughtMoreThanOversoldForFixedPortfolio = overboughtMoreThanOversoldForFixedPortfolio;
    }
	

    #region MarketCloseEventHandler
    
		
    //sets currentGenomeIndex with the genome's index that crosses an overbought/oversold threshold with the
    //highest degree and sets currentTickersGainOrLoss accordingly
    protected override void openPositions_chooseBestGenome(IndexBasedEndOfDayTimer timer)
    {
      //default index is the first
      //genome (with the highest plain fitness), if no other genome
      //has crossed the threshold with a higher degree than the first genome
      double currentMaxDegreeOfCrossingThreshold = 0.0;
      double currentDegreeOfCrossingThreshold = 0.0;
      for(int i = 0; i < this.numOfDifferentGenomesToEvaluateOutOfSample; i++)
      {
        double currentChosenWeightedPositionsGainOrLoss = 
                 this.getCurrentWeightedPositionsGainOrLoss(timer, i);
        if(currentChosenWeightedPositionsGainOrLoss != 999.0)
          //currentChosenTickersValue has been properly computed
        {
          //computing degree of crossing threshold 
          if(currentChosenWeightedPositionsGainOrLoss >= 1.0 + this.currentOverboughtThreshold[i])
          {
            currentDegreeOfCrossingThreshold = 
              (currentChosenWeightedPositionsGainOrLoss - 1.0 - this.currentOverboughtThreshold[i])/
            		(1 + this.currentOverboughtThreshold[i]);
 
          }
          else if (currentChosenWeightedPositionsGainOrLoss <= 1.0 - this.currentOversoldThreshold[i])
          {
            currentDegreeOfCrossingThreshold = 
              (1.0 - this.currentOversoldThreshold[i] - currentChosenWeightedPositionsGainOrLoss)/
            		(1.0 - this.currentOversoldThreshold[i]);
          }
          if(currentDegreeOfCrossingThreshold > currentMaxDegreeOfCrossingThreshold)
          {
          	currentMaxDegreeOfCrossingThreshold = currentDegreeOfCrossingThreshold;
          	this.currentGenomeIndex = i;
          	this.currentWeightedPositionsGainOrLoss = currentChosenWeightedPositionsGainOrLoss;
          }
        }
      }
    }

    protected override void openPositions(IndexBasedEndOfDayTimer timer)
    {
      this.currentWeightedPositionsGainOrLoss = 999.0; //
    	this.openPositions_chooseBestGenome(timer);
      if(this.currentWeightedPositionsGainOrLoss != 999.0)
    	//currentWeightedPositionsGainOrLoss has been properly computed
    	{
        if(this.currentWeightedPositionsGainOrLoss >= 1.0 + currentOverboughtThreshold[this.currentGenomeIndex] &&
           this.portfolioType == PortfolioType.ShortAndLong)
    		{
					this.weightedPositionsToEvaluateOutOfSample[this.currentGenomeIndex].Reverse();
					try{
						AccountManager.OpenPositions(this.weightedPositionsToEvaluateOutOfSample[this.currentGenomeIndex],
							this.account);
						this.portfolioHasBeenOverbought = true;
						this.portfolioHasBeenOversold = false;
					}
					catch(Exception ex){
						ex=ex;
					}
					finally{
						this.weightedPositionsToEvaluateOutOfSample[this.currentGenomeIndex].Reverse();
					}
        }
        else if (this.currentWeightedPositionsGainOrLoss <= 1.0 - currentOversoldThreshold[this.currentGenomeIndex])
    		{
					AccountManager.OpenPositions(this.weightedPositionsToEvaluateOutOfSample[this.currentGenomeIndex],
						this.account);
          this.portfolioHasBeenOverbought = false;
          this.portfolioHasBeenOversold = true;
        }
        this.currentAccountValue = this.account.GetMarketValue();
    	}
    }
   
    public override void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
     	if(this.account.Portfolio.Count > 0)
     	  this.marketCloseEventHandler_closeIfItIsTimeToClose();
      else if ( this.account.Portfolio.Count == 0 &&
    	          this.weightedPositionsToEvaluateOutOfSample != null )
			//portfolio is empty and optimization has been already launched    		
    		this.openPositions((IndexBasedEndOfDayTimer)sender);
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
					this.weightedPositionsToEvaluateOutOfSample[addedGenomes] = new WeightedPositions(
						((GenomeMeaningPVO)currentGenome.Meaning).TickersPortfolioWeights, 
						new SignedTickers( ((GenomeMeaningPVO)currentGenome.Meaning).Tickers ) );
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
