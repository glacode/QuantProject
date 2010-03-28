/*
QuantProject - Quantitative Finance Library

LinearRegressionGeneticChooser.cs
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

using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.Optimizing.GenomeManagers;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// genetic IInSampleChooser for the linear regression strategy
	/// </summary>
	[Serializable]
	public class LinearRegressionGeneticChooser : GeneticChooser
	{
		private DecoderForLinearRegressionTestingPositions
			decoderForLinearRegressionTestingPositions;
		private IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling;
		private IReturnIntervalsBuilderForTradingAndForSignaling
			returnIntervalsBuilderForTradingAndForSignaling;
		private IEligiblesSelector eligiblesSelectorForSignalingTickers;
		private int seedForRandomGeneratorForGenomeManagerForTradingTickers;
		private int seedForRandomGeneratorForGenomeManagerForSignalingTickers;
		
		public LinearRegressionGeneticChooser(
			int numberOfBestTestingPositionsToBeReturned ,
			Benchmark benchmark ,
			DecoderForLinearRegressionTestingPositions decoderForLinearRegressionTestingPositions ,
			IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling ,
			IReturnIntervalsBuilderForTradingAndForSignaling
			returnIntervalsBuilderForTradingAndForSignaling ,
			IFitnessEvaluator fitnessEvaluator ,
			HistoricalMarketValueProvider historicalMarketValueProvider ,
			IEligiblesSelector eligiblesSelectorForSignalingTickers ,
			double crossoverRate , double mutationRate , double elitismRate ,
			int populationSizeForGeneticOptimizer ,
			int generationNumberForGeneticOptimizer ,
			int seedForRandomGeneratorForTheGeneticOptimizer ,
			int	seedForRandomGeneratorForGenomeManagerForTradingTickers ,
			int seedForRandomGeneratorForGenomeManagerForSignalingTickers ) :
			base(
				2 ,
				numberOfBestTestingPositionsToBeReturned ,
				benchmark ,
				new DummyDecoderForTestingPositions() ,
				fitnessEvaluator ,
				historicalMarketValueProvider ,
				crossoverRate ,
				mutationRate ,
				elitismRate ,
				populationSizeForGeneticOptimizer ,
				generationNumberForGeneticOptimizer ,
				seedForRandomGeneratorForTheGeneticOptimizer
			)
		{
			this.decoderForLinearRegressionTestingPositions =
				decoderForLinearRegressionTestingPositions;
			this.returnIntervalSelectorForSignaling = returnIntervalSelectorForSignaling;
			this.returnIntervalsBuilderForTradingAndForSignaling =
				returnIntervalsBuilderForTradingAndForSignaling;
			this.eligiblesSelectorForSignalingTickers = eligiblesSelectorForSignalingTickers;
			this.seedForRandomGeneratorForGenomeManagerForTradingTickers =
				seedForRandomGeneratorForGenomeManagerForTradingTickers;
			this.seedForRandomGeneratorForGenomeManagerForSignalingTickers =
				seedForRandomGeneratorForGenomeManagerForSignalingTickers;
		}
		
		#region GetGenomeManager
		public override IGenomeManager GetGenomeManager(
			EligibleTickers eligibleTickersForTrading ,
			ReturnsManager returnsManager )
		{
//			IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling =
//				new ShiftedTimeIntervalSelectorForSignaling( new TimeSpan( -24 , 0 , 0 ) );
			EligibleTickers eligibleTickersForSignaling =
				this.eligiblesSelectorForSignalingTickers.GetEligibleTickers(
					returnsManager.ReturnIntervals.BordersHistory );
			ReturnIntervals returnIntervalsForTrading;
			ReturnIntervals returnIntervalsForSignaling;
			this.returnIntervalsBuilderForTradingAndForSignaling.BuildIntervals(
				returnsManager , this.returnIntervalSelectorForSignaling ,
				eligibleTickersForTrading.Tickers , eligibleTickersForSignaling.Tickers ,
				out returnIntervalsForTrading , out returnIntervalsForSignaling );
			ReturnsManager returnsManagerForTradingTickers =
				new ReturnsManager(
					returnIntervalsForTrading ,
					this.historicalMarketValueProvider );
			ReturnsManager returnsManagerForSignalingTickers =
				new ReturnsManager(
					returnIntervalsForSignaling ,
					this.historicalMarketValueProvider );
			

			GenomeManagerForLinearRegression genomeManagerForLinearRegression =
				new GenomeManagerForLinearRegression(
					eligibleTickersForTrading ,
					eligibleTickersForSignaling ,
//					returnsManager ,
					returnsManagerForTradingTickers ,
					returnsManagerForSignalingTickers ,
					this.decoderForLinearRegressionTestingPositions ,
					this.fitnessEvaluator ,
					GenomeManagerType.ShortAndLong ,
					this.seedForRandomGeneratorForGenomeManagerForTradingTickers ,
					this.seedForRandomGeneratorForGenomeManagerForSignalingTickers );
			return genomeManagerForLinearRegression;
		}
		#endregion GetGenomeManager
		
		protected override string getHashCodeForGenome( Genome genome )
		{
			return ((TestingPositions)(genome.Meaning)).HashCodeForTickerComposition;
		}
		
		public override object AnalyzeInSample(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			TestingPositions[] bestTestingPositionsInSample =
				(TestingPositions[])base.AnalyzeInSample( eligibleTickers , returnsManager );
			foreach ( LinearRegressionTestingPositions linearRegressionTestingPositions in
			         bestTestingPositionsInSample )
				linearRegressionTestingPositions.LinearRegression =
					((LinearRegressionFitnessEvaluator)this.fitnessEvaluator).SetUpAndRunLinearRegression(
						linearRegressionTestingPositions );
			return bestTestingPositionsInSample;
		}
	}
}
