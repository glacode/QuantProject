/*
QuantProject - Quantitative Finance Library

WFLagDebugPositionsEndOfDayTimerHandler.cs
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

using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger
{
	/// <summary>
	/// Strategy to debug the log for the current positions
	/// </summary>
	public class WFLagDebugPositionsEndOfDayTimerHandler
	{
		private Account account;
		private WFLagChosenPositions wFLagChosenPositions;

		public WFLagDebugPositionsEndOfDayTimerHandler(
			Account account ,
			WFLagChosenPositions wFLagChosenPositions )
		{
			this.account = account;
			this.wFLagChosenPositions = wFLagChosenPositions;
		}
		#region FiveMinutesBeforeMarketCloseEventHandler
		private double todayTotalGainForLinearCombination()
		{
			double todayTotalGain = 0;
			DateTime today = this.account.EndOfDayTimer.GetCurrentTime().DateTime;
			foreach ( string signedTicker in
				this.wFLagChosenPositions.DrivingPositions.Keys )
				todayTotalGain +=
					SignedTicker.GetCloseToCloseDailyReturn( signedTicker ,	today );
//			todayTotalGain += totalGainForSignedTicker( signedTicker );
			return todayTotalGain;
		}
		private bool isDrivingPositionsTodayValueHigherThanYesterday()
		{
			double todayTotalGain = this.todayTotalGainForLinearCombination();
			return todayTotalGain > 0;
		}
		private string getFirstAccountPosition()
		{
			IEnumerator accountPositions =
				this.account.Portfolio.Values.GetEnumerator();
			accountPositions.MoveNext();
			Position firstPosition = (Position)accountPositions.Current;
			string firstAccountPosition = SignedTicker.GetSignedTicker( firstPosition );
			return firstAccountPosition;
		}
		private bool isCurrentlyReversed()
		{
			string firstAccountPosition = this.getFirstAccountPosition();
			bool isReversed =
				this.wFLagChosenPositions.PortfolioPositions.ContainsKey(
				SignedTicker.GetOppositeSignedTicker( firstAccountPosition ) );
			return isReversed;
		}
		private bool fiveMinutesBeforeMarketCloseEventHandler_arePositionsToBeClosed()
		{
			bool returnValue = ( this.account.Portfolio.Count > 0 ) &&
				(	( this.isDrivingPositionsTodayValueHigherThanYesterday() &&
				this.isCurrentlyReversed() ) ||
				( !this.isDrivingPositionsTodayValueHigherThanYesterday() &&
				!this.isCurrentlyReversed() ) );
			return returnValue;
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_closePositions()
		{
			ArrayList tickers = new ArrayList();
			foreach ( string ticker in this.account.Portfolio.Keys )
				tickers.Add( ticker );
			foreach ( string ticker in tickers )
				this.account.ClosePosition( ticker );
		}
		private bool fiveMinutesBeforeMarketCloseEventHandler_arePositionsToBeOpened()
		{
			bool returnValue = ( this.account.Portfolio.Count == 0 );
			return returnValue;
		}
		private long getQuantity(	string ticker )
		{
			double accountValue = this.account.GetMarketValue();
			double currentTickerAsk =
				this.account.DataStreamer.GetCurrentAsk( ticker );
			double maxPositionValueForThisTicker =
				accountValue / this.wFLagChosenPositions.PortfolioPositions.Count;
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
			this.openPosition( SignedTicker.GetTicker( signedTicker ) ,
				SignedTicker.GetMarketOrderType( signedTicker ) );
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_openOppositePosition(
			string signedTicker )
		{
			this.fiveMinutesBeforeMarketCloseEventHandler_openThisPosition(
				SignedTicker.GetOppositeSignedTicker( signedTicker ) );
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_openPosition(
			string signedTicker )
		{
			if ( this.isDrivingPositionsTodayValueHigherThanYesterday() )
				fiveMinutesBeforeMarketCloseEventHandler_openThisPosition(
					signedTicker );
			else
				fiveMinutesBeforeMarketCloseEventHandler_openOppositePosition(
					signedTicker );
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_openPositions()
		{
			//			this.chosenTickers.SetTickers( this.bestPerformingTickers , this.account );
			foreach ( string signedTicker in
				this.wFLagChosenPositions.PortfolioPositions.Keys )
				this.fiveMinutesBeforeMarketCloseEventHandler_openPosition(
					signedTicker );
		}
		public void FiveMinutesBeforeMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( this.fiveMinutesBeforeMarketCloseEventHandler_arePositionsToBeClosed() )
				this.fiveMinutesBeforeMarketCloseEventHandler_closePositions();
			if ( this.fiveMinutesBeforeMarketCloseEventHandler_arePositionsToBeOpened() )
				fiveMinutesBeforeMarketCloseEventHandler_openPositions();
		}
		#endregion
	}
}
