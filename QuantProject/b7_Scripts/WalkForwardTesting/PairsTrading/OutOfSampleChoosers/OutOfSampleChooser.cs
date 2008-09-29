/*
QuantProject - Quantitative Finance Library

OutOfSampleChooser.cs
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
using System.Collections;

using QuantProject.ADT.Collections;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Given the in sample TestingPositions candidates,
	/// this class selects the positions to be opened
	/// </summary>
	[Serializable]
	public abstract class OutOfSampleChooser
	{
		private double minThresholdForGoingLong;
		private double maxThresholdForGoingLong;
		private double minThresholdForGoingShort;
		private double maxThresholdForGoingShort;


		public OutOfSampleChooser(
			double minThresholdForGoingLong ,
			double maxThresholdForGoingLong ,
			double minThresholdForGoingShort ,
			double maxThresholdForGoingShort )
		{
			this.minThresholdForGoingLong = minThresholdForGoingLong;
			this.maxThresholdForGoingLong = maxThresholdForGoingLong;
			this.minThresholdForGoingShort = minThresholdForGoingShort;
			this.maxThresholdForGoingShort = maxThresholdForGoingShort;
		}

		/// <summary>
		/// returns the positions to be opened
		/// </summary>
		/// <param name="inefficientCouples">a collection of couples that
		/// are strongly correlated in sample, but were not so
		/// correlated in the latest second phase interval</param>
		/// <param name="inSampleReturnsManager"></param>
		/// <returns></returns>
		protected abstract WeightedPositions getPositionsToBeOpened(
			WeightedPositions[] inefficientCouples ,
			ReturnsManager inSampleReturnsManager );

		#region GetPositionsToBeOpened

		#region getInefficientCouples

		#region getReturnsManagerForLastSecondPhaseInterval
		private DateTime
			getIntervalBeginForLastSecondPhaseInterval(
			ReturnIntervals outOfSampleReturnIntervals )
		{
			// this method will be invoked only if (this.returnIntervals.Count >= 2)
			int secondLastIntervalIndex =
				outOfSampleReturnIntervals.Count - 2;
			ReturnInterval secondLastInterval =
				outOfSampleReturnIntervals[ secondLastIntervalIndex ];
			return secondLastInterval.End;
		}
		private DateTime
			getIntervalEndForLastSecondPhaseInterval(
			ReturnIntervals outOfSampleReturnIntervals )
		{
			return outOfSampleReturnIntervals.LastInterval.Begin;
		}
		private ReturnInterval
			getReturnIntervalForLastSecondPhaseInterval(
			ReturnIntervals outOfSampleReturnIntervals )
		{
			DateTime intervalBegin =
				this.getIntervalBeginForLastSecondPhaseInterval(
				outOfSampleReturnIntervals );
			DateTime intervalEnd =
				this.getIntervalEndForLastSecondPhaseInterval(
				outOfSampleReturnIntervals );
			ReturnInterval returnIntervalForLastSecondPhaseInterval =
				new ReturnInterval( intervalBegin , intervalEnd );
			return returnIntervalForLastSecondPhaseInterval;
		}
		private ReturnIntervals
			getReturnIntervalsForLastSecondPhaseInterval(
			ReturnIntervals outOfSampleReturnIntervals )
		{
			ReturnInterval returnIntervalForLastSecondPhaseInterval =
				this.getReturnIntervalForLastSecondPhaseInterval(
				outOfSampleReturnIntervals );
			ReturnIntervals returnIntervalsForLastSecondPhaseInterval =
				new ReturnIntervals( returnIntervalForLastSecondPhaseInterval );
			return returnIntervalsForLastSecondPhaseInterval;
		}
		private ReturnsManager getReturnsManagerForLastSecondPhaseInterval(
			ReturnIntervals outOfSampleReturnIntervals ,
			HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample )
		{
			ReturnIntervals returnIntervals =
				this.getReturnIntervalsForLastSecondPhaseInterval(
				outOfSampleReturnIntervals );
			//			ReturnsManager returnsManager =
			//				new ReturnsManager( returnIntervals ,
			//				this.historicalAdjustedQuoteProvider );
			ReturnsManager returnsManager =
				new ReturnsManager( returnIntervals ,
				historicalMarketValueProviderForChosingPositionsOutOfSample );
			return returnsManager;
		}
		#endregion getReturnsManagerForLastSecondPhaseInterval
		private double getReturnForTheLastSecondPhaseInterval(
			ReturnsManager returnsManagerForLastSecondPhaseInterval ,
			WeightedPositions weightedPositions )
		{
			// returnsManager should contain a single ReturnInterval, and
			// this ReturnInterval should be the last second phase interval
			double returnForTheLastSecondPhaseInterval =
				weightedPositions.GetReturn( 0 ,
				returnsManagerForLastSecondPhaseInterval );
			return returnForTheLastSecondPhaseInterval;
		}

		/// <summary>
		/// Inverts one of the two positions
		/// </summary>
		/// <param name="weightedPositions"></param>
		/// <returns></returns>
		private WeightedPositions getCandidateForPortfolio(
			WeightedPositions weightedPositions )
		{
			double[] weights = new double[ 2 ];
			weights[ 0 ] = ((WeightedPosition)weightedPositions[ 0 ]).Weight;
			weights[ 1 ] = -((WeightedPosition)weightedPositions[ 1 ]).Weight;
			string[] tickers = new String[ 2 ];
			tickers[ 0 ] = ((WeightedPosition)weightedPositions[ 0 ]).Ticker;
			tickers[ 1 ] = ((WeightedPosition)weightedPositions[ 1 ]).Ticker;
			WeightedPositions candidateForPortfolio =
				new WeightedPositions( weights , tickers );
			return candidateForPortfolio;
		}
		// if the currentWeightedPositions' return satisfies the thresholds
		// then this method returns the WeightedPositions that might be opened.
		// Otherwise (currentWeightedPositions' return does NOT
		// satisfy the thresholds) this method returns null
		private WeightedPositions
			getPositionsIfInefficiencyIsInTheRange(
			ReturnsManager returnsManagerForLastSecondPhaseInterval ,
			WeightedPositions currentWeightedPositions )
		{
			WeightedPositions weightedPositionsToBeOpened = null;
			try
			{
				double returnForTheLastSecondPhaseInterval =
					this.getReturnForTheLastSecondPhaseInterval(
					returnsManagerForLastSecondPhaseInterval ,
					currentWeightedPositions );
				if ( ( returnForTheLastSecondPhaseInterval >=
					this.minThresholdForGoingLong ) &&
					( returnForTheLastSecondPhaseInterval <=
					this.maxThresholdForGoingLong ) )
					// it looks like there has been an inefficiency that
					// might be recovered, by going short
					weightedPositionsToBeOpened = currentWeightedPositions.Opposite;
				if ( ( -returnForTheLastSecondPhaseInterval >=
					this.minThresholdForGoingShort ) &&
					( -returnForTheLastSecondPhaseInterval <=
					this.maxThresholdForGoingShort ) )
					// it looks like there has been an inefficiency that
					// might be recovered, by going long
					weightedPositionsToBeOpened = currentWeightedPositions;
			}
			catch( TickerNotExchangedException ex )
			{
				string dummy = ex.Message;
			}
			return weightedPositionsToBeOpened;
		}
		private void
			addPositionsIfInefficiencyForCurrentCoupleIsInTheRange(
			TestingPositions[] bestTestingPositionsInSample ,
			ReturnsManager returnsManagerForLastSecondPhaseInterval ,
			int currentTestingPositionsIndex ,
			ArrayList inefficientCouples )
		{
			WeightedPositions currentWeightedPositions =
				bestTestingPositionsInSample[ currentTestingPositionsIndex ].WeightedPositions;
			WeightedPositions candidateForPortfolio =
				this.getCandidateForPortfolio( currentWeightedPositions );
			WeightedPositions weightedPositionsThatMightBeOpended =
				this.getPositionsIfInefficiencyIsInTheRange(
				returnsManagerForLastSecondPhaseInterval , candidateForPortfolio );
			if ( weightedPositionsThatMightBeOpended != null )
				// the current couple has not an inefficiency that's in the range
				inefficientCouples.Add( weightedPositionsThatMightBeOpended );
		}
		protected ArrayList getInefficientCouples(
			TestingPositions[] bestTestingPositionsInSample ,
			ReturnsManager returnsManagerForLastSecondPhaseInterval )
		{
			ArrayList inefficientCouples = new ArrayList();
			for ( int currentTestingPositionsIndex = 0 ;
				currentTestingPositionsIndex < bestTestingPositionsInSample.Length ;
				currentTestingPositionsIndex++ )
				this.addPositionsIfInefficiencyForCurrentCoupleIsInTheRange(
					bestTestingPositionsInSample ,
					returnsManagerForLastSecondPhaseInterval ,
					currentTestingPositionsIndex ,
					inefficientCouples );
//			
//			while ( ( weightedPositionsToBeOpended == null )
//				&& ( currentTestingPositionsIndex < bestTestingPositionsInSample.Length ) )
//			{
//				weightedPositionsToBeOpended =
//					this.getPositionsToBeOpenedWithRespectToCurrentWeightedPositions(
//					bestTestingPositionsInSample ,
//					returnsManagerForLastSecondPhaseInterval ,
//					currentTestingPositionsIndex );
//				currentTestingPositionsIndex++;
//			}
//			return weightedPositionsToBeOpended;
			return inefficientCouples;
		}
		private ArrayList
			getArrayListOfInefficientCouples(
			TestingPositions[] bestTestingPositionsInSample ,
			ReturnIntervals outOfSampleReturnIntervals ,
			HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample )
		{
			ReturnsManager returnsManagerForLastSecondPhaseInterval =
				this.getReturnsManagerForLastSecondPhaseInterval(
				outOfSampleReturnIntervals ,
				historicalMarketValueProviderForChosingPositionsOutOfSample );
			ArrayList inefficientCouples =
				this.getInefficientCouples( bestTestingPositionsInSample ,
				returnsManagerForLastSecondPhaseInterval );
			return inefficientCouples;
		}
		#region getInefficientCouplesFromArrayList
		private WeightedPositions[] getInefficientCouplesFromArrayList_actually(
			ArrayList arrayListOfInefficientCouples )
		{
			WeightedPositions[] inefficientCouples =
				new WeightedPositions[ arrayListOfInefficientCouples.Count ];
			for ( int i = 0 ; i < arrayListOfInefficientCouples.Count ; i++ )
				inefficientCouples[ i ] =
					(WeightedPositions)arrayListOfInefficientCouples[ i ];
			return inefficientCouples;
		}
		private WeightedPositions[] getInefficientCouplesFromArrayList(
			ArrayList arrayListOfInefficientCouples )
		{
			WeightedPositions[] inefficientCouples = null;
			if ( arrayListOfInefficientCouples.Count > 0 )
				// at least a couple was found, with an inefficiency in the range
				inefficientCouples =
					this.getInefficientCouplesFromArrayList_actually(
					arrayListOfInefficientCouples );
			return inefficientCouples;
		}
		#endregion getInefficientCouplesFromArrayList
		private WeightedPositions[]
			getInefficientCouples_withAtLeastASecondPhaseInterval(
			TestingPositions[] bestTestingPositionsInSample ,
			ReturnIntervals outOfSampleReturnIntervals ,
			HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample )
		{
			ArrayList arrayListOfInefficientCouples =
				this.getArrayListOfInefficientCouples(
				bestTestingPositionsInSample ,
				outOfSampleReturnIntervals ,
				historicalMarketValueProviderForChosingPositionsOutOfSample );
			WeightedPositions[] inefficientCouples =
				this.getInefficientCouplesFromArrayList( arrayListOfInefficientCouples );
			return inefficientCouples;
		}
//		private WeightedPositions
//			getInfefficientCouples_withAtLeastASecondPhaseInterval(
//			TestingPositions[] bestTestingPositionsInSample ,
//			ReturnIntervals outOfSampleReturnIntervals ,
//			IHistoricalQuoteProvider
//			historicalQuoteProviderForChosingPositionsOutOfSample )
//		{
//			WeightedPositions weightedPositions =
//				this.getPositionsToBeOpened_withAtLeastASecondPhaseInterval_actually(
//				bestTestingPositionsInSample ,
//				outOfSampleReturnIntervals ,
//				historicalQuoteProviderForChosingPositionsOutOfSample );
////			WeightedPositions weightedPositionsToBeReturned = null;
////			if ( weightedPositions != null )
////				// at least one of the BestTestingPositions shows an inefficiency
////				// above the threshold
////				weightedPositionsToBeReturned =
////					selectWeightedPositions( weightedPositions );
////			return weightedPositionsToBeReturned;
//			return weightedPositions;
//		}
		private WeightedPositions[] getInefficientCouples(
			TestingPositions[] bestTestingPositionsInSample ,
			ReturnIntervals outOfSampleReturnIntervals ,
			HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample )
		{
			WeightedPositions[] inefficientCouples = null;
			if ( outOfSampleReturnIntervals.Count >= 2 )
				// at least a second phase interval exists
				inefficientCouples =
					this.getInefficientCouples_withAtLeastASecondPhaseInterval(
					bestTestingPositionsInSample ,
					outOfSampleReturnIntervals ,
					historicalMarketValueProviderForChosingPositionsOutOfSample );
			return inefficientCouples;

		}
		#endregion getInefficientCouples
		/// <summary>
		/// Selects the WeghtedPositions to actually be opened
		/// </summary>
		/// <param name="bestTestingPositionsInSample">most correlated couples,
		/// in sample</param>
		/// <param name="outOfSampleReturnIntervals">return intervals for
		/// the current backtest</param>
		/// <param name="minThreshold">min requested inefficiency</param>
		/// <param name="maxThreshold">max allowed inefficiency</param>
		/// <param name="inSampleReturnsManager"></param>
		/// <returns></returns>
		public virtual WeightedPositions GetPositionsToBeOpened(
			TestingPositions[] bestTestingPositionsInSample ,
			ReturnIntervals outOfSampleReturnIntervals ,
			HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample ,
			ReturnsManager inSampleReturnsManager )
		{
			WeightedPositions positionsToBeOpened = null;
			WeightedPositions[] inefficientCouples =
				this.getInefficientCouples(
				bestTestingPositionsInSample ,
				outOfSampleReturnIntervals ,
				historicalMarketValueProviderForChosingPositionsOutOfSample );
			if ( inefficientCouples != null )
				// at least an inefficient couple has been found
				positionsToBeOpened =
					this.getPositionsToBeOpened( inefficientCouples ,
					inSampleReturnsManager );
			return positionsToBeOpened;
		}
		#endregion GetPositionsToBeOpened

		#region getLongPositionTickers

		#region getArrayListOfLongPositionTickers
		private void checkParameters_forCurrentCouple(
			WeightedPositions inefficientCouple )
		{
			if ( inefficientCouple.Count != 2 )
				throw new Exception( "The method getLongPositionTickers() expects " +
					"an array of WeightedPositions, each of wich contains exactly " +
					"two positions!" );
		}
		private void addPositionIfLong(
			WeightedPosition weightedPosition ,
			Set alreadyAddedLongPositionTickers ,
			ArrayList arrayListOfLongPositionTickers )
		{
			if ( ( weightedPosition.IsLong ) &&
				( !alreadyAddedLongPositionTickers.Contains(
				weightedPosition.Ticker ) ) )
				// the given weightedPosition is long and its ticker has not
				// yet been added to the set of tickers for long positions
			{
				arrayListOfLongPositionTickers.Add( weightedPosition.Ticker );
				alreadyAddedLongPositionTickers.Add( weightedPosition.Ticker );
			}
		}
		private void addLongPositions(
			WeightedPositions inefficientCouple ,
			Set alreadyAddedLongPositionTickers ,
			ArrayList arrayListOfLongPositionTickers )
		{
			this.checkParameters_forCurrentCouple( inefficientCouple );
			this.addPositionIfLong( inefficientCouple[ 0 ] ,
				alreadyAddedLongPositionTickers ,
				arrayListOfLongPositionTickers );
			this.addPositionIfLong( inefficientCouple[ 1 ] ,
				alreadyAddedLongPositionTickers ,
				arrayListOfLongPositionTickers );
		}
		private ArrayList getArrayListOfLongPositionTickers(
			WeightedPositions[] inefficientCouples )
		{
			Set alreadyAddedLongPositionTickers = new Set();
			ArrayList arrayListOfLongPositionTickers = new ArrayList();
			foreach( WeightedPositions weightedPositions in inefficientCouples )
				this.addLongPositions( weightedPositions ,
					alreadyAddedLongPositionTickers ,
					arrayListOfLongPositionTickers );
			return arrayListOfLongPositionTickers;
		}
		#endregion getArrayListOfLongPositionTickers

		private string[] getLongPositionTickers(
			ArrayList arrayListOfLongPositionTickers )
		{
			string[] longPositionTickers =
				new string[ arrayListOfLongPositionTickers.Count ];
			for( int i = 0; i < arrayListOfLongPositionTickers.Count ; i++ )
				longPositionTickers[ i ] =
					(string)arrayListOfLongPositionTickers[ i ];
			return longPositionTickers;
		}

		/// <summary>
		/// returns a set of tickers for long positions in the
		/// inefficient couples
		/// </summary>
		/// <param name="inefficientCouples"></param>
		/// <returns></returns>
		protected string[] getLongPositionTickers(
			WeightedPositions[] inefficientCouples )
		{
			ArrayList arrayListOfLongPositionTickers =
				this.getArrayListOfLongPositionTickers( inefficientCouples );
			string[] longPositionTickers =
				this.getLongPositionTickers( arrayListOfLongPositionTickers );
			return longPositionTickers;
		}
		#endregion
	}
}
