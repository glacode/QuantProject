/*
QuantProject - Quantitative Finance Library

Matrix.cs
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
	/// Provides methods for matrix algebra
	/// </summary>
	public class Matrix
	{
		public Matrix()
		{
		}
		
		#region Multiply
		/// <summary>
		/// multiplies a matrix times a vector, and returns a vector
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static double[] Multiply( double[,] x , double[] y )
		{
			if ( x.GetLength( 1 ) != y.Length )
				throw new Exception( "x and y are not conformable!" );
			int n = y.Length;
			int k = x.GetLength( 0 );
			double[] xTimesY = new double[ k ];
			for( int j = 0 ; j < k ; j++ )
			{
				xTimesY[ j ] = 0;
				for( int t = 0 ; t < n ; t++ )
					xTimesY[ j ] += x[ j , t ] * y[ t ];
			}
			return xTimesY;
		}
		#endregion Multiply
		
		/// <summary>
		/// returns xTranspose times y
		/// </summary>
		/// <param name="x">its transpose multiplies y</param>
		/// <param name="y">it's tpremultiplied by x transpose</param>
		/// <returns></returns>
		public static double[,] TransposeTheFirstMatrixAndMultiply( double[,] x , double [,] y )
		{
			if ( x.GetLength( 0 ) != y.GetLength( 0 ) )
				throw new Exception( "x and y are not conformable!" );
			// x is k x m, y is k x n
			int m = x.GetLength( 1 );
			int k = x.GetLength( 0 );
			int n = y.GetLength( 1 );
			double[,] xTransposeTimesY = new double[ m , n ];
			for( int i = 0 ; i < m ; i++ )
				for( int j = 0 ; j < n ; j++ )
			{
				double innerProduct = 0;
				for( int t = 0 ; t < k ; t++ )
					innerProduct += x[ t , i ] * y[ t , j ];
				xTransposeTimesY[ i , j ] = innerProduct;
			}
			return xTransposeTimesY;
		}
		
		/// <summary>
		/// returns x times yTranspose
		/// </summary>
		/// <param name="x">multiplies the transpose of y</param>
		/// <param name="y">it's transposed and then premultiplied by x</param>
		/// <returns></returns>
		public static double[,] TransposeTheSecondMatrixAndMultiply( double[,] x , double [,] y )
		{
			if ( x.GetLength( 1 ) != y.GetLength( 1 ) )
				throw new Exception( "x and y are not conformable!" );
			// x is m x k, y is n x k
			int m = x.GetLength( 0 );
			int k = x.GetLength( 1 );
			int n = y.GetLength( 0 );
			double[,] xTimesYTranspose = new double[ m , n ];
			for( int i = 0 ; i < m ; i++ )
				for( int j = 0 ; j < n ; j++ )
			{
				double innerProduct = 0;
				for( int t = 0 ; t < k ; t++ )
					innerProduct += x[ i , t ] * y[ j , t ];
				xTimesYTranspose[ i , j ] = innerProduct;
			}
			return xTimesYTranspose;
		}
	}
}
