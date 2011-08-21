/*
QuantProject - Quantitative Finance Library

DrivenBySharpeRatioInSampleChooser.cs
Copyright (C) 2011
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

using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.Strategies;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.Optimizing.GenomeManagers;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;


namespace QuantProject.Scripts.TickerSelectionTesting.DrivenBySharpeRatio.InSampleChoosers.Genetic
{
	/// <summary>
	/// In sample genetic analyzer for the
	/// Driven by Sharpe Ratio strategy 
	/// </summary>
	[Serializable]
	public class DrivenBySharpeRatioInSampleChooser : GeneticChooser
	{
    protected GenomeManagerType genomeManagerType;
    protected bool keepOnRunningUntilConvergenceHasReached;
		protected double minConvergenceRate;
    	
		public DrivenBySharpeRatioInSampleChooser(
			int numberOfPortfolioPositions , int numberOfBestTestingPositionsToBeReturned ,
			Benchmark benchmark ,
			IDecoderForTestingPositions decoderForTestingPositions ,
			GenomeManagerType genomeManagerType ,
			IFitnessEvaluator fitnessEvaluator ,
			HistoricalMarketValueProvider historicalMarketValueProvider ,
			double crossoverRate , double mutationRate , double elitismRate ,
			int populationSizeForGeneticOptimizer ,
			int generationNumberForGeneticOptimizer ,
			int seedForRandomGenerator,
			bool keepOnRunningUntilConvergenceHasReached,
			double minConvergenceRate)
	    :
			base(numberOfPortfolioPositions, numberOfBestTestingPositionsToBeReturned,
			benchmark ,	decoderForTestingPositions ,
			fitnessEvaluator , historicalMarketValueProvider ,
			crossoverRate , mutationRate , elitismRate ,
			populationSizeForGeneticOptimizer ,
			generationNumberForGeneticOptimizer ,
			seedForRandomGenerator )
		{
    	this.genomeManagerType = genomeManagerType;
    	this.keepOnRunningUntilConvergenceHasReached =
    		keepOnRunningUntilConvergenceHasReached;
    	this.minConvergenceRate = minConvergenceRate;
		}
		
		protected override string getHashCodeForGenome(QuantProject.ADT.Optimizing.Genetic.Genome genome)
		{
			string returnValue = ((TestingPositions)genome.Meaning).HashCodeForTickerComposition;
			return returnValue;
		}
    
		public override IGenomeManager GetGenomeManager(EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager)
		{
			return new GenomeManagerForDrivenBySharpeRatio(eligibleTickers, this.numberOfPortfolioPositions,
			                                        this.decoderForTestingPositions, this.fitnessEvaluator,
			                                        this.genomeManagerType , returnsManager,
			                                        this.seedForRandomGeneratorForTheGeneticOptimizer);
		}
		
//		public DrivenBySharpeRatioInSampleChooser GetCopy(int numOfPortfolioPositions , int numOfBestTestingPositionsToBeReturned ,
//			IDecoderForTestingPositions decoder, int populationSizeForGO,
//			int generationNumberForGO)
//		{
//			DrivenBySharpeRatioInSampleChooser returnValue =
//				new DrivenBySharpeRatioInSampleChooser(numOfPortfolioPositions,
//				    	numOfBestTestingPositionsToBeReturned, this.benchmark,
//				    	decoder, this.genomeManagerType, fitnessEvaluator,
//				    	this.historicalMarketValueProvider, this.crossoverRate, this.mutationRate,
//				    	this.elitismRate, populationSizeForGO, generationNumberForGO,
//				    	this.seedForRandomGeneratorForTheGeneticOptimizer, this.keepOnRunningUntilConvergenceHasReached,
//				    	this.minConvergenceRate);
//			
//			return returnValue;
//		}
	}
}
