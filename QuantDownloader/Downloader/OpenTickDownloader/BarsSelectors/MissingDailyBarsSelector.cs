/*
QuantProject - Quantitative Finance Library

MissingDailyBarsSelector.cs
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
using System.Collections;
using System.Data;

using QuantProject.ADT.Collections;
using QuantProject.ADT.Timing;
using QuantProject.Data;
using QuantProject.DataAccess;

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	/// <summary>
	/// Selects daily bars, but only those who are not already in the database
	/// </summary>
	public class MissingDailyBarsSelector : DailyBarsSelector
	{
		/// <summary>
		/// contains the dateTimeForOpen for those bars that are
		/// in the database, for the curren ticker and the given
		/// barInterval
		/// </summary>
		private Set barsAlreadyInTheDatabaseForTheCurrentTicker;
		
		/// <summary>
		/// the last ticker for whom the bars have been read from
		/// the database
		/// </summary>
		private string currentTicker;
		
		public MissingDailyBarsSelector(
			string[] tickers ,
			DateTime firstDate ,
			DateTime lastDate ,
			int barInterval ,
			DateTime firstBarOpenTimeInNewYorkTimeZone ,
			int numberOfBarsToBeDownloadedForEachDay ) :
			base(
				tickers ,
				firstDate ,
				lastDate ,
				barInterval ,
				firstBarOpenTimeInNewYorkTimeZone ,
				numberOfBarsToBeDownloadedForEachDay )
		{
		}
		
		#region isTheCurrentBarSelectable
		
		#region isCurrentBarInTheDatabase
		
		#region updateBarsAlreadyInTheDatabase
		
		#region updateBarsAlreadyInTheDatabase_actually
		
		#region getBarsInTheDatabase
//		private string getSqlTimeConstant( DateTime time )
//		{
//			string sqlTimeConstant =
//				time.Hour.ToString().PadLeft( 2 , '0' ) + ":" +
//				time.Minute.ToString().PadLeft( 2 , '0' ) + ":" +
//				time.Second.ToString().PadLeft( 2 , '0' );
//			return sqlTimeConstant;
//		}
//		private string getSqlTimeConstantForFirstDailyBar()
//		{
//			string sqlTimeConstantForFirstDailyBar =
//				this.getSqlTimeConstant(
//					this.firstBarOpenTimeInNewYorkTimeZone );
//			return sqlTimeConstantForFirstDailyBar;
//		}
		
//		#region getSqlTimeConstantForLastDailyBar
//		private DateTime getLastBarOpenTimeInNewYorkTimeZone()
//		{
//			int secondsToBeAdded = this.barInterval *
//				( this.numberOfBarsToBeDownloadedForEachDay - 1 );
//			DateTime lastBarOpenTimeInNewYorkTimeZone =
//				this.firstBarOpenTimeInNewYorkTimeZone.AddSeconds(
//					secondsToBeAdded );
//			return lastBarOpenTimeInNewYorkTimeZone;
//		}
//		private string getSqlTimeConstantForLastDailyBar()
//		{
//			DateTime lastBarOpenTimeInNewYorkTimeZone =
//				this.getLastBarOpenTimeInNewYorkTimeZone();
//			string sqlTimeConstantForLastDailyBar =
//				this.getSqlTimeConstant(
//					lastBarOpenTimeInNewYorkTimeZone );
//			return sqlTimeConstantForLastDailyBar;
//		}
//		#endregion getSqlTimeConstantForLastDailyBar
		
		
		private Time getLastBarOpenTimeInNewYorkTimeZone()
		{
			int secondsToBeAdded = this.barInterval *
				( this.numberOfBarsToBeDownloadedForEachDay - 1 );
			DateTime lastBarOpenDateTimeInNewYorkTimeZone =
				this.firstBarOpenTimeInNewYorkTimeZone.AddSeconds(
					secondsToBeAdded );
			Time lastBarOpenTimeInNewYorkTimeZone =
				new Time( lastBarOpenDateTimeInNewYorkTimeZone );
			return lastBarOpenTimeInNewYorkTimeZone;
		}
		
		private DataTable getBarsInTheDatabase( string ticker )
		{
			Time lastBarOpenTimeInNewYorkTimeZone =
				this.getLastBarOpenTimeInNewYorkTimeZone();
			DataTable barsInTheDatabase =
				new QuantProject.Data.DataTables.Bars(
					ticker , this.firstDate , this.lastDate ,
					new Time( this.firstBarOpenTimeInNewYorkTimeZone ) ,
					lastBarOpenTimeInNewYorkTimeZone ,
					this.barInterval );
//			string sql =
//				"select baDateTimeForOpen from bars " +
//				"where (baTicker='" + ticker + "') and " +
//				"(baInterval=" + this.barInterval + ") and" +
//				"(baDateTimeForOpen>=" +
//				DataBaseWriter.GetDateConstant( this.firstDate ) + ") and" +
//				"(baDateTimeForOpen<" +
//				DataBaseWriter.GetDateConstant( this.lastDate.AddDays( 1 ) )
//				+ ") and" +
//				"(Format([baDateTimeForOpen],'hh:mm:ss')>='" +
//				this.getSqlTimeConstantForFirstDailyBar() + "') and" +
//				"(Format([baDateTimeForOpen],'hh:mm:ss')<='" +
//				this.getSqlTimeConstantForLastDailyBar() + "');";
//			DataTable barsInTheDatabase =
//				SqlExecutor.GetDataTable( sql );
			return barsInTheDatabase;
		}
		#endregion getBarsInTheDatabase

		#region updateBarsAlreadyInTheDatabase
		private void clearBarsAlreadyInTheDatabaseForTheCurrentTicker()
		{
			if ( this.barsAlreadyInTheDatabaseForTheCurrentTicker == null )
				// this is the first time bars are added and
				// barsAlreadyInTheDatabaseForTheCurrentTicker has not
				// even been created, yet
				this.barsAlreadyInTheDatabaseForTheCurrentTicker = new Set();
		}
		private void updateBarsAlreadyInTheDatabase(
			DataTable dtBarsInTheDatabase )
		{
			this.clearBarsAlreadyInTheDatabaseForTheCurrentTicker();
			this.barsAlreadyInTheDatabaseForTheCurrentTicker.Clear();
			foreach( DataRow dataRow in dtBarsInTheDatabase.Rows )
				this.barsAlreadyInTheDatabaseForTheCurrentTicker.Add( dataRow[ 0 ] );
		}
		#endregion updateBarsAlreadyInTheDatabase
		
		private void updateBarsAlreadyInTheDatabase_actually()
		{
			DataTable dtBarsInTheDatabase =
				this.getBarsInTheDatabase( this.getCurrentTicker() );
			this.updateBarsAlreadyInTheDatabase( dtBarsInTheDatabase );
			this.currentTicker = this.getCurrentTicker();
		}
		#endregion updateBarsAlreadyInTheDatabase_actually

		private void updateBarsAlreadyInTheDatabase()
		{
			if ( this.currentTicker !=
			    this.getCurrentTicker() )
				// the current ticker has changed and so
				// this.barsAlreadyInTheDatabase is not up to date
				this.updateBarsAlreadyInTheDatabase_actually();
		}
		#endregion updateBarsAlreadyInTheDatabase
		
		private bool isCurrentBarAlreadyInTheDatabase_withSetUpdated()
		{
			DateTime dateTimeForCurrentCandidateBarOpenInNewYorkTimeZone =
				this.getDateTimeForCurrentCandidateBarOpenInNewYorkTimeZone();
			bool isAlreadyInTheDatabase =
				this.barsAlreadyInTheDatabaseForTheCurrentTicker.Contains(
					dateTimeForCurrentCandidateBarOpenInNewYorkTimeZone );
			return isAlreadyInTheDatabase;
		}
		private bool isCurrentBarAlreadyInTheDatabase()
		{
			this.updateBarsAlreadyInTheDatabase();
			bool isAlreadyInTheDatabase =
				this.isCurrentBarAlreadyInTheDatabase_withSetUpdated();
			return isAlreadyInTheDatabase;
		}
		#endregion isCurrentBarInTheDatabase

		protected override bool isTheCurrentBarSelectable()
		{
			bool isSelectable =
				( ( this.isAPossibleMarketDay( this.currentDate ) ) &&
				 ( ! this.isCurrentBarAlreadyInTheDatabase() ) );
			return isSelectable;
		}
		#endregion isTheCurrentBarSelectable
	}
}
