/*
QuantProject - Quantitative Finance Library

GenomeManagerForDrivenBySharpeRatio.cs
Copyright (C) 2011 
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
using System.Data;
using System.Collections;

using QuantProject.ADT;
using QuantProject.ADT.Statistics;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Data;
using QuantProject.Data.DataTables;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.TickersRelationships;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.GenomeManagers;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TickerSelectionTesting.DrivenBySharpeRatio.InSampleChoosers.Genetic
{
	/// <summary>
	/// Implements a simple GenomeManager for finding
	/// the portfolio with the highest Sharpe Ratio
	/// </summary>
	[Serializable]
  public class GenomeManagerForDrivenBySharpeRatio : BasicGenomeManager
  {
    public GenomeManagerForDrivenBySharpeRatio(EligibleTickers eligibleTickers,
                           int numberOfTickersInPortfolio,
                           IDecoderForTestingPositions decoderForTestingPositions,
                           IFitnessEvaluator fitnessEvaluator,
                           GenomeManagerType genomeManagerType,
													 ReturnsManager returnsManager,
													 int seedForRandomGenerator)
                           :
														base(eligibleTickers,
														numberOfTickersInPortfolio,
														decoderForTestingPositions,
														fitnessEvaluator,		
														genomeManagerType,
														returnsManager,
														seedForRandomGenerator)
                                
                          
    {
    }
  	
		public override double GetFitnessValue(Genome genome)
		{
			double fitnessValue = double.MinValue;
			fitnessValue =
					this.fitnessEvaluator.GetFitnessValue(genome.Meaning, this.returnsManager);
				
			return fitnessValue;
		}
  	
    public override Genome[] GetChilds(Genome parent1, Genome parent2)
    {
    	return
      	GenomeManipulator.MixGenesWithoutDuplicates(parent1, parent2);
    }
		
    public override int GetNewGeneValue(Genome genome, int genePosition)
    {
     // in this implementation new gene values must be different from
      // the others already stored in the given genome
      int returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
                           genome.GetMaxValueForGenes(genePosition) + 1);
      while( GenomeManipulator.IsTickerContainedInGenome(returnValue,genome) )
      //the portfolio can't have a long position and a short one for the same ticker 
      {
        returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
                           genome.GetMaxValueForGenes(genePosition) + 1);
      }
      return returnValue;
    }
	  
    public override void Mutate(Genome genome)
    {
      // in this implementation only one gene is mutated
      // the new value has to be different from all the other genes of the genome
      int genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
      int newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
                            genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);
     	while( GenomeManipulator.IsTickerContainedInGenome(newValueForGene,genome) )
			//the portfolio can't have a long position and a short one for the same ticker
      {
        newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
                            genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);
      }
      GenomeManagement.MutateOneGene(genome, genePositionToBeMutated, newValueForGene);
    }
  }
}
