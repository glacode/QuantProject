using System;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace QuantDownloader
{
	/// <summary>
	/// Summary description for DataBaseImporter.
	/// </summary>
	public class DataBaseImporter
	{
    private StreamReader streamReader;
    private OleDbConnection oleDbConnection;
		public DataBaseImporter( OleDbConnection oleDbConnection , StreamReader streamReader )
		{
			this.streamReader = streamReader;
      this.oleDbConnection = oleDbConnection;
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

      //Create Dataset and table
      DataSet myDataSet = new DataSet();
   
      //Set MissingSchemaAction Property
      myDataAdapter.MissingSchemaAction = MissingSchemaAction.Add;
   
   
      //Set MissingMappingAction Property
      myDataAdapter.MissingMappingAction = MissingMappingAction.Passthrough;

      //Fill data adapter
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
    
        Line = this.streamReader.ReadLine();
      }
   
      myDataAdapter.Update(myDataSet, "Data");
  
    }
	}
}
