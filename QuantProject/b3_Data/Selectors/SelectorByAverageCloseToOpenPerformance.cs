/*
QuantProject - Quantitative Finance Library

SelectorByAverageCloseToOpenPerformance.cs
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
  /// Class for selection on tickers by average close (at day x) 
  /// to open (at day x+1) performance
  /// </summary>
  public class SelectorByAverageCloseToOpenPerformance : TickerSelector, ITickerSelector 
  {
        
    
    public SelectorByAverageCloseToOpenPerformance(DataTable setOfTickersToBeSelected, 
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
    public SelectorByAverageCloseToOpenPerformance(string groupID, 
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
    private double getTableOfSelectedTickers_getTickersFromTable_getAverageCTOForTicker(string ticker)
    {
      Data.DataTables.Quotes tickerQuotes = new Data.DataTables.Quotes(ticker, this.firstQuoteDate, this.lastQuoteDate);
      float[] closeToOpenReturns = new float[tickerQuotes.Rows.Count - 1];
      for(int i = 0;i<tickerQuotes.Rows.Count - 1; i++)
      {
        closeToOpenReturns[i] = 
          ( (float)tickerQuotes.Rows[i+1]["quOpen"]*
          (float)tickerQuotes.Rows[i+1]["quAdjustedClose"]/
          (float)tickerQuotes.Rows[i+1]["quClose"] )
          /(float)tickerQuotes.Rows[i]["quAdjustedClose"] - 1;
      }
      return QuantProject.ADT.Statistics.BasicFunctions.SimpleAverage(closeToOpenReturns);
    }
    private DataTable getTableOfSelectedTickers_getTickersFromTable()
    {
      if(!this.setOfTickersToBeSelected.Columns.Contains("AverageCloseToOpenPerformance"))
        this.setOfTickersToBeSelected.Columns.Add("AverageCloseToOpenPerformance", System.Type.GetType("System.Double"));
      foreach(DataRow row in this.setOfTickersToBeSelected.Rows)
      {
        try
        {
          row["AverageCloseToOpenPerformance"] = -1000000.0;
          row["AverageCloseToOpenPerformance"] =
          	this.getTableOfSelectedTickers_getTickersFromTable_getAverageCTOForTicker((string)row[0]);
        }
        catch(Exception ex)
        {
        	string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
        }
      }
      DataTable tableToReturn =
        ExtendedDataTable.CopyAndSort(this.setOfTickersToBeSelected,
                                      "AverageCloseToOpenPerformance>-1000000.0",
                                      "AverageCloseToOpenPerformance",
                                      this.isOrderedInASCMode);
      ExtendedDataTable.DeleteRows(tableToReturn, this.maxNumOfReturnedTickers);
      return tableToReturn;
    }
     
    private DataTable getTableOfSelectedTickers_getTickersFromGroup()
    {
      this.setOfTickersToBeSelected = 
        QuantProject.DataAccess.Tables.Tickers_tickerGroups.GetTickers(this.groupID);
      return this.getTableOfSelectedTickers_getTickersFromTable();
    }


    public DataTable GetTableOfSelectedTickers()
    {
      if(this.setOfTickersToBeSelected == null)
        return this.getTableOfSelectedTickers_getTickersFromGroup();
      else//setOfTickersToBeSelected != null
        return this.getTableOfSelectedTickers_getTickersFromTable();
    }
    public void SelectAllTickers()
    {
      ;
    }		
   }
}
