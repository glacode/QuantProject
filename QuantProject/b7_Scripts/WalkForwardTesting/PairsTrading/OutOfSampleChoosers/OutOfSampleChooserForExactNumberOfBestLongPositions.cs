/*
QuantProject - Quantitative Finance Library

OutOfSampleChooserForExactNumberOfBestLongPositions.cs
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

using QuantProject.ADT.Collections;
using QuantProject.ADT.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Selects an exact number of long position to be used out of sample:
	/// Given the inefficient couples, the long positons
	/// returned are those belonging to the highest correlated couples
	/// </summary>
	[Serializable]
	public class OutOfSampleChooserForExactNumberOfBestLongPositions :
		OutOfSampleChooser
	{
		private int exactNumberOfBestLongPositionsToBeReturned;

		/// <summary>
		/// Returns exactNumberOfBestLongPositions long positions, if at
		/// least exactNumberOfBestLongPositions inefficient couples
		/// were found. Given the inefficient couples, the long positons
		/// returned are those belonging to the highest correlated couples
		/// </summary>
		/// <param name="exactNumberOfBestLongPositions"></param>
		/// <param name="minThresholdForGoingLong"></param>
		/// <param name="maxThresholdForGoingLong"></param>
		/// <param name="minThresholdForGoingShort"></param>
		/// <param name="maxThresholdForGoingShort"></param>
		public OutOfSampleChooserForExactNumberOfBestLongPositions(
			int exactNumberOfBestLongPositionsToBeReturned ,
			Time firstTimeToTestInefficiency ,
			double minThresholdForGoingLong ,
			double maxThresholdForGoingLong ,
			double minThresholdForGoingShort ,
			double maxThresholdForGoingShort ) :
			base(
				firstTimeToTestInefficiency ,
				minThresholdForGoingLong ,
				maxThresholdForGoingLong ,
				minThresholdForGoingShort ,
				maxThresholdForGoingShort )
		{
			this.exactNumberOfBestLongPositionsToBeReturned =
				exactNumberOfBestLongPositionsToBeReturned;
		}

		#region getPositionsToBeOpened
//		WeightedPositions getLongWeightedPositionsFromSingleTicker(
//			string ticker )
//		{
//			double[] weights = { 1 };
//			string[] tickers = { ticker };
//			WeightedPositions weightedPositionsToBeReturned =
//				new WeightedPositions( weights , tickers );
//			return weightedPositionsToBeReturned;
//		}
		#region getWeights
		private double[] getNotNormalizedEqualWeights()
		{
			double[] notNormalizedWeights =
				new double[ this.exactNumberOfBestLongPositionsToBeReturned ];
			for ( int i = 0 ; i < this.exactNumberOfBestLongPositionsToBeReturned ; i++ )
				notNormalizedWeights[ i ] = 1;
			return notNormalizedWeights;
		}
		private double[] getWeights()
		{
			double[] notNormalizedWeights = this.getNotNormalizedEqualWeights();
			double[] normalizedWeights =
				WeightedPositions.GetNormalizedWeights( notNormalizedWeights );
			return normalizedWeights;
		}
		#endregion getWeights
		private WeightedPositions getPositionsToBeOpened(
			string[] longPositionTickers )
		{
			double[] weights = this.getWeights();
			string[] tickers =
				new String[ this.exactNumberOfBestLongPositionsToBeReturned ];
			Array.Copy(	longPositionTickers , tickers ,
			           this.exactNumberOfBestLongPositionsToBeReturned );

			// comment out the following three lines to select only the second best long ticker
//			weights = new double[ 1 ]; weights[ 0 ] = 1;
//			tickers = new String[ 1 ];
//			Array.Copy( longPositionTickers , 1 , tickers , 0 , 1 );

			WeightedPositions weightedPositionsToOpened =
				new WeightedPositions( weights , tickers );
			return weightedPositionsToOpened;
		}
		protected override WeightedPositions getPositionsToBeOpened(
			WeightedPositions[] inefficientCouples ,
			ReturnsManager inSampleReturnsManager )
		{
			WeightedPositions weightedPositionsToBeOpened = null;
			string[] longPositionTickers =
				this.getLongPositionTickers( inefficientCouples );
			if ( longPositionTickers.Length >=
			    this.exactNumberOfBestLongPositionsToBeReturned )
				// at least a long position has been found in the inefficient couples
				weightedPositionsToBeOpened =
					this.getPositionsToBeOpened( longPositionTickers );
//				weightedPositionsToBeOpened =
//					this.getLongWeightedPositionsFromSingleTicker(
//					longPositionTickers[ 0 ] );
			return weightedPositionsToBeOpened;
		}
		#endregion getPositionsToBeOpened
	}
}
