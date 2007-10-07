/*
QuantProject - Quantitative Finance Library

WFLagDecoderForBalancedFixedPortfolioAndBalancedDriving.cs
Copyright (C) 2007
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

using QuantProject.ADT.Collections;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WeightedPositionsChoosers.Decoding
{
	/// <summary>
	/// Decodes an array of int to a WFLagWeightedPositions object
	/// Weights are balanced with respect to volatility both for portfolio
	/// positions and for driving positions
	/// </summary>
	public class WFLagDecoderForFixedPortfolioWithBalancedVolatilityAndDrivingWithBalancedVolatility
		: IWFLagDecoder
	{
		private WFLagEligibleTickers eligibleTickersForDrivingWeightedPositions;
		private int numberOfDrivingPositions;
		private SignedTickers portfolioSignedTickers;
		private ReturnsManager returnsManager;
		
		private WFLagMeaningForUndecodableEncoded
			wFLagMeaningForUndecodableEncoded;
		private string[] tickersForPortfolioPositions;

		public
			WFLagDecoderForFixedPortfolioWithBalancedVolatilityAndDrivingWithBalancedVolatility(
			WFLagEligibleTickers eligibleTickersForDrivingWeightedPositions ,
			int numberOfDrivingPositions ,
			SignedTickers portfolioSignedTickers ,
			ReturnsManager returnsManager )
		{
			this.eligibleTickersForDrivingWeightedPositions =
				eligibleTickersForDrivingWeightedPositions;
			this.numberOfDrivingPositions = numberOfDrivingPositions;
			this.portfolioSignedTickers = portfolioSignedTickers;
			this.returnsManager = returnsManager;
			this.wFLagMeaningForUndecodableEncoded =
				new WFLagMeaningForUndecodableEncoded();
		}
		#region Decode
		#region getSignedTickersForDrivingPositions
		private int[] getTickerRelatedGeneValuesForDrivingPositions(
			int[] encoded )
		{
			// this method is useless for a fixed portfolio decoder,
			// but I've written as a guideline for other decoders which
			// will use numberOfDrivingPositions instead of encoded.Length
			return IntArrayManager.SubArray( encoded , 0 ,
				encoded.Length );
		}
		private void decodeSignedTicker_checkParameters(
			int geneValue , WFLagEligibleTickers eligibleTickers )
		{
			if ( geneValue >= eligibleTickers.EligibleTickers.Rows.Count )
				throw new Exception( "geneValue is too (positive) large for eligibleTickers  !!" );
			if ( geneValue < -eligibleTickers.EligibleTickers.Rows.Count )
				throw new Exception( "geneValue is too (negative) large for eligibleTickers  !!" );
		}
		private SignedTicker decodeSignedTicker(	int signedTickerCode ,
			WFLagEligibleTickers eligibleTickers )
		{
			SignedTicker signedTicker;
			string ticker;
			decodeSignedTicker_checkParameters( signedTickerCode , eligibleTickers );
			if ( signedTickerCode >= 0 )
			{
				// long ticker
				ticker =
					( string )eligibleTickers.EligibleTickers.Rows[ signedTickerCode ][ 0 ];
				signedTicker = new SignedTicker( ticker , PositionType.Long );
			}
			else
			{
				// short ticker
				ticker =
					( string )eligibleTickers.EligibleTickers.Rows[ -(signedTickerCode+1) ][ 0 ];
				signedTicker = new SignedTicker( ticker , PositionType.Short );
			}
			return signedTicker;
		}
		private SignedTicker decodeSignedTickers( int i ,
			int[] tickerRelatedGeneValues ,	WFLagEligibleTickers eligibleTickers )
		{
			int currentGeneValue = tickerRelatedGeneValues[ i ];
			return this.decodeSignedTicker( currentGeneValue , eligibleTickers );
		}
		private SignedTickers decodeSignedTickers( int[] tickerRelatedGeneValues ,
			WFLagEligibleTickers eligibleTickers )
		{
			SignedTickers signedTickers =	new SignedTickers();
			for( int i = 0 ; i < tickerRelatedGeneValues.Length ; i++ )
			{
				SignedTicker signedTicker = this.decodeSignedTickers(
					i , tickerRelatedGeneValues , eligibleTickers );
				signedTickers.Add( signedTicker );
			}
			return signedTickers;
		}
		private SignedTickers getSignedTickersForDrivingPositions( int[] encoded )
		{
			int[] encodedDrivingPositions =
				this.getTickerRelatedGeneValuesForDrivingPositions( encoded );
			return this.decodeSignedTickers( encodedDrivingPositions ,
				this.eligibleTickersForDrivingWeightedPositions );
		}
		#endregion getSignedTickersForDrivingPositions
		private string[] getTickersForDrivingPositions( int[] encoded )
		{
			return this.getSignedTickersForDrivingPositions( encoded ).Tickers;
		}
		private bool isDecodable( int[] encoded )
		{
			return
				( WeightedPositions.AreValidTickers(
				this.getTickersForDrivingPositions( encoded ) ) );
		}
		private WeightedPositions decodeDrivingWeightedPositions(
			int[] encoded )
		{
			SignedTickers signedTickers =
				this.getSignedTickersForDrivingPositions( encoded );
			double[] balancedWeightsForDrivingPositions =
				WeightedPositions.GetBalancedWeights( signedTickers ,
				this.returnsManager );
			WeightedPositions weightedPositions = new WeightedPositions(
				balancedWeightsForDrivingPositions , signedTickers.Tickers );
			return weightedPositions;
		}
		#region getTickersForPortfolioPositions

		private void setTickerForPortfolioPositions( int i )
		{
			SignedTicker signedTicker = this.portfolioSignedTickers[ i ];
			this.tickersForPortfolioPositions[ i ] = signedTicker.Ticker;
		}
		private void setTickersForPortfolioPositions()
		{
			this.tickersForPortfolioPositions = new string[ this.portfolioSignedTickers.Count ];
			for( int i=0 ; i < portfolioSignedTickers.Count ; i++ )
				this.setTickerForPortfolioPositions( i );
		}
		private string[] getTickersForPortfolioPositions()
		{
			if ( this.tickersForPortfolioPositions == null )
				this.setTickersForPortfolioPositions();
			return this.tickersForPortfolioPositions;
		}
		#endregion //getTickersForPortfolioPositions
		private double[] getBalancedWeightsForPortfolioPositions()
		{
			return WeightedPositions.GetBalancedWeights( this.portfolioSignedTickers ,
				this.returnsManager );
		}
		private WeightedPositions decodeWeightedPositions( double[] weights ,
			string[] tickers )
		{
			double[] normalizedWeights =
				WeightedPositions.GetNormalizedWeights( weights );
			WeightedPositions weightedPositions =	new WeightedPositions(
				normalizedWeights , tickers );
			return weightedPositions;
		}

		private WeightedPositions decodePortfolioWeightedPositions(
			int[] encoded )
		{
			string[] tickersForPortfolioPositions =	this.getTickersForPortfolioPositions();
			double[] portfolioPositionsWeights =
				this.getBalancedWeightsForPortfolioPositions();
			return this.decodeWeightedPositions(
				portfolioPositionsWeights ,
				tickersForPortfolioPositions );
		}
		private WFLagWeightedPositions decodeDecodable( int[] encoded )
		{
			WeightedPositions drivingWeightedPositions =
				this.decodeDrivingWeightedPositions( encoded );
			WeightedPositions portfolioWeightedPositions =
				this.decodePortfolioWeightedPositions( encoded );
			WFLagWeightedPositions wFLagWeightedPositions =
				new WFLagWeightedPositions(
				drivingWeightedPositions , portfolioWeightedPositions );
			return wFLagWeightedPositions;
		}
		/// <summary>
		/// A positive array value means a long position.
		/// A negative array value means a short position.
		/// The positive value n means the same ticker as the value -(n+1).
		/// Thus, if there are p (>0) eligible tickers, array values should
		/// range from -p to p-1
		/// </summary>
		/// <param name="encoded"></param>
		/// <returns></returns>
		public object Decode( int[] encoded )
		{
			object meaning;
			if ( this.isDecodable( encoded ) )
				// genome can be decoded to a WFLagWeightedPositions object
				meaning = this.decodeDecodable( encoded );
			else
				// genome cannot be decoded to a WFLagWeightedPositions object
				meaning = this.wFLagMeaningForUndecodableEncoded;
			return meaning;
		}
		#endregion Decode
	}
}
