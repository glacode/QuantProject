/*
QuantProject - Quantitative Finance Library

GenomeManagerWithDuplicateGenes.cs
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
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Business.Strategies.Optimizing.GenomeManagers
{
	/// <summary>
	/// Genome manager with duplicate genes
	/// </summary>
	public class GenomeManagerWithDuplicateGenes : BasicGenomeManager
	{
		public GenomeManagerWithDuplicateGenes(
			int numberOfPortfolioPositions ,
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager ,
			IDecoderForTestingPositions decoderForTestingPositions ,
			IFitnessEvaluator fitnessEvaluator ,
			GenomeManagerType genomeManagerType ,
			int seedForRandomGenerator ) :
			base(
				eligibleTickers ,
				numberOfPortfolioPositions ,
				decoderForTestingPositions ,
				fitnessEvaluator ,
				genomeManagerType ,
				returnsManager ,
				seedForRandomGenerator )
		{
		}
		public override int GetNewGeneValue( Genome genome , int genePosition )
		{
			int minGeneValue = this.GetMinValueForGenes( genePosition );
			int maxGeneValue = this.GetMaxValueForGenes( genePosition );
			int returnValue =
				GenomeManagement.RandomGenerator.Next(
				minGeneValue , maxGeneValue + 1);
			return returnValue;
		}
//		public object Decode( Genome genome )
//		{
//			return this.decoderForWeightedPositions.Decode(
//				genome.Genes() , this.eligibleTickers , this.returnsManager );
//		}
//		public double GetFitnessValue( Genome genome )
//		{
//			object meaning = this.Decode( genome );
//			double fitnessValue =
//				this.fitnessEvaluator.GetFitnessValue( meaning , this.returnsManager );
//			return fitnessValue;
//		}
		public override Genome[] GetChilds(
			Genome parent1 , Genome parent2 )
		{
			return
				GenomeManagement.AlternateFixedCrossover(parent1, parent2);
		}

		public override void Mutate( Genome genome )
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
