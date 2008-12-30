/*
QuantProject - Quantitative Finance Library

DbDataAdapterProvider.cs
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
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

using MySql.Data.MySqlClient;

namespace QuantProject.DataAccess
{
	/// <summary>
	/// Provides a DbDataAdapter for the given Sql command
	/// </summary>
	public class DbDataAdapterProvider
	{
		public DbDataAdapterProvider()
		{
		}
		
		#region GetDbDataAdapter
		private static void ensureConnectionIsOpen()
		{
			if( ConnectionProvider.DbConnection.State != ConnectionState.Open )
				ConnectionProvider.DbConnection.Open();
		}
		private static DbDataAdapter getDbDataAdapter( string selectCommandText )
		{
			DbDataAdapter dbDataAdapter = null;
			switch ( ConnectionProvider.DbType )
			{
				case DbType.Access:
					dbDataAdapter = new OleDbDataAdapter(
						selectCommandText , (OleDbConnection)ConnectionProvider.DbConnection );
					break;
				case DbType.MySql:
					dbDataAdapter = new MySqlDataAdapter(
						selectCommandText , (MySqlConnection)ConnectionProvider.DbConnection );
					break;
				default:
					throw new Exception(
						"Unknown database type. Complete the switch statement, please" );
			}
//			if ( ConnectionProvider.DbConnection is OleDbConnection )
//				dbDataAdapter = new OleDbDataAdapter(
//					selectCommandText , (OleDbConnection)ConnectionProvider.DbConnection );
//			if ( ConnectionProvider.DbConnection is MySqlConnection )
//				dbDataAdapter = new MySqlDataAdapter(
//					selectCommandText , (MySqlConnection)ConnectionProvider.DbConnection );
			return dbDataAdapter;
		}
		public static DbDataAdapter GetDbDataAdapter( string selectCommandText )
		{
			DbDataAdapterProvider.ensureConnectionIsOpen();
			DbDataAdapter dbDataAdapter =
				DbDataAdapterProvider.getDbDataAdapter( selectCommandText );
			return dbDataAdapter;
		}
		#endregion GetDbDataAdapter
	}
}
