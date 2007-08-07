/*
QuantProject - Quantitative Finance Library

WFLagDebugPositions.cs
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
using System.Drawing;

using QuantProject.ADT.Collections;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;
using QuantProject.Presentation.Reporting.WindowsForm;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger
{
	/// <summary>
	/// Class used to test the Lag Strategy in sample
	/// </summary>
	public class WFLagDebugPositions
	{
		private WFLagChosenPositions wFLagChosenPositions;
		private DateTime preSampleFirstDateTime;
		private DateTime inSampleFirstDateTime;
		private DateTime inSampleLastDateTime;
		private DateTime postSampleLastDateTime;
		private string benchmark;

		private HistoricalAdjustedQuoteProvider
			historicalQuoteProvider;
		private IndexBasedEndOfDayTimer endOfDayTimer;
		private Account account;
		private WFLagDebugPositionsEndOfDayTimerHandler
			endOfDayTimerHandler;

		/// <summary>
		/// To test the log for a Lag Strategy's walk forward test
		/// </summary>
		/// <param name="WFLagRunDebugPositions"></param>
		/// <param name="transactionDateTime"></param>
		/// <param name="inSampleDays"></param>
		/// <param name="preSampleDays"></param>
		/// <param name="postSampleDays"></param>
		public WFLagDebugPositions(
			WFLagChosenPositions wFLagChosenPositions ,
			DateTime inSampleLastDateTime ,
			int preSampleDays ,
			int inSampleDays ,
			int postSampleDays ,
			string benchmark )
		{
			this.wFLagChosenPositions = wFLagChosenPositions;
			this.inSampleLastDateTime = inSampleLastDateTime;
			this.inSampleFirstDateTime =
				this.inSampleLastDateTime.AddDays( -inSampleDays + 1 );
			this.preSampleFirstDateTime =
				this.inSampleFirstDateTime.AddDays( -preSampleDays );
			this.postSampleLastDateTime =
				this.inSampleLastDateTime.AddDays( postSampleDays );
			this.benchmark = benchmark;
		}
		#region Run
		private void run_initializeHistoricalQuoteProvider()
		{
			this.historicalQuoteProvider =
				new HistoricalAdjustedQuoteProvider();
		}
		private void run_initializeEndOfDayTimer()
		{
			this.endOfDayTimer =
				new IndexBasedEndOfDayTimer(
				new EndOfDayDateTime( this.preSampleFirstDateTime ,
				EndOfDaySpecificTime.MarketOpen ), this.benchmark );
		}
		private void run_initializeAccount()
		{
			this.account = new Account( "WFLagDebugPositions" ,
				this.endOfDayTimer ,
				new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ,
				this.historicalQuoteProvider ) ,
				new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ,
				this.historicalQuoteProvider ) );
		}
		private void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler =
				new WFLagDebugPositionsEndOfDayTimerHandler(
				this.account , this.wFLagChosenPositions );
		}
		public void marketOpenEventHandler(	Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( this.account.Transactions.Count == 0 )
				this.account.AddCash( 30000 );
		}
		#region oneHourAfterMarketCloseEventHandler
		#region getEquityLineForSignedTickers
		/// <summary>
		/// Returns a virtual amount of quantities for each virtual ticker.
		/// Non integer values can be returned also, that's why we call
		/// them virtual quantities
		/// </summary>
		/// <returns></returns>
		private Hashtable getVirtualQuantities( ICollection positions ,
			DateTime dateTime )
		{
			Hashtable virtualQuantities = new Hashtable();
			double valueForEachPosition = 30000 /
				positions.Count;
			foreach( string stringForSignedTicker in	positions )
			{
				SignedTicker signedTicker = new SignedTicker( stringForSignedTicker );
				string ticker = signedTicker.Ticker;
				EndOfDayDateTime endOfDayDateTime =
					new EndOfDayDateTime( dateTime , EndOfDaySpecificTime.MarketClose );
				double tickerQuote =
					this.historicalQuoteProvider.GetMarketValue(
					ticker , endOfDayDateTime );
				double virtualQuantity = valueForEachPosition / tickerQuote;
				if ( signedTicker.IsShort )
					virtualQuantity = -virtualQuantity;
				virtualQuantities.Add( ticker , virtualQuantity );
			}
			return virtualQuantities;
		}
		private double getCash( Hashtable drivingTickerVirtualQuantities )
		{
			double cash = 30000;
			double valueForEachPosition =
				cash / drivingTickerVirtualQuantities.Count;
			foreach ( double virtualQuantity in
				drivingTickerVirtualQuantities.Values )
			{
				if ( virtualQuantity > 0 )
					// long position
					cash -= valueForEachPosition;
				else
					// virtualQuantity > 0 i.e. short position
					cash += valueForEachPosition;
			}
			return cash;
		}
		private double getPortfolioValueForSignedTickers( DateTime dateTime ,
			Hashtable tickerVirtualQuantities )
		{
			double portfolioValueForDrivingPositions = 0;
			foreach( string ticker in tickerVirtualQuantities.Keys )
			{
				EndOfDayDateTime endOfDayDateTime =	new EndOfDayDateTime(
					dateTime , EndOfDaySpecificTime.MarketClose );
				double tickerQuote = this.historicalQuoteProvider.GetMarketValue(
					ticker , endOfDayDateTime );
				double virtualQuantity = (double)tickerVirtualQuantities[ ticker ];
				portfolioValueForDrivingPositions +=	virtualQuantity * tickerQuote;
			}
			return portfolioValueForDrivingPositions;
		}
		private EquityLine getEquityLineForSignedTickers(
			ICollection signedTickers , Report report )
		{
			EquityLine equityLineForPortfolioPositions =
				report.AccountReport.EquityLine;
			EquityLine equityLineForSignedTickers =
				new EquityLine();
			Hashtable virtualQuantities =
				this.getVirtualQuantities( signedTickers ,
				(DateTime)equityLineForPortfolioPositions.GetKey( 0 ) );
			double cash = this.getCash( virtualQuantities ); 
			foreach( DateTime dateTime in
				equityLineForPortfolioPositions.Keys )
				equityLineForSignedTickers.Add( dateTime ,
					cash + this.getPortfolioValueForSignedTickers( dateTime ,
					virtualQuantities ) );
			return equityLineForSignedTickers;
		}
		#endregion
//		private void addEquityLineForSignedTickers(
//			ICollection signedTickers , Color color , Report report )
//		{
//			EquityLine equityLineSignedTickers =
//				this.getEquityLineForSignedTickers(
//				signedTickers , report );
//			report.AddEquityLine( equityLineSignedTickers ,
//				color );
//		}
		private void addEquityLineForWeightedPositions(
			WeightedPositions weightedPositions , Color color , Report report )
		{
			EquityLine equityLineForWeightedPositions =
				weightedPositions.GetVirtualEquityLine(
				30000 , report.AccountReport.EquityLine );
			report.AddEquityLine( equityLineForWeightedPositions ,
				color );
		}
		
		public void oneHourAfterMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( ( ( IEndOfDayTimer )sender ).GetCurrentTime().DateTime >
				this.postSampleLastDateTime )
			{
				// the simulation has reached the ending date
				this.account.EndOfDayTimer.Stop();
				Report report = new Report( this.account , this.historicalQuoteProvider );
				report.Create( "WFLag debug positions" , 1 ,
					new EndOfDayDateTime( this.postSampleLastDateTime ,
					EndOfDaySpecificTime.OneHourAfterMarketClose ) ,
					this.benchmark , false );
//				EquityLine equityLineForDrivingPositions =
//					this.getEquityLineForPositions( report );
//				this.addEquityLineForSignedTickers(
//					this.wFLagChosenPositions.DrivingWeightedPositions.Keys ,
//					Color.YellowGreen ,	report );
				this.addEquityLineForWeightedPositions(
					this.wFLagChosenPositions.DrivingWeightedPositions ,
					Color.YellowGreen ,	report );
				this.addEquityLineForWeightedPositions(
					this.wFLagChosenPositions.PortfolioWeightedPositions ,
					Color.Brown ,	report );
				//				this.addEquityLineForSignedTickers(
				//					this.wFLagChosenPositions.PortfolioWeightedPositions.Keys ,
				//					Color.Brown ,	report );
				report.Show();
			}
		}
		#endregion
		public void Run()
		{
			run_initializeHistoricalQuoteProvider();
			run_initializeEndOfDayTimer();
			run_initializeAccount();
			run_initializeEndOfDayTimerHandler();
			this.endOfDayTimer.MarketOpen +=
				new MarketOpenEventHandler( this.marketOpenEventHandler );
			this.endOfDayTimer.FiveMinutesBeforeMarketClose +=
				new FiveMinutesBeforeMarketCloseEventHandler(
				this.endOfDayTimerHandler.FiveMinutesBeforeMarketCloseEventHandler );
			this.endOfDayTimer.OneHourAfterMarketClose +=
				new OneHourAfterMarketCloseEventHandler(
				this.oneHourAfterMarketCloseEventHandler );
			this.endOfDayTimer.Start();
		}
		#endregion
	}
}
