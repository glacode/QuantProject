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
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;

using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.ADT.Timing;

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
		
		public Bars( string ticker)
		{
			
		}
		
		public Bars( string ticker , DateTime startDateTime , DateTime endDateTime )
		{
			
		}
		/// <summary>
		/// Returns the dateTime of the first bar for the given ticker
		/// </summary>
		/// <param name="ticker">ticker for which the dateTime of the first bar has to be returned</param>
		/// <param name="intervalFrameInSeconds">interval frame in seconds for the ticker's bars</param>
		/// <returns></returns>
		public static DateTime GetFirstBarDateTime( string ticker, int intervalFrameInSeconds )
		{
			DataTable dataTable = SqlExecutor.GetDataTable(
				"select min(" + Bars.DateTimeForOpen + ") as minDate from bars " +
				"where " + Bars.TickerFieldName + "='" + ticker + "' and " +
				Bars.IntervalFrameInSeconds + "='" + intervalFrameInSeconds + "' " +
				"group by " + Bars.TickerFieldName + ")" );
			return (DateTime)(dataTable.Rows[ 0 ][ "minDate" ]);
		}
		/// <summary>
		/// Returns the dateTime of the last bar for the given ticker
		/// </summary>
		/// <param name="ticker">ticker for which the dateTime of the last bar has to be returned</param>
		/// <param name="intervalFrameInSeconds">interval frame in seconds for the ticker's bars</param>
		/// <returns></returns>
		public static DateTime GetLastBarDateTime( string ticker, int intervalFrameInSeconds )
		{
			DataTable dataTable = SqlExecutor.GetDataTable(
				"select * from bars where " + Bars.TickerFieldName + "='" + ticker + "' and " +
				Bars.IntervalFrameInSeconds + "='" + intervalFrameInSeconds + "' " +
				"order by " + Bars.DateTimeForOpen + " DESC");
			return (DateTime)(dataTable.Rows[0][ Bars.DateTimeForOpen ]);
		}
		/// <summary>
		/// Returns the number of Bars for the given ticker
		/// </summary>
		/// <param name="ticker">ticker for which the number of bars has to be returned</param>
		/// <param name="intervalFrameInSeconds">interval frame in seconds for the ticker's bars</param>
		/// <returns></returns>
		public static int GetNumberOfBars( string ticker, int intervalFrameInSeconds )
		{
			DataTable dataTable = SqlExecutor.GetDataTable(
				"select * from bars " +
				"where " + Bars.TickerFieldName + "='" + ticker + "' and " +
				Bars.IntervalFrameInSeconds + "='" + intervalFrameInSeconds + "' " );
			return dataTable.Rows.Count;
		}

		/// <summary>
		/// Returns the number of bars at which the given ticker has been effectively traded
		/// (volume > 0)
		/// </summary>
		/// <param name="ticker">ticker for which the number of bars has to be returned</param>
		/// <param name="intervalFrameInSeconds">interval frame in seconds for the ticker's bars</param>
		/// <returns></returns>
		public static int GetNumberOfBarsWithEffectiveTrades( string ticker, DateTime firstDateTime,
		                                                     DateTime lastDateTime, int intervalFrameInSeconds)
		{
			DataTable dataTable = SqlExecutor.GetDataTable(
				"select * from bars " +
				"WHERE " + Bars.TickerFieldName + "='" + ticker + "' and " +
				Bars.IntervalFrameInSeconds + "='" + intervalFrameInSeconds + "' " +
				"and " + Bars.Volume + ">0" + " and " + Bars.DateTimeForOpen + " BETWEEN " + SQLBuilder.GetDateConstant(firstDateTime) + " " +
				"and " + SQLBuilder.GetDateConstant(lastDateTime) );
			return dataTable.Rows.Count;
		}
		
		#region GetOpen, GetHigh, GetLow, GetClose
		
		private static double getValueFromBar( string ticker, DateTime dateTime, int intervalFrameInSeconds,
		                                     	 string fieldNameContainingValue)
		{
			double returnValue = Double.NaN;
			string sqlQuery = "select " + fieldNameContainingValue + " from bars " +
				"where " + Bars.TickerFieldName + "='" + ticker + "' and " +
				Bars.IntervalFrameInSeconds + "='" + intervalFrameInSeconds + "' " +
				"and " + Bars.DateTimeForOpen + "=" + SQLBuilder.GetDateTimeConstant(dateTime);
			DataTable dataTable = SqlExecutor.GetDataTable( sqlQuery );
			if( dataTable.Rows.Count > 0 )
				returnValue = (double)dataTable.Rows[0][0];
			else
				throw new EmptyQueryException( sqlQuery );
			return returnValue;
		}
		/// <summary>
		/// Returns the open for the given ticker for the given bar that opens at the specified date time
		/// </summary>
		/// <param name="ticker">ticker for which the raw open has to be returned</param>
		/// <param name="intervalFrameInSeconds">interval frame in seconds for the ticker's bar</param>
		/// <returns></returns>
		public static double GetOpen( string ticker, DateTime dateTime, int intervalFrameInSeconds )
		{
			return getValueFromBar(ticker, dateTime, intervalFrameInSeconds, Bars.Open);
		}
		/// <summary>
		/// Returns the high for the given ticker for the given bar that opens at the specified date time
		/// </summary>
		/// <param name="ticker">ticker for which the raw high has to be returned</param>
		/// <param name="intervalFrameInSeconds">interval frame in seconds for the ticker's bar</param>
		/// <returns></returns>
		public static double GetHigh( string ticker, DateTime dateTime, int intervalFrameInSeconds )
		{
			return getValueFromBar(ticker, dateTime, intervalFrameInSeconds, Bars.High);
		}
		/// <summary>
		/// Returns the low for the given ticker for the given bar that opens at the specified date time
		/// </summary>
		/// <param name="ticker">ticker for which the raw low has to be returned</param>
		/// <param name="intervalFrameInSeconds">interval frame in seconds for the ticker's bar</param>
		/// <returns></returns>
		public static double GetLow( string ticker, DateTime dateTime, int intervalFrameInSeconds )
		{
			return getValueFromBar(ticker, dateTime, intervalFrameInSeconds, Bars.Low);
		}
		/// <summary>
		/// Returns the close for the given ticker for the given bar that opens at the specified date time
		/// </summary>
		/// <param name="ticker">ticker for which the raw close has to be returned</param>
		/// <param name="intervalFrameInSeconds">interval frame in seconds for the ticker's bar</param>
		/// <returns></returns>
		public static double GetClose( string ticker, DateTime dateTime, int intervalFrameInSeconds )
		{
			return getValueFromBar(ticker, dateTime, intervalFrameInSeconds, Bars.Close);
		}
		
		#endregion
		
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
		/// returns the bars DataTable for the given ticker
		/// </summary>
		/// <param name="ticker">ticker whose bars are to be returned</param>
		/// <param name="intervalFrameInSeconds">interval frame in seconds for the ticker's bars</param>
		/// <returns></returns>
		public static DataTable GetTickerBars( string ticker, int intervalFrameInSeconds )
		{
			string sql = "select * from bars " +
				"where " + Bars.TickerFieldName + "='" + ticker + "' and " +
				Bars.IntervalFrameInSeconds + "='" + intervalFrameInSeconds + "' " +
				"order by " + Bars.DateTimeForOpen;
			return SqlExecutor.GetDataTable( sql );
		}
		
		/// <summary>
		/// returns the bars DataTable for the given ticker
		/// </summary>
		/// <param name="ticker">ticker whose quotes are to be returned</param>
		/// <param name="firstBarDateTime">The first bar date time</param>
		/// <param name="lastBarDateTime">The last bar date time</param>
		/// <param name="intervalFrameInSeconds">interval frame in seconds for the ticker's bars</param>
		/// <returns></returns>
		public static DataTable GetTickerBars( string ticker, DateTime firstBarDateTime,
		                                      DateTime lastBarDateTime, int intervalFrameInSeconds)
		{
			string sql = "select * from bars " +
				"where " + Bars.TickerFieldName + "='" + ticker + "' and " +
				Bars.IntervalFrameInSeconds + "='" + intervalFrameInSeconds + "' " +
				"and " + Bars.DateTimeForOpen + " between " + SQLBuilder.GetDateConstant(firstBarDateTime) + " " +
				"and " + SQLBuilder.GetDateConstant(lastBarDateTime) + " " +
				"order by " + Bars.DateTimeForOpen;
			return SqlExecutor.GetDataTable( sql );
		}
		
		/// <summary>
		/// returns the bars DataTable for the given ticker
		/// </summary>
		/// <param name="ticker">ticker whose quotes are to be returned</param>
		/// <param name="exchange">the exchange where the ticker was traded</param>
		/// <param name="firstBarDateTime">The first bar date time</param>
		/// <param name="lastBarDateTime">The last bar date time</param>
		/// <param name="intervalFrameInSeconds">interval frame in seconds for
		/// the ticker's bars</param>
		/// <returns></returns>
		public static DataTable GetTickerBars(
			string ticker , string exchange ,
			DateTime firstBarDateTime , DateTime lastBarDateTime , long intervalFrameInSeconds )
		{
			string sql = "select * from bars " +
				"where " + Bars.TickerFieldName + "='" + ticker + "' and " +
				Bars.Exchange + "='" + exchange + "' and " +
				Bars.IntervalFrameInSeconds + "=" + intervalFrameInSeconds + " " +
				"and " + Bars.DateTimeForOpen + " between " +
				SQLBuilder.GetDateTimeConstant(firstBarDateTime) + " " +
				"and " + SQLBuilder.GetDateTimeConstant(lastBarDateTime) + " " +
				"order by " + Bars.DateTimeForOpen;
			return SqlExecutor.GetDataTable( sql );
		}

		
		/// <summary>
		/// Returns the bars for the given instrument , since startDateTime to endDateTime
		/// </summary>
		/// <param name="tickerOrGroupID">The symbol of a ticker or the groupID corresponding to a specific set of tickers</param>
		/// <param name="startDateTime"></param>
		/// <param name="endDateTime"></param>
		/// <param name="intervalFrameInSeconds">interval frame in seconds for the ticker's bars</param>
		/// <returns></returns>
		public static void SetDataTable( string tickerOrGroupID , DateTime startDateTime , DateTime endDateTime ,
		                                DataTable dataTable, int intervalFrameInSeconds)
		{
			string sql;
			if(Tickers_tickerGroups.HasTickers(tickerOrGroupID))
				sql =	"select * from bars INNER JOIN tickers_tickerGroups ON " +
					"bars." + Bars.TickerFieldName + "=tickers_tickerGroups." + Tickers_tickerGroups.Ticker + " " +
					"where " + Tickers_tickerGroups.GroupID + "='" + tickerOrGroupID + "' and " +
					Bars.IntervalFrameInSeconds + "=" + intervalFrameInSeconds + " " +
					"and " + Bars.DateTimeForOpen + ">=" + SQLBuilder.GetDateTimeConstant( startDateTime ) + " " +
					"and " + Bars.DateTimeForOpen + "<=" + SQLBuilder.GetDateTimeConstant( endDateTime ) + " " +
					"order by " + Bars.DateTimeForOpen;
			else
				sql =	"select * from bars " +
					"where " + Bars.TickerFieldName + "='" + tickerOrGroupID + "' and " +
					Bars.IntervalFrameInSeconds + "=" + intervalFrameInSeconds + " " +
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
		/// <param name="intervalFrameInSeconds">interval frame in seconds for the ticker's bars</param>
		public static void SetDataTable( ICollection tickerCollection , DateTime dateTime , DataTable dataTable, int intervalFrameInSeconds)
		{
			string sql;
			sql =	"select * from bars " +
				"where " + setDataTable_getTickerListWhereClause( tickerCollection ) + " and " +
				Bars.IntervalFrameInSeconds + "='" + intervalFrameInSeconds + "' " +
				" and " + Bars.DateTimeForOpen + "=" + SQLBuilder.GetDateTimeConstant( dateTime ) + " " +
				"order by " + Bars.TickerFieldName;
			
			SqlExecutor.SetDataTable( sql , dataTable );
		}
		#endregion
		
		#region SetDataTable
		
		private static string getSql(
			string ticker , DateTime firstDate , DateTime lastDate ,
			Time firstBarOpenTimeInNewYorkTimeZone , Time lastBarOpenTimeInNewYorkTimeZone ,
			int intervalFrameInSeconds )
		{
			string sql =
				"select baDateTimeForOpen from bars " +
				"where (baTicker='" + ticker + "') and " +
				"(baInterval=" + intervalFrameInSeconds + ") and" +
				"(baDateTimeForOpen>=" +
				SQLBuilder.GetDateConstant( firstDate ) + ") and" +
				"(baDateTimeForOpen<" +
				SQLBuilder.GetDateConstant( lastDate.AddDays( 1 ) )
				+ ") and (" +
				SQLBuilder.GetFilterForTime(
					"baDateTimeForOpen" , SqlComparisonOperator.GreaterThanOrEqual ,
					firstBarOpenTimeInNewYorkTimeZone ) +
//				"(Format([baDateTimeForOpen],'hh:mm:ss')>='" +
//				this.getSqlTimeConstantForFirstDailyBar() +
				") and (" +
				SQLBuilder.GetFilterForTime(
					"baDateTimeForOpen" , SqlComparisonOperator.LessThanOrEqual ,
					lastBarOpenTimeInNewYorkTimeZone ) +
//				"(Format([baDateTimeForOpen],'hh:mm:ss')<='" +
//				this.getSqlTimeConstantForLastDailyBar() +
				");";
			return sql;
		}
		
		/// <summary>
		/// fills the parameter dataTable with all the daily bars between
		/// firstBarOpenTimeInNewYorkTimeZone and lastBarOpenTimeInNewYorkTimeZone
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="firstDate"></param>
		/// <param name="lastDate"></param>
		/// <param name="firstBarOpenTimeInNewYorkTimeZone"></param>
		/// <param name="lastBarOpenTimeInNewYorkTimeZone"></param>
		/// <param name="dataTable"></param>
		/// <param name="intervalFrameInSeconds"></param>
		public static void SetDataTable(
			string ticker , DateTime firstDate , DateTime lastDate ,
			Time firstBarOpenTimeInNewYorkTimeZone , Time lastBarOpenTimeInNewYorkTimeZone ,
			DataTable dataTable, int intervalFrameInSeconds )
		{
			string sql = Bars.getSql(
				ticker , firstDate , lastDate ,
				firstBarOpenTimeInNewYorkTimeZone , lastBarOpenTimeInNewYorkTimeZone ,
				intervalFrameInSeconds );
			SqlExecutor.SetDataTable( sql , dataTable );
		}
		#endregion SetDataTable
		
		
		#region SetDataTable

		#region getSql
		
		#region getFilterForTimeList
		
		#region getFilterForTimeList_withTailToBeRemoved
		private static string getFilterForTimeList( Time time )
		{
			string filterForTimeList =
				SQLBuilder.GetFilterForTime(
					"baDateTimeForOpen" , SqlComparisonOperator.Equal , time ) +
				" OR ";
			return filterForTimeList;
		}
		private static string getFilterForTimeList_withTailToBeRemoved(
			List<Time> dailyTimes )
		{
			string filterForTimeList_withTailToBeRemoved = "";
			foreach ( Time time in dailyTimes )
				filterForTimeList_withTailToBeRemoved +=
					Bars.getFilterForTimeList( time );
			return filterForTimeList_withTailToBeRemoved;
		}
		#endregion getFilterForTimeList_withTailToBeRemoved
		
		private static string getFilterForTimeList(
			List<Time> dailyTimes )
		{
			// TO DO
			// a "where ... in ..." statement could be used here, it would lead to a
			// shorter Sql statement
			string filterWithTailToBeRemoved =
				Bars.getFilterForTimeList_withTailToBeRemoved( dailyTimes );
			string filterForTimeList =
				filterWithTailToBeRemoved.Substring(
					0 , filterWithTailToBeRemoved.Length - " OR ".Length );
			return filterForTimeList;
		}
		#endregion getFilterForTimeList
		
		private static string getSql(
			string ticker , DateTime firstDate , DateTime lastDate ,
			List<Time> dailyTimes , int intervalFrameInSeconds )
		{
			string sql =
				"select baDateTimeForOpen from bars " +
				"where (baTicker='" + ticker + "') and " +
				"(baInterval=" + intervalFrameInSeconds + ") and" +
				"(baDateTimeForOpen>=" +
				SQLBuilder.GetDateConstant( firstDate ) + ") and" +
				"(baDateTimeForOpen<" +
				SQLBuilder.GetDateConstant( lastDate.AddDays( 1 ) )
				+ ") and (" +
				Bars.getFilterForTimeList( dailyTimes ) +
//				SQLBuilder.GetFilterForTime(
//					"baDateTimeForOpen" , SqlComparisonOperator.GreaterThanOrEqual ,
//					firstBarOpenTimeInNewYorkTimeZone ) +
				////				"(Format([baDateTimeForOpen],'hh:mm:ss')>='" +
				////				this.getSqlTimeConstantForFirstDailyBar() +
//				") and (" +
//				SQLBuilder.GetFilterForTime(
//					"baDateTimeForOpen" , SqlComparisonOperator.LessThanOrEqual ,
//					lastBarOpenTimeInNewYorkTimeZone ) +
				////				"(Format([baDateTimeForOpen],'hh:mm:ss')<='" +
				////				this.getSqlTimeConstantForLastDailyBar() +
				");";
			return sql;
		}
		#endregion getSql
		
		/// <summary>
		/// fills the parameter dataTable with all the daily bars at the given
		/// daily times
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="firstDate"></param>
		/// <param name="lastDate"></param>
		/// <param name="dailyTimes"></param>
		/// <param name="dataTable"></param>
		/// <param name="intervalFrameInSeconds"></param>
		public static void SetDataTable(
			string ticker , DateTime firstDate , DateTime lastDate ,
			List<Time> dailyTimes ,
			DataTable dataTable, int intervalFrameInSeconds )
		{
			string sql = Bars.getSql(
				ticker , firstDate , lastDate ,
				dailyTimes ,
				intervalFrameInSeconds );
			SqlExecutor.SetDataTable( sql , dataTable );
		}
		#endregion SetDataTable
		
		#region AddBar
		
		#region getSqlCommand
		
//		#region getSqlCommand_getValues
//		private string getSqlCommandFor_AddBar( double value )
//		{
//			string formattedValue =
//				value.ToString().Replace( ',' , '.' );
//			return formattedValue;
//		}
		private static string getSqlCommandFor_AddBar_getValues(
			string ticker , string exchange , DateTime dateTimeForOpenInESTTime , long interval ,
			double open , double high , double low , double close , double volume )
		{
			string values =
				"'" + ticker + "' , " +
				"'" + exchange + "' , " +
				SQLBuilder.GetDateTimeConstant( dateTimeForOpenInESTTime ) + " , " +
				interval + " , " +
				SQLBuilder.FormatDoubleForSql( open ) + " , " +
				SQLBuilder.FormatDoubleForSql( high ) + " , " +
				SQLBuilder.FormatDoubleForSql( low ) + " , " +
				SQLBuilder.FormatDoubleForSql( close ) + " , " +
				volume + " , " +
				SQLBuilder.GetDateTimeConstant( DateTime.Now );
			return values;
		}
//		#endregion getSqlCommand_getValues
		
		private static string getSqlCommandFor_AddBar(
			string ticker , string exchange , DateTime dateTimeForOpenInESTTime , long interval ,
			double open , double high , double low , double close , double volume )
		{
			string sqlCommand =
				"INSERT INTO bars " +
				"( baTicker, baExchange, baDateTimeForOpen, baInterval, " +
				"baOpen, baHigh, baLow, baClose, baVolume, baWhenAdded ) " +
				"SELECT " +
				Bars.getSqlCommandFor_AddBar_getValues(
					ticker , exchange , dateTimeForOpenInESTTime , interval ,
					open , high , low , close , volume ) + ";";
			return sqlCommand;
		}
		#endregion getSqlCommand
		
		/// <summary>
		/// Adds a bar to the database. A new record is added to the table 'bars'
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="exchange"></param>
		/// <param name="dateTimeForOpenInESTTime"></param>
		/// <param name="interval"></param>
		/// <param name="open"></param>
		/// <param name="high"></param>
		/// <param name="low"></param>
		/// <param name="close"></param>
		/// <param name="volume"></param>
		public static void AddBar(
			string ticker , string exchange , DateTime dateTimeForOpenInESTTime , long interval ,
			double open , double high , double low , double close , double volume )
		{
			string sqlCommand =
				Bars.getSqlCommandFor_AddBar(
					ticker , exchange , dateTimeForOpenInESTTime , interval ,
					open , high , low , close , volume );
//			try
//			{
			SqlExecutor.ExecuteNonQuery( sqlCommand );
//			}
//			catch( Exception exception )
//			{
//				if ( !DataBase.IsExceptionForForbiddenDataDuplication( exception ) )
//					// the exception is not due to a duplicated bar
//					throw exception;
//			}
		}
		#endregion AddBar
		
		/// <summary>
		/// true iif the database contains a bar for the given key values
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="exchange"></param>
		/// <param name="dateTimeForOpenInESTTime"></param>
		/// <param name="interval"></param>
		public static bool ContainsBar(
			string ticker , string exchange , DateTime dateTimeForOpenInESTTime , long interval )
		{
			DataTable bars = Bars.GetTickerBars(
				ticker , exchange , dateTimeForOpenInESTTime , dateTimeForOpenInESTTime , interval );
			bool containsBar = ( bars.Rows.Count > 0 );
			return containsBar;
		}
	}
}
