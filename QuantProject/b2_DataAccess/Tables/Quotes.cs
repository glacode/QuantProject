using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using QuantProject.ADT;
using QuantProject.ADT.Histories;

namespace QuantProject.DataAccess.Tables
{
	/// <summary>
	/// Class to access the Quotes table
	/// </summary>
	public class Quotes
	{
       
    private DataTable quotes;
     
		// these static fields provide field name in the database table
		// They are intended to be used through intellisense when necessary
		public static string TickerFieldName = "quTicker";	// Ticker cannot be simply used because
																												// it is already used below
		public static string Date = "quDate";
		public static string Open = "quOpen";
		public static string High = "quHigh";
		public static string Low = "quLow";
		public static string Close = "quClose";
		public static string Volume = "quVolume";
		public static string AdjustedClose = "quAdjustedClose";
		public static string AdjustedCloseToCloseRatio = "quAdjustedCloseToCloseRatio";


		/// <summary>
		/// Gets the ticker whose quotes are contained into the Quotes object
		/// </summary>
		/// <returns></returns>
		public string Ticker
		{
			get{ return ((string)this.quotes.Rows[ 0 ][ "quTicker" ]); }
		}

		public Quotes( string ticker)
		{
			this.quotes = Quotes.GetTickerQuotes( ticker );
		}
		/// <summary>
		/// Creates quotes for the given instrument, since the startDate to the endDate
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		public Quotes( string ticker , DateTime startDate , DateTime endDate )
		{
			/// TO DO
		}
		/// <summary>
		/// Returns the first date for the given ticker
		/// </summary>
		/// <param name="ticker">ticker for which the starting date has to be returned</param>
		/// <returns></returns>
		public static DateTime GetStartDate( string ticker )
		{
			DataTable dataTable = SqlExecutor.GetDataTable(
				"select * from quotes where quTicker='" + ticker + "' " +
				"order by quDate");
			return (DateTime)(dataTable.Rows[ 0 ][ "quDate" ]);
		}
		/// <summary>
		/// Returns the last date for the given ticker
		/// </summary>
		/// <param name="ticker">ticker for which the lasat date has to be returned</param>
		/// <returns></returns>
		public static DateTime GetEndDate( string ticker )
		{
			DataTable dataTable = SqlExecutor.GetDataTable(
				"select * from quotes where quTicker='" + ticker + "' " +
				"order by quDate DESC");
			return (DateTime)(dataTable.Rows[0][ "quDate" ]);
		}
    /// <summary>
    /// Returns the number of quotes for the given ticker
    /// </summary>
    /// <param name="ticker">ticker for which the number of quotes has to be returned</param>
    /// <returns></returns>
    public static int GetNumberOfQuotes( string ticker )
    {
      DataTable dataTable = SqlExecutor.GetDataTable(
        "select * from quotes where quTicker='" + ticker + "'" );
      return dataTable.Rows.Count;
    }
    
    /// <summary>
    /// It provides updating the database for each closeToCloseRatio contained in the given table
    /// (the table refers to the ticker passed as the first parameter)
    /// </summary>
    private static void commitAllCloseToCloseRatios(string ticker,
                                                    DataTable tableContainingCloseToCloseRatios )
    {
      string notUsed = ticker;
      OleDbSingleTableAdapter adapter = 
                      new OleDbSingleTableAdapter("SELECT * FROM quotes WHERE 1=2",
                                                  tableContainingCloseToCloseRatios);
      adapter.OleDbDataAdapter.Update(tableContainingCloseToCloseRatios);
      
      /*
      foreach(DataRow row in tableContainingCloseToCloseRatios.Rows)
      {
        Quotes.UpdateCloseToCloseRatio(ticker, (DateTime)row[Quotes.Date],
                                       (double)row[Quotes.AdjustedCloseToCloseRatio]);
      }
      */
      
    }

    /// <summary>
    /// It provides computation of the adjustedCloseToCloseRatios for the given ticker
    /// </summary>
    public static void ComputeAndCommitCloseToCloseRatios( string ticker)
    {
      DateTime start = DateTime.Now;
      DataTable tickerQuotes = Quotes.GetTickerQuotes(ticker);
      DateTime tickerQuotesTime = DateTime.Now;
      Quotes.ComputeCloseToCloseValues(tickerQuotes);
      DateTime computation = DateTime.Now;
      Quotes.commitAllCloseToCloseRatios(ticker, tickerQuotes);
      DateTime commit = DateTime.Now;
      MessageBox.Show("start : " + start.ToString() + "\n" +
                      "loading quotes - finished : " + tickerQuotesTime.ToString() + "\n" +
                      "computation - finished: " + computation.ToString() + "\n" +
                      "commit - finished: " + commit.ToString());
    }
    
    /// <summary>
    /// It provides deletion of all quotes from the table "quotes" for
    /// the given ticker
    /// </summary>
    public static void Delete( string ticker)
    {
      try
      {
        SqlExecutor.ExecuteNonQuery("DELETE * FROM quotes " +
                                    "WHERE quTicker ='" +
                                    ticker + "'");
      }
      catch(Exception ex)
      {
        string notUsed = ex.ToString();
      }
    }
    /// <summary>
    /// It provides deletion of the quote from the table "quotes" for
    /// the given ticker for a specified date
    /// </summary>
    public static void Delete( string ticker, DateTime dateOfTheQuoteToBeDeleted )
    {
      try
      {
      SqlExecutor.ExecuteNonQuery("DELETE * FROM quotes " +
                                  "WHERE quTicker ='" +
                                  ticker + "' AND quDate =" +
                                  SQLBuilder.GetDateConstant(dateOfTheQuoteToBeDeleted));
      }
      catch(Exception ex)
      {
        string notUsed = ex.ToString();
      }
    }
    /// <summary>
    /// It provides deletion of the quote from the table "quotes" for
    /// the given ticker for the specified interval
    /// </summary>
    public static void Delete( string ticker, DateTime fromDate,
                                DateTime toDate)
    {
      try
      {
        SqlExecutor.ExecuteNonQuery("DELETE * FROM quotes " +
          "WHERE quTicker ='" +
          ticker + "' AND quDate >=" +
          SQLBuilder.GetDateConstant(fromDate) + " " +
          "AND quDate<=" + SQLBuilder.GetDateConstant(toDate));
      }
      catch(Exception ex)
      {
        string notUsed = ex.ToString();
      }
    }
    /// <summary>
    /// It provides addition of the given quote's values into table "quotes" 
    /// </summary>
    public static void Add( string ticker, DateTime date, double open, 
                            double high, double low, double close,
                            double volume, double adjustedClose)
    {
      try
      {
      SqlExecutor.ExecuteNonQuery("INSERT INTO quotes(quTicker, quDate, quOpen, " +
                                  "quHigh, quLow, quClose, quVolume, quAdjustedClose) " +
                                  "VALUES('" + ticker + "', " + SQLBuilder.GetDateConstant(date) + ", " +
                                  open + ", " + high + ", " + low + ", " + close + ", " +
                                  volume + ", " + adjustedClose + ")");
      }
      catch(Exception ex)
      {
        string notUsed = ex.ToString();
      }
    }             

    public static bool IsAdjustedCloseChanged(string ticker, DateTime dateToCheck,
                                              float currentAdjustedValueFromSource)
    {
      bool isAdjustedCloseChanged = false;
      try
      {
        float adjustedCloseInDatabase;
        double absoluteRelativeDifference;
          DataTable tableOfSingleRow = 
                    SqlExecutor.GetDataTable("SELECT * FROM quotes WHERE quTicker='" +
                                              ticker + "' AND quDate=" + 
                                              SQLBuilder.GetDateConstant(dateToCheck));
          adjustedCloseInDatabase = (float)(tableOfSingleRow.Rows[0]["quAdjustedClose"]);
          absoluteRelativeDifference = 
                            Math.Abs((currentAdjustedValueFromSource - adjustedCloseInDatabase)/currentAdjustedValueFromSource);
          if(absoluteRelativeDifference>ConstantsProvider.MaxRelativeDifferenceForAdjustedValues)
              isAdjustedCloseChanged = true;
        return isAdjustedCloseChanged;
      }
      catch(Exception ex)
      {
        string notUsed = ex.ToString();
        return isAdjustedCloseChanged;
      }
    }             
    
    #region IsAdjustedCloseToCloseRatioChanged

    private static bool isAtLeastOneValueChanged(DataTable tableDB, DataTable tableSource)
                                                          
    {
      try
      {
        int numRows = tableDB.Rows.Count;
        DateTime date;
        double adjCTCInDatabase;
        double adjCTCInSource;
        double absoluteRelativeDifference;
        DataColumn[] columnPrimaryKey = new DataColumn[0];
        columnPrimaryKey[0]= tableSource.Columns["quDate"];
        tableSource.PrimaryKey = columnPrimaryKey;
        DataRow rowToCheck;
        for(int i = 0;i != numRows;i++)
        {
          date = (DateTime)tableDB.Rows[i][Quotes.Date];
          adjCTCInDatabase = (double)tableDB.Rows[i][Quotes.AdjustedCloseToCloseRatio];
          rowToCheck = tableSource.Rows.Find(date);
          if(rowToCheck != null)
          {
            adjCTCInSource = (double)rowToCheck[Quotes.AdjustedCloseToCloseRatio];
            absoluteRelativeDifference = Math.Abs((adjCTCInDatabase - adjCTCInSource)/adjCTCInSource);
            if(absoluteRelativeDifference > ConstantsProvider.MaxRelativeDifferenceForCloseToCloseRatios )
               return true;
          }
        }
      }
      catch(Exception ex)
      {
        string notUsed = ex.ToString();
      }
      return false;

    }             


    public static void ComputeCloseToCloseValues(DataTable tableContainingNewAdjustedValues)
                                                          
    {
      try
      {
        if(!tableContainingNewAdjustedValues.Columns.Contains(Quotes.AdjustedCloseToCloseRatio))
        {
          tableContainingNewAdjustedValues.Columns.Add(Quotes.AdjustedCloseToCloseRatio,
                                                       System.Type.GetType("System.Double"));
        }
        int numRows = tableContainingNewAdjustedValues.Rows.Count;
        float previousClose;
        float currentClose;
        for(int i = 0;i != numRows;i++)
        {
          if(i == 0)
          //the first available quote has 0 as closeToClose ratio
          {
            tableContainingNewAdjustedValues.Rows[i][Quotes.AdjustedCloseToCloseRatio] = 0;
          }
          else
          {
            previousClose = (float)tableContainingNewAdjustedValues.Rows[i-1]["quAdjustedClose"];
            currentClose = (float)tableContainingNewAdjustedValues.Rows[i]["quAdjustedClose"];
            tableContainingNewAdjustedValues.Rows[i][Quotes.AdjustedCloseToCloseRatio] = 
                                                  (currentClose - previousClose)/previousClose; 
          }
        }

      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }

    }             

    /// <summary>
    /// It returns true if the adjustedCloseToCloseRatio computed
    /// with the adjusted values from the source is not equal to the ratio
    /// stored in the database 
    /// </summary>
    public static bool IsAdjustedCloseToCloseRatioChanged(string ticker,
                                                          DataTable tableContainingNewAdjustedValues)
                                                          
    {
      bool isAdjustedCloseToCloseRatioChanged = false;
      try
      {
        DataTable adjustedCloseToCloseFromDatabase = 
          SqlExecutor.GetDataTable("SELECT " + Quotes.Date + ", " +
                                   Quotes.AdjustedCloseToCloseRatio + " " +
                                   "FROM quotes WHERE " + Quotes.TickerFieldName + "='" + ticker +
                                   "' ORDER BY " + Quotes.Date);
         Quotes.ComputeCloseToCloseValues(tableContainingNewAdjustedValues);
         isAdjustedCloseToCloseRatioChanged = 
              Quotes.isAtLeastOneValueChanged(adjustedCloseToCloseFromDatabase, tableContainingNewAdjustedValues);
      }
      catch(Exception ex)
      {
        string notUsed = ex.ToString();
        return true;
      }
      
      return isAdjustedCloseToCloseRatioChanged;

    }             
    
    #endregion
    
    /// <summary>
    /// returns most liquid tickers with the given features
    /// </summary>

    public static DataTable GetMostLiquidTickers( string groupID,
                                                  DateTime firstQuoteDate,
                                                  DateTime lastQuoteDate,
                                                  long maxNumOfReturnedTickers)
    {
      string sql = "SELECT TOP " + maxNumOfReturnedTickers + " tickers.tiTicker, tickers.tiCompanyName, " +
                    "Avg([quVolume]*[quAdjustedClose]) AS AverageTradedValue " +
                    "FROM quotes INNER JOIN (tickers INNER JOIN tickers_tickerGroups " +
                    "ON tickers.tiTicker = tickers_tickerGroups.ttTiId) " +
                    "ON quotes.quTicker = tickers_tickerGroups.ttTiId " +
                    "WHERE tickers_tickerGroups.ttTgId='" + groupID + "' " +
                    "AND quotes.quDate BETWEEN " +
                    SQLBuilder.GetDateConstant(firstQuoteDate) + " AND " +
                    SQLBuilder.GetDateConstant(lastQuoteDate) + 
                    "GROUP BY tickers.tiTicker, tickers.tiCompanyName " +
                    "ORDER BY Avg([quVolume]*[quAdjustedClose]) DESC";

      return SqlExecutor.GetDataTable( sql );
    }
    

		#region GetHashValue
		private string getHashValue_getQuoteString_getRowString_getSingleValueString( Object value )
		{
			string returnValue;
			if ( value.GetType() == Type.GetType( "System.DateTime" ) )
				returnValue = ( (DateTime) value ).ToString();
			else
			{
				if ( value.GetType() == Type.GetType( "System.Double" ) )
					returnValue = ( (float) value ).ToString( "F2" );
				else
					returnValue = value.ToString();
			}

			return returnValue + ";";
		}
		/// <summary>
		/// Computes the string representing the concatenation for a single quote row
		/// </summary>
		/// <param name="dataRow"></param>
		/// <returns></returns>
		private StringBuilder getHashValue_getQuoteString_getRowString( DataRowView dataRow )
		{
			StringBuilder returnValue = new StringBuilder( "" );
			foreach ( DataColumn dataColumn in dataRow.DataView.Table.Columns )
				if ( dataColumn.ColumnName != "quTicker" )
					returnValue.Append( getHashValue_getQuoteString_getRowString_getSingleValueString(
						dataRow[ dataColumn.Ordinal ] ) );
			//					returnValue += "ggg";
			//					returnValue += getHashValue_getQuoteString_getRowString_getSingleValueString(
			//						dataRow[ dataColumn ] );
			return returnValue;
		}
		/// <summary>
		/// Computes the string representing the concatenation of all the quotes
		/// </summary>
		/// <param name="ticker"></param>
		/// <returns></returns>
		private string getHashValue_getQuoteString( DataView quotes )
		{
			StringBuilder returnValue = new StringBuilder( "" );
			foreach ( DataRowView dataRow in quotes )
				returnValue.Append( getHashValue_getQuoteString_getRowString( dataRow ) );
			return returnValue.ToString();
		}
		/// <summary>
		/// Computes the hash value for the contained quotes
		/// </summary>
		/// <returns>Hash value for all the quotes</returns>
		public string GetHashValue()
		{
			DataView quotes = new DataView( this.quotes );
			return HashProvider.GetHashValue( getHashValue_getQuoteString( quotes ) );
		}
		/// <summary>
		/// Computes the hash value for the contained quotes
		/// since startDate, to endDate
		/// </summary>
		/// <param name="startDate">date where hash begins being computed</param>
		/// <param name="endDate">date where hash ends being computed</param>
		/// <returns></returns>
		public string GetHashValue( DateTime startDate , DateTime endDate )
		{
			DataView quotes = new DataView( this.quotes );
			quotes.RowFilter = "( (quDate>=" + SQLBuilder.GetDateConstant( startDate ) +
				") and (quDate<=" + SQLBuilder.GetDateConstant( endDate ) + ") )";
			return HashProvider.GetHashValue( getHashValue_getQuoteString( quotes ) );
		}
		#endregion

		/// <summary>
		/// Computes the hash value for the quotes for the given ticker
		/// </summary>
		/// <param name="ticker">Ticker whose quotes must be hashed</param>
		/// <returns>Hash value for all the quotes for the given ticker</returns>
//		public static string GetHashValue( string ticker )
//		{
//			return HashProvider.GetHashValue( GetHashValue( GetTickerQuotes( ticker ) ) );
//		}
		
		/// <summary>
		/// returns the quotes DataTable for the given ticker
		/// </summary>
		/// <param name="instrumentKey">ticker whose quotes are to be returned</param>
		/// <returns></returns>
		public static DataTable GetTickerQuotes( string instrumentKey )
		{
			string sql = "select * from quotes where quTicker='" + instrumentKey + "' " +
				"order by quDate";
			return SqlExecutor.GetDataTable( sql );
		}
		/// <summary>
		/// Returns the quotes for the given instrument , since startDate to endDate
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public static void SetDataTable( string ticker , DateTime startDate , DateTime endDate ,
			DataTable dataTable)
		{
			string sql =
				"select * from quotes " +
				"where " + Quotes.TickerFieldName + "='" + ticker + "' " +
        "and " + Quotes.Date + ">=" + SQLBuilder.GetDateConstant( startDate ) +
        "and " + Quotes.Date + "<=" + SQLBuilder.GetDateConstant( endDate );
			SqlExecutor.SetDataTable( sql , dataTable );
		}

		/// <summary>
		/// returns the Date for the quote that is precedingDays before
		/// quoteDate
		/// </summary>
		/// <param name="quoteDate"></param>
		/// <param name="precedingDays"></param>
		/// <returns></returns>
		public DateTime GetPrecedingDate( DateTime quoteDate , int precedingDays )
		{
			History history = new History();
			history.Import( this.quotes , "quDate" , "quAdjustedClose" );
			return (DateTime) history.GetKey( Math.Max( 0 ,
				history.IndexOfKeyOrPrevious( quoteDate ) -
				precedingDays ) );
		}

		/// <summary>
		/// returns the Date for the quote that is followingDays after
		/// quoteDate
		/// </summary>
		/// <param name="quoteDate"></param>
		/// <param name="precedingDays"></param>
		/// <returns></returns>
		public DateTime GetFollowingDate( DateTime quoteDate , int followingDays )
		{
			History history = new History();
			history.Import( this.quotes , "quDate" , "quAdjustedClose" );
			return (DateTime) history.GetKey( Math.Max( 0 ,
				history.IndexOfKeyOrPrevious( quoteDate ) -
				followingDays ) );
		}
    

    /// <summary>
    /// Provides updating the database with the closeToCloseRatio for
    /// the given ticker at a specified date
    /// </summary>
    public static void UpdateCloseToCloseRatio( string ticker, DateTime date, double closeToCloseRatio )
    {
      string sql = "UPDATE quotes SET quotes.quAdjustedCloseToCloseRatio =" +
                    closeToCloseRatio + " WHERE quotes.quTicker='" + ticker + "' AND " +
                    "quotes.quDate=" + SQLBuilder.GetDateConstant(date);
      SqlExecutor.ExecuteNonQuery (sql);
    }

    /* Now useless, maybe ...
    /// <summary>
    /// Returns a DataTable containing ticker, date and adjusted value
    /// for the given group of tickers
    /// </summary>
    /// <param name="groupID">group for which the dataTable has to be returned</param>
    /// <returns></returns>
    public static DataTable GetTableWithAdjustedValues( string groupID )
    {
      string sql = "SELECT quotes.quTicker, quotes.quDate, quotes.quAdjustedClose, " +
        "tickers_tickerGroups.ttTgId FROM quotes INNER JOIN " + 
        "tickers_tickerGroups ON quotes.quTicker = tickers_tickerGroups.ttTiId " +
        "WHERE tickers_tickerGroups.ttTgId='" + groupID + "' " +
        "ORDER BY quotes.quTicker, quotes.quDate";
      return SqlExecutor.GetDataTable( sql );
    }
    */
	}
}
