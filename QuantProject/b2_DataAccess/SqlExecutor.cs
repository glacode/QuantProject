using System;
using System.Data;
using System.Data.OleDb;

namespace QuantProject.DataAccess
{
	/// <summary>
	/// Executes Sql queries returning a DataTable
	/// </summary>
	public class SqlExecutor
	{
		public SqlExecutor()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public static DataTable GetDataTable( string SqlQuery )
		{
			DataTable dataTable = new DataTable();
			OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter( SqlQuery , ConnectionProvider.OleDbConnection );
			oleDbDataAdapter.Fill( dataTable );
			return dataTable;
		}
		public static void ExecuteNonQuery( string SqlNonQuery )
		{
			OleDbCommand oleDbCommand = new OleDbCommand( SqlNonQuery ,
				ConnectionProvider.OleDbConnection );
			oleDbCommand.ExecuteNonQuery();
		}
	}
}
