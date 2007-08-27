/*
QuantProject - Quantitative Finance Library

WFLagGenomeManagerForFixedPortfolioWithNormalDrivingAndPortfolio.cs
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

using QuantProject.ADT.Histories;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.ADT.Statistics;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.EquityEvaluation;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// This class implements IGenomeManager, in order to find the
	/// best driving position group and the best
	/// portfolio position group with respect to the lag strategy.
	/// It uses fixed portfolio tickers and it
	/// is designed to optimize the driving portfolio.
	/// Normalized weights are used both for the driving and for the
	/// portfolio tickers.
	/// </summary>
	public class WFLagGenomeManagerForFixedPortfolioWithNormalDrivingAndPortfolio : IGenomeManager
	{
		private int numberOfDrivingPositions;
		private SignedTickers portfolioSignedTickers;
		private History timeLineForOptimization; // this time line goes from
		// the first optimization date for driving positions to the
		// last optimization date; this optimization is meant to be
		// launched one hour after the last market close

//		private DateTime firstOptimizationDateForDrivingPositions;
//		private DateTime lastOptimizationDate;
//		private int numberOfEligibleTickersForDrivingWeightedPositions;
		protected DataTable eligibleTickersForDrivingWeightedPositions;
		protected DataTable eligibleTickersForPortfolioWeightedPositions;
//		private DateTime firstOptimizationDateForDrivingPositions;
//		private DateTime lastOptimizationDate;

//		private double minimumPositionWeight;

//		protected WFLagCandidates wFLagCandidates;

		private IEquityEvaluator equityEvaluator;

		private WFLagMeaningForUndecodableGenomes wFLagMeaningForUndecodableGenomes;
		private string[] tickersForPortfolioPositions;
		private CloseToCloseReturnsManager closeToCloseReturnsManager;


		public int GenomeSize
		{
			get
			{
				return ( this.numberOfDrivingPositions );
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
		public WFLagGenomeManagerForFixedPortfolioWithNormalDrivingAndPortfolio(
			int numberOfDrivingPositions ,
			DataTable eligibleTickersForDrivingWeightedPositions ,
			SignedTickers portfolioSignedTickers ,
			History timeLineForOptimization ,
			IEquityEvaluator equityEvaluator ,
			int seedForRandomGenerator )

		{
			this.numberOfDrivingPositions = numberOfDrivingPositions;
//			this.numberOfEligibleTickersForDrivingWeightedPositions =
//				eligibleTickersForDrivingWeightedPositions.Rows.Count;
			this.eligibleTickersForDrivingWeightedPositions =
				eligibleTickersForDrivingWeightedPositions;
//			this.eligibleTickersForPortfolioWeightedPositions =
//				eligibleTickersForPortfolioWeightedPositions;
			this.portfolioSignedTickers = portfolioSignedTickers;
			this.timeLineForOptimization = timeLineForOptimization;

//			this.minimumPositionWeight = 0.2;	// TO DO this value should become a constructor parameter
			
			this.equityEvaluator = equityEvaluator;

//			GenomeManagement.SetRandomGenerator(
//				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator
//				+ this.firstOptimizationDate.DayOfYear );
//			GenomeManagement.SetRandomGenerator(
//				11 );
			GenomeManagement.SetRandomGenerator( seedForRandomGenerator );

//			this.wFLagCandidates = new WFLagCandidates(
//				this.eligibleTickersForDrivingWeightedPositions ,
//				this.firstOptimizationDateForDrivingPositions , this.lastOptimizationDate );
			this.closeToCloseReturnsManager =
				new CloseToCloseReturnsManager( this.timeLineForOptimization );

			this.wFLagMeaningForUndecodableGenomes =
				new WFLagMeaningForUndecodableGenomes();
		}
		public int GetMinValueForGenes( int genePosition )
		{
			int minValueForGene =
				-this.eligibleTickersForDrivingWeightedPositions.Rows.Count;
			return minValueForGene;
		}
		public int GetMaxValueForGenes( int genePosition )
		{
			return this.eligibleTickersForDrivingWeightedPositions.Rows.Count - 1;
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

		private float[] getFitnessValue_getLinearCombinationReturns(
			WeightedPositions weightedPositions )
		{
			return weightedPositions.GetReturns(
				this.closeToCloseReturnsManager );
		}
		private float[] getFitnessValue_getStrategyReturn(
			float[] drivingPositionsReturns , float[] portfolioPositionsReturns )
		{
			// strategyReturns contains one element less than drivingPositionsReturns,
			// because there is no strategy for the very first period (at least
			// one day signal is needed)
			float[] strategyReturns = new float[ portfolioPositionsReturns.Length - 1 ];
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
		private float getFitnessValue( float[] strategyReturns )
		{
//			double fitnessValue =
//				AdvancedFunctions.GetSharpeRatio(
//				strategyReturns );
//			double fitnessValue =
//				AdvancedFunctions.GetExpectancyScore(
//				strategyReturns );
			float fitnessValue =
				this.equityEvaluator.GetReturnsEvaluation(
				strategyReturns );

			//			double fitnessValue =
			//				this.getFitnessValue_withGoodFinal( strategyReturns );
			//			double fitnessValue =
			//				BasicFunctions.GetSimpleAverage( strategyReturns ) /
			//				( Math.Pow( BasicFunctions.GetStdDev( strategyReturns ) , 1.3 ) );
			return fitnessValue;
		}

		public float GetFitnessValue(
			WFLagWeightedPositions wFLagWeightedPositions )
		{
			float[] drivingPositionsReturns =
				this.getFitnessValue_getLinearCombinationReturns(
				wFLagWeightedPositions.DrivingWeightedPositions );
			float[] portfolioPositionsReturns =
				this.getFitnessValue_getLinearCombinationReturns(
				wFLagWeightedPositions.PortfolioWeightedPositions );
			float[] strategyReturns =
				this.getFitnessValue_getStrategyReturn(
				drivingPositionsReturns , portfolioPositionsReturns );
			float fitnessValue = this.getFitnessValue( strategyReturns );
			return fitnessValue;
		}
		public double GetFitnessValue( Genome genome )
		{
			double fitnessValue;
			object genomeMeaning = this.Decode( genome );
			if ( genomeMeaning is WFLagWeightedPositions )
			{
				// all driving positions genes are distinct and
				// all portfolio positions genes are distinct
				WFLagWeightedPositions wFLagWeightedPositions =
					(WFLagWeightedPositions)genomeMeaning;
				fitnessValue = this.GetFitnessValue( wFLagWeightedPositions );
			}
			else
			{
				// genome contains a duplicate gene either for
				// driving positions or for portfolio positions
				fitnessValue = -0.4;
			}
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
			if ( firstGenePosition + numberOfGenes > genome.Size )
				throw new Exception( "firstGenePosition + numberOfGenes is >= genome.Size !!" );
		}
		private int[] getGenes( Genome genome , int firstGenePosition , int numberOfGenes )
		{
			getGenes_checkParameters( genome , firstGenePosition , numberOfGenes );
			int[] genes = new int[ numberOfGenes ];
			for ( int i = 0 ; i < numberOfGenes ; i++ )
			{
				int genePosition = firstGenePosition + i;
				genes[ i ] =	genome.Genes()[ genePosition ];
			}
			return genes;
		}
		#region getSignedTickersForDrivingPositions
		private int[] getTickerRelatedGeneValuesForDrivingPositions(
			Genome genome )
		{
			return this.getGenes( genome , 0 , this.numberOfDrivingPositions );
		}
		private void decodeSignedTicker_checkParameters(
			int geneValue , DataTable eligibleTickers )
		{
			if ( geneValue >= eligibleTickers.Rows.Count )
				throw new Exception( "geneValue is too (positive) large for eligibleTickers  !!" );
			if ( geneValue < -eligibleTickers.Rows.Count )
				throw new Exception( "geneValue is too (negative) large for eligibleTickers  !!" );
		}
		private SignedTicker decodeSignedTicker(	int geneValue ,
			DataTable eligibleTickers )
		{
			SignedTicker signedTicker;
			string ticker;
			decodeSignedTicker_checkParameters( geneValue , eligibleTickers );
			if ( geneValue >= 0 )
			{
				// long ticker
				ticker = ( string )eligibleTickers.Rows[ geneValue ][ 0 ];
				signedTicker = new SignedTicker( ticker , PositionType.Long );
			}
			else
			{
				// short ticker
				ticker = ( string )eligibleTickers.Rows[ -(geneValue+1) ][ 0 ];
				signedTicker = new SignedTicker( ticker , PositionType.Short );
			}
			return signedTicker;
		}
		private SignedTicker decodeSignedTickers( int i ,
			int[] tickerRelatedGeneValues ,	DataTable eligibleTickers )
		{
			int currentGeneValue = tickerRelatedGeneValues[ i ];
			return this.decodeSignedTicker( currentGeneValue , eligibleTickers );
		}
		private SignedTickers decodeSignedTickers( int[] tickerRelatedGeneValues ,
			DataTable eligibleTickers )
		{
			SignedTickers signedTickers =	new SignedTickers();
			for( int i = 0 ; i < tickerRelatedGeneValues.Length ; i++ )
			{
				SignedTicker signedTicker = this.decodeSignedTickers(
					i , tickerRelatedGeneValues , eligibleTickers );
				signedTickers.Add( signedTicker );
			}
			return signedTickers;
		}
		private SignedTickers getSignedTickersForDrivingPositions( Genome genome )
		{
			int[] geneValues = this.getTickerRelatedGeneValuesForDrivingPositions( genome );
			return this.decodeSignedTickers( geneValues ,
				this.eligibleTickersForDrivingWeightedPositions );
		}
		#endregion getSignedTickersForDrivingPositions
		private string[] getTickersForDrivingPositions( Genome genome )
		{
			return this.getSignedTickersForDrivingPositions( genome ).Tickers;
		}
		#region getTickersForPortfolioPositions

		private void setTickerForPortfolioPositions( int i )
		{
			SignedTicker signedTicker = this.portfolioSignedTickers[ i ];
			this.tickersForPortfolioPositions[ i ] = signedTicker.Ticker;
		}
		private void setTickersForPortfolioPositions()
		{
			this.tickersForPortfolioPositions = new string[ this.portfolioSignedTickers.Count ];
			for( int i=0 ; i < portfolioSignedTickers.Count ; i++ )
				this.setTickerForPortfolioPositions( i );
		}
		private string[] getTickersForPortfolioPositions()
		{
			if ( this.tickersForPortfolioPositions == null )
				this.setTickersForPortfolioPositions();
			return this.tickersForPortfolioPositions;
		}
		#endregion //getTickersForPortfolioPositions
		private bool isDecodable( Genome genome )
		{
			return
				( WeightedPositions.AreValidTickers( this.getTickersForDrivingPositions( genome ) ) );
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
//		private int getTickerIndex( Genome genome , int genePosition )
//		{
//			int tickerIndex = genome.GetGeneValue( genePosition );
//			if ( tickerIndex < 0 )
//				// the position is short
//				tickerIndex += -this.GetMinValueForGenes( genePosition );
//			return tickerIndex;
//		}
//		private double getWeight( Genome genome , int genePosition )
//		{
//			double weight = 1;
//			if ( genome.GetGeneValue( genePosition ) < 0 )
//				// the position is short
//				weight = -1;
//			return weight;
//		}
//		private double[] getWeights( Genome genome , int firstGenePosition , int numberOfGenes )
//		{
//			double[] weights = new double[ numberOfGenes ];
//			for ( int i = 0 ; i < numberOfGenes ; i++ )
//			{
//				int genePosition = firstGenePosition + i;
//				weights[ i ] = this.getWeight( genome , genePosition );
//			}
//			return weights;
//		}
//		private double[] getWeightsForDrivingPositions( Genome genome )
//		{
//			return this.getWeights( genome , 0 , this.numberOfDrivingPositions );
//		}
//		private double[] getWeightsForPortfolioPositions( Genome genome )
//		{
//			return this.getWeights( genome , this.numberOfDrivingPositions ,
//				this.numberOfPortfolioPositions );
//		}
		#region decodeWeightedPositions
//		#region getNormalizedWeightValues
//		private double getAdditionalWeight( int weightRelatedGeneValue )
//		{
//			double midrangeValue = (
//				this.GetMinValueForGenes( 0 ) + this.GetMaxValueForGenes( 0 ) ) / 2;
//			double singleWeightFreeRange = 1 - this.minimumPositionWeight;
//			double scaleRange = Convert.ToDouble(
//				this.GetMinValueForGenes( 0 ) - this.GetMaxValueForGenes( 0 ) );
//			double nonScaledAdditionalWeight = Convert.ToDouble( weightRelatedGeneValue ) -
//				midrangeValue;
//			double scaledAdditionalWeight =
//				nonScaledAdditionalWeight * singleWeightFreeRange / scaleRange;
//			return scaledAdditionalWeight;
//		}
//		private double getNonNormalizedWeightValue( int weightRelatedGeneValue )
//		{
//			double additionalWeight = this.getAdditionalWeight( weightRelatedGeneValue );
//			double nonNormalizedWeightValue = 0;
//			if ( additionalWeight >= 0 )
//				// the gene value represents a long position
//				nonNormalizedWeightValue = this.minimumPositionWeight + additionalWeight;
//			else
//				// additionalWeight < 0 , i.e. the gene value represents a short position
//				nonNormalizedWeightValue = -this.minimumPositionWeight + additionalWeight;
//			return nonNormalizedWeightValue;
//		}
//		private double[] getNonNormalizedWeightValues( int[] weightRelatedGeneValues )
//		{
//			double[] nonNormalizedWeightValues = new double[ weightRelatedGeneValues.Length ];
//			for ( int i = 0 ; i < weightRelatedGeneValues.Length ; i++ )
//				nonNormalizedWeightValues[ i ] =
//					this.getNonNormalizedWeightValue( weightRelatedGeneValues[ i ] );
//			return nonNormalizedWeightValues;
//		}
//		private double getNormalizingFactor( double[] nonNormalizedWeightValues )
//		{
//			// the absolute value for each nonNormalizedWeightValue is between
//			// this.minimumPositionWeight and 1
//			double totalForNonNormalizedWeightValues = BasicFunctions.SumOfAbs( nonNormalizedWeightValues );
//			double normalizingFactor = 1 / totalForNonNormalizedWeightValues;
//			return normalizingFactor;
//		}
//		private double[] getNormalizedWeightValues( double[] nonNormalizedWeightValues ,
//			double normalizingFactor )
//		{
//			return BasicFunctions.MultiplyBy( nonNormalizedWeightValues , normalizingFactor );
//		}
//		private double[] getNormalizedWeights( double[] nonNormalizedWeights )
//		{
//			double normalizingFactor = this.getNormalizingFactor( nonNormalizedWeights );
//			return getNormalizedWeightValues( nonNormalizedWeights , normalizingFactor );
//		}
////		private double[] getNormalizedWeightValues( int[] weightRelatedGeneValues )
////		{
////			double[] nonNormalizedWeightValues =
////				this.getNonNormalizedWeightValues( weightRelatedGeneValues );
////			return this.getNormalizedWeightValues( nonNormalizedWeightValues );
////		}
//		#endregion
		private WeightedPositions decodeWeightedPositions( double[] weights ,
			string[] tickers , DataTable eligibleTickers )
		{
			double[] normalizedWeights =
				WeightedPositions.GetNormalizedWeights( weights );
			WeightedPositions weightedPositions =	new WeightedPositions(
				normalizedWeights , tickers );
			return weightedPositions;
		}
		#endregion
		private double[] getNormalizedWeightsForDrivingPositions(
			string[] tickersForDrivingPositions )
		{
			return WeightedPositions.GetBalancedWeights( this.portfolioSignedTickers ,
				this.closeToCloseReturnsManager );
		}
		private WeightedPositions decodeDrivingWeightedPositions(
			Genome genome )
		{
			SignedTickers signedTickers =
				this.getSignedTickersForDrivingPositions( genome );
			double[] balancedWeightsForDrivingPositions =
				WeightedPositions.GetBalancedWeights( signedTickers ,
				this.closeToCloseReturnsManager );
			WeightedPositions weightedPositions = new WeightedPositions(
				balancedWeightsForDrivingPositions , signedTickers.Tickers );
//			string[] tickersForDrivingPositions =
//				this.getTickersForDrivingPositions( genome );
//			double[] weightsForDrivingPositions =
//				this.getWeightsForDrivingPositions( genome );
//			double[] balancedWeightsForDrivingPositions =
//				WeightedPositions.GetBalancedWeights( qui!!!! , weightsForDrivingPositions );
//			return this.decodeWeightedPositions(
//				weightsForDrivingPositions ,
//				tickersForDrivingPositions ,
//				this.eligibleTickersForDrivingWeightedPositions );
			return weightedPositions;
		}
		private double[] getBalancedWeightsForPortfolioPositions()
		{
			return WeightedPositions.GetBalancedWeights( this.portfolioSignedTickers ,
				this.closeToCloseReturnsManager );
		}
		private WeightedPositions decodePortfolioWeightedPositions(
			Genome genome )
		{
			string[] tickersForPortfolioPositions =	this.getTickersForPortfolioPositions();
			double[] portfolioPositionsWeights =
				this.getBalancedWeightsForPortfolioPositions();
			return this.decodeWeightedPositions(
				portfolioPositionsWeights ,
				tickersForPortfolioPositions ,
				this.eligibleTickersForPortfolioWeightedPositions );
		}
		private WFLagWeightedPositions decodeDecodableGenome( Genome genome )
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
				meaning = this.decodeDecodableGenome( genome );
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
