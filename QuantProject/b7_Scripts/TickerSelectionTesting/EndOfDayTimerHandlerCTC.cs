/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerCTC.cs
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
  /// portfolio (with a given days of life)!
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerCTC
  {
    private DataTable eligibleTickers;
    private string[] chosenTickers;
    private string[] lastChosenTickers;
    
    private string tickerGroupID;
    private int numberOfEligibleTickers;
    private int numberOfTickersToBeChosen;
    private int numDaysOfPortfolioLife;
    private int daysCounter;
    private int generationNumberForGeneticOptimizer;
		
    private Account account;
    private ArrayList orders;

    public int NumberOfEligibleTickers
    {
      get { return this.numberOfEligibleTickers; }
    }
		
    public Account Account
    {
      get { return this.account; }
    }
		
    public EndOfDayTimerHandlerCTC(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysOfPortfolioLife, 
                                Account account,
                                int generationNumberForGeneticOptimizer)
    {
      this.tickerGroupID = tickerGroupID;
      this.numberOfEligibleTickers = numberOfEligibleTickers;
      this.numberOfTickersToBeChosen = numberOfTickersToBeChosen;
      this.numDaysOfPortfolioLife = numDaysOfPortfolioLife;
      this.daysCounter = 0;
      this.account = account;
      this.generationNumberForGeneticOptimizer = generationNumberForGeneticOptimizer;
      this.orders = new ArrayList();
      this.chosenTickers = new string[numberOfTickersToBeChosen];
      this.lastChosenTickers = new string[numberOfTickersToBeChosen];
    }
		    
    #region MarketOpenEventHandler
    private DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
    {
      SelectorByLiquidity mostLiquid = new SelectorByLiquidity(this.tickerGroupID,false,
                                      currentDate.AddDays(-90), currentDate, this.numberOfEligibleTickers);
      this.eligibleTickers = mostLiquid.GetTableOfSelectedTickers();
      SelectorByQuotationAtEachMarketDay quotedInEachMarketDayFromMostLiquid = 
        new SelectorByQuotationAtEachMarketDay(this.eligibleTickers,
                                  false, currentDate.AddDays(-90),currentDate,
                                  this.numberOfEligibleTickers, "^MIBTEL");
      return quotedInEachMarketDayFromMostLiquid.GetTableOfSelectedTickers();
    }
    
    
    private void setTickers(DateTime currentDate)
    {
      DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
      if(setOfTickersToBeOptimized.Rows.Count > this.chosenTickers.Length*2)
        //the optimization process is meaningful only if the initial set of tickers is 
        //larger than the number of tickers to be chosen                     
      
      {
        IGenomeManager genManEfficientCTCPortfolio = 
          new GenomeManagerForEfficientCTCPortfolio(setOfTickersToBeOptimized,
          currentDate.AddDays(-90), 
          currentDate, this.numberOfTickersToBeChosen,
          this.numDaysOfPortfolioLife, 0.01);
        GeneticOptimizer GO = new GeneticOptimizer(genManEfficientCTCPortfolio);
        //GO.KeepOnRunningUntilConvergenceIsReached = true;
        GO.GenerationNumber = this.generationNumberForGeneticOptimizer;
        GO.Run(false);
        this.chosenTickers = (string[])GO.BestGenome.Meaning;
      }
      //else it will be buyed again the previous optimized portfolio
      //that's it the actual chosenTickers member
    }

    /// <summary>
    /// Handles a "Market Open" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      if(this.daysCounter == 0 || this.daysCounter == this.numDaysOfPortfolioLife - 1)
      //at next close it will be time to open a new portfolio
      {
        this.setTickers(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime);
        //sets tickers to be chosen at next close
        this.orders.Clear();
      }
    }
		#endregion

    #region MarketCloseEventHandler
    private void marketCloseEventHandler_orderChosenTickers_addToOrderList_forTicker(string ticker)
    {
      double cashForSinglePosition = this.account.CashAmount / this.numberOfTickersToBeChosen;
      long quantity =
        Convert.ToInt64( Math.Floor( cashForSinglePosition / this.account.DataStreamer.GetCurrentBid( ticker ) ) );
      Order order = new Order( OrderType.MarketBuy, new Instrument( ticker ) , quantity );
      this.orders.Add(order);
    }
    private void marketCloseEventHandler_orderChosenTickers_addToOrderList()
    {
      int idx = 0;
      foreach ( string ticker in this.chosenTickers )
      {
        this.lastChosenTickers[idx] = ticker;
        if(ticker != null)
           marketCloseEventHandler_orderChosenTickers_addToOrderList_forTicker( ticker );
        idx++;
      }
    }
    private void marketCloseEventHandler_orderChosenTickers()
    {
      this.marketCloseEventHandler_orderChosenTickers_addToOrderList();
    }


    private void marketCloseEventHandler_openPositions()
    {
      if(this.orders.Count == 0 && this.account.Transactions.Count == 0)
        this.account.AddCash(15000);     
      
      this.marketCloseEventHandler_orderChosenTickers();
      
      foreach(object item in this.orders)
      {
        this.account.AddOrder((Order)item);
      }
    }

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
        
    public void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      this.daysCounter++;
      if(this.daysCounter == this.numDaysOfPortfolioLife)
      //it's time to change portfolio
      {
        this.marketCloseEventHandler_closePositions();
        this.marketCloseEventHandler_openPositions();
        this.daysCounter = 0;
      }
    }
    
    #endregion

		
  }
}
