/*
QuantProject - Quantitative Finance Library

EligibleTickers.cs
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
using System.Collections.Generic;
using System.Data;

namespace QuantProject.Business.Strategies.Eligibles
{
	/// <summary>
	/// Strongly typed collection of tickers
	/// </summary>
	[Serializable]
	public class EligibleTickers : System.Collections.CollectionBase
	{
		private string[] tickers;
		public string[] Tickers
		{
			get  
			{
				if(this.tickers == null)
				{
					this.tickers = new string[this.Count];
					for(int i = 0;i<this.Count; i++)
						this.tickers[i] = this[i];
				}
				return this.tickers;
			}
		}
		private DataTable sourceDataTable;
		/// <summary>
		/// Returns the dataTable that has been used for
		/// the construction of the given instance
		/// </summary>
		public DataTable SourceDataTable
		{
			get  
			{
				return this.sourceDataTable;
			}
		}
		public EligibleTickers( ICollection<string> tickers )
		{
			foreach( string ticker in tickers )
				this.List.Add( ticker );
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dtTickers">DataTable in the form returned by
		/// a ticker selector</param>
		public EligibleTickers( DataTable dtTickers )
		{
			this.sourceDataTable = dtTickers;
			this.addTickers( dtTickers );
		}
		#region addTickers
		private void checkParameter( DataRow dataRow )
		{
			if ( !( dataRow[ 0 ] is string ) )
				throw new Exception( "The datatable of eligible tickers is " +
				                    "expected to have a single element in each " +
				                    "DataRow and that element should be a string" );			
		}
		private void addTicker_actually( string ticker )
		{
			if ( !this.List.Contains( ticker ) )
				this.List.Add( ticker );
		}
		private void addTicker( DataRow dataRow )
		{
			this.checkParameter( dataRow );
			this.addTicker_actually( (string)dataRow[ 0 ] );
		}		
		private void addTickers( DataTable dtTickers )
		{
			foreach ( DataRow dataRow in dtTickers.Rows )
				this.addTicker( dataRow );
		}
		#endregion addTickers
		public string this[ int index ]  
		{
			get  
			{
				return( (string) this.List[ index ] );
			}
			set
			{
//				if ( !(value is string) )
//					throw new Exception( "You are trying to add " +
//						"a ticker that is not a string !!" );
				this.List[ index ] = value;
			}
		}
		
		public void AddAdditionalEligibles( EligibleTickers eligibileTickers )
		{
			foreach ( string ticker in eligibileTickers.Tickers )
				this.addTicker_actually( ticker );
		}
	}
}
