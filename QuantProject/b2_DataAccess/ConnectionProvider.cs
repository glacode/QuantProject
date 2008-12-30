/*
QuantProject - Quantitative Finance Library

ConnectionProvider.cs
Copyright (C) 2008
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
using System.Data.Common;
using System.Data.OleDb;

using MySql.Data.MySqlClient;

namespace QuantProject.DataAccess
{
	/// <summary>
	/// Provides database connections
	/// </summary>
	public class ConnectionProvider
	{
		private static DbConnection dbConnection;

		#region DbConnection
		#region getDbConnection
		
		#region getAccessDbConnection
		
		#region getAccessConnectionString
		private static string getMdbPath()
		{
			string mdbPath = ConfigManager.MdbPath;
			return mdbPath;
		}
		private static string getAccessConnectionString()
		{
			string mdbPath = ConnectionProvider.getMdbPath();
			string connectionString =
				@"Provider=Microsoft.Jet.OLEDB.4.0;Password="""";User ID=Admin;Data Source=" +
				mdbPath +
				@";Jet OLEDB:Registry Path="""";Jet OLEDB:Database Password="""";Jet OLEDB:Engine Type=5;Jet OLEDB:Database Locking Mode=1;Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Global Bulk Transactions=1;Jet OLEDB:New Database Password="""";Jet OLEDB:Create System Database=False;Jet OLEDB:Encrypt Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;Jet OLEDB:SFP=False";
			return connectionString;
		}
		#endregion getAccessConnectionString
		
		private static DbConnection getAccessDbConnection()
		{
			string connectionString = ConnectionProvider.getAccessConnectionString();
			DbConnection dbConnection = new OleDbConnection( connectionString );
			try
			{
				dbConnection.Open();
			}
			catch(Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.ToString());
				dbConnection = null;
			}
			return dbConnection;
		}
		#endregion getAccessDbConnection
		
		#region getMySQLDbConnection
		private static DbConnection getMySQLDbConnection()
		{
			DbConnection mySQLDbConnection =
				new MySqlConnection( ConfigManager.MySqlConnectionString );
			return mySQLDbConnection;
		}
		#endregion getMySQLDbConnection
		
		private static DbConnection getDbConnection()
		{
			DbConnection dbConnection = null;
			if ( ConfigManager.IsChosenDbTypeAccess )
				dbConnection = ConnectionProvider.getAccessDbConnection();
			else
				// current db type is MySQL
				dbConnection = ConnectionProvider.getMySQLDbConnection();
			return dbConnection;
		}
		#endregion getDbConnection
		private static void updateDatabaseStructure()
		{
			DataBaseVersionManager dataBaseVersionManager = new DataBaseVersionManager(dbConnection);
			dataBaseVersionManager.UpdateDataBaseStructure();
		}
		/// <summary>
		/// Returns an OleDbConnection to the database
		/// </summary>
		public static DbConnection DbConnection
		{
			get
			{
				if ( dbConnection == null )
				{
					ConnectionProvider.dbConnection =
						ConnectionProvider.getDbConnection();
					ConnectionProvider.updateDatabaseStructure();
				}
				return dbConnection;
			}
		}
		#endregion DbConnection
		
		public static DbType DbType
		{
			get
			{
				DbType dbType = DbType.Undefined;
				if ( ConnectionProvider.DbConnection is OleDbConnection )
					dbType = DbType.Access;
				if ( ConnectionProvider.DbConnection is MySqlConnection )
					dbType = DbType.MySql;
				return dbType;
			}
		}
		public ConnectionProvider()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
