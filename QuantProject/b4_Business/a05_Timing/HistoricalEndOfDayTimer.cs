/*
QuantProject - Quantitative Finance Library

HistoricalDataStreamer.cs
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
using System.Collections;

using QuantProject.ADT;

namespace QuantProject.Business.Timing
{
	/// <summary>
	/// IDataStreamer implementation using historical data
	/// </summary>
	public class HistoricalEndOfDayTimer : IEndOfDayTimer
	{
		protected bool isActive;	// true iff the timer is started and not stopped

		protected Hashtable tickers;

		protected EndOfDayDateTime currentTime;

		protected EndOfDayDateTime startDateTime;
//		private EndOfDayDateTime endDateTime;

		public EndOfDayDateTime StartDateTime
		{
			get	{	return this.startDateTime;	}
			set	{	this.startDateTime = value;	}
		}

//		public EndOfDayDateTime EndDateTime
//		{
//			get	{	return this.endDateTime;	}
//			set	{	this.endDateTime = value;	}
//		}

		public event MarketOpenEventHandler MarketOpen;
		public event FiveMinutesBeforeMarketCloseEventHandler FiveMinutesBeforeMarketClose;
		public event MarketCloseEventHandler MarketClose;
		public event OneHourAfterMarketCloseEventHandler OneHourAfterMarketClose;

		public HistoricalEndOfDayTimer( EndOfDayDateTime startDateTime )
		{
			this.startDateTime = startDateTime;
//			this.endDateTime = EndDateTime;
			this.tickers = new Hashtable();
		}
    
    protected void callEvents()
    {
      if ( ( this.MarketOpen != null ) && ( this.currentTime.EndOfDaySpecificTime ==
        EndOfDaySpecificTime.MarketOpen ) )
        this.MarketOpen( this , new EndOfDayTimingEventArgs( this.currentTime ) );
      if ( ( this.FiveMinutesBeforeMarketClose != null ) && ( this.currentTime.EndOfDaySpecificTime ==
        EndOfDaySpecificTime.FiveMinutesBeforeMarketClose ) )
        this.FiveMinutesBeforeMarketClose( this , new EndOfDayTimingEventArgs( this.currentTime ) );
      if ( ( this.MarketClose != null ) && ( this.currentTime.EndOfDaySpecificTime ==
        EndOfDaySpecificTime.MarketClose ) )
        this.MarketClose( this , new EndOfDayTimingEventArgs( this.currentTime ) );
      if ( ( this.OneHourAfterMarketClose != null ) && ( this.currentTime.EndOfDaySpecificTime ==
        EndOfDaySpecificTime.OneHourAfterMarketClose ) )
        this.OneHourAfterMarketClose( this , new EndOfDayTimingEventArgs( this.currentTime ) );
    }

    protected virtual void moveNext()
    {
      this.currentTime.MoveNext();
    }
    
    protected virtual void activeTimer()
    {
      this.isActive = true;
      this.currentTime = this.startDateTime.Copy();
    }
		/// <summary>
		/// Starts the time walking simulation
		/// </summary>
		public virtual void Start()
		{
      this.activeTimer();
			while ( this.isActive )
			{
        this.callEvents();
				this.moveNext();
			}
		}

		public EndOfDayDateTime GetCurrentTime()
		{
			return this.currentTime;
		}

		/// <summary>
		/// Stops the timer
		/// </summary>
		public void Stop()
		{
			this.isActive = false;
		}
	}
}
