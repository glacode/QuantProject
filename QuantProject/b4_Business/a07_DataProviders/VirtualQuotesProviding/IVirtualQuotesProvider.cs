/*
QuantProject - Quantitative Finance Library

IVirtualQuotesProvider.cs
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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.*/

using System;

namespace QuantProject.Business.DataProviders.VirtualQuotesProviding
{
	/// <summary>
	/// Interface to be implemented by objects providing 
	/// virtual quotes for tickers (real or not) whose
	/// quotes are not stored in the DB
	/// </summary>
	public interface IVirtualQuotesProvider
	{
		/// <summary>
		/// Returns a virtual quote for the given virtual ticker
		/// </summary>
		/// <param name="virtualTicker">Ticker for which virtual quotes
		/// are provided (virtual = not stored in DB)</param>
		/// <param name="dateTime"></param>
		/// <param name="historicalMarketValueProvider">The historical market value provider
		/// providing real quotes on which virtual quote are based</param>
		/// <returns></returns>
		double GetVirtualQuote( string virtualTicker ,
		                        DateTime dateTime,
		                        HistoricalMarketValueProvider historicalMarketValueProvider);
//		/// <summary>
//		/// returns the ticker with real quotes that
//		/// are used by the provider for computing 
//		/// the virtual quote
//		/// </summary>
//		/// <param name="virtualTicker">The virtual ticker for which virtual quotes
//		/// are provided (virtual = not stored in DB)</param>
//		/// <returns></returns>
//		string GetUnderlyingTicker ( string virtualTicker );
		
		/// <summary>
		/// returns bool if the ticker is contained in
		/// the current IVirtualQuotesProvider
		/// </summary>
		/// <param name="virtualTicker">Ticker for which virtual quotes
		/// are provided (virtual = not stored in DB)</param>
		bool Contains ( string virtualTicker );
		
		/// <summary>
		/// True iif the quote of the given virtual ticker 
		/// is available at the given DateTime
		/// (nalogous to WasExchanged method in HistoricalMarketValueProvider class)
		/// </summary>
		/// <param name="virtualTicker"></param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		bool IsAvailable( string virtualTicker , DateTime dateTime );
	}
}

