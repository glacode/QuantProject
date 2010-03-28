/*
QuantProject - Quantitative Finance Library

FakeHistoricalMarketValueProvider.cs
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
using System.Collections;

using QuantProject.Business.DataProviders;

namespace QuantTesting.Business.DataProviders
{
	/// <summary>
	/// Implements a fake HistoricalMarketValueProvider, where
	/// market values are given by the Add method
	/// </summary>
	public class FakeHistoricalMarketValueProvider : HistoricalMarketValueProvider
	{
		private Hashtable marketValues;
		
		public FakeHistoricalMarketValueProvider()
		{
			this.marketValues = new Hashtable();
		}
		
		protected override string getDescription()
		{
			return "fake";
		}

		public override bool WasExchanged(string ticker, DateTime dateTime)
		{
			bool containsDateTime = this.marketValues.ContainsKey( dateTime );
			Hashtable marketValues = (Hashtable)this.marketValues[ dateTime ] ;
			bool wasExchanged =
				( this.marketValues.ContainsKey( dateTime ) &&
				 ( (Hashtable)this.marketValues[ dateTime ] ).ContainsKey( ticker ) );
			return wasExchanged;
		}
		
		#region GetMarketValue
		public override double GetMarketValue( string ticker , DateTime dateTime )
		{
			if ( !this.marketValues.ContainsKey( dateTime ) )
				throw new Exception( "There are no quotes for DateTime " + dateTime.ToString() );
			if ( !( (Hashtable)this.marketValues[ dateTime ] ).ContainsKey( ticker ) )
				throw new Exception(
					"There are no quotes for the ticker " + ticker + "at the DateTime " +
					dateTime.ToString() );
			// quote is not present
			double marketValue = (double)( ( (Hashtable)this.marketValues[ dateTime ] )[ ticker ] );
			return marketValue;
		}
		#endregion GetMarketValue
//
//		public History GetMarketValues( string ticker , History history )
//		{
//			eaer
//		}

		
		public void Add( string ticker , DateTime dateTime , double marketValue )
		{
			if ( !this.marketValues.ContainsKey( dateTime ) )
				this.marketValues.Add( dateTime , new Hashtable() );
			( (Hashtable)this.marketValues[ dateTime ] ).Add( ticker , marketValue );
		}
	}
}
