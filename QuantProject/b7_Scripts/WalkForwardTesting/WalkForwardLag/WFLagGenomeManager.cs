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

		private WFLagCandidates wFLagCandidates;

		public int GenomeSize
		{
			get
			{
				return this.numberOfDrivingPositions + this.numberOfTickersInPortfolio;
			}
		}
//		public GeneticOptimizer CurrentGeneticOptimizer
//		{
//			get{ return this.currentGeneticOptimizer; }
//			set{ this.currentGeneticOptimizer = value; }
//		}

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

			GenomeManagement.SetRandomGenerator(
				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator +
				firstQuoteDate.Date.DayOfYear );

			this.wFLagCandidates = new WFLagCandidates( this.eligibleTickers ,
				this.firstQuoteDate , this.lastQuoteDate );
		}

		public int GetMinValueForGenes( int genePosition )
		{
			return -this.numberOfEligibleTickers;
		}
		public int GetMaxValueForGenes( int genePosition )
		{
			return this.numberOfEligibleTickers - 1;
		}

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
		private ArrayList getEnumeratedSignedTickers(
			ICollection signedTickers )
		{
			ArrayList enumeratedSignedTickers = new ArrayList();
			foreach ( string signedTicker in signedTickers )
				enumeratedSignedTickers.Add( signedTicker );
			return enumeratedSignedTickers;
		}
		private ArrayList getTickers( ICollection signedTickers )
		{
			ArrayList tickers = new ArrayList();
			foreach ( string signedTicker in signedTickers )
				tickers.Add( WFLagGenomeManager.GetTicker( signedTicker ) );
			return tickers;
		}
		private float[] getMultipliers( ArrayList signedTickers )
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
			ArrayList enumeratedSignedTicker =
				this.getEnumeratedSignedTickers( signedTickers );
			int numberOfSignedTickers = enumeratedSignedTicker.Count;
			ArrayList tickers = this.getTickers( enumeratedSignedTicker );
			float[] multipliers = this.getMultipliers( enumeratedSignedTicker );
			// arrays of close to close returns, one for each signed ticker
			float[][] tickersReturns =
				this.wFLagCandidates.GetTickersReturns( tickers );
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
		private double[] getFinalReturns( double[] strategyReturns ,
			int finalLength )
		{
			double[] finalReturns = new double[ finalLength ];
			for ( int i = strategyReturns.Length - finalLength ;
				i < strategyReturns.Length ; i++ )
				finalReturns[ i - ( strategyReturns.Length - finalLength ) ] =
					strategyReturns[ i ];
			return finalReturns;
		}
		private double getFitnessValue_sharpeRatio(
			double[] returns )
		{
			double fitnessValue =
				AdvancedFunctions.GetSharpeRatio(
				returns );
			return fitnessValue;
		}
		private double getFitnessValue_withGoodFinal(
			double[] strategyReturns )
		{
			double[] secondHalfStrategyReturns =
				this.getFinalReturns( strategyReturns ,
				strategyReturns.Length/2 );
			double[] fourthQuorterStrategyReturns =
				this.getFinalReturns( strategyReturns ,
				strategyReturns.Length/4 );
			double fitnessValue =
				this.getFitnessValue_sharpeRatio( strategyReturns ) *
				this.getFitnessValue_sharpeRatio( secondHalfStrategyReturns ) *
				this.getFitnessValue_sharpeRatio( fourthQuorterStrategyReturns );
			return fitnessValue;
		}

		private double getFitnessValue( double[] strategyReturns )
		{
			double fitnessValue =
				AdvancedFunctions.GetSharpeRatio(
				strategyReturns );
//			double fitnessValue =
//				AdvancedFunctions.GetExpectancyScore(
//				strategyReturns );
			//			double fitnessValue =
//				this.getFitnessValue_withGoodFinal( strategyReturns );
			//			double fitnessValue =
			//				BasicFunctions.GetSimpleAverage( strategyReturns ) /
			//				( Math.Pow( BasicFunctions.GetStdDev( strategyReturns ) , 1.3 ) );
			return fitnessValue;
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
			double fitnessValue = this.getFitnessValue( strategyReturns );
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
				GenomeManagement.RandomGenerator.Next(
				genome.GetMinValueForGenes( i ) ,
				genome.GetMaxValueForGenes( i ) + 1);
			while( GenomeManipulator.IsTickerContainedInGenome(returnValue,
				genome) )
				// the portfolio can't have a long position and a
				// short one for the same ticker
			{
				returnValue = GenomeManagement.RandomGenerator.Next(
					genome.GetMinValueForGenes( i ) ,
					genome.GetMaxValueForGenes( i ) + 1 );
			}
			return returnValue;
		}

	}
}
