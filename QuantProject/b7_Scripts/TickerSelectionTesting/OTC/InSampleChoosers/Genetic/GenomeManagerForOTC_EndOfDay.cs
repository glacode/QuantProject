/*
QuantProject - Quantitative Finance Library

GenomeManagerForOTC_EndOfDay.cs
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

namespace QuantProject.Scripts.TickerSelectionTesting.OTC.InSampleChoosers.Genetic
{
	/// <summary>
	/// Implements what needed to use the Genetic Optimizer
	/// for finding the portfolio that best suites
	/// the Open To Close strategy using EOD data
	/// </summary>
	[Serializable]
  public class GenomeManagerForOTC_EndOfDay : BasicGenomeManager
  {
    public GenomeManagerForOTC_EndOfDay(EligibleTickers eligibleTickers,
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
  
  //		private bool getFitnessValue_isMixed_areSignsAllPositiveOrAllNegative(int[] signs)
//		{
//			bool returnValue = false;
//			int n = signs.Length;
//			int sumOfSigns = 0;
//			for(int i = 0; i < n ; i++)
//				sumOfSigns += signs[i];
//			int absoluteOfSumOfSigns = Math.Abs(sumOfSigns);
//			if( absoluteOfSumOfSigns != n &&
//			    absoluteOfSumOfSigns != - n )
//				returnValue = true;
//			return returnValue;
//		}
//		
//		private bool getFitnessValue_isMixed(Genome genome)
//		{
//			int[] signs = genome.Genes();
//			int n = signs.Length;
//			for(int i = 0; i < n ; i++)
//			{
//				if(signs[i] >= 0)
//					signs[i] = 1;
//				else
//					signs[i] = -1;
//			}
//			return getFitnessValue_isMixed_areSignsAllPositiveOrAllNegative(signs);
//		}	
  	
	public override double GetFitnessValue(Genome genome)
	{
		double fitnessValue = double.MinValue;
		
		if( (this.genomeSize > 1 && 
		     this.genomeManagerType == GenomeManagerType.OnlyMixed &&
		     ((OTCPositions)genome.Meaning).BothLongAndShortPositions ) ||
		     this.genomeManagerType != GenomeManagerType.OnlyMixed )
		//if the genomeManager has to evaluate only mixed portfolio,
		// with at least two tickers and
		//the current genome is not made of tickers with the same sign OR
		//the genomeManager can evaluate mixed or not mixed portfolios
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
