/*
QuantProject - Quantitative Finance Library

DataStreamerHandler.cs
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
using System.Data;

using QuantProject.Business.Financial.Accounting;
using QuantProject.Data.DataProviders;
using QuantProject.ADT;
using QuantProject.Data.Selectors;

namespace QuantProject.Scripts.TickerSelectionTesting.BestTwoIndipendent
{
	/// <summary>
	/// Implements NewQuoteEventHandler. This is the core strategy!
	/// </summary>
	public class DataStreamerHandler
	{
		private Account account;
    private ExtendedDateTime openAccountDate;

		public Account Account
		{
			get { return this.account; }
		}
		public DataStreamerHandler()
		{
			this.account = new Account( "Main" );
      this.openAccountDate = new ExtendedDateTime(new DateTime(2002,1,1), BarComponent.Open);
      this.account.AddCash(this.openAccountDate, 10000);
		}
    
    private DataTable newQuoteEventHandler_getTickersToBuy(Quote encomingQuote)
    {
      TickerSelector liquidSelector = new TickerSelector(SelectionType.Liquidity, false,
                                      "NSDQ100", this.openAccountDate.DateTime,
                                      encomingQuote.ExtendedDataTable.DateTime,50);
      TickerSelector volatilityCTOselector = 
        new TickerSelector(liquidSelector.GetTableOfSelectedTickers(),
        SelectionType.CloseToOpenVolatility, true, "",
        this.openAccountDate.DateTime, encomingQuote.ExtendedDataTable.DateTime, 40);
      TickerSelector avgCTOperformanceSelector = 
        new TickerSelector(volatilityCTOselector.GetTableOfSelectedTickers(),
        SelectionType.AverageCloseToOpenPerformance, true, "",
        this.openAccountDate.DateTime, encomingQuote.ExtendedDataTable.DateTime, 20);
      TickerSelector correlationSelector = 
        new TickerSelector(avgCTOperformanceSelector.GetTableOfSelectedTickers(),
        SelectionType.CloseToOpenLinearCorrelation, true, "",
        this.openAccountDate.DateTime, encomingQuote.ExtendedDataTable.DateTime, 2);
      return correlationSelector.GetTableOfSelectedTickers();
    }

    public void NewQuoteEventHandler(
			Object sender , NewQuoteEventArgs eventArgs )
		{
      Quote encomingQuote = eventArgs.Quote;
      
      if(this.openAccountDate.DateTime.AddDays(30).CompareTo(encomingQuote.ExtendedDataTable.DateTime)<0)
      //it has elasped enough time for a significant simulation of the strategy
      {
          DataTable tickersToBuy = this.newQuoteEventHandler_getTickersToBuy(encomingQuote);
          //the script has to be completed ...

      }

		}
	}
}
