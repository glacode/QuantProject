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
using QuantProject.ADT.Statistics;
using QuantProject.ADT.Optimizing;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;

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
			
			microsoftCloseHistoryStandardDeviation = 
			microsoftCloseHistory.GetFunctionHistory (Function.StandardDeviation,5,
													  (DateTime)this.microsoftCloseHistory.GetKey(0),
													  (DateTime)this.microsoftCloseHistory.GetKey(this.microsoftCloseHistory.Count - 1));
			microsoftCloseHistorySimpleAverage = 
				microsoftCloseHistory.GetFunctionHistory (Function.SimpleAverage,5,
													  (DateTime)this.microsoftCloseHistory.GetKey(0),
													  (DateTime)this.microsoftCloseHistory.GetKey(this.microsoftCloseHistory.Count - 1));
		}

		public override Signals GetSignals( ExtendedDateTime extendedDateTime )
		{
			Signals signals = new Signals();
			Signal signal = new Signal();
			if ( extendedDateTime.BarComponent == BarComponent.Close)
			{	
				if(microsoftCloseHistoryStandardDeviation.IsDecreased(extendedDateTime.DateTime))
				{
					if ( this.microsoftCloseHistorySimpleAverage.IsDecreased(extendedDateTime.DateTime) )
					{
						
						signal.Add( new Order( OrderType.MarketSell , new Instrument( "MSFT" ) , 1 ,
							new EndOfDayDateTime(
							new Instrument( "MSFT" ).GetNextMarketDay( extendedDateTime.DateTime ) ,
							EndOfDaySpecificTime.MarketOpen ) ) );
						signals.Add( signal );
						
					}
					else
					{
						signal.Add( new Order( OrderType.MarketBuy , new Instrument( "MSFT" ) , 1 ,
							new EndOfDayDateTime( new Instrument( "MSFT" ).GetNextMarketDay( extendedDateTime.DateTime ) ,
							EndOfDaySpecificTime.MarketOpen ) ) );
						signals.Add( signal );
					}
				}
				else
				//if no signal is given by the history of standard deviation
				//add a special signal to be managed by the account strategy
				{
					signal.Add( new Order( OrderType.MarketBuy , new Instrument( "MSFT" ) , 0 ,
						new EndOfDayDateTime( new Instrument( "MSFT" ).GetNextMarketDay( extendedDateTime.DateTime ) ,
						EndOfDaySpecificTime.MarketOpen ) ) );
					signals.Add( signal );
				}
			}	
			return signals;
		}
    }	
}
