/*
QuantProject - Quantitative Finance Library

TestLinearRegression.cs
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

using QuantProject.ADT.Econometrics;

namespace QuantTesting.ADT.Econometrics
{
	[TestFixture]
	/// <summary>
	/// Test for the class QuantProject.ADT.Econometrics.LinearRegression
	/// </summary>
	public class TestLinearRegression
	{
		[Test]
		public void TestMethod()
		{
			double[] y = new double[] { 20.10 , 13.30 , 24.40 , 24.95 };

			double[,] X = new double[,] {
				{ 1 , 1 , 4 } ,
				{ 1 , 3 , 1 } ,
				{ 1 , 5 , 3 } ,
				{ 1 , 7 , 2 } };

//			double[,] X = new double[,] {
//				{ 1 , 1 , 1 , 1 } ,
//				{ 1 , 3 , 5 , 7 } ,
//				{ 4 , 1 , 3 , 2 } };

//			double[] weights = new double[] { 1 , 1 , 1 , 1 };

			LinearRegression linearRegression = new LinearRegression();
			linearRegression.RunRegression( y , X );
			
			Assert.AreEqual( 3.728571429 , linearRegression.EstimatedCoefficients[ 0 ] , 0.00000001 );
			Assert.AreEqual( 1.999404762 , linearRegression.EstimatedCoefficients[ 1 ] , 0.00000001 );
			Assert.AreEqual( 3.58452381 , linearRegression.EstimatedCoefficients[ 2 ] , 0.00000001 );
			
			Assert.AreEqual( 0.999876323 , linearRegression.CenteredRSquare , 0.0000000001 );
			
			Assert.AreEqual( 0.033928571 , linearRegression.Residuals[ 0 ] , 0.000000001 );
			Assert.AreEqual( -0.079166667 , linearRegression.Residuals[ 2 ] , 0.000000001 );
			
			Assert.AreEqual( 86.871875 , linearRegression.CenteredTotalSumOfSquares , 0.0000000000001 );
			
			Assert.AreEqual( 0.892857143 , linearRegression.HatMatrixDiagonal[ 0 ] , 0.000000001 );
			Assert.AreEqual( 0.702380952 , linearRegression.HatMatrixDiagonal[ 3 ] , 0.000000001 );
		}
	}
}
