/*
QuantProject - Quantitative Finance Library

TsMSFTsimpleTest.cs
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
	/// Summary description for tsMSFTsimpleTest.
	/// </summary>
	public class TsMSFTwalkForward : TradingSystem
	{
		public TsMSFTwalkForward()
		{
			//
			// TODO: Add constructor logic here
			//
		}

    private History microsoftCloseHistory;
    private History microsoftCloseHistorySMA;

    public override void InitializeData()
    {
      Parameter parameter = (Parameter) this.Parameters[ "SMAdays" ];
      microsoftCloseHistory = QuoteCache.GetCloseHistory( "MSFT" );
      microsoftCloseHistorySMA = microsoftCloseHistory.GetSimpleMovingAverage( (int) parameter.Value );
    }

    public override Signals GetSignals( ExtendedDateTime extendedDateTime )
    {
      Signals signals = new Signals();
      if ( extendedDateTime.BarComponent == BarComponent.Close )
      {
        Signal signal = new Signal();
        if ( microsoftCloseHistory.Cross( microsoftCloseHistorySMA ,
          extendedDateTime.DateTime ) )
        {
          signal.Add( new Order( OrderType.MarketBuy , new Instrument( "MSFT" ) , 1 ,
            new ExtendedDateTime( new Instrument( "MSFT" ).GetNextMarketDay( extendedDateTime.DateTime ) ,
            BarComponent.Open ) ) );
          signals.Add( signal );
        }
        else
        {
          if ( microsoftCloseHistorySMA.Cross( microsoftCloseHistory ,
            extendedDateTime.DateTime ) )
          {
            signal.Add( new Order( OrderType.MarketSell , new Instrument( "MSFT" ) , 1 ,
              new ExtendedDateTime(
              new Instrument( "MSFT" ).GetNextMarketDay( extendedDateTime.DateTime ) ,
              BarComponent.Open ) ) );
            signals.Add( signal );
          }
        }
      }
      return signals;
    }
	}
}
