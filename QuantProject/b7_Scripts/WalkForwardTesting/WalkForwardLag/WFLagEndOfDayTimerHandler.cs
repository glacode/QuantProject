/*
QuantProject - Quantitative Finance Library

WFLagEndOfDayTimerHandler.cs
Copyright (C) 2003
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
using System.Collections;

using QuantProject.ADT;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Timing;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WeightedPositionsChoosers;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger;


namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	public delegate void NewChosenPositionsEventHandler(
		Object sender , WFLagNewChosenPositionsEventArgs eventArgs );

	/// <summary>
	/// Implements OneHourAfterMarketCloseEventHandler
	/// and FiveMinutesBeforeMarketCloseEventHandler for the
	/// Lag strategy. This is the core strategy!
	/// </summary>
	public class WFLagEndOfDayTimerHandler : EndOfDayTimerHandler
	{
		private string tickerGroupID;
		private string benchmark;
		private int numberEligibleTickers;
		QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WeightedPositionsChoosers.
			IWFLagWeightedPositionsChooser wFLagWeightedPositionsChooser;
		private int outOfSampleWindowDays;
		private Account account;
//		private IEquityEvaluator equityEvaluator;

		private WFLagEligibleTickers eligibleTickers;
//		private WFLagChosenTickers chosenTickers;

		private HistoricalAdjustedQuoteProvider historicalAdjustedQuoteProvider;

		private DateTime lastOptimizationDate;
		private bool arePositionsUpToDateWithChosenTickers;

		public event InSampleNewProgressEventHandler InSampleNewProgress;
		public event NewChosenPositionsEventHandler NewChosenPositions;

		public WFLagEndOfDayTimerHandler(
			string tickerGroupID ,
			string benchmark ,
			int numberEligibleTickers ,
			IWFLagWeightedPositionsChooser wFLagWeightedPositionsChooser ,
			int outOfSampleWindowDays ,
			Account account )
		{
			this.tickerGroupID = tickerGroupID;
			this.benchmark = benchmark;
			this.numberEligibleTickers = numberEligibleTickers;
			this.wFLagWeightedPositionsChooser = wFLagWeightedPositionsChooser;
			this.outOfSampleWindowDays = outOfSampleWindowDays;
			this.account = account;

			this.eligibleTickers = new WFLagEligibleTickers(
				this.tickerGroupID ,
				this.benchmark ,
				this.numberEligibleTickers ,
				this.wFLagWeightedPositionsChooser.NumberDaysForInSampleOptimization ,
				this.account.Timer );

//			this.chosenTickers = new WFLagChosenTickers(
//				this.wFLagWeightedPositionsChooser );
			this.wFLagWeightedPositionsChooser.NewProgress +=
				new NewProgressEventHandler( this.bestPerformingNewProgress );

			this.historicalAdjustedQuoteProvider =
				new HistoricalAdjustedQuoteProvider();
			this.lastOptimizationDate = DateTime.MinValue;
		}
		private DateTime now()
		{
			return this.account.Timer.GetCurrentDateTime();
		}
		private void bestPerformingNewProgress(
			Object sender , NewProgressEventArgs eventArgs )
		{
			this.InSampleNewProgress( sender , eventArgs );
		}
		
		#region marketCloseEventHandler
		private double getTodayReturnForTicker( string ticker )
		{
			double todayMarketValueAtClose =
				this.account.DataStreamer.GetCurrentBid( ticker );
			DateTime yesterdayAtClose =
				HistoricalEndOfDayTimer.GetMarketClose(
					this.account.Timer.GetCurrentDateTime().AddDays( - 1 ) );
//			new	EndOfDayDateTime(
//				this.account.EndOfDayTimer.GetCurrentTime().DateTime.AddDays( - 1 ) ,
//				EndOfDaySpecificTime.MarketClose );
			double yesterdayMarketValueAtClose =
				this.historicalAdjustedQuoteProvider.GetMarketValue(
					ticker , yesterdayAtClose );
			double returnValue =
				( todayMarketValueAtClose - yesterdayMarketValueAtClose ) /
				yesterdayMarketValueAtClose ;
			return returnValue;
		}
		private double getTodayReturn(
			WeightedPosition weightedPosition )
		{
			double todayReturnForTicker = this.getTodayReturnForTicker(
				weightedPosition.Ticker );
			double returnMultiplier = weightedPosition.Weight;
			return todayReturnForTicker * returnMultiplier;
		}
		/// <summary>
		/// true iff driving positions are down today
		/// </summary>
		/// <returns></returns>
		private bool isToReverse()
		{
			double totalReturn = 0;
			foreach ( WeightedPosition weightedPosition in
			         this.wFLagWeightedPositionsChooser.WFLagChosenPositions.DrivingWeightedPositions.Values )
				totalReturn += this.getTodayReturn( weightedPosition );
			return totalReturn < 0;
		}
		private OrderType
			fiveMinutesBeforeMarketCloseEventHandler_openPosition_getOrderType(
				WeightedPosition weightedPosition , bool isToReverse )
		{
			OrderType orderType = OrderType.MarketBuy;
			if ( ( weightedPosition.IsShort && !isToReverse ) ||
			    ( weightedPosition.IsLong && isToReverse ) )
				orderType = OrderType.MarketSellShort;
			return orderType;
		}
		private long getMaxBuyableShares( WeightedPosition weightedPosition )
		{
			double maxPositionValue =	this.account.GetMarketValue() *
				Math.Abs( weightedPosition.Weight );
			double currentAsk =
				this.account.DataStreamer.GetCurrentAsk( weightedPosition.Ticker );
			return Convert.ToInt64(	Math.Floor(	maxPositionValue / currentAsk ) );
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_openPosition(
			WeightedPosition weightedPosition , bool isToReverse )
		{
			string ticker = weightedPosition.Ticker;
			OrderType orderType =
				this.fiveMinutesBeforeMarketCloseEventHandler_openPosition_getOrderType(
					weightedPosition , isToReverse );
			double maxPositionValue = this.account.GetMarketValue() /
				this.wFLagWeightedPositionsChooser.NumberOfPortfolioPositions;
			long sharesToBeTraded = this.getMaxBuyableShares( weightedPosition );
			this.account.AddOrder( new Order( orderType ,
			                                 new Instrument( ticker ) , sharesToBeTraded ) );
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_openPositions_actually()
		{
			bool isToReverse = this.isToReverse();
			foreach ( WeightedPosition weightedPosition
			         in this.wFLagWeightedPositionsChooser.WFLagChosenPositions.PortfolioWeightedPositions.Values )
				this.fiveMinutesBeforeMarketCloseEventHandler_openPosition(
					weightedPosition , isToReverse );
			this.arePositionsUpToDateWithChosenTickers = true;
		}
		private void marketCloseEventHandler_openPositions()
		{
			if ( this.wFLagWeightedPositionsChooser.WFLagChosenPositions != null )
				this.fiveMinutesBeforeMarketCloseEventHandler_openPositions_actually();
		}
		private double getTodayReturnForDrivingPositions()
		{
			double totalReturn = 0;
			foreach ( WeightedPosition weightedPosition in
			         this.wFLagWeightedPositionsChooser.WFLagChosenPositions.DrivingWeightedPositions.Values )
				totalReturn += this.getTodayReturn( weightedPosition );
			return totalReturn;
		}
		private Position getFirstPosition()
		{
			IEnumerator positions = this.account.Portfolio.Values.GetEnumerator();
			positions.Reset();
			positions.MoveNext();
			Position position = (Position)positions.Current;
			return position;
		}

		private WeightedPosition getWeightedPosition( string ticker )
		{
			return this.wFLagWeightedPositionsChooser.WFLagChosenPositions.PortfolioWeightedPositions.GetWeightedPosition(
				ticker );
		}
		private bool isReversed()
		{
			Position position = this.getFirstPosition();
			PositionType positionType = position.Type;
			string firstPositionTicker = position.Instrument.Key;
			WeightedPosition weightedPosition =
				this.getWeightedPosition( firstPositionTicker );
			bool isReversedPosition =
				( ( weightedPosition.IsLong &&
				   ( position.Type == PositionType.Short ) ) ||
				 ( weightedPosition.IsShort &&
				  ( position.Type == PositionType.Long ) ) );
			return isReversedPosition;
		}
		private bool isReversingNeeded()
		{
			double todayReturnForDrivingPositions =
				this.getTodayReturnForDrivingPositions();
			bool portfolioIsReversed = this.isReversed();
			bool areToBeClosed =
				( ( todayReturnForDrivingPositions < 0 ) && ( !portfolioIsReversed ) )
				||
				( ( todayReturnForDrivingPositions > 0 ) && ( portfolioIsReversed ) );
			return areToBeClosed;
		}
		private bool arePositionsToBeClosed()
		{
			bool areToBeClosed =
				( !this.arePositionsUpToDateWithChosenTickers ) ||
				this.isReversingNeeded();
			return areToBeClosed;
		}
		private void marketCloseEventHandler_closePositions()
		{
			ArrayList tickers = new ArrayList();
			foreach ( string ticker in this.account.Portfolio.Keys )
				tickers.Add( ticker );
			foreach ( string ticker in tickers )
				this.account.ClosePosition( ticker );
		}
		protected override void marketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( this.account.Portfolio.Count == 0 )
				marketCloseEventHandler_openPositions();
			else
			{
				if ( this.arePositionsToBeClosed() )
				{
					this.marketCloseEventHandler_closePositions();
					this.marketCloseEventHandler_openPositions();
				}
			}
		}
		#endregion
		
		protected override void marketOpenEventHandler(
			Object sender , DateTime dateTime )
		{
			;
		}
		
		public bool AreBestTickersToBeChosen()
		{
			bool returnValue =
				( ( ( this.account.Portfolio.Count == 0 )
				   && ( ( this.lastOptimizationDate == DateTime.MinValue ) ) ) ||
				 ( this.now() >=
				  this.lastOptimizationDate.AddDays( this.outOfSampleWindowDays ) ) );
			return returnValue;
		}
		protected override void oneHourAfterMarketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( this.AreBestTickersToBeChosen() )
				// the portfolio is empty and
				// either the lastOptimizationDate has not been set yet
				// or outOfSampleWindowDays elapsed since last optimization
			{
				this.eligibleTickers.SetTickers();
				string outputMessage = "Number of Eligible tickers: " +
					this.eligibleTickers.EligibleTickers.Rows.Count;
				RunWalkForwardLag.WriteToTextLog( outputMessage );
				this.wFLagWeightedPositionsChooser.ChosePositions(
					this.eligibleTickers ,
					this.eligibleTickers ,
					this.now() );
				this.arePositionsUpToDateWithChosenTickers = false;
				WFLagLogItem wFLagLogItem =
					new WFLagLogItem(
						this.wFLagWeightedPositionsChooser.WFLagChosenPositions ,
						this.wFLagWeightedPositionsChooser.GenerationWhenChosenPositionsWereFound ,
						this.now() );
				this.NewChosenPositions(
					this , new WFLagNewChosenPositionsEventArgs(
						wFLagLogItem ) );
				this.lastOptimizationDate = this.now();
			}
			//			oneHourAfterMarketCloseEventHandler_orderChosenTickers( ( IEndOfDayTimer ) sender );
		}
	}
}
