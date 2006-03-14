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
using QuantProject.Business.Timing;
using QuantProject.Scripts.SimpleTesting;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
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

		public event InSampleNewProgressEventHandler InSampleNewProgress;

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
				WFLagGenomeManager.GetTicker( signedTicker ) );
			int returnMultiplier = this.getReturnMultiplier( signedTicker );
			return todayReturnForTicker * returnMultiplier;
		}
		/// <summary>
		/// true iff driving positions are down today
		/// </summary>
		/// <returns></returns>
		private bool isToReverse()
		{
			double totalReturn = 0;
			foreach ( string signedTicker in this.chosenTickers.DrivingPositions.Keys )
				totalReturn += this.getTodayReturnForSignedTicker( signedTicker );
			return totalReturn < 0;
		}
		private OrderType
			fiveMinutesBeforeMarketCloseEventHandler_openPosition_getOrderType(
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
		private void fiveMinutesBeforeMarketCloseEventHandler_openPositions_actually()
		{
			bool isToReverse = this.isToReverse();
			foreach ( string signedTicker
									in this.chosenTickers.PortfolioPositions.Keys )
				this.fiveMinutesBeforeMarketCloseEventHandler_openPosition( 
					signedTicker , isToReverse );
		}
		private void fiveMinutesBeforeMarketCloseEventHandler_openPositions()
		{
			if ( this.chosenTickers.DrivingPositions != null )
				this.fiveMinutesBeforeMarketCloseEventHandler_openPositions_actually();
		}
		private double getTodayReturnForDrivingPositions()
		{
			double totalReturn = 0;
			foreach ( string signedTicker in
				this.chosenTickers.DrivingPositions.Keys )
				totalReturn += this.getTodayReturnForSignedTicker( signedTicker );
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
		private string getSignedTicker( string ticker )
		{
			string signedTicker = "";
			if ( this.chosenTickers.PortfolioPositions.ContainsKey( ticker ) )
				signedTicker = ticker;
			if ( this.chosenTickers.PortfolioPositions.ContainsKey( "-" + ticker ) )
				signedTicker = "-" + ticker;
			if ( signedTicker == "" )
				throw new Exception( "Nor ticker, nor '-'+ticker are contained in " +
					"chosenTickers.PortfolioPositions ; this is an unexpected " +
					"situation, when this method is invoked." );
			return signedTicker;
		}
		private bool isShort( string signedTicker )
		{
			return ( signedTicker.StartsWith( "-" ) );
		}
		private bool isLong( string signedTicker )
		{
			return ( !this.isShort( signedTicker ) );
		}
		private bool isContainedInPortfolio( string signedTicker )
		{
			string ticker = this.getTicker( signedTicker );
			bool isContained =
				( this.account.Portfolio.ContainsKey( ticker ) )
				&&
				( ( this.isLong( signedTicker ) &&
				this.account.Portfolio.IsLong( ticker ) ) ||
				( !this.isLong( signedTicker ) &&
				this.account.Portfolio.IsShort( ticker ) ) );
			return isContained;
		}
		private bool doPositionsCorrespondTo( ICollection signedTickers )
		{
			bool areUpTodate = true;
			foreach ( string signedTicker in signedTickers )
				areUpTodate = areUpTodate && this.isContainedInPortfolio( signedTicker );
			return areUpTodate;
		}
		private string reverse( string signedTicker )
		{
			string reversedSignedTicker = "";
			if ( this.isLong( signedTicker ) )
				reversedSignedTicker = "-" + signedTicker;
			if ( !this.isLong( signedTicker ) )
				// signedTicker starts with a "-" character
				reversedSignedTicker = signedTicker.Substring( 1 );
			return reversedSignedTicker;
		}
		private ICollection reverse( ICollection signedTickers )
		{
			Hashtable reversedCollection = new Hashtable();
			foreach ( string signedTicker in signedTickers )
				reversedCollection.Add( this.reverse( signedTicker ) , null );
			return reversedCollection.Keys;
		}
		private bool arePositionsUpToDateWithChosenTickers()
		{
			bool areUpTodate =
				this.doPositionsCorrespondTo(
				this.chosenTickers.PortfolioPositions.Keys ) ||
				this.doPositionsCorrespondTo(
				this.reverse( this.chosenTickers.PortfolioPositions.Keys ) );
			return areUpTodate;
		}
		private bool isReversed()
		{
			Position position = this.getFirstPosition();
			PositionType positionType = position.Type;
			string positionTicker = position.Instrument.Key;
			String signedTicker = this.getSignedTicker( positionTicker );
			bool isReversedPosition =
				( ( this.isLong( signedTicker )&&
				( position.Type == PositionType.Short ) ) ||
				( !this.isLong( signedTicker ) &&
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
				(!this.arePositionsUpToDateWithChosenTickers()) ||
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
		private bool areBestTickersToBeChosen()
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
			if ( this.areBestTickersToBeChosen() )
				// the portfolio is empty and
				// either the lastOptimizationDate has not been set yet
				// or outOfSampleWindowDays elapsed since last optimization
			{
				this.eligibleTickers.SetTickers();
				Console.WriteLine( "Number of Eligible tickers: " +
					this.eligibleTickers.EligibleTickers.Rows.Count );
				this.chosenTickers.SetSignedTickers( this.eligibleTickers );
				this.lastOptimizationDate = this.now().DateTime;
			}
			//			oneHourAfterMarketCloseEventHandler_orderChosenTickers( ( IEndOfDayTimer ) sender );
		}
	}
}
