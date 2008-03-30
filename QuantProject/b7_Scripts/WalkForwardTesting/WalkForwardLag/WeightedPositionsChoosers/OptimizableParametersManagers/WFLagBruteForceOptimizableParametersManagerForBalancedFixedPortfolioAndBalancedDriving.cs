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
		: CombinationBasedBruteForceOptimizableParametersManager
	{
//		private double[] weightsForDrivingPositions;
		private DataTable eligibleTickersForDrivingPositions;
		private IWFLagDecoder wFLagDecoder;
		private WFLagFitnessEvaluator wFLagFitnessEvaluator;

//		private Combination drivingCombination;

//		private double[] standardDeviationForDrivingPositions;
//		private float[][] drivingPositionsCloseToCloseReturns;

//		public object Current
//		{
//			get
//			{
//				int[] currentValues = new int[ this.drivingCombination.Length ];
//				for ( int i = 0 ; i < this.drivingCombination.Length ; i ++ )
//					currentValues[ i ] = this.drivingCombination.GetValue( i );
////				for ( int i = this.drivingCombination.Length ;
////					i < this.drivingCombination.Length +
////					this.portfolioCombination.Length ; i ++ )
////					currentValues[ i ] =
////						this.portfolioCombination.GetValue( i - this.drivingCombination.Length );
//				BruteForceOptimizableParameters bruteForceOptimizableParameters =
//					new BruteForceOptimizableParameters( currentValues ,
//					this );
//				return bruteForceOptimizableParameters;
//			}
//		}
//
//
//		public int TotalIterations
//		{
//			get
//			{
//				return Convert.ToInt32( this.drivingCombination.TotalNumberOfCombinations );
//			}
//		}

		public WFLagBruteForceOptimizableParametersManagerForBalancedFixedPortfolioAndBalancedDriving(
			DataTable eligibleTickersForDrivingPositions ,
			string portfolioLongTicker ,
			string portfolioShortTicker ,
			DateTime firstOptimizationDate ,
			DateTime lastOptimizationDate ,
			int numberOfDrivingPositions ,
			IWFLagDecoder wFLagDecoder ,
			IEquityEvaluator equityEvaluator ,
			ReturnsManager returnsManager ) :
			base( new Combination(
			- eligibleTickersForDrivingPositions.Rows.Count ,
			eligibleTickersForDrivingPositions.Rows.Count - 1 ,
			numberOfDrivingPositions ) )
		{
			this.eligibleTickersForDrivingPositions =
				eligibleTickersForDrivingPositions;
			this.wFLagDecoder = wFLagDecoder;
			this.wFLagFitnessEvaluator =
				new WFLagFitnessEvaluator( equityEvaluator ,
				returnsManager );
//			this.drivingCombination = new Combination(
//				- eligibleTickersForDrivingPositions.Rows.Count ,
//				eligibleTickersForDrivingPositions.Rows.Count - 1 ,
//				numberOfDrivingPositions );
		}

//		public bool MoveNext()
//		{
//			return this.drivingCombination.MoveNext();
//		}
//		public void Reset()
//		{
//			this.drivingCombination.Reset();
//		}

		public override object Decode( BruteForceOptimizableParameters
			bruteForceOptimizableParameters )
		{
			return this.wFLagDecoder.Decode(
				bruteForceOptimizableParameters.GetValues() );
		}
		public override double GetFitnessValue(
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
