/*
QuantProject - Quantitative Finance Library

LinearCombinationTest.cs
Copyright (C) 2003 
Glauco Siliprandi

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
using System.Collections;

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Presentation.Reporting.WindowsForm;


namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Object to test a linear combination strategy on given positions,
	/// on a given period of time 
	/// </summary>
	public class LinearCombinationTest
	{
		private DateTime firstDate;
		private DateTime lastDate;
		private string[] signedTickers;

		private HistoricalRawQuoteProvider historicalQuoteProvider;
		private HistoricalEndOfDayTimer historicalEndOfDayTimer;
		private Account account;

		public LinearCombinationTest( DateTime firstDate , DateTime lastDate ,
			string[] signedTickers )
		{
			this.firstDate = firstDate;
			this.lastDate = lastDate;
			this.signedTickers = signedTickers;
		}
		private long marketOpenEventHandler_addOrder_getQuantity(
			string ticker )
		{
			double accountValue = this.account.GetMarketValue();
			double currentTickerAsk =
				this.account.DataStreamer.GetCurrentAsk( ticker );
			double maxPositionValueForThisTicker =
				accountValue/this.signedTickers.Length;
			long quantity = Convert.ToInt64(	Math.Floor(
				maxPositionValueForThisTicker /	currentTickerAsk ) );
			return quantity;
		}
		private void marketOpenEventHandler_addOrder( string signedTicker )
		{
			OrderType orderType = GenomeRepresentation.GetOrderType( signedTicker );
			string ticker = GenomeRepresentation.GetTicker( signedTicker );
			long quantity = marketOpenEventHandler_addOrder_getQuantity( ticker );
			Order order = new Order( orderType , new Instrument( ticker ) ,
				quantity );
			this.account.AddOrder( order );
		}
		private void marketOpenEventHandler_addOrders()
		{
			foreach ( string signedTicker in this.signedTickers )
				marketOpenEventHandler_addOrder( signedTicker );
		}
		private void marketOpenEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( ( this.account.CashAmount == 0 ) &&
				( this.account.Transactions.Count == 0 ) )
				// cash has not been added yet
				this.account.AddCash( 30000 );
			marketOpenEventHandler_addOrders();
		}

		private void fiveMinutesBeforeMarketCloseEventHandler_closePositions()
		{
			ArrayList tickers = new ArrayList();
			foreach ( Position position in this.account.Portfolio.Positions )
				tickers.Add( position.Instrument.Key );
			foreach ( string ticker in tickers )
				this.account.ClosePosition( ticker );
		}
		private void fiveMinutesBeforeMarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs)
		{
			fiveMinutesBeforeMarketCloseEventHandler_closePositions();
		}
		private void oneHourAfterMarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( this.account.EndOfDayTimer.GetCurrentTime().DateTime >=
				this.lastDate )
				this.account.EndOfDayTimer.Stop();
		}

		public void Run()
		{
			this.historicalEndOfDayTimer =
				new IndexBasedEndOfDayTimer(
				new EndOfDayDateTime( this.firstDate ,
				EndOfDaySpecificTime.MarketOpen ) , "DYN" );
			this.historicalQuoteProvider =
				new HistoricalRawQuoteProvider();
			this.account = new Account( "LinearCombination" , historicalEndOfDayTimer ,
				new HistoricalEndOfDayDataStreamer( historicalEndOfDayTimer ,
				this.historicalQuoteProvider ) ,
				new HistoricalEndOfDayOrderExecutor( historicalEndOfDayTimer ,
				this.historicalQuoteProvider ) );
//			OneRank oneRank = new OneRank( account ,
//				this.endDateTime );
			this.historicalEndOfDayTimer.MarketOpen +=
				new MarketOpenEventHandler(
				this.marketOpenEventHandler );
			this.historicalEndOfDayTimer.FiveMinutesBeforeMarketClose +=
				new FiveMinutesBeforeMarketCloseEventHandler(
				this.fiveMinutesBeforeMarketCloseEventHandler );
			this.historicalEndOfDayTimer.OneHourAfterMarketClose +=
				new OneHourAfterMarketCloseEventHandler(
				this.oneHourAfterMarketCloseEventHandler );
			this.account.EndOfDayTimer.Start();

			Report report = new Report( this.account , this.historicalQuoteProvider );
			report.Create( "Linear Combination" , 1 ,
				new EndOfDayDateTime( this.lastDate , EndOfDaySpecificTime.MarketClose ) ,
				"^SPX" );
			//			ObjectArchiver.Archive( report.AccountReport ,
			//				@"C:\Documents and Settings\Glauco\Desktop\reports\runOneRank.qPr" );
			report.Show();
		}
	}
}
