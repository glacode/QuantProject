/*
QuantProject - Quantitative Finance Library

TimeZoneManager.cs
Copyright (C) 2008
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

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	/// <summary>
	/// Handles DateTime conversion from EST/EDT to UTC
	/// </summary>
	public class TimeZoneManager
	{
		
		static Hashtable dSTPeriods;
		
		public TimeZoneManager()
		{
		}
		
		#region IsDaylightSavingTime
		
		#region getDstPeriod
		
		#region initialize_dstPeriods_ifTheCase
		
		#region initialize_dstPeriods
		
		#region initialize_dstPeriods_add
		private static void initialize_dstPeriods_add(
			int year , int monthForBegin , int dayForBegin ,
			int monthForEnd , int dayForEnd )
		{
			DateTime dateTimeForBegin =
				new DateTime( year , monthForBegin , dayForBegin ,
				             1 , 59 , 59 );
			DateTime dateTimeForEnd =
				new DateTime( year , monthForEnd , dayForEnd ,
				             1 , 59 , 59 );
			DSTPeriod dSTPeriod =
				new DSTPeriod( dateTimeForBegin , dateTimeForEnd );
			dSTPeriods.Add( year , dSTPeriod );
		}
		#endregion initialize_dstPeriods_add
		
		private static void initialize_dstPeriods()
		{
			// dates have been taken by this page http://www.timeanddate.com/worldclock/timezone.html?n=179
			dSTPeriods = new Hashtable();
			initialize_dstPeriods_add( 2000 , 4 , 2 , 10 , 29 );
			initialize_dstPeriods_add( 2001 , 4 , 1 , 10 , 28 );
			initialize_dstPeriods_add( 2002 , 4 , 7 , 10 , 27 );
			initialize_dstPeriods_add( 2003 , 4 , 6 , 10 , 26 );
			initialize_dstPeriods_add( 2004 , 4 , 4 , 10 , 31 );
			initialize_dstPeriods_add( 2005 , 4 , 3 , 10 , 30 );
			initialize_dstPeriods_add( 2006 , 4 , 2 , 10 , 29 );
			initialize_dstPeriods_add( 2007 , 3 , 11 , 11 , 4 );
			initialize_dstPeriods_add( 2008 , 3 , 9 , 11 , 2 );
			initialize_dstPeriods_add( 2009 , 3 , 8 , 11 , 1 );
		}
		#endregion initialize_dstPeriods
		
		private static void initialize_dstPeriods_ifTheCase()
		{
			if ( dSTPeriods == null )
				initialize_dstPeriods();
		}
		#endregion initialize_dstPeriods_ifTheCase
		
		#region getDstPeriod_withInitialized_dstPeriods
		private static void getDstPeriod_withInitialized_dstPeriods_checkParameters(
			DateTime dateTimeInNewYorkTimeZone )
		{
			if ( !TimeZoneManager.dSTPeriods.ContainsKey( dateTimeInNewYorkTimeZone.Year ) )
				throw new Exception(
					"We don't have DaylightSavingTime information for the year " +
					dateTimeInNewYorkTimeZone.Year + ". Please check the method " +
					"initialize_dstPeriods() and possibly complete it with more years" );
		}
		private static DSTPeriod getDstPeriod_withInitialized_dstPeriods(
			DateTime dateTimeInNewYorkTimeZone )
		{
			TimeZoneManager.getDstPeriod_withInitialized_dstPeriods_checkParameters(
				dateTimeInNewYorkTimeZone );
			DSTPeriod dSTPeriod =
				(DSTPeriod)dSTPeriods[ dateTimeInNewYorkTimeZone.Year ];
			return dSTPeriod;
		}
		#endregion getDstPeriod_withInitialized_dstPeriods
		
		private static DSTPeriod getDstPeriod( DateTime dateTimeInNewYorkTimeZone )
		{
			TimeZoneManager.initialize_dstPeriods_ifTheCase();
			DSTPeriod dSTPeriod =
				TimeZoneManager.getDstPeriod_withInitialized_dstPeriods(
					dateTimeInNewYorkTimeZone );
			return dSTPeriod;
		}
		#endregion getDstPeriod
		
		/// <summary>
		/// True iif the given DateTime is a daylightSavingTime
		/// </summary>
		/// <param name="dateTimeInNewYorkTimeZone"></param>
		/// <returns></returns>
		public static bool IsDaylightSavingTime(
			DateTime dateTimeInNewYorkTimeZone )
		{
			DSTPeriod dSTPeriod =
				TimeZoneManager.getDstPeriod( dateTimeInNewYorkTimeZone );
			bool isDaylightSavingTime =
				( dSTPeriod.Begin.CompareTo( dateTimeInNewYorkTimeZone ) < 0 ) &&
				( dSTPeriod.End.CompareTo( dateTimeInNewYorkTimeZone ) > 0 );
			return isDaylightSavingTime;
		}
		#endregion IsDaylightSavingTime
		
		/// <summary>
		/// Converts a DateTime in the New York time zone
		/// (that can either be EST or EDT) to UTC
		/// </summary>
		/// <param name="dateTimeInNewYorkTimeZone"></param>
		/// <returns></returns>
		public static DateTime ConvertToUTC(
			DateTime dateTimeInNewYorkTimeZone )
		{
			DateTime dateTimeInUTC;
			if ( TimeZoneManager.IsDaylightSavingTime(
				dateTimeInNewYorkTimeZone ) )
				dateTimeInUTC = dateTimeInNewYorkTimeZone.AddHours( 4 );
			else
				dateTimeInUTC = dateTimeInNewYorkTimeZone.AddHours( 5 );
			return dateTimeInUTC;
		}
		
		public static DateTime ConvertToEST(
			DateTime dateTimeInUTC )
		{
			DateTime dateTimeInEST = dateTimeInUTC.AddHours( -5 );
			if ( TimeZoneManager.IsDaylightSavingTime( dateTimeInEST ) )
				dateTimeInEST = dateTimeInUTC.AddHours( -4 );
			return dateTimeInEST;
		}
	}
}
