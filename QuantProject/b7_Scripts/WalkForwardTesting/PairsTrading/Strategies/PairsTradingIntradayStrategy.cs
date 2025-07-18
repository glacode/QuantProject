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
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Pairs trading strategy with intraday bars
	/// </summary>
	[Serializable]
	public class PairsTradingIntradayStrategy : BasicStrategyForBacktester
	{
		private Time firstTimeToTestInefficiency;
		private Time lastTimeToTestInefficiency;
		private Time timeToClosePositions;
		private HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample;
		private OutOfSampleChooser outOfSampleChooser;
		private IExitStrategy exitStrategy;
		private Time timeToBeginToTryToOpenPositions;
		private Time timeToStopToTryToOpenPositions;
		
		public PairsTradingIntradayStrategy(
			int numDaysBeetweenEachOtpimization ,
			int numDaysForInSampleOptimization ,
			IIntervalsSelector intervalsSelectorForInSample ,
			Time firstTimeToTestInefficiency ,
			Time lastTimeToTestInefficiency ,
			Time timeToClosePositions ,
			IEligiblesSelector eligiblesSelector ,
			IInSampleChooser inSampleChooser ,
			HistoricalMarketValueProvider historicalMarketValueProviderForInSample ,
			HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample ,
			OutOfSampleChooser outOfSampleChooser ,
			IExitStrategy exitStrategy ,
			Time timeToBeginToTryToOpenPositions ,
			Time timeToStopToTryToOpenPositions
		) :
			base(
				numDaysBeetweenEachOtpimization ,
				numDaysForInSampleOptimization ,
				intervalsSelectorForInSample ,
				intervalsSelectorForInSample ,  // this parameter should be intervalsSelectorForOutOfSample, but it is not going to be used by this strategy
				eligiblesSelector ,
				inSampleChooser ,
				historicalMarketValueProviderForInSample )
		{
			this.checkParameters(
				timeToBeginToTryToOpenPositions ,
				timeToStopToTryToOpenPositions );
			this.firstTimeToTestInefficiency = firstTimeToTestInefficiency;
			this.lastTimeToTestInefficiency = lastTimeToTestInefficiency;
			this.timeToClosePositions = timeToClosePositions;
			this.historicalMarketValueProviderForChosingPositionsOutOfSample =
				historicalMarketValueProviderForChosingPositionsOutOfSample;
			this.outOfSampleChooser = outOfSampleChooser;
			this.exitStrategy = exitStrategy;
			this.timeToBeginToTryToOpenPositions = timeToBeginToTryToOpenPositions;
			this.timeToStopToTryToOpenPositions = timeToStopToTryToOpenPositions;
		}
		
		private void checkParameters(
			Time timeToBeginToTryToOpenPositions ,
			Time timeToStopToTryToOpenPositions )
		{
			if ( timeToBeginToTryToOpenPositions >= timeToStopToTryToOpenPositions )
				throw new Exception(
					"timeToBeginToTryToOpenPositions come before timeToStopToTryToOpenPositions" );
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
		
		
		#region arePositionsToBeOpened
		private bool isAValidTimeToTryToOpenPositins()
		{
			Time currentTime = this.time();
			bool isAValidTimeToTryToOpen =
				(
					( currentTime >= this.timeToBeginToTryToOpenPositions ) &&
					( currentTime <= this.timeToStopToTryToOpenPositions )
				);
			return isAValidTimeToTryToOpen;
		}
		protected override bool arePositionsToBeOpened()
		{
			bool areToBeOpened = ( this.Account.Portfolio.Count == 0 );
			areToBeOpened = ( areToBeOpened && ( this.bestTestingPositionsInSample != null ) );
			areToBeOpened = ( areToBeOpened && this.isAValidTimeToTryToOpenPositins() );
			return areToBeOpened;
		}
		#endregion arePositionsToBeOpened

		protected override bool arePositionsToBeClosed()
		{
			bool areToBeClosed =
				( this.Account.Portfolio.Count > 0 );
			areToBeClosed = (
				areToBeClosed &&
				this.exitStrategy.ArePositionsToBeClosed(
					this.now() , this.Account ) );
//				( this.time() == this.timeToClosePositions ) );
			return ( areToBeClosed );
		}

		#region getPositionsToBeOpened
		private DateTime getDateTimeForCurrentDate( Time time )
		{
			DateTime now = this.now();
			DateTime dateTimeForCurrentDate = new DateTime(
				now.Year , now.Month , now.Day ,
				time.Hour ,	time.Minute , time.Second );
			return dateTimeForCurrentDate;
		}
//		private DateTime getFirstDateTimeToTestInefficiency()
//		{
		////			DateTime now = this.now();
		////			DateTime firstDateTimeToTestInefficiency = new DateTime(
		////				now.Year , now.Month , now.Day ,
		////				this.firstTimeToTestInefficiency.Hour ,
		////				this.firstTimeToTestInefficiency.Minute ,
		////				this.firstTimeToTestInefficiency.Second );
//			DateTime firstDateTimeToTestInefficiency =
//				this.getDateTimeForCurrentDate( this.firstTimeToTestInefficiency );
//			return firstDateTimeToTestInefficiency;
//		}
//		private DateTime getLastDateTimeToTestInefficiency()
//		{
		////			DateTime now = this.now();
		////			DateTime lastDateTimeToTestInefficiency = new DateTime(
		////				now.Year , now.Month , now.Day ,
		////				this.lastTimeToTestInefficiency.Hour ,
		////				this.lastTimeToTestInefficiency.Minute ,
		////				this.lastTimeToTestInefficiency.Second );
//			DateTime lastDateTimeToTestInefficiency =
//				this.getDateTimeForCurrentDate( this.lastTimeToTestInefficiency );
//			return lastDateTimeToTestInefficiency;
//		}
//		private DateTime getDateTimeToClosePositions()
//		{
//			DateTime dateTimeToClosePositions =
//				this.getDateTimeForCurrentDate( this.timeToClosePositions );
//			return dateTimeToClosePositions;
//		}
		protected override WeightedPositions getPositionsToBeOpened()
		{
//			DateTime firstDateTimeToTestInefficiency =
//				this.getFirstDateTimeToTestInefficiency();
//			DateTime lastDateTimeToTestInefficiency =
//				this.getLastDateTimeToTestInefficiency();
			// attention! we are looking in the future here, but we do it
			// just to avoid picking a ticker for which we don't have
			// the market value when we will close the positions
//			DateTime dateTimeToClosePositions =
//				this.getDateTimeToClosePositions();
			DateTime currentDateTime = this.now();
			WeightedPositions weightedPositions =
				this.outOfSampleChooser.GetPositionsToBeOpened(
					this.bestTestingPositionsInSample ,
//					firstDateTimeToTestInefficiency ,
					currentDateTime ,
//					dateTimeToClosePositions ,
					this.historicalMarketValueProviderForChosingPositionsOutOfSample ,
					this.inSampleReturnsManager );
			return weightedPositions;
		}
		#endregion getPositionsToBeOpened
	}
}
