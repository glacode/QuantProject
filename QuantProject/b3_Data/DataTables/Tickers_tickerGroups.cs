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
using QuantProject.ADT;
using QuantProject.DataAccess.Tables;

namespace QuantProject.Data.DataTables
{
	/// <summary>
	/// The DataTable to work with tickers inside a given group.
	/// </summary>
	
  public class Tickers_tickerGroups : DataTable
	{
    // these static fields provide field name in the database table
    // They are intended to be used through intellisense when necessary
    public static string GroupID = "ttTgId";
    public static string Ticker = "ttTiId";
        
    public Tickers_tickerGroups(string groupID)
    {
      this.fillDataTable(groupID);
    }
    
    private void fillDataTable( string groupID )
    {
      QuantProject.DataAccess.Tables.Tickers_tickerGroups.SetDataTable(groupID, this);

    }
       
  }
}
