using System;
using System.Collections;
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

    public static DateTime DateWithDifferentCloseToClose = new DateTime(1900,1,1);


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
		public static DateTime GetFirstQuoteDate( string ticker )
		{
			DataTable dataTable = SqlExecutor.GetDataTable(
				"select min(quDate) as minDate from quotes where quTicker='" + ticker + "' " +
				"group by quTicker");
			return (DateTime)(dataTable.Rows[ 0 ][ "minDate" ]);
		}
		/// <summary>
		/// Returns the last date for the given ticker
		/// </summary>
		/// <param name="ticker">ticker for which the lasat date has to be returned</param>
		/// <returns></returns>
		public static DateTime GetLastQuoteDate( string ticker )
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
    /// Returns the number of days at which the given ticker has beeb effectively traded
    /// </summary>
    /// <param name="ticker">ticker for which the number of days has to be returned</param>
    /// <returns></returns>
    public static int GetNumberOfDaysWithEffectiveTrades( string ticker, DateTime firstDate,
                                                          DateTime lastDate)
    {
      DataTable dataTable = SqlExecutor.GetDataTable(
        "select * from quotes WHERE quTicker='" + ticker + "'" +
        " AND " + "quVolume>0" + " AND quDate BETWEEN " + SQLBuilder.GetDateConstant(firstDate) + 
        " AND " + SQLBuilder.GetDateConstant(lastDate));
      return dataTable.Rows.Count;
    }

    /// <summary>
    /// Returns the adjusted close value for the given ticker at the specified date
    /// is returned
    /// </summary>
    /// <param name="ticker">ticker for which the adj close has to be returned</param>
    /// <returns></returns>
    public static float GetAdjustedClose( string ticker, DateTime date )
    {
      DataTable dataTable = SqlExecutor.GetDataTable(
        "select quAdjustedClose from quotes where quTicker='" + ticker + "' " +
        "and quDate=" + SQLBuilder.GetDateConstant(date) );
      return (float)dataTable.Rows[0][0];
    }
    /// <summary>
    /// Returns the raw (not adjusted) close for the given ticker at the specified date
    /// is returned
    /// </summary>
    /// <param name="ticker">ticker for which the raw close has to be returned</param>
    /// <returns></returns>
    public static float GetRawClose( string ticker, DateTime date )
    {
      DataTable dataTable = SqlExecutor.GetDataTable(
        "select quClose from quotes where quTicker='" + ticker + "' " +
        "and quDate=" + SQLBuilder.GetDateConstant(date) );
      return (float)dataTable.Rows[0][0];
    }
    /// <summary>
    /// Returns the raw (not adjusted) open for the given ticker at the specified date
    /// is returned
    /// </summary>
    /// <param name="ticker">ticker for which the raw open has to be returned</param>
    /// <returns></returns>
    public static float GetRawOpen( string ticker, DateTime date )
    {
      DataTable dataTable = SqlExecutor.GetDataTable(
        "select quOpen from quotes where quTicker='" + ticker + "' " +
        "and quDate=" + SQLBuilder.GetDateConstant(date) );
      return (float)dataTable.Rows[0][0];
    }
/* moved now to the quotes object in the data layer, where the names are slightly different
      
    /// <summary>
    /// Returns true if a quote is available for the given ticker at the given date
    /// </summary>
    /// <param name="ticker">ticker for which the check has to be done</param>
    /// <param name="date">date of the check</param>
    /// <returns></returns>
    public static bool IsQuoteAvailable( string ticker, DateTime date )
    {
      DataTable dataTable = SqlExecutor.GetDataTable(
        "select quAdjustedClose from quotes where quTicker='" + ticker + "' " +
        "and quDate=" + SQLBuilder.GetDateConstant(date) );
      string booleanStringValue = "false";
      if(dataTable.Rows.Count>0)
        booleanStringValue = "True";
      return Boolean.Parse(booleanStringValue);
    }

    public static DateTime GetFollowingValidQuoteDate( string ticker, DateTime date )
    {
      if(Quotes.IsQuoteAvailable(ticker, date))
      {
        return date;
      }
      else return GetFollowingValidQuoteDate(ticker, date.AddDays(1));
    }
    
    public static DateTime GetPrecedingValidQuoteDate( string ticker, DateTime date )
    {
      if(Quotes.IsQuoteAvailable(ticker, date))
      {
        return date;
      }
      else return GetPrecedingValidQuoteDate(ticker, date.Subtract(new TimeSpan(1,0,0)));
    }
*/
    /// <summary>
    /// It provides updating the database for each closeToCloseRatio contained in the given table
    /// (the table refers to the ticker passed as the first parameter)
    /// </summary>
    private static void commitAllCloseToCloseRatios(DataTable tableContainingCloseToCloseRatios )
    {
      OleDbSingleTableAdapter adapter = 
                      new OleDbSingleTableAdapter("SELECT * FROM quotes WHERE 1=2",
                                                  tableContainingCloseToCloseRatios);
      adapter.OleDbDataAdapter.ContinueUpdateOnError = true;
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
    /// It provides computation and commiting to database
    /// of the adjustedCloseToCloseRatios for the given ticker
    /// </summary>
    public static void ComputeAndCommitCloseToCloseRatios( string ticker)
    {
      DataTable tickerQuotes = Quotes.GetTickerQuotes(ticker);
      Quotes.ComputeCloseToCloseValues(tickerQuotes);
      Quotes.commitAllCloseToCloseRatios(tickerQuotes);
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
        double absoluteDifference;
          DataTable tableOfSingleRow = 
                    SqlExecutor.GetDataTable("SELECT * FROM quotes WHERE quTicker='" +
                                              ticker + "' AND quDate=" + 
                                              SQLBuilder.GetDateConstant(dateToCheck));
          adjustedCloseInDatabase = (float)(tableOfSingleRow.Rows[0]["quAdjustedClose"]);
          absoluteDifference = Math.Abs(currentAdjustedValueFromSource - adjustedCloseInDatabase);
          if(absoluteDifference>ConstantsProvider.MaxDifferenceForAdjustedValues)
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
    
    private static void isAtLeastOneValueChanged_setPrimaryKey(DataTable tableToBeSet)
    {
      DataColumn[] columnPrimaryKeys = new DataColumn[1];
      columnPrimaryKeys[0] = tableToBeSet.Columns[Quotes.Date];
      tableToBeSet.PrimaryKey = columnPrimaryKeys;
    }

    private static bool isAtLeastOneValueChanged(DataTable tableDB, DataTable tableSource)
                                                          
    {
      bool returnValue = false;
      int numRows = tableDB.Rows.Count;
      DateTime date;
      float adjCTCInDatabase, adjCTCInSource;
      double absoluteDifference;
      DataRow rowToCheck;
      for(int i = 0;i != numRows;i++)
      {
        date = (DateTime)tableDB.Rows[i][Quotes.Date];
        adjCTCInDatabase = (float)tableDB.Rows[i][Quotes.AdjustedCloseToCloseRatio];
        isAtLeastOneValueChanged_setPrimaryKey(tableSource);
        rowToCheck = tableSource.Rows.Find(date);
        if(rowToCheck != null)
        {
          adjCTCInSource = (float)rowToCheck[Quotes.AdjustedCloseToCloseRatio];
          absoluteDifference = Math.Abs(adjCTCInDatabase - adjCTCInSource);
          if(absoluteDifference > ConstantsProvider.MaxDifferenceForCloseToCloseRatios )
            {
              Quotes.DateWithDifferentCloseToClose = date;
              returnValue = true;
            }
        }
      }
      return returnValue;
    }             


    public static void ComputeCloseToCloseValues(DataTable tableOfAllQuotesOfAGivenTicker)
    {
      //DataColumn[] columnPrimaryKey = new DataColumn[1];
      //columnPrimaryKey[0]= tableOfAllQuotesOfAGivenTicker.Columns[Quotes.Date];
      //tableOfAllQuotesOfAGivenTicker.PrimaryKey = columnPrimaryKey;
      
      int numRows = tableOfAllQuotesOfAGivenTicker.Rows.Count;
      DataView orderedByDate = new DataView(tableOfAllQuotesOfAGivenTicker,
                                          Quotes.AdjustedClose + ">=0",
                                          Quotes.Date + " ASC", DataViewRowState.CurrentRows);
      float previousClose;
      float currentClose;
      DateTime date;
      DataRow rowToBeChanged;
      DataRow[] foundRows;
      for(int i = 0;i != numRows;i++)
      {
        date = (DateTime)orderedByDate[i].Row[Quotes.Date];
        foundRows = tableOfAllQuotesOfAGivenTicker.Select(Quotes.Date + "=" +
                                                          SQLBuilder.GetDateConstant(date));
        rowToBeChanged = foundRows[0];
        if(i == 0)
        //the first available quote ... 
        
        {
          // ... has no closeToClose valid ratio
          rowToBeChanged[Quotes.AdjustedCloseToCloseRatio] = 0;
        }
        else
        {
          //the other quotes have a previous and a current close
          previousClose = (float)orderedByDate[i-1].Row[Quotes.AdjustedClose];
          currentClose = (float)orderedByDate[i].Row[Quotes.AdjustedClose];
        
          if(previousClose != 0)
          // if the previouse adj close is not 0 the CTC value has to be computed
              rowToBeChanged[Quotes.AdjustedCloseToCloseRatio] = currentClose/previousClose;
       }
       
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
    /// returns tickers ordered by a liquidity index
    /// </summary>
    public static DataTable GetTickersByLiquidity( bool orderInASCMode, string groupID,
                                                  DateTime firstQuoteDate,
                                                  DateTime lastQuoteDate,
                                                  long maxNumOfReturnedTickers)
    {
      string sql = "SELECT TOP " + maxNumOfReturnedTickers + " tickers.tiTicker, tickers.tiCompanyName, " +
                    "Avg([quVolume]*[quClose]) AS AverageTradedValue " +
                    "FROM quotes INNER JOIN (tickers INNER JOIN tickers_tickerGroups " +
                    "ON tickers.tiTicker = tickers_tickerGroups.ttTiId) " +
                    "ON quotes.quTicker = tickers_tickerGroups.ttTiId " +
                    "WHERE tickers_tickerGroups.ttTgId='" + groupID + "' " +
                    "AND quotes.quDate BETWEEN " +
                    SQLBuilder.GetDateConstant(firstQuoteDate) + " AND " +
                    SQLBuilder.GetDateConstant(lastQuoteDate) + 
                    "GROUP BY tickers.tiTicker, tickers.tiCompanyName " +
                    "ORDER BY Avg([quVolume]*[quClose])";
      string sortDirection = " DESC";
      if(orderInASCMode)
        sortDirection = " ASC";
      sql = sql + sortDirection;
      return SqlExecutor.GetDataTable( sql );
    }

		/// <summary>
		/// returns tickers ordered by liquidity, with a specified min volume
		/// </summary>
		/// <param name="orderInASCMode">true iff return must be ordered</param>
		/// <param name="groupID"></param>
		/// <param name="firstQuoteDate"></param>
		/// <param name="lastQuoteDate"></param>
		/// <param name="maxNumOfReturnedTickers"></param>
		/// <param name="minVolume"></param>
		/// <returns></returns>
		public static DataTable GetTickersByLiquidity( bool orderInASCMode, string groupID,
			DateTime firstQuoteDate,
			DateTime lastQuoteDate,
			long minVolume,
			long maxNumOfReturnedTickers
			)
		{
			string sql = "SELECT TOP " + maxNumOfReturnedTickers + " tickers.tiTicker, tickers.tiCompanyName, " +
				"Avg([quVolume]*[quClose]) AS AverageTradedValue " +
				"FROM quotes INNER JOIN (tickers INNER JOIN tickers_tickerGroups " +
				"ON tickers.tiTicker = tickers_tickerGroups.ttTiId) " +
				"ON quotes.quTicker = tickers_tickerGroups.ttTiId " +
				"WHERE tickers_tickerGroups.ttTgId='" + groupID + "' " +
				"AND quotes.quDate BETWEEN " +
				SQLBuilder.GetDateConstant(firstQuoteDate) + " AND " +
				SQLBuilder.GetDateConstant(lastQuoteDate) + 
				"GROUP BY tickers.tiTicker, tickers.tiCompanyName " +
				"HAVING Avg([quVolume])>=" + minVolume.ToString() + " " +
				"ORDER BY Avg([quVolume])";
			string sortDirection = " DESC";
			if(orderInASCMode)
				sortDirection = " ASC";
			sql = sql + sortDirection;
			return SqlExecutor.GetDataTable( sql );
		}

    

    /// <summary>
    /// Returns tickers ordered by a close to close volatility index (stdDev of adjustedCloseToClose ratio)
    /// </summary>
    public static DataTable GetTickersByCloseToCloseVolatility( bool orderInASCMode, string groupID,
                                                    DateTime firstQuoteDate,
                                                    DateTime lastQuoteDate,
                                                    long maxNumOfReturnedTickers)
    {
      string sql = "SELECT TOP " + maxNumOfReturnedTickers + " tickers.tiTicker, tickers.tiCompanyName, " +
        "StDev(quotes.quAdjustedCloseToCloseRatio) AS AdjCloseToCloseStandDev " +
        "FROM quotes INNER JOIN (tickers INNER JOIN tickers_tickerGroups " +
        "ON tickers.tiTicker = tickers_tickerGroups.ttTiId) " +
        "ON quotes.quTicker = tickers_tickerGroups.ttTiId " +
        "WHERE tickers_tickerGroups.ttTgId='" + groupID + "' " +
        "AND quotes.quDate BETWEEN " +
        SQLBuilder.GetDateConstant(firstQuoteDate) + " AND " +
        SQLBuilder.GetDateConstant(lastQuoteDate) + 
        "GROUP BY tickers.tiTicker, tickers.tiCompanyName " +
        "ORDER BY StDev(quotes.quAdjustedCloseToCloseRatio)";
      string sortDirection = " DESC";
      if(orderInASCMode)
        sortDirection = " ASC";
      sql = sql + sortDirection;
      return SqlExecutor.GetDataTable( sql );
    }

    /// <summary>
    /// Returns tickers ordered by the open to close volatility index (stdDev of OTC ratio)
    /// </summary>
    public static DataTable GetTickersByOpenToCloseVolatility( bool orderInASCMode, string groupID,
      DateTime firstQuoteDate,
      DateTime lastQuoteDate,
      long maxNumOfReturnedTickers)
    {
      string sql = "SELECT TOP " + maxNumOfReturnedTickers + " tickers.tiTicker, tickers.tiCompanyName, " +
        "StDev(quotes.quClose/quotes.quOpen - 1) AS OpenToCloseStandDev " +
        "FROM quotes INNER JOIN (tickers INNER JOIN tickers_tickerGroups " +
        "ON tickers.tiTicker = tickers_tickerGroups.ttTiId) " +
        "ON quotes.quTicker = tickers_tickerGroups.ttTiId " +
        "WHERE tickers_tickerGroups.ttTgId='" + groupID + "' " +
        "AND quotes.quDate BETWEEN " +
        SQLBuilder.GetDateConstant(firstQuoteDate) + " AND " +
        SQLBuilder.GetDateConstant(lastQuoteDate) + 
        "GROUP BY tickers.tiTicker, tickers.tiCompanyName " +
        "ORDER BY StDev(quotes.quClose/quotes.quOpen - 1)";
      string sortDirection = " DESC";
      if(orderInASCMode)
        sortDirection = " ASC";
      sql = sql + sortDirection;
      return SqlExecutor.GetDataTable( sql );
    }

    /// <summary>
    /// Returns tickers ordered by average close to close performance 
    /// </summary>
    public static DataTable GetTickersByAverageCloseToClosePerformance( bool orderInASCMode, string groupID,
                                                          DateTime firstQuoteDate,
                                                          DateTime lastQuoteDate,
                                                          long maxNumOfReturnedTickers)
    {
      string sql = "SELECT TOP " + maxNumOfReturnedTickers + " tickers.tiTicker, tickers.tiCompanyName, " +
        "Avg(quotes.quAdjustedCloseToCloseRatio) AS AverageCloseToClosePerformance " +
        "FROM quotes INNER JOIN (tickers INNER JOIN tickers_tickerGroups " +
        "ON tickers.tiTicker = tickers_tickerGroups.ttTiId) " +
        "ON quotes.quTicker = tickers_tickerGroups.ttTiId " +
        "WHERE tickers_tickerGroups.ttTgId='" + groupID + "' " +
        "AND quotes.quDate BETWEEN " +
        SQLBuilder.GetDateConstant(firstQuoteDate) + " AND " +
        SQLBuilder.GetDateConstant(lastQuoteDate) + 
        "GROUP BY tickers.tiTicker, tickers.tiCompanyName " +
        "ORDER BY Avg(quotes.quAdjustedCloseToCloseRatio)";
      string sortDirection = " DESC";
      if(orderInASCMode)
        sortDirection = " ASC";
      sql = sql + sortDirection;
      return SqlExecutor.GetDataTable( sql );
    }

    /// <summary>
    /// Returns tickers ordered by average open to close performance (in the same bar)
    /// </summary>
    public static DataTable GetTickersByAverageOpenToClosePerformance( bool orderInASCMode, string groupID,
                                                            DateTime firstQuoteDate,
                                                            DateTime lastQuoteDate,
                                                            double maxAbsoluteAverageOTCPerformance,
                                                            long maxNumOfReturnedTickers)
    {
      string sql = "SELECT TOP " + maxNumOfReturnedTickers + " tickers.tiTicker, tickers.tiCompanyName, " +
        "Avg(quotes.quClose/quotes.quOpen - 1) AS AverageOpenToClosePerformance " +
        "FROM quotes INNER JOIN (tickers INNER JOIN tickers_tickerGroups " +
        "ON tickers.tiTicker = tickers_tickerGroups.ttTiId) " +
        "ON quotes.quTicker = tickers_tickerGroups.ttTiId " +
        "WHERE tickers_tickerGroups.ttTgId='" + groupID + "' " +
        "AND quotes.quDate BETWEEN " +
        SQLBuilder.GetDateConstant(firstQuoteDate) + " AND " +
        SQLBuilder.GetDateConstant(lastQuoteDate) + 
        "GROUP BY tickers.tiTicker, tickers.tiCompanyName " +
      	"HAVING Avg(quotes.quClose/quotes.quOpen - 1) <= " + maxAbsoluteAverageOTCPerformance +
      	" AND Avg(quotes.quClose/quotes.quOpen - 1) >= -" + maxAbsoluteAverageOTCPerformance + " " +
        "ORDER BY Avg(quotes.quClose/quotes.quOpen)";
      string sortDirection = " DESC";
      if(orderInASCMode)
        sortDirection = " ASC";
      sql = sql + sortDirection;
      return SqlExecutor.GetDataTable( sql );
    }

    /// <summary>
    /// returns tickers ordered by the average raw open price that is over
    /// a given minimum, at a given time interval
    /// </summary>
    public static DataTable GetTickersByRawOpenPrice( bool orderInASCMode, string groupID,
                                                      DateTime firstQuoteDate,
                                                      DateTime lastQuoteDate,
                                                      long maxNumOfReturnedTickers, double minPrice )
    {
      string sql = "SELECT TOP " + maxNumOfReturnedTickers + " quotes.quTicker, tickers.tiCompanyName, " +
        "Avg(quotes.quOpen) AS AverageRawOpenPrice " +
        "FROM (quotes INNER JOIN tickers ON quotes.quTicker=tickers.tiTicker) " +
        "INNER JOIN tickers_tickerGroups ON tickers.tiTicker=tickers_tickerGroups.ttTiId " +
        "WHERE quotes.quDate Between " + SQLBuilder.GetDateConstant(firstQuoteDate) + " " +
        "AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + " " +
        "AND " + "tickers_tickerGroups.ttTgId='" + groupID + "' " +
        "GROUP BY quotes.quTicker, tickers.tiCompanyName " +
        "HAVING Avg(quotes.quOpen) >= " + minPrice + " " + 
        "ORDER BY Avg(quotes.quOpen)";
      string sortDirection = " DESC";
      if(orderInASCMode)
        sortDirection = " ASC";
      sql = sql + sortDirection;
      return SqlExecutor.GetDataTable( sql );
    }

    /// <summary>
    /// returns tickers ordered by average raw open price level,
    /// with a given standard deviation, in a given time interval
    /// </summary>
    public static DataTable GetTickersByRawOpenPrice( bool orderInASCMode, string groupID,
                                                    DateTime firstQuoteDate,
                                                    DateTime lastQuoteDate,
                                                    long maxNumOfReturnedTickers, double minPrice,
                                                    double maxPrice, double minStdDeviation,
                                                    double maxStdDeviation)
    {
      string sql = "SELECT TOP " + maxNumOfReturnedTickers + " quotes.quTicker, tickers.tiCompanyName, " +
                    "Avg(quotes.quOpen) AS AverageRawOpenPrice, StDev(quotes.quOpen) AS StdDevRawOpenPrice " +
                  "FROM (quotes INNER JOIN tickers ON quotes.quTicker=tickers.tiTicker) " +
                  "INNER JOIN tickers_tickerGroups ON tickers.tiTicker=tickers_tickerGroups.ttTiId " +
                  "WHERE quotes.quDate Between " + SQLBuilder.GetDateConstant(firstQuoteDate) + " " +
                  "AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + " " +
                  "AND " + "tickers_tickerGroups.ttTgId='" + groupID + "' " +
                  "GROUP BY quotes.quTicker, tickers.tiCompanyName " +
                  "HAVING Avg(quotes.quOpen) BETWEEN " + minPrice + " AND " + maxPrice + " " +
                    "AND StDev(quotes.quOpen) BETWEEN " + minStdDeviation + " AND " + maxStdDeviation + " " +
                  "ORDER BY Avg(quotes.quOpen)";
      string sortDirection = " DESC";
      if(orderInASCMode)
        sortDirection = " ASC";
      sql = sql + sortDirection;
      return SqlExecutor.GetDataTable( sql );
    }
    /*moved to Quotes inside data layer
    /// <summary>
    /// returns tickers counting how many times raw close is greater than raw open 
    /// for the given interval of days (within the given group of tickers).
    /// Tickers are ordered by the number of days raw open is greater than raw close
    /// </summary>
    public static DataTable GetTickersByOpenToCloseWinningDays( bool orderInASCMode, string groupID,
                                                    DateTime firstQuoteDate,
                               											DateTime lastQuoteDate,
                                                    long maxNumOfReturnedTickers)
    {
      
      string sql = "SELECT TOP " + maxNumOfReturnedTickers + " quotes.quTicker, tickers.tiCompanyName, " +
                  "Count(quotes.quClose) AS CloseToOpenWinningDays " +
      						"FROM (quotes INNER JOIN tickers ON quotes.quTicker=tickers.tiTicker) " +
                  "INNER JOIN tickers_tickerGroups ON tickers.tiTicker=tickers_tickerGroups.ttTiId " +
                  "WHERE quotes.quDate Between " + SQLBuilder.GetDateConstant(firstQuoteDate) + " " +
                  "AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + " " +
                  "AND " + "tickers_tickerGroups.ttTgId='" + groupID + "' " +
      						"AND " + "quotes.quClose > quotes.quOpen " +
                  "GROUP BY quotes.quTicker, tickers.tiCompanyName " +
                  "ORDER BY Count(quotes.quClose)";
      string sortDirection = " DESC";
      if(orderInASCMode)
        sortDirection = " ASC";
      sql = sql + sortDirection;
      return SqlExecutor.GetDataTable( sql );
    }
    */
    /// <summary>
    /// returns the average traded value for the given ticker in the specified interval
    /// </summary>
    public static double GetAverageTradedValue( string ticker,
                                                DateTime firstQuoteDate,
                                                DateTime lastQuoteDate)
                                                
    {
      DataTable dt;
      string sql = "SELECT quotes.quTicker, " +
          "Avg([quVolume]*[quClose]) AS AverageTradedValue " +
          "FROM quotes WHERE quTicker ='" + 
          ticker + "' " +
          "AND quotes.quDate BETWEEN " + SQLBuilder.GetDateConstant(firstQuoteDate) + 
          " AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + 
          " GROUP BY quotes.quTicker";
      dt = SqlExecutor.GetDataTable( sql );
      if(dt.Rows.Count==0)
        return 0;
      else
        return (double)dt.Rows[0]["AverageTradedValue"];
     }

    /// <summary>
    /// returns the average traded volume for the given ticker in the specified interval
    /// </summary>
    public static double GetAverageTradedVolume( string ticker,
      DateTime firstQuoteDate,
      DateTime lastQuoteDate)
                                                
    {
      DataTable dt;
      string sql = "SELECT quotes.quTicker, " +
        "Avg([quVolume]) AS AverageTradedVolume " +
        "FROM quotes WHERE quTicker ='" + 
        ticker + "' " +
        "AND quotes.quDate BETWEEN " + SQLBuilder.GetDateConstant(firstQuoteDate) + 
        " AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + 
        " GROUP BY quotes.quTicker";
      dt = SqlExecutor.GetDataTable( sql );
      if(dt.Rows.Count==0)
        return 0;
      else
        return (double)dt.Rows[0]["AverageTradedVolume"];
    }

    /// <summary>
    /// returns the average close to close performance value for the given ticker in the specified interval
    /// </summary>
    public static double GetAverageCloseToClosePerformance( string ticker,
                                                            DateTime firstQuoteDate,
                                                            DateTime lastQuoteDate)
                                                
    {
      DataTable dt;
      string sql = "SELECT quotes.quTicker, " +
        "Avg([quAdjustedCloseToCloseRatio]) AS AverageCloseToClosePerformance " +
        "FROM quotes WHERE quTicker ='" + 
        ticker + "' " +
        "AND quotes.quDate BETWEEN " + SQLBuilder.GetDateConstant(firstQuoteDate) + 
        " AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + 
        " GROUP BY quotes.quTicker";
      dt = SqlExecutor.GetDataTable( sql );
      if(dt.Rows.Count==0)
        return 0;
      else
        return (double)dt.Rows[0]["AverageCloseToClosePerformance"];
    }

    /// <summary>
    /// returns the average open to close performance
    /// for the given ticker in the specified interval
    /// </summary>
    public static double GetAverageOpenToClosePerformance(string ticker,
                                                          DateTime firstQuoteDate,
                                                          DateTime lastQuoteDate)
                                                
    {
      DataTable dt;
      string sql = "SELECT quotes.quTicker, " +
        "Avg([quClose]/[quOpen] - 1) AS AverageOpenToClosePerformance " +
        "FROM quotes WHERE quTicker ='" + 
        ticker + "' " +
        "AND quotes.quDate BETWEEN " + SQLBuilder.GetDateConstant(firstQuoteDate) + 
        " AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + 
        " GROUP BY quotes.quTicker";
      dt = SqlExecutor.GetDataTable( sql );
      return (double)dt.Rows[0]["AverageOpenToClosePerformance"];
    }


    /// <summary>
    /// returns the standard deviation of the adjusted close to close ratio
    /// for the given ticker in the specified interval
    /// </summary>
    public static double GetAdjustedCloseToCloseStandardDeviation( string ticker,
                                                                    DateTime firstQuoteDate,
                                                                    DateTime lastQuoteDate)
                                                
    {
      double adjCloseToCloseStdDev = 0.0;
			DataTable dt;
      string sql = "SELECT quotes.quTicker, " +
        "StDev(quotes.quAdjustedCloseToCloseRatio) AS AdjCloseToCloseStandDev " +
        "FROM quotes WHERE quTicker ='" + 
        ticker + "' " +
        "AND quotes.quDate BETWEEN " + SQLBuilder.GetDateConstant(firstQuoteDate) + 
        " AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + 
        " GROUP BY quotes.quTicker";
      dt = SqlExecutor.GetDataTable( sql );
      
			if( dt.Rows.Count > 0 &&
				  DBNull.Value != dt.Rows[0]["AdjCloseToCloseStandDev"] )
        adjCloseToCloseStdDev = (double)dt.Rows[0]["AdjCloseToCloseStandDev"];

			return adjCloseToCloseStdDev;
    }
    
    /// <summary>
    /// returns the standard deviation of the open to close ratio
    /// for the given ticker in the specified interval
    /// </summary>
    public static double GetOpenToCloseStandardDeviation( string ticker,
                                                          DateTime firstQuoteDate,
                                                          DateTime lastQuoteDate)
                                                
    {
      DataTable dt;
      string sql = "SELECT quotes.quTicker, " +
        "StDev(quotes.quClose/quotes.quOpen - 1) AS OpenToCloseStandDev " +
        "FROM quotes WHERE quTicker ='" + 
        ticker + "' " +
        "AND quotes.quDate BETWEEN " + SQLBuilder.GetDateConstant(firstQuoteDate) + 
        " AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + 
        " GROUP BY quotes.quTicker";
      dt = SqlExecutor.GetDataTable( sql );
      return (double)dt.Rows[0]["OpenToCloseStandDev"];
    }

    /// <summary>
    /// returns the standard deviation of the adjusted close to open ratio
    /// for the given ticker in the specified interval
    /// </summary>
    public static double GetCloseToOpenStandardDeviation( string ticker,
      DateTime firstQuoteDate,
      DateTime lastQuoteDate)
                                                
    {
      double returnValue = Double.MaxValue;
      
      DataTable dt;
      string sql = "SELECT quotes.quTicker, " +
        "StDev(quotes.quClose/quotes.quOpen) AS CloseToOpenStandDev " +
        "FROM quotes WHERE quTicker ='" + 
        ticker + "' " +
        "AND quotes.quDate BETWEEN " + SQLBuilder.GetDateConstant(firstQuoteDate) + 
        " AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + 
        " GROUP BY quotes.quTicker";
      dt = SqlExecutor.GetDataTable( sql );
      if(dt.Rows.Count > 0)
      {  
        if( dt.Rows[0]["CloseToOpenStandDev"] is double )
        //cast is possible
            returnValue = (double)dt.Rows[0]["CloseToOpenStandDev"];
      }
      return returnValue;
    } 
    
    /// <summary>
    /// returns the average raw open price for the given ticker, 
    /// at the specified time interval 
    /// </summary>
    public static double GetAverageRawOpenPrice( string ticker,
                                                DateTime firstQuoteDate,
                                                DateTime lastQuoteDate)
                                                
    {
      double returnValue = 0;
    	DataTable dt;
      string sql = "SELECT quotes.quTicker, tickers.tiCompanyName, " +
                    "Avg(quotes.quOpen) AS AverageRawOpenPrice " +
                  "FROM (quotes INNER JOIN tickers ON quotes.quTicker=tickers.tiTicker) " +
                  "INNER JOIN tickers_tickerGroups ON tickers.tiTicker=tickers_tickerGroups.ttTiId " +
                  "WHERE quotes.quTicker ='" + ticker + 
      			  "' AND quotes.quDate Between " + SQLBuilder.GetDateConstant(firstQuoteDate) + " " +
                  "AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + " " +
                  "GROUP BY quotes.quTicker, tickers.tiCompanyName";
      dt = SqlExecutor.GetDataTable( sql );
      if(dt.Rows.Count > 0)
      {  
        if( dt.Rows[0]["AverageRawOpenPrice"] is double )
        //cast is possible
            returnValue = (double)dt.Rows[0]["AverageRawOpenPrice"];
      }
      return returnValue;
      	
     }

	/// <summary>
    /// returns raw open price's standard deviation for the given ticker,
    /// at the specified time interval
	/// </summary>
    public static double GetRawOpenPriceStdDeviation( string ticker,
                                                DateTime firstQuoteDate,
                                                DateTime lastQuoteDate)
                                                
    {
      double returnValue = Double.MaxValue;
			DataTable dt;
      string sql = "SELECT quotes.quTicker, tickers.tiCompanyName, " +
                    "StDev(quotes.quOpen) AS RawOpenPriceStdDev " +
                  "FROM (quotes INNER JOIN tickers ON quotes.quTicker=tickers.tiTicker) " +
                  "INNER JOIN tickers_tickerGroups ON tickers.tiTicker=tickers_tickerGroups.ttTiId " +
                  "WHERE quotes.quTicker ='" + ticker + 
      			  "' AND quotes.quDate Between " + SQLBuilder.GetDateConstant(firstQuoteDate) + " " +
                  "AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + " " +
                  "GROUP BY quotes.quTicker, tickers.tiCompanyName";
      dt = SqlExecutor.GetDataTable( sql );
      if(dt.Rows.Count > 0)
      {  
        if( dt.Rows[0]["RawOpenPriceStdDev"] is double )
        //cast is possible
            returnValue = (double)dt.Rows[0]["RawOpenPriceStdDev"];
      }
      return returnValue;
     }
    
	  /// <summary>
    /// Returns number of days for which raw close was greater than raw open 
    /// for the given interval of days (for the given ticker).
    /// </summary>
    public static int GetNumberOfOpenToCloseWinningDays(string ticker,
                                                   DateTime firstQuoteDate,
                               										 DateTime lastQuoteDate)
    {
      DataTable dt;
      int returnValue = 0;
    	string sql = "SELECT Count(*) AS CloseToOpenWinningDays " +
      						"FROM quotes WHERE " + 
    							"quotes.quDate Between " + SQLBuilder.GetDateConstant(firstQuoteDate) + " " +
                  "AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + " " +
                  "AND " + "quotes.quTicker='" + ticker + "' " +
    							"AND quotes.quClose > quotes.quOpen";
    	
     	dt = SqlExecutor.GetDataTable( sql );
     	if(dt.Rows.Count > 0)
     	{
     		if(dt.Rows[0][0] is int)
     			returnValue = (int)dt.Rows[0][0];
     	}
     	return returnValue;
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
    /// returns the quotes DataTable for the given ticker
    /// </summary>
    /// <param name="instrumentKey">ticker whose quotes are to be returned</param>
    /// <param name="firstQuoteDate">The first quote date</param>
    /// <param name="lastQuoteDate">The last quote date</param>
    /// <returns></returns>
    public static DataTable GetTickerQuotes( string instrumentKey, DateTime firstQuoteDate,
                                              DateTime lastQuoteDate)
    {
      string sql = "select * from quotes where quTicker='" + instrumentKey + "' " +
        "AND quotes.quDate BETWEEN " + SQLBuilder.GetDateConstant(firstQuoteDate) + 
        " AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + " " +
        "order by quDate";
      return SqlExecutor.GetDataTable( sql );
    }
		/// <summary>
		/// Returns the quotes for the given instrument , since startDate to endDate
		/// </summary>
		/// <param name="tickerOrGroupID">The symbol of a ticker or the groupID corresponding to a specific set of tickers</param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <returns></returns>
		public static void SetDataTable( string tickerOrGroupID , DateTime startDate , DateTime endDate ,
			DataTable dataTable)
		{
			string sql;
			if(Tickers_tickerGroups.HasTickers(tickerOrGroupID))
				sql =	"select * from quotes INNER JOIN tickers_tickerGroups ON " +
					"quotes." + Quotes.TickerFieldName + "=tickers_tickerGroups." + Tickers_tickerGroups.Ticker + " " +
					"where " + Tickers_tickerGroups.GroupID + "='" + tickerOrGroupID + "' " +
					"and " + Quotes.Date + ">=" + SQLBuilder.GetDateConstant( startDate ) + " " +
					"and " + Quotes.Date + "<=" + SQLBuilder.GetDateConstant( endDate ) + " " +
					"order by " + Quotes.Date;
			else
				sql =	"select * from quotes " +
					"where " + Quotes.TickerFieldName + "='" + tickerOrGroupID + "' " +
					"and " + Quotes.Date + ">=" + SQLBuilder.GetDateConstant( startDate ) + " " +
					"and " + Quotes.Date + "<=" + SQLBuilder.GetDateConstant( endDate ) + " " +
					"order by " + Quotes.Date;
			
			SqlExecutor.SetDataTable( sql , dataTable );
		}

		#region SetDataTable for tickerList
		private static string setDataTable_getTickerListWhereClause_getSingleTickerWhereClause(
			string ticker )
		{
			return "(" + Quotes.TickerFieldName + "='" + ticker + "')";
		}
		private static string setDataTable_getTickerListWhereClause( ICollection tickerCollection )
		{
			string returnValue = "";
			foreach (string ticker in tickerCollection)
				if ( returnValue == "" )
					// this is the first ticker to handle
					returnValue += setDataTable_getTickerListWhereClause_getSingleTickerWhereClause( ticker );
				else
					// this is not the first ticker to handle
					returnValue += " or " +
						setDataTable_getTickerListWhereClause_getSingleTickerWhereClause( ticker );
			return "( " + returnValue + " )";
		}
		/// <summary>
		/// Builds a Quotes data table containing a row for each ticker in the
		/// collection, with the quotes for the given DateTime
		/// </summary>
		/// <param name="tickerCollection">Tickers whose quotes are to be fetched</param>
		/// <param name="dateTime">Date for the quotes to be fetched</param>
		/// <param name="dataTable">Output parameter</param>
		public static void SetDataTable( ICollection tickerCollection , DateTime dateTime , DataTable dataTable)
		{
			string sql;
			sql =	"select * from quotes " +
				"where " + setDataTable_getTickerListWhereClause( tickerCollection ) +
				" and " + Quotes.Date + "=" + SQLBuilder.GetDateConstant( dateTime ) + " " +
				"order by " + Quotes.TickerFieldName;
			
			SqlExecutor.SetDataTable( sql , dataTable );
		}
		#endregion



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
    
    /// <summary>
    /// Provides updating the database with a new adjusted close for
    /// the given ticker at a specified date
    /// </summary>
    public static void UpdateAdjustedClose( string ticker, DateTime date, double adjustedClose )
    {
      string sql = "UPDATE quotes SET quotes.quAdjustedClose=" +
        adjustedClose + " WHERE quotes.quTicker='" + ticker + "' AND " +
        "quotes.quDate=" + SQLBuilder.GetDateConstant(date);
      SqlExecutor.ExecuteNonQuery (sql);
    }

    /* Now useless
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
