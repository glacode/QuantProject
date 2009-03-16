/*
QuantProject - Quantitative Finance Library

FixedPreviousDateAtClose.cs
Copyright (C) 2009
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

using QuantProject.ADT.Timing;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// The interval begin is fixed, meaning that for every request the same
	/// interval begin is computed; the interval begin that's returned has the following features:
	/// - both firstTicker and secondTicker have been exchanged at that time
	/// - the returned date time is equal or before maxAllowedDateTime
	/// - an effort is made return a date time that is as close as possible to maxAllowedDateTime
	/// </summary>
	public class FixedPreviousDateAtClose : IIntervalBeginFinder
	{
		DateTime intervalBegin;
		HistoricalMarketValueProvider historicalMarketValueProvider;
		
		public FixedPreviousDateAtClose(
			string firstTicker , string secondTicker , DateTime maxAllowedDateTime ,
			HistoricalMarketValueProvider historicalMarketValueProvider )
		{
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.intervalBegin = this.findIntervalBegin(
				firstTicker , secondTicker , maxAllowedDateTime );
		}
		
		#region findIntervalBegin
		private void checkForLoop( Date nextDateToTry , DateTime maxAllowedDateTime )
		{
			DateTime dateTimeToBeSubtracted = new DateTime(
				nextDateToTry.Year , nextDateToTry.Month , nextDateToTry.Day , 0 , 0 , 0 );
			TimeSpan timeSpan = maxAllowedDateTime.Subtract( dateTimeToBeSubtracted );
			if ( timeSpan.Days >= 15 )
				throw new Exception(
					"No eligible date time has been found in the previous 15 days. " +
					"You might want to check maxAllowedDateTime" );
		}
		
		#region tryThisDate
		
		#region areExchangedInTheDay
		
		#region isExchanged
		
		#region getDateTimesToTry
		private Time getFirstTimeToTry( Date dateToTry , DateTime maxAllowedDateTime )
		{
			Time firstTimeToTry = new Time( maxAllowedDateTime );
			Date dateForMaxAllowedDateTime = new Date( maxAllowedDateTime );
			if ( dateToTry < dateForMaxAllowedDateTime )
				// the first tried date had no quotes for both tickers; we are now
				// examining earlier days
				firstTimeToTry = new Time( 15 , 59 , 0 );
			return firstTimeToTry;
		}
		private void addForCurrentTime(
			Time currentTime , Date dateToTry , List<DateTime> dateTimesToTry )
		{
			DateTime dateTime = new DateTime(
				dateToTry.Year , dateToTry.Month , dateToTry.Day ,
				currentTime.Hour , currentTime.Minute , currentTime.Second );
			dateTimesToTry.Add( dateTime );
		}
		private List<DateTime> getDateTimesToTry( Date dateToTry , DateTime maxAllowedDateTime )
		{
			List<DateTime> dateTimesToTry = new List<DateTime>();
			Time currentTime = this.getFirstTimeToTry( dateToTry , maxAllowedDateTime );
			while ( currentTime >= HistoricalEndOfDayTimer.TimeForMarketOpen )
			{
				this.addForCurrentTime( currentTime , dateToTry , dateTimesToTry );
				currentTime = currentTime.AddMinutes( -29 );
			}
			return dateTimesToTry;
		}
		#endregion getDateTimesToTry
		
		#region isExchanged
		private bool isExchanged( string ticker , List<DateTime> dateTimesToTry )
		{
			bool isExchangedVerified = false;
			int currentDateTimeToTryIndex = 0;
			while ( !isExchangedVerified && ( currentDateTimeToTryIndex < dateTimesToTry.Count ) )
			{
				DateTime dateTimeToTry = dateTimesToTry[ currentDateTimeToTryIndex ];
				isExchangedVerified =
					this.historicalMarketValueProvider.WasExchanged( ticker , dateTimeToTry );
				currentDateTimeToTryIndex++;
			}
			return isExchangedVerified;
		}
		#endregion isExchanged
		
		private bool isExchanged(
			string ticker , Date dateToTry , DateTime maxAllowedDateTime )
		{
			List<DateTime> dateTimesToTry = this.getDateTimesToTry( dateToTry , maxAllowedDateTime );
			bool isActuallyExchanged = this.isExchanged( ticker , dateTimesToTry );
			return isActuallyExchanged;
		}
		#endregion isExchanged
		
		private bool areExchangedInTheDay(
			string firstTicker , string secondTicker ,
			Date dateToTry , DateTime maxAllowedDateTime )
		{
			bool areExchanged =
				this.isExchanged( firstTicker , dateToTry , maxAllowedDateTime ) &&
				this.isExchanged( secondTicker , dateToTry , maxAllowedDateTime );
			return areExchanged;
		}
		#endregion areExchangedInTheDay
		
		#region findTheLatestCommonDateTime
		private DateTime getFirstDateTimeToTry(
			Date dateToTry , DateTime maxAllowedDateTime )
		{
			DateTime firstDateTimeToTry = new DateTime(
				dateToTry.Year , dateToTry.Month , dateToTry.Day , 15 , 59 , 0 );
			if ( maxAllowedDateTime < firstDateTimeToTry )
				firstDateTimeToTry = maxAllowedDateTime;
			return firstDateTimeToTry;
		}
		private DateTime findTheLatestCommonDateTime(
			string firstTicker , string secondTicker ,
			Date dateToTry , DateTime maxAllowedDateTime )
		{
			DateTime latestCommonDateTime = DateTime.MinValue;
			DateTime dateTimeToTry = this.getFirstDateTimeToTry( dateToTry , maxAllowedDateTime );
			while ( latestCommonDateTime == DateTime.MinValue )
			{
				if
					(
						this.historicalMarketValueProvider.WasExchanged( firstTicker , dateTimeToTry )
						&&
						this.historicalMarketValueProvider.WasExchanged( secondTicker , dateTimeToTry )
					)
					// both firstTicker and secondTicker are exchanged at dateTimeToTry
					latestCommonDateTime = dateTimeToTry;
				dateTimeToTry = dateTimeToTry.AddMinutes( -1 );
			}
			return latestCommonDateTime;
		}
		#endregion findTheLatestCommonDateTime
		
		private DateTime tryThisDate(
			string firstTicker , string secondTicker ,
			Date dateToTry , DateTime maxAllowedDateTime )
		{
			DateTime dateTime = DateTime.MinValue;
			if ( this.areExchangedInTheDay(
				firstTicker , secondTicker , dateToTry , maxAllowedDateTime ) )
				dateTime = this.findTheLatestCommonDateTime(
					firstTicker , secondTicker , dateToTry , maxAllowedDateTime );
			return dateTime;
		}
		#endregion tryThisDate
		
		private DateTime findIntervalBegin(
			string firstTicker , string secondTicker , DateTime maxAllowedDateTime )
		{
			Date nextDateToTry = new Date( maxAllowedDateTime );
			DateTime intervalBeginCandidate = this.tryThisDate(
				firstTicker , secondTicker , nextDateToTry , maxAllowedDateTime );
			while ( intervalBeginCandidate == DateTime.MinValue )
			{
				// nextDateToTry does not contain a minute with a market value for both tickers
				nextDateToTry = nextDateToTry.AddDays( -1 );
				intervalBeginCandidate = this.tryThisDate(
					firstTicker , secondTicker , nextDateToTry , maxAllowedDateTime );
				this.checkForLoop( nextDateToTry , maxAllowedDateTime );
			}
			return intervalBeginCandidate;
		}
		#endregion findIntervalBegin
		
		public DateTime GetIntervalBeginDateTime( DateTime dateTime )
		{
//			DateTime yesterday = dateTime.AddDays( -1 );
//			DateTime yesterdayAtClose = HistoricalEndOfDayTimer.GetMarketClose( yesterday );
//			DateTime intervalBeginDateTime = yesterdayAtClose.AddMinutes( -1 );
			return this.intervalBegin;
		}
	}
}
