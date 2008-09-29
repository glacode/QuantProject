/*
QuantProject - Quantitative Finance Library

ReturnInterval.cs
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

using QuantProject.ADT;
using QuantProject.ADT.Histories;

namespace QuantProject.Business.Strategies.ReturnsManagement.Time
{
	/// <summary>
	/// Time interval: to be used to compute a strategy return
	/// on the interval. The interval's end must be later than the
	/// interval's begin
	/// </summary>
	[Serializable]
	public class ReturnInterval
	{
		private DateTime begin;
		private DateTime end;

		public DateTime Begin
		{
			get { return this.begin; }
		}
		public DateTime End
		{
			get { return this.end; }
		}

		/// <summary>
		/// Time interval: the end must be later than the begin
		/// </summary>
		/// <param name="begin">first interval border</param>
		/// <param name="end">last interval border</param>
		public ReturnInterval( DateTime begin , DateTime end )
		{
			this.checkParameters( begin , end );
			this.begin = ExtendedDateTime.Copy( begin );
			this.end = ExtendedDateTime.Copy( end );
		}
		private void checkParameters( DateTime begin ,
			DateTime end )
		{
			if ( begin.CompareTo( end ) >= 0 )
				// begin is equal or greater greater or equal to end
				throw new Exception( "begin must be smaller than end!" ); 
		}
		/// <summary>
		/// True iff for each interval border, there is a DateTime
		/// value in the History that is exactly the same
		/// DateTime as the border value
		/// </summary>
		/// <param name="endOfDayHistory"></param>
		/// <returns></returns>
		public bool AreBordersCoveredBy( History history )
		{
			bool areCovered =
				history.ContainsKey( this.Begin ) &&
				history.ContainsKey( this.End );
			return areCovered;
		}
		/// <summary>
		/// True iif this Interval begins before dateTime
		/// </summary>
		/// <param name="endOfDayDateTime"></param>
		/// <returns></returns>
		public bool BeginsBefore( DateTime dateTime )
		{
			bool beginsBefore =
				( this.Begin.CompareTo( dateTime ) < 0 );
			return beginsBefore;
		}
	}
}
