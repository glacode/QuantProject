/*
QuantProject - Quantitative Finance Library

BasicEndOfDayStrategyForBacktester.cs
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
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;

namespace QuantProject.Business.Strategies
{
	/// <summary>
	/// To be implemented by those strategies that take the same
	/// actions both on market open and on market close.
	/// For these strategies, decisions are not based on the type of
	/// market status switch, but rather on the underlying out of sample
	/// intervals.
	/// In practical terms, these strategies run the same code for both on
	/// market open and market close events
	/// </summary>
	public abstract class SymmetricEndOfDayStrategyForBacktester :
		BasicEndOfDayStrategyForBacktester
	{
		public SymmetricEndOfDayStrategyForBacktester(
			int numDaysBeetweenEachOtpimization ,
			int numDaysForInSampleOptimization ,
			IIntervalsSelector intervalsSelectorForInSample ,
			IIntervalsSelector intervalsSelectorForOutOfSample ,
			IEligiblesSelector eligiblesSelector ,
			IInSampleChooser inSampleChooser ,
			IHistoricalQuoteProvider historicalQuoteProviderForInSample	) :
			base(
			numDaysBeetweenEachOtpimization ,
			numDaysForInSampleOptimization ,
			intervalsSelectorForInSample ,
			intervalsSelectorForOutOfSample ,
			eligiblesSelector ,
			inSampleChooser ,
			historicalQuoteProviderForInSample )
		{
			//
			// TODO: Add constructor logic here
			//
		}
		protected abstract bool arePositionsToBeClosed();
		protected abstract bool arePositionsToBeOpened();
		protected abstract WeightedPositions getPositionsToBeOpened();

		protected sealed override bool marketOpenEventHandler_arePositionsToBeClosed()
		{
			return this.arePositionsToBeClosed();
		}
		protected sealed override bool marketOpenEventHandler_arePositionsToBeOpened()
		{
			return this.arePositionsToBeOpened();
		}
		protected sealed override WeightedPositions
			marketOpenEventHandler_getPositionsToBeOpened()
		{
			return this.getPositionsToBeOpened();
		}
		protected sealed override bool marketCloseEventHandler_arePositionsToBeClosed()
		{
			return this.arePositionsToBeClosed();
		}
		protected sealed override bool marketCloseEventHandler_arePositionsToBeOpened()
		{
			return this.arePositionsToBeOpened();
		}
		protected sealed override WeightedPositions
			marketCloseEventHandler_getPositionsToBeOpened()
		{
			return this.getPositionsToBeOpened();
		}
	}
}
