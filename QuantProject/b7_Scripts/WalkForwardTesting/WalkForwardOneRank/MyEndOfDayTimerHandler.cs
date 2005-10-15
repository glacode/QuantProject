/*
QuantProject - Quantitative Finance Library

TimerHandler.cs
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

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardOneRank
{
	public delegate void InSampleNewProgressEventHandler(
	Object sender , NewProgressEventArgs eventArgs );

	/// <summary>
	/// Implements OneHourAfterMarketCloseEventHandler and TwoMinutesBeforeMarketCloseEventHandler.
	/// This is the core strategy!
	/// </summary>
	public class EndOfDayTimerHandler
	{
    private EligibleTickers eligibleTickers;
		private BestPerformingTickers bestPerformingTickers;
		private ChosenTickers chosenTickers;

    private int numberEligibleTickers;
		private int numberBestPeformingTickers;
		private int numberOfTickersToBeChosen;
		private int inSampleWindowDays;
		private int outOfSampleWindowDays;

		private Account account;

		private HistoricalAdjustedQuoteProvider historicalAdjustedQuoteProvider;

		public event InSampleNewProgressEventHandler InSampleNewProgress;

		public int NumberEligibleTickers
		{
			get { return this.numberEligibleTickers; }
		}
		public int NumberBestPeformingTickers
		{
			get { return this.numberBestPeformingTickers; }
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
		public EndOfDayTimerHandler( int numberEligibleTickers , int numberBestPeformingTickers ,
			int numberOfTickersToBeChosen , int inSampleWindowDays , int outOfSampleWindowDays ,
			Account account )
		{
			this.numberEligibleTickers = numberEligibleTickers;
			this.numberBestPeformingTickers = numberBestPeformingTickers;
			this.numberOfTickersToBeChosen = numberOfTickersToBeChosen;
			this.inSampleWindowDays = inSampleWindowDays;
			this.outOfSampleWindowDays = outOfSampleWindowDays;
			this.account = account;

			this.eligibleTickers = new EligibleTickers( numberEligibleTickers ,
				inSampleWindowDays );
			this.bestPerformingTickers = new BestPerformingTickers( numberBestPeformingTickers ,
				this.inSampleWindowDays );
			this.bestPerformingTickers.NewProgress +=
				new NewProgressEventHandler( this.bestPerformingNewProgress );
			this.chosenTickers = new ChosenTickers( this.numberOfTickersToBeChosen );

			this.historicalAdjustedQuoteProvider =
				new HistoricalAdjustedQuoteProvider();
		}
		private void bestPerformingNewProgress(
			Object sender , NewProgressEventArgs eventArgs )
		{
			this.InSampleNewProgress( sender , eventArgs );
		}
		#region OneHourAfterMarketCloseEventHandler
		private void oneHourAfterMarketCloseEventHandler_orderChosenTickers_closePositions(
			IEndOfDayTimer endOfDayTimer )
		{
			foreach ( Position position in this.account.Portfolio )
				if ( this.chosenTickers.Contains( position.Instrument.Key ) )
				{
					this.account.ClosePosition( position );
				}
		}
		private void oneHourAfterMarketCloseEventHandler_orderChosenTickers_openPositions_forTicker(
			string ticker )
		{
			double cashForSinglePosition = this.account.CashAmount / this.numberOfTickersToBeChosen;
			long quantity =
				Convert.ToInt64( Math.Floor( cashForSinglePosition / this.account.DataStreamer.GetCurrentBid( ticker ) ) );
			Order order = new Order( OrderType.MarketBuy , new Instrument( ticker ) , quantity );
			this.account.AddOrder( order );
		}
		private void oneHourAfterMarketCloseEventHandler_orderChosenTickers_openPositions()
		{
			foreach ( string ticker in this.chosenTickers.Keys )
				if ( !this.account.Contains( ticker ) )
				{
					oneHourAfterMarketCloseEventHandler_orderChosenTickers_openPositions_forTicker( ticker );
				}
		}
		private void oneHourAfterMarketCloseEventHandler_orderChosenTickers(
			IEndOfDayTimer endOfDayTimer )
		{
			this.oneHourAfterMarketCloseEventHandler_orderChosenTickers_closePositions( endOfDayTimer );
			this.oneHourAfterMarketCloseEventHandler_orderChosenTickers_openPositions();
		}
		/// <summary>
		/// Handles a "One hour after market close" event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		public void OneHourAfterMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( ( this.eligibleTickers.Count == 0 ) ||
				( endOfDayTimingEventArgs.EndOfDayDateTime.DateTime.CompareTo(
				this.bestPerformingTickers.LastUpdate.AddDays( this.outOfSampleWindowDays ) ) >= 0 ) )
				// either eligible tickers have never been defined yet
				// or this.outOfSampleWindowDays days elapsed since last best performing tickers calculation
			{
				this.eligibleTickers.SetTickers( endOfDayTimingEventArgs.EndOfDayDateTime.DateTime );
				this.bestPerformingTickers.SetTickers( this.eligibleTickers ,
					endOfDayTimingEventArgs.EndOfDayDateTime.DateTime );
			}
//			oneHourAfterMarketCloseEventHandler_orderChosenTickers( ( IEndOfDayTimer ) sender );
		}
		#endregion
		#region FiveMinutesBeforeMarketCloseEventHandler
		private void fiveMinutesBeforeMarketCloseEventHandler_closePosition(
			string ticker )
		{
			this.account.ClosePosition( ticker );
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_closePositions()
		{
      ArrayList tickers = new ArrayList();
			foreach ( string ticker in this.account.Portfolio.Keys )
				tickers.Add( ticker );
			foreach ( string ticker in tickers )
				fiveMinutesBeforeMarketCloseEventHandler_closePosition( ticker );
		}
		private bool todayHigherThanYesterday( string ticker )
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
			bool returnValue =
				( todayMarketValueAtClose > yesterdayMarketValueAtClose );
			return returnValue;
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_openPosition(
			string ticker )
		{
			double maxPositionValue = this.account.GetMarketValue() / this.numberOfTickersToBeChosen;
			long sharesToBeTraded = OneRank.MaxBuyableShares( ticker ,
				maxPositionValue , this.account.DataStreamer );
			if ( this.todayHigherThanYesterday( ticker ) )
				// today close value for ticker is higher than yesterday
				// close for ticker
				this.account.AddOrder( new Order( OrderType.MarketBuy ,
					new Instrument( ticker ) , sharesToBeTraded ) );
			else
				// today close value for ticker is not higher than yesterday
				// close for ticker
				this.account.AddOrder( new Order( OrderType.MarketSellShort ,
					new Instrument( ticker ) , sharesToBeTraded ) );
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_openPositions()
		{
			this.chosenTickers.SetTickers( this.bestPerformingTickers , this.account );
			foreach ( string ticker in this.chosenTickers.Keys )
				this.fiveMinutesBeforeMarketCloseEventHandler_openPosition( ticker );
		}
		public void FiveMinutesBeforeMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			this.fiveMinutesBeforeMarketCloseEventHandler_closePositions();
			fiveMinutesBeforeMarketCloseEventHandler_openPositions();
		}
		#endregion
	}
}
