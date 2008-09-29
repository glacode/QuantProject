/*
QuantProject - Quantitative Finance Library

SelectorForMaketClose.cs
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

namespace QuantProject.Business.Financial.Accounting.Reporting
{
	/// <summary>
	/// Selects market close date times, for non week end days
	/// </summary>
	[Serializable]
	public class SelectorForMaketClose : IDateTimeSelectorForEquityLine
	{
		private DateTime currentDateTime;
		
		public SelectorForMaketClose( DateTime startingDateTime )
		{
			if ( HistoricalEndOfDayTimer.IsMarketClose( startingDateTime ) )
				this.currentDateTime = startingDateTime.AddDays( -1 );
			else
				this.currentDateTime = startingDateTime;
		}
		
		public DateTime GetNextDateTime()
		{
			this.currentDateTime =
				this.currentDateTime.AddDays( 1 );
			while (
				( this.currentDateTime.DayOfWeek == DayOfWeek.Saturday ) ||
				( this.currentDateTime.DayOfWeek == DayOfWeek.Sunday ) )
				this.currentDateTime =
					this.currentDateTime.AddDays( 1 );
			this.currentDateTime =
				HistoricalEndOfDayTimer.GetMarketClose( this.currentDateTime );
			return this.currentDateTime;
		}
	}
}
