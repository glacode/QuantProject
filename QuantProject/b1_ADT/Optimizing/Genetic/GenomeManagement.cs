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
  public sealed class GenomeManagement 
	{
		
    public static Random RandomGenerator = new Random((int)DateTime.Now.Ticks);
    private static int genomeSize;
		private static Genome[] childs;
		private static int[,] maskForChilds;
    /*
    public GenomeManagement()
    {
	  				    	
		}
		*/
  	/// <summary>
		/// Returns an array of genome (childs) based 
		/// on classical one point crossover genes recombination
		/// </summary>
  	public static Genome[] OnePointCrossover(Genome parent1, Genome parent2)
		{
			if(parent1.Size != parent2.Size)
				throw new Exception("Genomes must have the same size!");	
			GenomeManagement.genomeSize = parent1.Size;
		  GenomeManagement.childs = new Genome[2]{parent1.Clone(),parent1.Clone()};
			
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
			return GenomeManagement.childs;
		}
		
		private static void setMaskForChildsForUniformCrossover()
		{
			for(int childIndex = 0 ;
					childIndex < GenomeManagement.childs.Length;
					childIndex++)
      {
        for(int i = 0 ; i < GenomeManagement.genomeSize ; i++)
      	{
  	      maskForChilds[childIndex,i] = GenomeManagement.RandomGenerator.Next(1,3);
       	}
      }
		}
		
		private static void setMaskForChildsForAlternateFixedCrossover()
		{
			for(int childIndex = 0 ;
					childIndex < GenomeManagement.childs.Length;
					childIndex++)
      {
        for(int i = 0 ; i < GenomeManagement.genomeSize ; i++)
      	{
  	  		if(i%2 == 0)
					//index is even
						maskForChilds[childIndex, i] = 1;
					else
					// index is odd
						maskForChilds[childIndex, i] = 2;
	     	}
      }
		}
		
		private static void setMaskForChildsForMixingWithoutDuplicates(Genome parent1, Genome parent2)
		{
			for(int childIndex = 0 ;
					childIndex < GenomeManagement.childs.Length;
					childIndex++)
      {
        for(int i = 0 ; i < GenomeManagement.genomeSize ; i++)
      	{
   	      if(parent2.HasGene(parent1.GetGeneValue(i)) ||
   	         parent1.HasGene(parent2.GetGeneValue(i)))//index contains a common gene
  	      {
						maskForChilds[childIndex, i] = childIndex + 1;
  	      }
					else// index doesn't contain a common gene
					{
  	      	if(i%2 == 0)//index is even
							maskForChilds[childIndex, i] = childIndex%2 + 1;
						else// index is odd
							maskForChilds[childIndex, i] = childIndex%2 + 2;
					}
       	}
      }
		}
		
		//it assumes just two parents only
		private static void setChildsUsingMaskForChilds(Genome parent1,
		                                                Genome parent2)
		{
			for(int childIndex = 0 ;
					childIndex < GenomeManagement.childs.Length;
					childIndex++)
      {
        for(int i = 0 ; i < GenomeManagement.genomeSize ; i++)
      	{
  	      if(maskForChilds[childIndex,i]==1)
           	childs[childIndex].SetGeneValue(parent1.GetGeneValue(i), i);
       		else//maskForChilds[childIndex,i]==2
           	childs[childIndex].SetGeneValue(parent2.GetGeneValue(i), i);
        }
      }
			
		}
		
    /// <summary>
		/// This method returns an array of genomes based on 
		/// a random recombination of the genes of parents
		/// </summary> 
    public static Genome[] UniformCrossover(Genome parent1, Genome parent2)
    {
      if(parent1.Size != parent2.Size)
				throw new Exception("Genomes must have the same size!");	
      GenomeManagement.genomeSize = parent1.Size;
      GenomeManagement.childs = 
      						new Genome[2]{parent1.Clone(),parent1.Clone()};
      GenomeManagement.maskForChilds = new int[childs.Length, GenomeManagement.genomeSize];
			GenomeManagement.setMaskForChildsForUniformCrossover();
     	GenomeManagement.setChildsUsingMaskForChilds(parent1, parent2);
			    
      return GenomeManagement.childs;
    }

    /// <summary>
		/// This method returns an array of genomes based on 
		/// an alternate fixed recombination of the genes of parents
		/// </summary> 
    public static Genome[] AlternateFixedCrossover(Genome parent1, Genome parent2)
    {
      if(parent1.Size != parent2.Size)
				throw new Exception("Genomes must have the same size!");	
      GenomeManagement.genomeSize = parent1.Size;
      GenomeManagement.childs = 
      						new Genome[2]{parent1.Clone(),parent1.Clone()};
      GenomeManagement.maskForChilds = new int[childs.Length, GenomeManagement.genomeSize];
      GenomeManagement.setMaskForChildsForAlternateFixedCrossover();
     	GenomeManagement.setChildsUsingMaskForChilds(parent1, parent2);
      
      return GenomeManagement.childs;
    }

		/// <summary>
		/// This method returns an array of genomes based on 
		/// a mix of the genes of parents, such that childs,
		/// if possible, are different from parents and, at 
		/// the same time, childs' genes are not duplicated  
		/// </summary> 
    public static Genome[] MixGenesWithoutDuplicates(Genome parent1, Genome parent2)
    {
      if(parent1.Size > (parent1.MaxValueForGenes - parent1.MinValueForGenes + 1))
			//it is impossible not to duplicate genes if size is too
			// large for the range of variation of each gene
				throw new Exception("Impossible to avoid duplicates with the given size!");
      if(parent1.Size != parent2.Size)
				throw new Exception("Genomes must have the same size!");	
      GenomeManagement.genomeSize = parent1.Size;
      GenomeManagement.childs = 
      						new Genome[2]{parent1.Clone(),parent1.Clone()};
      GenomeManagement.maskForChilds = new int[childs.Length, GenomeManagement.genomeSize];
      GenomeManagement.setMaskForChildsForMixingWithoutDuplicates(parent1, parent2);
     	GenomeManagement.setChildsUsingMaskForChilds(parent1, parent2);
      
      return GenomeManagement.childs;
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
