/*
QuantProject - Quantitative Finance Library

RunSimpleSelection.cs
Copyright (C) 2003
Marco Milletti

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
using System.Collections;
using System.Data;
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
using QuantProject.Business.Financial.Accounting.Commissions;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Presentation.Reporting.WindowsForm;


namespace QuantProject.Scripts.TickerSelectionTesting.SimpleSelection
{
	/// <summary>
	/// Base class for simple selection test
	/// The strategy simply chooses the tickers with the highest fitness,
	/// for short or long trading
	/// </summary>
	[Serializable]
	public class RunSimpleSelection
	{
		protected string tickerGroupID;
		protected int numberOfEligibleTickers;
		protected int numberOfTickersToBeChosen;
		protected int numDaysForOptimizationPeriod;

		protected ReportTable reportTable;
		protected DateTime startDateTime;
		protected DateTime endDateTime;
		protected HistoricalMarketValueProvider historicalMarketValueProvider;

		protected EndOfDayTimerHandlerSimpleSelection endOfDayTimerHandler;

		protected Account account;
		
		protected QuantProject.Business.Timing.Timer endOfDayTimer;

		protected string benchmark;
		
		protected string scriptName;
		
		protected double targetReturn;
		
		protected PortfolioType portfolioType;
		
		protected DateTime startingTimeForScript;
		protected double maxRunningHours;
		//if MaxNumberOfHoursForScript has elapsed and the script
		//is still running, it will be stopped.
		
		public PortfolioType TypeOfPortfolio
		{
			get { return this.portfolioType; }
		}
		
		public virtual string ScriptName
		{
			get{return this.scriptName;}
			set{this.scriptName = value;}
		}
		
		public DateTime TimerLastDate
		{
			get{return this.endOfDayTimer.GetCurrentDateTime();}
		}
		
		public RunSimpleSelection(string benchmark,
		                          DateTime startDate, DateTime endDate,
		                          PortfolioType portfolioType,
		                          double maxRunningHours)
		{
			
			this.startDateTime =
				HistoricalEndOfDayTimer.GetFiveMinutesBeforeMarketClose( startDate );
//			new EndOfDayDateTime(
//				startDate, EndOfDaySpecificTime.FiveMinutesBeforeMarketClose );
			this.endDateTime =
				HistoricalEndOfDayTimer.GetOneHourAfterMarketClose( endDate );
//			new EndOfDayDateTime(
//				endDate, EndOfDaySpecificTime.OneHourAfterMarketClose );
			this.benchmark = benchmark;
			this.ScriptName = "SimpleTestGeneric";
			this.portfolioType = portfolioType;
			this.startingTimeForScript = DateTime.Now;
			this.maxRunningHours = maxRunningHours;
			//this.numIntervalDays = 3;
		}
		
		public RunSimpleSelection(string tickerGroupID, int numberOfEligibleTickers,
		                          int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
		                          string benchmark,
		                          DateTime startDate, DateTime endDate,
		                          double targetReturn,
		                          PortfolioType portfolioType,
		                          double maxRunningHours)
		{
			//this.progressBarForm = new ProgressBarForm();
			this.tickerGroupID = tickerGroupID;
			this.numberOfEligibleTickers = numberOfEligibleTickers;
			this.numberOfTickersToBeChosen = numberOfTickersToBeChosen;
			this.numDaysForOptimizationPeriod = numDaysForOptimizationPeriod;
			this.reportTable = new ReportTable( "Summary_Reports" );
			this.startDateTime =
				HistoricalEndOfDayTimer.GetFiveMinutesBeforeMarketClose( startDate );
//				new EndOfDayDateTime(
//				startDate, EndOfDaySpecificTime.FiveMinutesBeforeMarketClose );
			this.endDateTime =
				HistoricalEndOfDayTimer.GetOneHourAfterMarketClose( endDate );
//			new EndOfDayDateTime(
//				endDate, EndOfDaySpecificTime.OneHourAfterMarketClose );
			this.benchmark = benchmark;
			this.ScriptName = "SimpleTestGeneric";
			this.targetReturn = targetReturn;
			this.portfolioType = portfolioType;
			this.startingTimeForScript = DateTime.Now;
			this.maxRunningHours = maxRunningHours;
			//this.numIntervalDays = 3;
		}
		
		#region Run
		
		protected virtual void run_initializeEndOfDayTimer()
		{
			//default endOfDayTimer
			this.endOfDayTimer =
				new IndexBasedEndOfDayTimer( this.startDateTime, this.benchmark );
			
		}
		
		protected virtual void run_initializeAccount()
		{
			//default account with no commissions
			this.account = new Account( this.scriptName , this.endOfDayTimer ,
			                           new HistoricalDataStreamer( this.endOfDayTimer ,
			                                                              this.historicalMarketValueProvider ) ,
			                           new HistoricalOrderExecutor( this.endOfDayTimer ,
			                                                               this.historicalMarketValueProvider ));
			
		}
		protected virtual void run_initializeEndOfDayTimerHandler()
		{
			//always needs specific implementation in inherited classes;
		}
		
		protected virtual void run_initializeHistoricalQuoteProvider()
		{
			//always needs specific implementation in inherited classes;
		}
		
		protected void checkDateForReport_createDirIfNotPresent(string dirPath)
		{
			if(!Directory.Exists(dirPath))
				Directory.CreateDirectory(dirPath);
		}
		
//		protected virtual void checkDateForReport(
//			Object sender , DateTime dateTime)
//		{
//			if(dateTime.EndOfDayDateTime.DateTime>=this.endDateTime.DateTime ||
//			   DateTime.Now >= this.startingTimeForScript.AddHours(this.maxRunningHours))
//				//last date is reached by the timer or maxRunning hours
//				//are elapsed from the time script started
//				this.SaveScriptResults();
//		}
		
		protected virtual void checkDateForReport(
			Object sender , DateTime dateTime)
		{
			if ( HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
			{
				if( dateTime >= this.endDateTime ||
				   DateTime.Now >= this.startingTimeForScript.AddHours(this.maxRunningHours))
					//last date is reached by the timer or maxRunning hours
					//are elapsed from the time script started
					this.SaveScriptResults();
			}
		}

		
		public virtual void SaveScriptResults()
		{
			string fileName = "SimpleSelectionFrom"+this.numberOfEligibleTickers +
				"OptDays" + this.numDaysForOptimizationPeriod + "Portfolio" +
				this.numberOfTickersToBeChosen +
				"Target" + Convert.ToString(this.targetReturn) +
				Convert.ToString(this.portfolioType);
			string dirNameWhereToSaveReports =
				System.Configuration.ConfigurationManager.AppSettings["ReportsArchive"] +
				"\\" + this.ScriptName + "\\";
			string dirNameWhereToSaveTransactions =
				System.Configuration.ConfigurationManager.AppSettings["TransactionsArchive"] +
				"\\" + this.ScriptName + "\\";

			//default report with numIntervalDays = 1
			AccountReport accountReport = this.account.CreateReport(fileName,1,
			                                                        this.endOfDayTimer.GetCurrentDateTime(),
			                                                        this.benchmark,
			                                                        new HistoricalAdjustedQuoteProvider());
			this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveReports);
			ObjectArchiver.Archive(accountReport,
			                       dirNameWhereToSaveReports +
			                       fileName + ".qPr");
			//
			this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveTransactions);
			ObjectArchiver.Archive(this.account.Transactions,
			                       dirNameWhereToSaveTransactions +
			                       fileName + ".qPt");
			//
			this.endOfDayTimer.Stop();
		}
		
		protected virtual void run_initialize()
		{
			run_initializeHistoricalQuoteProvider();
			run_initializeEndOfDayTimer();
			run_initializeAccount();
			run_initializeEndOfDayTimerHandler();
			//run_initializeProgressHandlers();
		}
		
		private void newDateTimeEventHandler( object sender , DateTime dateTime )
		{
			if ( HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
				this.checkDateForReport( sender , dateTime );
		}

		protected virtual void run_addEventHandlers()
		{
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.newDateTimeEventHandler );

//			this.endOfDayTimer.MarketClose +=
//				new MarketCloseEventHandler(
//					this.checkDateForReport);
			
			//in inherited classes'override method:
			//add here TimerHandler's handlers to timer's events
			//example
			//this.endOfDayTimer.EVENT_NAME +=
			//  new EVENT_NAMEEventHandler(
			//  this.endOfDayTimerHandler.EVENT_NAMEEventHandler);
		}
		
		
		public virtual void Run()
		{
			this.run_initialize();
			this.run_addEventHandlers();
			//this.progressBarForm.Show();
			this.endOfDayTimer.Start();
		}
		
		#endregion
		
	}
}
