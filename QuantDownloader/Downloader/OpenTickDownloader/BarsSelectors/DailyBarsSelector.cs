/*
QuantProject - Quantitative Finance Library

DailyBarsSelector.cs
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

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	/// <summary>
	/// Selects all daily bars, without looking at what it's in the database, already
	/// </summary>
	public class DailyBarsSelector : IBarsSelector
	{
		private string[] tickers;
		protected DateTime firstDate;
		protected DateTime lastDate;
		/// <summary>
		/// lenght, in seconds, for a bar (60 for a one minute bar)
		/// </summary>
		protected int barInterval;
		protected DateTime firstBarOpenTimeInNewYorkTimeZone;
		protected int numberOfBarsToBeDownloadedForEachDay;
		
		/// <summary>
		/// points to the ticker for the current bar to be selected
		/// </summary>
		private int currentTickerIndex;
		protected DateTime currentDate;
		/// <summary>
		/// (0 based) current bar in the currentDate
		/// </summary>
		private int currentDailyBar;
				
		public bool AreAllBarsAlredyGiven
		{
			get
			{
				bool areAllBarsAlredyGiven =
					( this.currentTickerIndex == ( this.tickers.Length - 1 ) ) &&
					( ( this.currentDate.CompareTo( this.lastDate ) == 0 ) &&
					( this.currentDailyBar ==
					  this.numberOfBarsToBeDownloadedForEachDay - 1 ) );
				return areAllBarsAlredyGiven; }
		}
		
		/// <summary>
		/// Selects all daily bars, without looking at what it's
		/// in the database, already
		/// </summary>
		/// <param name="tickers">the tickers whose bars are to be downloaded</param>
		/// <param name="firstDate">first date for the days to be considered</param>
		/// <param name="lastDate">last date for the days to be considered</param>
		/// <param name="barInterval">lenght, in seconds, for a bar (60 for
		/// a one minute bar)</param>
		/// <param name="firstBarOpenTimeInNewYorkTimeZone">time for the open
		/// of the first bar that has to be downloaded, for every day;
		/// use New York time zone for this parameter</param>
		/// <param name="numberOfBarsToBeDownloadedForEachDay">number of bars
		/// to be downloaded every day</param>
		public DailyBarsSelector(
			string[] tickers ,
			DateTime firstDate ,
			DateTime lastDate ,
			int barInterval ,
			DateTime firstBarOpenTimeInNewYorkTimeZone ,
			int numberOfBarsToBeDownloadedForEachDay )
		{
			this.checkParameters(
			firstDate , lastDate , numberOfBarsToBeDownloadedForEachDay );
			
			this.tickers = tickers;
			this.firstDate = firstDate;
			this.lastDate = lastDate;
			this.barInterval = barInterval;
			this.firstBarOpenTimeInNewYorkTimeZone =
				firstBarOpenTimeInNewYorkTimeZone;
			this.numberOfBarsToBeDownloadedForEachDay =
				numberOfBarsToBeDownloadedForEachDay;
			
			this.currentTickerIndex = 0;
			this.currentDate = this.firstDate.AddDays( - 1 );
			this.currentDailyBar =
				numberOfBarsToBeDownloadedForEachDay - 1;
			
			this.moveToTheNextSelectedBar();
		}
		
		#region checkParameters
		private void checkParameters_checkNoTime( DateTime dateTime )
		{
			if (
				( dateTime.Hour != 0 ) ||
				( dateTime.Minute != 0 ) ||
				( dateTime.Second != 0 ) )
				throw new Exception( "A date is expected!" );
		}
		private void checkParameters(
			DateTime firstDate , DateTime lastDate ,
			int numberOfBarsToBeDownloadedForEachDay )
		{
			this.checkParameters_checkNoTime( firstDate );
			this.checkParameters_checkNoTime( lastDate );
			if ( firstDate.CompareTo( lastDate ) > 0 )
				throw new Exception( "firstDate cannot follow lastDate!" );
			if ( numberOfBarsToBeDownloadedForEachDay <= 0 )
				throw new Exception(
					"numberOfBarsToBeDownloadedForEachDay must be greater than zero!" );
		}
		#endregion checkParameters
		
//		private void handleCaseTheLastBarIdentifierWasTheLastOneForTheDay()
//		{
//			if ( this.currentDailyBar >
//			    this.numberOfBarsToBeDownloadedForEachDay )
//				// the last bar identifier was the last one for the day
//			{
//				this.currentDailyBar = 0;
//				this.currentDate.AddDays( 1 );
//			}
//		}
		
		#region moveToTheNextBar
		
		#region doNextStep
		
		#region doNextStep_actually
		private void doNextStep_actually_moveToTheNextDay()
		{
			if ( this.currentDate == this.lastDate )
			{
				// all bars for the current ticker have been signaled
				this.currentTickerIndex++;
				this.currentDate = this.firstDate;
			}
			else
				// for the current ticker there may be
				// some other bars to be signaled out
				this.currentDate = this.currentDate.AddDays( 1 );
			this.currentDailyBar = 0;
		}
		private void doNextStep_actually()
		{
			if ( this.currentDailyBar ==
			    ( this.numberOfBarsToBeDownloadedForEachDay - 1 ) )
				// the current bar identifier is the last one for the
				// current (ticker,date)
				this.doNextStep_actually_moveToTheNextDay();
			else
				// the current bar identifier is not the last one
				// for the current (ticker,date)
				this.currentDailyBar ++;
		}
		#endregion doNextStep_actually
		private void doNextStep()
		{
			this.doNextStep_actually();
//			this.handle_areAllBarsAlredyGiven();
		}
		#endregion doNextStep
		
		private bool isTheCurrentBarBeyondTheLastDate()
		{
			bool isBeyondTheLastDate =
				( this.currentDate.CompareTo( this.lastDate ) > 0 );
			return isBeyondTheLastDate;
		}
		
		#region isTheCurrentBarSelectable
		protected bool isAPossibleMarketDay( DateTime currentDate )
		{
			bool isAPossibleMarkDay =
				( currentDate.DayOfWeek != DayOfWeek.Saturday ) &&
				( currentDate.DayOfWeek != DayOfWeek.Sunday ) &&
				!( ( currentDate.Month == 1 ) && ( currentDate.Day == 1 ) ) &&
				!( ( currentDate.Month == 12 ) && ( currentDate.Day == 25 ) );
			
			return isAPossibleMarkDay;
		}
		protected virtual bool isTheCurrentBarSelectable()
		{
			bool isSelectable =
				this.isAPossibleMarketDay( this.currentDate );
			return isSelectable;
		}
		#endregion isTheCurrentBarSelectable
		
		private void moveToTheNextSelectedBar()
		{
			this.doNextStep();
			while ( !this.isTheCurrentBarBeyondTheLastDate() &&
				!this.isTheCurrentBarSelectable() )
				this.doNextStep();
//			DateTime currentDate = this.firstDate;
//			while ( currentDate <= this.lastDate )
//			{
//				if ( this.isAPossibleMarketDay( currentDate ) )
//					this.fillQueue_requestBars( currentDate );
//				currentDate = currentDate.AddDays( 1 );
//			}
		}
		#endregion moveToTheNextBar
		
		#region GetNextBarIdentifier
		protected string getCurrentTicker()
		{
			string currentTicker = this.tickers[ this.currentTickerIndex ];
			return currentTicker;
		}
		
		#region getNextBarIdentifier_actually
		protected DateTime getDateTimeForCurrentCandidateBarOpenInNewYorkTimeZone()
		{
			DateTime dateTimeForCurrentCandidateBarOpenInNewYorkTimeZone =
				new DateTime(
					currentDate.Year ,
					currentDate.Month ,
					currentDate.Day ,
					this.firstBarOpenTimeInNewYorkTimeZone.Hour ,
					this.firstBarOpenTimeInNewYorkTimeZone.Minute ,
					this.firstBarOpenTimeInNewYorkTimeZone.Second ).AddSeconds(
				this.currentDailyBar * this.barInterval );
			return dateTimeForCurrentCandidateBarOpenInNewYorkTimeZone;
		}
		private BarIdentifier getNextBarIdentifier_actually()
		{
			DateTime dateTimeForCurrentCandidateBarOpenInNewYorkTimeZone =
				this.getDateTimeForCurrentCandidateBarOpenInNewYorkTimeZone();
			BarIdentifier barIdentifier =
				new BarIdentifier(
					this.getCurrentTicker() ,
					dateTimeForCurrentCandidateBarOpenInNewYorkTimeZone ,
					this.barInterval );
			return barIdentifier;
		}
		#endregion getNextBarIdentifier_actually

		public BarIdentifier GetNextBarIdentifier()
		{
			if ( this.AreAllBarsAlredyGiven )
				throw new Exception( "There are no more bars to be selected!" );
			BarIdentifier barIdentifier =
				this.getNextBarIdentifier_actually();
			this.moveToTheNextSelectedBar();
			return barIdentifier;
		}
		#endregion GetNextBarIdentifier
	}
}
