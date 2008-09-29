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
	public class WFLagDebugPositionsEndOfDayTimerHandler : EndOfDayTimerHandler
	{
		private Account account;
		private WFLagWeightedPositions wFLagWeightedPositions;

		public WFLagDebugPositionsEndOfDayTimerHandler(
			Account account ,
			WFLagWeightedPositions wFLagWeightedPositions )
		{
			this.account = account;
			this.wFLagWeightedPositions = wFLagWeightedPositions;
		}
		
		#region marketCloseEventHandler
		private double todayTotalGainForLinearCombination()
		{
			double todayTotalGain = 0;
			DateTime today = this.account.Timer.GetCurrentDateTime();
			foreach ( WeightedPosition weightedPosition in
			         this.wFLagWeightedPositions.DrivingWeightedPositions.Values )
				todayTotalGain +=
					weightedPosition.GetCloseToCloseDailyReturn( today );
//			todayTotalGain += totalGainForSignedTicker( signedTicker );
			return todayTotalGain;
		}
		private bool isDrivingPositionsTodayValueHigherThanYesterday()
		{
			double todayTotalGain = this.todayTotalGainForLinearCombination();
			return todayTotalGain > 0;
		}
		private Position getFirstAccountPosition()
		{
			IEnumerator accountPositions =
				this.account.Portfolio.Values.GetEnumerator();
			accountPositions.MoveNext();
			Position firstPosition = (Position)accountPositions.Current;
			return firstPosition;
		}
		private bool isCurrentlyReversed()
		{
			Position firstAccountPosition = this.getFirstAccountPosition();
			WeightedPosition weightedPosition =
				this.wFLagWeightedPositions.PortfolioWeightedPositions.GetWeightedPosition(
					firstAccountPosition.Instrument.Key );
			bool isReversed =
				( ( weightedPosition.IsLong && firstAccountPosition.IsShort ) ||
				 ( weightedPosition.IsShort && firstAccountPosition.IsLong ) );
//			bool isReversed =
//				this.wFLagChosenPositions.PortfolioWeightedPositions.ContainsKey(
//				SignedTicker.GetOppositeSignedTicker( firstAccountPosition ) );
			return isReversed;
		}
		private bool marketCloseEventHandler_arePositionsToBeClosed()
		{
			bool returnValue = ( this.account.Portfolio.Count > 0 ) &&
				(	( this.isDrivingPositionsTodayValueHigherThanYesterday() &&
				   this.isCurrentlyReversed() ) ||
				 ( !this.isDrivingPositionsTodayValueHigherThanYesterday() &&
				  !this.isCurrentlyReversed() ) );
			return returnValue;
		}
		private void marketCloseEventHandler_closePositions()
		{
			ArrayList tickers = new ArrayList();
			foreach ( string ticker in this.account.Portfolio.Keys )
				tickers.Add( ticker );
			foreach ( string ticker in tickers )
				this.account.ClosePosition( ticker );
		}
		private bool marketCloseEventHandler_arePositionsToBeOpened()
		{
			bool returnValue = ( this.account.Portfolio.Count == 0 );
			return returnValue;
		}
		private long getQuantity(	WeightedPosition weightedPosition )
		{
			double accountValue = this.account.GetMarketValue();
			double currentTickerAsk =
				this.account.DataStreamer.GetCurrentAsk( weightedPosition.Ticker );
			double maxPositionValueForThisTicker =
				accountValue * Math.Abs( weightedPosition.Weight );
			long quantity = Convert.ToInt64(	Math.Floor(
				maxPositionValueForThisTicker /	currentTickerAsk ) );
			return quantity;
		}
		private void openPosition( WeightedPosition weightedPosition )
		{
			long quantity = this.getQuantity( weightedPosition );
			OrderType orderType = weightedPosition.GetOrderType();
			Order order = new Order(
				orderType , new Instrument( weightedPosition.Ticker ) ,	quantity );
			this.account.AddOrder( order );
		}
		private void marketCloseEventHandler_openThisPosition(
			WeightedPosition weightedPosition )
		{
			this.openPosition( weightedPosition );
		}
		private void marketCloseEventHandler_openOppositePosition(
			WeightedPosition weightedPosition )
		{
			this.marketCloseEventHandler_openThisPosition(
				weightedPosition.GetOppositeWeightedPosition() );
		}
		private void marketCloseEventHandler_openPosition(
			WeightedPosition weightedPosition )
		{
			if ( this.isDrivingPositionsTodayValueHigherThanYesterday() )
				this.marketCloseEventHandler_openThisPosition(
					weightedPosition );
			else
				this.marketCloseEventHandler_openOppositePosition(
					weightedPosition );
		}
		private void marketCloseEventHandler_openPositions()
		{
			//			this.chosenTickers.SetTickers( this.bestPerformingTickers , this.account );
			foreach ( WeightedPosition weightedPosition in
			         this.wFLagWeightedPositions.PortfolioWeightedPositions.Values )
				this.marketCloseEventHandler_openPosition(
					weightedPosition );
		}
		protected override void marketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( this.marketCloseEventHandler_arePositionsToBeClosed() )
				this.marketCloseEventHandler_closePositions();
			if ( this.marketCloseEventHandler_arePositionsToBeOpened() )
				this.marketCloseEventHandler_openPositions();
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
