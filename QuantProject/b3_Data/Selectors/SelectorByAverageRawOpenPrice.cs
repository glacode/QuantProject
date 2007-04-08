/*
QuantProject - Quantitative Finance Library

SelectorByAverageRawOpenPrice.cs
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
  /// Class for selection of tickers for which average raw open price
  /// and raw open price's standard deviation 
  /// belong to a specified ranges, in a given time interval.
  /// </summary>
   public class SelectorByAverageRawOpenPrice : TickerSelector, ITickerSelector 
  {
        
    private double minPrice;
    private double maxPrice;
    private double minStdDeviation;
    private double maxStdDeviation; 

    public SelectorByAverageRawOpenPrice(DataTable setOfTickersToBeSelected, 
                               bool orderInASCmode,
                               DateTime firstQuoteDate,
                               DateTime lastQuoteDate,
                               long maxNumOfReturnedTickers, double minPrice,
                                double maxPrice, double minStdDeviation,
                                double maxStdDeviation):
                                    base(setOfTickersToBeSelected, 
                                         orderInASCmode,
                                         firstQuoteDate,
                                         lastQuoteDate,
                                         maxNumOfReturnedTickers)
    {
      this.minPrice = minPrice;
      this.maxPrice = maxPrice;
      this.minStdDeviation = minStdDeviation;
      this.maxStdDeviation = maxStdDeviation;
    }
    
    public SelectorByAverageRawOpenPrice(DataTable setOfTickersToBeSelected, 
                                        bool orderInASCmode,
                                        DateTime firstQuoteDate,
                                        DateTime lastQuoteDate,
                                        long maxNumOfReturnedTickers, double minPrice):
                                        base(setOfTickersToBeSelected, 
                                        orderInASCmode,
                                        firstQuoteDate,
                                        lastQuoteDate,
                                        maxNumOfReturnedTickers)
    {
      this.minPrice = minPrice;
      this.maxPrice = 0.0;
      this.minStdDeviation = 0.0;
      this.maxStdDeviation = 0.0;
    }

    public SelectorByAverageRawOpenPrice(string groupID, 
                              bool orderInASCmode,
                              DateTime firstQuoteDate,
                              DateTime lastQuoteDate,
                              long maxNumOfReturnedTickers, double minPrice,
                              double maxPrice, double minStdDeviation,
                              double maxStdDeviation):
                                base(groupID, 
                                    orderInASCmode,
                                    firstQuoteDate,
                                    lastQuoteDate,
                                    maxNumOfReturnedTickers)
    {
      this.minPrice = minPrice;
      this.maxPrice = maxPrice;
      this.minStdDeviation = minStdDeviation;
      this.maxStdDeviation = maxStdDeviation;
    }

     public SelectorByAverageRawOpenPrice(string groupID, 
                            bool orderInASCmode,
                            DateTime firstQuoteDate,
                            DateTime lastQuoteDate,
                            long maxNumOfReturnedTickers, double minPrice):
                            base(groupID, 
                            orderInASCmode,
                            firstQuoteDate,
                            lastQuoteDate,
                            maxNumOfReturnedTickers)
     {
       this.minPrice = minPrice;
       this.maxPrice = 0.0;
       this.minStdDeviation = 0.0;
       this.maxStdDeviation = 0.0;
     }

    public DataTable GetTableOfSelectedTickers()
    {
      DataTable returnValue;
      if(this.maxPrice == 0.0 && this.minStdDeviation == 0.0 && this.maxStdDeviation == 0.0)
      //selection only by average raw open price over a given minimum level
      {
        if(this.setOfTickersToBeSelected == null)
          returnValue = QuantProject.DataAccess.Tables.Quotes.GetTickersByRawOpenPrice(this.isOrderedInASCMode,
            this.groupID, this.firstQuoteDate, this.lastQuoteDate, this.maxNumOfReturnedTickers,
            this.minPrice);        
        else
          returnValue = QuantProject.Data.DataTables.Quotes.GetTickersByAverageRawOpenPrice(this.isOrderedInASCMode,
            this.setOfTickersToBeSelected, this.firstQuoteDate, this.lastQuoteDate, this.maxNumOfReturnedTickers,
            this.minPrice);
      }
      else//selection is performed considering maxAveragePrice and min / max std Deviation
      {
        if(this.setOfTickersToBeSelected == null)
          returnValue = QuantProject.DataAccess.Tables.Quotes.GetTickersByRawOpenPrice(this.isOrderedInASCMode,
            this.groupID, this.firstQuoteDate, this.lastQuoteDate, this.maxNumOfReturnedTickers,
            this.minPrice, this.maxPrice, this.minStdDeviation, this.maxStdDeviation);        
        else
          returnValue = QuantProject.Data.DataTables.Quotes.GetTickersByAverageRawOpenPrice(this.isOrderedInASCMode,
            this.setOfTickersToBeSelected, this.firstQuoteDate, this.lastQuoteDate, this.maxNumOfReturnedTickers,
            this.minPrice, this.maxPrice, this.minStdDeviation, this.maxStdDeviation);
      }
      return returnValue;
    }
    public void SelectAllTickers()
    {
      ;
    }	
	}
}
