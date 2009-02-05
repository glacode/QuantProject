/*
QuantProject - Quantitative Finance Library

SimpleBarCache.cs
Copyright (C) 2009
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

namespace QuantProject.Data.DataProviders.Bars.Caching
{
	/// <summary>
	/// It isn't really a cache: it just access to database
	/// </summary>
	public class SimpleBarCache : IBarCache
	{
		private int intervalFrameInSeconds;
		public SimpleBarCache(int intervalFrameInSeconds)
		{
			this.intervalFrameInSeconds = intervalFrameInSeconds;
		}
		
		/// <summary>
		/// returns the market value for the given ticker, for the bar
		/// that opens at the given dateTime
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public double GetMarketValue( string ticker, DateTime dateTime )
		{
			double returnValue = double.MinValue;
			try
			{
				returnValue = 
					QuantProject.DataAccess.Tables.Bars.GetOpen( ticker , dateTime , this.intervalFrameInSeconds );
			}
			catch( Exception ex )
			{
				string message = ex.Message; // used to avoid warning
				throw new MissingBarException(
					ticker , dateTime , this.intervalFrameInSeconds);
			}
			return returnValue;
		}
				
		public bool WasExchanged( string ticker , DateTime dateTime )
		{
			double marketValue = double.MinValue;
			try
			{
				marketValue = this.GetMarketValue( ticker , dateTime );
			}
			catch ( MissingBarException missingBarException )
			{
				string doNothing = missingBarException.Message; doNothing += "";
			}
			return (marketValue != Double.MinValue);
		}
	}
}
