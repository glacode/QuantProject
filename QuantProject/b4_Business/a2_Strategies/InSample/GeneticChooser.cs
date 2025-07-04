/*
QuantProject - Quantitative Finance Library

GeneticChooser.cs
Copyright (C) 2008
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
using QuantProject.ADT.Messaging;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies.OutOfSample;

namespace QuantProject.Business.Strategies.InSample
{
	/// <summary>
	/// Abstract GeneticChooser to be used for
	/// in sample optimization
	/// </summary>
	[Serializable]
	public abstract class GeneticChooser : IInSampleChooser
	{
		public event NewProgressEventHandler NewProgress;
		public event NewMessageEventHandler NewMessage;
		
		protected int numberOfPortfolioPositions;
		protected int numberOfBestTestingPositionsToBeReturned;
		//the genetic chooser will return the requested number
		//of genome (which meaning is a TestingPositions) that
		//have a different hash code (
		//the method that returns the hash code for genome
		//is virtual)
//		protected TestingPositions[] bestTestingPositions;
		protected Benchmark benchmark;
		protected IDecoderForTestingPositions decoderForTestingPositions;
		protected IFitnessEvaluator fitnessEvaluator;
		protected HistoricalMarketValueProvider historicalMarketValueProvider;
		protected IGenomeManager genomeManager;
		protected double crossoverRate;
		protected double mutationRate;
		protected double elitismRate;
		protected int populationSizeForGeneticOptimizer;
		protected int generationNumberForGeneticOptimizer;
		protected int seedForRandomGeneratorForTheGeneticOptimizer;
		
		[NonSerialized]
		protected ArrayList currentGeneration;
		[NonSerialized]
		protected ArrayList nextGeneration;

		[NonSerialized]
		protected GeneticOptimizer geneticOptimizer;
		
		private bool useClassicGeneticOptimizer;


		public string Description
		{
			get
			{
				string description = "PopSize_" + this.populationSizeForGeneticOptimizer +
					"_GenNum_" + this.generationNumberForGeneticOptimizer +
					"_FitnEval_" + this.fitnessEvaluator.Description +
					"_DecoderForTestingPositions_" + this.decoderForTestingPositions;
				return description;
			}
		}
		
		/// <summary>
		/// Abstract GeneticChooser to be used for
		/// in sample optimization
		/// </summary>
		/// <param name="numberOfBestTestingPositionsToBeReturned">
		/// The number of TestingPositions that the
		/// AnalyzeInSample method will return
		/// </param>
		public GeneticChooser(
			int numberOfPortfolioPositions , int numberOfBestTestingPositionsToBeReturned ,
			Benchmark benchmark ,
			IDecoderForTestingPositions decoderForTestingPositions ,
			IFitnessEvaluator fitnessEvaluator ,
			HistoricalMarketValueProvider historicalMarketValueProvider ,
			double crossoverRate , double mutationRate , double elitismRate ,
			int populationSizeForGeneticOptimizer ,
			int generationNumberForGeneticOptimizer ,
			int seedForRandomGeneratorForTheGeneticOptimizer )
		{
			this.commonInitialization(
				numberOfPortfolioPositions , numberOfBestTestingPositionsToBeReturned ,
				benchmark ,
				decoderForTestingPositions ,
				fitnessEvaluator ,
				historicalMarketValueProvider ,
				crossoverRate , mutationRate , elitismRate ,
				populationSizeForGeneticOptimizer ,
				generationNumberForGeneticOptimizer ,
				seedForRandomGeneratorForTheGeneticOptimizer );
			this.useClassicGeneticOptimizer = true;
		}
		
		private void commonInitialization(
			int numberOfPortfolioPositions , int numberOfBestTestingPositionsToBeReturned ,
			Benchmark benchmark ,
			IDecoderForTestingPositions decoderForTestingPositions ,
			IFitnessEvaluator fitnessEvaluator ,
			HistoricalMarketValueProvider historicalMarketValueProvider ,
			double crossoverRate , double mutationRate , double elitismRate ,
			int populationSizeForGeneticOptimizer ,
			int generationNumberForGeneticOptimizer ,
			int seedForRandomGeneratorForTheGeneticOptimizer )
		{
			this.numberOfPortfolioPositions = numberOfPortfolioPositions;
			this.numberOfBestTestingPositionsToBeReturned = numberOfBestTestingPositionsToBeReturned;
			this.benchmark = benchmark;
			this.decoderForTestingPositions = decoderForTestingPositions;
			this.fitnessEvaluator =	fitnessEvaluator;
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.crossoverRate = crossoverRate;
			this.mutationRate = mutationRate;
			this.elitismRate = elitismRate;
			this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
			this.generationNumberForGeneticOptimizer =
				generationNumberForGeneticOptimizer;
			this.seedForRandomGeneratorForTheGeneticOptimizer =
				seedForRandomGeneratorForTheGeneticOptimizer;
		}
		
		public GeneticChooser(
			int numberOfPortfolioPositions , int numberOfBestTestingPositionsToBeReturned ,
			Benchmark benchmark ,
			IDecoderForTestingPositions decoderForTestingPositions ,
			IFitnessEvaluator fitnessEvaluator ,
			HistoricalMarketValueProvider historicalMarketValueProvider ,
			double crossoverRate , double mutationRate , double elitismRate ,
			int populationSizeForGeneticOptimizer ,
			int generationNumberForGeneticOptimizer ,
			int seedForRandomGeneratorForTheGeneticOptimizer ,
			ArrayList currentGeneration ,
			ArrayList nextGeneration	)
		{
			this.commonInitialization(
				numberOfPortfolioPositions , numberOfBestTestingPositionsToBeReturned ,
				benchmark ,
				decoderForTestingPositions ,
				fitnessEvaluator ,
				historicalMarketValueProvider ,
				crossoverRate , mutationRate , elitismRate ,
				populationSizeForGeneticOptimizer ,
				generationNumberForGeneticOptimizer ,
				seedForRandomGeneratorForTheGeneticOptimizer );
			this.currentGeneration = currentGeneration;
			this.nextGeneration = nextGeneration;
			this.useClassicGeneticOptimizer = false;
		}


		#region AnalyzeInSample
		private void analyzeInSample_checkParameters(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			if ( eligibleTickers.Count <	this.numberOfPortfolioPositions )
				throw new Exception( "Eligible tickers at date " +
				  eligibleTickers.DateAtWhichTickersAreEligible.ToString() + " " +
				  "for driving positions contains only " +
					eligibleTickers.Count + " " +
					"elements, while number of portfolio positions is " +
					this.numberOfPortfolioPositions );
			if ( this.numberOfBestTestingPositionsToBeReturned > 
						this.populationSizeForGeneticOptimizer )
				throw new Exception( "Eligible tickers for driving positions contains " +
				                    "only " + eligibleTickers.Count +
				                    " elements, while number of portfolio positions is " +
				                    this.numberOfPortfolioPositions );
			if ( this.numberOfBestTestingPositionsToBeReturned >
			    this.populationSizeForGeneticOptimizer )
				throw new Exception( "Number of BestTestingPositions for " +
				                    "out of sample testing is too high with " +
				                    "respect to the population size of the " +
				                    "genetic optimizer" );
		}
		
		#region newGenerationEventHandler
		private void sendNewProgress( NewGenerationEventArgs e )
		{
			if ( this.NewProgress != null )
				this.NewProgress( this ,
				                 new NewProgressEventArgs( e.GenerationCounter , e.GenerationNumber ) );
		}
		#region sendNewMessage
		private double getProgressMessage_getAverageFitness(NewGenerationEventArgs e)
		{
			double totalFitness = 0.0;
			int populationSize = e.CurrentGeneticOptimizer.PopulationSize;
			for(int i = 0; i < populationSize; i++)
				totalFitness += ((Genome)e.Generation[i]).Fitness;
			
			return totalFitness / populationSize;
		}
		private string getProgressMessage(NewGenerationEventArgs e)
		{
			Genome bestGenome = (Genome)e.Generation[e.Generation.Count - 1];
			if( e.CurrentGeneticOptimizer.BestGenome != null )
				bestGenome = e.CurrentGeneticOptimizer.BestGenome;
			double worstFitness = ((Genome)e.Generation[0]).Fitness;
			if( e.CurrentGeneticOptimizer.WorstGenome != null)
				worstFitness = e.CurrentGeneticOptimizer.WorstGenome.Fitness;
			double averageFitness = this.getProgressMessage_getAverageFitness(e);
			string progressMessage =
				e.GenerationCounter.ToString() + " / " +
				e.GenerationNumber.ToString() +
				" ; Abs Best: " + bestGenome.Fitness.ToString("0.00000000") +
				" (gen: " + bestGenome.Generation.ToString() + ")" +
				" ; Abs Worst: " + worstFitness.ToString("0.000000") + 
				" ; Avg of gen : " + averageFitness.ToString("0.0000") +
				" - " + DateTime.Now.ToString();
			return progressMessage;
		}
		private void sendNewMessage( NewGenerationEventArgs e )
		{
			string message = this.getProgressMessage( e );
			NewMessageEventArgs newMessageEventArgs =
				new NewMessageEventArgs( message );
			if( this.NewMessage != null )
				this.NewMessage( this , newMessageEventArgs );
		}
		#endregion sendNewMessage
		private void newGenerationEventHandler(
			object sender , NewGenerationEventArgs e )
		{
			this.sendNewProgress( e );
			this.sendNewMessage( e );
		}
		#endregion newGenerationEventHandler
		
		//it returns the inherited HashCode from object
		//normally, it should be overrided in inherited classes
		protected abstract string getHashCodeForGenome(Genome genome);
//		{
//			string returnValue = genome.Meaning.GetHashCode().ToString();
		////			if (this.choosePositionsWithAtLeastOneDifferentTicker)
		////				returnValue = ((TestingPositions)genome.Meaning).HashCodeForTickerComposition;
		////			else
//			return returnValue;
//		}

		
		private TestingPositions[] getBestTestingPositionsInSample_getTestingPositionsActually()
		{
			TestingPositions[] bestTestingPositions = new TestingPositions[numberOfBestTestingPositionsToBeReturned];
			GeneticOptimizer GO = this.geneticOptimizer;
			int addedTestingPositions = 0;
			int counter = 0;
			Genome currentGenome = null;
			string currentGenomeHashcode;
			Hashtable genomesCollector = new Hashtable();
			while(addedTestingPositions < this.numberOfBestTestingPositionsToBeReturned &&
			      counter < GO.PopulationSize)
			{
				currentGenome = (Genome)GO.CurrentGeneration[GO.PopulationSize - 1 - counter];
				currentGenomeHashcode = this.getHashCodeForGenome(currentGenome);
				if( counter == 0 || !genomesCollector.ContainsKey(currentGenomeHashcode) )
				{
					bestTestingPositions[addedTestingPositions] =
						(TestingPositions)currentGenome.Meaning;
					((TestingPositions)bestTestingPositions[addedTestingPositions]).FitnessInSample =
						currentGenome.Fitness;
					((IGeneticallyOptimizable)bestTestingPositions[addedTestingPositions]).Generation =
						currentGenome.Generation;
					genomesCollector.Add(currentGenomeHashcode, null);
					addedTestingPositions++;
				}
				counter ++ ;
			}
			return bestTestingPositions;
		}

		public abstract IGenomeManager GetGenomeManager(EligibleTickers eligibleTickers ,
		                                                ReturnsManager returnsManager);
		//returns a specific IGenomeManager object in inherited classes
		
		private TestingPositions[] getBestTestingPositionsInSample(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager
		)
		{
			this.genomeManager = this.GetGenomeManager(eligibleTickers, returnsManager);
			if ( this.useClassicGeneticOptimizer )
				// the user has not provided custom generations
				this.geneticOptimizer = new GeneticOptimizer(
					this.crossoverRate ,
					this.mutationRate ,
					this.elitismRate ,
					this.populationSizeForGeneticOptimizer ,
					this.generationNumberForGeneticOptimizer ,
					this.genomeManager ,
					this.seedForRandomGeneratorForTheGeneticOptimizer );
			else
				// the user has provided custom generations
				this.geneticOptimizer = new AlternativeGeneticOptimizer(
					this.crossoverRate ,
					this.mutationRate ,
					this.elitismRate ,
					this.populationSizeForGeneticOptimizer ,
					this.generationNumberForGeneticOptimizer ,
					this.genomeManager ,
					this.seedForRandomGeneratorForTheGeneticOptimizer ,
					this.currentGeneration ,
					this.nextGeneration );
			
			this.geneticOptimizer.NewGeneration +=
				new NewGenerationEventHandler( this.newGenerationEventHandler );
			this.geneticOptimizer.Run( false );
			
			return this.getBestTestingPositionsInSample_getTestingPositionsActually();
		}

		/// <summary>
		/// Returns the best TestingPositions with respect to the in sample data
		/// </summary>
		/// <param name="eligibleTickers"></param>
		/// <returns></returns>
		public virtual object AnalyzeInSample(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			this.analyzeInSample_checkParameters( eligibleTickers ,
			                                     returnsManager );
			TestingPositions[] bestTestingPositionsInSample =
				this.getBestTestingPositionsInSample( eligibleTickers ,
				                                     returnsManager );
			return bestTestingPositionsInSample;
		}
		#endregion AnalyzeInSample
	}
}
