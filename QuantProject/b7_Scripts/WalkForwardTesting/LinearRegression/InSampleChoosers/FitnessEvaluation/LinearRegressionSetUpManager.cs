/*
QuantProject - Quantitative Finance Library

LinearRegressionSetUpManager.cs
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

using QuantProject.ADT.Collections;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Sets up a linear regression so that it then can be evaluated
	/// </summary>
	[Serializable]
	public class LinearRegressionSetupManager : ILinearRegressionSetupManager
	{


		public LinearRegressionSetupManager()
		{
		}
		
		#region SetUpTheLinearRegression
		
		#region getRegressand

		protected double[] getRegressand(
			IReturnsCalculator returnsCalculatorForTheRegressand , IReturnsManager returnsManager )
		{
//			WeightedPositions weightedPositionsForRegressand =
//				this.getWeightedPositionsForRegressand(
//					weightedPositions , returnsManager );
			float[] floatReturns = returnsCalculatorForTheRegressand.GetReturns(
				returnsManager , 0 , returnsManager.NumberOfReturns - 1 );
			double[] returns = FloatArrayManager.ConvertToDouble( floatReturns );
			return returns;
		}
		#endregion getRegressand
		
		#region getRegressors
		private void addConstantRegressor( double[,] regressors )
		{
			for( int j = 0 ; j < regressors.GetLength( 0 ) ; j++ )
				regressors[ j , 0 ] = 1;
		}
		
		#region addVariableRegressors
		
		#region addVariableRegressor
		private void addVariableRegressor(
			int regressorIndex ,
			IReturnsCalculator[] returnCalculatorForTheRegressors ,
			IReturnsManager returnsManager ,
			double[,] regressors )
		{
			IReturnsCalculator returnCalculatorForCurrentRegressor =
				returnCalculatorForTheRegressors[ regressorIndex - 1 ];  // the first regressor is the constant
//				this.getWeightedPositionsForRegressor(
//					regressorIndex , weightedPositions , returnsManager );
			for( int j = 0 ; j < regressors.GetLength( 0 ) ; j++ )
				regressors[ j , regressorIndex ] =
					returnCalculatorForCurrentRegressor.GetReturn( j , returnsManager );
		}
		#endregion addVariableRegressor
		
		private void addVariableRegressors(
			IReturnsCalculator[] returnCalculatorForTheRegressors ,
			IReturnsManager returnsManager ,
			double[,] regressors )
		{
			for( int i = 1 ; i < regressors.GetLength( 1 ) ; i++ )
				this.addVariableRegressor(
					i , returnCalculatorForTheRegressors , returnsManager , regressors );
		}
		#endregion addVariableRegressors
		
		protected double[,] getRegressors(
			IReturnsCalculator[] returnCalculatorForTheRegressors ,
			IReturnsManager returnsManagerForTheRegressor )
		{
			int numberOfObservations = returnsManagerForTheRegressor.NumberOfReturns;
			int numberOfRegressors = returnCalculatorForTheRegressors.Length + 1;  // one is added for the constant
			double[,] regressors = new double[ numberOfObservations , numberOfRegressors ];
			this.addConstantRegressor( regressors );
			this.addVariableRegressors(
				returnCalculatorForTheRegressors , returnsManagerForTheRegressor , regressors );
			return regressors;
		}
		#endregion getRegressors
		
		private double[] getRegressionWeights( int numberOfObservations )
		{
			double[] regressionWeights = new double[ numberOfObservations ];
			for( int i = 0 ; i < numberOfObservations ; i++ )
				regressionWeights[ i ] = 1;
			return regressionWeights;
		}
		
		/// <summary>
		/// Sets up a linear regression so that it then can be evaluated.
		/// A constant regressor is always added
		/// </summary>
		/// <param name="weightedPositionsForTheRegressand"></param>
		/// <param name="weightedPositionsForTheRegressors"></param>
		/// <param name="returnsManager"></param>
		public ILinearRegressionValues SetUpTheLinearRegressionValues(
			IReturnsCalculator returnsCalculatorForTheRegressand ,
			IReturnsCalculator[] returnCalculatorForTheRegressors ,
			IReturnsManager returnsManagerForTheRegressand ,
			IReturnsManager returnsManagerForTheRegressor )
		{
			double[] regressand = this.getRegressand(
				returnsCalculatorForTheRegressand , returnsManagerForTheRegressand );
			double[,] regressors = this.getRegressors(
				returnCalculatorForTheRegressors , returnsManagerForTheRegressor );
			double[] regressionWeights = this.getRegressionWeights( regressand.Length );
//			QuantProject.ADT.Econometrics.LinearRegression linearRegression =
//				new QuantProject.ADT.Econometrics.LinearRegression();
//			linearRegression.Regress( regressand , regressors , regressionWeights );
			LinearRegressionValues linearRegressionValues = new LinearRegressionValues(
				regressand , regressors , regressionWeights );
			return linearRegressionValues;
		}
		#endregion SetUpTheLinearRegression
	}
}
