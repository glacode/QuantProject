/*
QuantProject - Quantitative Finance Library

ConnectionProvider.cs
Copyright (C) 2003 
Glauco Siliprandi

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
using System.Data.OleDb;
using QuantProject.DataAccess;

namespace QuantProject.DataAccess
{
	/// <summary>
	/// Provides database connections
	/// </summary>
	public class ConnectionProvider
	{
		public ConnectionProvider()
		{
			//
			// TODO: Add constructor logic here
			//
		}
    private static OleDbConnection oleDbConnection;

    /// <summary>
    /// Returns an OleDbConnection to the database
    /// </summary>
    public static OleDbConnection OleDbConnection
    {
      get
      {
        if ( oleDbConnection == null )
        {
          DataBaseLocator dataBaseLocator = new DataBaseLocator("MDB"); 
          string mdbPath = dataBaseLocator.Path;
          string connectionString =
            @"Provider=Microsoft.Jet.OLEDB.4.0;Password="""";User ID=Admin;Data Source=" +
            mdbPath +
            @";Jet OLEDB:Registry Path="""";Jet OLEDB:Database Password="""";Jet OLEDB:Engine Type=5;Jet OLEDB:Database Locking Mode=1;Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Global Bulk Transactions=1;Jet OLEDB:New Database Password="""";Jet OLEDB:Create System Database=False;Jet OLEDB:Encrypt Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;Jet OLEDB:SFP=False";
          oleDbConnection = new OleDbConnection( connectionString );
        }
        DataBaseVersionManager dataBaseVersionManager = new DataBaseVersionManager(oleDbConnection);
		dataBaseVersionManager.UpdateDataBaseStructure();
		return oleDbConnection;
      }
    }
	}
}
