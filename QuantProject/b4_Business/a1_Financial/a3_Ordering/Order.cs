/*
QuantProject - Quantitative Finance Library

Order.cs
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
using QuantProject.Business.Financial.Instruments;
using QuantProject.ADT;

namespace QuantProject.Business.Financial.Ordering
{
	/// <summary>
	/// Summary description for Order.
	/// </summary>
	public class Order
	{
    private OrderType orderType;
    private Instrument instrument;
    private long quantity;
    private ExtendedDateTime extendedDateTime;

    public OrderType Type
    {
      get { return orderType; }
      set { orderType=value;  }
    }

    public Instrument Instrument
    {
      get { return instrument; }
      set { instrument=value;  }
    }
    
    public long Quantity
    {
      get { return quantity; }
      set { quantity=value;  }
    }

    public ExtendedDateTime ExtendedDateTime
    {
      get { return extendedDateTime; }
      set { extendedDateTime=value;  }
    }

//    public Order( OrderType orderType , Instrument instrument , long quantity )
//    {
//      Type = orderType;
//      this.Instrument = instrument;
//      Quantity = quantity;
//    }

    public Order( OrderType orderType , Instrument instrument ,
      long quantity , ExtendedDateTime extendedDateTime )
    {
      Type = orderType;
      this.Instrument = instrument;
      Quantity = quantity;
      this.extendedDateTime = extendedDateTime;
    }
  }
}
