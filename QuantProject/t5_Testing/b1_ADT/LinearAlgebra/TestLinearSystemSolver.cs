/*
 * Created by SharpDevelop.
 * User: Glauco
 * Date: 3/21/2010
 * Time: 8:10 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
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
