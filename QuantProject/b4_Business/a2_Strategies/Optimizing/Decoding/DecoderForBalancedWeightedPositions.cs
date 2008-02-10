/*
QuantProject - Quantitative Finance Library

DecoderForBalancedWeightedPositions.cs
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

using QuantProject.ADT.Optimizing.Decoding;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Business.Strategies.Optimizing.Decoding
{
	/// <summary>
	/// Decodes optimization candidates to balanced WeightedPositions (i.e.
	/// WeightedPositions where weights are normalized for volatility)
	/// </summary>
	public class DecoderForBalancedWeightedPositions :
		IDecoderForWeightedPositions
	{
		private MeaningForUndecodableEncoded meaningForUndecodableEncoded;

		public string Description
		{
			get
			{
				string description = "Dcdr_blncdWghtdPstns";
				return description;
			}
		}


		public DecoderForBalancedWeightedPositions()
		{
			this.meaningForUndecodableEncoded = new MeaningForUndecodableEncoded();
		}

		#region Decode

		#region decodeSignedTickers
		private void decodeSignedTicker_checkParameters(
			int geneValue , EligibleTickers eligibleTickers )
		{
			if ( geneValue >= eligibleTickers.Count )
				throw new Exception( "geneValue is too (positive) large for eligibleTickers  !!" );
			if ( geneValue < -eligibleTickers.Count )
				throw new Exception( "geneValue is too (negative) large for eligibleTickers  !!" );
		}
		private SignedTicker decodeSignedTicker(	int signedTickerCode ,
			EligibleTickers eligibleTickers )
		{
			SignedTicker signedTicker;
			string ticker;
			decodeSignedTicker_checkParameters( signedTickerCode , eligibleTickers );
			if ( signedTickerCode >= 0 )
			{
				// long ticker
				ticker = eligibleTickers[ signedTickerCode ];
				signedTicker = new SignedTicker( ticker , PositionType.Long );
			}
			else
			{
				// short ticker
				ticker = eligibleTickers[ -(signedTickerCode+1) ];
				signedTicker = new SignedTicker( ticker , PositionType.Short );
			}
			return signedTicker;
		}
		private SignedTicker decodeSignedTickers( int i ,
			int[] tickerRelatedGeneValues ,	EligibleTickers eligibleTickers )
		{
			int currentGeneValue = tickerRelatedGeneValues[ i ];
			return this.decodeSignedTicker( currentGeneValue , eligibleTickers );
		}
		private SignedTickers decodeSignedTickers( int[] tickerRelatedGeneValues ,
			EligibleTickers eligibleTickers )
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
		#endregion decodeSignedTickers

		#region isDecodable
		private string[] getTickersForPositions( int[] encoded ,
			EligibleTickers eligibleTickers )
		{
			SignedTickers signedTickersForPositions =
				this.decodeSignedTickers( encoded , eligibleTickers );
			return signedTickersForPositions.Tickers;
		}
		private bool isDecodable( int[] encoded , EligibleTickers eligibleTickers )
		{
			return
				( WeightedPositions.AreValidTickers(
				this.getTickersForPositions( encoded , eligibleTickers ) ) );
		}
		#endregion isDecodable
		#region decodeDecodable
		private WeightedPositions decodeDecodable(
			int[] encoded , EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			SignedTickers signedTickers =
				this.decodeSignedTickers( encoded , eligibleTickers );
			double[] balancedWeightsForDrivingPositions =
				WeightedPositions.GetBalancedWeights( signedTickers ,
				returnsManager );
			WeightedPositions weightedPositions = new WeightedPositions(
				balancedWeightsForDrivingPositions , signedTickers.Tickers );
			return weightedPositions;
		}

		#endregion eligibleTickers
		/// <summary>
		/// A positive array value means a long position.
		/// A negative array value means a short position.
		/// The positive value n means the same ticker as the value -(n+1).
		/// Thus, if there are p (>0) eligible tickers, array values should
		/// range from -p to p-1
		/// </summary>
		/// <param name="encoded"></param>
		/// <returns></returns>
		public object Decode( int[] encoded , EligibleTickers eligibleTickers ,
		                    ReturnsManager returnsManager )
		{
			object meaning;
			if ( this.isDecodable( encoded , eligibleTickers ) )
				// genome can be decoded to a WeightedPositions object
				meaning = this.decodeDecodable(
					encoded , eligibleTickers , returnsManager );
			else
				// genome cannot be decoded to a WeightedPositions object
				meaning = this.meaningForUndecodableEncoded;
			return meaning;
		}
		#endregion Decode
	}
}
