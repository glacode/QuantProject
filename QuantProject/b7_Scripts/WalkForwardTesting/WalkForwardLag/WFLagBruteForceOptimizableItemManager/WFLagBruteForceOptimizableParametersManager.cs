/*
QuantProject - Quantitative Finance Library

WFLagBruteForceOptimizableParametersManager.cs
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
	/// in order to find the best driving position group and the best
	/// portfolio position group with respect to the lag strategy.
	/// Weights are NOT used in this implementation
	/// </summary>
	public class WFLagBruteForceOptimizableParametersManager :
		IBruteForceOptimizableParametersManager
	{
		private Combination drivingCombination;
		private Combination portfolioCombination;

		private int numberOfDrivingPositions;
		private int numberOfPortfolioPositions;
		private WFLagGenomeManager wFLagGenomeManager;
		private DataTable eligibleTickersForDrivingPositions;
		private DataTable eligibleTickersForPortfolioPositions;

		public WFLagBruteForceOptimizableParametersManager(
			DataTable eligibleTickersForDrivingPositions ,
			DataTable eligibleTickersForPortfolioPositions ,
			DateTime firstOptimizationDate ,
			DateTime lastOptimizationDate ,
			int numberOfDrivingPositions ,
			int numberOfPortfolioPositions )
		{
			this.eligibleTickersForDrivingPositions =
				eligibleTickersForDrivingPositions;
			this.eligibleTickersForPortfolioPositions =
				eligibleTickersForPortfolioPositions;
			this.numberOfDrivingPositions = numberOfDrivingPositions;
			this.numberOfPortfolioPositions = numberOfPortfolioPositions;
			this.drivingCombination = new Combination(
				- eligibleTickersForDrivingPositions.Rows.Count ,
				eligibleTickersForDrivingPositions.Rows.Count - 1 ,
				numberOfDrivingPositions );
			this.portfolioCombination = new Combination(
				- eligibleTickersForPortfolioPositions.Rows.Count ,
				eligibleTickersForPortfolioPositions.Rows.Count - 1 ,
				numberOfPortfolioPositions );
			this.wFLagGenomeManager = new WFLagGenomeManager(
				eligibleTickersForDrivingPositions ,
				eligibleTickersForPortfolioPositions ,
				firstOptimizationDate ,
				lastOptimizationDate ,
				numberOfDrivingPositions ,
				numberOfPortfolioPositions ,
				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator );
		}
		public bool MoveNext()
		{
			bool isNotTheLast = true;
			if ( !this.portfolioCombination.MoveNext() )
			{
				// last portfolio combination
				this.portfolioCombination.Reset();
				isNotTheLast = this.drivingCombination.MoveNext();
			}
			return isNotTheLast;
		}
		public void Reset()
		{
			this.drivingCombination.Reset();
			this.portfolioCombination.Reset();
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
			int[] currentValues = new int[ this.drivingCombination.Length +
				this.portfolioCombination.Length ];
			for ( int i = 0 ; i < this.drivingCombination.Length ; i ++ )
				currentValues[ i ] = this.drivingCombination.GetValue( i );
			for ( int i = this.drivingCombination.Length ;
				i < this.drivingCombination.Length +
				this.portfolioCombination.Length ; i ++ )
				currentValues[ i ] =
					this.portfolioCombination.GetValue( i - this.drivingCombination.Length );
			BruteForceOptimizableParameters bruteForceOptimizableParameters =
				new BruteForceOptimizableParameters( currentValues ,
				this );
			return bruteForceOptimizableParameters;
		}
		#endregion
		#region Decode
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
		private double[] getWeightRelatedParameterValuesForPortfolioPositions(
			int[] optimizableParameters )
		{
			double[] weightRelatedParameterValuesForPortfolioPositions =
				new double[ this.numberOfPortfolioPositions ];
			int firstPositionForPortfolioRelatedGenomes =
				this.numberOfDrivingPositions;
			for ( int i = 0 ; i < this.numberOfPortfolioPositions ; i++ )
			{
				int parameterPosition =
					firstPositionForPortfolioRelatedGenomes + i;
				weightRelatedParameterValuesForPortfolioPositions[ i ] =
					this.getWeight( optimizableParameters , parameterPosition ) /
					this.numberOfPortfolioPositions;
			}
			return weightRelatedParameterValuesForPortfolioPositions;
		}
		private int getTickerIndexForPortfolioPositions( int[] parameterValues ,
			int parameterPosition )
		{
			int tickerIndex = parameterValues[ parameterPosition ];
			if ( tickerIndex < 0 )
				// the position is short
				tickerIndex +=
					this.eligibleTickersForPortfolioPositions.Rows.Count;
			return tickerIndex;
		}
		private int[] getTickerRelatedParameterValuesForPortfolioPositions(
			int[] parameterValues )
		{
			int[] tickerRelatedParameterValuesForPortfolioPositions =
				new int[ this.numberOfPortfolioPositions ];
			int firstPositionForPortfolioRelatedGenomes =
				this.numberOfDrivingPositions;
			for ( int i = 0 ; i < this.numberOfPortfolioPositions ; i++ )
			{
				int parameterPosition =
					firstPositionForPortfolioRelatedGenomes + i;
				tickerRelatedParameterValuesForPortfolioPositions[ i ] =
					this.getTickerIndexForPortfolioPositions( parameterValues ,
					parameterPosition );
			}
			return tickerRelatedParameterValuesForPortfolioPositions;
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
		private WeightedPositions decodePortfolioWeightedPositions(
			int[] optimizableParameters )
		{
			double[] weightRelatedParameterValuesForPortfolioPositions =
				this.getWeightRelatedParameterValuesForPortfolioPositions(
				optimizableParameters );
			int[] tickerRelatedParameterValuesForPortfolioPositions =
				this.getTickerRelatedParameterValuesForPortfolioPositions(
				optimizableParameters );
			return decodeWeightedPositions(
				weightRelatedParameterValuesForPortfolioPositions ,
				tickerRelatedParameterValuesForPortfolioPositions ,
				this.eligibleTickersForPortfolioPositions );
		}
		public object Decode( BruteForceOptimizableParameters
			bruteForceOptimizableItem )
		{
			int[] optimizableItemValues = bruteForceOptimizableItem.GetValues();
			WeightedPositions drivingWeightedPositions =
				this.decodeDrivingWeightedPositions( optimizableItemValues );
			WeightedPositions portfolioWeightedPositions =
				this.decodePortfolioWeightedPositions( optimizableItemValues );
			WFLagWeightedPositions wFLagWeightedPositions =
				new WFLagWeightedPositions(
				drivingWeightedPositions , portfolioWeightedPositions );

			return wFLagWeightedPositions;
		}
		#endregion
		#region GetFitnessValue
		public double GetFitnessValue(
			BruteForceOptimizableParameters bruteForceOptimizableItem )
		{
			double fitnessValue;
			WFLagWeightedPositions wFLagWeightedPositions =
				( WFLagWeightedPositions )this.Decode( bruteForceOptimizableItem );
			int optimizableItem = bruteForceOptimizableItem.Length;
			int decodedWeightedPositions =
				wFLagWeightedPositions.DrivingWeightedPositions.Count +
				wFLagWeightedPositions.PortfolioWeightedPositions.Count;
			if ( decodedWeightedPositions < optimizableItem )
				// the optimizable parameters object
				// contains a duplicate element either for
				// driving positions or for portfolio positions
				//fitnessValue = double.MinValue;
				fitnessValue = -0.2;
			else
				// all driving position parameters are distinct and
				// all portfolio position parameters are distinct
				fitnessValue =
					this.wFLagGenomeManager.GetFitnessValue( wFLagWeightedPositions );
			return fitnessValue;
		}
		#endregion
		private int getMinValueForParameter( int parameterPosition )
		{
			int minValueForParameter =
				-this.eligibleTickersForDrivingPositions.Rows.Count;
			if ( parameterPosition >= this.numberOfDrivingPositions )
				// the parameter is for a portfolio position
				minValueForParameter =
					-this.eligibleTickersForPortfolioPositions.Rows.Count;
			return minValueForParameter;
		}
		private int getMaxValueForParameter( int parameterPosition )
		{
			int maxValueForParameter =
				this.eligibleTickersForDrivingPositions.Rows.Count - 1;
			if ( parameterPosition >= this.numberOfDrivingPositions )
				// the parameter is for a portfolio position
				maxValueForParameter =
					this.eligibleTickersForPortfolioPositions.Rows.Count - 1;
			return maxValueForParameter;
		}
	}
}
