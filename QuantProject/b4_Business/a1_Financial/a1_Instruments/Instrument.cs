/*
QuantProject - Quantitative Finance Library

Instrument.cs
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
using QuantProject.ADT.Histories;
using QuantProject.Data;

namespace QuantProject.Business.Financial.Instruments
{
	/// <summary>
	/// Summary description for Instrument.
	/// </summary>
  [Serializable]
  public class Instrument : Keyed
	{
		public Instrument( string key)
		{
			//
			// TODO: Add constructor logic here
			//
      this.Key = key;
		}

    public double GetMarketValue( ExtendedDateTime extendedDateTime )
    {
      return DataProvider.GetMarketValue( this.Key , extendedDateTime );
    }

    public long GetMaxBuyableQuantity( double availableAmount ,
      ExtendedDateTime extendedDateTime )
    {
      return (long) Math.Floor( availableAmount / this.GetMarketValue( extendedDateTime ) );
    }

    public DateTime GetNextMarketDay( DateTime dateTime )
    {
      History history = DataProvider.GetOpenHistory( this.Key );
      return history.GetNextDay( dateTime );
    }
	}
}
