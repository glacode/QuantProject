/*
QuantProject - Quantitative Finance Library

HistoricalQuoteProvider.cs
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

using QuantProject.Data.DataProviders.Quotes;

namespace QuantProject.Business.DataProviders
{
	/// <summary>
	/// Description of HistoricalQuoteProvider.
	/// </summary>
	[Serializable]
	public abstract class HistoricalQuoteProvider : HistoricalMarketValueProvider
	{
		public HistoricalQuoteProvider()
		{
		}
		
		protected abstract override string getDescription();

		
		public override bool WasExchanged(string ticker, DateTime dateTime)
		{
			bool wasExchanged =
				HistoricalQuotesProvider.WasExchanged( ticker , dateTime );
			return wasExchanged;
		}
	}
}
