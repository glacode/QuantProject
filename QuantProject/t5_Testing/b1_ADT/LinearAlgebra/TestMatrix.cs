/*
QuantProject - Quantitative Finance Library

TestLinearSystemSolver.cs
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
	/// Test for the class QuantProject.ADT.LinearAlgebra.Matrix
	/// </summary>
	public class TestMatrix
	{
		public TestMatrix()
		{
		}
		[Test]
		public void TestProductMatrixTimesVector()
		{
			double[,] x = new double[ 3 , 2 ] {
				{ 1 , 2 } ,
				{ 3 , 4 } ,
				{ 5 , 6 } };
			double[] y = new double[ 2 ] { 2 , 3 };
			
			double[] product = Matrix.Multiply( x , y );
			
			Assert.AreEqual( 8 , product[ 0 ] );
			Assert.AreEqual( 18 , product[ 1 ] );
			Assert.AreEqual( 28 , product[ 2 ] );
		}
		
		[Test]
		public void TestTransposeTheFirstMatrixAndMultiply()
		{
			double[,] x = new double[ 2 , 3 ] {
				{ 1 , 2 , 3 } ,
				{ 4 , 5 , 6 } };
			double[,] y = new double[ 2 , 2 ] {
				{ 1 , 2 } ,
				{ 3 , 4 } };
			
			double[,] product = Matrix.TransposeTheFirstMatrixAndMultiply( x , y );
			
			Assert.AreEqual( 3 , product.GetLength( 0 ) );
			Assert.AreEqual( 2 , product.GetLength( 1 ) );
			Assert.AreEqual( 13 , product[ 0 , 0 ] );
			Assert.AreEqual( 18 , product[ 0 , 1 ] );
			Assert.AreEqual( 17 , product[ 1 , 0 ] );
			Assert.AreEqual( 24 , product[ 1 , 1 ] );
			Assert.AreEqual( 21 , product[ 2 , 0 ] );
			Assert.AreEqual( 30 , product[ 2 , 1 ] );
		}
		
		[Test]
		public void TestTransposeTheSecondMatrixAndMultiply()
		{
			double[,] x = new double[ 2 , 2 ] {
				{ 1 , 2 } ,
				{ -3 , 4 } };
			double[,] y = new double[ 3 , 2 ] {
				{ -1 , 5 } ,
				{ 3 , 2 } ,
				{ -1 , 6 } };
			
			double[,] product = Matrix.TransposeTheSecondMatrixAndMultiply( x , y );
			
			Assert.AreEqual( 2 , product.GetLength( 0 ) );
			Assert.AreEqual( 3 , product.GetLength( 1 ) );
			Assert.AreEqual( 9 , product[ 0 , 0 ] );
			Assert.AreEqual( 7 , product[ 0 , 1 ] );
			Assert.AreEqual( 11 , product[ 0 , 2 ] );
			Assert.AreEqual( 23 , product[ 1 , 0 ] );
			Assert.AreEqual( -1 , product[ 1 , 1 ] );
			Assert.AreEqual( 27 , product[ 1 , 2 ] );
		}
	}
}
