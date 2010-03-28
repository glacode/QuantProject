/*
QuantProject - Quantitative Finance Library

DecoderForLinearRegressionTestingPositionsWithCouples.cs
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

using QuantProject.ADT.Collections;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Simple decoder for the Linear Regression strategy.
	/// The genome is decoded as follows: the first two tickers are used to decode
	/// a trading portfolio.
	/// For each of the other tickers is created a portfolio with that single ticker and
	/// such portfolio is used as a signaling portfolio.
	/// The first two tickers are decoded using the eligible tickers
	/// for trading, while the other tickers are decoded using the eligible tickers
	/// for signaling
	/// </summary>
	[Serializable]
	public class DecoderForLinearRegressionTestingPositions // : IDecoderForTestingPositions
	{
//		private int numberOfTickersForTrading;
//
		public int NumberOfTickersForTrading {
//			get { return this.numberOfTickersForTrading; }
			get { return 2; }
		}
//
		private int numberOfSignalingPortfolios;
//
		public int NumberOfSignalingPortfolios {
			get { return this.numberOfSignalingPortfolios; }
		}
		private BasicDecoderForTestingPositions basicDecoderForTestingPositions;
		
		public DecoderForLinearRegressionTestingPositions( int numberOfSignalingPortfolios )
//			int numberOfTickersForTrading , int numberOfTickersForSignaling )
		{
//			this.numberOfTickersForTrading = numberOfTickersForTrading;
			this.numberOfSignalingPortfolios = numberOfSignalingPortfolios;
			this.basicDecoderForTestingPositions = new BasicDecoderForTestingPositions();
		}
		
		#region Decode
		private void decode_checkParameters( int[] genome )
		{
			int numberOfExpectedGenes = this.numberOfSignalingPortfolios + 2;
			if ( genome.Length != numberOfExpectedGenes )
				throw new Exception(
					"The given genom contains " + genome.Length + " genes, but " +
					numberOfExpectedGenes + " where expected!" );
		}
		private int[] getSubGenome( int[] genome , int startingPosition , int length )
		{
			int[] subGenome = new int[ length ];
			for( int i = startingPosition ; i < startingPosition + length ; i++ )
				subGenome[ i - startingPosition ] = genome[ i ];
			return subGenome;
		}
		private WeightedPositions decodeWeightedPositionsForTrading(
			int[] encoded ,
			EligibleTickers eligibleTickersForTrading ,
			IReturnsManager returnsManager )
		{
			int[] encodedForTradingTickers =
//				this.getSubGenome( encoded , 0 , this.numberOfTickersForTrading );
				this.getSubGenome( encoded , 0 , 2 );
			WeightedPositions weightedPositionsForTrading =
				this.basicDecoderForTestingPositions.Decode(
					encodedForTradingTickers ,
					eligibleTickersForTrading , returnsManager ).WeightedPositions;
			return weightedPositionsForTrading;
		}
		private WeightedPositions decodeWeightedPositionsForSignaling(
			int[] encoded ,
			EligibleTickers eligibleTickersForSignaling ,
			IReturnsManager returnsManager )
		{
			int[] encodedForSignalingTickers =
				this.getSubGenome(
//					encoded , this.numberOfTickersForTrading , this.numberOfTickersForSignaling );
					encoded , 2 , encoded.Length - 2 );
			WeightedPositions weightedPositionsForSignaling =
				this.basicDecoderForTestingPositions.Decode(
					encodedForSignalingTickers ,
					eligibleTickersForSignaling , returnsManager ).WeightedPositions;
			return weightedPositionsForSignaling;
		}
		
		#region getTestingPositions
		protected virtual WeightedPositions getBalancedPortfolio(
			SignedTickers signedTickers ,
			IReturnsManager returnsManager )
		{
			WeightedPositions balancedPortfolio = null;
			if ( !CollectionManager.ContainsDuplicates( signedTickers.Tickers ) )
			{
				double[] balancedWeights = WeightedPositions.GetBalancedWeights(
					signedTickers , returnsManager );
				balancedPortfolio =
					new WeightedPositions( balancedWeights , signedTickers.Tickers );
			}
			return balancedPortfolio;
		}
		
		#region getTradingPortfolio
		protected virtual SignedTickers getSignedTickersForTradingPortfolio(
			WeightedPositions weightedPositionsForTrading ,
			IReturnsManager returnsManager )
		{
			SignedTickers signedTickersForTradingPortfolio = new SignedTickers(
				new SignedTicker[] {
					weightedPositionsForTrading.SignedTickers[ 0 ] ,
					weightedPositionsForTrading.SignedTickers[ 1 ] } );
			return signedTickersForTradingPortfolio;
		}
		private WeightedPositions getTradingPortfolio(
			WeightedPositions weightedPositionsForTrading ,
			IReturnsManager returnsManager )
		{
			SignedTickers signedTickers = this.getSignedTickersForTradingPortfolio(
				weightedPositionsForTrading , returnsManager );
			WeightedPositions tradingPortfolio = this.getBalancedPortfolio(
				signedTickers , returnsManager );
			return tradingPortfolio;
		}
		#endregion getTradingPortfolio
		
		#region getSignalingPortfolios
		protected virtual SignedTickers getSignedTickersForSignalingPortfolio(
			int portfolioIndex ,
			WeightedPositions weightedPositionsForTrading ,
			WeightedPositions weightedPositionsForSignaling ,
			IReturnsManager returnsManager )
		{
			SignedTickers signedTickersForSignalingPortfolio = new SignedTickers(
				new SignedTicker[] {
//					weightedPositionsForTrading.SignedTickers[ 0 ] ,
					weightedPositionsForSignaling.SignedTickers[ portfolioIndex ] } );
			return signedTickersForSignalingPortfolio;
		}
		private WeightedPositions getSignalingPortfolio(
			int portfolioIndex ,
			WeightedPositions weightedPositionsForTrading ,
			WeightedPositions weightedPositionsForSignaling ,
			IReturnsManager returnsManager )
		{
			SignedTickers signedTickers = this.getSignedTickersForSignalingPortfolio(
				portfolioIndex , weightedPositionsForTrading , weightedPositionsForSignaling ,
				returnsManager );
			WeightedPositions signalingPortfolio = this.getBalancedPortfolio(
				signedTickers , returnsManager );
			return signalingPortfolio;
		}
		private WeightedPositions[] getSignalingPortfolios(
			WeightedPositions weightedPositionsForTrading ,
			WeightedPositions weightedPositionsForSignaling ,
			IReturnsManager returnsManager )
		{
			bool containsSignalingPortfolioWithDuplicateTickers = false;
			WeightedPositions[] signalingPortfolios = new WeightedPositions[
				weightedPositionsForSignaling.Count ];
			for ( int j=0 ; j < weightedPositionsForSignaling.Count ; j++ )
			{
				signalingPortfolios[ j ] = this.getSignalingPortfolio(
					j , weightedPositionsForTrading , weightedPositionsForSignaling ,
					returnsManager );
				if ( signalingPortfolios[ j ] == null )
					containsSignalingPortfolioWithDuplicateTickers = true;
			}
			if ( containsSignalingPortfolioWithDuplicateTickers )
				signalingPortfolios = null;
			return signalingPortfolios;
		}
		#endregion getSignalingPortfolios
		

		
		private TestingPositions getTestingPositions(
			WeightedPositions weightedPositionsForTrading ,
			WeightedPositions weightedPositionsForSignaling ,
			IReturnsManager returnsManagerForTradingTickers ,
			IReturnsManager returnsManagerForSignalingTickers )
		{
			TestingPositions testingPositions = new TestingPositionsForUndecodableEncoded();
			WeightedPositions tradingPortfolio = this.getTradingPortfolio(
				weightedPositionsForTrading , returnsManagerForTradingTickers );
			WeightedPositions[] signalingPortfolios = this.getSignalingPortfolios(
				weightedPositionsForTrading , weightedPositionsForSignaling ,
				returnsManagerForSignalingTickers );
			if ( tradingPortfolio != null && signalingPortfolios != null )
				// all portfolios are valid, because none of them contained
				// duplicated tickers
				testingPositions = new LinearRegressionTestingPositions(
					signalingPortfolios , tradingPortfolio );
			return testingPositions;
		}
		#endregion getTestingPositions
		
		/// <summary>
		/// The genome is decoded as follows: the first two tickers are used to decode
		/// a trading portfolio.
		/// For each of the other tickers is created a portfolio with that single ticker and
		/// such portfolio is used as a signaling portfolio.
		/// The first two tickers are decoded using the eligible tickers
		/// for trading, while the other tickers are decoded using the eligible tickers
		/// for signaling
		/// </summary>
		/// <param name="encoded"></param>
		/// <param name="eligibleTickersForTrading"></param>
		/// <param name="eligibleTickersForSignaling"></param>
		/// <param name="returnsManagerForTradingTickers"></param>
		/// <param name="returnsManagerForSignalingTickers"></param>
		/// <returns></returns>
		public TestingPositions Decode(
			int[] encoded ,
			EligibleTickers eligibleTickersForTrading ,
			EligibleTickers eligibleTickersForSignaling ,
			IReturnsManager returnsManagerForTradingTickers ,
			IReturnsManager returnsManagerForSignalingTickers )
		{
			this.decode_checkParameters( encoded );
			WeightedPositions weightedPositionsForTrading =
				this.decodeWeightedPositionsForTrading(
					encoded , eligibleTickersForTrading , returnsManagerForTradingTickers );
			WeightedPositions weightedPositionsForSignaling =
				this.decodeWeightedPositionsForSignaling(
					encoded , eligibleTickersForSignaling , returnsManagerForSignalingTickers );
			TestingPositions meaning = new TestingPositionsForUndecodableEncoded();
			if ( weightedPositionsForTrading != null && weightedPositionsForSignaling != null )
				// there were not duplicated tickers in the encoded
				meaning = this.getTestingPositions(
					weightedPositionsForTrading , weightedPositionsForSignaling ,
					returnsManagerForTradingTickers , returnsManagerForSignalingTickers );
			return meaning;
		}
		#endregion Decode
	}
}
