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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using QuantProject.ADT;
using QuantProject.ADT.FileManaging;
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
using QuantProject.Scripts.SimpleTesting;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardOneRank
{
	/// <summary>
	/// Script to test the One Rank strategy on many tickers, chosing the best group
	/// when a fixed time span has elapsed.
	/// </summary>
	public class RunWalkForwardOneRank : Script , IWalkForwardProgressNotifier
	{
		private HistoricalMarketValueProvider historicalMarketValueProvider =
			new HistoricalAdjustedQuoteProvider();
		private ReportTable reportTable;

		private DateTime startDateTime;
		private DateTime endDateTime;
		int numberDaysForPerformanceCalculation;

		private int numIntervalDays;

		private ProgressBarForm progressBarForm;

		private EndOfDayTimerHandler endOfDayTimerHandler;

		private Account account;
		
		private QuantProject.Business.Timing.Timer endOfDayTimer;

		public RunWalkForwardOneRank()
		{
			this.reportTable = new ReportTable( "Summary_Reports" );
			this.startDateTime =
				HistoricalEndOfDayTimer.GetMarketClose(
					new DateTime( 2002 , 1 , 1 ) );
//			new EndOfDayDateTime(
//				new DateTime( 2002 , 1 , 1 ) , EndOfDaySpecificTime.MarketOpen );
			this.endDateTime =
				HistoricalEndOfDayTimer.GetOneHourAfterMarketClose(
					new DateTime( 2002 , 12 , 31 ) );
//				new EndOfDayDateTime(
//				new DateTime( 2002 , 12 , 31 ) , EndOfDaySpecificTime.OneHourAfterMarketClose );
			this.numberDaysForPerformanceCalculation = 120;
			this.numIntervalDays = 1;
		}

		public event NewProgressEventHandler InSampleNewProgress;
		public event NewProgressEventHandler OutOfSampleNewProgress;

		#region Run
		private void run_initializeEndOfDayTimer()
		{
			this.endOfDayTimer =
				new IndexBasedEndOfDayTimer( this.startDateTime, "^SPX" );
		}
		private void run_initializeAccount()
		{
			this.account = new Account( "WalkForwardOneRank" , this.endOfDayTimer ,
			                           new HistoricalDataStreamer( this.endOfDayTimer ,
			                                                              this.historicalMarketValueProvider ) ,
			                           new HistoricalOrderExecutor( this.endOfDayTimer ,
			                                                               this.historicalMarketValueProvider ) );
		}
		private void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler = new EndOfDayTimerHandler( 400 , 20 , 5 ,
			                                                     this.numberDaysForPerformanceCalculation , 30 ,
			                                                     this.account );
//			this.endOfDayTimerHandler = new EndOfDayTimerHandler( 4 , 3 , 2 , 100 , 30 ,
//				this.account );
		}
		private  void inSampleNewProgressEventHandler(
			Object sender , NewProgressEventArgs eventArgs )
		{
			this.InSampleNewProgress( this , eventArgs );
		}
		#region
		private void run_initializeProgressBar_newThread()
		{
			this.progressBarForm = new ProgressBarForm( this );
			this.progressBarForm.ShowDialog();
		}
		private void run_initializeProgressBar()
		{
			Thread thread = new Thread(new ThreadStart(run_initializeProgressBar_newThread));
//			thread.IsBackground = true;
			thread.Start();
		}
		#endregion
		private void run_initializeProgressHandlers()
		{
			this.endOfDayTimerHandler.InSampleNewProgress +=
				new InSampleNewProgressEventHandler( this.inSampleNewProgressEventHandler );
		}
		
		public void marketOpenEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( this.account.Transactions.Count == 0 )
				this.account.AddCash( 30000 );
		}
		
		#region oneHourAfterMarketCloseEventHandler
		private void oneHourAfterMarketCloseEventHandler_handleProgessBarForm(
			QuantProject.Business.Timing.Timer endOfDayTimer )
		{
			long elapsedDays = Convert.ToInt64( ((TimeSpan)( endOfDayTimer.GetCurrentDateTime() -
			                                                this.startDateTime )).TotalDays );
			double totalDays = Convert.ToDouble( ((TimeSpan)( this.endDateTime -
			                                                 this.startDateTime )).TotalDays + 1);
			if ( Math.Floor( elapsedDays / totalDays * 100 ) >
			    Math.Floor( ( elapsedDays - 1 ) / totalDays * 100 ) )
			{
				// a new out of sample time percentage point has been elapsed
				int currentProgress = Convert.ToInt16( Math.Floor( elapsedDays / totalDays * 100 ) );
				NewProgressEventArgs newProgressEventArgs =
					new NewProgressEventArgs( currentProgress , 100 );
				this.OutOfSampleNewProgress( this , newProgressEventArgs );
			}
		}
		
		#region mouseEventHandler
		private void showOneRankForm( object sender ,
		                             MouseEventArgs eventArgs )
		{
			DataGrid dataGrid = (DataGrid)sender;
			Point point = new Point( eventArgs.X , eventArgs.Y );
			DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest( point );
			DataTable dataTable = (DataTable)dataGrid.DataSource;
			DataRow dataRow = dataTable.Rows[ hitTestInfo.Row ];
			//			MessageBox.Show( dataRow[ "DateTime" ].ToString() );
			DateTime rowDateTime = (DateTime)dataRow[ "DateTime" ];
			string rowTicker = (string)dataRow[ "InstrumentKey"];
			OneRankForm oneRankForm = new OneRankForm();
			oneRankForm.FirstDateTime =
				rowDateTime.AddDays( -this.numberDaysForPerformanceCalculation );
			oneRankForm.LastDateTime = rowDateTime;
			oneRankForm.Ticker = rowTicker;
			oneRankForm.Show();
		}
		private void mouseEventHandler( object sender , MouseEventArgs eventArgs )
		{
			if ( eventArgs.Button == MouseButtons.Right )
				this.showOneRankForm( sender , eventArgs );
		}
		#endregion
		
		private void oneHourAfterMarketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( ( ( QuantProject.Business.Timing.Timer )sender ).GetCurrentDateTime() >
			    this.endDateTime )
			{
				// the simulation has reached the ending date
				this.account.Timer.Stop();
//				this.progressBarForm.Close();
//				ObjectArchiver.Archive( this.account ,
//					@"C:\Documents and Settings\Glauco\Desktop\reports\final.qP" );
				Report report = new Report( this.account , this.historicalMarketValueProvider );
				report.Create( "WFT One Rank" , this.numIntervalDays , this.endDateTime , "MSFT" );
				report.TransactionGrid.MouseUp +=
					new MouseEventHandler( this.mouseEventHandler );
				report.Show();
			}
			else
				// the simulation has not reached the ending date, yet
				this.oneHourAfterMarketCloseEventHandler_handleProgessBarForm(
					( QuantProject.Business.Timing.Timer )sender );
		}
		#endregion
		
		private void newDateTimeEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( HistoricalEndOfDayTimer.IsMarketOpen( dateTime ) )
				this.marketOpenEventHandler( sender , dateTime );
//			if ( HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
//				this.marketCloseEventHandler( sender , dateTime );
			if ( HistoricalEndOfDayTimer.IsOneHourAfterMarketClose( dateTime ) )
				this.oneHourAfterMarketCloseEventHandler( sender , dateTime );
		}

		
		public override void Run()
		{
			run_initializeEndOfDayTimer();
			run_initializeAccount();
			run_initializeEndOfDayTimerHandler();
			run_initializeProgressBar();
			run_initializeProgressHandlers();
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.newDateTimeEventHandler );
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.endOfDayTimerHandler.NewDateTimeEventHandler );

//			this.endOfDayTimer.MarketOpen +=
//				new MarketOpenEventHandler( this.marketOpenEventHandler );
//			this.endOfDayTimer.OneHourAfterMarketClose +=
//				new OneHourAfterMarketCloseEventHandler(
//				this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler );
//			this.endOfDayTimer.OneHourAfterMarketClose +=
//				new OneHourAfterMarketCloseEventHandler(
//				this.oneHourAfterMarketCloseEventHandler );
//			this.endOfDayTimer.FiveMinutesBeforeMarketClose +=
//				new FiveMinutesBeforeMarketCloseEventHandler(
//				this.endOfDayTimerHandler.FiveMinutesBeforeMarketCloseEventHandler );
			
//			this.progressBarForm.Show();
			this.endOfDayTimer.Start();
		}
		#endregion
	}
}
