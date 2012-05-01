/*
QuantProject - Quantitative Finance Library

TestPSquareAndPressStatistic.cs
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

using QuantProject.ADT.Econometrics;

namespace QuantTesting.ADT.Econometrics
{
	[TestFixture]
	/// <summary>
	/// Test for the class QuantProject.ADT.Econometrics.LinearRegression
	/// </summary>
	public class TestPSquareAndPressStatistic
	{
		[Test]
		public void TestMethod()
		{
			double[] y = new double[] { 20.10 , 13.30 , 24.40 , 24.95 , 33.30 };

			double[,] X = new double[,] {
				{ 1 , 1 , 4 } ,
				{ 1 , 3 , 1 } ,
				{ 1 , 5 , 3 } ,
				{ 1 , 7 , 2 } ,
				{ 1 , 4 , 6 } };

			LinearRegression linearRegression = new LinearRegression();
			linearRegression.RunRegression( y , X );
			
			Assert.AreEqual( 0.047736797 , linearRegression.PredictedResidualsSumOfSquares , 0.000000001 );
			Assert.AreEqual( 334.58125 , linearRegression.PredictedCenteredTotalSumOfSquares , 0.000000000001 );
			Assert.AreEqual( 0.999857324 , linearRegression.CenteredPSquare , 0.000000001 );
		}
	}
}
