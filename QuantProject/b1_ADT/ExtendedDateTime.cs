/*
QuantProject - Quantitative Finance Library

ExtendedDateTime.cs
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

namespace QuantProject.ADT
{
	/// <summary>
	/// Summary description for DateTime.
	/// </summary>
  public class ExtendedDateTime : IComparable
  {
    private DateTime dateTime;
    private BarComponent barComponent;
    private bool isSimpleDateTime;

    public DateTime DateTime
    {
      get { return dateTime; }
      set { dateTime = value; }
    }

    public BarComponent BarComponent
    {
      get { return barComponent; }
      set { barComponent = value; }
    }

    public bool IsSimpleDateTime
    {
      get { return isSimpleDateTime; }
    }

    public ExtendedDateTime( DateTime dateTime )
    {
      this.dateTime = dateTime;
      this.isSimpleDateTime = true;
    }

    public ExtendedDateTime( DateTime dateTime , BarComponent barComponent )
    {
      this.dateTime = dateTime;
      this.barComponent = barComponent;
      this.isSimpleDateTime = false;
    }

    public int CompareTo( Object barDateTimeToCast )
    {
      ExtendedDateTime extendedDateTime = (ExtendedDateTime) barDateTimeToCast;
      int compareTo = 0;
      if (  ( this.DateTime < extendedDateTime.DateTime ) ||
        ( ( this.DateTime == extendedDateTime.DateTime ) &&
        ( this.barComponent == BarComponent.Open ) &&
        ( extendedDateTime.barComponent == BarComponent.Close ) ) )
        compareTo = -1;
      else
      {
        if ( ( this.DateTime == extendedDateTime.DateTime ) &&
          ( this.barComponent == extendedDateTime.barComponent ) )
          compareTo = 0;
        else
          compareTo = 1;
      }
      return compareTo;
    }

		/// <summary>
		/// Returns the subsequent ExtendedDateTime
		/// </summary>
		/// <returns></returns>
		public ExtendedDateTime MoveNext()
		{
			ExtendedDateTime returnValue;
			if ( this.barComponent == BarComponent.Open )
				returnValue = new ExtendedDateTime( this.dateTime , BarComponent.Close );
			else
				// this.barComponent == BarComponent.Close
				returnValue = new ExtendedDateTime( this.dateTime.AddDays( 1 ) , BarComponent.Open );
			return returnValue;
		}
    public override string ToString()
    {
      return this.DateTime.ToString() + " - " + this.BarComponent.ToString();
    }
//		public static SortedList GetArray( SortedList sortedDateTimes )
//		{
//			SortedList dateTimes = new DateTime[ sortedDateTimes.Count ];
//			for ( int i = 0; i<sortedDateTimes.Count ; i++ )
//			{
//				if ( !(sortedDateTimes.GetByIndex( i ) is DateTime ) )
//					throw( new Exception( "sortedDateTimes is expected to contain only DateTime elements. " +
//						"The element number " + i + " is not a DateTime, instead." ) );
//				else
//				{
//					DateTime dateTime = (DateTime)sortedDateTimes.GetByIndex( i );
//					dateTimes[ i ] = dateTime;
//				}
//			}
//			return dateTimes;
//		}
	}
}
