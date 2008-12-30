/*
QuantProject - Quantitative Finance Library

DbCommandProvider.cs
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
	/// Provides a DbCommandBuilder for the given DbDataProvider
	/// </summary>
	public class DbCommandBuilderProvider
	{
		public DbCommandBuilderProvider()
		{
		}
		public static DbCommandBuilder GetDbCommanBuilder( DbDataAdapter dbDataAdapter )
		{
			DbCommandBuilder dbCommandBuilder = null;
			switch ( ConnectionProvider.DbType )
			{
				case DbType.Access:
					dbCommandBuilder = new OleDbCommandBuilder(
						(OleDbDataAdapter)dbDataAdapter );
					break;
				case DbType.MySql:
					dbCommandBuilder = new MySqlCommandBuilder(
						(MySqlDataAdapter)dbDataAdapter );
					break;
				default:
					throw new Exception(
						"Unknown database type. Complete the switch statement, please" );
			}
//			if ( dbDataAdapter is OleDbDataAdapter )
//				dbCommandBuilder = new OleDbCommandBuilder(
//					(OleDbDataAdapter)dbDataAdapter );
//			if ( dbDataAdapter is MySqlDataAdapter )
//				dbCommandBuilder = new MySqlCommandBuilder(
//					(MySqlDataAdapter)dbDataAdapter );
			return dbCommandBuilder;
		}
	}
}
