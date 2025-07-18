/*
QuantProject - Quantitative Finance Library

PairsTradingBruteForceOptimizableParametersManager.cs
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

using QuantProject.ADT.Optimizing.BruteForce;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.Optimizing.BruteForce;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// BruteForceOptimizableParametersManager to be used by the pairs
	/// trading strategy.
	/// Two TestingPositions are considered equivalent as TopBestParameters
	/// (and only one is kept among them) iif they have the same tickers and
	/// the same fitness.
	/// This is a short way to say that, with respect to the
	/// pairs trading strategy, (a,b) is equivalent to
	/// (-a,-b), and that (a,-b) is equivalent to (-a,b)
	/// We use fitness similarity instead of checking for all possible long/short
	/// combinations
	/// </summary>
	[Serializable]
	public class PairsTradingBruteForceOptimizableParametersManager :
		BruteForceOptimizableParametersManagerForBalancedVolatility
	{
		public PairsTradingBruteForceOptimizableParametersManager(
			EligibleTickers eligibleTickers ,
			int numberOfPositions ,
			IDecoderForTestingPositions decoderForTestingPositions ,
			IFitnessEvaluator fitnessEvaluator ,
			ReturnsManager returnsManager ) :
			base(
			eligibleTickers ,
			numberOfPositions ,
			decoderForTestingPositions ,
			fitnessEvaluator ,
			returnsManager )
		{
		}

		#region AreEquivalentAsTopBestParameters
		private void areEquivalentAsTopBestParameters_checkParameters(
			BruteForceOptimizableParameters bruteForceOptimizableParameters1 ,
			BruteForceOptimizableParameters bruteForceOptimizableParameters2 )
		{
			if ( !(bruteForceOptimizableParameters1.Meaning is TestingPositions) )
				throw new Exception( "The first parameter is expected " +
					"to represent a TestingPositions!" );
			if ( !(bruteForceOptimizableParameters2.Meaning is TestingPositions) )
				throw new Exception( "The second parameter is expected " +
					"to represent a TestingPositions!" );
		}
		private bool haveTheSameTickers(
			TestingPositions testingPositions1 ,
			TestingPositions testingPositions2 )
		{			
			string hashCodeForMeaning1 =
				testingPositions1.HashCodeForTickerComposition;
			string hashCodeForMeaning2 =
				testingPositions2.HashCodeForTickerComposition;
			bool areEquivalentAsTopBestParameters =
				( hashCodeForMeaning1 == hashCodeForMeaning2 );
			return areEquivalentAsTopBestParameters;
		}
		private bool haveTheSameFitness(
			BruteForceOptimizableParameters bruteForceOptimizableParameters1 ,
			BruteForceOptimizableParameters bruteForceOptimizableParameters2 )
		{			
			double fitness1 = bruteForceOptimizableParameters1.Fitness;
			double fitness2 = bruteForceOptimizableParameters2.Fitness;
			double percDifference = Math.Abs(
				fitness1 / fitness2 - 1 );
			bool haveTheSameFitness =
				( percDifference < 0.00001 );
//			if ( areEquivalentAsTopBestParameters )
//			{
//				string forBreakpoint = "";
//				forBreakpoint = forBreakpoint + "a";
//			}
//			return areEquivalentAsTopBestParameters;
			return haveTheSameFitness;
		}
		/// Two TestingPositions are considered equivalent as TopBestParameters
		/// (and only one is kept among them) iif they have the same tickers and
		/// the same fitness.
		/// This is a short way to say that, with respect to the
		/// pairs trading strategy, (a,b) is equivalent to
		/// (-a,-b), and that (a,-b) is equivalent to (-a,b)
		/// We use fitness similarity instead of checking for all possible long/short
		/// combinations
		public override bool AreEquivalentAsTopBestParameters(
			BruteForceOptimizableParameters bruteForceOptimizableParameters1 ,
			BruteForceOptimizableParameters bruteForceOptimizableParameters2 )
		{
			this.areEquivalentAsTopBestParameters_checkParameters(
				bruteForceOptimizableParameters1 , bruteForceOptimizableParameters2 );

			// the following is a short way to say that, with respect to the
			// pairs trading strategy, (a,b) is equivalent to
			// (-a,-b), and that (a,-b) is equivalent to (-a,b)
			// We use fitness similarity instead of checking for all possible long/short
			// combinations
			bool areEquivalentAsTopBestParameters =
				this.haveTheSameTickers(
				((TestingPositions)bruteForceOptimizableParameters1.Meaning ) ,
				((TestingPositions)bruteForceOptimizableParameters2.Meaning) ) &&
				this.haveTheSameFitness(
				bruteForceOptimizableParameters1 ,
				bruteForceOptimizableParameters2 );
			return areEquivalentAsTopBestParameters;
		}
		#endregion AreEquivalentAsTopBestParameters
	}
}
