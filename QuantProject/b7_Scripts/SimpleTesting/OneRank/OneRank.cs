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

using QuantProject.Data.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.SimpleTesting
{
	/// <summary>
	/// Performs the OneRank strategy on a single ticker
	/// </summary>
	public class OneRank
	{
		private Account account;

		private DateTime lastDateTime;

		public void fiveMinutesBeforeMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			long sharesToBeBought;
			double todayMarketValueAtClose = this.account.DataStreamer.GetCurrentBid(
				this.account.Key );
			EndOfDayDateTime yesterdayAtClose = new
				EndOfDayDateTime( this.account.EndOfDayTimer.GetCurrentTime().DateTime.AddHours( - 1 ) ,
				EndOfDaySpecificTime.MarketClose );
			double yesterdayMarketValueAtClose = HistoricalDataProvider.GetMarketValue(
				this.account.Key ,
				yesterdayAtClose.GetNearestExtendedDateTime() );
			if ( ( todayMarketValueAtClose > yesterdayMarketValueAtClose ) &&
				( !this.account.Contains( this.account.Key ) ) )
			{
				// today close is higher than yesterday close and no position
				// is kept in portfolio, yet
				sharesToBeBought = Convert.ToInt64(
					Math.Floor( this.account.CashAmount /
					this.account.DataStreamer.GetCurrentAsk( this.account.Key ) ) );
				this.account.AddOrder( new Order( OrderType.MarketBuy ,
					new Instrument( this.account.Key ) , sharesToBeBought ) );
			}
			if ( ( todayMarketValueAtClose < yesterdayMarketValueAtClose ) &&
				( this.account.Contains( this.account.Key ) ) )
				this.account.ClosePosition( this.account.Key );
			if ( this.account.EndOfDayTimer.GetCurrentTime().DateTime >
				this.lastDateTime )
				this.account.EndOfDayTimer.Stop();
		}
		/// <summary>
		/// Runs the OneRank strategy
		/// </summary>
		/// <param name="account">Account with which to run the strategy</param>
		/// <param name="lastDateTime">Date to stop the strategy</param>
		public OneRank( Account account , DateTime lastDateTime )
		{
			this.account = account;
			this.lastDateTime = lastDateTime;
			this.account.EndOfDayTimer.FiveMinutesBeforeMarketClose +=
				new FiveMinutesBeforeMarketCloseEventHandler(
				this.fiveMinutesBeforeMarketCloseEventHandler );
			this.account.EndOfDayTimer.Start();
		}
	}
}
