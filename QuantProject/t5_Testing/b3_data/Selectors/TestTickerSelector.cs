/*
QuantProject - Quantitative Finance Library

TestTickerSelector.cs
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
using System.Data;
using NUnit.Framework;

using QuantProject.Data.Selectors;

namespace QuantTesting.Data.Selectors
{
	[TestFixture]
	public class TestTickerSelector
	{
		/// <summary>
		/// Unit test for the QuantProject.Data.Selectors.TickerSelector class
		/// </summary>
		[Test]
		public void TestMethod()
		{
			string[] tickers = { "AAAA" , "BBBB" , "CCCC" };
			DataTable dataTableForTickerSelectors =
				TickerSelector.GetDataTableForTickerSelectors( tickers );
			DataRow dataRow = dataTableForTickerSelectors.Rows[ 1 ];
			Assert.AreEqual( "BBBB" , dataRow[ 0 ] );
		}
	}
}
