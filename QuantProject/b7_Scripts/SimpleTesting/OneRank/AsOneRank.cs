/*
QuantProject - Quantitative Finance Library

AsProfunds.cs
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
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Strategies;

namespace QuantProject.Scripts
{
	/// <summary>
	/// Summary description for AsProfunds.
	/// </summary>
	public class AsOneRank : AccountStrategy
	{
		public AsOneRank( Account account ) : base( account )
		{
			//
			// TODO: Add constructor logic here
			//
		}
    public override ArrayList GetOrdersForCurrentVirtualOrder( Order virtualOrder )
    {
      ArrayList orders = new ArrayList();
      switch(virtualOrder.Type)       
      {
        case OrderType.MarketBuy:
          if ( !this.account.Portfolio.IsLong( virtualOrder.Instrument ) )
            orders.Add( new Order( OrderType.MarketBuy ,
              virtualOrder.Instrument ,
              virtualOrder.Instrument.GetMaxBuyableQuantity(
              this.account.CashAmount +
              this.account.Portfolio.GetMarketValue( virtualOrder.ExtendedDateTime ) ,
              virtualOrder.ExtendedDateTime ) , virtualOrder.ExtendedDateTime ) );
          break;
        case OrderType.MarketSell:
          if ( this.account.Portfolio.IsLong( virtualOrder.Instrument ) )
            orders.Add( new Order( OrderType.MarketSell ,
              virtualOrder.Instrument ,
              (long) this.account.Portfolio.GetPosition( virtualOrder.Instrument ).Quantity ,
              virtualOrder.ExtendedDateTime ) );
          break;
        default:            
          break;      
      }
      return orders;
    }
	}
}
