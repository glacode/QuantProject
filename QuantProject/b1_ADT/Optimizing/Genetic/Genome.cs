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
    private int size;
    private double fitness;
    private object meaning;
    private bool hasBeenCloned;
    private bool hasBeenChanged;
    
    private IGenomeManager genomeManager;
    private GeneticOptimizer geneticOptimizer;
		private int generation;
		private bool hasFitnessBeenAssigned;
    
    public bool HasBeenCloned
    {
      get{return this.hasBeenCloned;}
      //set{this.hasBeenCloned = value;}
    }
    
    /// <summary>
    /// Returns true if a gene has been set
    /// by calling SetGeneValue
    /// </summary>
    public bool HasBeenChanged
    {
      get{return this.hasBeenChanged;}
    }
    
    private void setFitnessValue(double fitnessValue)
    {
      this.fitness = fitnessValue;
      this.hasFitnessBeenAssigned = true;
    }
    
    
    public double Fitness
    {
      get
			{
				if ( double.IsNaN( this.fitness ) )
					throw new Exception(
						"The fitness for this genome is not a number!" );
				if ( ! this.hasFitnessBeenAssigned )
				// the genome's fitness has not been assigned yet
					this.setFitnessValue(this.genomeManager.GetFitnessValue(this));
				
				return this.fitness;
			}
    }

    public object Meaning
    {
			get
			{
				if ( this.meaning == null )
				// the genome's meaning has not been assigned yet
					this.meaning = this.genomeManager.Decode( this );
				return this.meaning;
			}
    }

    public int Size
    {
      get{return this.size;}
    }
    
    public int GetMinValueForGenes(int genePosition)
    {
      return this.genomeManager.GetMinValueForGenes(genePosition);
    }
		
    public int GetMaxValueForGenes(int genePosition)
    {
      return this.genomeManager.GetMaxValueForGenes(genePosition);
    }
		
    /// <summary>
    /// Returns the generation at which 
    /// the current genome has been generated
    /// by the Genetic Optimizer
    /// </summary>
    public int Generation
    {
      get{return this.generation;}
      //set{this.generation = value;}
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
    public Genome(IGenomeManager genomeManager,
                  GeneticOptimizer geneticOptimizer)
		{
			this.genomeManager = genomeManager;
			this.geneticOptimizer = geneticOptimizer;
			this.size = this.genomeManager.GenomeSize;
 			this.genes = new int[ this.size ];
		}
  
  	public void AssignMeaning()
    {
      this.meaning = this.genomeManager.Decode(this);
    }	 

		public void CreateGenes()
		{
			for (int i = 0 ; i < this.size ; i++)
				this.genes[i] = this.genomeManager.GetNewGeneValue(this,i);
			//whenever at least one gene has been written,
			//the current generation number is stored
			this.generation = this.geneticOptimizer.GenerationCounter;
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
      Genome returnValue = new Genome(this.genomeManager, this.geneticOptimizer);
      returnValue.CopyValuesInGenes(this.genes);
      returnValue.setFitnessValue(this.fitness);
      returnValue.meaning = this.meaning;
      returnValue.generation = this.Generation;
      returnValue.hasBeenCloned = true;
      
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
			//whenever at least one gene has been written,
			//the current generation number is stored
			this.generation = this.geneticOptimizer.GenerationCounter;
		}
    
    public void SetGeneValue(int geneValue, int genePosition)
    {
      if(geneValue < this.GetMinValueForGenes(genePosition) ||
         geneValue > this.GetMaxValueForGenes(genePosition) )
      	throw new IndexOutOfRangeException("Gene value not valid for the gene at" +
      	                                   " the given position!");
      
      this.genes[genePosition] = geneValue;
      //whenever at least one gene has been written,
			//the current generation number is stored
      this.generation = this.geneticOptimizer.GenerationCounter;
      this.hasBeenChanged = true;
    }
    
    public int GetGeneValue(int genePosition)
    {
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
    /// It returns true if the given gene is already stored in the current genome
    /// </summary>
    /// <param name="fromGenePosition">First gene position from which the checking
    /// 		has to be done, inside the given genome</param>
    /// <param name="toGenePosition">Last gene position to which the checking
    /// 		has to be done, inside the given genome</param>
    public bool HasGene(int geneValue, int fromGenePosition,
                        int toGenePosition)
    {
      if(fromGenePosition < 0 ||
    	   toGenePosition < 0 ||
    	   fromGenePosition >= this.size ||
    	   toGenePosition >= this.size)
    		throw new IndexOutOfRangeException("error in parameters fromGenePosition or toGenePosition!");
    		                                   
    	bool returnValue = false;
      for(int i = fromGenePosition; i <= toGenePosition; i++)
      {
      	if( geneValue == this.Genes()[i] )
              returnValue = true;
      }
      return returnValue;
    }

    /// <summary>
    /// It returns true if the given gene is already stored in the current genome
    /// at the given genePosition
    /// </summary>
    /// <param name="genePosition">Gene position where the check
    /// 		has to be done, inside the given genome</param>
    public bool HasGene(int geneValue, int genePosition)
    {
      if(genePosition >= this.size)
    		throw new IndexOutOfRangeException("bad genePosition parameter!");
    		                                   
    	return geneValue == this.Genes()[genePosition];
    }
    
    /// <summary>
    /// It returns true if the current instance shares no gene with the given
    /// genome
    /// </summary>
    public bool SharesNoGeneWith(Genome genomeToBeCompared)
    {
      bool returnValue = true;
      foreach(int gene in this.Genes())
      {
        if( genomeToBeCompared.HasGene(gene) )
          return false;
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
