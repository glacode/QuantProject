using System;
using System.Data;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Summary description for TickerDownloader.
	/// </summary>
  public class TickerDownloader
  {
    private System.Data.OleDb.OleDbConnection oleDbConnection1;
    private WebDownloader p_myForm;
    private DataRow p_currentDataTickerRow;
    private string p_quTicker;
    private int p_numRows;
    private DateTime startDate = new DateTime( 2000 , 1 , 1 );
    private DateTime endDate = DateTime.Today;
    private int endDay = DateTime.Now.Day;
    private int endMonth = DateTime.Now.Month;
    private int endYear = DateTime.Now.Year;
    public TickerDownloader( WebDownloader myForm, DataRow currentDataTickerRow, string quTicker , int numRows )
    {
      p_myForm = myForm;
      p_currentDataTickerRow = currentDataTickerRow;
      p_quTicker = quTicker;
      p_numRows = numRows;
      this.oleDbConnection1 = myForm.OleDbConnection1;
    }
    private void addTickerTo_gridDataSet()
    {
      
      DataRow newRow = p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].NewRow();
      newRow[ "tiTicker" ] = p_quTicker;
      newRow[ "currentState" ] = "Start";
      //lock( p_myForm.dsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Rows )
      //{
      try
      {
        //MessageBox.Show( (p_myForm == null).ToString() );
        //MessageBox.Show( (p_myForm.dsTickerCurrentlyDownloaded == null).ToString() );
        //MessageBox.Show( (p_myForm.dsTickerCurrentlyDownloaded.Tables[ "Tickers" ] == null).ToString() );
        //MessageBox.Show( (p_myForm.dsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Rows == null).ToString() );
        //MessageBox.Show( (newRow == null).ToString() );
        p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Rows.Add( newRow );
      } 
      catch (Exception ex)
      {
        MessageBox.Show( ex.ToString() );
      }
      //}
      p_myForm.dataGrid1.Refresh();
    }

    private void removeTickerFrom_gridDataSet()
    {
      DataRow[] myRows = p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Select( "tiTicker='" + p_quTicker + "'" );
      p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Rows.Remove( myRows[ 0 ] );
      DataRow newRow = p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].NewRow();
      newRow[ "tiTicker" ] = p_quTicker;
      newRow[ "currentState" ] = "Start";
      p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Rows.Add( newRow );
      p_myForm.dataGrid1.Refresh();
    }
    private void updateCurrentStatus( string newState )
    {
      lock( p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ] )
      {
        DataRow[] myRows = p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Select( "tiTicker='" + p_quTicker + "'" );
        myRows[ 0 ][ "currentState" ] = newState;
        p_myForm.dataGrid1.Refresh();
      }
    }

    private void addTickerToFaultyTickers()
    {
      System.Data.OleDb.OleDbCommand odc = new System.Data.OleDb.OleDbCommand();
      odc.CommandText = "insert into faultyTickers ( ftTicker , ftDateTime ) " +
        "values ( '" + p_quTicker + "' , #" +
        DateTime.Now.Month + "/" +
        DateTime.Now.Day + "/" +
        DateTime.Now.Year + " " +
        DateTime.Now.Hour + "." +
        DateTime.Now.Minute + "." +
        DateTime.Now.Second + "# )";
      odc.Connection = this.oleDbConnection1;
      odc.ExecuteNonQuery();
    }
    private void writeFile_tickerCsv_forNextTimeFrame( DateTime currBeginDate , DateTime currEndDate )
    {
      int a = currBeginDate.Month - 1;
      int b = currBeginDate.Day;
      int c = currBeginDate.Year;
      int d = currEndDate.Month - 1;
      int e = currEndDate.Day;
      int f = currEndDate.Year;
      int numTrials = 1;

      //updateCurrentStatus( " 1 " );
      while (numTrials < 5)
      {
        this.p_myForm.Refresh();
        try
        {
          HttpWebRequest Req = (HttpWebRequest)WebRequest.Create("http:" + "//table.finance.yahoo.com/table.csv?a=" 
            + a + "&b=" + b + "&c=" + c +"&d=" + d + "&e=" + e + "&f=" + f + "&s=" + p_quTicker + "&y=0&g=d&ignore=.csv");

          Req.Method = "GET";
          Req.Timeout = 10000;

          HttpWebResponse hwr = (HttpWebResponse)Req.GetResponse();

          //updateCurrentStatus( " 2 " );
          Stream strm = hwr.GetResponseStream();
          //updateCurrentStatus( " 3 " );
          StreamReader sr = new StreamReader(strm);
//          StreamWriter sw=new StreamWriter( "C:\\Documents and Settings\\Glauco\\My Documents\\Visual Studio Projects\\QuantProject\\csvFiles\\" + p_quTicker + ".csv");
//          sw.Write(myString);
//          sw.Close();
          DataBaseImporter dataBaseImporter =
            new DataBaseImporter( this.oleDbConnection1 , sr );
          dataBaseImporter.ImportTicker( p_quTicker );
          sr.Close();
          strm.Close();


//          import_tickerCsv_into_quotesCsv();
          updateCurrentStatus( d + "/" + e + "/" + f );
          numTrials = 6 ;
          //updateCurrentStatus( " scritto file! " );
        }
        catch (Exception exception)
        {
          MessageBox.Show( exception.ToString() );
          updateCurrentStatus( "Tentativo: " + numTrials );
          numTrials++;
          if (numTrials > 5)
            addTickerToFaultyTickers();
        }
      }
    }
    private void writeFile_tickerCsv()
    {
      DateTime currBeginDate = new DateTime();
      currBeginDate = startDate;
      while ( currBeginDate < endDate )
      {
        DateTime currEndDate = new DateTime();
        if ( DateTime.Compare( DateTime.Today , currBeginDate.AddDays( 200 ) ) < 0 )
          currEndDate = DateTime.Today;
        else
          currEndDate = currBeginDate.AddDays( 200 );
        writeFile_tickerCsv_forNextTimeFrame( currBeginDate , currEndDate );
        currBeginDate = currEndDate.AddDays( 1 );
      }
    }
    private void import_tickerCsv_into_quotesCsv()
    {
      string sConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
        "C:\\Documents and Settings\\Glauco\\My Documents\\Visual Studio Projects\\QuantProject\\csvFiles\\" +
        ";Extended Properties=\"Text;HDR=YES;FMT=Delimited\"";

      System.Data.OleDb.OleDbConnection objConn = new System.Data.OleDb.OleDbConnection(sConnectionString);
      objConn.Open();
      System.Data.OleDb.OleDbCommand odCommand = new System.Data.OleDb.OleDbCommand();
      odCommand.Connection = objConn;
      try
      {
        odCommand.CommandText = "insert into quotes.csv SELECT '" + p_quTicker + "' as Ticker, * FROM " + p_quTicker + ".csv";
        odCommand.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        MessageBox.Show( ex.ToString() );
      }
      objConn.Close();
    }
    public void downloadTicker()
    {
    {
      addTickerTo_gridDataSet();
      writeFile_tickerCsv();
      //Monitor.Pulse( p_myForm.dsTickerCurrentlyDownloaded.Tables[ "Tickers" ] );
    }
    }
  }
}
