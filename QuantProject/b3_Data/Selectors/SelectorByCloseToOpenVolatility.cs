/*
QuantProject - Quantitative Finance Library

SelectorByCloseToOpenVolatility.cs
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
  /// Class for selection on tickers by Close to open volatility
  /// </summary>
   public class SelectorByCloseToOpenVolatility : TickerSelector, ITickerSelector 
  {
        
    
    public SelectorByCloseToOpenVolatility(DataTable setOfTickersToBeSelected, 
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
     public SelectorByCloseToOpenVolatility(string groupID, 
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


     private double getTableOfSelectedTickers_getTickersFromTable_getCTOStdDevForTicker(string ticker)
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
       return QuantProject.ADT.Statistics.BasicFunctions.StdDev(closeToOpenReturns);
     }
     private DataTable getTableOfSelectedTickers_getTickersFromTable()
     {
       if(!this.setOfTickersToBeSelected.Columns.Contains("CloseToOpenStandardDeviation"))
         this.setOfTickersToBeSelected.Columns.Add("CloseToOpenStandardDeviation", System.Type.GetType("System.Double"));
       double CTOStdDev;
       foreach(DataRow row in this.setOfTickersToBeSelected.Rows)
       {
       	try
       	{
       		row["CloseToOpenStandardDeviation"] = -1000000.0;
       		CTOStdDev = this.getTableOfSelectedTickers_getTickersFromTable_getCTOStdDevForTicker((string)row[0]);
       		if( !Double.IsInfinity(CTOStdDev) && !Double.IsNaN(CTOStdDev) )
       			row["CloseToOpenStandardDeviation"] = CTOStdDev;
       		
       	}
       	catch(Exception ex)
       	{
       		string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
       	}
       }
       DataTable tableToReturn =
       	ExtendedDataTable.CopyAndSort(this.setOfTickersToBeSelected,
       	                              "CloseToOpenStandardDeviation>-1000000.0",
       	                              "CloseToOpenStandardDeviation",
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
