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
using System.Data.OleDb;

namespace QuantProject.DataAccess
{
	/// <summary>
	/// Returns a complete OleDbDataAdapter (with proper edit commands already set) to work with
	/// the given table
	/// </summary>
	public class OleDbSingleTableAdapter
	{
		private string tableName;
		public string TableName
		{
			get { return this.tableName; }
			set { this.tableName = value; }
		}
		private OleDbDataAdapter oleDbDataAdapter;
		public OleDbDataAdapter OleDbDataAdapter
		{
			get { return this.oleDbDataAdapter; }
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
				//this.dataTable = new DataTable();
        //the member is already set by the constructor
				this.oleDbDataAdapter =
					new OleDbDataAdapter( selectStatement , ConnectionProvider.OleDbConnection );
				OleDbCommandBuilder oleDbCommandBuilder = new OleDbCommandBuilder( oleDbDataAdapter );
				this.oleDbDataAdapter.InsertCommand = oleDbCommandBuilder.GetInsertCommand();
				this.oleDbDataAdapter.UpdateCommand = oleDbCommandBuilder.GetUpdateCommand();
				this.oleDbDataAdapter.DeleteCommand = oleDbCommandBuilder.GetDeleteCommand();
				this.oleDbDataAdapter.Fill( this.dataTable );
			}
			catch ( Exception ex )
			{
				string exceptionMessage = ex.Message + "\n" + ex.StackTrace;
				Console.WriteLine( exceptionMessage );
			}
		}

		public OleDbSingleTableAdapter( string selectStatement )
		{
			this.dataTable = new DataTable();
			setAdapter( selectStatement );
		}
    
    public OleDbSingleTableAdapter(string selectStatement, DataTable table )
    {
			this.dataTable = table;
			setAdapter( selectStatement );
    }

		public OleDbSingleTableAdapter()
		{
		}

		public void SetAdapter( string tableName )
		{
			string selectStatement =
				"select * from " + tableName + " where 1=2";
			setAdapter( selectStatement );
		}
	}
}
