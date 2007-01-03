/*
QuantProject - Quantitative Finance Library

GenomeManagerForWeightedEfficientPortfolio.cs
Copyright (C) 2003 
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
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
  /// <summary>
  /// This is the base class implementing IGenomeManager, in order to find
  /// efficient portfolios in which tickers are weighted differently
  /// </summary>
  [Serializable]
  public class GenomeManagerForWeightedEfficientPortfolio : GenomeManagerForEfficientPortfolio
  {
    public GenomeManagerForWeightedEfficientPortfolio(DataTable setOfInitialTickers,
      DateTime firstQuoteDate,
      DateTime lastQuoteDate,
      int numberOfTickersInPortfolio,
      double targetPerformance,
      PortfolioType portfolioType):base(setOfInitialTickers,
                                        firstQuoteDate,
                                        lastQuoteDate,
                                        numberOfTickersInPortfolio,
                                        targetPerformance,
                                        portfolioType)
                          
    {
      this.genomeSize = 2*this.genomeSize;
      //at even position the gene is used for finding
      //the coefficient for the ticker represented at the next odd position
    }
    
    #region override getPortfolioRatesOfReturn
    
    protected override double getTickerWeight(int[] genes, int tickerPositionInGenes)
    {
      double minimumWeight = (1.0 - ConstantsProvider.AmountOfVariableWeightToBeAssignedToTickers)/
                             (genes.Length / 2);
      double totalOfValuesForWeightsInGenes = 0.0;
      for(int j = 0; j<genes.Length; j++)
      {
        if(j%2==0)
          //ticker weight is contained in genes at even position
          totalOfValuesForWeightsInGenes += Math.Abs(genes[j]) + 1.0;
        //0 has to be avoided !
      }
      double freeWeight = (Math.Abs(genes[tickerPositionInGenes-1]) + 1.0)/totalOfValuesForWeightsInGenes;
      return minimumWeight + freeWeight * (1.0 - minimumWeight * genes.Length / 2);
    }
 
    protected override double[] getPortfolioRatesOfReturn(int[] genes)
    {
      double[] returnValue = new double[this.numberOfExaminedReturns];
      for(int i = 0; i<returnValue.Length; i++)    
      {  
        for(int j = 0; j<genes.Length; j++)
        {
        	if(j%2==1)
        	//ticker ID is contained in genes at odd position
	        	returnValue[i] +=
	        	this.getPortfolioRatesOfReturn_getRateOfTickerToBeAddedToTheArray(genes,j,i);
        }
      }
      return returnValue;
    }

    #endregion

    #region override Decode

    public override object Decode(Genome genome)
    {
      string[] arrayOfTickers = new string[genome.Genes().Length/2];
      double[] arrayOfTickersWeights = new double[genome.Genes().Length/2];
      int indexOfTicker;
      int i = 0;//for the arrayOfTickers
      for(int index = 0; index < genome.Genes().Length; index++)
      {
        if(index%2==1)
          //indexForTicker is contained in genes at odd position
        {
          indexOfTicker = (int)genome.Genes().GetValue(index);
          arrayOfTickers[i] = this.decode_getTickerCodeForLongOrShortTrade(indexOfTicker);
          arrayOfTickersWeights[i] = this.getTickerWeight(genome.Genes(), index);
          i++;
        }
      }
      GenomeMeaning meaning = new GenomeMeaning(arrayOfTickers,
																				        arrayOfTickersWeights,
																				        this.PortfolioRatesOfReturn[this.portfolioRatesOfReturn.Length - 1],
																				        this.RateOfReturn,
																				        this.Variance);
      return meaning;
      
    }
    #endregion

    public override int GetNewGeneValue(Genome genome, int genePosition)
    {
      // in this implementation only new gene values pointing to tickers
      // must be different from the others already stored (in odd positions of genome)
      int returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
                                                              genome.GetMaxValueForGenes(genePosition) + 1);
      while(genePosition%2 == 1 
            && GenomeManipulator.IsTickerContainedInGenome(returnValue,genome))
      //while in the given position has to be stored
      //a new gene pointing to a ticker and
      //the proposed returnValue points to a ticker
      //already stored in the given genome
      {
        // a new returnValue has to be generated
      	returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
                                                            genome.GetMaxValueForGenes(genePosition) + 1);
      }

      return returnValue;
    }
    
    #region override Mutate
    
    //OLD VERSION   
//    public override void Mutate(Genome genome, double mutationRate)
//    {
//      // in this implementation only one gene is mutated
//      int genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
//      int newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
//                                                                  genome.GetMaxValueForGenes(genePositionToBeMutated) +1);
//       
//      while(genePositionToBeMutated%2 == 1 &&
//            GenomeManipulator.IsTickerContainedInGenome(newValueForGene,genome))
//      //while in the proposed genePositionToBeMutated has to be stored
//      //a new gene pointing to a ticker and
//      //the proposed newValueForGene points to a ticker
//      //already stored in the given genome
//      {
//        newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
//                                                                genome.GetMaxValueForGenes(genePositionToBeMutated) +1);
//      }
//      GenomeManagement.MutateOneGene(genome, mutationRate,
//        genePositionToBeMutated, newValueForGene);
//    }
		
    private int mutate_MutateOnlyOneWeight_getNewWeight(Genome genome, int genePositionToBeMutated)
    {
      int returnValue;
      double partOfGeneToSubtractOrAdd = 0.25;
      int geneValue = Math.Abs(genome.GetGeneValue(genePositionToBeMutated));
      int subtractOrAdd = GenomeManagement.RandomGenerator.Next(2);
      if(subtractOrAdd == 1)//subtract a part of the gene value from the gene value itself
        returnValue = geneValue - Convert.ToInt32(partOfGeneToSubtractOrAdd*geneValue);
      else
        returnValue = Math.Min(genome.GetMaxValueForGenes(genePositionToBeMutated),
                               geneValue + Convert.ToInt32(partOfGeneToSubtractOrAdd*geneValue));
      return returnValue;
    }

    private void mutate_MutateOnlyOneWeight(Genome genome)
    {
      int genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
      while(genePositionToBeMutated%2 == 1 )
        //while the proposed genePositionToBeMutated points to a ticker
        genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
	      
      int newValueForGene = this.mutate_MutateOnlyOneWeight_getNewWeight(genome, genePositionToBeMutated);	
	    
      GenomeManagement.MutateOneGene(genome, genePositionToBeMutated, newValueForGene);
    }
    
    private void mutate_MutateAllGenes(Genome genome)
    {
      for(int genePositionToBeMutated = 0;
          genePositionToBeMutated < genome.Genes().Length;
          genePositionToBeMutated ++)
      {
        int newValueForGene = 
          GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
            genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);
        while(genePositionToBeMutated%2 == 1 
          && GenomeManipulator.IsTickerContainedInGenome(newValueForGene,genome))
          //while in the given position has to be stored
          //a new gene pointing to a ticker and
          //the proposed newValueForGene points to a ticker
          //already stored in the given genome
          // a new newalueForGene has to be generated
          newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
            genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);
		      
        genome.SetGeneValue(newValueForGene, genePositionToBeMutated);
      }
    }
        
    //new version
		//mutation means just a change in one single weight
		//or in a complete new genome (with a probability of 50%)
    public override void Mutate(Genome genome)
    {
    	int mutateOnlyOneWeight = GenomeManagement.RandomGenerator.Next(2);
    	if(mutateOnlyOneWeight == 1)
    	 	this.mutate_MutateOnlyOneWeight(genome);
    	else//mutate all genome's genes
    	  this.mutate_MutateAllGenes(genome); 
    }

    #endregion

  }

}
