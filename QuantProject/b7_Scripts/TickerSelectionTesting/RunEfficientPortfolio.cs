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
    protected int numDaysForLiquidity;
    protected int generationNumberForGeneticOptimizer;
    protected int populationSizeForGeneticOptimizer;

    protected ReportTable reportTable;
    protected EndOfDayDateTime startDateTime;
    protected EndOfDayDateTime endDateTime;
    //protected int numIntervalDays;// number of days for the equity line graph
		protected IHistoricalQuoteProvider historicalQuoteProvider;


    //protected ProgressBarForm progressBarForm;

    protected EndOfDayTimerHandler endOfDayTimerHandler;

    protected Account account;
		
    protected IEndOfDayTimer endOfDayTimer;

    protected string benchmark;
    
    protected string scriptName;
    
    protected double targetReturn;
    
    protected PortfolioType portfolioType;
    
    protected DateTime startingTimeForScript;
    protected double maxRunningHours;
    //if MaxNumberOfHoursForScript has elapsed and the script
    //is still running, it will be stopped.
    
    public string[] LastChosenTickers
    {
      get { return this.endOfDayTimerHandler.LastChosenTickers; }
    }
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
    	get{return this.endOfDayTimer.GetCurrentTime().DateTime ;}
    }	
	
		public RunEfficientPortfolio(string benchmark,
                                DateTime startDate, DateTime endDate, 
                                PortfolioType portfolioType, 
                                double maxRunningHours)
		{
     
      this.startDateTime = new EndOfDayDateTime(
        startDate, EndOfDaySpecificTime.FiveMinutesBeforeMarketClose );
      this.endDateTime = new EndOfDayDateTime(
        endDate, EndOfDaySpecificTime.OneHourAfterMarketClose );
      this.benchmark = benchmark;
     	this.ScriptName = "EfficientGeneric";
     	this.portfolioType = portfolioType;
     	this.startingTimeForScript = DateTime.Now;
      this.maxRunningHours = maxRunningHours;
      //this.numIntervalDays = 3;
		}
    
    public RunEfficientPortfolio(string tickerGroupID, int numberOfEligibleTickers,
                                int numberOfTickersToBeChosen, int numDaysForLiquidity, 
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
      this.numDaysForLiquidity = numDaysForLiquidity;
      this.generationNumberForGeneticOptimizer = generationNumberForGeneticOptimizer;
      this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
      this.reportTable = new ReportTable( "Summary_Reports" );
      this.startDateTime = new EndOfDayDateTime(
        startDate, EndOfDaySpecificTime.FiveMinutesBeforeMarketClose );
      this.endDateTime = new EndOfDayDateTime(
        endDate, EndOfDaySpecificTime.OneHourAfterMarketClose );
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
        new IndexBasedEndOfDayTimer( this.startDateTime, this.benchmark );
    	
    }
    
    protected virtual void run_initializeAccount()
    {
      //default account with no commissions
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
     	//always needs specific implementation in inherited classes; 
    }
    
    private void checkDateForReport_createDirIfNotPresent(string dirPath)
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
                      "LiqDays" + this.numDaysForLiquidity + "Portfolio" +
                      this.numberOfTickersToBeChosen + "GenNum" + 
                      this.generationNumberForGeneticOptimizer +
                      "PopSize" + this.populationSizeForGeneticOptimizer +
        							"Target" + Convert.ToString(this.targetReturn) + 
        							Convert.ToString(this.portfolioType);
      string dirNameWhereToSaveReports = System.Configuration.ConfigurationSettings.AppSettings["ReportsArchive"] +
                         								"\\" + this.ScriptName + "\\";
      string dirNameWhereToSaveTransactions = System.Configuration.ConfigurationSettings.AppSettings["TransactionsArchive"] +
                       									"\\" + this.ScriptName + "\\";
      //default report with numIntervalDays = 1
      AccountReport accountReport = this.account.CreateReport(fileName,1,
                                    		this.endOfDayTimer.GetCurrentTime(),
                                    		this.benchmark,
                                        new HistoricalAdjustedQuoteProvider());
      this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveReports);
      ObjectArchiver.Archive(accountReport,
                             dirNameWhereToSaveReports + 
                             fileName + ".qPr");
      this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveTransactions);
      ObjectArchiver.Archive(this.account.Transactions,
                             dirNameWhereToSaveTransactions +
                             fileName + ".qPt");
      
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
