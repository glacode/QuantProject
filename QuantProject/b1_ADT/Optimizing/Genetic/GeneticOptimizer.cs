/*
QuantProject - Quantitative Finance Library

GeneticOptimizer.cs
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
  /// The class needs to be initialized by an object implementing
  /// IGenomeManager interface 
  /// Default GO parameters: crossoverRate = 0.85, mutationRate = 0.02, elitismRate = 0.01,
  /// populationSize = 1000, generationNumber = 100
  /// keepOnRunningUntilConvergenceIsReached = false, minConvergenceRate = 0.80 
  /// </summary>
	public class GeneticOptimizer
	{
    #region fields  
    
    private double mutationRate = 0.02;
    private double crossoverRate = 0.85;
    private double elitismRate = 0.01;
    private double minConvergenceRate = 0.80;
    private bool keepOnRunningUntilConvergenceIsReached = false;
    private int populationSize = 1000;
    private int generationNumber = 100;
    private int genomeSize;
    private int minValueForGenes;
    private int maxValueForGenes;
    private double totalFitness;
    private Genome bestGenome;
    private Genome worstGenome;
    private IGenomeManager genomeManager;
    private GenomeComparer genomeComparer;
	
    private ArrayList currentGeneration;
    private ArrayList currentEliteToTransmitToNextGeneration;
    private ArrayList nextGeneration;
    private ArrayList cumulativeFitnessList;
	
    private static Random random = new Random((int)DateTime.Now.Ticks);
    private int generationCounter;
    #endregion
    
    #region properties
    public double MutationRate
    {
      get{return mutationRate;}
      set{mutationRate = value;}
    }
    public double CrossoverRate
    {
      get{return crossoverRate;}
      set{crossoverRate = value;}
    }
    
    public double ElitismRate
    {
      get{return this.elitismRate;}
      set{this.elitismRate = value;}
    }
    
    public double MinConvergenceRate
    {
      get{return this.minConvergenceRate;}
      set{this.minConvergenceRate = value;}
    }
    
    public bool KeepOnRunningUntilConvergenceIsReached
    {
      get{return this.keepOnRunningUntilConvergenceIsReached ;}
      set{this.keepOnRunningUntilConvergenceIsReached = value;}
    }

    public int PopulationSize
    {
      get{return populationSize;}
      set{populationSize = value;}
    }
    
    public int GenerationNumber
    {
      get{return generationNumber;}
      set{generationNumber = value;}
    }
		public int GenerationCounter
    {
      get{return this.generationCounter;}
    }

    public int GenomeSize
    {
      get{return genomeSize;}
    }
    
    public Genome BestGenome
    {
      get{return this.bestGenome;}
    }
    
    public Genome WorstGenome
    {
      get{return this.worstGenome;}
    }
    #endregion
    
    /// <summary>
    /// The class needs to be initialized by an object implementing
    /// IGenomeManager interface 
    /// Default GO parameters: crossoverRate = 0.85, mutationRate = 0.02, elitismRate = 0.01,
    /// populationSize = 1000, generationNumber = 100
    /// keepOnRunningUntilConvergenceIsReached = false, minConvergenceRate = 0.80 
    /// </summary>
    public GeneticOptimizer(IGenomeManager genomeManager)
    {
      this.genomeManager = genomeManager;
      
      this.commonInitialization();
    }
    
    
    public GeneticOptimizer(double crossoverRate, double mutationRate, double elitismRate, 
                            int populationSize, int generationNumber,
                            IGenomeManager genomeManager)
    {
      this.crossoverRate = crossoverRate;
      this.mutationRate = mutationRate;
      this.elitismRate = elitismRate;
      this.populationSize = populationSize;
      this.generationNumber = generationNumber;
      this.genomeManager = genomeManager;
      
      this.commonInitialization();
    }

    private void commonInitialization()
    {
      this.genomeSize = this.genomeManager.GenomeSize;
      this.minValueForGenes = this.genomeManager.MinValueForGenes;
      this.maxValueForGenes = this.genomeManager.MaxValueForGenes;
      this.genomeComparer = new GenomeComparer();
      this.cumulativeFitnessList = new ArrayList(this.PopulationSize);
      this.currentGeneration = new ArrayList(this.PopulationSize);
      this.nextGeneration = new ArrayList();
      this.currentEliteToTransmitToNextGeneration =
        new ArrayList((int)(this.ElitismRate*(double)this.PopulationSize));
      
      this.generationCounter = 1;
    }
    
    /// <summary>
    /// Method which starts the GeneticOptmizer
    /// </summary>
    public void Run(bool showOutputToConsole)
    {
      this.createFirstGeneration(showOutputToConsole);
      if(this.keepOnRunningUntilConvergenceIsReached)
      //The GO keeps on generating new population until convergence is reached
      {
        for (int i = 0; !this.IsConvergenceReached(); i++)
        {
        	this.generateNewPopulation(true);
        }
      }
      else // the GO simply generates the given number of populations and then stops
      {
        for (int i = 0; i < this.generationNumber; i++)
        {
          this.generateNewPopulation(true);
        }
      }
    }
    
    private void generateNewPopulation(bool showOutputToConsole)
    {
    	this.createNextGeneration();
      this.updateBestGenomeFoundInRunning((Genome)this.currentGeneration[populationSize-1]);
      this.updateWorstGenomeFoundInRunning((Genome)this.currentGeneration[0]);
      if (showOutputToConsole)
           this.showOutputToConsole();
    }
    
    private bool IsConvergenceReached()
    {
      bool returnValue = false;
      double averageFitnessOfCurrentGeneration = this.totalFitness / this.populationSize;
      double bestFitnessReachedUntilNow = this.BestGenome.Fitness;

      if (averageFitnessOfCurrentGeneration >= 
                            this.minConvergenceRate * bestFitnessReachedUntilNow )
          returnValue = true;
      
      return returnValue;
 
    }

    private void createFirstGeneration(bool showOutputToConsole)
    {
      if (this.genomeSize == 0 || this.genomeSize < 0)
        throw new IndexOutOfRangeException("Genome size not set");
      this.createGenomes();
      this.currentGeneration.Sort(this.genomeComparer);
      this.calculateTotalFitness();
      this.updateCumulativeFitnessList();
      this.setInitialBestAndWorstGenomes();
      
      if (showOutputToConsole)
        this.showOutputToConsole();
    }
    
    private void showOutputToConsole()
    {
      System.Console.WriteLine("\n\n\n\n*_*_*_*_*_*_*_*_*_*_*\n\nGeneration " + this.generationCounter +"\n");
      for(int i = 0; i < this.populationSize; i++)
      {
        System.Console.WriteLine((string)((Genome)this.currentGeneration[i]).Meaning +
          "--> " + ((Genome)this.currentGeneration[i]).Fitness);
      }
      //Console.WriteLine("Press enter key to continue ...");
			//Console.ReadLine();
    }

    /// <summary>
    /// It returns an int corresponding to a certain genome.
    /// The probability for a genome to be selected depends
    /// proportionally on the level of fitness.
    /// </summary>
    private int rouletteSelection()
    {
      double randomFitness = this.totalFitness *(double)GeneticOptimizer.random.Next(1,1001)/1000;
      int idx = -1;
      int mid;
      int first = 0;
      int last = this.populationSize -1;
      mid = (last - first)/2;

      //  Need to implement a specific search, because the
      //  ArrayList's BinarySearch is for exact values only
      
      while (idx == -1 && first <= last)
      {
        if (randomFitness < (double)this.cumulativeFitnessList[mid])
        {
          last = mid;
        }
        else if (randomFitness >= (double)this.cumulativeFitnessList[mid])
        {
          first = mid;
        }
        
        mid = (first + last)/2;
        if ((last - first) == 1) 
          idx = last;
          
      }
      return idx;
    }


    /// <summary>
    /// Calculate total fitness for current generation
    /// </summary>
    private void calculateTotalFitness()
    {
      this.totalFitness = 0;
      for (int i = 0; i < this.populationSize; i++)
      {
        Genome g = ((Genome) this.currentGeneration[i]);
        this.totalFitness += g.Fitness;
      }
      
    }
    
    /// <summary>
    /// Rank current generation and sort in order of fitness.
    /// </summary>
    private void updateCumulativeFitnessList()
    {
      double cumulativeFitness = 0.0;
      this.cumulativeFitnessList.Clear();
      for (int i = 0; i < this.populationSize; i++)
      {
        cumulativeFitness += ((Genome)this.currentGeneration[i]).Fitness;
        this.cumulativeFitnessList.Add(cumulativeFitness);
      }
    }
    private void createGenomes()
    {
      for (int i = 0; i < this.populationSize; i++)
      {
        Genome g = new Genome(this.genomeManager);
        g.CreateGenes();
        g.AssignMeaning();
        g.CalculateFitness();
        this.currentGeneration.Add(g);
      }
    }

    private void setCurrentEliteToTransmitToNextGeneration()
    {
      this.currentEliteToTransmitToNextGeneration.Clear();
      
      for(int i = populationSize - 1;
              i >=(populationSize - this.elitismRate*this.populationSize);
              i--)
      {
        this.currentEliteToTransmitToNextGeneration.Add((Genome)this.currentGeneration[i]);
      }
    }

    private void transmitEliteToNextGeneration()
    {
      for(int i = 0;
          i < this.currentEliteToTransmitToNextGeneration.Count-1;
          i++)
        
      {
        this.nextGeneration.Add(this.currentEliteToTransmitToNextGeneration[i]);
      }
    }

    private void createNextGeneration()
    {
      this.nextGeneration.Clear();

      for (int i = 0 ; i < this.populationSize ; i+=2)
      {
        int indexForParent1 = this.rouletteSelection();
        int indexForParent2 = this.rouletteSelection();
        
        Genome parent1, parent2;
      	Genome[] childs;
        parent1 = ((Genome) this.currentGeneration[indexForParent1]);
        parent2 = ((Genome) this.currentGeneration[indexForParent2]);
        if ((double)GeneticOptimizer.random.Next(1,1001)/1000 < this.crossoverRate)
        {
          childs = this.genomeManager.GetChilds(parent1, parent2);
         }
        else
        {//if the crossover doesn't take place there are only
        //two childs, identical to parents
          childs = new Genome[2];
          childs[0] = parent1.Clone();
          childs[1] = parent2.Clone();
        }
        foreach(Genome g in childs){
        	this.nextGeneration.Add(g);
        }
      }
      this.setCurrentEliteToTransmitToNextGeneration();
      this.transmitEliteToNextGeneration();
      
      this.mutateGenomes(this.nextGeneration);
      this.calculateFitnessAndMeaningForAllGenomes(this.nextGeneration);
      this.nextGeneration.Sort(this.genomeComparer);
      this.updateCurrentGeneration();
      this.currentGeneration.Sort(this.genomeComparer); 
      this.calculateTotalFitness();
      this.updateCumulativeFitnessList();
      
      this.generationCounter++;
    }
    
    //mutate all genomes of the given population, according to the mutation rate
    private void mutateGenomes(ArrayList populationOfGenomes)
    {
      foreach(Genome g in populationOfGenomes)
      {
        if(g != null)
           this.genomeManager.Mutate(g, this.MutationRate);
      }
    }
    
    //calculate Fitness and Meaning for each genome in populationOfGenomes
    private void calculateFitnessAndMeaningForAllGenomes(ArrayList populationOfGenomes)
    {
      foreach(Genome g in populationOfGenomes)
      {
        if(g != null)
        {
          g.CalculateFitness();
          g.AssignMeaning();
        }
      }
    }

    private void updateCurrentGeneration()
    {
      this.currentGeneration.Clear();
      // Note that next generation is greater than current:
      // due to the population size, genomes with lowest fitness are abandoned
      for (int i = 1 ; i <= this.populationSize; i++)
        this.currentGeneration.Add(this.nextGeneration[this.nextGeneration.Count - i]);
    }

    private void updateBestGenomeFoundInRunning(Genome genomeValue)
    {
      if(genomeValue.Fitness > this.bestGenome.Fitness)
        this.bestGenome = genomeValue.Clone();
    }
    private void updateWorstGenomeFoundInRunning(Genome genomeValue)
    {
      if(genomeValue.Fitness < this.worstGenome.Fitness)
        this.worstGenome = genomeValue.Clone();
    }
    private void setInitialBestAndWorstGenomes()
    {
      this.bestGenome = (Genome)this.currentGeneration[this.populationSize-1];
      this.worstGenome = (Genome)this.currentGeneration[0];
    }
    
  }
}
