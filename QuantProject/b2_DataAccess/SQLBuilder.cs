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

		internal static string GetDateConstant( DateTime dateTime )
		{
			string getDateConstant;
			getDateConstant = "#" + dateTime.Month + "/" + dateTime.Day + "/" +
				dateTime.Year + "#";
			return getDateConstant;
		}

		/// <summary>
		/// returns a string to be used as a DateTime constant for a query
		/// for an Access database
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		internal static string GetDateTimeConstant( DateTime dateTime )
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
				"(Format([baDateTimeForOpen],'hh.mm.ss')" +
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
	}
}
