/*
QuantProject - Quantitative Finance Library

PairsTradingStrategy.cs
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
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;


namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Pairs Trading strategy with in sample optimizations
	/// </summary>
	public class PairsTradingStrategy : SymmetricEndOfDayStrategyForBacktester
	{
		private IHistoricalQuoteProvider
			historicalQuoteProviderForChosingPositionsOutOfSample;
		private OutOfSampleChooser outOfSampleChooser;

		public PairsTradingStrategy(
			int numDaysBeetweenEachOtpimization ,
			int numDaysForInSampleOptimization ,
			IIntervalsSelector intervalsSelectorForInSample ,
			IIntervalsSelector intervalsSelectorForOutOfSample ,
			IEligiblesSelector eligiblesSelector ,
			IInSampleChooser inSampleChooser ,
			IHistoricalQuoteProvider historicalQuoteProviderForInSample ,
			IHistoricalQuoteProvider
			historicalQuoteProviderForChosingPositionsOutOfSample ,
			OutOfSampleChooser outOfSampleChooser ) :
			base(
			numDaysBeetweenEachOtpimization ,
			numDaysForInSampleOptimization ,
			intervalsSelectorForInSample ,
			intervalsSelectorForOutOfSample ,
			eligiblesSelector ,
			inSampleChooser ,
			historicalQuoteProviderForInSample )
		{
			this.historicalQuoteProviderForChosingPositionsOutOfSample =
				historicalQuoteProviderForChosingPositionsOutOfSample;			
			this.outOfSampleChooser = outOfSampleChooser;
		}
		
		protected override string getTextIdentifier()
		{
			return "pairsTrdng";
		}
		
		#region arePositionsToBeClosed
		private ReturnInterval getSecondLastReturnInterval()
		{
			int indexForTheSecondLastReturnInterval =
				this.returnIntervals.Count - 2;
			ReturnInterval secondLastReturnInterval =
				this.returnIntervals[ indexForTheSecondLastReturnInterval ];
			return secondLastReturnInterval;
		}
		protected override bool arePositionsToBeClosed()
		{
			bool areToBeClosed = false;
			if ( this.returnIntervals.Count > 1 )
				// at least two intervals have already been
				// added, out of sample
			{
				ReturnInterval secondLastReturnInterval =
					this.getSecondLastReturnInterval();
				areToBeClosed = this.now().IsEqualTo( secondLastReturnInterval.End );				
			}
			return ( areToBeClosed );
		}
		#endregion arePositionsToBeClosed

		protected override bool arePositionsToBeOpened()
		{
			bool beginsTheLastInterval =
				( this.now().IsEqualTo(
				this.lastIntervalAppended().Begin ) );
			return ( beginsTheLastInterval );
		}

		protected override WeightedPositions getPositionsToBeOpened()
		{
			WeightedPositions weightedPositions =
				this.outOfSampleChooser.GetPositionsToBeOpened(
				this.bestTestingPositionsInSample ,
				this.returnIntervals ,
				this.historicalQuoteProviderForChosingPositionsOutOfSample ,
				this.inSampleReturnsManager );
			return weightedPositions;
		}
		
		protected override LogItem getLogItem( EligibleTickers eligibleTickers )
		{
			PairsTradingLogItem logItem =
				new PairsTradingLogItem( this.now() ,
				                        this.bestTestingPositionsInSample ,
				                        this.numDaysForInSampleOptimization ,
				                        eligibleTickers.Count );
			return logItem;
		}
	}
}
