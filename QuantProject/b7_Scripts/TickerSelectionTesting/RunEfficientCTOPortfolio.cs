/*
QuantProject - Quantitative Finance Library

RunEfficientCTOPorfolio.cs
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
  public class RunEfficientCTOPorfolio : Script
	{
    //DateTime lastDate = DateTime.Now.Date;
		//DateTime firstDate = DateTime.Now.Date.AddDays(-60);
    //these two members are used by the old script
    DateTime lastDate = new DateTime(2004,11,25);
    DateTime firstDate = new DateTime(2004,9,25);
    //
    private string tickerGroupID;
    private int numberOfEligibleTickers;
    private int numberOfTickersToBeChosen;
    private int numDaysForLiquidity;
    private int generationNumberForGeneticOptimizer;
    private int populationSizeForGeneticOptimizer;

    private ReportTable reportTable;
    private EndOfDayDateTime startDateTime;
    private EndOfDayDateTime endDateTime;
    //private int numIntervalDays;// number of days for the equity line graph
		private IHistoricalQuoteProvider historicalQuoteProvider =
			new HistoricalRawQuoteProvider();


    //private ProgressBarForm progressBarForm;

    private EndOfDayTimerHandlerCTO endOfDayTimerHandler;

    private Account account;
		
    private IEndOfDayTimer endOfDayTimer;
		
    public RunEfficientCTOPorfolio(string tickerGroupID, int numberOfEligibleTickers, 
                                    int numberOfTickersToBeChosen, int numDaysForLiquidity, 
                                    int generationNumberForGeneticOptimizer,
                                    int populationSizeForGeneticOptimizer)
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
        new DateTime( 2004 , 9 , 1 ) , EndOfDaySpecificTime.FiveMinutesBeforeMarketClose );
      this.endDateTime = new EndOfDayDateTime(
        new DateTime( 2004 , 9 , 10 ) , EndOfDaySpecificTime.OneHourAfterMarketClose );
      //this.numIntervalDays = 3;
		}
    #region Run
    
      
    private void run_FindBestPortfolioForNextTrade()
    {
      //"STOCKMI"
      /*
       * SelectorByLiquidity mostLiquid = new TickerSelector(SelectionType.Liquidity,
                                                    false, "STOCKMI", firstDate, lastDate, 70);
      DataTable tickers = mostLiquid.GetTableOfSelectedTickers();
 	    IGenomeManager genManEfficientCTOPortfolio = 
                   new GenomeManagerForEfficientCTOPortfolio(tickers,firstDate,
                                                              lastDate, 6,1, 0.005);
      GeneticOptimizer GO = new GeneticOptimizer(genManEfficientCTOPortfolio);
      //GO.KeepOnRunningUntilConvergenceIsReached = true;
      GO.GenerationNumber = 10;
      GO.MutationRate = 0.05;
      GO.Run(true);
      //it has to be changed the decode implementation for this IGenomeManager
      System.Console.WriteLine("\n\nThe best solution found is: " + (string)GO.BestGenome.Meaning +
        " with {0} generations", GO.GenerationCounter);
        */
      ;
		}

    private void run_initializeEndOfDayTimer()
    {
      this.endOfDayTimer =
        new IndexBasedEndOfDayTimer( this.startDateTime, "^MIBTEL" );
    }
    private void run_initializeAccount()
    {
      this.account = new Account( "EfficientCloseToOpenPortfolio" , this.endOfDayTimer ,
        new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ,
					this.historicalQuoteProvider ) ,
        new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ,
					this.historicalQuoteProvider ));
     
    }
    private void run_initializeEndOfDayTimerHandler()
    {
      this.endOfDayTimerHandler = new EndOfDayTimerHandlerCTO(this.tickerGroupID,
                                                              this.numberOfEligibleTickers,
                                                              this.numberOfTickersToBeChosen,
                                                              this.numDaysForLiquidity,
                                                              this.account,
                                                              this.generationNumberForGeneticOptimizer, 
                                                              this.populationSizeForGeneticOptimizer);
        
        
    }
    /*
    private  void inSampleNewProgressEventHandler(
      Object sender , NewProgressEventArgs eventArgs )
    {
      this.progressBarForm.ProgressBarInSample.Value = eventArgs.CurrentProgress;
      this.progressBarForm.ProgressBarInSample.Refresh();
    }
    private void run_initializeProgressHandlers()
    {
      this.endOfDayTimerHandler.InSampleNewProgress +=
        new InSampleNewProgressEventHandler( this.inSampleNewProgressEventHandler );
    }
    */
		#region oneHourAfterMarketCloseEventHandler
    /*
    private void oneHourAfterMarketCloseEventHandler_handleProgessBarForm(
      IEndOfDayTimer endOfDayTimer )
    {
      long elapsedDays = Convert.ToInt64( ((TimeSpan)( endOfDayTimer.GetCurrentTime().DateTime - 
        this.startDateTime.DateTime )).TotalDays );
      long totalDays = Convert.ToInt64( ((TimeSpan)( this.endDateTime.DateTime - 
        this.startDateTime.DateTime )).TotalDays );
      if ( Math.Floor( elapsedDays / totalDays * 100 ) >
        Math.Floor( ( elapsedDays - 1 ) / totalDays * 100 ) )
        // a new out of sample time percentage point has been elapsed
        this.progressBarForm.ProgressBarOutOfSample.Value =
          Convert.ToInt16( Math.Floor( elapsedDays / totalDays * 100 ) );
    }
    public void oneHourAfterMarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      this.oneHourAfterMarketCloseEventHandler_handleProgessBarForm(
        ( IEndOfDayTimer )sender );
      if ( ( ( IEndOfDayTimer )sender ).GetCurrentTime().DateTime >
        this.endDateTime.DateTime )
      {
        // the simulation has reached the ending date
        this.account.EndOfDayTimer.Stop();
        this.progressBarForm.Close();
      }
    }
    */
		#endregion
    
    private void checkDateForReport(Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs)
    {
      //Report report;

      if(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime>=this.endDateTime.DateTime )
      {
        this.endOfDayTimer.Stop();
        //report = new Report( this.account , this.historicalQuoteProvider );
        //report.Show("CTO_Portfolio" , this.numIntervalDays , this.endDateTime , "^MIBTEL" );
        string name = "From"+this.numberOfEligibleTickers +
                      "LiqDays" + this.numDaysForLiquidity + "Portfolio" +
                      this.numberOfTickersToBeChosen + "GenNum" + 
                      this.generationNumberForGeneticOptimizer +
                      "PopSize" + this.populationSizeForGeneticOptimizer;
        AccountReport accountReport = this.account.CreateReport(name,1,this.endDateTime,"^MIBTEL",
                                                              new HistoricalAdjustedQuoteProvider());
        ObjectArchiver.Archive(accountReport,
                              "C:\\Documents and Settings\\Marco\\Documenti\\ProgettiOpenSource\\Quant\\SavedReports\\OpenCloseScripts\\" +
                               name + ".rep");
        
        //ObjectArchiver.Archive(this.account,
        //                       "C:\\Documents and Settings\\Marco\\Documenti\\ProgettiOpenSource\\Quant\\SavedAccounts\\OpenCloseScripts\\" +
        //                       name + ".acc");

      }
    }
    public override void Run()
    {
      //old script
      //this.run_FindBestPortfolioForNextTrade();
      
      run_initializeEndOfDayTimer();
      run_initializeAccount();
      run_initializeEndOfDayTimerHandler();
      //run_initializeProgressHandlers();
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
      //this.endOfDayTimer.OneHourAfterMarketClose +=
        //new OneHourAfterMarketCloseEventHandler(
        //this.oneHourAfterMarketCloseEventHandler );
      
      //this.progressBarForm.Show();
      this.endOfDayTimer.Start();
      
    }
    #endregion 
	}
}
