/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerBiasedPVONoThresholds.cs
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
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.DataProviders;
using QuantProject.Data;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.Data.DataTables;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.WalkForwardTesting.LinearCombination;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.BiasedPVO.BiasedOTC_PVONoThresholds; 


namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.BiasedPVO.BiasedPVONoThresholds
{
	/// <summary>
  /// Implements MarketOpenEventHandler and MarketCloseEventHandler
  /// These handlers contain the core strategy for the Biased Portfolio Value
  /// Oscillator, with no thresholds
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerBiasedPVONoThresholds : EndOfDayTimerHandlerBiasedOTC_PVONoThresholds
  {
    protected double minimumAcceptableGain;

    public EndOfDayTimerHandlerBiasedPVONoThresholds(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
                                Account account,                                
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark,
                                int numOfDifferentGenomesToEvaluateOutOfSample,
                                int numDaysBetweenEachOptimization,
                                PortfolioType portfolioType, double maxAcceptableCloseToCloseDrawdown,
                                double minimumAcceptableGain):
    														base(tickerGroupID, numberOfEligibleTickers, 
                                numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
                                account,                                
                                generationNumberForGeneticOptimizer,
                                populationSizeForGeneticOptimizer,
                                benchmark,numOfDifferentGenomesToEvaluateOutOfSample,
                                numDaysBetweenEachOptimization,portfolioType)
                                
    {
      this.minimumAcceptableGain = minimumAcceptableGain;
			this.maxAcceptableCloseToCloseDrawdown = maxAcceptableCloseToCloseDrawdown;
    }
    
	  #region MarketCloseEventHandler
    
		protected override double getCurrentWeightedPositionsGainOrLoss(
	  	IndexBasedEndOfDayTimer timer,
	  	ReturnsManager returnsManager,
	  	int indexForChosenWeightedPositions)
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
					new EndOfDayDateTime(finalDateForHalfPeriod,
					EndOfDaySpecificTime.MarketClose) ,
					this.benchmark , this.numDaysForReturnCalculation ) ,
					new HistoricalAdjustedQuoteProvider() );
				returnValue = this.chosenWeightedPositions.GetReturn(0,returnsManager);
			}
			catch(MissingQuotesException ex)
			{
				ex = ex;
			}
			return returnValue;
		}   
	          
    protected virtual void marketCloseEventHandler_closeIfItIsTimeToClose_updateStopLossAndTakeProfitConditions()
    {
      this.previousAccountValue = this.currentAccountValue;
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
      else
      {
      	this.stopLossConditionReached = false;
        this.takeProfitConditionReached = false;
      }
    }   

    protected virtual void marketCloseEventHandler_closeIfItIsTimeToClose()
    {
      this.marketCloseEventHandler_closeIfItIsTimeToClose_updateStopLossAndTakeProfitConditions();
      if(this.stopLossConditionReached ||
          this.takeProfitConditionReached ||
          this.numDaysElapsedSinceLastOptimization + 1 == this.numDaysBetweenEachOptimization )
          //stop loss or take profit conditions have been reached
          //or after the close it is necessary to run
          //another optimization
      {
        AccountManager.ClosePositions(this.account);
      }
    }
    
    public override void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
     	if(this.account.Portfolio.Count > 0)
    	 	//this.marketCloseEventHandler_reverseOrClose((IndexBasedEndOfDayTimer)sender);
     	  this.marketCloseEventHandler_closeIfItIsTimeToClose();
      else if ( this.account.Portfolio.Count == 0 &&
    	         this.weightedPositionsToEvaluateOutOfSample[0] != null )
			//portfolio is empty and optimization has been already launched    		
    		this.marketCloseEventHandler_openPositions((IndexBasedEndOfDayTimer)sender);
    }

    #endregion
    

    #region OneHourAfterMarketCloseEventHandler
        
        
    protected override void setTickers(DateTime currentDate,
      bool setGenomeCounter)
    {
      DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
      this.iGenomeManager =
          new GenomeManagerBiasedPVONoThresholds(setOfTickersToBeOptimized,
          currentDate.AddDays(-this.numDaysForOptimizationPeriod), 
          currentDate, this.numberOfTickersToBeChosen,
          this.portfolioType,this.benchmark);
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
