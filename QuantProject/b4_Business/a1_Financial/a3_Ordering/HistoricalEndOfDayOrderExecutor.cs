/*
QuantProject - Quantitative Finance Library

HistoricalEndOfDayOrderExecutor.cs
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

using QuantProject.Data.DataProviders;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Transactions;
using QuantProject.Business.Timing;


namespace QuantProject.Business.Financial.Ordering
{
	/// <summary>
	/// Simulates historical order executions/rejections for end of day simulation
	/// </summary>
	[Serializable]
	public class HistoricalEndOfDayOrderExecutor : IOrderExecutor
	{
		private IEndOfDayTimer timer;
		private IHistoricalQuoteProvider historicalQuoteProvider;

		public HistoricalEndOfDayOrderExecutor( IEndOfDayTimer timer ,
			IHistoricalQuoteProvider historicalQuoteProvider )
		{
			this.timer = timer;
			this.historicalQuoteProvider = historicalQuoteProvider;
		}

		public event OrderFilledEventHandler OrderFilled;

		// Tries to execute the order
		public void Execute( Order order )
		{
			double instrumentPrice = this.historicalQuoteProvider.GetMarketValue( order.Instrument.Key ,
				this.timer.GetCurrentTime() );
			EndOfDayTransaction endOfDayTransaction = new EndOfDayTransaction(
				TimedTransaction.GetTransactionType( order.Type ) , order.Instrument ,
				order.Quantity , instrumentPrice ,
				new EndOfDayDateTime( this.timer.GetCurrentTime().DateTime ,
				this.timer.GetCurrentTime().EndOfDaySpecificTime ) );
			OrderFilled( this , new OrderFilledEventArgs( order , endOfDayTransaction ) );
		}
	}
}
