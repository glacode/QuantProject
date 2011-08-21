/*
QuantProject - Quantitative Finance Library

GenomeManagerForDrivenByFVProvider.cs
Copyright (C) 2010 
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
using System.Data;
using System.Collections;

using QuantProject.ADT;
using QuantProject.ADT.Statistics;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Data;
using QuantProject.Data.DataTables;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.TickersRelationships;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.GenomeManagers;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TickerSelectionTesting.DrivenByFundamentals.DrivenByFairValueProvider.InSampleChoosers.Genetic
{
	/// <summary>
	/// Implements what we need to use the Genetic Optimizer
	/// for finding the portfolio that fits 
	/// the strategy driven by a particular IFairValueProvider object
	/// The optimazion that uses this genomeManager just seeks
	/// the portfolio with the highest evaluation provided
	/// by the given IFitnessEvaluator, just using past returns OR
	/// the portfolio with the highest level of a combined fitness
	/// (evaluation of past returns provided by the given IFitnessEvaluator
	/// mixed with an evaluation based on fundamentals)
	/// </summary>
	[Serializable]
  public class GenomeManagerForDrivenByFVProvider : BasicGenomeManager
  {
  	private HistoricalMarketValueProvider historicalMarketValueProvider;
  	private bool mixPastReturnsEvaluationWithFundamentalEvaluation;
  	private Timer timer;
  	
    public GenomeManagerForDrivenByFVProvider(EligibleTickers eligibleTickers,
                           int numberOfTickersInPortfolio,
                           IDecoderForTestingPositions decoderForTestingPositions,
                           IFitnessEvaluator fitnessEvaluator,
                           GenomeManagerType genomeManagerType,
													 ReturnsManager returnsManager,
													 HistoricalMarketValueProvider historicalMarketValueProvider,
													 Timer timer,
													 bool mixPastReturnsEvaluationWithFundamentalEvaluation,
													 int seedForRandomGenerator)
                           :
														base(eligibleTickers,
														numberOfTickersInPortfolio,
														decoderForTestingPositions,
														fitnessEvaluator,		
														genomeManagerType,
														returnsManager,
														seedForRandomGenerator)
                                
                          
    {
  		this.historicalMarketValueProvider =
  			historicalMarketValueProvider;
  		this.timer = timer;
  		this.mixPastReturnsEvaluationWithFundamentalEvaluation = 
  			mixPastReturnsEvaluationWithFundamentalEvaluation;
			//the range is the same as if it was OnlyLong:
			//the sign of tickers depend on the fair value level
			//with respect to the market price
  		this.minValueForGenes = 0;
			this.maxValueForGenes = this.eligibleTickers.Count - 1;
    }
  	
  	#region getFitnessByFundamentals
  	
  	private double getFitnessValue_getFundamentalFitness_getBuyPrice(WeightedPosition position)
		{
			double returnValue;
			object[] keys = new object[1];
			keys[0] = position.Ticker;
			DataRow foundRow = 
				this.eligibleTickers.SourceDataTable.Rows.Find(keys);
			returnValue = (double)foundRow["AverageMarketPrice"];
			
			return returnValue;
		}
		
		private double getFitnessValue_getFundamentalFitness_getFairPrice(WeightedPosition position)
		{
			double returnValue;
			object[] keys = new object[1];
			keys[0] = position.Ticker;
			DataRow foundRow = 
				this.eligibleTickers.SourceDataTable.Rows.Find(keys);
			returnValue = (double)foundRow["FairPrice"];
			
			return returnValue;
		}
		  	
  	private double getFitnessValue_getFundamentalFitness(Genome genome)
		{
  		double theoreticalProfit = 0.0;
  		double buyPriceOfCurrentPosition, fairPriceOfCurrentPosition,
  					 weightOfCurrentPosition;
  		WeightedPositions weightedPositionFromGenome =
  			((TestingPositions)genome.Meaning).WeightedPositions;
			foreach( WeightedPosition position in weightedPositionFromGenome )
			{
				buyPriceOfCurrentPosition = getFitnessValue_getFundamentalFitness_getBuyPrice(position);
				fairPriceOfCurrentPosition = getFitnessValue_getFundamentalFitness_getFairPrice(position);
				weightOfCurrentPosition = position.Weight;
				theoreticalProfit +=
					weightOfCurrentPosition*(fairPriceOfCurrentPosition-buyPriceOfCurrentPosition)/
					buyPriceOfCurrentPosition;
			}
  		return theoreticalProfit;
  	}
  	#endregion getFitnessByFundamentals
  	
		public override double GetFitnessValue(Genome genome)
		{
			double fitnessValue = double.MinValue;
			double fitnessValueFromPastReturns =
					this.fitnessEvaluator.GetFitnessValue(genome.Meaning, this.returnsManager);
			double fitnessGivenByFundamentals = 1.0;
			if( this.mixPastReturnsEvaluationWithFundamentalEvaluation )
				fitnessGivenByFundamentals = 
					this.getFitnessValue_getFundamentalFitness(genome);
			fitnessValue = 
				fitnessValueFromPastReturns * fitnessGivenByFundamentals;
			
			return fitnessValue;
		}
  	
    public override Genome[] GetChilds(Genome parent1, Genome parent2)
    {
    	return
      	GenomeManipulator.MixGenesWithoutDuplicates(parent1, parent2);
    }
		
    public override int GetNewGeneValue(Genome genome, int genePosition)
    {
     // in this implementation new gene values must be different from
      // the others already stored in the given genome
      int returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
                           genome.GetMaxValueForGenes(genePosition) + 1);
      while( GenomeManipulator.IsTickerContainedInGenome(returnValue,genome) )
      //the portfolio can't have a long position and a short one for the same ticker 
      {
        returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
                           genome.GetMaxValueForGenes(genePosition) + 1);
      }
      return returnValue;
    }
	  
    public override void Mutate(Genome genome)
    {
      // in this implementation only one gene is mutated
      // the new value has to be different from all the other genes of the genome
      int genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
      int newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
                            genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);
     	while( GenomeManipulator.IsTickerContainedInGenome(newValueForGene,genome) )
			//the portfolio can't have a long position and a short one for the same ticker
      {
        newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
                            genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);
      }
      GenomeManagement.MutateOneGene(genome, genePositionToBeMutated, newValueForGene);
    }
  }
}
