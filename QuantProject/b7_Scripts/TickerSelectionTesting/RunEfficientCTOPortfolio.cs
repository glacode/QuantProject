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
	public class RunEfficientCTOPorfolio : Script
	{
    //DateTime lastDate = DateTime.Now.Date;
		//DateTime firstDate = DateTime.Now.Date.AddDays(-60);
    //these two members are used by the old script
    DateTime lastDate = new DateTime(2004,11,25);
    DateTime firstDate = new DateTime(2004,9,25);
    //
    
    private ReportTable reportTable;
    private EndOfDayDateTime startDateTime;
    private EndOfDayDateTime endDateTime;
    private int numIntervalDays;// number of days for the equity line graph

    //private ProgressBarForm progressBarForm;

    private EndOfDayTimerHandler endOfDayTimerHandler;

    private Account account;
		
    private IEndOfDayTimer endOfDayTimer;
		
    public RunEfficientCTOPorfolio()
		{
      //this.progressBarForm = new ProgressBarForm();
      this.reportTable = new ReportTable( "Summary_Reports" );
      this.startDateTime = new EndOfDayDateTime(
        new DateTime( 2002 , 1 , 1 ) , EndOfDaySpecificTime.FiveMinutesBeforeMarketClose );
      this.endDateTime = new EndOfDayDateTime(
        new DateTime( 2002 , 3 , 31 ) , EndOfDaySpecificTime.OneHourAfterMarketClose );
      this.numIntervalDays = 7;
		}
    #region Run
    
      
    private void run_FindBestPortfolioForNextTrade()
    {
      //"STOCKMI"
      TickerSelector mostLiquid = new TickerSelector(SelectionType.Liquidity,
                                                    false, "STOCKMI", firstDate, lastDate, 70);
      DataTable tickers = mostLiquid.GetTableOfSelectedTickers();
 	    IGenomeManager genManEfficientCTOPortfolio = 
                   new GenomeManagerForEfficientCTOPortfolio(tickers,firstDate,
                                                              lastDate, 6, 0.005, 0.05);
      GeneticOptimizer GO = new GeneticOptimizer(genManEfficientCTOPortfolio);
      //GO.KeepOnRunningUntilConvergenceIsReached = true;
      GO.GenerationNumber = 7;
      GO.MutationRate = 0.05;
      GO.Run(true);
      //it has to be changed the decode implementation for this IGenomeManager
      System.Console.WriteLine("\n\nThe best solution found is: " + (string)GO.BestGenome.Meaning +
        " with {0} generations", GO.GenerationCounter);
		}

    private void run_initializeEndOfDayTimer()
    {
      this.endOfDayTimer =
        new HistoricalEndOfDayTimer( this.startDateTime );
    }
    private void run_initializeAccount()
    {
      this.account = new Account( "EfficientCloseToOpenPortfolio" , this.endOfDayTimer ,
        new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ) ,
        new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ) );
     
    }
    private void run_initializeEndOfDayTimerHandler()
    {
      this.endOfDayTimerHandler = new EndOfDayTimerHandler(70,6,this.account );
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

    public override void Run()
    {
      //old script
      //this.run_FindBestPortfolioForNextTrade();
      
      Report report;
      run_initializeEndOfDayTimer();
      run_initializeAccount();
      run_initializeEndOfDayTimerHandler();
      //run_initializeProgressHandlers();
      this.endOfDayTimer.MarketOpen +=
        new MarketOpenEventHandler(
        this.endOfDayTimerHandler.MarketOpenEventHandler);  
      
      this.endOfDayTimer.FiveMinutesBeforeMarketClose +=
        new FiveMinutesBeforeMarketCloseEventHandler(
        this.endOfDayTimerHandler.FiveMinutesBeforeMarketCloseEventHandler );

      this.endOfDayTimer.OneHourAfterMarketClose +=
        new OneHourAfterMarketCloseEventHandler(
        this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler );
      //this.endOfDayTimer.OneHourAfterMarketClose +=
        //new OneHourAfterMarketCloseEventHandler(
        //this.oneHourAfterMarketCloseEventHandler );
      
      //this.progressBarForm.Show();
      this.endOfDayTimer.Start();
      report = new Report( this.account );
      report.Show("CTO_Portfolio" , this.numIntervalDays , this.startDateTime , "CTO_Portfolio" );

      
    }
    #endregion 
	}
}
