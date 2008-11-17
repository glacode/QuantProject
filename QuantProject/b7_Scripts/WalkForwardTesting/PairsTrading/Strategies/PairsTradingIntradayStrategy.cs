/*
QuantProject - Quantitative Finance Library

PairsTradingIntradayStrategy.cs
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

using QuantProject.ADT.Timing;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Pairs trading strategy with intraday bars
	/// </summary>
	public class PairsTradingIntradayStrategy : BasicStrategyForBacktester
	{
		private Time firstTimeToTestInefficiency;
		private Time lastTimeToTestInefficiency;
		private Time timeToClosePositions;
		private HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample;
		private OutOfSampleChooser outOfSampleChooser;
		
		public PairsTradingIntradayStrategy(
			int numDaysBeetweenEachOtpimization ,
			int numDaysForInSampleOptimization ,
			IIntervalsSelector intervalsSelectorForInSample ,
//			IIntervalsSelector intervalsSelectorForOutOfSample ,
			Time firstTimeToTestInefficiency ,
			Time lastTimeToTestInefficiency ,
			Time timeToClosePositions ,
			IEligiblesSelector eligiblesSelector ,
			IInSampleChooser inSampleChooser ,
			HistoricalMarketValueProvider historicalMarketValueProviderForInSample ,
			HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample ,
			OutOfSampleChooser outOfSampleChooser ) :
			base(
				numDaysBeetweenEachOtpimization ,
				numDaysForInSampleOptimization ,
				intervalsSelectorForInSample ,
//			intervalsSelectorForOutOfSample ,
				eligiblesSelector ,
				inSampleChooser ,
				historicalMarketValueProviderForInSample )
		{
			this.firstTimeToTestInefficiency = firstTimeToTestInefficiency;
			this.lastTimeToTestInefficiency = lastTimeToTestInefficiency;
			this.timeToClosePositions = timeToClosePositions;
			this.historicalMarketValueProviderForChosingPositionsOutOfSample =
				historicalMarketValueProviderForChosingPositionsOutOfSample;
			this.outOfSampleChooser = outOfSampleChooser;
		}
		
		protected override string getTextIdentifier()
		{
			return "pairsTrdngIntrdy";
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
		
		protected override bool arePositionsToBeClosed()
		{
			bool areToBeClosed =
				( this.Account.Portfolio.Count > 1 );
			areToBeClosed = (
				areToBeClosed &&
				( this.time() == this.timeToClosePositions ) );
			return ( areToBeClosed );
		}
		
		protected override bool arePositionsToBeOpened()
		{
			bool areToBeOpened = ( this.time() == this.lastTimeToTestInefficiency );
			return ( areToBeOpened );
		}

		#region getPositionsToBeOpened
		private DateTime getFirstDateTimeToTestInefficiency()
		{
			DateTime now = this.now();
			DateTime firstDateTimeToTestInefficiency = new DateTime(
				now.Year , now.Month , now.Day ,
				this.firstTimeToTestInefficiency.Hour ,
				this.firstTimeToTestInefficiency.Minute ,
				this.firstTimeToTestInefficiency.Second );
			return firstDateTimeToTestInefficiency;
		}
		private DateTime getLastDateTimeToTestInefficiency()
		{
			DateTime now = this.now();
			DateTime lastDateTimeToTestInefficiency = new DateTime(
				now.Year , now.Month , now.Day ,
				this.lastTimeToTestInefficiency.Hour ,
				this.lastTimeToTestInefficiency.Minute ,
				this.lastTimeToTestInefficiency.Second );
			return lastDateTimeToTestInefficiency;
		}
		protected override WeightedPositions getPositionsToBeOpened()
		{
			DateTime firstDateTimeToTestInefficiency =
				this.getFirstDateTimeToTestInefficiency();
			DateTime lastDateTimeToTestInefficiency =
				this.getLastDateTimeToTestInefficiency();
			WeightedPositions weightedPositions =
				this.outOfSampleChooser.GetPositionsToBeOpened(
					this.bestTestingPositionsInSample ,
					firstDateTimeToTestInefficiency ,
					lastDateTimeToTestInefficiency ,
					this.historicalMarketValueProviderForChosingPositionsOutOfSample ,
					this.inSampleReturnsManager );
			return weightedPositions;
		}
		#endregion getPositionsToBeOpened
	}
}
