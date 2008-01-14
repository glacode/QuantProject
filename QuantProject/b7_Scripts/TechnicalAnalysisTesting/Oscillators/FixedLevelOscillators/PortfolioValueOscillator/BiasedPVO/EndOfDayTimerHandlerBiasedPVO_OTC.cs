/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerBiasedPVO_OTC.cs
Copyright (C) 2008
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
using QuantProject.ADT.Statistics;
using QuantProject.ADT.FileManaging;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.TickersRelationships;
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
  /// Oscillator, using Open To Close returns
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerBiasedPVO_OTC : EndOfDayTimerHandlerBiasedPVO
  {
    
    public EndOfDayTimerHandlerBiasedPVO_OTC(string tickerGroupID, int numberOfEligibleTickers,
                                         double minPriceForTickersToBeChosen,
                                         double maxPriceForTickersToBeChosen,
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
                                Account account,
                                string pathOfFileContainingGenomes,
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark,
                                int numOfDifferentGenomesToEvaluateOutOfSample,
                               	bool resetThresholdsBeforeCheckingOutOfSample,
                               	int numDaysForThresholdsReComputation,
                                double numOfStdDevForThresholdsComputation,
                                double maxCoefficientForDegreeComputationOfCrossingThreshold,
                                bool buyOnlyPositionsThatAreMovingTogether,
                                bool doNotOpenReversedPositionsThatHaveJustBeenClosed,
                                int numDaysOfStayingOnTheMarket,
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
                                minPriceForTickersToBeChosen,
                                maxPriceForTickersToBeChosen,
                                numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
                                account,
                                pathOfFileContainingGenomes,
                                generationNumberForGeneticOptimizer,
                                populationSizeForGeneticOptimizer,
                                benchmark,
                                numOfDifferentGenomesToEvaluateOutOfSample,
                               	resetThresholdsBeforeCheckingOutOfSample,
                               	numDaysForThresholdsReComputation,
                                numOfStdDevForThresholdsComputation,
                                maxCoefficientForDegreeComputationOfCrossingThreshold,
                                buyOnlyPositionsThatAreMovingTogether,
                                doNotOpenReversedPositionsThatHaveJustBeenClosed,
                                1, numDaysOfStayingOnTheMarket,
                                minLevelForOversoldThreshold,
                                maxLevelForOversoldThreshold,
                                minLevelForOverboughtThreshold,
                                maxLevelForOverboughtThreshold,
                                divisorForThresholdComputation,
                                symmetricalThresholds,
                                overboughtMoreThanOversoldForFixedPortfolio,
                                numDaysBetweenEachOptimization,
                                portfolioType, maxAcceptableCloseToCloseDrawdown,
                                minimumAcceptableGain)
      
    {
			
    }
		
  	#region MarketOpen
  			
		public override void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      if ( this.account.Portfolio.Count == 0 &&
           this.weightedPositionsToEvaluateOutOfSample[0] != null )
			//portfolio is empty and optimization has been already launched
     	   		this.openPositions( (IndexBasedEndOfDayTimer)sender );
    }
    
    protected override void openPositions_chooseGenome_resetThresholds(DateTime today)
    {
			for(int i = 0; i < this.weightedPositionsToEvaluateOutOfSample.Length; i++)
    	{
				this.currentOversoldThreshold[i] = (double)this.maxLevelForOversoldThreshold/
					(double)this.divisorForThresholdComputation;
    		this.currentOverboughtThreshold[i] = (double)this.maxLevelForOverboughtThreshold/
					(double)this.divisorForThresholdComputation;
     	}			
//  UPDATE WITH DailyOpenToCloseIntervals
//    	ReturnsManager returnsManager = new ReturnsManager(
//    		new CloseToCloseIntervals(new EndOfDayDateTime(today.AddDays(-this.numDaysForThresholdsReComputation), EndOfDaySpecificTime.MarketClose),
//		  	                         new EndOfDayDateTime(today, EndOfDaySpecificTime.MarketClose),
//		  	                         this.benchmark,
//		  	                         this.numDaysForOscillatingPeriod),
//		  	new HistoricalAdjustedQuoteProvider() );
//    	//double returnsAverage;
//    	double returnsStdDev;
//    	for(int i = 0; i < this.weightedPositionsToEvaluateOutOfSample.Length; i++)
//    	{
////				returnsAverage = BasicFunctions.GetSimpleAverage(
////					this.weightedPositionsToEvaluateOutOfSample[i].GetReturns(returnsManager) );
//				returnsStdDev = BasicFunctions.GetStdDev(
//    			this.weightedPositionsToEvaluateOutOfSample[i].GetReturns(returnsManager) );
//     		this.currentOversoldThreshold[i] = -this.numOfStdDevForThresholdsComputation * returnsStdDev;
//    		this.currentOverboughtThreshold[i] = this.numOfStdDevForThresholdsComputation * returnsStdDev;
//     	}
    }
    //NEW IMPLEMENTATION of chooseBestGenome, now named chooseGenome
    //sets currentGenomeIndex with the first genome's index that crosses an overbought/oversold threshold with the
    //"acceptable" degree and sets currentWeightedPositionsGainOrLoss accordingly
    protected override void openPositions_chooseGenome(IndexBasedEndOfDayTimer timer)
    {
      DateTime today = timer.GetCurrentTime().DateTime;
		  DateTime previousTradingDay = 
		  	timer.GetPreviousDateTime(1);
		  ReturnsManager returnsManager = new ReturnsManager(
		  	new CloseToOpenIntervals(new EndOfDayDateTime(previousTradingDay, EndOfDaySpecificTime.MarketClose),
		  	                         new EndOfDayDateTime(today, EndOfDaySpecificTime.MarketOpen),
		  	                         this.benchmark),
		  	new HistoricalAdjustedQuoteProvider() );
		  if(this.resetThresholdsBeforeCheckingOutOfSample)
		  	this.openPositions_chooseGenome_resetThresholds(today);
		  this.currentWeightedPositionsGainOrLoss = double.MinValue;
		  double currentWeightedPositionsGainOrLoss_temp = double.MinValue;
      for(int i = 0; i < this.numOfDifferentGenomesToEvaluateOutOfSample; i++)
      {
        currentWeightedPositionsGainOrLoss_temp = 
                 this.getCurrentWeightedPositionsGainOrLoss(
        				 timer, returnsManager, i);
				if( currentWeightedPositionsGainOrLoss_temp != double.MinValue &&
					  ( this.buyOnlyPositionsThatAreMovingTogether == false ||
        	      ( this.buyOnlyPositionsThatAreMovingTogether == true && 
									this.areAllTickersMovingTogetherUpOrDown(timer, returnsManager, i) == true	 ) )		)
        //currentWeightedPositionsGainOrLoss_temp has been properly computed and
				//only positions that are moving together can be bought
        {
          if( (currentWeightedPositionsGainOrLoss_temp >= this.currentOverboughtThreshold[i] &&
               currentWeightedPositionsGainOrLoss_temp <= this.maxCoefficientForDegreeComputationOfCrossingThreshold *
            																						 this.currentOverboughtThreshold[i] ) ||
						  (currentWeightedPositionsGainOrLoss_temp <= - this.currentOversoldThreshold[i] && 
        	          Math.Abs(currentWeightedPositionsGainOrLoss_temp) <= this.maxCoefficientForDegreeComputationOfCrossingThreshold *
            																						 								 this.currentOversoldThreshold[i] ) )
          // if the current genome matches the requested criteria
          {
						this.currentGenomeIndex = i;
						this.currentWeightedPositionsGainOrLoss = currentWeightedPositionsGainOrLoss_temp;
						i = this.numOfDifferentGenomesToEvaluateOutOfSample; //exit from for
          }
        }
      }
    }
        
		#endregion
		
    public override void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
     	if(this.account.Portfolio.Count > 0)
     	{
     		this.lastWeightedPositionsClosed = AccountManager.GetWeightedPositions(this.account);
				AccountManager.ClosePositions(this.account);
     	}
    }
   
    #region OneHourAfterMarketCloseEventHandler
        
    protected override void setTickers(DateTime currentDate,
      bool setGenomeCounter)
    {
      DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
      this.iGenomeManager =
          new GenomeManagerPVO_OTC(setOfTickersToBeOptimized,
          currentDate.AddDays(-this.numDaysForOptimizationPeriod), 
          currentDate, this.numberOfTickersToBeChosen,
          this.minLevelForOversoldThreshold,
          this.maxLevelForOversoldThreshold,
          this.minLevelForOverboughtThreshold,
          this.maxLevelForOverboughtThreshold,
          this.divisorForThresholdComputation,
          this.symmetricalThresholds,
          this.overboughtMoreThanOversoldForFixedPortfolio,
          this.portfolioType, this.benchmark);
      GeneticOptimizer GO = new GeneticOptimizer(this.iGenomeManager,
          this.populationSizeForGeneticOptimizer, 
          this.generationNumberForGeneticOptimizer,
          this.seedForRandomGenerator);
//      if(setGenomeCounter)
//        this.genomeCounter = new GenomeCounter(GO);
      GO.MutationRate = 0.1;
      GO.CrossoverRate = 0.85;
      GO.Run(false);
       
      this.setTickers_updateTickersWeightsAndThresholdsAndAddGenomesForLog(GO, setOfTickersToBeOptimized.Rows.Count);
           	
    }
    
             
    #endregion

  }
}
