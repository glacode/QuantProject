/*
QuantProject - Quantitative Finance Library

IGenomeManager.cs
Copyright (C) 2004 
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


namespace QuantProject.ADT.Optimizing.Genetic
{
	/// <summary>
	/// Interface to be implemented by any object used to run
	/// an instance of the GeneticOptimizer class.
	/// 
	/// The interface determines the genome format and provides:
	/// - an objective function used to calculate genome's fitness;
	/// - a decode function used to calculate genome's meaning as an object;
	/// - a GetChilds method used by the genetic optimizer to generate new
	///   population;
	/// - a Mutate method used by the genetic optimizer to mutate a 
	///   given genome
	/// </summary>
	public interface IGenomeManager
	{
    int GenomeSize{get;}
    int MinValueForGenes{get;}
    int MaxValueForGenes{get;}
    GeneticOptimizer CurrentGeneticOptimizer{get;set;}
    
    int GetNewGeneValue(Genome genome, int genePosition); // Used in generation of genes
    																    //  by the Genome parameter
    double GetFitnessValue(Genome genome);
    object Decode(Genome genome);
		Genome[] GetChilds(Genome parent1, Genome parent2);
		void Mutate(Genome genome, double mutationRate);
  }
}
