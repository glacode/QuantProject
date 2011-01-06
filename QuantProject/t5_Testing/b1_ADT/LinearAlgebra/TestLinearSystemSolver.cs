/*
QuantProject - Quantitative Finance Library

TestLinearSystemSolver.cs
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
using NUnit.Framework;

using QuantProject.ADT.LinearAlgebra;

namespace QuantTesting.ADT.LinearAlgebra
{
	[TestFixture]
	/// <summary>
	/// Test for the class QuantProject.ADT.LinearAlgebra.LinearSystemSolver
	/// </summary>
	public class TestLinearSystemSolver
	{
		[Test]
		public void TestMethod()
		{
			double[,] A = new double[ 3 , 3 ] {
				{ 0 , 1 , 1 } ,
				{ 2 , 0 , 3 } ,
				{ 1 , 1 , 1 } };
			double[] b = new double[ 3 ] { 2 , 5 , 3 };
			
			double[] solution = LinearSystemSolver.FindSolution( A , b );
			
			Assert.AreEqual( 1 , solution[ 0 ] );
			Assert.AreEqual( 1 , solution[ 1 ] );
			Assert.AreEqual( 1 , solution[ 2 ] );
		}
		
		[Test]
		public void TestMethod1()
		{
			double[,] A = new double[ 3 , 3 ] {
				{ 2 , 1 , -1 } ,
				{ -3 , -1 , 2 } ,
				{ -2 , 1 , 2 } };
			double[] b = new double[ 3 ] { 8 , -11 , -3 };
			
			double[] solution = LinearSystemSolver.FindSolution( A , b );
			
			Assert.AreEqual( 2 , solution[ 0 ] );
			Assert.AreEqual( 3 , solution[ 1 ] );
			Assert.AreEqual( -1 , solution[ 2 ] , 0.0000001 );
		}
	}
}
