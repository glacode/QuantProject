/*
QuantDownloader - Quantitative Finance Library

FaultyTickers.cs
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
using QuantProject.DataAccess;


namespace QuantProject.DataAccess.Tables
{
	/// <summary>
	/// Class to access the Faulty Tickers table (containing tickers not
	/// downloaded because of some error)
	/// </summary>
	public class FaultyTickers
	{

    // these static fields provide field name in the database table
    // They are intended to be used through intellisense when necessary
    public static string Ticker = "ftTicker";
    public static string Date = "ftDate";

    private DataTable faultyTickers;
    private int count;
    
    public FaultyTickers()
    {
      this.faultyTickers = SqlExecutor.GetDataTable("SELECT * FROM faultyTickers");
      this.count = this.faultyTickers.Rows.Count;
    }

    /// <summary>
    /// Number of tickers in FaultyTickers table
    /// </summary>
    public int Count
    {
      get
      {
        return this.count;
      }
    }
    /// <summary>
    /// Returns the actual faultyTickers table
    /// </summary>
    public DataTable Table
    {
      get
      {
        return this.faultyTickers;;
      }
    }
    private static bool isAlreadyStored(string ticker)
    {
      bool returnValue = false;
      DataTable table = SqlExecutor.GetDataTable("SELECT * FROM faultyTickers " + 
                                                  "WHERE ftTicker='"+ ticker +
                                                  "'");
      if(table.Rows.Count>0)
        returnValue = true;

      return returnValue;
    }

    private static void updateDate(string ticker, DateTime newDateForTicker)
    {
      string sql = "UPDATE faultyTickers SET faultyTickers.ftDate =" +
        SQLBuilder.GetDateConstant(newDateForTicker)+
        " WHERE faultyTickers.ftTicker='" + ticker + "'";
      SqlExecutor.ExecuteNonQuery (sql);
    }
    /// <summary>
    /// Adds a new record to FaultyTickers table or,
    /// in case the ticker to be added is already stored, 
    /// updates trial date for the given ticker
    /// </summary>
    public static void AddOrUpdate(string faultyTickerToBeAdded,
                           DateTime errorDateInDownloading)
    {
      try
      {
        if(FaultyTickers.isAlreadyStored(faultyTickerToBeAdded))
          FaultyTickers.updateDate(faultyTickerToBeAdded, errorDateInDownloading);
        else
          SqlExecutor.ExecuteNonQuery("INSERT INTO faultyTickers(ftTicker, ftDate) " +
            "VALUES('" + faultyTickerToBeAdded + 
            "', " + SQLBuilder.GetDateConstant(errorDateInDownloading) + ")");
      }
      catch(Exception ex)
      {
        string notUsed = ex.ToString();
      }
    }
        
  }
}
