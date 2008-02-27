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
		protected TestingPositions[] bestTestingPositions;
		protected Benchmark benchmark;
		protected IDecoderForTestingPositions decoderForTestingPositions;
		protected IFitnessEvaluator fitnessEvaluator;
		protected IHistoricalQuoteProvider historicalQuoteProvider;
		protected IGenomeManager genomeManager;
		protected double crossoverRate;
		protected double mutationRate;
		protected double elitismRate;
		protected int populationSizeForGeneticOptimizer;
		protected int generationNumberForGeneticOptimizer;
		protected int seedForRandomGenerator;

		protected GeneticOptimizer geneticOptimizer;


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
			IHistoricalQuoteProvider historicalQuoteProvider ,
			double crossoverRate , double mutationRate , double elitismRate ,
			int populationSizeForGeneticOptimizer ,
			int generationNumberForGeneticOptimizer ,
			int seedForRandomGenerator )
		{
			this.numberOfPortfolioPositions = numberOfPortfolioPositions;
			this.numberOfBestTestingPositionsToBeReturned = numberOfBestTestingPositionsToBeReturned;
//			this.choosePositionsWithAtLeastOneDifferentTicker =
//				choosePositionsWithAtLeastOneDifferentTicker;
			this.bestTestingPositions = new TestingPositions[numberOfBestTestingPositionsToBeReturned];
			this.benchmark = benchmark;
			this.decoderForTestingPositions = decoderForTestingPositions;
			this.fitnessEvaluator =	fitnessEvaluator;
			this.historicalQuoteProvider = historicalQuoteProvider;
			this.crossoverRate = crossoverRate;
			this.mutationRate = mutationRate;
			this.elitismRate = elitismRate;
			this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
			this.generationNumberForGeneticOptimizer =
				generationNumberForGeneticOptimizer;
			this.seedForRandomGenerator = seedForRandomGenerator;
		}

		#region AnalyzeInSample
		private void analyzeInSample_checkParameters(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			if ( eligibleTickers.Count <	this.numberOfPortfolioPositions )
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
		private string getProgressMessage(
			int generationCounter , int generationNumber )
		{
			string progressMessage =
				generationCounter.ToString() + " / " +
				generationNumber.ToString() +
				" - " +
				DateTime.Now.ToString();
			return progressMessage;
		}
		private void sendNewMessage( NewGenerationEventArgs e )
		{
			string message = this.getProgressMessage(
				e.GenerationCounter , e.GenerationNumber );
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
		
		//it returns a hashCode for the given genome
		//normally, it should be overrided in inherited classes
		protected abstract string getHashCodeForGenome(Genome genome);
//		{
//			string returnValue = genome.Meaning.GetHashCode().ToString();
////			if (this.choosePositionsWithAtLeastOneDifferentTicker)
////				returnValue = ((TestingPositions)genome.Meaning).HashCodeForTickerComposition;
////			else
//			return returnValue;
//		}
		
		private void setBestTestingPositions()
		{
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
					this.bestTestingPositions[addedTestingPositions] = 
						(TestingPositions)currentGenome.Meaning;
      		genomesCollector.Add(currentGenomeHashcode, null);
					addedTestingPositions++;
      	}
      	counter ++ ;
      }  
		}

		protected abstract IGenomeManager getGenomeManager(EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager);
		//returns a specific IGenomeManager object in inherited classes
		
		private TestingPositions[] getBestTestingPositionsInSample(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager
			)
		{
			this.genomeManager = this.getGenomeManager(eligibleTickers, returnsManager);
			this.geneticOptimizer = new GeneticOptimizer(
				this.crossoverRate ,
				this.mutationRate ,
				this.elitismRate ,
				this.populationSizeForGeneticOptimizer ,
				this.generationNumberForGeneticOptimizer ,
				this.genomeManager ,
				this.seedForRandomGenerator );
			this.geneticOptimizer.NewGeneration +=
				new NewGenerationEventHandler( this.newGenerationEventHandler );
			this.geneticOptimizer.Run( false );
			this.setBestTestingPositions();
	
			return this.bestTestingPositions;
		}

		/// <summary>
		/// Returns the best TestingPositions with respect to the in sample data
		/// </summary>
		/// <param name="eligibleTickers"></param>
		/// <returns></returns>
		public object AnalyzeInSample(
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
