/*
QuantProject - Quantitative Finance Library

EndOfDayDateTime.cs
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

namespace QuantProject.Business.Timing
{
	/// <summary>
	/// Date time to be used with end of day strategies
	/// </summary>
	public class EndOfDayDateTime : IComparable
	{
		private DateTime dateTime;
		private EndOfDaySpecificTime endOfDaySpecificTime;

		public DateTime DateTime
		{
			get { return dateTime; }
			set { dateTime = value; }
		}

		public EndOfDaySpecificTime EndOfDaySpecificTime
		{
			get { return endOfDaySpecificTime; }
			set { endOfDaySpecificTime = value; }
		}

		public EndOfDayDateTime( DateTime dateTime , EndOfDaySpecificTime endOfDaySpecificTime )
		{
			this.dateTime = dateTime;
			this.endOfDaySpecificTime = endOfDaySpecificTime;
		}

		public BarComponent GetNearestBarComponent()
		{
			BarComponent returnValue = BarComponent.Close;
			switch ( this.endOfDaySpecificTime )
			{
				case EndOfDaySpecificTime.FiveMinutesBeforeMarketClose:
					returnValue = BarComponent.Close;
					break;
				case EndOfDaySpecificTime.MarketClose:
					returnValue = BarComponent.Close;
					break;
				case EndOfDaySpecificTime.MarketOpen:
					returnValue = BarComponent.Open;
					break;
				case EndOfDaySpecificTime.OneHourAfterMarketClose:
					returnValue = BarComponent.Close;
					break;
			}
			return returnValue;
		}
		public ExtendedDateTime GetNearestExtendedDateTime()
		{
			return new ExtendedDateTime( this.dateTime ,
				this.GetNearestBarComponent() );
		}
		public int CompareTo( object endOfDayDateTime )
		{
			int returnValue = -1;
			EndOfDayDateTime toBeCompared = (EndOfDayDateTime)( endOfDayDateTime );
			if ( ( this.DateTime == toBeCompared.DateTime ) &&
				( this.endOfDaySpecificTime == toBeCompared.endOfDaySpecificTime ) )
				// this is the same as the end of day date time to be compared
				returnValue = 0;
			if ( ( this.DateTime > toBeCompared.DateTime ) ||
				( ( this.DateTime == toBeCompared.DateTime ) &&
				( this.endOfDaySpecificTime > toBeCompared.endOfDaySpecificTime ) ) )
				// this is the greater than the end of day date time to be compared
				returnValue = 1;
			return returnValue;
		}
		#region MoveNext
		private	EndOfDaySpecificTime getNextSpecificTime( )
		{
			EndOfDaySpecificTime returnValue;
			Type type = EndOfDaySpecificTime.GetType();
			Array values = EndOfDaySpecificTime.GetValues( type );
			returnValue = ( EndOfDaySpecificTime)values.GetValue( 0 );
			for ( int i = 0 ; i < values.Length - 1 ; i++ )
				if ( this.endOfDaySpecificTime == ( EndOfDaySpecificTime)values.GetValue( i ) )
					returnValue = ( EndOfDaySpecificTime)values.GetValue( i + 1 );
			return returnValue;
		}
		/// <summary>
		/// Moves to the next end of day date time
		/// </summary>
		public void MoveNext()
		{
			EndOfDaySpecificTime nextSpecificTime = this.getNextSpecificTime();
			if ( nextSpecificTime < this.endOfDaySpecificTime )
			{
				// the current end of day specific time is the last end of day specific time in the day
				this.dateTime = this.dateTime.AddDays( 1 );
			}
			this.endOfDaySpecificTime = nextSpecificTime;
		}
		#endregion
	}
}
