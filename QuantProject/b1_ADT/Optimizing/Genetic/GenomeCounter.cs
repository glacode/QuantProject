/*
QuantProject - Quantitative Finance Library

GenomeCounter.cs
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
	/// This object subscribes GeneticOptimizer's NewGeneration event
	/// in order to count how many genomes are totally tested
	/// Note: two different genomes are supposed to have different
	/// fitness values 
	///   
	/// </summary>
	[Serializable]
	public class GenomeCounter
	{
	  private Hashtable fitnessCollector;
    private GeneticOptimizer geneticOptimizer;
    		
    public int TotalEvaluatedGenomes
    {
      get{return this.fitnessCollector.Count;}
    }
   	public double BestFitness
    {
      get{return this.geneticOptimizer.BestGenome.Fitness;}
    }
    /// <summary>
    /// The instance of GenomeCounter has to be created
    /// before running the given GeneticOptimizer
    /// </summary>
    public GenomeCounter(GeneticOptimizer geneticOptimizer)
		{
    	this.geneticOptimizer = geneticOptimizer;
    	this.fitnessCollector = new Hashtable();
    	this.geneticOptimizer.NewGeneration += 
    		new NewGenerationEventHandler(this.newGenerationEventHandler);
    }
		
    private void newGenerationEventHandler(Object sender,
                          NewGenerationEventArgs newGenerationEventArgs)
    {
    	foreach(Genome genome in this.geneticOptimizer.CurrentGeneration)
    	{
    		if(!this.fitnessCollector.ContainsKey(genome.Fitness))
    		//the hashtable fitnessCollector doesn't contain the
    		//current genome's fitness, yet
    			this.fitnessCollector.Add(genome.Fitness, null);
    	}
    }
    
	
	}
}
