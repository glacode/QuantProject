/*
QuantProject - Quantitative Finance Library

RunSimplePairTradingIS.cs
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

using QuantProject.ADT.FileManaging;
using QuantProject.Data.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Timing;
using QuantProject.Business.Financial.Accounting.Commissions;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.DataProviders;
using QuantProject.Presentation.Reporting.WindowsForm;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.ArbitrageTesting.PairTrading.SimplePairTrading;


namespace QuantProject.Scripts.ArbitrageTesting.PairTrading.SimplePairTrading.InSample
{
	
  /// <summary>
	/// Script to test the pair trading strategy for
	/// two tickers for a given time period
	/// </summary>
  [Serializable]
  public class RunSimplePairTradingIS
  {
    private double maxNumOfStdDevForNormalGap;
    private int numDaysForGap;
    private double averageGap;
    private double stdDevGap;
    private EndOfDayTimerHandlerSimplePT endOfDayTimerHandler;
    private string firstTicker;
    private string secondTicker;
    private DateTime startDateTime;
    private DateTime endDateTime;
    private IHistoricalQuoteProvider historicalQuoteProvider;
    private Account account;
    private IEndOfDayTimer endOfDayTimer;
    private string benchmark;
    private string scriptName;

    public RunSimplePairTradingIS(double maxNumOfStdDevForNormalGap,
                                  int numDaysForGap,
                                  double averageGap,
                                  double stdDevGap,
                                  string firstTicker, string secondTicker,
                                  DateTime startDate, DateTime endDate)
                                  
    {
      this.maxNumOfStdDevForNormalGap = maxNumOfStdDevForNormalGap;
      this.numDaysForGap = numDaysForGap;
      this.averageGap = averageGap;
      this.stdDevGap = stdDevGap;
      this.firstTicker = firstTicker;
      this.secondTicker = secondTicker;
      this.startDateTime = startDate;
      this.endDateTime = endDate;
      this.benchmark = "^GSPC";
      this.scriptName = "SimplePairTradingForGivenTickers";
    }
    
    #region Run
        
    private void run_initializeEndOfDayTimer()
    {
      //default endOfDayTimer
      this.endOfDayTimer =
        new IndexBasedEndOfDayTimer( new EndOfDayDateTime(this.startDateTime,
                                                          EndOfDaySpecificTime.MarketOpen),
                                     this.benchmark );
    	
    }
    
    protected void run_initializeAccount()
    {
      //default account with no commissions and no slippage calculation
      this.account = new Account( this.scriptName , this.endOfDayTimer ,
        new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ,
        this.historicalQuoteProvider ) ,
        new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ,
        this.historicalQuoteProvider ));
     
    }
        
    private void run_initializeHistoricalQuoteProvider()
    {
      this.historicalQuoteProvider = new HistoricalAdjustedQuoteProvider();
    }
    
        
    private void checkDateForReport(Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs)
    {
      if(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime>=this.endDateTime)
        //last date is reached by the timer
        this.showReport();
    }
    
    private void showReport()
    {
      AccountReport accountReport = this.account.CreateReport(this.scriptName, 1,
                                        this.endOfDayTimer.GetCurrentTime(),
                                        this.benchmark,
                                        new HistoricalAdjustedQuoteProvider());
      Report report = new Report(accountReport);
      report.Show();
      this.endOfDayTimer.Stop();
       
    }
    
    private void run_initialize()
    {
      run_initializeHistoricalQuoteProvider();
      run_initializeEndOfDayTimer();
      run_initializeAccount();
      run_initializeEndOfDayTimerHandler();
    }
    
    
    public void Run()
    {
      this.run_initialize();
      this.run_addEventHandlers();
      this.endOfDayTimer.Start();
    }
    
    private void run_initializeEndOfDayTimerHandler()
    {
      this.endOfDayTimerHandler = new EndOfDayTimerHandlerSimplePTIS(this.maxNumOfStdDevForNormalGap,
                                                                     this.numDaysForGap,
                                                                     this.averageGap,
                                                                     this.stdDevGap,
                                                                     this.firstTicker, this.secondTicker,
                                                                     this.startDateTime, this.endDateTime,
                                                                     this.account);
    }
    
    
    private void run_addEventHandlers()
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
    
	}
}
