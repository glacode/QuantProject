/*
QuantDownloader - Quantitative Finance Library

Tickers_tickerGroups.cs
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
using System.Windows.Forms;
using QuantProject.DataAccess;


namespace QuantProject.DataAccess.Tables
{
	/// <summary>
	/// Class to access the Tickers_tickerGroups table, where tickers are collected
	/// into groups
	/// </summary>
	public class Tickers_tickerGroups
	{

    // these static fields provide field name in the database table
    // They are intended to be used through intellisense when necessary
    public static string GroupID = "ttTgId";
    public static string Ticker = "ttTiId";

    public Tickers_tickerGroups()
    {
      
    }
    /// <summary>
    /// It provides deletion of the single ticker from the specified group
    /// </summary>
    public static void Delete( string tickerToDelete,
                               string fromGroupID)
    {
      try
      {
        SqlExecutor.ExecuteNonQuery("DELETE * FROM tickers_tickerGroups " +
          "WHERE " + Tickers_tickerGroups.Ticker + "='" +
          tickerToDelete + "' AND " + Tickers_tickerGroups.GroupID + "='" +
          fromGroupID + "'");
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }

    /// <summary>
    /// It provides deletion of an entire group of tickers
    /// </summary>
    public static void Delete( string groupToDelete)
    {
      try
      {
        SqlExecutor.ExecuteNonQuery("DELETE * FROM tickers_tickerGroups " +
          "WHERE " + Tickers_tickerGroups.GroupID + "='" +
          groupToDelete + "'");
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }
    
    /// <summary>
    /// It returns a table containing tickers of a given groupID
    /// </summary>
    public static DataTable GetTickers( string groupID)
    {
      /// TO DO use a join in order to return a table with tiTicker and company name  
      return SqlExecutor.GetDataTable("SELECT " + Tickers_tickerGroups.Ticker + " FROM tickers_tickerGroups " +
          "WHERE " + Tickers_tickerGroups.GroupID + "='" +
          groupID + "'");
    }

    /// <summary>
    /// It returns true if some tickers are grouped in the given groupID
    /// </summary>
    public static bool HasTickers( string groupID)
    {
      /// TO DO use a join in order to return a table with tiTicker and company name  
      DataTable tickers = SqlExecutor.GetDataTable("SELECT " + Tickers_tickerGroups.Ticker + " FROM tickers_tickerGroups " +
        "WHERE " + Tickers_tickerGroups.GroupID + "='" +
        groupID + "'");
      return tickers.Rows.Count > 0;
    }

   
  }
}
