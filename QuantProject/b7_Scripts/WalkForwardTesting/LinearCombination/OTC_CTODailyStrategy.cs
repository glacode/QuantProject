/*
QuantProject - Quantitative Finance Library

OTC_CTODailyStrategy.cs
Copyright (C) 2003 
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
using System.Collections;

using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Open to close - Close to open daily strategy
	/// </summary>
	[Serializable]
	public class OTC_CTODailyStrategy : IEndOfDayStrategy
	{
		private Account account;
		private string[] signedTickers;
		private double[] weightsForSignedTickers;

		public OTC_CTODailyStrategy( Account account ,
			string[] signedTickers, double[] weightsForSignedTickers)
		{
			this.account = account;
			this.signedTickers = signedTickers;
			this.weightsForSignedTickers = weightsForSignedTickers;
		}
		private long addOrders_addOrder_getQuantity(
			int indexForSignedTicker )
		{
			double accountValue = this.account.GetMarketValue();
			double currentTickerAsk =
				this.account.DataStreamer.GetCurrentAsk( SignedTicker.GetTicker(this.signedTickers[indexForSignedTicker]) );
			double maxPositionValueForThisTicker =
				accountValue*this.weightsForSignedTickers[indexForSignedTicker];
			long quantity = Convert.ToInt64(	Math.Floor(
				maxPositionValueForThisTicker /	currentTickerAsk ) );
			return quantity;
		}
		private void addOrders_addOrder( int indexForSignedTicker )
		{
			OrderType orderType = GenomeRepresentation.GetOrderType( this.signedTickers[indexForSignedTicker] );
			string ticker = GenomeRepresentation.GetTicker( this.signedTickers[indexForSignedTicker] );
			long quantity = addOrders_addOrder_getQuantity( indexForSignedTicker );
			Order order = new Order( orderType , new Instrument( ticker ) ,
				quantity );
			this.account.AddOrder( order );
		}
		private void addOrders()
		{
			for(int i = 0; i<this.signedTickers.Length; i++)
				  this.addOrders_addOrder( i );
		}
		public void MarketOpenEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			this.closePositions();
      if ( ( this.account.CashAmount == 0 ) &&
				( this.account.Transactions.Count == 0 ) )
				// cash has not been added yet
				this.account.AddCash( 30000 );
			this.addOrders();
		}
		private void closePositions()
		{
			ArrayList tickers = new ArrayList();
			foreach ( Position position in this.account.Portfolio.Positions )
				tickers.Add( position.Instrument.Key );
			foreach ( string ticker in tickers )
				this.account.ClosePosition( ticker );
		}
		public void FiveMinutesBeforeMarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs)
		{
			
		}

		public void MarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs)
		{
      this.closePositions();
      SignedTicker.ChangeSignOfEachTicker(this.signedTickers);
      this.addOrders();
      SignedTicker.ChangeSignOfEachTicker(this.signedTickers);
		}
		
    public void OneHourAfterMarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs)
		{
		}

	}
}
