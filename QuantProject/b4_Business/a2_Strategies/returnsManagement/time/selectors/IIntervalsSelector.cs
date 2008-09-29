/*
QuantProject - Quantitative Finance Library

IIntervalsSelector.cs
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

using QuantProject.Business.Timing;

namespace QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors
{
	/// <summary>
	/// Interface for intervals' selectors. An interval selector is used in walk
	/// forward strategy to compute return intervals for both the in sample
	/// optimizations and the out of sample strategy
	/// </summary>
	public interface IIntervalsSelector
	{
		/// <summary>
		/// returns the first interval of a series (starting from dateTime)
		/// </summary>
		ReturnInterval GetFirstInterval( DateTime dateTime );

		/// <summary>
		/// returns the next interval for the given returnIntervals
		/// </summary>
		ReturnInterval GetNextInterval( ReturnIntervals returnIntervals );
	}
}
