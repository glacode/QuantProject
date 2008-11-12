/*
QuantProject - Quantitative Finance Library

DailyBarCache.cs
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

using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.ADT.Timing;
using QuantProject.DataAccess;

namespace QuantProject.Data.DataProviders.Bars.Caching
{
	/// <summary>
	/// Caches bar values for some days, for the given times
	/// </summary>
	public class DailyBarCache : IBarCache
	{
		private int barInterval;
		private List< Time > dailyTimes;
		
		private int roughNumberOfItemsToBeCachedWithASingleQuery;

		private BarCacheData barOpenValues;
//		private int numberOfBarsIn_barOpenValues;
//		private int maxNumberOfItemsIn_barOpenValues;
//		private int numberOfItemsToBeRemovedFrom_barOpenValues_whenCacheIsCleanedUp;
//		List<DateTime> dateTimesForCleaningUp_barOpenValues;
//		List<string> tickersForCleaningUp_barOpenValues;
		
		private BarCacheData barsMissingInTheDatabase;
//		private int numberOfBarsIn_barsMissingInTheDatabase;
//		private int maxNumberOfItemsIn_barsMissingInTheDatabase;
//		private int	numberOfItemsToBeRemovedFrom_barsMissingInTheDatabase_whenCacheIsCleanedUp;
//		List<DateTime> dateTimesForCleaningUp_barsMissingInTheDatabase;
//		List<string> tickersForCleaningUp_barsMissingInTheDatabase;


		public DailyBarCache(
			int barInterval ,
			List< Time > dailyTimes )
		{
			this.checkParameters( dailyTimes );
			this.barInterval = barInterval;
			this.dailyTimes = dailyTimes;

			this.roughNumberOfItemsToBeCachedWithASingleQuery = 100;

			int maxNumberOfItemsIn_barOpenValues = 100000;
			int numberOfItemsToBeRemovedFrom_barOpenValues_whenCacheIsCleanedUp =
				maxNumberOfItemsIn_barOpenValues / 2;
			
			this.barOpenValues = new BarCacheData(
				maxNumberOfItemsIn_barOpenValues ,
				numberOfItemsToBeRemovedFrom_barOpenValues_whenCacheIsCleanedUp );
				
//			this.barOpenValues = new Dictionary< DateTime , Dictionary< string , double > >();
//			this.numberOfBarsIn_barOpenValues = 0;
				
			int maxNumberOfItemsIn_barsMissingInTheDatabase = 100000;
			int numberOfItemsToBeRemovedFrom_barsMissingInTheDatabase_whenCacheIsCleanedUp =
				maxNumberOfItemsIn_barOpenValues / 2;

			this.barsMissingInTheDatabase = new BarCacheData(
				maxNumberOfItemsIn_barsMissingInTheDatabase ,
				numberOfItemsToBeRemovedFrom_barsMissingInTheDatabase_whenCacheIsCleanedUp );
//			this.numberOfBarsIn_barsMissingInTheDatabase = 0;
		}
		
		#region checkParameters
		
		#region checkDailyTimesAreActuallyTimes
//		private void checkIfItActuallyIsATime( DateTime dateTime )
//		{
//			if ( !ExtendedDateTime.IsTime( dateTime ) )
//				throw new Exception(
//					"dailyTimes have to be times: use the method " +
//					"QuantProject.ADT.ExtendedDateTime.GetTime() " +
//					"to build them" );
//		}
//		private void checkDailyTimesAreActuallyTimes( DateTime[] dailyTimes )
//		{
//			foreach ( DateTime dateTime in dailyTimes )
//				this.checkIfItActuallyIsATime( dateTime );
//		}
		#endregion checkDailyTimesAreActuallyTimes
		
		#region checkIfDailyTimesAreInStrictAscendingOrder
		private void checkIfDailyTimesAreInStrictAscendingOrder(
			int indexForCurrentDailyTime , List< Time > dailyTimes )
		{
			Time currentDailyTime =
				dailyTimes[ indexForCurrentDailyTime ];
			Time nextDailyTime =
				dailyTimes[ indexForCurrentDailyTime + 1 ];
			if ( currentDailyTime >= nextDailyTime )
				throw new Exception(
					"dailyTimes are not in strict ascending order. " +
					"dailyTimes have to be in strict ascending order. " +
					"dailyTime with index " + indexForCurrentDailyTime +
					" is >= than dailyTime with index " +
					( indexForCurrentDailyTime + 1 ) +
					" while it should <" );
		}
		private void checkIfDailyTimesAreInStrictAscendingOrder( List< Time > dailyTimes )
		{
			for ( int index = 0 ; index < dailyTimes.Count - 1 ; index++ )
				this.checkIfDailyTimesAreInStrictAscendingOrder(
					index , dailyTimes );
		}
		#endregion checkIfDailyTimesAreInStrictAscendingOrder
		
		private void checkParameters( List< Time > dailyTimes )
		{
//			this.checkDailyTimesAreActuallyTimes( dailyTimes );
			this.checkIfDailyTimesAreInStrictAscendingOrder( dailyTimes );
		}
		#endregion checkParameters
		
		#region GetMarketValue
		
		private bool isABarMissingInTheDatabase( string ticker, DateTime dateTime )
		{
//			bool isMissing = false;
//			if ( this.barsMissingInTheDatabase.Bars.ContainsKey( dateTime ) )
//				// it is known at least a missing bar for the given dateTime
//				isMissing =
//					this.barsMissingInTheDatabase.Bars[ dateTime ].ContainsKey( ticker );
			bool isMissing =
				this.barsMissingInTheDatabase.ContainsBar( ticker , dateTime );
			return isMissing;
		}
		
		#region getMarketValueForBarThatCouldBeInTheDatabase
		
		private bool isInCache( string ticker , DateTime dateTime )
		{
//			bool isIn_barOpenValues =
//				( ( this.barOpenValues.Bars.ContainsKey( dateTime ) ) &&
//				 this.barOpenValues.Bars[ dateTime ].ContainsKey( ticker ) );
			bool isIn_barOpenValues = this.barOpenValues.ContainsBar( ticker , dateTime );
			return isIn_barOpenValues;
		}
		
		#region getMarketValueForBarThatsNotInCacheButCouldBeInTheDatabase
		
		#region updateDictionaries
		
		#region getBarOpenValues
		
		#region getLastDateTime
		private int getNumberOfDaysToBeCachedForASingleQuery()
		{
			int numberOfDaysToBeCachedForASingleQuery =
				this.roughNumberOfItemsToBeCachedWithASingleQuery /
				this.dailyTimes.Count;
			return numberOfDaysToBeCachedForASingleQuery;
		}
		private DateTime getLastDateTime( DateTime firstDateTime )
		{
			int numberOfDaysToBeCachedForASingleQuery =
				this.getNumberOfDaysToBeCachedForASingleQuery();
			DateTime lastDateTime =
				firstDateTime.AddDays( numberOfDaysToBeCachedForASingleQuery );
			return lastDateTime;
		}
		#endregion getLastDateTime
		
		private History getBarOpenValues( string ticker, DateTime firstDateTime )
		{
			DateTime lastDateTime = this.getLastDateTime( firstDateTime );
			History barOpenValues = DataBase.GetBarOpenHistory(
				ticker ,
				this.barInterval ,
				firstDateTime ,
				lastDateTime ,
				this.dailyTimes );
			return barOpenValues;
		}
		#endregion getBarOpenValues
		
		#region update_barOpenValues
		
		private void addBarOpenValue(
			string ticker, DateTime dateTime , History barOpenValuesFromQuery )
		{
			double barOpenValue =
				(double)barOpenValuesFromQuery[ dateTime ];
			this.barOpenValues.AddBar(
				ticker , dateTime , barOpenValue );
		}
		
		private void update_barOpenValues(
			string ticker, History barOpenValuesFromQuery )
		{
			foreach ( DateTime dateTime in barOpenValuesFromQuery.TimeLine )
				this.addBarOpenValue( ticker , dateTime , barOpenValuesFromQuery );
		}
		#endregion update_barOpenValues

		#region update_barsMissingInTheDatabase
		
		#region getDateTimesForMissingBarsToBeAdded
//		private List< DateTime > getDateTimesIn_barOpenValues()
//		{
//			List< DateTime > dateTimesIn_barOpenValues =
//				new List<DateTime>( this.barOpenValues.Bars.Keys );
//			dateTimesIn_barOpenValues.Sort();
//			return dateTimesIn_barOpenValues;
//		}
		
		#region removeThisDateIfItIsNotToBeAddedToMissingBars
		
//		private bool isIn_barsMissingInTheDatabase(
//			string ticker , DateTime dateTime )
//		{
//			bool isInCacheForMissingBars =
//				( ( this.barsMissingInTheDatabase.Bars.ContainsKey( dateTime ) ) &&
//				 this.barOpenValues.Bars[ dateTime ].ContainsKey( ticker ) );
//			return isInCacheForMissingBars;
//		}
		private bool hasToBeAddedToMissingBars(
			string ticker ,
			DateTime firstDateTime ,
			DateTime lastDateTime ,
			DateTime candidateDateTime )
		{
			bool hasToBeAdded =
				( ( candidateDateTime >= firstDateTime ) &&
				 ( candidateDateTime <= lastDateTime ) &&
				 ( !this.isInCache( ticker , candidateDateTime ) ) &&
				 ( !this.barsMissingInTheDatabase.ContainsBar(
				 	ticker , candidateDateTime ) ) );
			return hasToBeAdded;
		}
		private void removeThisDateIfItIsNotToBeAddedToMissingBars(
			string ticker ,
			DateTime firstDateTime ,
			DateTime lastDateTime ,
			DateTime candidateDateTime ,
			List< DateTime >  dateTimesForMissingBarsToBeAdded )
		{
			if ( !this.hasToBeAddedToMissingBars(
				ticker , firstDateTime , lastDateTime , candidateDateTime ) )
				dateTimesForMissingBarsToBeAdded.Remove( candidateDateTime );
		}
		private void removeThisDateIfItIsNotToBeAddedToMissingBars(
			string ticker ,
			DateTime firstDateTime ,
			DateTime lastDateTime ,
			int indexFor_dateTimesForMissingBarsToBeAdded ,
			List< DateTime >  dateTimesForMissingBarsToBeAdded )
		{
			DateTime candidateDateTime =
				dateTimesForMissingBarsToBeAdded[ indexFor_dateTimesForMissingBarsToBeAdded ];
			this.removeThisDateIfItIsNotToBeAddedToMissingBars(
				ticker , firstDateTime , lastDateTime , candidateDateTime ,
				dateTimesForMissingBarsToBeAdded );
		}
		#endregion removeThisDateIfItIsNotToBeAddedToMissingBars
		
		private List< DateTime > getDateTimesForMissingBarsToBeAdded(
			string ticker ,
			DateTime firstDateTime ,
			DateTime lastDateTime ,
			List< DateTime >  dateTimesForMissingBarsToBeAdded )
		{
			int indexFor_dateTimesForMissingBarsToBeAdded = 0;
			while ( ( indexFor_dateTimesForMissingBarsToBeAdded <
			         dateTimesForMissingBarsToBeAdded.Count ) &&
			       ( dateTimesForMissingBarsToBeAdded[
			       	indexFor_dateTimesForMissingBarsToBeAdded ] <=
			        lastDateTime ) )
				this.removeThisDateIfItIsNotToBeAddedToMissingBars(
					ticker , firstDateTime , lastDateTime ,
					indexFor_dateTimesForMissingBarsToBeAdded ,
					dateTimesForMissingBarsToBeAdded );
			return dateTimesForMissingBarsToBeAdded;
		}
		private List< DateTime > getDateTimesForMissingBarsToBeAdded(
			string ticker , DateTime firstDateTime , DateTime lastDateTime )
		{
			List< DateTime > dateTimesIn_barOpenValues =
				this.barOpenValues.DateTimes;
//				this.getDateTimesIn_barOpenValues();
			List< DateTime >  dateTimesForMissingBarsToBeAdded =
				this.getDateTimesForMissingBarsToBeAdded(
					ticker , firstDateTime , lastDateTime , dateTimesIn_barOpenValues );
			return dateTimesForMissingBarsToBeAdded;
		}
		private List< DateTime >  getDateTimesForMissingBarsToBeAdded(
			string ticker , DateTime firstDateTime )
		{
			DateTime lastDateTime = this.getLastDateTime( firstDateTime );
			List< DateTime > dateTimesForMissingBarsToBeAdded =
				this.getDateTimesForMissingBarsToBeAdded(
					ticker , firstDateTime , lastDateTime );
			return dateTimesForMissingBarsToBeAdded;
		}
		#endregion getDateTimesForMissingBarsToBeAdded

		#region update_barsMissingInTheDatabase
//		
//		#region addBarTo_barsMissingInTheDatabase
//		
//		#region removeValuesFrom_barsMissingInTheDatabase_ifMaxSizeHasBeenReached
//		
//		#region removeValuesFrom_barsMissingInTheDatabase
//		
//		#region initializeMembersForCleaningUp_barsMissingInTheDatabase
//		private void initialize_dateTimesForCleaningUp_barsMissingInTheDatabase()
//		{
//			this.dateTimesForCleaningUp_barsMissingInTheDatabase =
//				new List<DateTime>( this.barsMissingInTheDatabase.Keys );
//			this.dateTimesForCleaningUp_barsMissingInTheDatabase.Sort();
//		}
//		
//		#region initialize_tickersForCleaningUp_barsMissingInTheDatabase
//		private List< string >
//			getTickersForTheCurrentDateTimeToBeCleanedUpInMissingBars()
//		{
//			DateTime currentDateTimeForCleaningUp_barsMissingInTheDatabase =
//				this.dateTimesForCleaningUp_barsMissingInTheDatabase[ 0 ];
//			List< string >
//				tickersForTheCurrentDateTimeToBeCleanedUpInMissingBars =
//				this.barsMissingInTheDatabase[
//					currentDateTimeForCleaningUp_barsMissingInTheDatabase ];
//			return tickersForTheCurrentDateTimeToBeCleanedUpInMissingBars;
//		}
//		private void initialize_tickersForCleaningUp_barsMissingInTheDatabase()
//		{
//			this.tickersForCleaningUp_barOpenValues =
//				this.getTickersForTheCurrentDateTimeToBeCleanedUpInMissingBars();
//		}
//		#endregion initialize_tickersForCleaningUp_barsMissingInTheDatabase
//		
//		private void initializeMembersForCleaningUp_barsMissingInTheDatabase()
//		{
//			this.initialize_dateTimesForCleaningUp_barsMissingInTheDatabase();
//			this.initialize_tickersForCleaningUp_barsMissingInTheDatabase();
//		}
//		#endregion initializeMembersForCleaningUp_barsMissingInTheDatabase
//		
//		private void removeValuesFrom_barsMissingInTheDatabase()
//		{
//			this.initializeMembersForCleaningUp_barsMissingInTheDatabase();
//			while ( this.numberOfBarsIn_barsMissingInTheDatabase >
//			       this.maxNumberOfItemsIn_barsMissingInTheDatabase -
//			       this.numberOfItemsToBeRemovedFrom_barsMissingInTheDatabase_whenCacheIsCleanedUp )
//				this.removeNextItemFrom_barsMissingInTheDatabase();
//		}
//		#endregion removeValuesFrom_barsMissingInTheDatabase
//		
//		private void removeValuesFrom_barsMissingInTheDatabase_ifMaxSizeHasBeenReached()
//		{
//			if ( this.numberOfBarsIn_barsMissingInTheDatabase >=
//			    this.maxNumberOfItemsIn_barsMissingInTheDatabase )
//				// the cache for missing bars is full
//				this.removeValuesFrom_barsMissingInTheDatabase();
//		}
//		#endregion removeValuesFrom_barsMissingInTheDatabase_ifMaxSizeHasBeenReached
//		
//		private void addBarTo_barsMissingInTheDatabase(
//			string ticker, DateTime dateTime )
//		{
//			this.removeValuesFrom_barsMissingInTheDatabase_ifMaxSizeHasBeenReached();
//			this.addBarTo_barsMissingInTheDatabase_withFreeSpaceAssured();
//		}
//		#endregion addBarTo_barsMissingInTheDatabase
//		
		private void update_barsMissingInTheDatabase(
			string ticker, List< DateTime > dateTimesForMissingBarsToBeAdded )
		{
			foreach ( DateTime dateTime in dateTimesForMissingBarsToBeAdded )
				this.barsMissingInTheDatabase.AddBar( ticker , dateTime , double.MinValue );
//				this.addBarTo_barsMissingInTheDatabase( ticker , dateTime );
		}
		#endregion update_barsMissingInTheDatabase
		
		private void update_barsMissingInTheDatabase(
			string ticker, DateTime firstDateTime , History barOpenValues )
		{
			List< DateTime > dateTimesForMissingBarsToBeAdded =
				this.getDateTimesForMissingBarsToBeAdded(
					ticker , firstDateTime );
			this.update_barsMissingInTheDatabase(
				ticker , dateTimesForMissingBarsToBeAdded );
		}
		#endregion update_barsMissingInTheDatabase
		
		private void updateDictionaries(
			string ticker, DateTime dateTime , History barOpenValues )
		{
			this.update_barOpenValues( ticker, barOpenValues );
			this.update_barsMissingInTheDatabase(
				ticker , dateTime , barOpenValues );
		}

		private void updateDictionaries( string ticker, DateTime dateTime )
		{
			History barOpenValues = this.getBarOpenValues( ticker , dateTime );
			this.updateDictionaries( ticker , dateTime , barOpenValues );
		}
		#endregion updateDictionaries
		
		#region getMarketValueWithUpdatedDictionaries
		private double getMarketValueWithUpdatedDictionaries(
			string ticker, DateTime dateTime )
		{
			double marketValue = double.MinValue;
			if ( !this.barOpenValues.ContainsBar( ticker , dateTime ) )
				// the requested bar is not in the database
				throw new MissingBarException( ticker , dateTime , this.barInterval );
			else
				// the requested bar is in the database
				marketValue = this.barOpenValues.GetBarValue( ticker , dateTime );
			return marketValue;
		}		
		#endregion getMarketValueWithUpdatedDictionaries
		
		private double getMarketValueForBarThatsNotInCacheButCouldBeInTheDatabase(
			string ticker, DateTime dateTime )
		{
			this.updateDictionaries( ticker , dateTime );
			double marketValue =
				this.getMarketValueWithUpdatedDictionaries( ticker , dateTime );
			return marketValue;
		}
		#endregion getMarketValueForBarThatsNotInCacheButCouldBeInTheDatabase
		
		private double getMarketValueForBarThatCouldBeInTheDatabase(
			string ticker, DateTime dateTime )
		{
			double marketValue = double.MinValue;
			if ( this.isInCache( ticker , dateTime ) )
				// the requested bar is already in cache
				marketValue = this.barOpenValues.GetBarValue( ticker , dateTime );
			else
				// the requested bar is not in cache, but it could be in the database
				marketValue = this.getMarketValueForBarThatsNotInCacheButCouldBeInTheDatabase(
					ticker , dateTime );
			return marketValue;
		}
		#endregion getMarketValueForBarThatCouldBeInTheDatabase
		
		
		/// <summary>
		/// returns the market value for the given ticker, for the bar
		/// that opens at the given dateTime
		/// </summary>
		/// <param name="ticker"></param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public double GetMarketValue( string ticker, DateTime dateTime )
		{
			double marketValue = double.MinValue;
			if ( this.isABarMissingInTheDatabase( ticker , dateTime ) )
				// the bar is not in the database
				throw new MissingBarException( ticker , dateTime , this.barInterval );
			else
				// the bar can be in the database
				marketValue = this.getMarketValueForBarThatCouldBeInTheDatabase(
					ticker , dateTime );
			return marketValue;
		}
		#endregion GetMarketValue
	}
}
