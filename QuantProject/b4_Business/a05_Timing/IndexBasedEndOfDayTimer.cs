/*
QuantProject - Quantitative Finance Library

IndexBasedEndOfDayTimer.cs
Copyright (C) 2003
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
using QuantProject.Data.DataTables;

using QuantProject.ADT;

namespace QuantProject.Business.Timing
{
	/// <summary>
	/// ITimer implementation using a market index as the base
	/// for time walking simulation; end of day timing events are risen
	/// only for those days when the index is traded
	/// </summary>
	[Serializable]
	public class IndexBasedEndOfDayTimer : HistoricalEndOfDayTimer
	{
		private string marketIndex;
		public string MarketIndex
		{
			get	{	return this.marketIndex;	}
		}
		
		[NonSerialized]
		private Quotes indexQuotes;
		public Quotes IndexQuotes
		{
			get { return this.indexQuotes;  }
		}
		
		private int currentDateArrayPosition;
		public int CurrentDateArrayPosition
		{
			get { return this.currentDateArrayPosition ;  }
		}

		
		#region commonInitialization
		private void indexBasedEndOfDayTimer(
			DateTime startDateTime, string marketIndex)
		{
			this.marketIndex = marketIndex;
			if ( this.indexQuotes.Rows.Count == 0 )
			{
				string errorMessage = "IndexBasedEndOfDayTimer error: the given " +
					"index (" + marketIndex + ") has no quotes in the interval.";
				throw new Exception( errorMessage );
			}
			this.StartDateTime =
				HistoricalEndOfDayTimer.GetMarketOpen( startDateTime );
			this.tickers = new Hashtable();
			this.currentDateArrayPosition = 0;
		}
		private void commonInitialization(
			string marketIndex ,
			DateTime startDateTime ,
			DateTime endDateTime )
		{
			DateTime startDate =
				ExtendedDateTime.GetDate( startDateTime );
			this.indexQuotes = new Quotes(marketIndex,startDate,DateTime.Now);
			this.indexBasedEndOfDayTimer(startDateTime, marketIndex);
		}
		#endregion commonInitialization

		public IndexBasedEndOfDayTimer(
			DateTime startDateTime,
			string marketIndex): base(startDateTime)
		{
			this.commonInitialization( marketIndex,startDateTime,DateTime.Now );
		}
		
		public IndexBasedEndOfDayTimer(
			DateTime startDateTime,
			DateTime endDateTime,
			string marketIndex): base(startDateTime)
		{
			this.commonInitialization( marketIndex,startDateTime,endDateTime.AddDays(10) );
		}
		
//		/// <summary>
//		/// Starts the time walking simulation, based on index's dates
//		/// </summary>
//		public override void Start()
//		{
//			base.activateTimer();
//			while ( this.isActive )
//			{
//				base.callEvents();
//				this.moveNext();
//			}
//		}
		
		protected override bool isDone()
		{
			bool lastDateHasAlreadyBeenLaunched;
			lastDateHasAlreadyBeenLaunched = 
				this.IndexQuotes.Rows.Count - 1 == this.currentDateArrayPosition;
			return lastDateHasAlreadyBeenLaunched;
		}
		
		#region moveNext
		private void moveNext_with_currentDateTime_properlyInitialized()
		{
			if ( HistoricalEndOfDayTimer.IsOneHourAfterMarketClose(
				this.currentDateTime ) )
			{
				// current time is the last end of day event for the day
				DateTime followingDateForIndex =
					this.indexQuotes.GetFollowingDate(this.currentDateTime, 1);
				this.currentDateTime =
					HistoricalEndOfDayTimer.GetMarketOpen( followingDateForIndex );
				this.currentDateArrayPosition++;
			}
			else
				// current time is the last end of day event for the day
				base.moveNext();
				
//			this.currentDateTime =
//				HistoricalEndOfDayTimer.ge
//			EndOfDaySpecificTime nextSpecificTime = this.currentTime.GetNextEndOfDaySpecificTime();
//			if ( nextSpecificTime < this.currentTime.EndOfDaySpecificTime )
//			{
//				// the current end of day specific time is the last end of day specific time in the day
//				this.currentTime.DateTime =
//					this.indexQuotes.GetFollowingDate(this.currentTime.DateTime, 1);
//				this.currentDateArrayPosition++;
//			}
//			this.currentTime.EndOfDaySpecificTime = nextSpecificTime;
		}
		//move the current endOfDayDateTime to the next moment
		//at which the market is surely open
		protected override void moveNext()
		{
			DateTime dateTimeForTheFirstStep =
				HistoricalEndOfDayTimer.GetMarketOpen(
			    	this.indexQuotes.StartDate );
			if ( this.currentDateTime < dateTimeForTheFirstStep )
				this.currentDateTime = dateTimeForTheFirstStep;
			this.moveNext_with_currentDateTime_properlyInitialized();
		}
		#endregion moveNext
		
		/// <summary>
		/// Gets the previous index based date time
		/// </summary>
		public DateTime GetPreviousDateTime()
		{
			return this.indexQuotes.GetPrecedingDate(this.currentDateTime,1);
		}
		/// <summary>
		/// Gets the date time that is 'precedingDays' days before
		/// the current date time of the current timer
		/// </summary>
		public DateTime GetPreviousDateTime(int precedingDays)
		{
			return this.indexQuotes.GetPrecedingDate(
				this.currentDateTime,precedingDays);
		}

	}
}
