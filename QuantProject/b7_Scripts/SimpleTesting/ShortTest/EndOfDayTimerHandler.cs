/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandler.cs
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
using System.Collections;

using QuantProject.ADT;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;
using QuantProject.Scripts.SimpleTesting;

namespace QuantProject.Scripts.SimpleTesting.ShortTest
{
	/// <summary>
	/// Short Test core strategy
	/// </summary>
	public class EndOfDayTimerHandler
	{

		private Account account;
		private DateTime endDateTime;

		public Account Account
		{
			get { return this.account; }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="numberEligibleTickers">number of tickers to be chosen with the first selection:
		/// the best performers will be chosen among these first selected instruments</param>
		/// <param name="numberBestPeformingTickers">number of instruments to be chosen, as the best
		/// performers, among the eligible tickers</param>
		/// <param name="numberOfTickersToBeChosen">number of instruments to be chosen,
		/// among the best performers</param>
		/// <param name="windowDays">number of days between two consecutive
		/// best performing ticker calculation</param>
		public EndOfDayTimerHandler( Account account ,
			DateTime endDateTime )
		{
			this.account = account;
			this.endDateTime = endDateTime;
		}
		/// <summary>
		/// Handles a "Market Open" event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		public void MarketOpenEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			this.account.AddOrder( new Order( OrderType.MarketSellShort ,
				new Instrument( "MSFT" ) , 100 ) );
		}
		public void MarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			this.account.ClosePosition( "MSFT" );
			if ( this.account.EndOfDayTimer.GetCurrentTime().DateTime >
				this.endDateTime )
				this.account.EndOfDayTimer.Stop();

		}
	}
}
