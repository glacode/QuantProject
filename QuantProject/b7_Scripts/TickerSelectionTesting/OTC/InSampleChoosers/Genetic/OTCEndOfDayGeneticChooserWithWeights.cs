/*
QuantProject - Quantitative Finance Library

OTCEndOfDayGeneticChooserWithWeights.cs
Copyright (C) 2009
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


namespace QuantProject.Scripts.TickerSelectionTesting.OTC.InSampleChoosers.Genetic
{
	/// <summary>
	/// In sample genetic analyzer for the Open to Close strategy
	/// using EOD data and optimizing weights
	/// </summary>
	[Serializable]
	public class OTCEndOfDayGeneticChooserWithWeights : OTCEndOfDayGeneticChooser
	{
		public OTCEndOfDayGeneticChooserWithWeights(
			int numberOfPortfolioPositions , int numberOfBestTestingPositionsToBeReturned ,
			Benchmark benchmark ,
			GenomeManagerType genomeManagerType ,
			IFitnessEvaluator fitnessEvaluator ,
			HistoricalMarketValueProvider historicalMarketValueProvider ,
			double crossoverRate , double mutationRate , double elitismRate ,
			int populationSizeForGeneticOptimizer ,
			int generationNumberForGeneticOptimizer ,
			int seedForRandomGenerator)
	    :
			base(numberOfPortfolioPositions, numberOfBestTestingPositionsToBeReturned,
    	benchmark ,	new DecoderForOTCPositionsWithWeights(),
    	genomeManagerType,
    	fitnessEvaluator , historicalMarketValueProvider ,
			crossoverRate , mutationRate , elitismRate ,
			populationSizeForGeneticOptimizer ,
			generationNumberForGeneticOptimizer ,
			seedForRandomGenerator )
		{
		}
    
		public override IGenomeManager GetGenomeManager(EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager)
		{
			return new GenomeManagerForWeightedOTC_EndOfDay(eligibleTickers, this.numberOfPortfolioPositions,
			                                        this.decoderForTestingPositions, this.fitnessEvaluator,
			                                        this.genomeManagerType , returnsManager,
			                                        this.seedForRandomGeneratorForTheGeneticOptimizer);
		}
	}
}
