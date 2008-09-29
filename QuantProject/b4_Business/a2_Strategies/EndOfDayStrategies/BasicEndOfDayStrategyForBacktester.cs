/*
QuantProject - Quantitative Finance Library

BasicEndOfDayStrategyForBacktester.cs
Copyright (C) 2008
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
using QuantProject.ADT.Messaging;

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;

namespace QuantProject.Business.Strategies
{
	/// <summary>
	/// Basic abstract implementation of IEndOfDayStrategyForBacktester,
	/// that should be inherited by specific strategies
	/// </summary>
	[Serializable]
	public abstract class BasicEndOfDayStrategyForBacktester :
		IStrategyForBacktester
	{
		public event NewLogItemEventHandler NewLogItem;
		public event NewMessageEventHandler NewMessage;

		protected int numDaysBeetweenEachOtpimization;
		protected int numDaysForInSampleOptimization;
		protected IIntervalsSelector intervalsSelectorForInSample;
		protected IIntervalsSelector intervalsSelectorForOutOfSample;
		protected IEligiblesSelector eligiblesSelector;
		protected IInSampleChooser inSampleChooser;
		protected HistoricalMarketValueProvider historicalMarketValueProviderForInSample;

		protected DateTime lastOptimizationDateTime;
		protected ReturnIntervals returnIntervals;
		private Account account;

		protected ReturnsManager inSampleReturnsManager;

		protected TestingPositions[] bestTestingPositionsInSample;

		public Account Account
		{
			get { return this.account; }
			set { this.account = value; }
		}

		public virtual bool StopBacktestIfMaxRunningHoursHasBeenReached
		{
			get
			{
				return this.areOptimalWeightedPositionsToBeUpdated();
			}
		}
		
		/// <summary>
		/// short text identifier, to be added to the
		/// strategy description
		/// </summary>
		/// <returns></returns>
		protected abstract string getTextIdentifier();
		
		public string Description
		{
			get
			{
				string descriptionForLogFileName =
					"Strtgy_" +
					this.getTextIdentifier() +
					"nmDysBtwnOptmztns_" +
					this.numDaysBeetweenEachOtpimization.ToString() + "_" +
					this.eligiblesSelector.Description + "_" +
					this.inSampleChooser.Description +
					"_oS_longOnly";
				return descriptionForLogFileName;
			}
		}

		public BasicEndOfDayStrategyForBacktester()
		{
		}
		
		public BasicEndOfDayStrategyForBacktester(
			int numDaysBeetweenEachOtpimization ,
			int numDaysForInSampleOptimization ,
			IIntervalsSelector intervalsSelectorForInSample ,
			IIntervalsSelector intervalsSelectorForOutOfSample ,
			IEligiblesSelector eligiblesSelector ,
			IInSampleChooser inSampleChooser ,
			HistoricalMarketValueProvider historicalMarketValueProviderForInSample
		)
		{
			this.numDaysBeetweenEachOtpimization = numDaysBeetweenEachOtpimization;
			this.numDaysForInSampleOptimization = numDaysForInSampleOptimization;
			this.intervalsSelectorForInSample = intervalsSelectorForInSample;
			this.intervalsSelectorForOutOfSample = intervalsSelectorForOutOfSample;
			this.eligiblesSelector = eligiblesSelector;
			this.inSampleChooser = inSampleChooser;
			this.historicalMarketValueProviderForInSample =
				historicalMarketValueProviderForInSample;
			
			this.returnIntervals =
				new ReturnIntervals( this.intervalsSelectorForOutOfSample );
		}

		#region marketOpenEventHandler

		protected abstract bool marketOpenEventHandler_arePositionsToBeClosed();
		protected abstract bool marketOpenEventHandler_arePositionsToBeOpened();
		protected abstract
			WeightedPositions marketOpenEventHandler_getPositionsToBeOpened();
		
		private void marketOpenEventHandler()
		{
			this.updateReturnIntervals();
			if ( this.marketOpenEventHandler_arePositionsToBeClosed() )
				AccountManager.ClosePositions( this.account );
			if ( this.marketOpenEventHandler_arePositionsToBeOpened() )
			{
				WeightedPositions positionsToBeOpened =
					this.marketOpenEventHandler_getPositionsToBeOpened();
				if ( positionsToBeOpened != null )
					// positions to be opened are available, actually
					AccountManager.OpenPositions(
						this.marketOpenEventHandler_getPositionsToBeOpened() ,
						this.account );
			}
		}
		#endregion marketOpenEventHandler

//		public void FiveMinutesBeforeMarketCloseEventHandler(
//			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
//		{
//		}

		#region marketCloseEventHandler

		protected abstract bool marketCloseEventHandler_arePositionsToBeClosed();
		protected abstract bool marketCloseEventHandler_arePositionsToBeOpened();
		protected abstract
			WeightedPositions marketCloseEventHandler_getPositionsToBeOpened();
		
		private void marketCloseEventHandler()
		{
			this.updateReturnIntervals();
			if ( this.marketCloseEventHandler_arePositionsToBeClosed() )
				AccountManager.ClosePositions( this.account );
			if ( this.marketCloseEventHandler_arePositionsToBeOpened() )
			{
				WeightedPositions positionsToBeOpened =
					this.marketCloseEventHandler_getPositionsToBeOpened();
				if ( positionsToBeOpened != null )
					// positions to be opened are available, actually
					AccountManager.OpenPositions(
						this.marketCloseEventHandler_getPositionsToBeOpened() ,
						this.account );
			}
		}
		#endregion marketCloseEventHandler


		#region oneHourAfterMarketCloseEventHandler
		
		#region areOptimalWeightedPositionsToBeUpdated
		private bool areOptimalWeightedPositionsToBeUpdated_actually()
		{
			DateTime dateTimeForNextOptimization =
				this.lastOptimizationDateTime.AddDays(
					this.numDaysBeetweenEachOtpimization );
			DateTime currentSimulatedDateTime = this.now();
			bool areToBeUpdated =
				( ( ( this.account.Portfolio.Count == 0 )
				   && ( ( this.lastOptimizationDateTime == DateTime.MinValue ) ) ) ||
				 ( currentSimulatedDateTime >= dateTimeForNextOptimization ) );
			return areToBeUpdated;
		}
		private bool areOptimalWeightedPositionsToBeUpdated()
		{
			bool areToBeUpdated;
//			if ( !this.account.Timer.IsActive )
//				// the backtester has stopped the timer because the
//				// backtest has gone beyond max running hours
//				areToBeUpdated = false;
//			else
				areToBeUpdated =
					this.areOptimalWeightedPositionsToBeUpdated_actually();
			return areToBeUpdated;
		}
		#endregion areOptimalWeightedPositionsToBeUpdated
		
		#region getInSampleReturnIntervals
		private DateTime getInSampleReturnIntervals_getFirstDateTime()
		{
			DateTime someDaysBefore = this.now().AddDays(
				-this.numDaysForInSampleOptimization );
			DateTime firstDateTime =
				HistoricalEndOfDayTimer.GetMarketOpen( someDaysBefore );
//			EndOfDayDateTime firstDate = new EndOfDayDateTime(
//				someDaysBefore , EndOfDaySpecificTime.MarketOpen );
			return firstDateTime;
		}
		private ReturnIntervals getInSampleReturnIntervals()
		{
			DateTime firstDateTime =
				this.getInSampleReturnIntervals_getFirstDateTime();
			DateTime lastDateTime =
				HistoricalEndOfDayTimer.GetMarketClose( this.now() );
//				new EndOfDayDateTime( this.now().DateTime ,
//				                     EndOfDaySpecificTime.MarketClose );
			ReturnIntervals inSampleReturnIntervals =
				new ReturnIntervals( this.intervalsSelectorForInSample );
			inSampleReturnIntervals.AppendFirstInterval( firstDateTime );
			if ( inSampleReturnIntervals.LastDateTime < lastDateTime )
				inSampleReturnIntervals.AppendIntervalsButDontGoBeyondLastDate(
					lastDateTime );
			return inSampleReturnIntervals;
		}
		#endregion getInSampleReturnIntervals
		
		private void checkQualityFor_bestTestingPositionsInSample()
		{
			for( int i = 0 ; i < this.bestTestingPositionsInSample.Length ; i++ )
				if ( this.bestTestingPositionsInSample[ i ] == null )
				throw new Exception(
					"The IInSampleChooser should have returned an array " +
					"of non null bestTestingPositionsInSample!" );
		}
		private void notifyMessage( EligibleTickers eligibleTickers )
		{
			string message = "Number of Eligible tickers: " +
				eligibleTickers.Count +
				" - " +
				DateTime.Now.ToString();
			NewMessageEventArgs newMessageEventArgs =
				new NewMessageEventArgs( message );
			if ( this.NewMessage != null )
				this.NewMessage( this , newMessageEventArgs );
		}
		
		#region logOptimizationInfo
		protected abstract LogItem getLogItem( EligibleTickers eligibleTickers );

		private void raiseNewLogItem( EligibleTickers eligibleTickers )
		{
			LogItem logItem =
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
		
		private void updateOptimalTestingPositions_actually()
		{
			ReturnIntervals inSampleReturnIntervals =
				this.getInSampleReturnIntervals();
			EligibleTickers eligibleTickers =
				this.eligiblesSelector.GetEligibleTickers(
					inSampleReturnIntervals.BordersHistory );
			this.inSampleReturnsManager = new ReturnsManager(
				inSampleReturnIntervals ,
				this.historicalMarketValueProviderForInSample );
			this.bestTestingPositionsInSample =
				(TestingPositions[])this.inSampleChooser.AnalyzeInSample(
					eligibleTickers , this.inSampleReturnsManager );
			this.checkQualityFor_bestTestingPositionsInSample();

			this.notifyMessage( eligibleTickers );
			this.logOptimizationInfo( eligibleTickers );
		}
		private void updateOptimalTestingPositions()
		{
			this.updateOptimalTestingPositions_actually();
			this.lastOptimizationDateTime = this.now();
//			FixedLengthTwoPhasesLogItem logItem =
//				new FixedLengthTwoPhasesLogItem( this.now() );
//			logItem.BestWeightedPositionsInSample = this.bestWeightedPositionsInSample;
		}
		private void oneHourAfterMarketCloseEventHandler()
		{
			if ( this.areOptimalWeightedPositionsToBeUpdated() )
				this.updateOptimalTestingPositions();
		}
		#endregion oneHourAfterMarketCloseEventHandler
		
		#region NewDateTimeEventHandler
		public void NewDateTimeEventHandler( Object sender , DateTime dateTime )
		{
			if ( HistoricalEndOfDayTimer.IsMarketOpen( dateTime ) )
				this.marketOpenEventHandler();
			if ( HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
				this.marketCloseEventHandler();
			if ( HistoricalEndOfDayTimer.IsOneHourAfterMarketClose( dateTime ) )
				this.oneHourAfterMarketCloseEventHandler();
		}
		#endregion NewDateTimeEventHandler

//		protected bool isInsampleOptimizationNeeded()
//		{
//			DateTime dateTimeForNextOptimization =
//				this.lastOptimizationDateTime.AddDays(
//					this.numDaysBeetweenEachOtpimization );
//			bool returnValue =
//				( ( ( this.account.Portfolio.Count == 0 )
//				   && ( ( this.lastOptimizationDateTime == DateTime.MinValue ) ) ) ||
//				 ( this.now().DateTime >= dateTimeForNextOptimization ) );
//			return returnValue;
//		}

		protected DateTime now()
		{
			DateTime simulatedDateTime =
				this.account.Timer.GetCurrentDateTime();
			return simulatedDateTime;
		}
		private void updateReturnIntervals()
		{
			DateTime currentDateTime = this.now();
			if ( this.returnIntervals.Count == 0 )
				// no interval has been added yet
				this.returnIntervals.AppendFirstInterval(
					currentDateTime );
			else
				// at least one interval has already been added
				if ( this.returnIntervals.LastDateTime <= currentDateTime )
				this.returnIntervals.AppendIntervalsToGoJustBeyond(
					currentDateTime );
		}
		protected ReturnInterval lastIntervalAppended()
		{
			ReturnInterval lastInterval =
				this.returnIntervals[ this.returnIntervals.Count - 1 ];
			return lastInterval;
		}
	}
}
