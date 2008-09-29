/*
QuantProject - Quantitative Finance Library

FLTPSimpleStrategy.cs
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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Scripts.General.Strategies;

namespace QuantProject.Scripts.WalkForwardTesting.FixedLengthTwoPhases
{
	/// <summary>
	/// Strategy without optimization, that implements the
	/// Fixed Length Two Phases strategy for a given, fixed
	/// WeightedPositions object
	/// </summary>
	public class FLTPSimpleStrategy : SimpleStrategy
	{
		public override bool StopBacktestIfMaxRunningHoursHasBeenReached
		{
			get
			{
				return true;
			}
		}

		public FLTPSimpleStrategy(
			WeightedPositions weightedPositions ,
			IIntervalsSelector intervalsSelector ,
			HistoricalMarketValueProvider historicalMarketValueProvider ) :
			base(
				weightedPositions ,
				intervalsSelector ,
				historicalMarketValueProvider )
		{
		}

		#region getPositionsToBeOpened
		private bool isCurrentIntervalOddInterval()
		{
			bool isOddInterval =
				( this.returnIntervals.Count % 2 == 1 );
			return isOddInterval;
		}
		protected override
			WeightedPositions getPositionsToBeOpened()
		{
			WeightedPositions positionsToBeOpened;
			if ( this.isCurrentIntervalOddInterval() )
				// the current interval is an odd interval (first, third, ...)
				positionsToBeOpened = this.weightedPositions;
			else
				// the current interval is an even interval (second, fourth, ...)
				positionsToBeOpened = this.weightedPositions.Opposite;
			return positionsToBeOpened;
		}
		#endregion getPositionsToBeOpened
		
		protected override string getTextIdentifier()
		{
			return "FLTPSmplStrtgy";
		}
	}
}
