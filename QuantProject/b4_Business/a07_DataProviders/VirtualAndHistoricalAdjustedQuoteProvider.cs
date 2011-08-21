/*
QuantProject - Quantitative Finance Library

VirtualAndHistoricalAdjustedQuoteProvider.cs
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

using QuantProject.ADT;
using QuantProject.DataAccess.Tables;
using QuantProject.Business.Timing;
using QuantProject.Business.DataProviders.VirtualQuotesProviding;
using QuantProject.Data.DataProviders.Caching;
using QuantProject.Data.DataProviders.Quotes;

namespace QuantProject.Business.DataProviders
{
	/// <summary>
	/// Returns historical o virtual (not stored in DB) adjusted quotes,
	/// through the given IVirtualQuotesProvider
	/// </summary>
	[Serializable]
	public class VirtualAndHistoricalAdjustedQuoteProvider : HistoricalAdjustedQuoteProvider
	{
		IVirtualQuotesProvider virtualQuotesProvider;
		
		public VirtualAndHistoricalAdjustedQuoteProvider(IVirtualQuotesProvider virtualQuotesProvider) :
																							base()
		{
			this.virtualQuotesProvider = virtualQuotesProvider;
		}
		
		public override double GetMarketValue(
			string instrumentKey ,
			DateTime dateTime )
		{
			MarketStatusSwitch marketStatusSwitch =
				HistoricalAdjustedQuoteProvider.GetMarketStatusSwitch( dateTime );
			double marketValue;
			if( this.virtualQuotesProvider.Contains(instrumentKey) )
			//the current instrument key is virtual	
			{
				marketValue =
					this.virtualQuotesProvider.GetVirtualQuote(instrumentKey,
					                                           dateTime,
					                                           this);
			}
			else// the given ticker is not virtual (so it should be in the DB)
			{
				try{
				marketValue =
					HistoricalQuotesProvider.GetAdjustedMarketValue(
						instrumentKey ,	ExtendedDateTime.GetDate( dateTime ) ,
						marketStatusSwitch );
				}
				catch(MissingQuoteException ex)
				{
					string str = ex.Message;
					double averageQuote;
					averageQuote = 
						Quotes.GetAverageAdjustedClosePrice(instrumentKey,
						                                    dateTime.AddDays(-45),
						                                    dateTime);
					marketValue = averageQuote;
				}
			}
			return marketValue;
		}


		protected override string getDescription()
		{
			return "VirtualAndHistoricalAdj";
		}
		
		public override bool WasExchanged(string ticker, DateTime dateTime)
		{
			bool wasExchanged;
			if( this.virtualQuotesProvider.Contains(ticker) )
			//the current ticker is virtual			
				wasExchanged = 
					this.virtualQuotesProvider.IsAvailable(ticker, dateTime);
			else
			//the current ticker is real (and its quotes should be stored in the DB)
				wasExchanged =
					HistoricalQuotesProvider.WasExchanged( ticker , dateTime );
			
			return wasExchanged;
		}
	}
}
