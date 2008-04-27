/*
QuantProject - Quantitative Finance Library

PairsTradingStrategy.cs
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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;


namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Pairs Trading strategy with in sample optimizations
	/// </summary>
	public class PairsTradingStrategy : SymmetricEndOfDayStrategyForBacktester
	{
//		private WeightedPositions positionsToBeOpened;

//		private double
//			minThresholdForGoingLong,
//			maxThresholdForGoingLong,
//			minThresholdForGoingShort,
//			maxThresholdForGoingShort;
			
		private IHistoricalQuoteProvider
			historicalQuoteProviderForChosingPositionsOutOfSample;
		private OutOfSampleChooser outOfSampleChooser;

		public PairsTradingStrategy(
			int numDaysBeetweenEachOtpimization ,
			int numDaysForInSampleOptimization ,
			IIntervalsSelector intervalsSelector ,
			IEligiblesSelector eligiblesSelector ,
			IInSampleChooser inSampleChooser ,
			IHistoricalQuoteProvider historicalQuoteProviderForInSample ,
			IHistoricalQuoteProvider
			historicalQuoteProviderForChosingPositionsOutOfSample ,
			OutOfSampleChooser outOfSampleChooser ) :
			base(
			numDaysBeetweenEachOtpimization ,
			numDaysForInSampleOptimization ,
			intervalsSelector ,
			eligiblesSelector ,
			inSampleChooser ,
			historicalQuoteProviderForInSample )
		{
//			this.minThresholdForGoingLong = minThresholdForGoingLong;
//			this.maxThresholdForGoingLong = maxThresholdForGoingLong;
//			this.minThresholdForGoingShort = minThresholdForGoingShort;
//			this.maxThresholdForGoingShort = maxThresholdForGoingShort;

			this.historicalQuoteProviderForChosingPositionsOutOfSample =
				historicalQuoteProviderForChosingPositionsOutOfSample;			
			this.outOfSampleChooser = outOfSampleChooser;
		}
		
		protected override string getTextIdentifier()
		{
			return "pairsTrdng";
		}
		
		#region arePositionsToBeClosed
		private ReturnInterval getSecondLastReturnInterval()
		{
			int indexForTheSecondLastReturnInterval =
				this.returnIntervals.Count - 2;
			ReturnInterval secondLastReturnInterval =
				this.returnIntervals[ indexForTheSecondLastReturnInterval ];
			return secondLastReturnInterval;
		}
		protected override bool arePositionsToBeClosed()
		{
			bool areToBeClosed = false;
			if ( this.returnIntervals.Count > 1 )
				// at least two intervals have already been
				// added, out of sample
			{
				ReturnInterval secondLastReturnInterval =
					this.getSecondLastReturnInterval();
				areToBeClosed = this.now().IsEqualTo( secondLastReturnInterval.End );				
			}
			return ( areToBeClosed );
		}
		#endregion arePositionsToBeClosed

//		protected override bool marketOpenEventHandler_arePositionsToBeOpened()
//		{
//			return this.arePositionsToBeOpened();
//		}
//		protected override bool marketCloseEventHandler_arePositionsToBeOpened()
//		{
//			return this.arePositionsToBeOpened();
//		}
		
		protected override bool arePositionsToBeOpened()
		{
			bool beginsTheLastInterval =
				( this.now().IsEqualTo(
				this.lastIntervalAppended().Begin ) );
			return ( beginsTheLastInterval );
		}

//		#region getPositionsToBeOpened
//		#region getReturnsManagerForLastSecondPhaseInterval
//		private EndOfDayDateTime
//			getIntervalBeginForLastSecondPhaseInterval()
//		{
//			// this method will be invoked only if (this.returnIntervals.Count >= 2)
//			int secondLastIntervalIndex =
//				this.returnIntervals.Count - 2;
//			ReturnInterval secondLastInterval =
//				this.returnIntervals[ secondLastIntervalIndex ];
//			return secondLastInterval.End;
//		}
//		private EndOfDayDateTime
//			getIntervalEndForLastSecondPhaseInterval()
//		{
//			return this.lastIntervalAppended().Begin;
//		}
//		private ReturnInterval
//			getReturnIntervalForLastSecondPhaseInterval()
//		{
//			EndOfDayDateTime intervalBegin =
//				this.getIntervalBeginForLastSecondPhaseInterval();
//			EndOfDayDateTime intervalEnd =
//				this.getIntervalEndForLastSecondPhaseInterval();
//			ReturnInterval returnIntervalForLastSecondPhaseInterval =
//				new ReturnInterval( intervalBegin , intervalEnd );
//			return returnIntervalForLastSecondPhaseInterval;
//		}
//		private ReturnIntervals
//			getReturnIntervalsForLastSecondPhaseInterval()
//		{
//			ReturnInterval returnIntervalForLastSecondPhaseInterval =
//				this.getReturnIntervalForLastSecondPhaseInterval();
//			ReturnIntervals returnIntervalsForLastSecondPhaseInterval =
//				new ReturnIntervals( returnIntervalForLastSecondPhaseInterval );
//			return returnIntervalsForLastSecondPhaseInterval;
//		}
//		private ReturnsManager getReturnsManagerForLastSecondPhaseInterval()
//		{
//			ReturnIntervals returnIntervals =
//				this.getReturnIntervalsForLastSecondPhaseInterval();
////			ReturnsManager returnsManager =
////				new ReturnsManager( returnIntervals ,
////				this.historicalAdjustedQuoteProvider );
//			ReturnsManager returnsManager =
//				new ReturnsManager( returnIntervals ,
//				this.historicalQuoteProviderForChosingPositionsOutOfSample );
//			return returnsManager;
//		}
//		#endregion getReturnsManagerForLastSecondPhaseInterval
//		private double getReturnForTheLastSecondPhaseInterval(
//			ReturnsManager returnsManager ,
//			WeightedPositions weightedPositions )
//		{
//			// returnsManager should contain a single ReturnInterval, and
//			// this ReturnInterval should be the last second phase interval
//			double returnForTheLastSecondPhaseInterval =
//				weightedPositions.GetReturn( 0 , returnsManager );
//			return returnForTheLastSecondPhaseInterval;
//		}
//
//		/// <summary>
//		/// Inverts one of the two positions
//		/// </summary>
//		/// <param name="weightedPositions"></param>
//		/// <returns></returns>
//		private WeightedPositions getCandidateForPortfolio(
//			WeightedPositions weightedPositions )
//		{
//			double[] weights = new double[ 2 ];
//			weights[ 0 ] = ((WeightedPosition)weightedPositions[ 0 ]).Weight;
//			weights[ 1 ] = -((WeightedPosition)weightedPositions[ 1 ]).Weight;
//			string[] tickers = new String[ 2 ];
//			tickers[ 0 ] = ((WeightedPosition)weightedPositions[ 0 ]).Ticker;
//			tickers[ 1 ] = ((WeightedPosition)weightedPositions[ 1 ]).Ticker;
//			WeightedPositions candidateForPortfolio =
//				new WeightedPositions( weights , tickers );
//			return candidateForPortfolio;
//		}
//		// if the currentWeightedPositions' return satisfies the thresholds
//		// then this method returns the WeightedPositions to be opened.
//		// Otherwise (currentWeightedPositions' return does NOT
//		// satisfy the thresholds) this method returns null
//		private WeightedPositions
//			getPositionsToBeOpenedWithRespectToCurrentWeightedPositions(
//			ReturnsManager returnsManager ,
//			WeightedPositions currentWeightedPositions )
//		{
//			WeightedPositions weightedPositionsToBeOpened = null;
//			try
//			{
//				double returnForTheLastSecondPhaseInterval =
//					this.getReturnForTheLastSecondPhaseInterval(
//					returnsManager ,
//					currentWeightedPositions );
//				if ( ( returnForTheLastSecondPhaseInterval >=
//					this.minThresholdForGoingLong ) &&
//					( returnForTheLastSecondPhaseInterval <=
//					this.maxThresholdForGoingLong ) )
//					// it looks like there has been an inefficiency that
//					// might be recovered, by going short
//					weightedPositionsToBeOpened = currentWeightedPositions.Opposite;
//				if ( ( -returnForTheLastSecondPhaseInterval >=
//					this.minThresholdForGoingShort ) &&
//					( -returnForTheLastSecondPhaseInterval <=
//					this.maxThresholdForGoingShort ) )
//					// it looks like there has been an inefficiency that
//					// might be recovered, by going long
//					weightedPositionsToBeOpened = currentWeightedPositions;
//			}
//			catch( TickerNotExchangedException ex )
//			{
//				string dummy = ex.Message;
//			}
//			return weightedPositionsToBeOpened;
//		}
//		private WeightedPositions
//			getPositionsToBeOpenedWithRespectToCurrentWeightedPositions(
//			ReturnsManager returnsManager ,
//			int currentTestingPositionsIndex )
//		{
//			WeightedPositions currentWeightedPositions =
//				this.bestTestingPositionsInSample[ currentTestingPositionsIndex ].WeightedPositions;
//			WeightedPositions candidateForPortfolio =
//				this.getCandidateForPortfolio( currentWeightedPositions );
//			WeightedPositions weightedPositionsToBeOpended =
//				this.getPositionsToBeOpenedWithRespectToCurrentWeightedPositions(
//				returnsManager , candidateForPortfolio );
//			return weightedPositionsToBeOpended;
//		}
//		protected WeightedPositions getPositionsToBeOpened(
//			ReturnsManager returnsManager )
//		{
//			WeightedPositions weightedPositionsToBeOpended = null;
//			int currentTestingPositionsIndex = 0;
//			while ( ( weightedPositionsToBeOpended == null )
//				&& ( currentTestingPositionsIndex < this.bestTestingPositionsInSample.Length ) )
//			{
//				weightedPositionsToBeOpended =
//					this.getPositionsToBeOpenedWithRespectToCurrentWeightedPositions(
//					returnsManager , currentTestingPositionsIndex );
//				currentTestingPositionsIndex++;
//			}
//			return weightedPositionsToBeOpended;
//		}
//		private WeightedPositions
//			getPositionsToBeOpened_withAtLeastASecondPhaseInterval_actually()
//		{
//			ReturnsManager returnsManager =
//				this.getReturnsManagerForLastSecondPhaseInterval();
//			WeightedPositions weightedPositions =
//				this.getPositionsToBeOpened( returnsManager );
//			return weightedPositions;
//		}
//		/// <summary>
//		/// To be overriden if a subset of the positions has to be returned
//		/// </summary>
//		/// <param name="weightedPositions"></param>
//		/// <returns></returns>
//		protected virtual WeightedPositions selectWeightedPositions(
//			WeightedPositions weightedPositions )
//		{
//			return weightedPositions;
//		}
//		private WeightedPositions
//			getPositionsToBeOpened_withAtLeastASecondPhaseInterval()
//		{
//			WeightedPositions weightedPositions =
//				this.getPositionsToBeOpened_withAtLeastASecondPhaseInterval_actually();
//			WeightedPositions weightedPositionsToBeReturned = null;
//			if ( weightedPositions != null )
//				// at least one of the BestTestingPositions shows an inefficiency
//				// above the threshold
//				weightedPositionsToBeReturned =
//					selectWeightedPositions( weightedPositions );
//			return weightedPositionsToBeReturned;
//		}
//		protected override WeightedPositions getPositionsToBeOpened()
//		{
//			WeightedPositions weightedPositions = null;
//			if ( this.returnIntervals.Count >= 2 )
//				// at least a second phase interval exists
//				weightedPositions =
//					this.getPositionsToBeOpened_withAtLeastASecondPhaseInterval();
//			return weightedPositions;
//		}
//		#endregion

		protected override WeightedPositions getPositionsToBeOpened()
		{
			WeightedPositions weightedPositions =
				this.outOfSampleChooser.GetPositionsToBeOpened(
				this.bestTestingPositionsInSample ,
				this.returnIntervals ,
				this.historicalQuoteProviderForChosingPositionsOutOfSample ,
				this.inSampleReturnsManager );
			return weightedPositions;
		}
		
		protected override LogItem getLogItem( EligibleTickers eligibleTickers )
		{
			PairsTradingLogItem logItem =
				new PairsTradingLogItem( this.now() ,
				                        this.bestTestingPositionsInSample ,
				                        this.numDaysForInSampleOptimization ,
				                        eligibleTickers.Count );
//			logItem.BestWeightedPositionsInSample =
//				this.bestTestingPositionsInSample.WeightedPositions;
//			logItem.NumberOfEligibleTickers =
//				eligibleTickers.Count;
			return logItem;
		}
	}
}
