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

namespace QuantProject.DataAccess.Tables
{
	/// <summary>
	/// The DataTable where to store tickers.
	/// It has the same structure of DB's table and it contains static string fields
	/// providing the name of the corresponding table's field
	/// NOTE: Static fields are intended to be used with intellisense
	/// </summary>
	
  public class TickerDataTable : DataTable
	{
    // these static fields provide field name in the database table
    // They are intended to be used through intellisense when necessary
    
    static string tiTicker = "tiTicker";
    static string tiCompanyName = "tiCompanyName";
    
    public TickerDataTable()
    {
      this.setStructure();
    }
    
    private void setStructure()
    {
      DataColumn tiTicker = new DataColumn(TickerDataTable.tiTicker, System.Type.GetType("System.String"));
      tiTicker.Unique = true;
      tiTicker.AllowDBNull = false;
      DataColumn tiCompanyName = new DataColumn(TickerDataTable.tiCompanyName, System.Type.GetType("System.String"));
      this.Columns.Add(tiTicker);
      this.Columns.Add(tiCompanyName);
    }

  }
}
