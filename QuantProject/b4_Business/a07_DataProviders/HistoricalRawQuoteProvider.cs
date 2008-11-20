/*
QuantProject - Quantitative Finance Library

HistoricalRawQuoteProvider.cs
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
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders.Quotes;

namespace QuantProject.Business.DataProviders
{
	/// <summary>
	/// Returns historical raw quotes
	/// </summary>
	[Serializable]
	public class HistoricalRawQuoteProvider : HistoricalQuoteProvider
	{
		public HistoricalRawQuoteProvider()
		{
		}
		public override double GetMarketValue(
			string instrumentKey ,
			DateTime dateTime )
		{
			MarketStatusSwitch marketStatusSwitch =
				HistoricalAdjustedQuoteProvider.GetMarketStatusSwitch( dateTime ); // TO DO move this method to an abstract class BasicQuotesProvider to be inherited both by HistoricalAdjustedQuoteProvider and by HistoricalRawQuoteProvider
			double marketValue = HistoricalQuotesProvider.GetRawMarketValue(
				instrumentKey ,
				ExtendedDateTime.GetDate( dateTime ) ,
				marketStatusSwitch );
			return marketValue;
		}

		protected override string getDescription()
		{
			return "raw";
		}
		
//		public override bool WasExchanged(string ticker, DateTime dateTime)
//		{
//			bool wasExchanged =
//				HistoricalQuotesProvider.WasExchanged( ticker , dateTime );
//			return wasExchanged;
//		}
	}
}
