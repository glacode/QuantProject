using System;
using System.Data;
using System.Data.OleDb;

namespace QuantProject.Applications.Downloader.Validate
{
	/// <summary>
	/// Summary description for QuotesToBeValidated.
	/// </summary>
	public class QuotesToBeValidated : DataTable
	{
    private string selectStatement;
    private OleDbDataAdapter oleDbDataAdapter;
    
    public QuotesToBeValidated( string tickerIsLike )
		{
//      this.selectStatement =
//        "select * from quotes where quTicker like '" + tickerIsLike + "'";
      this.selectStatement =
        "select * from quotes where quTicker = '" + tickerIsLike + "'";
      this.oleDbDataAdapter = new OleDbDataAdapter( selectStatement , AdoNetTools.OleDbConnection );
      try
      {
        this.oleDbDataAdapter.Fill( this );
      }
      catch (Exception exception)
      {
        Console.WriteLine( exception.ToString() );
      }
    }
	}
}
