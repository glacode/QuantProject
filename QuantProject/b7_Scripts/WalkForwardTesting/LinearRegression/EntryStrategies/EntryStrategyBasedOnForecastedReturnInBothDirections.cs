/*
QuantProject - Quantitative Finance Library

EntryStrategyBasedOnForecastedReturnInBothDirections.cs
Copyright (C) 2011
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
using QuantProject.Business.Strategies.ReturnsManagement.Time;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Enumerates the given testing positions, until the forecasted return is either
	/// larger than a given threshold or smaller than a negative threashold: in the
	/// former case, the straight weighted positions are returned; in the latter case,
	/// the opposite of the weighted positions are returned
	/// </summary>
	[Serializable]
	public class EntryStrategyBasedOnForecastedReturnInBothDirections :
		EntryStrategyBasedOnForecastedReturn
	{
		public EntryStrategyBasedOnForecastedReturnInBothDirections(
			double minAverageExpectedReturn ,
			ILinearRegressionFitnessEvaluator fitnessEvaluator ,
			IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling ,
			HistoricalMarketValueProvider historicalMarketValueProvider ) :
			base( minAverageExpectedReturn , fitnessEvaluator ,
			     returnIntervalSelectorForSignaling , historicalMarketValueProvider )
		{
		}
		
		protected override WeightedPositions tryThisCandidate(
			LinearRegressionTestingPositions candidate, double expectedReturn )
		{
			WeightedPositions weightedPositionsToBeOpened = null;
			if ( expectedReturn >= this.minAverageExpectedReturn )
				weightedPositionsToBeOpened = candidate.WeightedPositions;
			if ( expectedReturn < -this.minAverageExpectedReturn )
				weightedPositionsToBeOpened = candidate.WeightedPositions.Opposite;
			return weightedPositionsToBeOpened;
		}
	}
}
