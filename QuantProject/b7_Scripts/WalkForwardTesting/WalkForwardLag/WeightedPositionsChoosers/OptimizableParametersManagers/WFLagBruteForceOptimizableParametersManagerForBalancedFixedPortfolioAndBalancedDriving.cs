/*
QuantProject - Quantitative Finance Library

WFLagBruteForceOptimizableParametersManagerForBalancedFixedPortfolioAndBalancedDriving.cs
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

using QuantProject.ADT.Optimizing.BruteForce;
using QuantProject.ADT.Statistics;
using QuantProject.ADT.Statistics.Combinatorial;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WeightedPositionsChoosers.Decoding;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WeightedPositionsChoosers
{
	/// <summary>
	/// Implements IBruteForceOptimizableParametersManager
	/// Weights are balanced with respect to volatility both for portfolio
	/// positions and for driving positions
	/// Portfolio positions are given to the constructor, while
	/// driving positions are optimized
	/// </summary>
	public class
		WFLagBruteForceOptimizableParametersManagerForBalancedFixedPortfolioAndBalancedDriving
		: IBruteForceOptimizableParametersManager
	{
//		private double[] weightsForDrivingPositions;
		private DataTable eligibleTickersForDrivingPositions;
		private IWFLagDecoder wFLagDecoder;
		private WFLagFitnessEvaluator wFLagFitnessEvaluator;

		private Combination drivingCombination;

//		private double[] standardDeviationForDrivingPositions;
//		private float[][] drivingPositionsCloseToCloseReturns;

		public object Current
		{
			get
			{
				int[] currentValues = new int[ this.drivingCombination.Length ];
				for ( int i = 0 ; i < this.drivingCombination.Length ; i ++ )
					currentValues[ i ] = this.drivingCombination.GetValue( i );
//				for ( int i = this.drivingCombination.Length ;
//					i < this.drivingCombination.Length +
//					this.portfolioCombination.Length ; i ++ )
//					currentValues[ i ] =
//						this.portfolioCombination.GetValue( i - this.drivingCombination.Length );
				BruteForceOptimizableParameters bruteForceOptimizableParameters =
					new BruteForceOptimizableParameters( currentValues ,
					this );
				return bruteForceOptimizableParameters;
			}
		}


		public int TotalIterations
		{
			get
			{
				return Convert.ToInt32( this.drivingCombination.TotalNumberOfCombinations );
			}
		}

		public WFLagBruteForceOptimizableParametersManagerForBalancedFixedPortfolioAndBalancedDriving(
			DataTable eligibleTickersForDrivingPositions ,
			string portfolioLongTicker ,
			string portfolioShortTicker ,
			DateTime firstOptimizationDate ,
			DateTime lastOptimizationDate ,
			int numberOfDrivingPositions ,
			IWFLagDecoder wFLagDecoder ,
			IEquityEvaluator equityEvaluator ,
			ReturnsManager returnsManager )
		{
			this.eligibleTickersForDrivingPositions =
				eligibleTickersForDrivingPositions;
			this.wFLagDecoder = wFLagDecoder;
			this.wFLagFitnessEvaluator =
				new WFLagFitnessEvaluator( equityEvaluator ,
				returnsManager );
			this.drivingCombination = new Combination(
				- eligibleTickersForDrivingPositions.Rows.Count ,
				eligibleTickersForDrivingPositions.Rows.Count - 1 ,
				numberOfDrivingPositions );
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
//		#region getWeightRelatedParameterValuesForDrivingPositions
//		private ArrayList getArrayListOfEligibleTickersForDrivingPositions()
//		{
//			ArrayList tickers = new ArrayList();
//			foreach ( DataRow eligibleDrivingTicker in
//				this.eligibleTickersForDrivingWeightedPositions.Rows )
//				tickers.Add( eligibleDrivingTicker[ 0 ] );
//			return tickers;
//		}
//		private void setDrivingPositionsCloseToCloseReturns()
//		{
//			ArrayList tickers =
//				this.getArrayListOfEligibleTickersForDrivingPositions();
//			this.drivingPositionsCloseToCloseReturns =
//				this.wFLagCandidates.GetTickersReturns( tickers );
//		}
//		private void setStandardDeviationForDrivingPositionsActually(
//			int drivingPositionIndex )
//		{
//			this.standardDeviationForDrivingPositions[ drivingPositionIndex ] =
//				BasicFunctions.GetStdDev(
//				this.drivingPositionsCloseToCloseReturns[ drivingPositionIndex ] );
//		}
//		private void setStandardDeviationForDrivingPositionsActually()
//		{
//			this.setDrivingPositionsCloseToCloseReturns();
//			this.standardDeviationForDrivingPositions =
//				new double[ this.eligibleTickersForDrivingWeightedPositions.Rows.Count ];
//			for ( int i = 0 ;
//				i < this.eligibleTickersForDrivingWeightedPositions.Rows.Count ;
//				i++ )
//				this.setStandardDeviationForDrivingPositionsActually( i );
//		}
//		private void setStandardDeviationForDrivingPositions()
//		{
//			if ( this.standardDeviationForDrivingPositions == null )
//				// this.standardDeviationForDrivingPositions has not been set yet
//				this.setStandardDeviationForDrivingPositionsActually();
//		}
//		private double getStandardDeviation( int[] parameterValues ,
//			int parameterPosition )
//		{
//			int tickerIndex =	this.getTickerIndexForDrivingPosition(
//				parameterValues , parameterPosition );
//			return this.standardDeviationForDrivingPositions[ tickerIndex ];
//		}
//		private double getMaxStandardDeviationForCurrentDrivingPositions(
//			int[] optimizableParameters )
//		{
//			double maxStandardDeviationForCurrentDrivingPositions = 0;
//			for ( int parameterPosition = 0 ; parameterPosition < optimizableParameters.Length ;
//				parameterPosition++ )
//			{
//				double drivingPositionStandardDeviation =
//					this.getStandardDeviation( optimizableParameters , parameterPosition );
//				if ( drivingPositionStandardDeviation > maxStandardDeviationForCurrentDrivingPositions )
//					maxStandardDeviationForCurrentDrivingPositions = drivingPositionStandardDeviation;
//			}
//			return maxStandardDeviationForCurrentDrivingPositions;
//		}
//		private double getNonNormalizedWeightForDrivingPosition(
//			int[] parameterValues ,
//			int parameterPosition ,
//			double maxStandardDeviationForCurrentDrivingPositions )
//		{
//			double drivingPositionStandardDeviation =
//				this.getStandardDeviation( parameterValues , parameterPosition );
//			double nonNormalizedWeightForDrivingPosition =
//				maxStandardDeviationForCurrentDrivingPositions /
//				drivingPositionStandardDeviation;
//			if ( parameterValues[ parameterPosition ] < 0 )
//				nonNormalizedWeightForDrivingPosition =
//					-nonNormalizedWeightForDrivingPosition;
//			return nonNormalizedWeightForDrivingPosition;
//		}
//		private double[] getNonNormalizedWeightsForDrivingPositionsUsingStandardDeviations(
//			int[] parameterValues ,
//			double maxStandardDeviationForCurrentDrivingPositions )
//		{
//			double[] nonNormalizedWeightsForDrivingPositions =
//				new double[ parameterValues.Length ];
//			for ( int parameterPosition = 0 ; parameterPosition < parameterValues.Length ;
//				parameterPosition++ )
//			{
//				nonNormalizedWeightsForDrivingPositions[ parameterPosition ] =
//					this.getNonNormalizedWeightForDrivingPosition(
//					parameterValues , parameterPosition ,
//					maxStandardDeviationForCurrentDrivingPositions );
//			}
//			return nonNormalizedWeightsForDrivingPositions;
//		}
//		private double[] getNonNormalizedWeightsForDrivingPositions(
//			int[] parameterValues )
//		{
//			double maxStandardDeviationForCurrentDrivingPositions =
//				this.getMaxStandardDeviationForCurrentDrivingPositions( parameterValues );
//			return getNonNormalizedWeightsForDrivingPositionsUsingStandardDeviations(
//				parameterValues , maxStandardDeviationForCurrentDrivingPositions );
//		}
//		private double[] getWeightsForDrivingPositions(
//			int[] parameterValues )
//		{
//			double[] nonNormalizedWeightsForDrivingPositions =
//				this.getNonNormalizedWeightsForDrivingPositions( parameterValues );
//			return
//				WeightedPositions.GetNormalizedWeights(
//				nonNormalizedWeightsForDrivingPositions );
//		}
//		protected override double[]
//			getWeightRelatedParameterValuesForDrivingPositions(
//			int[] parameterValues )
//		{
//			this.setStandardDeviationForDrivingPositions();
//			return this.getWeightsForDrivingPositions( parameterValues );
//		}
//		#endregion
		public bool MoveNext()
		{
			return this.drivingCombination.MoveNext();
		}
		public void Reset()
		{
			this.drivingCombination.Reset();
		}

		public object Decode( BruteForceOptimizableParameters
			bruteForceOptimizableParameters )
		{
			return this.wFLagDecoder.Decode(
				bruteForceOptimizableParameters.GetValues() );
		}
		public double GetFitnessValue(
			BruteForceOptimizableParameters bruteForceOptimizableParameters )
		{
			object meaning = this.Decode(
				bruteForceOptimizableParameters );
			double fitnessValue =
				this.wFLagFitnessEvaluator.GetFitnessValue( meaning );
			return fitnessValue;
		}
	}
}
