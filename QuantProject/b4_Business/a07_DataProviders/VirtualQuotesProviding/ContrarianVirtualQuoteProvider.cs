/*
QuantProject - Quantitative Finance Library

ContrarianVirtualQuoteProvider.cs
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

using QuantProject.Business.DataProviders;

namespace QuantProject.Business.DataProviders.VirtualQuotesProviding
{
	/// <summary>
	/// This class implements IVirtualQuotesProvider interface in such a way
	/// that, given a real underlying ticker (A), it returns quotes based on the
	/// given formula:
	/// set a = return of A (underlying real ticker) in interval p;
	/// set z = return of Z (virtual ticker) in p;
	/// z = - a /(a + 1)
	/// With this z, having set the following:
	/// "pEA" = equity invested in A a the beginning of p;
	/// "EAp" = equity invested in A a the end of p;
	/// rA = EAp / pEA
	/// "pEZ" = equity invested in Z a the beginning of p;
	/// "ZAp" = equity invested in Z a the end of p;
	/// We have the following equality:
	/// rZ = EZp / pEZ = 1 / rA
	/// </summary>
	[Serializable]
	public class ContrarianVirtualQuoteProvider : IVirtualQuotesProvider
	{
		private HistoricalBarProvider historicalBarProvider;
		private HistoricalRawQuoteProvider historicalRawQuoteProvider;
		private HistoricalAdjustedQuoteProvider historicalAdjustedQuoteProvider;
		
		public ContrarianVirtualQuoteProvider(List<VirtualTicker>  )
		{
			this.historicalBarProvider = historicalBarProvider;
			this.historicalRawQuoteProvider = historicalRawQuoteProvider;
			this.historicalAdjustedQuoteProvider = historicalAdjustedQuoteProvider;
		}
		
		protected override string getDescription()
		{
			return "adjustedBarProvider";
		}
		
		#region GetMarketValue
		
		private double getAdjustmenFactor( string ticker ,	DateTime dateTime )
		{
			DateTime marketCloseDateTime = HistoricalEndOfDayTimer.GetMarketClose( dateTime );
			double rawClose = this.historicalRawQuoteProvider.GetMarketValue(
				ticker , marketCloseDateTime );
			double adjustedClose = this.historicalAdjustedQuoteProvider.GetMarketValue(
				ticker , marketCloseDateTime );
			double adjustmentFactor = adjustedClose / rawClose;
			return adjustmentFactor;
		}
		public override double GetMarketValue(	string ticker ,	DateTime dateTime )
		{
			double adjustmentFactor = this.getAdjustmenFactor( ticker , dateTime );
			double marketValue = double.NaN;
			try
			{
				marketValue = this.historicalBarProvider.GetMarketValue(
					ticker , dateTime ) * adjustmentFactor;
			}
			catch( MissingBarException missingBarException )
			{
				string forBreakPoint = missingBarException.Message; forBreakPoint += " ";
				throw new TickerNotExchangedException( ticker , dateTime );
			}
			return marketValue;
		}
		#endregion GetMarketValue
		
		public override bool WasExchanged( string ticker, DateTime dateTime )
		{
			bool wasExchanged =
				this.historicalBarProvider.WasExchanged( ticker , dateTime );
			return wasExchanged;
		}
	}
}
