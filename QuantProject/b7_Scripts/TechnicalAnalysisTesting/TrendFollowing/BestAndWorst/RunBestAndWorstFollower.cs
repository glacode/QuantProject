/*
QuantProject - Quantitative Finance Library

RunBestAndWorstFollower.cs
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


namespace QuantProject.Scripts.TechnicalAnalysisTesting.TrendFollowing.BestAndWorst
{
	/// <summary>
	/// Script that implements the Best and Worst Follower strategy:
	/// every n closes, l long positions and s short positions will be open,
	/// buying and shorting, accordingly, the l best tickers
	/// and the s worst tickers.
	/// For chosing the best and worst tickers just the 
	/// previous close to close ratio is considered.
	/// The fundamental of the strategy should be the fact (to be verified ...) 
	/// that great moves are confirmed the next days 
	/// (just the reversal of simpleOHTest)
	
	/// At each close, open positions are closed.
	/// </summary>
  [Serializable]
  public class RunBestAndWorstFollower
  {
    private string scriptName;
    private string tickerGroupID;
    private string benchmark;
    private int numberOfEligibleTickers;
		private int lengthInDaysForPerformance;
    private int numOfBestTickers;
    private int numOfWorstTickers;
    private int numOfTickersForBuying;
    private int numOfTickersForShortSelling;
    private DateTime startDate;
    private DateTime endDate;
    private double maxRunningHours;
    private DateTime startingTimeForScript;
    private Account account;
    private IHistoricalQuoteProvider historicalQuoteProvider;
    private IEndOfDayTimer endOfDayTimer;

    public RunBestAndWorstFollower(string tickerGroupID, string benchmark,
      int numberOfEligibleTickers, int lengthInDaysForPerformance,
			int numOfBestTickers, 
      int numOfWorstTickers, int numOfTickersForBuying,
      int numOfTickersForShortSelling,
      DateTime startDate, DateTime endDate,
      double maxRunningHours)
    {
      this.tickerGroupID = tickerGroupID;
      this.benchmark = benchmark;
      this.numberOfEligibleTickers = numberOfEligibleTickers;
			this.lengthInDaysForPerformance = lengthInDaysForPerformance;
      this.numOfBestTickers = numOfBestTickers;
      this.numOfWorstTickers = numOfWorstTickers;
      this.numOfTickersForBuying = numOfTickersForBuying;
      this.numOfTickersForShortSelling = numOfTickersForShortSelling;
      this.startDate = startDate;
      this.endDate = endDate;
      this.maxRunningHours = maxRunningHours;
      this.scriptName = "BWFollower";
//      this.historicalQuoteProvider = new HistoricalRawQuoteProvider();
			this.historicalQuoteProvider = new HistoricalAdjustedQuoteProvider();
      this.endOfDayTimer = new IndexBasedEndOfDayTimer(
        new EndOfDayDateTime( this.startDate ,
        EndOfDaySpecificTime.MarketOpen ) , this.benchmark );
    }


    public void Run()
    {
      this.startingTimeForScript = DateTime.Now;
      this.account = new Account( "BestAndWorstFollower" , this.endOfDayTimer ,
        new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ,
        this.historicalQuoteProvider ) ,
        new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ,
        this.historicalQuoteProvider ) );
      EndOfDayTimerHandlerBWFollower endOfDayTimerHandler =
        new EndOfDayTimerHandlerBWFollower(this.tickerGroupID, this.numberOfEligibleTickers,
        this.lengthInDaysForPerformance, this.numOfBestTickers, this.numOfWorstTickers,
        this.numOfTickersForBuying, this.numOfTickersForShortSelling, 
        this.account, this.benchmark);
      
//			this.endOfDayTimer.MarketOpen += new MarketOpenEventHandler(
//        endOfDayTimerHandler.MarketOpenEventHandler );

      this.endOfDayTimer.MarketClose += new MarketCloseEventHandler(
        endOfDayTimerHandler.MarketCloseEventHandler );

      this.endOfDayTimer.MarketClose += new MarketCloseEventHandler(
        this.checkDateForReport);

//      this.endOfDayTimer.OneHourAfterMarketClose += new OneHourAfterMarketCloseEventHandler(
//        endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler );
      
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
      	"_best_" + this.numOfBestTickers +
      	"_worst_" + this.numOfWorstTickers +
      	"_Long_" + this.numOfTickersForBuying +
      	"_Short_" + this.numOfTickersForShortSelling +
				"_lenInDays_" + this.lengthInDaysForPerformance;
      string dirNameWhereToSaveReports = System.Configuration.ConfigurationSettings.AppSettings["ReportsArchive"] +
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
