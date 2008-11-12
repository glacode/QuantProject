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
	public struct Time : IEquatable<Time> , IComparable<Time>
	{
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

		
		
		#region Equals and GetHashCode implementation
		// The code in this region is useful if you want to use this structure in collections.
		// If you don't need it, you can just remove the region and the ": IEquatable<Time>" declaration.
		
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
	}
}
