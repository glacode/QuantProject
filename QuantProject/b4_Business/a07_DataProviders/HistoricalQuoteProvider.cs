/*
QuantProject - Quantitative Finance Library

HistoricalQuoteProvider.cs
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

using QuantProject.Business.Timing;

namespace QuantProject.Business.DataProviders
{
	/// <summary>
	/// Abstract base class for historical quotes providers
	/// </summary>
	[Serializable]
	public abstract class HistoricalQuoteProvider : IHistoricalQuoteProvider
	{
		public string Description
		{
			get
			{
				return "QtPrvdr_" + this.getDescription();
			} 
		}

		public HistoricalQuoteProvider()
		{
		}

		protected abstract string getDescription();

		public abstract double GetMarketValue( string ticker ,
			EndOfDayDateTime endOfDayDateTime );

		/// <summary>
		/// True iif the given ticker was traded at the given time
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="endOfDayDateTime"></param>
		/// <returns></returns>
		public bool WasExchanged( string ticker ,
			EndOfDayDateTime endOfDayDateTime )
		{
			bool wasExchanged =
				QuantProject.Data.DataProviders.HistoricalDataProvider.WasExchanged(
				ticker , endOfDayDateTime.GetNearestExtendedDateTime() );
			return wasExchanged;
		}
		#region GetEndOfDayQuotes
		private void addQuoteActually( string ticker ,
			EndOfDayDateTime endOfDayDateTime ,
			EndOfDayHistory endOfDayQuotes )
		{
			double quote = this.GetMarketValue( ticker ,
				endOfDayDateTime );
			endOfDayQuotes.Add( endOfDayDateTime , quote );
		}
		private void addQuote( string ticker , int historyIndex ,
			EndOfDayHistory	endOfDayHistory , EndOfDayHistory endOfDayQuotes )
		{
			EndOfDayDateTime currentEndOfDayDateTime =
				(EndOfDayDateTime)endOfDayHistory.GetByIndex( historyIndex );
			if ( this.WasExchanged( ticker , currentEndOfDayDateTime ) )
			{
				this.addQuoteActually( ticker , currentEndOfDayDateTime ,
					endOfDayQuotes );
			}
			else
				throw new TickerNotExchangedException(
					ticker , currentEndOfDayDateTime );
		}
		public EndOfDayHistory GetEndOfDayQuotes( string ticker ,
			EndOfDayHistory endOfDayHistory )
		{
			EndOfDayHistory endOfDayQuotes = new EndOfDayHistory();
			for ( int i = 0 ; i < endOfDayHistory.Count ; i++ )
				this.addQuote( ticker , i , endOfDayHistory , endOfDayQuotes );
			return endOfDayQuotes;
		}
		#endregion GetEndOfDayQuotes
	}
}
