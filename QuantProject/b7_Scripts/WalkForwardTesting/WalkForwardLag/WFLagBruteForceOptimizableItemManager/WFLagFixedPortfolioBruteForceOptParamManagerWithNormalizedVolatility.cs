/*
QuantProject - Quantitative Finance Library

WFLagFixedPortfolioBruteForceOptParamManagerWithNormalizedVolatility.cs
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
using System.Collections;
using System.Data;

using QuantProject.ADT.Statistics;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// Implements IBruteForceOptimizableParametersManager using weights
	/// to normalize the portfolio's tickers volatility
	/// </summary>
	public class WFLagFixedPortfolioBruteForceOptParamManagerWithNormalizedVolatility
		: WFLagFixedPortfolioBruteForceOptParamManagerWithPortfolioNormalizedVolatility
	{
//		private double[] weightsForDrivingPositions;
		private double[] standardDeviationForDrivingPositions;
		private float[][] drivingPositionsCloseToCloseReturns;

		public WFLagFixedPortfolioBruteForceOptParamManagerWithNormalizedVolatility(
			DataTable eligibleTickersForDrivingPositions ,
			string portfolioLongTicker ,
			string portfolioShortTicker ,
			DateTime firstOptimizationDate ,
			DateTime lastOptimizationDate ,
			int numberOfDrivingPositions ) :
			base( eligibleTickersForDrivingPositions ,
			portfolioLongTicker ,
			portfolioShortTicker ,
			firstOptimizationDate ,
			lastOptimizationDate ,
			numberOfDrivingPositions )
		{
		}
//		protected override double[]
//			getWeightRelatedParameterValuesForDrivingPositions(
//			int[] optimizableParameters )
//		{
//			double[] weightsForPortfolioPositions =
//				this.getWeightsForPortfolioPositions();
//			string[] tickersForPortfolioPositions =
//				new string[ 2 ] { this.portfolioLongTicker , this.portfolioShortTicker };
//			WeightedPositions weightedPositions =
//				new WeightedPositions( weightsForPortfolioPositions ,
//				tickersForPortfolioPositions );
//			return weightedPositions;
//		}
		#region getWeightRelatedParameterValuesForDrivingPositions
		private ArrayList getArrayListOfEligibleTickersForDrivingPositions()
		{
			ArrayList tickers = new ArrayList();
			foreach ( DataRow eligibleDrivingTicker in
				this.eligibleTickersForDrivingWeightedPositions.Rows )
				tickers.Add( eligibleDrivingTicker[ 0 ] );
			return tickers;
		}
		private void setDrivingPositionsCloseToCloseReturns()
		{
			ArrayList tickers =
				this.getArrayListOfEligibleTickersForDrivingPositions();
			this.drivingPositionsCloseToCloseReturns =
				this.wFLagCandidates.GetTickersReturns( tickers );
		}
		private void setStandardDeviationForDrivingPositionsActually(
			int drivingPositionIndex )
		{
			this.standardDeviationForDrivingPositions[ drivingPositionIndex ] =
				BasicFunctions.GetStdDev(
				this.drivingPositionsCloseToCloseReturns[ drivingPositionIndex ] );
		}
		private void setStandardDeviationForDrivingPositionsActually()
		{
			this.setDrivingPositionsCloseToCloseReturns();
			this.standardDeviationForDrivingPositions =
				new double[ this.eligibleTickersForDrivingWeightedPositions.Rows.Count ];
			for ( int i = 0 ;
				i < this.eligibleTickersForDrivingWeightedPositions.Rows.Count ;
				i++ )
				this.setStandardDeviationForDrivingPositionsActually( i );
		}
		private void setStandardDeviationForDrivingPositions()
		{
			if ( this.standardDeviationForDrivingPositions == null )
				// this.standardDeviationForDrivingPositions has not been set yet
				this.setStandardDeviationForDrivingPositionsActually();
		}
		private double getStandardDeviation( int[] parameterValues ,
			int parameterPosition )
		{
			int tickerIndex =	this.getTickerIndexForDrivingPosition(
				parameterValues , parameterPosition );
			return this.standardDeviationForDrivingPositions[ tickerIndex ];
		}
		private double getMaxStandardDeviationForCurrentDrivingPositions(
			int[] optimizableParameters )
		{
			double maxStandardDeviationForCurrentDrivingPositions = 0;
			for ( int parameterPosition = 0 ; parameterPosition < optimizableParameters.Length ;
				parameterPosition++ )
			{
				double drivingPositionStandardDeviation =
					this.getStandardDeviation( optimizableParameters , parameterPosition );
				if ( drivingPositionStandardDeviation > maxStandardDeviationForCurrentDrivingPositions )
					maxStandardDeviationForCurrentDrivingPositions = drivingPositionStandardDeviation;
			}
			return maxStandardDeviationForCurrentDrivingPositions;
		}
		private double getNonNormalizedWeightForDrivingPosition(
			int[] parameterValues ,
			int parameterPosition ,
			double maxStandardDeviationForCurrentDrivingPositions )
		{
			double drivingPositionStandardDeviation =
				this.getStandardDeviation( parameterValues , parameterPosition );
			double nonNormalizedWeightForDrivingPosition =
				maxStandardDeviationForCurrentDrivingPositions /
				drivingPositionStandardDeviation;
			if ( parameterValues[ parameterPosition ] < 0 )
				nonNormalizedWeightForDrivingPosition =
					-nonNormalizedWeightForDrivingPosition;
			return nonNormalizedWeightForDrivingPosition;
		}
		private double[] getNonNormalizedWeightsForDrivingPositionsUsingStandardDeviations(
			int[] parameterValues ,
			double maxStandardDeviationForCurrentDrivingPositions )
		{
			double[] nonNormalizedWeightsForDrivingPositions =
				new double[ parameterValues.Length ];
			for ( int parameterPosition = 0 ; parameterPosition < parameterValues.Length ;
				parameterPosition++ )
			{
				nonNormalizedWeightsForDrivingPositions[ parameterPosition ] =
					this.getNonNormalizedWeightForDrivingPosition(
					parameterValues , parameterPosition ,
					maxStandardDeviationForCurrentDrivingPositions );
			}
			return nonNormalizedWeightsForDrivingPositions;
		}
		private double[] getNonNormalizedWeightsForDrivingPositions(
			int[] parameterValues )
		{
			double maxStandardDeviationForCurrentDrivingPositions =
				this.getMaxStandardDeviationForCurrentDrivingPositions( parameterValues );
			return getNonNormalizedWeightsForDrivingPositionsUsingStandardDeviations(
				parameterValues , maxStandardDeviationForCurrentDrivingPositions );
		}
		private double[] getWeightsForDrivingPositions(
			int[] parameterValues )
		{
			double[] nonNormalizedWeightsForDrivingPositions =
				this.getNonNormalizedWeightsForDrivingPositions( parameterValues );
			return
				WeightedPositions.GetNormalizedWeights(
				nonNormalizedWeightsForDrivingPositions );
		}
		protected override double[]
			getWeightRelatedParameterValuesForDrivingPositions(
			int[] parameterValues )
		{
			this.setStandardDeviationForDrivingPositions();
			return this.getWeightsForDrivingPositions( parameterValues );
		}
		#endregion
	}
}
