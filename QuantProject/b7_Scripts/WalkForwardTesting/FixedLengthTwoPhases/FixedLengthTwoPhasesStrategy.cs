/*
QuantProject - Quantitative Finance Library

FixedLengthTwoPhasesStrategy.cs
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
using System.Data;

using QuantProject.ADT.Messaging;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;
using QuantProject.Presentation;

namespace QuantProject.Scripts.WalkForwardTesting.FixedLengthTwoPhases
{
	/// <summary>
	/// FixedLengthTwoPhases strategy with in sample optimizations.
	/// The strategy goes with the best weighted positions for the
	/// first phase, then goes with the opposite for the second phase
	/// </summary>
	public class FixedLengthTwoPhasesStrategy : IEndOfDayStrategyForBacktester
	{
		public event NewLogItemEventHandler NewLogItem;
		public event NewMessageEventHandler NewMessage;

		private int numberOfPortfolioPositions;
		private int numDaysBeetweenEachOtpimization;
		private int numDaysForInSampleOptimization;
		private Benchmark benchmark;
		private IIntervalsSelector intervalsSelector;
		private IEligiblesSelector eligiblesSelector;
		private IInSampleChooser inSampleChooser;

		private DateTime lastOptimizationDateTime;
		private ReturnIntervals returnIntervals;
		private IHistoricalQuoteProvider historicalQuoteProvider;

		private Account account;
//		private bool arePositionsToBeClosed;
//		private bool arePositionsToBeOpened;
		private WeightedPositions bestWeightedPositionsInSample;

		public Account Account
		{
			get { return this.account; }
			set { this.account = value; }
		}


		public bool StopBacktestIfMaxRunningHoursHasBeenReached
		{
			get
			{
				return this.isInsampleOptimizationNeeded();
			}
		}
		public string Description
		{
			get
			{
				string descriptionForLogFileName =
					"Strtgy_fltp_" +
					"nmDysBtwnOptmztns_" +
					this.numDaysBeetweenEachOtpimization.ToString() + "_" +
					this.eligiblesSelector.Description + "_" +
					this.inSampleChooser.Description +
					"_oS_longOnly";
				return descriptionForLogFileName;
			}
		}

		public FixedLengthTwoPhasesStrategy(
			int numberOfPortfolioPositions ,
			int numDaysBeetweenEachOtpimization ,
			int numDaysForInSampleOptimization ,
			Benchmark benchmark ,
			IIntervalsSelector intervalsSelector ,
			IEligiblesSelector eligiblesSelector ,
			IInSampleChooser inSampleChooser ,
			IHistoricalQuoteProvider historicalQuoteProvider
			 )
		{
			this.numberOfPortfolioPositions = numberOfPortfolioPositions;
			this.numDaysBeetweenEachOtpimization = numDaysBeetweenEachOtpimization;
			this.numDaysForInSampleOptimization = numDaysForInSampleOptimization;
			this.benchmark = benchmark;
			this.intervalsSelector = intervalsSelector;
			this.eligiblesSelector = eligiblesSelector;
			this.inSampleChooser = inSampleChooser;
			this.historicalQuoteProvider = historicalQuoteProvider;
			
			this.returnIntervals =
				new ReturnIntervals( this.intervalsSelector );

//			this.arePositionsToBeClosed = false;
//			this.arePositionsToBeOpened = false;
		}
		#region MarketOpenEventHandler
		private bool marketOpenEventHandler_arePositionsToBeClosed()
		{
			bool arePositionsToBeClosed = (
				( this.account.Portfolio.Count > 0 ) &&
				( this.now().IsEqualTo(
					this.lastIntervalAppended().Begin ) ) );
			return arePositionsToBeClosed;
		}
		#region marketOpenEventHandler_arePositionsToBeOpened
		private bool lastAppendedIntervalIsALongPeriod()
		{
			bool isALongPeriod =
				( ( this.returnIntervals.Count % 2 ) == 1 );
			return isALongPeriod;
		}
		private bool currentTimeBeginsALongPeriod()
		{
			bool beginsTheLastInterval =
				( this.now().IsEqualTo(
					this.lastIntervalAppended().Begin ) );
			bool lastIntervalIsALongPeriod =
				this.lastAppendedIntervalIsALongPeriod();
			return ( beginsTheLastInterval && lastIntervalIsALongPeriod );
		}
		private bool marketOpenEventHandler_arePositionsToBeOpened()
		{
			bool arePositionsToBeOpened = (
				( this.bestWeightedPositionsInSample != null ) &&
				( this.currentTimeBeginsALongPeriod() ) );
			return arePositionsToBeOpened;
		}
		#endregion marketOpenEventHandler_arePositionsToBeOpened
		public void MarketOpenEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			this.updateReturnIntervals();
			if ( this.marketOpenEventHandler_arePositionsToBeClosed() )
				AccountManager.ClosePositions( this.account );
			if ( this.marketOpenEventHandler_arePositionsToBeOpened() )
				AccountManager.OpenPositions( this.bestWeightedPositionsInSample ,
					this.account );
		}
		#endregion MarketOpenEventHandler
		public void FiveMinutesBeforeMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
		}
		#region MarketCloseEventHandler
		private bool marketCloseEventHandler_arePositionsToBeClosed()
		{
			return marketOpenEventHandler_arePositionsToBeClosed();
		}
		private bool marketCloseEventHandler_arePositionsToBeOpened()
		{
			// this strategy goes long only
			return false;
		}
		public void MarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			this.updateReturnIntervals();
			if ( this.marketCloseEventHandler_arePositionsToBeClosed() )
				AccountManager.ClosePositions( this.account );
			if ( this.marketCloseEventHandler_arePositionsToBeOpened() )
			{
				this.bestWeightedPositionsInSample.Reverse();
				AccountManager.OpenPositions(
					this.bestWeightedPositionsInSample ,
					this.account );
				this.bestWeightedPositionsInSample.Reverse();
			}
		}
		#endregion MarketCloseEventHandler
		#region OneHourAfterMarketCloseEventHandler
		private bool optimalWeightedPositionsAreToBeUpdated()
		{
			TimeSpan timeSpanSinceLastOptimization =
				this.now().DateTime - this.lastOptimizationDateTime;
			bool areToBeUpdated =	( timeSpanSinceLastOptimization.Days >=
				this.numDaysBeetweenEachOtpimization );
			return areToBeUpdated;
		}
		#region getInSampleReturnIntervals
		private EndOfDayDateTime getInSampleReturnIntervals_getFirstDate()
		{
			DateTime firstDateTime = this.now().DateTime.AddDays(
				-this.numDaysForInSampleOptimization );
			EndOfDayDateTime firstDate = new EndOfDayDateTime(
				firstDateTime , EndOfDaySpecificTime.MarketOpen );
			return firstDate;
		}
		private ReturnIntervals getInSampleReturnIntervals()
		{
			EndOfDayDateTime firstDate =
				this.getInSampleReturnIntervals_getFirstDate();
			EndOfDayDateTime lastDate =
				new EndOfDayDateTime( this.now().DateTime ,
				EndOfDaySpecificTime.MarketClose );
			ReturnIntervals inSampleReturnIntervals =
				new ReturnIntervals( this.intervalsSelector );
			inSampleReturnIntervals.AppendFirstInterval( firstDate );
			if ( inSampleReturnIntervals.LastEndOfDayDateTime.IsLessThan(
				lastDate ) )
				inSampleReturnIntervals.AppendIntervalsButDontGoBeyondLastDate(
					lastDate );
			return inSampleReturnIntervals;
		}
		#endregion getInSampleReturnIntervals
		private void notifyMessage( EligibleTickers eligibleTickers )
		{
			string message = "Number of Eligible tickers: " +
				eligibleTickers.Count;
			NewMessageEventArgs newMessageEventArgs =
				new NewMessageEventArgs( message );
			if ( this.NewMessage != null )
				this.NewMessage( this , newMessageEventArgs );
		}
		#region logOptimizationInfo
		private void outputMessage( string message )
		{
			MessageManager.DisplayMessage( message ,
				"FixedLengthUpDown.Txt" );
		}
		private FixedLengthTwoPhasesLogItem getLogItem( EligibleTickers eligibleTickers )
		{
			FixedLengthTwoPhasesLogItem logItem =
				new FixedLengthTwoPhasesLogItem( this.now() );
			logItem.BestWeightedPositionsInSample =
				this.bestWeightedPositionsInSample;
			logItem.NumberOfEligibleTickers =
				eligibleTickers.Count;
			return logItem;
		}
		private void raiseNewLogItem( EligibleTickers eligibleTickers )
		{
			FixedLengthTwoPhasesLogItem logItem =
				this.getLogItem( eligibleTickers );
			NewLogItemEventArgs newLogItemEventArgs =
				new NewLogItemEventArgs( logItem );
			this.NewLogItem( this , newLogItemEventArgs );
		}
		private void logOptimizationInfo( EligibleTickers eligibleTickers )
		{
			this.raiseNewLogItem( eligibleTickers );
		}
		#endregion logOptimizationInfo
		private void updateOptimalWeightedPositions_actually()
		{
			ReturnIntervals inSampleReturnIntervals =
				this.getInSampleReturnIntervals();
			EligibleTickers eligibleTickers =
				this.eligiblesSelector.GetEligibleTickers(
				inSampleReturnIntervals.BordersHistory );
			ReturnsManager returnsManager = new ReturnsManager(
				inSampleReturnIntervals , this.historicalQuoteProvider );
			this.bestWeightedPositionsInSample =
				(WeightedPositions)this.inSampleChooser.AnalyzeInSample(
					eligibleTickers , returnsManager );

			this.notifyMessage( eligibleTickers );
			this.logOptimizationInfo( eligibleTickers );
		}
		private void updateOptimalWeightedPositions()
		{
			this.updateOptimalWeightedPositions_actually();
			this.lastOptimizationDateTime = this.now().DateTime;
			FixedLengthTwoPhasesLogItem logItem =
				new FixedLengthTwoPhasesLogItem( this.now() );
			logItem.BestWeightedPositionsInSample = this.bestWeightedPositionsInSample;
//				this.wFLagWeightedPositionsChooser.WFLagChosenPositions ,
//				this.wFLagWeightedPositionsChooser.GenerationWhenChosenPositionsWereFound ,
//				this.now().DateTime );
//			this.NewChosenPositions(
//				this , new WFLagNewChosenPositionsEventArgs(
//				wFLagLogItem ) );
		}
		public void OneHourAfterMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( this.optimalWeightedPositionsAreToBeUpdated() )
				this.updateOptimalWeightedPositions();
		}
		#endregion OneHourAfterMarketCloseEventHandler
		public bool isInsampleOptimizationNeeded()
		{
			DateTime dateTimeForNextOptimization =
				this.lastOptimizationDateTime.AddDays(
				this.numDaysBeetweenEachOtpimization );
			bool returnValue =
				( ( ( this.account.Portfolio.Count == 0 )
				&& ( ( this.lastOptimizationDateTime == DateTime.MinValue ) ) ) ||
				( this.now().DateTime >= dateTimeForNextOptimization ) );
			return returnValue;
		}
		private void updateReturnIntervals()
		{
			EndOfDayDateTime currentEndOfDayDateTime =
				this.account.EndOfDayTimer.GetCurrentTime();
			if ( this.returnIntervals.Count == 0 )
				// no interval has been added yet
				this.returnIntervals.AppendFirstInterval(
					currentEndOfDayDateTime );
			else
				// at least one interval has already been added
				if ( this.returnIntervals.LastEndOfDayDateTime.IsLessThanOrEqualTo(
					currentEndOfDayDateTime ) )
					this.returnIntervals.AppendIntervalsToGoJustBeyond(
						currentEndOfDayDateTime );
		}
		private EndOfDayDateTime now()
		{
			return this.account.EndOfDayTimer.GetCurrentTime();
		}
		private ReturnInterval lastIntervalAppended()
		{
			ReturnInterval lastInterval =
				this.returnIntervals[ this.returnIntervals.Count - 1 ];
			return lastInterval;
		}
	}
}
