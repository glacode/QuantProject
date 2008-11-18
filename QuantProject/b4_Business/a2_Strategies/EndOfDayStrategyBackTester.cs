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
using QuantProject.Business.Financial.Accounting.AccountProviding;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Timing;

namespace QuantProject.Business.Strategies
{
	/// <summary>
	/// Performs a backtest for an end of day strategy
	/// </summary>
	[Serializable]
	public class EndOfDayStrategyBackTester : IMessageSender , ILogDescriptor
	{
		public event NewMessageEventHandler NewMessage;
		
		// the following event is used to avoid the strategy receiving
		// NewDateTime events directly from the timer: this way the
		// strategy does not receive any new message once the backtester
		// has been completed
		private event NewDateTimeEventHandler newDateTime;


		private string backTestID;
		private IStrategyForBacktester strategyForBacktester;
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		private DateTime firstDateTime;
		private DateTime lastDateTime;
		private Benchmark benchmark;
		private double cashToStart;
		private double maxRunningHours;
		private IAccountProvider accountProvider;
		private DateTime startingTimeForScript;
		private Timer timer;
		private DateTime actualLastDateTime;
		private Account account;
		private AccountReport accountReport;
		private BackTestLog backTestLog;
		private DateTime realDateTimeWhenTheBackTestIsStopped;
		
		public HistoricalMarketValueProvider HistoricalMarketValueProvider
		{
			get { return this.historicalMarketValueProvider; }
		}
		public Benchmark Benchmark
		{
			get { return this.benchmark; }
		}
		/// <summary>
		/// Returns the simulated DateTime when the backtest is started
		/// (not the real time)
		/// </summary>
		public DateTime FirstDateTime
		{
			get
			{
				return this.firstDateTime;
			}
		}
		/// <summary>
		/// Returns the simulated DateTime when the backtest is stopped
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
		public Timer Timer
		{
			get
			{
				return this.timer;
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
					"_" + this.historicalMarketValueProvider.Description +
					"_" + this.strategyForBacktester.Description;
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
		                                  Timer timer ,
		                                  IStrategyForBacktester strategyForBacktester ,
		                                  HistoricalMarketValueProvider historicalMarketValueProvider ,
		                                  IAccountProvider accountProvider,
		                                  DateTime firstDateTime , DateTime lastDateTime ,
		                                  Benchmark benchmark ,
		                                  double cashToStart ,
		                                  double maxRunningHours )
		{
			this.endOfDayStrategyBackTester_checkParameters(
				strategyForBacktester ,
				historicalMarketValueProvider ,
				accountProvider,
				firstDateTime , lastDateTime ,
				benchmark ,
				cashToStart ,
				maxRunningHours );
			this.backTestID = backTestID;
			this.strategyForBacktester = strategyForBacktester;
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.accountProvider = accountProvider;
			this.firstDateTime = firstDateTime;
			this.lastDateTime = lastDateTime;
			this.benchmark = benchmark;
			this.cashToStart = cashToStart;
			this.maxRunningHours = maxRunningHours;
			
			this.timer = timer;

//			this.initialize_endOfDayTimer();
			this.account = this.accountProvider.GetAccount(
				this.timer, this.historicalMarketValueProvider);
			this.strategyForBacktester.Account = this.account;
			this.backTestLog = new BackTestLog( backTestID , firstDateTime ,
			                                   lastDateTime , benchmark );
			this.actualLastDateTime = DateTime.MinValue;
			this.realDateTimeWhenTheBackTestIsStopped = DateTime.MinValue;
		}
		private void endOfDayStrategyBackTester_checkParameters(
			IStrategyForBacktester strategyForBacktester ,
			HistoricalMarketValueProvider historicalMarketValueProvider ,
			IAccountProvider accountProvider,
			DateTime firstDateTime , DateTime lastDateTime ,
			Benchmark benchmark ,
			double cashToStart ,
			double maxRunningHours )
		{
			if ( strategyForBacktester == null )
				throw new Exception( "endOfDayStrategy cannot be null!" );
			if ( historicalMarketValueProvider == null )
				throw new Exception( "historicalQuoteProvider cannot be null!" );
			if ( accountProvider == null )
				throw new Exception( "accountProvider cannot be null!" );
			if ( firstDateTime.CompareTo( lastDateTime ) > 0 )
				throw new Exception( "firstDateTime is greater than lastDateTime!" );
			if ( cashToStart <= 0 )
				throw new Exception( "cashToStart must be greater than zero!" );
			if ( maxRunningHours <= 0 )
				throw new Exception( "maxRunningHours must be greater than zero!" );
		}

		private void initialize_endOfDayTimer()
		{
			DateTime startingDateTime = HistoricalEndOfDayTimer.GetMarketOpen(
				this.firstDateTime );
//				new EndOfDayDateTime( this.firstDateTime ,
//				EndOfDaySpecificTime.MarketOpen );
			this.timer = new IndexBasedEndOfDayTimer(
				startingDateTime , this.benchmark.Ticker );
		}
		
		#region Run
		#region run_addEventHandlers
		private void handlerToAddCashToStart(
			Object sender , DateTime dateTime )
		{
			if ( this.account.Transactions.Count == 0 )
				// no cash has been added yet
				this.account.AddCash( this.cashToStart );
		}
		private void run_addEventHandlers_addHandlersToAddCashToStart()
		{
			this.newDateTime +=
				new NewDateTimeEventHandler(
					this.handlerToAddCashToStart );
//			this.timer.NewDateTime +=
//				new NewDateTimeEventHandler(
//					this.handlerToAddCashToStart );
			
//			this.endOfDayTimer.MarketOpen +=
//				new MarketOpenEventHandler(
//				this.handlerToAddCashToStart );
//			this.endOfDayTimer.FiveMinutesBeforeMarketClose +=
//				new FiveMinutesBeforeMarketCloseEventHandler(
//				this.handlerToAddCashToStart );
//			this.endOfDayTimer.MarketClose +=
//				new MarketCloseEventHandler(
//				this.handlerToAddCashToStart );
//			this.endOfDayTimer.OneHourAfterMarketClose +=
//				new OneHourAfterMarketCloseEventHandler(
//				this.handlerToAddCashToStart );
		}
		private void run_addEventHandlers_addStrategyHandlers()
		{
			this.newDateTime +=
				new NewDateTimeEventHandler(
					this.strategyForBacktester.NewDateTimeEventHandler );
//			this.endOfDayTimer.MarketOpen +=
//				new MarketOpenEventHandler(
//				this.endOfDayStrategy.MarketOpenEventHandler );
//			this.endOfDayTimer.FiveMinutesBeforeMarketClose +=
//				new FiveMinutesBeforeMarketCloseEventHandler(
//				this.endOfDayStrategy.FiveMinutesBeforeMarketCloseEventHandler );
//			this.endOfDayTimer.OneHourAfterMarketClose +=
//				new OneHourAfterMarketCloseEventHandler(
//				this.endOfDayStrategy.OneHourAfterMarketCloseEventHandler );
//			this.endOfDayTimer.MarketClose +=
//				new MarketCloseEventHandler(
//				this.endOfDayStrategy.MarketCloseEventHandler );
		}
		private void run_addEventHandlers()
		{
			this.run_addEventHandlers_addHandlersToAddCashToStart();
			this.timer.NewDateTime +=
				new NewDateTimeEventHandler( this.newDateTimeEventHandler );
			this.run_addEventHandlers_addStrategyHandlers();
//			this.endOfDayTimer.MarketClose +=
//				new MarketCloseEventHandler(
//				this.marketCloseEventHandler );
			this.strategyForBacktester.NewLogItem +=
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
			this.timer.Start();
		}
		#endregion Run

		#region newDateTimeEventHandler
		
		private bool isTimeToStop( DateTime currentTime )
		{
			DateTime maxEndingDateTimeForScript =
				this.startingTimeForScript.AddHours( this.maxRunningHours );
			DateTime realTime = DateTime.Now;
			bool scriptTimeElapsed = ( realTime >= maxEndingDateTimeForScript );
			bool stopBacktestIfMaxRunningHoursHasBeenReached =
				this.strategyForBacktester.StopBacktestIfMaxRunningHoursHasBeenReached;
			return
				( ( currentTime > this.lastDateTime ) ||
				 ( scriptTimeElapsed &&
				  stopBacktestIfMaxRunningHoursHasBeenReached ) );
		}
//		private void removeTimerEventHandlers()
//		{
//			foreach( NewDateTimeEventHandler newDateTimeEventHandler in
//			        this.timer.NewDateTime.GetInvocationList())
//			{
//				this.timer.NewDateTime -= newDateTimeEventHandler;
//			}
//		}
		
		#region stopTheScript
		private void stopTheScript( DateTime currentDateTime )
		{
			this.actualLastDateTime =
				ExtendedDateTime.Min( this.lastDateTime , currentDateTime );
			this.timer.Stop();
//			this.removeTimerEventHandlers();
//			this.timer.NewDateTime -=
//				new NewDateTimeEventHandler(
//					this.strategyForBacktester.NewDateTimeEventHandler );
			this.realDateTimeWhenTheBackTestIsStopped = DateTime.Now;
			this.accountReport = this.account.CreateReport(
				"" ,
				1 , currentDateTime , this.benchmark.Ticker ,
				this.historicalMarketValueProvider );
			this.accountReport.Name = this.Description;
		}
//		private void stopTheScriptIfTheCase( Object sender )
//		{
//			DateTime currentDateTime =
//				( ( Timer )sender ).GetCurrentDateTime();
//			if ( this.isTimeToStop( currentDateTime ) )
//			{
//				// either the simulation has reached the ending date or
//				// too much time elapsed since the simulation started
//				this.stopTheScript( currentDateTime );
//			}
//		}
		#endregion stopTheScript
		
		private void notifyProgress(
			Timer timer )
		{
			string progressMessage = "Current out of sample date:" +
				this.timer.GetCurrentDateTime().ToString() + " - " +
				"First date:" + this.firstDateTime.ToString() + " - " +
				"Last date:" + this.lastDateTime.ToString() + " - " +
				"Real time:" + DateTime.Now;
			NewMessageEventArgs newMessageEventArgs =
				new NewMessageEventArgs( progressMessage );
			if(this.NewMessage != null)
				this.NewMessage( this , newMessageEventArgs );
		}
		private void newDateTimeEventHandler(
			Object sender , DateTime dateTime )
		{
//			EndOfDayDateTime currentEndOfDayDateTime =
//				( ( IEndOfDayTimer )sender ).GetCurrentTime();
//			DateTime currentDateTime = currentEndOfDayDateTime.DateTime;
			if ( this.isTimeToStop( dateTime ) )
				this.stopTheScript( dateTime );
			else
				this.newDateTime( sender , dateTime );
			if ( HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
				this.notifyProgress( ( Timer )sender );
		}
		#endregion newDateTimeEventHandler

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
