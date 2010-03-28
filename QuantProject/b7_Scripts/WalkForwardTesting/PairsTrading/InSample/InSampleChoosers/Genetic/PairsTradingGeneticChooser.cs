/*
QuantProject - Quantitative Finance Library

PairsTradingGeneticChooser.cs
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

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// genetic IInSampleChooser for the pairs trading strategy
	/// </summary>
	[Serializable]
	public class PairsTradingGeneticChooser : GeneticChooser
	{
		public PairsTradingGeneticChooser(
			int numberOfBestTestingPositionsToBeReturned ,
			Benchmark benchmark ,
			IDecoderForTestingPositions decoderForTestingPositions ,
			IFitnessEvaluator fitnessEvaluator ,
			HistoricalMarketValueProvider historicalMarketValueProvider ,
			double crossoverRate , double mutationRate , double elitismRate ,
			int populationSizeForGeneticOptimizer ,
			int generationNumberForGeneticOptimizer ,
			int seedForRandomGenerator ) :
			base(
				2 ,
				numberOfBestTestingPositionsToBeReturned ,
				benchmark ,
				decoderForTestingPositions ,
				fitnessEvaluator ,
				historicalMarketValueProvider ,
				crossoverRate ,
				mutationRate ,
				elitismRate ,
				populationSizeForGeneticOptimizer ,
				generationNumberForGeneticOptimizer ,
				seedForRandomGenerator )
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public override IGenomeManager GetGenomeManager(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			GenomeManagerWithDuplicateGenes	genomeManagerWithDuplicateGenes =
				new GenomeManagerWithDuplicateGenes(
				2 ,
				eligibleTickers ,
				returnsManager ,
				this.decoderForTestingPositions ,
				this.fitnessEvaluator ,
				GenomeManagerType.ShortAndLong ,
				this.seedForRandomGeneratorForTheGeneticOptimizer );
			return genomeManagerWithDuplicateGenes;
		}
		protected override string getHashCodeForGenome( Genome genome )
		{
			return ((TestingPositions)(genome.Meaning)).HashCodeForTickerComposition;
		}
	}
}
