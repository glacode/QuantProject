/*
QuantProject - Quantitative Finance Library

TimedTransaction.cs
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
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;

namespace QuantProject.Business.Financial.Accounting.Transactions
{

	/// <summary>
	/// Transaction to be used with end of day strategies
	/// </summary>
  [Serializable]
  public class EndOfDayTransaction : Transaction
  {
    private EndOfDayDateTime endOfDayDateTime;

    public EndOfDayDateTime EndOfDayDateTime
    {
      get
      {
        return endOfDayDateTime;
      }
    }


    public EndOfDayTransaction( TransactionType transactionType , Double transactionAmount ,
      EndOfDayDateTime endOfDayDateTime )
      : base( transactionType , transactionAmount )
    {
      //base( transactionType , transactionAmount );
      this.endOfDayDateTime = endOfDayDateTime;
    }

    public EndOfDayTransaction( TransactionType transactionType , Instrument instrument ,
      long quantity , double instrumentPrice , EndOfDayDateTime endOfDayDateTime )
      : base( transactionType , instrument , quantity , instrumentPrice )
    {
      this.endOfDayDateTime = endOfDayDateTime;
    }

		static public TransactionType GetTransactionType( OrderType orderType )
		{
			TransactionType returnValue;
			switch ( orderType )
			{
				case OrderType.LimitBuy:
					returnValue = TransactionType.BuyLong;
					break;
				case OrderType.MarketBuy:
					returnValue = TransactionType.BuyLong;
					break;
				case OrderType.LimitCover:
					returnValue = TransactionType.Cover;
					break;
				case OrderType.MarketCover:
					returnValue = TransactionType.Cover;
					break;
				case OrderType.LimitSell:
					returnValue = TransactionType.Sell;
					break;
				case OrderType.MarketSell:
					returnValue = TransactionType.Sell;
					break;
				case OrderType.LimitSellShort:
					returnValue = TransactionType.SellShort;
					break;
				case OrderType.MarketSellShort:
					returnValue = TransactionType.SellShort;
					break;
					//this line should never be reached!
				default:
					returnValue = TransactionType.AddCash;
					break;
			}
			return returnValue;
		}
    public override string ToString()
    {
      return
        base.ToString() +
        "\n   DateTime: " + this.endOfDayDateTime.ToString();
    }
  }
}
