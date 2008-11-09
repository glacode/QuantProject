/*
QuantProject - Quantitative Finance Library

OpenToOpenIntervals.cs
Copyright (C) 2008
Marco Milletti

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

namespace QuantProject.Business.Strategies.ReturnsManagement.Time
{
	/// <summary>
	/// Open to Open intervals to be used to compute open to open returns
	/// </summary>
	[Serializable]
	public class OpenToOpenIntervals : ReturnIntervals
	{
		/// <summary>
		/// Creates the open to open intervals for the given benchmark, from
		/// the first DateTime to the last DateTime
		/// </summary>
		/// <param name="firstEndOfDayDateTime"></param>
		/// <param name="lastEndOfDayDateTime"></param>
		/// <param name="benchmark"></param>
		public OpenToOpenIntervals( DateTime firstDateTime ,
			DateTime lastDateTime , string benchmark ) :
			base( firstDateTime , lastDateTime , benchmark )
		{
			
		}
		/// <summary>
		/// Creates the open to open intervals for the given benchmark, from
		/// the first DateTime to the last DateTime:
		/// each interval begins at a given market day "i" and ends at
		/// market day "i + intervalLength"
		/// </summary>
		/// <param name="firstEndOfDayDateTime"></param>
		/// <param name="lastEndOfDayDateTime"></param>
		/// <param name="benchmark"></param>
		/// <param name="intervalLength"></param> 
		public OpenToOpenIntervals( DateTime firstDateTime ,
			DateTime lastDateTime , 
			string benchmark , int intervalLength ) :
			base( firstDateTime , lastDateTime , benchmark ,
			      intervalLength)
		{
			
		}
		
		private void addInterval( int i )
		{
			DateTime dateTimeForIntervalBegin =
				HistoricalEndOfDayTimer.GetMarketOpen(
					(DateTime)this.marketDaysForBenchmark.GetKey( i ) );
			DateTime dateTimeForIntervalEnd =
				HistoricalEndOfDayTimer.GetMarketOpen(
					(DateTime)this.marketDaysForBenchmark.GetKey( i + this.intervalLength ) );
			ReturnInterval returnInterval = new ReturnInterval(
				dateTimeForIntervalBegin , dateTimeForIntervalEnd );
//				new EndOfDayDateTime( dateTimeForIntervalBegin ,
//				EndOfDaySpecificTime.MarketOpen ) ,
//				new EndOfDayDateTime( dateTimeForIntervalEnd ,
//				EndOfDaySpecificTime.MarketOpen ) );
			this.Add( returnInterval );
		}
		protected override void setIntervals()
		{
			for( int i = 0 ;
			     i < this.marketDaysForBenchmark.Count - this.intervalLength;
			     i = i + this.intervalLength )
				this.addInterval( i );
		}
	}
}
