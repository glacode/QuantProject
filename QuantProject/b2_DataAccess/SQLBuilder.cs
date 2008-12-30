/*
QuantProject - Quantitative Finance Library

SQLBuilder.cs
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

using QuantProject.ADT;
using QuantProject.ADT.Timing;

namespace QuantProject.DataAccess
{
	/// <summary>
	/// Summary description for SQLBuilder.
	/// </summary>
	internal class SQLBuilder
	{
		internal SQLBuilder()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region GetDateConstant
		private static string getDateConstantForAccess( DateTime dateTime )
		{
			string dateConstantForAccess = "#" + dateTime.Month + "/" + dateTime.Day + "/" +
				dateTime.Year + "#";
			return dateConstantForAccess;			
		}
		private static string getDateConstantForMySQL( DateTime dateTime )
		{
			string dateConstantForMySql =
				"'" +
				dateTime.Year + "-" +
				dateTime.Month + "-" +
				dateTime.Day +
//				" " +
//				dateTime.Hour + ":" +
//				dateTime.Minute + ":" +
//				dateTime.Second +
				"'";
			return dateConstantForMySql;			
		}
		internal static string GetDateConstant( DateTime dateTime )
		{
			string getDateConstant = null;
			switch ( ConnectionProvider.DbType )
			{
				case DbType.Access:
					getDateConstant =
						SQLBuilder.getDateConstantForAccess( dateTime );
					break;
				case DbType.MySql:
					getDateConstant =
						SQLBuilder.getDateConstantForMySQL( dateTime );
					break;
				default:
					throw new Exception(
						"Unknown database type. Complete the switch statement, please" );
			}
			return getDateConstant;
		}
		#endregion GetDateConstant

		#region GetDateTimeConstant
		private static string getDateTimeConstantForAccess( DateTime dateTime )
		{
			string dateTimeConstant =
				"#" +
				dateTime.Month + "/" +
				dateTime.Day + "/" +
				dateTime.Year + " " +
				dateTime.Hour + ":" +
				dateTime.Minute + ":" +
				dateTime.Second +
				"#";
			return dateTimeConstant;		
		}
		private static string getDateTimeConstantForMySQL( DateTime dateTime )
		{
			string dateTimeConstant =
				"'" +
				dateTime.Year + "-" +
				dateTime.Month + "-" +
				dateTime.Day + " " +
				dateTime.Hour + ":" +
				dateTime.Minute + ":" +
				dateTime.Second +
				"'";
			return dateTimeConstant;		
		}
		/// <summary>
		/// returns a string to be used as a DateTime constant for a query
		/// for an Access database
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		internal static string GetDateTimeConstant( DateTime dateTime )
		{
			string dateTimeConstant = null;
			switch ( ConnectionProvider.DbType )
			{
				case DbType.Access:
					dateTimeConstant =
						SQLBuilder.getDateTimeConstantForAccess( dateTime );
					break;
				case DbType.MySql:
					dateTimeConstant =
						SQLBuilder.getDateTimeConstantForMySQL( dateTime );
					break;
				default:
					throw new Exception(
						"Unknown database type. Complete the switch statement, please" );
			}
			return dateTimeConstant;
		}
		#endregion GetDateTimeConstant
		
		#region GetTimeConstant
		public static string GetTimeConstant( Time time )
		{
			string timeConstant =
				"'" +
				time.Hour.ToString( "00" ) + "." +
				time.Minute.ToString( "00" ) + "." +
				time.Second.ToString( "00" ) + "'";
			return timeConstant;
		}
		#endregion GetTimeConstant
		
		#region GetFilterForTime
//		private static void getFilterForTime_checkParameters( DateTime time )
//		{
		////			if ( ( comparisonOperator != "=" ) &&
		////			    ( comparisonOperator != "<" ) &&
		////			    ( comparisonOperator != "<=" ) &&
		////			    ( comparisonOperator != ">" ) &&
		////			    ( comparisonOperator != ">=" ) )
		////				throw new Exception(
		////					"comparisonOperator can either be '=' or '<' or '<=' or '>' or '>='" );
//			if ( !ExtendedDateTime.IsTime( time ) )
//				throw new Exception(
//					"time is actually not a time. Use the method " +
//					"QuantProject.ADT.ExtendedDateTime.GetTime() to build your time" );
//		}
		
		#region getFilterForTime_actually
		
		#region getFormatFunctionForTime
		private static string getFormatFunctionForTime( string dateTimeFieldName )
		{
			string formatFunctionForTime = null;
			switch ( ConnectionProvider.DbType )
			{
				case DbType.Access:
					formatFunctionForTime = "Format([" + dateTimeFieldName + "],'hh.mm.ss')";
					break;
				case DbType.MySql:
					formatFunctionForTime = "Date_Format(" + dateTimeFieldName + ",'%H.%i.%s')";
					break;
				default:
					throw new Exception(
						"Unknown database type. Complete the switch statement, please" );
			}
			return formatFunctionForTime;
		}
		#endregion getFormatFunctionForTime
		
		private static string getSqlStringForComparisonOperator(
			SqlComparisonOperator sqlComparisonOperator )
		{
			string sqlStringForComparisonOperator = "";
			switch( sqlComparisonOperator )
			{
				case SqlComparisonOperator.Equal:
					sqlStringForComparisonOperator = "=";
					break;
				case SqlComparisonOperator.LessThan:
					sqlStringForComparisonOperator = "<";
					break;
				case SqlComparisonOperator.LessThanOrEqual:
					sqlStringForComparisonOperator = "<=";
					break;
				case SqlComparisonOperator.GreaterThan:
					sqlStringForComparisonOperator = ">";
					break;
				case SqlComparisonOperator.GreaterThanOrEqual:
					sqlStringForComparisonOperator = ">=";
					break;
			}
			return sqlStringForComparisonOperator;
		}
		private static string getFilterForTime_actually(
			string fieldName , SqlComparisonOperator sqlComparisonOperator , Time time )
		{
			string filterForDailyTime =
//				"(Format([baDateTimeForOpen],'hh.mm.ss')" +
				"(" +
				SQLBuilder.getFormatFunctionForTime( "baDateTimeForOpen" ) +
				SQLBuilder.getSqlStringForComparisonOperator( sqlComparisonOperator ) +
				SQLBuilder.GetTimeConstant( time ) + ")";
			return filterForDailyTime;
		}
		#endregion getFilterForTime_actually
		
		/// <summary>
		/// returns a sql where expression, to compare a field value with
		/// the given time
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="comparisonOperator">it can either be "=" or "<" or
		/// "<=" or ">" or ">="</param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		internal static string GetFilterForTime(
			string fieldName , SqlComparisonOperator sqlComparisonOperator , Time time )
		{
//			SQLBuilder.getFilterForTime_checkParameters( time );
			string filterForTime =
				SQLBuilder.getFilterForTime_actually(
					fieldName , sqlComparisonOperator , time );
			return filterForTime;
		}
		#endregion GetFilterForTime
	
		/// <summary>
		/// returns the name of the function used by the current database type,
		/// to compute the standard deviation
		/// </summary>
		/// <returns></returns>
		public static string GetStandardDeviationFunctionName()
		{
			string setStandardDeviationFunctionName = null;
			switch ( ConnectionProvider.DbType )
			{
				case DbType.Access:
					setStandardDeviationFunctionName = "StDev";
					break;
				case DbType.MySql:
					setStandardDeviationFunctionName = "STDDEV_POP";
					break;
				default:
					throw new Exception(
						"Unknown database type. Complete the switch statement, please" );
			}
			return setStandardDeviationFunctionName;
		}
	}
}
