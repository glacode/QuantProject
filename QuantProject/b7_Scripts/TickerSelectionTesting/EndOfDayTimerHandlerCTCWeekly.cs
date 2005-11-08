/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerCTCWeekly.cs
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
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.ADT.Optimizing.Genetic;

namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	
  /// <summary>
  /// Implements MarketOpenEventHandler and MarketCloseEventHandler
  /// These handlers contain the core strategy for the efficient close to close
  /// weekly portfolio (with a given days of life)!
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerCTCWeekly : EndOfDayTimerHandler
  {
    protected int numDaysForReturnCalculation;
    protected double maxAcceptableCloseToCloseDrawdown;
    protected bool stopLossConditionReached;
    protected double currentAccountValue;
    protected double previousAccountValue;
    
    public EndOfDayTimerHandlerCTCWeekly(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
                                Account account,                                
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark,
                                int numDaysForReturnCalculation,
                                double targetReturn,
                               	PortfolioType portfolioType, double maxAcceptableCloseToCloseDrawdown):
    														base(tickerGroupID, numberOfEligibleTickers, 
                                numberOfTickersToBeChosen, numDaysForOptimizationPeriod, account,
                                generationNumberForGeneticOptimizer,
                                populationSizeForGeneticOptimizer,
                                benchmark, targetReturn,
                                portfolioType)
    {
      this.numDaysForReturnCalculation = numDaysForReturnCalculation;
      this.maxAcceptableCloseToCloseDrawdown = maxAcceptableCloseToCloseDrawdown;
      this.stopLossConditionReached = false;
      this.currentAccountValue = 0.0;
      this.previousAccountValue = 0.0;
    }
	

    #region MarketCloseEventHandler
    
   
    
    protected void updateStopLossCondition()
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
    
    public override void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
    	this.updateStopLossCondition();
    	
    	if(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime.DayOfWeek == 
    	   DayOfWeek.Monday)
    		this.openPositions();
    	
    	if(this.stopLossConditionReached ||
    	   endOfDayTimingEventArgs.EndOfDayDateTime.DateTime.DayOfWeek == 
    	   DayOfWeek.Friday)
    		this.closePositions();
    	
    }
    
    #endregion
    
    #region OneHourAfterMarketCloseEventHandler
    
    protected DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
    {
      SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID,
    	                                                      currentDate);
                                      
      this.eligibleTickers = temporizedGroup.GetTableOfSelectedTickers();
      SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromEligible = 
        new SelectorByQuotationAtEachMarketDay(this.eligibleTickers,
                                  false, currentDate.AddDays(-this.numDaysForOptimizationPeriod),currentDate,
                                  this.numberOfEligibleTickers, this.benchmark);
      return quotedAtEachMarketDayFromEligible.GetTableOfSelectedTickers();
    }
    
    
    protected virtual void setTickers(DateTime currentDate)
    {
      DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
      if(setOfTickersToBeOptimized.Rows.Count > this.chosenTickers.Length*2)
        //the optimization process is meaningful only if the initial set of tickers is 
        //larger than the number of tickers to be chosen                     
      
      {
        
      	//double targetReturnForEachPeriodOfPortfolioLife =
      	//	Math.Pow(1.60,(double)(1.0/(360.0/this.numDaysOfPortfolioLife))) - 1.0;
      	//the target has to be such that annual system return is minimum 50%
      	//(with no commissions and bid-ask spreads)
      	IGenomeManager genManEfficientCTCPortfolio =
          new GenomeManagerForEfficientCTCPortfolio(setOfTickersToBeOptimized,
          currentDate.AddDays(-this.numDaysForOptimizationPeriod), 
          currentDate, this.numberOfTickersToBeChosen,
          this.numDaysForReturnCalculation,
          this.targetReturn,
         	this.portfolioType);
        GeneticOptimizer GO = new GeneticOptimizer(genManEfficientCTCPortfolio,
                                                    this.populationSizeForGeneticOptimizer, 
                                                    this.generationNumberForGeneticOptimizer);
        //GO.KeepOnRunningUntilConvergenceIsReached = true;
        GO.Run(false);
        this.chosenTickers = ((GenomeMeaning)GO.BestGenome.Meaning).Tickers;
        this.chosenTickersPortfolioWeights = ((GenomeMeaning)GO.BestGenome.Meaning).TickersPortfolioWeights;
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
     
      if(this.account.Portfolio.Count == 0 &&
    	   endOfDayTimingEventArgs.EndOfDayDateTime.DateTime.DayOfWeek == 
    	   DayOfWeek.Friday)
      {
        this.orders.Clear();
        this.setTickers(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime);
        //it sets tickers to be chosen at next Monday
      }
    }
		#endregion
  }
}
