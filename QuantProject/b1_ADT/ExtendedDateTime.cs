/*
QuantProject - Quantitative Finance Library

ExtendedDateTime.cs
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

namespace QuantProject.ADT
{
	/// <summary>
	/// Summary description for DateTime.
	/// </summary>
  public static class ExtendedDateTime
  {
		/// <summary>
		/// Returns a DateTime short description suitable for file names (no slashes)
		/// hours, minutes and seconds are NOT displayed
		/// </summary>
		/// <returns></returns>
		public static string GetShortDescriptionForFileName(
			DateTime dateTime )
		{
			string stringForFileName =
				dateTime.Year.ToString() + "_" +
				dateTime.Month.ToString().PadLeft( 2 , '0' ) + "_" +
				dateTime.Day.ToString().PadLeft( 2 , '0' );
			return stringForFileName;
		}
		/// <summary>
		/// Returns a DateTime short description suitable for file names (no slashes)
		/// hours, minutes and seconds are displayed also
		/// </summary>
		/// <returns></returns>
		public static string GetCompleteShortDescriptionForFileName(
			DateTime dateTime )
		{
			string stringForFileName =
				GetShortDescriptionForFileName( dateTime ) + "_" +
				dateTime.Hour.ToString() + "_" +
				dateTime.Minute.ToString().PadLeft( 2 , '0' ) + "_" +
				dateTime.Second.ToString().PadLeft( 2 , '0' );
			return stringForFileName;
		}
		/// <summary>
		/// Returns the earliest date
		/// </summary>
		/// <param name="dateTime1"></param>
		/// <param name="dateTime2"></param>
		/// <returns></returns>
		public static DateTime Min( DateTime dateTime1 , DateTime dateTime2 )
		{
			DateTime returnValue = dateTime1;
			if ( dateTime2 < dateTime1 )
				returnValue = dateTime2;
			return returnValue;
		}

		/// <summary>
		/// Returns the date, for the given date time, i.e. hours, minutes
		/// and seconds are set to zero
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static DateTime GetDate( DateTime dateTime )
		{
			DateTime date =
				new DateTime(
					dateTime.Year , dateTime.Month , dateTime.Day ,
					0 , 0 , 0 );
			return date;
		}
		
		/// <summary>
		/// returns a copy of the given DateTime
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static DateTime Copy( DateTime dateTime )
		{
			DateTime newDateTime =
				new DateTime(
					dateTime.Year , dateTime.Month , dateTime.Day ,
					dateTime.Hour , dateTime.Minute , dateTime.Second );
			return newDateTime;
		}		
		
		public static bool IsDate( DateTime dateTime )
		{
			bool isDate =
				( ( dateTime.Hour == 0 ) &&
				 ( dateTime.Minute == 0 ) &&
				 ( dateTime.Second == 0 ) );
			return isDate;
		}
		
		/// <summary>
		/// True iff the time for the first argument is equal
		/// to the time for the second argument
		/// </summary>
		/// <param name="dateTime1"></param>
		/// <param name="dateTime2"></param>
		/// <returns></returns>
		public static bool HaveTheSameTime(
			DateTime dateTime1 , DateTime dateTime2 )
		{
			bool returnValue =
				( dateTime1.Hour == dateTime2.Hour ) &&
				( dateTime1.Minute == dateTime2.Minute ) &&
				( dateTime1.Second == dateTime2.Second );
			return returnValue;
		}
		
		#region IsFirstTimeLessThenSecondTime
		private static DateTime getTime( DateTime dateTime )
		{
			DateTime time = new DateTime(
				1900 , 1 , 1 ,
				dateTime.Hour , dateTime.Minute , dateTime.Second );
			return time;
		}
		/// <summary>
		/// true iif the time for dateTime1 is less than the time
		/// for dateTime2
		/// </summary>
		/// <param name="dateTime1"></param>
		/// <param name="dateTime2"></param>
		public static bool IsFirstTimeLessThenSecondTime(
			DateTime dateTime1 , DateTime dateTime2 )
		{
			DateTime time1 = ExtendedDateTime.getTime( dateTime1 );
			DateTime time2 = ExtendedDateTime.getTime( dateTime2 );
			bool isFirstTimeLessThenSecondTime =
				( time1 < time2 );
			return isFirstTimeLessThenSecondTime;
		}
		#endregion IsFirstTimeLessThenSecondTime
	}
}
