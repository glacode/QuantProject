/*
QuantProject - Quantitative Finance Library

AlternativeGeneticOptimizer.cs
Copyright (C) 2011
Marco Milletti, Glauco Siliprandi

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
	/// GeneticOptimizer with custom generations
	/// </summary>
	[Serializable]
	public class AlternativeGeneticOptimizer : GeneticOptimizer
	{
		#region fields
//		private Random random;
//		
//		private double mutationRate;
//		private double crossoverRate;
//		private double elitismRate;
//		private double minConvergenceRate;
//		private bool keepOnRunningUntilConvergenceIsReached;
//		private int populationSize;
//		private int generationNumber;
//		private int genomeSize;
//		private double totalSpecialFitnessForRouletteSelection;
//		private double totalFitness;
//		private Genome bestGenome;
//		private Genome worstGenome;
//		private IGenomeManager genomeManager;
//		private GenomeComparer genomeComparer;
		
		//    private ArrayList currentGeneration;
//		private ArrayList currentGeneration;
//		private ArrayList currentEliteToTransmitToNextGeneration;
//		private ArrayList nextGeneration;
//		private ArrayList cumulativeSpecialFitnessListForRouletteSelection;
		
//		private int generationCounter;
		//    private double averageRandomFitness;
		//    private double standardDeviationOfRandomFitness;
		
		#endregion
		
		public override int PopulationSize
		{
			get{
				int popSize = this.populationSize;
				if ( this.currentGeneration != null )
					popSize = this.currentGeneration.Count;
				return popSize;
			}
			set{populationSize = value;}
		}

		
		/// <summary>
		/// GeneticOptimizer with custom generations
		/// </summary>
		
		public AlternativeGeneticOptimizer(double crossoverRate, double mutationRate, double elitismRate,
		                                   int populationSize, int generationNumber,
		                                   IGenomeManager genomeManager, int seedForRandomGenerator ,
		                                   ArrayList currentGeneration , ArrayList nextGeneration ) :
			base( crossoverRate, mutationRate, elitismRate,
			     populationSize, generationNumber,
			     genomeManager, seedForRandomGenerator )
		{
			//    	this.commonInitialization(genomeManager, populationSize, generationNumber);
			//    	this.crossoverRate = crossoverRate;
			//    	this.mutationRate = mutationRate;
			//    	this.elitismRate = elitismRate;
			//    	this.random = new Random(seedForRandomGenerator);
			this.currentGeneration = currentGeneration;
			this.nextGeneration = nextGeneration;
		}


		protected override void generateNewPopulation(bool showOutputToConsole)
		{
			this.createNextGeneration();
			this.generationCounter++;
			this.updateBestGenomeFoundInRunning((Genome)this.currentGeneration[
				this.currentGeneration.Count-1]);
			this.updateWorstGenomeFoundInRunning((Genome)this.currentGeneration[0]);
			if (showOutputToConsole)
				this.showOutputToConsole();
		}
		
		/// <summary>
		/// It returns an int corresponding to a certain genome.
		/// The probability for a genome to be selected depends
		/// proportionally on the level of fitness.
		/// </summary>
		protected override int rouletteSelection()
		{
			double randomFitness = this.totalSpecialFitnessForRouletteSelection *(double)this.random.Next(1,1001)/1000;
			int idx = -1;
			int first = 0;
			//      int last = this.populationSize -1;
			int last = this.cumulativeSpecialFitnessListForRouletteSelection.Count -1;
			int mid = (last - first)/2;
			//  Need to implement a specific search, because the
			//  ArrayList's BinarySearch is for exact values only
			while (idx == -1 && first <= last)
			{
				if (randomFitness < (double)this.cumulativeSpecialFitnessListForRouletteSelection[mid])
				{
					last = mid;
				}
				else if (randomFitness >= (double)this.cumulativeSpecialFitnessListForRouletteSelection[mid])
				{
					first = mid;
				}
				mid = (first + last)/2;
				if ((last - first) == 1)
					idx = last;//it's time to exit from the while loop
			}
			return idx;
		}


		protected override void updateCumulativeSpecialFitnessListForRouletteSelection()
		{
			double cumulativeSpecialFitness = 0.0;
			this.cumulativeSpecialFitnessListForRouletteSelection.Clear();
			for (int i = 0; i < this.currentGeneration.Count ; i++)
			{
				cumulativeSpecialFitness +=
					((Genome)this.currentGeneration[i]).Fitness +
					Math.Abs(((Genome)this.currentGeneration[0]).Fitness);
				this.cumulativeSpecialFitnessListForRouletteSelection.Add(cumulativeSpecialFitness);
			}
		}

		protected override void createNextGeneration_transmitEliteToNextGeneration()
		{
			this.currentEliteToTransmitToNextGeneration.Clear();
			
			for(int i = this.currentGeneration.Count - 1;
			    i >=(this.currentGeneration.Count - this.elitismRate*this.populationSize - 1);
			    i--)
			{
				if(this.currentGeneration[i] is Genome)
					this.currentEliteToTransmitToNextGeneration.Add((Genome)this.currentGeneration[i]);
			}
			
			for(int i = 0;
			    i < this.currentEliteToTransmitToNextGeneration.Count;
			    i++)
				
			{
//				if ( this.nextGeneration.CanBeAdded( (Genome)this.currentEliteToTransmitToNextGeneration[i] ) )
					this.nextGeneration.Add((Genome)this.currentEliteToTransmitToNextGeneration[i]);
			}
		}

		protected override void updateCurrentGeneration()
		{
			this.nextGeneration.Sort(this.genomeComparer);
			this.currentGeneration.Clear();
			int numOfNextGeneration = this.nextGeneration.Count;
			// Note that next generation is greater than current:
			// due to the population size, genomes with lowest fitness are abandoned
			for (int i = 1 ; i <= this.nextGeneration.Count; i++)
//				if ( this.currentGeneration.CanBeAdded( (Genome)this.nextGeneration[numOfNextGeneration - i] ) )
				this.currentGeneration.Add(this.nextGeneration[numOfNextGeneration - i]);
		}
		
		protected override void setInitialBestAndWorstGenomes()
		{
			this.bestGenome = ((Genome)this.currentGeneration[this.currentGeneration.Count-1]).Clone();
			this.worstGenome = ((Genome)this.currentGeneration[0]).Clone();
		}

	}
}
