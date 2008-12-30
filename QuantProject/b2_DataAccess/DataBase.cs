/*
QuantProject - Quantitative Finance Library

DataBase.cs
Copyright (C) 2003
Glauco Siliprandi

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
using System.Data.Common;
using System.Data.OleDb;
using System.Windows.Forms;

using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.ADT.Timing;

namespace QuantProject.DataAccess
{
	/// <summary>
	/// Summary description for DataBase.
	/// </summary>
	/// 
	public class DataBase
	{
		private static DbConnection dbConnection = ConnectionProvider.DbConnection;

		public DataBase()
		{
			//
			
			//
		}

		/// <summary>
		/// Returns the field name corresponding to the quote field
		/// </summary>
		/// <param name="quoteField">Discriminates among Open, High, Low and Closure</param>
		/// <returns>Field name corresponding to the quote field</returns>
		private static string getFieldName( QuoteField quoteField )
		{
			string fieldName = "";
			switch ( quoteField )
			{
				case QuoteField.Open:
					fieldName = "quOpen";
					break;
				case QuoteField.High:
					fieldName = "quHigh";
					break;
				case QuoteField.Low:
					fieldName = "quLow";
					break;
				case QuoteField.Close:
					fieldName = "quClose";
					break;
				case QuoteField.AdjustedClose:
					fieldName = "quAdjustedClose";
					break;
				case QuoteField.AdjustedCloseToCloseRatio:
					fieldName = "quAdjustedCloseToCloseRatio";
					break;
				case QuoteField.Volume:
					fieldName = "quVolume";
					break;
				default:
					break;
			}
			return fieldName;
		}

		#region "GetHistory"
		private static History getHistory_try( string instrumentKey , QuoteField quoteField ,
		                                      DateTime firstDate , DateTime lastDate )
		{
			History history = new History();
			string commandString =
				"select * from quotes where (quTicker='" + instrumentKey + "') " +
				"and (quDate>=" + SQLBuilder.GetDateConstant( firstDate ) + ") " +
				"and (quDate<=" + SQLBuilder.GetDateConstant( lastDate ) + ")";
//			OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter( commandString , dbConnection );
//			DataTable dataTable = new DataTable();
			DataTable dataTable = SqlExecutor.GetDataTable( commandString );
//			oleDbDataAdapter.Fill( dataTable );
			history.Import( dataTable , "quDate" , getFieldName( quoteField ) );
			return history;
		}
		private static History getHistory_common( string instrumentKey , QuoteField quoteField ,
		                                         DateTime firstDate , DateTime lastDate )
		{
			History history;
			try
			{
				history = getHistory_try( instrumentKey , quoteField , firstDate , lastDate );
			}
			catch (Exception ex)
			{
				MessageBox.Show( ex.ToString() );
				history = null;
			}
			return history;
		}
		/// <summary>
		/// Returns the full history for the instrument and the specified quote field
		/// </summary>
		/// <param name="instrumentKey">Identifier (ticker) for the instrument whose story
		/// has to be returned</param>
		/// <param name="quoteField">Discriminates among Open, High, Low and Closure</param>
		/// <returns>The history for the given instrument and quote field</returns>
		public static History GetHistory( string instrumentKey , QuoteField quoteField )
		{
			return getHistory_common( instrumentKey , quoteField , DateTime.MinValue , DateTime.MaxValue );
		}

		/// <summary>
		/// Returns the history for the instrument and the specified quote field
		/// </summary>
		/// <param name="instrumentKey">Identifier (ticker) for the instrument whose story
		/// has to be returned</param>
		/// <param name="quoteField">Discriminates among Open, High, Low and Closure</param>
		/// <param name="firstDate">First date for quotes to be fetched</param>
		/// <param name="lastDate">Last date for quotes to be fetched</param>
		/// <returns>The history for the given instrument and quote field</returns>
		public static History GetHistory( string instrumentKey , QuoteField quoteField ,
		                                 DateTime firstDate , DateTime lastDate )
		{
			return getHistory_common( instrumentKey , quoteField , firstDate , lastDate );
		}
		#endregion
		public static double GetQuote( string ticker ,
		                              QuoteField quoteField , DateTime dateTime )
		{
			double quote = Double.MinValue;
			string sqlQuery =
				"select " + getFieldName( quoteField ) + " " +
				"from quotes where (quTicker='" + ticker + "') " +
				"and (quDate=" + SQLBuilder.GetDateConstant( dateTime ) + ")";
			DataTable quotes = new DataTable();
			try
			{
				quotes = SqlExecutor.GetDataTable( sqlQuery );
			}
			catch (Exception ex)
			{
				MessageBox.Show( ex.ToString() );
			}
			if ( quotes.Rows.Count == 0 )
				throw new MissingQuoteException( ticker , dateTime );
			else
				quote = (double)( quotes.Rows[ 0 ][ 0 ] );
			return quote;
		}
		public static bool WasExchanged( string ticker ,
		                                DateTime dateTime )
		{
			string sqlQuery =
				"select * " +
				"from quotes where (quTicker='" + ticker + "') " +
				"and (quDate=" +
				SQLBuilder.GetDateConstant( dateTime ) + ")";
			DataTable quotes = new DataTable();
			try
			{
				quotes = SqlExecutor.GetDataTable( sqlQuery );
			}
			catch (Exception ex)
			{
				MessageBox.Show( ex.ToString() );
			}
			return ( quotes.Rows.Count > 0 );
		}
		
		#region getHistory
		private static History getHistory(
			DataTable barDataTable , string barFieldName )
		{
			History history = new History();
			history.Import(
				barDataTable , BarFieldNames.DateTimeForOpen , barFieldName );
			return history;
		}
		#endregion getHistory
		
		#region GetBarOpenHistory
		
		#region getBarDataTable
		
		#region getSqlForBarDataTable
		
		#region getFilterForDailyTimes
		
//		#region getFilterForDailyTime
//		private getFilterForDailyTime( DateTime dateTime )
//		{
//			string filterForDailyTime =
//				"(Format([baDateTimeForOpen],'hh:mm:ss')>='" +
//				this.getSqlTimeConstantForFirstDailyBar() + "')";
//		}
//		#endregion getFilterForDailyTime
		
		private static string getFilterForDailyTimes( List< Time > dailyTimes )
		{
			string filterForDailyTimes = "";
			foreach( Time time in dailyTimes )
				filterForDailyTimes =
					filterForDailyTimes +
					SQLBuilder.GetFilterForTime(
						"baDateTimeForOpen" , SqlComparisonOperator.Equal , time ) +
					" or ";
			filterForDailyTimes = filterForDailyTimes.Substring(
				0 , filterForDailyTimes.Length - " or ".Length );
			return filterForDailyTimes;
		}
		#endregion getFilterForDailyTimes
		
		private static string getSqlForBarDataTable(
			string ticker ,
			int barInterval ,
			string barFieldName ,
			DateTime firstDateTime ,
			DateTime lastDateTime ,
			List< Time > dailyTimes )
		{
			string sql =
				"select " + BarFieldNames.DateTimeForOpen + "," + BarFieldNames.Open +
				" from bars " +
				"where (baTicker='" + ticker + "') and " +
				"(baInterval=" + barInterval + ") and" +
				"(baDateTimeForOpen>=" +
				SQLBuilder.GetDateTimeConstant( firstDateTime ) + ") and" +
				"(baDateTimeForOpen<=" +
				SQLBuilder.GetDateTimeConstant( lastDateTime )
				+ ") and (" +
				DataBase.getFilterForDailyTimes( dailyTimes ) +
				")";
//			"(Format([baDateTimeForOpen],'hh:mm:ss')>='" +
//				DataBase.getSqlTimeConstantForFirstDailyBar() + "') and" +
//				"(Format([baDateTimeForOpen],'hh:mm:ss')<='" +
//				DataBase.getSqlTimeConstantForLastDailyBar() + "');";
			return sql;
		}
		#endregion getSqlForBarDataTable
		
		private static DataTable getBarDataTable(
			string ticker ,
			int barInterval ,
			string barFieldName ,
			DateTime firstDateTime ,
			DateTime lastDateTime ,
			List< Time > dailyTimes )
		{
			string sql = DataBase.getSqlForBarDataTable(
				ticker ,
				barInterval ,
				barFieldName ,
				firstDateTime ,
				lastDateTime ,
				dailyTimes );
			DataTable barDataTable = SqlExecutor.GetDataTable( sql );
			return barDataTable;
		}
		#endregion getBarDataTable
		
		/// <summary>
		/// returns the market value for the given ticker, for all days in the given
		/// interval, at the open of all the bars that begin at dailyTimes
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="barInterval"></param>
		/// <param name="firstDateTime"></param>
		/// <param name="lastDateTime"></param>
		/// <param name="dailyTimes"></param>
		/// <returns></returns>
		public static History GetBarOpenHistory(
			string ticker ,
			int barInterval ,
			DateTime firstDateTime ,
			DateTime lastDateTime ,
			List< Time > dailyTimes )
		{
			DataTable barDataTable = DataBase.getBarDataTable(
				ticker ,
				barInterval ,
				BarFieldNames.Open ,
				firstDateTime ,
				lastDateTime ,
				dailyTimes );
			History barOpenHistory = DataBase.getHistory(
				barDataTable , BarFieldNames.Open );
			return barOpenHistory;
		}
		#endregion GetBarOpenHistory
	}
}
