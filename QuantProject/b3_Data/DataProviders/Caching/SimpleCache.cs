/*
QuantProject - Quantitative Finance Library

Cache.cs
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

using QuantProject.ADT;
using QuantProject.DataAccess;

namespace QuantProject.Data.DataProviders.Caching
{
	/// <summary>
	/// Handles quote caching from mass storage to main memory
	/// </summary>
	public class SimpleCache : ICache
	{
		public SimpleCache()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public double GetQuote( string ticker , DateTime dateTime , QuoteField quoteField )
		{
			double returnValue = double.MinValue;
			try
			{
				returnValue = DataBase.GetQuote( ticker , quoteField , dateTime );
			}
			catch( QuantProject.DataAccess.MissingQuoteException ex )
			{
				string message = ex.Message; // used to avoid warning
				throw new QuantProject.Data.DataProviders.Caching.MissingQuoteException(
					ticker , dateTime );
			}
			return returnValue;
		}
		public bool WasExchanged( string ticker ,
			ExtendedDateTime extendedDateTime )
		{
			return DataBase.WasExchanged( ticker , extendedDateTime );
		}
	}
}
