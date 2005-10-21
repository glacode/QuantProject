/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerSimpleSelectionOpenToClose.cs
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
using QuantProject.Data.DataTables;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TickerSelectionTesting.SimpleSelection
{
	
  /// <summary>
  /// Base class for EndOfDayTimerHandlers for simple selection
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerSimpleSelectionOpenToClose : EndOfDayTimerHandlerSimpleSelection
  {
    protected int numDaysBetweenEachOptimization;
    private int numDaysElapsedSinceLastOptimization;
   
    public EndOfDayTimerHandlerSimpleSelectionOpenToClose(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod, Account account,
                                string benchmark, double targetReturn,
                                PortfolioType portfolioType, int numDaysBetweenEachOptimization):
                              base(tickerGroupID, numberOfEligibleTickers, 
                              numberOfTickersToBeChosen, numDaysForOptimizationPeriod, account,
                              benchmark, targetReturn,
                              portfolioType)
    {
      this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
      this.numDaysElapsedSinceLastOptimization = 0;
    }
		
    
    public override void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      this.openPositions();
    }
    public override void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      this.closePositions();
    }

    #region OneHourAfterMarketCloseEventHandler
      
    protected DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
    {
      /*
      SelectorByAverageRawOpenPrice selectorByOpenPrice = 
                  new SelectorByAverageRawOpenPrice(this.tickerGroupID, false,
                          currentDate.AddDays(-this.numDaysForLiquidity), currentDate,
                          this.numberOfEligibleTickers, this.minPriceForMinimumCommission,
                          this.maxPriceForMinimumCommission, 0, 2);
      DataTable tickersByPrice = selectorByOpenPrice.GetTableOfSelectedTickers();
      */
     	
      SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID, currentDate);
      SelectorByOpenCloseCorrelationToBenchmark lessCorrelatedFromTemporizedGroup = 
        new SelectorByOpenCloseCorrelationToBenchmark(temporizedGroup.GetTableOfSelectedTickers(),
        this.benchmark,true,
        currentDate.AddDays(-this.numDaysForOptimizationPeriod ),
        currentDate,
        this.numberOfEligibleTickers);
      
      this.eligibleTickers = lessCorrelatedFromTemporizedGroup.GetTableOfSelectedTickers();
      SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromEligible = 
        new SelectorByQuotationAtEachMarketDay( this.eligibleTickers,
        false, currentDate.AddDays(-this.numDaysForOptimizationPeriod),
        currentDate, this.numberOfEligibleTickers, this.benchmark);
      //SelectorByWinningOpenToClose winners =
      //	new SelectorByWinningOpenToClose(quotedAtEachMarketDayFromMostLiquid.GetTableOfSelectedTickers(),
      //	                                 false, currentDate.AddDays(-2),
      //	                                 currentDate, this.numberOfEligibleTickers/4);      	                                 
      //return winners.GetTableOfSelectedTickers();
      //SelectorByOpenCloseCorrelationToBenchmark lessCorrelated = 
      //  new SelectorByOpenCloseCorrelationToBenchmark(quotedAtEachMarketDayFromEligible.GetTableOfSelectedTickers(),
      //                                                this.benchmark, true,
      //                                                currentDate.AddDays(-this.numDaysForLiquidity),
      //                                                currentDate, this.numberOfEligibleTickers/2);
      return quotedAtEachMarketDayFromEligible.GetTableOfSelectedTickers();
      //return lessCorrelated.GetTableOfSelectedTickers();
    }
    
    protected virtual void setTickers(DateTime currentDate)
    {
      
      DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
      if(setOfTickersToBeOptimized.Rows.Count >= this.chosenTickers.Length)
        //the optimization process is possible only if the initial set of tickers is 
        //as large as the number of tickers to be chosen                     
      
      {
        BestTickersScreenerOpenToClose OTCScreener = 
          new BestTickersScreenerOpenToClose(setOfTickersToBeOptimized,
                                             currentDate.AddDays(-this.numDaysForOptimizationPeriod),
                                             currentDate, this.targetReturn, 
                                             this.portfolioType);
        this.chosenTickers = OTCScreener.GetBestTickers(this.numberOfTickersToBeChosen);
      }
      //else it will be buyed again the previous optimized portfolio
      //that's it the actual chosenTickers member
    }

    protected void oneHourAfterMarketCloseEventHandler_updatePrices()
    {
      //min price for minimizing commission amount
      //according to IB Broker's commission scheme
      this.minPriceForMinimumCommission = this.account.CashAmount/(this.numberOfTickersToBeChosen*100);
      this.maxPriceForMinimumCommission = this.maxPriceForMinimumCommission;
      //just to avoid warning message
    }
    
    /// <summary>
    /// Handles a "One hour after market close" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public override void OneHourAfterMarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      this.orders.Clear();
      //this.oneHourAfterMarketCloseEventHandler_updatePrices();
      if(this.numDaysElapsedSinceLastOptimization == 
        this.numDaysBetweenEachOptimization - 1)
      {
        this.setTickers(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime);
        //sets tickers to be chosen next Market Open event
        this.numDaysElapsedSinceLastOptimization = 0;
      }
      else
      {
        this.numDaysElapsedSinceLastOptimization++;
      }
    	
    }
		   
    #endregion

  } // end of class
}
