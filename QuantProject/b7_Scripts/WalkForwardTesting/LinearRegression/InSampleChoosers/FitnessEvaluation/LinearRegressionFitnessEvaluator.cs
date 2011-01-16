/*
QuantProject - Quantitative Finance Library

LinearRegressionFitnessEvaluator.cs
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

using QuantProject.ADT.Econometrics;
using QuantProject.ADT.Statistics;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Computes (in sample) the OLS estimate of the following regression:
	/// the regressand is the return of the TradingPortfolio; the regressors
	/// are the constant and the SignalingPortfolios
	/// </summary>
	[Serializable]
	public class LinearRegressionFitnessEvaluator : // MarshalByRefObject ,	// MarshalByRefObject added for testing
	ILinearRegressionFitnessEvaluator
	{
//		private bool hasReturnsManagerForTradingTickersBeenSetSinceLastGetFitnessValueRequest;
//		private bool hasReturnsManagerForSignalingTickersBeenSetSinceLastGetFitnessValueRequest;
		
//		private ILinearRegressionValues linearRegressionValues;
		private ILinearRegressionSetupManager linearRegressionSetupManger;
		
		private IReturnsManager returnsManagerForTradingTickers;
		
		public IReturnsManager ReturnsManagerForTradingTickers {
			get { return this.returnsManagerForTradingTickers; }
			set
			{
				this.returnsManagerForTradingTickers = value;
//				this.hasReturnsManagerForTradingTickersBeenSetSinceLastGetFitnessValueRequest =
//					true;
			}
		}
		
		private IReturnsManager returnsManagerForSignalingTickers;
		
		public IReturnsManager ReturnsManagerForSignalingTickers {
			get { return this.returnsManagerForSignalingTickers; }
			set
			{
				this.returnsManagerForSignalingTickers = value;
//				this.hasReturnsManagerForSignalingTickersBeenSetSinceLastGetFitnessValueRequest =
//					true;
			}
		}
		
		public string Description
		{
			get
			{
				return "lnrRgrssnRSquare";
			}
		}

		public LinearRegressionFitnessEvaluator(
			ILinearRegressionSetupManager linearRegressionSetupManger )
		{
			this.linearRegressionSetupManger = linearRegressionSetupManger;
//			this.hasReturnsManagerForTradingTickersBeenSetSinceLastGetFitnessValueRequest =
//				false;
//			this.hasReturnsManagerForSignalingTickersBeenSetSinceLastGetFitnessValueRequest =
//				false;
		}
		
		public QuantProject.ADT.Econometrics.ILinearRegression SetUpAndRunLinearRegression(
			LinearRegressionTestingPositions testingPositions )
		{
			ILinearRegressionValues linearRegressionValues =
				this.linearRegressionSetupManger.SetUpTheLinearRegressionValues(
					testingPositions.TradingPortfolio , testingPositions.SignalingPortfolios ,
					this.returnsManagerForTradingTickers ,
					this.returnsManagerForSignalingTickers );
//			QuantProject.ADT.Econometrics.LinearRegression linearRegression =
//				new QuantProject.ADT.Econometrics.LinearRegressionWithoutCovarianceMatrix();
			QuantProject.ADT.Econometrics.LinearRegression linearRegression =
				new QuantProject.ADT.Econometrics.LinearRegression();
			linearRegression.RunRegression(
				linearRegressionValues.Regressand ,
				linearRegressionValues.Regressors );
//				linearRegressionValues.RegressorWeights );
			return linearRegression;
		}
		
		#region GetFitnessValue
		private void getFitnessValue_checkParameters( object meaning )
		{
			if ( !( meaning is TestingPositionsForUndecodableEncoded ) &&
			    !( meaning is LinearRegressionTestingPositions ) )
				throw new Exception(
					"The meaning should always be a LinearRegressionTestingPositions!" );
			if ( this.returnsManagerForTradingTickers == null )
				throw new Exception(
					"The property ReturnsManagerForTradingTickers has not been set. It must " +
					"be set before invoking GetFitnessValue()!" );
			if ( this.returnsManagerForSignalingTickers == null )
				throw new Exception(
					"The property ReturnsManagerForSignalingTickers has not been set. It must " +
					"be set before invoking GetFitnessValue()!" );
		}

		#region getFitnessValue

		private double getFitnessValue( LinearRegressionTestingPositions testingPositions )
		{
			QuantProject.ADT.Econometrics.ILinearRegression linearRegression =
				this.SetUpAndRunLinearRegression( testingPositions );
			double fitnessValue = linearRegression.CenteredRSquare;
			return fitnessValue;
//
//			double fitnessValue;
//			fitnessValue = this.getFitnessValue(
//				testingPositions.TradingPortfolio ,
//				testingPositions.SignalingPortfolios );
//			return fitnessValue;
		}
		#endregion getFitnessValue
		
		
		public double GetFitnessValue( object meaning , IReturnsManager returnsManager )
		{
			this.getFitnessValue_checkParameters( meaning );
			double fitnessValue = -3;  // value if the encoded was not properly decodable
			if ( !(meaning is TestingPositionsForUndecodableEncoded ) )
				// the encoded was actually decodable; meaning is a LinearRegressionTestingPositions
				
				// this method doesn't use returnsManager. It uses
				// this.returnsManagerForTradingTickers and this.returnsMangerForSignalingTickers instead
				fitnessValue =
					this.getFitnessValue( (LinearRegressionTestingPositions)meaning );
			if ( fitnessValue < -0.03 && fitnessValue > -2 )
			{
				;  // for breakpoint
			}
			return fitnessValue;
		}
		#endregion GetFitnessValue
		
		#region GetIndependentVariablesValues
		private bool wereAllSignalingTickersExchanged(
			LinearRegressionTestingPositions linearRegressionTestingPositions ,
			ReturnInterval outOfSampleReturnIntervalForSignaling ,
			HistoricalMarketValueProvider historicalMarketValueProvider )
		{
			bool wereExchanged =
				( historicalMarketValueProvider.WereAllExchanged(
					linearRegressionTestingPositions.SignalingTickers , outOfSampleReturnIntervalForSignaling.Begin )
				 && historicalMarketValueProvider.WereAllExchanged(
				 	linearRegressionTestingPositions.SignalingTickers , outOfSampleReturnIntervalForSignaling.End ) );
			return wereExchanged;
		}

		private double[] getIndependentVariablesValuesOnValidReturnedInterval(
			LinearRegressionTestingPositions linearRegressionTestingPositions ,
			ReturnInterval returnInterval ,
			HistoricalMarketValueProvider historicalMarketValueProvider )
		{
			double[] independentVariablesValues =
//				new double[ weightedPositions.Count - 2 + 1 ];  // one is added for the constant
				new double[ linearRegressionTestingPositions.SignalingPortfolios.Length + 1 ]; // one is added for the constant
			independentVariablesValues[ 0 ] = 0; // regressors include the constant and the constant's return is zero
			ReturnsManager returnsManager = new ReturnsManager(
				new ReturnIntervals( returnInterval ) , historicalMarketValueProvider );
			for( int j = 0 ; j < linearRegressionTestingPositions.SignalingPortfolios.Length ; j++ )
			{
				WeightedPositions portfolioForCurrentIndependentVariable =
					linearRegressionTestingPositions.SignalingPortfolios[ j ];
				independentVariablesValues[ j + 1 ] =
					portfolioForCurrentIndependentVariable.GetReturn( 0 , returnsManager );
			}
			return independentVariablesValues;
		}
		/// <summary>
		/// returns an array with the values for the independent variables. This is
		/// going to be used out of sample and it should be coherent with how
		/// the regressors are built in sample. If
		/// </summary>
		public virtual double[] GetIndependentVariablesValues(
			LinearRegressionTestingPositions linearRegressionTestingPositions ,
			ReturnInterval outOfSampleReturnIntervalForSignaling ,
			HistoricalMarketValueProvider historicalMarketValueProvider )
		{
			double[] independentVariablesValues = null;
			if ( this.wereAllSignalingTickersExchanged (
				linearRegressionTestingPositions , outOfSampleReturnIntervalForSignaling ,
				historicalMarketValueProvider ) )
				independentVariablesValues =
					this.getIndependentVariablesValuesOnValidReturnedInterval(
						linearRegressionTestingPositions , outOfSampleReturnIntervalForSignaling ,
						historicalMarketValueProvider );
			return independentVariablesValues;
		}
		#endregion GetIndependentVariablesValues
	}
}
