/*
QuantProject - Quantitative Finance Library

Genome.cs
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
	/// Representation of an individual through features
	/// provided by biology
	/// </summary>
	[Serializable]
	public class Genome : IComparable
	{
	  private int[] genes;
    private int minValueForGenes;
    private int maxValueForGenes;
    private int size;
    private double fitness;
    private object meaning;

    IGenomeManager genomeManager;
		
    public double Fitness
    {
      get{return this.fitness;}
      set{this.fitness = value;}
    }

    public object Meaning
    {
      get{return this.meaning;}
      set{this.meaning = value;}
    }

    public int Size
    {
      get{return this.size;}
    }
    
    public int MinValueForGenes
    {
      get{return this.minValueForGenes;}
    }
    
    public int MaxValueForGenes
    {
      get{return this.maxValueForGenes;}
    }
		
    //implementation of IComparable interface
    public int CompareTo(object obj) {
        if(obj is Genome) {
            Genome genome = (Genome)obj;

            return this.Fitness.CompareTo(genome.Fitness);
        }
        
        throw new ArgumentException("Object is not a Genome");    
    }
		//end of implementation of IComparable interface
    
    /// <summary>
    /// It creates a  new genome object initialized by a IGenomeManager
    /// </summary>
    public Genome(IGenomeManager genomeManager)
		{
			this.genomeManager = genomeManager;
			this.size = this.genomeManager.GenomeSize;
    	this.minValueForGenes = this.genomeManager.MinValueForGenes;
    	this.maxValueForGenes = this.genomeManager.MaxValueForGenes;
 			this.genes = new int[ this.size ];
		}
  
  	public void AssignMeaning()
    {
      this.meaning = this.genomeManager.Decode(this);
    }	 

		public void CreateGenes()
		{
			for (int i = 0 ; i < this.size ; i++)
				this.genes[i] = this.genomeManager.GetNewGeneValue(this);
		}
    
    public void CalculateFitness()
    {
      this.fitness = this.genomeManager.GetFitnessValue(this);
    }
    
    
    /// <summary>
    /// It creates an new genome, identical to the current instance
    /// </summary>
    public Genome Clone()
    {
      Genome returnValue = new Genome(this.genomeManager);
      returnValue.CopyValuesInGenes(this.genes);
      returnValue.Fitness = this.Fitness;
      returnValue.Meaning = this.Meaning;
      
      return returnValue;
    }
            
    public int[] Genes()
		{
			return this.genes;
		}

		public void CopyValuesInGenes(int[] valuesToBeCopied)
		{
			for (int i = 0 ; i < this.size ; i++)
				this.genes[i] = valuesToBeCopied[i];
		}
    
    public void SetGeneValue(int geneValue, int genePosition)
    {
      if(genePosition >= this.size || genePosition<0)
        throw new IndexOutOfRangeException("Gene position not valid for the genome! ");
      
      this.genes[genePosition] = geneValue;
    }
    
    public int GetGeneValue(int genePosition)
    {
      if(genePosition >= this.size || genePosition<0)
        throw new IndexOutOfRangeException("Gene position not valid for the genome! ");
      
      return this.genes[genePosition];
    }
    
    /// <summary>
    /// It returns true if the given gene is already stored in the current genome
    /// </summary>
    public bool HasGene(int geneValue)
    {
      bool returnValue = false;
      foreach(int gene in this.Genes())
      {
        if( geneValue == gene )
              returnValue = true;
      }
      return returnValue;
    }

    /// <summary>
	  /// It returns true if the current instance of genome has some duplicate
	  /// values in genes
	  /// </summary>
    public bool HasSomeDuplicateGenes()
    {
      bool returnValue = false;
      for(int i = 0; i < this.size ; i++)
      {
        for(int j = i + 1; j < this.size ; j++)
        {
          if(this.genes[i] == this.genes[j])            
        		returnValue = true;
        }
      }
      return returnValue;
    }
    
	}
}
