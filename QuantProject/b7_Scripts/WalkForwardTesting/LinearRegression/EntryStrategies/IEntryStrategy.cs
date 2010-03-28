/*
QuantProject - Quantitative Finance Library

IEntryStrategy.cs
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

using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement.Time;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Returns the WeightedPositions to be opened out of sample.
	/// Returns null if none is suitable
	/// </summary>
	public interface IEntryStrategy
	{
		/// <summary>
		/// Returns the WeightedPositions to be opened out of sample.
		/// Returns null if none is suitable
		/// </summary>
		/// <returns>null if it finds no positions as required</returns>
		WeightedPositions GetPositionsToBeOpened(
			TestingPositions[] bestTestingPositionsInSample ,
			ReturnIntervals outOfSampleReturnIntervals );
	}
}
