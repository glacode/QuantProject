/*
QuantProject - Quantitative Finance Library

PriceRatioProvider.cs
Copyright (C) 2009
Marco Milletti

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

using QuantProject.ADT.Histories;
using QuantProject.ADT.Timing;
using QuantProject.Data.DataProviders.Quotes;
using QuantProject.ADT.Statistics;
using QuantProject.Business.DataProviders;

namespace QuantProject.Business.Strategies.TickersRelationships
{
	/// <summary>
	/// Class that provides methods concerning
	/// the price ratios between tickers
	/// </summary>
	[Serializable]
	public class PriceRatioProvider
	{
		private static string firstTicker;
		private static string secondTicker;
		private static DateTime startDateTime;
		private static DateTime endDateTime;
		private static double[] firstTickerPrices;
		private static double[] secondTickerPrices;
		private static double[] pricesRatios;
		private static double priceRatioAverage;
		private static double priceRatioStandardDeviation;
		
//		private string[] tickers;
//		private int numOfCombinationTwoByTwo;
//		private DateTime firstDateTime;
//		private DateTime lastDateTime;
		
		private static void getPriceRatioAverage_setPriceRatioAverage(string firstTicker, string secondTicker,
		                                    		 DateTime startDateTime, DateTime lastDateTime)
		{
			setPricesAndPricesRatios( firstTicker, secondTicker,
		                            startDateTime, lastDateTime );
			priceRatioAverage = BasicFunctions.SimpleAverage(pricesRatios);
		}
		/// <summary>
		/// the price ratio is computed by the following formula: 
		/// firstTickerPrice / secondTickerPrice
		/// </summary>
		public static double GetPriceRatioAverage( string firstTicker, string secondTicker,
		                                    DateTime startDateTime, DateTime lastDateTime )
		{
			getPriceRatioAverage_setPriceRatioAverage(firstTicker, secondTicker, startDateTime, lastDateTime);
			return priceRatioAverage;
		}
		
		private static void getPriceRatioStandardDeviation_setPriceRatioStandardDeviation(string firstTicker, string secondTicker,
		                                    							 DateTime startDateTime, DateTime lastDateTime)
		{
			setPricesAndPricesRatios( firstTicker, secondTicker,
		                            startDateTime, lastDateTime );
			priceRatioStandardDeviation = BasicFunctions.StdDev(pricesRatios);
		}
		/// <summary>
		/// the price ratio is computed by the following formula: 
		/// firstTickerPrice / secondTickerPrice
		/// </summary>
		public static double GetPriceRatioStandardDeviation( string firstTicker, string secondTicker,
		                                    								 DateTime startDateTime, DateTime lastDateTime )
		{
			getPriceRatioStandardDeviation_setPriceRatioStandardDeviation(firstTicker, secondTicker, startDateTime, lastDateTime);
			return priceRatioStandardDeviation;
		}
		
		/// <summary>
		/// Creates the Price Ratio provider
		/// </summary>
		public PriceRatioProvider(
//															string[] tickersToAnalyze,
//		                          DateTime startDate, DateTime endDate
		                         )
		{
//			this.tickers = tickersToAnalyze;
//			int n = this.tickers.Length;
//			//combinations without repetitions:
//			//n_fatt /( k_fatt * (n-k)_fatt ): when k = 2,
//			// it can be reduced to this simple formula:
//			this.numOfCombinationTwoByTwo = n * (n - 1) / 2;
			
		}
				
		#region setPricesAndPricesRatios
		private static void setPricesAndPricesRatios_set_setAdjustedCloseEndOfDayPrices()
		{
		  try{
				History firstTickerAdjCloseHistory =
			  	HistoricalQuotesProvider.GetAdjustedCloseHistory(firstTicker, startDateTime, endDateTime);
			  History secondTickerAdjCloseHistory = 
			  	HistoricalQuotesProvider.GetAdjustedCloseHistory(secondTicker, startDateTime, endDateTime);
			  History secondTickerAdjCloseHistoryWithDatesCommonToFirst =
			  	secondTickerAdjCloseHistory.Select(firstTickerAdjCloseHistory);
			  firstTickerPrices = new double[firstTickerAdjCloseHistory.Count];
			  secondTickerPrices = new double[secondTickerAdjCloseHistoryWithDatesCommonToFirst.Count];
			  for(int i = 0; i < firstTickerAdjCloseHistory.Count; i++)
				{	
			  	firstTickerPrices[i] = 
			  		(double)firstTickerAdjCloseHistory[firstTickerAdjCloseHistory.GetKey(i)];
			  	secondTickerPrices[i] = 
			  		(double)secondTickerAdjCloseHistoryWithDatesCommonToFirst[secondTickerAdjCloseHistoryWithDatesCommonToFirst.GetKey(i)];
			 	}
		} 
			catch(Exception ex)
			{
				string s = ex.ToString();
			}
		}
		private static void setPricesAndPricesRatios_set()
		{
			setPricesAndPricesRatios_set_setAdjustedCloseEndOfDayPrices();
			if(firstTickerPrices == null || secondTickerPrices == null || 
			   firstTickerPrices.Length == 0 || secondTickerPrices.Length == 0)
				throw new Exception("At least a ticker has not available prices for the given period");
			if(firstTickerPrices.Length != secondTickerPrices.Length)
				throw new Exception("Tickers have not the same number of prices");
		}
		private static void setPricesAndPricesRatios_setStaticMembers(string firstTicker, string secondTicker, 
		                                     DateTime startDateTime, DateTime lastDateTime)
		{
			priceRatioAverage = double.MinValue;
			priceRatioStandardDeviation = double.MinValue;
			PriceRatioProvider.startDateTime = startDateTime;
			PriceRatioProvider.endDateTime = lastDateTime;
			PriceRatioProvider.firstTicker = firstTicker;
			PriceRatioProvider.secondTicker = secondTicker;
		}
		private static void setPricesAndPricesRatios(string firstTicker, string secondTicker,
		                                    		 		 DateTime startDateTime, DateTime lastDateTime)
		{
			if( firstTicker != PriceRatioProvider.firstTicker ||
			    secondTicker != PriceRatioProvider.secondTicker ||
			    startDateTime != PriceRatioProvider.startDateTime ||
			    lastDateTime != PriceRatioProvider.endDateTime )
			{
				setPricesAndPricesRatios_setStaticMembers(firstTicker, secondTicker, startDateTime, lastDateTime);
				setPricesAndPricesRatios_set();
				pricesRatios = new double[firstTickerPrices.Length];
				for( int i = 0; i < pricesRatios.Length; i++ )
						pricesRatios[i] = firstTickerPrices[i] / secondTickerPrices[i];
			}
		}
		#endregion setPricesAndPricesRatios
		
	}	
}


