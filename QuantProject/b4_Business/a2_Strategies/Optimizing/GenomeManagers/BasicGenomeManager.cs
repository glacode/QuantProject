/*
QuantProject - Quantitative Finance Library

BasicGenomeManager.cs
Copyright (C) 2008
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

using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;


namespace QuantProject.Business.Strategies.Optimizing.GenomeManagers
{
	/// <summary>
	/// This is a basic abstract implementation of IGenomeManager, that should be
	/// inherited by specific GenomeManagers for specific strategies
	/// </summary>
	[Serializable]
	public abstract class BasicGenomeManager : IGenomeManager
	{
		protected int genomeSize;
		protected int minValueForGenes;
		protected int maxValueForGenes;
		protected GenomeManagerType genomeManagerType;
		protected EligibleTickers eligibleTickers;
		protected IReturnsManager returnsManager;
		protected IDecoderForTestingPositions decoderForTestingPositions;
		protected IFitnessEvaluator fitnessEvaluator;
    
		public virtual int GenomeSize
		{
			get{return this.genomeSize;}
		}
		public virtual int GetMinValueForGenes(int genePosition)
		{
			return this.minValueForGenes;
		}
		public virtual int GetMaxValueForGenes(int genePosition)
		{
			return this.maxValueForGenes;
		}
		
		public BasicGenomeManager(EligibleTickers eligibleTickers,
		                          int numberOfTickersInPortfolio,
		                          IDecoderForTestingPositions decoderForTestingPositions,
		                          IFitnessEvaluator fitnessEvaluator,
		                          GenomeManagerType genomeManagerType,
		                          IReturnsManager returnsManager ,
		                          int seedForRandomGenerator )
			
		{
			this.eligibleTickers = eligibleTickers;
			this.genomeSize = numberOfTickersInPortfolio;
			this.decoderForTestingPositions = decoderForTestingPositions;
			this.fitnessEvaluator = fitnessEvaluator;
			this.genomeManagerType = genomeManagerType;
			this.returnsManager = returnsManager;
			this.setMinAndMaxValueForGenes();
			GenomeManagement.SetRandomGenerator( seedForRandomGenerator );
		}
    
		private void setMinAndMaxValueForGenes()
		{
			switch (this.genomeManagerType) 
			{
				case GenomeManagerType.OnlyLong :        
					//OnlyLong orders are admitted
					this.minValueForGenes = 0;
					this.maxValueForGenes = this.eligibleTickers.Count - 1;
					break;
				case GenomeManagerType.OnlyShort :        
					//OnlyShort orders are admitted
					this.minValueForGenes = - this.eligibleTickers.Count;
					//if gene g is negative, it refers to the ticker |g|-1 to be shorted
					this.maxValueForGenes = - 1;
					break;
				default :        
					//Both Long and Short orders are admitted
					this.minValueForGenes = - this.eligibleTickers.Count;
					this.maxValueForGenes = this.eligibleTickers.Count - 1;
					break;
			}
		}
		
		public virtual double GetFitnessValue(Genome genome)
		{
			double fitnessValue = 
					this.fitnessEvaluator.GetFitnessValue(genome.Meaning, this.returnsManager);
				
			return fitnessValue;
		}
    
		public abstract Genome[] GetChilds(Genome parent1, Genome parent2);
		public abstract int GetNewGeneValue(Genome genome, int genePosition);
//		{
//			// in this implementation new gene values must be different from
//			// the others already stored in the given genome
//			int returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
//				genome.GetMaxValueForGenes(genePosition) + 1);
//			while( GenomeManipulator.IsTickerContainedInGenome(returnValue,genome) )
//				//the portfolio can't have a long position and a short one for the same ticker 
//			{
//				returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
//					genome.GetMaxValueForGenes(genePosition) + 1);
//			}
//			return returnValue;
//		}
        
		public abstract void Mutate(Genome genome);
//		{
//			// in this implementation only one gene is mutated
//			// the new value has to be different from all the other genes of the genome
//			int genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
//			int newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
//				genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);
//			while( GenomeManipulator.IsTickerContainedInGenome(newValueForGene,genome) )
//				//the portfolio can't have a long position and a short one for the same ticker
//			{
//				newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
//					genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);
//			}
//			GenomeManagement.MutateOneGene(genome, genePositionToBeMutated, newValueForGene);
//		}

//		public virtual object Decode(BruteForceOptimizableParameters bruteForceOptimizableParameters)
//		{
//			//
//		}
		
		public virtual object Decode(Genome genome)
		{
			object decoded =
				this.decoderForTestingPositions.Decode(genome.Genes(),
				                                       this.eligibleTickers,	this.returnsManager);
			return decoded;
		}
	}
}
