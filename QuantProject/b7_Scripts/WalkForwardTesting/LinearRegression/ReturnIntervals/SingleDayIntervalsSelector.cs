/*
QuantProject - Quantitative Finance Library

SingleDayCloseToCloseIntervalsSelector.cs
Copyright (C) 2010
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

using QuantProject.ADT.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Returns an interval only if the benchmark has values
	/// both at the interval begin and at the interval end.
	/// The interval must be one day long. Then, for instance,
	/// thursday market close to friday market close is returned, but
	/// friday market close to monday marked close is not returned, because
	/// three days elapse since friday market close to monday market close
	/// </summary>
	[Serializable]
	public class SingleDayIntervalsSelector : IIntervalsSelector
	{
		private Benchmark benchmark;
		private Time dailyTime;
		private List<DayOfWeek> acceptableDaysOfTheWeekForTheEndOfEachInterval;
		private TimeSpan maxTimeSpanToLookAhead;
		
		/// <summary>
		/// Returns an interval only if the benchmark has values
		/// both at the interval begin and at the interval end.
		/// The interval must be one day long. Then, for instance,
		/// thursday market close to friday market close is returned, but
		/// friday market close to monday marked close is not returned, because
		/// three days elapse since friday market close to monday market close
		/// </summary>
		/// <param name="benchmark"></param>
		/// <param name="dailyTime">each interval will go from
		/// this time for one day to this same time for the next day</param>
		/// <param name="acceptableDaysOfTheWeekForTheEndOfEachInterval">an interval
		/// is returned only if the day of the week on which the interval ends
		/// is in this list</param>
		/// <param name="maxTimeSpanToLookAhead">if a new interval is not found
		/// before maxTimeSpanToLookAhead has elapsed, then stop searching for
		/// the next interval</param>
		public SingleDayIntervalsSelector(
			Benchmark benchmark , Time dailyTime ,
			List<DayOfWeek> acceptableDaysOfTheWeekForTheEndOfEachInterval ,
			TimeSpan maxTimeSpanToLookAhead )
		{
			this.benchmark = benchmark;
			this.dailyTime = dailyTime;
			this.acceptableDaysOfTheWeekForTheEndOfEachInterval =
				acceptableDaysOfTheWeekForTheEndOfEachInterval;
			this.maxTimeSpanToLookAhead = maxTimeSpanToLookAhead;
		}
		
		private bool isValidReturnInterval( ReturnInterval returnInterval )
		{
			TimeSpan intervalLength = returnInterval.End.Subtract( returnInterval.Begin );
			TimeSpan requiredIntervalLength = TimeSpan.FromDays( 1 );
			bool isValidReturnInterval = ( intervalLength == requiredIntervalLength );
			isValidReturnInterval &= 
				this.acceptableDaysOfTheWeekForTheEndOfEachInterval.Contains(
					returnInterval.End.DayOfWeek );
			return isValidReturnInterval;
		}
		
		#region GetFirstInterval
		
		#region getReturnIntervalCandidate
		private DateTime getFirstIntervalBegin(
			DateTime startingDateTime , DateTime lastAcceptableDateTime )
		{
			DateTime firstIntervalBegin =
				this.benchmark.GetThisOrNextDateTimeWhenTraded(
					startingDateTime , this.dailyTime , lastAcceptableDateTime );
			return firstIntervalBegin;
		}
		private DateTime getIntervalEnd(
			DateTime startingDateTime , DateTime lastAcceptableDateTime )
		{
			DateTime intervalEnd =
				this.benchmark.GetThisOrNextDateTimeWhenTraded(
					startingDateTime.AddMinutes( 1 ) , this.dailyTime , lastAcceptableDateTime );
			return intervalEnd;
		}
		private ReturnInterval getReturnIntervalCandidate(
			DateTime startingDateTime , DateTime lastAcceptableDateTime )
		{
			ReturnInterval intervalCandidate = null;
			DateTime intervalCandidateBegin =
				this.getFirstIntervalBegin( startingDateTime , lastAcceptableDateTime );
			DateTime intervalCandidateEnd = DateTime.MinValue;
			if ( intervalCandidateBegin != DateTime.MinValue )
				// a potentially valid first interval begin has been found
				intervalCandidateEnd =
					this.getIntervalEnd( intervalCandidateBegin , lastAcceptableDateTime );
			if ( ( intervalCandidateBegin != DateTime.MinValue ) &&
			    ( intervalCandidateEnd != DateTime.MinValue ) )
				// a potentially valida return interval has been found
				intervalCandidate = new ReturnInterval(
					intervalCandidateBegin , intervalCandidateEnd );
			return intervalCandidate;
		}
		#endregion getReturnIntervalCandidate
		
		public ReturnInterval GetFirstInterval(
			DateTime startingDateTime )
		{
			DateTime lastAcceptableDateTime = startingDateTime.Add( this.maxTimeSpanToLookAhead );
			ReturnInterval returnIntervalCandidate = this.getReturnIntervalCandidate(
				startingDateTime , lastAcceptableDateTime );
			while ( ( returnIntervalCandidate != null ) &&
			       !this.isValidReturnInterval( returnIntervalCandidate ) &&
			       ( returnIntervalCandidate.End < lastAcceptableDateTime ) )
				returnIntervalCandidate = this.getReturnIntervalCandidate(
					returnIntervalCandidate.End , lastAcceptableDateTime );
			return returnIntervalCandidate;
		}
		#endregion GetFirstInterval
		
		public ReturnInterval GetNextInterval( ReturnIntervals returnIntervals )
		{
			DateTime firstAcceptableDateTimeForNextInterval = returnIntervals.LastDateTime;
			ReturnInterval nextInterval = this.GetFirstInterval(
				firstAcceptableDateTimeForNextInterval );
			return nextInterval;
		}
	}
}
