/*
QuantProject - Quantitative Finance Library

OleDbSingleTableAdapter.cs
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
using System.Data;
using System.Data.Common;

namespace QuantProject.DataAccess
{
	/// <summary>
	/// Returns a complete DbDataAdapter (with proper edit commands already set) to work with
	/// the given table
	/// </summary>
	public class SingleTableDbDataAdapter
	{
		private string tableName;
		public string TableName
		{
			get { return this.tableName; }
			set { this.tableName = value; }
		}
		private DbDataAdapter dbDataAdapter;
		public DbDataAdapter DbDataAdapter
		{
			get { return this.dbDataAdapter; }
		}
		
		private DataTable dataTable;
		public DataTable DataTable
		{
			get { return this.dataTable; }
		}

		private void setAdapter( string selectStatement )
		{
			try
			{
//				this.dbDataAdapter =
//					new DbDataAdapter( selectStatement , ConnectionProvider.DbConnection );
				this.dbDataAdapter = DbDataAdapterProvider.GetDbDataAdapter( selectStatement );
//				DbCommandBuilder oleDbCommandBuilder = new DbCommandBuilder( dbDataAdapter );
				DbCommandBuilder dbCommandBuilder =
					DbCommandBuilderProvider.GetDbCommanBuilder( this.dbDataAdapter );
				this.dbDataAdapter.InsertCommand = dbCommandBuilder.GetInsertCommand();
				this.dbDataAdapter.UpdateCommand = dbCommandBuilder.GetUpdateCommand();
				this.dbDataAdapter.DeleteCommand = dbCommandBuilder.GetDeleteCommand();
				this.dbDataAdapter.Fill( this.dataTable );
			}
			catch ( Exception ex )
			{
				string exceptionMessage = ex.Message + "\n" + ex.StackTrace;
				Console.WriteLine( exceptionMessage );
				throw;
			}
		}

		public SingleTableDbDataAdapter( string selectStatement )
		{
			this.dataTable = new DataTable();
			this.setAdapter( selectStatement );
		}
    
    public SingleTableDbDataAdapter(string selectStatement, DataTable table )
    {
			this.dataTable = table;
			setAdapter( selectStatement );
    }

		public SingleTableDbDataAdapter()
		{
			this.dataTable = new DataTable();
		}

		public void SetAdapter( string tableName )
		{
			string selectStatement =
				"select * from " + tableName + " where 1=2";
			setAdapter( selectStatement );
		}
	}
}
