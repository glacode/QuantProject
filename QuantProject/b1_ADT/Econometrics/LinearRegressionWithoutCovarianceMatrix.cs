/*
QuantProject - Quantitative Finance Library

LinearRegressionWithoutCovarianceMatrix.cs
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

using QuantProject.ADT.LinearAlgebra;

namespace QuantProject.ADT.Econometrics
{
	/// <summary>
	/// Computes a Linear Regression, but it doesn't provide ANOVA (ANalysis Of VAriance)
	/// </summary>
	[Serializable]
	public class LinearRegressionWithoutCovarianceMatrix : ILinearRegression
	{
		private double sumOfSquareResiduals;
		private double centeredTotalSumOfSquares;

		private double[] estimatedCoefficients;
		
		public double[] EstimatedCoefficients {
			get
			{
				if ( this.estimatedCoefficients == null )
					throw new Exception(
						"The method RunRegression() must be invoked before " +
						"reading this property!" );
				return this.estimatedCoefficients;
			}
		}
		
		private double centeredRSquare;
		
		public double CenteredRSquare {
			get { return this.centeredRSquare; }
		}
		
		public LinearRegressionWithoutCovarianceMatrix()
		{
		}
		
		#region RunRegression
		private void runRegression_checkParameters( double[] regressand , double[,] regressors )
		{
			if ( regressand.Length != regressors.GetLength( 0 ) )
				throw new Exception(
					"The regressand doesn't have the same number of observations as " +
					"the regressors!" );
		}
		private double[,] getXtransposeX( double[,] X )
		{
			int n = X.GetLength( 0 );
			int k = X.GetLength( 1 );
			double[,] xTransposeX = new double[ k , k ];
			for( int i = 0 ; i < k ; i++ )
				for( int j = 0 ; j < k ; j++ )
			{
				xTransposeX[ i , j ] = 0;
				for( int t = 0 ; t < n ; t++ )
					xTransposeX[ i , j ] += X[ t , i ] * X[ t , j ];
			}
			return xTransposeX;
		}
		private double[] getXtransposeY( double[] y , double[,] X )
		{
			int n = y.Length;
			int k = X.GetLength( 1 );
			double[] xTransposeY = new double[ k ];
			for( int j = 0 ; j < k ; j++ )
			{
				xTransposeY[ j ] = 0;
				for( int t = 0 ; t < n ; t++ )
					xTransposeY[ j ] += X[ t , j ] * y[ t ];
			}
			return xTransposeY;
		}
		
		#region computeRSquare
		private double getYBar( double[] regressand )
		{
			double yBar = 0;
			for( int i = 0 ; i < regressand.Length ; i++ )
				yBar += regressand[ i ];
			yBar = yBar / regressand.Length;
			return yBar;
		}
		
		#region getYHat
		private double getYHat( double[] regressand , double[,] regressors , int i )
		{
			double returnValue = 0;
			for( int j = 0 ; j < regressors.GetLength( 1 ) ; j++ )
				returnValue += this.estimatedCoefficients[ j ] * regressors[ i , j ];
			return returnValue;
		}
		
		private double[] getYHat( double[] regressand , double[,] regressors )
		{
			double[] yHat = new double[ regressand.Length ];
			for( int i = 0 ; i < regressand.Length ; i++ )
				yHat[ i ] = this.getYHat( regressand , regressors , i );
			return yHat;
		}
		#endregion getYHat
		
		private void setSumOfSquareResidualsAndCenteredTotalSumOfSquares(
			double[] regressand , double[,] regressors , double yBar , double[] yHat )
		{
			this.sumOfSquareResiduals = 0;
			this.centeredTotalSumOfSquares = 0;
			for( int i = 0 ; i < regressand.Length ; i++ )
			{
				this.sumOfSquareResiduals +=
					( yHat[ i ] - regressand[ i ] ) * ( yHat[ i ] - regressand[ i ] );
				this.centeredTotalSumOfSquares +=
					( regressand[ i ] - yBar ) * ( regressand[ i ] - yBar );
			}
		}
		private void setSumOfSquareResidualsAndCenteredTotalSumOfSquares(
			double[] regressand , double[,] regressors )
		{
			double yBar = this.getYBar( regressand );
			double[] yHat = this.getYHat( regressand , regressors );
			this.setSumOfSquareResidualsAndCenteredTotalSumOfSquares(
				regressand , regressors , yBar , yHat );
		}
		private void computeCenteredRSquare(
			double[] regressand , double[,] regressors )
		{
			this.setSumOfSquareResidualsAndCenteredTotalSumOfSquares(
				regressand , regressors);
			this.centeredRSquare =
				1 - this.sumOfSquareResiduals / this.centeredTotalSumOfSquares;
		}
		#endregion computeRSquare
		
		public void RunRegression( double[] regressand , double[,] regressors )
		{
			this.runRegression_checkParameters( regressand , regressors );
			double[,] xTransposeX = this.getXtransposeX( regressors );
			double[] xTransposeY = this.getXtransposeY( regressand , regressors );
			this.estimatedCoefficients = LinearSystemSolver.FindSolution(
				xTransposeX , xTransposeY );
			this.computeCenteredRSquare( regressand , regressors );
		}
		#endregion RunRegression
	}
}
