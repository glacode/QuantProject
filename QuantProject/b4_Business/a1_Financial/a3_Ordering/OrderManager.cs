/*
QuantProject - Quantitative Finance Library

OrderManager.cs
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
using QuantProject.Data.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Transactions;

namespace QuantProject.Business.Financial.Ordering
{
	/// <summary>
	/// Summary description for OrderManager.
	/// </summary>
	public class OrderManager
	{
		public OrderManager()
		{
			//
			// TODO: Add constructor logic here
			//
		}

//    public virtual double GetInstrumentPrice( Order order )
//    {
//      return HistoricalDataProvider.GetMarketValue( order.Instrument.Key ,
//				order.EndOfDayDateTime.GetNearestExtendedDateTime() );
//    }
    #region "GetTransaction"

    private TransactionType getTransactionType( Order order )
    {
      switch (order.Type)
      {
        case OrderType.LimitBuy:
          return TransactionType.BuyLong;
        case OrderType.LimitCover:
          return TransactionType.Cover;
        case OrderType.LimitSell:
          return TransactionType.Sell;
        case OrderType.LimitSellShort:
          return TransactionType.SellShort;
        case OrderType.MarketBuy:
          return TransactionType.BuyLong;
        case OrderType.MarketCover:
          return TransactionType.Cover;
        case OrderType.MarketSell:
          return TransactionType.Sell;
        case OrderType.MarketSellShort:
          return TransactionType.SellShort;
        default:
          // it should never be reached
          return TransactionType.BuyLong;
      }
    }

    public EndOfDayTransaction GetTransaction( Order order ,
			IDataStreamer dataStreamer )
    {
      double instrumentPrice = dataStreamer.GetCurrentBid(
				order.Instrument.Key );
      EndOfDayTransaction transaction = new EndOfDayTransaction(
        getTransactionType( order ) , order.Instrument ,
        order.Quantity , instrumentPrice ,
        order.EndOfDayDateTime );
      return transaction;
    }
    #endregion

    public TransactionHistory GetTransactions( ArrayList orders ,
			IDataStreamer dataStreamer )
    {
      TransactionHistory transactionHistory = new TransactionHistory();
      foreach (Order order in orders)
        transactionHistory.Add( this.GetTransaction( order ,
					dataStreamer ) );
      return transactionHistory;
    }
	}
}
