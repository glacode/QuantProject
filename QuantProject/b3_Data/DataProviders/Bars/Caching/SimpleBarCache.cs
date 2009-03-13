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

using QuantProject.ADT;
using QuantProject.DataAccess;

namespace QuantProject.Data.DataProviders.Bars.Caching
{
	/// <summary>
	/// It isn't really a cache: it just access the database
	/// </summary>
	public class SimpleBarCache : IBarCache
	{
		private int intervalFrameInSeconds;
		private BarComponent barComponent;
		
		public SimpleBarCache(int intervalFrameInSeconds, BarComponent barComponent)
		{
			this.intervalFrameInSeconds = intervalFrameInSeconds;
			this.barComponent = barComponent;
		}
		
		public SimpleBarCache(int intervalFrameInSeconds)
		{
			this.intervalFrameInSeconds = intervalFrameInSeconds;
			this.barComponent = BarComponent.Open;
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
			double returnValue = double.NaN;
			try
			{
				switch (this.barComponent)
    		{
		  		case BarComponent.Open :
						returnValue = QuantProject.DataAccess.Tables.Bars.GetOpen( ticker , dateTime , this.intervalFrameInSeconds );
						break;
					case BarComponent.Close :
						returnValue = QuantProject.DataAccess.Tables.Bars.GetClose( ticker , dateTime , this.intervalFrameInSeconds );
						break;
					case BarComponent.High :
						returnValue = QuantProject.DataAccess.Tables.Bars.GetHigh( ticker , dateTime , this.intervalFrameInSeconds );
						break;
					case BarComponent.Low :
						returnValue = QuantProject.DataAccess.Tables.Bars.GetLow( ticker , dateTime , this.intervalFrameInSeconds );
						break;
					//this line should never be reached!
					default:
						returnValue = QuantProject.DataAccess.Tables.Bars.GetOpen( ticker , dateTime , this.intervalFrameInSeconds );
						break;
      	}
			}
			catch( EmptyQueryException ex )
			{
				string notUsed = ex.ToString();
				throw new MissingBarException(ticker , dateTime , this.intervalFrameInSeconds);
			}
			return returnValue;
		}
				
		public bool WasExchanged( string ticker , DateTime dateTime )
		{
			bool wasExchanged = true;
//			double marketValue = double.NaN;
			try
			{
				this.GetMarketValue( ticker , dateTime );
			}
			catch ( MissingBarException missingBarException )
			{
				// the bar is not in the database
				wasExchanged = false;
				string toAvoidCompileWarning = missingBarException.Message;
			}
			return wasExchanged;
		}
	}
}
