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
    private DateTime startDate;
    private DateTime endDate = DateTime.Today;
    private int endDay = DateTime.Now.Day;
    private int endMonth = DateTime.Now.Month;
    private int endYear = DateTime.Now.Year;
    private DateTime INITIAL_DATE = new DateTime(1980, 1, 1); 
    public TickerDownloader( WebDownloader myForm, DataRow currentDataTickerRow, string quTicker , int numRows )
    {
      this.startDate = this.INITIAL_DATE;
      p_myForm = myForm;
      p_currentDataTickerRow = currentDataTickerRow;
      p_quTicker = quTicker;
      p_numRows = numRows;
      this.oleDbConnection1 = myForm.OleDbConnection1;
    }

    /*
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
    */	
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
    
	  private void importTickerForCurrentTimeFrame( DateTime currBeginDate , DateTime currEndDate )
    {
      int a = currBeginDate.Month - 1;
      int b = currBeginDate.Day;
      int c = currBeginDate.Year;
      int d = currEndDate.Month - 1;
      int e = currEndDate.Day;
      int f = currEndDate.Year;
      int numTrials = 1;

      while (numTrials < 5)
      {
        this.p_myForm.Refresh();
        try
        {
          HttpWebRequest Req = (HttpWebRequest)WebRequest.Create("http:" + "//table.finance.yahoo.com/table.csv?a=" 
            + a + "&b=" + b + "&c=" + c +"&d=" + d + "&e=" + e + "&f=" + f + "&s=" + p_quTicker + "&y=0&g=d&ignore=.csv");
          Req.Method = "GET";
          Req.Timeout = 25000;
          HttpWebResponse hwr = (HttpWebResponse)Req.GetResponse();
          Stream strm = hwr.GetResponseStream();
          StreamReader sr = new StreamReader(strm);

          DataBaseImporter dataBaseImporter =
            new DataBaseImporter( this.oleDbConnection1 , sr, this.p_myForm.radioButtonOverWriteYes.Checked );
          dataBaseImporter.ImportTicker( p_quTicker );
          sr.Close();
          strm.Close();
          hwr.Close();
          
		  updateCurrentStatus( d + "/" + e + "/" + f );
          numTrials = 6 ;
        }
        catch (Exception exception)
        {
          MessageBox.Show( exception.ToString() );
          updateCurrentStatus( "Trial: " + numTrials );
          numTrials++;
          if (numTrials > 5)
            addTickerToFaultyTickers();
        }
      }
    }
    
	  private void setTimeFrameAndImportTickerForEachTimeFrame(double numDaysOfTimeFrame)
    {
      if(numDaysOfTimeFrame == 0)
        return;

      DateTime currBeginDate = new DateTime();
      currBeginDate = startDate;
      while ( currBeginDate < endDate )
      {
        DateTime currEndDate = new DateTime();
        if ( DateTime.Compare( DateTime.Today , currBeginDate.AddDays( numDaysOfTimeFrame ) ) < 0 )
          currEndDate = DateTime.Today;
        else
          currEndDate = currBeginDate.AddDays( numDaysOfTimeFrame );
        this.importTickerForCurrentTimeFrame ( currBeginDate , currEndDate );
        currBeginDate = currEndDate.AddDays( 1 );
      }
    }
    
    private void addTickerTo_gridDataSet()
    {
      DataRow newRow = p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].NewRow();
      newRow[ "tiTicker" ] = p_quTicker;
      newRow[ "currentState" ] = "Searching ...";
      try
      {
        p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Rows.Add( newRow );
        p_myForm.labelNumberOfTickersToDownload.Text = 
                      Convert.ToString(Convert.ToInt16(p_myForm.labelNumberOfTickersToDownload.Text) - 1);
      } 
      catch (Exception ex)
      {
        MessageBox.Show( ex.ToString() );
      }
      p_myForm.dataGrid1.Refresh();
    }

    public void DownloadTicker()
    {
      // update grid in webdownloader form
	    addTickerTo_gridDataSet();
      /* if(tickerIsInDatabase && p_myForm.UpdateFlagYes)
      {
        // try to import ticker before the first quote
        
        // try to import ticker after the last quote
      }
      else
      // tickers'quotes are downloaded for the first time
      {*/
        if(this.p_myForm.checkBoxIsDicotomicSearchActivated.Checked  == true) 
          this.startDate = firstAvailableDateOnYahoo(this.INITIAL_DATE, this.endDate);
          setTimeFrameAndImportTickerForEachTimeFrame(200);

      //}

      //Monitor.Pulse( p_myForm.dsTickerCurrentlyDownloaded.Tables[ "Tickers" ] );
    }
    public void DownloadTicker(DateTime startingDate)
    {
      this.INITIAL_DATE = startingDate;
      this.DownloadTicker();
      
    }


    private bool isAtLeastOneDateAvailable(StreamReader streamReader, int daysToBeTested)
    {
      string Line;
      Line = streamReader.ReadLine();
      // column headers are read
      Line = streamReader.ReadLine();
      //actual values are read
      bool isOneDateAvailableInNextDaysToBeTested = false;
      int numDays = 1;
      while(numDays < daysToBeTested)
      {
        if(Line != null)
        {
          isOneDateAvailableInNextDaysToBeTested = true;
          numDays = daysToBeTested + 1;
        }
        numDays++;

      }
      return isOneDateAvailableInNextDaysToBeTested;

    }
    

    private bool getResponseForTimeWindow( DateTime initialDateOfTheTimeWindow,
                                            int daysOfTheTimeWindow )
    {
      int a = initialDateOfTheTimeWindow.Month - 1;
      int b = initialDateOfTheTimeWindow.Day;
      int c = initialDateOfTheTimeWindow.Year;
      DateTime endDateOfTheTimeWindow = initialDateOfTheTimeWindow.AddDays(daysOfTheTimeWindow);
      int d = endDateOfTheTimeWindow.Month - 1;
      int e = endDateOfTheTimeWindow.Day;
      int f = endDateOfTheTimeWindow.Year;
      HttpWebRequest Req;
      HttpWebResponse hwr;
      Stream strm;
      StreamReader sr;
      bool response = false;
      int numTrials = 1;
      while(numTrials < 5)
      {
        try
        {
          Req = (HttpWebRequest)WebRequest.Create("http:" + "//table.finance.yahoo.com/table.csv?a=" 
            + a + "&b=" + b + "&c=" + c +"&d=" + d + "&e=" + e + "&f=" + f + "&s=" + p_quTicker + "&y=0&g=d&ignore=.csv");
          Req.Method = "GET";
          Req.Timeout = 20000;
          hwr = (HttpWebResponse)Req.GetResponse();
          strm = hwr.GetResponseStream();
          sr = new StreamReader(strm);
          response = this.isAtLeastOneDateAvailable(sr, daysOfTheTimeWindow );
          sr.Close();
          strm.Close();
          hwr.Close();
          numTrials = 6;
        }
        catch (Exception exception)
        {
          MessageBox.Show( exception.ToString() + "\n\n for: " + initialDateOfTheTimeWindow.ToString());
          numTrials++;
        }
      }
      return response;
    }
    private DateTime getMiddleDate(DateTime startingDate,
                                   DateTime endingDate)
    {
      DateTime middleDate;
      double differenceInDays = endingDate.Subtract(startingDate).Days;
      middleDate = startingDate.AddDays(differenceInDays / 2);
      return middleDate;
    }


    private DateTime firstAvailableDateOnYahoo(DateTime startingDate,
                                                DateTime endingDate)
    {
      
      if(startingDate == this.INITIAL_DATE)
      // at the first call, when the actual parameters are INITIAL_DATE and now
      // the dicotomic search may be useless ...
        if(getResponseForTimeWindow(startingDate, 10) == true)
        {
          return startingDate;
        }
      
      DateTime middleDate = getMiddleDate(startingDate, endingDate);
      
      if(endingDate.Subtract(startingDate).Days <= 10)
      {
        return startingDate;
      }
      else
      {
        if (getResponseForTimeWindow(middleDate, 10) == true)
        {
          return firstAvailableDateOnYahoo(startingDate, middleDate);
        }   
        else
        {
          return firstAvailableDateOnYahoo(middleDate, endingDate);
        }
      }

    }
  }
}
