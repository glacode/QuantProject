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
using QuantProject.DataAccess.Tables;

namespace QuantProject.Data.DataTables
{
	/// <summary>
	/// The DataTable where to store tickers.
	/// It has the same structure of DB's table
	/// </summary>
	
  public class TickerDataTable : DataTable
	{
    private static TickerDataTable clipboard;
    public static TickerDataTable Clipboard
    {
      get{ return TickerDataTable.clipboard; }
      set{ TickerDataTable.clipboard = value; }
    }



    public TickerDataTable()
    {
      this.setStructure();
    }
    
    private void setStructure()
    {
      DataColumn ticker = new DataColumn(Tickers.Ticker, System.Type.GetType("System.String"));
      ticker.Unique = true;
      ticker.AllowDBNull = false;
      DataColumn companyName = new DataColumn(Tickers.CompanyName, System.Type.GetType("System.String"));
      this.Columns.Add(ticker);
      this.Columns.Add(companyName);
    }

    public static TickerDataTable ConvertToTickerDataTable(DataTable dataTableToConvert)
    {
      TickerDataTable tickerDataTable = new TickerDataTable();
      DataRow rowToAdd;
      try
      {
        foreach(DataRow row in dataTableToConvert.Rows)
        {
          rowToAdd = tickerDataTable.NewRow();
          rowToAdd[Tickers.Ticker] = row[Tickers.Ticker];
          rowToAdd[Tickers.CompanyName] = row[Tickers.CompanyName];
          tickerDataTable.Rows.Add(rowToAdd);
        }
        
      }
      catch(Exception ex)
      {
        string notUsed = ex.ToString();
      }
      return tickerDataTable;
      
    }

  }
}
