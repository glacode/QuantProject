/*
QuantProject - Quantitative Finance Library

SelectorByCloseToCloseLinearCorrelation.cs
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
  /// Class for selection on tickers by close to close linear correlation
  /// </summary>
   public class SelectorByCloseToCloseLinearCorrelation : TickerSelector , ITickerSelector
  {
        
    
    public SelectorByCloseToCloseLinearCorrelation(DataTable setOfTickersToBeSelected, 
                               bool orderInASCmode,
                               DateTime firstQuoteDate,
                               DateTime lastQuoteDate,
                               long maxNumOfReturnedTickers):
                                    base(setOfTickersToBeSelected, 
                                         orderInASCmode,
                                         firstQuoteDate,
                                         lastQuoteDate,
                                         maxNumOfReturnedTickers)
    {
     
    }
     public SelectorByCloseToCloseLinearCorrelation(string groupID, 
                                bool orderInASCmode,
                                DateTime firstQuoteDate,
                                DateTime lastQuoteDate,
                                long maxNumOfReturnedTickers):
                                  base(groupID, 
                                      orderInASCmode,
                                      firstQuoteDate,
                                      lastQuoteDate,
                                      maxNumOfReturnedTickers)
     {
     
     }


    public DataTable GetTableOfSelectedTickers()
    {
      if(this.setOfTickersToBeSelected == null)
        return QuantProject.Data.DataTables.Quotes.GetTickersByAdjCloseToClosePearsonCorrelationCoefficient(this.isOrderedInASCMode,
                                                    this.groupID,
                                                    this.firstQuoteDate,
                                                    this.lastQuoteDate); 
      else
        return QuantProject.Data.DataTables.Quotes.GetTickersByAdjCloseToClosePearsonCorrelationCoefficient(this.isOrderedInASCMode,
                                                    this.setOfTickersToBeSelected,
                                                    this.firstQuoteDate,
                                                    this.lastQuoteDate);
    }
    
    
    public void SelectAllTickers()
    {
      ;
    }	
   }
}
