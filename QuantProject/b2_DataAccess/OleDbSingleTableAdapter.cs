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
		
		public OleDbSingleTableAdapter( string selectStatement )
		{
			try
			{
//				string selectStatement =
//					"select * from " + tableName + " where 1=2";
				this.oleDbDataAdapter =
					new OleDbDataAdapter( selectStatement , ConnectionProvider.OleDbConnection );
				OleDbCommandBuilder oleDbCommandBuilder = new OleDbCommandBuilder( oleDbDataAdapter );
				this.oleDbDataAdapter.InsertCommand = oleDbCommandBuilder.GetInsertCommand();
				this.oleDbDataAdapter.UpdateCommand = oleDbCommandBuilder.GetUpdateCommand();
				this.oleDbDataAdapter.DeleteCommand = oleDbCommandBuilder.GetDeleteCommand();
				this.dataTable = new DataTable();
				this.oleDbDataAdapter.Fill( this.dataTable );
			}
			catch ( Exception ex )
			{
				string exceptionMessage = ex.Message + "\n" + ex.StackTrace;
				Console.WriteLine( exceptionMessage );
			}
		}
    
    public OleDbSingleTableAdapter(string selectStatement, DataTable table )
    {
      try
      {
        
        //				string selectStatement =
        //					"select * from " + tableName + " where 1=2";
        this.oleDbDataAdapter =
          new OleDbDataAdapter( selectStatement , ConnectionProvider.OleDbConnection );
        OleDbCommandBuilder oleDbCommandBuilder = new OleDbCommandBuilder( oleDbDataAdapter );
        this.oleDbDataAdapter.InsertCommand = oleDbCommandBuilder.GetInsertCommand();
        this.oleDbDataAdapter.UpdateCommand = oleDbCommandBuilder.GetUpdateCommand();
        this.oleDbDataAdapter.DeleteCommand = oleDbCommandBuilder.GetDeleteCommand();
        this.dataTable = table;
        this.oleDbDataAdapter.Fill( this.dataTable );
      }
      catch ( Exception ex )
      {
        string exceptionMessage = ex.Message + "\n" + ex.StackTrace;
        Console.WriteLine( exceptionMessage );
      }
    }


//		public void SetAdapterAndDataTable( string tableName )
//		{
//		}
	}
}
