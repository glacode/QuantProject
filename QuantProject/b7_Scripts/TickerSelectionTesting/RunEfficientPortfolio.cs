/*
QuantProject - Quantitative Finance Library

RunEfficientPortfolio.cs
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
using QuantProject.ADT.Optimizing.Genetic;
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
using QuantProject.Scripts.WalkForwardTesting.LinearCombination;


namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	/// <summary>
	/// Base class for running efficient portfolio tests using genetic optimizer
	/// </summary>
	[Serializable]
	public class RunEfficientPortfolio
	{
		protected string tickerGroupID;
		protected int numberOfEligibleTickers;
		protected int numberOfTickersToBeChosen;
		protected int numDaysForOptimizationPeriod;
		protected int generationNumberForGeneticOptimizer;
		protected int populationSizeForGeneticOptimizer;

		protected ReportTable reportTable;
		protected DateTime startDateTime;
		protected DateTime endDateTime;
		//protected int numIntervalDays;// number of days for the equity line graph
		protected HistoricalMarketValueProvider historicalMarketValueProvider;


		//protected ProgressBarForm progressBarForm;

		protected EndOfDayTimerHandler endOfDayTimerHandler;

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
			get{return this.endOfDayTimer.GetCurrentDateTime() ;}
		}
		
		public RunEfficientPortfolio(string benchmark,
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
			this.ScriptName = "EfficientGeneric";
			this.portfolioType = portfolioType;
			this.startingTimeForScript = DateTime.Now;
			this.maxRunningHours = maxRunningHours;
			//this.numIntervalDays = 3;
		}
		
		public RunEfficientPortfolio(string tickerGroupID, int numberOfEligibleTickers,
		                             int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
		                             int generationNumberForGeneticOptimizer,
		                             int populationSizeForGeneticOptimizer, string benchmark,
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
			this.generationNumberForGeneticOptimizer = generationNumberForGeneticOptimizer;
			this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
			this.reportTable = new ReportTable( "Summary_Reports" );
			this.startDateTime =
				HistoricalEndOfDayTimer.GetFiveMinutesBeforeMarketClose( startDate );
//				new EndOfDayDateTime(
//				startDate, EndOfDaySpecificTime.FiveMinutesBeforeMarketClose );
			this.endDateTime =
				HistoricalEndOfDayTimer.GetOneHourAfterMarketClose( endDate );
//				new EndOfDayDateTime(
//				endDate, EndOfDaySpecificTime.OneHourAfterMarketClose );
			this.benchmark = benchmark;
			this.ScriptName = "EfficientGeneric";
			this.targetReturn = targetReturn;
			this.portfolioType = portfolioType;
			this.startingTimeForScript = DateTime.Now;
			this.maxRunningHours = maxRunningHours;
			//this.numIntervalDays = 3;
		}
		
		protected string getGenomeCounterInfo()
		{
			string returnValue = "";
			if(this.endOfDayTimerHandler.GenomeCounter != null)
				returnValue = "Total generated genomes: " +
					this.endOfDayTimerHandler.GenomeCounter.TotalEvaluatedGenomes.ToString() +
					"; Current fitness: " +
					this.endOfDayTimerHandler.GenomeCounter.BestFitness.ToString();
			return returnValue;
		}

		#region Run
		
		protected virtual void run_initializeEndOfDayTimer()
		{
			//default endOfDayTimer
			this.endOfDayTimer =
				new IndexBasedEndOfDayTimer( this.startDateTime, this.endDateTime,
				                            this.benchmark );
			
		}
		
		protected virtual void run_initializeAccount()
		{
			//default account with no commissions and no slippage calculation
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
		
		public virtual string SaveScriptResults_CreateFileName()
		{
			return   DateTime.Now.Hour.ToString().PadLeft(2,'0') + "_" +
				DateTime.Now.Minute.ToString().PadLeft(2,'0') + "_" +
				"From_" + this.tickerGroupID + "_" +
				+ this.numberOfEligibleTickers +
				"_OptDays" + this.numDaysForOptimizationPeriod + "_Port" +
				this.numberOfTickersToBeChosen + "GenNum" +
				this.generationNumberForGeneticOptimizer +
				"PopSize" + this.populationSizeForGeneticOptimizer +
				Convert.ToString(this.portfolioType);
		}
		
		
		public virtual void SaveScriptResults()
		{
			string fileName = this.SaveScriptResults_CreateFileName();
			string dirNameWhereToSaveReports =
				System.Configuration.ConfigurationManager.AppSettings["ReportsArchive"] +
				"\\" + this.ScriptName + "\\";
			string dirNameWhereToSaveTransactions =
				System.Configuration.ConfigurationManager.AppSettings["TransactionsArchive"] +
				"\\" + this.ScriptName + "\\";
			string dirNameWhereToSaveBestGenomes =
				System.Configuration.ConfigurationManager.AppSettings["GenomesArchive"] +
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
			this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveBestGenomes);
			OptimizationOutput optimizationOutput = new OptimizationOutput();
			foreach(GenomeRepresentation genomeRepresentation in this.endOfDayTimerHandler.BestGenomes)
				optimizationOutput.Add(genomeRepresentation);
			ObjectArchiver.Archive(optimizationOutput,
			                       dirNameWhereToSaveBestGenomes +
			                       fileName + ".bgn");
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
		protected virtual void run_addEventHandlers()
		{
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.checkDateForReport );

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
