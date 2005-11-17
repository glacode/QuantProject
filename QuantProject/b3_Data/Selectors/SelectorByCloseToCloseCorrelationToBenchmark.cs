/*
QuantProject - Quantitative Finance Library

SelectorByCloseToCloseCorrelationToBenchmark.cs
Copyright (C) 2005 
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
  /// Class for selection on tickers by close to close absolute correlation to 
  /// a given benchmark
  /// </summary>
   public class SelectorByCloseToCloseCorrelationToBenchmark : TickerSelector, ITickerSelector
  {
    private string benchmark;
    private int numDaysBetweenEachClose;
    
    public SelectorByCloseToCloseCorrelationToBenchmark(DataTable setOfTickersToBeSelected,
                               string benchmark,
                               bool orderInASCmode,
                               DateTime firstQuoteDate,
                               DateTime lastQuoteDate,
                               long maxNumOfReturnedTickers,
                               int numDaysBetweenEachClose):
                                    base(setOfTickersToBeSelected, 
                                         orderInASCmode,
                                         firstQuoteDate,
                                         lastQuoteDate,
                                         maxNumOfReturnedTickers)
    {
      this.benchmark = benchmark;
      this.numDaysBetweenEachClose = numDaysBetweenEachClose;
    }
     public SelectorByCloseToCloseCorrelationToBenchmark(string groupID, 
                                string benchmark,
                                bool orderInASCmode,
                                DateTime firstQuoteDate,
                                DateTime lastQuoteDate,
                                long maxNumOfReturnedTickers,
                               	int numDaysBetweenEachClose):
                                  base(groupID, 
                                      orderInASCmode,
                                      firstQuoteDate,
                                      lastQuoteDate,
                                      maxNumOfReturnedTickers)
     {
        this.benchmark = benchmark;
        this.numDaysBetweenEachClose = numDaysBetweenEachClose;
     }


    public DataTable GetTableOfSelectedTickers()
    {
      if(this.setOfTickersToBeSelected == null)
        return QuantProject.Data.DataTables.Quotes.GetTickersByCloseToCloseCorrelationToBenchmark(this.isOrderedInASCMode,
                                    this.groupID,this.benchmark,
                                    this.firstQuoteDate, this.lastQuoteDate,
                                    this.maxNumOfReturnedTickers, this.numDaysBetweenEachClose);        

      else
        return QuantProject.Data.DataTables.Quotes.GetTickersByCloseToCloseCorrelationToBenchmark(this.isOrderedInASCMode,
          this.setOfTickersToBeSelected,this.benchmark,
          this.firstQuoteDate, this.lastQuoteDate,
          this.maxNumOfReturnedTickers, this.numDaysBetweenEachClose);      
    }
    public void SelectAllTickers()
    {
      ;
    }	
	}
}
