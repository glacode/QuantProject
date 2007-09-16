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
using System.Collections;

using QuantProject.Business.Timing;

namespace QuantProject.Business.Strategies.ReturnsManagement.Time
{
	/// <summary>
	/// End of day interval: to be used to compute a strategy return
	/// on the interval. The interval's end must be later than the
	/// interval's begin
	/// </summary>
	public class ReturnInterval
	{
		private EndOfDayDateTime begin;
		private EndOfDayDateTime end;

		public EndOfDayDateTime Begin
		{
			get { return this.begin; }
		}
		public EndOfDayDateTime End
		{
			get { return this.end; }
		}

		/// <summary>
		/// End of day interval: the end must be later than the begin
		/// </summary>
		/// <param name="begin">first interval border</param>
		/// <param name="end">last interval border</param>
		public ReturnInterval( EndOfDayDateTime begin ,
			EndOfDayDateTime end )
		{
			this.checkParameters( begin , end );
			this.begin = begin;
			this.end = end;
		}
		private void checkParameters( EndOfDayDateTime begin ,
			EndOfDayDateTime end )
		{
			if ( begin.CompareTo( end ) >= 0 )
				// begin is equal or greater greater or equal to end
				throw new Exception( "begin must be smaller than end!" ); 
		}
		/// <summary>
		/// True iff for each interval border, there is an EndOfDayDateTime
		/// value in the EndOfDayHistory that is exactly the same
		/// EndOfDayDateTime as the border value
		/// </summary>
		/// <param name="endOfDayHistory"></param>
		/// <returns></returns>
		public bool AreBordersCoveredBy( EndOfDayHistory endOfDayHistory )
		{
			bool areCovered =
				endOfDayHistory.ContainsKey( this.Begin ) &&
				endOfDayHistory.ContainsKey( this.End );
			return areCovered;
		}
		/// <summary>
		/// True iif this Interval begins before endOfDayDateTime
		/// </summary>
		/// <param name="endOfDayDateTime"></param>
		/// <returns></returns>
		public bool BeginsBefore( EndOfDayDateTime endOfDayDateTime )
		{
			bool beginsBefore =
				( this.Begin.CompareTo( endOfDayDateTime ) < 0 );
			return beginsBefore;
		}
	}
}
