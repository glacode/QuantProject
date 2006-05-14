/*
QuantProject - Quantitative Finance Library

WFLagCandidates.cs
Copyright (C) 2003
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
using System.Collections;
using System.Data;

using QuantProject.Data.DataTables;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// Set of candidates for the next optimization
	/// (close to close returns are computed and stored here).
	/// </summary>
	public class WFLagCandidates
	{
		private DataTable eligibleTickers;
		private DateTime firstQuoteDate;
		private DateTime lastQuoteDate;

		private Hashtable closeToCloseReturns;

		public WFLagCandidates( DataTable eligibleTickers ,
			DateTime firstQuoteDate , DateTime lastQuoteDate )
		{
			this.eligibleTickers = eligibleTickers;
			this.firstQuoteDate = firstQuoteDate;
			this.lastQuoteDate = lastQuoteDate;
		}
		/// <summary>
		/// arrays of close to close returns, one for each ticker
		/// </summary>
		/// <param name="signedTickers">tickers</param>
		/// <returns></returns>
		public float[][] GetTickersReturns( ArrayList tickers )
		{
			if ( this.closeToCloseReturns == null )
				this.set_closeToCloseReturns();
			float[][] tickersReturns = new float[ tickers.Count ][];
			int i = 0;
			foreach ( string ticker in tickers )
			{
				tickersReturns[ i ] = (float[])this.closeToCloseReturns[ ticker ];
				i++;
			}
			return tickersReturns;
		}

		#region set_closeToCloseReturns
		private float[] getCloseToCloseTickerReturns( string ticker )
		{
			Quotes tickerQuotes =
				new Quotes( ticker , this.firstQuoteDate , this.lastQuoteDate );
			float[] tickerAdjustedCloses =
				QuantProject.Data.ExtendedDataTable.GetArrayOfFloatFromColumn(
				tickerQuotes , "quAdjustedClose");
			float[] closeToCloseTickerReturns =
				new float[ tickerAdjustedCloses.Length - 1 ];
			int i = 0; //index for ratesOfReturns array
			for( int idx = 0 ; idx < tickerAdjustedCloses.Length - 1 ; idx++ )
			{
				closeToCloseTickerReturns[ i ] =
					tickerAdjustedCloses[ idx + 1 ] /	tickerAdjustedCloses[ idx ] - 1;
				i++;
			}	
			return closeToCloseTickerReturns;
		}
		private void set_closeToCloseReturns()
		{
			this.closeToCloseReturns = new Hashtable();
			for( int i = 0 ; i < this.eligibleTickers.Rows.Count ; i++ )
			{
				string ticker = (string)this.eligibleTickers.Rows[ i ][ 0 ];
				this.closeToCloseReturns[ ticker ] =
					this.getCloseToCloseTickerReturns( ticker );
			}
		}
		#endregion

//		private void retrieveData()
//		{
//			this.closeToCloseReturns =
//				new double[ this.eligibleTickers.Rows.Count ];
//			for(int i = 0; i<this.eligibleTickers.Rows.Count; i++)
//			{
//				string ticker = (string)this.eligibleTickers.Rows[i][0];
//				this.closeToCloseReturns[i] = new WFLagCandidate( ticker,
//					this.getArrayOfRatesOfReturn( ticker ) );
//				//				this.closeToCloseReturns[i] = new CandidateProperties( ticker,
//				//					this.getArrayOfRatesOfReturn( ticker ) );
//			}
//		}
	}
}
