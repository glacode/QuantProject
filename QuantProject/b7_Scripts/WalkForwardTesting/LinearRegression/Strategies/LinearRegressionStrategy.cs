/*
QuantProject - Quantitative Finance Library

LinearRegressionStrategy.cs
Copyright (C) 2010
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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Builds a model for prediction of a WeightedPositions return on the next
	/// return interval
	/// </summary>
	[Serializable]
	public class LinearRegressionStrategy : BasicStrategyForBacktester
	{
		private IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling;
		private IEntryStrategy entryStrategy;
		private IExitStrategy exitStrategy;

		public LinearRegressionStrategy(
			int numDaysBeetweenEachOtpimization ,
			int numDaysForInSampleOptimization ,
			IIntervalsSelector intervalsSelectorForInSample ,
			IIntervalsSelector intervalsSelectorForOutOfSample ,
			IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling ,
			IEligiblesSelector eligiblesSelectorForTradingTickers ,
//			IEligiblesSelector eligiblesSelectorForSignalingTickers ,
			IInSampleChooser inSampleChooser ,
			HistoricalMarketValueProvider historicalMarketValueProviderForInSample ,
			HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample ,
			IEntryStrategy entryStrategy ,
//			OutOfSampleChooser outOfSampleChooser ,
			IExitStrategy exitStrategy ) :
			base(
				numDaysBeetweenEachOtpimization ,
				numDaysForInSampleOptimization ,
				intervalsSelectorForInSample ,
				intervalsSelectorForOutOfSample ,
				eligiblesSelectorForTradingTickers ,
				inSampleChooser ,
				historicalMarketValueProviderForInSample )
		{
			this.intervalsSelectorForOutOfSample = intervalsSelectorForOutOfSample;
			this.returnIntervalSelectorForSignaling = returnIntervalSelectorForSignaling;
			this.entryStrategy = entryStrategy;
			this.exitStrategy = exitStrategy;
		}
		
		protected override string getTextIdentifier()
		{
			return "lnrRgrssn";
		}
		
		protected override LogItem getLogItem( EligibleTickers eligibleTickers )
		{
			LinearRegressionLogItem logItem =
				new LinearRegressionLogItem(
					this.now() ,
					this.bestTestingPositionsInSample ,
					(DateTime)this.inSampleReturnsManager.ReturnIntervals.BordersHistory.GetByIndex( 0 ) ,
					this.intervalsSelectorForInSample ,
					this.returnIntervalSelectorForSignaling );
//					this.numDaysForInSampleOptimization ,
//					eligibleTickers.Count );
			return logItem;
		}

		protected override bool arePositionsToBeClosed()
		{
			bool areToBeClosed =
				( this.Account.Portfolio.Count > 0 );
			areToBeClosed = (
				areToBeClosed &&
				this.exitStrategy.ArePositionsToBeClosed(
					this.now() , this.outOfSampleReturnIntervals ) );
//				( this.time() == this.timeToClosePositions ) );
			return ( areToBeClosed );
		}
		
		protected override bool arePositionsToBeOpened()
		{
			bool areToBeOpened = false;
			if ( this.outOfSampleReturnIntervals.Count >= 1 &&
			    this.bestTestingPositionsInSample != null )
			{
//				ReturnInterval seconLastInterval =
//					this.outOfSampleReturnIntervals.SeconLastInterval;
//				areToBeOpened = ( this.now() == seconLastInterval.Begin );
				ReturnInterval lastOutOfSampleInterval =
					this.outOfSampleReturnIntervals.LastInterval;
				areToBeOpened = ( this.now() == lastOutOfSampleInterval.Begin );
			}
			return areToBeOpened;
		}
		
		protected override WeightedPositions getPositionsToBeOpened()
		{
//			DateTime currentDateTime = this.now();
//			WeightedPositions weightedPositions =
//				this.outOfSampleChooser.GetPositionsToBeOpened(
//					this.bestTestingPositionsInSample ,
			////					firstDateTimeToTestInefficiency ,
//					currentDateTime ,
			////					dateTimeToClosePositions ,
//					this.historicalMarketValueProviderForChosingPositionsOutOfSample ,
//					this.inSampleReturnsManager );
			WeightedPositions weightedPositions = this.entryStrategy.GetPositionsToBeOpened(
				this.bestTestingPositionsInSample , this.outOfSampleReturnIntervals );
			return weightedPositions;
		}

	}
}
