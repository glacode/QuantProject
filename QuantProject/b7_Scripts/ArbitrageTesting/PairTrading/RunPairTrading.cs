/*
QuantProject - Quantitative Finance Library

RunPairTrading.cs
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
using QuantProject.Presentation.Reporting.WindowsForm;
using QuantProject.Scripts.WalkForwardTesting.LinearCombination;


namespace QuantProject.Scripts.ArbitrageTesting.PairTrading
{
	/// <summary>
	/// Base class for running scripts based
	/// on pair trading strategy
	/// </summary>
	[Serializable]
  public class RunPairTrading
	{
    protected string tickerGroupID;
    protected int numberOfEligibleTickers;
    protected int numDaysForOptimizationPeriod;
    protected int generationNumberForGeneticOptimizer;
    protected int populationSizeForGeneticOptimizer;
    protected EndOfDayDateTime startDateTime;
    protected EndOfDayDateTime endDateTime;
		protected IHistoricalQuoteProvider historicalQuoteProvider;
    protected Account account;
		
    protected IEndOfDayTimer endOfDayTimer;

    protected string benchmark;
    
    protected string scriptName;
    
    protected double maxNumOfStdDevForNormalGap;
    protected int minNumOfDaysForGapComputation;
    protected int maxNumOfDaysForGapComputation;
    protected int numDaysBetweenEachOptimization;
    protected DateTime startingTimeForScript;
    protected double maxRunningHours;
    //if MaxNumberOfHoursForScript has elapsed and the script
    //is still running, it will be stopped.
    
    public virtual string ScriptName
    {
    	get{return this.scriptName;}
    	set{this.scriptName = value;}
    }
    
    public DateTime TimerLastDate
    {
    	get{return this.endOfDayTimer.GetCurrentTime().DateTime ;}
    }	
	
//		public RunPairTrading(string benchmark,
//                          DateTime startDate, DateTime endDate,
//                          double maxLevelForNormalGap,
//                          double maxRunningHours)
//		{
//     
//      this.startDateTime = new EndOfDayDateTime(
//        startDate, EndOfDaySpecificTime.FiveMinutesBeforeMarketClose );
//      this.endDateTime = new EndOfDayDateTime(
//        endDate, EndOfDaySpecificTime.OneHourAfterMarketClose );
//      this.maxLevelForNormalGap = maxLevelForNormalGap;
//      this.benchmark = benchmark;
//     	this.ScriptName = "EfficientGeneric";
//     	this.startingTimeForScript = DateTime.Now;
//      this.maxRunningHours = maxRunningHours;
//      //this.numIntervalDays = 3;
//		}
    
    public RunPairTrading(string tickerGroupID, int numberOfEligibleTickers,
                                int numDaysForOptimizationPeriod, 
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer, string benchmark,
                                DateTime startDate, DateTime endDate, 
                                int minNumOfDaysForGapComputation, int maxNumOfDaysForGapComputation, 
                                double maxNumOfStdDevForNormalGap, int numDaysBetweenEachOptimization,
                                double maxRunningHours)
		{
      //this.progressBarForm = new ProgressBarForm();
      this.tickerGroupID = tickerGroupID;
      this.numberOfEligibleTickers = numberOfEligibleTickers;
      this.numDaysForOptimizationPeriod = numDaysForOptimizationPeriod;
      this.generationNumberForGeneticOptimizer = generationNumberForGeneticOptimizer;
      this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
      this.startDateTime = new EndOfDayDateTime(
        startDate, EndOfDaySpecificTime.FiveMinutesBeforeMarketClose );
      this.endDateTime = new EndOfDayDateTime(
        endDate, EndOfDaySpecificTime.OneHourAfterMarketClose );
      this.benchmark = benchmark;
     	this.ScriptName = "PairTradingGeneric";
     	this.maxNumOfStdDevForNormalGap = maxNumOfStdDevForNormalGap;
      this.minNumOfDaysForGapComputation = minNumOfDaysForGapComputation;
      this.maxNumOfDaysForGapComputation = maxNumOfDaysForGapComputation;
      this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
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
      //default account with no commissions and no slippage calculation
    	this.account = new Account( this.scriptName , this.endOfDayTimer ,
        new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ,
					this.historicalQuoteProvider ) ,
        new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ,
					this.historicalQuoteProvider ));
     
    }
    protected virtual void run_initializeEndOfDayTimerHandler()
    {
     	//always needs specific implementation in inherited classes; 
    }
    
    protected virtual void run_initializeHistoricalQuoteProvider()
    {
      this.historicalQuoteProvider = new HistoricalAdjustedQuoteProvider();
    }
    
    protected void checkDateForReport_createDirIfNotPresent(string dirPath)
    {
    	if(!Directory.Exists(dirPath))
    		Directory.CreateDirectory(dirPath);
    }
    
    protected virtual void checkDateForReport(Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs)
    {
      if(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime>=this.endDateTime.DateTime ||
         DateTime.Now >= this.startingTimeForScript.AddHours(this.maxRunningHours))
      //last date is reached by the timer or maxRunning hours
      //are elapsed from the time script started
    		this.SaveScriptResults();
    }
    
    public virtual void SaveScriptResults()
    {
      string fileName = "From"+this.numberOfEligibleTickers +
                      "OptDays" + this.numDaysForOptimizationPeriod + "GenNum" + 
                      this.generationNumberForGeneticOptimizer +
                      "PopSize" + this.populationSizeForGeneticOptimizer;
      string dirNameWhereToSaveReports = System.Configuration.ConfigurationSettings.AppSettings["ReportsArchive"] +
                         								"\\" + this.ScriptName + "\\";
      string dirNameWhereToSaveTransactions = System.Configuration.ConfigurationSettings.AppSettings["TransactionsArchive"] +
                       									"\\" + this.ScriptName + "\\";
//      string dirNameWhereToSaveBestGenomes = System.Configuration.ConfigurationSettings.AppSettings["GenomesArchive"] +
//                                        "\\" + this.ScriptName + "\\";
      //default report with numIntervalDays = 1
      AccountReport accountReport = this.account.CreateReport(fileName,1,
                                    		this.endOfDayTimer.GetCurrentTime(),
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
//      this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveBestGenomes);
      
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
      this.endOfDayTimer.MarketClose +=
        new MarketCloseEventHandler(
        this.checkDateForReport);
      
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
