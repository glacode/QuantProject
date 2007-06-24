/*
QuantProject - Quantitative Finance Library

WFLagGeneticWithEqualWeights.cs
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
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.EquityEvaluation;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// This class implements IGenomeManager, in order to find the
	/// best driving position group and the best
	/// portfolio position group with respect to the lag strategy.
	/// It uses equal weights both within the driving positions and within the
	/// portfolio positions.
	/// </summary>
	public class WFLagGeneticWithEqualWeights : IGenomeManager
	{
		private int numberOfDrivingPositions;
		private int numberOfPortfolioPositions;
		private int numberOfEligibleTickersForDrivingWeightedPositions;
		protected DataTable eligibleTickersForDrivingWeightedPositions;
		protected DataTable eligibleTickersForPortfolioWeightedPositions;
		private DateTime firstOptimizationDate;
		private DateTime lastOptimizationDate;

		private double minimumPositionWeight;

		protected WFLagCandidates wFLagCandidates;

		private IEquityEvaluator equityEvaluator;

		private WFLagMeaningForUndecodableGenomes wFLagMeaningForUndecodableGenomes;


		public int GenomeSize
		{
			get
			{
				return ( this.numberOfDrivingPositions + this.numberOfPortfolioPositions );
			}
		}


		/// <summary>
		/// This class implements IGenomeManager, in order to find the
		/// best driving position group and the best
		/// portfolio position group with respect to the lag strategy.
		/// Weighted positions are used for both the driving positions
		/// and the portfolio positions
		/// </summary>
		/// <param name="eligibleTickersForDrivingWeightedPositions">weighted positions
		/// for driving positions will be chosen among these tickers</param>
		/// <param name="eligibleTickersForPortfolioWeightedPositions">weighted positions
		/// for portfolio positions will be chosen among these tickers</param>
		/// <param name="firstOptimizationDate"></param>
		/// <param name="lastQuoteDate"></param>
		/// <param name="numberOfDrivingPositions"></param>
		/// <param name="numberOfTickersInPortfolio"></param>
		public WFLagGeneticWithEqualWeights(
			DataTable eligibleTickersForDrivingWeightedPositions ,
			DataTable eligibleTickersForPortfolioWeightedPositions ,
			DateTime firstOptimizationDate ,
			DateTime lastOptimizationDate ,
			int numberOfDrivingPositions ,
			int numberOfPortfolioPositions ,
			IEquityEvaluator equityEvaluator ,
			int seedForRandomGenerator )

		{
			this.numberOfDrivingPositions = numberOfDrivingPositions;
			this.numberOfPortfolioPositions = numberOfPortfolioPositions;
			this.numberOfEligibleTickersForDrivingWeightedPositions =
				eligibleTickersForDrivingWeightedPositions.Rows.Count;
			this.eligibleTickersForDrivingWeightedPositions =
				eligibleTickersForDrivingWeightedPositions;
			this.eligibleTickersForPortfolioWeightedPositions =
				eligibleTickersForPortfolioWeightedPositions;
			this.firstOptimizationDate = firstOptimizationDate;
			this.lastOptimizationDate = lastOptimizationDate;

			this.minimumPositionWeight = 0.2;	// TO DO this value should become a constructor parameter
			
			this.equityEvaluator = equityEvaluator;

//			GenomeManagement.SetRandomGenerator(
//				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator
//				+ this.firstOptimizationDate.DayOfYear );
//			GenomeManagement.SetRandomGenerator(
//				11 );
			GenomeManagement.SetRandomGenerator( seedForRandomGenerator );

			this.wFLagCandidates = new WFLagCandidates(
				this.eligibleTickersForDrivingWeightedPositions ,
				this.firstOptimizationDate , this.lastOptimizationDate );

			this.wFLagMeaningForUndecodableGenomes =
				new WFLagMeaningForUndecodableGenomes();
		}
		public int GetMinValueForGenes( int genePosition )
		{
			int minValueForGene =
				-this.numberOfEligibleTickersForDrivingWeightedPositions;
			return minValueForGene;
		}
		public int GetMaxValueForGenes( int genePosition )
		{
			return this.numberOfEligibleTickersForDrivingWeightedPositions - 1;
		}

		#region GetFitnessValue
		private string[] getTickers( WeightedPositions weightedPositions )
		{
			string[] tickers = new string[ weightedPositions.Count ];
			for ( int i = 0 ; i < weightedPositions.Count ; i++ )
			{
				WeightedPosition weightedPosition = weightedPositions.GetWeightedPosition( i );
				tickers[ i ] = weightedPosition.Ticker;
			}
			return tickers;
		}
		private float[] getMultipliers( WeightedPositions weightedPositions )
		{
			float[] multipliers = new float[ weightedPositions.Count ];
			for ( int i = 0 ; i < weightedPositions.Count ; i++ )
			{
				WeightedPosition weightedPosition = weightedPositions.GetWeightedPosition( i );
				multipliers[ i ] = Convert.ToSingle( weightedPosition.Weight );
			}
			return multipliers;
		}

		private double[] getFitnessValue_getLinearCombinationReturns(
			WeightedPositions weightedPositions )
		{
//			ArrayList enumeratedweightedPositions =
//				this.getEnumeratedWeightedPositions( weightedPositions );
			int numberOfWeightedPositions = weightedPositions.Count;
			string[] tickers = this.getTickers( weightedPositions );
			float[] multipliers = this.getMultipliers( weightedPositions );
			// arrays of close to close returns, one for each signed ticker
			float[][] tickersReturns =
				this.wFLagCandidates.GetTickersReturns( tickers );
			double[] linearCombinationReturns =
				new double[ tickersReturns[ 0 ].Length ];
			for( int i = 0; i < linearCombinationReturns.Length ; i++ )
				// computes linearCombinationReturns[ i ]
			{
				linearCombinationReturns[ i ] = 0;
				for ( int j=0 ; j < weightedPositions.Count ; j++ )
				{
					double weightedPositionReturn =
						tickersReturns[ j ][ i ] * multipliers[ j ];
					linearCombinationReturns[ i ] += weightedPositionReturn;
				}
			}
			return linearCombinationReturns;
		}
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
		private double getFitnessValue( double[] strategyReturns )
		{
//			double fitnessValue =
//				AdvancedFunctions.GetSharpeRatio(
//				strategyReturns );
//			double fitnessValue =
//				AdvancedFunctions.GetExpectancyScore(
//				strategyReturns );
			double fitnessValue =
				this.equityEvaluator.GetReturnsEvaluation(
				strategyReturns );

			//			double fitnessValue =
			//				this.getFitnessValue_withGoodFinal( strategyReturns );
			//			double fitnessValue =
			//				BasicFunctions.GetSimpleAverage( strategyReturns ) /
			//				( Math.Pow( BasicFunctions.GetStdDev( strategyReturns ) , 1.3 ) );
			return fitnessValue;
		}

		public double GetFitnessValue(
			WFLagWeightedPositions wFLagWeightedPositions )
		{
			double[] drivingPositionsReturns =
				this.getFitnessValue_getLinearCombinationReturns(
				wFLagWeightedPositions.DrivingWeightedPositions );
			double[] portfolioPositionsReturns =
				this.getFitnessValue_getLinearCombinationReturns(
				wFLagWeightedPositions.PortfolioWeightedPositions );
			double[] strategyReturns =
				this.getFitnessValue_getStrategyReturn(
				drivingPositionsReturns , portfolioPositionsReturns );
			double fitnessValue = this.getFitnessValue( strategyReturns );
			return fitnessValue;
		}
		public double GetFitnessValue( Genome genome )
		{
			double fitnessValue;
			WFLagWeightedPositions wFLagWeightedPositions =
				( WFLagWeightedPositions )this.Decode( genome );
			int genomeLength = genome.Genes().Length;
			int decodedWeightedPositions =
				wFLagWeightedPositions.DrivingWeightedPositions.Count +
				wFLagWeightedPositions.PortfolioWeightedPositions.Count;
			if ( decodedWeightedPositions < genomeLength )
				// genome contains a duplicate gene either for
				// driving positions or for portfolio positions
				//fitnessValue = double.MinValue;
				fitnessValue = -0.2;
			else
				// all driving positions genes are distinct and
				// all portfolio positions genes are distinct
				fitnessValue =
					this.GetFitnessValue( wFLagWeightedPositions );
			return fitnessValue;
		}
		#endregion
		public Genome[] GetChilds( Genome parent1 , Genome parent2 )
		{
			return
				GenomeManagement.AlternateFixedCrossover(parent1, parent2);
		}
		public void Mutate( Genome genome )
		{
//			int newValueForGene = GenomeManagement.RandomGenerator.Next(
//				genome.MinValueForGenes ,
//				genome.MaxValueForGenes + 1 );
			int genePositionToBeMutated =
				GenomeManagement.RandomGenerator.Next( genome.Size ); 
			int newValueForGene =
				this.GetNewGeneValue( genome , genePositionToBeMutated );
			GenomeManagement.MutateOneGene( genome ,
				genePositionToBeMutated , newValueForGene );
		}
		#region Decode
		private void getGenes_checkParameters( Genome genome , int firstGenePosition ,
			int numberOfGenes )
		{
			if ( firstGenePosition < 0 )
				throw new Exception( "firstGenePosition is less than zero!" );
			if ( firstGenePosition >= genome.Size )
				throw new Exception( "firstGenePosition is >= than genome.Size!" );
			if ( numberOfGenes <0 )
				throw new Exception( "firstGenePosition is less than zero!" );
			if ( firstGenePosition + numberOfGenes >= genome.Size )
				throw new Exception( "firstGenePosition + numberOfGenes is >= genome.Size !!" );
		}
		private int[] getGenes( Genome genome , int firstGenePosition , int numberOfGenes )
		{
			getGenes_checkParameters( genome , firstGenePosition , numberOfGenes );
			int[] genes = new int[ numberOfGenes ];
			for ( int i = 0 ; i < this.numberOfPortfolioPositions ; i++ )
			{
				int genePosition = firstGenePosition + i;
				genes[ i ] =	this.getTickerIndex( genome , genePosition );
			}
			return genes;
		}
		private int[] getTickerRelatedGeneValuesForDrivingPositions(
			Genome genome )
		{
			return this.getGenes( genome , 0 , this.numberOfDrivingPositions );
		}
		private int[] getTickerRelatedGeneValuesForPortfolioPositions(
			Genome genome )
		{
			return this.getGenes( genome , this.numberOfDrivingPositions ,
				this.numberOfPortfolioPositions );
		}
		private void decodeTicker_checkParameters( int geneValue , DataTable eligibleTickers )
		{
			if ( geneValue >= eligibleTickers.Rows.Count )
				throw new Exception( "geneValue is too (positive) large for eligibleTickers  !!" );
			if ( geneValue < -eligibleTickers.Rows.Count )
				throw new Exception( "geneValue is too (negative) large for eligibleTickers  !!" );
		}
		private string decodeTicker( int geneValue , DataTable eligibleTickers )
		{
			string ticker;
			decodeTicker_checkParameters( geneValue , eligibleTickers );
			if ( geneValue >= 0 )
				// long ticker
				ticker = ( string )eligibleTickers.Rows[ geneValue ][ 0 ];
			else
				// short ticker
				ticker = ( string )eligibleTickers.Rows[ -(geneValue+1) ][ 0 ];
			return ticker;
		}
		private string[] decodeTickers( int[] tickerRelatedGeneValues ,
			DataTable eligibleTickers )
		{
			string[] tickers = new string[ tickerRelatedGeneValues.Length ];
			for( int i = 0 ; i < tickerRelatedGeneValues.Length ; i++ )
			{
				int currentGeneValue = tickerRelatedGeneValues[ i ];
				tickers[ i ] = decodeTicker( currentGeneValue , eligibleTickers );
				
				tickers[ i ] =
					( string )eligibleTickers.Rows[ currentGeneValue ][ 0 ];
			}
			return tickers;
		}
		private string[] getTickersForDrivingPositions( Genome genome )
		{
			int[] geneValues = this.getTickerRelatedGeneValuesForDrivingPositions( genome );
			return this.decodeTickers( geneValues , this.eligibleTickersForDrivingWeightedPositions );
		}
		private string[] getTickersForPortfolioPositions( Genome genome )
		{
			int[] geneValues = this.getTickerRelatedGeneValuesForPortfolioPositions( genome );
			return this.decodeTickers( geneValues , this.eligibleTickersForPortfolioWeightedPositions );
		}
		private bool isDecodable( Genome genome )
		{
			return
				( WeightedPositions.AreValidTickers( this.getTickersForDrivingPositions( genome ) ) ) &&
				( WeightedPositions.AreValidTickers( this.getTickersForPortfolioPositions( genome ) ) );
		}
//		private int[] getWeightRelatedGeneValuesForDrivingPositions(
//			Genome genome )
//		{
//			int[] weightRelatedGeneValuesForDrivingPositions =
//				new int[ this.numberOfDrivingPositions ];
//			for ( int genePosition = 0 ;
//				genePosition < this.numberOfDrivingPositions ; genePosition++ )
//				weightRelatedGeneValuesForDrivingPositions[ genePosition ] =
//					this.getWeight( genome , genePosition );
//			return weightRelatedGeneValuesForDrivingPositions;
//		}
		private int getTickerIndex( Genome genome , int genePosition )
		{
			int tickerIndex = genome.GetGeneValue( genePosition );
			if ( tickerIndex < 0 )
				// the position is short
				tickerIndex += -this.GetMinValueForGenes( genePosition );
			return tickerIndex;
		}
		private double getWeight( Genome genome , int genePosition )
		{
			double weight = 1;
			if ( genome.GetGeneValue( genePosition ) < 0 )
				// the position is short
				weight = -1;
			return weight;
		}
		private double[] getWeights( Genome genome , int firstGenePosition , int numberOfGenes )
		{
			double[] weights = new double[ numberOfGenes ];
			for ( int i = 0 ; i < numberOfGenes ; i++ )
			{
				int genePosition = firstGenePosition + i;
				weights[ i ] = this.getWeight( genome , genePosition );
			}
			return weights;
		}
		private double[] getWeightsForDrivingPositions( Genome genome )
		{
			return this.getWeights( genome , 0 , this.numberOfDrivingPositions );
		}
		private double[] getWeightsForPortfolioPositions( Genome genome )
		{
			return this.getWeights( genome , this.numberOfDrivingPositions ,
				this.numberOfPortfolioPositions );
		}
		#region decodeWeightedPositions
		#region getNormalizedWeightValues
		private double getAdditionalWeight( int weightRelatedGeneValue )
		{
			double midrangeValue = (
				this.GetMinValueForGenes( 0 ) + this.GetMaxValueForGenes( 0 ) ) / 2;
			double singleWeightFreeRange = 1 - this.minimumPositionWeight;
			double scaleRange = Convert.ToDouble(
				this.GetMinValueForGenes( 0 ) - this.GetMaxValueForGenes( 0 ) );
			double nonScaledAdditionalWeight = Convert.ToDouble( weightRelatedGeneValue ) -
				midrangeValue;
			double scaledAdditionalWeight =
				nonScaledAdditionalWeight * singleWeightFreeRange / scaleRange;
			return scaledAdditionalWeight;
		}
		private double getNonNormalizedWeightValue( int weightRelatedGeneValue )
		{
			double additionalWeight = this.getAdditionalWeight( weightRelatedGeneValue );
			double nonNormalizedWeightValue = 0;
			if ( additionalWeight >= 0 )
				// the gene value represents a long position
				nonNormalizedWeightValue = this.minimumPositionWeight + additionalWeight;
			else
				// additionalWeight < 0 , i.e. the gene value represents a short position
				nonNormalizedWeightValue = -this.minimumPositionWeight + additionalWeight;
			return nonNormalizedWeightValue;
		}
		private double[] getNonNormalizedWeightValues( int[] weightRelatedGeneValues )
		{
			double[] nonNormalizedWeightValues = new double[ weightRelatedGeneValues.Length ];
			for ( int i = 0 ; i < weightRelatedGeneValues.Length ; i++ )
				nonNormalizedWeightValues[ i ] =
					this.getNonNormalizedWeightValue( weightRelatedGeneValues[ i ] );
			return nonNormalizedWeightValues;
		}
		private double getNormalizingFactor( double[] nonNormalizedWeightValues )
		{
			// the absolute value for each nonNormalizedWeightValue is between
			// this.minimumPositionWeight and 1
			double totalForNonNormalizedWeightValues = BasicFunctions.SumOfAbs( nonNormalizedWeightValues );
			double normalizingFactor = 1 / totalForNonNormalizedWeightValues;
			return normalizingFactor;
		}
		private double[] getNormalizedWeightValues( double[] nonNormalizedWeightValues ,
			double normalizingFactor )
		{
			return BasicFunctions.MultiplyBy( nonNormalizedWeightValues , normalizingFactor );
		}
		private double[] getNormalizedWeights( double[] nonNormalizedWeights )
		{
			double normalizingFactor = this.getNormalizingFactor( nonNormalizedWeights );
			return getNormalizedWeightValues( nonNormalizedWeights , normalizingFactor );
		}
//		private double[] getNormalizedWeightValues( int[] weightRelatedGeneValues )
//		{
//			double[] nonNormalizedWeightValues =
//				this.getNonNormalizedWeightValues( weightRelatedGeneValues );
//			return this.getNormalizedWeightValues( nonNormalizedWeightValues );
//		}
		#endregion
		private WeightedPositions decodeWeightedPositions( double[] weights ,
			string[] tickers , DataTable eligibleTickers )
		{
			double[] normalizedWeights = this.getNormalizedWeights( weights );
			WeightedPositions weightedPositions =	new WeightedPositions(
				normalizedWeights , tickers );
			return weightedPositions;
		}
		#endregion
		private WeightedPositions decodeDrivingWeightedPositions(
			Genome genome )
		{
			double[] weightsForDrivingPositions =	this.getWeightsForDrivingPositions( genome );
			string[] tickersForDrivingPositions =	this.getTickersForDrivingPositions( genome );
			return decodeWeightedPositions(
				weightsForDrivingPositions ,
				tickersForDrivingPositions ,
				this.eligibleTickersForDrivingWeightedPositions );
		}
		private WeightedPositions decodePortfolioWeightedPositions(
			Genome genome )
		{
			double[] portfolioPositionsWeights =	this.getWeightsForPortfolioPositions( genome );
			string[] tickersForPortfolioPositions =	this.getTickersForPortfolioPositions( genome );
			return decodeWeightedPositions(
				portfolioPositionsWeights ,
				tickersForPortfolioPositions ,
				this.eligibleTickersForPortfolioWeightedPositions );
		}
		private WFLagWeightedPositions dedcodeDecodableGenome( Genome genome )
		{
			WeightedPositions drivingWeightedPositions =
				this.decodeDrivingWeightedPositions( genome );
			WeightedPositions portfolioWeightedPositions =
				this.decodePortfolioWeightedPositions( genome );
			WFLagWeightedPositions wFLagWeightedPositions =
				new WFLagWeightedPositions(
				drivingWeightedPositions , portfolioWeightedPositions );
			return wFLagWeightedPositions;
		}
		/// <summary>
		/// A positive genome value means a long position. A negative genome value means a
		/// short position.
		/// The gene positive value n means the same ticker as the value -(n+1).
		/// Thus, if there are p (>0) eligible tickers, genome values should range from -p to p-1
		/// </summary>
		/// <param name="genome"></param>
		/// <returns></returns>
		public virtual object Decode(Genome genome)
		{
			object meaning;
			if ( this.isDecodable( genome ) )
				// genome can be decoded to a WFLagWeightedPositions object
				meaning = this.dedcodeDecodableGenome( genome );
			else
				// genome cannot be decoded to a WFLagWeightedPositions object
				meaning = this.wFLagMeaningForUndecodableGenomes;
			return meaning;

		}
		#endregion //Decode
		#region GetNewGeneValue
		public int GetNewGeneValue( Genome genome , int genePosition )
		{
			int minGeneValue = this.GetMinValueForGenes( genePosition );
			int maxGeneValue = this.GetMaxValueForGenes( genePosition );
			int returnValue =
				GenomeManagement.RandomGenerator.Next(
				minGeneValue , maxGeneValue + 1);
			return returnValue;
		}
		#endregion
	}
}
