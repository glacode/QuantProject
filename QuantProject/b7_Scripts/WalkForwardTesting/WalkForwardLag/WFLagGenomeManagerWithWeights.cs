using System;
using System.Collections;
using System.Data;

using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.ADT.Statistics;
using QuantProject.Business.Strategies;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// This class implements IGenomeManager, in order to find the
	/// best driving position group and the best
	/// portfolio position group with respect to the lag strategy.
	/// Weighted positions are used for both the driving positions
	/// and the portfolio positions
	/// </summary>
	public class WFLagGenomeManagerWithWeights : IGenomeManager
	{
		private int numberOfDrivingPositions;
		private int numberOfTickersInPortfolio;
		private int numberOfEligibleTickersForDrivingWeightedPositions;
		private DataTable eligibleTickersForDrivingWeightedPositions;
		private DataTable eligibleTickersForPortfolioWeightedPositions;
		private DateTime firstOptimizationDate;
		private DateTime lastOptimizationDate;

		private double minimumPositionWeight;

		private WFLagCandidates wFLagCandidates;


		private GeneticOptimizer currentGeneticOptimizer;

		public int GenomeSize
		{
			get
			{
				return ( this.numberOfDrivingPositions + this.numberOfTickersInPortfolio ) * 2;
			}
		}

		public int MinValueForGenes
		{
			get { return -this.numberOfEligibleTickersForDrivingWeightedPositions; }
		}
		public int MaxValueForGenes
		{
			get { return this.numberOfEligibleTickersForDrivingWeightedPositions - 1; }
		}
		public GeneticOptimizer CurrentGeneticOptimizer
		{
			get{ return this.currentGeneticOptimizer; }
			set{ this.currentGeneticOptimizer = value; }
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
		public WFLagGenomeManagerWithWeights(
			DataTable eligibleTickersForDrivingWeightedPositions ,
			DataTable eligibleTickersForPortfolioWeightedPositions ,
			DateTime firstOptimizationDate ,
			DateTime lastOptimizationDate ,
			int numberOfDrivingPositions ,
			int numberOfTickersInPortfolio )

		{
			this.numberOfDrivingPositions = numberOfDrivingPositions;
			this.numberOfTickersInPortfolio = numberOfTickersInPortfolio;
			this.numberOfEligibleTickersForDrivingWeightedPositions =
				eligibleTickersForDrivingWeightedPositions.Rows.Count;
			this.eligibleTickersForDrivingWeightedPositions =
				eligibleTickersForDrivingWeightedPositions;
			this.eligibleTickersForPortfolioWeightedPositions =
				eligibleTickersForPortfolioWeightedPositions;
			this.firstOptimizationDate = firstOptimizationDate;
			this.lastOptimizationDate = lastOptimizationDate;

			this.minimumPositionWeight = 0.2;	// TO DO this value should become a constructor parameter

			this.wFLagCandidates = new WFLagCandidates(
				this.eligibleTickersForDrivingWeightedPositions ,
				this.firstOptimizationDate , this.lastOptimizationDate );
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
						double fitnessValue =
							AdvancedFunctions.GetExpectancyScore(
							strategyReturns );
			//			double fitnessValue =
			//				this.getFitnessValue_withGoodFinal( strategyReturns );
			//			double fitnessValue =
			//				BasicFunctions.GetSimpleAverage( strategyReturns ) /
			//				( Math.Pow( BasicFunctions.GetStdDev( strategyReturns ) , 1.3 ) );
			return fitnessValue;
		}

		private double getFitnessValue(
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
			if ( ( decodedWeightedPositions * 2 ) < genomeLength )
				// genome contains a duplicate gene either for
				// driving positions or for portfolio positions
				fitnessValue = double.MinValue;
			else
				// all driving positions genes are distinct and
				// all portfolio positions genes are distinct
				fitnessValue =
					this.getFitnessValue( wFLagWeightedPositions );
			return fitnessValue;
		}
		#endregion
		public Genome[] GetChilds( Genome parent1 , Genome parent2 )
		{
			return
				GenomeManagement.AlternateFixedCrossover(parent1, parent2);
		}
		public void Mutate( Genome genome , double mutationRate )
		{
//			int newValueForGene = GenomeManagement.RandomGenerator.Next(
//				genome.MinValueForGenes ,
//				genome.MaxValueForGenes + 1 );
			int genePositionToBeMutated =
				GenomeManagement.RandomGenerator.Next( genome.Size ); 
			int newValueForGene =
				this.GetNewGeneValue( genome , genePositionToBeMutated );
			GenomeManagement.MutateOneGene( genome , mutationRate ,
				genePositionToBeMutated , newValueForGene );
		}
		#region Decode
		private int[] getWeightRelatedGeneValuesForDrivingPositions(
			Genome genome )
		{
			int[] weightRelatedGeneValuesForDrivingPositions =
				new int[ this.numberOfDrivingPositions ];
			for ( int i = 0 ; i < this.numberOfDrivingPositions ; i++ )
			{
				int genePosition = 2 * i;
				weightRelatedGeneValuesForDrivingPositions[ i ] =
					genome.GetGeneValue( genePosition );
			}
			return weightRelatedGeneValuesForDrivingPositions;
		}
		private int[] getTickerRelatedGeneValuesForDrivingPositions(
			Genome genome )
		{
			int[] tickerRelatedGeneValuesForDrivingPositions =
				new int[ this.numberOfDrivingPositions ];
			for ( int i = 0 ; i < this.numberOfDrivingPositions ; i++ )
			{
				int genePosition = 2 * i + 1;
				tickerRelatedGeneValuesForDrivingPositions[ i ] =
					genome.GetGeneValue( genePosition );
			}
			return tickerRelatedGeneValuesForDrivingPositions;
		}
		private int[] getWeightRelatedGeneValuesForPortfolioPositions(
			Genome genome )
		{
			int[] weightRelatedGeneValuesForPortfolioPositions =
				new int[ this.numberOfTickersInPortfolio ];
			int firstPositionForPortfolioRelatedGenomes =
				this.numberOfDrivingPositions * 2;
			for ( int i = 0 ; i < this.numberOfTickersInPortfolio ; i++ )
			{
				int genePosition =
					firstPositionForPortfolioRelatedGenomes + 2 * i;
				weightRelatedGeneValuesForPortfolioPositions[ i ] =
					genome.GetGeneValue( genePosition );
			}
			return weightRelatedGeneValuesForPortfolioPositions;
		}
		private int[] getTickerRelatedGeneValuesForPortfolioPositions(
			Genome genome )
		{
			int[] tickerRelatedGeneValuesForPortfolioPositions =
				new int[ this.numberOfTickersInPortfolio ];
			int firstPositionForPortfolioRelatedGenomes =
				this.numberOfDrivingPositions * 2;
			for ( int i = 0 ; i < this.numberOfTickersInPortfolio ; i++ )
			{
				int genePosition =
					firstPositionForPortfolioRelatedGenomes + 2 * i + 1;
				tickerRelatedGeneValuesForPortfolioPositions[ i ] =
					genome.GetGeneValue( genePosition );
			}
			return tickerRelatedGeneValuesForPortfolioPositions;
		}
		#region decodeWeightedPositions
		#region getNormalizedWeightValues
		private double getAdditionalWeight( int weightRelatedGeneValue )
		{
			double midrangeValue = ( this.MinValueForGenes + this.MaxValueForGenes ) / 2;
			double singleWeightFreeRange = 1 - this.minimumPositionWeight;
			double scaleRange = Convert.ToDouble( this.MaxValueForGenes - this.MinValueForGenes );
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
		private double[] getNormalizedWeightValues( double[] nonNormalizedWeightValues )
		{
			double normalizingFactor = this.getNormalizingFactor( nonNormalizedWeightValues );
			return getNormalizedWeightValues( nonNormalizedWeightValues , normalizingFactor );
		}
		private double[] getNormalizedWeightValues( int[] weightRelatedGeneValues )
		{
			double[] nonNormalizedWeightValues =
				this.getNonNormalizedWeightValues( weightRelatedGeneValues );
			return this.getNormalizedWeightValues( nonNormalizedWeightValues );
		}
		#endregion
		private string[] decodeTickers( int[] tickerRelatedGeneValues ,
			DataTable eligibleTickers )
		{
			string[] tickers = new string[ tickerRelatedGeneValues.Length ];
			for( int i = 0 ; i < tickerRelatedGeneValues.Length ; i++ )
			{
				int currentGeneValue = tickerRelatedGeneValues[ i ];
				tickers[ i ] =
					( string )eligibleTickers.Rows[ currentGeneValue ][ 0 ];
			}
			return tickers;
		}
		private WeightedPositions decodeWeightedPositions( int[] weightRelatedGeneValues ,
			int[] tickerRelatedGeneValues , DataTable eligibleTickers )
		{
			double[] normalizedWeightValues = this.getNormalizedWeightValues( weightRelatedGeneValues );
			string[] tickers = this.decodeTickers( tickerRelatedGeneValues , eligibleTickers );
			WeightedPositions weightedPositions =	new WeightedPositions(
				normalizedWeightValues , tickers );
			return weightedPositions;
		}
		#endregion
		private WeightedPositions decodeDrivingWeightedPositions(
			Genome genome )
		{
			int[] weightRelatedGeneValuesForDrivingPositions =
				this.getWeightRelatedGeneValuesForDrivingPositions( genome );
			int[] tickerRelatedGeneValuesForDrivingPositions =
				this.getTickerRelatedGeneValuesForDrivingPositions( genome );
			return decodeWeightedPositions(
				weightRelatedGeneValuesForDrivingPositions ,
				tickerRelatedGeneValuesForDrivingPositions ,
				this.eligibleTickersForDrivingWeightedPositions );
			//
			//			double weight = this.decodeDrivingWeight( genome , geneIndex );
			//			string ticker = this.decodeDrivingTicker( genome , geneIndex );
			//			WeightedPosition weightedPosition = new WeightedPosition(
			//				weight , ticker );
			//			wFLagWeightedPositions.DrivingWeightedPositions.AddWeightedPosition(
			//				weightedPosition );
		}
		private WeightedPositions decodePortfolioWeightedPositions(
			Genome genome )
		{
			int[] weightRelatedGeneValuesForPortfolioPositions =
				this.getWeightRelatedGeneValuesForPortfolioPositions( genome );
			int[] tickerRelatedGeneValuesForPortfolioPositions =
				this.getTickerRelatedGeneValuesForPortfolioPositions( genome );
			return decodeWeightedPositions(
				weightRelatedGeneValuesForPortfolioPositions ,
				tickerRelatedGeneValuesForPortfolioPositions ,
				this.eligibleTickersForPortfolioWeightedPositions );
			//
			//			double weight = this.decodeDrivingWeight( genome , geneIndex );
			//			string ticker = this.decodeDrivingTicker( genome , geneIndex );
			//			WeightedPosition weightedPosition = new WeightedPosition(
			//				weight , ticker );
			//			wFLagWeightedPositions.DrivingWeightedPositions.AddWeightedPosition(
			//				weightedPosition );
		}
		public virtual object Decode(Genome genome)
		{
			WeightedPositions drivingWeightedPositions =
				this.decodeDrivingWeightedPositions( genome );
			WeightedPositions portfolioWeightedPositions =
				this.decodePortfolioWeightedPositions( genome );
			WFLagWeightedPositions wFLagWeightedPositions =
				new WFLagWeightedPositions(
				drivingWeightedPositions , portfolioWeightedPositions );

			//			int[] drivingGeneValues = this.getDrivingGeneValues( genome );
			//			for ( int geneIndex = 0 ; geneIndex < this.numberOfDrivingPositions * 2 ;
			//				geneIndex += 2 )
			//				this.decode_addDrivingPosition( genome , geneIndex , wFLagWeightedPositions );
			//			for ( int geneIndex = this.numberOfDrivingPositions * 2 ;
			//				geneIndex <
			//				( this.numberOfDrivingPositions + this.numberOfTickersInPortfolio ) * 2 ;
			//				geneIndex += 2 )
			//				this.decode_addPortfolioPosition( genome , geneIndex , wFLagSignedTickers );
			//			string[] arrayOfTickers = new string[genome.Genes().Length];
			//			int indexOfTicker;
			//			for(int index = 0; index < genome.Genes().Length; index++)
			//			{
			//				indexOfTicker = (int)genome.Genes().GetValue(index);
			//				arrayOfTickers[index] =
			//					this.decode_getSignedTickerForGeneValue(indexOfTicker);
			//			}
			return wFLagWeightedPositions;
		}
		#endregion
		#region GetNewGeneValue
		private int getMinGeneValue( int i )
		{
			int minGeneValue = 0;
			if ( i % 2 == 0 )
				minGeneValue = this.MinValueForGenes;
			return minGeneValue;
		}
		private int getMaxGeneValue( int i )
		{
			return this.MaxValueForGenes;
		}
		public int GetNewGeneValue( Genome genome , int i )
		{
			int minGeneValue = this.getMinGeneValue( i );
			int maxGeneValue = this.getMaxGeneValue( i );
			int returnValue =
				GenomeManagement.RandomGenerator.Next(
				minGeneValue , maxGeneValue + 1);
			return returnValue;
		}
		#endregion
	}
}
