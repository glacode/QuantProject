/*
QuantDownloader - Quantitative Finance Library

TickerDataTable.cs
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
using System.Data;
using QuantProject.ADT;
using QuantProject.DataAccess.Tables;

namespace QuantProject.Data.DataTables
{
	/// <summary>
	/// The DataTable where to store tickers.
	/// </summary>
	
  public class TickerDataTable : DataTable
	{
    private static DataTable clipboard;
    public static DataTable Clipboard
    {
      get{ return TickerDataTable.clipboard; }
      set{ TickerDataTable.clipboard = value; }
    }

    public TickerDataTable()
    {
      
    }
    
    public static void AddColumnsOfTickerTable(DataTable table)
    {
     try
      {
      table.Columns.Add("tiTicker", System.Type.GetType("System.String"));
      table.Columns.Add("tiCompanyName", System.Type.GetType("System.String"));
			}
			catch(Exception ex)
			{
				string notUsed = ex.ToString();
			}
    }
    
    public static DataTable GetBestPerformingTickers(string groupID,
                                                      DateTime firstQuoteDate,
                                                      DateTime lastQuoteDate,
                                                      long maxNumOfReturnedTickers)
    {
      DataTable groupOfTicker = Tickers_tickerGroups.GetTickers(groupID);
      //TO DO change to a structure compatible with TickerDataTable
      groupOfTicker.Columns.Add("SimpleReturn", System.Type.GetType("System.Double"));
      try
      {
        double firstQuote, lastQuote;
        foreach(DataRow row in groupOfTicker.Rows)
        {
          firstQuote = QuantProject.DataAccess.Tables.Quotes.GetAdjustedClose((string)row[0],
                                                                              firstQuoteDate);
          lastQuote = QuantProject.DataAccess.Tables.Quotes.GetAdjustedClose((string)row[0],
                                                                              lastQuoteDate);
          row["SimpleReturn"] = (lastQuote - firstQuote) / firstQuote;
        }

      }
      catch(Exception ex)
      {
        System.Windows.Forms.MessageBox.Show(ex.ToString());
      }
      ExtendedDataTable.Sort(groupOfTicker, "SimpleReturn");
      ExtendedDataTable.DeleteRows(groupOfTicker, maxNumOfReturnedTickers);
      return groupOfTicker;              
      
    }



  }
}
