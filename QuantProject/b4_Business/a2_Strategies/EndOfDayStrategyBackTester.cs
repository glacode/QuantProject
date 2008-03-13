/*
QuantProject - Quantitative Finance Library

EndOfDayStrategy.cs
Copyright (C) 2007
Glauco Siliprandi

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

using QuantProject.ADT;
using QuantProject.ADT.FileManaging;
using QuantProject.ADT.Messaging;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using	QuantProject.Business.Financial.Accounting.AccountProviding;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Timing;

namespace QuantProject.Business.Strategies
{
	/// <summary>
	/// Performs a backtest for an end of day strategy
	/// </summary>
	public class EndOfDayStrategyBackTester : IMessageSender , ILogDescriptor
	{
		public event NewMessageEventHandler NewMessage;

		private string backTestID;
		private IEndOfDayStrategyForBacktester endOfDayStrategy;
		private IHistoricalQuoteProvider historicalQuoteProvider;
		private DateTime firstDateTime;
		private DateTime lastDateTime;
		private Benchmark benchmark;
		private double cashToStart;
		private double maxRunningHours;
		private IAccountProvider accountProvider;
		private DateTime startingTimeForScript;
		private IEndOfDayTimer endOfDayTimer;
		private DateTime actualLastDateTime;
		private Account account;
		private AccountReport accountReport;
		private BackTestLog backTestLog;
		private DateTime realDateTimeWhenTheBackTestIsStopped;
		
		public IHistoricalQuoteProvider HistoricalQuoteProvider
		{
			get { return this.historicalQuoteProvider; }
		}
		public Benchmark Benchmark
		{
			get { return this.benchmark; }
		}
		/// <summary>
		/// Returns the dimulated DateTime when the backtest is stopped
		/// (not the real time)
		/// </summary>
		public DateTime ActualLastDateTime
		{
			get
			{
				this.checkThisPropertyRequiresBacktestIsCompleted();
				return this.actualLastDateTime;
			}
		}
		public Account Account
		{
			get
			{
				return this.account;
			}
		}
		/// <summary>
		/// Once the backtest is completed, this property returns the
		/// AccountReport for the internal Account
		/// </summary>
		public AccountReport AccountReport
		{
			get
			{
				this.checkThisPropertyRequiresBacktestIsCompleted();
				return this.accountReport;
			}
		}
		public BackTestLog Log
		{
			get { return this.backTestLog; }
		}
		/// <summary>
		/// The timer used by the backtester, to simulate the time ticking
		/// </summary>
		public IEndOfDayTimer EndOfDayTimer
		{
			get
			{
				return this.endOfDayTimer;
			}
		}
		public string Description
		{
			get
			{
				string description =
					ExtendedDateTime.GetCompleteShortDescriptionForFileName(
					this.realDateTimeWhenTheBackTestIsStopped ) + "_" +
					"from_" +
					ExtendedDateTime.GetShortDescriptionForFileName( this.firstDateTime ) +
					"_to_" +
					ExtendedDateTime.GetShortDescriptionForFileName( this.actualLastDateTime ) +
					"_annlRtrn_" + this.AccountReport.Summary.AnnualSystemPercentageReturn.FormattedValue +
					"_maxDD_" + this.AccountReport.Summary.MaxEquityDrawDown.FormattedValue +
					"_" + this.historicalQuoteProvider.Description +
					"_" + this.endOfDayStrategy.Description;
				return description.Substring( 0 , Math.Min( description.Length , 200 ) );
			}
		}

		private void checkThisPropertyRequiresBacktestIsCompleted()
		{
			if ( this.actualLastDateTime == DateTime.MinValue )
				// the timer has not been stopped yet
				throw new Exception( "This property cannot be invoked " +
					"while the backtest is still running!" );
		}

		public EndOfDayStrategyBackTester( string backTestID ,
			IEndOfDayStrategyForBacktester endOfDayStrategy ,
			IHistoricalQuoteProvider historicalQuoteProvider ,
			IAccountProvider accountProvider,
			DateTime firstDateTime , DateTime lastDateTime ,
			Benchmark benchmark ,
			double cashToStart ,
			double maxRunningHours )
		{
			this.backTestID = backTestID;
			this.endOfDayStrategy = endOfDayStrategy;
			this.historicalQuoteProvider = historicalQuoteProvider;
			this.accountProvider = accountProvider;
			this.firstDateTime = firstDateTime;
			this.lastDateTime = lastDateTime;
			this.benchmark = benchmark;
			this.cashToStart = cashToStart;
			this.maxRunningHours = maxRunningHours;

      this.initialize_endOfDayTimer();
			this.account = this.accountProvider.GetAccount(this.endOfDayTimer,
											this.historicalQuoteProvider);
			this.endOfDayStrategy.Account = this.account;
			this.backTestLog = new BackTestLog( backTestID , firstDateTime ,
				lastDateTime , benchmark );
			this.actualLastDateTime = DateTime.MinValue;
			this.realDateTimeWhenTheBackTestIsStopped = DateTime.MinValue;
		}

		private void initialize_endOfDayTimer()
		{
			EndOfDayDateTime endOfDayDateTime =
				new EndOfDayDateTime( this.firstDateTime ,
				EndOfDaySpecificTime.MarketOpen );
			this.endOfDayTimer =
				new IndexBasedEndOfDayTimer( endOfDayDateTime ,
				this.benchmark.Ticker );
		}
		
		#region Run
		#region run_addEventHandlers
		private void handlerToAddCashToStart(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( this.account.Transactions.Count == 0 )
				// no cash has been added yet
				this.account.AddCash( this.cashToStart );
		}
		private void run_addEventHandlers_addHandlersToAddCashToStart()
		{
			this.endOfDayTimer.MarketOpen +=
				new MarketOpenEventHandler(
				this.handlerToAddCashToStart );
			this.endOfDayTimer.FiveMinutesBeforeMarketClose +=
				new FiveMinutesBeforeMarketCloseEventHandler(
				this.handlerToAddCashToStart );
			this.endOfDayTimer.MarketClose +=
				new MarketCloseEventHandler(
				this.handlerToAddCashToStart );
			this.endOfDayTimer.OneHourAfterMarketClose +=
				new OneHourAfterMarketCloseEventHandler(
				this.handlerToAddCashToStart );
		}
		private void run_addEventHandlers_addStrategyHandlers()
		{
			this.endOfDayTimer.MarketOpen +=
				new MarketOpenEventHandler(
				this.endOfDayStrategy.MarketOpenEventHandler );
			this.endOfDayTimer.FiveMinutesBeforeMarketClose +=
				new FiveMinutesBeforeMarketCloseEventHandler(
				this.endOfDayStrategy.FiveMinutesBeforeMarketCloseEventHandler );
			this.endOfDayTimer.OneHourAfterMarketClose +=
				new OneHourAfterMarketCloseEventHandler(
				this.endOfDayStrategy.OneHourAfterMarketCloseEventHandler );
			this.endOfDayTimer.MarketClose +=
				new MarketCloseEventHandler(
				this.endOfDayStrategy.MarketCloseEventHandler );
		}
		private void run_addEventHandlers()
		{
			this.run_addEventHandlers_addHandlersToAddCashToStart();
			this.run_addEventHandlers_addStrategyHandlers();
			this.endOfDayTimer.MarketClose +=
				new MarketCloseEventHandler(
				this.marketCloseEventHandler );
			this.endOfDayStrategy.NewLogItem +=
				new NewLogItemEventHandler(
				this.newLogItemEventHandler	);
		}
		#endregion run_addEventHandlers
		/// <summary>
		/// Performes the actual backtest
		/// </summary>
		public void Run()
		{
			this.startingTimeForScript = DateTime.Now;

//			run_initializeEndOfDayTimer();
//			run_initializeAccount();
//			run_initializeEndOfDayTimerHandler();
//			run_initializeProgressHandlers();
			run_addEventHandlers();
			//			this.progressBarForm.Show();
			this.endOfDayTimer.Start();
		}
		#endregion Run

		#region marketCloseEventHandler
		private bool isTimeToStop( DateTime currentTime )
		{
			DateTime maxEndingDateTimeForScript =
				this.startingTimeForScript.AddHours( this.maxRunningHours );
			bool scriptTimeElapsed = ( DateTime.Now >= maxEndingDateTimeForScript );
			bool stopBacktestIfMaxRunningHoursHasBeenReached =
				this.endOfDayStrategy.StopBacktestIfMaxRunningHoursHasBeenReached;
			return
				( ( currentTime > this.lastDateTime ) ||
				( scriptTimeElapsed &&
				stopBacktestIfMaxRunningHoursHasBeenReached ) );
		}
		private void marketCloseEventHandler_notifyProgress(
			IEndOfDayTimer endOfDayTimer )
		{
			string progressMessage = "Current out of sample date:" +
				endOfDayTimer.GetCurrentTime().DateTime.ToString() + " - " +
				"First date:" + this.firstDateTime.ToString() + " - " +
				"Last date:" + this.lastDateTime.ToString();
			NewMessageEventArgs newMessageEventArgs =
				new NewMessageEventArgs( progressMessage );
			if(this.NewMessage != null)
				this.NewMessage( this , newMessageEventArgs );
		}
		private void marketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			EndOfDayDateTime currentEndOfDayDateTime =
				( ( IEndOfDayTimer )sender ).GetCurrentTime();
			DateTime currentDateTime = currentEndOfDayDateTime.DateTime;
			if ( this.isTimeToStop( currentDateTime ) )
			{
				// either the simulation has reached the ending date or
				// too much time elapsed since the simulation started
				this.actualLastDateTime =
					ExtendedDateTime.Min( this.lastDateTime , currentDateTime );
				this.endOfDayTimer.Stop();
				this.realDateTimeWhenTheBackTestIsStopped = DateTime.Now;
				this.accountReport = this.account.CreateReport(
					"" ,
					1 , currentEndOfDayDateTime , this.benchmark.Ticker ,
					this.historicalQuoteProvider );
				this.accountReport.Name = this.Description;

				//				this.progressBarForm.Close();
				//				ObjectArchiver.Archive( this.account ,
				//					@"C:\Documents and Settings\Glauco\Desktop\reports\final.qP" );
				//				this.saveLog( currentTime );
				//				this.showReport( sender );
				//				WFMultiOneRankReportDebugger wFMultiOneRankReportDebugger =
				//					new WFMultiOneRankReportDebugger( this.numberOfPortfolioPositions ,
				//					this.numberDaysForInSampleOptimization , this.benchmark );
				//				report.TransactionGrid.MouseUp +=
				//					new MouseEventHandler(
				//					wFMultiOneRankReportDebugger.MouseClickEventHandler );
			}
						else
							// the simulation has not reached the ending date, yet
							this.marketCloseEventHandler_notifyProgress(
								( IEndOfDayTimer )sender );
		}
		#endregion marketCloseEventHandler

		private void newLogItemEventHandler( object sender ,
			NewLogItemEventArgs eventArgs )
		{
			//			WFLagChosenPositions wFLagChosenPositions =
			//				new WFLagChosenPositions( eventArgs.WFLagChosenTickers ,
			//				this.endOfDayTimer.GetCurrentTime().DateTime );
			this.backTestLog.Add( eventArgs.LogItem );
		}
	}
}
