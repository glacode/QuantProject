/*
QuantProject - Quantitative Finance Library

PVOGeneticChooser.cs
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


namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.InSampleChoosers
{
	/// <summary>
	/// In sample genetic analyzer for the Portfolio Value Oscillator 
	/// </summary>
	[Serializable]
	public class PVOGeneticChooser : GeneticChooser
	{
		protected int numDaysForOscillatingPeriod;
		protected int minLevelForOversoldThreshold;
    protected int maxLevelForOversoldThreshold;
    protected int minLevelForOverboughtThreshold;
    protected int maxLevelForOverboughtThreshold;
    protected int divisorForThresholdComputation;
    protected bool symmetricalThresholds;
    protected bool overboughtMoreThanOversoldForFixedPortfolio;
    		
    public PVOGeneticChooser(
    	int numDaysForOscillatingPeriod,
			int numberOfPortfolioPositions , int numberOfBestTestingPositionsToBeReturned ,
			Benchmark benchmark ,
			IDecoderForTestingPositions decoderForTestingPositions ,
			IFitnessEvaluator fitnessEvaluator ,
			HistoricalMarketValueProvider historicalMarketValueProvider ,
			double crossoverRate , double mutationRate , double elitismRate ,
			int populationSizeForGeneticOptimizer ,
			int generationNumberForGeneticOptimizer ,
			int seedForRandomGenerator, 
		  int minLevelForOversoldThreshold , int maxLevelForOversoldThreshold,
	    int minLevelForOverboughtThreshold , int maxLevelForOverboughtThreshold,
	    int divisorForThresholdComputation,
	    bool symmetricalThresholds , bool overboughtMoreThanOversoldForFixedPortfolio)
	    :
			base(numberOfPortfolioPositions, numberOfBestTestingPositionsToBeReturned,
			benchmark ,	decoderForTestingPositions ,
			fitnessEvaluator , historicalMarketValueProvider ,
			crossoverRate , mutationRate , elitismRate ,
			populationSizeForGeneticOptimizer ,
			generationNumberForGeneticOptimizer ,
			seedForRandomGenerator )
		{
			this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
			this.minLevelForOversoldThreshold = minLevelForOversoldThreshold;
			this.maxLevelForOversoldThreshold = maxLevelForOversoldThreshold;
			this.minLevelForOverboughtThreshold = minLevelForOverboughtThreshold;
			this.maxLevelForOverboughtThreshold = maxLevelForOverboughtThreshold;
			this.divisorForThresholdComputation = divisorForThresholdComputation;
			this.symmetricalThresholds = symmetricalThresholds;
			this.overboughtMoreThanOversoldForFixedPortfolio = overboughtMoreThanOversoldForFixedPortfolio;
		}

		protected override string getHashCodeForGenome(QuantProject.ADT.Optimizing.Genetic.Genome genome)
		{
			string returnValue = ((TestingPositions)genome.Meaning).HashCodeForTickerComposition;
			return returnValue;
		}
    
		protected override IGenomeManager getGenomeManager(EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager)
		{
			return  
				new PVOGenomeManager(eligibleTickers, this.numberOfPortfolioPositions,
														 this.numDaysForOscillatingPeriod ,
				                     this.minLevelForOversoldThreshold, this.maxLevelForOversoldThreshold,
				                     this.minLevelForOverboughtThreshold, this.maxLevelForOverboughtThreshold,
				                     this.divisorForThresholdComputation, this.decoderForTestingPositions,
				                     this.fitnessEvaluator, this.symmetricalThresholds,
				                     this.overboughtMoreThanOversoldForFixedPortfolio,
				                     GenomeManagerType.ShortAndLong, returnsManager,
														 this.seedForRandomGenerator);
		}

	}
}

