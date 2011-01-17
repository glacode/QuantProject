/*
QuantProject - Quantitative Finance Library

OTCOnlyLongEndOfDayBruteForceOptimizableParametersManager.cs
Copyright (C) 2009
Marco Milletti

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
using QuantProject.ADT.Statistics.Combinatorial;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.Optimizing.BruteForce;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TickerSelectionTesting.OTC.InSampleChoosers.BruteForce
{
	/// <summary>
	/// BruteForceOptimizableParametersManager to be used by the OTC intraday strategy.
	/// </summary>
	[Serializable]
	public class OTCOnlyLongEndOfDayBruteForceOptimizableParametersManager :
		CombinationBasedBruteForceOptimizableParametersManager
	{
		private EligibleTickers eligibleTickers;
		private int numberOfPositions;
		private IDecoderForTestingPositions decoderForTestingPositions;
		private IFitnessEvaluator fitnessEvaluator;
		private ReturnsManager returnsManager;

		
		public OTCOnlyLongEndOfDayBruteForceOptimizableParametersManager(
			EligibleTickers eligibleTickers ,
			int numberOfPositions ,
			IDecoderForTestingPositions decoderForTestingPositions ,
			IFitnessEvaluator fitnessEvaluator ,
			ReturnsManager returnsManager ) :
				base( new Combination(
			0 ,	eligibleTickers.Count - 1 ,
			numberOfPositions ) )
			
		{
			this.eligibleTickers = eligibleTickers;
			this.numberOfPositions = numberOfPositions;
			this.decoderForTestingPositions = decoderForTestingPositions;
			this.fitnessEvaluator = fitnessEvaluator;
			this.returnsManager = returnsManager;
		}
		
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
		public override bool AreEquivalentAsTopBestParameters(
			BruteForceOptimizableParameters bruteForceOptimizableParameters1 ,
			BruteForceOptimizableParameters bruteForceOptimizableParameters2 )
		{
			this.areEquivalentAsTopBestParameters_checkParameters(
				bruteForceOptimizableParameters1 , bruteForceOptimizableParameters2 );
			
			bool areEquivalentAsTopBestParameters =
				((TestingPositions)bruteForceOptimizableParameters1.Meaning ).HashCode ==
				((TestingPositions)bruteForceOptimizableParameters2.Meaning ).HashCode;
			
			return areEquivalentAsTopBestParameters;
		}
		#endregion AreEquivalentAsTopBestParameters
	}
}
		
		
