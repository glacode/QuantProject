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
	/// Two TestingPositions are considered equivalent as TopBestPositions
	/// (and only one is kept among them) iif they have the same tickers
	/// (consider that if two WeightedPosition are highly correlated, the
	/// two opposite WeightedPosition are highly correlated too)
	/// </summary>
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
			object meaning1 , object meaning2 )
		{
			if ( !(meaning1 is TestingPositions) )
				throw new Exception( "The first parameter is expected " +
					"to be a TestingPositions!" );
			if ( !(meaning2 is TestingPositions) )
				throw new Exception( "The second parameter is expected " +
					"to be a TestingPositions!" );
		}
		/// Two TestingPositions are considered equivalent as TopBestPositions
		/// (and only one is kept among them) iif they have the same tickers
		/// (consider that if two WeightedPosition are highly correlated, the
		/// two opposite WeightedPosition are highly correlated too)
		public override bool AreEquivalentAsTopBestParameters(
			object meaning1 , object meaning2 )
		{
			this.areEquivalentAsTopBestParameters_checkParameters(
				meaning1 , meaning2 );
			string hashCodeForMeaning1 =
				((TestingPositions)meaning1).HashCodeForTickerComposition;
			string hashCodeForMeaning2 =
				((TestingPositions)meaning2).HashCodeForTickerComposition;
			bool areEquivalentAsTopBestParameters =
				( hashCodeForMeaning1 == hashCodeForMeaning2 );
			return areEquivalentAsTopBestParameters;
		}
		#endregion AreEquivalentAsTopBestParameters
	}
}
