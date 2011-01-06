/*
QuantProject - Quantitative Finance Library

LowerTriangular.cs
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
	/// Provides static methods for lower triangular matrices
	/// </summary>
	public class LowerTriangular
	{
		public LowerTriangular()
		{
		}
		
		#region GetInverse
		/// <summary>
		/// returnes the inverse of the given lower kxk triangular matrix
		/// </summary>
		/// <param name="lowerTriangular"></param>
		static public double[,] GetInverse( double[,] lowerTriangular )
		{
			int k = lowerTriangular.GetLength( 0 );
			double[,] inverse = new double[ k , k ];
			for( int column = 0 ; column < k ; column++ )
			{
				if ( lowerTriangular[ column , column ] == 0 )
					throw new Exception( "lowerTriangular is a singular matrix!" );
				inverse[ column , column ] = 1 / lowerTriangular[ column , column ];
				for( int row = column + 1 ; row < k ; row++ )
				{
					double numerator = 0;
					for( int h = column ; h < row ; h++ )
						numerator += lowerTriangular[ row , h ] * inverse[ h , column ];
					if ( lowerTriangular[ row , row ] == 0 )
						throw new Exception( "lowerTriangular is a singular matrix!" );
					inverse[ row , column ] = - numerator / lowerTriangular[ row , row ];
				}
			}
			return inverse;
		}
		#endregion GetInverse
	}
}
