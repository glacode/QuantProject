/*
QuantProject - Quantitative Finance Library

Timer.cs
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

using QuantProject.ADT;

namespace QuantProject.Business.Timing
{
	public delegate void NewDateTimeEventHandler(
		Object sender , DateTime dateTime );

	/// <summary>
	/// Class to be extended by timers for simulations
	/// </summary>
	[Serializable]
	public abstract class Timer
	{
		public event NewDateTimeEventHandler NewDateTime;
		
		protected bool isActive;	// true iff the timer is started and not stopped
		protected DateTime currentDateTime;
		
		public bool IsActive
		{
			get { return this.isActive; }
		}
		
		public Timer()
		{
		}
		
		protected abstract void initializeTimer();
		protected abstract void moveNext();

		/// <summary>
		/// The last date time sent out
		/// </summary>
		/// <returns></returns>
		public virtual DateTime GetCurrentDateTime()
		{
			if ( !this.isActive )
				throw new Exception(
					"Either Start() has not been invoked yet, or " +
					"Stop() has already been invoked" );
			return this.currentDateTime;
		}

		#region Start
		
		#region activateTimer
		private void activateTimer()
		{
			this.initializeTimer();
			this.isActive = true;
		}
		#endregion activateTimer
		
		/// <summary>
		/// Starts the time walking simulation
		/// </summary>
		private void callEvents()
		{
			if ( this.NewDateTime != null )
				this.NewDateTime(
					this ,
					new DateTime(
						this.currentDateTime.Year ,
						this.currentDateTime.Month ,
						this.currentDateTime.Day ,
						this.currentDateTime.Hour ,
						this.currentDateTime.Minute ,
						this.currentDateTime.Second ) );
			
//			if ( ( this.MarketOpen != null ) && ( this.currentTime.EndOfDaySpecificTime ==
//			                                     EndOfDaySpecificTime.MarketOpen ) )
//				this.MarketOpen( this , new EndOfDayTimingEventArgs( this.currentTime ) );
//			if ( ( this.FiveMinutesBeforeMarketClose != null ) && ( this.currentTime.EndOfDaySpecificTime ==
//			                                                       EndOfDaySpecificTime.FiveMinutesBeforeMarketClose ) )
//				this.FiveMinutesBeforeMarketClose( this , new EndOfDayTimingEventArgs( this.currentTime ) );
//			if ( ( this.MarketClose != null ) && ( this.currentTime.EndOfDaySpecificTime ==
//			                                      EndOfDaySpecificTime.MarketClose ) )
//				this.MarketClose( this , new EndOfDayTimingEventArgs( this.currentTime ) );
//			if ( ( this.OneHourAfterMarketClose != null ) && ( this.currentTime.EndOfDaySpecificTime ==
//			                                                  EndOfDaySpecificTime.OneHourAfterMarketClose ) )
//				this.OneHourAfterMarketClose( this , new EndOfDayTimingEventArgs( this.currentTime ) );
		}
		/// <summary>
		/// The timer is instructed to begin to fire timing events
		/// </summary>
		public virtual void Start()
		{
			this.activateTimer();
			while ( this.isActive )
			{
				this.callEvents();
				this.moveNext();
			}
		}
		#endregion Start
		
		/// <summary>
		/// The timer is instructed to stop to fire timing events
		/// </summary>
		public void Stop()
		{
			this.isActive = false;
		}
	}
}
