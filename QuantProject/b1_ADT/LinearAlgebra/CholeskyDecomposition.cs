/*
QuantProject - Quantitative Finance Library

CholeskyDecomposition.cs
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

namespace QuantProject.ADT.LinearAlgebra
{
	/// <summary>
	/// Takes a positive definite matrix and returns it's decomposition in the form LL^T,
	/// with L lower triangular
	/// </summary>
	public class CholeskyDecomposition
	{
		public CholeskyDecomposition()
		{
		}
		
		#region GetLowerTriangular
		public static double[,] GetLowerTriangular( double[,] positiveDefiniteMatrix )
		{
			int k = positiveDefiniteMatrix.GetLength( 0 );
			double[,] lowerTriangular = new double[ k , k ];
			for( int column = 0 ; column < k ; column++ )
			{
				// computes lowerTriangular[ column , column ]
				double partialSum = 0;
				for( int j = 0 ; j < column ; j++ )
					partialSum += lowerTriangular[ column , j ] * lowerTriangular[ column , j ];
				double numberToBeRootSquared = positiveDefiniteMatrix[ column , column ] - partialSum;
				if ( numberToBeRootSquared <= 0 )
					throw new Exception( "positiveDefiniteMatrix is actually not positive definite" );
				lowerTriangular[ column , column ] = Math.Sqrt( numberToBeRootSquared );
				
				// computes entries below lowerTriangular[ column , column ]
				for( int i = column + 1 ; i < k ; i++ )
				{
					double partialSumB = 0;
					for( int j = 0 ; j < column ; j++ )
						partialSumB += lowerTriangular[ i , j ] * lowerTriangular[ column , j ];
					lowerTriangular[ i , column ] =
						( positiveDefiniteMatrix[ i , column ] - partialSumB ) /
						lowerTriangular[ column , column ];
				}
			}
			return lowerTriangular;			
		}
		#endregion GetLowerTriangular
	}
}
