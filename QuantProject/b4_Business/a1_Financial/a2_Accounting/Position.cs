/*
QuantProject - Quantitative Finance Library

Position.cs
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

namespace QuantProject.Business.Financial.Accounting
{
	/// <summary>
	/// Summary description for Position.
	/// </summary>
	/// 
  [Serializable]
  public class Position
	{
    private Instrument instrument;
    private long quantity;

    public Instrument Instrument
    {
      get { return instrument; }
      set { instrument = value; }
    }

    public long Quantity
    {
      get { return quantity; }
      set { quantity = value; }
    }

		public PositionType Type
		{
			get
			{
				if ( this.Quantity == 0 )
					throw new Exception( "Position Type cannot be requested " +
						"when no position is held!" );
				PositionType positionType;
				if ( this.Quantity > 0 )
					positionType = PositionType.Long;
				else
					// this.Quantity < 0
					positionType = PositionType.Short;
				return positionType;
			}
		}
		public bool IsLong
		{
			get { return ( this.Quantity >= 0 ); }
		}
		public bool IsShort
		{
			get { return ( !this.IsLong ); }
		}
		public Position( Instrument instrument , long quantity )
		{
			this.instrument = instrument;
      this.quantity = quantity;
		}

    public override string ToString()
    {      
      return
        " Instrument: " + this.Instrument.Key +
        " Quantity: " + this.Quantity + "\n";
    }
	}
}
