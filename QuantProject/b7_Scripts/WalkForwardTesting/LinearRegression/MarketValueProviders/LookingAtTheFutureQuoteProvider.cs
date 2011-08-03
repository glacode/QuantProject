/*
QuantProject - Quantitative Finance Library

LookingAtTheFutureQuoteProvider.cs
Copyright (C) 2011
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

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// tickerWithFutureQuotes will have the same quote as tickerWithRealQuotes
	/// will have tomorrow; then tickerWithFutureQuotes can be used as a perfect predictor
	/// </summary>
	[Serializable]
	public class LookingAtTheFutureQuoteProvider : HistoricalQuoteProvider
	{
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		private string tickerWithRealQuotes;
		private string tickerWithFutureQuotes;
		
		/// <summary>
		/// tickerWithFutureQuotes will have the same quote as tickerWithRealQuotes
		/// will have tomorrow; then tickerWithFutureQuotes can be used as a perfect predictor
		/// </summary>
		/// <param name="tickerWithRealQuotes"></param>
		/// <param name="tickerWithFutureQuotes"></param>
		public LookingAtTheFutureQuoteProvider(
			HistoricalMarketValueProvider historicalMarketValueProvider ,
			string tickerWithRealQuotes , string tickerWithFutureQuotes )
		{
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.tickerWithRealQuotes = tickerWithRealQuotes;
			this.tickerWithFutureQuotes = tickerWithFutureQuotes;
		}
		
		protected override string getDescription()
		{
			return "lookingAtTheFuture";
		}
		
		#region GetMarketValue
		private double getFutureMarketValue( DateTime dateTime )
		{
			double futureMarketValue = double.MinValue;
			DateTime futureDateTime = dateTime.AddDays( 1 );
			while ( ( futureMarketValue == double.MinValue ) && ( futureDateTime < dateTime.AddDays( 10 ) ) )
			{
				if ( this.historicalMarketValueProvider.WasExchanged(
					this.tickerWithRealQuotes , futureDateTime ) )
					// the ticker with real quotes was exchanged in the future DateTime
					futureMarketValue = this.historicalMarketValueProvider.GetMarketValue(
						this.tickerWithRealQuotes , futureDateTime );
				futureDateTime = futureDateTime.AddDays( 1 );
			}
			return futureMarketValue;
		}
		public override double GetMarketValue( string ticker , DateTime dateTime )
		{
			double marketValue = double.MinValue;
			
			if ( ticker != this.tickerWithFutureQuotes )
				marketValue = this.historicalMarketValueProvider.GetMarketValue( ticker , dateTime );
			else
				marketValue = this.getFutureMarketValue( dateTime );
			return marketValue;
		}
		#endregion GetMarketValue
	}
}
