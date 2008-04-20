/*
QuantProject - Quantitative Finance Library

LongOnlyPairsTradingStrategy.cs
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
	/// Pairs Trading strategy that selects only the long position
	/// </summary>
	public class LongOnlyPairsTradingStrategy :
		PairsTradingStrategy
	{
		public LongOnlyPairsTradingStrategy(
			int numDaysBeetweenEachOtpimization ,
			int numDaysForInSampleOptimization ,
			IIntervalsSelector intervalsSelector ,
			IEligiblesSelector eligiblesSelector ,
			IInSampleChooser inSampleChooser ,
			IHistoricalQuoteProvider historicalQuoteProviderForInSample ,
			IHistoricalQuoteProvider
			historicalQuoteProviderForChosingPositionsOutOfSample ,
			double minThresholdForGoingLong ,
			double maxThresholdForGoingLong ,
			double minThresholdForGoingShort ,
			double maxThresholdForGoingShort ) :
			base(
			numDaysBeetweenEachOtpimization ,
			numDaysForInSampleOptimization ,
			intervalsSelector ,
			eligiblesSelector ,
			inSampleChooser ,
			historicalQuoteProviderForInSample ,
			historicalQuoteProviderForChosingPositionsOutOfSample ,
			minThresholdForGoingLong ,
			maxThresholdForGoingLong ,
			minThresholdForGoingShort ,
			maxThresholdForGoingShort )
		{
		}
		
		protected override string getTextIdentifier()
		{
			return "pairsTrdngOnlyLong";
		}

		#region selectWeightedPositions
		private WeightedPositions selectWeightedPositionIfTheCase(
			WeightedPosition weightedPosition )
		{
			WeightedPositions
				weightedPositionsToBeReturned = null;
			if ( weightedPosition.IsLong )
			{
				double[] weights = { 1 };
				string[] tickers = { weightedPosition.Ticker };
				weightedPositionsToBeReturned =
					new WeightedPositions( weights , tickers );
			}
			return weightedPositionsToBeReturned;
		}
		protected override WeightedPositions selectWeightedPositions(
			WeightedPositions weightedPositions )
		{
			WeightedPositions weightedPositionsToBeReturned = 
				this.selectWeightedPositionIfTheCase( weightedPositions[ 0 ] );
			if ( weightedPositionsToBeReturned == null )
				// the first weighted position was not the one to be selected
				weightedPositionsToBeReturned =
					this.selectWeightedPositionIfTheCase( weightedPositions[ 1 ] );
			return weightedPositionsToBeReturned;
		}
		#endregion selectWeightedPositions
		
	}
}
