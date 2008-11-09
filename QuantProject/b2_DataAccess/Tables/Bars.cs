/*
QuantDownloader - Quantitative Finance Library

Bars.cs
Copyright (C) 2008 
Marco Milletti

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

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
	public class Bars
	{
    // these static fields provide field name in the database table
		// They are intended to be used through intellisense when necessary
		public static string TickerFieldName = "baTicker";	// Ticker cannot be simply used because
		public static string Exchange = "baExchange";		
		public static string DateTimeForOpen = "baDateTimeForOpen";
		public static string IntervalFrameInSeconds = "baInterval";
		public static string Open = "baOpen";
		public static string High = "baHigh";
		public static string Low = "baLow";
		public static string Close = "baClose";
		public static string Volume = "baVolume";   
    
		private DataTable bars;
    
		/// <summary>
		/// Gets the ticker whose bars are contained into the Bars object
		/// </summary>
		/// <returns></returns>
		public string Ticker
		{
			get{ return ((string)this.bars.Rows[ 0 ][ Bars.TickerFieldName ]); }
		}

		public Bars( string ticker)
		{
			this.bars = Bars.GetTickerBars( ticker );
		}
		/// <summary>
		/// Creates bars for the given instrument, since the startDateTime to the endDateTime
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="startDateTime"></param>
		/// <param name="endDateTime"></param>
		public Bars( string ticker , DateTime startDateTime , DateTime endDateTime )
		{
			/// TO DO
		}
		/// <summary>
		/// Returns the dateTime of the first bar for the given ticker
		/// </summary>
		/// <param name="ticker">ticker for which the dateTime of the first bar has to be returned</param>
		/// <returns></returns>
		public static DateTime GetFirstBarDateTime( string ticker )
		{
			DataTable dataTable = SqlExecutor.GetDataTable(
				"select min(" + Bars.DateTimeForOpen + ") as minDate from bars where " + Bars.TickerFieldName + "='" + ticker + "' " +
				"group by " + Bars.TickerFieldName + ")" );
			return (DateTime)(dataTable.Rows[ 0 ][ "minDate" ]);
		}
		/// <summary>
		/// Returns the dateTime of the last bar for the given ticker
		/// </summary>
		/// <param name="ticker">ticker for which the dateTime of the last bar has to be returned</param>
		/// <returns></returns>
		public static DateTime GetLastBarDateTime( string ticker )
		{
			DataTable dataTable = SqlExecutor.GetDataTable(
				"select * from bars where " + Bars.TickerFieldName + "='" + ticker + "' " +
				"order by " + Bars.DateTimeForOpen + " DESC");
			return (DateTime)(dataTable.Rows[0][ Bars.DateTimeForOpen ]);
		}
    /// <summary>
    /// Returns the number of Bars for the given ticker
    /// </summary>
    /// <param name="ticker">ticker for which the number of bars has to be returned</param>
    /// <returns></returns>
    public static int GetNumberOfBars( string ticker )
    {
      DataTable dataTable = SqlExecutor.GetDataTable(
        "select * from bars where " + Bars.TickerFieldName + "='" + ticker + "'" );
      return dataTable.Rows.Count;
    }

    /// <summary>
    /// Returns the number of bars at which the given ticker has been effectively traded
    /// (volume > 0)
    /// </summary>
    /// <param name="ticker">ticker for which the number of bars has to be returned</param>
    /// <returns></returns>
    public static int GetNumberOfBarsWithEffectiveTrades( string ticker, DateTime firstDateTime,
                                                          DateTime lastDateTime)
    {
      DataTable dataTable = SqlExecutor.GetDataTable(
        "select * from bars WHERE " + Bars.TickerFieldName + "='" + ticker + "'" +
        " AND " + Bars.Volume + ">0" + " AND " + Bars.DateTimeForOpen + " BETWEEN " + SQLBuilder.GetDateConstant(firstDateTime) + 
        " AND " + SQLBuilder.GetDateConstant(lastDateTime));
      return dataTable.Rows.Count;
    }
    
    /// <summary>
    /// Returns the close for the given ticker at the specified date
    /// time
    /// </summary>
    /// <param name="ticker">ticker for which the close has to be returned</param>
    /// <returns></returns>
    public static float GetClose( string ticker, DateTime dateTime )
    {
      DataTable dataTable = SqlExecutor.GetDataTable(
        "select " + Bars.Close +" from bars where " + Bars.TickerFieldName + "='" + ticker + "' " +
        "and " + Bars.DateTimeForOpen + "=" + SQLBuilder.GetDateConstant(dateTime) );
      return (float)dataTable.Rows[0][0];
    }
    /// <summary>
    /// Returns the open for the given ticker at the specified date
    /// time
    /// </summary>
    /// <param name="ticker">ticker for which the raw open has to be returned</param>
    /// <returns></returns>
    public static float GetOpen( string ticker, DateTime dateTime )
    {
      DataTable dataTable = SqlExecutor.GetDataTable(
        "select " + Bars.Open +" from bars where " + Bars.TickerFieldName + "='" + ticker + "' " +
        "and " + Bars.DateTimeForOpen + "=" + SQLBuilder.GetDateConstant(dateTime) );
      return (float)dataTable.Rows[0][0];
    }
    
    
//    /// <summary>
//    /// It provides deletion of the quote from the table "quotes" for
//    /// the given ticker for a specified date
//    /// </summary>
//    public static void Delete( string ticker, DateTime dateOfTheQuoteToBeDeleted )
//    {
//      try
//      {
//      SqlExecutor.ExecuteNonQuery("DELETE * FROM quotes " +
//                                  "WHERE quTicker ='" +
//                                  ticker + "' AND quDate =" +
//                                  SQLBuilder.GetDateConstant(dateOfTheQuoteToBeDeleted));
//      }
//      catch(Exception ex)
//      {
//        string notUsed = ex.ToString();
//      }
//    }
//    /// <summary>
//    /// It provides deletion of the quote from the table "quotes" for
//    /// the given ticker for the specified interval
//    /// </summary>
//    public static void Delete( string ticker, DateTime fromDate,
//                                DateTime toDate)
//    {
//      try
//      {
//        SqlExecutor.ExecuteNonQuery("DELETE * FROM quotes " +
//          "WHERE quTicker ='" +
//          ticker + "' AND quDate >=" +
//          SQLBuilder.GetDateConstant(fromDate) + " " +
//          "AND quDate<=" + SQLBuilder.GetDateConstant(toDate));
//      }
//      catch(Exception ex)
//      {
//        string notUsed = ex.ToString();
//      }
//    }
//    /// <summary>
//    /// It provides addition of the given quote's values into table "quotes" 
//    /// </summary>
//    public static void Add( string ticker, DateTime date, double open, 
//                            double high, double low, double close,
//                            double volume, double adjustedClose)
//    {
//      try
//      {
//      SqlExecutor.ExecuteNonQuery("INSERT INTO quotes(quTicker, quDate, quOpen, " +
//                                  "quHigh, quLow, quClose, quVolume, quAdjustedClose) " +
//                                  "VALUES('" + ticker + "', " + SQLBuilder.GetDateConstant(date) + ", " +
//                                  open + ", " + high + ", " + low + ", " + close + ", " +
//                                  volume + ", " + adjustedClose + ")");
//      }
//      catch(Exception ex)
//      {
//        string notUsed = ex.ToString();
//      }
//    }             

    
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

//		/// <summary>
//		/// returns tickers ordered by liquidity, with a specified min volume
//		/// </summary>
//		/// <param name="orderInASCMode">true iff return must be ordered</param>
//		/// <param name="groupID"></param>
//		/// <param name="firstQuoteDate"></param>
//		/// <param name="lastQuoteDate"></param>
//		/// <param name="maxNumOfReturnedTickers"></param>
//		/// <param name="minVolume"></param>
//		/// <returns></returns>
//		public static DataTable GetTickersByLiquidity( bool orderInASCMode, string groupID,
//			DateTime firstQuoteDate,
//			DateTime lastQuoteDate,
//			long minVolume,
//			long maxNumOfReturnedTickers
//			)
//		{
//			string sql = "SELECT TOP " + maxNumOfReturnedTickers + " tickers.tiTicker, tickers.tiCompanyName, " +
//				"Avg([quVolume]*[quClose]) AS AverageTradedValue " +
//				"FROM quotes INNER JOIN (tickers INNER JOIN tickers_tickerGroups " +
//				"ON tickers.tiTicker = tickers_tickerGroups.ttTiId) " +
//				"ON quotes.quTicker = tickers_tickerGroups.ttTiId " +
//				"WHERE tickers_tickerGroups.ttTgId='" + groupID + "' " +
//				"AND quotes.quDate BETWEEN " +
//				SQLBuilder.GetDateConstant(firstQuoteDate) + " AND " +
//				SQLBuilder.GetDateConstant(lastQuoteDate) + 
//				"GROUP BY tickers.tiTicker, tickers.tiCompanyName " +
//				"HAVING Avg([quVolume])>=" + minVolume.ToString() + " " +
//				"ORDER BY Avg([quVolume])";
//			string sortDirection = " DESC";
//			if(orderInASCMode)
//				sortDirection = " ASC";
//			sql = sql + sortDirection;
//			return SqlExecutor.GetDataTable( sql );
//		}
//
//    
//
//    /// <summary>
//    /// Returns tickers ordered by a close to close volatility index (stdDev of adjustedCloseToClose ratio)
//    /// </summary>
//    public static DataTable GetTickersByCloseToCloseVolatility( bool orderInASCMode, string groupID,
//                                                    DateTime firstQuoteDate,
//                                                    DateTime lastQuoteDate,
//                                                    long maxNumOfReturnedTickers)
//    {
//      string sql = "SELECT TOP " + maxNumOfReturnedTickers + " tickers.tiTicker, tickers.tiCompanyName, " +
//        "StDev(quotes.quAdjustedCloseToCloseRatio) AS AdjCloseToCloseStandDev " +
//        "FROM quotes INNER JOIN (tickers INNER JOIN tickers_tickerGroups " +
//        "ON tickers.tiTicker = tickers_tickerGroups.ttTiId) " +
//        "ON quotes.quTicker = tickers_tickerGroups.ttTiId " +
//        "WHERE tickers_tickerGroups.ttTgId='" + groupID + "' " +
//        "AND quotes.quDate BETWEEN " +
//        SQLBuilder.GetDateConstant(firstQuoteDate) + " AND " +
//        SQLBuilder.GetDateConstant(lastQuoteDate) + 
//        "GROUP BY tickers.tiTicker, tickers.tiCompanyName " +
//        "ORDER BY StDev(quotes.quAdjustedCloseToCloseRatio)";
//      string sortDirection = " DESC";
//      if(orderInASCMode)
//        sortDirection = " ASC";
//      sql = sql + sortDirection;
//      return SqlExecutor.GetDataTable( sql );
//    }
//
//    /// <summary>
//    /// Returns tickers ordered by the open to close volatility index (stdDev of OTC ratio)
//    /// </summary>
//    public static DataTable GetTickersByOpenToCloseVolatility( bool orderInASCMode, string groupID,
//      DateTime firstQuoteDate,
//      DateTime lastQuoteDate,
//      long maxNumOfReturnedTickers)
//    {
//      string sql = "SELECT TOP " + maxNumOfReturnedTickers + " tickers.tiTicker, tickers.tiCompanyName, " +
//        "StDev(quotes.quClose/quotes.quOpen - 1) AS OpenToCloseStandDev " +
//        "FROM quotes INNER JOIN (tickers INNER JOIN tickers_tickerGroups " +
//        "ON tickers.tiTicker = tickers_tickerGroups.ttTiId) " +
//        "ON quotes.quTicker = tickers_tickerGroups.ttTiId " +
//        "WHERE tickers_tickerGroups.ttTgId='" + groupID + "' " +
//        "AND quotes.quDate BETWEEN " +
//        SQLBuilder.GetDateConstant(firstQuoteDate) + " AND " +
//        SQLBuilder.GetDateConstant(lastQuoteDate) + 
//        "GROUP BY tickers.tiTicker, tickers.tiCompanyName " +
//        "ORDER BY StDev(quotes.quClose/quotes.quOpen - 1)";
//      string sortDirection = " DESC";
//      if(orderInASCMode)
//        sortDirection = " ASC";
//      sql = sql + sortDirection;
//      return SqlExecutor.GetDataTable( sql );
//    }
//
//    /// <summary>
//    /// Returns tickers ordered by average close to close performance 
//    /// </summary>
//    public static DataTable GetTickersByAverageCloseToClosePerformance( bool orderInASCMode, string groupID,
//                                                          DateTime firstQuoteDate,
//                                                          DateTime lastQuoteDate,
//                                                          long maxNumOfReturnedTickers)
//    {
//      string sql = "SELECT TOP " + maxNumOfReturnedTickers + " tickers.tiTicker, tickers.tiCompanyName, " +
//        "Avg(quotes.quAdjustedCloseToCloseRatio) AS AverageCloseToClosePerformance " +
//        "FROM quotes INNER JOIN (tickers INNER JOIN tickers_tickerGroups " +
//        "ON tickers.tiTicker = tickers_tickerGroups.ttTiId) " +
//        "ON quotes.quTicker = tickers_tickerGroups.ttTiId " +
//        "WHERE tickers_tickerGroups.ttTgId='" + groupID + "' " +
//        "AND quotes.quDate BETWEEN " +
//        SQLBuilder.GetDateConstant(firstQuoteDate) + " AND " +
//        SQLBuilder.GetDateConstant(lastQuoteDate) + 
//        "GROUP BY tickers.tiTicker, tickers.tiCompanyName " +
//        "ORDER BY Avg(quotes.quAdjustedCloseToCloseRatio)";
//      string sortDirection = " DESC";
//      if(orderInASCMode)
//        sortDirection = " ASC";
//      sql = sql + sortDirection;
//      return SqlExecutor.GetDataTable( sql );
//    }
//
//    /// <summary>
//    /// Returns tickers ordered by average open to close performance (in the same bar)
//    /// </summary>
//    public static DataTable GetTickersByAverageOpenToClosePerformance( bool orderInASCMode, string groupID,
//                                                            DateTime firstQuoteDate,
//                                                            DateTime lastQuoteDate,
//                                                            double maxAbsoluteAverageOTCPerformance,
//                                                            long maxNumOfReturnedTickers)
//    {
//      string sql = "SELECT TOP " + maxNumOfReturnedTickers + " tickers.tiTicker, tickers.tiCompanyName, " +
//        "Avg(quotes.quClose/quotes.quOpen - 1) AS AverageOpenToClosePerformance " +
//        "FROM quotes INNER JOIN (tickers INNER JOIN tickers_tickerGroups " +
//        "ON tickers.tiTicker = tickers_tickerGroups.ttTiId) " +
//        "ON quotes.quTicker = tickers_tickerGroups.ttTiId " +
//        "WHERE tickers_tickerGroups.ttTgId='" + groupID + "' " +
//        "AND quotes.quDate BETWEEN " +
//        SQLBuilder.GetDateConstant(firstQuoteDate) + " AND " +
//        SQLBuilder.GetDateConstant(lastQuoteDate) + 
//        "GROUP BY tickers.tiTicker, tickers.tiCompanyName " +
//      	"HAVING Avg(quotes.quClose/quotes.quOpen - 1) <= " + maxAbsoluteAverageOTCPerformance +
//      	" AND Avg(quotes.quClose/quotes.quOpen - 1) >= -" + maxAbsoluteAverageOTCPerformance + " " +
//        "ORDER BY Avg(quotes.quClose/quotes.quOpen)";
//      string sortDirection = " DESC";
//      if(orderInASCMode)
//        sortDirection = " ASC";
//      sql = sql + sortDirection;
//      return SqlExecutor.GetDataTable( sql );
//    }
//
//    /// <summary>
//    /// returns tickers ordered by the average raw open price that is over
//    /// a given minimum, at a given time interval
//    /// </summary>
//    public static DataTable GetTickersByRawOpenPrice( bool orderInASCMode, string groupID,
//                                                      DateTime firstQuoteDate,
//                                                      DateTime lastQuoteDate,
//                                                      long maxNumOfReturnedTickers, double minPrice )
//    {
//      string sql = "SELECT TOP " + maxNumOfReturnedTickers + " quotes.quTicker, tickers.tiCompanyName, " +
//        "Avg(quotes.quOpen) AS AverageRawOpenPrice " +
//        "FROM (quotes INNER JOIN tickers ON quotes.quTicker=tickers.tiTicker) " +
//        "INNER JOIN tickers_tickerGroups ON tickers.tiTicker=tickers_tickerGroups.ttTiId " +
//        "WHERE quotes.quDate Between " + SQLBuilder.GetDateConstant(firstQuoteDate) + " " +
//        "AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + " " +
//        "AND " + "tickers_tickerGroups.ttTgId='" + groupID + "' " +
//        "GROUP BY quotes.quTicker, tickers.tiCompanyName " +
//        "HAVING Avg(quotes.quOpen) >= " + minPrice + " " + 
//        "ORDER BY Avg(quotes.quOpen)";
//      string sortDirection = " DESC";
//      if(orderInASCMode)
//        sortDirection = " ASC";
//      sql = sql + sortDirection;
//      return SqlExecutor.GetDataTable( sql );
//    }
//
//    /// <summary>
//    /// returns tickers ordered by average raw open price level,
//    /// with a given standard deviation, in a given time interval
//    /// </summary>
//    public static DataTable GetTickersByRawOpenPrice( bool orderInASCMode, string groupID,
//                                                    DateTime firstQuoteDate,
//                                                    DateTime lastQuoteDate,
//                                                    long maxNumOfReturnedTickers, double minPrice,
//                                                    double maxPrice, double minStdDeviation,
//                                                    double maxStdDeviation)
//    {
//      string sql = "SELECT TOP " + maxNumOfReturnedTickers + " quotes.quTicker, tickers.tiCompanyName, " +
//                    "Avg(quotes.quOpen) AS AverageRawOpenPrice, StDev(quotes.quOpen) AS StdDevRawOpenPrice " +
//                  "FROM (quotes INNER JOIN tickers ON quotes.quTicker=tickers.tiTicker) " +
//                  "INNER JOIN tickers_tickerGroups ON tickers.tiTicker=tickers_tickerGroups.ttTiId " +
//                  "WHERE quotes.quDate Between " + SQLBuilder.GetDateConstant(firstQuoteDate) + " " +
//                  "AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + " " +
//                  "AND " + "tickers_tickerGroups.ttTgId='" + groupID + "' " +
//                  "GROUP BY quotes.quTicker, tickers.tiCompanyName " +
//                  "HAVING Avg(quotes.quOpen) BETWEEN " + minPrice + " AND " + maxPrice + " " +
//                    "AND StDev(quotes.quOpen) BETWEEN " + minStdDeviation + " AND " + maxStdDeviation + " " +
//                  "ORDER BY Avg(quotes.quOpen)";
//      string sortDirection = " DESC";
//      if(orderInASCMode)
//        sortDirection = " ASC";
//      sql = sql + sortDirection;
//      return SqlExecutor.GetDataTable( sql );
//    }
    
//    /// <summary>
//    /// returns the average traded value for the given ticker in the specified interval
//    /// </summary>
//    public static double GetAverageTradedValue( string ticker,
//                                                DateTime firstQuoteDate,
//                                                DateTime lastQuoteDate)
//                                                
//    {
//      DataTable dt;
//      string sql = "SELECT quotes.quTicker, " +
//          "Avg([quVolume]*[quClose]) AS AverageTradedValue " +
//          "FROM quotes WHERE quTicker ='" + 
//          ticker + "' " +
//          "AND quotes.quDate BETWEEN " + SQLBuilder.GetDateConstant(firstQuoteDate) + 
//          " AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + 
//          " GROUP BY quotes.quTicker";
//      dt = SqlExecutor.GetDataTable( sql );
//      if(dt.Rows.Count==0)
//        return 0;
//      else
//        return (double)dt.Rows[0]["AverageTradedValue"];
//     }
//
//    /// <summary>
//    /// returns the average traded volume for the given ticker in the specified interval
//    /// </summary>
//    public static double GetAverageTradedVolume( string ticker,
//      DateTime firstQuoteDate,
//      DateTime lastQuoteDate)
//                                                
//    {
//      DataTable dt;
//      string sql = "SELECT quotes.quTicker, " +
//        "Avg([quVolume]) AS AverageTradedVolume " +
//        "FROM quotes WHERE quTicker ='" + 
//        ticker + "' " +
//        "AND quotes.quDate BETWEEN " + SQLBuilder.GetDateConstant(firstQuoteDate) + 
//        " AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + 
//        " GROUP BY quotes.quTicker";
//      dt = SqlExecutor.GetDataTable( sql );
//      if(dt.Rows.Count==0)
//        return 0;
//      else
//        return (double)dt.Rows[0]["AverageTradedVolume"];
//    }
//
//    /// <summary>
//    /// returns the average close to close performance value for the given ticker in the specified interval
//    /// </summary>
//    public static double GetAverageCloseToClosePerformance( string ticker,
//                                                            DateTime firstQuoteDate,
//                                                            DateTime lastQuoteDate)
//                                                
//    {
//      DataTable dt;
//      string sql = "SELECT quotes.quTicker, " +
//        "Avg([quAdjustedCloseToCloseRatio]) AS AverageCloseToClosePerformance " +
//        "FROM quotes WHERE quTicker ='" + 
//        ticker + "' " +
//        "AND quotes.quDate BETWEEN " + SQLBuilder.GetDateConstant(firstQuoteDate) + 
//        " AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + 
//        " GROUP BY quotes.quTicker";
//      dt = SqlExecutor.GetDataTable( sql );
//      if(dt.Rows.Count==0)
//        return 0;
//      else
//        return (double)dt.Rows[0]["AverageCloseToClosePerformance"];
//    }
//
//    /// <summary>
//    /// returns the average open to close performance
//    /// for the given ticker in the specified interval
//    /// </summary>
//    public static double GetAverageOpenToClosePerformance(string ticker,
//                                                          DateTime firstQuoteDate,
//                                                          DateTime lastQuoteDate)
//                                                
//    {
//      DataTable dt;
//      string sql = "SELECT quotes.quTicker, " +
//        "Avg([quClose]/[quOpen] - 1) AS AverageOpenToClosePerformance " +
//        "FROM quotes WHERE quTicker ='" + 
//        ticker + "' " +
//        "AND quotes.quDate BETWEEN " + SQLBuilder.GetDateConstant(firstQuoteDate) + 
//        " AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + 
//        " GROUP BY quotes.quTicker";
//      dt = SqlExecutor.GetDataTable( sql );
//      return (double)dt.Rows[0]["AverageOpenToClosePerformance"];
//    }
//
//
//    /// <summary>
//    /// returns the standard deviation of the adjusted close to close ratio
//    /// for the given ticker in the specified interval
//    /// </summary>
//    public static double GetAdjustedCloseToCloseStandardDeviation( string ticker,
//                                                                    DateTime firstQuoteDate,
//                                                                    DateTime lastQuoteDate)
//                                                
//    {
//      double adjCloseToCloseStdDev = 0.0;
//			DataTable dt;
//      string sql = "SELECT quotes.quTicker, " +
//        "StDev(quotes.quAdjustedCloseToCloseRatio) AS AdjCloseToCloseStandDev " +
//        "FROM quotes WHERE quTicker ='" + 
//        ticker + "' " +
//        "AND quotes.quDate BETWEEN " + SQLBuilder.GetDateConstant(firstQuoteDate) + 
//        " AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + 
//        " GROUP BY quotes.quTicker";
//      dt = SqlExecutor.GetDataTable( sql );
//      
//			if( dt.Rows.Count > 0 &&
//				  DBNull.Value != dt.Rows[0]["AdjCloseToCloseStandDev"] )
//        adjCloseToCloseStdDev = (double)dt.Rows[0]["AdjCloseToCloseStandDev"];
//
//			return adjCloseToCloseStdDev;
//    }
//    
//    /// <summary>
//    /// returns the standard deviation of the open to close ratio
//    /// for the given ticker in the specified interval
//    /// </summary>
//    public static double GetOpenToCloseStandardDeviation( string ticker,
//                                                          DateTime firstQuoteDate,
//                                                          DateTime lastQuoteDate)
//                                                
//    {
//      DataTable dt;
//      string sql = "SELECT quotes.quTicker, " +
//        "StDev(quotes.quClose/quotes.quOpen - 1) AS OpenToCloseStandDev " +
//        "FROM quotes WHERE quTicker ='" + 
//        ticker + "' " +
//        "AND quotes.quDate BETWEEN " + SQLBuilder.GetDateConstant(firstQuoteDate) + 
//        " AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + 
//        " GROUP BY quotes.quTicker";
//      dt = SqlExecutor.GetDataTable( sql );
//      return (double)dt.Rows[0]["OpenToCloseStandDev"];
//    }
//
//    /// <summary>
//    /// returns the standard deviation of the adjusted close to open ratio
//    /// for the given ticker in the specified interval
//    /// </summary>
//    public static double GetCloseToOpenStandardDeviation( string ticker,
//      DateTime firstQuoteDate,
//      DateTime lastQuoteDate)
//                                                
//    {
//      double returnValue = Double.MaxValue;
//      
//      DataTable dt;
//      string sql = "SELECT quotes.quTicker, " +
//        "StDev(quotes.quClose/quotes.quOpen) AS CloseToOpenStandDev " +
//        "FROM quotes WHERE quTicker ='" + 
//        ticker + "' " +
//        "AND quotes.quDate BETWEEN " + SQLBuilder.GetDateConstant(firstQuoteDate) + 
//        " AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + 
//        " GROUP BY quotes.quTicker";
//      dt = SqlExecutor.GetDataTable( sql );
//      if(dt.Rows.Count > 0)
//      {  
//        if( dt.Rows[0]["CloseToOpenStandDev"] is double )
//        //cast is possible
//            returnValue = (double)dt.Rows[0]["CloseToOpenStandDev"];
//      }
//      return returnValue;
//    } 
//    
//    /// <summary>
//    /// returns the average raw open price for the given ticker, 
//    /// at the specified time interval 
//    /// </summary>
//    public static double GetAverageRawOpenPrice( string ticker,
//                                                DateTime firstQuoteDate,
//                                                DateTime lastQuoteDate)
//                                                
//    {
//      double returnValue = 0;
//    	DataTable dt;
//      string sql = "SELECT quotes.quTicker, tickers.tiCompanyName, " +
//                    "Avg(quotes.quOpen) AS AverageRawOpenPrice " +
//                  "FROM (quotes INNER JOIN tickers ON quotes.quTicker=tickers.tiTicker) " +
//                  "INNER JOIN tickers_tickerGroups ON tickers.tiTicker=tickers_tickerGroups.ttTiId " +
//                  "WHERE quotes.quTicker ='" + ticker + 
//      			  "' AND quotes.quDate Between " + SQLBuilder.GetDateConstant(firstQuoteDate) + " " +
//                  "AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + " " +
//                  "GROUP BY quotes.quTicker, tickers.tiCompanyName";
//      dt = SqlExecutor.GetDataTable( sql );
//      if(dt.Rows.Count > 0)
//      {  
//        if( dt.Rows[0]["AverageRawOpenPrice"] is double )
//        //cast is possible
//            returnValue = (double)dt.Rows[0]["AverageRawOpenPrice"];
//      }
//      return returnValue;
//      	
//     }
//
//	/// <summary>
//    /// returns raw open price's standard deviation for the given ticker,
//    /// at the specified time interval
//	/// </summary>
//    public static double GetRawOpenPriceStdDeviation( string ticker,
//                                                DateTime firstQuoteDate,
//                                                DateTime lastQuoteDate)
//                                                
//    {
//      double returnValue = Double.MaxValue;
//			DataTable dt;
//      string sql = "SELECT quotes.quTicker, tickers.tiCompanyName, " +
//                    "StDev(quotes.quOpen) AS RawOpenPriceStdDev " +
//                  "FROM (quotes INNER JOIN tickers ON quotes.quTicker=tickers.tiTicker) " +
//                  "INNER JOIN tickers_tickerGroups ON tickers.tiTicker=tickers_tickerGroups.ttTiId " +
//                  "WHERE quotes.quTicker ='" + ticker + 
//      			  "' AND quotes.quDate Between " + SQLBuilder.GetDateConstant(firstQuoteDate) + " " +
//                  "AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + " " +
//                  "GROUP BY quotes.quTicker, tickers.tiCompanyName";
//      dt = SqlExecutor.GetDataTable( sql );
//      if(dt.Rows.Count > 0)
//      {  
//        if( dt.Rows[0]["RawOpenPriceStdDev"] is double )
//        //cast is possible
//            returnValue = (double)dt.Rows[0]["RawOpenPriceStdDev"];
//      }
//      return returnValue;
//     }
//    
//	  /// <summary>
//    /// Returns number of days for which raw close was greater than raw open 
//    /// for the given interval of days (for the given ticker).
//    /// </summary>
//    public static int GetNumberOfOpenToCloseWinningDays(string ticker,
//                                                   DateTime firstQuoteDate,
//                               										 DateTime lastQuoteDate)
//    {
//      DataTable dt;
//      int returnValue = 0;
//    	string sql = "SELECT Count(*) AS CloseToOpenWinningDays " +
//      						"FROM quotes WHERE " + 
//    							"quotes.quDate Between " + SQLBuilder.GetDateConstant(firstQuoteDate) + " " +
//                  "AND " + SQLBuilder.GetDateConstant(lastQuoteDate) + " " +
//                  "AND " + "quotes.quTicker='" + ticker + "' " +
//    							"AND quotes.quClose > quotes.quOpen";
//    	
//     	dt = SqlExecutor.GetDataTable( sql );
//     	if(dt.Rows.Count > 0)
//     	{
//     		if(dt.Rows[0][0] is int)
//     			returnValue = (int)dt.Rows[0][0];
//     	}
//     	return returnValue;
//    }
	
  	/// <summary>
		/// returns the bars DataTable for the given ticker
		/// </summary>
		/// <param name="instrumentKey">ticker whose bars are to be returned</param>
		/// <returns></returns>
		public static DataTable GetTickerBars( string instrumentKey )
		{
			string sql = "select * from bars where " + Bars.TickerFieldName + "='" + instrumentKey + "' " +
				"order by " + Bars.DateTimeForOpen;
			return SqlExecutor.GetDataTable( sql );
		}
    
    /// <summary>
    /// returns the bars DataTable for the given ticker
    /// </summary>
    /// <param name="instrumentKey">ticker whose quotes are to be returned</param>
    /// <param name="firstBarDateTime">The first bar date time</param>
    /// <param name="lastBarDateTime">The last bar date time</param>
    /// <returns></returns>
    public static DataTable GetTickerBars( string instrumentKey, DateTime firstBarDateTime,
                                              DateTime lastBarDateTime)
    {
      string sql = "select * from bars where " + Bars.TickerFieldName + "='" + instrumentKey + "' " +
        "AND " + Bars.DateTimeForOpen + " BETWEEN " + SQLBuilder.GetDateConstant(firstBarDateTime) + 
        " AND " + SQLBuilder.GetDateConstant(lastBarDateTime) + " " +
        "order by " + Bars.DateTimeForOpen;
      return SqlExecutor.GetDataTable( sql );
    }
		/// <summary>
		/// Returns the bars for the given instrument , since startDateTime to endDateTime
		/// </summary>
		/// <param name="tickerOrGroupID">The symbol of a ticker or the groupID corresponding to a specific set of tickers</param>
		/// <param name="startDateTime"></param>
		/// <param name="endDateTime"></param>
		/// <returns></returns>
		public static void SetDataTable( string tickerOrGroupID , DateTime startDateTime , DateTime endDateTime ,
			DataTable dataTable)
		{
			string sql;
			if(Tickers_tickerGroups.HasTickers(tickerOrGroupID))
				sql =	"select * from bars INNER JOIN tickers_tickerGroups ON " +
					"bars." + Bars.TickerFieldName + "=tickers_tickerGroups." + Tickers_tickerGroups.Ticker + " " +
					"where " + Tickers_tickerGroups.GroupID + "='" + tickerOrGroupID + "' " +
					"and " + Bars.DateTimeForOpen + ">=" + SQLBuilder.GetDateTimeConstant( startDateTime ) + " " +
					"and " + Bars.DateTimeForOpen + "<=" + SQLBuilder.GetDateTimeConstant( endDateTime ) + " " +
					"order by " + Bars.DateTimeForOpen;
			else
				sql =	"select * from bars " +
					"where " + Bars.TickerFieldName + "='" + tickerOrGroupID + "' " +
					"and " + Bars.DateTimeForOpen + ">=" + SQLBuilder.GetDateTimeConstant( startDateTime ) + " " +
					"and " + Bars.DateTimeForOpen + "<=" + SQLBuilder.GetDateTimeConstant( endDateTime ) + " " +
					"order by " + Bars.DateTimeForOpen;
			
			SqlExecutor.SetDataTable( sql , dataTable );
		}

		#region SetDataTable for tickerList
		private static string setDataTable_getTickerListWhereClause_getSingleTickerWhereClause(
			string ticker )
		{
			return "(" + Bars.TickerFieldName + "='" + ticker + "')";
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
		/// Builds a Bars data table containing a row for each ticker in the
		/// collection, with the bars for the given DateTime
		/// </summary>
		/// <param name="tickerCollection">Tickers whose quotes are to be fetched</param>
		/// <param name="dateTime">Date for the quotes to be fetched</param>
		/// <param name="dataTable">Output parameter</param>
		public static void SetDataTable( ICollection tickerCollection , DateTime dateTime , DataTable dataTable)
		{
			string sql;
			sql =	"select * from bars " +
				"where " + setDataTable_getTickerListWhereClause( tickerCollection ) +
				" and " + Bars.DateTimeForOpen + "=" + SQLBuilder.GetDateTimeConstant( dateTime ) + " " +
				"order by " + Bars.TickerFieldName;
			
			SqlExecutor.SetDataTable( sql , dataTable );
		}
		#endregion

	}
}
