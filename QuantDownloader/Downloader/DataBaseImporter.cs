using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using QuantProject.DataAccess.Tables;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Summary description for DataBaseImporter.
	/// </summary>
	public class DataBaseImporter
	{
		private StreamReader streamReader;
		private OleDbConnection oleDbConnection;
		private OleDbDataAdapter oleDbDataAdapter;
		private OleDbCommand oleDbCommand;
    private bool overWriteExistingRecords; 
		
		public DataBaseImporter( OleDbConnection oleDbConnection , StreamReader streamReader,
                              bool overWriteExistingRecords)
		{
			this.streamReader = streamReader;
			this.oleDbConnection = oleDbConnection;
			this.oleDbDataAdapter = new OleDbDataAdapter();
			this.oleDbCommand = new OleDbCommand("", this.oleDbConnection);
      this.overWriteExistingRecords = overWriteExistingRecords;
		}
		
		private void updateDataBase(OleDbDataAdapter myAdapter, DataSet myDataSet,
									string tableName)
		{
			try
			{
				myAdapter.Update(myDataSet, tableName);
			}
			catch(Exception ex)
			{
				// no consequences if there are some lines that are not inserted (due to key violation)        
				string notUsed = ex.ToString();
			}
		}
    /*
    private void deleteExistingQuote(string tickerToDelete, DateTime dateOfQuoteToDelete)
    {
      try
      {
        if(this.overWriteExistingRecords == false)
          return;
        //this.oleDbDataAdapter.DeleteCommand = new OleDbCommand("DELETE * FROM quotes " +
        //                                                        "WHERE quTicker ='" +
        //                                                        tickerToDelete + "' AND " +
        //                                                        "quDate =#" + 
        //                                                        dateOfQuoteToDelete + "#");
        //this.oleDbDataAdapter.DeleteCommand.ExecuteNonQuery();
      }
      catch(Exception ex)
      {
        string notUsed = ex.ToString();
      }
    }
    */

		public void ImportTicker( string ticker )
		{
		  string Line;
		  string[] LineIn;

		  /* string strAccessSelect = "Select * from quotes where 1=2";
		  OleDbCommand myAccessCommand = new 
			  OleDbCommand( strAccessSelect , oleDbConnection );

		  OleDbDataAdapter myDataAdapter = new 
			  OleDbDataAdapter( myAccessCommand );
		  OleDbCommandBuilder myCB = new OleDbCommandBuilder( myDataAdapter );

		  DataSet myDataSet = new DataSet();
		  myDataAdapter.MissingSchemaAction = MissingSchemaAction.Add;
		  myDataAdapter.MissingMappingAction = MissingMappingAction.Passthrough;
		  myDataAdapter.Fill( myDataSet , "Data" );*/

		  Line = streamReader.ReadLine();
		  Line = streamReader.ReadLine();
  		
      while ( Line != null && ! Line.StartsWith("<"))
		  {
			  LineIn=Line.Split(',');
  	    
			  /*DataRow myRow=myDataSet.Tables["Data"].NewRow();

			  myRow[ "quTicker" ] = ticker;
			  myRow[ "quDate" ]=DateTime.Parse( LineIn[0] );
			  myRow[ "quOpen" ]=Double.Parse( LineIn[1] );
			  myRow[ "quHigh" ]=Double.Parse( LineIn[2] );
			  myRow[ "quLow" ]=Double.Parse( LineIn[3] );
			  myRow[ "quClose" ]=Double.Parse( LineIn[4] );
			  myRow[ "quVolume" ]=Double.Parse( LineIn[5] );
			  myRow[ "quAdjustedClose" ]=Double.Parse( LineIn[6] );
			  
        myDataSet.Tables["Data"].Rows.Add(myRow);*/
        
        //the corresponding record is deleted if in the web downloader form
        // the radio button "over Write existing record" is checked
        if(this.overWriteExistingRecords)
          Quotes.Delete(ticker, DateTime.Parse( LineIn[0] ));
        Quotes.Add(ticker,DateTime.Parse( LineIn[0] ), Double.Parse( LineIn[1] ),
                   Double.Parse(LineIn[2]), Double.Parse(LineIn[3]), Double.Parse(LineIn[4]),
                   Double.Parse(LineIn[5]), Double.Parse(LineIn[6]));
        Line = this.streamReader.ReadLine();
		  }
		  //this.updateDataBase(myDataAdapter, myDataSet,"Data");
		}
	  	
	}

}
