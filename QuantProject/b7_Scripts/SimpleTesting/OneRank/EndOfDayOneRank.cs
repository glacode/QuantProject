/*
QuantProject - Quantitative Finance Library

OneRank.cs
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
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;

namespace QuantProject.Scripts.SimpleTesting
{
	/// <summary>
	/// One rank strategy on a single ticker
	/// </summary>
	public class EndOfDayOneRank
	{
		private string ticker;
		private Account account;
		private IHistoricalQuoteProvider historicalQuoteProvider =
			new HistoricalAdjustedQuoteProvider();

		private void fiveMinutesBeforeMarketCloseHandler(
			Object sender , EndOfDayTimingEventArgs eventArgs )
		{
			if ( this.account.GetMarketValue( this.ticker ) <=
				this.historicalQuoteProvider.GetMarketValue( this.ticker ,
				new EndOfDayDateTime(
					this.account.EndOfDayTimer.GetCurrentTime().DateTime.AddDays( - 1 ) ,
				EndOfDaySpecificTime.MarketClose ) ) )
			{
				// today's closing value is lower or equal than yesterday's closing value
				if ( this.account.Portfolio.Contains( this.ticker ) )
					this.account.ClosePosition( this.ticker );
			}
			else
			{
				// today's closing value is higher than yesterday's closing value
				if ( !this.account.Portfolio.Contains( this.ticker ) )
				{
					int maxBuyableQuantity = Convert.ToInt16(
						Math.Floor( this.account.CashAmount / this.account.GetMarketValue( this.ticker )));
					this.account.AddOrder( new Order( OrderType.MarketBuy ,
						new Instrument( ticker ) ,
						maxBuyableQuantity ) );
					}
			}
		}
		public EndOfDayOneRank( string ticker , Account account )
		{
			this.ticker = ticker;
			this.account = account;
			this.account.AddCash( 10000 );
			this.account.EndOfDayTimer.FiveMinutesBeforeMarketClose +=
				new FiveMinutesBeforeMarketCloseEventHandler(
					this.fiveMinutesBeforeMarketCloseHandler );
		}
	}
}
