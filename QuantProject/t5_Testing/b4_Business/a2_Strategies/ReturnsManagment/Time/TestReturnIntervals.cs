/*
QuantProject - Quantitative Finance Library

TestReturnIntervals.cs
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

using QuantProject.Business.Strategies.ReturnsManagement.Time;

namespace QuantTesting.Business.Strategies.ReturnsManagement.Time
{
	/// <summary>
	/// Unit test for the QuantProject.Business.Strategies.ReturnsManagement.Time.ReturnIntervals
	/// class
	/// </summary>
	[TestFixture]
	public class TestReturnIntervals
	{
		#region TestMethod
		private ReturnIntervals getReturnIntervals()
		{
			ReturnInterval firstReturnInterval = new ReturnInterval(
				new DateTime( 2010 , 1 , 1 , 9 , 30 , 0 ) ,
				new DateTime( 2010 , 1 , 1 , 16 , 0 , 0 ) );
			ReturnInterval secondReturnInterval = new ReturnInterval(
				new DateTime( 2010 , 1 , 1 , 16 , 0 , 0 ) ,
				new DateTime( 2010 , 1 , 2 , 9 , 30 , 0 ) );
			ReturnIntervals returnIntervals = new ReturnIntervals();
			returnIntervals.Add( firstReturnInterval);
			returnIntervals.Add( secondReturnInterval );
			return returnIntervals;
		}
		[Test]
		public void TestMethod()
		{
			ReturnIntervals returnIntervals = this.getReturnIntervals();
			ReturnInterval secondLastInterval = returnIntervals.SeconLastInterval;
			Assert.AreEqual( new DateTime( 2010 , 1 , 1 , 16 , 0 , 0 ) , secondLastInterval.End );
		}
		#endregion TestMethod
	}
}
