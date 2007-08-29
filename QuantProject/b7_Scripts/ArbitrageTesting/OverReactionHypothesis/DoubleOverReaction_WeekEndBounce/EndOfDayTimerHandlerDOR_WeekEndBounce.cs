/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerDOR_WeekEndBounce.cs
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

namespace QuantProject.Scripts.ArbitrageTesting.OverReactionHypothesis.DoubleOverReaction_WeekEndBounce
{
	/// <summary>
  /// Implements MarketOpenEventHandler and MarketCloseEventHandler
  /// These handlers contain the core strategy for
  /// the Double OverReaction WeekEnd Bounce Test,
  /// based on the OverReaction Hypothesis
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerDOR_WeekEndBounce
  {
    private string tickerGroupID;
    private int numberOfEligibleTickers;
    private string benchmark;
    private Account account;
    private int numOfWorstTickers;
    private int numOfBestTickers;
		private int lengthInDaysForPerformance;
    private int numOfTickersForBuying;
    private int numOfTickersForShortSelling;
    private string[] bestTickers;
    private string[] worstTickers;
    private string[] chosenTickers;
    private string[] lastOrderedTickers;
    private ArrayList orders;
		private bool thereAreEnoughBestTickers;
		private bool thereAreEnoughWorstTickers;
    
    public EndOfDayTimerHandlerDOR_WeekEndBounce(string tickerGroupID, int numberOfEligibleTickers, 
                                            int numOfBestTickers, 
																			      int numOfWorstTickers, 
																						int lengthInDaysForPerformance,
																						int numOfTickersForBuying,
																			      int numOfTickersForShortSelling,
																			      Account account, string benchmark)
    {
      this.tickerGroupID = tickerGroupID;
      this.numberOfEligibleTickers = numberOfEligibleTickers;
      this.account = account;
      this.benchmark = benchmark;
      this.numOfBestTickers = numOfBestTickers;
      this.bestTickers = new string[this.numOfBestTickers];
      this.numOfWorstTickers = numOfWorstTickers;
      this.worstTickers = new string[this.numOfWorstTickers];
			this.lengthInDaysForPerformance = lengthInDaysForPerformance;
      this.numOfTickersForBuying = numOfTickersForBuying;
      this.numOfTickersForShortSelling = numOfTickersForShortSelling;
      this.chosenTickers = new string[this.numOfTickersForBuying + this.numOfTickersForShortSelling];
      this.lastOrderedTickers = new string[this.chosenTickers.Length];
      this.orders = new ArrayList();
    }
    
    #region MarketOpenEventHandler

    private void closePosition( string ticker )
    {
      this.account.ClosePosition( ticker );
    }
    
    private void closePositions()
    {
      if(this.lastOrderedTickers != null)
        foreach( string ticker in this.lastOrderedTickers )
                for( int i = 0; i<this.account.Portfolio.Keys.Count; i++ )
                    if( this.account.Portfolio[ticker]!=null )
                       closePosition( ticker );
    }
    
    
    private void addOrderForTicker(string[] tickers, 
                                   int tickerPosition )
    {
      SignedTickers signedTickers = new SignedTickers(tickers);
			string ticker = 
        SignedTicker.GetTicker(tickers[tickerPosition]);
      double cashForSinglePosition = 
        this.account.CashAmount / this.chosenTickers.Length;
      long quantity =
        Convert.ToInt64( Math.Floor( cashForSinglePosition / this.account.DataStreamer.GetCurrentBid( ticker ) ) );
      Order order = 
        new Order( 
        signedTickers[tickerPosition].MarketOrderType,
        new Instrument( ticker ) , quantity );
      this.orders.Add(order);
    }	

    private void addChosenTickersToOrderList(string[] tickers)
    {
      for( int i = 0; i<tickers.Length; i++)
      {
        if(tickers[i] != null)
        {  
          this.addOrderForTicker( tickers, i );
          this.lastOrderedTickers[i] = 
            SignedTicker.GetTicker(tickers[i]);
        }
      }
    }

    private void openPositions(string[] tickers)
    {
      this.addChosenTickersToOrderList(tickers);
      //execute orders actually
      foreach(object item in this.orders)
        this.account.AddOrder((Order)item);
    }
    
    private double setChosenTickers_getGainOrLossFromPreviousClose(string signedTicker)
    {
      IndexBasedEndOfDayTimer currentTimer = (IndexBasedEndOfDayTimer)this.account.EndOfDayTimer;
      ExtendedDateTime nowAtOpen = 
        new ExtendedDateTime(currentTimer.GetCurrentTime().DateTime,
        BarComponent.Open);
      ExtendedDateTime previousClose =
        new ExtendedDateTime(currentTimer.GetPreviousDateTime(),
        BarComponent.Close);
      double currentValueAtOpen =
        HistoricalDataProvider.GetAdjustedMarketValue(SignedTicker.GetTicker(signedTicker), nowAtOpen);
      double previousValueAtClose = 
        HistoricalDataProvider.GetAdjustedMarketValue(SignedTicker.GetTicker(signedTicker), previousClose);
      
      return (currentValueAtOpen - previousValueAtClose) / previousValueAtClose; 
    }

    private void setChosenTickers_addTickersForShorting()
    {
      DataTable worstTickersOrderedByGainAtOpen = new DataTable();
      worstTickersOrderedByGainAtOpen.Columns.Add("ticker", Type.GetType("System.String"));
      worstTickersOrderedByGainAtOpen.Columns.Add("gainAtOpen", Type.GetType("System.Double"));
      object[] values = new object[2];
      for (int i = 0; i<this.worstTickers.Length; i++)
      {
        values[0] = this.worstTickers[i];
        values[1] = this.setChosenTickers_getGainOrLossFromPreviousClose(this.worstTickers[i]);
        worstTickersOrderedByGainAtOpen.Rows.Add(values);
      }
      DataRow[] orderedRows = new DataRow[this.bestTickers.Length];
      orderedRows = worstTickersOrderedByGainAtOpen.Select("", "gainAtOpen DESC");
      for(int i = 0;i<this.numOfTickersForShortSelling; i++)
          if( (double)orderedRows[i]["gainAtOpen"] > 0.0 )
          //at open, current ticker is gaining
             this.chosenTickers[this.numOfTickersForBuying + i] = "-" +
                                              (string)orderedRows[i]["ticker"];
    }
      
    private void setChosenTickers_addTickersForBuying()
    {
      DataTable bestTickersOrderedByLossAtOpen = new DataTable();
      bestTickersOrderedByLossAtOpen.Columns.Add("ticker", Type.GetType("System.String"));
      bestTickersOrderedByLossAtOpen.Columns.Add("lossAtOpen", Type.GetType("System.Double"));
      object[] values = new object[2];
      for (int i = 0; i<this.bestTickers.Length; i++)
      {
        values[0] = this.bestTickers[i];
        values[1] = - this.setChosenTickers_getGainOrLossFromPreviousClose(this.bestTickers[i]);
        bestTickersOrderedByLossAtOpen.Rows.Add(values);
      }
      DataRow[] orderedRows = new DataRow[this.bestTickers.Length];
      orderedRows = bestTickersOrderedByLossAtOpen.Select("", "lossAtOpen DESC");
      for( int i = 0; i<this.numOfTickersForBuying; i++)
           if( (double)orderedRows[i]["lossAtOpen"] > 0.0 )
           //at open, current ticker is losing
              this.chosenTickers[i] = (string)orderedRows[i]["ticker"];
    }

    private void setChosenTickersBothForLongAndShort()
    {
			if( this.thereAreEnoughBestTickers && 
					this.thereAreEnoughWorstTickers )
      {
				try
				{
					this.setChosenTickers_addTickersForBuying();
					this.setChosenTickers_addTickersForShorting();
				}
				catch(Exception ex)
				{ex = ex;}
      }
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
        this.account.AddCash(30000);
      
      if (  (this.account.Portfolio.Count == 0 &&
             endOfDayTimingEventArgs.EndOfDayDateTime.DateTime.DayOfWeek ==
             DayOfWeek.Monday) ||
            (this.account.Portfolio.Count == 0 &&
             endOfDayTimingEventArgs.EndOfDayDateTime.DateTime.DayOfWeek == 
             DayOfWeek.Tuesday)  )
      {
      	this.setChosenTickersBothForLongAndShort();
        bool allTickersHasBeenChosenForLongAndShort = true;
	      for( int i = 0; i<this.chosenTickers.Length; i++)
	      {
	        if(this.chosenTickers[i] == null)
	          allTickersHasBeenChosenForLongAndShort = false;
	      }
	      if(allTickersHasBeenChosenForLongAndShort)
	        	this.openPositions(this.chosenTickers);
      }
      else if (endOfDayTimingEventArgs.EndOfDayDateTime.DateTime.DayOfWeek ==
               DayOfWeek.Wednesday ||
               endOfDayTimingEventArgs.EndOfDayDateTime.DateTime.DayOfWeek ==
               DayOfWeek.Thursday ||
               endOfDayTimingEventArgs.EndOfDayDateTime.DateTime.DayOfWeek ==
               DayOfWeek.Friday)
	     	 	this.closePositions();
      	
    }
		#endregion
        
    public void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
     	
    }

    #region OneHourAfterMarketCloseEventHandler
    
		private void oneHourAfterMarketCloseEventHandler_clear()
		{
			this.orders.Clear();
			this.thereAreEnoughBestTickers = false;
			this.thereAreEnoughWorstTickers = false;
			
			for(int i = 0; i<this.chosenTickers.Length;i++)
				this.chosenTickers[i] = null;
		}
 

    /// <summary>
    /// Handles a "One hour after market close" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public void OneHourAfterMarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      this.oneHourAfterMarketCloseEventHandler_clear();
    	
      if (	endOfDayTimingEventArgs.EndOfDayDateTime.DateTime.DayOfWeek ==
						DayOfWeek.Friday &&
						(	(IndexBasedEndOfDayTimer)sender ).CurrentDateArrayPosition >= this.lengthInDaysForPerformance 	 )
    	{
	   		DateTime currentDate = endOfDayTimingEventArgs.EndOfDayDateTime.DateTime;
				int currentDateArrayPositionInTimer = ((IndexBasedEndOfDayTimer)sender).CurrentDateArrayPosition;
				SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID,
	                                            currentDate);
	      DataTable tickersFromGroup = temporizedGroup.GetTableOfSelectedTickers();
	      	      
				SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromGroup = 
					new SelectorByQuotationAtEachMarketDay(tickersFromGroup,
								false, currentDate.AddDays(-30), currentDate,
								tickersFromGroup.Rows.Count, this.benchmark);
	
				SelectorByAverageRawOpenPrice byPrice = 
	        new SelectorByAverageRawOpenPrice( quotedAtEachMarketDayFromGroup.GetTableOfSelectedTickers(),
								false, currentDate.AddDays(-10), currentDate,
								tickersFromGroup.Rows.Count, 20 );
	      
	      SelectorByLiquidity mostLiquidSelector =
	        new SelectorByLiquidity(byPrice.GetTableOfSelectedTickers(),
	        false,currentDate.AddDays(-30), currentDate,
	        this.numberOfEligibleTickers);
	      
	      SelectorByCloseToCloseVolatility lessVolatile = 
	      	new SelectorByCloseToCloseVolatility(mostLiquidSelector.GetTableOfSelectedTickers(),
	      	                                    true,currentDate.AddDays(-30), currentDate,
	      	                                    this.numberOfEligibleTickers/2);
	 			
	      DateTime firstDateForPerformanceComputation =
				(DateTime)((IndexBasedEndOfDayTimer)sender).IndexQuotes.Rows[currentDateArrayPositionInTimer - this.lengthInDaysForPerformance]["quDate"];
		    
	 			SelectorByAbsolutePerformance bestTickersFromLessVolatile =
					new SelectorByAbsolutePerformance(lessVolatile.GetTableOfSelectedTickers(),
					false,firstDateForPerformanceComputation,currentDate,this.bestTickers.Length);
	
				SelectorByAbsolutePerformance worstTickersFromLessVolatile =
					new SelectorByAbsolutePerformance(lessVolatile.GetTableOfSelectedTickers(),
					true,firstDateForPerformanceComputation,currentDate,this.bestTickers.Length);
	
				DataTable tableOfBestTickers = bestTickersFromLessVolatile.GetTableOfSelectedTickers();
				if(tableOfBestTickers.Rows.Count >= this.bestTickers.Length)
				{	
					this.thereAreEnoughBestTickers = true;
					for(int i = 0;i<this.bestTickers.Length;i++)
					{
						if( (double)tableOfBestTickers.Rows[i]["SimpleReturn"] > 0.0 )
							this.bestTickers[i] = (string)tableOfBestTickers.Rows[i][0];
						else
							this.thereAreEnoughBestTickers = false;
					}
				}
				
				DataTable tableOfWorstTickers = worstTickersFromLessVolatile.GetTableOfSelectedTickers();
				if(tableOfWorstTickers.Rows.Count >= this.worstTickers.Length)
				{	
					this.thereAreEnoughWorstTickers = true;
					for(int i = 0;i<this.worstTickers.Length;i++)
					{
						if( (double)tableOfWorstTickers.Rows[i]["SimpleReturn"] < 0.0 )
							this.worstTickers[i] = (string)tableOfWorstTickers.Rows[i][0];
						else
							this.thereAreEnoughWorstTickers = false;
					}
				}
    	}
    }
    
    #endregion
  
  }
}
