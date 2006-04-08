/*
QuantProject - Quantitative Finance Library

SelectorByQuotationNotAtEachMarketDay.cs
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
using System.Data;
using System.Windows.Forms;
using QuantProject.DataAccess.Tables;
using QuantProject.Data.DataTables;

namespace QuantProject.Data.Selectors
{
  /// <summary>
  /// Class for selection on tickers NOT quoted at each market day (market index)
  /// </summary>
   public class SelectorByQuotationNotAtEachMarketDay : TickerSelector , ITickerSelector
  {
    private string marketIndex;    
    
    public SelectorByQuotationNotAtEachMarketDay(DataTable setOfTickersToBeSelected, 
                               bool orderInASCmode,
                               DateTime firstQuoteDate,
                               DateTime lastQuoteDate,
                               long maxNumOfReturnedTickers, string marketIndex):
                                    base(setOfTickersToBeSelected, 
                                         orderInASCmode,
                                         firstQuoteDate,
                                         lastQuoteDate,
                                         maxNumOfReturnedTickers)
    {
      this.marketIndex = marketIndex;
    }
     public SelectorByQuotationNotAtEachMarketDay(string groupID, 
                                bool orderInASCmode,
                                DateTime firstQuoteDate,
                                DateTime lastQuoteDate,
                                long maxNumOfReturnedTickers, string marketIndex):
                                  base(groupID, 
                                      orderInASCmode,
                                      firstQuoteDate,
                                      lastQuoteDate,
                                      maxNumOfReturnedTickers)
     {
      this.marketIndex = marketIndex;
     }


    public DataTable GetTableOfSelectedTickers()
    {
      if(this.marketIndex == "")
        throw new Exception("You first need to set TickerSelector's property <<MarketIndex>>!");
           
      if(this.setOfTickersToBeSelected == null)
        return QuantProject.Data.DataTables.TickerDataTable.GetTickersNotQuotedAtEachMarketDay(
          this.marketIndex, this.groupID, this.firstQuoteDate, this.lastQuoteDate,
          this.maxNumOfReturnedTickers);        

      else
        return QuantProject.Data.DataTables.TickerDataTable.GetTickersNotQuotedAtEachMarketDay(
          this.marketIndex, this.setOfTickersToBeSelected, this.firstQuoteDate, this.lastQuoteDate,
          this.maxNumOfReturnedTickers);
    }
    public void SelectAllTickers()
    {
      ;
    }	
	}
}
