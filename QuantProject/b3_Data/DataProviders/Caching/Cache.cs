/*
QuantProject - Quantitative Finance Library

Cache.cs
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
using QuantProject.ADT.Histories;
using QuantProject.DataAccess;

namespace QuantProject.Data.DataProviders.Caching
{
	/// <summary>
	/// Handles quote cashing, from mass storage to main memory
	/// </summary>
	public class Cache : Hashtable,ICache
	{
		/// <summary>
		/// number of pages currently cached
		/// </summary>
		private int currentNumPages;

		private int maxNumPages;

		/// <summary>
		/// rank for the page where it has been peformed the last GetQuote()
		/// </summary>
		private long currentPageRank;

		public int MaxNumPages
		{
			get { return this.maxNumPages; }
			set { this.maxNumPages = value; }
		}
		private int pagesToBeRemovedWithGarbageCollection;

		public int PagesToBeRemovedWithGarbageCollection
		{
			get { return this.pagesToBeRemovedWithGarbageCollection; }
			set { this.pagesToBeRemovedWithGarbageCollection = value; }
		}

		public Cache()
		{
			this.currentNumPages = 0;
			this.currentPageRank = 0;
			this.maxNumPages = ConstantsProvider.CachePages;
			this.pagesToBeRemovedWithGarbageCollection =
				ConstantsProvider.PagesToBeRemovedFromCache;
		}
//		private Hashtable getQuotes( string ticker )
//		{
//			return ( Hashtable )this[ ticker ];
//		}
//		private Hashtable getQuotes( string ticker , int year )
//		{
//			return ( Hashtable )(this.getQuotes( ticker )[ year ] );
//		}
//		private Hashtable getQuotes( string ticker , DateTime dateTime )
//		{
//			return this.getQuotes( ticker , dateTime.Year );
//		}

		private string getKey( string ticker , int year , QuoteField quoteField )
		{
			string key1 = ticker;
			string key2 = year.ToString();
			string key3 = quoteField.ToString();
			string returnValue = key1 + ";" + key2 + ";" + key3;
			return returnValue;
		}
		private CachePage getCachePage( string ticker , int year ,
			QuoteField quoteField )
		{
			return (CachePage)( this[ this.getKey( ticker , year , quoteField ) ] );
		}
		private CachePage getCachePage( string ticker , DateTime dateTime ,
			QuoteField quoteField )
		{
			return this.getCachePage( ticker , dateTime.Year , quoteField );
		}
		#region removeUnusedPages
		private long removeUnusedPages_getMinPageRankToKeep()
		{
			ArrayList arrayList = new ArrayList();
			foreach ( string key in this.Keys )
				arrayList.Add( ((CachePage)this[ key ]).Rank );
			arrayList.Sort();
			return (long)arrayList[ this.pagesToBeRemovedWithGarbageCollection ];
		}
//		private void removeCachePage( string ticker , int year ,
//			QuoteField quoteField )
//		{
//			CachePage cachePage = this.getCachePage( ticker , year , quoteField );
//			cachePage.Clear();
//			this.Remove( this.getKey( ticker , year , quoteField ) );
//			this.getQuotes( ticker , year ).Remove( quoteField );
//			this.currentNumPages -- ;
//		}
		private void removeUnusedPages()
		{
			long minPageRankToKeep = removeUnusedPages_getMinPageRankToKeep();
			ArrayList keysToBeRemoved = new ArrayList();
			foreach ( string key in this.Keys )
			{
				CachePage cachePage = ( CachePage )this[ key ];
				if ( cachePage.Rank < minPageRankToKeep )
					// the current cache page has not been requested recently
					keysToBeRemoved.Add( key );
			}
			foreach ( string key in keysToBeRemoved )
				this.Remove( key );
		}
		#endregion
//		private void getQuote_checkEarlyDateException( DateTime dateTime )
//		{
//			if ( dateTime < ConstantsProvider.MinQuoteDateTime )
//				throw new EarlyDateException( dateTime );
//		}
		#region addPage
//		private void addTicker( string ticker )
//		{
//			if ( !this.ContainsKey( ticker ) )
//			{
//				this.Add( ticker , new Hashtable() );
//			}
//		}
//		private void addYear( string ticker , int year )
//		{
//			Hashtable quotesForTicker = this.getQuotes( ticker );
//			if ( !quotesForTicker.ContainsKey( year ) )
//			{
//				quotesForTicker.Add( year , new Hashtable() );
//			}
//		}
//		private void addCachePage( string ticker , int year , QuoteField quoteField )
//		{
//			Hashtable quotesForTickerAndYear = this.getQuotes( ticker , year );
//			if ( !quotesForTickerAndYear.ContainsKey( quoteField ) )
//			{
//				CachePage cachePage = new CachePage( ticker , year , quoteField );
//				cachePage.LoadData();
//				quotesForTickerAndYear.Add( quoteField , cachePage );
//			}
//		}
		private void addPage_actually( string ticker , int year , QuoteField quoteField )
		{
			if ( this.Count + 1 > this.maxNumPages )
				this.removeUnusedPages();
			CachePage cachePage = new CachePage( ticker , year , quoteField );
			cachePage.LoadData();
			this.Add( this.getKey( ticker , year , quoteField ) , cachePage );
			this.currentNumPages ++ ;
		}
		private void addPage( string ticker , int year , QuoteField quoteField )
		{
			if ( !this.ContainsKey( this.getKey( ticker , year , quoteField ) ) )
				// ticker quotes have not been cached yet,
				// for the given year
				this.addPage_actually( ticker , year , quoteField );
		}
		private void addPage( string ticker , DateTime dateTime , QuoteField quoteField )
		{
			this.addPage( ticker , dateTime.Year , quoteField );
		}
		#endregion
		public double GetQuote( string ticker , DateTime dateTime , QuoteField quoteField )
		{
			double returnValue;
			//			this.getQuote_checkEarlyDateException( dateTime );
			this.addPage( ticker , dateTime , quoteField );
			try
			{
				CachePage cachePage = this.getCachePage( ticker , dateTime , quoteField );
				if ( cachePage.Quotes.Count == 0 )
				{
					this.addPage( ticker , dateTime.Year - 1 , quoteField );
					cachePage = this.getCachePage( ticker , dateTime.Year - 1 , quoteField );
					if ( cachePage.Quotes.Count == 0 )
						// ticker has no quotes both in the dateTime year and in the
						// previous year
						throw new MissingQuoteException( ticker , dateTime );
				}

				if ( cachePage.Rank != this.currentPageRank )
				{
					this.currentPageRank ++ ;
					cachePage.Rank = this.currentPageRank;
				}
				returnValue = Convert.ToDouble(
					cachePage.Quotes.GetByKeyOrPrevious( dateTime ) );
			}
			catch ( IndexOfKeyOrPreviousException ex )
			{
				// the given date is at the beginning of the year and no
				// date is given for such date
				string message = ex.Message; // to avoid warning
				returnValue = this.GetQuote( ticker ,
					new DateTime( dateTime.Year - 1 , 12 , 31 ) , quoteField );
			}
			return returnValue;
		}
		public bool WasExchanged( string ticker , ExtendedDateTime extendedDateTime )
		{
			bool returnValue;
			// forces quote caching
			this.GetQuote( ticker , extendedDateTime.DateTime , QuoteField.Open );
			if ( !((CachePage)this[ this.getKey( ticker , extendedDateTime.DateTime.Year ,
				QuoteField.Open ) ] ).Quotes.ContainsKey( extendedDateTime.DateTime ) )
				// the ticker was not exchanged at the given date
				returnValue = false;
			else
				// the ticker was exchanged at the given date
				returnValue = true;
			return returnValue;
		}
	}
}
