/*
QuantProject - Quantitative Finance Library

FixedLengthTwoPhasesGeneticChooser.cs
Copyright (C) 2007
Glauco Siliprandi

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

using QuantProject.ADT;
using QuantProject.ADT.Messaging;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.WalkForwardTesting.FixedLengthTwoPhases
{
	/// <summary>
	/// In sample analyzer for the walk forward fixed length two phases strategy
	/// </summary>
	public class FixedLengthTwoPhasesGeneticChooser : IInSampleChooser
	{
		public event NewProgressEventHandler NewProgress;
		public event NewMessageEventHandler NewMessage;

		private int numberOfPortfolioPositions;
		private int inSampleDays;
		private Benchmark benchmark;
		private IDecoderForWeightedPositions decoderForWeightedPositions;
		private FixedLengthTwoPhasesFitnessEvaluator fixedLengthTwoPhasesFitnessEvaluator;
		private IHistoricalQuoteProvider historicalQuoteProvider;
		private double crossoverRate;
		private double mutationRate;
		private double elitismRate;
		private int populationSizeForGeneticOptimizer;
		private int generationNumberForGeneticOptimizer;
		private int seedForRandomGenerator;

		private GeneticOptimizer geneticOptimizer;


		public string Description
		{
			get
			{
				string description = "genetic_" +
					"longOnly_" +
					"gnrtnSz_" + this.populationSizeForGeneticOptimizer +
					"_gnrtnNmbr_" + this.generationNumberForGeneticOptimizer;
				return description;
			}
		}
		public FixedLengthTwoPhasesGeneticChooser(
			int numberOfPortfolioPositions , int inSampleDays , Benchmark benchmark ,
			IDecoderForWeightedPositions decoderForWeightedPositions ,
			FixedLengthTwoPhasesFitnessEvaluator fixedLengthTwoPhasesFitnessEvaluator ,
			IHistoricalQuoteProvider historicalQuoteProvider ,
			double crossoverRate , double mutationRate , double elitismRate ,
			int populationSizeForGeneticOptimizer ,
			int generationNumberForGeneticOptimizer ,
			int seedForRandomGenerator )
		{
			this.numberOfPortfolioPositions = numberOfPortfolioPositions;
			this.inSampleDays =	inSampleDays;
			this.benchmark = benchmark;
			this.decoderForWeightedPositions = decoderForWeightedPositions;
			this.fixedLengthTwoPhasesFitnessEvaluator =
				fixedLengthTwoPhasesFitnessEvaluator;
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
				throw new Exception( "Eligilbe tickers for driving positions contains " +
					"only " + eligibleTickers.Count +
					" elements, while NumberOfDrivingPositions is " +
					this.numberOfPortfolioPositions );
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
//			// comment out this line if no debug is done
//			WFLagGenerationDebugger wFLagGenerationDebugger =
//				new WFLagGenerationDebugger(
//				e.Generation ,
//				this.timeWhenChosePositionsIsRequested.DateTime ,
			//				this.NumberDaysForInSampleOptimization ,
			//				this.benchmark );
			//			wFLagGenerationDebugger.Debug();
			this.sendNewProgress( e );
			this.sendNewMessage( e );
		}
		#endregion newGenerationEventHandler
		private void checkIfBestGenomeIsDecodable(
			IGenomeManager genomeManager , Genome genome )
		{
			object genomeMeaning = genomeManager.Decode( genome );
			if ( !(genomeMeaning is WeightedPositions) )
				throw new Exception( "The genome is not a WeightedPositions. " +
					"It should happen only if the genome is undecodable. This " +
					"should never happen for the best genome." );
		}

		private WeightedPositions getBestWeightedPositionsInSample(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager
			)
		{
			FixedLengthTwoPhasesGenomeManager	genomeManager = 
				new FixedLengthTwoPhasesGenomeManager(
				this.numberOfPortfolioPositions ,
				eligibleTickers ,
				returnsManager ,
				this.decoderForWeightedPositions ,
				this.fixedLengthTwoPhasesFitnessEvaluator ,
				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator );

			this.geneticOptimizer = new GeneticOptimizer(
				this.crossoverRate ,
				this.mutationRate ,
				this.elitismRate ,
				this.populationSizeForGeneticOptimizer ,
				this.generationNumberForGeneticOptimizer ,
				genomeManager ,
				this.seedForRandomGenerator );

			this.geneticOptimizer.NewGeneration +=
				new NewGenerationEventHandler( this.newGenerationEventHandler );

			this.geneticOptimizer.Run( false );

//			this.generation = geneticOptimizer.BestGenome.Generation;

			this.checkIfBestGenomeIsDecodable(
				genomeManager , this.geneticOptimizer.BestGenome );

			WeightedPositions bestWeightedPositionsInSample =
				(WeightedPositions)genomeManager.Decode( this.geneticOptimizer.BestGenome );
			
			return bestWeightedPositionsInSample;
		}

		/// <summary>
		/// Returns the best WeightedPositions with respect to the in sample data
		/// </summary>
		/// <param name="eligibleTickers"></param>
		/// <param name="currentOutOfSampleEndOfDayDateTime"></param>
		/// <returns></returns>
		public object AnalyzeInSample(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			this.analyzeInSample_checkParameters( eligibleTickers ,
				returnsManager );
			WeightedPositions bestWeightedPositionsInSample =
				this.getBestWeightedPositionsInSample( eligibleTickers ,
				returnsManager );
			return bestWeightedPositionsInSample;
		}
		#endregion AnalyzeInSample
	}
}
