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

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardMultiOneRank
{
	/// <summary>
	/// Script to test the One Rank strategy on many tickers, chosing the best group
	/// when a fixed time span has elapsed.
	/// </summary>
	public class RunWalkForwardMultiOneRank : Script
	{
		private IHistoricalQuoteProvider historicalQuoteProvider;
    private ReportTable reportTable;

		private string tickerGroupID;
		private int numberEligibleTickers;
		private int numberOfPositionsToBeChosen;
		private int numberDaysForInSampleOptimization;
		private int numDaysBetweenEachOptimization;
		private int generationNumberForGeneticOptimizer;
		private int populationSizeForGeneticOptimizer;
		private string benchmark;
		private DateTime firstDateTime;
		private DateTime lastDateTime;
		private double maxRunningHours;

		private WFMultiOneRankEndOfDayTimerHandler endOfDayTimerHandler;

		private Account account;
		
		private IEndOfDayTimer endOfDayTimer;

		public RunWalkForwardMultiOneRank(
			string tickerGroupID ,
			int numberEligibleTickers ,
			int numberOfPositionsToBeChosen ,
			int numberDaysForInSampleOptimization ,
			int numDaysBetweenEachOptimization ,
			int generationNumberForGeneticOptimizer ,
			int populationSizeForGeneticOptimizer ,
			string benchmark ,
			DateTime firstDateTime ,
			DateTime lastDateTime ,
			double maxRunningHours )
		{
			this.tickerGroupID = tickerGroupID;
			this.numberEligibleTickers = numberEligibleTickers;
			this.numberOfPositionsToBeChosen = numberOfPositionsToBeChosen;
			this.numberDaysForInSampleOptimization = numberDaysForInSampleOptimization;
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
			this.generationNumberForGeneticOptimizer = generationNumberForGeneticOptimizer;
			this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
			this.benchmark = benchmark;
			this.firstDateTime = firstDateTime;
			this.lastDateTime = lastDateTime;

			this.maxRunningHours = maxRunningHours;

			this.reportTable = new ReportTable( "Summary_Reports" );
		}

		public event NewProgressEventHandler InSampleNewProgress;
//		public event NewProgressEventHandler OutOfSampleNewProgress;

    #region Run
		private void run_initializeHistoricalQuoteProvider()
		{
			this.historicalQuoteProvider =
				new HistoricalAdjustedQuoteProvider();
		}
		private void run_initializeEndOfDayTimer()
		{
			EndOfDayDateTime endOfDayDateTime =
				new EndOfDayDateTime( firstDateTime ,
				EndOfDaySpecificTime.MarketOpen );
			this.endOfDayTimer =
				new IndexBasedEndOfDayTimer( endOfDayDateTime ,
					this.benchmark );
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
			this.endOfDayTimerHandler =
				new WFMultiOneRankEndOfDayTimerHandler(
				this.tickerGroupID ,
				this.numberEligibleTickers ,
				this.numberOfPositionsToBeChosen ,
				this.numberDaysForInSampleOptimization ,
				this.numDaysBetweenEachOptimization ,
				this.account ,
				this.generationNumberForGeneticOptimizer ,
				this.populationSizeForGeneticOptimizer );
//			this.endOfDayTimerHandler = new EndOfDayTimerHandler( 4 , 3 , 2 , 100 , 30 ,
//				this.account );
		}
		private  void inSampleNewProgressEventHandler(
			Object sender , NewProgressEventArgs eventArgs )
		{
			if ( !(this.InSampleNewProgress == null) )
				this.InSampleNewProgress( this , eventArgs );
			Console.WriteLine(
				eventArgs.CurrentProgress.ToString() + " / " +
				eventArgs.Goal.ToString() +
				" - " +
				DateTime.Now.ToString() );
		}
//		private void run_initializeProgressBar_newThread()
//		{
//			this.progressBarForm = new ProgressBarForm( this );
//			this.progressBarForm.ShowDialog();
//		}
//		private void run_initializeProgressBar()
//		{
//			Thread thread = new Thread(new ThreadStart(run_initializeProgressBar_newThread));
////			thread.IsBackground = true;
//			thread.Start();
//		}
		private void run_initializeProgressHandlers()
		{
			this.endOfDayTimerHandler.InSampleNewProgress +=
				new InSampleNewProgressEventHandler( this.inSampleNewProgressEventHandler );
		}
		#region oneHourAfterMarketCloseEventHandler
		//		private void oneHourAfterMarketCloseEventHandler_handleProgessBarForm(
		//			IEndOfDayTimer endOfDayTimer )
		//		{
		//			long elapsedDays = Convert.ToInt64( ((TimeSpan)( endOfDayTimer.GetCurrentTime().DateTime - 
		//				this.startDateTime.DateTime )).TotalDays );
		//			double totalDays = Convert.ToDouble( ((TimeSpan)( this.endDateTime.DateTime - 
		//				this.startDateTime.DateTime )).TotalDays + 1);
		//			if ( Math.Floor( elapsedDays / totalDays * 100 ) >
		//				Math.Floor( ( elapsedDays - 1 ) / totalDays * 100 ) )
		//			{
		//				// a new out of sample time percentage point has been elapsed
		//				int currentProgress = Convert.ToInt16( Math.Floor( elapsedDays / totalDays * 100 ) );
		//				NewProgressEventArgs newProgressEventArgs =
		//					new NewProgressEventArgs( currentProgress , 100 );
		//				this.OutOfSampleNewProgress( this , newProgressEventArgs );
		//			}
		//		}
		private void oneHourAfterMarketCloseEventHandler_handleProgessBarForm(
			IEndOfDayTimer endOfDayTimer )
		{
			string progress = "Current out of sample date:" +
				endOfDayTimer.GetCurrentTime().DateTime.ToString() + " - " +
				"First date:" + this.firstDateTime.ToString() + " - " +
				"Last date:" + this.lastDateTime.ToString();
			Console.WriteLine( progress );
		}
		public void marketOpenEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( this.account.Transactions.Count == 0 )
				this.account.AddCash( 30000 );
		}
		#region oneHourAfterMarketCloseEventHandler
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
				rowDateTime.AddDays( -this.numberDaysForInSampleOptimization );
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
		public void oneHourAfterMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( ( ( IEndOfDayTimer )sender ).GetCurrentTime().DateTime >
				this.lastDateTime )
			{
				// the simulation has reached the ending date
				this.account.EndOfDayTimer.Stop();
//				this.progressBarForm.Close();
//				ObjectArchiver.Archive( this.account ,
//					@"C:\Documents and Settings\Glauco\Desktop\reports\final.qP" );
				Report report = new Report( this.account , this.historicalQuoteProvider );
				report.Create( "WFT One Rank" , 1 ,
					new EndOfDayDateTime( this.lastDateTime ,
					EndOfDaySpecificTime.OneHourAfterMarketClose ) ,
					this.benchmark );
				report.TransactionGrid.MouseUp +=
					new MouseEventHandler( this.mouseEventHandler );
				report.Show();
			}
			else
				// the simulation has not reached the ending date, yet
				this.oneHourAfterMarketCloseEventHandler_handleProgessBarForm(
					( IEndOfDayTimer )sender );
		}
		#endregion
		private void run_addEventHandlers()
		{
			this.endOfDayTimer.MarketOpen +=
				new MarketOpenEventHandler( this.marketOpenEventHandler );
			this.endOfDayTimer.FiveMinutesBeforeMarketClose +=
				new FiveMinutesBeforeMarketCloseEventHandler(
				this.endOfDayTimerHandler.FiveMinutesBeforeMarketCloseEventHandler );
			this.endOfDayTimer.OneHourAfterMarketClose +=
				new OneHourAfterMarketCloseEventHandler(
				this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler );
			this.endOfDayTimer.OneHourAfterMarketClose +=
				new OneHourAfterMarketCloseEventHandler(
				this.oneHourAfterMarketCloseEventHandler );
		}
		public override void Run()
		{
			run_initializeHistoricalQuoteProvider();
			run_initializeEndOfDayTimer();
			run_initializeAccount();
			run_initializeEndOfDayTimerHandler();
//			run_initializeProgressBar();
			run_initializeProgressHandlers();
			run_addEventHandlers();
//			this.progressBarForm.Show();
			this.endOfDayTimer.Start();
		}
    #endregion
	}
}
