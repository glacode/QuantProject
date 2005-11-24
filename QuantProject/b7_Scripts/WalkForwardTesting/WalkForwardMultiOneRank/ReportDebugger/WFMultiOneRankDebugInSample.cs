/*
QuantProject - Quantitative Finance Library

WFMultiOneRankDebugInSample.cs
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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Presentation.Reporting.WindowsForm;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardMultiOneRank
{
	/// <summary>
	/// Simulation to test the in sample optimization
	/// </summary>
	public class WFMultiOneRankDebugInSample
	{
		private string[] signedTickers;
		private DateTime dateTime;
		private int numberDaysForInSampleOptimization;
		private string benchmark;

		private DateTime startDateTime;
		private IHistoricalQuoteProvider historicalQuoteProvider;
		private IndexBasedEndOfDayTimer endOfDayTimer;
		private Account account;
		private WFMultiOneRankDebugEndOfDayTimerHandler
			endOfDayTimerHandler;

		public WFMultiOneRankDebugInSample( string[] signedTickers ,
			DateTime dateTime , int numberDaysForInSampleOptimization ,
			string benchmark )
		{
			this.signedTickers = signedTickers;
			this.dateTime = dateTime;
			this.numberDaysForInSampleOptimization =
				numberDaysForInSampleOptimization;
			this.benchmark = benchmark;

			this.startDateTime = this.dateTime.AddDays(
				-this.numberDaysForInSampleOptimization - 1 );
			this.historicalQuoteProvider =
				new HistoricalAdjustedQuoteProvider();
		}
		#region run
		private void run_initializeEndOfDayTimer()
		{
			this.endOfDayTimer =
				new IndexBasedEndOfDayTimer(
				new EndOfDayDateTime( this.startDateTime ,
				EndOfDaySpecificTime.MarketOpen ), this.benchmark );
		}
		private void run_initializeAccount()
		{
			this.account = new Account( "WFMultiOneRankDebugInSample" ,
				this.endOfDayTimer ,
				new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ,
				this.historicalQuoteProvider ) ,
				new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ,
				this.historicalQuoteProvider ) );
		}
		private void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler =
				new WFMultiOneRankDebugEndOfDayTimerHandler(
				this.account , this.signedTickers );
		}
		public void marketOpenEventHandler(	Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( this.account.Transactions.Count == 0 )
				this.account.AddCash( 30000 );
		}
		public void oneHourAfterMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( ( ( IEndOfDayTimer )sender ).GetCurrentTime().DateTime >
				this.dateTime )
			{
				// the simulation has reached the ending date
				this.account.EndOfDayTimer.Stop();
				Report report = new Report( this.account , this.historicalQuoteProvider );
				report.Create( "WFT One Rank" , 1 ,
					new EndOfDayDateTime( this.dateTime , EndOfDaySpecificTime.OneHourAfterMarketClose ) , "^SPX" );
				report.Show();
			}
		}
		public void Run()
		{
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
