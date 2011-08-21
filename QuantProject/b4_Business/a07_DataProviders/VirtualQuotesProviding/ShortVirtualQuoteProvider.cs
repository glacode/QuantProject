/*
QuantProject - Quantitative Finance Library

ShortVirtualQuoteProvider.cs
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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using QuantProject.DataAccess.Tables;
using QuantProject.Business.DataProviders;

namespace QuantProject.Business.DataProviders.VirtualQuotesProviding
{
	/// <summary>
	/// This class implements IVirtualQuotesProvider interface in such a way
	/// that virtual quotes for the given virtual ticker
	/// are such that corresponding returns are just
	/// the opposite of underlyingRealTickers'returns
	/// (buying the virtual ticker is equivalent
	/// to shorting the underlying real ticker)
	/// </summary>
	[Serializable]
	public class ShortVirtualQuoteProvider : BasicDerivedVirtualQuoteProvider
	{
		private int numberOfMarketDaysForBaseFixingBeforeCurrentDate;
		private DateTime closeDateTimeAsBase;
		
		public ShortVirtualQuoteProvider(IList<DerivedVirtualTicker> derivedVirtualTickers,
		                                 HistoricalMarketValueProvider historicalMarketValueProvider,
		                                 int numberOfMarketDaysForBaseFixingBeforeCurrentDate) :
					 base(derivedVirtualTickers, historicalMarketValueProvider)
		{
			if(numberOfMarketDaysForBaseFixingBeforeCurrentDate <= 0)
				throw new Exception("numberOfMarketDaysForBaseFixingBeforeCurrentDate has to be " +
				                    "greater than 0!");
			this.numberOfMarketDaysForBaseFixingBeforeCurrentDate =
				numberOfMarketDaysForBaseFixingBeforeCurrentDate;
		}
		
		public ShortVirtualQuoteProvider(IList<DerivedVirtualTicker> derivedVirtualTickers,
		                                 HistoricalMarketValueProvider historicalMarketValueProvider,
		                                 DateTime closeDateTimeAsBase) :
					 base(derivedVirtualTickers, historicalMarketValueProvider)
		{
			this.numberOfMarketDaysForBaseFixingBeforeCurrentDate = 0;
			this.closeDateTimeAsBase = closeDateTimeAsBase;
		}
		
		protected override DateTime getCloseDateTimeAsBase(string realTicker,
		                                                   DateTime currentDateTime)
		{
			if(this.numberOfMarketDaysForBaseFixingBeforeCurrentDate != 0)
			//the closeDateTime chosen as a base is a moving one
			{
				QuantProject.Data.DataTables.Quotes realTickerQuotes =
					new QuantProject.Data.DataTables.Quotes(realTicker, currentDateTime.AddDays(-2 * this.numberOfMarketDaysForBaseFixingBeforeCurrentDate),
					                                        currentDateTime);
				DateTime baseDate = 
					realTickerQuotes.GetPrecedingDate(currentDateTime, this.numberOfMarketDaysForBaseFixingBeforeCurrentDate);
				this.closeDateTimeAsBase = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day,
					             						 16, 0, 0);
			}
			return this.closeDateTimeAsBase;
		}
		
		protected override double getVirtualQuoteActually(double firstQuoteOfUnderlyingRealTicker,
		                                                  double currentQuoteOfUnderlyingRealTicker,
		                                                  double firstQuoteOfVirtualTicker)
		{
			double returnValue;
			double returnForUnderlyingTicker =
				(currentQuoteOfUnderlyingRealTicker - firstQuoteOfUnderlyingRealTicker) /
				firstQuoteOfUnderlyingRealTicker;
			returnValue = (1.0 - returnForUnderlyingTicker) *
										firstQuoteOfVirtualTicker;
			
			return returnValue;
		}
	}
}
