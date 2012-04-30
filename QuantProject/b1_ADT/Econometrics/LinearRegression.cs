/*
QuantProject - Quantitative Finance Library

LinearRegression.cs
Copyright (C) 2011
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
	/// Computes a Linear Regression with covariance matrix
	/// and ANOVA (ANalysis Of VAriance)
	/// </summary>
	[Serializable]
	public class LinearRegression : ILinearRegression
	{
		private double[] regressand;
		private double[,] regressors;
		private double[,] covarianceMatrix;		// (XTransposeX)^-1
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
		
		#region HatMatrixDiagonal
		private double[] hatMatrixDiagonal;
		
		#region setHatMatrixDiagonal
		
		#region getHatMatrixDiagonal
		
		#region get_i_th_row_of_X_XtransposeX_inverse
		private double get_i_th_row_of_X_XtransposeX_inverse( int i , int j )
		{
			double partialSum = 0;
			for( int q = 0 ; q < this.regressors.GetLength( 1 ) ; q++ )
				partialSum += this.regressors[ i , q ] *
					this.covarianceMatrix[ q , j ];
			return partialSum;
		}

		private double[] get_i_th_row_of_X_XtransposeX_inverse( int i )
		{
			double[] i_th_row_of_X_XtransposeX_inverse = new double[ this.regressors.GetLength( 1 ) ];
			for( int j = 0 ; j < this.regressors.GetLength( 1 ) ; j++ )
				i_th_row_of_X_XtransposeX_inverse[ j ] =
					this.get_i_th_row_of_X_XtransposeX_inverse( i , j );
			return i_th_row_of_X_XtransposeX_inverse;
		}
		#endregion get_i_th_row_of_X_XtransposeX_inverse
		
		private double getHatMatrixDiagonal( int i )
		{
			double[] i_th_row_of_X_XtransposeX_inverse =
				this.get_i_th_row_of_X_XtransposeX_inverse( i );
			double partialSum = 0;
			for( int j = 0 ; j < this.regressors.GetLength( 1 ) ; j++ )
				partialSum += i_th_row_of_X_XtransposeX_inverse[ j ] *
					this.regressors[ i , j ];
			return partialSum;
		}
		#endregion getHatMatrixDiagonal
		
		private void setHatMatrixDiagonal()
		{
			this.hatMatrixDiagonal = new double[ this.regressand.Length ];
			for( int i = 0 ; i < hatMatrixDiagonal.Length ; i++ )
				this.hatMatrixDiagonal[ i ] = this.getHatMatrixDiagonal( i );
		}
		#endregion setHatMatrixDiagonal
		
		/// <summary>
		/// returns the diagonal of the hat matrix
		/// </summary>
		public double[] HatMatrixDiagonal
		{
			get{
				if ( this.hatMatrixDiagonal == null )
					this.setHatMatrixDiagonal();
				return this.hatMatrixDiagonal;
			}
		}
		#endregion HatMatrixDiagonal
		
		private double centeredRSquare;
		
		public double CenteredRSquare {
			get { return this.centeredRSquare; }
		}
		
		#region Residuals
		
		private double[] residuals;
		
		#region setResiduals
		
		#region setResidual
		
		private double getPredictedValue( int i )
		{
			double predictedValue = 0;
			for ( int j = 0 ; j < this.EstimatedCoefficients.Length ; j++ )
				predictedValue +=
					this.estimatedCoefficients[ j ] * this.regressors[ i , j ];
			return predictedValue;
		}
		
		private void setResidual( int i )
		{
			double predictedValue = this.getPredictedValue( i );
			this.residuals[ i ] = this.regressand[ i ] - predictedValue;
		}
		#endregion setResidual
		
		private void setResiduals()
		{
			this.residuals = new double[ this.regressand.Length ];
			for( int i = 0 ; i < this.regressand.Length ; i++ )
				this.setResidual( i );
		}
		#endregion setResiduals
		
		public double[] Residuals {
			get {
				if ( this.residuals == null )
					this.setResiduals();
				return this.residuals; }
		}
		#endregion Residuals

		
		#region CenteredTotalSumOfSquares
		
		#region setCenteredTotalSumOfSquares
		private double getYBar()
		{
			double partialSum = 0;
			for( int i = 0 ; i < this.regressand.Length ; i++ )
				partialSum += this.regressand[ i ];
			
			double yBar = partialSum / this.regressand.Length;
			return yBar;
		}
		private void setCenteredTotalSumOfSquares()
		{
			double partialSum = 0;
			double yBar = this.getYBar();
			for( int i = 0 ; i < this.regressand.Length ; i++ )
				partialSum += Math.Pow( this.regressand[ i ] - yBar , 2 );
			this.centeredTotalSumOfSquares = partialSum;
		}
		#endregion setCenteredTotalSumOfSquares
		
		public double CenteredTotalSumOfSquares
		{
			get
			{
				if ( this.centeredTotalSumOfSquares == double.MinValue )
					this.setCenteredTotalSumOfSquares();
				return this.centeredTotalSumOfSquares;	
			}
		}
		#endregion CenteredTotalSumOfSquares
		
		#region PredictedResidualsSumOfSquares
		
		private double predictedResidualsSumOfSquares;

		#region setPredictedResidualsSumOfSquares
		private double getExternalResidual( int i )
		{
			double externalResidual = this.Residuals[ i ] / ( 1 - this.HatMatrixDiagonal[ i ] );
			return externalResidual;
		}
		private void setPredictedResidualsSumOfSquares()
		{
			double partialSum = 0;
			for( int i = 0 ; i < this.regressand.Length ; i++ )
				partialSum += Math.Pow( this.getExternalResidual( i ) , 2 );
			this.predictedResidualsSumOfSquares = partialSum;
		}
		#endregion setPredictedResidualsSumOfSquares
		
		/// <summary>
		///  the PRESS statistic, a statistic based on the leave-one-out technique
		/// </summary>
		public double PredictedResidualsSumOfSquares {
			get {
				if ( this.predictedResidualsSumOfSquares == Double.MinValue )
					// the PRESS statistic has not been set yet
					this.setPredictedResidualsSumOfSquares();
				return this.predictedResidualsSumOfSquares; }
		}
		#endregion PredictedResidualsSumOfSquares
		
		
		/// <summary>
		/// similar to the Total Sum of Squares, but each observation is subtracted
		/// the mean of the other observations: in other words, the current observation
		/// does not partecipate to the computationo of the current mean. It has been
		/// proven that the Prodicted Total Sum Of Squares (defined as above) is equal to
		/// the (usual) Centered Total Sum of Squares times (n/(n-1))^2
		/// </summary>
		public double PredictedCenteredTotalSumOfSquares
		{
			get
			{
				double n = Convert.ToDouble( this.regressand.Length );
				double predictedTotalSumOfSquares =
					this.CenteredTotalSumOfSquares * Math.Pow(  n / ( n - 1 ) , 2 );
				return predictedTotalSumOfSquares;
			}
		}

		/// <summary>
		/// a normalized version of the the PRESS statistics
		/// </summary>
		public double CenteredPSquare {
			get
			{
				double centerdPSquare = 1 - this.PredictedResidualsSumOfSquares /
					this.PredictedCenteredTotalSumOfSquares;
				return centerdPSquare;
			}
		}
		
		public LinearRegression()
		{
			this.predictedResidualsSumOfSquares = double.MinValue;
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
//		private double[] getXtransposeY( double[] y , double[,] X )
//		{
//			int n = y.Length;
//			int k = X.GetLength( 1 );
//			double[] xTransposeY = new double[ k ];
//			for( int j = 0 ; j < k ; j++ )
//			{
//				xTransposeY[ j ] = 0;
//				for( int t = 0 ; t < n ; t++ )
//					xTransposeY[ j ] += X[ t , j ] * y[ t ];
//			}
//			return xTransposeY;
//		}
		
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
			this.regressand = regressand;
			this.regressors = regressors;
			double[,] xTransposeX = this.getXtransposeX( regressors );
			this.covarianceMatrix = PositiveDefiniteMatrix.GetInverse( xTransposeX );
			double[,] xTransposeXInverseXTranspose =
				Matrix.TransposeTheSecondMatrixAndMultiply( this.covarianceMatrix , regressors );
//			double[] xTransposeY = this.getXtransposeY( regressand , regressors );
			this.estimatedCoefficients = Matrix.Multiply( xTransposeXInverseXTranspose , regressand );
			this.computeCenteredRSquare( regressand , regressors );
		}
		#endregion RunRegression
	}
}
