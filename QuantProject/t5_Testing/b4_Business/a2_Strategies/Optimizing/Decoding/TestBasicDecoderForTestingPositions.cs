/*
QuantProject - Quantitative Finance Library

TestBasicDecoderForTestingPositions.cs
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

using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;

using QuantTesting.Business.DataProviders;

namespace QuantTesting.Business.Strategies.Optimizing.Decoding
{
	[TestFixture]
	/// <summary>
	/// Test for the QuantProject.Business.Strategies.Optimizing.Decoding.BasicDecoderForTestingPositions
	/// class
	/// </summary>
	public class TestBasicDecoderForTestingPositions
	{
		#region TestMethod
		private ReturnsManager getReturnsManager()
		{
			ReturnInterval returnInterval = new ReturnInterval(
				new DateTime( 2008 , 2 , 1 , 16 , 0 , 0 ) ,
				new DateTime( 2008 , 2 , 3 , 16 , 0 , 0 ) );
			ReturnIntervals returnIntervals = new ReturnIntervals( returnInterval );
			
			FakeHistoricalMarketValueProvider fakeHistoricalMarketValueProvider =
				new FakeHistoricalMarketValueProvider();			
			
			ReturnsManager returnsManager = new ReturnsManager(
				returnIntervals , fakeHistoricalMarketValueProvider );
			return returnsManager;
		}
		[Test]
		public void TestMethod()
		{
			EligibleTickers eligibleTickers =
				new EligibleTickers( new string[] { "AAAA" , "BBBB" , "CCCC" , "DDDD" } );
			int[] encoded = { -4 , 0 , 1 };
			BasicDecoderForTestingPositions basicDecoderForTestingPositions =
				new BasicDecoderForTestingPositions();
			ReturnsManager returnsManager = this.getReturnsManager();
			
			WeightedPositions weightedPositions =
				basicDecoderForTestingPositions.Decode(
					encoded , eligibleTickers , returnsManager ).WeightedPositions;
			
			Assert.AreEqual( "DDDD" , weightedPositions[ 0 ].Ticker );
			Assert.AreEqual( -0.333333 , weightedPositions[ 0 ].Weight , 0.001 );
			Assert.AreEqual(
				"BBBB" , weightedPositions[ 2 ].Ticker , "weightedPositions[ 2 ].Ticker" );
			Assert.AreEqual( 0.333333 , weightedPositions[ 2 ].Weight , 0.001 );
			Assert.AreEqual( "DDDD" , weightedPositions.SignedTickers[ 0 ].Ticker ,
			                "weightedPositions.SignedTickers[ 0 ].Ticker" );
			Assert.AreEqual( "AAAA" , weightedPositions.SignedTickers[ 1 ].Ticker ,
			                "weightedPositions.SignedTickers[ 1 ].Ticker" );
		}
		#endregion TestMethod
	}
}
