/*
QuantProject - Quantitative Finance Library

FixedLengthTwoPhasesGeneticChooser.cs
Copyright (C) 2007
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

using QuantProject.ADT;
using QuantProject.ADT.Messaging;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.Optimizing.GenomeManagers;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.WalkForwardTesting.FixedLengthTwoPhases
{
	/// <summary>
	/// In sample analyzer for the walk forward fixed length two phases strategy
	/// </summary>
	public class FixedLengthTwoPhasesGeneticChooser : GeneticChooser
	{

		public FixedLengthTwoPhasesGeneticChooser(
			int numberOfPortfolioPositions ,
			int numberOfBestTestingPositionsToBeReturned ,
			Benchmark benchmark ,
			IDecoderForTestingPositions decoderForTestingPositions ,
			IFitnessEvaluator fitnessEvaluator ,
			IHistoricalQuoteProvider historicalQuoteProvider ,
			double crossoverRate , double mutationRate , double elitismRate ,
			int populationSizeForGeneticOptimizer ,
			int generationNumberForGeneticOptimizer ,
			int seedForRandomGenerator ) :
			base(
				numberOfPortfolioPositions ,
				numberOfBestTestingPositionsToBeReturned ,
				benchmark ,
				decoderForTestingPositions ,
				fitnessEvaluator ,
				historicalQuoteProvider ,
				crossoverRate ,
				mutationRate ,
				elitismRate ,
				populationSizeForGeneticOptimizer ,
				generationNumberForGeneticOptimizer ,
				seedForRandomGenerator )
		{
		}



		
		protected override IGenomeManager getGenomeManager(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			GenomeManagerWithDuplicateGenes	genomeManagerWithDuplicateGenes =
				new GenomeManagerWithDuplicateGenes(
					this.numberOfPortfolioPositions ,
					eligibleTickers ,
					returnsManager ,
					this.decoderForTestingPositions ,
					this.fitnessEvaluator ,
					GenomeManagerType.ShortAndLong ,
					this.seedForRandomGenerator );
			return genomeManagerWithDuplicateGenes;
		}
		
		protected override string getHashCodeForGenome( Genome genome )
		{
			return ((FLTPTestingPositions)(genome.Meaning)).HashCodeForTickerComposition;
		}

	}
}
