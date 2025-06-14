/*
QuantProject - Quantitative Finance Library

IndexBasedHistoricalTimer.cs
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
using System.Collections.Generic;

using QuantProject.ADT.Histories;
using QuantProject.ADT.Timing;
using QuantProject.Data.DataTables;

namespace QuantProject.Business.Timing
{
	/// <summary>
	/// Throws out DateTime(s) at given daily times, but only if the given
	/// index is quoted at those DateTime(s)
	/// </summary>
	[Serializable]
	public class IndexBasedHistoricalTimer : Timer
	{
		private string indexTicker;
		private DateTime firstDateTime;
		private DateTime lastDateTime;
		private List< Time > dailyTimes;
		private int intervalFrameInSeconds;
		
		private List< DateTime > dateTimesToBeThrown;
		private int currentDateTimeIndex;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="indexTicker"></param>
		/// <param name="firstDateTime"></param>
		/// <param name="lastDateTime"></param>
		/// <param name="dailyTimes">daily times: these times must be in strict ascending
		/// order and they must be intraday (i.e. smaller than one hour after
		/// market close)</param>
		/// <param name="intervalFrameInSeconds"></param>
		public IndexBasedHistoricalTimer(
			string indexTicker ,
			DateTime firstDateTime ,
			DateTime lastDateTime ,
			List< Time > dailyTimes,
			int intervalFrameInSeconds)
		{
			this.checkParameters( dailyTimes );
			this.indexTicker = indexTicker;
			this.firstDateTime = firstDateTime;
			this.lastDateTime = lastDateTime;
			this.dailyTimes = dailyTimes;
			this.intervalFrameInSeconds = intervalFrameInSeconds;
		}
		
		private void checkParameters( List< Time > dailyTimes )
		{
			Time.CheckStrictlyAscending( dailyTimes );
		}
		
		#region initializeTimer
		
		#region initialize_dateTimesToBeThrown
		
		#region addDailyTimesAndIfTheCaseAddOneHourAfterMarketCloseDateTime
		
//		#region addDailyTimeAndIfTheCaseAddOneHourAfterMarketCloseDateTime
//		
//		#region addNextDateTimeAndIfTheCaseAddOneHourAfterMarketCloseDateTime
//
//		#region addOneHourAfterMarketCloseDateTimeIfTheCase
//		
////		#region isOneHourAfterMarktetCloseToBeInserted
////		private bool isOneHourAfterMarktetCloseToBeInserted_withOneAdded(
////			DateTime nextDateTimeToBeAdded )
////		{
////			Date dateForLastDateTimeAdded =
////				new Date( this.dateTimesToBeThrown[ this.dateTimesToBeThrown.Count - 1 ] );
////			Date dateForNextDateTimeToBeAdded =
////				new Date( nextDateTimeToBeAdded );
////			bool isToBeInserted = ( dateForNextDateTimeToBeAdded > dateForLastDateTimeAdded );
////			return isToBeInserted;
////		}
////		private bool isOneHourAfterMarktetCloseToBeInserted( DateTime nextDateTimeToBeAdded )
////		{
////			bool isToBeInserted = false;
////			if ( this.dateTimesToBeThrown.Count > 0 )
////				// at least one DateTime has already been added
////				isToBeInserted = isOneHourAfterMarktetCloseToBeInserted_withOneAdded(
////					nextDateTimeToBeAdded );
////			return isToBeInserted;
////		}
////		#endregion isOneHourAfterMarktetCloseToBeInserted
//		
//		#region addOneHourAfterMarketCloseDateTime
//		private DateTime getOneHourAfterMarketCloseForLastDateTimeAdded()
//		{
//			DateTime dateTimeForLastDateTimeAdded =
//				this.dateTimesToBeThrown[ this.dateTimesToBeThrown.Count - 1 ];
//			DateTime oneHourAfterMarketCloseForLastDateTimeAdded =
//				HistoricalEndOfDayTimer.GetOneHourAfterMarketClose(
//					dateTimeForLastDateTimeAdded );
//			return oneHourAfterMarketCloseForLastDateTimeAdded;
//		}
//		private void addOneHourAfterMarketCloseDateTime( DateTime nextDateTimeToBeAdded )
//		{
//			DateTime oneHourAfterMarketCloseForLastDateTimeAdded =
//				this.getOneHourAfterMarketCloseForLastDateTimeAdded();
//			this.dateTimesToBeThrown.Add(
//				oneHourAfterMarketCloseForLastDateTimeAdded );
//		}
//		#endregion addOneHourAfterMarketCloseDateTime
//		
//		private void addOneHourAfterMarketCloseDateTimeIfTheCase(
//			DateTime nextDateTimeToBeAdded )
//		{
//			if ( this.isOneHourAfterMarktetCloseToBeInserted( nextDateTimeToBeAdded ) )
//				this.addOneHourAfterMarketCloseDateTime( nextDateTimeToBeAdded );
//		}
//		#endregion addOneHourAfterMarketCloseDateTimeIfTheCase
//		
//		private void addNextDateTimeAndIfTheCaseAddOneHourAfterMarketCloseDateTime(
//			DateTime nextDateTimeToBeAdded )
//		{
//			this.addOneHourAfterMarketCloseDateTimeIfTheCase( nextDateTimeToBeAdded );
//			this.dateTimesToBeThrown.Add( nextDateTimeToBeAdded );
//		}
//		
//		#endregion addNextDateTimeAndIfTheCaseAddOneHourAfterMarketCloseDateTime
//		
//		private void addDailyTimeAndIfTheCaseAddOneHourAfterMarketCloseDateTime(
//			Time time , DateTime benchmarkMarketDay )
//		{
//			DateTime nextDateTimeToBeAdded = Time.GetDateTimeFromMerge(
//				benchmarkMarketDay , time );
//			this.addNextDateTimeAndIfTheCaseAddOneHourAfterMarketCloseDateTime(
//				nextDateTimeToBeAdded );
//		}
//		#endregion addDailyTimeAndIfTheCaseAddOneHourAfterMarketCloseDateTime
		
		#region addDailyTimes
		private void addDailyTime( Time time , DateTime benchmarkMarketDay )
		{
			DateTime dateTimeToBeAdded = Time.GetDateTimeFromMerge(
				benchmarkMarketDay , time );
			this.dateTimesToBeThrown.Add( dateTimeToBeAdded );
		}
		private void addDailyTimes( DateTime benchmarkMarketDay )
		{
			foreach ( Time time in this.dailyTimes )
				this.addDailyTime(	time , benchmarkMarketDay );
		}
		#endregion addDailyTimes
		
		#region addOneHourAfterMarketCloseDateTime
		
		#region checkAndAddOneHourAfterMarketCloseDateTime
		private void checkIfOneHourAfterMarketCloseDateTimeFollowsLastAddedDateTime(
			DateTime oneHourAfterMarketCloseDateTime )
		{
			DateTime lastDateTimeAdded = this.dateTimesToBeThrown[
				this.dateTimesToBeThrown.Count - 1 ];
			if ( lastDateTimeAdded >= oneHourAfterMarketCloseDateTime )
				throw new Exception(
					"dailyTimes given to this object's constructor cannot be " +
					"larger or equal to one hour after market close" );
		}
		private void checkAndAddOneHourAfterMarketCloseDateTime(
			DateTime oneHourAfterMarketCloseDateTime )
		{
			this.checkIfOneHourAfterMarketCloseDateTimeFollowsLastAddedDateTime(
				oneHourAfterMarketCloseDateTime );
			this.dateTimesToBeThrown.Add( oneHourAfterMarketCloseDateTime );
		}
		#endregion checkAndAddOneHourAfterMarketCloseDateTime
		
		private void addOneHourAfterMarketCloseDateTime( DateTime benchmarkMarketDay )
		{
			DateTime oneHourAfterMarketCloseDateTime =
				HistoricalEndOfDayTimer.GetOneHourAfterMarketClose(
					benchmarkMarketDay );
			this.checkAndAddOneHourAfterMarketCloseDateTime( oneHourAfterMarketCloseDateTime );
		}
		#endregion addOneHourAfterMarketCloseDateTime
		
		private void addDailyTimesAndOneHourAfterMarketCloseDateTime(
			DateTime benchmarkMarketDay )
		{
			this.addDailyTimes( benchmarkMarketDay );
			this.addOneHourAfterMarketCloseDateTime( benchmarkMarketDay );
		}
		#endregion addDailyTimesAndIfTheCaseAddOneHourAfterMarketCloseDateTime
		
		private void initialize_dateTimesToBeThrown(
			History benchmarkMarketDays )
		{
			this.dateTimesToBeThrown = new List< DateTime >();
			foreach ( DateTime benchmarkMarketDay in benchmarkMarketDays.Keys )
				this.addDailyTimesAndOneHourAfterMarketCloseDateTime(
					benchmarkMarketDay );
		}
		private void initialize_dateTimesToBeThrown()
		{
			History benchmarkMarketDays =
				Quotes.GetMarketDays(
					this.indexTicker , this.firstDateTime , this.lastDateTime );
//				Bars.GetMarketDateTimes(
//					this.indexTicker , this.firstDateTime , this.lastDateTime ,
//					this.dailyTimes, this.intervalFrameInSeconds );
			this.initialize_dateTimesToBeThrown( benchmarkMarketDays );
		}
		#endregion initialize_dateTimesToBeThrown
		
		protected override void initializeTimer()
		{
			this.initialize_dateTimesToBeThrown();
			this.currentDateTimeIndex = 0;
			this.currentDateTime = this.dateTimesToBeThrown[ currentDateTimeIndex ];
		}
		#endregion initializeTimer
		
		#region moveNext
		private void checkIfNoMoreDateTimesAreToBeThrown()
		{
			if ( this.currentDateTimeIndex >= this.dateTimesToBeThrown.Count - 1 )
				throw new Exception(
					"This timer has no other DateTime(s) to be thrown out. " +
					"This should never happen, the backtest should have invoked the method " +
					"QuantProject.Business.Timing.Timer.Stop() before getting to this " +
					"point." );
		}
		
		protected override bool isDone()
		{
			bool isThisTimerDone =
				( this.currentDateTimeIndex >= ( this.dateTimesToBeThrown.Count - 1 ) );
			return isThisTimerDone;
		}
//
//		private void moveNext_actually()
//		{
//			this.currentDateTimeIndex++;
//			this.currentDateTime = this.dateTimesToBeThrown[ currentDateTimeIndex ];
//			this.setCompleteIfTheCase();
//		}
		
		protected override void moveNext()
		{
			this.currentDateTimeIndex++;
			this.currentDateTime = this.dateTimesToBeThrown[ currentDateTimeIndex ];
		}
		#endregion moveNext
	}
}
