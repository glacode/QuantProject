/*
QuantProject - Quantitative Finance Library

FixedLengthTwoPhasesIntervalsSelector.cs
Copyright (C) 2007
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

using QuantProject.ADT.Histories;
using QuantProject.Business.Timing;

namespace QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors
{
	/// <summary>
	/// Selects a set of intervals where there is a fixed number of time steps
	/// for even intervals (first, third, ...) and (possilby) a different
	/// fixed number of benchmark time steps for odd intervals (second, fourth, ...).
	/// A benchmark time step is either an "open to close" interval or
	/// a "close to open" interval (refered to dateTimes when the benchmark is
	/// exchanged)
	/// </summary>
	public class FixedLengthTwoPhasesIntervalsSelector : IIntervalsSelector
	{
		private int firstPhaseBenchmarkTimeSteps;
		private int secondPhaseBenchmarkTimeSteps;
		private Benchmark benchmark;
		
		/// <summary>
		/// Selects a set of intervals where there is a fixed number of time steps
		/// for even intervals (first, third, ...) and (possilby) a different
		/// fixed number of benchmark time steps for odd intervals (second, fourth, ...)
		/// A benchmark time step is either an "open to close" interval or
		/// a "close to open" interval (refered to dateTimes when the benchmark is
		/// exchanged)
		/// </summary>
		/// <param name="firstPhaseBenchmarkTimeSteps">number of benchmark time steps
		/// for the first phase (odd intervals)</param>
		/// <param name="secondPhaseBenchmarkTimeSteps">number of benchmark time steps
		/// for the second phase (even intervals)</param>
		/// <param name="benchmark"></param>
		public FixedLengthTwoPhasesIntervalsSelector(
			int firstPhaseBenchmarkTimeSteps ,
			int secondPhaseBenchmarkTimeSteps ,
			Benchmark benchmark )
		{
			this.firstPhaseBenchmarkTimeSteps = firstPhaseBenchmarkTimeSteps;
			this.secondPhaseBenchmarkTimeSteps = secondPhaseBenchmarkTimeSteps;
			this.benchmark = benchmark;
		}
		#region getIntervalEnd
		private int getBenchmarkTimeStepsForCurrentPhase( int numIntervalsAlreadyDone )
		{
			int benchmarkTimeStepsForCurrentPhase;
			if ( ( numIntervalsAlreadyDone % 2 ) == 0 )
				// current phase is the first phase
				benchmarkTimeStepsForCurrentPhase = this.firstPhaseBenchmarkTimeSteps;
			else
				// current phase is the second phase
				benchmarkTimeStepsForCurrentPhase = this.secondPhaseBenchmarkTimeSteps;
			return benchmarkTimeStepsForCurrentPhase;
		}
		private EndOfDayDateTime getNextReturnIntervalEnd(
			EndOfDayDateTime returnIntervalBegin ,
			int benchmarkTimeStepsForCurrentPhase )
		{
			EndOfDayDateTime currentEndOfDayDateTime =
				returnIntervalBegin;
			for ( int i = 0 ; i < benchmarkTimeStepsForCurrentPhase ; i++ )
			{
				currentEndOfDayDateTime =
					this.benchmark.GetTimeStep( currentEndOfDayDateTime ).End;
			}
			return currentEndOfDayDateTime;
		}
		private EndOfDayDateTime getIntervalEnd(
			EndOfDayDateTime nextReturnIntervalBegin ,
			int numIntervalsAlreadyDone )
		{
			int benchmarkTimeStepsForCurrentPhase =
				this.getBenchmarkTimeStepsForCurrentPhase( numIntervalsAlreadyDone );
			EndOfDayDateTime nextReturnIntervalEnd =
				this.getNextReturnIntervalEnd( nextReturnIntervalBegin ,
				benchmarkTimeStepsForCurrentPhase );
			return nextReturnIntervalEnd;
		}
		#endregion getIntervalEnd
		
		public ReturnInterval GetNextInterval( ReturnIntervals returnIntervals )
		{
			EndOfDayDateTime nextReturnIntervalBegin =
				returnIntervals[ returnIntervals.Count - 1 ].End;
			EndOfDayDateTime nextReturnIntervalEnd =
				this.getIntervalEnd( nextReturnIntervalBegin ,
				                                        returnIntervals.Count );
			ReturnInterval nextInterval = new ReturnInterval(
				nextReturnIntervalBegin , nextReturnIntervalEnd );
			return nextInterval;
		}
		#region GetFirstInterval
		private EndOfDayDateTime getFirstIntervalBegin(
			EndOfDayDateTime startingEndOfDayDateTime )
		{
//			qui!!!! fai un ciclo while non exchanged (trova
//			                                          il primo
//			                                          successivo
//			                                          in cui il
//			                                          benchmark e'
//			                                          scambiato)
			EndOfDayDateTime firstIntervalBegin =
				this.benchmark.GetThisOrNextMarketStatusSwitch(
				startingEndOfDayDateTime );
//			EndOfDayDateTime firstIntervalBegin =
//				startingEndOfDayDateTime;
//			if ( ( startingEndOfDayDateTime.EndOfDaySpecificTime !=
//			      EndOfDaySpecificTime.MarketOpen ) &&
//			      ( startingEndOfDayDateTime.EndOfDaySpecificTime !=
//			       EndOfDaySpecificTime.MarketClose ) )
//				// startingEndOfDayDateTime does not mark a market
//				// status switch
//				firstIntervalBegin =
//					startingEndOfDayDateTime.GetNextMarketStatusSwitch();
			return firstIntervalBegin;
		}
		public ReturnInterval GetFirstInterval(
				EndOfDayDateTime startingEndOfDayDateTime )
		{
			EndOfDayDateTime firstIntervalBegin =
				this.getFirstIntervalBegin( startingEndOfDayDateTime );
			EndOfDayDateTime firstIntervalEnd =
				this.getIntervalEnd( firstIntervalBegin , 0 );
			ReturnInterval nextInterval = new ReturnInterval(
				firstIntervalBegin , firstIntervalEnd );
			return nextInterval;
		}
		#endregion GetFirstInterval
	}
}
