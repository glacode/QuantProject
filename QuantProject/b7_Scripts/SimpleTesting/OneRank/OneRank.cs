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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;


namespace QuantProject.Scripts.SimpleTesting
{
	/// <summary>
	/// Performs the OneRank strategy on a single ticker
	/// </summary>
	public class OneRank : EndOfDayTimerHandler
	{
		private Account account;

		private DateTime lastDateTime;
		private HistoricalMarketValueProvider historicalMarketValueProvider =
			new HistoricalAdjustedQuoteProvider();

		public static long MaxBuyableShares( string ticker , double cashAmount, IDataStreamer dataStreamer )
		{
			return Convert.ToInt64(
				Math.Floor( cashAmount / dataStreamer.GetCurrentAsk( ticker ) ) );
		}

		protected override void marketOpenEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( ( this.account.CashAmount == 0 ) &&
			    ( this.account.Transactions.Count == 0 ) )
				// cash has not been added yet
				this.account.AddCash( 10000 );
			if ( dateTime > this.lastDateTime )
				this.account.Timer.Stop();
		}
		private void buyLongTicker()
		{
			long sharesToBeBought;
			sharesToBeBought = MaxBuyableShares( this.account.Key ,
			                                    this.account.CashAmount , this.account.DataStreamer );
			this.account.AddOrder( new Order( OrderType.MarketBuy ,
			                                 new Instrument( this.account.Key ) , sharesToBeBought ) );
		}
		private void sellShortTicker()
		{
			long sharesToBeSold;
			sharesToBeSold = MaxBuyableShares( this.account.Key ,
			                                  this.account.CashAmount , this.account.DataStreamer );
			this.account.AddOrder( new Order( OrderType.MarketSellShort ,
			                                 new Instrument( this.account.Key ) , sharesToBeSold ) );
		}
		private void marketCloseEventHandler_withTickerExchangedNow()
		{
			double todayMarketValueAtClose = this.account.DataStreamer.GetCurrentBid(
				this.account.Key );
			DateTime yesterdayAtClose =
				HistoricalEndOfDayTimer.GetMarketClose(
					this.account.Timer.GetCurrentDateTime().AddDays( - 1 ) );
//			new	EndOfDayDateTime( this.account.EndOfDayTimer.GetCurrentTime().DateTime.AddDays( - 1 ) ,
//			                     EndOfDaySpecificTime.MarketClose );
			double yesterdayMarketValueAtClose = this.historicalMarketValueProvider.GetMarketValue(
				this.account.Key , yesterdayAtClose );
			if ( ( todayMarketValueAtClose > yesterdayMarketValueAtClose ) )
			{
				if ( this.account.Contains( this.account.Key ) )
				{
					// today close is higher than yesterday close and
					// a position is already kept in portfolio
					if ( this.account.Portfolio.GetPosition(
						this.account.Key ).Type == PositionType.Short )
						// today close is higher than yesterday close and
						// a short position is kept in portfolio
					{
						this.account.ClosePosition( this.account.Key );
						this.buyLongTicker();
					}
				}
				else
					// today close is higher than yesterday close and
					// no position is kept in portfolio
					this.buyLongTicker();
			}
			if ( ( todayMarketValueAtClose < yesterdayMarketValueAtClose ) )
			{
				if ( this.account.Contains( this.account.Key ) )
				{
					// today close is lower than yesterday close and
					// a position is already kept in portfolio
					if ( this.account.Portfolio.GetPosition(
						this.account.Key ).Type == PositionType.Long )
						// today close is lower than yesterday close and
						// a long position is kept in portfolio
					{
						this.account.ClosePosition( this.account.Key );
						this.sellShortTicker();
					}
				}
				else
					// today close is lower than yesterday close and
					// no position is kept in portfolio
					this.sellShortTicker();
			}
		}
		protected override void marketCloseEventHandler(
			Object sender , DateTime dateTime )
		{

			if ( this.account.DataStreamer.IsExchanged( this.account.Key ) )
				// the given ticker is currently exchanged
				marketCloseEventHandler_withTickerExchangedNow();
		}
		protected override void oneHourAfterMarketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			;
		}
		/// <summary>
		/// Runs the OneRank strategy
		/// </summary>
		/// <param name="account">Account with which to run the strategy</param>
		/// <param name="lastDateTime">Date to stop the strategy</param>
		public OneRank( Account account1 , DateTime lastDateTime )
		{
			this.account = account1;
			this.lastDateTime = lastDateTime;
			
			this.account.Timer.NewDateTime +=
				new NewDateTimeEventHandler( this.NewDateTimeEventHandler );
//			this.account.Timer.MarketOpen +=
//				new MarketOpenEventHandler(
//					this.marketOpenEventHandler );
//			this.account.Timer.FiveMinutesBeforeMarketClose +=
//				new FiveMinutesBeforeMarketCloseEventHandler(
//					this.fiveMinutesBeforeMarketCloseEventHandler );
			
			this.account.Timer.Start();
		}
	}
}
