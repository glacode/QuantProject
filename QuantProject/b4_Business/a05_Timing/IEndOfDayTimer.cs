/*
QuantProject - Quantitative Finance Library

IEndOfDayTimer.cs
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
	public delegate void MarketOpenEventHandler(
	Object sender , EndOfDayTimingEventArgs eventArgs );

	public delegate void FiveMinutesBeforeMarketCloseEventHandler(
	Object sender , EndOfDayTimingEventArgs eventArgs );

	public delegate void MarketCloseEventHandler(
	Object sender , EndOfDayTimingEventArgs eventArgs );

	public delegate void OneHourAfterMarketCloseEventHandler(
	Object sender , EndOfDayTimingEventArgs eventArgs );

	/// <summary>
	/// Interface to be implemented by timers for end of day simulations
	/// </summary>
	public interface IEndOfDayTimer
	{
		event MarketOpenEventHandler MarketOpen;
		event FiveMinutesBeforeMarketCloseEventHandler FiveMinutesBeforeMarketClose;
		event MarketCloseEventHandler MarketClose;
		event OneHourAfterMarketCloseEventHandler OneHourAfterMarketClose;
		EndOfDayDateTime GetCurrentTime();
		/// <summary>
		/// The timer is instructed to begin to fire timing events
		/// </summary>
		void Start();
		void Stop();
	}
}
