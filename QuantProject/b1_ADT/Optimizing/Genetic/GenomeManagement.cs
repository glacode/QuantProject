/*
QuantProject - Quantitative Finance Library

GenomeManagement.cs
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
using QuantProject.ADT.Optimizing.Genetic;

namespace QuantProject.ADT.Optimizing.Genetic
{
	/// <summary>
	/// Class providing static methods to manage Genomes
	/// (crossovering, mutating, etc.)
	/// </summary>
	[Serializable]
  public sealed class GenomeManagement 
	{
		
    public static Random RandomGenerator; 
    private static int genomeSize;
		private static Genome[] childs;
		private static int[,] maskForChilds;
    
    static GenomeManagement()
    {
	  		RandomGenerator = new Random((int)DateTime.Now.Ticks);
		    childs = new Genome[2];  	
		}
    
    private static void initializeStaticMembers(Genome parent1, Genome parent2)
    {
      genomeSize = parent1.Size;
      childs[0] = parent1.Clone();
      childs[1] = parent2.Clone();
      //the two childs now points to their parents
      maskForChilds = new int[childs.Length, genomeSize];
    }

    private static void assignFitnessAndMeaningToChilds()
    {
      foreach(Genome child in childs)
      {
        child.AssignMeaning();
        child.CalculateFitness();
      }
    }
  	/// <summary>
		/// Returns an array of genome (length = 2) based 
		/// on classical one point crossover genes recombination
		/// </summary>
  	public static Genome[] OnePointCrossover(Genome parent1, Genome parent2)
		{
			GenomeManagement.initializeStaticMembers(parent1, parent2);
      if(parent1.Size != parent2.Size)
				throw new Exception("Genomes must have the same size!");	
      int pos = GenomeManagement.RandomGenerator.Next(0, parent1.Size);
			for(int i = 0 ; i < parent1.Size ; i++)
			{
				if (i < pos)
				{
					childs[0].SetGeneValue(parent1.GetGeneValue(i), i);
					childs[1].SetGeneValue(parent2.GetGeneValue(i), i);
				}
				else
				{
					childs[0].SetGeneValue(parent2.GetGeneValue(i), i);
					childs[1].SetGeneValue(parent1.GetGeneValue(i), i);
				}
			}
			GenomeManagement.assignFitnessAndMeaningToChilds();
      return GenomeManagement.childs;
		}
		
		private static void setMaskForChildsForUniformCrossover()
		{
			for(int childIndex = 0; childIndex < 2; childIndex++)
      {
        for(int i = 0 ; i < GenomeManagement.genomeSize ; i++)
      	{
  	      maskForChilds[childIndex,i] = GenomeManagement.RandomGenerator.Next(1,3);
       	}
      }
		}
		
		private static void setMaskForChildsForAlternateFixedCrossover()
		{
			for(int childIndex = 0; childIndex < 2; childIndex++)
      {
        for(int genePos = 0; genePos < genomeSize; genePos++)
      	{
  	  		if(genePos%2 == 0)
					//gene position is even
						maskForChilds[childIndex, genePos] = 1;
					else
					// gene position is odd
						maskForChilds[childIndex, genePos] = 2;
	     	}
      }
		}
    
    private static int firstGenePositionOfParent1NotPresentInParent2(Genome parent1,
                                                                Genome parent2,
                                                               int constToDiscoverGenesDuplicates)
    {
      int returnValue = -1;
      for(int genePos = 0 ;
          genePos < GenomeManagement.genomeSize && returnValue == -1;
          genePos++)
      {
      	int geneValue = parent1.GetGeneValue(genePos);
      	if(!parent2.HasGene(geneValue) &&
      	   !parent2.HasGene(geneValue + constToDiscoverGenesDuplicates) &&
      	   !parent2.HasGene(geneValue - constToDiscoverGenesDuplicates))
          returnValue = genePos;
      }    
      return returnValue;
    }
			
    private static bool setMaskForChildsForMixingWithoutDuplicates(Genome parent1, Genome parent2,
		                                                           int constToDiscoverGenesDuplicates)
		{
			bool returnValue = false;
      int firstGenePosOfParent1NotPresentInParent2 = 
        firstGenePositionOfParent1NotPresentInParent2(parent1, parent2, constToDiscoverGenesDuplicates);
      int firstGenePosOfParent2NotPresentInParent1 = 
        firstGenePositionOfParent1NotPresentInParent2(parent2, parent1, constToDiscoverGenesDuplicates);
      if(firstGenePosOfParent1NotPresentInParent2 > -1 &&
         firstGenePosOfParent2NotPresentInParent1 > -1 )
        //there is at least a gene in parent1 not present in parent2 and viceversa
      {
          for(int genePos = 0 ; genePos < GenomeManagement.genomeSize ; genePos++)
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
		
		private static void setChildsUsingMaskForChilds(Genome parent1,
		                                                Genome parent2)
		{
      for(int childIndex = 0; childIndex < 2; childIndex++)
      {
        for(int genePos = 0 ; genePos < GenomeManagement.genomeSize ; genePos++)
      	{
  	      if(maskForChilds[childIndex,genePos]==1)
           	childs[childIndex].SetGeneValue(parent1.GetGeneValue(genePos), genePos);
       		else//maskForChilds[childIndex,genePos]==2
           	childs[childIndex].SetGeneValue(parent2.GetGeneValue(genePos), genePos);
        }
      }
			
		}
		
    /// <summary>
		/// This method returns an array of genomes based on 
		/// a random recombination of the genes of parents
		/// </summary> 
    public static Genome[] UniformCrossover(Genome parent1, Genome parent2)
    {
      initializeStaticMembers(parent1, parent2);
      if(parent1.Size != parent2.Size)
				throw new Exception("Genomes must have the same size!");	
			setMaskForChildsForUniformCrossover();
     	setChildsUsingMaskForChilds(parent1, parent2);
			    
      return childs;
    }

    /// <summary>
		/// This method returns an array of genomes based on 
		/// an alternate fixed recombination of the genes of parents
		/// </summary> 
    public static Genome[] AlternateFixedCrossover(Genome parent1, Genome parent2)
    {
      initializeStaticMembers(parent1, parent2);
      if(parent1.Size != parent2.Size)
				throw new Exception("Genomes must have the same size!");	
      setMaskForChildsForAlternateFixedCrossover();
     	setChildsUsingMaskForChilds(parent1, parent2);
      
      return childs;
    }
		
    private static void launchExIfAChildHasDuplicateGenes(int constToDiscoverGenesDuplicates)
		{
      foreach(Genome gen in childs)
      {
      	if(gen.HasSomeDuplicateGenes(constToDiscoverGenesDuplicates))
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
    /// <param name="constToDiscoverGenesDuplicates">Gene y is a duplicate of gene x iff y = x or y = x + constToDiscoverGenesDuplicates</param>
    public static Genome[] MixGenesWithoutDuplicates(Genome parent1, Genome parent2,
		                                             int constToDiscoverGenesDuplicates)
    {
      initializeStaticMembers(parent1, parent2);
      if(parent1.Size > (parent1.MaxValueForGenes - parent1.MinValueForGenes + 1))
			//it is impossible not to duplicate genes if size is too
			// large for the range of variation of each gene
				throw new Exception("Impossible to avoid duplicates with the given size!");
      if(parent1.Size != parent2.Size)
				throw new Exception("Genomes must have the same size!");	
      
      if(setMaskForChildsForMixingWithoutDuplicates(parent1, parent2,
                                                    constToDiscoverGenesDuplicates))
     	  setChildsUsingMaskForChilds(parent1, parent2);
      launchExIfAChildHasDuplicateGenes(constToDiscoverGenesDuplicates);
      return childs;
    }


		static public void MutateAllGenes(Genome genome, double mutationRate)
		{
			for (int pos = 0 ; pos < genome.Size; pos++)
			{
				if (GenomeManagement.RandomGenerator.Next(0,101) < (int)(mutationRate*100))
					genome.SetGeneValue(GenomeManagement.RandomGenerator.Next(genome.MinValueForGenes,
                                   genome.MaxValueForGenes + 1), pos);
			}
		}
		
		static public void MutateOneGene(Genome genome, double mutationRate,
		                          				int genePosition)
		{
		
			if (GenomeManagement.RandomGenerator.Next(0,101) < (int)(mutationRate*100))
				genome.SetGeneValue(GenomeManagement.RandomGenerator.Next(genome.MinValueForGenes,
                            genome.MaxValueForGenes + 1), genePosition);
			
		}
    
    static public void MutateOneGene(Genome genome, double mutationRate,
                                      int genePosition, int newValueOfGene)
    {
		  if (GenomeManagement.RandomGenerator.Next(0,101) < (int)(mutationRate*100))
              genome.SetGeneValue(newValueOfGene, genePosition);
			
    }
	}
}
