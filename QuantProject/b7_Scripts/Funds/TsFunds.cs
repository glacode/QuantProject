/*
QuantProject - Quantitative Finance Library

TsProfunds.cs
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
using QuantProject.ADT.Histories;
using QuantProject.ADT.Optimizing;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;

namespace QuantProject.Scripts
{
	/// <summary>
	/// Summary description for TsProfunds.
	/// </summary>
	public class TsFunds : TradingSystem
	{
    public string Ticker;

		public TsFunds()
		{
			//
			// TODO: Add constructor logic here
			//
		}
    private History fund;
//    private History microsoftCloseHistorySMA;

    public override void InitializeData()
    {
      fund = QuoteCache.GetOpenHistory( Ticker );
//      microsoftCloseHistorySMA = microsoftCloseHistory.GetSimpleMovingAverage( (int) parameter.Value );
    }

    public override Signals GetSignals( ExtendedDateTime extendedDateTime )
    {
      Signals signals = new Signals();
      if ( !fund.IsLastKey( extendedDateTime.DateTime ) &&
        ( extendedDateTime.BarComponent == BarComponent.Close ) )
      {
        DateTime previousMarketDay = (DateTime)((History)fund).GetKey(
          ((History)fund).IndexOfKeyOrPrevious( extendedDateTime.DateTime ) );
        DateTime nextMarketDay =
          new Instrument( Ticker ).GetNextMarketDay( extendedDateTime.DateTime );
        Signal signal = new Signal();
        if ( (Single)fund[ nextMarketDay ] > (Single)fund[ previousMarketDay ] )
        {
          signal.Add( new Order( OrderType.MarketBuy , new Instrument( Ticker ) , 1 ,
            new ExtendedDateTime( nextMarketDay , BarComponent.Open ) ) );
          signals.Add( signal );
        }
        if ( (Single)fund[ nextMarketDay ] < (Single)fund[ previousMarketDay ] )
        {
          signal.Add( new Order( OrderType.MarketSell , new Instrument( Ticker ) , 1 ,
            new ExtendedDateTime( nextMarketDay , BarComponent.Open ) ) );
          signals.Add( signal );
        }
      }
      return signals;
    }
	}
}
