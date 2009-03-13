/*
QuantProject - Quantitative Finance Library

HistoricalBarInterpolatingProvider.cs
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
using System.Data;

using QuantProject.ADT.Timing;
using QuantProject.Data.DataTables;
using QuantProject.Data.DataProviders.Bars;
using QuantProject.Data.DataProviders.Bars.Caching;

namespace QuantProject.Business.DataProviders
{
	/// <summary>
	/// Returns historical bars. When a bar is missing for a given time,
	/// an interpolated value is returned (just the average of previous market value
	/// and next market value). If only previous or next is available, the
	/// available value is returned
	/// </summary>
	[Serializable]
	public class HistoricalBarInterpolatingProvider : HistoricalMarketValueProvider
	{
		protected QuantProject.Data.DataProviders.Bars.HistoricalBarProvider
			historicalBarProvider;
		protected DataTable barsForDayForTicker;
		protected DateTime currentDate;
		protected string currentTicker;
		
		public HistoricalBarInterpolatingProvider( IBarCache barCache )
		{
			this.historicalBarProvider =
				new QuantProject.Data.DataProviders.Bars.HistoricalBarProvider(
					barCache );
		}
		
		#region oldImplementation getValuesOfPrevious Next
//		private double getMarketValue_getValueOfTheFirstNextMinuteAvailable(string ticker , 
//		                                               DateTime dateTime )
//		{
//			double marketValue = double.MinValue;
//			DateTime nextDateTime = dateTime.AddMinutes(1);
//			DateTime closingTime = 
//				Time.GetDateTimeFromMerge( dateTime, new Time(16,0,0) );
//			while( nextDateTime.CompareTo(closingTime) <= 0 && 
//			       marketValue == double.MinValue )
//			{
//				if( this.historicalBarProvider.WasExchanged(ticker, nextDateTime) )
//					marketValue = this.historicalBarProvider.GetMarketValue(ticker, nextDateTime);
//				else
//					nextDateTime = nextDateTime.AddMinutes(1);
//			}
//			return marketValue;
//		}
//		
//		private double getMarketValue_getValueOfTheFirstPreviousMinuteAvailable(string ticker , 
//		                                               DateTime dateTime )
//		{
//			double marketValue = double.MinValue;
//			DateTime previousDateTime = dateTime.AddMinutes(-1);
//			DateTime openingTime = 
//				Time.GetDateTimeFromMerge( dateTime, new Time(9,30,0) );
//			while( previousDateTime.CompareTo(openingTime) >= 0 && 
//			       marketValue == double.MinValue )
//			{
//				if( this.historicalBarProvider.WasExchanged(ticker, previousDateTime) )
//					marketValue = this.historicalBarProvider.GetMarketValue(ticker, previousDateTime);
//				else
//					previousDateTime = previousDateTime.AddMinutes(-1);
//			}
//			return marketValue;
//		}
		#endregion
		
		private double getInterpolatedValue_getValueOfTheFirstNextMinuteAvailable(string ticker , 
		                                               DateTime dateTime )
		{
			double returnValue = double.MinValue;
			Bars barsFromNextMinuteOfDateTimeToClose =
				new Bars(ticker, dateTime, dateTime, 
				         new Time(dateTime).AddMinutes(1),
				         new Time(16,0,0), 60);
			if( barsFromNextMinuteOfDateTimeToClose.Rows.Count > 0 )
				returnValue = 
					barsFromNextMinuteOfDateTimeToClose.GetFirstValidOpen(dateTime);
			return returnValue;
		}
		
		private double getInterpolatedValue_getValueOfTheFirstPreviousMinuteAvailable(string ticker , 
		                                               DateTime dateTime )
		{
			double returnValue = double.MinValue;
			Bars barsFromOpenToPrecedingMinuteOfDateTime =
				new Bars(ticker, dateTime, dateTime,
				         new Time(9,30,0),
				         new Time(dateTime).AddMinutes(-1), 60);
			if( barsFromOpenToPrecedingMinuteOfDateTime.Rows.Count > 0 )
				returnValue = 
					barsFromOpenToPrecedingMinuteOfDateTime.GetFirstValidOpen(dateTime);
			return returnValue;
		}
		
		private double getInterpolatedValue_getInterpolatedOrThePreviousOrTheNextValue(double previousMarketValue,
		                                                                      double nextMarketValue)
		{
			double returnValue = 0.0;
			if(previousMarketValue == double.MinValue && 
			   nextMarketValue != double.MinValue)
					returnValue = nextMarketValue;
			else if(previousMarketValue != double.MinValue && 
			        nextMarketValue == double.MinValue)
					returnValue = previousMarketValue;
			else if(previousMarketValue != double.MinValue && 
			        nextMarketValue != double.MinValue)
					returnValue = (previousMarketValue + nextMarketValue)/2;
			return returnValue;
		}
		
		protected virtual double getInterpolatedValue(string ticker ,
																								  DateTime dateTime)
		{
			double previousMarketValue =
					this.getInterpolatedValue_getValueOfTheFirstPreviousMinuteAvailable(ticker , dateTime);
			double nextMarketValue = 
					this.getInterpolatedValue_getValueOfTheFirstNextMinuteAvailable(ticker , dateTime);
			return
					this.getInterpolatedValue_getInterpolatedOrThePreviousOrTheNextValue(previousMarketValue, nextMarketValue);
		}
		
		public override double GetMarketValue(
			string ticker ,
			DateTime dateTime )
		{
			double marketValue = 0.0;
			if ( this.historicalBarProvider.WasExchanged( ticker , dateTime ) )
				marketValue = this.historicalBarProvider.GetMarketValue(
					ticker , dateTime );
			else
			{
				marketValue = this.getInterpolatedValue(ticker, dateTime);
			}
			return marketValue;
		}
		
		protected override string getDescription()
		{
			return "barInterpolatingProvider";
		}
		
		private void wasExchanged_setBarsForDayForTickerIfNecessary(string ticker, DateTime dateTime)
		{
			if( this.currentTicker != ticker || this.currentDate != dateTime )
			{	
				this.currentDate = dateTime;
				this.currentTicker = ticker;
				this.barsForDayForTicker =
					QuantProject.DataAccess.Tables.Bars.GetTickerBars(ticker,
                                  Time.GetDateTimeFromMerge(dateTime, new Time(9,30,0)),
                                  Time.GetDateTimeFromMerge(dateTime, new Time(16,0,0)),
                                  60 );
			}
		}
		//it returns true just if there is a 
		//bar for the day in dateTime
		public override bool WasExchanged(string ticker, DateTime dateTime)
		{
			bool returnValue = false;
			this.wasExchanged_setBarsForDayForTickerIfNecessary(ticker, dateTime);
			if( this.barsForDayForTicker.Rows.Count > 0 )
				returnValue = true;
			return returnValue;
		}
	}
}
