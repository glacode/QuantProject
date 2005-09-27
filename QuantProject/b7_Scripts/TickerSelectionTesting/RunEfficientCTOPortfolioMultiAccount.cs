/*
QuantProject - Quantitative Finance Library

RunEfficientCTOPorfolioMultiAccount.cs
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
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors; 
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Presentation.Reporting.WindowsForm;


namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	
  /// <summary>
	/// Script to buy at open and sell at close 
	/// the efficient close to open daily portfolio
	/// The efficient portfolio's generation rules
	/// (contained in the EndOfDayTimerHandler) are:
	/// - choose the most liquid tickers;
	/// - choose the most efficient portfolio among these tickers
	/// </summary>
  [Serializable]
  public class RunEfficientCTOPorfolioMultiAccount : RunEfficientPortfolio
  {
    protected int numDaysBetweenEachOptimization;
    private int distanceBetweenEachGenomeToTest;
    private int numberOfAccounts;
    private Account[] accounts;
  
    public RunEfficientCTOPorfolioMultiAccount(string tickerGroupID, int numberOfEligibleTickers, 
      int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod, 
      int generationNumberForGeneticOptimizer,
      int populationSizeForGeneticOptimizer, string benchmark,
      DateTime startDate, DateTime endDate, double targetReturn,
      PortfolioType portfolioType, double maxRunningHours,
     	int numDaysBetweenEachOptimization, int numberOfAccounts, int distanceBetweenEachGenomeToTest):
      base(tickerGroupID, numberOfEligibleTickers, 
      numberOfTickersToBeChosen, numDaysForOptimizationPeriod, 
      generationNumberForGeneticOptimizer,
      populationSizeForGeneticOptimizer, benchmark,
      startDate, endDate, targetReturn,
      portfolioType, maxRunningHours)
    {
      this.ScriptName = "MultiAccountOpenCloseScripts";
      this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
      this.distanceBetweenEachGenomeToTest = distanceBetweenEachGenomeToTest;
      this.numberOfAccounts = numberOfAccounts;
      this.accounts = new Account[numberOfAccounts];
    }
    
    #region auxiliary overriden methods for Run
        
    
    protected override void run_initializeAccount()
    {
      for(int i = 0; i<this.accounts.Length; i++)
      {
        this.accounts[i] = new Account( this.ScriptName , this.endOfDayTimer ,
                          new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ,
                            this.historicalQuoteProvider ) ,
                          new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ,
                            this.historicalQuoteProvider ));
      }
     
    }
    
    
    protected override void run_initializeEndOfDayTimerHandler()
    {
      this.endOfDayTimerHandler = new EndOfDayTimerHandlerCTOMultiAccount(this.tickerGroupID,
        this.numberOfEligibleTickers,
        this.numberOfTickersToBeChosen,
        this.numDaysForOptimizationPeriod,
        this.accounts,
        this.generationNumberForGeneticOptimizer, 
        this.populationSizeForGeneticOptimizer,
        this.benchmark,
        this.targetReturn,
        this.portfolioType, this.numDaysBetweenEachOptimization,this.numberOfAccounts,
        this.distanceBetweenEachGenomeToTest);
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
    #endregion 
    
    //necessary far calling RunEfficientPortfolio.Run()
    //in classes that inherit from this class
    public override void Run()
    {
      base.Run();
    }
    public override void SaveScriptResults()
    {
      string fileName = "From"+this.numberOfEligibleTickers +
        "OptDays" + this.numDaysForOptimizationPeriod + "Portfolio" +
        this.numberOfTickersToBeChosen + "GenNum" + 
        this.generationNumberForGeneticOptimizer +
        "PopSize" + this.populationSizeForGeneticOptimizer +
        "Target" + Convert.ToString(this.targetReturn) + 
        Convert.ToString(this.portfolioType);
      string dirNameWhereToSaveAccounts = System.Configuration.ConfigurationSettings.AppSettings["AccountsArchive"] +
        "\\" + this.ScriptName + "\\";
      
      this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveAccounts);
      for(int i = 0; i<this.accounts.Length; i++)
        ObjectArchiver.Archive(accounts[i],
                                dirNameWhereToSaveAccounts +
                                fileName + "#" + i.ToString() + ".qPa");
      this.endOfDayTimer.Stop();
    }
	}
}
