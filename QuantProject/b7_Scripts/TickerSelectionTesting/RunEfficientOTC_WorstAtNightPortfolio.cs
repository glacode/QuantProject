/*
QuantProject - Quantitative Finance Library

RunEfficientOTCPortfolio_WorstAtNight.cs
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
using QuantProject.Business.Financial.Accounting.Slippage;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors; 
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Presentation.Reporting.WindowsForm;


namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	/// <summary>
	/// Script to buy at open and sell at close 
	/// the efficient open to close daily portfolio
	/// </summary>
  [Serializable]
  public class RunEfficientOTCPortfolio_WorstAtNight : RunEfficientPortfolio
  {
    protected int numDaysBetweenEachOptimization;
		protected int numOfGenomesForCTOScanning;
    public RunEfficientOTCPortfolio_WorstAtNight(string tickerGroupID, int numberOfEligibleTickers, 
      int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod, 
      int generationNumberForGeneticOptimizer,
      int populationSizeForGeneticOptimizer, string benchmark,
      DateTime startDate, DateTime endDate, double targetReturn,
      PortfolioType portfolioType, double maxRunningHours,
     	int numDaysBetweenEachOptimization, int numOfGenomesForCTOScanning):
      base(tickerGroupID, numberOfEligibleTickers, 
      numberOfTickersToBeChosen, numDaysForOptimizationPeriod, 
      generationNumberForGeneticOptimizer,
      populationSizeForGeneticOptimizer, benchmark,
      startDate, endDate, targetReturn,
      portfolioType, maxRunningHours)
    {
      //this.ScriptName = "OpenCloseScriptsSharpeRatioWithCoeff";
      //this.ScriptName = "OpenCloseScriptsSharpeRatio";
      this.ScriptName = "OTC_WorstAtNight_SharpeRatioNoCoeff";
      //this.ScriptName = "OpenCloseScripts";
      this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
      this.numOfGenomesForCTOScanning = numOfGenomesForCTOScanning;
		}
    
        
    // delete remark delimitations for having ib commission 
    // and a fixed percentage calculation of slippage
    protected override void run_initializeAccount()
    {
      this.account = new Account(this.ScriptName,
                                 this.endOfDayTimer ,
                                 new HistoricalEndOfDayDataStreamer(this.endOfDayTimer ,
                                                                    this.historicalQuoteProvider ) ,
                                 new HistoricalEndOfDayOrderExecutor(this.endOfDayTimer ,
                                                                     this.historicalQuoteProvider)//,
                                                                    // new FixedPercentageSlippageManager(this.historicalQuoteProvider,
                                                                    // this.endOfDayTimer,0.08)), 
                                 );                                    
                                 //new IBCommissionManager());
     
    }
    
    
    protected override void run_initializeEndOfDayTimerHandler()
    {
      this.endOfDayTimerHandler = new EndOfDayTimerHandlerOTC_WorstAtNight(this.tickerGroupID,
        this.numberOfEligibleTickers,
        this.numberOfTickersToBeChosen,
        this.numDaysForOptimizationPeriod,
        this.account,
        this.generationNumberForGeneticOptimizer, 
        this.populationSizeForGeneticOptimizer,
        this.benchmark,
        this.targetReturn,
        this.portfolioType, this.numDaysBetweenEachOptimization,
				this.numOfGenomesForCTOScanning);
    }
    
    protected override void run_initializeHistoricalQuoteProvider()
    {
      this.historicalQuoteProvider = new HistoricalRawQuoteProvider();
      //this.historicalQuoteProvider = new HistoricalAdjustedQuoteProvider();
    }
    
    protected override void run_addEventHandlers()
    {
      this.endOfDayTimer.MarketOpen +=
        new MarketOpenEventHandler(
        this.endOfDayTimerHandler.MarketOpenEventHandler);  
      
      this.endOfDayTimer.MarketClose +=
        new MarketCloseEventHandler(
        this.endOfDayTimerHandler.MarketCloseEventHandler);
      
      this.endOfDayTimer.MarketClose +=
        new MarketCloseEventHandler(
        this.checkDateForReport);
      
      this.endOfDayTimer.OneHourAfterMarketClose +=
        new OneHourAfterMarketCloseEventHandler(
        this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler );
    }
    
    public override string SaveScriptResults_CreateFileName()
    {
      return  DateTime.Now.Hour.ToString().PadLeft(2,'0') + "_" + 
							DateTime.Now.Minute.ToString().PadLeft(2,'0') + "_" + 
							"From_" + this.tickerGroupID + "_" +
	            + this.numberOfEligibleTickers +
	            "_OptDays" + this.numDaysForOptimizationPeriod + "_Port" +
	            this.numberOfTickersToBeChosen + "_GenNum" + 
	            this.generationNumberForGeneticOptimizer +
	            "_PopSize" + this.populationSizeForGeneticOptimizer +
      				"_GenomesScanned" + this.numOfGenomesForCTOScanning;
		}
    
    
    //necessary far calling RunEfficientPortfolio.Run()
    //in classes that inherit from this class
    public override void Run()
    {
      base.Run();
    }
	}
}
