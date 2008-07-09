/*
QuantProject - Quantitative Finance Library

DSTPeriod.cs
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
	/// A period when the Daylight Saving Time is applied
	/// </summary>
	public class DSTPeriod
	{
		private DateTime begin;
		private DateTime end;
		
		public DateTime Begin {
			get { return begin; }
		}		
		public DateTime End {
			get { return end; }
		}

		/// <summary>
		/// A period when the Daylight Saving Time is applied
		/// </summary>
		/// <param name="begin">begin of the period</param>
		/// <param name="end">end of the period</param>
		public DSTPeriod( DateTime begin , DateTime end )
		{
			this.checkParameters( begin , end );
			this.begin = begin;
			this.end = end;
		}
		
		#region checkParameters
		private void checkForSunday( DateTime dateTime )
		{
			if ( dateTime.DayOfWeek != DayOfWeek.Sunday )
				throw new Exception(
					"Begin and end of a DSTPeriod must be a Sunday!" );
		}
		private void checkParameters( DateTime begin , DateTime end )
		{
			this.checkForSunday( begin );
			this.checkForSunday( end );
		}
		#endregion checkParameters
	}
}
