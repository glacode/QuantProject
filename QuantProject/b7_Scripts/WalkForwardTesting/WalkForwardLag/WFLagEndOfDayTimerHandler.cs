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
using QuantProject.Business.Timing;
using QuantProject.Scripts.SimpleTesting;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	public delegate void NewChosenTickersEventHandler(
	Object sender , WFLagNewChosenTickersEventArgs eventArgs );

	/// <summary>
	/// Implements OneHourAfterMarketCloseEventHandler
	/// and FiveMinutesBeforeMarketCloseEventHandler for the
	/// Lag strategy. This is the core strategy!
	/// </summary>
	public class WFLagEndOfDayTimerHandler
	{
		private string tickerGroupID;
		private string benchmark;
		private int numberEligibleTickers;
		private int numberOfPositionsToBeChosen;
		private int numberOfDrivingPositions;
		private int inSampleWindowDays;
		private int outOfSampleWindowDays;
		private Account account;
		private int generationNumberForGeneticOptimizer;
		private int populationSizeForGeneticOptimizer;

		private WFLagEligibleTickers eligibleTickers;
		private WFLagChosenTickers chosenTickers;

		private HistoricalAdjustedQuoteProvider historicalAdjustedQuoteProvider;

		private DateTime lastOptimizationDate;
		private bool arePositionsUpToDateWithChosenTickers;

		public event InSampleNewProgressEventHandler InSampleNewProgress;
		public event NewChosenTickersEventHandler NewChosenTickers;

		public WFLagEndOfDayTimerHandler(
			string tickerGroupID ,
			string benchmark ,
			int numberEligibleTickers ,
			int numberOfPositionsToBeChosen ,
			int numberOfDrivingPositions ,
			int inSampleWindowDays , int outOfSampleWindowDays ,
			Account account ,
			int generationNumberForGeneticOptimizer ,
			int populationSizeForGeneticOptimizer )
		{
			this.tickerGroupID = tickerGroupID;
			this.benchmark = benchmark;
			this.numberEligibleTickers = numberEligibleTickers;
			this.numberOfPositionsToBeChosen = numberOfPositionsToBeChosen;
			this.numberOfDrivingPositions = numberOfDrivingPositions;
			this.inSampleWindowDays = inSampleWindowDays;
			this.outOfSampleWindowDays = outOfSampleWindowDays;
			this.account = account;
			this.generationNumberForGeneticOptimizer =
				generationNumberForGeneticOptimizer;
			this.populationSizeForGeneticOptimizer =
				populationSizeForGeneticOptimizer;

			this.eligibleTickers = new WFLagEligibleTickers(
				this.tickerGroupID ,
				this.benchmark ,
				this.numberEligibleTickers ,
				this.inSampleWindowDays ,
				this.account.EndOfDayTimer );

			this.chosenTickers = new WFLagChosenTickers(
				this.numberOfDrivingPositions ,
				this.numberOfPositionsToBeChosen ,
				this.inSampleWindowDays ,
				this.account.EndOfDayTimer ,
				this.generationNumberForGeneticOptimizer ,
				this.populationSizeForGeneticOptimizer );
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
		#region fiveMinutesBeforeMarketCloseEventHandler_openPositions
//		private string getTicker( string signedTicker )
//		{
//			string returnValue;
//			if ( signedTicker.IndexOf( "-" ) == 0 )
//				returnValue = signedTicker.Substring( 1 , signedTicker.Length - 1 );
//			else
//				returnValue = signedTicker;
//			return returnValue;
//		}
//		private int getReturnMultiplier( string signedTicker )
//		{
//			int returnValue;
//			if ( signedTicker.IndexOf( "-" ) == 0 )
//				returnValue = -1;
//			else
//				returnValue = 1;
//			return returnValue;
//		}
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
				this.chosenTickers.DrivingWeightedPositions.Values )
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
				this.numberOfPositionsToBeChosen;
			long sharesToBeTraded = this.getMaxBuyableShares( weightedPosition );
			this.account.AddOrder( new Order( orderType ,
				new Instrument( ticker ) , sharesToBeTraded ) );
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_openPositions_actually()
		{
			bool isToReverse = this.isToReverse();
			foreach ( WeightedPosition weightedPosition
									in this.chosenTickers.PortfolioWeightedPositions.Values )
				this.fiveMinutesBeforeMarketCloseEventHandler_openPosition( 
					weightedPosition , isToReverse );
			this.arePositionsUpToDateWithChosenTickers = true;
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_openPositions()
		{
			if ( this.chosenTickers.DrivingWeightedPositions != null )
				this.fiveMinutesBeforeMarketCloseEventHandler_openPositions_actually();
		}
		private double getTodayReturnForDrivingPositions()
		{
			double totalReturn = 0;
			foreach ( WeightedPosition weightedPosition in
				this.chosenTickers.DrivingWeightedPositions.Values )
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
//		private string getSignedTicker( string ticker )
//		{
//			string signedTicker = "";
//			if ( this.chosenTickers.PortfolioWeightedPositions.ContainsKey( ticker ) )
//				signedTicker = ticker;
//			if ( this.chosenTickers.PortfolioWeightedPositions.ContainsKey( "-" + ticker ) )
//				signedTicker = "-" + ticker;
//			if ( signedTicker == "" )
//				throw new Exception( "Nor ticker, nor '-'+ticker are contained in " +
//					"chosenTickers.PortfolioPositions ; this is an unexpected " +
//					"situation, when this method is invoked." );
//			return signedTicker;
//		}
//		private bool isShort( string signedTicker )
//		{
//			return ( signedTicker.StartsWith( "-" ) );
//		}
//		private bool isLong( string signedTicker )
//		{
//			return ( !this.isShort( signedTicker ) );
//		}
//		private bool isContainedInPortfolio( WeightedPosition weightedPosition )
//		{
//			bool isContained = false;
//			if ( this.account.Portfolio.ContainsKey( weightedPosition.Ticker ) )
//			{
//				Position position =
//					this.account.Portfolio.GetPosition( weightedPosition.Ticker );
//				isContained = ( position.q
//			}
//			bool isContained =
//				( this.account.Portfolio.ContainsKey( weightedPosition.Ticker ) )
//				&&
//				( ( weightedPosition.IsLong &&
//				this.account.Portfolio.IsLong( weightedPosition.Ticker ) ) ||
//				( weightedPosition.IsShort &&
//				this.account.Portfolio.IsShort( weightedPosition.Ticker ) ) );
//			return isContained;
//		}
//		private bool doPositionsCorrespondTo( ICollection signedTickers )
//		{
//			bool areUpTodate = true;
//			foreach ( string signedTicker in signedTickers )
//				areUpTodate = areUpTodate &&
//					this.isContainedInPortfolio( signedTicker );
//			return areUpTodate;
//		}
//		private string reverse( string signedTicker )
//		{
//			string reversedSignedTicker = "";
//			if ( this.isLong( signedTicker ) )
//				reversedSignedTicker = "-" + signedTicker;
//			if ( !this.isLong( signedTicker ) )
//				// signedTicker starts with a "-" character
//				reversedSignedTicker = signedTicker.Substring( 1 );
//			return reversedSignedTicker;
//		}
//		private ICollection reverse( ICollection signedTickers )
//		{
//			Hashtable reversedCollection = new Hashtable();
//			foreach ( string signedTicker in signedTickers )
//				reversedCollection.Add( this.reverse( signedTicker ) , null );
//			return reversedCollection.Keys;
//		}
//		private bool arePositionsUpToDateWithChosenTickers()
//		{
//			bool areUpTodate =
//				this.doPositionsCorrespondTo(
//				this.chosenTickers.PortfolioWeightedPositions ) ||
//				this.doPositionsCorrespondTo(
//				this.reverse( this.chosenTickers.PortfolioWeightedPositions ) );
//			return areUpTodate;
//		}
		private WeightedPosition getWeightedPosition( string ticker )
		{
			return this.chosenTickers.PortfolioWeightedPositions.GetWeightedPosition(
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
		private void fiveMinutesBeforeMarketCloseEventHandler_closePositions()
		{
			ArrayList tickers = new ArrayList();
			foreach ( string ticker in this.account.Portfolio.Keys )
				tickers.Add( ticker );
			foreach ( string ticker in tickers )
				this.account.ClosePosition( ticker );
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
		public bool AreBestTickersToBeChosen()
		{
			bool returnValue =
				( ( ( this.account.Portfolio.Count == 0 )
				&& ( ( this.lastOptimizationDate == DateTime.MinValue ) ) ) ||
				( this.now().DateTime >=
				this.lastOptimizationDate.AddDays( this.outOfSampleWindowDays ) ) );
			return returnValue;
		}
		public void OneHourAfterMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
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
//				Console.WriteLine( "Number of Eligible tickers: " +
//					this.eligibleTickers.EligibleTickers.Rows.Count );
				this.chosenTickers.SetWeightedPositions( this.eligibleTickers );
				this.arePositionsUpToDateWithChosenTickers = false;
				this.NewChosenTickers( this ,
					new WFLagNewChosenTickersEventArgs( this.chosenTickers ) );
				this.lastOptimizationDate = this.now().DateTime;
			}
			//			oneHourAfterMarketCloseEventHandler_orderChosenTickers( ( IEndOfDayTimer ) sender );
		}
	}
}
