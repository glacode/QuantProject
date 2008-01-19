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
	public delegate void NewGenerationEventHandler(
	Object sender , NewGenerationEventArgs eventArgs );

	/// <summary>
  /// The class needs to be initialized by an object implementing
  /// IGenomeManager interface 
  /// Default GO parameters: crossoverRate = 0.85, mutationRate = 0.05, elitismRate = 0.01,
  /// populationSize = 1000, generationNumber = 100
  /// keepOnRunningUntilConvergenceIsReached = false, minConvergenceRate = 0.80 
  /// </summary>
  [Serializable]
	public class GeneticOptimizer
	{
    #region fields  
    private Random random;
    
    private double mutationRate;
    private double crossoverRate;
    private double elitismRate;
    private double minConvergenceRate;
    private bool keepOnRunningUntilConvergenceIsReached;
    private int populationSize;
    private int generationNumber;
    private int genomeSize;
    private double totalSpecialFitnessForRouletteSelection;
    private double totalFitness;
    private Genome bestGenome;
    private Genome worstGenome;
    private IGenomeManager genomeManager;
    private GenomeComparer genomeComparer;
	
    private ArrayList currentGeneration;
    private ArrayList currentEliteToTransmitToNextGeneration;
    private ArrayList nextGeneration;
    private ArrayList cumulativeSpecialFitnessListForRouletteSelection;
	
    private int generationCounter;
    private double averageRandomFitness;
    private double standardDeviationOfRandomFitness;
    
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
    public ArrayList CurrentGeneration
    {
      get{return this.currentGeneration;}
    }
    
    /// <summary>
    /// Average fitness of a group of genomes (this.numberOfGenomesForAverageRandomFitness)
    /// chosen at random
    /// </summary>
    public double AverageRandomFitness
    {
      get{return this.averageRandomFitness;}
    }
    /// <summary>
    /// Standard deviation of fitness of a group of genomes (this.numberOfGenomesForAverageRandomFitness)
    /// chosen at random
    /// </summary>
    public double StandardDeviationOfRandomFitness
    {
      get{return this.standardDeviationOfRandomFitness;}
    }
    #endregion

		public event NewGenerationEventHandler NewGeneration;
    
    /// <summary>
    /// The class needs to be initialized by an object implementing
    /// IGenomeManager interface 
    /// Default GO parameters: crossoverRate = 0.85, mutationRate = 0.02, elitismRate = 0.01,
    /// keepOnRunningUntilConvergenceIsReached = false, minConvergenceRate = 0.80 
    /// </summary>
    public GeneticOptimizer(IGenomeManager genomeManager, int populationSize,
                            int generationNumber)
    {
      this.commonInitialization(genomeManager, populationSize, generationNumber);
      this.random = new Random(ConstantsProvider.SeedForRandomGenerator);
    }
    
    public GeneticOptimizer(IGenomeManager genomeManager, int populationSize,
                            int generationNumber, int seedForRandomGenerator)
    {
      this.commonInitialization(genomeManager, populationSize, generationNumber);
      this.random = new Random(seedForRandomGenerator);
    }
    
    public GeneticOptimizer(double crossoverRate, double mutationRate, double elitismRate, 
                            int populationSize, int generationNumber,
                            IGenomeManager genomeManager)
    {
      this.commonInitialization(genomeManager, populationSize, generationNumber);
      this.crossoverRate = crossoverRate;
      this.mutationRate = mutationRate;
      this.elitismRate = elitismRate;
      this.random = new Random(ConstantsProvider.SeedForRandomGenerator);
    }
    
    public GeneticOptimizer(double crossoverRate, double mutationRate, double elitismRate, 
                            int populationSize, int generationNumber,
                            IGenomeManager genomeManager, int seedForRandomGenerator)
    {
      this.commonInitialization(genomeManager, populationSize, generationNumber);
      this.crossoverRate = crossoverRate;
      this.mutationRate = mutationRate;
      this.elitismRate = elitismRate;
      this.random = new Random(seedForRandomGenerator);
    }

    private void commonInitialization(IGenomeManager genomeManager, 
                                      int populationSize, int generationNumber)
    {
      //default parameters for the GO
    	this.mutationRate = 0.05;
   		this.crossoverRate = 0.85;
    	this.elitismRate = 0.01;
    	this.minConvergenceRate = 0.80;
    	this.keepOnRunningUntilConvergenceIsReached = false;
    	
    	this.genomeManager = genomeManager;
    	this.populationSize = populationSize;
    	this.generationNumber = generationNumber;
    	this.genomeSize = this.genomeManager.GenomeSize;
      this.genomeComparer = new GenomeComparer();
      this.cumulativeSpecialFitnessListForRouletteSelection = new ArrayList(this.PopulationSize);
      this.currentGeneration = new ArrayList(this.PopulationSize);
      int eliteNumber = (int)(this.ElitismRate*(double)this.PopulationSize);
      this.currentEliteToTransmitToNextGeneration = new ArrayList(eliteNumber);
      this.nextGeneration = new ArrayList(this.populationSize +
                                         	eliteNumber);
      this.generationCounter = 1;
    }
    
    /// <summary>
    /// It updates AverageRandomFitness and StandardDeviationOfRandomFitness
    /// properties with the average and std dev fitness
    /// of a given group of genomes chosen at random
    /// </summary>
    public void CalculateRandomFitness()
    {
      this.createFirstGeneration(false);
      this.averageRandomFitness = this.totalSpecialFitnessForRouletteSelection/this.currentGeneration.Count;
      this.standardDeviationOfRandomFitness = this.calculateRandomFitness_getStdDevOfRandomFitness();
    }
    /// <summary>
    /// It updates StandardDeviationOfRandomFitness property with the std dev of
    /// fitness of a given group of genomes chosen at random
    /// </summary>
    private double calculateRandomFitness_getStdDevOfRandomFitness()
    {
      double[] fitnesses = new double[this.currentGeneration.Count];
      for(int i = 0; i<this.currentGeneration.Count; i++)
        fitnesses[i] = ((Genome)this.currentGeneration[i]).Fitness;
      return QuantProject.ADT.Statistics.BasicFunctions.StdDev(fitnesses);
    }
    
    private void run_calculateRandomFitness()
    {
      GeneticOptimizer GOForAverageRandomFitness = 
        new GeneticOptimizer(this.genomeManager, ConstantsProvider.NumGenomesForRandomFitnessComputation,
                             0,ConstantsProvider.SeedForRandomGenerator);
      GOForAverageRandomFitness.CalculateRandomFitness(); 
      this.averageRandomFitness = GOForAverageRandomFitness.AverageRandomFitness;
      this.standardDeviationOfRandomFitness = GOForAverageRandomFitness.StandardDeviationOfRandomFitness;
    }
    /// <summary>
    /// Method to start the GeneticOptmizer
    /// </summary>
    public void Run(bool showOutputToConsole)
    {
      //this.run_calculateRandomFitness(); to be tested yet
      if (this.genomeSize == 0 || this.genomeSize < 0)
        throw new IndexOutOfRangeException("Genome size not set");
      this.createFirstGeneration(showOutputToConsole);
      if(this.keepOnRunningUntilConvergenceIsReached)
      //The GO keeps on generating new population until convergence is reached
      {
        for (int i = 0; !this.IsConvergenceReached(); i++)
         	this.generateNewPopulation(showOutputToConsole);
      }
      else // the GO simply generates the given number of populations and then stops
      {
        for (int i = 0; i < this.generationNumber; i++)
          this.generateNewPopulation(showOutputToConsole);
      }
    }
    
    private void generateNewPopulation(bool showOutputToConsole)
    {
    	this.createNextGeneration();
      this.generationCounter++;
      this.updateBestGenomeFoundInRunning((Genome)this.currentGeneration[populationSize-1]);
      this.updateWorstGenomeFoundInRunning((Genome)this.currentGeneration[0]);
      if (showOutputToConsole)
           this.showOutputToConsole();
    }
    
    private bool IsConvergenceReached()
    {
      bool returnValue = false;
      double averageFitness = this.totalFitness / this.populationSize;
      double bestFitnessReachedUntilNow = this.BestGenome.Fitness;
      if (averageFitness >= this.minConvergenceRate * bestFitnessReachedUntilNow )
          returnValue = true;
      
      return returnValue;
    }

    private void sortCurrentGenerationAndFireNewGenerationEvent()
    {
      // comment out the following three lines if you have an exception
      // in the Sort method and you want to break to the proper line
//      double fitness;
//      for ( int i = 0 ; i < this.currentGeneration.Count ; i++ )
//      	fitness = ((Genome) this.currentGeneration[ i ]).Fitness;

      this.currentGeneration.Sort(this.genomeComparer);
      if(this.NewGeneration != null)
        this.NewGeneration( this , new NewGenerationEventArgs(
          this.currentGeneration , this.generationCounter , this.generationNumber ) );
    }

    private void setSpecialFitnessForRouletteSelection()
    {
      this.calculateTotalSpecialFitnessForRouletteSelectionAndPlainTotalFitness();
      this.updateCumulativeSpecialFitnessListForRouletteSelection();
    }

    private void createFirstGeneration(bool showOutputToConsole)
    {
      this.createGenomes();
      this.sortCurrentGenerationAndFireNewGenerationEvent();
      this.setInitialBestAndWorstGenomes();
      this.setSpecialFitnessForRouletteSelection();
      if (showOutputToConsole)
        this.showOutputToConsole();
    }
    
    private void showOutputToConsole()
    {
      string genes = "";
      //System.Console.WriteLine("\n*_*_*_*_*_*_*_*_*_*_*\n\nGeneration " + this.generationCounter +"\n");
//      string pathFile = System.Configuration.ConfigurationSettings.AppSettings["GenericArchive"] +
//                    "\\GenomesThroughoutGenerations.txt";
//  	  System.IO.StreamWriter w = System.IO.File.AppendText(pathFile);
      for(int i = 0; i < this.populationSize; i++)
      {
        int genomeNumber = i + 1;
      	foreach(int gene in ((Genome)this.currentGeneration[i]).Genes() )
            genes = genes + " " + gene.ToString();
				System.Console.WriteLine("\r\n" + this.GenerationCounter + " " + genomeNumber + " " + genes + " " + 
                ((Genome)this.currentGeneration[i]).Fitness + " " +
                ((Genome)this.currentGeneration[i]).HasBeenMutated.ToString() + " " +
                ((Genome)this.currentGeneration[i]).HasBeenCloned.ToString() + " " +
                ((Genome)this.currentGeneration[i]).Generation + " " +
                   this.BestGenome.Fitness);
//        w.Write("\r\n" + this.GenerationCounter + " " + genomeNumber + " " + genes + " " + 
//                ((Genome)this.currentGeneration[i]).Fitness + " " +
//                ((Genome)this.currentGeneration[i]).HasBeenMutated.ToString() + " " +
//                ((Genome)this.currentGeneration[i]).HasBeenCloned.ToString() + " " +
//                ((Genome)this.currentGeneration[i]).Generation + " " +
//                 this.BestGenome.Fitness);
        genes = "";
      }
//      w.Flush();
//      w.Close();
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
      double randomFitness = this.totalSpecialFitnessForRouletteSelection *(double)this.random.Next(1,1001)/1000;
      int idx = -1;
      int first = 0;
      int last = this.populationSize -1;
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


    /// <summary>
    /// Calculate total special fitness for current generation
    /// and plain total fitness
    /// </summary>
    private void calculateTotalSpecialFitnessForRouletteSelectionAndPlainTotalFitness()
    {
      this.totalSpecialFitnessForRouletteSelection = 0.0;
      this.totalFitness = 0.0;
      double worstSpecialFitnessForRouletteSelection = 
      	Math.Abs(((Genome)this.currentGeneration[0]).Fitness);
      foreach(Genome g in this.currentGeneration)
      {
      	this.totalSpecialFitnessForRouletteSelection += g.Fitness +
      								worstSpecialFitnessForRouletteSelection;
      	this.totalFitness += g.Fitness;
      }
      																									
            
    }
        
    private void updateCumulativeSpecialFitnessListForRouletteSelection()
    {
      double cumulativeSpecialFitness = 0.0;
      this.cumulativeSpecialFitnessListForRouletteSelection.Clear();
      for (int i = 0; i < this.populationSize; i++)
      {
        cumulativeSpecialFitness += 
        		((Genome)this.currentGeneration[i]).Fitness +
        	Math.Abs(((Genome)this.currentGeneration[0]).Fitness);
        this.cumulativeSpecialFitnessListForRouletteSelection.Add(cumulativeSpecialFitness);
      }
    }
    private void createGenomes()
    {
      for (int i = 0; i < this.populationSize; i++)
      {
        Genome g = new Genome(this.genomeManager, this);
        g.CreateGenes();
        this.currentGeneration.Add(g);
      }
    }

    private void createNextGeneration_transmitEliteToNextGeneration()
    {
      this.currentEliteToTransmitToNextGeneration.Clear();
      
      for(int i = populationSize - 1;
        i >=(populationSize - this.elitismRate*this.populationSize - 1);
        i--)
      {
        if(this.currentGeneration[i] is Genome)
          this.currentEliteToTransmitToNextGeneration.Add((Genome)this.currentGeneration[i]);
      }
      
      for(int i = 0;
          i < this.currentEliteToTransmitToNextGeneration.Count;
          i++)
        
      {
    		this.nextGeneration.Add((Genome)this.currentEliteToTransmitToNextGeneration[i]);
      }
    }

    private void createNextGeneration_addChildsWithRouletteSelection()
    {
      for (int i = 0 ; i < this.populationSize ; i+=2)
      {
        int indexForParent1 = this.rouletteSelection();
        int indexForParent2 = this.rouletteSelection();
        Genome parent1, parent2;
        Genome[] childs;
        parent1 = ((Genome) this.currentGeneration[indexForParent1]);
        parent2 = ((Genome) this.currentGeneration[indexForParent2]);
        if ((double)this.random.Next(1,1001)/1000 < this.crossoverRate)
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
        foreach(Genome g in childs)
          this.nextGeneration.Add(g);
      }
    }

    private void createNextGeneration()
    {
      this.nextGeneration.Clear();
      this.createNextGeneration_addChildsWithRouletteSelection();
      this.mutateGenomes();
      this.createNextGeneration_transmitEliteToNextGeneration();
      this.updateCurrentGeneration();
      this.sortCurrentGenerationAndFireNewGenerationEvent();
      this.setSpecialFitnessForRouletteSelection();
    }
    
    private void mutateGenomes()
    {
      foreach(Genome g in this.nextGeneration)
      {
        if( GenomeManagement.RandomGenerator.Next(0,101) < (int)(this.mutationRate*100) ) 
        {
        	this.genomeManager.Mutate(g);
        	g.HasBeenMutated = true;
        }
      }
    }
 
    private void updateCurrentGeneration()
    {
      this.nextGeneration.Sort(this.genomeComparer);
      this.currentGeneration.Clear();
      int numOfNextGeneration = this.nextGeneration.Count;
      // Note that next generation is greater than current:
      // due to the population size, genomes with lowest fitness are abandoned
      for (int i = 1 ; i <= this.populationSize; i++)
        this.currentGeneration.Add(this.nextGeneration[numOfNextGeneration - i]);
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
      this.bestGenome = ((Genome)this.currentGeneration[this.populationSize-1]).Clone();
      this.worstGenome = ((Genome)this.currentGeneration[0]).Clone();
    }
    
  }
}
