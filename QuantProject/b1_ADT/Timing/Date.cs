/*
QuantProject - Quantitative Finance Library

Date.cs
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
	/// Represents a date (for any possible time)
	/// </summary>
	public struct Date : IEquatable<Date>
	{
		DateTime standardDateTime;
		
		public int Year
		{
			get { return this.standardDateTime.Year; }
		}
		public int Month
		{
			get { return this.standardDateTime.Month; }
		}
		public int Day
		{
			get { return this.standardDateTime.Day; }
		}
		
		// Represents a date (for any possible time)
		public Date( DateTime dateTime )
		{
			this.standardDateTime =
				new DateTime(
					dateTime.Year , dateTime.Month , dateTime.Day , 0 , 0 , 0 );
		}
		
		// Represents a date (for any possible time)
		public Date( int year , int month , int day )
		{
			this.standardDateTime =
				new DateTime( year , month , day , 0 , 0 , 0 );
		}
		
		public Date AddDays( int days )
		{
			DateTime standardDateTimeForResult = this.standardDateTime.AddDays( days );
			Date date = new Date( standardDateTimeForResult );
			return date;
		}

		
		#region Equals and GetHashCode implementation
		// The code in this region is useful if you want to use this structure in collections.
		// If you don't need it, you can just remove the region and the ": IEquatable<Date>" declaration.
		
		public override bool Equals(object obj)
		{
			if (obj is Date)
				return Equals((Date)obj); // use Equals method below
			else
				return false;
		}
		
		public bool Equals(Date other)
		{
			// add comparisions for all members here
			return this.standardDateTime == other.standardDateTime;
		}
		
		public override int GetHashCode()
		{
			// combine the hash codes of all members here (e.g. with XOR operator ^)
			return standardDateTime.GetHashCode();
		}
		
		public static bool operator ==(Date lhs, Date rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(Date lhs, Date rhs)
		{
			return !(lhs.Equals(rhs)); // use operator == and negate result
		}
		
		public static bool operator <=(Date lhs, Date rhs)
		{
			bool isLessThanOrEqual =
				( lhs.standardDateTime <= rhs.standardDateTime );
			return isLessThanOrEqual;
		}
		
		public static bool operator >=(Date lhs, Date rhs)
		{
			bool isGreaterThanOrEqual =
				( lhs.standardDateTime >= rhs.standardDateTime );
			return isGreaterThanOrEqual;
		}
		
		public static bool operator <(Date lhs, Date rhs)
		{
			bool isLessThan =
				( lhs.standardDateTime < rhs.standardDateTime );
			return isLessThan;
		}
		
		public static bool operator >(Date lhs, Date rhs)
		{
			bool isGreaterThan =
				( lhs.standardDateTime > rhs.standardDateTime );
			return isGreaterThan;
		}
		#endregion
	}
}
