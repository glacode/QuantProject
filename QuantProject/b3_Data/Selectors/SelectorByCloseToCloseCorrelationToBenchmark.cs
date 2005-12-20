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
  /// NOTE that
  /// close values are grouped in pairs and the first close value in each group is 
  /// not the last close in the previous group. There is, in other words, a discontinuity
  /// between each group, with length equal to the group's length
  /// </summary>
   public class SelectorByCloseToCloseCorrelationToBenchmark : TickerSelector, ITickerSelector
  {
    private string benchmark;
    private int numDaysBetweenEachClose;
    /// <summary>
    /// Creates a new instance of the selector
    /// </summary>
    /// <param name="setOfTickersToBeSelected">The data table containing in the first column the tickers that have to be ordered</param>
    /// <param name="benchmark">Benchmark code</param>
    /// <param name="orderInASCmode">Ordering mode</param>
    /// <param name="firstQuoteDate">The first date for the interval</param>
    /// <param name="lastQuoteDate">The last date for the interval</param>
    /// <param name="maxNumOfReturnedTickers">Max number of tickers to be returned</param>
    /// <param name="numDaysBetweenEachClose">Number of days between closes to be studied. NOTE that
    /// close values are grouped in pairs and the first close value in each group is 
    /// not the last close in the previous group. There is, in other words, a discontinuity
    /// between each group, with length equal to the group's length </param>
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
     
     /// <summary>
     /// Creates a new instance of the selector
     /// </summary>
     /// <param name="groupID">The group ID containing the tickers that have to be ordered</param>
     /// <param name="benchmark">Benchmark code</param>
     /// <param name="orderInASCmode">Ordering mode</param>
     /// <param name="firstQuoteDate">The first date for the interval</param>
     /// <param name="lastQuoteDate">The last date for the interval</param>
     /// <param name="maxNumOfReturnedTickers">Max number of tickers to be returned</param>
     /// <param name="numDaysBetweenEachClose">Number of days between closes to be studied. NOTE that
     /// close values are grouped in pairs and the first close value in each group is 
     /// not the last close in the previous group. There is, in other words, a discontinuity
     /// between each group, with length equal to the group's length </param>
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
