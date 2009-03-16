/*
QuantProject - Quantitative Finance Library

MovingPreviousDateAtClose.cs
Copyright (C) 2009
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

using QuantProject.Business.Timing;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// For any given date time, it is returned the day before at 15:59
	/// </summary>
	public class MovingPreviousDateAtClose : IIntervalBeginFinder
	{
		public MovingPreviousDateAtClose()
		{
		}
		
		public DateTime GetIntervalBeginDateTime( DateTime dateTime )
		{
//			DateTime yesterday = dateTime.AddDays( -1 );
//			DateTime yesterdayAtClose = HistoricalEndOfDayTimer.GetMarketClose( yesterday );
//			DateTime intervalBeginDateTime = yesterdayAtClose.AddMinutes( -1 );
			DateTime intervalBeginDateTime =
				MovingPreviousDateAtClose.GetPreviousDateAtClose( dateTime );
			return intervalBeginDateTime;
		}
		
		public static DateTime GetPreviousDateAtClose( DateTime dateTime )
		{
			DateTime yesterday = dateTime.AddDays( -1 );
			DateTime yesterdayAtClose = HistoricalEndOfDayTimer.GetMarketClose( yesterday );
			DateTime intervalBeginDateTime = yesterdayAtClose.AddMinutes( -1 );
			return intervalBeginDateTime;
		}
	}
}
