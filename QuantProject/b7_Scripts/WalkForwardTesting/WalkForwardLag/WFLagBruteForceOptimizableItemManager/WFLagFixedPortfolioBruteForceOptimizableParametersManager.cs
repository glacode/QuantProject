/*
QuantProject - Quantitative Finance Library

WFLagFixedPortfolioBruteForceOptimizableParametersManager.cs
Copyright (C) 2006
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
using System.Data;

using QuantProject.ADT.Optimizing.BruteForce;
using QuantProject.ADT.Statistics.Combinatorial;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// This class implements IBruteForceOptimizableParametersManager,
	/// in order to find the best driving positions for a fixed
	/// portfolio with two tickers (one long position and one short
	/// position).
	/// Weights are NOT used in this implementation
	/// </summary>
	public class WFLagFixedPortfolioBruteForceOptimizableParametersManager :
		WFLagGenomeManager , IBruteForceOptimizableParametersManager
	{
		private Combination drivingCombination;

		private int numberOfDrivingPositions;
//		protected WFLagGenomeManager wFLagGenomeManager;

		private DataTable eligibleTickersForDrivingPositions;
		protected string portfolioLongTicker;
		protected string portfolioShortTicker;

		public int TotalIterations
		{
			get
			{
				return Convert.ToInt32( this.drivingCombination.TotalNumberOfCombinations );
			}
		}
		public WFLagFixedPortfolioBruteForceOptimizableParametersManager(
			DataTable eligibleTickersForDrivingPositions ,
			string portfolioLongTicker ,
			string portfolioShortTicker ,
			DateTime firstOptimizationDate ,
			DateTime lastOptimizationDate ,
			int numberOfDrivingPositions ) :
			base(
			eligibleTickersForDrivingPositions ,
			eligibleTickersForDrivingPositions ,
			firstOptimizationDate ,
			lastOptimizationDate ,
			numberOfDrivingPositions ,
			2 ,
			QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator )
		{
			this.eligibleTickersForDrivingPositions =
				eligibleTickersForDrivingPositions;
			this.portfolioLongTicker = portfolioLongTicker;
			this.portfolioShortTicker = portfolioShortTicker;
			this.numberOfDrivingPositions = numberOfDrivingPositions;
			this.drivingCombination = new Combination(
				- eligibleTickersForDrivingPositions.Rows.Count ,
				eligibleTickersForDrivingPositions.Rows.Count - 1 ,
				numberOfDrivingPositions );
//			this.wFLagGenomeManager = new WFLagGenomeManager(
//				eligibleTickersForDrivingPositions ,
//				eligibleTickersForDrivingPositions ,
//				firstOptimizationDate ,
//				lastOptimizationDate ,
//				numberOfDrivingPositions ,
//				2 ,
//				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator );
		}
		public bool MoveNext()
		{
			return this.drivingCombination.MoveNext();
		}
		public void Reset()
		{
			this.drivingCombination.Reset();
		}
		#region Current
		public object Current
		{
			get
			{
				return this.getCurrent();
			}
		}
		private object getCurrent()
		{
			int[] currentValues = new int[ this.drivingCombination.Length ];
			for ( int i = 0 ; i < this.drivingCombination.Length ; i ++ )
				currentValues[ i ] = this.drivingCombination.GetValue( i );
			BruteForceOptimizableParameters bruteForceOptimizableParameters =
				new BruteForceOptimizableParameters( currentValues ,
				this );
			return bruteForceOptimizableParameters;
		}
		#endregion
		#region Decode
		private bool isReallyMeaningful( BruteForceOptimizableParameters
			bruteForceOptimizableParameters )
		{
			int[] optimizableParameterValues = bruteForceOptimizableParameters.GetValues();
			int[] tickerRelatedParameterValues =
				this.getTickerRelatedParameterValuesForDrivingPositions(
				optimizableParameterValues );
			string[] tickersForDrivingPositions = this.decodeTickers(
				tickerRelatedParameterValues , this.eligibleTickersForDrivingPositions );
			return WeightedPositions.AreValidTickers( tickersForDrivingPositions );
		}
		private double getWeight( int[] optimizableItem , int index )
		{
			int weight = 1;
			if ( optimizableItem[ index ] < 0 )
				// the position is short
				weight = -1;
			return weight;
		}
		private double[] getWeightRelatedParameterValuesForDrivingPositions(
			int[] optimizableParameters )
		{
			double[] weightRelatedParameterValuesForDrivingPositions =
				new double[ this.numberOfDrivingPositions ];
			for ( int parameterPosition = 0 ;
				parameterPosition < this.numberOfDrivingPositions ; parameterPosition++ )
				weightRelatedParameterValuesForDrivingPositions[ parameterPosition ] =
					this.getWeight( optimizableParameters , parameterPosition ) /
					this.numberOfDrivingPositions;
			return weightRelatedParameterValuesForDrivingPositions;
		}
		private int getTickerIndexForDrivingPositions( int[] parameterValues ,
			int parameterPosition )
		{
			int tickerIndex = parameterValues[ parameterPosition ];
			if ( tickerIndex < 0 )
				// the position is short
				tickerIndex +=
					this.eligibleTickersForDrivingPositions.Rows.Count;
			return tickerIndex;
		}
		private int[] getTickerRelatedParameterValuesForDrivingPositions(
			int[] optimizableParameters )
		{
			int[] tickerRelatedParameterValuesForDrivingPositions =
				new int[ this.numberOfDrivingPositions ];
			for ( int parameterPosition = 0 ;
				parameterPosition < this.numberOfDrivingPositions ; parameterPosition++ )
				tickerRelatedParameterValuesForDrivingPositions[ parameterPosition ] =
					this.getTickerIndexForDrivingPositions(
					optimizableParameters , parameterPosition );
			return tickerRelatedParameterValuesForDrivingPositions;
		}

		#region decodeWeightedPositions
		private string[] decodeTickers( int[] tickerRelatedParameterValues ,
			DataTable eligibleTickers )
		{
			string[] tickers = new string[ tickerRelatedParameterValues.Length ];
			for( int i = 0 ; i < tickerRelatedParameterValues.Length ; i++ )
			{
				int currentParameterValue = tickerRelatedParameterValues[ i ];
				
				tickers[ i ] =
					( string )eligibleTickers.Rows[ currentParameterValue ][ 0 ];
			}
			return tickers;
		}
		private WeightedPositions decodeWeightedPositions(
			double[] weightRelatedParameterValues ,
			int[] tickerRelatedParameterValues , DataTable eligibleTickers )
		{
			string[] tickers = this.decodeTickers(
				tickerRelatedParameterValues , eligibleTickers );
			WeightedPositions weightedPositions =	new WeightedPositions(
				weightRelatedParameterValues , tickers );
			return weightedPositions;
		}
		#endregion
		private WeightedPositions decodeDrivingWeightedPositions(
			int[] optimizableItemValues )
		{
			double[] weightRelatedParameterValuesForDrivingPositions =
				this.getWeightRelatedParameterValuesForDrivingPositions( optimizableItemValues );
			int[] tickerRelatedParameterValuesForDrivingPositions =
				this.getTickerRelatedParameterValuesForDrivingPositions( optimizableItemValues );
			return decodeWeightedPositions(
				weightRelatedParameterValuesForDrivingPositions ,
				tickerRelatedParameterValuesForDrivingPositions ,
				this.eligibleTickersForDrivingPositions );
		}
		protected virtual WeightedPositions decodePortfolioWeightedPositions(
			int[] optimizableParameters )
		{
			double[] weightsForPortfolioPositions = new double[ 2 ] { 0.5 , -0.5 };
			string[] tickersForPortfolioPositions =
				new string[ 2 ] { this.portfolioLongTicker , this.portfolioShortTicker };
			WeightedPositions weightedPositions =
				new WeightedPositions( weightsForPortfolioPositions ,
				tickersForPortfolioPositions );
			return weightedPositions;
		}
		private object decodeMeaningfulParameters( BruteForceOptimizableParameters
			bruteForceOptimizableParameters )
		{
			int[] optimizablePrameterValues = bruteForceOptimizableParameters.GetValues();
			WeightedPositions drivingWeightedPositions =
				this.decodeDrivingWeightedPositions( optimizablePrameterValues );
			WeightedPositions portfolioWeightedPositions =
				this.decodePortfolioWeightedPositions( optimizablePrameterValues );
			WFLagWeightedPositions wFLagWeightedPositions =
				new WFLagWeightedPositions(
				drivingWeightedPositions , portfolioWeightedPositions );

			return wFLagWeightedPositions;
		}
		public object Decode( BruteForceOptimizableParameters
			bruteForceOptimizableParameters )
		{
			object meaning;
			if ( this.isReallyMeaningful( bruteForceOptimizableParameters ) )
				meaning = this.decodeMeaningfulParameters( bruteForceOptimizableParameters );
			else
				meaning = null;
			return meaning;
		}
		#endregion
		#region GetFitnessValue
		public double GetFitnessValue(
			BruteForceOptimizableParameters bruteForceOptimizableItem )
		{
			double fitnessValue;
			WFLagWeightedPositions wFLagWeightedPositions =
				( WFLagWeightedPositions )this.Decode( bruteForceOptimizableItem );
			if ( wFLagWeightedPositions == null )
				// the optimizable parameters object
				// contains a duplicate ticker for
				// driving positions. It is not meaningful in this implementation
				fitnessValue = -0.2;
			else
				// all driving position parameters refer to distinct tickers
				fitnessValue =
					base.GetFitnessValue( wFLagWeightedPositions );
			return fitnessValue;
		}
		#endregion
		private int getMinValueForParameter( int parameterPosition )
		{
			int minValueForParameter =
				-this.eligibleTickersForDrivingPositions.Rows.Count;
			return minValueForParameter;
		}
		private int getMaxValueForParameter( int parameterPosition )
		{
			int maxValueForParameter =
				this.eligibleTickersForDrivingPositions.Rows.Count - 1;
			return maxValueForParameter;
		}
	}
}
