/*
QuantProject - Quantitative Finance Library

HistoricalDataProvider.cs
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

namespace QuantProject.Data.DataProviders
{
	/// <summary>
	/// Static methods to access the historical data
	/// </summary>
	public class HistoricalDataProvider
	{
		private static Hashtable cachedHistories = new Hashtable();

		private static Cache privateCache = new Cache();


		private static DateTime minDate = DateTime.MinValue;
		private static DateTime maxDate = DateTime.MaxValue;

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

		public HistoricalDataProvider()
		{
			//
			// TODO: Add constructor logic here
			//
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
//		public static double GetMarketValue( string instrumentKey , ExtendedDateTime extendedDateTime )
//		{
//			double returnValue;
//			if ( !cachedHistories.ContainsKey( instrumentKey ) ||
//				!(((Hashtable)cachedHistories[ instrumentKey ]).ContainsKey(
//				extendedDateTime.QuoteField)) )
//				// the instrument instrumentKey has not been cached yet, for the given bar component
//				cache( instrumentKey , extendedDateTime.QuoteField );
//			returnValue = Convert.ToDouble(
//				( (History) ((Hashtable)
//				cachedHistories[ instrumentKey ])[ extendedDateTime.QuoteField ] ).GetByIndex(
//				( (History) ((Hashtable) cachedHistories[ instrumentKey ])[ extendedDateTime.QuoteField ]
//				).IndexOfKeyOrPrevious( extendedDateTime.DateTime ) ) );
//			return returnValue;
//		}
//		public static double GetMarketValue( string instrumentKey , DateTime dateTime ,
//			QuoteField barComponent )
//		{
//			//DateTime dateTime = 
//			return GetMarketValue( instrumentKey , new ExtendedDateTime( dateTime , barComponent ) );
//		}
//
		private static double getQuote( string instrumentKey , DateTime dateTime , QuoteField quoteField )
		{
			double returnValue;
			if ( !cachedHistories.ContainsKey( instrumentKey ) ||
				!(((Hashtable)cachedHistories[ instrumentKey ]).ContainsKey(
				quoteField )) )
				// the instrument instrumentKey has not been cached yet, for the given bar component
				cache( instrumentKey , quoteField );
			returnValue = Convert.ToDouble(
				( (History) ((Hashtable)
				cachedHistories[ instrumentKey ])[ quoteField ] ).GetByIndex(
				( (History) ((Hashtable) cachedHistories[ instrumentKey ])[ quoteField ]
				).IndexOfKeyOrPrevious( dateTime ) ) );
			return returnValue;
		}
		/// <summary>
		/// Returns the adjusted market value for the given ticker, at the given ExtendedDateTime
		/// </summary>
		/// <param name="instrumentKey">instrument identifier</param>
		/// <param name="extendedDateTime"></param>
		/// <returns></returns>
		public static double GetAdjustedMarketValue( string instrumentKey , ExtendedDateTime extendedDateTime )
		{
			double returnValue;
			double adjustedClose = privateCache.GetQuote( instrumentKey ,
				extendedDateTime.DateTime , QuoteField.AdjustedClose );
			if ( extendedDateTime.BarComponent == BarComponent.Close )
				returnValue = adjustedClose;
			else
				// extendedDateTime.BarComponent is equal to BarComponent.Open
			{
				double open = privateCache.GetQuote( instrumentKey ,
					extendedDateTime.DateTime , QuoteField.Open );
				double close = privateCache.GetQuote( instrumentKey ,
					extendedDateTime.DateTime , QuoteField.Close );
				returnValue = open * adjustedClose / close;
			}
			return returnValue;
		}
		/// <summary>
		/// Returns the raw market value for the given ticker, at the given ExtendedDateTime
		/// </summary>
		/// <param name="instrumentKey">instrument identifier</param>
		/// <param name="extendedDateTime"></param>
		/// <returns></returns>
		public static double GetRawMarketValue( string instrumentKey , ExtendedDateTime extendedDateTime )
		{
			double returnValue;
			if ( extendedDateTime.BarComponent == BarComponent.Close )
				returnValue = getQuote( instrumentKey ,
					extendedDateTime.DateTime , QuoteField.Close );
			else
				// extendedDateTime.BarComponent is equal to BarComponent.Open
				returnValue = getQuote( instrumentKey ,
					extendedDateTime.DateTime , QuoteField.Open );
			return returnValue;
		}
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
		public static bool WasExchanged( string ticker , ExtendedDateTime extendedDateTime )
		{
			return privateCache.WasExchanged( ticker , extendedDateTime );
		}

		/// <summary>
		/// returns the quotes DataTable for the given ticker
		/// </summary>
		/// <param name="instrumentKey">ticker whose quotes are to be returned</param>
		/// <returns></returns>
		public static DataTable GetTickerQuotes( string instrumentKey )
		{
			return Quotes.GetTickerQuotes( instrumentKey );
		}
		#region GetQuotes
		private static double getQuotes_getQuoteValue( DataRow dataRow , ExtendedDateTime extendedDateTime )
		{
			double returnValue;
			/// TO DO: evaluate to use a Quote class, that derives from the DataRow class
			/// and use properties instead of dataRow accessing
			if ( extendedDateTime.BarComponent == BarComponent.Open )
				returnValue = (double)dataRow[ QuantProject.Data.DataTables.Quotes.Open ] *
					(double)dataRow[ QuantProject.Data.DataTables.Quotes.AdjustedClose ] /
					(double)dataRow[ QuantProject.Data.DataTables.Quotes.Close ];
			else
				returnValue = (double)dataRow[ QuantProject.Data.DataTables.Quotes.AdjustedClose ];
			return returnValue;
		}
		/// <summary>
		/// Returns the hashtable containing, for each ticker, the
		/// quote for the given extended date time
		/// </summary>
		/// <param name="tickerCollection">List of tickers whose quotes are to be fetched</param>
		/// <param name="extendedDateTime"></param>
		/// <returns></returns>
		public static Hashtable GetQuotes( ICollection tickerCollection ,
			ExtendedDateTime extendedDateTime )
		{
			Hashtable returnValue = new Hashtable();
			QuantProject.Data.DataTables.Quotes quotes =
				new QuantProject.Data.DataTables.Quotes( tickerCollection ,extendedDateTime.DateTime );
			foreach (DataRow dataRow in quotes.Rows)
				returnValue.Add( dataRow[ QuantProject.Data.DataTables.Quotes.TickerFieldName ] ,
					getQuotes_getQuoteValue( dataRow , extendedDateTime ) );
			return returnValue;
		}
		#endregion
	}
}
