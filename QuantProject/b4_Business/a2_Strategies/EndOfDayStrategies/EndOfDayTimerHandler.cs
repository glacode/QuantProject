/*
QuantProject - Quantitative Finance Library

EndOfDayStrategy.cs
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

using QuantProject.Business.Timing;

namespace QuantProject.Business.Strategies
{
	/// <summary>
	/// Abstract class to be extended by end of day timer handlers
	/// </summary>
	[Serializable]
	public abstract class EndOfDayTimerHandler
	{
		public EndOfDayTimerHandler()
		{
		}
		
		#region NewDateTimeEventHandler
		protected abstract void marketOpenEventHandler(
			Object sender , DateTime dateTime )
			;
		protected abstract void marketCloseEventHandler(
			Object sender , DateTime dateTime )
			;
		protected abstract void oneHourAfterMarketCloseEventHandler(
			Object sender , DateTime dateTime )
			;
		public virtual void NewDateTimeEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( HistoricalEndOfDayTimer.IsMarketOpen( dateTime ) )
				this.marketOpenEventHandler( sender , dateTime );
			if ( HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
				this.marketCloseEventHandler( sender , dateTime );
			if ( HistoricalEndOfDayTimer.IsOneHourAfterMarketClose( dateTime ) )
				this.oneHourAfterMarketCloseEventHandler( sender , dateTime );
		}
		#endregion NewDateTimeEventHandler
	}
}
