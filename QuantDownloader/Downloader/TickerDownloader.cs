using System;
using System.Data;
using System.IO;
using System.Net;
using System.Windows.Forms;
using QuantProject.DataAccess;
using QuantProject.DataAccess.Tables;
using QuantProject.ADT;

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
    private DateTime INITIAL_DATE = ConstantsProvider.InitialDateTimeForDownload;
    private DateTime startDate;
    private DateTime endDate = DateTime.Today;
    private int endDay = DateTime.Now.Day;
    private int endMonth = DateTime.Now.Month;
    private int endYear = DateTime.Now.Year;
    private int numberOfQuotesInDatabase;
    private DataTable downloadedValuesFromSource = new DataTable("quotes");
    private OleDbSingleTableAdapter adapter;
    private Stream stream;
    private StreamReader streamReader;
    
    public TickerDownloader( WebDownloader myForm, DataRow currentDataTickerRow, string quTicker , int numRows )
    {
      this.startDate = this.INITIAL_DATE;
      p_myForm = myForm;
      p_currentDataTickerRow = currentDataTickerRow;
      p_quTicker = quTicker;
      p_numRows = numRows;
      this.oleDbConnection1 = myForm.OleDbConnection1;
      this.adapter = new OleDbSingleTableAdapter("SELECT * FROM quotes WHERE 1=2",
                                                  this.downloadedValuesFromSource);
      this.downloadedValuesFromSource.Columns[Quotes.AdjustedCloseToCloseRatio].DefaultValue = 0;
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
	
    #region Update Status 
    private void updateCurrentStatus( string newState )
    {
      lock( p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ] )
      {
        DataRow[] myRows = p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Select( "tiTicker='" + p_quTicker + "'" );
        myRows[ 0 ][ "currentState" ] = newState;
        p_myForm.dataGrid1.Refresh();
      }
    }
    
    private void updateCurrentStatusAdjustedClose(string status )
    {
      lock( p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ] )
      {
        DataRow[] myRows = p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Select( "tiTicker='" + p_quTicker + "'" );
        myRows[ 0 ][ "adjustedClose" ] = status;
        p_myForm.dataGrid1.Refresh();
      }
    }

    private void updateCurrentStatusAdjustedCloseToCloseRatio(string status )
    {
      lock( p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ] )
      {
        DataRow[] myRows = p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Select( "tiTicker='" + p_quTicker + "'" );
        myRows[ 0 ][ "adjCloseToCloseRatio" ] = status;
        p_myForm.dataGrid1.Refresh();
      }
    }

    private void updateCurrentStatusDatabaseUpdated(string status )
    {
      lock( p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ] )
      {
        DataRow[] myRows = p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Select( "tiTicker='" + p_quTicker + "'" );
        myRows[ 0 ][ "databaseUpdated" ] = status;
        p_myForm.dataGrid1.Refresh();
      }
    }

#endregion

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
    
    /*
    private void addNewAdjustedValueInTable( StreamReader streamReader,
                                             DataTable tableToWhichValuesHaveToBeAdded )
    {
      string Line;
      string[] LineIn;
      DataRow row;

      Line = streamReader.ReadLine();
      Line = streamReader.ReadLine();
  		
      while ( Line != null && ! Line.StartsWith("<"))
      {
        LineIn=Line.Split(',');
  	    row = tableToWhichValuesHaveToBeAdded.NewRow();
        row[0] = DateTime.Parse( LineIn[0] );
        row[1] = Double.Parse(LineIn[6]);
        row[2] = 0;
        tableToWhichValuesHaveToBeAdded.Rows.Add(row);
        
        Line = streamReader.ReadLine();
      }
    }
    */

    /*
    private DataTable getTableOfNewAdjustedValues( DateTime currBeginDate , DateTime currEndDate )
    {
     
      
      int a = currBeginDate.Month - 1;
      int b = currBeginDate.Day;
      int c = currBeginDate.Year;
      int d = currEndDate.Month - 1;
      int e = currEndDate.Day;
      int f = currEndDate.Year;
      DataTable table = new DataTable();
      table.Columns.Add("quDate", System.Type.GetType("System.DateTime"));
      table.Columns.Add("quAdjustedClose", System.Type.GetType("System.Double"));
      table.Columns.Add("quAdjustedCloseToCloseRatio", System.Type.GetType("System.Double"));
      try
      {
        HttpWebRequest Req = (HttpWebRequest)WebRequest.Create("http:" + "//table.finance.yahoo.com/table.csv?a=" 
          + a + "&b=" + b + "&c=" + c +"&d=" + d + "&e=" + e + "&f=" + f + "&s=" + p_quTicker + "&y=0&g=d&ignore=.csv");
        Req.Method = "GET";
        Req.Timeout = ConstantsProvider.TimeOutValue;
        HttpWebResponse hwr = (HttpWebResponse)Req.GetResponse();
        Stream strm = hwr.GetResponseStream();
        StreamReader sr = new StreamReader(strm);
        this.addNewAdjustedValueInTable(sr, table);
        sr.Close();
        strm.Close();
        //hwr.Close();
      }
      catch (Exception exception)
      {
        MessageBox.Show( exception.ToString() );
      }
      return table;
      
    }
    */
    /// <summary>
    /// Adds rows to the table (member of the object) containing the downloaded values
    /// </summary>
    private void addCurrentStreamToTable()
    {
      string Line;
      string[] LineIn;
      if(this.streamReader==null)
        return;
      Line = streamReader.ReadLine();
      Line = streamReader.ReadLine();
  		
      while ( Line != null && ! Line.StartsWith("<"))
      {
        LineIn=Line.Split(',');
  	    
        DataRow myRow =this.downloadedValuesFromSource.NewRow();

        myRow[ "quTicker" ] = this.p_quTicker;
        myRow[ "quDate" ]=DateTime.Parse( LineIn[0] );
        myRow[ "quOpen" ]=Double.Parse( LineIn[1] );
        myRow[ "quHigh" ]=Double.Parse( LineIn[2] );
        myRow[ "quLow" ]=Double.Parse( LineIn[3] );
        myRow[ "quClose" ]=Double.Parse( LineIn[4] );
        myRow[ "quVolume" ]=Double.Parse( LineIn[5] );
        myRow[ "quAdjustedClose" ]=Double.Parse( LineIn[6] );
			  
        this.downloadedValuesFromSource.Rows.Add(myRow);

        Line = streamReader.ReadLine();
      }
    }


	  private void importTickerForCurrentTimeFrame( DateTime currBeginDate , DateTime currEndDate )
    {
        try
        {
          this.p_myForm.Refresh();
          this.setStreamsFromYahoo(currBeginDate, currEndDate);
          this.addCurrentStreamToTable();
          if(this.streamReader!=null)
            this.streamReader.Close();
          else
            return;
          this.updateCurrentStatus(currEndDate.ToShortDateString());

		      //this.updateCurrentStatus( d + "/" + e + "/" + f );
        }
        catch (Exception exception)
        {
          string notUsed = exception.ToString();
          //MessageBox.Show( exception.ToString() );
          /*updateCurrentStatus( "Trial: " + numTrials );
          numTrials++;
          if (numTrials > 5)
            addTickerToFaultyTickers();*/
        }
    }
    


	  private DataTable getTableOfDownloadedValues()
    {
      double numDaysOfTimeFrame = ConstantsProvider.MaxNumDaysDownloadedAtEachConnection;
      this.downloadedValuesFromSource.Rows.Clear();
      this.downloadedValuesFromSource.AcceptChanges();
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
      return this.downloadedValuesFromSource;

    }
    
    private DataTable getTableOfDownloadedValues(DateTime newStartDate, DateTime newEndDate)
    {
      this.startDate = newStartDate;
      this.endDate = newEndDate;
      return this.getTableOfDownloadedValues();
    }

    private void addTickerTo_gridDataSet()
    {
      DataRow newRow = p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].NewRow();
      newRow[ "tiTicker" ] = p_quTicker;
      newRow[ "currentState" ] = "Searching ...";
      newRow[ "databaseUpdated" ] = "No";
      newRow[ "adjustedClose"] = "...";
      newRow[ "adjCloseToCloseRatio"] = "...";
      try
      {
        p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Rows.Add( newRow );
        p_myForm.labelNumberOfTickersToDownload.Text = 
                      Convert.ToString(Convert.ToInt16(p_myForm.labelNumberOfTickersToDownload.Text) - 1);
      } 
      catch (Exception ex)
      {
        string notUsed = ex.ToString();
        //MessageBox.Show( ex.ToString() );
      }
      p_myForm.dataGrid1.Refresh();
    }

    private void resetStartDateIfNecessary()
    {
      if(this.p_myForm.checkBoxIsDicotomicSearchActivated.Checked  == true) 
        this.startDate = firstAvailableDateOnYahoo(this.INITIAL_DATE, this.endDate);
    }

    private void downloadTickerBeforeFirstQuote()
    {
      this.endDate = Quotes.GetStartDate(this.p_quTicker);
      this.resetStartDateIfNecessary();
      this.checkForNewAdjustedAndContinueOrStop();
    }
    
    private bool getResponseForRepeatedChecks(int numberOfRepeatedChecks)
    {
      bool response = false;
      QuantProject.Data.DataTables.Quotes tickerQuotes =
				new QuantProject.Data.DataTables.Quotes(this.p_quTicker);
      for(int i = 1; i< this.numberOfQuotesInDatabase; i += this.numberOfQuotesInDatabase/numberOfRepeatedChecks)
      {
        DateTime dateToCheck = tickerQuotes.GetPrecedingDate(this.startDate, i);
        response = 
              Quotes.IsAdjustedCloseChanged(this.p_quTicker, dateToCheck,
                                            this.adjustedCloseFromSource(dateToCheck));
      }
      return response;
    }
    
    private void checkForNewAdjustedAndContinueOrStop()
    {
      try
      {
        if(this.getResponseForRepeatedChecks(ConstantsProvider.NumberOfCheckToPerformOnAdjustedValues))
        {
          this.updateCurrentStatusAdjustedClose("Changed!");
          if (Quotes.IsAdjustedCloseToCloseRatioChanged(this.p_quTicker, 
              this.getTableOfDownloadedValues(Quotes.GetStartDate(this.p_quTicker),
                                               Quotes.GetEndDate(this.p_quTicker))))
          {
               this.updateCurrentStatusAdjustedCloseToCloseRatio("Changed at " + 
                                                                  Quotes.DateWithDifferentCloseToClose);
               //stop
          }
          else
          {
               this.updateCurrentStatusAdjustedCloseToCloseRatio("OK");
               this.updateAdjustedClose();
               this.updateCurrentStatusAdjustedClose("Updated!");
               this.downloadedValuesFromSource = this.getTableOfDownloadedValues(Quotes.GetEndDate(this.p_quTicker),
                                                                                 DateTime.Now);
               this.commitDownloadedValuesToDatabase();
          } 
        }
        else
        //download is executed
        {
          this.downloadedValuesFromSource = this.getTableOfDownloadedValues();
          this.commitDownloadedValuesToDatabase();
          this.updateCurrentStatusAdjustedClose("OK");
        }
      }
      catch(Exception ex)
      {
        string notUsed = ex.ToString();
        //MessageBox.Show(ex.ToString());
      }
    }
    
    private void updateAdjustedClose()
    {
      foreach(DataRow row in this.downloadedValuesFromSource.Rows)
      {
        Quotes.UpdateAdjustedClose(this.p_quTicker, (DateTime)row[Quotes.Date], (float)row[Quotes.AdjustedClose]);
      }
    }
    
    
    private void downloadTickerAfterLastQuote()
    {
      this.startDate = Quotes.GetEndDate(this.p_quTicker);
      this.endDate = DateTime.Today;
      this.checkForNewAdjustedAndContinueOrStop();
    }
    
    private float adjustedCloseFromSource(DateTime adjustedCloseDate)
    {
      string Line;
      string[] LineIn = null;
      this.setStreamsFromYahoo(adjustedCloseDate, 0);
      Line = this.streamReader.ReadLine();
      Line = this.streamReader.ReadLine();
      if ( Line != null && ! Line.StartsWith("<"))
      {
        LineIn=Line.Split(',');
      }
      return Single.Parse(LineIn[6]);
    }
    
    private void commitDownloadedValuesToDatabase()
    {
      if(this.p_myForm.IsOverWriteYesSelected)
        Quotes.Delete(this.p_quTicker);
      if(this.p_myForm.IsComputeCloseToCloseRatioSelected)
        QuantProject.DataAccess.Tables.Quotes.ComputeCloseToCloseValues(this.downloadedValuesFromSource);
      this.adapter.OleDbDataAdapter.ContinueUpdateOnError = true;
      int rowsUpdated = this.adapter.OleDbDataAdapter.Update(this.downloadedValuesFromSource);
      if(rowsUpdated > 0)
        this.updateCurrentStatusDatabaseUpdated("YES");
      else
        this.updateCurrentStatus("Not found updatable quotes");
    }

    private void resetAndImportTicker()
    {
      this.resetStartDateIfNecessary();
      this.downloadedValuesFromSource = this.getTableOfDownloadedValues();
      this.commitDownloadedValuesToDatabase();
    }

    public void DownloadTicker()
    {
      // update grid in webdownloader form
      Cursor.Current = Cursors.WaitCursor; 
      addTickerTo_gridDataSet();
      this.numberOfQuotesInDatabase = Quotes.GetNumberOfQuotes(this.p_quTicker);
      if(this.numberOfQuotesInDatabase < 1)
      // ticker's quotes are downloaded for the first time 
      {
        this.resetAndImportTicker();
      }
      // in all these cases some ticker's quotes are in the database
      // and the options choosen by the user in the web downloader form are checked
      else if(this.p_myForm.IsOnlyAfterLastQuoteSelected && 
              this.numberOfQuotesInDatabase >= 1)
      {
        this.downloadTickerAfterLastQuote();
      }
      else if(this.p_myForm.IsBeforeAndAfterSelected &&
              this.numberOfQuotesInDatabase >= 1)
      {
        this.downloadTickerBeforeFirstQuote();
        this.downloadTickerAfterLastQuote();
      }
      else if(this.p_myForm.IsOverWriteNoSelected &&
              this.numberOfQuotesInDatabase >= 1)
      {
        this.resetStartDateIfNecessary(); 
        this.checkForNewAdjustedAndContinueOrStop();
      }
      else if(this.p_myForm.IsOverWriteYesSelected &&
        this.numberOfQuotesInDatabase >= 1)
      {
        this.resetAndImportTicker();
      }
      Cursor.Current = Cursors.Default; 
  
        // ticker's quotes are downloaded for the first time or
        // the user has chosen to download all quotes

      
    }
    public void DownloadTicker(DateTime startingDate)
    {
      this.INITIAL_DATE = startingDate;
      this.DownloadTicker();
    }


    private bool isAtLeastOneDateAvailable(StreamReader streamReader, int daysToBeTested)
    {
      string Line;
      if(streamReader == null)
        return false;
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
    
    private void setStreamsFromYahoo( DateTime initialDateOfTheTimeWindow,int daysOfTheTimeWindow)
                                      
    {
      DateTime endDateOfTheTimeWindow = initialDateOfTheTimeWindow.AddDays(daysOfTheTimeWindow);
      this.setStreamsFromYahoo(initialDateOfTheTimeWindow, endDateOfTheTimeWindow);
    }

    /*
    private StreamReader getStreamReaderFromYahoo(DateTime startDate, DateTime endDate)
    {
      int a = startDate.Month - 1;
      int b = startDate.Day;
      int c = startDate.Year;
      int d = endDate.Month - 1;
      int e = endDate.Day;
      int f = endDate.Year;
      HttpWebRequest Req;
      HttpWebResponse hwr;
      Stream strm;
      StreamReader sr = null;
      int numTrials = 1;
      while(numTrials < 5)
      {
        try
        {
          //					Req = (HttpWebRequest)WebRequest.Create("http:" + "//table.finance.yahoo.com/table.csv?a=" 
          //						+ a + "&b=" + b + "&c=" + c +"&d=" + d + "&e=" + e + "&f=" + f + "&s=" + p_quTicker + "&y=0&g=d&ignore=.csv");
          string url = "http:" + "//ichart.yahoo.com/table.csv?a="
            + a + "&b=" + b + "&c=" + c +"&d=" + d + "&e=" + e + "&f=" + f + "&s=" + p_quTicker + "&y=0&g=d&ignore=.csv";
          Req = (HttpWebRequest)WebRequest.Create( url );
          Req.Method = "GET";
          Req.Timeout = ConstantsProvider.TimeOutValue;
          hwr = (HttpWebResponse)Req.GetResponse();
          strm = hwr.GetResponseStream();
          sr = new StreamReader(strm);
          numTrials = 6;
          
        }
        catch (Exception exception)
        {
          string notUsed = exception.ToString();
          numTrials++;
        }
      }
      return sr;
    }
*/
    private void setStreamsFromYahoo(DateTime startDate, DateTime endDate)
    {
      int a = startDate.Month - 1;
      int b = startDate.Day;
      int c = startDate.Year;
      int d = endDate.Month - 1;
      int e = endDate.Day;
      int f = endDate.Year;
      HttpWebRequest Req;
      HttpWebResponse hwr;
      string url;
      url = "http:" + "//ichart.yahoo.com/table.csv?a="
            + a + "&b=" + b + "&c=" + c +"&d=" + d + "&e=" + e + "&f=" + f + "&s=" + p_quTicker + "&y=0&g=d&ignore=.csv";
      //url = "http:" + "//table.finance.yahoo.com/table.csv?a=" 
      	//	+ a + "&b=" + b + "&c=" + c +"&d=" + d + "&e=" + e + "&f=" + f + "&s=" + p_quTicker + "&y=0&g=d&ignore=.csv";
      int numTrials = 1;
      while(numTrials < 5)
      {
        try
        {
          Req = (HttpWebRequest)WebRequest.Create( url );
          Req.Method = "GET";
          Req.Timeout = ConstantsProvider.TimeOutValue;
          hwr = (HttpWebResponse)Req.GetResponse();
          this.stream = hwr.GetResponseStream();
          this.streamReader = new StreamReader(this.stream);
          numTrials = 6;
        }
        
        catch (Exception exception)
        {
          string notUsed = exception.ToString();
          numTrials++;
          if(numTrials==5)
            FaultyTickers.AddOrUpdate(this.p_quTicker, DateTime.Now.Date);
            // to add faulty tickers to database table
            //throw new Exception("It has not been possible to set streams from Yahoo: \n\n" +
            //                    "Check the connection to the internet or the following url: \n\n" +
            //                    url,exception);
        }
      }
    }


    private bool getResponseForTimeWindow( DateTime initialDateOfTheTimeWindow,
                                            int daysOfTheTimeWindow )
    {
      this.setStreamsFromYahoo(initialDateOfTheTimeWindow, daysOfTheTimeWindow);
      bool response = false;
      response = this.isAtLeastOneDateAvailable(this.streamReader,daysOfTheTimeWindow );
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
