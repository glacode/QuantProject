/*
QuantProject - Quantitative Finance Library

BasicDecoderForTestingPositions.cs
Copyright (C) 2008
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
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Business.Strategies.Optimizing.Decoding
{
	/// <summary>
	/// Decodes optimization candidates to a plain
	/// TestingPositions
	/// In this implementation, with the encoded items
	/// can be decoded only tickers
	/// </summary>
	[Serializable]
	public class BasicDecoderForTestingPositions :
		IDecoderForTestingPositions
	{
		protected int[] tickerRelatedGeneValues;
		protected int[] encoded;
		protected EligibleTickers eligibleTickers;
		protected IReturnsManager returnsManager;

		/// <summary>
		/// short description to be used for the file name
		/// </summary>
		/// <returns></returns>
		protected virtual string getDescription()
		{
			return "BscFrTstngPstns";
		}
		public string Description
		{
			get
			{
				string description = "Dcdr_" + this.getDescription();
				return description;
			}
		}
		
//		public virtual string Description
//		{
//			get
//			{
//				string description =
//					"BasicDecoderForTestingPositions_DecodedOnlyTickers_EqualWeights";
//				return description;
//			}
//		}

		public BasicDecoderForTestingPositions()
		{
			
		}
		
		#region decodeSignedTickers
		private void decodeSignedTicker_checkParameters( int geneValue )
		{
			if ( geneValue >= this.eligibleTickers.Count )
				throw new Exception( "geneValue is too (positive) large for eligibleTickers  !!" );
			if ( geneValue < -this.eligibleTickers.Count )
				throw new Exception( "geneValue is too (negative) large for eligibleTickers  !!" );
		}
		
		private SignedTicker decodeSignedTickers( int i )
		{
			int signedTickerCode = this.tickerRelatedGeneValues[ i ];
			SignedTicker signedTicker;
			string ticker;
			decodeSignedTicker_checkParameters( signedTickerCode );
			if ( signedTickerCode >= 0 )
			{
				// long ticker
				ticker = this.eligibleTickers[ signedTickerCode ];
				signedTicker = new SignedTicker( ticker , PositionType.Long );
			}
			else
			{
				// short ticker
				ticker = this.eligibleTickers[ -(signedTickerCode+1) ];
				signedTicker = new SignedTicker( ticker , PositionType.Short );
			}
			return signedTicker;
		}
		
		protected SignedTickers decodeSignedTickers()
		{
			SignedTickers signedTickers =	new SignedTickers();
			for( int i = 0 ; i < this.tickerRelatedGeneValues.Length ; i++ )
			{
				SignedTicker signedTicker = this.decodeSignedTickers( i );
				signedTickers.Add( signedTicker );
			}
			return signedTickers;
		}
		#endregion decodeSignedTickers

		protected virtual TestingPositions getMeaningForUndecodable()
		{
			return new TestingPositionsForUndecodableEncoded();
		}

		#region isDecodable
		private string[] getTickersForPositions()
		{
			SignedTickers signedTickersForPositions =
				this.decodeSignedTickers();
			return signedTickersForPositions.Tickers;
		}
		private bool isDecodable()
		{
			return
				( WeightedPositions.AreValidTickers(
									this.getTickersForPositions() ) );
		}
		#endregion isDecodable

		#region decodeDecodable
		
		protected virtual double[] getWeights(SignedTickers signedTickers)
		{
			//in this implementation encoded doesn't contain
			//information for weights: so weights are all the same
			double[] weights = new double[this.tickerRelatedGeneValues.Length];
			double coefficient;
			for(int i = 0; i<weights.Length; i++)
			{	
				if(signedTickers[i].IsLong)
					coefficient = 1.0;
				else//is Short
					coefficient = -1.0;
				weights[i] = coefficient / weights.Length;
			}
			return weights;
		}
		
		protected virtual double[] getUnsignedWeights(SignedTickers signedTickers)
		{
			//in this implementation encoded doesn't contain
			//information for weights: so weights are all the same
			double[] weights = new double[this.tickerRelatedGeneValues.Length];
			for(int i = 0; i<weights.Length; i++)
			{	
				weights[i] = 1.0 / weights.Length;
			}
			return weights;
		}
		
		protected virtual TestingPositions
			getTestingPositions( double[] weights ,
			                    string[] tickers )
		{
			TestingPositions testingPositions =	new TestingPositions(
					new WeightedPositions( weights , tickers ) );
			return testingPositions;
		}

		protected virtual TestingPositions decodeDecodable()
		{
			SignedTickers signedTickers =	this.decodeSignedTickers();
			double[] weights = this.getWeights(signedTickers);
			TestingPositions testingPositions =
				this.getTestingPositions(
					weights , signedTickers.Tickers );
	
			return testingPositions;
		}

		#endregion decodeDecodable

		protected virtual void setTickerRelatedGeneValues()
		{
			this.tickerRelatedGeneValues = this.encoded;
			//in this implementation all encoded contains
			//information only for tickers
		}

		private void decode_updateProtectedMembers(int[] encoded , EligibleTickers eligibleTickers ,
			                   IReturnsManager returnsManager)
		{
			this.encoded = encoded;
			this.eligibleTickers = eligibleTickers;
			this.returnsManager = returnsManager;
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
		public TestingPositions Decode(int[] encoded , EligibleTickers eligibleTickers ,
		             IReturnsManager returnsManager)
		{
			this.decode_updateProtectedMembers(encoded , eligibleTickers ,
			                   								 returnsManager);
			this.setTickerRelatedGeneValues();
			TestingPositions meaning = this.getMeaningForUndecodable();
			if ( this.isDecodable() )
			// encoded, normally a Genome, can be decoded to a TestingPositions
				meaning = this.decodeDecodable();
			
			return meaning;
		}
		
	}
}
