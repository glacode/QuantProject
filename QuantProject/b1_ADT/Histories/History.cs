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
using System.Data;
using QuantProject.ADT;


namespace QuantProject.ADT.Histories
{
	/// <summary>
	/// Summary description for History.
	/// </summary>
  [Serializable]
  public class History : AdvancedSortedList
  {
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

    public DateTime GetNextDay( DateTime dateTime )
    {
      if ( this.IndexOfKey( dateTime ) == ( this.Count - 1 ) )
        // it is the last dateTime in the history
        return dateTime.AddDays( 1 );
      else
        return (DateTime) this.GetKey( this.IndexOfKeyOrPrevious( dateTime ) + 1 );
    }

	  #region "Millo_1 - SimpleAverage and StandardDeviation"

	  public History GetSimpleAverage( int onEachPeriodOf , DateTime startDateTime , DateTime endDateTime )
	  {
		  History simpleAverage = new History();
		  int index = this.IndexOfKeyOrPrevious(startDateTime); 
		  double[] data = new double[onEachPeriodOf];
		  double checkValue = 0;
		  int i = 0;
		  while (
			  ( index < this.Count ) &&
			  ( ((IComparable)this.GetKey( index )).CompareTo( endDateTime ) <= 0 ) )
		  {
			  DateTime dateTime = (DateTime)this.GetKey( index );
			  if (Math.Floor(index/onEachPeriodOf) == checkValue &&
					i < onEachPeriodOf)
			  {	
				  data[i] = Convert.ToDouble(this.GetByIndex(index));
				  i++;
				  //simpleAverage.Add(this.GetKey( index ), null);
			  }
			  else //Changes the period
			  {	
				  i = 0;
				  simpleAverage.Add( dateTime , Stat.SimpleAverage(data) );
			  }
			  index++;
			  checkValue = Math.Floor(index/onEachPeriodOf);//update checkValue
		  }

		  return simpleAverage;
	  }

	  public History GetStandardDeviation( int onEachPeriodOf , DateTime startDateTime , DateTime endDateTime )
	  {
		  History stdDev = new History();
		  int index = this.IndexOfKeyOrPrevious(startDateTime); 
		  double[] data = new double[onEachPeriodOf];
		  double checkValue = 0;
		  int i = 0;
		  while (
			  ( index < this.Count ) &&
			  ( ((IComparable)this.GetKey( index )).CompareTo( endDateTime ) <= 0 ) )
		  {
			  DateTime dateTime = (DateTime)this.GetKey( index );
			  if (Math.Floor(index/onEachPeriodOf) == checkValue &&
					i < onEachPeriodOf)
			  {	
				  data[i] = Convert.ToDouble(this.GetByIndex(index));
				  i++;
				  //stdDev.Add(this.GetKey( index ), null);
			  }
			  else //Changes the period
			  {	
				  i = 0;
				  stdDev.Add( dateTime , Stat.StdDev (data) );
			  }
			  index++;
			  checkValue = Math.Floor(index/onEachPeriodOf);//update checkValue
		  }

		  return stdDev ;
	  }
	  public bool IsDecreased(DateTime dateTime)
	  {	
		  bool isDecreased;
		  int index = this.IndexOfKey(dateTime);
		  if ( index <= 0 )
			  isDecreased = false;
		  else
		  {
			  isDecreased = Convert.ToDouble( this[ dateTime ]) <
				  Convert.ToDouble( this.GetByIndex(index - 1) );
		  }
		  return isDecreased;
		  
			  
	  }


#endregion



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
