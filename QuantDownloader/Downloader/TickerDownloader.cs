using System;
using System.Threading;
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
    private int numOfTickersToDownload;
    private bool checkBoxIsDicotomicSearchActivated;
    private bool isCheckCloseToCloseSelected;
    private bool isOverWriteSelected;
    private bool isComputeCloseToCloseRatioSelected;
    private bool isOnlyAfterLastQuoteSelected;
    private bool isOnlyAddMissingSelected;
    private bool isOverWriteYesSelected;
    private bool isSingleQuoteSelected;
    private DateTime selectedDateForSingleQuote;
    private string currentTicker;
    private DateTime INITIAL_DATE;
    private DateTime startDate;
    private DateTime endDate = DateTime.Today;
    private int endDay = DateTime.Now.Day;
    private int endMonth = DateTime.Now.Month;
    private int endYear = DateTime.Now.Year;
    private int numberOfQuotesInDatabase;
    private DataTable downloadedValuesFromSource;
    private OleDbSingleTableAdapter adapter;
    private Stream stream;
    private StreamReader streamReader;
    private HttpWebResponse httpWebResponse;
    
    
    private void tickerDownloader_copyPropertiesFromForm( WebDownloader myForm )
    {
    	this.startDate = myForm.StartingDate;
    	numOfTickersToDownload = this.p_myForm.DsTickerCurrentlyDownloaded.Tables["Tickers"].Rows.Count;
    	checkBoxIsDicotomicSearchActivated = myForm.checkBoxIsDicotomicSearchActivated.Checked;
    	isCheckCloseToCloseSelected = myForm.IsCheckCloseToCloseSelected;
      isOverWriteSelected = myForm.IsOverWriteYesSelected;
      isComputeCloseToCloseRatioSelected = myForm.IsComputeCloseToCloseRatioSelected;
      isOnlyAfterLastQuoteSelected = myForm.IsOnlyAfterLastQuoteSelected;
      isOnlyAddMissingSelected = myForm.IsOnlyAddMissingSelected;
      isOverWriteYesSelected = myForm.IsOverWriteYesSelected;
      isSingleQuoteSelected = myForm.IsSingleQuoteSelected;
      selectedDateForSingleQuote = myForm.SelectedDateForSingleQuote;
    }
    
    
    public TickerDownloader( WebDownloader myForm )
    {
      this.INITIAL_DATE = ConstantsProvider.InitialDateTimeForDownload;
    	p_myForm = myForm;
    	this.tickerDownloader_copyPropertiesFromForm( myForm );
      this.oleDbConnection1 = myForm.OleDbConnection1;
      this.downloadedValuesFromSource = new DataTable("quotes");
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
				p_myForm.CurrentStateForCurrentUpdatingTicker = newState;
				p_myForm.Invalidate();
//        string columnName = p_myForm.DsTickerCurrentlyDownloaded.Tables["Tickers"].Columns[0].ColumnName;
//        DataRow[] myRows = p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Select( columnName + "='" + p_quTicker + "'" );
//        myRows[ 0 ][ "currentState" ] = newState;
//        p_myForm.dataGrid1.Refresh();
      }
    }
    
    private void updateCurrentStatusAdjustedClose( string status )
    {
      lock( p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ] )
      {
      	p_myForm.AdjustedCloseInfoForCurrentUpdatingTicker = status;
      	p_myForm.Invalidate();
//        string columnName = p_myForm.DsTickerCurrentlyDownloaded.Tables["Tickers"].Columns[0].ColumnName;
//        DataRow[] myRows = p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Select( columnName + "='" + p_quTicker + "'" );
//        myRows[ 0 ][ "adjustedClose" ] = status;
//        p_myForm.dataGrid1.Refresh();
      }
    }

    private void updateCurrentStatusAdjustedCloseToCloseRatio( string status )
    {
      lock( p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ] )
      {
      	p_myForm.AdjCloseToCloseRatioInfoForCurrentUpdatingTicker = status;
      	p_myForm.Invalidate();
//        string columnName = p_myForm.DsTickerCurrentlyDownloaded.Tables["Tickers"].Columns[0].ColumnName;
//        DataRow[] myRows = p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Select(columnName + "='" + p_quTicker + "'" );
//        myRows[ 0 ][ "adjCloseToCloseRatio" ] = status;
//        p_myForm.dataGrid1.Refresh();
      }
    }

    private void updateCurrentLastQuoteInDB(string status )
    {
      lock( p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ] )
      {
      	p_myForm.LastQuoteInDBForCurrentUpdatingTicker = status;
      	p_myForm.Invalidate();
      }
    }
    
    private void updateCurrentStatusDatabaseUpdated(string status )
    {
      lock( p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ] )
      {
      	p_myForm.DatabaseUpdatedInfoForCurrentUpdatingTicker = status;
      	p_myForm.Invalidate();
//        string columnName = p_myForm.DsTickerCurrentlyDownloaded.Tables["Tickers"].Columns[0].ColumnName;
//        DataRow[] myRows = p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Select(columnName +  "='" + p_quTicker + "'" );
//        myRows[ 0 ][ "databaseUpdated" ] = status;
//        p_myForm.dataGrid1.Refresh();
      }
    }

#endregion

    private void addTickerToFaultyTickers()
    {
      System.Data.OleDb.OleDbCommand odc = new System.Data.OleDb.OleDbCommand();
      odc.CommandText = "insert into faultyTickers ( ftTicker , ftDateTime ) " +
        "values ( '" + currentTicker + "' , #" +
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

        myRow[ "quTicker" ] = this.currentTicker;
        myRow[ "quDate" ]=DateTime.Parse( LineIn[0] );
        myRow[ "quOpen" ]=Double.Parse( LineIn[1] );
        myRow[ "quHigh" ]=Double.Parse( LineIn[2] );
        myRow[ "quLow" ]=Double.Parse( LineIn[3] );
        myRow[ "quClose" ]=Double.Parse( LineIn[4] );
        myRow[ "quVolume" ]=Math.Min(Double.Parse( LineIn[5]),Convert.ToDouble(Int32.MaxValue));
        myRow[ "quAdjustedClose" ]=Double.Parse( LineIn[6] );
			  
        this.downloadedValuesFromSource.Rows.Add(myRow);

        Line = streamReader.ReadLine();
      }
    }


	  private void importTickerForCurrentTimeFrame( DateTime currBeginDate , DateTime currEndDate )
    {
        try
        {
          this.setStreamsFromYahoo(currBeginDate, currEndDate);
          this.addCurrentStreamToTable();
          this.updateCurrentStatus(currEndDate.ToShortDateString());
          if(this.streamReader!=null)
          {	
            this.streamReader.Close();
            this.stream.Close();
            this.httpWebResponse.Close();
          }  
          else
            return;
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
      if(currBeginDate == endDate)
        //just a single quote requested: initial date is 10 day before end date
        //just for close to close direct computation
      {
        this.importTickerForCurrentTimeFrame ( currBeginDate.AddDays(-10) , endDate );
        int rowsDownloaded = this.downloadedValuesFromSource.Rows.Count;
        for(int i = rowsDownloaded-1;i>1 ;i--)
        //for the computation of ctc ratio it is necessary to have previous quote
          this.downloadedValuesFromSource.Rows.RemoveAt(i);
        return this.downloadedValuesFromSource;
      }
      else
      {
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
    }
    
    private DataTable getTableOfDownloadedValues(DateTime newStartDate, DateTime newEndDate)
    {
      this.startDate = newStartDate;
      this.endDate = newEndDate;
      return this.getTableOfDownloadedValues();
    }

//    private void addTickerTo_gridDataSet()
//    {
//      
//    	DataRow newRow = p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].NewRow();
//      newRow[ 0 ] = p_quTicker;
//      newRow[ "currentState" ] = "Searching ...";
//      newRow[ "databaseUpdated" ] = "No";
//      newRow[ "adjustedClose"] = "...";
//      newRow[ "adjCloseToCloseRatio"] = "...";
//      try
//      {
//        p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Rows.Add( newRow );
//        p_myForm.labelNumberOfTickersToDownload.Text = 
//                      Convert.ToString(Convert.ToInt16(p_myForm.labelNumberOfTickersToDownload.Text) - 1);
//      } 
//      catch (Exception ex)
//      {
//        string notUsed = ex.ToString();
//        //MessageBox.Show( ex.ToString() );
//      }
//      p_myForm.dataGrid1.Refresh();
//    }

    private void resetStartDateIfNecessary()
    {
      if(this.checkBoxIsDicotomicSearchActivated  == true) 
        this.startDate = firstAvailableDateOnYahoo(this.INITIAL_DATE, this.endDate);
    }

    private void downloadTickers_downloadTickerBeforeFirstQuote()
    {
      this.endDate = Quotes.GetFirstQuoteDate(this.currentTicker);
      this.resetStartDateIfNecessary();
      this.checkForNewAdjustedAndContinueOrStop();
    }
    
    private bool isAdjustedCloseChanged(int numberOfRepeatedChecks)
    {
      bool response = false;
      
      QuantProject.Data.DataTables.Quotes tickerQuotes =
				new QuantProject.Data.DataTables.Quotes(this.currentTicker);
//      for(
      	int i = 1;
//          i < this.numberOfQuotesInDatabase && response == false;
//          i += this.numberOfQuotesInDatabase/numberOfRepeatedChecks)
      {
        DateTime dateToCheck = tickerQuotes.GetPrecedingDate(this.startDate, i);
        float adjustedCloseFromSource = float.MaxValue;
        try{
					adjustedCloseFromSource = this.getAdjustedCloseFromSource(dateToCheck);
					this.httpWebResponse.Close();
        }
        catch(Exception ex){
        	string notUsed = ex.ToString();
        }
        	response =
              Quotes.IsAdjustedCloseChanged( this.currentTicker, dateToCheck,
                                             adjustedCloseFromSource );
      }
      
      return response;
    }
    
    private void checkForNewAdjustedAndContinueOrStop_dummy()
    {
    	System.Threading.Thread.Sleep(500);
    	this.updateCurrentStatusAdjustedCloseToCloseRatio("OK");
      this.updateCurrentStatusAdjustedClose("Updated!");
      this.updateCurrentStatusAdjustedClose("OK");
      this.updateCurrentStatus("Yes");
    }
    private void checkForNewAdjustedAndContinueOrStop_writeNoQuoteFoundMessageIfTheCase()
    {
    	if(this.downloadedValuesFromSource.Rows.Count == 0)
         	this.updateCurrentStatus("No quote found");
    }
    
    private void checkForNewAdjustedAndContinueOrStop_downloadNewQuotesAndCommit()
    {
    	DateTime lastQuoteDateInDB = Quotes.GetLastQuoteDate(this.currentTicker);
    	this.updateCurrentLastQuoteInDB(lastQuoteDateInDB.ToShortDateString());
    	this.downloadedValuesFromSource =
         this.getTableOfDownloadedValues( lastQuoteDateInDB ,
                                          DateTime.Now );
      this.checkForNewAdjustedAndContinueOrStop_writeNoQuoteFoundMessageIfTheCase();
      this.commitDownloadedValuesToDatabase();
    }
    
    private void checkForNewAdjustedAndContinueOrStop()
    {
      try
      {
        bool adjCloseToCloseRatioChanged = false;
        if ( this.isCheckCloseToCloseSelected )
        	adjCloseToCloseRatioChanged = Quotes.IsAdjustedCloseToCloseRatioChanged( this.currentTicker,
              													this.getTableOfDownloadedValues(Quotes.GetFirstQuoteDate(this.currentTicker),
                                               Quotes.GetLastQuoteDate(this.currentTicker)));
       	if(this.isAdjustedCloseChanged(ConstantsProvider.NumberOfCheckToPerformOnAdjustedValues))
        {
          this.updateCurrentStatusAdjustedClose("Changed!");
          if( !this.isCheckCloseToCloseSelected )
          //it is necessary to download values first for past adjusted values	to be updated
          	this.downloadedValuesFromSource = this.getTableOfDownloadedValues(Quotes.GetFirstQuoteDate(this.currentTicker),
                                               Quotes.GetLastQuoteDate(this.currentTicker));
          //else values have been already downloaded for checking adjCloseToCloseRatios
          if (this.isCheckCloseToCloseSelected && adjCloseToCloseRatioChanged )
          {
             this.updateCurrentStatusAdjustedCloseToCloseRatio("Changed at " + 
                                                                  Quotes.DateWithDifferentCloseToClose);
             //stop downloading ticker if the close to close ratio has changed
          }
          else if ( this.isCheckCloseToCloseSelected && !adjCloseToCloseRatioChanged )
          //adjusted close changed, but the close to close ratio
          // has not been changed
          {
             this.updateCurrentStatusAdjustedCloseToCloseRatio("OK");
             this.updateAdjustedClose();
             this.checkForNewAdjustedAndContinueOrStop_downloadNewQuotesAndCommit();
          }
          else if ( !isCheckCloseToCloseSelected )
          {
             this.updateCurrentStatusAdjustedCloseToCloseRatio("Not Checked");
             this.updateAdjustedClose();
             this.checkForNewAdjustedAndContinueOrStop_downloadNewQuotesAndCommit();
          }     
        }
        else//adjusted close has not been changed
        //download is executed directly
        {
          this.updateCurrentStatusAdjustedClose("Not changed");
          this.checkForNewAdjustedAndContinueOrStop_downloadNewQuotesAndCommit();
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
        Quotes.UpdateAdjustedClose(this.currentTicker, (DateTime)row[Quotes.Date], (float)row[Quotes.AdjustedClose]);
      }
      this.updateCurrentStatusAdjustedClose("Updated");
    }
    
    private void downloadTickers_downloadTickerForTheSelectedDate(DateTime date)
    {
      this.startDate = date;
      this.endDate = date;
      this.checkForNewAdjustedAndContinueOrStop();
    }
    
    private void downloadTickers_downloadTickerAfterLastQuote()
    {
      this.startDate = Quotes.GetLastQuoteDate(this.currentTicker);
      this.updateCurrentLastQuoteInDB(this.startDate.ToShortDateString());
      this.endDate = DateTime.Today;
      //this.checkForNewAdjustedAndContinueOrStop_dummy();
//      this.resetStartDateIfNecessary();
//      this.downloadedValuesFromSource = this.getTableOfDownloadedValues();
//      this.commitDownloadedValuesToDatabase();
      this.checkForNewAdjustedAndContinueOrStop();
    }
    
    private float getAdjustedCloseFromSource(DateTime adjustedCloseDate)
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
      if(this.isOverWriteYesSelected)
      {
        foreach(DataRow row in this.downloadedValuesFromSource.Rows)
          Quotes.Delete(this.currentTicker, (DateTime)row["quDate"]);
      }
      if(this.isComputeCloseToCloseRatioSelected)
        QuantProject.DataAccess.Tables.Quotes.ComputeCloseToCloseValues(this.downloadedValuesFromSource);
      this.adapter.OleDbDataAdapter.ContinueUpdateOnError = true;
      int rowsUpdated = this.adapter.OleDbDataAdapter.Update(this.downloadedValuesFromSource);
      if(rowsUpdated > 0)
      	this.updateCurrentStatusDatabaseUpdated("Added " + rowsUpdated.ToString() + " rows");
      else
        this.updateCurrentStatusDatabaseUpdated("No row added");
    }

    private void downloadTickers_refreshIndexOfCurrentTicker( int indexOfCurrentUpdatingTicker )
    {
    	lock(p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ])
    	{
    		p_myForm.IndexOfCurrentUpdatingTicker = indexOfCurrentUpdatingTicker;
    	}
    }
    
    private void downloadTickers_refreshCurrentTicker( int indexOfCurrentUpdatingTicker )
    {
    	lock(p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ])
    	{
    		this.currentTicker = 
    			(string)p_myForm.DsTickerCurrentlyDownloaded.Tables[ "Tickers" ].Rows[ indexOfCurrentUpdatingTicker ][0];
    	}
    }
    
    private void downloadTickers_resetAndImportTicker()
    {
      this.resetStartDateIfNecessary();
      this.downloadedValuesFromSource = this.getTableOfDownloadedValues();
      this.commitDownloadedValuesToDatabase();
    }
    
    private void downloadTickers_setInfoCurrentStatusToDefault()
    {
    	this.updateCurrentLastQuoteInDB("...");
    	this.updateCurrentStatus("...");
   		this.updateCurrentStatusDatabaseUpdated("...");
   		this.updateCurrentStatusAdjustedClose("...");
   		this.updateCurrentStatusAdjustedCloseToCloseRatio("...");
   	}
    
    
    public void DownloadTickers()
    {
    	for( int i = 0; i < this.numOfTickersToDownload; i++ )
    	{
    		this.downloadTickers_setInfoCurrentStatusToDefault();
    		this.downloadTickers_refreshCurrentTicker( i );
    		this.downloadTickers_refreshIndexOfCurrentTicker( i );
	    	this.numberOfQuotesInDatabase = Quotes.GetNumberOfQuotes(this.currentTicker);
	      if(this.numberOfQuotesInDatabase < 1)
	      // ticker's quotes are downloaded for the first time 
	      {
	        this.downloadTickers_resetAndImportTicker();
	      }
	      // in all these cases some ticker's quotes are in the database
	      // and the options choosen by the user in the web downloader form are checked
	      else if(this.isOnlyAfterLastQuoteSelected && 
	              this.numberOfQuotesInDatabase >= 1)
	      {
	        this.downloadTickers_downloadTickerAfterLastQuote();
	      }
	      else if(this.isOnlyAddMissingSelected &&
	              this.numberOfQuotesInDatabase >= 1)
	      {
	        this.resetStartDateIfNecessary(); 
	        this.checkForNewAdjustedAndContinueOrStop();
	      }
	      else if(this.isOverWriteYesSelected &&
	        this.numberOfQuotesInDatabase >= 1)
	      {
	        this.downloadTickers_resetAndImportTicker();
	      }
	      else if(this.isSingleQuoteSelected)
	      {
	        if(Quotes.GetTickerQuotes(this.currentTicker,
	      	   				this.selectedDateForSingleQuote,
	      	   				this.selectedDateForSingleQuote).Rows.Count == 0)
	        //there's no quote for the ticker at the given date
	      		this.downloadTickers_downloadTickerForTheSelectedDate(this.p_myForm.SelectedDateForSingleQuote);
	      }
	      p_myForm.Invalidate();
	      Thread.Sleep(200);
	   	}
    	this.p_myForm.DownloadingInProgress = false;
    }
    
    private bool isAtLeastOneDateAvailable(StreamReader streamReader, int daysToBeTested)
    {
      bool returnValue = false;
      string Line;
      try{
	      Line = streamReader.ReadLine();
	      // column headers are read
	      Line = streamReader.ReadLine();
	      //actual values are read
	      
	      int numDays = 1;
	      while(numDays < daysToBeTested)
	      {
	        if(Line != null)
	        {
	          returnValue = true;
	          numDays = daysToBeTested + 1;
	        }
	        numDays++;
      	}
      }
      catch (Exception exception)
      {
          string notUsed = exception.ToString();
      }    
      return returnValue;
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
      string url;
      url = "http:" + "//ichart.yahoo.com/table.csv?a="
            + a + "&b=" + b + "&c=" + c +"&d=" + d + "&e=" + e + "&f=" + f + "&s=" + currentTicker + "&y=0&g=d&ignore=.csv";
//      url = "http:" + "//table.finance.yahoo.com/table.csv?a=" 
//      		+ a + "&b=" + b + "&c=" + c +"&d=" + d + "&e=" + e + "&f=" + f + "&s=" + currentTicker + "&y=0&g=d&ignore=.csv";
      int numTrials = 1;
      int maxNumTrials = 3;
      while( numTrials < maxNumTrials )
      {
        try
        {
          Req = (HttpWebRequest)WebRequest.Create( url );
          Req.Method = "GET";
          Req.Timeout = ConstantsProvider.TimeOutValue;
          this.httpWebResponse = (HttpWebResponse)Req.GetResponse();
          this.stream = this.httpWebResponse.GetResponseStream();
          this.streamReader = new StreamReader(this.stream);
          numTrials = maxNumTrials;
        }
        catch (Exception exception)
        {
          string notUsed = exception.ToString();
          numTrials++;
          if( numTrials == maxNumTrials )
            FaultyTickers.AddOrUpdate(this.currentTicker, DateTime.Now.Date);
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
      if( response == true )
      	this.updateCurrentStatus("Found quotes from " + initialDateOfTheTimeWindow.ToShortDateString() +
                               " to " + initialDateOfTheTimeWindow.AddDays(daysOfTheTimeWindow).ToShortDateString() );
      else // response == false
      	this.updateCurrentStatus("No quote found");
      
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
