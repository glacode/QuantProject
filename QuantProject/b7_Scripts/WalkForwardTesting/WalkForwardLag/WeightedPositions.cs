/*
QuantProject - Quantitative Finance Library

WeightedPositions.cs
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
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	[Serializable]
	/// <summary>
	/// Consistent group of weighted positions: weights are
	/// checked to sum up to 1.
	/// </summary>
	public class WeightedPositions : SortedList
	{
		/// <summary>
		/// returns the type for this class
		/// </summary>
		public static System.Type Type
		{
			get { return getType(); }
		}
		public WeightedPositions( double[] normalizedWeightValues ,
			string[] tickers )
		{
			this.checkParameters( normalizedWeightValues , tickers );
			for ( int i=0 ; i < tickers.Length ; i++ )
			{
				string ticker = tickers[ i ];
				double weight = normalizedWeightValues[ i ];
				if ( !this.ContainsKey( ticker ) )
					this.Add( ticker , new WeightedPosition( weight , ticker ) );
				else
					((WeightedPosition)this[ ticker ]).Weight += weight;
			}
		}
		#region checkParameters
		private void checkParameters_checkDoubleTickers( string[] tickers )
		{
			SortedList sortedTicker = new SortedList();
			foreach ( string ticker in tickers )
				if ( !sortedTicker.ContainsKey( ticker ) )
					sortedTicker.Add( ticker , ticker );
				else
					throw new Exception( "The WeightedPositions constructur " +
						"has received a tickers parameter with the ticker '" +
						ticker + "' that is contained twice! This is not allowed." );
		}
		private void checkParameters( double[] normalizedWeightValues ,
			string[] tickers )
		{
			if ( normalizedWeightValues.Length != tickers.Length )
				throw new Exception( "The number of normalized weights is " +
					"different from the number of tickers. They should be the same " +
					"number!" );
			double totalWeight =
				ADT.Statistics.BasicFunctions.SumOfAbs( normalizedWeightValues );
			if ( ( totalWeight < 0.999 ) || ( totalWeight > 1.001 ) )
				throw new Exception( "The total of (absolute) weights " +
					"should sum up to 1, " +
					"but it sums up to " + totalWeight.ToString() );
			this.checkParameters_checkDoubleTickers( tickers );
		}
		#endregion
		public WeightedPosition GetWeightedPosition( string ticker )
		{
			return (WeightedPosition)this[ ticker ];
		}
		public WeightedPosition GetWeightedPosition( int i )
		{
			return (WeightedPosition)this.GetByIndex( i );
		}
		#region GetEquityLine
		/// <summary>
		/// Returns a virtual amount of quantities for each virtual ticker.
		/// Non integer values can be returned also, that's why we call
		/// them virtual quantities
		/// </summary>
		/// <returns></returns>
		private Hashtable getVirtualQuantities( double beginningCash , DateTime dateTime )
		{
			Hashtable virtualQuantities = new Hashtable();
			HistoricalAdjustedQuoteProvider historicalAdjustedQuoteProvider =
				new HistoricalAdjustedQuoteProvider();
			EndOfDayDateTime endOfDayDateTime =
				new EndOfDayDateTime( dateTime , EndOfDaySpecificTime.MarketClose );
			foreach( WeightedPosition weightedPosition in	this.Values )
			{
				string ticker = weightedPosition.Ticker;
				double valueForThisPosition =
					beginningCash * Math.Abs( weightedPosition.Weight );
				double tickerQuote =
					historicalAdjustedQuoteProvider.GetMarketValue(
					ticker , endOfDayDateTime );
				double virtualQuantityForThisPosition = valueForThisPosition / tickerQuote;
				if ( weightedPosition.IsShort )
					virtualQuantityForThisPosition = -virtualQuantityForThisPosition;
				virtualQuantities.Add( ticker , virtualQuantityForThisPosition );
			}
			return virtualQuantities;
		}
		private double getCash( double beginningCash , Hashtable virtualQuantities )
		{
			double cash = beginningCash;
			foreach ( WeightedPosition weightedPosition in this.Values )
			{
				double thisVirtualQuantity =
					(double)virtualQuantities[ weightedPosition.Ticker ];
				double thisPositionValue =
					beginningCash * Math.Abs( weightedPosition.Weight );
				if ( thisVirtualQuantity > 0 )
					// long position
					cash -= thisPositionValue;
				else
					// virtualQuantity < 0 i.e. short position
					cash += thisPositionValue;
			}
			return cash;
		}
		private double getVirtualPortfolioValue( DateTime dateTime ,
			Hashtable tickerVirtualQuantities )
		{
			HistoricalAdjustedQuoteProvider historicalAdjustedQuoteProvider =
				new HistoricalAdjustedQuoteProvider();
			double virtualPortfolioValue = 0;
			foreach( string ticker in tickerVirtualQuantities.Keys )
			{
				EndOfDayDateTime endOfDayDateTime =	new EndOfDayDateTime(
					dateTime , EndOfDaySpecificTime.MarketClose );
				double tickerQuote = historicalAdjustedQuoteProvider.GetMarketValue(
					ticker , endOfDayDateTime );
				double virtualQuantity = (double)tickerVirtualQuantities[ ticker ];
				virtualPortfolioValue +=	virtualQuantity * tickerQuote;
			}
			return virtualPortfolioValue;
		}
		/// <summary>
		/// computes the equity line for this weighted positions. The equity line is virtual because
		/// quantities for each ticker can be non integer also (it is simulated an account where
		/// every single penny is always invested, so fractional quantities are simulated)
		/// </summary>
		/// <param name="beginningCash">cash amount when the simulated strategy begins</param>
		/// <param name="equityDates">an ordered list of DateTime(s) where the equity line has to be computed</param>
		/// <returns></returns>
		public EquityLine GetVirtualEquityLine(
			double beginningCash , SortedList equityDates )
		{
			DateTime firstDate = (DateTime)equityDates.GetKey( 0 );
			EquityLine equityLine =
				new EquityLine();
			Hashtable virtualQuantities =
				this.getVirtualQuantities( beginningCash , firstDate );
			double cash = this.getCash( beginningCash , virtualQuantities ); 
			for( int i = 0 ; i < equityDates.Count ; i++ )
			{
				DateTime dateTime = (DateTime)equityDates.GetKey( i );
				equityLine.Add( dateTime ,
					cash + this.getVirtualPortfolioValue( dateTime ,
					virtualQuantities ) );
			}
			return equityLine;
		}
		#endregion
		#region getType
		private static WeightedPositions getSimpleDummyInstance()
		{
			double[] weights = {1.0};
			string[] tickers = {"DUMMY"};
			WeightedPositions dummyInstance = new WeightedPositions( weights , tickers );
			return dummyInstance;
		}
		private static System.Type getType()
		{
			WeightedPositions weightedPositions =
				WeightedPositions.getSimpleDummyInstance();
			return weightedPositions.GetType();
		}
		#endregion
		#region GetCloseToClosePortfolioReturns
//		private static double getCloseToCloseReturn( string ticker ,
//			SortedList datesForReturnComputation , int i )
//		{
//			DateTime previousDate = (DateTime)datesForReturnComputation.GetByIndex( i );
//			DateTime currentDate =
//				(DateTime)datesForReturnComputation.GetByIndex( i + 1 );
//			HistoricalAdjustedQuoteProvider historicalQuoteProvider =
//				new HistoricalAdjustedQuoteProvider();
//			double previousQuote = historicalQuoteProvider.GetMarketValue( ticker ,
//				new EndOfDayDateTime( previousDate , EndOfDaySpecificTime.MarketClose ) );
//			double currentQuote = historicalQuoteProvider.GetMarketValue( ticker ,
//				new EndOfDayDateTime( currentDate , EndOfDaySpecificTime.MarketClose ) );
//			double closeToCloseReturn = currentQuote / previousQuote - 1.0;
//			return closeToCloseReturn;
//		}
//		private static double getMultiplier( string signedTicker )
//		{
//			double multiplier = 1.0;
//			if ( IsShort( signedTicker ) )
//				multiplier = -multiplier;
//			return multiplier;
//		}
		private double getCloseToClosePortfolioReturn(
			SortedList datesForReturnComputation , int i )
		{
			DateTime dateTime =
				(DateTime)datesForReturnComputation.GetByIndex( i + 1 );
			double dailyReturn = 0.0;
			foreach ( WeightedPosition weightedPosition in
				this.Values )
				dailyReturn +=
					weightedPosition.GetCloseToCloseDailyReturn( dateTime );
			return dailyReturn;
		}
		private double[] getCloseToClosePortfolioReturns(
			SortedList datesForReturnComputation )
		{
			// the return for first DateTime cannot be computed so the returned
			// array will have one element less the datesForReturnComputation
			double[] closeToClosePortfolioReturns =
				new double[ datesForReturnComputation.Count - 1 ];
			for ( int i=0 ; i < closeToClosePortfolioReturns.Length ; i++ )
				closeToClosePortfolioReturns[ i ] =
					this.getCloseToClosePortfolioReturn(
					datesForReturnComputation , i );
			return closeToClosePortfolioReturns;
		}
		/// <summary>
		/// Gets portfolio's return (for this weighted positions) computed on the
		/// market days contained in commonMarketDays
		/// </summary>
		/// <param name="commonMarketDays">SortedList of DateTime objects: positions
		/// are assumed to be exchanged in all such market days, otherwise
		/// an exception should be thrown</param>
		/// <returns></returns>
		public double[] GetCloseToClosePortfolioReturns( SortedList commonMarketDays )
		{
			// TO DO check parameter: check if each position is exchanged in each common market day
			return
				getCloseToClosePortfolioReturns( commonMarketDays );
		}
		#endregion
		/// <summary>
		/// It controls if a WeightedPositions class may be build on the
		/// given tickers
		/// </summary>
		/// <param name="tickers"></param>
		/// <returns></returns>
		public static bool AreValidTickers( string[] tickers )
		{
			return !QuantProject.ADT.Collections.CollectionManager.ContainsDuplicates(
				tickers );
		}
		#region GetNormalizedWeights
		private static double getAbsoluteWeightSum( double[] nonNormalizedWeights )
		{
			double absoluteWeightSum = 0;
			foreach ( double nonNormalizedWeight in nonNormalizedWeights )
				absoluteWeightSum += Math.Abs( nonNormalizedWeight );
			return absoluteWeightSum;
		}
		private static double getNormalizingFactor( double[] nonNormalizedWeights )
		{
			double absoluteWeightSum =
				getAbsoluteWeightSum( nonNormalizedWeights );
			double normalizingFactor = 1 / absoluteWeightSum;
			return normalizingFactor;
		}
		private static double[] getNormalizedWeights( double[] nonNormalizedWeights ,
			double normalizingFactor )
		{
			double[] normalizedWeights = new double[ nonNormalizedWeights.Length ];
			for ( int i = 0 ; i < nonNormalizedWeights.Length ; i ++ )
				normalizedWeights[ i ] = nonNormalizedWeights[ i ] * normalizingFactor;
			return normalizedWeights;
		}
		/// <summary>
		/// Returns weights whose absolute values sum up to 1
		/// </summary>
		/// <param name="nonNormalizedWeights"></param>
		/// <returns></returns>
		/// 
		public static double[] GetNormalizedWeights( double[] nonNormalizedWeights )
		{
			double normalizingFactor =
				getNormalizingFactor( nonNormalizedWeights );
			return
				getNormalizedWeights( nonNormalizedWeights , normalizingFactor );
		}
		#endregion
	}
}
