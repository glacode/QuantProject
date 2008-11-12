/*
QuantProject - Quantitative Finance Library

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
using System.Runtime.Serialization;
using System.Text;

using QuantProject.ADT;
using QuantProject.ADT.Collections;
using QuantProject.ADT.Statistics;
using QuantProject.ADT.Histories;
using QuantProject.ADT.Timing;
using QuantProject.DataAccess;
using QuantProject.DataAccess.Tables;

namespace QuantProject.Data.DataTables
{
	/// <summary>
	/// DataTable for bars table data
	/// </summary>
	[Serializable]
	public class Bars : DataTable
	{
		public static string TickerFieldName = "baTicker";	// Ticker cannot be simply used because
		public static string Exchange = "baExchange";
		public static string DateTimeForOpen = "baDateTimeForOpen";
		public static string IntervalFrameInSeconds = "baInterval";
		public static string Open = "baOpen";
		public static string High = "baHigh";
		public static string Low = "baLow";
		public static string Close = "baClose";
		public static string Volume = "baVolume";
		private History history;
		
		/// <summary>
		/// Builds a Bars data table containing a row for each ticker in the
		/// collection, with the bars for the given DateTime
		/// </summary>
		/// <param name="tickerCollection">Tickers whose quotes are to be fetched</param>
		/// <param name="dateTime">DateTime for the bars to be fetched</param>
		public Bars( ICollection tickerCollection , DateTime dateTime )
		{
			QuantProject.DataAccess.Tables.Bars.SetDataTable(
				tickerCollection , dateTime , this );
		}
		public Bars( string ticker , DateTime startDateTime , DateTime endDateTime )
		{
			this.fillDataTable( ticker , startDateTime , endDateTime );
		}

		/// <summary>
		/// builds a Bars data table containing the ticker's bars for the
		/// market dateTimes contained in the marketDateTimes SortedList
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="marketDateTimes"></param>
		public Bars( string ticker , SortedList marketDateTimes )
		{
			DateTime firstDateTime = (DateTime)marketDateTimes.GetByIndex( 0 );
			DateTime lastDateTime = (DateTime)marketDateTimes.GetByIndex(
				marketDateTimes.Count - 1 );
			this.fillDataTable( ticker , firstDateTime , lastDateTime );
			this.removeNonContainedDateTimes( marketDateTimes );
		}
		#region removeNonContainedDateTimes
		private ArrayList	removeNonContainedDateTimes_getDataRowsToBeRemoved(
			SortedList marketDateTimes )
		{
			ArrayList dataRowsToBeRemoved = new ArrayList();
			foreach( DataRow dataRow in this.Rows )
				if ( !marketDateTimes.ContainsKey(
					(DateTime)dataRow[ Bars.DateTimeForOpen ] ) )
				dataRowsToBeRemoved.Add( dataRow );
			return dataRowsToBeRemoved;
		}
		private void removeDataRows( ICollection dataRowsToBeRemoved )
		{
			foreach ( DataRow dataRowToBeRemoved in dataRowsToBeRemoved )
				this.Rows.Remove( dataRowToBeRemoved );
		}
		private void removeNonContainedDateTimes( SortedList marketDateTimes )
		{
			ArrayList dataRowsToBeRemoved =
				this.removeNonContainedDateTimes_getDataRowsToBeRemoved(
					marketDateTimes );
			this.removeDataRows( dataRowsToBeRemoved );
		}
		#endregion

		public Bars( string ticker )
		{
			this.fillDataTable(
				ticker ,
				QuantProject.DataAccess.Tables.Bars.GetFirstBarDateTime( ticker ) ,
				QuantProject.DataAccess.Tables.Bars.GetLastBarDateTime( ticker ) );
		}
		public Bars(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
		private void fillDataTable( string ticker , DateTime startDateTime , DateTime endDateTime )
		{
			QuantProject.DataAccess.Tables.Bars.SetDataTable(
				ticker , startDateTime , endDateTime , this );
			this.setPrimaryKeys();
		}
		private void setPrimaryKeys()
		{
			DataColumn[] columnPrimaryKeys = new DataColumn[1];
			columnPrimaryKeys[0] = this.Columns[Bars.DateTimeForOpen];
			this.PrimaryKey = columnPrimaryKeys;
		}

		/// <summary>
		/// returns date times when the ticker was exchanged, within a given
		/// date time interval
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="firstDateTime">begin interval</param>
		/// <param name="lastDateTime">end interval</param>
		/// <returns></returns>
		public static History GetMarketDateTimes( string ticker ,
		                                         DateTime firstDateTime , DateTime lastDateTime )
		{
			Bars bars = new Bars( ticker , firstDateTime , lastDateTime );
			History marketDateTimes = new History();
//			int i = 0;
			foreach ( DataRow dataRow in bars.Rows )
//			{
				marketDateTimes.Add(
					(DateTime)dataRow[ Bars.DateTimeForOpen ] ,
					(DateTime)dataRow[ Bars.DateTimeForOpen ] );
//				i++;
//			}
			return marketDateTimes;
		}
		
		#region GetMarketDateTimes
		
//		#region checkIfAreTimes
//		private static void checkIfIsTime( DateTime timeCandidate )
//		{
//			if ( !ExtendedDateTime.IsTime( timeCandidate ) )
//				// the timeCandidate does not represent a time
//				throw new Exception(
//					"The given DateTime was expected to be a time, but " +
//					"it is not. Use QuantProject.ADT.ExtendedDateTime.GetTime() " +
//					"to build such times.";
//			}
//		private static void checkIfAreTimes( List< DateTime > dailyTimes )
//		{
//			foreach ( DateTime dateTime in dailyTimes )
//				this.checkIfIsTime( DateTime dateTime );
//		}
//		#endregion checkIfAreTimes
		
		#region getMarketDateTimes
		
		#region removeMissingTimes
		
		#region removeMissingTime
		private static bool isTimeInDailyTimes(
			DateTime dateTime , List< Time > dailyTimes )
		{
			Time time = new Time( dateTime );
			bool isInDailyTimes =
				dailyTimes.Contains( time );
			return isInDailyTimes;
		}
		private static void removeMissingTime(
			DateTime candidateToBeRemoved ,
			List< Time > dailyTimes , History marketDateTimes )
		{
			if ( !Bars.isTimeInDailyTimes( candidateToBeRemoved , dailyTimes ) )
				// the candidate's time is not in the given list of daily times
				marketDateTimes.Remove( candidateToBeRemoved );
		}
		#endregion removeMissingTime
		
		private static void removeMissingTimes(
			List< Time > dailyTimes , History marketDateTimes )
		{
			foreach ( DateTime dateTime in marketDateTimes )
				Bars.removeMissingTime(
					dateTime , dailyTimes , marketDateTimes );
		}
		private static History getMarketDateTimes(
			string ticker ,	DateTime firstDateTime , DateTime lastDateTime ,
			List< Time > dailyTimes )
		{
			History marketDateTimes =
				Bars.GetMarketDateTimes(
					ticker , firstDateTime  , lastDateTime );
			Bars.removeMissingTimes( dailyTimes , marketDateTimes );
			return marketDateTimes;
		}
		#endregion removeMissingTimes
		
		#endregion getMarketDateTimes
		
		/// <summary>
		/// returns date times when the ticker was exchanged, within a given
		/// date time interval, but only when the time is in dailyTimes
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="firstDateTime"></param>
		/// <param name="lastDateTime"></param>
		/// <param name="dailyTimes"></param>
		/// <returns></returns>
		public static History GetMarketDateTimes(
			string ticker ,	DateTime firstDateTime , DateTime lastDateTime ,
			List< Time > dailyTimes )
		{
//			Bars.checkIfAreTimes( dailyTimes );
			History marketDateTimes =
				Bars.getMarketDateTimes(
					ticker , firstDateTime , lastDateTime , dailyTimes );
			return marketDateTimes;
		}
		#endregion GetMarketDateTimes
		
		#region GetCommonMarketDateTimes
		private static Hashtable getMarketDateTimes( ICollection tickers , DateTime firstDateTime ,
		                                            DateTime lastDateTime )
		{
			Hashtable marketDateTimes = new Hashtable();
			foreach ( string ticker in tickers )
				if ( !marketDateTimes.ContainsKey( ticker ) )
			{
				History marketDateTimesForSingleTicker =
					GetMarketDateTimes( ticker , firstDateTime , lastDateTime );
				marketDateTimes.Add( ticker , marketDateTimesForSingleTicker );
			}
			return marketDateTimes;
		}
		private static bool isCommonDateTime( ICollection tickers , DateTime dateTime ,
		                                     Hashtable marketDateTimes )
		{
			bool itIsCommon = true;
			foreach ( string ticker in tickers )
				itIsCommon = itIsCommon &&
					((List< DateTime >)marketDateTimes[ ticker ]).Contains( dateTime );
			return itIsCommon;
		}
		private static void getCommonMarketDateTimes_ifTheCaseAdd( ICollection tickers , DateTime dateTime ,
		                                                          Hashtable marketDateTimes , AdvancedSortedList commonMarketDateTimes )
		{
			if ( isCommonDateTime( tickers , dateTime , marketDateTimes ) )
				commonMarketDateTimes.Add( dateTime , dateTime );
		}
		private static SortedList getCommonMarketDateTimes( ICollection tickers ,
		                                                   DateTime firstDateTime , DateTime lastDateTime , Hashtable marketDateTimes )
		{
			AdvancedSortedList commonMarketDateTimes = new AdvancedSortedList();
			DateTime currentDateTime = firstDateTime;
			while ( currentDateTime <= lastDateTime )
			{
				getCommonMarketDateTimes_ifTheCaseAdd( tickers ,
				                                      currentDateTime , marketDateTimes , commonMarketDateTimes );
				currentDateTime = currentDateTime.AddMinutes( 1 );
				//CHECK this statement: it could be cause great delay ...
			}
			return commonMarketDateTimes;
		}

		public static SortedList GetCommonMarketDateTimes( ICollection tickers ,
		                                                  DateTime firstDateTime , DateTime lastDateTime )
		{
			Hashtable marketDateTimes = getMarketDateTimes( tickers , firstDateTime , lastDateTime );
			return getCommonMarketDateTimes( tickers , firstDateTime , lastDateTime , marketDateTimes );
		}

		#endregion
		
		/// <summary>
		/// Gets the ticker whose bars are contained into the bars object
		/// </summary>
		/// <returns></returns>
		public string Ticker
		{
			get{ return ((string)this.Rows[ 0 ][ Bars.TickerFieldName ]); }
		}

		/// <summary>
		/// Gets the dateTime of the first bar contained into the Bars object
		/// </summary>
		/// <returns></returns>
		public DateTime StartDateTime
		{
			get{ return ((DateTime)this.Rows[ 0 ][ Bars.DateTimeForOpen ]); }
		}
		/// <summary>
		/// Gets the dateTime of the last bar contained into the Bars object
		/// </summary>
		/// <returns></returns>
		public DateTime EndDateTime
		{
			get{ return ((DateTime)this.Rows[ this.Rows.Count - 1 ][ Bars.DateTimeForOpen ]); }
		}
		
		#region GetHashValue
		/*
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
			DataView quotes = new DataView( this );
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
			DataView quotes = new DataView( this );
			quotes.RowFilter = "( (quDate>=" + FilterBuilder.GetDateConstant( startDate ) +
				") and (quDate<=" + FilterBuilder.GetDateConstant( endDate ) + ") )";
			return HashProvider.GetHashValue( getHashValue_getQuoteString( quotes ) );
		}
		 */
		#endregion
		
		private void setHistory()
		{
			if ( this.history == null )
				// history has not been set, yet
			{
				this.history = new History();
				this.history.Import( this , Bars.DateTimeForOpen , Bars.Close );
			}
		}
		/// <summary>
		/// returns the DateTime for the bar that is precedingMinutes before
		/// dateTime
		/// </summary>
		/// <param name="dateTime"></param>
		/// <param name="precedingMinutes"></param>
		/// <returns></returns>
		public DateTime GetPrecedingDateTime( DateTime dateTime , int precedingMinutes )
		{
			setHistory();
			return (DateTime) history.GetKey( Math.Max( 0 ,
			                                           history.IndexOfKeyOrPrevious( dateTime ) -
			                                           precedingMinutes ) );
		}


		/// <summary>
		/// returns the DateTime for the bar that is followingMinutes after
		/// dateTime
		/// </summary>
		/// <param name="dateTime"></param>
		/// <param name="followingMinutes"></param>
		/// <returns></returns>
		public DateTime GetFollowingDateTime( DateTime dateTime , int followingMinutes )
		{
			setHistory();
			int indexOfKeyOrPrevious =
				history.IndexOfKeyOrPrevious( dateTime );
			DateTime followingDateTime;
			try
			{
				followingDateTime =	(DateTime) history.GetKey( Math.Max( 0 ,
				                                                        indexOfKeyOrPrevious + followingMinutes ) );
			}
			catch ( ArgumentOutOfRangeException exception )
			{
				string message = exception.Message;
				throw new Exception( "Quotes.GetFollowingDateTime() error: there is not " +
				                    "a dateTime for dateTime=" + dateTime.ToString() +
				                    " and followingMinutes=" + followingMinutes );
			}
			return followingDateTime;
		}
		
		/// <summary>
		/// Returns true if a bar is available at the given dateTime
		/// </summary>
		/// <param name="dateTime">dateTime</param>
		/// <returns></returns>
		public bool HasDateTime( DateTime dateTime )
		{
			/*alternative code, but primary keys need to be set first
      bool hasDate;
      hasDate = this.Rows.Contains(date.Date);
      return hasDate;*/
			setHistory();
			return this.history.ContainsKey(dateTime);
		}
		/// <summary>
		/// If the ticker has a bar at the given dateTime, then it returns the given dateTime,
		/// else it returns the immediate following dateTime at which a bar is available
		/// </summary>
		/// <param name="dateTime">dateTime</param>
		/// <returns></returns>
		public DateTime GetBarDateTimeOrFollowing( DateTime dateTime )
		{
			if( this.HasDateTime( dateTime ) )
			{
				return dateTime;
			}
			else
			{
				return GetBarDateTimeOrFollowing( dateTime.AddMinutes( 1 ) );
			}
		}
		/// <summary>
		/// If the ticker has a bar at the given dateTime, then it returns the given dateTime,
		/// else it returns the immediate preceding dateTime at which a bar is available
		/// </summary>
		/// <param name="dateTime">dateTime</param>
		/// <returns></returns>
		public DateTime GetBarDateTimeOrPreceding( DateTime dateTime )
		{
			if( this.HasDateTime( dateTime ) )
			{
				return dateTime;
			}
			else
			{
				return GetBarDateTimeOrPreceding( dateTime.AddMinutes( - 1 ) );
			}
		}

		/// <summary>
		/// If the ticker has a bar at the given dateTime, then it returns the given dateTime,
		/// else it returns the first valid following dateTime at which a bar is available
		/// (or the first valid preceding dateTime, in case dateTime is >= the dateTime of the last available bar)
		/// </summary>
		/// <param name="dateTime">dateTime</param>
		/// <returns></returns>
		public DateTime GetFirstValidBarDateTime(DateTime dateTime)
		{
			DateTime startDateTime =  this.StartDateTime;
			DateTime endDateTime = this.EndDateTime;
			if( dateTime < startDateTime || (dateTime>=startDateTime && dateTime<=endDateTime))
			{
				return this.GetBarDateTimeOrFollowing(dateTime);
			}
			else
			{
				return this.GetBarDateTimeOrPreceding(dateTime);
			}
		}
		/// <summary>
		/// returns the first bar dateTime for the ticker
		/// </summary>
		/// <param name="ticker"></param>
		/// <returns></returns>
		public static DateTime GetFirstBarDateTime( string ticker )
		{
			return QuantProject.DataAccess.Tables.Bars.GetFirstBarDateTime( ticker );
		}


		/// <summary>
		/// Gets the close at the given dateTime
		/// </summary>
		/// <returns></returns>
		public float GetClose(DateTime dateTime )
		{
			object[] keys = new object[1];
			keys[0] = dateTime.Date;
			DataRow foundRow = this.Rows.Find(keys);
			if(foundRow==null)
				throw new Exception("No bar for such a dateTime!");
			return (float)foundRow[ Bars.Close ];
		}

		/// <summary>
		/// Gets the first valid raw (not adjusted) close at the given date
		/// </summary>
		/// <returns></returns>
		public float GetFirstValidClose(DateTime dateTime )
		{
			object[] keys = new object[1];
			keys[0] = this.GetFirstValidBarDateTime(dateTime);
			DataRow foundRow = this.Rows.Find(keys);
			if(foundRow==null)
				throw new Exception("No bar for such a dateTime!");
			return (float)foundRow[Bars.Close];
		}
		
		/// <summary>
		/// Gets the first valid open at the given dateTime
		/// </summary>
		/// <returns></returns>
		public float GetFirstValidOpen(DateTime dateTime )
		{
			object[] keys = new object[1];
			keys[0] = this.GetFirstValidBarDateTime(dateTime);
			DataRow foundRow = this.Rows.Find(keys);
			if(foundRow==null)
				throw new Exception("No bar for such a dateTime!");
			return (float)foundRow[Bars.Open];
		}
	}
}
