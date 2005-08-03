/*
QuantProject - Quantitative Finance Library

GenomeManipulator.cs
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
using System.Collections;
using QuantProject.ADT;
using QuantProject.ADT.Optimizing.Genetic;

namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	/// <summary>
	/// Class providing static methods for manipulating Genomes
	/// (used by th IGenomeManagerForEfficientPortfolio)
	/// </summary>
	[Serializable]
  public sealed class GenomeManipulator 
	{
		
    public static Random RandomGenerator; 
    private static int genomeSize;
		private static Genome[] childs;
		private static int[,] maskForChilds;
    
    static GenomeManipulator()
    {
	  	RandomGenerator = new Random(ConstantsProvider.SeedForRandomGenerator);
		  childs = new Genome[2];  	
		}
    
    private static void initializeStaticMembers(Genome parent1, Genome parent2)
    {
      genomeSize = parent1.Size;
      childs[0] = parent1.Clone();
      childs[1] = parent2.Clone();
      //the two childs now points to their parents,
      //and so do maskForChilds
      maskForChilds = new int[childs.Length, genomeSize];
      for(int i = 0; i<genomeSize; i++)
      {
      	maskForChilds[0,i]=1;
      	maskForChilds[1,i]=2;
      }
    }

    private static void assignFitnessAndMeaningToChilds()
    {
      foreach(Genome child in childs)
      {
        child.AssignMeaning();
        child.CalculateFitness();
      }
    }

    /*old
    private static int firstGenePositionOfParent1NotPresentInParent2(Genome parent1,
                                                                    Genome parent2)
    {
      int returnValue = -1;
      for(int genePos = 0 ;
          genePos < GenomeManipulator.genomeSize && returnValue == -1;
          genePos++)
      {
      	int geneValue = parent1.GetGeneValue(genePos);
      	if(geneValue >= 0)
        {
          if(!parent2.HasGene(geneValue) &&
             !parent2.HasGene(-Math.Abs(geneValue) - 1))
               returnValue = genePos;
        }
        else
        {
          if(!parent2.HasGene(geneValue) &&
             !parent2.HasGene(Math.Abs(geneValue) - 1))
               returnValue = genePos;
        }
      }    
      return returnValue;
    }
    */
		/* old	
    private static bool setMaskForChildsForMixingWithoutDuplicates(Genome parent1, Genome parent2)
		                                                           
		{
			bool returnValue = false;
      int firstGenePosOfParent1NotPresentInParent2 = 
        firstGenePositionOfParent1NotPresentInParent2(parent1, parent2);
      int firstGenePosOfParent2NotPresentInParent1 = 
        firstGenePositionOfParent1NotPresentInParent2(parent2, parent1);
      if(firstGenePosOfParent1NotPresentInParent2 > -1 &&
         firstGenePosOfParent2NotPresentInParent1 > -1 )
        //there is at least a gene in parent1 not present in parent2 and viceversa
      {
          for(int genePos = 0 ; genePos < GenomeManipulator.genomeSize ; genePos++)
          {
            if(genePos == firstGenePosOfParent1NotPresentInParent2)
                maskForChilds[0, genePos] = 1;
            else
                maskForChilds[0, genePos] = 2;
            
            if(genePos == firstGenePosOfParent2NotPresentInParent1)
              maskForChilds[1, genePos] = 2;
            else
              maskForChilds[1, genePos] = 1;
          }
          returnValue = true;
      }
      return returnValue;
		}
    */
    private static int[] genePositionsOfParent1NotPresentInParent2(Genome parent1,
                                                                   Genome parent2)
    {
      int[] returnValue = new int[parent1.Size];
      for(int i = 0; i < returnValue.Length; i++)
      {
        returnValue[i] = - 1;
        int geneValue = parent1.GetGeneValue(i);
        if(geneValue >= 0)
        {
          if(!parent2.HasGene(geneValue) &&
            !parent2.HasGene(-Math.Abs(geneValue) - 1))
            returnValue[i] = i;
        }
        else
        {
          if(!parent2.HasGene(geneValue) &&
            !parent2.HasGene(Math.Abs(geneValue) - 1))
            returnValue[i] = i;
        }
      }
      return returnValue;
    }
    
    private static void setMaskForChildsForMixingWithoutDuplicates(Genome parent1, Genome parent2)
		                                                           
    {
      int[] genePlacesOfParent1NotPresentInParent2 = 
        genePositionsOfParent1NotPresentInParent2(parent1, parent2);
      int[] genePlacesOfParent2NotPresentInParent1 = 
        genePositionsOfParent1NotPresentInParent2(parent2, parent1);
      for(int i = 0;i<parent1.Size;i++)
      {
        if(i%2 == 0)
        //genes are ex-changed only at even positions
        {  
        	if(genePlacesOfParent2NotPresentInParent1[i]!= - 1)
        	{
        		maskForChilds[0, i] = 2;//the change between genes
        		maskForChilds[1, i] = 1;//creates a child different from parents
        	}
        	else if(genePlacesOfParent1NotPresentInParent2[i]!= - 1) //see 1st if
        	{
        		maskForChilds[0, i] = 1;
        		maskForChilds[1, i] = 2;
        	}
        }
      }
    }
		
    private static void setChildsUsingMaskForChilds(Genome parent1,
		                                                Genome parent2)
		{
      for(int childIndex = 0; childIndex < 2; childIndex++)
      {
        for(int genePos = 0 ; genePos < GenomeManipulator.genomeSize ; genePos++)
      	{
  	      if(maskForChilds[childIndex,genePos]==1)
           	childs[childIndex].SetGeneValue(parent1.GetGeneValue(genePos), genePos);
       		else//maskForChilds[childIndex,genePos]==2
           	childs[childIndex].SetGeneValue(parent2.GetGeneValue(genePos), genePos);
        }
      }
			
		}
		
    private static bool hasSomeDuplicateGenes(Genome genome)
    {
      bool returnValue = false;
      for(int i = 0; i < genome.Size ; i++)
      {
        for(int j = i + 1; j < genome.Size ; j++)
        {
          if(genome.Genes()[i] == genome.Genes()[j] ||
            ((genome.Genes()[i]<0 && genome.Genes()[j]>0) && Math.Abs(genome.Genes()[i]) - 1 == genome.Genes()[j]) ||
            ((genome.Genes()[i]>0 && genome.Genes()[j]<0) && -Math.Abs(genome.Genes()[i]) - 1 == genome.Genes()[j]))
            returnValue = true;
        }
      }
      return returnValue;
    }
    //just for debugging purposes
    private static void throwExcIfAChildHasDuplicateGenes()
		{
      foreach(Genome gen in childs)
      {
      	if(GenomeManipulator.hasSomeDuplicateGenes(gen))
      		throw new Exception("A child with duplicate genes has been generated!");
      }
			
		}
		/// <summary>
		/// This method returns an array of genomes based on 
		/// a mix of the genes of parents, such that the 2 childs,
		/// if possible, are different from parents and, at 
		/// the same time, childs' genes are not duplicated  
		/// </summary> 
		/// <param name="parent1">First genome parent from which genes are to be mixed in offspring</param>
    /// <param name="parents">Second genome parent from which genes are to be mixed in offspring</param>
    /// <param name="constToDiscoverGenesDuplicates">Gene y is a duplicate of gene x iff y = |x| - 1
    ///  or y = -|x| -1</param>
    public static Genome[] MixGenesWithoutDuplicates(Genome parent1, Genome parent2)
    {
      initializeStaticMembers(parent1, parent2);
      if(parent1.Size > (parent1.MaxValueForGenes - parent1.MinValueForGenes + 1))
			//it is impossible not to duplicate genes if size is too
			// large for the range of variation of each gene
				throw new Exception("Impossible to avoid duplicates with the given size!");
      if(parent1.Size != parent2.Size)
				throw new Exception("Genomes must have the same size!");	
      
      setMaskForChildsForMixingWithoutDuplicates(parent1, parent2);
      setChildsUsingMaskForChilds(parent1, parent2);
      //throwExcIfAChildHasDuplicateGenes(); //just for debugging purposes
      return childs;
    }

	}
}
