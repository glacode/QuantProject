/*
QuantProject - Quantitative Finance Library

TsMSFTsimpleTest_2.cs
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
using QuantProject.ADT.Histories;
using QuantProject.ADT.Optimizing;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;

namespace QuantProject.Scripts
{
	/// <summary>
	/// Summary description for tsMSFTsimpleTest_2.
	/// </summary>
	public class TsMSFTsimpleTest_2 : TradingSystem
	{
		public TsMSFTsimpleTest_2()
		{
			//
			// TODO: Add constructor logic here
			//
		}

    private History microsoftCloseHistory;
    private History microsoftCloseHistorySimpleAverage;
	private History microsoftCloseHistoryStandardDeviation;

    public override void InitializeData()
    {
      microsoftCloseHistory = QuoteCache.GetCloseHistory( "MSFT" );
      microsoftCloseHistorySimpleAverage = 
		  microsoftCloseHistory.GetSimpleAverage(5, new DateTime(2000,1,1),new DateTime(2000,12,31));
	  microsoftCloseHistoryStandardDeviation = 
		  microsoftCloseHistory.GetStandardDeviation(5, new DateTime(2000,1,1),new DateTime(2000,12,31));
	}

    public override Signals GetSignals( ExtendedDateTime extendedDateTime )
    {
		Signals signals = new Signals();
		if ( extendedDateTime.BarComponent == BarComponent.Close &&
			microsoftCloseHistoryStandardDeviation.IsDecreased(extendedDateTime.DateTime))
		{
			Signal signal = new Signal();
			if ( this.microsoftCloseHistorySimpleAverage.IsDecreased(extendedDateTime.DateTime) )
			{
				
				signal.Add( new Order( OrderType.MarketSell , new Instrument( "MSFT" ) , 1 ,
					new ExtendedDateTime(
					new Instrument( "MSFT" ).GetNextMarketDay( extendedDateTime.DateTime ) ,
					BarComponent.Open ) ) );
				signals.Add( signal );
			}
			else
			{
				signal.Add( new Order( OrderType.MarketBuy , new Instrument( "MSFT" ) , 1 ,
					new ExtendedDateTime( new Instrument( "MSFT" ).GetNextMarketDay( extendedDateTime.DateTime ) ,
					BarComponent.Open ) ) );
				signals.Add( signal );
			}

		}
		
		return signals;
    }
    }	
}
