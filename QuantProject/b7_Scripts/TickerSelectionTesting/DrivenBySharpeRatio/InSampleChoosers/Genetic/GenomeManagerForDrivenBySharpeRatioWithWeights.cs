/*
QuantProject - Quantitative Finance Library

GenomeManagerForDrivenBySharpeRatioWithWeights.cs
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
  public class GenomeManagerForDrivenBySharpeRatioWithWeights : GenomeManagerForDrivenBySharpeRatio
  {
    public GenomeManagerForDrivenBySharpeRatioWithWeights(EligibleTickers eligibleTickers,
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
  		this.genomeSize = 2 * this.genomeSize;
    }
  	
  	public override int GetMinValueForGenes(int genePosition)
		{
  		int returnValue = this.minValueForGenes;
  		//default value for genes mapping tickers
  		if(genePosition %2 == 0)
  		//the gene maps weights 
  			returnValue = 10;
  		return returnValue;
		}
		public override int GetMaxValueForGenes(int genePosition)
		{
			int returnValue = this.maxValueForGenes;
  		//default value for genes mapping tickers
			if(genePosition %2 == 0)
			//the gene maps weights	
  			returnValue = 100;
  		return returnValue;
		}
  	
  	public override Genome[] GetChilds(Genome parent1, Genome parent2)
    {
			return GenomeManagement.OnePointCrossover(parent1, parent2);
    }
		
    public override int GetNewGeneValue(Genome genome, int genePosition)
    {
      //The optimization has to be run only on weights, for this genome manager
      //The number of tickers in eligibles is equal to the 
      //number of genes dedicated to tickers
      int returnValue;
      if( genePosition %2 == 0 )
      //gene position is even, so it contains a weight
      	returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
                           genome.GetMaxValueForGenes(genePosition)+1);
      else
      //gene position is odd, so it contains a ticker code
      //which is fixed in this implementation
      	returnValue = (genePosition - 1) / 2;
      
      return returnValue;
    }
	  
    public override void Mutate(Genome genome)
    {
      // in this implementation only one gene, corresponding to
      // a weight, has to be mutated
      int genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
     	int newValueForGene;
     	while( genePositionToBeMutated %2 != 0 )
			//the genePosition doesn't point to a weight
      {
        genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
      }
      newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
                        	genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);

     	GenomeManagement.MutateOneGene(genome, genePositionToBeMutated, newValueForGene);
    }
  }
}
