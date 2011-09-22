/*
QuantProject - Quantitative Finance Library

EntryStrategyBasedOnForecastedReturn.cs
Copyright (C) 2010
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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement.Time;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Two positions are chosen, if the forecasted return is at least
	/// a given threshold
	/// </summary>
	[Serializable]
	public class EntryStrategyBasedOnForecastedReturn : IEntryStrategy
	{
		protected double minAverageExpectedReturn;
		private ILinearRegressionFitnessEvaluator fitnessEvaluator;
		private IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling;
		private HistoricalMarketValueProvider historicalMarketValueProvider;
//		private bool haveParametersBeenSetAfterLastGetPositions;
		
//		TestingPositions[] bestTestingPositionsInSample;
		
		
		/// <summary>
		/// Two positions are chosen, if the forecasted return is at least
		/// a given threshold
		/// </summary>
		/// <param name="minAverageExpectedReturn"></param>
		public EntryStrategyBasedOnForecastedReturn(
			double minAverageExpectedReturn ,
			ILinearRegressionFitnessEvaluator fitnessEvaluator ,
			IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling ,
			HistoricalMarketValueProvider historicalMarketValueProvider )
		{
			this.minAverageExpectedReturn = minAverageExpectedReturn;
			this.fitnessEvaluator = fitnessEvaluator;
			this.returnIntervalSelectorForSignaling = returnIntervalSelectorForSignaling;
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			
//			this.haveParametersBeenSetAfterLastGetPositions = false;
		}
		
//		public void SetParametersForGetPositionsToBeOpened(
//			TestingPositions[] bestTestingPositionsInSample )
//		{
//			this.bestTestingPositionsInSample = bestTestingPositionsInSample;
//			this.haveParametersBeenSetAfterLastGetPositions = true;
//		}
		
		#region GetPositionsToBeOpened
		
		#region getPositionsToBeOpened
		
		#region tryThisCandidate
		
		private bool wereAllSignalingTickersExchanged(
			LinearRegressionTestingPositions candidate ,
			ReturnInterval outOfSampleReturnIntervalForSignaling )
		{
			bool wereExchanged =
				( this.historicalMarketValueProvider.WereAllExchanged(
					candidate.SignalingTickers , outOfSampleReturnIntervalForSignaling.Begin )
				 && this.historicalMarketValueProvider.WereAllExchanged(
				 	candidate.SignalingTickers , outOfSampleReturnIntervalForSignaling.End ) );
			return wereExchanged;
		}
		
		#region computeForecastedReturn
		private double[] getOutOfSampleValuesForSignalingPortfolios(
			LinearRegressionTestingPositions candidate ,
			ReturnInterval outOfSampleReturnIntervalForSignaling )
		{
//			ReturnInterval outOfSampleReturnIntervalSignaling =
//				this.returnIntervalSelectorForSignaling.GetReturnIntervalUsedForSignaling(
//					outOfSampleReturnIntervalForTrading );
			double[] outOfSampleValuesForIndependentVariables =
				this.fitnessEvaluator.GetIndependentVariablesValues(
					candidate ,
					outOfSampleReturnIntervalForSignaling ,
					this.historicalMarketValueProvider );
			return outOfSampleValuesForIndependentVariables;
		}
		private double computeForecastedReturnOnValidReturnIntervalForSignaling(
			LinearRegressionTestingPositions candidate ,
			double[] outOfSampleValuesForSignalingPortfolios )
		{
			// the first coefficient is for the constant regressor
			double forecastedReturn = candidate.LinearRegression.EstimatedCoefficients[ 0 ];
			
			for ( int i = 1;
			     i < candidate.LinearRegression.EstimatedCoefficients.Length ;
			     i++ )
				forecastedReturn +=
					candidate.LinearRegression.EstimatedCoefficients[ i ] *
					outOfSampleValuesForSignalingPortfolios[ i - 1 ];
			return forecastedReturn;
		}
		private double computeForecastedReturn(
			LinearRegressionTestingPositions candidate ,
			ReturnInterval outOfSampleReturnIntervalForSignaling )
		{
			double forecastedReturn = double.MinValue;
			double[] outOfSampleValuesForSignalingPortfolios =
				this.getOutOfSampleValuesForSignalingPortfolios(
					candidate , outOfSampleReturnIntervalForSignaling );
			if ( outOfSampleValuesForSignalingPortfolios != null )
				// all signaling tickers where actually exchanged on the
				// given outOfSampleReturnIntervalForSignaling
				forecastedReturn = this.computeForecastedReturnOnValidReturnIntervalForSignaling(
					candidate , outOfSampleValuesForSignalingPortfolios );
			return forecastedReturn;
		}
		#endregion computeForecastedReturn
		
		protected virtual WeightedPositions tryThisCandidate(
			LinearRegressionTestingPositions candidate , double expectedReturn )
		{
			WeightedPositions weightedPositionsToBeOpened = null;
			if ( expectedReturn >= this.minAverageExpectedReturn )
				weightedPositionsToBeOpened = candidate.WeightedPositions;
			return weightedPositionsToBeOpened;
		}
		
		private WeightedPositions tryThisCandidate(
			LinearRegressionTestingPositions candidate ,
			ReturnInterval outOfSampleReturnIntervalForSignaling )
		{
			WeightedPositions weightedPositionsToBeOpened = null;
			if ( this.wereAllSignalingTickersExchanged (
				candidate , outOfSampleReturnIntervalForSignaling ) )
			{
				double expectedReturn = this.computeForecastedReturn(
					candidate , outOfSampleReturnIntervalForSignaling );
				weightedPositionsToBeOpened = this.tryThisCandidate( candidate , expectedReturn );
//				if ( expectedReturn >= this.minAverageExpectedReturn )
//					weightedPositionsToBeOpened = candidate.WeightedPositions;
			}
			return weightedPositionsToBeOpened;
		}
		private WeightedPositions tryThisCandidate(
			int indexOfTheCurrentCandidate ,
			TestingPositions[] bestTestingPositionsInSample ,
			ReturnInterval outOfSampleReturnIntervalForSignaling )
		{
			LinearRegressionTestingPositions candidate =
				(LinearRegressionTestingPositions)bestTestingPositionsInSample[
					indexOfTheCurrentCandidate ];
			WeightedPositions weightedPositionsToBeOpened =
				this.tryThisCandidate(
					candidate , outOfSampleReturnIntervalForSignaling );
			return weightedPositionsToBeOpened;
		}
		#endregion tryThisCandidate
		private WeightedPositions getPositionsToBeOpened(
			TestingPositions[] bestTestingPositionsInSample ,
			ReturnInterval outOfSampleReturnIntervalForSignaling )
		{
			WeightedPositions weightedPositionsToBeOpened = null;
			int indexOfTheCurrentCandidate = 0;
			while ( ( weightedPositionsToBeOpened == null ) &&
			       ( indexOfTheCurrentCandidate < bestTestingPositionsInSample.Length ) )
			{
				weightedPositionsToBeOpened = this.tryThisCandidate(
					indexOfTheCurrentCandidate , bestTestingPositionsInSample ,
					outOfSampleReturnIntervalForSignaling );
				indexOfTheCurrentCandidate++;
			}
			return weightedPositionsToBeOpened;
		}
		#endregion getPositionsToBeOpened
		
		public WeightedPositions GetPositionsToBeOpened(
			TestingPositions[] bestTestingPositionsInSample ,
			ReturnIntervals outOfSampleReturnIntervals )
		{
			ReturnInterval outOfSampleReturnIntervalForTrading =
				outOfSampleReturnIntervals.LastInterval;
			ReturnInterval outOfSampleReturnIntervalForSignaling =
				this.returnIntervalSelectorForSignaling.GetReturnIntervalUsedForSignaling(
					outOfSampleReturnIntervalForTrading );
//			if ( !this.haveParametersBeenSetAfterLastGetPositions )
//				throw new Exception(
//					"The method SetParametersForGetPositionsToBeOpened() has to be called " +
//					"before calling the method GetPositionsToBeOpened()  ! " );
//			else
			WeightedPositions positionsToBeOpened =
				this.getPositionsToBeOpened(
					bestTestingPositionsInSample , outOfSampleReturnIntervalForSignaling );
//			this.haveParametersBeenSetAfterLastGetPositions = false;
			return positionsToBeOpened;
		}
		#endregion GetPositionsToBeOpened
	}
}
