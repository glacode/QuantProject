/*
QuantProject - Quantitative Finance Library

EvenIntervalsSelector.cs
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
	/// Only even intervals (second, fourth, ...) are returned, odd intervals
	/// (first, third, ...) are skipped instead
	/// </summary>
	[Serializable]
	public class EvenIntervalsSelector : OddIntervalsSelector
	{
		
		/// <summary>
		/// Selects a set of intervals where there is a fixed number of benchmark
		/// time steps
		/// for odd intervals (first, third, ...) and a (possibly) different
		/// fixed number of benchmark time steps for even intervals (second, fourth, ...).
		/// A benchmark time step is either an "open to close" interval or
		/// a "close to open" interval (refered to dateTimes when the benchmark is
		/// exchanged)
		/// Only even intervals (second, fourth, ...) are returned, odd intervals
		/// (first, third, ...) are skipped instead
		/// </summary>
		/// <param name="benchmarkTimeStepsForOddIntervals">number of benchmark time steps
		/// for odd intervals (first, third, fifth, ...)</param>
		/// <param name="benchmarkTimeStepsForEvenIntervals">number of benchmark time steps
		/// for even intervals (second, fourth, sixth, ...)</param>
		/// <param name="benchmark"></param>
		public EvenIntervalsSelector(
			int benchmarkTimeStepsForOddIntervals ,
			int benchmarkTimeStepsForEvenIntervals ,
			Benchmark benchmark ) : base(
			benchmarkTimeStepsForOddIntervals ,
			benchmarkTimeStepsForEvenIntervals ,
			benchmark )
		{
		}
		
		public override ReturnInterval GetFirstInterval(
			DateTime startingDateTime )
		{
			ReturnInterval firstInterval =
				this.fixedLengthTwoPhasesIntervalsSelector.GetFirstInterval(
					startingDateTime );
			ReturnIntervals returnIntervalsWithTheFirstIntervalOnly =
				new ReturnIntervals( firstInterval );
			ReturnInterval secondInterval =
				this.fixedLengthTwoPhasesIntervalsSelector.GetNextInterval(
				returnIntervalsWithTheFirstIntervalOnly );
			return secondInterval;
		}	
	}
}
