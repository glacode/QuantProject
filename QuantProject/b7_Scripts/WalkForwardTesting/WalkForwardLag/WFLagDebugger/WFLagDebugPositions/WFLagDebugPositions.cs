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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Ordering;
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
					this.benchmark );
				report.Show();
			}
		}
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
