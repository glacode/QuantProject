/*
QuantProject - Quantitative Finance Library

SignedTicker.cs
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
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Data;
using QuantProject.Data.DataTables;


namespace QuantProject.Business.Strategies
{
	/// <summary>
	/// String representation for possible positions.
	/// Several methods are provided (to be used to implement
	/// different strategies).
	/// </summary>
	public class SignedTicker
	{
		private string ticker;
		private PositionType positionType;

		public string Ticker
		{
			get { return this.ticker; }
		}
		public PositionType PositionType
		{
			get { return this.positionType; }
		}
		public double Multiplier
		{
			get
			{
				double multiplier = 1;
				if ( this.IsShort )
					multiplier = -1;
				return multiplier;
			}
		}
		public bool IsShort
		{
			get
			{
				bool isShort = ( this.positionType == PositionType.Short );
				return isShort;
			}
		}
		public bool IsLong
		{
			get
			{
				return !this.IsShort;
			}
		}
		public SignedTicker OppositeSignedTicker
		{
			get
			{
				SignedTicker oppositeSignedTicker =
					new SignedTicker( this.Ticker ,
					this.getOppositePositionType( this.PositionType ) );
				return oppositeSignedTicker;
			}
		}
		private PositionType getOppositePositionType( PositionType positionType )
		{
			PositionType oppositePositionType;
			if ( this.IsLong )
				oppositePositionType = PositionType.Short;
			else
				// this.IsShort
				oppositePositionType = PositionType.Long;
			return oppositePositionType;
		}
		public SignedTicker( string ticker , PositionType positionType )
		{
			this.ticker = ticker;
			this.positionType = positionType;
		}
		public SignedTicker( string signedTicker )
		{
			this.signedTicker_checkParams( signedTicker );
			this.ticker = this.getTicker( signedTicker );
			this.positionType = this.getPositionType( signedTicker );
		}
		private bool isValidSignedTicker( string signedTicker )
		{
			bool isActuallyValidSignedTicker = false;
			string firstCharacter = signedTicker.Substring( 0 , 1 );
			if ( ( firstCharacter.CompareTo( "A" ) != - 1 ) &&
				( ( firstCharacter.CompareTo( "Z" ) != 1 ) ) )
				// the first signedTickers' character is an upcase letter
				isActuallyValidSignedTicker = true;
			if ( ( firstCharacter == "^" ) )
				isActuallyValidSignedTicker = true;
			if ( ( firstCharacter == "-" ) )
				isActuallyValidSignedTicker = true;
			return isActuallyValidSignedTicker;
		}
		private void signedTicker_checkParams( string signedTicker )
		{
			if ( !this.isValidSignedTicker( signedTicker ) )
				throw new Exception( "signedTickers is not a valid string to " +
					"identify and construct a new SignedTicker object!" );
		}
		private bool isStringForLongSignedTicker( string signedTicker )
		{
			return !(signedTicker.IndexOf( "-" ) == 0 );
		}
		private string getTicker( string signedTicker )
		{
			string ticker;
			if ( this.isStringForLongSignedTicker( signedTicker ) )
				ticker = signedTicker;
			else
				// signedTicker is a string for a short SignedTicker and
				// signedTicker begins with a '-' sign
				ticker = signedTicker.Substring( 1 );
			return ticker;
		}
		private PositionType getPositionType( string signedTicker )
		{
			PositionType positionType;
			if ( this.isStringForLongSignedTicker( signedTicker ) )
				positionType = PositionType.Long;
			else
				// signedTicker is a string for a short SignedTicker and
				// signedTicker begins with a '-' sign
				positionType = PositionType.Short;
			return positionType;
		}
		
		public static string GetSignedTicker( Position position )
		{
			string signedTicker = position.Instrument.Key;
			if ( position.Type == PositionType.Short )
				signedTicker = "-" + signedTicker;
			return signedTicker;
		}
		public static string GetTicker( string signedTicker )
		{
			string ticker = signedTicker;
			if ( signedTicker.StartsWith( "-" ) )
				ticker = signedTicker.Substring( 1 );
			return ticker;
		}
//		public static bool IsShort( string signedTicker )
//		{
//			return ( signedTicker.StartsWith( "-" ) );
//		}
//		public static bool IsLong( string signedTicker )
//		{
//			return ( !SignedTicker.IsShort( signedTicker ) );
//		}
		public OrderType MarketOrderType
		{
			get
			{
				OrderType orderType = OrderType.MarketBuy;
				if ( this.IsShort )
					orderType = OrderType.MarketSellShort;
				return orderType;
			}
		}
		public double GetCloseToCloseDailyReturn( DateTime today )
		{
			string ticker = this.Ticker;
			HistoricalAdjustedQuoteProvider historicalAdjustedQuoteProvider =
				new HistoricalAdjustedQuoteProvider();
			double todayMarketValueAtClose =
				historicalAdjustedQuoteProvider.GetMarketValue(
					ticker , HistoricalEndOfDayTimer.GetMarketClose( today ) );
			DateTime yesterday = today.AddDays( -1 );
			DateTime yesterdayAtClose =
				HistoricalEndOfDayTimer.GetMarketClose( yesterday );
			double yesterdayMarketValueAtClose =
				historicalAdjustedQuoteProvider.GetMarketValue(
					ticker , yesterdayAtClose );
			double dalyReturnForLongPosition =
				( todayMarketValueAtClose / yesterdayMarketValueAtClose ) - 1;
			double dailyReturn;
			if ( this.IsShort )
				// signedTicker represents a short position
				dailyReturn = - dalyReturnForLongPosition;
			else
				// signedTicker represents a long position
				dailyReturn = dalyReturnForLongPosition;
			return dailyReturn;
		}
    
//		#region GetCloseToCloseReturnsForUnsignedTicker
//		/// <summary>
//		/// Returns the (adjusted) close to close returns for the given unsigned ticker,
//		/// starting from the firstQuoteDate and ending with the lastQuoteDate
//		/// </summary>
//		/// <param name="ticker"></param>
//		/// <param name="firstQuoteDate"></param>
//		/// <param name="lastQuoteDate"></param>
//		/// <returns></returns>
//		public static double GetCloseToCloseReturnsForUnsignedTicker(
//			string ticker , DateTime firstQuoteDate , DateTime lastQuoteDate )
//		{
////			Quotes tickerQuotes =
////				new Quotes( ticker , firstQuoteDate , lastQuoteDate );
////			float[] tickerAdjustedCloses =
////				QuantProject.Data.ExtendedDataTable.GetArrayOfFloatFromColumn(
////				tickerQuotes , "quAdjustedClose");
//			float[] tickerAdjustedCloses =
//				Quotes.GetArrayOfCloseToCloseRatios( ticker , firstQuoteDate , lastQuoteDate , 1 );
//			float[] closeToCloseTickerReturns =
//				new float[ tickerAdjustedCloses.Length - 1 ];
//			int i = 0; //index for ratesOfReturns array
//			for( int idx = 0 ; idx < tickerAdjustedCloses.Length - 1 ; idx++ )
//			{
//				closeToCloseTickerReturns[ i ] =
//					tickerAdjustedCloses[ idx + 1 ] /	tickerAdjustedCloses[ idx ] - 1;
//				i++;
//			}
//			return closeToCloseTickerReturns;
//		}
//		#endregion //GetCloseToCloseReturnsForUnsignedTicker
    
    
		/// <summary>
		/// Gets portfolio's return for a given period, for given tickers
		/// </summary>
		/// <param name="signedTickers">Array of signed tickers that compose the portfolio</param>
		/// <param name="tickersWeights">Array of weights for tickers - the same order has to be provided!</param>
		/// <param name="startDate">Start date for the period for which return has to be computed</param>
		/// <param name="endDate">End date for the period for which return has to be computed</param>
//		public static double GetCloseToClosePortfolioReturn( string[] signedTickers ,
//			double[] tickersWeights ,
//			SortedList commonMarketDays )
//		{
//			const double initialEquity = 1.0;
//			double equityValue = initialEquity;
//			Quotes[] tickersQuotes = new Quotes[ signedTickers.Length ];
//			for( int i = 0 ; i < signedTickers.Length ; i++ )
//			{
//				tickersQuotes[ i ] =
//					new Quotes( SignedTicker.GetTicker( (string)signedTickers[ i ] ) ,
//					commonMarketDays );
//			}
//			double[] returns = new double[ tickersQuotes[0].Rows.Count ];
//			getCloseToClosePortfolioReturn_setReturns( returns , signedTickers ,
//				tickersWeights , tickersQuotes);
//			for( int i = 0 ; i < returns.Length ; i++ )
//				equityValue = 
//							equityValue + equityValue * returns[i];
//			
//			return (equityValue - initialEquity)/initialEquity;
//		}
//		
		/// <summary>
		/// Gets portfolio's return, for the given tickers, considering only
		/// the market days contained in commonMarketDays
		/// </summary>
		/// <param name="signedTickers"></param>
		/// <param name="commonMarketDays"></param>
		/// <returns></returns>
//		public static double GetCloseToClosePortfolioReturn(
//			string[] signedTickers,
//			SortedList commonMarketDays )
//		{
//			double[] tickersWeights = new double[signedTickers.Length];
//			for(int i = 0; i<signedTickers.Length; i++)
//				tickersWeights[i] = 1.0/signedTickers.Length;
//			
//			return GetCloseToClosePortfolioReturn(
//				signedTickers , tickersWeights , commonMarketDays );
//		}
//    
		
		private static string[] getSignedTickersArray( ICollection signedTickers )
		{
			string[] signedTickersArray = new string[ signedTickers.Count ];
			int i = 0;
			foreach( string signedTicker in signedTickers )
			{
				signedTickersArray[ i ] = signedTicker;
				i++;
			}
			return signedTickersArray;
		}
		private static string[] getSignedTickerArray ( ICollection signedTickers )
		{
			string[] signedTickerArray = new string[ signedTickers.Count ];
			int i = 0;
			foreach( string signedTicker in signedTickers )
			{
				signedTickerArray[ i ] = signedTicker;
				i++;
			}
			return signedTickerArray;
		}
		#region GetCloseToClosePortfolioReturns
		private static double getCloseToCloseReturn( string ticker ,
			SortedList datesForReturnComputation , int i )
		{
			DateTime previousDate = (DateTime)datesForReturnComputation.GetByIndex( i );
			DateTime currentDate =
				(DateTime)datesForReturnComputation.GetByIndex( i + 1 );
			HistoricalAdjustedQuoteProvider historicalQuoteProvider =
				new HistoricalAdjustedQuoteProvider();
			double previousMarketValue = historicalQuoteProvider.GetMarketValue(
				ticker , HistoricalEndOfDayTimer.GetMarketClose( previousDate ) );
//				new EndOfDayDateTime( previousDate , EndOfDaySpecificTime.MarketClose ) );
			double currentMarketValue = historicalQuoteProvider.GetMarketValue(
				ticker , HistoricalEndOfDayTimer.GetMarketClose( currentDate ) );
//				new EndOfDayDateTime( currentDate , EndOfDaySpecificTime.MarketClose ) );
			double closeToCloseReturn = currentMarketValue / previousMarketValue - 1.0;
			return closeToCloseReturn;
		}
//		private static double getMultiplier( string signedTicker )
//		{
//			double multiplier;
//			if ( IsShort( signedTicker ) )
//				multiplier = -multiplier;
//			return multiplier;
//		}
//		private static double getCloseToClosePortfolioReturn(
//			string[] signedTickers , SortedList datesForReturnComputation , int i )
//		{
//			double dailyReturn = 0.0;
//			foreach ( String signedTicker in signedTickers )
//				dailyReturn += getMultiplier( signedTicker ) *
//					getCloseToCloseReturn( GetTicker( signedTicker ) ,
//					datesForReturnComputation , i ) /
//					signedTickers.Length;  // every position is considered with same weight
//			return dailyReturn;
//		}
//		private static double[] getCloseToClosePortfolioReturns(
//			string[] signedTickers , SortedList datesForReturnComputation )
//		{
//			// the return for first DateTime cannot be computed so the returned
//			// array will have one element less the datesForReturnComputation
//			double[] closeToClosePortfolioReturns =
//				new double[ datesForReturnComputation.Count - 1 ];
//			for ( int i=0 ; i < closeToClosePortfolioReturns.Length ; i++ )
//				closeToClosePortfolioReturns[ i ] =	getCloseToClosePortfolioReturn(
//					signedTickers , datesForReturnComputation , i );
//			return closeToClosePortfolioReturns;
//		}
		private static double[] getCloseToClosePortfolioReturns(
			ICollection signedTickers,	SortedList datesForReturnComputation )
		{
			string[] signedTickerArray =
				getSignedTickerArray( signedTickers );
			return getCloseToClosePortfolioReturns( signedTickerArray ,
				datesForReturnComputation );
		}
		/// <summary>
		/// Gets portfolio's return for a given period, for given tickers
		/// </summary>
		/// <param name="signedTickers">ICollection of signed tickers that compose the portfolio (each ticker has the same weight)</param>
		/// <param name="startDate">Start date for the period for which return has to be computed</param>
		/// <param name="endDate">End date for the period for which return has to be computed</param>
		public static double[] GetCloseToClosePortfolioReturns(
			ICollection signedTickers,	DateTime firstDate,	DateTime lastDate )
		{
			SortedList datesForReturnComputation = Quotes.GetCommonMarketDays(
				signedTickers , firstDate , lastDate );
			return GetCloseToClosePortfolioReturns(
				signedTickers , datesForReturnComputation );
		}
		/// <summary>
		/// Gets portfolio's return (for given signed tickers) computed on the
		/// market days contained in commonMarketDays
		/// </summary>
		/// <param name="signedTickers"></param>
		/// <param name="commonMarketDays">SortedList of DateTime objects</param>
		/// <returns></returns>
		public static double[] GetCloseToClosePortfolioReturns(
			ICollection signedTickers,	SortedList commonMarketDays )
		{
			return
				getCloseToClosePortfolioReturns( signedTickers , commonMarketDays );
		}
		#endregion
    
		/// <summary>
    /// Changes sign of each ticker contained in the given 
    /// array of signed tickers
    /// </summary>
    public static void ChangeSignOfEachTicker( string[] signedTickers )
		{
      for(int i = 0; i<signedTickers.Length; i++)
      {
        if(signedTickers[i] != null)
          signedTickers[i] = GetOppositeSignedTicker(signedTickers[i]);
      }
		}
		public static string GetOppositeSignedTicker( string stringForSignedTicker )
		{
			SignedTicker signedTicker = new SignedTicker( stringForSignedTicker );
			SignedTicker oppositeSignedTicker = signedTicker.OppositeSignedTicker;
			return oppositeSignedTicker.Ticker;
		}
	}
}
