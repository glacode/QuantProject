/*
QuantProject - Quantitative Finance Library

DecoderForFVProviderStrategy.cs
Copyright (C) 2011
Marco Milletti

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
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Strategies;	
using QuantProject.Business.Strategies.Optimizing.Decoding;

namespace QuantProject.Scripts.TickerSelectionTesting.DrivenByFundamentals.DrivenByFairValueProvider
{
	/// <summary>
	/// Decodes optimization candidates to a GeneticallyOptimizableTestingPositions
	/// </summary>
	[Serializable]
	public class DecoderForFVProviderStrategy : BasicDecoderForGeneticallyOptimizableTestingPositions
	{
		
		public DecoderForFVProviderStrategy() : base()
		{
		}
		
		private void decodeSignedTickers_checkParameters( int geneValue )
		{
			if ( geneValue >= this.eligibleTickers.Count ||
			   	 geneValue < 0 )
				throw new Exception( "geneValue has to be greater than 0 and less than the number of eligibles !" );
		}
		
		private PositionType decodeSignedTickers_getPositionType( int geneValue )
		{
			PositionType returnValue = PositionType.Long;
			object[] keys = new object[1];
			keys[0] = this.eligibleTickers.Tickers[ geneValue ];
			DataRow foundRow =
				this.eligibleTickers.SourceDataTable.Rows.Find(keys);
			double relativeDifferenceBetweenFairValueAndMarketPrice =
				(double)foundRow["RelativeDifferenceBetweenFairAndMarketPrice"];
			if( relativeDifferenceBetweenFairValueAndMarketPrice < 0.0 )
				returnValue = PositionType.Short;
			
			return returnValue;
		}
		
		protected override SignedTicker decodeSignedTickers( int i )
		{
			int signedTickerCode = this.tickerRelatedGeneValues[ i ];
			decodeSignedTickers_checkParameters( signedTickerCode );
			string ticker = this.eligibleTickers[ signedTickerCode ];
			PositionType positionType = this.decodeSignedTickers_getPositionType ( signedTickerCode );
			SignedTicker signedTicker = new SignedTicker( ticker , positionType );
			
			return signedTicker;
		}
		
		protected override string getDescription()
		{
			return "Dcdr_ForFairValueProvider";
		}
	}
}
