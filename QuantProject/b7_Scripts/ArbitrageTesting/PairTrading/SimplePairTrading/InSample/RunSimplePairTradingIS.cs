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
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		private Account account;
		private Timer timer;
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
			this.timer =
				new IndexBasedEndOfDayTimer(
					HistoricalEndOfDayTimer.GetMarketOpen( this.startDateTime ) ,
					//      		new EndOfDayDateTime(this.startDateTime,
					//                                                          EndOfDaySpecificTime.MarketOpen),
					this.benchmark );
			
		}
		
		protected void run_initializeAccount()
		{
			//default account with no commissions and no slippage calculation
			this.account = new Account( this.scriptName , this.timer ,
			                           new HistoricalDataStreamer( this.timer ,
			                                                              this.historicalMarketValueProvider ) ,
			                           new HistoricalOrderExecutor( this.timer ,
			                                                               this.historicalMarketValueProvider ));
			
		}
		
		private void run_initializeHistoricalQuoteProvider()
		{
			this.historicalMarketValueProvider = new HistoricalAdjustedQuoteProvider();
		}
		
		
		private void checkDateForReport( Object sender , DateTime dateTime )
		{
			if(dateTime>=this.endDateTime)
				//last date is reached by the timer
				this.showReport();
		}
		
		private void showReport()
		{
			AccountReport accountReport = this.account.CreateReport(this.scriptName, 1,
			                                                        this.timer.GetCurrentDateTime(),
			                                                        this.benchmark,
			                                                        new HistoricalAdjustedQuoteProvider());
			Report report = new Report(accountReport);
			report.Show();
			this.timer.Stop();
			
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
			this.timer.Start();
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
			this.timer.NewDateTime +=
				new NewDateTimeEventHandler( this.endOfDayTimerHandler.NewDateTimeEventHandler );
			this.timer.NewDateTime +=
				new NewDateTimeEventHandler( this.checkDateForReport );

//			this.timer.MarketOpen +=
//				new MarketOpenEventHandler(
//					this.endOfDayTimerHandler.MarketOpenEventHandler);
//			
//			this.timer.MarketClose +=
//				new MarketCloseEventHandler(
//					this.endOfDayTimerHandler.MarketCloseEventHandler);
//			
//			this.timer.MarketClose +=
//				new MarketCloseEventHandler(
//					this.checkDateForReport);
//			
//			this.timer.OneHourAfterMarketClose +=
//				new OneHourAfterMarketCloseEventHandler(
//					this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler );
		}
		#endregion
		
	}
}
