/*
QuantDownloader - Quantitative Finance Library

TickerGroups.cs
Copyright (C) 2004 
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
	/// Class to access the TickerGroups table
	/// </summary>
	public class TickerGroups
	{

    // these static fields provide field name in the database table
    // They are intended to be used through intellisense when necessary
    public static string GroupID = "tgId";
    public static string GroupDescription = "tgDescription";
    public static string ParentGroupID = "tgTgId";

    private DataTable tickerGroups;
    private int count;
    
    public TickerGroups()
    {
      this.tickerGroups = SqlExecutor.GetDataTable("SELECT * FROM tickerGroups");
      this.count = this.tickerGroups.Rows.Count;
    }

    /// <summary>
    /// Number of tickerGroups in tickers table
    /// </summary>
    public int Count
    {
      get
      {
        return this.count;
      }
    }
    
    public static void DeleteGroup (string groupID)
    {
      string sqlDeleteCommand = "DELETE * FROM tickerGroups WHERE " +
                                TickerGroups.GroupID + "='" + groupID + "'";
			SqlExecutor.ExecuteNonQuery(sqlDeleteCommand);
    }
    
   
  }
}
