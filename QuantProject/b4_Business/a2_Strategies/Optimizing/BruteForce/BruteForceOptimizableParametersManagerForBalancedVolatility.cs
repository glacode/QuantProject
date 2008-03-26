/*
QuantProject - Quantitative Finance Library

BruteForceOptimizableParametersManagerForBalancedPortfolio.cs
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
//using System.Collections;
//using System.Data;

using QuantProject.ADT.Optimizing.BruteForce;
using QuantProject.ADT.Statistics.Combinatorial;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;
//using QuantProject.ADT.Statistics;
//using QuantProject.Business.Strategies;
//using QuantProject.Business.Strategies.EquityEvaluation;
//using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Business.Strategies.Optimizing.BruteForce
{
	/// <summary>
	/// Implements IBruteForceOptimizableParametersManager
	/// for a single portfolio (i.e. a single WeightedPositions)
	/// Weights are balanced with respect to volatility
	/// </summary>
	public class
		BruteForceOptimizableParametersManagerForBalancedVolatility
		: CombinationBasedBruteForceOptimizableParametersManager
	{
//		private double[] weightsForDrivingPositions;
		private EligibleTickers eligibleTickers;
		private int numberOfPositions;
		private IDecoderForTestingPositions decoderForTestingPositions;
		private IFitnessEvaluator fitnessEvaluator;
		private ReturnsManager returnsManager;

//		private Combination combination;

//		private double[] standardDeviationForDrivingPositions;
//		private float[][] drivingPositionsCloseToCloseReturns;

//		public object Current
//		{
//			get
//			{
//				int[] currentValues = new int[ this.combination.Length ];
//				for ( int i = 0 ; i < this.combination.Length ; i ++ )
//					currentValues[ i ] = this.combination.GetValue( i );
//				BruteForceOptimizableParameters bruteForceOptimizableParameters =
//					new BruteForceOptimizableParameters( currentValues ,
//					this );
//				return bruteForceOptimizableParameters;
//			}
//		}


//		public int TotalIterations
//		{
//			get
//			{
//				return Convert.ToInt32( this.combination.TotalNumberOfCombinations );
//			}
//		}

		public BruteForceOptimizableParametersManagerForBalancedVolatility(
			EligibleTickers eligibleTickers ,
//			string portfolioLongTicker ,
			//			string portfolioShortTicker ,
			//			DateTime firstOptimizationDate ,
			//			DateTime lastOptimizationDate ,
			int numberOfPositions ,
			IDecoderForTestingPositions decoderForTestingPositions ,
			IFitnessEvaluator fitnessEvaluator ,
			ReturnsManager returnsManager ) :
			base( new Combination(
			- eligibleTickers.Count ,
			eligibleTickers.Count - 1 ,
			numberOfPositions ) )
		{
			this.eligibleTickers = eligibleTickers;
			this.numberOfPositions = numberOfPositions;
			this.decoderForTestingPositions = decoderForTestingPositions;
			this.fitnessEvaluator = fitnessEvaluator;
			this.returnsManager = returnsManager;
//			this.combination = new Combination(
//				- this.eligibleTickers.Count ,
//				this.eligibleTickers.Count - 1 ,
//				numberOfPositions );
		}

//		protected override Combination getCombination()
//		{
//			return new Combination(
//				- this.eligibleTickers.Count ,
//				this.eligibleTickers.Count - 1 ,
//				this.numberOfPositions );
//		}
//		public bool MoveNext()
//		{
//			return this.combination.MoveNext();
//		}
//		public void Reset()
//		{
//			this.combination.Reset();
//		}

//		protected override getCurrent( int[] currentValues )
//		{
//			BruteForceOptimizableParameters bruteForceOptimizableParameters =
//				new BruteForceOptimizableParameters( currentValues ,
//				this );
//			return bruteForceOptimizableParameters;
//		}

		public override object Decode( BruteForceOptimizableParameters
			bruteForceOptimizableParameters )
		{
			return this.decoderForTestingPositions.Decode(
				bruteForceOptimizableParameters.GetValues() ,
				this.eligibleTickers ,
				this.returnsManager );
		}
		public override double GetFitnessValue(
			BruteForceOptimizableParameters bruteForceOptimizableParameters )
		{
			object meaning = this.Decode(
				bruteForceOptimizableParameters );
			double fitnessValue =
				this.fitnessEvaluator.GetFitnessValue( meaning , this.returnsManager );
			return fitnessValue;
		}
	}
}
