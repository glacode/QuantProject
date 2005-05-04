/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerCTO.cs
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
  /// Implements MarketOpenEventHandler,
  /// TwoMinutesBeforeMarketCloseEventHandler and OneHourAfterMarketCloseEventHandler
  /// These handlers contain the core strategy for the efficient close to open portfolio!
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerCTO : EndOfDayTimerHandler
  {
    
    public EndOfDayTimerHandlerCTO(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysForLiquidity, Account account,
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark, double targetReturn,
                                PortfolioType portfolioType):
  															base(tickerGroupID, numberOfEligibleTickers, 
                                numberOfTickersToBeChosen, numDaysForLiquidity, account,
                                generationNumberForGeneticOptimizer,
                                populationSizeForGeneticOptimizer,
                                benchmark, targetReturn,
                                portfolioType)
    {
      
    }
		    
    #region MarketOpenEventHandler
        
    private void marketOpenEventHandler_orderChosenTickers_addToOrderList()
    {
      int idx = 0;
      foreach ( string ticker in this.chosenTickers )
      {
        if(ticker != null)
        {  
          this.addOrderForTicker( ticker );
          this.lastChosenTickers[idx] = 
          		GenomeManagerForEfficientPortfolio.GetCleanTickerCode(ticker);
        }
        idx++;
      }
    }
    
    private void marketOpenEventHandler_orderChosenTickers()
    {
      this.marketOpenEventHandler_orderChosenTickers_addToOrderList();
    }
    
    /// <summary>
    /// Handles a "Market Open" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public override void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      if(this.orders.Count == 0 && this.account.Transactions.Count == 0)
        this.account.AddCash(17000);     
      
      this.marketOpenEventHandler_orderChosenTickers();
      
      foreach(object item in this.orders)
      {
        this.account.AddOrder((Order)item);
      }
    }
		#endregion

    #region MarketCloseEventHandler
    
    private void marketCloseEventHandler_closePosition(
      string ticker )
    {
      this.account.ClosePosition( ticker );
    }
    private void marketCloseEventHandler_closePositions()
    {
      if(this.lastChosenTickers != null)
      {
        foreach( string ticker in this.lastChosenTickers)
        {
          for(int i = 0; i<this.account.Portfolio.Keys.Count; i++)
          {
            if(this.account.Portfolio[ticker]!=null)
              marketCloseEventHandler_closePosition( ticker );
          }
        }
      } 
    }
        
    public override void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      
      this.marketCloseEventHandler_closePositions();
    }
    
    #endregion

		#region OneHourAfterMarketCloseEventHandler
      
    private DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
    {
      /*
      SelectorByAverageRawOpenPrice selectorByOpenPrice = 
                  new SelectorByAverageRawOpenPrice(this.tickerGroupID, false,
                          currentDate.AddDays(-this.numDaysForLiquidity), currentDate,
                          this.numberOfEligibleTickers, this.minPriceForMinimumCommission,
                          this.maxPriceForMinimumCommission, 0, 2);
      DataTable tickersByPrice = selectorByOpenPrice.GetTableOfSelectedTickers();
      */
     	
     	SelectorByLiquidity mostLiquid = new SelectorByLiquidity(this.tickerGroupID, false,
                                      currentDate.AddDays(-this.numDaysForLiquidity), currentDate,
                                      this.numberOfEligibleTickers/2);
      //SelectorByOpenToCloseVolatility lessVolatile = 
      //	new SelectorByOpenToCloseVolatility(mostLiquid.GetTableOfSelectedTickers(),
      //	                                    true, currentDate.AddDays(-this.numDaysForLiquidity/3),
      //	                                    currentDate,
      //	                                    this.numberOfEligibleTickers/4);
      
      this.eligibleTickers = mostLiquid.GetTableOfSelectedTickers();
      SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromMostLiquid = 
        new SelectorByQuotationAtEachMarketDay( this.eligibleTickers,
                                   false, currentDate.AddDays(-this.numDaysForLiquidity),
                                    currentDate, this.numberOfEligibleTickers/2, this.benchmark);
      //SelectorByWinningOpenToClose winners =
      //	new SelectorByWinningOpenToClose(quotedAtEachMarketDayFromMostLiquid.GetTableOfSelectedTickers(),
      //	                                 false, currentDate.AddDays(-2),
      //	                                 currentDate, this.numberOfEligibleTickers/4);      	                                 
      //return winners.GetTableOfSelectedTickers();
      return quotedAtEachMarketDayFromMostLiquid.GetTableOfSelectedTickers();
    }
    
    
    private void setTickers(DateTime currentDate)
    {
      
      DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
      if(setOfTickersToBeOptimized.Rows.Count > this.chosenTickers.Length*2)
        //the optimization process is possible only if the initial set of tickers is 
        //as large as the number of tickers to be chosen                     
      
      {
        IGenomeManager genManEfficientCTOPortfolio = 
          new GenomeManagerForEfficientCTOPortfolio(setOfTickersToBeOptimized,
        	                                          currentDate.AddDays(-this.numDaysForLiquidity),
        	                                          currentDate,
        	                                          this.numberOfTickersToBeChosen,
        	                                          this.targetReturn,
        	                                         	this.portfolioType);
        GeneticOptimizer GO = new GeneticOptimizer(genManEfficientCTOPortfolio);
        //GO.KeepOnRunningUntilConvergenceIsReached = true;
        GO.GenerationNumber = this.generationNumberForGeneticOptimizer;
        GO.PopulationSize = this.populationSizeForGeneticOptimizer;
        GO.Run(false);
        this.chosenTickers = (string[])GO.BestGenome.Meaning;
        //this.lastChosenTickers = this.chosenTickers;
      }
      //else it will be buyed again the previous optimized portfolio
      //that's it the actual chosenTickers member
    }

    private void oneHourAfterMarketCloseEventHandler_updatePrices()
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
    	//this.oneHourAfterMarketCloseEventHandler_updatePrices();
      this.setTickers(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime);
      //sets tickers to be chosen next Market Open event
      this.orders.Clear();
    }
		   
    #endregion
		
  }
}
