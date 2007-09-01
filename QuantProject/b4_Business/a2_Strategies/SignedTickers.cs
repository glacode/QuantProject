/*
QuantProject - Quantitative Finance Library

SignedTicker.cs
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

namespace QuantProject.Business.Strategies
{
	/// <summary>
	/// strongly typed collection of SignedTicker(s)
	/// </summary>
	public class SignedTickers : System.Collections.CollectionBase
	{
		public double[] Multipliers
		{
			get
			{
				double[] multipliers = new double[ List.Count ];
				for ( int i = 0 ; i < List.Count ; i++ )
					multipliers[ i ] = ((SignedTicker)(List[ i ])).Multiplier;
				return multipliers;
			}
		}
		public string[] ArrayForSignedTickers
		{
			get
			{
				string[] arrayForSignedTickers = new string[ List.Count ];
				for ( int i = 0 ; i < List.Count ; i++ )
				{
					if( ((SignedTicker)(List[ i ])).IsLong )
						arrayForSignedTickers[ i ] = ((SignedTicker)(List[ i ])).Ticker;
					else //ticker is short
						arrayForSignedTickers[ i ] = "-"+((SignedTicker)(List[ i ])).Ticker;
				}
				return arrayForSignedTickers;
			}
		}
		/// <summary>
		/// Array of the tickers (without sign)
		/// </summary>
		public string[] Tickers
		{
			get
			{
				string[] arrayForTickers = new string[ List.Count ];
				for ( int i = 0 ; i < List.Count ; i++ )
//					arrayForTickers[ i ] = ((SignedTicker)(List[ i ])).Ticker;
					arrayForTickers[ i ] = this[ i ].Ticker;
				return arrayForTickers;
			}
		}
		public SignedTickers()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="signedTickers">string of semicolon separated signed tickers</param>
		public SignedTickers( string signedTickers )
		{
			this.addSignedTickers( signedTickers );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="signedTickers">array of SignedTicker(s)</param>
		public SignedTickers( string[] signedTickers )
		{
			this.addSignedTickers( signedTickers );
		}
		private void addSignedTickers( string signedTickers )
		{
			string[] arrayForSignedTickers = signedTickers.Split( ';' );
			this.addSignedTickers( arrayForSignedTickers );
		}
		private void addSignedTickers( string[] signedTickers )
		{
			foreach ( string signedTicker in signedTickers )
				this.Add( new SignedTicker( signedTicker ) );
		}
		public SignedTicker this[ int index ]  
		{
			get  
			{
				return( (SignedTicker) this.List[ index ] );
			}
			set  
			{
				this.List[ index ] = value;
			}
		}
		public void Add( SignedTicker signedTicker )
		{
			this.List.Add( signedTicker );
		}
	}
}
