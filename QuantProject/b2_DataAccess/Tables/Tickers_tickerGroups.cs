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
    public static string EventTypeFieldName = "ttEventType";
    public static string EventDate = "ttEventDate";

    public Tickers_tickerGroups()
    {
      
    }
    
    /// <summary>
    /// It fills the given dataTable with tickers belonging to a given group
    /// </summary>
    /// <param name="groupID">The groupID corresponding to the tickers whose
    /// values have to be stored in the given Data Table</param>
    /// <param name="dataTable">The dataTable where to store tickers</param>
    /// <returns></returns>
    public static void SetDataTable( string groupID , DataTable dataTable)
    {
      string sql;
      sql = "SELECT " + Tickers_tickerGroups.Ticker + " FROM tickers_tickerGroups " +
            "WHERE " + Tickers_tickerGroups.GroupID + "='" +
            groupID + "'";
      SqlExecutor.SetDataTable( sql , dataTable );
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
    
    private static string insert_getEventTypeCode(EventType eventType)
    {
      string returnValue = "I";//default value for Exit eventType
      switch (eventType)
      {
        case EventType.Exit:
          returnValue = "O";
          break;
      }
      return returnValue;
      
    }
    
    /// <summary>
    /// Adds a new row into tickers_tickerGroups 
    /// </summary>
    public static void Add( string ticker, string groupId, EventType eventType,
                                DateTime eventDate)
    {
      string eventTypeCode = insert_getEventTypeCode(eventType);
      SqlExecutor.ExecuteNonQuery("INSERT INTO tickers_tickerGroups(ttTiId, ttTgId, ttEventType, ttEventDate) " +
        "VALUES('" + ticker + "','" + groupId + "','" + eventTypeCode +
        "'," + SQLBuilder.GetDateConstant(eventDate)+ ")");
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
      return SqlExecutor.GetDataTable("SELECT DISTINCT " + Tickers_tickerGroups.Ticker + " FROM tickers_tickerGroups " +
          "WHERE " + Tickers_tickerGroups.GroupID + "='" +
          groupID + "'");
    }
    
    private static void getTickers_createView(string viewName,
                                              string sqlStatement)
    {
      try
      {	
      	string sqlForViewCreation = "CREATE VIEW " + viewName + " AS " + sqlStatement;
      	SqlExecutor.ExecuteNonQuery(sqlForViewCreation);
      }
      catch(Exception ex)
      {
      	ex = ex;
      }
    }
    
    /// <summary>
    /// It returns a table containing tickers effectively contained
    /// in the given group at the given Date
    /// </summary>
    public static DataTable GetTickers( string groupID, DateTime date)
    {
      
      string sqlLastEntries = "SELECT tickers_tickerGroups.ttTgId AS GroupIDEntries, " +
                              "tickers_tickerGroups.ttTiId AS TickerIDEntries, Max(tickers_tickerGroups.ttEventDate) " +
                               "AS MaxEntryDate FROM tickers_tickerGroups WHERE " +
                              "tickers_tickerGroups.ttEventType='I' " +
                              "GROUP BY tickers_tickerGroups.ttTgId, tickers_tickerGroups.ttTiId";
      getTickers_createView("Entries", sqlLastEntries);
      
      string sqlLastExits = "SELECT tickers_tickerGroups.ttTgId AS GroupIDExits, " +
                            "tickers_tickerGroups.ttTiId AS TickerIDExits, Max(tickers_tickerGroups.ttEventDate) " +
                            "AS MaxExitDate FROM tickers_tickerGroups WHERE " +
                            "tickers_tickerGroups.ttEventType='O' " +
                            "GROUP BY tickers_tickerGroups.ttTgId, tickers_tickerGroups.ttTiId";
      
      getTickers_createView("Exits", sqlLastExits);
      
      string sqlTickersAtTheGivenDate = "SELECT GroupIDEntries, " +
      									"TickerIDEntries FROM Entries LEFT JOIN Exits " + 
      									"ON (Entries.GroupIDEntries = Exits.GroupIDExits) AND " +
      									"(Entries.TickerIDEntries = Exits.TickerIDExits) WHERE " +
      									"GroupIDEntries ='" + groupID + "' AND " +
      									"MaxEntryDate<=" + SQLBuilder.GetDateConstant(date) + " AND " +
      									"(MaxExitDate Is Null OR MaxEntryDate>Exits.MaxExitDate)";
     
      return SqlExecutor.GetDataTable(sqlTickersAtTheGivenDate);
    }
    /*
    /// <summary>
    /// It returns a table containing all the tickers 
    /// gained through a recursion through all the groups contained in the given groupID
    /// </summary>
    public static DataTable GetTickersWithRecursionInsideGroup( string groupID)
    {
      /// TO DO use a join in order to return a table with tiTicker and company name  
      
    }
    */
    
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
