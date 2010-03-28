/*
QuantProject - Quantitative Finance Library

ReturnIntervals.cs
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
using System.Collections;

using QuantProject.ADT.Histories;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;

namespace QuantProject.Business.Strategies.ReturnsManagement.Time
{
	/// <summary>
	/// Time intervals in a timed sequence, i.e. the (n+1)th interval
	/// never begins before the (n)th interval ends
	/// </summary>
	[Serializable]
	public class ReturnIntervals : CollectionBase
	{
		protected DateTime firstDateTime;
		protected DateTime lastDateTime;
		protected string benchmark;
		protected History marketDaysForBenchmark;
		private History bordersHistory;
		protected int intervalLength;
		
		private IIntervalsSelector intervalsSelector;

		public ReturnInterval this[ int index ]
		{
			get
			{
				return( (ReturnInterval) this.List[ index ] );
			}
			set
			{
				this.List[ index ] = value;
			}
		}

		/// <summary>
		/// The end border for the last interval
		/// </summary>
		public DateTime LastDateTime
		{
			get
			{
				if ( this.Count == 0 )
					throw new Exception( "LastEndOfDayDateTime " +
					                    "cannot be used when ReturnIntervals has " +
					                    "no ReturnInterval added yet!" );
				return this[ this.Count - 1 ].End;
			}
		}
		public ReturnInterval LastInterval
		{
			get
			{
				if ( this.Count == 0 )
					throw new Exception( "LastInterval " +
					                    "cannot be used when ReturnIntervals has " +
					                    "no ReturnInterval added yet!" );
				ReturnInterval lastInterval = this[ this.Count - 1 ];
				return lastInterval;
			}
		}
		
		public ReturnInterval SeconLastInterval
		{
			get
			{
				if ( this.Count < 2 )
					throw new Exception(
						"SeconLastInterval cannot be used when ReturnIntervals has " +
						"less then two intervals!" );
				ReturnInterval lastInterval = this[ this.Count - 2 ];
				return lastInterval;
			}
		}
		/// <summary>
		/// The History made up by queuing all
		/// interval's borders
		/// </summary>
		public History BordersHistory
		{
			get
			{
				if ( this.bordersHistory == null )
					// the border's history has not been set yet
					this.setBordersHistory();
				return this.bordersHistory;
			}
		}
		
		private void returnIntervals_initialize_checkParameters(
			DateTime firstDateTime , DateTime lastDateTime ,
			string benchmark , int intervalLength)
		{
			if( lastDateTime <= firstDateTime )
				throw new Exception(
					"lastEndOfDayDateTime must be greater than firstEndOfDayDateTime!" );
			if( intervalLength < 1 )
				throw new Exception( "Interval length must greater than 0!" );
		}
		
		private void returnIntervals_initialize(
			DateTime firstDateTime , DateTime lastDateTime ,
			string benchmark , int intervalLength)
		{
			this.returnIntervals_initialize_checkParameters(
				firstDateTime, lastDateTime, benchmark, intervalLength);
			this.intervalLength = intervalLength;
			this.firstDateTime = firstDateTime;
			this.lastDateTime = lastDateTime;
			this.benchmark = benchmark;
			this.setMarketDaysForBenchmark();
			this.setIntervals();
		}

		/// <summary>
		/// Creates the proper intervals, for the given benchmark, from
		/// the first DateTime to the last DateTime
		/// </summary>
		/// <param name="firstEndOfDayDateTime"></param>
		/// <param name="lastEndOfDayDateTime"></param>
		/// <param name="benchmark"></param>
		public ReturnIntervals(
			DateTime firstDateTime , DateTime lastDateTime , string benchmark )
		{
			this.returnIntervals_initialize( firstDateTime,
			                                lastDateTime , benchmark , 1 );//default intervals are daily
		}
		
		/// <summary>
		/// Creates the proper intervals, for the given benchmark, from
		/// the first DateTime to the last DateTime
		/// </summary>
		/// <param name="firstDateTime"></param>
		/// <param name="lastDateTime"></param>
		/// <param name="benchmark"></param>
		/// <param name="intervalLength"></param>
		public ReturnIntervals(
			DateTime firstDateTime , DateTime lastDateTime ,
			string benchmark, int intervalLength )
		{
			this.returnIntervals_initialize(
				firstDateTime ,	lastDateTime, benchmark, intervalLength );
		}
				          
		/// <summary>
		/// Use this constructor if you want to create an empty
		/// collection of intervals. The object will then be populated
		/// adding intervals by means of an IIntervalSelector
		/// </summary>
		/// <param name="intervalSelector">to be used for adding
		/// intervals</param>
		public ReturnIntervals( IIntervalsSelector intervalSelector)
		{
			this.intervalsSelector = intervalSelector;
		}
		
		/// <summary>
		/// creates a ReturnIntervals with no ReturnInterval
		/// </summary>
		public ReturnIntervals()
		{
		}

		
		/// <summary>
		/// It creates a ReturnIntervals with just a ReturnInterval
		/// </summary>
		public ReturnIntervals( ReturnInterval returnInterval )
		{
			this.Add( returnInterval );
		}

		protected virtual void setMarketDaysForBenchmark()
		{
			this.marketDaysForBenchmark =
				QuantProject.Data.DataTables.Quotes.GetMarketDays( this.benchmark ,
				                                                  this.firstDateTime , this.lastDateTime );
		}
		
		protected virtual void setIntervals()
		{
			// TO DO remove this method and change
			// derived classes to become IIntervalSelector
		}
		
		/// <summary>
		/// True iff for each interval border, there is an EndOfDayDateTime
		/// value in the history's eod date times that is exactly the same
		/// EndOfDayDateTime as the border value
		/// </summary>
		/// <param name="endOfDayHistory"></param>
		/// <returns></returns>
		public bool AreIntervalBordersAllCoveredBy(
			History history )
		{
			bool areAllCovered = true;
			foreach ( ReturnInterval returnInterval in this  )
				if ( !returnInterval.AreBordersCoveredBy( history ) )
				areAllCovered = false;
			return areAllCovered;
		}
		#region setBordersHistory
		private bool beginsBeforeTheLastAdded( ReturnInterval
		                                      returnInterval )
		{
			bool beginsBefore =
				( ( this.bordersHistory != null ) &&
				 ( this.bordersHistory.Count > 0 ) &&
				 ( returnInterval.BeginsBefore(
				 	this.bordersHistory.LastDateTime ) ) );
			return beginsBefore;
		}
		private void checkIfTheIntervalIsWellOrdered(
			ReturnInterval returnInterval )
		{
			if ( this.beginsBeforeTheLastAdded( returnInterval ) )
				throw new Exception( "The first border of the given interval " +
				                    "begins before the last EndOfDayDateTime added to the history" );
		}
		private bool beginsOnTheLastAddedTime( ReturnInterval returnInterval )
		{
			bool returnValue =
				( ( this.bordersHistory != null ) &&
				 ( this.bordersHistory.Count > 0 ) &&
				 ( this.bordersHistory.LastDateTime == returnInterval.Begin ) );
			return returnValue;
		}
		private void setEndOfDayHistoryForCurrentInterval(
			ReturnInterval returnInterval )
		{
			this.checkIfTheIntervalIsWellOrdered( returnInterval );
			if ( this.beginsOnTheLastAddedTime( returnInterval ) )
				// the current return interval begins exactly where the
				// previous interval ended (for instance, it happens for
				// CloseToClose intervals)
				this.bordersHistory.Add( returnInterval.End , returnInterval.End );
			else
			{
				// the current return interval begins after the
				// previous interval ended (for instance, it happens for the
				// OpenToClose intervals)
				this.bordersHistory.Add( returnInterval.Begin , returnInterval.Begin );
				this.bordersHistory.Add( returnInterval.End , returnInterval.End );
			}
		}
		private void setBordersHistory()
		{
			this.bordersHistory = new History();
			for( int i = 0 ; i < this.Count ; i++ )
				this.setEndOfDayHistoryForCurrentInterval( this[ i ] );
		}
		#endregion setBordersHistory

		#region Add
		private void checkParameter( ReturnInterval returnInterval )
		{
			if ( ( this.Count > 0 ) && ( returnInterval.Begin < this.LastDateTime ) )
				throw new Exception( "returnInterval must be later" );
		}
		public void Add( ReturnInterval returnInterval )
		{
			this.checkParameter( returnInterval );
			this.List.Add( returnInterval );
		}
		#endregion Add
		
		#region appendIntervalsButDontGoBeyondLastDate
		private void appendIntervalsButDontGoBeyondLastDate_checkParameters(
			DateTime firstDate , DateTime lastDate )
		{
			if ( firstDate.CompareTo( lastDate ) >= 0 )
				throw new Exception( "lastDate must be greater than firstDate!" );
			if ( this.Count > 0 )
				// some interval has already been added
			{
				DateTime currentLastDateTime =
					this[ this.Count - 1 ].End;
				if ( firstDate < currentLastDateTime )
					throw new Exception( "firstDate cannot be smaller than " +
					                    "the end of the last interval already in this collection!" );
			}
		}
		private ReturnInterval getFirstIntervalToBeAdded( DateTime firstDate )
		{
			ReturnInterval firstIntervalToBeAdded;
			if ( this.Count == 0 )
				// this object is still empty: no interval has been added yet
				firstIntervalToBeAdded =
					this.intervalsSelector.GetFirstInterval( firstDate );
			else
				// this object already contains at least a ReturnInterval
				firstIntervalToBeAdded =
					this.intervalsSelector.GetNextInterval( this );
			return firstIntervalToBeAdded;
		}
		/// <summary>
		/// Appends a list of intervals, starting from firstDate and
		/// until the next one would go beyond lastDate.
		/// This method can be invoked even if the ReturnIntervals object
		/// is empty.
		/// The firstDate has to be earlier than lastDate
		/// </summary>
		/// <param name="firstDate"></param>
		/// <param name="lastDate"></param>
		private void appendIntervalsButDontGoBeyondLastDate(
			DateTime firstDate ,
			DateTime lastDate )
		{
			this.appendIntervalsButDontGoBeyondLastDate_checkParameters(
				firstDate , lastDate );
			ReturnInterval nextInterval =
				this.getFirstIntervalToBeAdded( firstDate );
			while ( nextInterval.End.CompareTo( lastDate ) <= 0 )
			{
				this.Add( nextInterval );
				nextInterval =
					this.intervalsSelector.GetNextInterval( this );
			}
		}
		#endregion appendIntervalsButDontGoBeyondLastDate
		
		#region AppendIntervalsButDontGoBeyondLastDate
		private void appendIntervalsButDontGoBeyondLastDate_checkParameters(
			DateTime lastDate )
		{
			if ( this.Count == 0 )
				throw new Exception(
					"ReturnIntervals.AppendIntervalsButDontGoBeyondLastDate( " +
					"EndOfDayDateTime lastDate ) " +
					"has been invoked but the current ReturnIntervals " +
					"object is empty!" );
			if ( lastDate <= this.LastDateTime )
				throw new Exception(
					"ReturnIntervals.AppendIntervalsButDontGoBeyondLastDate( " +
					"DateTime lastDate ) " +
					"has been invoked but lastDate must be larger than the " +
					"end of the last ReturnInterval already in this collection" );
		}
		/// <summary>
		/// Appends a list of intervals,
		/// until the next one would go beyond lastDate.
		/// This method can be invoked only if the ReturnIntervals object
		/// is not empty.
		/// The last added interval must end before lastDate
		/// </summary>
		/// <param name="lastDate"></param>
		public void AppendIntervalsButDontGoBeyondLastDate(
			DateTime lastDate )
		{
			this.appendIntervalsButDontGoBeyondLastDate_checkParameters(
				lastDate );
			DateTime firstDate = this.LastDateTime;
			this.appendIntervalsButDontGoBeyondLastDate( firstDate , lastDate );
		}
		#endregion AppendIntervalsButDontGoBeyondLastDate
		
		#region AppendIntervalsToGoJustBeyond
		private void appendIntervalsToGoJustBeyondLastDate_checkParameters(
			DateTime lastDateTime )
		{
			if ( this.Count == 0 )
				throw new Exception(
					"ReturnIntervals.AppendIntervalsToGoJustBeyond( EndOfDayDateTime lastDate ) " +
					"has been invoked but the current ReturnIntervals " +
					"object is empty!" );
			if ( lastDateTime < this.LastDateTime )
				throw new Exception(
					"ReturnIntervals.AppendIntervalsToGoJustBeyond( DateTime lastDateTime ) " +
					"has been invoked but lastDate must be larger than or equal to " +
					"the end of the last ReturnInterval already in the collection" );
		}

		/// <summary>
		/// Appends a list of intervals, and stops as soon
		/// as the last added interval exceeds lastDate.
		/// This method can be invoked only if the ReturnIntervals object
		/// is not empty.
		/// The last added interval cannot end after lastDate
		/// </summary>
		/// <param name="lastDate"></param>
		public void AppendIntervalsToGoJustBeyond(
			DateTime lastDate )
		{
			this.appendIntervalsToGoJustBeyondLastDate_checkParameters(
				lastDate );
			if ( this.LastDateTime < lastDate )
				// lastDate comes after the last interval already added
				this.AppendIntervalsButDontGoBeyondLastDate( lastDate );
			ReturnInterval lastInterval =
				this.intervalsSelector.GetNextInterval( this );
			this.Add( lastInterval );
		}
		#endregion AppendIntervalsToGoJustBeyond
		
		/// <summary>
		/// Appends the first interval (starting from firstDate)
		/// </summary>
		/// <param name="firstDate"></param>
		public void AppendFirstInterval(
			DateTime firstDate )
		{
			ReturnInterval firstInterval =
				this.intervalsSelector.GetFirstInterval( firstDate );
			this.Add( firstInterval );
		}
	}
}
