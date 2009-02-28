/*
QuantProject - Quantitative Finance Library

PriceAdjustmentDetector.cs
Copyright (C) 2009
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

using QuantProject.ADT.Timing;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Timing;

namespace QuantProject.Business.DataProviders
{
	/// <summary>
	/// Checks if a stock price has been adjusted due to a split, a dividen, etc.
	/// </summary>
	[Serializable]
	public class PriceAdjustmentDetector
	{
		private HistoricalAdjustedQuoteProvider historicalAdjustedQuoteProvider;
		private HistoricalRawQuoteProvider historicalRawQuoteProvider;
		
		/// <summary>
		/// Checks if a stock price has been adjusted due to a split, a dividen, etc.
		/// </summary>
		public PriceAdjustmentDetector()
		{
			this.historicalAdjustedQuoteProvider = new HistoricalAdjustedQuoteProvider();
			this.historicalRawQuoteProvider = new HistoricalRawQuoteProvider();
		}
		
		private bool isReturnIntervalMultiday( DateTime firstDateTime , DateTime lastDateTime )
		{
			Date intervalBeginDate = new Date( firstDateTime );
			Date intervalEndDate = new Date( lastDateTime );
			bool isMultiday = ( intervalBeginDate < intervalEndDate );
			return isMultiday;
		}
		
		#region hasThePriceBeenAdjustedWithinTheMultidayInterval
		
		#region getCloseToCloseReturn
		
		#region getCloseToCloseReturnsManager
		
		#region getCloseToCloseReturnIntervals
		private DateTime getMarketCloseDateTime( DateTime dateTime )
		{
			DateTime closeDateTime = HistoricalEndOfDayTimer.GetMarketClose( dateTime );
			return closeDateTime;
		}
		private ReturnIntervals getCloseToCloseReturnIntervals(
			DateTime firstDateTime , DateTime lastDateTime )
		{
			DateTime marketCloseDateTimeForIntervalBegin =
				this.getMarketCloseDateTime( firstDateTime );
			DateTime marketCloseDateTimeForIntervalEnd =
				this.getMarketCloseDateTime( lastDateTime );
			ReturnIntervals returnIntervals = new ReturnIntervals( new ReturnInterval(
				marketCloseDateTimeForIntervalBegin , marketCloseDateTimeForIntervalEnd ) );
			return returnIntervals;
		}
		#endregion getCloseToCloseReturnIntervals
		
		private ReturnsManager getCloseToCloseReturnsManager(
			string ticker , DateTime firstDateTime , DateTime lastDateTime ,
			HistoricalQuoteProvider historicalQuoteProvider )
		{
			ReturnIntervals closeToCloseReturnIntervals = this.getCloseToCloseReturnIntervals(
				firstDateTime , lastDateTime );
			ReturnsManager closeToCloseReturnsManager = new ReturnsManager(
				closeToCloseReturnIntervals , historicalQuoteProvider );
			return closeToCloseReturnsManager;
		}
		#endregion getCloseToCloseReturnsManager
		
		private double getCloseToCloseReturn(
			string ticker , DateTime firstDateTime , DateTime lastDateTime ,
			HistoricalQuoteProvider historicalQuoteProvider )
		{
			ReturnsManager closeToCloseReturnsManager = this.getCloseToCloseReturnsManager(
				ticker , firstDateTime , lastDateTime , historicalQuoteProvider );
			double closeToCloseReturn = closeToCloseReturnsManager.GetReturn(
				ticker , 0 );
			return closeToCloseReturn;
		}
		#endregion getCloseToCloseReturn
		
		private double getAdjustedCloseToCloseReturn(
			string ticker , DateTime firstDateTime , DateTime lastDateTime )
		{
			double adjustedCloseToCloseReturn =	this.getCloseToCloseReturn(
				ticker , firstDateTime , lastDateTime , this.historicalAdjustedQuoteProvider );
			return adjustedCloseToCloseReturn;
		}
		private double getRawCloseToCloseReturn(
			string ticker , DateTime firstDateTime , DateTime lastDateTime )
		{
			double rawCloseToCloseReturn =	this.getCloseToCloseReturn(
				ticker , firstDateTime , lastDateTime , this.historicalRawQuoteProvider );
			return rawCloseToCloseReturn;
		}
		private bool hasThePriceBeenAdjustedWithinTheMultidayInterval(
			string ticker , DateTime firstDateTime , DateTime lastDateTime )
		{
			double adjustedReturn = this.getAdjustedCloseToCloseReturn(
				ticker , firstDateTime , lastDateTime );
			double rawReturn = this.getRawCloseToCloseReturn(
				ticker , firstDateTime , lastDateTime );
			bool hasBeenAdjusted =
				( Math.Abs( adjustedReturn - rawReturn ) >= 0.01 );
			return hasBeenAdjusted;
		}
		#endregion hasThePriceBeenAdjustedWithinTheMultidayInterval
		
		/// <summary>
		/// true iif the price has been adjusted (due to split, dividens, etc.)
		/// between firstDateTime and lastDateTime
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="firstDateTime"></param>
		/// <param name="lastDateTime"></param>
		/// <returns></returns>
		public bool HasThePriceBeenAdjusted(
			string ticker , DateTime firstDateTime , DateTime lastDateTime )
		{
			bool hasBeenAdjusted =
				(
					this.isReturnIntervalMultiday( firstDateTime , lastDateTime )
					&&
					this.hasThePriceBeenAdjustedWithinTheMultidayInterval(
						ticker , firstDateTime , lastDateTime )
				);
			return hasBeenAdjusted;
		}
	}
}
