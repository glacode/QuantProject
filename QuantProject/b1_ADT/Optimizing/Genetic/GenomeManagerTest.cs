/*
QuantProject - Quantitative Finance Library

GenomeManagerTest.cs
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

namespace QuantProject.ADT.Optimizing.Genetic
{
	/// <summary>
	/// Create a simple class implementing IGenomeManager, in order to test the
	/// behaviour of the genetic optimizer
	/// </summary>
	[Serializable]
  public class GenomeManagerTest : IGenomeManager
  {
    private int genomeSize;
    private int minValueForGenes;
    private int maxValueForGenes;
         
    public int GenomeSize
    {
      get{return this.genomeSize;}
    }
    
            
    public GenomeManagerTest(int genomeSize, int minValueForGenes,
                             int maxValueForGenes)
                          
    {
      this.genomeSize = genomeSize;
      this.minValueForGenes = minValueForGenes;
      this.maxValueForGenes = maxValueForGenes;
    }
    
    public int GetMinValueForGenes(int genePosition)
    {
      return this.minValueForGenes;
    }
    
    public int GetMaxValueForGenes(int genePosition)
    {
      return this.maxValueForGenes;
    }
    
    public int GetNewGeneValue(Genome genome, int genePosition)
    {
      return GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
                                   genome.GetMaxValueForGenes(genePosition) + 1);
    }

 		public Genome[] GetChilds(Genome parent1, Genome parent2)
    {
      //return GenomeManagement.UniformCrossover(parent1, parent2);
			//return GenomeManagement.AlternateFixedCrossover(parent1, parent2);
    	return GenomeManagement.OnePointCrossover(parent1, parent2);
      //return GenomeManagement.MixGenesWithoutDuplicates(parent1, parent2);
    }
		
		public void Mutate(Genome genome)
    {
      GenomeManagement.MutateAllGenes(genome);
    }
    
    public double GetFitnessValue(Genome genome)
    {
      int[] intArray = genome.Genes();
      double returnValue;
      //function to be maximized
      double sum = 0;
      double product = 1;
      for (int i = 0; i < genome.Size; i++)
      {
        sum += (int)intArray[i];
        product *= (int)intArray[i];
      }
      returnValue = (double)(1/product);// fitness = 1 iff product is minimized
      //returnValue = (double)(product/sum);
      return returnValue;
    }
    
    public object Decode(Genome genome)
    {
      string sequenceOfGenes = "";
      foreach(int index in genome.Genes())
      {
        sequenceOfGenes += index + ";" ;
      }
      return (object)sequenceOfGenes;
    }

  }

}
