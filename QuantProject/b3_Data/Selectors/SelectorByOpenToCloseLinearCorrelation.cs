/*
QuantProject - Quantitative Finance Library

SelectorByOpenToCloseLinearCorrelation.cs
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
  /// Class for selection on tickers by open to close linear correlation
  /// </summary>
   public class SelectorByOpenToCloseLinearCorrelation : TickerSelector, ITickerSelector 
  {
        
    
    public SelectorByOpenToCloseLinearCorrelation(DataTable setOfTickersToBeSelected, 
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
     public SelectorByOpenToCloseLinearCorrelation(string groupID, 
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
      this.launchExceptionIfGroupIDIsNotEmpty();
      return QuantProject.Data.DataTables.Quotes.GetTickersByCloseToOpenPearsonCorrelationCoefficient(this.isOrderedInASCMode,
        this.setOfTickersToBeSelected,
        this.firstQuoteDate,
        this.lastQuoteDate);
    }
    
     private void launchExceptionIfGroupIDIsNotEmpty()
    {
      if(this.groupID!="")
      {
        throw new Exception("Not implemented: this type of selection works only with few tickers, at the moment");
      }
    }
    public void SelectAllTickers()
    {
      ;
    }	
}
}
