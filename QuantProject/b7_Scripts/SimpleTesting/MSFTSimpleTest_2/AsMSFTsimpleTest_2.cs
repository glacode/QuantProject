/*
QuantProject - Quantitative Finance Library

AsMSFTsimpleTest_2.cs
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
using QuantProject.ADT;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Strategies;

namespace QuantProject.Scripts
{
	/// <summary>
	/// Summary description for AsMSFTsimpleTest_2.
	/// </summary>
	public class AsMSFTsimpleTest_2 : AccountStrategy
	{
		public AsMSFTsimpleTest_2( Account account ) : base( account )
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
					if(virtualOrder.Quantity != 0)
					{
						if ( this.account.Portfolio.IsShort( virtualOrder.Instrument ) )
							orders.Add( new Order( OrderType.MarketCover ,
								virtualOrder.Instrument ,
								(long) - ((Position)this.account.Portfolio[ virtualOrder.Instrument.Key ]).Quantity ,
								virtualOrder.ExtendedDateTime ) );
						
						if ( !this.account.Portfolio.IsLong( virtualOrder.Instrument ) )
							orders.Add( new Order( OrderType.MarketBuy ,
								virtualOrder.Instrument ,
								virtualOrder.Instrument.GetMaxBuyableQuantity(
								this.account.CashAmount +
								this.account.Portfolio.GetMarketValue( virtualOrder.ExtendedDateTime ) ,
								virtualOrder.ExtendedDateTime ) , virtualOrder.ExtendedDateTime ) );
					}
					else
					//it is a special order that acts in order to close any open position
					{
						if ( this.account.Portfolio.IsShort( virtualOrder.Instrument ) )
							orders.Add( new Order( OrderType.MarketCover ,
								virtualOrder.Instrument ,
								-(long) this.account.Portfolio.GetPosition( virtualOrder.Instrument ).Quantity ,
								virtualOrder.ExtendedDateTime ) );
					
						if ( this.account.Portfolio.IsLong( virtualOrder.Instrument ) )
							orders.Add( new Order( OrderType.MarketSell  ,
								virtualOrder.Instrument ,
								(long) this.account.Portfolio.GetPosition( virtualOrder.Instrument ).Quantity ,
								virtualOrder.ExtendedDateTime ) );
					}
					break;

				case OrderType.MarketSell:
					if ( this.account.Portfolio.IsLong( virtualOrder.Instrument ) )
						orders.Add( new Order( OrderType.MarketSell ,
							virtualOrder.Instrument ,
							(long) this.account.Portfolio.GetPosition( virtualOrder.Instrument ).Quantity ,
							virtualOrder.ExtendedDateTime ) );
					if ( !this.account.Portfolio.IsShort( virtualOrder.Instrument ) )
						orders.Add( new Order( OrderType.MarketSellShort ,
							virtualOrder.Instrument ,
							virtualOrder.Instrument.GetMaxBuyableQuantity( 
							this.account.CashAmount +
							this.account.Portfolio.GetMarketValue( virtualOrder.ExtendedDateTime ) ,
							virtualOrder.ExtendedDateTime ) ,
							virtualOrder.ExtendedDateTime ) );
					break;
				default:            
					break;      
			}
			return orders;
		}

		public override Orders GetOrders( Signal signal )
		{
			// This is the default account strategy. You may wish to override it.
			// It assumes the signal contains a single virtual order and it invests
			// all the available cash according to such virtual order. It assumes
			// the account will always contain a single position (long or short)
			// determined by the last signal.
			Orders orders = new Orders();
			foreach ( Order virtualOrder in signal )
			{
				ArrayList ordersForCurrentVirtualOrder =
					this.GetOrdersForCurrentVirtualOrder( virtualOrder );
				foreach( Order order in ordersForCurrentVirtualOrder )
					orders.Add( order );
			}
			return orders;
		}
	
	}
	
}
