/*
QuantProject - Quantitative Finance Library

TestGenomeManagerForLinearRegression.cs
Copyright (C) 2010
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
using NUnit.Framework;

using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.Optimizing.GenomeManagers;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Scripts.WalkForwardTesting.LinearRegression;

using QuantTesting.Business.DataProviders;

namespace QuantTesting.Scripts.WalkForwardTesting.LinearRegression
{
	[TestFixture]
	/// <summary>
	/// Test for the class GenomeManagerForLinearRegression
	/// </summary>
	public class TestGenomeManagerForLinearRegression
	{
		[Test]
		public void TestMethod()
		{
			EligibleTickers eligibleTickersForTrading =
				new EligibleTickers( new string[] { "AAAA" , "BBBB" , "CCCC" } );
			EligibleTickers eligibleTickersForSignaling =
				new EligibleTickers(
					new string[] { "AAAA" , "BBBB" , "CCCC" , "DDDD" , "EEEE" } );
			ReturnsManager returnsManager = new ReturnsManager(
				new ReturnIntervals(
					new ReturnInterval(
						new DateTime( 2008 , 1 , 1 ) , new DateTime( 2008 , 1 , 2 ) ) ) ,
				new FakeHistoricalMarketValueProvider() );
			DecoderForLinearRegressionTestingPositions decoderForLinearRegressionTestingPositions =
				new DecoderForLinearRegressionTestingPositions( 4 );
			LinearRegressionFitnessEvaluator fitnessEvaluator =
				new LinearRegressionFitnessEvaluator(
					new LinearRegressionSetupManager() );
			GenomeManagerForLinearRegression genomeManagerForLinearRegression =
				new GenomeManagerForLinearRegression(
					eligibleTickersForTrading ,
					eligibleTickersForSignaling ,
					returnsManager ,
					returnsManager ,
					decoderForLinearRegressionTestingPositions ,
					fitnessEvaluator ,
					GenomeManagerType.ShortAndLong ,
					1 ,
					2 );

//						EligibleTickers eligibleTickersForTrading ,
//			EligibleTickers eligibleTickersForSignaling ,
////			ReturnsManager returnsManager ,
//			IReturnsManager returnsManagerForTradingTickers ,
//			IReturnsManager returnsManagerForSignalingTickers ,
//			DecoderForLinearRegressionTestingPositions
//			decoderForLinearRegressionTestingPositions ,
//			ILinearRegressionFitnessEvaluator fitnessEvaluator ,
//			GenomeManagerType genomeManagerType ,
//			int seedForRandomGeneratorForTradingTickers ,
//			int seedForRandomGeneratorForSignalingTickers ) :

			
			
			int minValueForTradingGene =
				genomeManagerForLinearRegression.GetMinValueForGenes( 1 );
			Assert.AreEqual( -3 , minValueForTradingGene );
			int maxValueForTradingGene =
				genomeManagerForLinearRegression.GetMaxValueForGenes( 1 );
			Assert.AreEqual( 2 , maxValueForTradingGene );
			int minValueForSignalingGene =
				genomeManagerForLinearRegression.GetMinValueForGenes( 2 );
			Assert.AreEqual( -5 , minValueForSignalingGene );
			int maxValueForSignalingGene =
				genomeManagerForLinearRegression.GetMaxValueForGenes( 2 );
			Assert.AreEqual( 4 , maxValueForSignalingGene );
		}
	}
}
