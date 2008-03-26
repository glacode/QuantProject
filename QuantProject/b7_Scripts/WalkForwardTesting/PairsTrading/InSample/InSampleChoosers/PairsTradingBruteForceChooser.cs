/*
QuantProject - Quantitative Finance Library

PairsTradingBruteForceChooser.cs
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
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Optimizing.BruteForce;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;


namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// brute force IInSampleChooser for the pairs trading strategy
	/// </summary>
	public class PairsTradingBruteForceChooser : BruteForceChooser
	{
		public PairsTradingBruteForceChooser(
			int numberOfBestTestingPositionsToBeReturned ,
			IDecoderForTestingPositions decoderForTestingPositions ,
			IFitnessEvaluator fitnessEvaluator ,
			IHistoricalQuoteProvider historicalQuoteProvider ) :
			base (
			numberOfBestTestingPositionsToBeReturned ,
			decoderForTestingPositions ,
			fitnessEvaluator ,
			historicalQuoteProvider )
		{
		}
		protected override IBruteForceOptimizableParametersManager
			getBruteForceOptimizableParametersManager(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			BruteForceOptimizableParametersManagerForBalancedVolatility
				bruteForceOptimizableParametersManager =
				new BruteForceOptimizableParametersManagerForBalancedVolatility(
				eligibleTickers ,
				2 ,
				this.decoderForTestingPositions ,
				this.fitnessEvaluator ,
				returnsManager );
			return bruteForceOptimizableParametersManager;
		}
	}
}
