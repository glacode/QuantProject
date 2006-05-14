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
		public SignedTicker( string ticker , PositionType positionType )
		{
			this.ticker = ticker;
			this.positionType = positionType;
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
		public static bool IsShort( string signedTicker )
		{
			return ( signedTicker.StartsWith( "-" ) );
		}
		public static bool IsLong( string signedTicker )
		{
			return ( !SignedTicker.IsShort( signedTicker ) );
		}
		public static string GetOppositeSignedTicker( string signedTicker )
		{
			string oppositeSignedTicker = "";
			if ( SignedTicker.IsShort( signedTicker ) )
				oppositeSignedTicker = SignedTicker.GetTicker( signedTicker );
			if ( SignedTicker.IsLong( signedTicker ) )
				oppositeSignedTicker = "-" + SignedTicker.GetTicker( signedTicker );
			return oppositeSignedTicker;
		}
		public static OrderType GetMarketOrderType( string signedTicker )
		{
			OrderType orderType = OrderType.MarketBuy;
			if ( SignedTicker.IsShort( signedTicker ) )
				orderType = OrderType.MarketSellShort;
			return orderType;
		}
		public static double GetCloseToCloseDailyReturn(
			string signedTicker , DateTime today )
		{
			string ticker = SignedTicker.GetTicker( signedTicker );
			HistoricalAdjustedQuoteProvider historicalAdjustedQuoteProvider =
				new HistoricalAdjustedQuoteProvider();
			double todayMarketValueAtClose =
				historicalAdjustedQuoteProvider.GetMarketValue(	ticker ,
				new EndOfDayDateTime( today , EndOfDaySpecificTime.MarketClose ) );
			DateTime yesterday = today.AddDays( -1 );
			EndOfDayDateTime yesterdayAtClose = new
				EndOfDayDateTime( yesterday ,	EndOfDaySpecificTime.MarketClose );
			double yesterdayMarketValueAtClose =
				historicalAdjustedQuoteProvider.GetMarketValue(
				ticker , yesterdayAtClose );
			double dalyReturnForLongPosition =
				( todayMarketValueAtClose / yesterdayMarketValueAtClose ) - 1;
			double dailyReturn;
			if ( SignedTicker.IsShort( signedTicker ) )
				// signedTicker represents a short position
				dailyReturn = - dalyReturnForLongPosition;
			else
				// signedTicker represents a long position
				dailyReturn = dalyReturnForLongPosition;
			return dailyReturn;
		}
    
    #region getCloseToClosePortfolioReturn_setReturns

    private static double getCloseToClosePortfolioReturn_setReturns_getReturn(int returnDay,
                        string[] signedTickers,double[] tickersWeights,Quotes[] tickersQuotes )
    {
      double returnValue = 0.0;
      float signOfTicker = 1.0f;
      for(int indexForTicker = 0; indexForTicker<signedTickers.Length; indexForTicker++)
      {
        if(SignedTicker.IsLong(signedTickers[indexForTicker]))
          signOfTicker = 1.0f;
        else
          signOfTicker = -1.0f;
      	
        returnValue +=
          signOfTicker *
          ((float)tickersQuotes[indexForTicker].Rows[returnDay][Quotes.AdjustedCloseToCloseRatio] - 1.0f)*
          (float)tickersWeights[indexForTicker];
      }
      return returnValue;
    }

    private static void getCloseToClosePortfolioReturn_setReturns(double[] returnsToSet,
                                                                  string[] signedTickers,
                                                                  double[] tickersWeights,
                                                                  Quotes[] tickersQuotes )
    {
      for(int i = 0; i < returnsToSet.Length; i++)
      {
        returnsToSet[i] =
          getCloseToClosePortfolioReturn_setReturns_getReturn(
             i,signedTickers,tickersWeights,tickersQuotes);
      }
    }
    
    #endregion

    /// <summary>
    /// Gets portfolio's return for a given period, for given tickers
    /// </summary>
    /// <param name="signedTickers">Array of signed tickers that compose the portfolio</param>
    /// <param name="tickersWeights">Array of weights for tickers - the same order has to be provided!</param>
    /// <param name="startDate">Start date for the period for which return has to be computed</param>
    /// <param name="endDate">End date for the period for which return has to be computed</param>
    public static double GetCloseToClosePortfolioReturn(string[] signedTickers,
                                                        double[] tickersWeights,
                                                        DateTime startDate,
                                                        DateTime endDate )
    {
      double returnValue = 0.0;
      Quotes[] tickersQuotes = new Quotes[signedTickers.Length];
      for(int i = 0; i<signedTickers.Length; i++)
      {
        tickersQuotes[i] = new Quotes(SignedTicker.GetTicker(signedTickers[i]),
                                      startDate, endDate);
        if(tickersQuotes[i].Rows.Count == 0)
        //no quotes are available at the given period
          throw new MissingQuotesException(SignedTicker.GetTicker(signedTickers[i]),
        	       													 startDate, endDate);
      }
      double[] returns = new double[tickersQuotes[0].Rows.Count];
      getCloseToClosePortfolioReturn_setReturns(returns,signedTickers,
                                                tickersWeights, tickersQuotes);
      for(int i = 0; i < returns.Length; i++)
        returnValue = (1.0+returnValue) * returns[i];

      return returnValue;
    }
		
    /// <summary>
    /// Gets portfolio's return for a given period, for given tickers
    /// </summary>
    /// <param name="signedTickers">Array of signed tickers that compose the portfolio (each ticker has the same weight)</param>
    /// <param name="startDate">Start date for the period for which return has to be computed</param>
    /// <param name="endDate">End date for the period for which return has to be computed</param>
    public static double GetCloseToClosePortfolioReturn(string[] signedTickers,
                                                        DateTime startDate,
                                                        DateTime endDate )
    {
    	double[] tickersWeights = new double[signedTickers.Length];
    	for(int i = 0; i<signedTickers.Length; i++)
    		tickersWeights[i] = 1.0/signedTickers.Length;
    	
    	return GetCloseToClosePortfolioReturn(
    	        signedTickers,tickersWeights, startDate, endDate);
    	
    }
    
    /// <summary>
    /// Changes sign of each ticker contained in the given 
    /// array of signed tickers
    /// </summary>
    public static void ChangeSignOfEachTicker( string[] signedTickers )
		{
    	for(int i = 0; i<signedTickers.Length; i++)
    		signedTickers[i] = GetOppositeSignedTicker(signedTickers[i]);
		}
	}
}
