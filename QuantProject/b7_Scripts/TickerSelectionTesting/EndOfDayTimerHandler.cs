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

    private int numberOfEligibleTickers;
    private int numberOfTickersToBeChosen;
		
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
		
    public EndOfDayTimerHandler(int numberOfEligibleTickers, 
      int numberOfTickersToBeChosen, 
      Account account )
    {
      this.numberOfEligibleTickers = numberOfEligibleTickers;
      this.numberOfTickersToBeChosen = numberOfTickersToBeChosen;
      this.account = account;
      this.orders = new ArrayList();
    }
		    
    #region MarketOpenEventHandler
    /// <summary>
    /// Handles a "Market Open" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      if(this.account.Transactions.Count == 0)
        this.account.AddCash(endOfDayTimingEventArgs.EndOfDayDateTime,
                              16000);
      foreach(object item in this.orders)
      {
        this.account.AddOrder((Order)item);
      }
    }
		#endregion

    #region FiveMinutesBeforeMarketCloseEventHandler
    
    private void fiveMinutesBeforeMarketCloseEventHandler_closePosition(
      string ticker )
    {
      this.account.ClosePosition( ticker );
    }
    private void fiveMinutesBeforeMarketCloseEventHandler_closePositions()
    {
      foreach ( string ticker in this.account.Portfolio.Keys )
        fiveMinutesBeforeMarketCloseEventHandler_closePosition( ticker );
    }
    private void fiveMinutesBeforeMarketCloseEventHandler_openPosition(
      string ticker )
    {
      double maxPositionValue = this.account.CashAmount / this.numberOfTickersToBeChosen;
      long sharesToBeBought = Convert.ToInt64(
        Math.Floor( maxPositionValue /
        this.account.DataStreamer.GetCurrentAsk( ticker ) ) );
      this.account.AddOrder( new Order( OrderType.MarketBuy ,
        new Instrument( ticker ) , sharesToBeBought ) );
    }
    private void fiveMinutesBeforeMarketCloseEventHandler_openPositions()
    {
      foreach ( string ticker in this.chosenTickers )
        this.fiveMinutesBeforeMarketCloseEventHandler_openPosition( ticker );
    }
    public void FiveMinutesBeforeMarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      this.fiveMinutesBeforeMarketCloseEventHandler_closePositions();
      //fiveMinutesBeforeMarketCloseEventHandler_openPositions();
    }
    
    #endregion

		#region OneHourAfterMarketCloseEventHandler
    private void oneHourAfterMarketCloseEventHandler_orderChosenTickers_closePositions(
      IEndOfDayTimer endOfDayTimer )
    {
      foreach ( Position position in this.account.Portfolio )
        foreach(string ticker in this.chosenTickers)
        {
          if (position.Instrument.Key == ticker )
          {
            this.account.ClosePosition( position );
          }
        }
        
    }
    private void oneHourAfterMarketCloseEventHandler_orderChosenTickers_openPositions_forTicker(
      string ticker )
    {
      double cashForSinglePosition = this.account.CashAmount / this.numberOfTickersToBeChosen;
      long quantity =
        Convert.ToInt64( Math.Floor( cashForSinglePosition / this.account.DataStreamer.GetCurrentBid( ticker ) ) );
      Order order = new Order( OrderType.MarketBuy , new Instrument( ticker ) , quantity );
      this.orders.Add(order);
    }
    private void oneHourAfterMarketCloseEventHandler_orderChosenTickers_openPositions()
    {
      foreach ( string ticker in this.chosenTickers )
        if ( !this.account.Contains( ticker ) )
        {
          oneHourAfterMarketCloseEventHandler_orderChosenTickers_openPositions_forTicker( ticker );
        }
    }
    private void oneHourAfterMarketCloseEventHandler_orderChosenTickers(
      IEndOfDayTimer endOfDayTimer )
    {
      //this.oneHourAfterMarketCloseEventHandler_orderChosenTickers_closePositions( endOfDayTimer );
      this.oneHourAfterMarketCloseEventHandler_orderChosenTickers_openPositions();
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
      this.orders.Clear();
      oneHourAfterMarketCloseEventHandler_orderChosenTickers( ( IEndOfDayTimer ) sender );
    }
		
    
    private void setTickers(DateTime currentDate)
    {
      TickerSelector mostLiquid = new TickerSelector(SelectionType.Liquidity,
        false, "STOCKMI", currentDate, currentDate.AddDays(60), this.numberOfEligibleTickers);
      this.eligibleTickers = mostLiquid.GetTableOfSelectedTickers();
      IGenomeManager genManEfficientCTOPortfolio = 
        new GenomeManagerForEfficientCTOPortfolio(this.eligibleTickers,currentDate, 
        currentDate.AddDays(60), 
        this.numberOfTickersToBeChosen, 0.005, 0.05);
      GeneticOptimizer GO = new GeneticOptimizer(genManEfficientCTOPortfolio);
      //GO.KeepOnRunningUntilConvergenceIsReached = true;
      GO.GenerationNumber = 4;
      GO.MutationRate = 0.05;
      GO.Run(false);
      this.chosenTickers = (string[])GO.BestGenome.Meaning;
    }
    #endregion
		
  }
}
