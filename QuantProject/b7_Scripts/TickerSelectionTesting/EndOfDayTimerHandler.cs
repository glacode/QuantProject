/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandler.cs
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
  public class EndOfDayTimerHandler
  {
    private DataTable eligibleTickers;
    private string[] chosenTickers;
    
    private string tickerGroupID;
    private int numberOfEligibleTickers;
    private int numberOfTickersToBeChosen;
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
		
    public EndOfDayTimerHandler(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, Account account,
                                int generationNumberForGeneticOptimizer)
    {
      this.tickerGroupID = tickerGroupID;
      this.numberOfEligibleTickers = numberOfEligibleTickers;
      this.numberOfTickersToBeChosen = numberOfTickersToBeChosen;
      this.account = account;
      this.generationNumberForGeneticOptimizer = generationNumberForGeneticOptimizer;
      this.orders = new ArrayList();
      this.chosenTickers = new string[numberOfTickersToBeChosen];
    }
		    
    #region MarketOpenEventHandler
    
    private void marketOpenEventHandler_orderChosenTickers_addToOrderList_forTicker(
      string ticker )
    {
 
      double cashForSinglePosition = this.account.CashAmount / this.numberOfTickersToBeChosen;
      long quantity =
        Convert.ToInt64( Math.Floor( cashForSinglePosition / this.account.DataStreamer.GetCurrentBid( ticker ) ) );
      Order order = new Order( OrderType.MarketBuy , new Instrument( ticker ) , quantity );
      this.orders.Add(order);
    }
    
    private void marketOpenEventHandler_orderChosenTickers_addToOrderList()
    {
      foreach ( string ticker in this.chosenTickers )
      {
        if(ticker != null)
          marketOpenEventHandler_orderChosenTickers_addToOrderList_forTicker( ticker );
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
    public void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      if(this.orders.Count == 0 && this.account.Transactions.Count == 0)
        this.account.AddCash(16000);     
      
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
      if(this.chosenTickers != null)
      {
        foreach( string ticker in this.chosenTickers)
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
      
      this.marketCloseEventHandler_closePositions();
    }
    
    #endregion

		#region OneHourAfterMarketCloseEventHandler
      
    private void setTickers(DateTime currentDate)
    {
      TickerSelector mostLiquid = new TickerSelector(SelectionType.Liquidity,
        false, this.tickerGroupID , currentDate.AddDays(-30), currentDate, this.numberOfEligibleTickers);
      this.eligibleTickers = mostLiquid.GetTableOfSelectedTickers();
      TickerSelector quotedInEachMarketDayFromMostLiquid = 
          new TickerSelector( this.eligibleTickers,
                               SelectionType.QuotedInEachMarketDay, false, "",
                               currentDate.AddDays(-30),currentDate,
                               this.numberOfEligibleTickers);
      quotedInEachMarketDayFromMostLiquid.MarketIndex = "^MIBTEL";   
      DataTable setOfTickersToBeOptimized =
          quotedInEachMarketDayFromMostLiquid.GetTableOfSelectedTickers();
      // to check this: it doesn't work !                               
      IGenomeManager genManEfficientCTOPortfolio = 
        new GenomeManagerForEfficientCTOPortfolio(setOfTickersToBeOptimized, currentDate.AddDays(-30), 
        currentDate, 
        this.numberOfTickersToBeChosen, 0.005, 0.05);
      GeneticOptimizer GO = new GeneticOptimizer(genManEfficientCTOPortfolio);
      //GO.KeepOnRunningUntilConvergenceIsReached = true;
      GO.GenerationNumber = this.generationNumberForGeneticOptimizer;
      GO.Run(false);
      this.chosenTickers = (string[])GO.BestGenome.Meaning;
    }

    /// <summary>
    /// Handles a "One hour after market close" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public void OneHourAfterMarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      this.setTickers(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime);
      //sets tickers to be chosen next Market Open event
      this.orders.Clear();
    }
		   
    #endregion
		
  }
}
