/*
QuantProject - Quantitative Finance Library

RunEfficientCTCPorfolio.cs
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



namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	/// <summary>
	/// Script to buy at close and sell at close
	/// after a specified number of market days  
	/// the efficient portfolio
	/// The efficient portfolio's generation rules
	/// (contained in the EndOfDayTimerHandler) are:
	/// - choose the most liquid tickers;
	/// - choose only tickers quoted at each market day
	///   during a given previous interval of days;
	/// - choose the most efficient portfolio among these tickers
	/// </summary>
	[Serializable]
  public class RunEfficientCTCPorfolio : Script
	{
    
    private ReportTable reportTable;
    private EndOfDayDateTime startDateTime;
    private EndOfDayDateTime endDateTime;
    private int numIntervalDays;// number of days for the equity line graph
		private IHistoricalQuoteProvider historicalQuoteProvider =
			new HistoricalAdjustedQuoteProvider();


    //private ProgressBarForm progressBarForm;

    private EndOfDayTimerHandlerCTC endOfDayTimerHandler;

    private Account account;
		
    private IEndOfDayTimer endOfDayTimer;
		
    public RunEfficientCTCPorfolio()
		{
      //this.progressBarForm = new ProgressBarForm();
      this.reportTable = new ReportTable( "Summary_Reports" );
      this.startDateTime = new EndOfDayDateTime(
        new DateTime( 2003 , 1 , 1 ) , EndOfDaySpecificTime.MarketOpen );
      this.endDateTime = new EndOfDayDateTime(
        new DateTime( 2003 , 1 , 31 ) , EndOfDaySpecificTime.MarketClose );
      this.numIntervalDays = 3; //for report
		}
    #region Run
    
      
    

    private void run_initializeEndOfDayTimer()
    {
      this.endOfDayTimer =
        new IndexBasedEndOfDayTimer( this.startDateTime, "^MIBTEL" );
    }
    private void run_initializeAccount()
    {
      this.account = new Account( "EfficientCloseToClosePortfolio" , this.endOfDayTimer ,
        new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ,
					this.historicalQuoteProvider ) ,
        new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ,
					this.historicalQuoteProvider ) );
     
    }
    private void run_initializeEndOfDayTimerHandler()
    {
      this.endOfDayTimerHandler = new EndOfDayTimerHandlerCTC("STOCKMI",70,5,3,this.account,10);
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
      Report report;

      if(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime>=this.endDateTime.DateTime )
      {
        this.endOfDayTimer.Stop();
        report = new Report( this.account , this.historicalQuoteProvider );
        report.Show("CTC_Portfolio" , this.numIntervalDays , this.endDateTime , "^MIBTEL" );
        //ObjectArchiver.Archive(this.account, "CtcPortfolioAccount.qP","C:\\");

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
      
      //this.endOfDayTimer.OneHourAfterMarketClose +=
        //new OneHourAfterMarketCloseEventHandler(
        //this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler );
      //this.endOfDayTimer.OneHourAfterMarketClose +=
        //new OneHourAfterMarketCloseEventHandler(
        //this.oneHourAfterMarketCloseEventHandler );
      
      //this.progressBarForm.Show();
      this.endOfDayTimer.Start();
      
    }
    #endregion 
	}
}
