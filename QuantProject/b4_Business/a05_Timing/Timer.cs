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
		
		private bool hasTheTimerBeenStarted;
		private bool hasTheTimerBeenStopped;	// true iff the timer was stop
		protected DateTime currentDateTime;
		
		#region IsDone
		protected abstract bool isDone();
		/// <summary>
		/// true iif all DateTime(s) have been thrown out by this timer
		/// </summary>
		public bool IsDone
		{
			get { return ( this.hasTheTimerBeenStopped || this.isDone() ); }
		}
		#endregion IsDone
		
		public Timer()
		{
			this.hasTheTimerBeenStarted = false;
			this.hasTheTimerBeenStopped = false;
		}
		
		protected abstract void initializeTimer();
		protected abstract void moveNext();

		/// <summary>
		/// The last date time sent out
		/// </summary>
		/// <returns></returns>
		public virtual DateTime GetCurrentDateTime()
		{
			if ( !this.hasTheTimerBeenStarted )
				throw new Exception(
					"Start() has not been invoked yet" );
			if ( this.hasTheTimerBeenStopped )
				throw new Exception(
					"Stop() has already been invoked" );
			return this.currentDateTime;
		}

		#region Start
		
		#region activateTimer
		private void activateTimer()
		{
			this.initializeTimer();
			this.hasTheTimerBeenStarted = true;
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
		}
		private void moveNextIfPossible()
		{
			if ( !this.IsDone )
				// there is at least one more bar to be thrown out, yet
				this.moveNext();
		}
		/// <summary>
		/// The timer is instructed to begin to fire timing events
		/// </summary>
		public virtual void Start()
		{
			this.activateTimer();
			while ( !this.hasTheTimerBeenStopped && !this.IsDone )
			{
				this.callEvents();
				this.moveNextIfPossible();
			}
		}
		#endregion Start
		
		/// <summary>
		/// The timer is instructed to stop to fire timing events
		/// </summary>
		public void Stop()
		{
			this.hasTheTimerBeenStopped = true;
		}
	}
}
