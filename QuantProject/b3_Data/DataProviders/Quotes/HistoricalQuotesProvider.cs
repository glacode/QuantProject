/*
QuantProject - Quantitative Finance Library

HistoricalQuotesProvider.cs
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
using System.Data;

using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.Data.DataProviders.Caching;
using QuantProject.DataAccess;
using QuantProject.DataAccess.Tables;

namespace QuantProject.Data.DataProviders.Quotes
{
	// TO DO : rename this class HistoricalQuotesProvider, then
	// add a class HistoricalBarsProvider and introduce a
	// IHistoricalDataProvider to be implemented by the two classes
	
	/// <summary>
	/// Static methods to access the historical data
	/// </summary>
	public class HistoricalQuotesProvider
	{
		private static Hashtable cachedHistories = new Hashtable();

//		private static Cache privateCache = new Cache();
		private static ICache privateCache = new Cache();


		private static DateTime minDate = DateTime.MinValue;
		private static DateTime maxDate = DateTime.MaxValue;
		
		private static DateTime timeForOpen =
			new DateTime( 1900 , 1 , 1 , 9 , 30 , 0 );
		private static DateTime timeForClose =
			new DateTime( 1900 , 1 , 1 , 16 , 0 , 0 );
		private static DateTime timeForOneHourAfterMarketClose =
			timeForClose.AddHours( 1 );

		/// <summary>
		/// When defined, sets the minimum DateTime to be cached
		/// </summary>
		public static DateTime MinDate
		{
			get { return minDate; }
			set { minDate = value; }
		}

		/// <summary>
		/// When defined, sets the maximum DateTime to be cached
		/// </summary>
		public static DateTime MaxDate
		{
			get { return maxDate; }
			set { maxDate = value; }
		}

		/// <summary>
		/// Caching class
		/// </summary>
		public static ICache Cache
		{
			get { return privateCache; }
			set { privateCache = value; }
		}

		/// <summary>
		/// Provides historical data
		/// </summary>
		public HistoricalQuotesProvider()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		
		
		
		
		
		
		private static void checkIfIsDate(
			DateTime dateTime )
		{
			if ( !ExtendedDateTime.IsDate( dateTime ) )
				throw new Exception(
					"The given dateTime contains time also. It must be a just date." );
		}

		// to be deleted
		public static void Add( string instrumentKey )
		{
			Hashtable barComponentHistories = new Hashtable();
			cachedHistories.Add( instrumentKey , barComponentHistories );
		}

		/// <summary>
		/// Adds a new instrument quote history to be cached in memory
		/// </summary>
		/// <param name="instrument">Instrument to be monitored</param>
		/// <param name="barComponent">Bar component to be monitored (Open, High, Low, Close or Volume)</param>
		public static void Add( string instrumentKey , QuoteField quoteField )
		{
			if ( !cachedHistories.ContainsKey( instrumentKey ) )
				cachedHistories.Add( instrumentKey , new Hashtable() );
			((Hashtable) cachedHistories[ instrumentKey ]).Add(
				quoteField , quoteField );
		}

		//    //public static void
//
		//    public static void SetCachedHistories(
		//      DateTime startDateTime , DateTime endDateTime )
		//    {
		//      ArrayList keyArray = new ArrayList();
		//      foreach (string instrumentKey in cachedHistories.Keys)
		//        keyArray.Add( instrumentKey );
		//      foreach (string instrumentKey in keyArray )
		//      {
		//        Hashtable barComponents = new Hashtable();
		//        foreach (BarComponent barComponent in
		//          (( Hashtable )cachedHistories[ instrumentKey ]).Keys )
		//          barComponents.Add( barComponent , barComponent );
		//        Hashtable histories = DataBase.GetHistories(
		//          instrumentKey , barComponents , startDateTime , endDateTime );
		//        cachedHistories[ instrumentKey ] = histories;
		//      }
		//    }
//
		private static History getHistory( string instrumentKey , QuoteField quoteField )
		{
			if ( ( !cachedHistories.Contains( instrumentKey ) ) ||
			    ( !((Hashtable)cachedHistories[ instrumentKey ]).Contains( quoteField ) ) )
			{
				Add( instrumentKey , quoteField );
				((Hashtable)cachedHistories[ instrumentKey ])[ quoteField ] =
					DataBase.GetHistory( instrumentKey , quoteField );
			}
			return (History)((Hashtable)cachedHistories[ instrumentKey ])[ quoteField ];
		}

		private static History getHistory( string ticker , QuoteField quoteField ,
		                                  DateTime firstDate , DateTime lastDate )
		{
			History returnValue = new History();
			DateTime currentDate = firstDate;
			while ( currentDate <= lastDate )
			{
				returnValue.Add( currentDate , privateCache.GetQuote(
					ticker , currentDate , quoteField ) );
				currentDate = currentDate.AddDays( 1 );
			}
			return returnValue;
		}

		public static History GetOpenHistory( string instrumentKey )
		{
			return getHistory( instrumentKey , QuoteField.Open );
		}

		public static History GetCloseHistory( string instrumentKey )
		{
			return getHistory( instrumentKey , QuoteField.Close );
		}

		public static History GetHighHistory( string instrumentKey )
		{
			return getHistory( instrumentKey , QuoteField.High );
		}

		public static History GetLowHistory( string instrumentKey )
		{
			return getHistory( instrumentKey , QuoteField.Low );
		}

		public static History GetAdjustedCloseHistory( string instrumentKey )
		{
			return getHistory( instrumentKey , QuoteField.AdjustedClose );
		}

		public static History GetAdjustedCloseHistory( string ticker ,
		                                              DateTime firstDate , DateTime lastDate )
		{
			return getHistory( ticker , QuoteField.AdjustedClose ,
			                  firstDate , lastDate );
		}

		private static History cache_getHistory( string instrumentKey , QuoteField quoteField )
		{
			History returnValue;
			if ( ( MinDate != DateTime.MinValue ) &&
			    ( MaxDate != DateTime.MaxValue ) )
				// the script has set a min date value and a max date value for the historical quotes
				returnValue = DataBase.GetHistory( instrumentKey , quoteField ,
				                                  MinDate , MaxDate );
			else
				// the script has NOT set a min date value and a max date value for the historical quotes
				returnValue = DataBase.GetHistory( instrumentKey , quoteField );
			return returnValue;
		}

		private static void cache( string instrumentKey , QuoteField quoteField )
		{
			if ( !cachedHistories.ContainsKey( instrumentKey ) )
				// no component at all for the instrument instrumentKey has been cached yet
				cachedHistories.Add( instrumentKey , new Hashtable() );
			History quoteHistory = cache_getHistory( instrumentKey , quoteField );
			((Hashtable)cachedHistories[ instrumentKey ]).Add( quoteField ,
			                                                  quoteHistory );
		}
		
		#region GetAdjustedMarketValue
		private static double getAdjustedMarketValue_actually(
			string instrumentKey , DateTime date ,
			MarketStatusSwitch  marketStatusSwitch )
		{
			double adjustedMarketValue;
			double adjustedClose = privateCache.GetQuote(
				instrumentKey ,	date , QuoteField.AdjustedClose );
			if ( marketStatusSwitch == MarketStatusSwitch.Close )
				// the requested quote is at market close
				adjustedMarketValue = adjustedClose;
			else
			{
				// the requested quote is at market open
				double rawOpen = privateCache.GetQuote(
					instrumentKey , date , QuoteField.Open );
				double rawClose = privateCache.GetQuote(
					instrumentKey , date , QuoteField.Close );
				adjustedMarketValue = rawOpen * adjustedClose / rawClose;
			}
			return adjustedMarketValue;
			
			
//			double rawOpen = privateCache.GetQuote(
//				instrumentKey , date , QuoteField.Open );
//			double rawClose = privateCache.GetQuote( instrumentKey ,
//			                                        extendedDateTime.DateTime , QuoteField.Close );
//			double returnValue =
//				rawOpen * adjustedClose / rawClose;
			
//			if ( extendedDateTime.BarComponent == BarComponent.Close )
//				returnValue = adjustedClose;
//			else
//				// extendedDateTime.BarComponent is equal to BarComponent.Open
//			{
//				double open = privateCache.GetQuote( instrumentKey ,
//				                                    extendedDateTime.DateTime , QuoteField.Open );
//				double close = privateCache.GetQuote( instrumentKey ,
//				                                     extendedDateTime.DateTime , QuoteField.Close );
//				returnValue = open * adjustedClose / close;
//			}
		}
		/// <summary>
		/// Returns the adjusted market value for the given ticker,
		/// at the given DateTime
		/// </summary>
		/// <param name="instrumentKey">instrument identifier</param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static double GetAdjustedMarketValue(
			string instrumentKey , DateTime date ,
			MarketStatusSwitch marketStatusSwitch )
		{
			HistoricalQuotesProvider.checkIfIsDate( date );
			double returnValue =
				HistoricalQuotesProvider.getAdjustedMarketValue_actually(
					instrumentKey , date , marketStatusSwitch );
			return returnValue;
		}
		#endregion GetAdjustedMarketValue
		
		#region GetRawMarketValue
		private static double getRawMarketValue_actually(
			string ticker , DateTime date ,
			MarketStatusSwitch marketStatusSwitch )
		{
			double rawMarketValue;
			if ( marketStatusSwitch == MarketStatusSwitch.Open )
				// the requested quote is at market open
				rawMarketValue = privateCache.GetQuote(
					ticker , date , QuoteField.Open );
			else
				// the requested quote is at market close
				rawMarketValue = privateCache.GetQuote(
					ticker , date , QuoteField.Close );
			return rawMarketValue;
		}
		/// <summary>
		/// Returns the raw market value for the given ticker,
		/// at the given DateTime
		/// </summary>
		/// <param name="ticker">instrument identifier</param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static double GetRawMarketValue(
			string ticker , DateTime date ,
			MarketStatusSwitch marketStatusSwitch )
		{
			HistoricalQuotesProvider.checkIfIsDate( date );
			double rawMarketValue =
				HistoricalQuotesProvider.getRawMarketValue_actually(
					ticker , date , marketStatusSwitch );
			return rawMarketValue;
		}
		#endregion GetRawMarketValue
		
//		public static bool WasExchanged( string instrumentKey , ExtendedDateTime extendedDateTime )
//		{
//			ExtendedDateTime atClose = new ExtendedDateTime(
//				extendedDateTime.DateTime , BarComponent.Close );
//			double marketValue = GetRawMarketValue( instrumentKey ,
//				atClose ); // forces caching if needed
//			History instrumentQuotes =
//				(History)((Hashtable)cachedHistories[ instrumentKey ])[ QuoteField.Close ];
//			bool returnValue = instrumentQuotes.ContainsKey( extendedDateTime.DateTime );
//			return returnValue;
//		}
		public static bool WasExchanged(
			string ticker , DateTime dateTime )
		{
			return privateCache.WasExchanged( ticker , dateTime );
		}

		/// <summary>
		/// returns the quotes DataTable for the given ticker
		/// </summary>
		/// <param name="instrumentKey">ticker whose quotes are to be returned</param>
		/// <returns></returns>
		public static DataTable GetTickerQuotes( string instrumentKey )
		{
			return QuantProject.DataAccess.Tables.Quotes.GetTickerQuotes(
				instrumentKey );
		}
		
		#region GetQuotes
		#region getQuotes_actually
		private static QuantProject.Data.DataTables.Quotes
			getQuotesDataTable(
				ICollection tickerCollection , DateTime dateTime )
		{
			DateTime date = ExtendedDateTime.GetDate( dateTime );
			QuantProject.Data.DataTables.Quotes dtQuotes =
				new QuantProject.Data.DataTables.Quotes(
					tickerCollection , date );
			return dtQuotes;
		}
		private static double getQuotes_getAdjustedQuote(
			DataRow dataRow ,
			DateTime date ,
			MarketStatusSwitch marketStatusSwitch )
		{
			double returnValue;
			/// TO DO: evaluate to use a Quote class, that derives from the DataRow class
			/// and use properties instead of dataRow accessing
			if ( marketStatusSwitch == MarketStatusSwitch.Close )
				// the requested quote is at market close
				returnValue = (double)dataRow[ QuantProject.Data.DataTables.Quotes.AdjustedClose ];
			else
				// the requested quote is at market open
				returnValue = (double)dataRow[ QuantProject.Data.DataTables.Quotes.Open ] *
					(double)dataRow[ QuantProject.Data.DataTables.Quotes.AdjustedClose ] /
					(double)dataRow[ QuantProject.Data.DataTables.Quotes.Close ];
			return returnValue;
		}
		private static Hashtable getAdjustedQuotes_actually(
			ICollection tickerCollection ,
			DateTime date ,
			MarketStatusSwitch marketStatusSwitch )
		{
			QuantProject.Data.DataTables.Quotes dtQuotes =
				HistoricalQuotesProvider.getQuotesDataTable(
					tickerCollection , date );
			Hashtable returnValue = new Hashtable();
			foreach (DataRow dataRow in dtQuotes.Rows)
				returnValue.Add(
					dataRow[
						QuantProject.Data.DataTables.Quotes.TickerFieldName ] ,
					HistoricalQuotesProvider.getQuotes_getAdjustedQuote(
						dataRow , date , marketStatusSwitch ) );
			return returnValue;
		}
		#endregion getQuotes_actually
		
		/// <summary>
		/// Returns the hashtable containing, for each ticker, the
		/// quote for the given extended date time
		/// </summary>
		/// <param name="tickerCollection">List of tickers whose quotes are to be fetched</param>
		/// <param name="extendedDateTime"></param>
		/// <returns></returns>
		public static Hashtable GetAdjustedQuotes(
			ICollection tickerCollection ,
			DateTime date ,
			MarketStatusSwitch marketStatusSwitch )
		{
			HistoricalQuotesProvider.checkIfIsDate( date );
			Hashtable htQuotes =
				HistoricalQuotesProvider.getAdjustedQuotes_actually(
					tickerCollection , date , marketStatusSwitch );
			return htQuotes;
		}
		#endregion GetQuotes
	}
}
