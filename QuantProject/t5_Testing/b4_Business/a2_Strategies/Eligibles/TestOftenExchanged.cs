/*
QuantProject - Quantitative Finance Library

TestOftenExchanged.cs
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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Mocks;

using QuantProject.ADT.Histories;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Data.Selectors;

using QuantTesting.Business.DataProviders;

namespace QuantTesting.Business.Strategies.Eligibles
{
	[TestFixture]
	/// <summary>
	/// Test for the QuantProject.Business.Strategies.Eligibles.OftenExchanged class
	/// </summary>
	public class TestOftenExchanged
	{
		#region Test_GetEligibleTickers
		private History getHistory()
		{
			History history = new History();
			history.Add( new DateTime( 2010 , 1 , 1 ) , null );
			history.Add( new DateTime( 2010 , 1 , 2 ) , null );
			history.Add( new DateTime( 2010 , 1 , 3 ) , null );
			return history;
		}
		private IHistoricalMarketValueProvider getHistoricalMarketValueProvider(
			History history )
		{
			FakeHistoricalMarketValueProvider fakeHistoricalMarketValueProviderValueBased =
				new FakeHistoricalMarketValueProvider();
			fakeHistoricalMarketValueProviderValueBased.Add(
				"A" , (DateTime)history.GetKey( 0 ) , 1 );
			fakeHistoricalMarketValueProviderValueBased.Add(
				"A" , (DateTime)history.GetKey( 1 ) , 1 );
			fakeHistoricalMarketValueProviderValueBased.Add(
				"A" , (DateTime)history.GetKey( 2 ) , 1 );
			fakeHistoricalMarketValueProviderValueBased.Add(
				"B" , (DateTime)history.GetKey( 0 ) , 1 );
//			fakeHistoricalMarketValueProviderValueBased.Add(
//				"B" , (DateTime)history.GetKey( 1 ) , 1 );
//			fakeHistoricalMarketValueProviderValueBased.Add(
//				"B" , (DateTime)history.GetKey( 2 ) , 1 );
			fakeHistoricalMarketValueProviderValueBased.Add(
				"C" , (DateTime)history.GetKey( 0 ) , 1 );
//			fakeHistoricalMarketValueProviderValueBased.Add(
//				"C" , (DateTime)history.GetKey( 1 ) , 1 );
			fakeHistoricalMarketValueProviderValueBased.Add(
				"C" , (DateTime)history.GetKey( 2 ) , 1 );
			return fakeHistoricalMarketValueProviderValueBased;
		}
		[Test]
		public void Test_GetEligibleTickers()
		{
			// the ticker "A" has 100% of the quotes
			// the ticker "B" has 33.3333% of the quotes
			// the ticker "C" has 66.6666% of the quotes
			
			DynamicMock dynamicMockTickerSelectorByGroup = new DynamicMock(
				typeof( ITickerSelectorByGroup ) );
			dynamicMockTickerSelectorByGroup.SetReturnValue(
				"GetSelectedTickers" ,
				new List<string>( new string[] { "A" , "B" , "C" } ) );
			ITickerSelectorByGroup mockTickerSelectorByGroup =
				(ITickerSelectorByGroup)dynamicMockTickerSelectorByGroup.MockInstance;
			
			History history = this.getHistory();
			
			IHistoricalMarketValueProvider historicalMarketValueProvider =
				this.getHistoricalMarketValueProvider( history );
			
			OftenExchanged oftenExchangedFor100Percent = new OftenExchanged(
				"dummy" ,  // the mock tickerSelectorByGroup doesn't use this group id
				mockTickerSelectorByGroup ,
				historicalMarketValueProvider ,
				1 );
			EligibleTickers eligibleTickers =
				oftenExchangedFor100Percent.GetEligibleTickers( history );
			Assert.AreEqual( 1 , eligibleTickers.Count );
			Assert.IsTrue( eligibleTickers.Tickers[ 0 ] == "A" );

			OftenExchanged oftenExchangedFor70Percent = new OftenExchanged(
				"dummy" ,  // the mock tickerSelectorByGroup doesn't use this group id
				mockTickerSelectorByGroup ,
				historicalMarketValueProvider ,
				0.7 );
			eligibleTickers =
				oftenExchangedFor70Percent.GetEligibleTickers( history );
			Assert.AreEqual( 1 , eligibleTickers.Count );
			Assert.IsTrue( eligibleTickers.Tickers[ 0 ] == "A" );

			OftenExchanged oftenExchangedFor50Percent = new OftenExchanged(
				"dummy" ,  // the mock tickerSelectorByGroup doesn't use this group id
				mockTickerSelectorByGroup ,
				historicalMarketValueProvider ,
				0.5 );
			eligibleTickers =
				oftenExchangedFor50Percent.GetEligibleTickers( history );
			Assert.AreEqual( 2 , eligibleTickers.Count );
			Assert.IsTrue( eligibleTickers.Tickers[ 0 ] == "A" );
			Assert.IsTrue( eligibleTickers.Tickers[ 1 ] == "C" );

			OftenExchanged oftenExchangedFor30Percent = new OftenExchanged(
				"dummy" ,  // the mock tickerSelectorByGroup doesn't use this group id
				mockTickerSelectorByGroup ,
				historicalMarketValueProvider ,
				0.3 );
			eligibleTickers =
				oftenExchangedFor30Percent.GetEligibleTickers( history );
			Assert.AreEqual( 3 , eligibleTickers.Count );
			Assert.IsTrue( eligibleTickers.Tickers[ 0 ] == "A" );
			Assert.IsTrue( eligibleTickers.Tickers[ 1 ] == "B" );
			Assert.IsTrue( eligibleTickers.Tickers[ 2 ] == "C" );
		}
		#endregion Test_GetEligibleTickers
	}
}
