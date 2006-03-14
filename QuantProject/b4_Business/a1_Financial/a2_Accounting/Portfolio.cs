/*
QuantProject - Quantitative Finance Library

Portfolio.cs
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
using System.Runtime.Serialization;
using System.Collections;
using System.Diagnostics;

using QuantProject.ADT;
using QuantProject.ADT.Collections;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;

namespace QuantProject.Business.Financial.Accounting
{
	/// <summary>
	/// Summary description for Portfolio.
	/// </summary>
  [Serializable]
  public class Portfolio : QPHashtable
  {
		public ICollection Positions
		{
			get { return this.Values; }
		}
    public Portfolio() : base()
    {
    }
    public Portfolio(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }


		public Position GetPosition( string ticker )
		{
			return ((Position)this[ ticker ]);
		}

		public Position GetPosition( Instrument instrument )
		{
			return this.GetPosition( instrument.Key );
		}

		public bool Contains( Instrument instrument )
		{
			return base.ContainsKey( instrument ) || 
				base.ContainsKey( instrument.Key );
		}
		/// <summary>
		/// True iff the instrument corresponding to ticker is already
		/// into the portfolio
		/// </summary>
		/// <param name="ticker"></param>
		/// <returns></returns>
		public bool Contains( string ticker )
		{
			return this.ContainsKey( ticker );
		}

		public bool IsLong( string ticker )
		{
			return this.Contains( ticker ) && (((Position)this[ ticker ]).Quantity > 0);
		}
		public bool IsLong( Instrument instrument )
		{
			return IsLong( instrument.Key );
		}

		public bool IsShort( string ticker )
		{
			return this.Contains( ticker ) && (((Position)this[ ticker ]).Quantity < 0);
		}
		public bool IsShort( Instrument instrument )
		{
			return this.IsShort( instrument.Key );
		}


    #region "Update"

    private void add( Instrument instrument , long quantity )
    {
      if ( this.Contains( instrument ) )
        // the portfolio already contains such instrument
      {
        this.GetPosition( instrument ).Quantity += quantity;
      }
      else
        // the portfolio doesn't contain such instrument yet
        this.Add( instrument.Key , new Position( instrument , quantity ) );
      if (this.GetPosition( instrument ).Quantity == 0)
        this.Remove( instrument.Key );
    }

    private void remove( Instrument instrument , long quantity )
    {
      if ( this.Contains( instrument ) )
        // the portfolio already contains such instrument
        this.GetPosition( instrument ).Quantity -= quantity;
      else
        // the portfolio doesn't contain such instrument yet
        this.Add( instrument.Key , new Position( instrument , -quantity ) ) ;
      if ( this.GetPosition( instrument ).Quantity == 0 )
        this.Remove( instrument.Key );
    }


    public bool Update( Transaction transaction )
    {
      bool errorArised = false;
      //Debug.WriteLine( transaction.ToString() );
      switch ( transaction.Type )
      {
        case TransactionType.BuyLong:
        case TransactionType.Cover:
          this.add( transaction.Instrument , transaction.Quantity );
          break;
        case TransactionType.SellShort:
        case TransactionType.Sell:
          this.remove( transaction.Instrument , transaction.Quantity );
          break;
				case TransactionType.AddCash:
					break;
        default:
          errorArised = true;
          break;
      }
      return errorArised;
    }

    #endregion

		public double GetMarketValue( EndOfDayDateTime endOfDayDateTime ,
			IHistoricalQuoteProvider historicalQuoteProvider )
		{
			double totalValue = 0;
			foreach (Position position in this.Values)
				totalValue += historicalQuoteProvider.GetMarketValue(
					position.Instrument.Key , endOfDayDateTime ) * position.Quantity;
			return totalValue;
		}
		public double GetMarketValue( IDataStreamer dataStreamer )
		{
			double totalValue = 0;
			foreach (Position position in this.Values)
				totalValue += dataStreamer.GetCurrentBid(
					position.Instrument.Key ) * position.Quantity;
			return totalValue;
		}
		public override string ToString()
    {
      string toString = "";

      foreach (Position position in this.Values )
        toString = toString + position.ToString();
      return toString;
    }
  }
}
