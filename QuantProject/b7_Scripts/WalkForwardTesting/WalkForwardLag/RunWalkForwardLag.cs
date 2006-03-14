/*
QuantProject - Quantitative Finance Library

RunWalkForwardLag.cs
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

using QuantProject.ADT;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Scripting;
using QuantProject.Business.Timing;
using QuantProject.Presentation.Reporting.WindowsForm;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// Script to test the Lag strategy on many tickers, chosing the best group
	/// when a fixed time span has elapsed.
	/// </summary>
	public class RunWalkForwardLag : Script
	{
		private string tickerGroupID;
		private int numberEligibleTickers;
		private int numberOfPortfolioPositions;
		private int numberOfDrivingPositions;
		private int numberDaysForInSampleOptimization;
		private int numDaysBetweenEachOptimization;
		private int generationNumberForGeneticOptimizer;
		private int populationSizeForGeneticOptimizer;
		private string benchmark;
		private DateTime firstDateTime;
		private DateTime lastDateTime;
		private double maxRunningHours;

		private IHistoricalQuoteProvider historicalQuoteProvider;
		private IEndOfDayTimer endOfDayTimer;
		private DateTime startingTimeForScript;
		private Account account;
		private WFLagEndOfDayTimerHandler endOfDayTimerHandler;

		public event NewProgressEventHandler InSampleNewProgress;

		public RunWalkForwardLag(
			string tickerGroupID ,
			int numberEligibleTickers ,
			int numberOfPortfolioPositions ,
			int numberOfDrivingPositions ,
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
			this.numberOfDrivingPositions = numberOfDrivingPositions;
			this.numberDaysForInSampleOptimization = numberDaysForInSampleOptimization;
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
			this.generationNumberForGeneticOptimizer = generationNumberForGeneticOptimizer;
			this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
			this.benchmark = benchmark;
			this.firstDateTime = firstDateTime;
			this.lastDateTime = lastDateTime;

			this.maxRunningHours = maxRunningHours;
		}

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
			this.account = new Account( "WalkForwardLag" , this.endOfDayTimer ,
				new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ,
				this.historicalQuoteProvider ) ,
				new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ,
				this.historicalQuoteProvider ) );
		}
		private void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler =
				new WFLagEndOfDayTimerHandler(
				this.tickerGroupID ,
				this.benchmark ,
				this.numberEligibleTickers ,
				this.numberOfPortfolioPositions ,
				this.numberOfDrivingPositions ,
				this.numberDaysForInSampleOptimization ,
				this.numDaysBetweenEachOptimization ,
				this.account ,
				this.generationNumberForGeneticOptimizer ,
				this.populationSizeForGeneticOptimizer );
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
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( this.account.Transactions.Count == 0 )
				this.account.AddCash( 30000 );
		}
		private void oneHourAfterMarketCloseEventHandler_handleProgessBarForm(
			IEndOfDayTimer endOfDayTimer )
		{
			string progress = "Current out of sample date:" +
				endOfDayTimer.GetCurrentTime().DateTime.ToString() + " - " +
				"First date:" + this.firstDateTime.ToString() + " - " +
				"Last date:" + this.lastDateTime.ToString();
			Console.WriteLine( progress );
		}
		public void oneHourAfterMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( ( ( ( IEndOfDayTimer )sender ).GetCurrentTime().DateTime >
				this.lastDateTime ) ||
				( DateTime.Now >=
				this.startingTimeForScript.AddHours( this.maxRunningHours ) ) )
			{
				// either the simulation has reached the ending date or
				// too much time elapsed since the simulation started
				this.account.EndOfDayTimer.Stop();
				//				this.progressBarForm.Close();
				//				ObjectArchiver.Archive( this.account ,
				//					@"C:\Documents and Settings\Glauco\Desktop\reports\final.qP" );
				DateTime lastReportDateTime = this.lastDateTime;
				if ( ( ( IEndOfDayTimer )sender ).GetCurrentTime().DateTime <
					lastReportDateTime )
					lastReportDateTime =
						( ( IEndOfDayTimer )sender ).GetCurrentTime().DateTime;
				Report report = new Report( this.account , this.historicalQuoteProvider );
				report.Create( "Walk Forward Lag" , 1 ,
					new EndOfDayDateTime( lastReportDateTime ,
					EndOfDaySpecificTime.OneHourAfterMarketClose ) ,
					this.benchmark );
//				WFMultiOneRankReportDebugger wFMultiOneRankReportDebugger =
//					new WFMultiOneRankReportDebugger( this.numberOfPortfolioPositions ,
//					this.numberDaysForInSampleOptimization , this.benchmark );
//				report.TransactionGrid.MouseUp +=
//					new MouseEventHandler(
//					wFMultiOneRankReportDebugger.MouseClickEventHandler );
				report.Show();
			}
			else
				// the simulation has not reached the ending date, yet
				this.oneHourAfterMarketCloseEventHandler_handleProgessBarForm(
					( IEndOfDayTimer )sender );
		}
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
			this.startingTimeForScript = DateTime.Now;

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
	}
}
