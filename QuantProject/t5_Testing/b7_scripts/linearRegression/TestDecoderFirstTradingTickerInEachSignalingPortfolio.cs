/*
QuantProject - Quantitative Finance Library

TestDecoderFirstTradingTickerInEachSignalingPortfolio.cs
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

using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
//using QuantProject.Business.Strategies.Optimizing.GenomeManagers;
using QuantProject.Scripts.WalkForwardTesting.LinearRegression;

using QuantTesting.ADT.Optimizing.Genetic;
using QuantTesting.Business.DataProviders;

namespace QuantTesting.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Test for the class DecoderFirstTradingTickerInEachSignalingPortfolio
	/// </summary>
	[TestFixture]
	public class TestDecoderFirstTradingTickerInEachSignalingPortfolio
	{

		
		#region TestDecode
		
		#region getReturnsManager
		
		#region getFakeHistoricalMarketValueProviderValueBased
		
		#region addMarketValue
		private double getMarketValue( int day )
		{
			double marketValue = 0;
			switch ( day )
			{
				case 1:
					marketValue = 31;
					break;
				case 2:
					marketValue = 32;
					break;
				case 3:
					marketValue = 33;
					break;
			}
			return marketValue;
		}
		private void addMarketValue(
			string ticker , DateTime dateTime ,
			FakeHistoricalMarketValueProvider fakeHistoricalMarketValueProvider )
		{
			if ( !fakeHistoricalMarketValueProvider.WasExchanged( ticker , dateTime ) )
			{
				// this market values has not been added yet
				double marketValue = this.getMarketValue( dateTime.Day );
				fakeHistoricalMarketValueProvider.Add( ticker , dateTime , marketValue );
			}
		}
		#endregion addMarketValue
		
		private void addMarketValues(
			string[] tickers , ReturnInterval returnInterval1 , ReturnInterval returnInterval2 ,
			FakeHistoricalMarketValueProvider fakeHistoricalMarketValueProvider )
		{
			foreach( string ticker in tickers )
			{
				this.addMarketValue(
					ticker , returnInterval1.Begin , fakeHistoricalMarketValueProvider );
				this.addMarketValue(
					ticker , returnInterval1.End , fakeHistoricalMarketValueProvider );
				this.addMarketValue(
					ticker , returnInterval2.Begin , fakeHistoricalMarketValueProvider );
				this.addMarketValue(
					ticker , returnInterval2.End , fakeHistoricalMarketValueProvider );
			}
		}
		private FakeHistoricalMarketValueProvider
			getFakeHistoricalMarketValueProviderValueBased(
				string[] eligibleTickersForTrading ,
				string[] eligibleTickersForSignaling ,
				ReturnInterval returnInterval1 ,
				ReturnInterval returnInterval2 )
		{
			FakeHistoricalMarketValueProvider fakeHistoricalMarketValueProvider =
				new FakeHistoricalMarketValueProvider();
			this.addMarketValues( eligibleTickersForTrading , returnInterval1 , returnInterval2 ,
			              fakeHistoricalMarketValueProvider );
			this.addMarketValues( eligibleTickersForSignaling , returnInterval1 , returnInterval2 ,
			              fakeHistoricalMarketValueProvider );
			return fakeHistoricalMarketValueProvider;
		}
		#endregion getFakeHistoricalMarketValueProviderValueBased
		
		private ReturnsManager getReturnsManager(
			string[] eligibleTickersForTrading ,
			string[] eligibleTickersForSignaling )
		{
			ReturnInterval returnInterval1 = new ReturnInterval(
				new DateTime( 2008 , 2 , 1 , 16 , 0 , 0 ) ,
				new DateTime( 2008 , 2 , 2 , 16 , 0 , 0 ) );
			ReturnInterval returnInterval2 = new ReturnInterval(
				new DateTime( 2008 , 2 , 2 , 16 , 0 , 0 ) ,
				new DateTime( 2008 , 2 , 3 , 16 , 0 , 0 ) );
			ReturnIntervals returnIntervals = new ReturnIntervals( returnInterval1 );
			returnIntervals.Add( returnInterval2 );
			
			FakeHistoricalMarketValueProvider fakeHistoricalMarketValueProvider =
				this.getFakeHistoricalMarketValueProviderValueBased(
					eligibleTickersForTrading , eligibleTickersForSignaling ,
					returnInterval1 , returnInterval2 );
			
			ReturnsManager returnsManager = new ReturnsManager(
				returnIntervals , fakeHistoricalMarketValueProvider );
			return returnsManager;
		}
		#endregion getReturnsManager
		
		[Test]
		public void TestDecode()
		{
			DecoderForLinearRegressionTestingPositions
				decoderForLinearRegressionTestingPositions =
				new DecoderFirstTradingTickerInEachSignalingPortfolio( 2 , 3 );
			
			Assert.AreEqual(
				2 , decoderForLinearRegressionTestingPositions.NumberOfTickersForTrading );
			Assert.AreEqual(
				3 , decoderForLinearRegressionTestingPositions.NumberOfSignalingPortfolios );
			

			EligibleTickers eligibleTickersForTrading =
				new EligibleTickers( new string[] { "TA" , "TB" , "TC" } );
			EligibleTickers eligibleTickersForSignaling =
				new EligibleTickers(
					new string[] { "SA" , "SB" , "SC" , "SD" , "SE" } );
//			Genome genome =	this.getGenome();
//					eligibleTickersForTrading ,
//					eligibleTickersForSignaling ,
//					decoderForLinearRegressionTestingPositions );
			ReturnsManager returnsManager = this.getReturnsManager(
				eligibleTickersForTrading.Tickers , eligibleTickersForSignaling.Tickers );

			
			LinearRegressionTestingPositions meaning =
				(LinearRegressionTestingPositions)
				decoderForLinearRegressionTestingPositions.Decode(
					new int[] { 0 , -3 , -5 , 2 , 0 } ,
					eligibleTickersForTrading , eligibleTickersForSignaling ,
					returnsManager , returnsManager );
//					returnsManagerForTradingTickers , returnsManagerForSignalingTickers );
			// the genome corresponds to { "TA" , "-TC" , "-SE" , "SC" , "SA" }
			// then weighted positions for trading are { "TA" , "-TC" }
			// and weighted positions for signaling are { "-SE" , "SC" , "SA" }
			
			
			// test trading portfolio
			Assert.AreEqual( 2 , meaning.TradingPortfolio.Count );
			Assert.AreEqual( "TC" , meaning.TradingPortfolio[ 1 ].Ticker );
			Assert.IsTrue( meaning.TradingPortfolio[ 1 ].Weight < 0 ,
			              "meaning.TradingPortfolio[ 1 ].Weight < 0" );
			
			// test signaling portfolio
			Assert.AreEqual( 3 , meaning.SignalingPortfolios.Length );
			WeightedPositions signalingPortfolio0 = meaning.SignalingPortfolios[ 0 ];
			Assert.AreEqual( "TA" , signalingPortfolio0[ 0 ].Ticker );
			Assert.IsTrue( signalingPortfolio0[ 0 ].Weight > 0 ,
			              "signalingPortfolio0[ 0 ].Weight > 0" );
			Assert.AreEqual( "SE" , signalingPortfolio0[ 1 ].Ticker );
			Assert.IsTrue( signalingPortfolio0[ 1 ].Weight < 0 ,
			              "signalingPortfolio0[ 1 ].Weight < 0" );
			WeightedPositions signalingPortfolio1 = meaning.SignalingPortfolios[ 1 ];
			Assert.AreEqual( "TA" , signalingPortfolio1[ 0 ].Ticker );
			Assert.IsTrue( signalingPortfolio1[ 0 ].Weight > 0 ,
			              "signalingPortfolio1[ 1 ].Weight > 0" );
			Assert.AreEqual( "SC" , signalingPortfolio1[ 1 ].Ticker );
			Assert.IsTrue( signalingPortfolio1[ 1 ].Weight > 0 ,
			              "signalingPortfolio1[ 1 ].Weight > 0" );
			WeightedPositions signalingPortfolio2 = meaning.SignalingPortfolios[ 2 ];
			Assert.AreEqual( "TA" , signalingPortfolio2[ 0 ].Ticker );
			Assert.IsTrue( signalingPortfolio2[ 0 ].Weight > 0 ,
			              "signalingPortfolio2[ 1 ].Weight > 0" );
			Assert.AreEqual( "SA" , signalingPortfolio2[ 1 ].Ticker );
			Assert.IsTrue( signalingPortfolio2[ 1 ].Weight > 0 ,
			              "signalingPortfolio2[ 1 ].Weight > 0" );
		}
		#endregion TestDecode
		
		[Test]
		public void TestDuplicatedTickers()
		{
			EligibleTickers eligibleTickersForTrading =
				new EligibleTickers( new string[] { "TA" , "TB" , "TC" } );
			EligibleTickers eligibleTickersForSignaling =
				new EligibleTickers(
					new string[] { "SA" , "SB" , "SC" , "SD" , "TA" } );
			DecoderForLinearRegressionTestingPositions
				decoderForLinearRegressionTestingPositions =
				new DecoderFirstTradingTickerInEachSignalingPortfolio( 2 , 3 );
			
			DynamicMock dynamicMockForReturnsManager = new DynamicMock( typeof(IReturnsManager) );
			IReturnsManager returnsManager = (IReturnsManager)dynamicMockForReturnsManager.MockInstance;
			
			TestingPositions testingPositions =
				decoderForLinearRegressionTestingPositions.Decode(
					new int[] { 0 , 0 , 1 , -3 , 3 } ,		// same ticker for trading (first and second)
					eligibleTickersForTrading ,
					eligibleTickersForSignaling , returnsManager , returnsManager );
			Assert.IsTrue( testingPositions is TestingPositionsForUndecodableEncoded );

			testingPositions =
				decoderForLinearRegressionTestingPositions.Decode(
					new int[] { 0 , -3 , -4 , -4 , 0 } ,	// same ticker for signaling (third and fourth)
					eligibleTickersForTrading ,
					eligibleTickersForSignaling , returnsManager , returnsManager );
			Assert.IsTrue( testingPositions is TestingPositionsForUndecodableEncoded );

			testingPositions =
				decoderForLinearRegressionTestingPositions.Decode(
					new int[] { 0 , -3 , 0 , 1 , -2 } ,		// same ticker for signaling (fourth and fifth)
					eligibleTickersForTrading ,
					eligibleTickersForSignaling , returnsManager , returnsManager );
			Assert.IsTrue( testingPositions is TestingPositionsForUndecodableEncoded );

			testingPositions = decoderForLinearRegressionTestingPositions.Decode(
				new int[] { 0 , -3 , 1 , -3 , -5 } ,	// same ticker for the first trading ticker and for the third signaling portfolio
				eligibleTickersForTrading ,
				eligibleTickersForSignaling , returnsManager , returnsManager );
			Assert.IsTrue( testingPositions is TestingPositionsForUndecodableEncoded );
			
			testingPositions =
				decoderForLinearRegressionTestingPositions.Decode(
					new int[] { 0 , -3 , 1 , -3 , 3 } ,		// no conflict
					eligibleTickersForTrading ,
					eligibleTickersForSignaling , returnsManager , returnsManager );
			Assert.IsFalse( testingPositions is TestingPositionsForUndecodableEncoded );
			
			testingPositions = decoderForLinearRegressionTestingPositions.Decode(
				new int[] { -3 , 0 , 1 , -3 , -5 } ,	// no conflict even if the second ticker is the same as the fifth one
				eligibleTickersForTrading ,
				eligibleTickersForSignaling , returnsManager , returnsManager );
			Assert.IsFalse( testingPositions is TestingPositionsForUndecodableEncoded );
		}
	}
}
