using System;
using System.Data;
using System.Data.Common;

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
//			OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter( SqlQuery , ConnectionProvider.DbConnection );
			DbDataAdapter dbDataAdapter =
				DbDataAdapterProvider.GetDbDataAdapter( SqlQuery );
			dbDataAdapter.Fill( dataTable );
			return dataTable;
		}
		public static void SetDataTable( string sqlQuery , DataTable dataTable )
		{
			DbDataAdapter dbDataAdapter =
				DbDataAdapterProvider.GetDbDataAdapter( sqlQuery );
			dbDataAdapter.Fill( dataTable );
		}
		public static int ExecuteNonQuery( string sqlNonQuery )
		{
			if(ConnectionProvider.DbConnection.State != ConnectionState.Open)
				ConnectionProvider.DbConnection.Open();
			//        OleDbCommand oleDbCommand = new OleDbCommand( SqlNonQuery ,
//				ConnectionProvider.DbConnection );
			DbCommand dbCommand = DbCommandProvider.GetDbCommand( sqlNonQuery );
			int numberOfRowsAffected = dbCommand.ExecuteNonQuery();
			return numberOfRowsAffected;
		}
	}
}
