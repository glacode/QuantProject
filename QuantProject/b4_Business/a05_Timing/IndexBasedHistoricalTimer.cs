/*
QuantProject - Quantitative Finance Library

IndexBasedHistoricalTimer.cs
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
using System.Collections.Generic;

using QuantProject.ADT.Histories;
using QuantProject.ADT.Timing;
using QuantProject.Data.DataTables;

namespace QuantProject.Business.Timing
{
	/// <summary>
	/// Throws out DateTime(s) at given daily times, but only if the given
	/// index is quoted at those DateTime(s)
	/// </summary>
	public class IndexBasedHistoricalTimer : Timer
	{
		private string indexTicker;
		DateTime firstDateTime;
		DateTime lastDateTime;
		List< Time > dailyTimes;
		
		List< DateTime > dateTimesToBeThrown;
		private int currentDateTimeIndex;
		
		public IndexBasedHistoricalTimer(
			string indexTicker ,
			DateTime firstDateTime ,
			DateTime lastDateTime ,
			List< Time > dailyTimes )
		{
			this.indexTicker = indexTicker;
			this.firstDateTime = firstDateTime;
			this.lastDateTime = lastDateTime;
			this.dailyTimes = dailyTimes;
		}
		
		#region initializeTimer
		
		#region initialize_dateTimesToBeThrown
		private void initialize_dateTimesToBeThrown(
			History dateTimesToBeThrownHistory )
		{
			this.dateTimesToBeThrown = new List< DateTime >();
			foreach ( DateTime dateTime in dateTimesToBeThrownHistory )
				this.dateTimesToBeThrown.Add( dateTime );
		}
		private void initialize_dateTimesToBeThrown()
		{
			History dateTimesToBeThrownHistory =
				Bars.GetMarketDateTimes(
					this.indexTicker , this.firstDateTime , this.lastDateTime ,
					this.dailyTimes );
			this.initialize_dateTimesToBeThrown( dateTimesToBeThrownHistory );
		}
		#endregion initialize_dateTimesToBeThrown
		
		protected override void initializeTimer()
		{
			this.initialize_dateTimesToBeThrown();
			this.currentDateTimeIndex = 0;
			this.currentDateTime = this.dateTimesToBeThrown[ currentDateTimeIndex ];
		}
		#endregion initializeTimer
		
		#region moveNext
		private void checkIfNoMoreDateTimesAreToBeThrown()
		{
			if ( this.currentDateTimeIndex >= this.dateTimesToBeThrown.Count )
				throw new Exception(
					"This timer has no other DateTime(s) to be thrown out. " +
					"This should never happen, the backtest should have invoked the method " +
					"QuantProject.Business.Timing.Timer.Stop() before gettint to this " +
					"point." );
			}
		private void moveNext_actually()
		{
			this.currentDateTimeIndex++;
			this.currentDateTime = this.dateTimesToBeThrown[ currentDateTimeIndex ];
		}
		protected override void moveNext()
		{
			this.checkIfNoMoreDateTimesAreToBeThrown();
			this.moveNext_actually();
		}
		#endregion moveNext
	}
}
