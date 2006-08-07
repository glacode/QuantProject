/*
QuantProject - Quantitative Finance Library

RunExtremeCounterTrend.cs
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
using System.Collections;
using System.Data;
using QuantProject.ADT;
using QuantProject.ADT.Optimizing.Genetic;
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
using QuantProject.Data.Selectors; 
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Presentation.Reporting.WindowsForm;
using QuantProject.ADT.FileManaging;
using QuantProject.Scripts.WalkForwardTesting.LinearCombination;


namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.ExtremeCounterTrend
{
	/// <summary>
	/// Script that implements an oscillating strategy,
	/// for finding tickers that tend to 
	/// earn (lose) from previous losses (gains), using efficient portfolios  
	/// </summary>
	[Serializable]
	public class RunExtremeCounterTrend : RunEfficientPortfolio
	{
    private int numDaysForReturnCalculation;
    private double maxAcceptableCloseToCloseDrawdown;
    private int numDaysBetweenEachOptimization;
		
    public RunExtremeCounterTrend(string tickerGroupID, int maxNumOfEligibleTickersForOptimization, 
                                    int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod, 
                                    int generationNumberForGeneticOptimizer,
                                    int populationSizeForGeneticOptimizer, string benchmark,
                                    DateTime startDate, DateTime endDate,
                                   	int numDaysForReturnCalculation,
                                   	int numDaysBetweenEachOptimization,
                                    PortfolioType portfolioType, double maxAcceptableCloseToCloseDrawdown, 
                                    double maxRunningHours):
																base(tickerGroupID, maxNumOfEligibleTickersForOptimization, 
                                    numberOfTickersToBeChosen, numDaysForOptimizationPeriod, 
                                    generationNumberForGeneticOptimizer,
                                    populationSizeForGeneticOptimizer, benchmark,
                                    startDate, endDate, 0.0,
                                   	portfolioType, maxRunningHours)
		{
      this.ScriptName = "ECT_SR_NoWeights";
      this.numDaysForReturnCalculation = numDaysForReturnCalculation;
      this.maxAcceptableCloseToCloseDrawdown = maxAcceptableCloseToCloseDrawdown;
      this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
		}

    #region auxiliary overriden methods for Run
    
    protected override void run_initializeEndOfDayTimerHandler()
    {
      this.endOfDayTimerHandler = new EndOfDayTimerHandlerECT(this.tickerGroupID, this.numberOfEligibleTickers,
    	                                                        this.numberOfTickersToBeChosen, this.numDaysForOptimizationPeriod,
                                                              this.account,
    	                                                        this.generationNumberForGeneticOptimizer,
    	                                                        this.populationSizeForGeneticOptimizer, this.benchmark,
    	                                                        this.numDaysForReturnCalculation,
    	                                                        this.numDaysBetweenEachOptimization,
                                                              this.portfolioType, this.maxAcceptableCloseToCloseDrawdown);
    }
    
    protected override void run_initializeHistoricalQuoteProvider()
    {
    	this.historicalQuoteProvider = new HistoricalAdjustedQuoteProvider();
    }
    
    protected override void run_addEventHandlers()
    {
           
      this.endOfDayTimer.MarketClose +=
        new MarketCloseEventHandler(
        this.endOfDayTimerHandler.MarketCloseEventHandler);
      
      this.endOfDayTimer.MarketClose +=
        new MarketCloseEventHandler(
        this.checkDateForReport);
      
      this.endOfDayTimer.OneHourAfterMarketClose += 
      	new OneHourAfterMarketCloseEventHandler(
      	   this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler);
    }

    public override void SaveScriptResults()
    {
      string fileName = this.scriptName + "_From_" + this.tickerGroupID + "_" + this.numberOfEligibleTickers +
                      "_DaysForOpt" + this.numDaysForOptimizationPeriod + "Tickers" +
                      this.numberOfTickersToBeChosen + "GenNum" + 
                      this.generationNumberForGeneticOptimizer +
                      "PopSize" + this.populationSizeForGeneticOptimizer +
        							"HalfPeriodDays" + Convert.ToString(this.numDaysForReturnCalculation) + 
        							Convert.ToString(this.portfolioType);
      string dirNameWhereToSaveReports = System.Configuration.ConfigurationSettings.AppSettings["ReportsArchive"] +
                         								"\\" + this.ScriptName + "\\";
      string dirNameWhereToSaveTransactions = System.Configuration.ConfigurationSettings.AppSettings["TransactionsArchive"] +
                       									"\\" + this.ScriptName + "\\";
      string dirNameWhereToSaveBestGenomes = System.Configuration.ConfigurationSettings.AppSettings["GenomesArchive"] +
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
      //
//      this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveTransactions);
//      ObjectArchiver.Archive(this.account.Transactions,
//                             dirNameWhereToSaveTransactions +
//                             fileName + ".qPt");
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
    
    
    #endregion 
	}
}
