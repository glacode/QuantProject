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
using System.IO;

using QuantProject.ADT;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Timing;
using QuantProject.Presentation;
using QuantProject.Presentation.Reporting.WindowsForm;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WeightedPositionsChoosers;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger;

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
		private IWFLagWeightedPositionsChooser
			wFLagWeightedPositionsChooser;
//		private int numberOfDrivingPositions;
//		private int numberOfPortfolioPositions;
		private int numberDaysForInSampleOptimization;
		private int numDaysBetweenEachOptimization;
//		private int generationNumberForGeneticOptimizer;
//		private int populationSizeForGeneticOptimizer;
		private string benchmark;
		private DateTime firstDateTime;
		private DateTime lastDateTime;
		private double maxRunningHours;

		private IHistoricalQuoteProvider historicalQuoteProvider;
		private IEndOfDayTimer endOfDayTimer;
		private DateTime startingTimeForScript;
		private Account account;
		private WFLagEndOfDayTimerHandler endOfDayTimerHandler;

    private WFLagLog wFLagLog;

		public event NewProgressEventHandler InSampleNewProgress;

		public RunWalkForwardLag(
			string tickerGroupID ,
			int maxNumberOfEligibleTickers ,
			IWFLagWeightedPositionsChooser wFLagWeightedPositionsChooser ,
			int numDaysBetweenEachOptimization ,
			DateTime firstDateTime ,
			DateTime lastDateTime ,
			double maxRunningHours )
		{
			this.tickerGroupID = tickerGroupID;
			this.numberEligibleTickers = maxNumberOfEligibleTickers;
			this.wFLagWeightedPositionsChooser = wFLagWeightedPositionsChooser;
			this.numberDaysForInSampleOptimization =
				wFLagWeightedPositionsChooser.NumberDaysForInSampleOptimization;
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
//			this.generationNumberForGeneticOptimizer = generationNumberForGeneticOptimizer;
//			this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
			this.benchmark =
				this.wFLagWeightedPositionsChooser.Benchmark;
			this.firstDateTime = firstDateTime;
			this.lastDateTime = lastDateTime;

			this.maxRunningHours = maxRunningHours;
			this.wFLagLog =
				new WFLagLog( this.numberDaysForInSampleOptimization ,
				this.benchmark );
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
				this.wFLagWeightedPositionsChooser ,
				this.numDaysBetweenEachOptimization ,
				this.account );
		}
		public static void WriteToTextLog( string message )
		{
//			Console.WriteLine( message );
//			Console.WriteLine( "" );
			System.Diagnostics.Debug.Listeners[0].WriteLine( message );

//			FileStream fileStream = new FileStream( "WFLagLog.Txt" ,
//				FileMode.OpenOrCreate );
			StreamWriter streamWriter = new StreamWriter( "WFLagLog.Txt" ,
				true );
			streamWriter.WriteLine( message );
			streamWriter.Close();
//			fileStream.Close();
		}
		private  void inSampleNewProgressEventHandler(
			Object sender , NewProgressEventArgs eventArgs )
		{
			if ( !(this.InSampleNewProgress == null) )
				this.InSampleNewProgress( this , eventArgs );
			// the following if statement is used to avoid too many output
			// when small populations are chosen. Comment it out
			// when large populations are chosen
//			if ( eventArgs.CurrentProgress % 20 == 0 )
				RunWalkForwardLag.WriteToTextLog(
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
		#region marketCloseEventHandler
		private void marketCloseEventHandler_handleProgessBarForm(
			IEndOfDayTimer endOfDayTimer )
		{
			string progress = "Current out of sample date:" +
				endOfDayTimer.GetCurrentTime().DateTime.ToString() + " - " +
				"First date:" + this.firstDateTime.ToString() + " - " +
				"Last date:" + this.lastDateTime.ToString();
//			Console.WriteLine( progress );
			RunWalkForwardLag.WriteToTextLog( progress );
		}
		private string getLongStringForDateTime( DateTime dateTime )
		{
			string stringForFileName =
				dateTime.Year.ToString() + "_" +
				dateTime.Month.ToString().PadLeft( 2 , '0' ) + "_" +
				dateTime.Day.ToString().PadLeft( 2 , '0' ) + "_" +
				dateTime.Hour.ToString().PadLeft( 2 , '0' ) + "_" +
				dateTime.Minute.ToString().PadLeft( 2 , '0' ) + "_" +
				dateTime.Second.ToString().PadLeft( 2 , '0' );
			return stringForFileName;
		}
		private string getShortStringForDateTime( DateTime dateTime )
		{
			string stringForFileName =
				dateTime.Year.ToString() + "_" +
				dateTime.Month.ToString().PadLeft( 2 , '0' ) + "_" +
				dateTime.Day.ToString().PadLeft( 2 , '0' );
			return stringForFileName;
		}
		private string getDefaultLogFileName( DateTime currentTime )
		{
			string defaultFileName =
				this.getLongStringForDateTime( DateTime.Now ) + "_" +
				"Group_" + this.tickerGroupID + "_" +
				"DrvPstns_" +
				this.wFLagWeightedPositionsChooser.NumberOfDrivingPositions + "_" +
				"PrtfPstns_" +
				this.wFLagWeightedPositionsChooser.NumberOfPortfolioPositions + "_" +
				"From_" + this.getShortStringForDateTime(
				(DateTime)this.account.Transactions.GetKey( 0 ) ) + "_" +
				"To_" + this.getShortStringForDateTime( currentTime ) + "_" +
				"inSample_" + this.numberDaysForInSampleOptimization.ToString() + "_" +
				"dysBtwnEachOptmzn_" + this.numDaysBetweenEachOptimization.ToString();
			return defaultFileName;
		}
		private void saveLog( DateTime currentTime )
		{
			this.wFLagLog.TransactionHistory = this.account.Transactions;
			string defaultFolderPath =
				"C:\\Documents and Settings\\Glauco\\Desktop\\reports\\WalkForwardLag";
			VisualObjectArchiver visualObjectArchiver =
				new VisualObjectArchiver();
			visualObjectArchiver.Save( this.wFLagLog , "qPWFLagLog" ,
				this.getDefaultLogFileName( currentTime ) , defaultFolderPath );
		}
		private void showReport( object sender )
		{
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
			report.Show();
		}
		private bool isTimeToStop( DateTime currentTime )
		{
			DateTime maxEndingDateTimeForScript =
				this.startingTimeForScript.AddHours( this.maxRunningHours );
			bool scriptTimeElapsed = ( DateTime.Now >= maxEndingDateTimeForScript );
			bool areBestTickersToBeChosen =
				this.endOfDayTimerHandler.AreBestTickersToBeChosen();
			return
			( ( currentTime >	this.lastDateTime ) ||
				( scriptTimeElapsed && areBestTickersToBeChosen ) );
		}
		private void marketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			DateTime currentTime =
				( ( IEndOfDayTimer )sender ).GetCurrentTime().DateTime;
			if ( this.isTimeToStop( currentTime ) )
			{
				// either the simulation has reached the ending date or
				// too much time elapsed since the simulation started
				this.account.EndOfDayTimer.Stop();
				//				this.progressBarForm.Close();
				//				ObjectArchiver.Archive( this.account ,
				//					@"C:\Documents and Settings\Glauco\Desktop\reports\final.qP" );
				this.saveLog( currentTime );
				this.showReport( sender );
//				WFMultiOneRankReportDebugger wFMultiOneRankReportDebugger =
//					new WFMultiOneRankReportDebugger( this.numberOfPortfolioPositions ,
//					this.numberDaysForInSampleOptimization , this.benchmark );
//				report.TransactionGrid.MouseUp +=
//					new MouseEventHandler(
//					wFMultiOneRankReportDebugger.MouseClickEventHandler );
			}
			else
				// the simulation has not reached the ending date, yet
				this.marketCloseEventHandler_handleProgessBarForm(
					( IEndOfDayTimer )sender );
		}
		#endregion
		private void newChosenPositionsEventHandler( object sender ,
			WFLagNewChosenPositionsEventArgs eventArgs )
		{
//			WFLagChosenPositions wFLagChosenPositions =
//				new WFLagChosenPositions( eventArgs.WFLagChosenTickers ,
//				this.endOfDayTimer.GetCurrentTime().DateTime );
			this.wFLagLog.Add( eventArgs.WFLagLogItem );
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
			this.endOfDayTimer.MarketClose +=
				new MarketCloseEventHandler(
				this.marketCloseEventHandler );
			this.endOfDayTimerHandler.NewChosenPositions +=
				new NewChosenPositionsEventHandler(
				this.newChosenPositionsEventHandler	);
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
