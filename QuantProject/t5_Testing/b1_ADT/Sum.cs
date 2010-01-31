/*
QuantProject - Quantitative Finance Library

Sum.cs
Copyright (C) 2003
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
