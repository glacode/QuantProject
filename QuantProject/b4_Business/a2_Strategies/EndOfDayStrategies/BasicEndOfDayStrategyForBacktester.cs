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
	public abstract class BasicEndOfDayStrategyForBacktester :
		IEndOfDayStrategyForBacktester
	{
		public event NewLogItemEventHandler NewLogItem;
		public event NewMessageEventHandler NewMessage;

		protected int numDaysBeetweenEachOtpimization;
		protected int numDaysForInSampleOptimization;
		protected IIntervalsSelector intervalsSelectorForInSample;
		protected IIntervalsSelector intervalsSelectorForOutOfSample;
		protected IEligiblesSelector eligiblesSelector;
		protected IInSampleChooser inSampleChooser;
		protected IHistoricalQuoteProvider historicalQuoteProviderForInSample;

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

		public bool StopBacktestIfMaxRunningHoursHasBeenReached
		{
			get
			{
				return this.optimalWeightedPositionsAreToBeUpdated();
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
			IHistoricalQuoteProvider historicalQuoteProviderForInSample
			)
		{
			this.numDaysBeetweenEachOtpimization = numDaysBeetweenEachOtpimization;
			this.numDaysForInSampleOptimization = numDaysForInSampleOptimization;
			this.intervalsSelectorForInSample = intervalsSelectorForInSample;
			this.intervalsSelectorForOutOfSample = intervalsSelectorForOutOfSample;
			this.eligiblesSelector = eligiblesSelector;
			this.inSampleChooser = inSampleChooser;
			this.historicalQuoteProviderForInSample =
				historicalQuoteProviderForInSample;
			
			this.returnIntervals =
				new ReturnIntervals( this.intervalsSelectorForOutOfSample );
		}

		#region MarketOpenEventHandler

		protected abstract bool marketOpenEventHandler_arePositionsToBeClosed();
		protected abstract bool marketOpenEventHandler_arePositionsToBeOpened();
		protected abstract
			WeightedPositions marketOpenEventHandler_getPositionsToBeOpened();
		
		public void MarketOpenEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
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
		#endregion MarketOpenEventHandler

		public void FiveMinutesBeforeMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
		}

		#region MarketCloseEventHandler

		protected abstract bool marketCloseEventHandler_arePositionsToBeClosed();
		protected abstract bool marketCloseEventHandler_arePositionsToBeOpened();
		protected abstract
			WeightedPositions marketCloseEventHandler_getPositionsToBeOpened();
		
		public void MarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
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
		#endregion MarketCloseEventHandler


		#region OneHourAfterMarketCloseEventHandler
		private bool optimalWeightedPositionsAreToBeUpdated()
		{
//			TimeSpan timeSpanSinceLastOptimization =
//				this.now().DateTime - this.lastOptimizationDateTime;
//			bool areToBeUpdated =	( timeSpanSinceLastOptimization.Days >=
//				this.numDaysBeetweenEachOtpimization );
//			return areToBeUpdated;
			DateTime dateTimeForNextOptimization =
				this.lastOptimizationDateTime.AddDays(
				this.numDaysBeetweenEachOtpimization );
			bool areToBeUpdated =
				( ( ( this.account.Portfolio.Count == 0 )
				&& ( ( this.lastOptimizationDateTime == DateTime.MinValue ) ) ) ||
				( this.now().DateTime >= dateTimeForNextOptimization ) );
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
				new ReturnIntervals( this.intervalsSelectorForInSample );
			inSampleReturnIntervals.AppendFirstInterval( firstDate );
			if ( inSampleReturnIntervals.LastEndOfDayDateTime.IsLessThan(
				lastDate ) )
				inSampleReturnIntervals.AppendIntervalsButDontGoBeyondLastDate(
					lastDate );
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
				eligibleTickers.Count;
			NewMessageEventArgs newMessageEventArgs =
				new NewMessageEventArgs( message );
			if ( this.NewMessage != null )
				this.NewMessage( this , newMessageEventArgs );
		}
		#region logOptimizationInfo
//		private void outputMessage( string message )
//		{
//			string dateStamp =
//				ExtendedDateTime.GetShortDescriptionForFileName( DateTime.Now );
//			MessageManager.DisplayMessage( message ,
//				"NotificationMessagesForCurrentStrategy_" +
//				dateStamp + ".Txt" );
//		}
		
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
				this.historicalQuoteProviderForInSample );
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
			this.lastOptimizationDateTime = this.now().DateTime;
//			FixedLengthTwoPhasesLogItem logItem =
//				new FixedLengthTwoPhasesLogItem( this.now() );
//			logItem.BestWeightedPositionsInSample = this.bestWeightedPositionsInSample;
		}
		public void OneHourAfterMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( this.optimalWeightedPositionsAreToBeUpdated() )
				this.updateOptimalTestingPositions();
		}
		#endregion OneHourAfterMarketCloseEventHandler

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

		protected EndOfDayDateTime now()
		{
			return this.account.EndOfDayTimer.GetCurrentTime();
		}
		private void updateReturnIntervals()
		{
			EndOfDayDateTime currentEndOfDayDateTime = this.now();
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
		protected ReturnInterval lastIntervalAppended()
		{
			ReturnInterval lastInterval =
				this.returnIntervals[ this.returnIntervals.Count - 1 ];
			return lastInterval;
		}
	}
}
