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
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		private ReportTable reportTable;

		private string tickerGroupID;
		private int numberEligibleTickers;
		private int numberOfPortfolioPositions;
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
		private QuantProject.Business.Timing.Timer endOfDayTimer;
		private DateTime startingTimeForScript;

		public RunWalkForwardMultiOneRank(
			string tickerGroupID ,
			int numberEligibleTickers ,
			int numberOfPortfolioPositions ,
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
			this.numberOfPortfolioPositions = numberOfPortfolioPositions;
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
			this.historicalMarketValueProvider =
				new HistoricalAdjustedQuoteProvider();
			this.startingTimeForScript = DateTime.Now;
		}
		private void run_initializeEndOfDayTimer()
		{
			DateTime dateTime =
				HistoricalEndOfDayTimer.GetMarketOpen( this.firstDateTime );
//				new EndOfDayDateTime( firstDateTime ,
//				EndOfDaySpecificTime.MarketOpen );
			this.endOfDayTimer =
				new IndexBasedEndOfDayTimer( dateTime ,
				                            this.benchmark );
		}
		private void run_initializeAccount()
		{
			this.account = new Account( "WalkForwardOneRank" , this.endOfDayTimer ,
			                           new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ,
			                                                              this.historicalMarketValueProvider ) ,
			                           new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ,
			                                                               this.historicalMarketValueProvider ) );
		}
		private void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler =
				new WFMultiOneRankEndOfDayTimerHandler(
					this.tickerGroupID ,
					this.benchmark ,
					this.numberEligibleTickers ,
					this.numberOfPortfolioPositions ,
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
		private void run_initializeProgressHandlers()
		{
			this.endOfDayTimerHandler.InSampleNewProgress +=
				new InSampleNewProgressEventHandler( this.inSampleNewProgressEventHandler );
		}
		
		public void marketOpenEventHandler(
			Object sender , DateTime sateTime )
		{
			if ( this.account.Transactions.Count == 0 )
				this.account.AddCash( 30000 );
		}
		
		#region oneHourAfterMarketCloseEventHandler
		private void oneHourAfterMarketCloseEventHandler_handleProgessBarForm(
			QuantProject.Business.Timing.Timer endOfDayTimer )
		{
			string progress = "Current out of sample date:" +
				endOfDayTimer.GetCurrentDateTime().ToString() + " - " +
				"First date:" + this.firstDateTime.ToString() + " - " +
				"Last date:" + this.lastDateTime.ToString();
			Console.WriteLine( progress );
		}		
		private void oneHourAfterMarketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( ( ( ( QuantProject.Business.Timing.Timer )sender ).GetCurrentDateTime() >
			      this.lastDateTime ) ||
			    ( DateTime.Now >=
			     this.startingTimeForScript.AddHours( this.maxRunningHours ) ) )
			{
				// the simulation has reached the ending date
				this.account.Timer.Stop();
//				this.progressBarForm.Close();
//				ObjectArchiver.Archive( this.account ,
//					@"C:\Documents and Settings\Glauco\Desktop\reports\final.qP" );
				DateTime lastReportDateTime = this.lastDateTime;
				if ( ( ( QuantProject.Business.Timing.Timer )sender ).GetCurrentDateTime() <
				    lastReportDateTime )
					lastReportDateTime =
						( ( QuantProject.Business.Timing.Timer )sender ).GetCurrentDateTime();
				Report report = new Report( this.account , this.historicalMarketValueProvider );
				report.Create(
					"WFT One Rank" , 1 ,
					HistoricalEndOfDayTimer.GetMarketClose( lastReportDateTime ) ,
//					new EndOfDayDateTime( lastReportDateTime ,
//					EndOfDaySpecificTime.OneHourAfterMarketClose ) ,
					this.benchmark );
				WFMultiOneRankReportDebugger wFMultiOneRankReportDebugger =
					new WFMultiOneRankReportDebugger( this.numberOfPortfolioPositions ,
					                                 this.numberDaysForInSampleOptimization , this.benchmark );
				report.TransactionGrid.MouseUp +=
					new MouseEventHandler(
						wFMultiOneRankReportDebugger.MouseClickEventHandler );
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

		
		private void run_addEventHandlers()
		{
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.newDateTimeEventHandler );
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.endOfDayTimerHandler.NewDateTimeEventHandler );

//			this.endOfDayTimer.MarketOpen +=
//				new MarketOpenEventHandler( this.marketOpenEventHandler );
//			this.endOfDayTimer.FiveMinutesBeforeMarketClose +=
//				new FiveMinutesBeforeMarketCloseEventHandler(
//				this.endOfDayTimerHandler.FiveMinutesBeforeMarketCloseEventHandler );
//			this.endOfDayTimer.OneHourAfterMarketClose +=
//				new OneHourAfterMarketCloseEventHandler(
//				this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler );
//			this.endOfDayTimer.OneHourAfterMarketClose +=
//				new OneHourAfterMarketCloseEventHandler(
//				this.oneHourAfterMarketCloseEventHandler );
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
