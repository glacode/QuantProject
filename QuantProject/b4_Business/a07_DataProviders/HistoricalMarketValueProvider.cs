/*
QuantProject - Quantitative Finance Library

HistoricalMarketValueProvider.cs
Copyright (C) 2007
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
using System.Collections.Generic;

using QuantProject.ADT.Histories;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders.Quotes;

namespace QuantProject.Business.DataProviders
{
	/// <summary>
	/// Abstract base class for historical market values providers
	/// </summary>
	[Serializable]
	public abstract class HistoricalMarketValueProvider :
		IHistoricalMarketValueProvider , ILogDescriptor
	{
		public string Description
		{
			get
			{
				return "QtPrvdr_" + this.getDescription();
			}
		}

		public HistoricalMarketValueProvider()
		{
		}

		protected abstract string getDescription();

		public abstract double GetMarketValue(
			string ticker ,
			DateTime dateTime );

		/// <summary>
		/// True iif the given ticker was traded at the given DateTime
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
//		public bool WasExchanged( string ticker ,
//		                         DateTime dateTime )
//		{
//			bool wasExchanged =
//				HistoricalQuotesProvider.WasExchanged( ticker , dateTime );
//			return wasExchanged;
//		}
		
		/// <summary>
		/// True iif the given ticker was traded at the given DateTime
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public abstract bool WasExchanged( string ticker , DateTime dateTime );
		
		/// <summary>
		/// True iif all the given tickers were traded at the given DateTime
		/// </summary>
		/// <param name="tickers"></param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public virtual bool WereAllExchanged( ICollection<string> tickers , DateTime dateTime )
		{
			IEnumerator<string>	tickersEnumerator = tickers.GetEnumerator();
			tickersEnumerator.Reset();
			bool wereAllExchanged = true;
			bool isEndOfCollection = !tickersEnumerator.MoveNext();
			while ( wereAllExchanged && !isEndOfCollection )
			{
				wereAllExchanged = this.WasExchanged( tickersEnumerator.Current , dateTime );
				isEndOfCollection = !tickersEnumerator.MoveNext();
			}
			return wereAllExchanged;
		}
		
		#region GetQuotes
		
		#region addQuoteActually
		private double getMarketValue( string ticker , DateTime dateTime )
		{
			double marketValue =
				this.GetMarketValue( ticker , dateTime );
			return marketValue;
		}
		private void addQuoteActually( string ticker ,
		                              DateTime dateTime ,
		                              History quotes )
		{
			double marketValue = this.getMarketValue( ticker , dateTime );
			quotes.Add( dateTime , marketValue );
		}
		#endregion addQuoteActually
		
		private void addMarketValue( string ticker , int historyIndex ,
		                            History	history , History quotes )
		{
			DateTime currentDateTime =
				(DateTime)history.GetByIndex( historyIndex );
			if ( this.WasExchanged( ticker , currentDateTime ) )
			{
				this.addQuoteActually( ticker , currentDateTime ,
				                      quotes );
			}
			else
				throw new TickerNotExchangedException(
					ticker , currentDateTime );
		}
		public History GetMarketValues( string ticker ,
		                               History history )
		{
			History quotes = new History();
			for ( int i = 0 ; i < history.Count ; i++ )
				this.addMarketValue( ticker , i , history , quotes );
			return quotes;
		}
		#endregion GetQuotes
		
		#region GetDateTimesWithMarketValues
		/// <summary>
		/// returns the subset of DateTimes when ticker is exchanged
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="history">DateTimes returned are a subset of this parameter</param>
		/// <returns></returns>
		public History GetDateTimesWithMarketValues( string ticker , History history )
		{
			History dateTimesWithMarketValues = new History();
			foreach( DateTime dateTime in history.Keys )
				if ( this.WasExchanged( ticker , dateTime ) )
					dateTimesWithMarketValues.Add( dateTime , dateTime );
			return dateTimesWithMarketValues;
		}
		#endregion GetDateTimesWithMarketValues
	}
}
