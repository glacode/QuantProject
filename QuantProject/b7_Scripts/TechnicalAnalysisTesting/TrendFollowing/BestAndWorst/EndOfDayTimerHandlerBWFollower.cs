/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerBWFollower.cs
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

namespace QuantProject.Scripts.TechnicalAnalysisTesting.TrendFollowing.BestAndWorst
{
	/// <summary>
	/// Implements MarketOpenEventHandler and MarketCloseEventHandler
	/// These handlers contain the core strategy for the Best and Worst follower,
	/// based on the assumption that largest moves are confirmed
	/// the next days
	/// </summary>
	[Serializable]
	public class EndOfDayTimerHandlerBWFollower
	{
		private string tickerGroupID;
		private int numberOfEligibleTickers;
		private int lengthInDaysForPerformance;
		private string benchmark;
		private Account account;
		private int numOfWorstTickers;
		private int numOfBestTickers;
		private int numOfTickersForBuying;
		private int numOfTickersForShortSelling;
		private string[] bestTickers;
		private string[] worstTickers;
		private string[] chosenTickers;
		private string[] lastOrderedTickers;
		private ArrayList orders;
		private bool thereAreEnoughBestTickers;
		private bool thereAreEnoughWorstTickers;
		private int closesElapsedWithSomeOpenPosition;

		
		public EndOfDayTimerHandlerBWFollower(string tickerGroupID, int numberOfEligibleTickers,
		                                      int lengthInDaysForPerformance,
		                                      int numOfBestTickers,
		                                      int numOfWorstTickers, int numOfTickersForBuying,
		                                      int numOfTickersForShortSelling,
		                                      Account account, string benchmark)
		{
			this.tickerGroupID = tickerGroupID;
			this.numberOfEligibleTickers = numberOfEligibleTickers;
			this.lengthInDaysForPerformance = lengthInDaysForPerformance;
			this.account = account;
			this.benchmark = benchmark;
			this.numOfBestTickers = numOfBestTickers;
			this.bestTickers = new string[this.numOfBestTickers];
			this.numOfWorstTickers = numOfWorstTickers;
			this.worstTickers = new string[this.numOfWorstTickers];
			this.numOfTickersForBuying = numOfTickersForBuying;
			this.numOfTickersForShortSelling = numOfTickersForShortSelling;
			this.chosenTickers = new string[this.numOfTickersForBuying + this.numOfTickersForShortSelling];
			this.lastOrderedTickers = new string[this.chosenTickers.Length];
			this.orders = new ArrayList();
		}
		
		#region MarketOpenEventHandler

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

		private void setChosenTickers_addTickersForShorting()
		{
			for( int i = 0;i<this.numOfTickersForShortSelling; i++)
				this.chosenTickers[this.numOfTickersForBuying + i] = "-" + this.worstTickers[i];
		}
		
		private void setChosenTickers_addTickersForBuying()
		{
			for( int i = 0; i<this.numOfTickersForBuying; i++ )
				this.chosenTickers[i] = this.bestTickers[i];
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
				{
					string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
				}
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

		#endregion

		#region MarketCloseEventHandler

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
		
		public void MarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if(this.account.Portfolio.Count > 0)
			{
				this.closesElapsedWithSomeOpenPosition++;
				if(this.closesElapsedWithSomeOpenPosition == this.lengthInDaysForPerformance)
				{
					this.closePositions();
					this.OneHourAfterMarketCloseEventHandler(sender,endOfDayTimingEventArgs);
					this.MarketOpenEventHandler(sender, endOfDayTimingEventArgs);
					this.closesElapsedWithSomeOpenPosition = 0;
				}
			}
			else//this.account.Portfolio.Count == 0
			{
				if( ((IndexBasedEndOfDayTimer)sender).CurrentDateArrayPosition >= this.lengthInDaysForPerformance )
				{
					this.OneHourAfterMarketCloseEventHandler(sender,endOfDayTimingEventArgs);
					this.MarketOpenEventHandler(sender, endOfDayTimingEventArgs);
				}
			}
		}

		#endregion

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
			DateTime currentDate = endOfDayTimingEventArgs.EndOfDayDateTime.DateTime;
			int currentDateArrayPositionInTimer = ((IndexBasedEndOfDayTimer)sender).CurrentDateArrayPosition;
			DateTime firstDateForPerformanceComputation =
				(DateTime)((IndexBasedEndOfDayTimer)sender).IndexQuotes.Rows[currentDateArrayPositionInTimer - this.lengthInDaysForPerformance]["quDate"];
			SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID,
			                                                      currentDate);
			DataTable tickersFromGroup = temporizedGroup.GetTableOfSelectedTickers();
			//remark from here for DEBUG
			
			SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromGroup =
				new SelectorByQuotationAtEachMarketDay(tickersFromGroup,
				                                       false, currentDate.AddDays(-30), currentDate,
				                                       tickersFromGroup.Rows.Count, this.benchmark);

			SelectorByAverageRawOpenPrice byPrice =
				new SelectorByAverageRawOpenPrice( quotedAtEachMarketDayFromGroup.GetTableOfSelectedTickers(),
				                                  false, currentDate.AddDays(-10), currentDate,
				                                  tickersFromGroup.Rows.Count, 25 );
			
			SelectorByLiquidity mostLiquidSelector =
				new SelectorByLiquidity(byPrice.GetTableOfSelectedTickers(),
				                        false,currentDate.AddDays(-30), currentDate,
				                        this.numberOfEligibleTickers);
			
			SelectorByCloseToCloseVolatility lessVolatile =
				new SelectorByCloseToCloseVolatility(mostLiquidSelector.GetTableOfSelectedTickers(),
				                                     true,currentDate.AddDays(-30), currentDate,
				                                     this.numberOfEligibleTickers/2);
			
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
					else//not all best tickers have gained
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
					else//not all best tickers have lost
						this.thereAreEnoughWorstTickers = false;
				}
			}
			//     //for DEBUG
			//     //remark from here for real running
			//       SelectorByLiquidity mostLiquidSelector =
			//        new SelectorByLiquidity(tickersFromGroup,
			//        false,currentDate.AddDays(-30), currentDate,
			//        this.numberOfEligibleTickers);
//
			//      SelectorByAverageCloseToClosePerformance bestTickersFromMostLiquid =
			//        new SelectorByAverageCloseToClosePerformance(mostLiquidSelector.GetTableOfSelectedTickers(),
			//        false,currentDate,currentDate,this.bestTickers.Length);
//
			//      SelectorByAverageCloseToClosePerformance worstTickersFromMostLiquid =
			//        new SelectorByAverageCloseToClosePerformance(mostLiquidSelector.GetTableOfSelectedTickers(),
			//        true,currentDate,currentDate,this.worstTickers.Length);
//
			//      DataTable tableOfBestTickers = bestTickersFromMostLiquid.GetTableOfSelectedTickers();
			//      for(int i = 0;i<this.bestTickers.Length;i++)
			//        if(tableOfBestTickers.Rows[i][0] != null)
			//           this.bestTickers[i] = (string)tableOfBestTickers.Rows[i][0];
//
			//      DataTable tableOfWorstTickers = worstTickersFromMostLiquid.GetTableOfSelectedTickers();
			//      for(int i = 0;i<this.worstTickers.Length;i++)
			//        if(tableOfWorstTickers.Rows[i][0] != null)
			//          this.worstTickers[i] = (string)tableOfWorstTickers.Rows[i][0];
		}
		#endregion
	}
}
