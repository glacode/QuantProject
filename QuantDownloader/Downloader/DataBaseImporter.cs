using System;
using System.Data;
using System.Data.OleDb;
using System.IO;

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
    private void deleteExistingQuote(string tickerToDelete, DateTime dateOfQuoteToDelete)
    {
      try
      {
        if(this.overWriteExistingRecords == false)
          return;
        this.oleDbDataAdapter.DeleteCommand = new OleDbCommand("DELETE * FROM quotes " +
                                                                "WHERE quTicker ='" +
                                                                tickerToDelete + "' AND " +
                                                                "quDate =#" + 
                                                                dateOfQuoteToDelete + "#");
        this.oleDbDataAdapter.DeleteCommand.ExecuteNonQuery();
      }
      catch(Exception ex)
      {
        // no consequences if there are some lines that are not inserted (due to key violation)        
        string notUsed = ex.ToString();
      }
    }


		public void ImportTicker( string ticker )
		{
		  string Line;
		  string[] LineIn;

		  string strAccessSelect = "Select * from quotes where 1=2";
		  OleDbCommand myAccessCommand = new 
			  OleDbCommand( strAccessSelect , oleDbConnection );

		  OleDbDataAdapter myDataAdapter = new 
			  OleDbDataAdapter( myAccessCommand );
		  OleDbCommandBuilder myCB = new OleDbCommandBuilder( myDataAdapter );

		  DataSet myDataSet = new DataSet();
		  myDataAdapter.MissingSchemaAction = MissingSchemaAction.Add;
		  myDataAdapter.MissingMappingAction = MissingMappingAction.Passthrough;
		  myDataAdapter.Fill( myDataSet , "Data" );

		  Line = streamReader.ReadLine();
		  Line = streamReader.ReadLine();
  		
      while ( Line != null )
		  {
			  LineIn=Line.Split(',');
  	    
			  DataRow myRow=myDataSet.Tables["Data"].NewRow();

			  myRow[ "quTicker" ] = ticker;
			  myRow[ "quDate" ]=DateTime.Parse( LineIn[0] );
			  myRow[ "quOpen" ]=Double.Parse( LineIn[1] );
			  myRow[ "quHigh" ]=Double.Parse( LineIn[2] );
			  myRow[ "quLow" ]=Double.Parse( LineIn[3] );
			  myRow[ "quClose" ]=Double.Parse( LineIn[4] );
			  myRow[ "quVolume" ]=Double.Parse( LineIn[5] );
			  myRow[ "quAdjustedClose" ]=Double.Parse( LineIn[6] );
			  //myRow[ "quAdjustedOpen" ]=Convert.ToDouble(myRow[ "quOpen" ])*
			  //  (Convert.ToDouble(myRow[ "quAdjustedClose" ])/Convert.ToDouble(myRow[ "quOpen" ]));

	  //        myRow["date"]=DateTime.Parse(LineIn[0]);
	  //        myRow["time"]=DateTime.Parse(LineIn[1]);
	  //        myRow["sv1485ri"]=Double.Parse(LineIn[2]);
	  //        myRow["sv14856s"]=Double.Parse(LineIn[3]);
	  //        myRow["d4461"]=Double.Parse(LineIn[4]);
	  //        myRow["d6sf"]=Double.Parse(LineIn[5]);
	  //        myRow["d6sdp"]=Double.Parse(LineIn[6]);
	  //        myRow["oppai"]=Double.Parse(LineIn[7]);
	  //        myRow["oppbi"]=Double.Parse(LineIn[8]);
	  //        myRow["opps"]=Double.Parse(LineIn[9]);
	  //        myRow["o24hrtf"]=Double.Parse(LineIn[10]);
	  //        myRow["oif"]=Double.Parse(LineIn[11]);
	  //        myRow["otct"]=Double.Parse(LineIn[12]);
	  //        myRow["d1abt"]=Double.Parse(LineIn[13]);
	  //        myRow["d1bbt"]=Double.Parse(LineIn[14]);
	  //        myRow["d3bt"]=Double.Parse(LineIn[15]);
	  //        myRow["d2bt"]=Double.Parse(LineIn[16]);
	  //        myRow["d5bt"]=Double.Parse(LineIn[17]);
	  //        myRow["d6bt"]=Double.Parse(LineIn[18]);
	  //        myRow["cv1480cvpi"]=Double.Parse(LineIn[19]);
	  //        myRow["cv1481cvpi"]=Double.Parse(LineIn[20]);
			  
        myDataSet.Tables["Data"].Rows.Add(myRow);
        this.deleteExistingQuote(ticker, DateTime.Parse(LineIn[0]));
        //the corresponding record is deleted if in the web downloader form
        // the radio button "over Write existing record" is checked
			  Line = this.streamReader.ReadLine();
		  }
		  this.updateDataBase(myDataAdapter, myDataSet,"Data");
		}
	  	
	}

}
