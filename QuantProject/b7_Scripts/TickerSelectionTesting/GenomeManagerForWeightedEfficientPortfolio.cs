/*
QuantProject - Quantitative Finance Library

GenomeManagerForWeightedEfficientPortfolio.cs
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
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;

namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	/// <summary>
	/// This is the base class implementing IGenomeManager, in order to find
	/// efficient portfolios in which tickers are weighted differently
	/// </summary>
	[Serializable]
	public abstract class GenomeManagerForWeightedEfficientPortfolio : GenomeManagerForEfficientPortfolio
	{
		private ReturnsManager returnsManager;
		
		public GenomeManagerForWeightedEfficientPortfolio(DataTable setOfInitialTickers,
		                                                  DateTime firstQuoteDate,
		                                                  DateTime lastQuoteDate,
		                                                  int numberOfTickersInPortfolio,
		                                                  double targetPerformance,
		                                                  PortfolioType portfolioType,
		                                                  string benchmark):base(setOfInitialTickers,
		                       firstQuoteDate,
		                       lastQuoteDate,
		                       numberOfTickersInPortfolio,
		                       targetPerformance,
		                       portfolioType, benchmark)
			
		{
			this.genomeSize = 2*this.genomeSize;
			//at even position the gene is used for finding
			//the coefficient for the ticker represented at the next odd position
			this.setReturnsManager(firstQuoteDate, lastQuoteDate);
		}
		
		protected abstract ReturnIntervals getReturnIntervals(
			DateTime firstDateTime,
			DateTime lastDateTime);

		private void setReturnsManager(DateTime firstQuoteDate,
		                               DateTime lastQuoteDate)
		{
			DateTime firstDateTime =
				HistoricalEndOfDayTimer.GetMarketOpen( firstQuoteDate );
//				new EndOfDayDateTime(firstQuoteDate, EndOfDaySpecificTime.MarketOpen);
			DateTime lastDateTime =
				HistoricalEndOfDayTimer.GetMarketClose( lastQuoteDate );
//				new EndOfDayDateTime(lastQuoteDate, EndOfDaySpecificTime.MarketClose);
			this.returnsManager =
				new ReturnsManager( this.getReturnIntervals(firstDateTime,
				                                            lastDateTime) ,
				                   new HistoricalAdjustedQuoteProvider() );
		}
		
		//this is a very generic implementation that will
		//be overriden by inherited classes specifying
		//the strategy and the type of returns
		protected override float[] getStrategyReturns()
		{
			return this.weightedPositionsFromGenome.GetReturns(this.returnsManager);
		}
		
		#region override Decode

		protected override double getTickerWeight(int[] genes, int tickerPositionInGenes)
		{
			double minimumWeight = (1.0 - ConstantsProvider.AmountOfVariableWeightToBeAssignedToTickers)/
				(genes.Length / 2);
			double totalOfValuesForWeightsInGenes = 0.0;
			for(int j = 0; j<genes.Length; j++)
			{
				if(j%2==0)
					//ticker weight is contained in genes at even position
					totalOfValuesForWeightsInGenes += Math.Abs(genes[j]) + 1.0;
				//0 has to be avoided !
			}
			double freeWeight = (Math.Abs(genes[tickerPositionInGenes-1]) + 1.0)/totalOfValuesForWeightsInGenes;
			return minimumWeight + freeWeight * (1.0 - minimumWeight * genes.Length / 2);
		}

		public override object Decode(Genome genome)
		{
			string[] arrayOfTickers = new string[genome.Genes().Length/2];
			double[] arrayOfTickersWeights = new double[genome.Genes().Length/2];
			int indexOfTicker;
			int i = 0;//for the arrayOfTickers
			for(int index = 0; index < genome.Genes().Length; index++)
			{
				if(index%2==1)
					//indexForTicker is contained in genes at odd position
				{
					indexOfTicker = (int)genome.Genes().GetValue(index);
					arrayOfTickers[i] = this.decode_getTickerCodeForLongOrShortTrade(indexOfTicker);
					arrayOfTickersWeights[i] = this.getTickerWeight(genome.Genes(), index);
					i++;
				}
			}
			GenomeMeaning meaning = new GenomeMeaning(arrayOfTickers,
			                                          arrayOfTickersWeights);
			return meaning;
		}
		#endregion

		public override int GetNewGeneValue(Genome genome, int genePosition)
		{
			// in this implementation only new gene values pointing to tickers
			// must be different from the others already stored (in odd positions of genome)
			int returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
			                                                        genome.GetMaxValueForGenes(genePosition) + 1);
			while(genePosition%2 == 1
			      && GenomeManipulator.IsTickerContainedInGenome(returnValue,genome))
				//while in the given position has to be stored
				//a new gene pointing to a ticker and
				//the proposed returnValue points to a ticker
				//already stored in the given genome
			{
				// a new returnValue has to be generated
				returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
				                                                    genome.GetMaxValueForGenes(genePosition) + 1);
			}
			return returnValue;
		}
		
		#region override Mutate
		
		//OLD VERSION
		//    public override void Mutate(Genome genome, double mutationRate)
		//    {
		//      // in this implementation only one gene is mutated
		//      int genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
		//      int newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
		//                                                                  genome.GetMaxValueForGenes(genePositionToBeMutated) +1);
//
		//      while(genePositionToBeMutated%2 == 1 &&
		//            GenomeManipulator.IsTickerContainedInGenome(newValueForGene,genome))
		//      //while in the proposed genePositionToBeMutated has to be stored
		//      //a new gene pointing to a ticker and
		//      //the proposed newValueForGene points to a ticker
		//      //already stored in the given genome
		//      {
		//        newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
		//                                                                genome.GetMaxValueForGenes(genePositionToBeMutated) +1);
		//      }
		//      GenomeManagement.MutateOneGene(genome, mutationRate,
		//        genePositionToBeMutated, newValueForGene);
		//    }
		
		private int mutate_MutateOnlyOneWeight_getNewWeight(Genome genome, int genePositionToBeMutated)
		{
			int returnValue;
			double partOfGeneToSubtractOrAdd = 0.03;
			int geneValue = genome.GetGeneValue(genePositionToBeMutated);
			int subtractOrAdd = GenomeManagement.RandomGenerator.Next(2);
			if(subtractOrAdd == 1)//subtract a part of the gene value from the gene value itself
			{
				if( geneValue < 0 )
					returnValue = Math.Max( geneValue - Convert.ToInt32(partOfGeneToSubtractOrAdd*Math.Abs(geneValue)),
					                       genome.GetMinValueForGenes(genePositionToBeMutated) );
				else // geneValue >= 0
					returnValue = geneValue - Convert.ToInt32(partOfGeneToSubtractOrAdd*Math.Abs(geneValue));
			}
			else//add a part of the gene value to the gene value itself
			{
				if( geneValue < 0 )
					returnValue = geneValue + Convert.ToInt32(partOfGeneToSubtractOrAdd*Math.Abs(geneValue));
				else // geneValue >= 0
					returnValue = Math.Min(genome.GetMaxValueForGenes(genePositionToBeMutated),
					                       geneValue + Convert.ToInt32(partOfGeneToSubtractOrAdd*geneValue));
			}
			return returnValue;
		}

		private void mutate_MutateOnlyOneWeight(Genome genome)
		{
			int genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
			while(genePositionToBeMutated%2 == 1 )
				//while the proposed genePositionToBeMutated points to a ticker
				genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
			
			int newValueForGene = this.mutate_MutateOnlyOneWeight_getNewWeight(genome, genePositionToBeMutated);
			
			GenomeManagement.MutateOneGene(genome, genePositionToBeMutated, newValueForGene);
		}
		
		private void mutate_MutateAllGenes(Genome genome)
		{
			for(int genePositionToBeMutated = 0;
			    genePositionToBeMutated < genome.Genes().Length;
			    genePositionToBeMutated ++)
			{
				int newValueForGene =
					GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
					                                      genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);
				while(genePositionToBeMutated%2 == 1
				      && GenomeManipulator.IsTickerContainedInGenome(newValueForGene,genome))
					//while in the given position has to be stored
					//a new gene pointing to a ticker and
					//the proposed newValueForGene points to a ticker
					//already stored in the given genome
					// a new newalueForGene has to be generated
					newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
					                                                        genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);
				
				genome.SetGeneValue(newValueForGene, genePositionToBeMutated);
			}
		}
		
		//new version
		//mutation means just a change in one single weight
		//or in a complete new genome (with a probability of 50%)
		public override void Mutate(Genome genome)
		{
			int mutateOnlyOneWeight = GenomeManagement.RandomGenerator.Next(2);
			if(mutateOnlyOneWeight == 1)
				this.mutate_MutateOnlyOneWeight(genome);
			else//mutate all genome's genes
				this.mutate_MutateAllGenes(genome);
		}

		#endregion

	}
}
