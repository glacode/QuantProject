/*
QuantProject - Quantitative Finance Library

WFLagGenomeManager.cs
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

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// This class implements IGenomeManager, in order to find the
	/// best driving position group and the best
	/// portfolio position group with respect to the lag strategy
	/// </summary>
	public class WFLagGenomeManager : IGenomeManager
	{
		private DataTable eligibleTickers;
		private DateTime firstQuoteDate;
		private DateTime lastQuoteDate;
		private int numberOfDrivingPositions;
		private int numberOfTickersInPortfolio;

		private int numberOfEligibleTickers;

		private GeneticOptimizer currentGeneticOptimizer;

		private WFLagCandidates wFLagCandidates;

		public int GenomeSize
		{
			get
			{
				return this.numberOfDrivingPositions + this.numberOfTickersInPortfolio;
			}
		}
		public int MinValueForGenes
		{
			get { return -this.numberOfEligibleTickers; }
		}
		public int MaxValueForGenes
		{
			get { return this.numberOfEligibleTickers - 1; }
		}
		public GeneticOptimizer CurrentGeneticOptimizer
		{
			get{ return this.currentGeneticOptimizer; }
			set{ this.currentGeneticOptimizer = value; }
		}

		public WFLagGenomeManager(
			DataTable eligibleTickers ,
			DateTime firstQuoteDate ,
			DateTime lastQuoteDate ,
			int numberOfDrivingPositions ,
			int numberOfTickersInPortfolio )
		{
			this.eligibleTickers = eligibleTickers;
			this.firstQuoteDate = firstQuoteDate;
			this.lastQuoteDate = lastQuoteDate;
			this.numberOfDrivingPositions = numberOfDrivingPositions;
			this.numberOfTickersInPortfolio = numberOfTickersInPortfolio;

			this.numberOfEligibleTickers = eligibleTickers.Rows.Count;

			GenomeManagement.SetRandomGenerator(QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator);

			this.wFLagCandidates = new WFLagCandidates( this.eligibleTickers ,
				this.firstQuoteDate , this.lastQuoteDate );

//			this.retrieveData();
		}

//		#region retrieveData
//		private float[] getArrayOfRatesOfReturn( string ticker )
//		{
//			float[] returnValue = null;
//			Quotes tickerQuotes =
//				new Quotes( ticker , this.firstQuoteDate , this.lastQuoteDate );
//			float[] allAdjValues =
//				QuantProject.Data.ExtendedDataTable.GetArrayOfFloatFromColumn(
//				tickerQuotes , "quAdjustedClose");
//			returnValue =	new float[ allAdjValues.Length ];
//			int i = 0; //index for ratesOfReturns array
//			for( int idx = 0 ; idx < allAdjValues.Length - 1 ; idx++ )
//			{
//				returnValue[ i ] = allAdjValues[ idx + 1 ] /	allAdjValues[ idx ] - 1;
//				i++;
//			}	
//			return returnValue;
//		}
//		private void retrieveData()
//		{
//			this.closeToCloseReturns =
//				new double[ this.eligibleTickers.Rows.Count ];
//			for(int i = 0; i<this.eligibleTickers.Rows.Count; i++)
//			{
//				string ticker = (string)this.eligibleTickers.Rows[i][0];
//				this.closeToCloseReturns[i] = new WFLagCandidate( ticker,
//					this.getArrayOfRatesOfReturn( ticker ) );
////				this.closeToCloseReturns[i] = new CandidateProperties( ticker,
////					this.getArrayOfRatesOfReturn( ticker ) );
//			}
//		}
//		#endregion

		public static string GetTicker( string signedTicker )
		{
			string returnValue;
			if ( signedTicker.IndexOf( "-" ) == 0 )
				returnValue = signedTicker.Substring( 1 , signedTicker.Length - 1 );
			else
				returnValue = signedTicker;
			return returnValue;
		}

		#region GetFitnessValue
		#region getFitnessValue_getLinearCombinationReturns
		private Hashtable getTickers( ICollection signedTickers )
		{
			Hashtable tickers = new Hashtable();
			foreach ( string signedTicker in signedTickers )
				tickers.Add( WFLagGenomeManager.GetTicker( signedTicker ) , null );
			return tickers;
		}
		private float[] getMultipliers( ICollection signedTickers )
		{
			float[] multipliers = new float[ signedTickers.Count ];
			int i = 0;
			foreach ( string signedTicker in signedTickers )
			{
				float multiplier = 1F;
				if ( signedTicker.IndexOf( "-" ) == 0 )
					multiplier = -1F;
				multipliers[ i ] = multiplier;
				i++;
			}
			return multipliers;
		}
		private double[] getFitnessValue_getLinearCombinationReturns(
			ICollection signedTickers )
		{
			int numberOfSignedTickers = signedTickers.Count;
			Hashtable tickers = this.getTickers( signedTickers );
			float[] multipliers = this.getMultipliers( signedTickers );
			// arrays of close to close returns, one for each signed ticker
			float[][] tickersReturns =
				this.wFLagCandidates.GetTickersReturns( tickers.Keys );
			double[] linearCombinationReturns =
				new double[ tickersReturns[ 0 ].Length ];
			for( int i = 0; i < linearCombinationReturns.Length ; i++ )
				// computes linearCombinationReturns[ i ]
			{
				linearCombinationReturns[ i ] = 0;
				for ( int j=0 ; j < numberOfSignedTickers ; j++ )
				{
					double signedTickerReturn =
						tickersReturns[ j ][ i ] * multipliers[ j ];
					// the investment is assumed to be equally divided for each
					// signed ticker
					linearCombinationReturns[ i ] += signedTickerReturn /
						numberOfSignedTickers;
				}
			}
			return linearCombinationReturns;
		}
		#endregion
		private double[] getFitnessValue_getStrategyReturn(
			double[] drivingPositionsReturns , double[] portfolioPositionsReturns )
		{
			// strategyReturns contains one element less than drivingPositionsReturns,
			// because there is no strategy for the very first period (at least
			// one day signal is needed)
			double[] strategyReturns = new double[ portfolioPositionsReturns.Length - 1 ];
			for ( int i = 0 ; i < portfolioPositionsReturns.Length - 1 ; i++ )
				if ( drivingPositionsReturns[ i ] < 0 )
					// the current linear combination of tickers, at period i
					// has a negative return
					
					// go short tomorrow
					strategyReturns[ i ] = -portfolioPositionsReturns[ i + 1 ];
				else
					// the current linear combination of tickers, at period i
					// has a positive return
					
					// go long tomorrow
					strategyReturns[ i ] = portfolioPositionsReturns[ i + 1 ];
			return strategyReturns;

		}
		private double getFitnessValue( WFLagSignedTickers wFLagSignedTickers )
		{
			double[] drivingPositionsReturns =
				this.getFitnessValue_getLinearCombinationReturns(
				wFLagSignedTickers.DrivingPositions.Keys );
			double[] portfolioPositionsReturns =
				this.getFitnessValue_getLinearCombinationReturns(
				wFLagSignedTickers.PortfolioPositions.Keys );
			double[] strategyReturns =
				this.getFitnessValue_getStrategyReturn(
				drivingPositionsReturns , portfolioPositionsReturns );
			double fitnessValue =
				AdvancedFunctions.GetSharpeRatio(
				strategyReturns );
			return fitnessValue;
		}
		public double GetFitnessValue( Genome genome )
		{
			WFLagSignedTickers wFLagSignedTickers =
				( WFLagSignedTickers )this.Decode( genome );
			return this.getFitnessValue( wFLagSignedTickers );
		}
		#endregion
		public Genome[] GetChilds( Genome parent1 , Genome parent2 )
		{
			return
				GenomeManipulator.MixGenesWithoutDuplicates(parent1, parent2);
		}
		public void Mutate( Genome genome , double mutationRate )
		{
			// in this implementation only one gene is mutated
			// the new value has to be different from all the other genes of the genome
			int newValueForGene = GenomeManagement.RandomGenerator.Next(
				genome.MinValueForGenes ,
				genome.MaxValueForGenes + 1 );
			int genePositionToBeMutated =
				GenomeManagement.RandomGenerator.Next( genome.Size ); 
			while( GenomeManipulator.IsTickerContainedInGenome(
				newValueForGene ,	genome )  )
				// the portfolio, in this implementation, 
				// can't have a long position and a short position
				// for the same ticker
			{
				newValueForGene = GenomeManagement.RandomGenerator.Next(
					genome.MinValueForGenes ,
					genome.MaxValueForGenes + 1 );
			}
			GenomeManagement.MutateOneGene( genome , mutationRate ,
				genePositionToBeMutated , newValueForGene );
		}
		#region Decode
		private string decode_getSignedTicker( int geneValue )
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
		private void decode_addDrivingPosition(
			Genome genome , int geneIndex ,	WFLagSignedTickers wFLagSignedTickers )
		{
			int indexOfTicker = (int)genome.Genes().GetValue( geneIndex );
			string signedTicker = this.decode_getSignedTicker( indexOfTicker );
			wFLagSignedTickers.DrivingPositions.Add( signedTicker , null );
		}
		private void decode_addPortfolioPosition(
			Genome genome , int geneIndex ,	WFLagSignedTickers wFLagSignedTickers )
		{
			int indexOfTicker = (int)genome.Genes().GetValue( geneIndex );
			string signedTicker = this.decode_getSignedTicker( indexOfTicker );
			wFLagSignedTickers.PortfolioPositions.Add( signedTicker , null );
		}
		private string decode_getSignedTickerForGeneValue( int geneValue )
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
			WFLagSignedTickers wFLagSignedTickers = new WFLagSignedTickers();
			for ( int geneIndex = 0 ; geneIndex < this.numberOfDrivingPositions ;
				geneIndex ++ )
				this.decode_addDrivingPosition( genome , geneIndex , wFLagSignedTickers );
			for ( int geneIndex = this.numberOfDrivingPositions ;
				geneIndex < this.numberOfDrivingPositions + this.numberOfTickersInPortfolio ;
				geneIndex ++ )
				this.decode_addPortfolioPosition( genome , geneIndex , wFLagSignedTickers );
			string[] arrayOfTickers = new string[genome.Genes().Length];
			int indexOfTicker;
			for(int index = 0; index < genome.Genes().Length; index++)
			{
				indexOfTicker = (int)genome.Genes().GetValue(index);
				arrayOfTickers[index] =
					this.decode_getSignedTickerForGeneValue(indexOfTicker);
			}
			return wFLagSignedTickers;
		}
		#endregion
		public int GetNewGeneValue( Genome genome , int i )
		{
			// in this implementation new gene values must be different from
			// the others already stored in the given genome
			int returnValue =
				GenomeManagement.RandomGenerator.Next(genome.MinValueForGenes,
				genome.MaxValueForGenes + 1);
			while( GenomeManipulator.IsTickerContainedInGenome(returnValue,
				genome) )
				// the portfolio can't have a long position and a
				// short one for the same ticker
			{
				returnValue = GenomeManagement.RandomGenerator.Next(genome.MinValueForGenes,
					genome.MaxValueForGenes + 1);
			}
			return returnValue;
		}

	}
}
