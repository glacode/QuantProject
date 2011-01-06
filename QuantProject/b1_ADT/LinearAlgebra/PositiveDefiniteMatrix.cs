/*
 * Created by SharpDevelop.
 * User: Glauco
 * Date: 30/12/2010
 * Time: 19.05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace QuantProject.ADT.LinearAlgebra
{
	/// <summary>
	/// Provides static methods for positive definite matrices
	/// </summary>
	public class PositiveDefiniteMatrix
	{
		public PositiveDefiniteMatrix()
		{
		}
		
		#region Invert
		private static double[,] getInverse( double[,] lowerTriangular )
		{
			double[,] lowerTriangularInverse = LowerTriangular.GetInverse( lowerTriangular );
			double[,] inverse = Matrix.TransposeTheFirstMatrixAndMultiply( lowerTriangularInverse , lowerTriangularInverse );
			return inverse;
		}
	
		/// <summary>
		/// inverts a positive definite matrix
		/// </summary>
		/// <param name="positiveDefiniteMatrix"></param>
		/// <returns></returns>
		public static double[,] GetInverse( double[,] positiveDefiniteMatrix )
		{
			double[,] lowerTriangular = CholeskyDecomposition.GetLowerTriangular(
				positiveDefiniteMatrix );
			
			double[,] inverse = PositiveDefiniteMatrix.getInverse( lowerTriangular );
			return inverse;
		}
		#endregion Invert
	}
}
