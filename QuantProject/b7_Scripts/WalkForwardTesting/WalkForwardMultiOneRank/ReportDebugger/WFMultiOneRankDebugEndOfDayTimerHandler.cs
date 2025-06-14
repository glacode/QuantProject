/*
QuantProject - Quantitative Finance Library

WFMultiOneRankDebugEndOfDayTimerHandler.cs
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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardMultiOneRank
{
	/// <summary>
	/// Strategy to debug the in sample optimization
	/// </summary>
	public class WFMultiOneRankDebugEndOfDayTimerHandler :
		QuantProject.Business.Strategies.EndOfDayTimerHandler
	{
		private Account account;
		string[] signedTickers;

		public WFMultiOneRankDebugEndOfDayTimerHandler(
			Account account , string[] signedTickers )
		{
			this.account = account;
			this.signedTickers = signedTickers;
		}
		
		#region FiveMinutesBeforeMarketCloseEventHandler
		private string getTicker( string signedTicker )
		{
			string returnValue = signedTicker;
			if ( signedTicker.StartsWith( "-" ) )
				returnValue = signedTicker.Substring( 1 );
			return returnValue;
		}
		private string getReverseSignedTicker( string signedTicker )
		{
			string returnValue;
			if ( signedTicker.StartsWith( "-" ) )
				returnValue = signedTicker.Substring( 1 );
			else
				returnValue = "-" + signedTicker;
			return returnValue;
		}
		private OrderType getOrderType( string signedTicker )
		{
			OrderType returnValue = OrderType.MarketBuy;
			if ( signedTicker.StartsWith( "-" ) )
				returnValue = OrderType.MarketSellShort;
			return returnValue;
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_closePosition(
			string ticker )
		{
			this.account.ClosePosition( ticker );
		}
		private void marketCloseEventHandler_closePositions()
		{
			ArrayList tickers = new ArrayList();
			foreach ( string ticker in this.account.Portfolio.Keys )
				tickers.Add( ticker );
			foreach ( string ticker in tickers )
				fiveMinutesBeforeMarketCloseEventHandler_closePosition( ticker );
		}
		private double totalGainForSignedTicker( string signedTicker )
		{
			string ticker = this.getTicker( signedTicker );
			HistoricalAdjustedQuoteProvider historicalAdjustedQuoteProvider =
				new HistoricalAdjustedQuoteProvider();
			double todayMarketValueAtClose =
				this.account.DataStreamer.GetCurrentBid( ticker );
			DateTime yesterdayAtClose =
				HistoricalEndOfDayTimer.GetMarketClose(
					this.account.Timer.GetCurrentDateTime().AddDays( - 1 ) );
//			new	EndOfDayDateTime(
//				this.account.EndOfDayTimer.GetCurrentTime().DateTime.AddDays( - 1 ) ,
//				EndOfDaySpecificTime.MarketClose );
			double yesterdayMarketValueAtClose =
				historicalAdjustedQuoteProvider.GetMarketValue(
					ticker , yesterdayAtClose );
			double returnForLongPosition =
				( todayMarketValueAtClose / yesterdayMarketValueAtClose ) - 1;
			double returnValue;
			if ( signedTicker.StartsWith( "-" ) )
				// signedTicker represents a short position
				returnValue = - returnForLongPosition;
			else
				// signedTicker represents a long position
				returnValue = returnForLongPosition;
			return returnValue;
		}
		private double todayTotalGainForLinearCombination()
		{
			double todayTotalGain = 0;
			foreach ( string signedTicker in this.signedTickers )
				todayTotalGain += totalGainForSignedTicker( signedTicker );
			return todayTotalGain;
		}
		private bool isLinearCombinationTodayValueHigherThanYesterday()
		{
			double todayTotalGain = todayTotalGainForLinearCombination();
			return todayTotalGain > 0;
		}
		private bool isCurrentlyReversed( string signedTicker )
		{
			PositionType positionType = this.account.Portfolio.GetPosition(
				this.getTicker( signedTicker ) ).Type;
			OrderType orderType = this.getOrderType( signedTicker );
			bool returnValue = ( ( ( orderType ==
			                        OrderType.MarketBuy ) && ( positionType == PositionType.Short ) ) ||
			                    ( orderType ==	OrderType.MarketSellShort ) &&
			                    ( positionType == PositionType.Long ) );
			return returnValue;
		}
		private bool isCurrentlyReversed()
		{
			return this.isCurrentlyReversed( this.signedTickers[ 0 ] );
		}
		private long getQuantity(	string ticker )
		{
			double accountValue = this.account.GetMarketValue();
			double currentTickerAsk =
				this.account.DataStreamer.GetCurrentAsk( ticker );
			double maxPositionValueForThisTicker =
				accountValue/this.signedTickers.Length;
			long quantity = Convert.ToInt64(	Math.Floor(
				maxPositionValueForThisTicker /	currentTickerAsk ) );
			return quantity;
		}
		private void openPosition( string ticker , OrderType orderType )
		{
			long quantity = this.getQuantity( ticker );
			Order order = new Order( orderType , new Instrument( ticker ) ,
			                        quantity );
			this.account.AddOrder( order );
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_openThisPosition(
			string signedTicker )
		{
			this.openPosition( this.getTicker( signedTicker ) ,
			                  this.getOrderType( signedTicker ) );
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_openReversePosition(
			string signedTicker )
		{
			this.fiveMinutesBeforeMarketCloseEventHandler_openThisPosition(
				this.getReverseSignedTicker( signedTicker ) );
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_openPosition(
			string signedTicker )
		{
			if ( this.isLinearCombinationTodayValueHigherThanYesterday() )
				fiveMinutesBeforeMarketCloseEventHandler_openThisPosition(
					signedTicker );
			else
				fiveMinutesBeforeMarketCloseEventHandler_openReversePosition(
					signedTicker );
		}
		private void marketCloseEventHandler_openPositions()
		{
//			this.chosenTickers.SetTickers( this.bestPerformingTickers , this.account );
			foreach ( string signedTicker in this.signedTickers )
				this.fiveMinutesBeforeMarketCloseEventHandler_openPosition(
					signedTicker );
		}
		private bool marketCloseEventHandler_arePositionsToBeClosed()
		{
			bool returnValue = ( this.account.Portfolio.Count > 0 ) &&
				(	( this.isLinearCombinationTodayValueHigherThanYesterday() &&
				   this.isCurrentlyReversed() ) ||
				 ( !this.isLinearCombinationTodayValueHigherThanYesterday() &&
				  !this.isCurrentlyReversed() ) );
			return returnValue;
		}
		private bool marketCloseEventHandler_arePositionsToBeOpened()
		{
			bool returnValue = ( this.account.Portfolio.Count == 0 );
			return returnValue;
		}
		protected override void marketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( this.marketCloseEventHandler_arePositionsToBeClosed() )
				this.marketCloseEventHandler_closePositions();
			if ( this.marketCloseEventHandler_arePositionsToBeOpened() )
				marketCloseEventHandler_openPositions();
		}
		#endregion

		protected override void marketOpenEventHandler(
			Object sender , DateTime dateTime )
		{
			;
		}

		protected override void oneHourAfterMarketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			;
		}

	}
}
