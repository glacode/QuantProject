/*
QuantProject - Quantitative Finance Library

HistoricalEndOfDayTimer.cs
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

using QuantProject.ADT;

namespace QuantProject.Business.Timing
{
	/// <summary>
	/// IDataStreamer implementation using historical data
	/// </summary>
	[Serializable]
	public class HistoricalEndOfDayTimer : Timer
	{
		protected Hashtable tickers;

		protected DateTime startDateTime;
//		private EndOfDayDateTime endDateTime;
		
		private static DateTime timeForMarketOpen =
			new DateTime( 1900 , 1 , 1 , 9 , 30 , 00 );
		private static DateTime timeForMarketClose =
			new DateTime( 1900 , 1 , 1 , 16 , 00 , 00 );
		
		/// <summary>
		/// time when the market opens
		/// </summary>
		public static DateTime TimeForMarketOpen {
			get { return HistoricalEndOfDayTimer.timeForMarketOpen; }
		}
		
		/// <summary>
		/// time when the market closes
		/// </summary>
		public static DateTime TimeForMarketClose {
			get { return HistoricalEndOfDayTimer.timeForMarketClose; }
		}

		/// <summary>
		/// time for one hour after market closes
		/// </summary>
		public static DateTime TimeForOneHourAfterMarketClose {
			get { return HistoricalEndOfDayTimer.TimeForMarketClose.AddHours( 1 ); }
		}

		public DateTime StartDateTime
		{
			get	{	return this.startDateTime;	}
			set	{	this.startDateTime = value;	}
		}

//		public EndOfDayDateTime EndDateTime
//		{
//			get	{	return this.endDateTime;	}
//			set	{	this.endDateTime = value;	}
//		}

		public HistoricalEndOfDayTimer( DateTime startDateTime )
		{
			this.startDateTime = startDateTime;
//			this.endDateTime = EndDateTime;
			this.tickers = new Hashtable();
			
//			HistoricalEndOfDayTimer.timeForMarketOpen =
//				new DateTime( 1900 , 1 , 1 , 9 , 30 , 00 );
//			HistoricalEndOfDayTimer.timeForMarketClose =
//				new DateTime( 1900 , 1 , 1 , 16 , 00 , 00 );
		}
		
		protected override void initializeTimer()
		{
			this.currentDateTime =
				ExtendedDateTime.Copy( this.startDateTime );
		}
		
		#region moveNext
		protected bool isBetweenMarketCloseAndOneHourAfterMarketClose()
		{
			bool isBetween =
				( ( this.currentDateTime >=
				   HistoricalEndOfDayTimer.GetMarketClose( this.currentDateTime ) )
				 &&
				 ( this.currentDateTime <
				  HistoricalEndOfDayTimer.GetOneHourAfterMarketClose(
				  	this.currentDateTime ) ) );
			return isBetween;
		}
		protected override void moveNext()
		{
			if ( this.isBetweenMarketCloseAndOneHourAfterMarketClose() )
				this.currentDateTime =
					HistoricalEndOfDayTimer.GetOneHourAfterMarketClose(
						this.currentDateTime );
			else
				// current time is either before market close or >= one hour
				// after market close
				this.currentDateTime =
					HistoricalEndOfDayTimer.GetNextMarketStatusSwitch(
						this.currentDateTime );
		}
		#endregion moveNext
		
		protected override bool isDone()
		{
			return false;
		}

		/// <summary>
		/// true iif the argument is at market open
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static bool IsMarketOpen( DateTime dateTime )
		{
			bool returnValue =
				ExtendedDateTime.HaveTheSameTime(
					dateTime , HistoricalEndOfDayTimer.TimeForMarketOpen );
			return returnValue;
		}
		
		/// <summary>
		/// true iif the argument is at market close
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static bool IsMarketClose( DateTime dateTime )
		{
			bool returnValue =
				ExtendedDateTime.HaveTheSameTime(
					dateTime , HistoricalEndOfDayTimer.TimeForMarketClose );
			return returnValue;
		}

		/// <summary>
		/// true iif the argument is one our after market close
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static bool IsOneHourAfterMarketClose( DateTime dateTime )
		{
			bool returnValue =
				ExtendedDateTime.HaveTheSameTime(
					dateTime ,
					HistoricalEndOfDayTimer.TimeForOneHourAfterMarketClose );
			return returnValue;
		}
		
		/// <summary>
		/// true iif the argument is an end of day relevant time
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static bool IsEndOfDayDateTime( DateTime dateTime )
		{
			bool isEndOfDayDateTime =
				( HistoricalEndOfDayTimer.IsMarketOpen( dateTime ) ||
				 HistoricalEndOfDayTimer.IsMarketClose( dateTime ) ||
				 HistoricalEndOfDayTimer.IsOneHourAfterMarketClose( dateTime ) );
			return isEndOfDayDateTime;
		}
		
		/// <summary>
		/// true iif the argument is either at market close or at market open
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static bool IsMarketStatusSwitch( DateTime dateTime )
		{
			bool isEndOfDayDateTime =
				( HistoricalEndOfDayTimer.IsMarketOpen( dateTime ) ||
				 HistoricalEndOfDayTimer.IsMarketClose( dateTime ) );
			return isEndOfDayDateTime;
		}
		
		#region GetNextMarketStatusSwitch
//		private void getNextMarketStatusSwitch_checkParameters(
//			DateTime dateTime)
//		{
//			if ( !HistoricalEndOfDayTimer.IsMarketOpen( dateTime ) &&
//			    !HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
//				throw new Exception( "dateTime must be a
//		}
		/// <summary>
		/// Returns either the next market close or the next market open,
		/// whichever is the nearest (all days are considered as market
		/// days, week-ends included)
		/// We have a market status switch when the market opens and when
		/// the market closes
		/// </summary>
		/// <returns></returns>
		public static DateTime GetNextMarketStatusSwitch(
			DateTime dateTime)
		{
//			this.getNextMarketStatusSwitch_checkParameters(
//				dateTime );
			DateTime nextMarketStatusSwitch;
			if ( ExtendedDateTime.IsFirstTimeLessThenSecondTime(
				dateTime , HistoricalEndOfDayTimer.TimeForMarketOpen ) )
				// dateTime's time is before market open
				nextMarketStatusSwitch = new DateTime(
					dateTime.Year , dateTime.Month , dateTime.Day ,
					HistoricalEndOfDayTimer.TimeForMarketOpen.Hour ,
					HistoricalEndOfDayTimer.TimeForMarketOpen.Minute ,
					HistoricalEndOfDayTimer.TimeForMarketOpen.Second );
			else
			{
				// dateTime's time is equal or after the market open
				if ( ExtendedDateTime.IsFirstTimeLessThenSecondTime(
					dateTime , HistoricalEndOfDayTimer.TimeForMarketClose ) )
					// dateTime's time is equal or after the market open
					// AND dateTime's time is before market close
					nextMarketStatusSwitch = new DateTime(
						dateTime.Year , dateTime.Month , dateTime.Day ,
						HistoricalEndOfDayTimer.TimeForMarketClose.Hour ,
						HistoricalEndOfDayTimer.TimeForMarketClose.Minute ,
						HistoricalEndOfDayTimer.TimeForMarketClose.Second );
				else
					// dateTime's time is equal or after the market close
				{
					DateTime nextDay =
						dateTime.AddDays( 1 );
					nextMarketStatusSwitch = new DateTime(
						nextDay.Year , nextDay.Month , nextDay.Day ,
						HistoricalEndOfDayTimer.TimeForMarketOpen.Hour ,
						HistoricalEndOfDayTimer.TimeForMarketOpen.Minute ,
						HistoricalEndOfDayTimer.TimeForMarketOpen.Second );
				}
			}
			
//
//				    if ( this.EndOfDaySpecificTime < EndOfDaySpecificTime.MarketOpen )
//				    	nextMarketStatusSwitch = new EndOfDayDateTime(
//				    		this.DateTime , EndOfDaySpecificTime.MarketOpen );
//				    else
//				    {
//				    	// this.EndOfDaySpecificTime >= EndOfDaySpecificTime.MarketOpen
//				    	if ( this.EndOfDaySpecificTime < EndOfDaySpecificTime.MarketClose )
//				    		// ( this.EndOfDaySpecificTime >= EndOfDaySpecificTime.MarketOpen )
//				    		// AND ( this.EndOfDaySpecificTime < EndOfDaySpecificTime.MarketClose )
//				    		nextMarketStatusSwitch = new EndOfDayDateTime(
//				    			this.DateTime , EndOfDaySpecificTime.MarketClose );
//				    	else
//				    		// ( this.EndOfDaySpecificTime >= EndOfDaySpecificTime.MarketClose )
//				    		nextMarketStatusSwitch = new EndOfDayDateTime(
//				    			this.DateTime.AddDays( 1 ) , EndOfDaySpecificTime.MarketOpen );
//				    }
			return nextMarketStatusSwitch;
		}
		#endregion GetNextMarketStatusSwitch
		
		/// <summary>
		/// Returns the market open time, for the date of the given dateTime
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static DateTime GetMarketOpen( DateTime dateTime )
		{
			DateTime date = ExtendedDateTime.GetDate( dateTime );
			DateTime marketOpen =
				HistoricalEndOfDayTimer.GetNextMarketStatusSwitch( date );
			return marketOpen;
		}
		
		/// <summary>
		/// Returns the market close time, for the date of the given dateTime
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static DateTime GetMarketClose( DateTime dateTime )
		{
			DateTime marketOpen =
				HistoricalEndOfDayTimer.GetMarketOpen( dateTime );
			DateTime marketClose =
				HistoricalEndOfDayTimer.GetNextMarketStatusSwitch( marketOpen );
			return marketClose;
		}

		
		/// <summary>
		/// Returns the one hour after market close time, for the
		/// date of the given dateTime
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static DateTime GetOneHourAfterMarketClose( DateTime dateTime )
		{
			DateTime marketClose =
				HistoricalEndOfDayTimer.GetMarketClose( dateTime );
			DateTime oneHourAfterMarketClose = marketClose.AddHours( 1 );
			return oneHourAfterMarketClose;
		}

		/// <summary>
		/// Returns returns five minutes before market close time, for the
		/// date of the given dateTime
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static DateTime GetFiveMinutesBeforeMarketClose( DateTime dateTime )
		{
			DateTime marketClose =
				HistoricalEndOfDayTimer.GetMarketClose( dateTime );
			DateTime fiveMinutesBeforeMarketClose = marketClose.AddMinutes( -5 );
			return fiveMinutesBeforeMarketClose;
		}
	}
}
