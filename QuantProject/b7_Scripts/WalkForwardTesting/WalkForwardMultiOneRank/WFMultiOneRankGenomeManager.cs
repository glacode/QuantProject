/*
QuantProject - Quantitative Finance Library

WFMultiOneRankGenomeManager.cs
Copyright (C) 2003 
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
using System.Collections;
using System.Data;

using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.ADT.Statistics;
using QuantProject.Data.DataTables;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardMultiOneRank
{
	/// <summary>
	/// This class implements IGenomeManager, in order to find the
	/// best position group with respect to the one rank strategy
	/// </summary>
	public class WFMultiOneRankGenomeManager : IGenomeManager
	{
		private DataTable eligibleTickers;
		private DateTime firstQuoteDate;
		private DateTime lastQuoteDate;
		private double targetPerformance;
		private int numberOfTickersInPortfolio;

		private int numberOfEligibleTickers;
		private CandidateProperties[] setOfCandidates;

		public int GenomeSize
		{
			get { return this.numberOfTickersInPortfolio; }
		}
//		public int MinValueForGenes
//		{
//			get { return -this.numberOfEligibleTickers; }
//		}
//		public int MaxValueForGenes
//		{
//			get { return this.numberOfEligibleTickers - 1; }
//		}
//		public GeneticOptimizer CurrentGeneticOptimizer
//		{
//			get{ return this.currentGeneticOptimizer; }
//			set{ this.currentGeneticOptimizer = value; }
//		}

		public WFMultiOneRankGenomeManager(
			DataTable eligibleTickers ,
			DateTime firstQuoteDate,
			DateTime lastQuoteDate,
			int numberOfTickersInPortfolio,
			double targetPerformance )
		{
			this.eligibleTickers = eligibleTickers;
			this.numberOfEligibleTickers = eligibleTickers.Rows.Count;
			this.firstQuoteDate = firstQuoteDate;
			this.lastQuoteDate = lastQuoteDate;
			this.numberOfTickersInPortfolio = numberOfTickersInPortfolio;
			this.targetPerformance = targetPerformance;

			GenomeManagement.SetRandomGenerator(QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator);

			this.retrieveData();
		}
		#region retrieveData
		private float[] getArrayOfRatesOfReturn( string ticker )
		{
			float[] returnValue = null;
			Quotes tickerQuotes =
				new Quotes( ticker , this.firstQuoteDate , this.lastQuoteDate );
			float[] allAdjValues =
				QuantProject.Data.ExtendedDataTable.GetArrayOfFloatFromColumn(
				tickerQuotes , "quAdjustedClose");
			returnValue =	new float[ allAdjValues.Length ];
			int i = 0; //index for ratesOfReturns array
			for( int idx = 0 ; idx < allAdjValues.Length - 1 ; idx++ )
			{
				returnValue[ i ] = allAdjValues[ idx + 1 ] /	allAdjValues[ idx ] - 1;
				i++;
			}	
			return returnValue;
		}
		private void retrieveData()
		{
			this.setOfCandidates =
				new CandidateProperties[ this.eligibleTickers.Rows.Count ];
			for(int i = 0; i<this.eligibleTickers.Rows.Count; i++)
			{
				string ticker = (string)this.eligibleTickers.Rows[i][0];
				this.setOfCandidates[i] = new CandidateProperties( ticker,
					this.getArrayOfRatesOfReturn( ticker ) );
			}
		}
		#endregion

		public int GetMinValueForGenes( int genePosition )
		{
			return -this.numberOfEligibleTickers;
		}
		public int GetMaxValueForGenes( int genePosition )
		{
			return this.numberOfEligibleTickers - 1;
		}

		#region GetFitnessValue
		#region getFitnessValue_getLinearCombinationReturns
		private int getPortfolioRatesOfReturn_getRateOfTickerToBeAddedToTheArray_getPositionInArray(
			int geneValueForTickerIdx )
		{
			int position = geneValueForTickerIdx;
			if( geneValueForTickerIdx < 0 )
				position = Math.Abs(geneValueForTickerIdx + 1);
			return position;
		}
		private float getPortfolioRatesOfReturn_getRateOfTickerToBeAddedToTheArray(
			int tickerIdx,
			int arrayElementPosition )
		{
			//			bool longReturns = false;
			//			if( tickerIdx > 0 )
			//				//the tickerIdx points to a ticker for which long returns are to be examined
			//				longReturns = true;
			int position =
				this.getPortfolioRatesOfReturn_getRateOfTickerToBeAddedToTheArray_getPositionInArray(tickerIdx);
			//			this.setOfCandidates[position].LongRatesOfReturn = longReturns;
//			float[] arrayOfRatesOfReturn =
//				this.setOfCandidates[ position ].ArrayOfRatesOfReturn;
			float currentReturn = this.setOfCandidates[ position ].GetReturn(
				arrayElementPosition , tickerIdx > 0 );
			//the investment is assumed to be equally divided for each ticker
			return ( currentReturn / this.GenomeSize );
		}
    
		private double[] getFitnessValue_getLinearCombinationReturns( int[] tickersIdx )
		{
			double[] returnValue =
				new double[ this.setOfCandidates[ 0 ].ArrayOfRatesOfReturn.Length ];
			for( int i = 0; i < returnValue.Length ; i++ ) 
			{
				foreach( int tickerIdx in tickersIdx )
					returnValue[ i ] +=
						this.getPortfolioRatesOfReturn_getRateOfTickerToBeAddedToTheArray(
						tickerIdx , i );
			}
			return returnValue;
		}
		#endregion

		private double[] getFitnessValue_getStrategyReturnForCurrentCandidates(
			double[] linearCombinationReturns )
		{
			// strategyReturns contains one element less than linearCombinationReturns,
			// because there is no strategy for the very first period (at least
			// one day signal is needed)
			double[] strategyReturns = new double[ linearCombinationReturns.Length - 1 ];
			for ( int i = 0 ; i < linearCombinationReturns.Length - 1 ; i++ )
				if ( linearCombinationReturns[ i ] < 0 )
					// the current linear combination of tickers, at period i
					// has a negative return
					
					// go short tomorrow
					strategyReturns[ i ] = -linearCombinationReturns[ i + 1 ];
				else
					// the current linear combination of tickers, at period i
					// has a positive return
					
					// go long tomorrow
					strategyReturns[ i ] = linearCombinationReturns[ i + 1 ];
			return strategyReturns;
		}
		public double GetFitnessValue( Genome genome )
		{
			ICollection tickers =
				( ICollection )this.Decode( genome );
			double[] linearCombinationReturns =
				this.getFitnessValue_getLinearCombinationReturns(
				genome.Genes() );
			double[] strategyReturns =
				this.getFitnessValue_getStrategyReturnForCurrentCandidates(
				linearCombinationReturns );
			double fitnessValue =
				AdvancedFunctions.GetSharpeRatio(
				strategyReturns );
			return fitnessValue;
		}
		#endregion
		public Genome[] GetChilds( Genome parent1 , Genome parent2 )
		{
			return
				GenomeManipulator.MixGenesWithoutDuplicates(parent1, parent2);
		}

		public void Mutate( Genome genome )
		{
			// in this implementation only one gene is mutated
			// the new value has to be different from all the other genes of the genome
			int genePositionToBeMutated =
				GenomeManagement.RandomGenerator.Next( genome.Size ); 
			int newValueForGene = GenomeManagement.RandomGenerator.Next(
				genome.GetMinValueForGenes( genePositionToBeMutated ) ,
				genome.GetMaxValueForGenes( genePositionToBeMutated ) + 1 );
			while( GenomeManipulator.IsTickerContainedInGenome(
				newValueForGene ,	genome )  )
				// the portfolio, in this implementation, 
				// can't have a long position and a short position
				// for the same ticker
			{
				newValueForGene = GenomeManagement.RandomGenerator.Next(
					genome.GetMinValueForGenes( genePositionToBeMutated ) ,
					genome.GetMaxValueForGenes( genePositionToBeMutated ) + 1 );
			}
			GenomeManagement.MutateOneGene( genome ,
				genePositionToBeMutated , newValueForGene );
		}
		#region Decode
		private string decode_getTickerCodeForLongOrShortTrade(int geneValue)
		{
			string initialCharForTickerCode = "";
			int position = geneValue;
			if( geneValue < 0 )
			{
				position = Math.Abs( geneValue ) - 1;
				initialCharForTickerCode = "-";
			}  
			return initialCharForTickerCode +
				( string )this.eligibleTickers.Rows[ position ][ 0 ];
		}
		public virtual object Decode(Genome genome)
		{
    	
			string[] arrayOfTickers = new string[genome.Genes().Length];
			int indexOfTicker;
			for(int index = 0; index < genome.Genes().Length; index++)
			{
				indexOfTicker = (int)genome.Genes().GetValue(index);
				arrayOfTickers[index] = this.decode_getTickerCodeForLongOrShortTrade(indexOfTicker);
			}
//			MeaningForGenome meaning = new MeaningForGenome(arrayOfTickers,
//				this.PortfolioRatesOfReturn[this.portfolioRatesOfReturn.Length - 1],
//				this.RateOfReturn,
//				this.Variance);
			return arrayOfTickers;
      
		}
    #endregion
		public int GetNewGeneValue( Genome genome , int genePosition )
		{
			// in this implementation new gene values must be different from
			// the others already stored in the given genome
			int returnValue = GenomeManagement.RandomGenerator.Next(
				genome.GetMinValueForGenes( genePosition ) ,
				genome.GetMaxValueForGenes( genePosition ) + 1 );
			while(GenomeManipulator.IsTickerContainedInGenome(returnValue,
				genome) )
				//the portfolio can't have a long position and a short one for the same ticker
			{
				returnValue = GenomeManagement.RandomGenerator.Next(
					genome.GetMinValueForGenes( genePosition ) ,
					genome.GetMaxValueForGenes( genePosition ) + 1);
			}

			return returnValue;
		}

	}
}
