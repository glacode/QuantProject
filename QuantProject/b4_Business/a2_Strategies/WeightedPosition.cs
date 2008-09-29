/*
QuantProject - Quantitative Finance Library

SignedTicker.cs
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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;

namespace QuantProject.Business.Strategies
{
	[Serializable]
	/// <summary>
	/// Weighted representation for a possible position
	/// </summary>
	public class WeightedPosition
	{
		private string ticker;
		private double weight;

		public string Ticker
		{
			get { return this.ticker; }
		}
		public double Weight
		{
			get { return this.weight; }
			set { this.weight = value; }
		}
		public bool IsShort
		{
			get { return ( this.weight < 0 ); }
		}
		public bool IsLong
		{
			get { return ( !this.IsShort ); }
		}
		public WeightedPosition( double weight , string ticker )
		{
			if ( ( weight > 1 ) || ( weight < -1 ) )
				throw new Exception( "Unacceptable weight value. The weight must be " +
					"in the [-1,+1] interval, but the given weight parameter value is " +
					weight.ToString() );
			else
			{
				this.weight = weight;
				this.ticker = ticker;
			}
		}
		/// <summary>
		/// returns a MarketBuy if the position is long, a
		/// MarketSellShort otherwise
		/// </summary>
		/// <returns></returns>
		public OrderType GetOrderType()
		{
			OrderType orderType = OrderType.MarketBuy;
			if ( this.IsShort )
				orderType = OrderType.MarketSellShort;
			return orderType;
		}
		/// <summary>
		/// returns the WeightedPosition with an opposite weight
		/// </summary>
		/// <returns></returns>
		public WeightedPosition GetOppositeWeightedPosition()
		{
			return new WeightedPosition( -this.weight , this.ticker );
		}
		/// <summary>
		/// returns the close to close daily return, for this weighted
		/// position, at the dateTime date
		/// </summary>
		/// <param name="dateTime">date when to compute the daily return</param>
		/// <returns></returns>
		public double GetCloseToCloseDailyReturn(	DateTime dateTime )
		{
			string ticker = this.Ticker;
			HistoricalAdjustedQuoteProvider historicalAdjustedQuoteProvider =
				new HistoricalAdjustedQuoteProvider();
			double todayMarketValueAtClose =
				historicalAdjustedQuoteProvider.GetMarketValue(
					ticker , HistoricalEndOfDayTimer.GetMarketClose( dateTime ) );
//				new EndOfDayDateTime( dateTime , EndOfDaySpecificTime.MarketClose ) );
			DateTime yesterday = dateTime.AddDays( -1 );
			DateTime yesterdayAtClose =
				HistoricalEndOfDayTimer.GetMarketClose( yesterday );
//				new	EndOfDayDateTime( yesterday ,	EndOfDaySpecificTime.MarketClose );
			double yesterdayMarketValueAtClose =
				historicalAdjustedQuoteProvider.GetMarketValue(
					ticker , yesterdayAtClose );
			double dailyReturnForLongPosition =
				( todayMarketValueAtClose / yesterdayMarketValueAtClose ) - 1;
			double dailyReturn = dailyReturnForLongPosition * this.Weight;
//			if ( this.IsShort )
//				// this weighted position represents a short position
//				dailyReturn = - dalyReturnForLongPosition;
//			else
//				// this weighted position represents a long position
//				dailyReturn = dalyReturnForLongPosition;
			return dailyReturn;
		}
		public override string ToString()
		{
			string toString = this.Ticker + ";" + this.Weight.ToString();
			return toString;
		}
		
		public bool HasTheSameSignedTickerAs(WeightedPosition weightedPosition)
		{
			if ( weightedPosition.Ticker == this.Ticker &&
					 ( (weightedPosition.IsLong && this.IsLong) ||
						 (weightedPosition.IsShort && this.IsShort)  )  )
				return true;
			else
				return false;
		}
		
		public bool HasTheOppositeSignedTickerAs(WeightedPosition weightedPosition)
		{
			if ( weightedPosition.Ticker == this.Ticker &&
					 ( (weightedPosition.IsShort && this.IsLong) ||
						 (weightedPosition.IsLong && this.IsShort)  )  )
				return true;
			else
				return false;
		}
	}
}
