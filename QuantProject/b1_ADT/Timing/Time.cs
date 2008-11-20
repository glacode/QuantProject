/*
QuantProject - Quantitative Finance Library

Time.cs
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

namespace QuantProject.ADT.Timing
{
	/// <summary>
	/// Represent a time (for any possible date)
	/// </summary>
	[Serializable]
	public struct Time : IEquatable<Time> , IComparable<Time>
	{
		#region IsValidTimeFormat
		public static bool IsValidTimeFormat ( string stringRepresentingTime )
		{
			bool returnValue = false;
			char[] chars = stringRepresentingTime.ToCharArray();
			if( 
			   //first - hour
			   (chars[0]=='0' || chars[0]=='1' || chars[0]=='2') &&
			   //second - hour
			   ( (chars[0]=='0' || chars[0]=='1') &&
			   		(chars[1]=='0' || chars[1]=='1' || chars[1]=='2' ||
			    	chars[1]=='3' || chars[1]=='4' || chars[1]=='5' ||
			    	chars[1]=='6' || chars[1]=='7' || chars[1]=='8' || chars[1]=='9')	||
			     	chars[0] == '2' && 
			     	(chars[1]=='0' || chars[1]=='1' || chars[1]=='2' ||
			    	chars[1]=='3' || chars[1]=='4') ) &&
			   //third - separator
			   (chars[2]==':') &&
			   //fourth - minute
				 (chars[3]=='0' || chars[3]=='1' || chars[3]=='2' ||
			    chars[3]=='3' || chars[3]=='4' || chars[3]=='5') &&
			   //fifth - minute
				 (chars[4]=='0' || chars[4]=='1' || chars[4]=='2' ||
			    	chars[4]=='3' || chars[4]=='4' || chars[4]=='5' ||
			    	chars[4]=='6' || chars[4]=='7' || chars[4]=='8' || chars[4]=='9') &&
			   //sixth - separator
			   (chars[5]==':') &&
			   //seventh - second
				 (chars[6]=='0' || chars[6]=='1' || chars[6]=='2' ||
			    chars[6]=='3' || chars[6]=='4' || chars[6]=='5') &&
			   //eighth - second
				 (chars[7]=='0' || chars[7]=='1' || chars[7]=='2' ||
			    	chars[7]=='3' || chars[7]=='4' || chars[7]=='5' ||
			    	chars[7]=='6' || chars[7]=='7' || chars[7]=='8' || chars[7]=='9') 
			  )
					returnValue = true;
			
			return returnValue;
		}
		#endregion isValidTimeFormat
		
		
		/// <summary>
		/// Returns a new DateTime, having the date as the given dateTime
		/// and the time as the given time
		/// </summary>
		/// <param name="dateTime">dateTime containing the date to merge with the given time in a new DateTime</param>
		/// /// <param name="time">time containing the hour, minute and second to merge with the given dateTime in a new DateTime</param>
		/// <returns></returns>
		public static DateTime GetDateTimeFromMerge ( DateTime dateTime, Time time )
		{
			return new DateTime( dateTime.Year , dateTime.Month , dateTime.Day,
			                     time.Hour , time.Minute , time.Second );
		}
		
		
		private DateTime standardDateTime;
		
		public int Hour
		{
			get { return this.standardDateTime.Hour; }
		}
		
		public int Minute
		{
			get { return this.standardDateTime.Minute; }
		}

		public int Second
		{
			get { return this.standardDateTime.Second; }
		}

		public Time( int hour , int minute , int second )
		{
			this.standardDateTime = new DateTime(
				1900 , 1 , 1 , hour , minute , second );
		}

		public Time( DateTime dateTime )
		{
			this.standardDateTime = new DateTime(
				1900 , 1 , 1 ,
				dateTime.Hour , dateTime.Minute , dateTime.Second );
		}
		
		private DateTime time_getStandardTimeFromString ( string stringRepresentingTime )
		{
			int hour = Convert.ToInt32(stringRepresentingTime.Substring(0,2));
			int minute = Convert.ToInt32(stringRepresentingTime.Substring(3,2));
			int second = Convert.ToInt32(stringRepresentingTime.Substring(6,2));
			return new DateTime(1900, 1, 1, hour, minute, second);
		}
		
		private void time_checkParameter ( string stringRepresentingTime )
		{
			if( stringRepresentingTime.Length != 8 || 
			    !Time.IsValidTimeFormat( stringRepresentingTime ) )
				throw new Exception("string is not in the requested time-format hh:mm:ss");
		}
		
		/// <summary>
		/// Represent a time (for any possible date)
		/// </summary>
		/// <param name="stringRepresentingTime">String representing time in format: hh:mm:ss</param>
		public Time( string stringRepresentingTime )
		{
			this.standardDateTime = new DateTime(1900,1,1,0,0,0);
			//just for compiling next two lines
			this.time_checkParameter( stringRepresentingTime );
			this.standardDateTime = 
				this.time_getStandardTimeFromString( stringRepresentingTime );
		}
		
		#region Equals and GetHashCode implementation
		// The code in this region is useful if you want to use this structure in collections.
		// If you don't need it, you can just remove the region and the ": IEquatable<Time>" declaration.
		public override bool Equals(object obj)
		{
			if (obj is Time)
				return Equals((Time)obj); // use Equals method below
			else
				return false;
		}
		
		public bool Equals(Time other)
		{
			// add comparisions for all members here
			return this.standardDateTime == other.standardDateTime;
		}
		
		public override int GetHashCode()
		{
			// combine the hash codes of all members here (e.g. with XOR operator ^)
			return standardDateTime.GetHashCode();
		}
		
		public static bool operator ==(Time lhs, Time rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(Time lhs, Time rhs)
		{
			return !(lhs.Equals(rhs)); // use operator == and negate result
		}
		#endregion Equals and GetHashCode implementation
		
		public static bool operator <=(Time lhs, Time rhs)
		{
			bool isLessThanOrEqual =
				( lhs.standardDateTime <= rhs.standardDateTime );
			return isLessThanOrEqual;
		}
		
		public static bool operator >=(Time lhs, Time rhs)
		{
			bool isGreaterThanOrEqual =
				( lhs.standardDateTime >= rhs.standardDateTime );
			return isGreaterThanOrEqual;
		}
		
		public static bool operator <(Time lhs, Time rhs)
		{
			bool isLessThan =
				( lhs.standardDateTime < rhs.standardDateTime );
			return isLessThan;
		}
		
		public static bool operator >(Time lhs, Time rhs)
		{
			bool isGreaterThan =
				( lhs.standardDateTime > rhs.standardDateTime );
			return isGreaterThan;
		}
		
		public int CompareTo(Time other)
		{
			int compareTo = 0;
			if ( this < other )
				compareTo = -1;
			if ( this > other )
				compareTo = 1;
			return compareTo;
		}
		
		/// <summary>
		/// True iff the time of the given dateTime is equal to
		/// the current instance
		/// </summary>
		/// <param name="dateTime">dateTime containing time to compare to the current instance</param>
		/// <returns></returns>
		public bool HasTheSameTime( DateTime dateTime )
		{
			bool returnValue =
				( this.Hour == dateTime.Hour ) &&
				( this.Minute == dateTime.Minute ) &&
				( this.Second == dateTime.Second );
			return returnValue;
		}
	}
}
