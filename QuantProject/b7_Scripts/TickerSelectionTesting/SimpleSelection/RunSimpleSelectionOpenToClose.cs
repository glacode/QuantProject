/*
QuantProject - Quantitative Finance Library

RunSimpleSelectionOpenToClose.cs
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


namespace QuantProject.Scripts.TickerSelectionTesting.SimpleSelection
{
	/// <summary>
	/// Script to buy at open and sell at close (within the same day)
	/// the tickers with the highest fitness, for short or long trade
	/// </summary>
	[Serializable]
	public class RunSimpleSelectionOpenToClose : RunSimpleSelection
	{
		protected int numDaysBetweenEachOptimization;
		public RunSimpleSelectionOpenToClose(string tickerGroupID, int numberOfEligibleTickers,
		                                     int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
		                                     string benchmark,
		                                     DateTime startDate, DateTime endDate, double targetReturn,
		                                     PortfolioType portfolioType, double maxRunningHours,
		                                     int numDaysBetweenEachOptimization):
			base(tickerGroupID, numberOfEligibleTickers,
			     numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
			     benchmark,
			     startDate, endDate, targetReturn,
			     portfolioType, maxRunningHours)
		{
			this.ScriptName = "SimpleSelectionOpenToCloseSharpeRatio";
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
		}
		
		#region auxiliary overriden methods for Run
		
		/* delete remark delimitations for having ib commission
    protected override void run_initializeAccount()
    {
      this.account = new Account( this.ScriptName , this.endOfDayTimer ,
        new HistoricalDataStreamer( this.endOfDayTimer ,
          this.historicalQuoteProvider ) ,
        new HistoricalOrderExecutor( this.endOfDayTimer ,
          this.historicalQuoteProvider ), new IBCommissionManager());
     
    }
		 */
		
		protected override void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler =
				new EndOfDayTimerHandlerSimpleSelectionOpenToClose(this.tickerGroupID,
				                                                   this.numberOfEligibleTickers,
				                                                   this.numberOfTickersToBeChosen,
				                                                   this.numDaysForOptimizationPeriod,
				                                                   this.account,
				                                                   this.benchmark,
				                                                   this.targetReturn,
				                                                   this.portfolioType, this.numDaysBetweenEachOptimization);
		}
		
		protected override void run_initializeHistoricalQuoteProvider()
		{
			this.historicalMarketValueProvider = new HistoricalRawQuoteProvider();
			//this.historicalQuoteProvider = new HistoricalAdjustedQuoteProvider();
		}
		
		private void newDateTimeEventHandler( object sender , DateTime dateTime )
		{
			if ( HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
				this.checkDateForReport( sender , dateTime );
		}

		protected override void run_addEventHandlers()
		{
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.endOfDayTimerHandler.NewDateTimeEventHandler );
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.newDateTimeEventHandler );

//			this.endOfDayTimer.MarketOpen +=
			//        new MarketOpenEventHandler(
			//        this.endOfDayTimerHandler.MarketOpenEventHandler);
//
			//      this.endOfDayTimer.MarketClose +=
			//        new MarketCloseEventHandler(
			//        this.endOfDayTimerHandler.MarketCloseEventHandler);
//
			//      this.endOfDayTimer.MarketClose +=
			//        new MarketCloseEventHandler(
			//        this.checkDateForReport);
//
			//      this.endOfDayTimer.OneHourAfterMarketClose +=
			//        new OneHourAfterMarketCloseEventHandler(
			//        this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler );
		}
		#endregion
		
		public override void SaveScriptResults()
		{
			string fileName = "SimpleOTCDailySelectionFrom"+this.numberOfEligibleTickers +
				"OptDays" + this.numDaysForOptimizationPeriod + "Portfolio" +
				this.numberOfTickersToBeChosen +
				"Target" + Convert.ToString(this.targetReturn) +
				Convert.ToString(this.portfolioType);
			string dirNameWhereToSaveReports =
				System.Configuration.ConfigurationManager.AppSettings["ReportsArchive"] +
				"\\" + this.ScriptName + "\\";
			string dirNameWhereToSaveTransactions =
				System.Configuration.ConfigurationManager.AppSettings["TransactionsArchive"] +
				"\\" + this.ScriptName + "\\";

			//default report with numIntervalDays = 1
			AccountReport accountReport = this.account.CreateReport(fileName,1,
			                                                        this.endOfDayTimer.GetCurrentDateTime(),
			                                                        this.benchmark,
			                                                        new HistoricalAdjustedQuoteProvider());
			this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveReports);
			ObjectArchiver.Archive(accountReport,
			                       dirNameWhereToSaveReports +
			                       fileName + ".qPr");
			//
			this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveTransactions);
			ObjectArchiver.Archive(this.account.Transactions,
			                       dirNameWhereToSaveTransactions +
			                       fileName + ".qPt");
			//
			this.endOfDayTimer.Stop();
		}
		
		
		//necessary far calling RunEfficientPortfolio.Run()
		//in classes that inherit from this class
		public override void Run()
		{
			base.Run();
		}
	}
}
