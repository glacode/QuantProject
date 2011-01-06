/*
QuantProject - Quantitative Finance Library

TestPositiveDefiniteMatrix.cs
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
	/// Test for the class QuantProject.ADT.LinearAlgebra.PositiveDefiniteMatrix
	/// </summary>
	public class TestPositiveDefiniteMatrix
	{
		public TestPositiveDefiniteMatrix()
		{
		}
		[Test]
		public void TestGetInverse()
		{
			double[,] positiveDefiniteMatrix = new double[ 3 , 3 ] {
				{ 9 , -6 , 12 } ,
				{ -6 , 5 , -11 } ,
				{ 12 , -11 , 29 } };

			double[,] inverse = PositiveDefiniteMatrix.GetInverse( positiveDefiniteMatrix );

			// the inverse is
			//	2/3		7/6		1/6
			//	7/6		13/4	3/4
			//	1/6		3/4		1/4			
			Assert.AreEqual( 3 , inverse.GetLength( 0 ) );
			Assert.AreEqual( 3 , inverse.GetLength( 1 ) );
			Assert.AreEqual( 2.0/3.0 , inverse[ 0 , 0 ] , 0.00000001 );
			Assert.AreEqual( 7.0/6.0 , inverse[ 0 , 1 ] , 0.00000001);
			Assert.AreEqual( 1.0/6.0 , inverse[ 0 , 2 ] , 0.00000001 );
			Assert.AreEqual( 7.0/6.0 , inverse[ 1 , 0 ] , 0.00000001 );
			Assert.AreEqual( 13.0/4.0 , inverse[ 1 , 1 ] );
			Assert.AreEqual( 3.0/4.0 , inverse[ 1 , 2 ] );
			Assert.AreEqual( 1.0/6.0 , inverse[ 2 , 0 ] , 0.00000001 );
			Assert.AreEqual( 3.0/4.0 , inverse[ 2 , 1 ] );
			Assert.AreEqual( 1.0/4.0 , inverse[ 2 , 2 ] );
		}
	}
}
