/*
QuantProject - Quantitative Finance Library

TestReturnIntervalsBuilderForTradingAndForSignaling.cs
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
using NUnit.Mocks;

using QuantProject.ADT.Histories;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Scripts.WalkForwardTesting.LinearRegression;

namespace QuantTesting.Scripts.WalkForwardTesting.LinearRegression
{
	public class FakeHistMarkValueProvForTestReturnIntervalsBuilderForTradingAndForSignaling :
		HistoricalMarketValueProvider
	{
//		public override History GetMarketValues( string ticker , History history )
//		{
//			return null;
//		}
		public override double GetMarketValue( string ticker , DateTime dateTime )
		{
			return double.MinValue;;
		}
		
		protected override string getDescription()
		{
			return "fake";
		}
		
		public override bool WasExchanged( string ticker , DateTime dateTime )
		{
			bool wasExchanged = true;
			if ( ( ticker == "TB" ) && ( dateTime == new DateTime( 2000 , 1 , 4 ) ) )
				wasExchanged = false;
			if ( ( ticker == "SA" ) && ( dateTime == new DateTime( 2000 , 1 , 9 ) ) )
				wasExchanged = false;
			return wasExchanged;
		}
	}
	/// <summary>
	/// Test for the class TestReturnIntervalsBuilderForTradingAndForSignaling
	/// </summary>
	[TestFixture]
	public class TestReturnIntervalsBuilderForTradingAndForSignaling
	{
		#region TestMethod
		[Test]
		public void TestMethod()
		{
			
//			// valid for signaling and for trading (in signaling - out trading)
//			ReturnInterval firstReturnInterval = new ReturnInterval(
//				new DateTime( 2000 , 1 , 1 ) , new DateTime( 2000 , 1 , 2 ) );
//			// valid for signaling and for trading (out signaling - in trading)
//			ReturnInterval secondReturnInterval = new ReturnInterval(
//				new DateTime( 2000 , 1 , 3 ) , new DateTime( 2000 , 1 , 4 ) );
//			// valid for signaling but NOT for trading (out signaling - out trading)
//			ReturnInterval thirdReturnInterval = new ReturnInterval(
//				new DateTime( 2000 , 1 , 5 ) , new DateTime( 2000 , 1 , 6 ) );
//			// valid for signaling and for trading, but lacks a quote for trading
//			// (in signaling - out trading)
//			ReturnInterval fourthReturnInterval = new ReturnInterval(
//				new DateTime( 2000 , 1 , 7 ) , new DateTime( 2000 , 1 , 8 ) );
//			// valid for trading but NOT for signaling (out signaling - in trading)
//			ReturnInterval fifthReturnInterval = new ReturnInterval(
//				new DateTime( 2000 , 1 , 9 ) , new DateTime( 2000 , 1 , 10 ) );
//			// valid for signaling and for trading (in signaling - out trading)
//			ReturnInterval sixthReturnInterval = new ReturnInterval(
//				new DateTime( 2000 , 1 , 11 ) , new DateTime( 2000 , 1 , 12 ) );
//			// valid for signaling and for trading (in signaling - in trading)
//			ReturnInterval seventhReturnInterval = new ReturnInterval(
//				new DateTime( 2000 , 1 , 13 ) , new DateTime( 2000 , 1 , 14 ) );
//			// valid for signaling and for trading, but lacks a quote for signaling
//			// (out signaling - in trading)
//			ReturnInterval eighthReturnInterval = new ReturnInterval(
//				new DateTime( 2000 , 1 , 15 ) , new DateTime( 2000 , 1 , 16 ) );
//			// valid for signaling and for trading (in signaling - out trading)
//			ReturnInterval ninethReturnInterval = new ReturnInterval(
//				new DateTime( 2000 , 1 , 17 ) , new DateTime( 2000 , 1 , 18 ) );
//			// valid for signaling and for trading (out signaling - in trading)
//			ReturnInterval tenthReturnInterval = new ReturnInterval(
//				new DateTime( 2000 , 1 , 19 ) , new DateTime( 2000 , 1 , 20 ) );
			
			// in
			ReturnInterval firstReturnIntervalForTrading = new ReturnInterval(
				new DateTime( 2000 , 1 , 2 ) , new DateTime( 2000 , 1 , 3 ) );
			// lacks a quote for trading - out
			ReturnInterval secondReturnIntervalForTrading = new ReturnInterval(
				new DateTime( 2000 , 1 , 4 ) , new DateTime( 2000 , 1 , 5 ) );
			// in
			ReturnInterval thirdReturnIntervalForTrading = new ReturnInterval(
				new DateTime( 2000 , 1 , 6 ) , new DateTime( 2000 , 1 , 7 ) );
			// in
			ReturnInterval fourthReturnIntervalForTrading = new ReturnInterval(
				new DateTime( 2000 , 1 , 8 ) , new DateTime( 2000 , 1 , 9 ) );
			// lacks a quote for signaling - out
			ReturnInterval fifthReturnIntervalForTrading = new ReturnInterval(
				new DateTime( 2000 , 1 , 10 ) , new DateTime( 2000 , 1 , 11 ) );
			// in
			ReturnInterval sixthReturnIntervalForTrading = new ReturnInterval(
				new DateTime( 2000 , 1 , 12 ) , new DateTime( 2000 , 1 , 13 ) );
			
			
			ReturnIntervals returnIntervals = new ReturnIntervals();
			returnIntervals.Add( firstReturnIntervalForTrading );
			returnIntervals.Add( secondReturnIntervalForTrading );
			returnIntervals.Add( thirdReturnIntervalForTrading );
			returnIntervals.Add( fourthReturnIntervalForTrading );
			returnIntervals.Add( fifthReturnIntervalForTrading );
			returnIntervals.Add( sixthReturnIntervalForTrading );
//			returnIntervals.Add( seventhReturnInterval );
//			returnIntervals.Add( eighthReturnInterval );
//			returnIntervals.Add( ninethReturnInterval );
//			returnIntervals.Add( tenthReturnInterval );
			
			
			FakeHistMarkValueProvForTestReturnIntervalsBuilderForTradingAndForSignaling
				fakeHistoricalMarketValueProvider = new
				FakeHistMarkValueProvForTestReturnIntervalsBuilderForTradingAndForSignaling();
			
			DynamicMock dynamicMockReturnsManager =
				new DynamicMock( typeof( IReturnsManager ) );
			dynamicMockReturnsManager.SetReturnValue(
				"get_ReturnIntervals" , returnIntervals );
			dynamicMockReturnsManager.SetReturnValue(
				"get_HistoricalMarketValueProvider" ,
				fakeHistoricalMarketValueProvider );
			IReturnsManager mockReturnsManager =
				(IReturnsManager)dynamicMockReturnsManager.MockInstance;
			
			string[] eligibleTickersForTrading = new string[]
			{ "TA" , "TB" };
			string[] eligibleTickersForSignaling = new string[]
			{ "SA" , "SB" };
			
//			DynamicMock dinamicMockReturnIntervalFilterForTrading =
//				new DynamicMock( typeof( IReturnIntervalFilter ) );
//			dinamicMockReturnIntervalFilterForTrading.ExpectAndReturn(
//				"IsValid" , firstReturnInterval , true );
//			dinamicMockReturnIntervalFilterForTrading.ExpectAndReturn(
//				"IsValid" , secondReturnInterval , true );
//			dinamicMockReturnIntervalFilterForTrading.ExpectAndReturn(
//				"IsValid" , thirdReturnInterval , true );
//			dinamicMockReturnIntervalFilterForTrading.ExpectAndReturn(
//				"IsValid" , fourthReturnInterval , true );
//			dinamicMockReturnIntervalFilterForTrading.ExpectAndReturn(
//				"IsValid" , fifthReturnInterval , false );
//			dinamicMockReturnIntervalFilterForTrading.ExpectAndReturn(
//				"IsValid" , sixthReturnInterval , true );
//			dinamicMockReturnIntervalFilterForTrading.ExpectAndReturn(
//				"IsValid" , seventhReturnInterval , true );
//			dinamicMockReturnIntervalFilterForTrading.ExpectAndReturn(
//				"IsValid" , eighthReturnInterval , true );
//			dinamicMockReturnIntervalFilterForTrading.ExpectAndReturn(
//				"IsValid" , ninethReturnInterval , true );
//			dinamicMockReturnIntervalFilterForTrading.ExpectAndReturn(
//				"IsValid" , tenthReturnInterval , true );
//			IReturnIntervalFilter mockReturnIntervalFilterForTrading =
//				(IReturnIntervalFilter)
//				dinamicMockReturnIntervalFilterForTrading.MockInstance;
			
//			DynamicMock dinamicMockReturnIntervalFilterForSignaling =
//				new DynamicMock( typeof( IReturnIntervalFilter ) );
//			dinamicMockReturnIntervalFilterForSignaling.ExpectAndReturn(
//				"IsValid" , firstReturnInterval , true );
//			dinamicMockReturnIntervalFilterForSignaling.ExpectAndReturn(
//				"IsValid" , secondReturnInterval , true );
//			dinamicMockReturnIntervalFilterForSignaling.ExpectAndReturn(
//				"IsValid" , thirdReturnInterval , false );
//			dinamicMockReturnIntervalFilterForSignaling.ExpectAndReturn(
//				"IsValid" , fourthReturnInterval , true );
//			dinamicMockReturnIntervalFilterForSignaling.ExpectAndReturn(
//				"IsValid" , fifthReturnInterval , true );
//			dinamicMockReturnIntervalFilterForSignaling.ExpectAndReturn(
//				"IsValid" , sixthReturnInterval , true );
//			dinamicMockReturnIntervalFilterForSignaling.ExpectAndReturn(
//				"IsValid" , seventhReturnInterval , true );
//			dinamicMockReturnIntervalFilterForSignaling.ExpectAndReturn(
//				"IsValid" , eighthReturnInterval , true );
//			dinamicMockReturnIntervalFilterForSignaling.ExpectAndReturn(
//				"IsValid" , ninethReturnInterval , true );
//			dinamicMockReturnIntervalFilterForSignaling.ExpectAndReturn(
//				"IsValid" , tenthReturnInterval , true );
//			IReturnIntervalFilter mockReturnIntervalFilterForSignaling =
//				(IReturnIntervalFilter)
//				dinamicMockReturnIntervalFilterForSignaling.MockInstance;
			
			// in
			ReturnInterval firstReturnIntervalForSignaling = new ReturnInterval(
				new DateTime( 2000 , 1 , 1 ) , new DateTime( 2000 , 1 , 2 ) );
			// lacks a quote for trading - out
			ReturnInterval secondReturnIntervalForSignaling = new ReturnInterval(
				new DateTime( 2000 , 1 , 3 ) , new DateTime( 2000 , 1 , 4 ) );
			// in
			ReturnInterval thirdReturnIntervalForSignaling = new ReturnInterval(
				new DateTime( 2000 , 1 , 5 ) , new DateTime( 2000 , 1 , 6 ) );
			// in
			ReturnInterval fourthReturnIntervalForSignaling = new ReturnInterval(
				new DateTime( 2000 , 1 , 7 ) , new DateTime( 2000 , 1 , 8 ) );
			// lacks a quote for signaling - out
			ReturnInterval fifthReturnIntervalForSignaling = new ReturnInterval(
				new DateTime( 2000 , 1 , 9 ) , new DateTime( 2000 , 1 , 10 ) );
			// in
			ReturnInterval sixthReturnIntervalForSignaling = new ReturnInterval(
				new DateTime( 2000 , 1 , 11 ) , new DateTime( 2000 , 1 , 12 ) );
			
			DynamicMock dynamicMockReturnIntervalSelectorForSignaling =
				new DynamicMock( typeof( IReturnIntervalSelectorForSignaling ) );
			dynamicMockReturnIntervalSelectorForSignaling.ExpectAndReturn(
				"GetReturnIntervalUsedForSignaling" , firstReturnIntervalForSignaling ,
				new object[] { firstReturnIntervalForTrading } );
			dynamicMockReturnIntervalSelectorForSignaling.ExpectAndReturn(
				"GetReturnIntervalUsedForSignaling" , secondReturnIntervalForSignaling ,
				new object[] { secondReturnIntervalForTrading } );
			dynamicMockReturnIntervalSelectorForSignaling.ExpectAndReturn(
				"GetReturnIntervalUsedForSignaling" , thirdReturnIntervalForSignaling ,
				new object[] { thirdReturnIntervalForTrading } );
			dynamicMockReturnIntervalSelectorForSignaling.ExpectAndReturn(
				"GetReturnIntervalUsedForSignaling" , fourthReturnIntervalForSignaling ,
				new object[] { fourthReturnIntervalForTrading } );
			dynamicMockReturnIntervalSelectorForSignaling.ExpectAndReturn(
				"GetReturnIntervalUsedForSignaling" , fifthReturnIntervalForSignaling ,
				new object[] { fifthReturnIntervalForTrading } );
			dynamicMockReturnIntervalSelectorForSignaling.ExpectAndReturn(
				"GetReturnIntervalUsedForSignaling" , sixthReturnIntervalForSignaling ,
				new object[] { sixthReturnIntervalForTrading } );
			IReturnIntervalSelectorForSignaling mockReturnIntervalSelectorForSignaling =
				(IReturnIntervalSelectorForSignaling)dynamicMockReturnIntervalSelectorForSignaling.MockInstance;

			ReturnIntervalsBuilderForTradingAndForSignaling
				returnIntervalsBuilderForTradingAndForSignaling =
				new ReturnIntervalsBuilderForTradingAndForSignaling();
//					mockReturnIntervalFilterForTrading ,
//					mockReturnIntervalFilterForSignaling );
			
			ReturnIntervals returnIntervalsForTrading;
			ReturnIntervals returnIntervalsForSignaling;
			returnIntervalsBuilderForTradingAndForSignaling.BuildIntervals(
				mockReturnsManager , mockReturnIntervalSelectorForSignaling ,
				eligibleTickersForTrading , eligibleTickersForSignaling ,
				out returnIntervalsForTrading , out returnIntervalsForSignaling );
			
			Assert.AreEqual( 4 , returnIntervalsForTrading.Count , "for trading" );
			Assert.AreEqual( 4 , returnIntervalsForSignaling.Count , "for signaling" );
			Assert.AreEqual( returnIntervalsForTrading.Count ,
			                returnIntervalsForSignaling.Count , "same length" );

			Assert.Contains( firstReturnIntervalForTrading , returnIntervalsForTrading );
			Assert.Contains( thirdReturnIntervalForTrading , returnIntervalsForTrading );
			Assert.Contains( fourthReturnIntervalForTrading , returnIntervalsForTrading );
			Assert.Contains( sixthReturnIntervalForTrading , returnIntervalsForTrading );
			Assert.IsFalse( returnIntervalsForTrading.BordersHistory.ContainsKey(
				secondReturnIntervalForTrading.Begin ) );
			
			Assert.Contains( firstReturnIntervalForSignaling , returnIntervalsForSignaling );
			Assert.Contains( thirdReturnIntervalForSignaling , returnIntervalsForSignaling );
			Assert.Contains( fourthReturnIntervalForSignaling , returnIntervalsForSignaling );
			Assert.Contains( sixthReturnIntervalForSignaling , returnIntervalsForSignaling );
			Assert.IsFalse( returnIntervalsForTrading.BordersHistory.ContainsKey(
				fifthReturnIntervalForSignaling.End ) );
			
//			void BuildIntervals(
//					IReturnsManager returnsManager ,
//					string[] eligibleTickersForTrading ,
//					string[] eligibleTickersForSignaling ,
//					out ReturnIntervals returnIntervalsForTrading ,
//					out ReturnIntervals returnIntervalsForSignaling );
		}
		#endregion TestMethod
	}
}
