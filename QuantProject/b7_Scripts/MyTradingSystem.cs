/*
QuantProject - Quantitative Finance Library

MyTradingSystem.cs
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
using QuantProject.Business.Timing;

namespace QuantProject.Scripts
{
	/// <summary>
	/// Summary description for MyTradingSystem.
	/// </summary>
	public class MyTradingSystem : TradingSystem
	{
    private History microsoftOpenHistory;
    private History microsoftOpenHistorySMA;

    public MyTradingSystem()
    {
    }

    public override void InitializeData()
    {
      Parameter parameter = (Parameter) this.Parameters[ "SMAdays" ];
      microsoftOpenHistory = QuoteCache.GetOpenHistory( "MSFT" );
      microsoftOpenHistorySMA = microsoftOpenHistory.GetSimpleMovingAverage( (int) parameter.Value );
    }

    public override Signals GetSignals( ExtendedDateTime extendedDateTime )
    {
      Signals signals = new Signals();
      Signal signal = new Signal();
      if ( microsoftOpenHistory.Cross( microsoftOpenHistorySMA ,
        extendedDateTime.DateTime ) && 
          ( Convert.ToDouble(microsoftOpenHistorySMA.GetValue( extendedDateTime.DateTime )) >
          ( Convert.ToDouble(microsoftOpenHistory.GetValue( extendedDateTime.DateTime )) *
          ( 100 + Convert.ToInt32(
          ((Parameter)this.Parameters[ "CrossPercentage" ]).Value ) ) / 100 ) ) )
      {
        signal.Add( new Order( OrderType.MarketBuy , new Instrument( "MSFT" ) , 1 ,
          new EndOfDayDateTime( new Instrument( "MSFT" ).GetNextMarketDay( extendedDateTime.DateTime ) ,
          EndOfDaySpecificTime.MarketOpen ) ) );
        signals.Add( signal );
      }
      else
      {
        if ( microsoftOpenHistorySMA.Cross( microsoftOpenHistory ,
          extendedDateTime.DateTime ) && 
          ( Convert.ToDouble(microsoftOpenHistorySMA.GetValue( extendedDateTime.DateTime )) >
          ( Convert.ToDouble(microsoftOpenHistory.GetValue( extendedDateTime.DateTime )) *
          ( 100 + Convert.ToInt32( ((Parameter)this.Parameters[ "CrossPercentage" ]).Value ) ) / 100 ) ) ) 
        {
          signal.Add( new Order( OrderType.MarketSell , new Instrument( "MSFT" ) , 1 ,
            new EndOfDayDateTime( new Instrument( "MSFT" ).GetNextMarketDay( extendedDateTime.DateTime ) ,
            EndOfDaySpecificTime.MarketOpen ) ) );
          signals.Add( signal );
        }
      }
      return signals;
    }
  }
}
