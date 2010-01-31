/*
 * Created by SharpDevelop.
 * User: Glauco
 * Date: 1/31/2010
 * Time: 7:26 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using NUnit.Framework;

using QuantProject.ADT.Statistics;

namespace QuantProject.Testing.Statistics
{
	/// <summary>
	/// This class tests the
	/// QuantProject.ADT.Statistics.BasicFunctions.Sum() method
	/// </summary>
	[TestFixture]
	public class BasicFunctionsSum
	{
		[Test]
		public void Sum()
		{
			double[] values = { 1 , 2 };
			double sumValue = BasicFunctions.Sum( values );
			Assert.AreEqual( 3 , sumValue );		// this is ok
			// comment out the following statemente if you want to get a successfull test
			Assert.AreEqual( 3.1 , sumValue );		// this one faults
		}
	}
}
