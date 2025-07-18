/*
QuantProject - Quantitative Finance Library

SimpleStrategy.cs
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

namespace QuantProject.Scripts.General.Strategies
{
	/// <summary>
	/// Strategy without optimization, that opens positions when a new
	/// interval begins and closes positions when the interval ends
	/// </summary>
	public class SimpleStrategy : SymmetricEndOfDayStrategyForBacktester
	{
		protected WeightedPositions weightedPositions;

		public SimpleStrategy(
			WeightedPositions weightedPositions ,
			IIntervalsSelector intervalsSelector ,
			HistoricalMarketValueProvider historicalMarketValueProvider ) :
			base(	999 ,	1 ,	intervalsSelector , intervalsSelector ,
			     new DummyEligibleSelector() ,
			     new DummyInSampleChooser() ,
			     historicalMarketValueProvider )
		{
			this.weightedPositions = weightedPositions;
		}

		protected override bool arePositionsToBeClosed()
		{
			// true iif the current EndOfDayDateTime falls on the end of either the last
			// interval or the second last interval
			bool arePositionsToBeClosed =
				( ( this.Account.Portfolio.Count > 0 ) &&
				 ( ( this.returnIntervals[ this.returnIntervals.Count - 1 ].End ==
				    this.now() )
				  ||
				  ( ( this.returnIntervals.Count > 1 ) &&
				   ( this.returnIntervals[ this.returnIntervals.Count - 2 ].End ==
				   	this.now() ) ) ) );
			return arePositionsToBeClosed;
		}
//		protected override bool marketCloseEventHandler_arePositionsToBeClosed()
//		{
//			return this.marketOpenEventHandler_arePositionsToBeClosed();
//		}
		protected override bool arePositionsToBeOpened()
		{
			// true iif the current EndOfDayDateTime falls on the begin of the last
			// interval
			bool arePositionsToBeOpened =
				( ( this.Account.Portfolio.Count == 0 ) &&
				 this.returnIntervals[ this.returnIntervals.Count - 1 ].Begin ==
				 this.now() );
			return arePositionsToBeOpened;
		}
//		protected override bool marketCloseEventHandler_arePositionsToBeOpened()
//		{
//			return this.marketOpenEventHandler_arePositionsToBeOpened();
//		}
		protected override
			WeightedPositions getPositionsToBeOpened()
		{
			return this.weightedPositions;
		}
//		protected override
//			WeightedPositions marketCloseEventHandler_getPositionsToBeOpened()
//		{
//			return this.marketOpenEventHandler_getPositionsToBeOpened();
//		}
		protected override LogItem getLogItem( EligibleTickers eligibleTickers )
		{
			DummyLogItem logItem = new DummyLogItem( this.now() );
			return logItem;
		}
		protected override string getTextIdentifier()
		{
			return "SmplStrtgy";
		}
	}
}
