/*
QuantProject - Quantitative Finance Library

IntradayReturnIntervals.cs
Copyright (C) 2009
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

using QuantProject.Business.Timing;

namespace QuantProject.Business.Strategies.ReturnsManagement.Time
{
	/// <summary>
	/// Intraday intervals to be used to compute intraday returns
	/// </summary>
	[Serializable]
	public class IntradayIntervals : ReturnIntervals
	{
		private QuantProject.ADT.Timing.Time firstDailyTime;
		private QuantProject.ADT.Timing.Time lastDailyTime;
		
		/// <summary>
		/// Creates the intraday intervals, of the given length in minutes,
		/// for the given benchmark, from
		/// the first DateTime to the last DateTime
		/// </summary>
		/// <param name="firstDateTime"></param>
		/// <param name="lastDateTime"></param>
		/// <param name="intervalLengthInMinutes"></param>
		/// <param name="benchmark"></param>
		public IntradayIntervals( DateTime firstDateTime , DateTime lastDateTime ,
		  int intervalLengthInMinutes, string benchmark ) : 
			base( firstDateTime , lastDateTime , benchmark, intervalLengthInMinutes )
		{
		}
		
		#region setIntervals
		private void addInterval( DateTime day, 
		                          QuantProject.ADT.Timing.Time dailyTimeForIntervalBegin,
		                          QuantProject.ADT.Timing.Time dailyTimeForIntervalEnd )
		{
			DateTime dateTimeForIntervalBegin =
				QuantProject.ADT.Timing.Time.GetDateTimeFromMerge(day, dailyTimeForIntervalBegin);
			DateTime dateTimeForIntervalEnd =
				QuantProject.ADT.Timing.Time.GetDateTimeFromMerge(day, dailyTimeForIntervalEnd);
			ReturnInterval returnInterval = new ReturnInterval(
				dateTimeForIntervalBegin , dateTimeForIntervalEnd );
			this.Add( returnInterval );
		}
		private void addIntervalsForTheCurrentDay( int indexOfDay )
		{
			DateTime day = (DateTime)this.marketDaysForBenchmark.GetKey( indexOfDay );
			QuantProject.ADT.Timing.Time currentBeginDailyTime = this.firstDailyTime;
			QuantProject.ADT.Timing.Time currentEndDailyTime = this.firstDailyTime.AddMinutes(this.intervalLength);
			while( currentBeginDailyTime < this.lastDailyTime && 
			       currentEndDailyTime <= this.lastDailyTime )
			{
				this.addInterval( day, currentBeginDailyTime, currentEndDailyTime );
				currentBeginDailyTime = currentEndDailyTime;
				currentEndDailyTime = currentBeginDailyTime.AddMinutes( intervalLength );
			}
		}
		
		protected override void setIntervals()
		{
			this.firstDailyTime = new QuantProject.ADT.Timing.Time(
				this.firstDateTime.Hour, this.firstDateTime.Minute,
				this.firstDateTime.Second);
			this.lastDailyTime = new QuantProject.ADT.Timing.Time(
				this.lastDateTime.Hour, this.lastDateTime.Minute,
				this.lastDateTime.Second);
			for( int i = 0 ; i < this.marketDaysForBenchmark.Count ; i++ )
				this.addIntervalsForTheCurrentDay( i );
		}
		
		#endregion setIntervals
	}
}

