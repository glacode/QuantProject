/*
QuantProject - Quantitative Finance Library

WFMultiOneRankEndOfDayTimerHandler.cs
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
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;
using QuantProject.Scripts.SimpleTesting;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardMultiOneRank
{
	public delegate void InSampleNewProgressEventHandler(
	Object sender , NewProgressEventArgs eventArgs );

	/// <summary>
	/// Implements OneHourAfterMarketCloseEventHandler and FiveMinutesBeforeMarketCloseEventHandler.
	/// This is the core strategy!
	/// </summary>
	public class WFMultiOneRankEndOfDayTimerHandler
	{
		private WFMultiOneRankEligibleTickers eligibleTickers;
		private WFMultiOneRankChosenTickers chosenTickers;

		private string tickerGroupID;
		private int numberEligibleTickers;
		private int numberOfPositionsToBeChosen;
		private int inSampleWindowDays;
		private int outOfSampleWindowDays;
		private Account account;
		private int generationNumberForGeneticOptimizer;
		private int populationSizeForGeneticOptimizer;


		private DateTime lastOptimizationDate;


		private HistoricalAdjustedQuoteProvider historicalAdjustedQuoteProvider;

		public event InSampleNewProgressEventHandler InSampleNewProgress;

		public int NumberEligibleTickers
		{
			get { return this.numberEligibleTickers; }
		}
		public int NumberOfTickersToBeChosen
		{
			get { return this.numberOfPositionsToBeChosen; }
		}
		public Account Account
		{
			get { return this.account; }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="numberEligibleTickers">number of tickers to be chosen with the first selection:
		/// the best performers will be chosen among these first selected instruments</param>
		/// <param name="numberBestPeformingTickers">number of instruments to be chosen, as the best
		/// performers, among the eligible tickers</param>
		/// <param name="numberOfTickersToBeChosen">number of instruments to be chosen,
		/// among the best performers</param>
		/// <param name="windowDays">number of days between two consecutive
		/// best performing ticker calculation</param>
		public WFMultiOneRankEndOfDayTimerHandler(
			string tickerGroupID ,
			int numberEligibleTickers ,
			int numberOfPositionsToBeChosen ,
			int inSampleWindowDays , int outOfSampleWindowDays ,
			Account account ,
			int generationNumberForGeneticOptimizer ,
			int populationSizeForGeneticOptimizer )
		{
			this.tickerGroupID = tickerGroupID;
			this.numberEligibleTickers = numberEligibleTickers;
			this.numberOfPositionsToBeChosen = numberOfPositionsToBeChosen;
			this.inSampleWindowDays = inSampleWindowDays;
			this.outOfSampleWindowDays = outOfSampleWindowDays;
			this.account = account;
			this.generationNumberForGeneticOptimizer =
				generationNumberForGeneticOptimizer;
			this.populationSizeForGeneticOptimizer =
				populationSizeForGeneticOptimizer;

			this.eligibleTickers =
				new WFMultiOneRankEligibleTickers( this.tickerGroupID ,
				numberEligibleTickers ,	inSampleWindowDays ,
				this.account.EndOfDayTimer );
			this.chosenTickers = new WFMultiOneRankChosenTickers(
				this.eligibleTickers , this.numberOfPositionsToBeChosen ,
				this.inSampleWindowDays ,	this.account.EndOfDayTimer ,
				this.generationNumberForGeneticOptimizer ,
				this. populationSizeForGeneticOptimizer );
			this.chosenTickers.NewProgress +=
				new NewProgressEventHandler( this.bestPerformingNewProgress );

			this.historicalAdjustedQuoteProvider =
				new HistoricalAdjustedQuoteProvider();
			this.lastOptimizationDate = DateTime.MinValue;
		}
		private EndOfDayDateTime now()
		{
			return this.account.EndOfDayTimer.GetCurrentTime();
		}
		private void bestPerformingNewProgress(
			Object sender , NewProgressEventArgs eventArgs )
		{
			this.InSampleNewProgress( sender , eventArgs );
		}
		private bool areBestTickersToBeChosen()
		{
			bool returnValue =
				( ( ( this.account.Portfolio.Count == 0 )
				&& ( ( this.lastOptimizationDate == DateTime.MinValue ) ) ) ||
				( this.now().DateTime >=
				this.lastOptimizationDate.AddDays( this.outOfSampleWindowDays ) ) );
			return returnValue;
		}
		/// <summary>
		/// Handles a "One hour after market close" event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		public void OneHourAfterMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( this.areBestTickersToBeChosen() )
				// the portfolio is empty and
				// either the lastOptimizationDate has not been set yet
				// or outOfSampleWindowDays elapsed since last optimization
			{
				//				this.eligibleTickers.SetTickers( endOfDayTimingEventArgs.EndOfDayDateTime.DateTime );
				this.eligibleTickers.SetTickers();
				this.chosenTickers.SetTickers( this.eligibleTickers );
				this.lastOptimizationDate = this.now().DateTime;
			}
			//			oneHourAfterMarketCloseEventHandler_orderChosenTickers( ( IEndOfDayTimer ) sender );
		}
		//		#endregion
		#region FiveMinutesBeforeMarketCloseEventHandler
		private string getTicker( string signedTicker )
		{
			string returnValue;
			if ( signedTicker.IndexOf( "-" ) == 0 )
				returnValue = signedTicker.Substring( 1 , signedTicker.Length - 1 );
			else
				returnValue = signedTicker;
			return returnValue;
		}
		private int getReturnMultiplier( string signedTicker )
		{
			int returnValue;
			if ( signedTicker.IndexOf( "-" ) == 0 )
				returnValue = -1;
			else
				returnValue = 1;
			return returnValue;
		}
		private double getTodayReturnForTicker( string ticker )
		{
			double todayMarketValueAtClose =
				this.account.DataStreamer.GetCurrentBid( ticker );
			EndOfDayDateTime yesterdayAtClose = new
				EndOfDayDateTime(
				this.account.EndOfDayTimer.GetCurrentTime().DateTime.AddDays( - 1 ) ,
				EndOfDaySpecificTime.MarketClose );
			double yesterdayMarketValueAtClose =
				this.historicalAdjustedQuoteProvider.GetMarketValue(
				ticker , yesterdayAtClose );
			double returnValue =
				( todayMarketValueAtClose - yesterdayMarketValueAtClose ) /
				yesterdayMarketValueAtClose ;
			return returnValue;
		}
		private double getTodayReturnForSignedTicker( string signedTicker )
		{
			double todayReturnForTicker = this.getTodayReturnForTicker(
				this.getTicker( signedTicker ) );
			int returnMultiplier = this.getReturnMultiplier( signedTicker );
			return todayReturnForTicker * returnMultiplier;
		}
		/// <summary>
		/// true iff chosen tickers linear combination daily return is down today
		/// </summary>
		/// <returns></returns>
		private bool isToReverse()
		{
			double totalReturn = 0;
			foreach ( string signedTicker in this.chosenTickers.Keys )
				totalReturn += this.getTodayReturnForSignedTicker( signedTicker );
			return totalReturn < 0;
		}
		private OrderType fiveMinutesBeforeMarketCloseEventHandler_openPosition_getOrderType(
			string signedTicker , bool isToReverse )
		{
			OrderType orderType = OrderType.MarketBuy;
			if ( ( signedTicker.StartsWith( "-" ) && !isToReverse ) ||
				( !signedTicker.StartsWith( "-" ) && isToReverse ) )
				orderType = OrderType.MarketSellShort;
			return orderType;
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_openPosition(
			string signedTicker , bool isToReverse )
		{
			string ticker = this.getTicker( signedTicker );
			OrderType orderType =
				this.fiveMinutesBeforeMarketCloseEventHandler_openPosition_getOrderType(
				signedTicker , isToReverse );
			double maxPositionValue = this.account.GetMarketValue() /
				this.numberOfPositionsToBeChosen;
			long sharesToBeTraded = OneRank.MaxBuyableShares( ticker ,
				maxPositionValue , this.account.DataStreamer );
			this.account.AddOrder( new Order( orderType ,
				new Instrument( ticker ) , sharesToBeTraded ) );
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_openPositions()
		{
			bool isToReverse = this.isToReverse();
			foreach ( string signedTicker in this.chosenTickers.Keys )
				this.fiveMinutesBeforeMarketCloseEventHandler_openPosition( 
					signedTicker , isToReverse );
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_closePosition(
			string ticker )
		{
			this.account.ClosePosition( ticker );
		}
		private double getTodayReturnForPosition( Position position )
		{
			string signedTicker = position.Instrument.Key;
			double returnValue;
			if ( position.Type == PositionType.Short )
				signedTicker = "-" + signedTicker;
			returnValue =
				this.getTodayReturnForSignedTicker( signedTicker );
			return returnValue;
		}
		private double getTodayReturnForPortfolioPositions()
		{
			double totalReturn = 0;
			foreach ( Position position in this.account.Portfolio.Positions )
				totalReturn += this.getTodayReturnForPosition( position );
			return totalReturn;
		}
		private bool arePositionsToBeClosed()
		{
			double todayReturnForPortfolioPositions =
				this.getTodayReturnForPortfolioPositions();
			return todayReturnForPortfolioPositions < 0;
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_closePositions_actually()
		{
			ArrayList tickers = new ArrayList();
			foreach ( string ticker in this.account.Portfolio.Keys )
				tickers.Add( ticker );
			foreach ( string ticker in tickers )
				fiveMinutesBeforeMarketCloseEventHandler_closePosition( ticker );
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_closePositions()
		{
			if ( this.arePositionsToBeClosed() )
				this.fiveMinutesBeforeMarketCloseEventHandler_closePositions_actually();
		}
		public void FiveMinutesBeforeMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( this.account.Portfolio.Count == 0 )
				fiveMinutesBeforeMarketCloseEventHandler_openPositions();
			else
			{
				if ( this.arePositionsToBeClosed() )
				{
					this.fiveMinutesBeforeMarketCloseEventHandler_closePositions();
					this.fiveMinutesBeforeMarketCloseEventHandler_openPositions();
				}
			}
		}
		#endregion
	}
}
