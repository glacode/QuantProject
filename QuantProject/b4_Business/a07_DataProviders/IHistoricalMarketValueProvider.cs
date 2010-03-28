/*
QuantProject - Quantitative Finance Library

IHistoricalMarketValueProvider.cs
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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.*/

using System;

using QuantProject.ADT.Histories;

namespace QuantProject.Business.DataProviders
{
	/// <summary>
	/// Interface to be implemented by historical market value providers
	/// </summary>
	public interface IHistoricalMarketValueProvider
	{
		string Description{ get; }
			
		History GetMarketValues( string ticker , History history );
		
		/// <summary>
		/// True iif the given ticker was traded at the given DateTime
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		bool WasExchanged( string ticker , DateTime dateTime );
		
		/// <summary>
		/// returns the subset of DateTimes when ticker is exchanged
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="history">DateTimes returned are a subset of this parameter</param>
		/// <returns></returns>
		History GetDateTimesWithMarketValues( string ticker , History history );
	}
}
