/*
QuantProject - Quantitative Finance Library

TestCholeskyDecomposition.cs
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
using NUnit.Framework;

using QuantProject.ADT.LinearAlgebra;

namespace QuantTesting.ADT.LinearAlgebra
{
	[TestFixture]
	/// <summary>
	/// Test for the class QuantProject.ADT.LinearAlgebra.CholeskyDecomposition
	/// </summary>
	public class TestCholeskyDecomposition
	{
		public TestCholeskyDecomposition()
		{
		}
		[Test]
		public void TestGetLowerTriangular()
		{
			double[,] positiveDefiniteMatrix = new double[ 3 , 3 ] {
				{ 9 , -6 , 12 } ,
				{ -6 , 5 , -11 } ,
				{ 12 , -11 , 29 } };

			double[,] lowerTriangular = CholeskyDecomposition.GetLowerTriangular( positiveDefiniteMatrix );

			// the cholescky decomposition is
			//	3	0	0
			//	-2	1	0
			//	4	-3	2			
			Assert.AreEqual( 3 , lowerTriangular.GetLength( 0 ) );
			Assert.AreEqual( 3 , lowerTriangular.GetLength( 1 ) );
			Assert.AreEqual( 3 , lowerTriangular[ 0 , 0 ] );
			Assert.AreEqual( 0 , lowerTriangular[ 0 , 1 ] );
			Assert.AreEqual( 0 , lowerTriangular[ 0 , 2 ] );
			Assert.AreEqual( -2 , lowerTriangular[ 1 , 0 ] );
			Assert.AreEqual( 1 , lowerTriangular[ 1 , 1 ] );
			Assert.AreEqual( 0 , lowerTriangular[ 1 , 2 ] );
			Assert.AreEqual( 4 , lowerTriangular[ 2 , 0 ] );
			Assert.AreEqual( -3 , lowerTriangular[ 2 , 1 ] );
			Assert.AreEqual( 2 , lowerTriangular[ 2 , 2 ] );
		}
	}
}
