/*
QuantProject - Quantitative Finance Library

RunSimpleOHTest.cs
Copyright (C) 2007
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
using QuantProject.ADT.FileManaging;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Presentation.Reporting.WindowsForm;


namespace QuantProject.Scripts.ArbitrageTesting.OverReactionHypothesis.SimpleOHTest
{
	/// <summary>
	/// Script that implements the SimpleOHTest strategy:
	/// at each open, l long positions and s short positions will be open,
	/// buying and shorting, accordingly, the l worst tickers 
	/// and the s best tickers.
	/// For chosing the best and worst tickers just the 
	/// previous close to close ratio is considered.
	/// The fundamental of the strategy should be the fact (to be verified ...) 
	/// that great moves are probably overreactions that may be followed by opposite moves (corrections).
	
	/// At each close, open positions are closed.
	/// </summary>
  [Serializable]
  public class RunSimpleOHTest
  {
    private string scriptName;
    private string tickerGroupID;
    private string benchmark;
    private int numberOfEligibleTickers;
		private int lengthInDaysForPerformance;
    private int numOfTickersForBuying;
    private int numOfTickersForShortSelling;
    private DateTime startDate;
    private DateTime endDate;
    private double maxRunningHours;
    private DateTime startingTimeForScript;
    private Account account;
    private IHistoricalQuoteProvider historicalQuoteProvider;
    private IEndOfDayTimer endOfDayTimer;

    public RunSimpleOHTest(string tickerGroupID, string benchmark,
      int numberOfEligibleTickers, int lengthInDaysForPerformance,
			int numOfTickersForBuying,
      int numOfTickersForShortSelling,
      DateTime startDate, DateTime endDate,
      double maxRunningHours)
    {
      this.tickerGroupID = tickerGroupID;
      this.benchmark = benchmark;
      this.numberOfEligibleTickers = numberOfEligibleTickers;
			this.lengthInDaysForPerformance = lengthInDaysForPerformance;
      this.numOfTickersForBuying = numOfTickersForBuying;
      this.numOfTickersForShortSelling = numOfTickersForShortSelling;
      this.startDate = startDate;
      this.endDate = endDate;
      this.maxRunningHours = maxRunningHours;
      this.scriptName = "SimpleOHTest";
//      this.historicalQuoteProvider = new HistoricalRawQuoteProvider();
			this.historicalQuoteProvider = new HistoricalAdjustedQuoteProvider();
      this.endOfDayTimer = new IndexBasedEndOfDayTimer(
        new EndOfDayDateTime( this.startDate ,
        EndOfDaySpecificTime.MarketOpen ) , this.benchmark );
    }


    public void Run()
    {
      this.startingTimeForScript = DateTime.Now;
      
			this.account = new Account( "SimpleOH" , this.endOfDayTimer ,
        new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ,
        this.historicalQuoteProvider ) ,
        new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ,
        this.historicalQuoteProvider ) );
      
			EndOfDayTimerHandlerSimpleOHTest endOfDayTimerHandler =
        new EndOfDayTimerHandlerSimpleOHTest(this.tickerGroupID, this.numberOfEligibleTickers,
        this.lengthInDaysForPerformance, this.numOfTickersForBuying, this.numOfTickersForShortSelling, 
        this.account, this.benchmark);
 
      this.endOfDayTimer.MarketClose += new MarketCloseEventHandler(
        endOfDayTimerHandler.MarketCloseEventHandler );

      this.endOfDayTimer.MarketClose += new MarketCloseEventHandler(
        this.checkDateForReport);
      
      this.endOfDayTimer.Start();
    }

    private void checkDateForReport(Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs)
    {
      if(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime>=this.endDate ||
        DateTime.Now >= this.startingTimeForScript.AddHours(this.maxRunningHours))
        //last date is reached by the timer or maxRunning hours
        //are elapsed from the time script started
        this.SaveScriptResults();
    }

    private void checkDateForReport_createDirIfNotPresent(string dirPath)
    {
      if(!Directory.Exists(dirPath))
        Directory.CreateDirectory(dirPath);
    }

    public void SaveScriptResults()
    {
      TimeSpan span;
			span = DateTime.Now.Subtract(this.startingTimeForScript);
			int secondsElapsed = span.Hours * 3600 + span.Minutes * 60 + span.Seconds;
			string fileName = 
				"SecondsElapsed_" + 
				secondsElapsed.ToString() + "_" +
				DateTime.Now.Hour.ToString().PadLeft(2,'0') + "_" + 
        DateTime.Now.Minute.ToString().PadLeft(2,'0') + "_" +
        this.scriptName +  "_From_" + this.tickerGroupID +
        "_elig_" + this.numberOfEligibleTickers + 
      	"_Long_" + this.numOfTickersForBuying +
      	"_Short_" + this.numOfTickersForShortSelling +
				"_lenInDays_" + this.lengthInDaysForPerformance;
      string dirNameWhereToSaveReports =
      	System.Configuration.ConfigurationManager.AppSettings["ReportsArchive"] +
        "\\" + this.scriptName + "\\";
          
      //default report with numIntervalDays = 1
      AccountReport accountReport = this.account.CreateReport(fileName,1,
        this.endOfDayTimer.GetCurrentTime(),
        this.benchmark,
//        new HistoricalRawQuoteProvider() );
        new HistoricalAdjustedQuoteProvider());
      this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveReports);
      ObjectArchiver.Archive(accountReport,
        dirNameWhereToSaveReports + 
        fileName + ".qPr");
            
      this.endOfDayTimer.Stop();
    }
  }   
}
