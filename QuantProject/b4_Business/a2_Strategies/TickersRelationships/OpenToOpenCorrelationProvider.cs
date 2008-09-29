/*
QuantProject - Quantitative Finance Library

OpenToOpenCorrelationProvider.cs
Copyright (C) 2008 
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
using System.Data;
using System.Collections;

using QuantProject.ADT.Statistics;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;

namespace QuantProject.Business.Strategies.TickersRelationships
{
  /// <summary>
  /// Class that provides open to open correlation's indexes two by two within a 
  /// given set of tickers
  /// </summary>
  [Serializable]
  public class OpenToOpenCorrelationProvider : CorrelationProvider
  {
		private int openToOpenIntervalLength;
  	
  	/// <summary>
		/// Creates the provider for the open to open correlation
		/// </summary>
		/// <param name="tickersToAnalyze">Array of tickers to be analyzed</param>
		/// <param name="startDate"></param>
		/// <param name="endDate"></param>
		/// <param name="openToOpenIntervalLength">Length of the open To open
		/// interval return</param>
		/// <param name="minimumAbsoluteReturnValue">Both current tickers' returns
		/// have to be greater than minimumAbsoluteReturnValue for being considered
		/// significant and so computed in the correlation formula</param>
		/// <param name="maximumAbsoluteReturnValue">Both current tickers' returns
		/// have to be less than maximumAbsoluteReturnValue</param>
		/// <param name="benchmark">The benchmark used for computation
		/// of returns</param>
    public OpenToOpenCorrelationProvider(string[] tickersToAnalyze,
					 DateTime startDate, DateTime endDate, int openToOpenIntervalLength,
					 float minimumAbsoluteReturnValue, float maximumAbsoluteReturnValue,
					 string benchmark) :
					 base(tickersToAnalyze,
					 startDate, endDate, openToOpenIntervalLength,
					 minimumAbsoluteReturnValue, maximumAbsoluteReturnValue,
					 benchmark)
    {
			this.openToOpenIntervalLength = openToOpenIntervalLength;						
    }

		/// <summary>
		/// Creates the provider for the open to open correlation
		/// </summary>
		/// <param name="tickersToAnalyze">Array of tickers to be analyzed</param>
		/// <param name="returnsManager"></param>
		/// <param name="minimumAbsoluteReturnValue">Both current tickers' returns
		/// have to be greater than minimumAbsoluteReturnValue for being considered
		/// significant and so computed in the correlation formula</param>
		/// <param name="maximumAbsoluteReturnValue">Both current tickers' returns
		/// have to be less than maximumAbsoluteReturnValue</param>
		public OpenToOpenCorrelationProvider( string[] tickersToAnalyze,
			ReturnsManager returnsManager,
			float minimumAbsoluteReturnValue, float maximumAbsoluteReturnValue ):
			base(tickersToAnalyze,
			returnsManager,
			minimumAbsoluteReturnValue, maximumAbsoluteReturnValue)
		{
			
		}
		
		protected override void setEndOfDayDatesTime(
			DateTime firstDate , DateTime lastDate )
		{
			this.firstDateTime =
				HistoricalEndOfDayTimer.GetMarketOpen( firstDate );
//				new EndOfDayDateTime(firstDate, EndOfDaySpecificTime.MarketOpen);
			this.lastDateTime = 
				HistoricalEndOfDayTimer.GetMarketOpen( lastDate );
//				new EndOfDayDateTime(lastDate, EndOfDaySpecificTime.MarketOpen);
		}
		
		protected override void setReturnsManager()
		{
			OpenToOpenIntervals openToOpenIntervals = 
				new OpenToOpenIntervals(this.firstDateTime, this.lastDateTime,
				                          this.benchmark, this.openToOpenIntervalLength);
			this.returnsManager = 
				new ReturnsManager(openToOpenIntervals, 
													 new HistoricalAdjustedQuoteProvider() );
		}
  } // end of class
}
