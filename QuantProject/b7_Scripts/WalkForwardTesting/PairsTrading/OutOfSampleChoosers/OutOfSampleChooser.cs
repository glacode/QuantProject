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
		
		private void checkParameters(
			DateTime firstDateTimeToTestInefficiency ,
			DateTime lastDateTimeToTestInefficiency ,
			DateTime dateTimeToClosePositions )
		{
			if ( firstDateTimeToTestInefficiency >= lastDateTimeToTestInefficiency )
				throw new Exception(
					"The first date time to test inefficiency must be strictly before " +
					"the last date time to test inefficiency." );
			if ( lastDateTimeToTestInefficiency >= dateTimeToClosePositions )
				throw new Exception(
					"The last date time to test inefficiency must be strictly before " +
					"the date time to close positions." );
		}
		
		#region getPositionsToBeOpened_actually

		#region getInefficientCouples
		
		#region getArrayListOfInefficientCouples

		#region getReturnsManagerForLastSecondPhaseInterval
//		private DateTime
//			getIntervalBeginForLastSecondPhaseInterval(
//				ReturnIntervals outOfSampleReturnIntervals )
//		{
//			// this method will be invoked only if (this.returnIntervals.Count >= 2)
//			int secondLastIntervalIndex =
//				outOfSampleReturnIntervals.Count - 2;
//			ReturnInterval secondLastInterval =
//				outOfSampleReturnIntervals[ secondLastIntervalIndex ];
//			return secondLastInterval.End;
//		}
//		private DateTime
//			getIntervalEndForLastSecondPhaseInterval(
//				ReturnIntervals outOfSampleReturnIntervals )
//		{
//			return outOfSampleReturnIntervals.LastInterval.Begin;
//		}
		
		#region getReturnIntervalsForLastSecondPhaseInterval
		private ReturnInterval
			getReturnIntervalForLastSecondPhaseInterval(
				DateTime firstDateTimeToTestInefficiency ,
				DateTime lastDateTimeToTestInefficiency
//				ReturnIntervals outOfSampleReturnIntervals
			)
		{
//			DateTime intervalBegin =
//				this.getIntervalBeginForLastSecondPhaseInterval(
//					outOfSampleReturnIntervals );
//			DateTime intervalEnd =
//				this.getIntervalEndForLastSecondPhaseInterval(
//					outOfSampleReturnIntervals );
			ReturnInterval returnIntervalForLastSecondPhaseInterval =
				new ReturnInterval(
					firstDateTimeToTestInefficiency , lastDateTimeToTestInefficiency );
			return returnIntervalForLastSecondPhaseInterval;
		}
		private ReturnIntervals
			getReturnIntervalsForLastSecondPhaseInterval(
				DateTime firstDateTimeToTestInefficiency ,
				DateTime lastDateTimeToTestInefficiency
//				ReturnIntervals outOfSampleReturnIntervals
			)
		{
			ReturnInterval returnIntervalForLastSecondPhaseInterval =
				this.getReturnIntervalForLastSecondPhaseInterval(
					firstDateTimeToTestInefficiency ,
					lastDateTimeToTestInefficiency
//					outOfSampleReturnIntervals
				);
			ReturnIntervals returnIntervalsForLastSecondPhaseInterval =
				new ReturnIntervals( returnIntervalForLastSecondPhaseInterval );
			return returnIntervalsForLastSecondPhaseInterval;
		}
		#endregion getReturnIntervalsForLastSecondPhaseInterval
		
		private ReturnsManager getReturnsManagerForLastSecondPhaseInterval(
			DateTime firstDateTimeToTestInefficiency ,
			DateTime lastDateTimeToTestInefficiency ,
//			ReturnIntervals outOfSampleReturnIntervals ,
			HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample )
		{
			ReturnIntervals returnIntervals =
				this.getReturnIntervalsForLastSecondPhaseInterval(
					firstDateTimeToTestInefficiency ,
					lastDateTimeToTestInefficiency
//					outOfSampleReturnIntervals
				);
			ReturnsManager returnsManager =
				new ReturnsManager( returnIntervals ,
				                   historicalMarketValueProviderForChosingPositionsOutOfSample );
			return returnsManager;
		}
		#endregion getReturnsManagerForLastSecondPhaseInterval
		
		#region getInefficientCouples
		
		#region areAllNeededMarketValuesAvailableForTheCurrentCouple
		private bool areAllNeededMarketValuesAvailableForTheCurrentCouple(
			TestingPositions currentCouple ,
			DateTime dateTimeToClosePositions ,
			HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample )
		{
			bool areAllAvailable = true;
			foreach ( WeightedPosition weightedPosition in
			         currentCouple.WeightedPositions.Values )
				areAllAvailable =
					(
						areAllAvailable &&
						// attention! we are looking in the future here, but we do it
						// just to avoid picking a ticker for which we don't have
						// the market value when we will close the positions
						historicalMarketValueProviderForChosingPositionsOutOfSample.WasExchanged(
							weightedPosition.Ticker , dateTimeToClosePositions )
					);
			return areAllAvailable;
		}
		private bool areAllNeededMarketValuesAvailableForTheCurrentCouple(
			TestingPositions[] bestTestingPositionsInSample ,
			int currentTestingPositionsIndex ,
			DateTime dateTimeToClosePositions ,
			HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample )
		{
			TestingPositions currentCouple =
				bestTestingPositionsInSample[ currentTestingPositionsIndex ];
			bool areAllAvailable =
				this.areAllNeededMarketValuesAvailableForTheCurrentCouple(
					currentCouple , dateTimeToClosePositions ,
					historicalMarketValueProviderForChosingPositionsOutOfSample );
			return areAllAvailable;
		}
		#endregion areAllNeededMarketValuesAvailableForTheCurrentCouple

		#region addPositionsIfInefficiencyForCurrentCoupleIsInTheRange
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
		
		#region getPositionsIfInefficiencyIsInTheRange
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
				      this.minThresholdForGoingShort ) &&
				    ( returnForTheLastSecondPhaseInterval <=
				     this.maxThresholdForGoingShort ) )
					// it looks like there has been an inefficiency that
					// might be recovered, by going short
					weightedPositionsToBeOpened = currentWeightedPositions.Opposite;
				if ( ( -returnForTheLastSecondPhaseInterval >=
				      this.minThresholdForGoingLong ) &&
				    ( -returnForTheLastSecondPhaseInterval <=
				     this.maxThresholdForGoingLong ) )
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
		#endregion getPositionsIfInefficiencyIsInTheRange
		
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
		#endregion addPositionsIfInefficiencyForCurrentCoupleIsInTheRange
		
		protected ArrayList getInefficientCouples(
			TestingPositions[] bestTestingPositionsInSample ,
			ReturnsManager returnsManagerForLastSecondPhaseInterval ,
			DateTime dateTimeToClosePositions ,
			HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample )
		{
			ArrayList inefficientCouples = new ArrayList();
			for ( int currentTestingPositionsIndex = 0 ;
			     currentTestingPositionsIndex < bestTestingPositionsInSample.Length ;
			     currentTestingPositionsIndex++ )
			{
				if ( this.areAllNeededMarketValuesAvailableForTheCurrentCouple(
					bestTestingPositionsInSample ,
					currentTestingPositionsIndex ,
					dateTimeToClosePositions ,
					historicalMarketValueProviderForChosingPositionsOutOfSample ) )
					this.addPositionsIfInefficiencyForCurrentCoupleIsInTheRange(
						bestTestingPositionsInSample ,
						returnsManagerForLastSecondPhaseInterval ,
						currentTestingPositionsIndex ,
						inefficientCouples );
			}
			return inefficientCouples;
		}
		#endregion getInefficientCouples
		
		private ArrayList
			getArrayListOfInefficientCouples(
				TestingPositions[] bestTestingPositionsInSample ,
				DateTime firstDateTimeToTestInefficiency ,
				DateTime lastDateTimeToTestInefficiency ,
				DateTime dateTimeToClosePositions ,
//				ReturnIntervals outOfSampleReturnIntervals ,
				HistoricalMarketValueProvider
				historicalMarketValueProviderForChosingPositionsOutOfSample )
		{
			ReturnsManager returnsManagerForLastSecondPhaseInterval =
				this.getReturnsManagerForLastSecondPhaseInterval(
					firstDateTimeToTestInefficiency ,
					lastDateTimeToTestInefficiency ,
//					outOfSampleReturnIntervals ,
					historicalMarketValueProviderForChosingPositionsOutOfSample );
			ArrayList inefficientCouples =
				this.getInefficientCouples(
					bestTestingPositionsInSample ,
					returnsManagerForLastSecondPhaseInterval ,
					dateTimeToClosePositions ,
					historicalMarketValueProviderForChosingPositionsOutOfSample );
			return inefficientCouples;
		}
		#endregion getArrayListOfInefficientCouples
		
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
				DateTime firstDateTimeToTestInefficiency ,
				DateTime lastDateTimeToTestInefficiency ,
				DateTime dateTimeToClosePositions ,
//				ReturnIntervals outOfSampleReturnIntervals ,
				HistoricalMarketValueProvider
				historicalMarketValueProviderForChosingPositionsOutOfSample )
		{
			ArrayList arrayListOfInefficientCouples =
				this.getArrayListOfInefficientCouples(
					bestTestingPositionsInSample ,
					firstDateTimeToTestInefficiency ,
					lastDateTimeToTestInefficiency ,
					dateTimeToClosePositions ,
//					outOfSampleReturnIntervals ,
					historicalMarketValueProviderForChosingPositionsOutOfSample );
			WeightedPositions[] inefficientCouples =
				this.getInefficientCouplesFromArrayList( arrayListOfInefficientCouples );
			return inefficientCouples;
		}
		private WeightedPositions[] getInefficientCouples(
			TestingPositions[] bestTestingPositionsInSample ,
			DateTime firstDateTimeToTestInefficiency ,
			DateTime lastDateTimeToTestInefficiency ,
			DateTime dateTimeToClosePositions ,
//			ReturnIntervals outOfSampleReturnIntervals ,
			HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample )
		{
			WeightedPositions[] inefficientCouples = null;
//			if ( outOfSampleReturnIntervals.Count >= 2 )
//				// at least a second phase interval exists
			inefficientCouples =
				this.getInefficientCouples_withAtLeastASecondPhaseInterval(
					bestTestingPositionsInSample ,
					firstDateTimeToTestInefficiency ,
					lastDateTimeToTestInefficiency ,
					dateTimeToClosePositions ,
//						outOfSampleReturnIntervals ,
					historicalMarketValueProviderForChosingPositionsOutOfSample );
			return inefficientCouples;

		}
		#endregion getInefficientCouples
		
		private WeightedPositions getPositionsToBeOpened_actually(
			TestingPositions[] bestTestingPositionsInSample ,
			DateTime firstDateTimeToTestInefficiency ,
			DateTime lastDateTimeToTestInefficiency ,
			DateTime dateTimeToClosePositions ,
//			ReturnIntervals outOfSampleReturnIntervals ,
			HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample ,
			ReturnsManager inSampleReturnsManager )
		{
			WeightedPositions positionsToBeOpened = null;
			WeightedPositions[] inefficientCouples =
				this.getInefficientCouples(
					bestTestingPositionsInSample ,
					firstDateTimeToTestInefficiency ,
					lastDateTimeToTestInefficiency ,
					dateTimeToClosePositions ,
//					outOfSampleReturnIntervals ,
					historicalMarketValueProviderForChosingPositionsOutOfSample );
			if ( inefficientCouples != null )
				// at least an inefficient couple has been found
				positionsToBeOpened =
					this.getPositionsToBeOpened( inefficientCouples ,
					                            inSampleReturnsManager );
			return positionsToBeOpened;
		}
		#endregion getPositionsToBeOpened_actually
		
		/// <summary>
		/// Selects the WeghtedPositions to actually be opened
		/// </summary>
		/// <param name="bestTestingPositionsInSample"></param>
		/// <param name="firstDateTimeToTestInefficiency"></param>
		/// <param name="lastDateTimeToTestInefficiency"></param>
		/// <param name="timeToClosePositins"></param>
		/// <param name="historicalMarketValueProviderForChosingPositionsOutOfSample"></param>
		/// <param name="inSampleReturnsManager"></param>
		/// <returns></returns>
		public virtual WeightedPositions GetPositionsToBeOpened(
			TestingPositions[] bestTestingPositionsInSample ,
			DateTime firstDateTimeToTestInefficiency ,
			DateTime lastDateTimeToTestInefficiency ,
			DateTime dateTimeToClosePositions ,
//			ReturnIntervals outOfSampleReturnIntervals ,
			HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample ,
			ReturnsManager inSampleReturnsManager )
		{
			this.checkParameters(
				firstDateTimeToTestInefficiency ,
				lastDateTimeToTestInefficiency ,
				dateTimeToClosePositions );
			WeightedPositions positionsToBeOpened =
				this.getPositionsToBeOpened_actually(
					bestTestingPositionsInSample ,
					firstDateTimeToTestInefficiency ,
					lastDateTimeToTestInefficiency ,
					dateTimeToClosePositions ,
					historicalMarketValueProviderForChosingPositionsOutOfSample ,
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
