/*
QuantProject - Quantitative Finance Library

OddIntervalsSelector.cs
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
	/// Selects a set of intervals where there is a fixed number of benchmark
	/// time steps
	/// for odd intervals (first, third, ...) and a (possibly) different
	/// fixed number of benchmark time steps for even intervals (second, fourth, ...).
	/// A benchmark time step is either an "open to close" interval or
	/// a "close to open" interval (refered to dateTimes when the benchmark is
	/// exchanged)
	/// Only odd intervals (first, third, ...) are returned, even intervals
	/// (second, fourth, ...) are skipped instead
	/// </summary>
	[Serializable]
	public class OddIntervalsSelector : IIntervalsSelector
	{
//		private int benchmarkTimeStepsForOddIntervals;
//		private int benchmarkTimeStepsForEvenIntervals;
//		private Benchmark benchmark;
		protected FixedLengthTwoPhasesIntervalsSelector
			fixedLengthTwoPhasesIntervalsSelector;
		
		/// <summary>
		/// Selects a set of intervals where there is a fixed number of benchmark
		/// time steps
		/// for odd intervals (first, third, ...) and a (possibly) different
		/// fixed number of benchmark time steps for even intervals (second, fourth, ...).
		/// A benchmark time step is either an "open to close" interval or
		/// a "close to open" interval (refered to dateTimes when the benchmark is
		/// exchanged)
		/// Only odd intervals (first, third, ...) are returned, even intervals
		/// (second, fourth, ...) are skipped instead
		/// </summary>
		/// <param name="benchmarkTimeStepsForOddIntervals">number of benchmark time steps
		/// for odd intervals (first, third, fifth, ...)</param>
		/// <param name="benchmarkTimeStepsForEvenIntervals">number of benchmark time steps
		/// for even intervals (second, fourth, sixth, ...)</param>
		/// <param name="benchmark"></param>
		public OddIntervalsSelector(
			int benchmarkTimeStepsForOddIntervals ,
			int benchmarkTimeStepsForEvenIntervals ,
			Benchmark benchmark )
		{
			this.fixedLengthTwoPhasesIntervalsSelector =
				new FixedLengthTwoPhasesIntervalsSelector(
					benchmarkTimeStepsForOddIntervals ,
					benchmarkTimeStepsForEvenIntervals ,
					benchmark );
		}
		
		public ReturnInterval GetNextInterval( ReturnIntervals returnIntervals )
		{
			ReturnInterval nextEvenInterval =
				this.fixedLengthTwoPhasesIntervalsSelector.GetNextInterval(
					returnIntervals );
			ReturnIntervals returnIntervalsWithTheNextEvenIntervalOnly =
				new ReturnIntervals( nextEvenInterval );
			ReturnInterval nextOddInterval =
				this.fixedLengthTwoPhasesIntervalsSelector.GetNextInterval(
					returnIntervalsWithTheNextEvenIntervalOnly );
			return nextOddInterval;
		}
		public virtual ReturnInterval GetFirstInterval(
			DateTime startingDateTime )
		{
			ReturnInterval firstInterval =
				this.fixedLengthTwoPhasesIntervalsSelector.GetFirstInterval(
					startingDateTime );
			return firstInterval;
		}
	
	}
}
