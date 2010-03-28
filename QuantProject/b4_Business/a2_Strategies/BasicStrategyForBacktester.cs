/*
QuantProject - Quantitative Finance Library

BasicStrategyForBacktester.cs
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

using QuantProject.ADT.Messaging;
using QuantProject.ADT.Timing;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;

namespace QuantProject.Business.Strategies
{
	/// <summary>
	/// Basic abstract implementation of IStrategyForBacktester,
	/// that should be inherited by specific strategies
	/// </summary>
	[Serializable]
	public abstract class BasicStrategyForBacktester : IStrategyForBacktester
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
		protected ReturnIntervals outOfSampleReturnIntervals;
		private Account account;
		
		protected ReturnsManager inSampleReturnsManager;
		
		protected TestingPositions[] bestTestingPositionsInSample;

		#region StopBacktestIfMaxRunningHoursHasBeenReached
		protected DateTime now()
		{
			DateTime simulatedDateTime =
				this.account.Timer.GetCurrentDateTime();
			return simulatedDateTime;
		}
		private bool areOptimalWeightedPositionsToBeUpdated()
		{
			DateTime dateTimeForNextOptimization =
				this.lastOptimizationDateTime.AddDays(
					this.numDaysBeetweenEachOtpimization );
			DateTime currentSimulatedDateTime = this.now();
			bool areToBeUpdated =
				( ( ( this.account.Portfolio.Count == 0 )
				   && ( ( this.lastOptimizationDateTime == DateTime.MinValue ) ) ) ||
				 ( currentSimulatedDateTime >= dateTimeForNextOptimization ) );
			areToBeUpdated = (
				areToBeUpdated &&
				HistoricalEndOfDayTimer.IsOneHourAfterMarketClose(
					currentSimulatedDateTime ) );
			return areToBeUpdated;
		}
		public virtual bool StopBacktestIfMaxRunningHoursHasBeenReached
		{
			get
			{
				return this.areOptimalWeightedPositionsToBeUpdated();
			}
		}
		#endregion StopBacktestIfMaxRunningHoursHasBeenReached
		
		public Account Account
		{
			get { return this.account; }
			set { this.account = value; }
		}

		#region Description
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
		#endregion Description

		public BasicStrategyForBacktester(
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
			
			this.outOfSampleReturnIntervals =
				new ReturnIntervals( this.intervalsSelectorForOutOfSample );
		}
		
		#region NewDateTimeEventHandler
		
		private void updateReturnIntervals()
		{
			DateTime currentDateTime = this.now();
			if ( this.outOfSampleReturnIntervals.Count == 0 )
				// no interval has been added yet
				this.outOfSampleReturnIntervals.AppendFirstInterval(
					currentDateTime );
			else
				// at least one interval has already been added
				if ( this.outOfSampleReturnIntervals.LastDateTime <= currentDateTime )
				this.outOfSampleReturnIntervals.AppendIntervalsToGoJustBeyond(
					currentDateTime );
		}

		#region updateOptimalTestingPositions
		
		#region updateOptimalTestingPositions_actually
		
		#region getInSampleReturnIntervals
		private DateTime getInSampleReturnIntervals_getFirstDateTime()
		{
			DateTime someDaysBefore = this.now().AddDays(
				-this.numDaysForInSampleOptimization );
			DateTime firstDateTime =
				HistoricalEndOfDayTimer.GetMarketOpen( someDaysBefore );
			return firstDateTime;
		}
		private ReturnIntervals getInSampleReturnIntervals()
		{
			DateTime firstDateTime =
				this.getInSampleReturnIntervals_getFirstDateTime();
			DateTime lastDateTime =
				HistoricalEndOfDayTimer.GetMarketClose( this.now() );
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
				" - Simulated time: " +
				this.account.Timer.GetCurrentDateTime().ToString() +
				" - Real time: " +
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
		#endregion updateOptimalTestingPositions_actually
		
		private void updateOptimalTestingPositions()
		{
			this.updateOptimalTestingPositions_actually();
			this.lastOptimizationDateTime = this.now();
		}
		#endregion updateOptimalTestingPositions
		
		protected abstract bool arePositionsToBeClosed();
		
		#region arePositionsToBeOpened
		protected ReturnInterval lastIntervalAppended()
		{
			ReturnInterval lastInterval =
				this.outOfSampleReturnIntervals[ this.outOfSampleReturnIntervals.Count - 1 ];
			return lastInterval;
		}
		protected virtual bool arePositionsToBeOpened()
		{
			bool beginsTheLastInterval =
				( this.now() ==
				 this.lastIntervalAppended().Begin );
			bool areToBeOpened =
				( beginsTheLastInterval && this.bestTestingPositionsInSample != null );
			return ( beginsTheLastInterval );
		}
		#endregion arePositionsToBeOpened
		
		protected abstract WeightedPositions getPositionsToBeOpened();
		
		public void NewDateTimeEventHandler( Object sender , DateTime dateTime )
		{
			this.updateReturnIntervals();
			if ( this.areOptimalWeightedPositionsToBeUpdated() )
				this.updateOptimalTestingPositions();
			if ( this.arePositionsToBeClosed() )
				AccountManager.ClosePositions( this.account );
			if ( this.arePositionsToBeOpened() )
			{
				WeightedPositions positionsToBeOpened =
					this.getPositionsToBeOpened();
				if ( positionsToBeOpened != null )
					// positions to be opened are available, actually
					AccountManager.OpenPositions(
						positionsToBeOpened , this.account , this.account.CashAmount );
			}
		}
		#endregion NewDateTimeEventHandler
		
		/// <summary>
		/// returns the current simulated time for the current simulated day
		/// </summary>
		/// <returns></returns>
		protected Time time()
		{
			DateTime simulatedDateTime =
				this.account.Timer.GetCurrentDateTime();
			Time simulatedTime = new Time( simulatedDateTime );
			return simulatedTime;
		}
	}
}
