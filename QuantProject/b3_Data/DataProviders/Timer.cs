/*
QuantProject - Quantitative Finance Library

Timer.cs
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

using QuantProject.ADT;

namespace QuantProject.Data.DataProviders
{
	public delegate void NewExtendedDateTimeHandler(
		Object sender , NewExtendedDateTimeEventArgs eventArgs );
	/// <summary>
	/// Simulates the time ticking
	/// </summary>
	public class Timer
	{

		private ExtendedDateTime startDateTime;
		private ExtendedDateTime endDateTime;
		private ExtendedDateTime currentDateTime;

		public ExtendedDateTime CurrentDateTime
		{
			get { return this.currentDateTime; }
		}

		public Timer( ExtendedDateTime startDateTime ,
			ExtendedDateTime endDateTime )
		{
			this.startDateTime = startDateTime;
			this.endDateTime = endDateTime;
		}
		public event NewExtendedDateTimeHandler NewExtendedDateTime;

		/// <summary>
		/// Starts the time ticking simulation
		/// </summary>
		public void Start()
		{
			this.currentDateTime = this.startDateTime;
			while ( this.currentDateTime.CompareTo( this.endDateTime ) <= 0 )
			{
				NewExtendedDateTime( this , new NewExtendedDateTimeEventArgs( this.currentDateTime ) );
				this.currentDateTime.MoveNext();
			}
		}
	}
}
