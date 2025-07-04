/*
QuantProject - Quantitative Finance Library

HistoricalBarProvider.cs
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

using QuantProject.Data.DataProviders.Bars.Caching;

namespace QuantProject.Data.DataProviders.Bars
{
	/// <summary>
	/// Returns historical bars, playing with caching
	/// </summary>
	[Serializable]
	public class HistoricalBarProvider
	{
		[NonSerialized]
		IBarCache barCache;
		
		public HistoricalBarProvider( IBarCache barCache )
		{
			this.barCache = barCache;
		}
		
		public double GetMarketValue( string ticker , DateTime dateTime )
		{
			double marketValue = this.barCache.GetMarketValue( ticker , dateTime );
			return marketValue;
		}
		
		/// <summary>
		/// true iif the ticker was exchange at the given dateTime
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public bool WasExchanged(
			string ticker , DateTime dateTime )
		{
			return this.barCache.WasExchanged( ticker , dateTime );
		}
	}
}
