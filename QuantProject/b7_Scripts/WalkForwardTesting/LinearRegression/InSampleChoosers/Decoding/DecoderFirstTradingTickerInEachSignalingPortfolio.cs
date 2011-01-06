/*
QuantProject - Quantitative Finance Library

DecoderFirstTradingTickerInEachSignalingPortfolio.cs
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
	/// Decoder for the Linear Regression strategy with ratios. The genome is decoded
	/// as follows: assume that the genome has n elements. Then it is decoded in an array
	/// of n-1 WeightedPositions: the i_th WeightedPositions contains the balanced portfolio
	/// with two positions: one for the ticker of the first gene and one for the ticker
	/// of the i+1_th gene; the first two tickers are decoded using the eligible tickers
	/// for trading, while the other tickers are decoded using the eligible tickers
	/// for signaling
	/// </summary>
	[Serializable]
	public class DecoderFirstTradingTickerInEachSignalingPortfolio :
		DecoderForLinearRegressionTestingPositions
	{

		public DecoderFirstTradingTickerInEachSignalingPortfolio(
			int numberOfTickersForTrading , int numberOfSignalingPortfolios ) :
			base( 2 , getNumberOfTickersForTrading( numberOfSignalingPortfolios ) )
		{
			this.expectedNumberOfGeneValues = 2 + numberOfSignalingPortfolios;
		}
		
		private static int[] getNumberOfTickersForTrading( int numberOfSignalingPortfolios )
		{
			int[] numberOfTickersInEachSignalingPortfolio =
				new int[ numberOfSignalingPortfolios ];
			for ( int i = 0 ; i < numberOfSignalingPortfolios ; i++ )
				numberOfTickersInEachSignalingPortfolio[ i ] = 2;
			return numberOfTickersInEachSignalingPortfolio;
		}

		protected override void decode_checkParameters( int[] genome )
		{
			if ( genome.Length != this.expectedNumberOfGeneValues )
				throw new Exception(
					"The given genome contains " + genome.Length + " genes, but " +
					this.expectedNumberOfGeneValues + " where expected!" );
		}


		protected override SignedTickers getSignedTickersForSignalingPortfolio(
			int portfolioIndex ,
			WeightedPositions weightedPositionsForTrading ,
			WeightedPositions weightedPositionsForSignaling ,
			IReturnsManager returnsManager )
		{
			SignedTickers signedTickersForSignalingPortfolio = new SignedTickers(
				new SignedTicker[] {
					weightedPositionsForTrading.SignedTickers[ 0 ] ,
					weightedPositionsForSignaling.SignedTickers[ portfolioIndex ] } );
			return signedTickersForSignalingPortfolio;
		}




		}
	}
