/*
QuantProject - Quantitative Finance Library

History.cs
Copyright (C) 2003
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
using System.Collections.Generic;
using System.Data;
using QuantProject.ADT;
using QuantProject.ADT.Statistics;


namespace QuantProject.ADT.Histories
{
	/// <summary>
	/// Summary description for History.
	/// </summary>
	[Serializable]
	public class History : AdvancedSortedList
	{
		/// <summary>
		/// Returns the ICollection of DateTimes on which the
		/// history is built
		/// </summary>
		public ICollection TimeLine
		{
			get { return this.Keys; }
		}
		
		/// <summary>
		/// The first DateTime value in the TimeLine
		/// </summary>
		public DateTime FirstDateTime
		{
			get
			{
				if ( this.TimeLine.Count == 0 )
					throw new Exception( "This History collection is empty, " +
					                    "thus FirstDateTime is meaningless!" );
				return (DateTime)(this.GetKey( 0 ));
			}
		}

		/// <summary>
		/// The last DateTime value in the TimeLine
		/// </summary>
		public DateTime LastDateTime
		{
			get
			{
				if ( this.TimeLine.Count == 0 )
					throw new Exception( "This History collection is empty, " +
					                    "thus LastDateTime is meaningless!" );
				int lastIndex = this.TimeLine.Count - 1;
				return (DateTime)( this.GetKey( lastIndex ) );
			}
		}

		public History() : base()
		{
		}
		
		public Object GetValue( DateTime dateTime )
		{
			return this[ dateTime ];
		}

		/// <summary>
		/// Imports DataTable data into this History object
		/// </summary>
		/// <param name="dataTable">Contains the data to be imported</param>
		/// <param name="dateTimeColumnName">Name of the column containing the DateTime keys
		/// to be imported</param>
		/// <param name="valueColumnName">Name of the column containing the values to be imported</param>
		public void Import( DataTable dataTable , string dateTimeColumnName , string valueColumnName )
		{
			foreach (DataRow dataRow in dataTable.Rows )
				this.Add( dataRow[ dateTimeColumnName ] , dataRow[ valueColumnName ] );
		}

		/// <summary>
		/// Returns an history where only common dates are selected
		/// </summary>
		/// <param name="selectingHistory">Provides the relevant dates to be selected</param>
		/// <returns></returns>
		public History Select( History selectingHistory )
		{
			History returnValue = new History();
			foreach ( DateTime dateTime in selectingHistory.Keys )
				if ( this.ContainsKey( dateTime ) )
				returnValue.Add( dateTime , this.GetValue( dateTime ) );
			return returnValue;
		}

		/// <summary>
		/// Returns true iif current history contains all the
		/// dates contained in the given comparingHistory
		/// </summary>
		/// <param name="comparingHistory">Provides the relevant dates to be checked
		/// in the current History</param>
		/// <returns></returns>
		public bool ContainsAllTheDatesIn( History comparingHistory )
		{
			bool returnValue = true;
			foreach ( DateTime dateTime in comparingHistory.Keys )
				if ( ! this.ContainsKey( ExtendedDateTime.GetDate( dateTime ) ) )
				returnValue = false;
			return returnValue;
		}
		
		/// <summary>
		/// Returns true iif current history contains all the
		/// dateTimes contained in the given comparingHistoryOfDateTimes
		/// </summary>
		public bool ContainsAllTheDateTimesIn( History comparingHistoryOfDateTimes )
		{
			bool returnValue = true;
			foreach ( DateTime dateTime in comparingHistoryOfDateTimes.Keys )
				if ( ! this.ContainsKey( dateTime ) )
				returnValue = false;
			return returnValue;
		}
		
		/// <summary>
		/// Returns true iif current history contains at the given percentage the
		/// dateTimes contained in the given comparingHistoryOfDateTimes
		/// </summary>
		public bool ContainsAtAGivenPercentageDateTimesIn( History comparingHistoryOfDateTimes, double percentageOfDateTimes )
		{
			if(percentageOfDateTimes <= 0 || percentageOfDateTimes > 100)
				throw new Exception ("invalid percentage");
			int numberOfContainedDateTimes = 0;
			int numberOfComparingDateTimes = comparingHistoryOfDateTimes.Count;
			foreach ( DateTime dateTime in comparingHistoryOfDateTimes.Keys )
				if ( this.ContainsKey( dateTime ) )
				numberOfContainedDateTimes++;
			
			return numberOfContainedDateTimes >=
				(percentageOfDateTimes * numberOfComparingDateTimes)/100;
		}
		
		public void Interpolate( ICollection dateTimeCollection ,
		                        IInterpolationMethod interpolationMethod )
		{
			foreach ( DateTime dateTime in dateTimeCollection )
				if ( !this.ContainsKey( dateTime ) )
				this.Add( dateTime , interpolationMethod.GetValue( this , dateTime ) );
		}

		/// <summary>
		/// Returns the i-1_th element in the history timeline
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public DateTime GetDateTime( int index )
		{
			return (DateTime)this.GetKey( index );
		}
		/// <summary>
		/// Add an history item, if no collision (contemporary events) is expected
		/// </summary>
		/// <param name="dateTime"></param>
		/// <param name="objectToAdd"></param>
		/// <returns></returns>
		public void Add( DateTime dateTime , Object objectToAdd )
		{
			base.Add( dateTime , objectToAdd );
		}

		/// <summary>
		/// Add an history item when collisions (contemporary events) are possible
		/// </summary>
		/// <param name="dateTime"></param>
		/// <param name="objectToAdd"></param>
		public void MultiAdd( DateTime dateTime , Object objectToAdd )
		{
			if (!this.ContainsKey( dateTime ))
			{
				this[ dateTime ] = new ArrayList();
			}
			((ArrayList)this[ dateTime ]).Add( objectToAdd );
		}

		
		#region MultiplyBy
		private double multiplyBy_getCurrentValue( DateTime key )
		{
			double returnValue;
			try
			{
				returnValue = Convert.ToDouble( this[ key ] );
			}
			catch ( Exception exception )
			{
				string errorMessage = "The current History object contains a value " +
					"that cannot be converted to a double. The wrong value is at the date " +
					key.ToString() + ".\nOriginal error message: " + exception.Message;
				throw new Exception( errorMessage );
			}
			return returnValue;
		}
		/// <summary>
		/// Returns an history where each value is multiplied by the given factor.
		/// The History instance must contain only numeric values
		/// </summary>
		/// <param name="factor"></param>
		/// <returns></returns>
		public History MultiplyBy( double factor )
		{
			if ( factor == 0.0 )
				throw new Exception( "factor can not be equal to zero" );
			History returnValue = new History();
			foreach ( DateTime key in this.Keys )
			{
				double currentValue = this.multiplyBy_getCurrentValue( key );
				returnValue.Add( key , currentValue * factor );
			}
			return returnValue;
		}
		#endregion
		public DateTime GetNextDay( DateTime dateTime )
		{
			if ( this.IndexOfKey( dateTime ) == ( this.Count - 1 ) )
				// it is the last dateTime in the history
				return dateTime.AddDays( 1 );
			else
				return (DateTime) this.GetKey( this.IndexOfKeyOrPrevious( dateTime ) + 1 );
		}
		//millo - fixed method
		public DateTime GetDay( DateTime initialDateTime, int numberOfDaysAhead )
		{
			if ( this.IndexOfKey( initialDateTime ) >= ( this.Count - numberOfDaysAhead ) )
				// initial dateTime + n° of days ahead > the last dateTime in History
			{
				DateTime dateTime;
				dateTime = (DateTime)this.GetKey(this.Count -1);
				return dateTime.AddDays(this.IndexOfKey(initialDateTime) + numberOfDaysAhead - this.Count);
			}
			else
				return (DateTime) this.GetKey( this.IndexOfKeyOrPrevious( initialDateTime ) + numberOfDaysAhead );
		}
		//millo

		#region "GetFunctionHistory"
		
		/// <summary>
		/// Gets a History object based on a statistical available function
		/// </summary>
		/// <remarks>
		/// Each History's item contains a specific statistical function
		/// calculated for each period whose length has to be specified by the user.
		/// The key for the History item is the initial date of each period
		/// </remarks>
		/// <param name="functionToBeCalculated">
		/// Statistical available function to be calculated and stored in the current History object
		/// </param>
		/// <param name="onEachPeriodOf">
		/// Length in day of each period of calculation
		/// </param>
		/// /// <param name="startDateTime">
		/// It sets the start date for the time interval containing the returned History
		/// </param>
		/// /// <param name="endDateTime">
		/// It sets the end date for the time interval containing the returned History
		/// </param>
		/// 
		public History GetFunctionHistory(Function functionToBeCalculated, int onEachPeriodOf,
		                                  DateTime startDateTime , DateTime endDateTime )
		{
			History functionHistory = new History();
			int currentHistoryIndex = this.IndexOfKeyOrPrevious(startDateTime);
			double[] data = new double[onEachPeriodOf];
			//the array contains the set of data whose length is specified by the user
			double periodIndex = 0;
			//in the while statement, if it isn't equal to Floor(currentHistoryIndex/onEachPeriodOf)
			//the current index belongs to the period with periodIndex increased by one
			int cursorThroughDataArray = 0;
			while (
				( currentHistoryIndex < this.Count ) &&
				( ((IComparable)this.GetKey( currentHistoryIndex )).CompareTo( endDateTime ) <= 0 ) )
			{
				

				if (Math.Floor(Convert.ToDouble(currentHistoryIndex/onEachPeriodOf)) == periodIndex &&
				    cursorThroughDataArray < onEachPeriodOf)

					//currentHistoryIndex belongs to the current period
				{
					data[cursorThroughDataArray] = Convert.ToDouble(this.GetByIndex(currentHistoryIndex));
					cursorThroughDataArray++;
					functionHistory.Add(this.GetKey( currentHistoryIndex ), null);
					currentHistoryIndex++;
					
				}
				else
					//currentHistoryIndex doesn't belong to the current period
					//so a new item can be added to the object History to be returned
				{
					cursorThroughDataArray = 0;
					DateTime dateTime = (DateTime)this.GetKey( currentHistoryIndex - onEachPeriodOf);
					switch (functionToBeCalculated)
					{
						case Function.SimpleAverage:
							functionHistory.SetByIndex(currentHistoryIndex - onEachPeriodOf,
							                           BasicFunctions.SimpleAverage(data));
							//functionHistory.Add( dateTime , BasicFunctions.SimpleAverage(data) );
							break;
						case Function.StandardDeviation :
							functionHistory.SetByIndex(currentHistoryIndex - onEachPeriodOf,
							                           BasicFunctions.StdDev(data));
							//functionHistory.Add( dateTime , BasicFunctions.StdDev(data) );
							break;
					}
				}

				periodIndex = Math.Floor(Convert.ToDouble(currentHistoryIndex/onEachPeriodOf));

			}

			return functionHistory;
		}

		#endregion
		
		/// <summary>
		/// It returns true if the current History item value is not null
		/// and is less than the immediate previous History item whose value is not null
		/// </summary>
		/// <param name="dateTime">The date key for current History item</param>
		public bool IsDecreased(DateTime dateTime)
		{
			bool isDecreased = false;
			int index = this.IndexOfKey(dateTime);
			int previousIndex = index - 1;
			if ( index <= 0)
				isDecreased = false;
			else
			{
				if(this.GetByIndex(index) != null)
				{
					while (this.GetByIndex(previousIndex) == null)
					{
						previousIndex --;
					}

					isDecreased = Convert.ToDouble( this.GetByIndex(index)) <
						Convert.ToDouble( this.GetByIndex(previousIndex) );
				}
			}
			return isDecreased;
		}


		#region "GetSimpleMovingAverage( int , DateTime , int )"
		private double currentContributionToCurrentSum(
			int index,
			DateTime dateTime ,
			int numPeriods
		)
		{
			double currentContribution;
			currentContribution = Convert.ToDouble( this.GetByIndex( index ) );
			if ( index >= numPeriods )
				currentContribution -=
					Convert.ToDouble( this.GetByIndex( index - numPeriods ) );
			return currentContribution;
		}
		
		public History GetSimpleMovingAverage( int numPeriods , DateTime startDateTime , DateTime endDateTime )
		{
			History simpleMovingAverage = new History();
			int index = this.IndexOfKeyOrPrevious( startDateTime );
			double currentSum = 0;
			while (
				( index < this.Count ) &&
				( ((IComparable)this.GetKey( index )).CompareTo( endDateTime ) <= 0 ) )
			{
				DateTime dateTime = (DateTime)this.GetKey( index );
				currentSum = currentSum +
					currentContributionToCurrentSum( index , dateTime , numPeriods );
				if ( index < ( numPeriods - 1 ) )
					// current period is not after numPeriods
					simpleMovingAverage.Add( this.GetKey( index ) , null );
				else
				{
					simpleMovingAverage.Add( dateTime , currentSum/numPeriods );
				}
				index++;
			}
			return simpleMovingAverage;
		}
		#endregion

		public History GetSimpleMovingAverage( int numPeriods )
		{
			//      History simpleMovingAverage = new History();
			//      double currentSum = 0;
			//      foreach (DictionaryEntry dictionaryEntry in this)
			//      {
			//        currentSum = currentSum +
			//          currentContributionToCurrentSum( dictionaryEntry , numPeriods );
			//        if ( this.IndexOfKey( dictionaryEntry.Key ) >= numPeriods )
			//        {
			//          simpleMovingAverage.Add( dictionaryEntry.Key , currentSum/numPeriods );
			//        }
			//        else
			//          // current period is not after numPeriods
			//          simpleMovingAverage.Add( dictionaryEntry.Key , null );
			//      }
			//      return simpleMovingAverage;

			return GetSimpleMovingAverage( numPeriods ,
			                              (DateTime) this.GetKey( 0 ) , (DateTime) this.GetKey( this.Count - 1 ) );
		}

		#region "Cross"
		private bool wasLessThan( History history , DateTime dateTime )
		{
			int backStep = 1;
			while ((backStep < this.IndexOfKey( dateTime )) &&
			       (this.GetByIndex(history.IndexOfKey( dateTime )-backStep) != null) &&
			       (history.GetByIndex(history.IndexOfKey( dateTime )-backStep) != null) &&
			       (Convert.ToDouble( this.GetByIndex(this.IndexOfKey( dateTime )-backStep) )==
			        Convert.ToDouble(history.GetByIndex(history.IndexOfKey( dateTime )-backStep)
			                        )))
				backStep ++;
			return
				(this.GetByIndex(history.IndexOfKey( dateTime )-backStep) != null) &&
				(history.GetByIndex(history.IndexOfKey( dateTime )-backStep) != null) &&
				(Convert.ToDouble( this.GetByIndex(this.IndexOfKey( dateTime )-backStep) )<
				 Convert.ToDouble(history.GetByIndex(history.IndexOfKey( dateTime )-backStep)
				                 ));
		}
		public bool Cross( History history , DateTime dateTime )
		{
			bool cross;
			if ( ( this.IndexOfKey( dateTime ) <= 0 ) || ( history.IndexOfKey( dateTime ) <= 0 ) )
				cross = false;
			else
			{
				cross = ( ( Convert.ToDouble( this[ dateTime ] ) ) >
				         ( Convert.ToDouble( history[ dateTime ] ) ) ) &&
					( wasLessThan( history , dateTime ) );
			}
			return cross;
		}
		#endregion

		public List< DateTime > DateTimes
		{
			get
			{
				List<DateTime> dateTimes =
					new List<DateTime>();
				foreach ( DateTime dateTime in this.Keys )
					dateTimes.Add( dateTime );
				return dateTimes;
			}
		}

		
		#region "ToString"
		private string singleToString( DateTime dateTime , Object historyValue )
		{
			return "\nDate: " + dateTime +
				"    Value: " + historyValue.ToString();
		}
		private string dictionaryEntryToString( DictionaryEntry dictionaryEntry )
		{
			string returnString = "";
			if ( dictionaryEntry.Value.GetType() != Type.GetType( "System.Collections.ArrayList" ) )
				// a single value is stored for this DateTime
				returnString = singleToString( (DateTime)dictionaryEntry.Key , dictionaryEntry.Value );
			else
				// possibly multivalues are stored for this DateTime
				foreach (Object historyValue in ((ArrayList)dictionaryEntry.Value) )
				returnString += singleToString( (DateTime)dictionaryEntry.Key , historyValue );
			return returnString;
		}
		public override string ToString()
		{
			string toString = "";
			foreach ( DictionaryEntry dictionaryEntry in this )
				toString += dictionaryEntryToString( dictionaryEntry );
			return toString;
		}
		#endregion

		public void ReportToConsole()
		{
			Console.Write( this.ToString() );
		}
	}
}
