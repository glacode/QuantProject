/*
QuantProject - Quantitative Finance Library

GenomeManagerWeightedPVO.cs
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
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.WeightedPVO
{
	/// <summary>
	/// Implements what needed to use the Genetic Optimizer
	/// for finding the portfolio that best suites
	/// the Portfolio Value Oscillator strategy
	/// </summary>
	[Serializable]
  public class GenomeManagerWeightedPVO : GenomeManagerPVO
  {
    protected int[] genePositionsPointingToTickers;
          
    public GenomeManagerWeightedPVO(DataTable setOfInitialTickers,
                           DateTime firstQuoteDate,
                           DateTime lastQuoteDate,
                           int numberOfTickersInPortfolio,
                           int numDaysForOscillatingPeriod,
                           int minLevelForOversoldThreshold,
	                         int maxLevelForOversoldThreshold,
	                         int minLevelForOverboughtThreshold,
	                         int maxLevelForOverboughtThreshold,
                           int divisorForThresholdComputation,
                           bool symmetricalThresholds,
                           bool overboughtMoreThanOversoldForFixedPortfolio,
                           PortfolioType inSamplePortfolioType,
                           string benchmark)
                           :
                          base(setOfInitialTickers,
                           firstQuoteDate, lastQuoteDate,
                           numberOfTickersInPortfolio,
                           numDaysForOscillatingPeriod,
                           minLevelForOversoldThreshold,
	                         maxLevelForOversoldThreshold,
	                         minLevelForOverboughtThreshold,
	                         maxLevelForOverboughtThreshold,
                           divisorForThresholdComputation,
                           symmetricalThresholds,
                           overboughtMoreThanOversoldForFixedPortfolio,
                           inSamplePortfolioType,
                           benchmark)
                                
                          
    {
    	this.genePositionsPointingToTickers = new int[(this.GenomeSize - this.numOfGenesDedicatedToThresholds)/2];
    }
    
    protected bool genePositionPointsToATicker(int genePosition)
    {
      return ( genePosition >= this.numOfGenesDedicatedToThresholds &&
    	        ( (this.numOfGenesDedicatedToThresholds == 1 && genePosition%2 == 0) ||
    	          (this.numOfGenesDedicatedToThresholds == 2 && genePosition%2 != 0)   )   );
    }
		
    protected bool genePositionPointsToAWeight(int genePosition)
    {
      return ( genePosition >= this.numOfGenesDedicatedToThresholds &&
    	        ( (this.numOfGenesDedicatedToThresholds == 1 && genePosition%2 != 0) ||
    	          (this.numOfGenesDedicatedToThresholds == 2 && genePosition%2 == 0)   )   );
    }
    
    protected bool genePositionPointsToATreshold(int genePosition)
    {
    	return ( genePosition < this.numOfGenesDedicatedToThresholds );
    }
    
    public int[] GetGenePositionsPointingToTickers(Genome genome)
    {
    	int i = 0;
    	for(int genePosition = this.numOfGenesDedicatedToThresholds;
    	    genePosition < genome.Size;
    	    genePosition++)
    	{
    		if(this.genePositionPointsToATicker(genePosition) )
    		{
    			this.genePositionsPointingToTickers[i] = genePosition;
    			i++;
    		}
    	}
    	return this.genePositionsPointingToTickers;
    }
   
    
    public override int GenomeSize
    {
      get{return this.genomeSize * 2
    			+ this.numOfGenesDedicatedToThresholds;}
    }

    #region Get Min and Max Value

    private int getMinValueForGenes_getMinValueForTicker()
    {
      int returnValue;
      switch (this.portfolioType)
      {
        case PortfolioType.OnlyLong :
          returnValue = 0;
          break;
        default://For ShortAndLong or OnlyShort portfolios
          returnValue = - this.originalNumOfTickers;
          break;
      }
      return returnValue;
    }

    public override int GetMinValueForGenes(int genePosition)
    {
    	int returnValue;
    	switch (genePosition)
    	{
    		case 0 ://gene for oversold threshold
					returnValue = this.minLevelForOversoldThreshold;
					break;
				case 1 ://gene for overbought threshold if thresholds are asymmetrical
					if(this.numOfGenesDedicatedToThresholds == 2)
						returnValue = this.minLevelForOverboughtThreshold;
					else
            returnValue = this.getMinValueForGenes_getMinValueForTicker();
				  break;
				default://gene for ticker or weight
					returnValue = this.getMinValueForGenes_getMinValueForTicker();
					break;
      }
    	return returnValue;
    }
    
    private int getMaxValueForGenes_getMaxValueForTicker()
    {
      int returnValue;
      switch (this.portfolioType)
      {
        case PortfolioType.OnlyShort :
          returnValue = - 1;
          break;
        default ://For ShortAndLong or OnlyLong portfolios
          returnValue = this.originalNumOfTickers - 1;
          break;
      }
      return returnValue;
    }

    public override int GetMaxValueForGenes(int genePosition)
    {
      int returnValue;
    	switch (genePosition)
    	{
    		case 0 ://gene for oversold threshold
					returnValue = this.maxLevelForOversoldThreshold;
					break;
				case 1 ://gene for overbought threshold when thresholds are asymmetrical
					if(this.numOfGenesDedicatedToThresholds == 2)
						returnValue = this.maxLevelForOverboughtThreshold;
					else
						returnValue = this.getMaxValueForGenes_getMaxValueForTicker();
					break;
				default://gene for ticker
					returnValue = this.getMaxValueForGenes_getMaxValueForTicker();
					break;
      }
    	return returnValue;
    }																
  	
    #endregion
												
    protected override double getTickerWeight(int[] genes, int tickerPositionInGenes)
    {
    	int numOfGenesDedicatedToWeights = this.genePositionsPointingToTickers.Length;
    	double minimumWeight = (1.0 - ConstantsProvider.AmountOfVariableWeightToBeAssignedToTickers)/
      											  numOfGenesDedicatedToWeights;
      double totalOfValuesForWeightsInGenes = 0.0;
      for(int j = this.numOfGenesDedicatedToThresholds; j<genes.Length; j++)
      {
        if(this.numOfGenesDedicatedToThresholds == 2 && j%2==0)
          //ticker weight is contained in genes at even position when 
        	//thresholds are asymmetrical
          totalOfValuesForWeightsInGenes += Math.Abs(genes[j]) + 1.0;
        	//0 has to be avoided !
       	else if (this.numOfGenesDedicatedToThresholds == 1 && j%2!=0)
       		//ticker weight is contained in genes at odd position when 
        	//thresholds are symmetrical
          totalOfValuesForWeightsInGenes += Math.Abs(genes[j]) + 1.0;
        	//0 has to be avoided !
       		
      }
      double freeWeight = (Math.Abs(genes[tickerPositionInGenes-1]) + 1.0)/totalOfValuesForWeightsInGenes;
      return minimumWeight + freeWeight * (1.0 - minimumWeight * numOfGenesDedicatedToWeights);
    }

    public override object Decode(Genome genome)
    {
      string[] arrayOfTickers = new string[this.genePositionsPointingToTickers.Length];
      double[] arrayOfTickerWeights =	new double[this.genePositionsPointingToTickers.Length];
      int geneForTicker;
      GenomeMeaningPVO meaning;
      int i = 0;//for the arrayOfTickers and the arrayOfWeights
      for(int genePosition = this.numOfGenesDedicatedToThresholds;
          genePosition < genome.Genes().Length;
          genePosition++)
      {
      	if( this.genePositionPointsToATicker(genePosition) )
      	{
      		geneForTicker = (int)genome.Genes().GetValue(genePosition);
        	arrayOfTickers[i] = 
                  this.decode_getTickerCodeForLongOrShortTrade(geneForTicker);
        	arrayOfTickerWeights[i] = this.getTickerWeight(genome.Genes(), genePosition);
        	i++;
       	}
      }// end of for cycle inside genome
      
      if(this.symmetricalThresholds)
      		meaning = new GenomeMeaningPVO(
                                      arrayOfTickers,
                                      arrayOfTickerWeights,
                                      Convert.ToDouble(genome.Genes()[0])/Convert.ToDouble(this.divisorForThresholdComputation),
                                      Convert.ToDouble(genome.Genes()[0])/Convert.ToDouble(this.divisorForThresholdComputation),
                                     	this.numDaysForOscillatingPeriod);
      else
      		meaning = new GenomeMeaningPVO(
                                      arrayOfTickers,
                                      arrayOfTickerWeights,
                                      Convert.ToDouble(genome.Genes()[0])/Convert.ToDouble(this.divisorForThresholdComputation),
                                      Convert.ToDouble(genome.Genes()[1])/Convert.ToDouble(this.divisorForThresholdComputation),
                                     	this.numDaysForOscillatingPeriod);
      return meaning;
    }

    public override Genome[] GetChilds(Genome parent1, Genome parent2)
    {
    	//in this simple implementation
    	//child have the tickers of one parent
    	//and the thresholds of the other
    	Genome[] childs = new Genome[2];
    	childs[0] = parent1.Clone();
    	childs[1] = parent2.Clone();
    	//exchange of genes coding thresholds
     	
    	if(this.symmetricalThresholds)//unique value for thresholds
    	{
    		childs[0].SetGeneValue(parent2.GetGeneValue(0),0);
    		childs[1].SetGeneValue(parent1.GetGeneValue(0),0);
    	}
    	else//two different values for thresholds
    	{
    		childs[0].SetGeneValue(parent2.GetGeneValue(0),0);
    		childs[1].SetGeneValue(parent1.GetGeneValue(0),0);
    		childs[0].SetGeneValue(parent2.GetGeneValue(1),1);
    		childs[1].SetGeneValue(parent1.GetGeneValue(1),1);
    	}
    	return childs;
    }

    private int getNewGeneValue_getGeneValue(Genome genome, int genePosition)
    {
      int returnValue;
      int minValueForGene = genome.GetMinValueForGenes(genePosition);
      int maxValueForGene = genome.GetMaxValueForGenes(genePosition);
      
      if(this.numOfGenesDedicatedToThresholds == 2 &&
          this.overboughtMoreThanOversoldForFixedPortfolio && genePosition == 1)
        //genePosition points to overbought threshold,
        //dipendent from the oversold one such that the portfolio tends to be fix
            returnValue = Convert.ToInt32(   
                              Convert.ToDouble( genome.GetGeneValue(0) ) * 
                              Convert.ToDouble(this.divisorForThresholdComputation) /
                              (  Convert.ToDouble(this.divisorForThresholdComputation) - 
                                 Convert.ToDouble( genome.GetGeneValue(0) )  )
                                          );
      else
           returnValue = GenomeManagement.RandomGenerator.Next(minValueForGene,
                          maxValueForGene + 1);

      return returnValue;
    }
    
    public override int GetNewGeneValue(Genome genome, int genePosition)
    {
      // in this implementation only new gene values pointing to tickers
      // must be different from the others already stored
      int returnValue = this.getNewGeneValue_getGeneValue(genome,genePosition);
   	
      while( this.genePositionPointsToATicker(genePosition) &&
	           GenomeManipulator.IsTickerContainedInGenome(returnValue,
      	                                                 genome,
      	                                                 this.GetGenePositionsPointingToTickers(genome) ) )
      //while in the given position has to be stored
      //a new gene pointing to a ticker and
      //the proposed returnValue points to a ticker
      //already stored in the given genome, at the given genePositionsPointingToTickers
	            returnValue = this.getNewGeneValue_getGeneValue(genome,genePosition);
	    
      return returnValue;
    }

//OLD VERSION
//    public override void Mutate(Genome genome, double mutationRate)
//    {
//      // in this implementation only one gene is mutated
//      int genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
//      int minValueForGene = genome.GetMinValueForGenes(genePositionToBeMutated);
//      int maxValueForGene = genome.GetMaxValueForGenes(genePositionToBeMutated);
//      int newValueForGene = GenomeManagement.RandomGenerator.Next(minValueForGene,
//	        																maxValueForGene +1);
//      
//      if( this.genePositionPointsToATicker(genePositionToBeMutated) )
//      {
//        while(GenomeManipulator.IsTickerContainedInGenome(newValueForGene,
//              genome, this.genePositionsPointingToTickers) )
//          //while the proposed newValueForGene points to a ticker
//          //already stored in the given genome, at the given genePositionsPointingToTickers
//                  newValueForGene = GenomeManagement.RandomGenerator.Next(minValueForGene,
//                                    maxValueForGene +1);
//        GenomeManagement.MutateOneGene(genome, mutationRate,
//          genePositionToBeMutated, newValueForGene);
//      }
//      else if( this.genePositionPointsToAWeight(genePositionToBeMutated) )
//      {
//        double partOfGeneToSubtractOrAdd = 0.25;
//        int geneValue = Math.Abs(genome.GetGeneValue(genePositionToBeMutated));
//        int subtractOrAdd = GenomeManagement.RandomGenerator.Next(2);
//        if(subtractOrAdd == 1)//subtract a part of the gene value from the gene value itself
//          newValueForGene = geneValue - Convert.ToInt32(partOfGeneToSubtractOrAdd*geneValue);
//        else
//          newValueForGene = Math.Min(genome.GetMaxValueForGenes(genePositionToBeMutated),
//              geneValue + Convert.ToInt32(partOfGeneToSubtractOrAdd*geneValue));
//	      
//        GenomeManagement.MutateOneGene(genome, mutationRate,
//                                       genePositionToBeMutated, newValueForGene);
//      }
//      else if ( this.numOfGenesDedicatedToThresholds == 2 &&
//                this.overboughtMoreThanOversoldForFixedPortfolio && 
//                genePositionToBeMutated == 0 )
//      // genePositionToBeMutated points to the oversold threshold and the
//      //the overbought one is determined
//      {
//        if (GenomeManagement.RandomGenerator.Next(0,101) < (int)(mutationRate*100))
//        {
//          genome.SetGeneValue(newValueForGene, genePositionToBeMutated);
//          genome.SetGeneValue(Convert.ToInt32(Convert.ToDouble(newValueForGene) * Convert.ToDouble(this.divisorForThresholdComputation) /
//    					                (Convert.ToDouble(this.divisorForThresholdComputation) - Convert.ToDouble(newValueForGene))), 1);
//        }
//      }
//      else if ( this.numOfGenesDedicatedToThresholds == 2 &&
//                this.overboughtMoreThanOversoldForFixedPortfolio && 
//                genePositionToBeMutated == 1 )
//        // genePositionToBeMutated points to the overbought threshold and, as the
//        //the overbought one is determined, the oversold one depends from the overbought
//      {
//        if (GenomeManagement.RandomGenerator.Next(0,101) < (int)(mutationRate*100))
//        {
//          genome.SetGeneValue(newValueForGene, genePositionToBeMutated);
//          genome.SetGeneValue(Convert.ToInt32(Convert.ToDouble(newValueForGene) * Convert.ToDouble(this.divisorForThresholdComputation) /
//            (Convert.ToDouble(this.divisorForThresholdComputation) + Convert.ToDouble(newValueForGene))), 0);
//        }
//      }
//    }


		private int mutate_MutateOnlyOneWeight_getNewWeight(Genome genome, int genePositionToBeMutated)
    {
      int returnValue;
      double partOfGeneToSubtractOrAdd = 0.25;
      int geneValue = Math.Abs( genome.GetGeneValue(genePositionToBeMutated) );
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
      while( this.genePositionPointsToAWeight(genePositionToBeMutated) == false )
        //while the proposed genePositionToBeMutated doesn't point to a weight
        genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
	      
      int newValueForGene = this.mutate_MutateOnlyOneWeight_getNewWeight(genome, genePositionToBeMutated);	
	    
      GenomeManagement.MutateOneGene(genome,
                          genePositionToBeMutated, newValueForGene);
    }

    private void mutate_MutateAllGenes(Genome genome)
    {
      for(int genePositionToBeMutated = 0;
          genePositionToBeMutated < genome.Genes().Length;
          genePositionToBeMutated ++)
      {
        int newValueForGene = 
        	this.GetNewGeneValue(genome, genePositionToBeMutated);
        genome.SetGeneValue(newValueForGene, genePositionToBeMutated);
      }
      
    }
    
		public override void Mutate(Genome genome)
    {
    	int mutateOnlyOneWeight = GenomeManagement.RandomGenerator.Next(2);
    	if(mutateOnlyOneWeight == 1)
    	 	this.mutate_MutateOnlyOneWeight(genome);
    	else//mutate all genome's genes
    	  this.mutate_MutateAllGenes(genome); 
    }

  }
}
