/*
QuantProject - Quantitative Finance Library

RunWalkForwardOneRank.cs
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
using System.Data;
using QuantProject.ADT;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Testing;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;
using QuantProject.Presentation.Reporting.WindowsForm;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardOneRank
{
	/// <summary>
	/// Script to test the One Rank strategy on many tickers, chosing the best group
	/// when a fixed time span has elapsed.
	/// </summary>
	public class RunWalkForwardOneRank : Script
	{
		private IHistoricalQuoteProvider historicalQuoteProvider =
			new HistoricalAdjustedQuoteProvider();
    private ReportTable reportTable;
    private EndOfDayDateTime startDateTime;
    private EndOfDayDateTime endDateTime;
    private int numIntervalDays;

    private ProgressBarForm progressBarForm;

		private EndOfDayTimerHandler endOfDayTimerHandler;

		private Account account;
		
		private IEndOfDayTimer endOfDayTimer;

		public RunWalkForwardOneRank()
		{
			this.progressBarForm = new ProgressBarForm();
			this.reportTable = new ReportTable( "Summary_Reports" );
			this.startDateTime = new EndOfDayDateTime(
				new DateTime( 1998 , 1 , 1 ) , EndOfDaySpecificTime.MarketOpen );
			this.endDateTime = new EndOfDayDateTime(
				new DateTime( 1998 , 1 , 30 ) , EndOfDaySpecificTime.OneHourAfterMarketClose );
			this.numIntervalDays = 1;
		}
    #region Run
		private void run_initializeEndOfDayTimer()
		{
			this.endOfDayTimer =
				new HistoricalEndOfDayTimer( this.startDateTime );
		}
		private void run_initializeAccount()
		{
			this.account = new Account( "WalkForwardOneRank" , this.endOfDayTimer ,
				new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ,
					this.historicalQuoteProvider ) ,
				new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ,
					this.historicalQuoteProvider ) );
		}
		private void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler = new EndOfDayTimerHandler( 50 , 20 , 5 , 360 , 30 ,
				this.account );
		}
		private  void inSampleNewProgressEventHandler(
			Object sender , NewProgressEventArgs eventArgs )
		{
			this.progressBarForm.ProgressBarInSample.Value = eventArgs.CurrentProgress;
			this.progressBarForm.ProgressBarInSample.Refresh();
		}
		private void run_initializeProgressHandlers()
		{
			this.endOfDayTimerHandler.InSampleNewProgress +=
				new InSampleNewProgressEventHandler( this.inSampleNewProgressEventHandler );
		}
		#region oneHourAfterMarketCloseEventHandler
		private void oneHourAfterMarketCloseEventHandler_handleProgessBarForm(
			IEndOfDayTimer endOfDayTimer )
		{
			long elapsedDays = Convert.ToInt64( ((TimeSpan)( endOfDayTimer.GetCurrentTime().DateTime - 
				this.startDateTime.DateTime )).TotalDays );
			double totalDays = Convert.ToDouble( ((TimeSpan)( this.endDateTime.DateTime - 
				this.startDateTime.DateTime )).TotalDays + 1);
			if ( Math.Floor( elapsedDays / totalDays * 100 ) >
				Math.Floor( ( elapsedDays - 1 ) / totalDays * 100 ) )
			{
				// a new out of sample time percentage point has been elapsed
				this.progressBarForm.ProgressBarOutOfSample.Value =
					Convert.ToInt16( Math.Floor( elapsedDays / totalDays * 100 ) );
				this.progressBarForm.ProgressBarOutOfSample.Refresh();
			}
		}
		public void marketOpenEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( this.account.Transactions.Count == 0 )
				this.account.AddCash( 30000 );
		}
		public void oneHourAfterMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			this.oneHourAfterMarketCloseEventHandler_handleProgessBarForm(
				 ( IEndOfDayTimer )sender );
			if ( ( ( IEndOfDayTimer )sender ).GetCurrentTime().DateTime >
				this.endDateTime.DateTime )
			{
				// the simulation has reached the ending date
				this.account.EndOfDayTimer.Stop();
				this.progressBarForm.Close();
				Report report = new Report( this.account , this.historicalQuoteProvider );
				report.Show( "WFT One Rank" , this.numIntervalDays , this.endDateTime , "MSFT" );
			}
		}
		#endregion
		public override void Run()
		{
			run_initializeEndOfDayTimer();
			run_initializeAccount();
			run_initializeEndOfDayTimerHandler();
			run_initializeProgressHandlers();
			this.endOfDayTimer.MarketOpen +=
				new MarketOpenEventHandler( this.marketOpenEventHandler );
			this.endOfDayTimer.OneHourAfterMarketClose +=
				new OneHourAfterMarketCloseEventHandler(
				this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler );
			this.endOfDayTimer.OneHourAfterMarketClose +=
				new OneHourAfterMarketCloseEventHandler(
				this.oneHourAfterMarketCloseEventHandler );
			this.endOfDayTimer.FiveMinutesBeforeMarketClose +=
				new FiveMinutesBeforeMarketCloseEventHandler(
				this.endOfDayTimerHandler.FiveMinutesBeforeMarketCloseEventHandler );
			this.progressBarForm.Show();
			this.endOfDayTimer.Start();
		}
    #endregion
	}
}
