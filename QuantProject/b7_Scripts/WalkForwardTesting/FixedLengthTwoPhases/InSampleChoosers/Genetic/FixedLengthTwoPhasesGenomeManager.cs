/*
QuantProject - Quantitative Finance Library

FixedLengthTwoPhasesGenomeManager.cs
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

using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Scripts.WalkForwardTesting.FixedLengthTwoPhases
{
	/// <summary>
	/// Genome manager for the fixed length two phases strategy
	/// </summary>
	public class FixedLengthTwoPhasesGenomeManager : IGenomeManager
	{
		int numberOfPortfolioPositions;
		EligibleTickers eligibleTickers;
		ReturnsManager returnsManager;
		IDecoderForWeightedPositions decoderForWeightedPositions;
		IFitnessEvaluator fitnessEvaluator;
		int seedForRandomGenerator;

		public FixedLengthTwoPhasesGenomeManager(
			int numberOfPortfolioPositions ,
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager ,
			IDecoderForWeightedPositions decoderForWeightedPositions ,
			IFitnessEvaluator fitnessEvaluator ,
			int seedForRandomGenerator )
		{
			this.numberOfPortfolioPositions = numberOfPortfolioPositions;
			this.eligibleTickers = eligibleTickers;
			this.returnsManager = returnsManager;
			this.decoderForWeightedPositions = decoderForWeightedPositions;
			this.fitnessEvaluator = fitnessEvaluator;
			this.seedForRandomGenerator = seedForRandomGenerator;
		}
		public int GenomeSize
		{
			get
			{
				return ( this.numberOfPortfolioPositions );
			}
		}
		public int GetMaxValueForGenes( int genePosition )
		{
			return this.eligibleTickers.Count - 1;
		}
		public int GetMinValueForGenes( int genePosition )
		{
			int minValueForGene =
				-this.eligibleTickers.Count;
			return minValueForGene;
		}
		public int GetNewGeneValue( Genome genome , int genePosition )
		{
			int minGeneValue = this.GetMinValueForGenes( genePosition );
			int maxGeneValue = this.GetMaxValueForGenes( genePosition );
			int returnValue =
				GenomeManagement.RandomGenerator.Next(
				minGeneValue , maxGeneValue + 1);
			return returnValue;
		}
		public object Decode( Genome genome )
		{
			return this.decoderForWeightedPositions.Decode(
				genome.Genes() , this.eligibleTickers , this.returnsManager );
		}
		public double GetFitnessValue( Genome genome )
		{
			object meaning = this.Decode( genome );
			double fitnessValue =
				this.fitnessEvaluator.GetFitnessValue( meaning , this.returnsManager );
			return fitnessValue;
		}
		public Genome[] GetChilds( Genome parent1 , Genome parent2 )
		{
			return
				GenomeManagement.AlternateFixedCrossover(parent1, parent2);
		}

		public void Mutate( Genome genome )
		{
			//			int newValueForGene = GenomeManagement.RandomGenerator.Next(
			//				genome.MinValueForGenes ,
			//				genome.MaxValueForGenes + 1 );
			int genePositionToBeMutated =
				GenomeManagement.RandomGenerator.Next( genome.Size ); 
			int newValueForGene =
				this.GetNewGeneValue( genome , genePositionToBeMutated );
			GenomeManagement.MutateOneGene( genome ,
				genePositionToBeMutated , newValueForGene );
		}
	}
}
