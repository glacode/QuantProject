/*
QuantProject - Quantitative Finance Library

SimplePTGenomeManipulator.cs
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

namespace QuantProject.Scripts.ArbitrageTesting.PairTrading.SimplePairTrading
{
	/// <summary>
	/// Class providing static methods for manipulating Genomes
	/// (used by GenomeManagerForSimplePT)
	/// </summary>
	[Serializable]
  public sealed class SimplePTGenomeManipulator 
	{
		
    public static Random RandomGenerator; 
    private static int genomeSize;
		private static Genome[] childs;
		private static int[,] maskForChilds;
    
    static SimplePTGenomeManipulator()
    {
	  	RandomGenerator = new Random(ConstantsProvider.SeedForRandomGenerator);
		  childs = new Genome[2];  	
		}
    
    private static void initializeStaticMembers(Genome parent1, Genome parent2)
    {
      genomeSize = parent1.Size;
      childs[0]=parent1.Clone();
      childs[1]=parent2.Clone();
      maskForChilds = new int[childs.Length, genomeSize];
      for(int i = 0; i<genomeSize; i++)
      {
      	maskForChilds[0,i]=1;
      	maskForChilds[1,i]=2;
      }
      //maskForChilds has been set in order to re-create
      //a copy of parents by using setChildsUsingMaskForChilds()
    }

    private static void setMaskForChildsForMixingWithoutDuplicates(Genome parent1, Genome parent2)
		                                                           
    {
      //exchange numOfDaysForGap
      maskForChilds[0, 0] = 2;
      maskForChilds[1, 0] = 1;
      //exchange tickers if possible
      if(!IsTickerContainedInGenome(parent1.GetGeneValue(1), parent2))
        maskForChilds[1, 1] = 1;
      if(!IsTickerContainedInGenome(parent2.GetGeneValue(1), parent1))
        maskForChilds[0, 1] = 2;
    }
    
    private static void setChildsUsingMaskForChilds(Genome parent1,
		                                                Genome parent2)
		{
      for(int childIndex = 0; childIndex < 2; childIndex++)
      {
        for(int genePos = 0 ; genePos < SimplePTGenomeManipulator.genomeSize ; genePos++)
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
		
    /// <summary>
		/// Returns true if a given gene, when decoded by the
		/// GenomeManagerForSimplePT, refers to a
		/// ticker already contained in a given genome 
		/// </summary> 
		/// <param name="geneCorrespondingToATicker">Gene, corresponding to a certain ticker, that has to be checked</param>
    /// <param name="genome">Genome containing or not the ticker geneCorrespondingToATicker refers to</param>
    public static bool IsTickerContainedInGenome(int geneCorrespondingToATicker,
                                                 Genome genome)
    {
      return geneCorrespondingToATicker == genome.GetGeneValue(1) ||
              geneCorrespondingToATicker == genome.GetGeneValue(2);
    }

	}
}
