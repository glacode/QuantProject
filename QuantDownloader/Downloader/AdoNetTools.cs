using System;
using System.Data.OleDb;
using QuantProject.DataAccess;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Summary description for AdoNetTools.
	/// </summary>
	public class AdoNetTools
	{
		public AdoNetTools()
		{
			//
			// TODO: Add constructor logic here
			//
		}

    private static OleDbConnection oleDbConnection;

    public static OleDbConnection OleDbConnection
    {
      get
      {
        if ( oleDbConnection == null )
          return oleDbConnection;
        else
        {
          DataBaseLocator dataBaseLocator = new DataBaseLocator("MDB"); 
          string mdbPath = dataBaseLocator.Path;
          string connectionString =
            @"Provider=Microsoft.Jet.OLEDB.4.0;Password="""";User ID=Admin;Data Source=" +
            mdbPath +
            @";Jet OLEDB:Registry Path="""";Jet OLEDB:Database Password="""";Jet OLEDB:Engine Type=5;Jet OLEDB:Database Locking Mode=1;Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Global Bulk Transactions=1;Jet OLEDB:New Database Password="""";Jet OLEDB:Create System Database=False;Jet OLEDB:Encrypt Database=False;Jet OLEDB:Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;Jet OLEDB:SFP=False";
          oleDbConnection = new OleDbConnection( connectionString );
          return oleDbConnection;
        }
      }
    }
	}
}
