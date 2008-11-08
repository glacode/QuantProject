/*
QuantProject - Quantitative Finance Library

BarCacheData.cs
Copyright (C) 2008
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

namespace QuantProject.Data.DataProviders.Bars.Caching
{
	/// <summary>
	/// Basic data structure used by bar caches: each IBarCache will use its own
	/// strategy, but it will probably use this class for adding bars to the cache
	/// and for checking if bars are already in the cache
	/// </summary>
	public class BarCacheData
	{
		private int maxNumberOfItemsIn_bars;
		private int numberOfItemsToBeRemovedFrom_bars_whenCacheIsCleanedUp;

		private Dictionary< DateTime , Dictionary< string , double > > bars;
		
//		public Dictionary<DateTime, Dictionary<string, double>> Bars {
//			get { return this.bars; }
//		}
		
		private int numberOfBarsIn_bars;
		List<DateTime> dateTimesForCleaningUp_bars;
		List<string> tickersForCleaningUp_bars;

		/// <summary>
		/// date times for which at least a bar is in the dictionary
		/// </summary>
		public List< DateTime > DateTimes
		{
			get
			{
				List<DateTime> dateTimes =
					new List<DateTime>( this.bars.Keys );
				return dateTimes;
			}
		}
		
		public BarCacheData(
			int maxNumberOfItemsIn_bars ,
			int numberOfItemsToBeRemovedFrom_bars_whenCacheIsCleanedUp	)
		{
			this.maxNumberOfItemsIn_bars = maxNumberOfItemsIn_bars;
			this.numberOfItemsToBeRemovedFrom_bars_whenCacheIsCleanedUp =
				numberOfBarsIn_bars;
			
			this.bars = new Dictionary< DateTime , Dictionary< string , double > >();
			this.numberOfBarsIn_bars = 0;
		}
		
		#region AddBar
		private void add_dateTime_toCacheIfTheCase(
			DateTime dateTime )
		{
			if ( !this.bars.ContainsKey( dateTime ) )
				// no data is yet in cache for this dateTime and a
				this.bars.Add( dateTime , new Dictionary< string , double >() );
		}
		
		#region addBar_with_dateTime_alreadyInCache
		
		#region addBar_actually
		
		#region removeValuesFromTheCacheIfMaxSizeHasBeenReached
		
		#region removeValuesFromTheCache
		
		#region initializeMembersForCleaningUp_bars
		private void initialize_dateTimesForCleaningUp_bars()
		{
			this.dateTimesForCleaningUp_bars =
				new List<DateTime>(this.bars.Keys);
			this.dateTimesForCleaningUp_bars.Sort();
		}
		
		#region initialize_tickersForCleaningUp_bars
		private Dictionary< string , double >
			getBarOpenValuesForTheCurrentDateTimeToBeCleanedUpInCache()
		{
			DateTime currentDateTimeForCleaningUp_bars =
				this.dateTimesForCleaningUp_bars[ 0 ];
			Dictionary< string , double >
				barOpenValuesForTheCurrentDateTimeToBeCleanedUpInCache =
				this.bars[ currentDateTimeForCleaningUp_bars ];
			return barOpenValuesForTheCurrentDateTimeToBeCleanedUpInCache;
		}
		private void initialize_tickersForCleaningUp_bars()
		{
			Dictionary< string , double >
				barOpenValuesForTheCurrentDateTimeToBeCleanedUpInCache =
				this.getBarOpenValuesForTheCurrentDateTimeToBeCleanedUpInCache();
			this.tickersForCleaningUp_bars =
				new List<string>(
					barOpenValuesForTheCurrentDateTimeToBeCleanedUpInCache.Keys );
		}
		#endregion initialize_tickersForCleaningUp_bars
		
		private void initializeMembersForCleaningUp_bars()
		{
			this.initialize_dateTimesForCleaningUp_bars();
//			this.currentDateTimeIndexForCleaningUp_bars = 0;
			this.initialize_tickersForCleaningUp_bars();
		}
		#endregion initializeMembersForCleaningUp_bars
		
		#region removeNextItemFrom_bars
		
//		private void update_tickersForCleaningUp_bars()
//		{
//			344
//		}
		
		#region removeNextItemFrom_bars_withUpdated_tickersForCleaningUp
		
		#region removeCurrentItemFrom_bars
		
		#region removeCurrentItemFrom_bars_actually
		private void removeCurrentItemFrom_bars_only()
		{
			DateTime dateTimeForCurrentItemToBeRemoved =
				this.dateTimesForCleaningUp_bars[ 0 ];
			string tickerForCurrentItemToBeRemoved =
				this.tickersForCleaningUp_bars[ 0 ];
			this.bars[ dateTimeForCurrentItemToBeRemoved ].Remove(
				tickerForCurrentItemToBeRemoved );
		}
		
		#region updateMembersUsedToCleanUp_bars
		
		#region moveToTheNextDateTimeForCleaningUp_bars
		private void moveToTheNextDateTimeForCleaningUp_bars()
		{
			this.dateTimesForCleaningUp_bars.RemoveAt( 0 );
			this.initialize_tickersForCleaningUp_bars();
		}
		#endregion moveToTheNextDateTimeForCleaningUp_bars
		
		private void updateMembersUsedToCleanUp_bars()
		{
			this.tickersForCleaningUp_bars.RemoveAt( 0 );
			if ( this.tickersForCleaningUp_bars.Count == 0 )
				// all the tickers have been removed for the current DateTime
				this.moveToTheNextDateTimeForCleaningUp_bars();
		}
		#endregion updateMembersUsedToCleanUp_bars
		
		private void removeCurrentItemFrom_bars_actually()
		{
			this.removeCurrentItemFrom_bars_only();
			this.updateMembersUsedToCleanUp_bars();
		}
		#endregion removeCurrentItemFrom_bars_actually
		
		private void removeCurrentItemFrom_bars()
		{
			removeCurrentItemFrom_bars_actually();
			this.numberOfBarsIn_bars--;
		}
		#endregion removeCurrentItemFrom_bars
		
		private void removeNextItemFrom_bars_withUpdated_tickersForCleaningUp()
		{
			this.removeCurrentItemFrom_bars();
			this.updateMembersUsedToCleanUp_bars();
		}
		#endregion removeNextItemFrom_bars_withUpdated_tickersForCleaningUp
		
		private void removeNextItemFrom_bars()
		{
			this.removeNextItemFrom_bars_withUpdated_tickersForCleaningUp();
		}
		#endregion removeNextItemFrom_bars
		
		private void removeValuesFromTheCache()
		{
			this.initializeMembersForCleaningUp_bars();
			while ( this.numberOfBarsIn_bars >
			       this.maxNumberOfItemsIn_bars -
			       this.numberOfItemsToBeRemovedFrom_bars_whenCacheIsCleanedUp )
				this.removeNextItemFrom_bars();
		}
		#endregion removeValuesFromTheCache
		
		private void removeValuesFromTheCacheIfMaxSizeHasBeenReached()
		{
			if ( this.numberOfBarsIn_bars >= this.maxNumberOfItemsIn_bars )
				// the cache is full
				this.removeValuesFromTheCache();
		}
		#endregion removeValuesFromTheCacheIfMaxSizeHasBeenReached
		
		private void addBar_actually(
			string ticker, double barOpenValue ,
			Dictionary< string , double > barsInDictionaryForTheGivenDateTime )
		{
			this.removeValuesFromTheCacheIfMaxSizeHasBeenReached();
			barsInDictionaryForTheGivenDateTime.Add( ticker , barOpenValue );
			this.numberOfBarsIn_bars++;
		}
		#endregion addBar_actually
		
		private void addBar(
			string ticker, double barOpenValue ,
			Dictionary< string , double > barsInDictionaryForTheGivenDateTime )
		{
			if ( !barsInDictionaryForTheGivenDateTime.ContainsKey( ticker ) )
				// the cache doesn't contain the open value
				// for the given ticker and the given dateTime
				this.addBar_actually(
					ticker , barOpenValue ,	barsInDictionaryForTheGivenDateTime);
		}
//		private void addBarOpenValue(
//			Dictionary< string , double > barOpenValuesInCacheForGivenDateTime ,
//			string ticker, DateTime dateTime , double theValue )
//		{
//			this.addBarOpenValue(
//				barOpenValuesInCacheForGivenDateTime , ticker , barOpenValue );
//		}
		private void addBar_with_dateTime_alreadyInCache(
			string ticker, DateTime dateTime , double theValue )
		{
			Dictionary< string , double > barsInDictionaryForTheGivenDateTime =
				this.bars[ dateTime ];
			this.addBar(
				ticker , theValue , barsInDictionaryForTheGivenDateTime );
		}
		#endregion addBar_with_dateTime_alreadyInCache
		
		/// <summary>
		/// adds a bar to the cache; if there is no more space in the cache, (supposed)
		/// old items are removed before adding the new bar
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="dateTime"></param>
		/// <param name="theValue"></param>
		public void AddBar(
			string ticker, DateTime dateTime , double theValue )
		{
			this.add_dateTime_toCacheIfTheCase( dateTime );
			this.addBar_with_dateTime_alreadyInCache(
				ticker , dateTime , theValue );
		}
		#endregion AddBar
		
		/// <summary>
		/// true iif the dictionary contains the requested bar
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public bool ContainsBar( string ticker , DateTime dateTime )
		{
			bool containsBar =
				( ( this.bars.ContainsKey( dateTime ) ) &&
				 ( this.bars[ dateTime ].ContainsKey( ticker ) ) );
			return containsBar;
		}
		

		/// <summary>
		/// returns the value for the given bar
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public double GetBarValue( string ticker , DateTime dateTime )
		{
			// we don't check if the bar is actually in the database,
			// because we want this method to be as fast as possible;
			// if the bar is missing, then an exception will be thrown by
			// the framework. It is responsability of the caller to decide if
			// to invoke the ContainsBar method before invoking this method
			double barValue = this.bars[ dateTime ][ ticker ];
			return barValue;
		}
	}
}
