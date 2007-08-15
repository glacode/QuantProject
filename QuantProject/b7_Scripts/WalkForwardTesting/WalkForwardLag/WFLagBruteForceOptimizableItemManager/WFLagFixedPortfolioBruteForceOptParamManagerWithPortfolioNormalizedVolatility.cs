/*
QuantProject - Quantitative Finance Library

WFLagFixedPortfolioBruteForceOptParamManagerWithPortfolioNormalizedVolatility.cs
Copyright (C) 2006
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
using System.Data;

using QuantProject.ADT.Statistics;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.EquityEvaluation;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// Implements IBruteForceOptimizableParametersManager using weights
	/// to normalize the portfolio's tickers volatility
	/// </summary>
	public class WFLagFixedPortfolioBruteForceOptParamManagerWithPortfolioNormalizedVolatility
		: WFLagFixedPortfolioBruteForceOptimizableParametersManager
	{
		private double[] weightsForPortfolioPositions;
		private double weightForLongPosition;
		private double weightForShortPosition;
		private double standardDeviationForShortPosition;
		private double standardDeviationForLongPosition;

		public WFLagFixedPortfolioBruteForceOptParamManagerWithPortfolioNormalizedVolatility(
			DataTable eligibleTickersForDrivingPositions ,
			string portfolioLongTicker ,
			string portfolioShortTicker ,
			DateTime firstOptimizationDate ,
			DateTime lastOptimizationDate ,
			int numberOfDrivingPositions ,
			IEquityEvaluator equityEvaluator ) :
			base( eligibleTickersForDrivingPositions ,
			portfolioLongTicker ,
			portfolioShortTicker ,
			firstOptimizationDate ,
			lastOptimizationDate ,
			numberOfDrivingPositions ,
			equityEvaluator )
		{
			this.standardDeviationForLongPosition = double.MinValue;
			this.standardDeviationForShortPosition = double.MinValue;
		}
		protected override WeightedPositions decodePortfolioWeightedPositions(
			int[] optimizableParameters )
		{
			double[] weightsForPortfolioPositions =
				this.getWeightsForPortfolioPositions();
			string[] tickersForPortfolioPositions =
				new string[ 2 ] { this.portfolioLongTicker , this.portfolioShortTicker };
			WeightedPositions weightedPositions =
				new WeightedPositions( weightsForPortfolioPositions ,
				tickersForPortfolioPositions );
			return weightedPositions;
		}
		#region getWeightsForPortfolioPositions
		private double getStandardDeviation( string ticker )
		{
			float[][] tickersReturns =
				this.wFLagCandidates.GetTickersReturns( new string[ 1 ] { ticker } );
			return BasicFunctions.GetStdDev( tickersReturns[ 0 ] );
		}
		private void setStandardDeviationForShortPosition()
		{
			this.standardDeviationForShortPosition =
				this.getStandardDeviation( this.portfolioShortTicker );
		}
		private double getStandardDeviationForShortPosition()
		{
			if ( this.standardDeviationForShortPosition == double.MinValue )
				// this.standardDeviationForShortPosition has not been set yet
				this.setStandardDeviationForShortPosition();
			return this.standardDeviationForShortPosition;
		}
		private void setStandardDeviationForLongPosition()
		{
			this.standardDeviationForLongPosition =
				this.getStandardDeviation( this.portfolioLongTicker );
		}
		private double getStandardDeviationForLongPosition()
		{
			if ( this.standardDeviationForLongPosition == double.MinValue )
				// this.standardDeviationForLongPosition has not been set yet
				this.setStandardDeviationForLongPosition();
			return this.standardDeviationForLongPosition;
		}
		private void setWeightsForPortfolioPositions()
		{
			this.weightForLongPosition =
				this.getStandardDeviationForShortPosition() /
				( this.getStandardDeviationForShortPosition() +
				this.getStandardDeviationForLongPosition() );
			this.weightForShortPosition =
				-1 + this.weightForLongPosition;
			this.weightsForPortfolioPositions =
				new double[ 2 ] { this.weightForLongPosition , this.weightForShortPosition };
		}
		private double[] getWeightsForPortfolioPositions()
		{
			if ( this.weightsForPortfolioPositions == null )
				this.setWeightsForPortfolioPositions();
			return this.weightsForPortfolioPositions;
		}
		#endregion
	}
}
