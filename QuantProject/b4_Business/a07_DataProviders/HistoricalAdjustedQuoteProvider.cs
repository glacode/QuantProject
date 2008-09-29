/*
QuantProject - Quantitative Finance Library

HistoricalAdjustedQuoteProvider.cs
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
	/// Returns historical adjusted quotes
	/// </summary>
	[Serializable]
	public class HistoricalAdjustedQuoteProvider : HistoricalMarketValueProvider
	{
		public HistoricalAdjustedQuoteProvider()
		{
		}
		
		// TO DO move GetMarketStatusSwitch to an abstract class
		// BasicQuotesProvider to be inherited both by HistoricalAdjustedQuoteProvider
		// and by HistoricalRawQuoteProvider
		#region GetMarketStatusSwitch
		private static void riseExceptionIfNotAtMarketSwitch( DateTime dateTime )
		{
			if ( !HistoricalEndOfDayTimer.IsMarketStatusSwitch( dateTime ) )
				throw new Exception(
					"when GetCurrentMarketStatusSwitch() is invoked, dateTime " +
					"is expected to either be a market open or a market close." );
		}
		/// <summary>
		/// Returns the market status switch that corresponds to dateTime
		/// </summary>
		/// <param name="dateTime">it has to be either a market open or a
		/// market close</param>
		/// <returns></returns>
		public static MarketStatusSwitch GetMarketStatusSwitch(
			DateTime dateTime )
		{
			HistoricalAdjustedQuoteProvider.riseExceptionIfNotAtMarketSwitch( dateTime );
			MarketStatusSwitch marketStatusSwitch =
				MarketStatusSwitch.Open;
			if ( HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
				marketStatusSwitch = MarketStatusSwitch.Close;
			return marketStatusSwitch;
		}
		#endregion GetMarketStatusSwitch

//		public override double GetMarketValue(
//			string instrumentKey ,
//			DateTime date ,
//			MarketStatusSwitch marketStatusSwitch )
//		{
//			double marketValue =
//				HistoricalQuotesProvider.GetAdjustedMarketValue(
//					instrumentKey ,	date , marketStatusSwitch );
//			return marketValue;
//		}
		
		public override double GetMarketValue(
			string instrumentKey ,
			DateTime dateTime )
		{
			MarketStatusSwitch marketStatusSwitch =
				HistoricalAdjustedQuoteProvider.GetMarketStatusSwitch( dateTime );
				double marketValue =
				HistoricalQuotesProvider.GetAdjustedMarketValue(
					instrumentKey ,
					ExtendedDateTime.GetDate( dateTime ) ,
					marketStatusSwitch );
			return marketValue;
		}


		protected override string getDescription()
		{
			return "adj";
		}
	}
}
