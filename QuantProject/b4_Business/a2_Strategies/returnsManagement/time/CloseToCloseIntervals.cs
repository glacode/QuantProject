/*
QuantProject - Quantitative Finance Library

CloseToCloseIntervals.cs
Copyright (C) 2007
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

namespace QuantProject.Business.Strategies.ReturnsManagement.Time
{
	/// <summary>
	/// Close to close intervals to be used to compute close to close returns
	/// </summary>
	public class CloseToCloseIntervals : ReturnIntervals
	{
		/// <summary>
		/// Creates the close to close intervals for the given benchmark, from
		/// the first DateTime to the last DateTime
		/// </summary>
		/// <param name="firstDateTime"></param>
		/// <param name="lastDateTime"></param>
		/// <param name="benchmark"></param>
		public CloseToCloseIntervals( DateTime firstDateTime ,
			DateTime lastDateTime , string benchmark ) :
			base( firstDateTime , lastDateTime , benchmark )
		{
			
		}
		/// <summary>
		/// Creates the close to close intervals for the given benchmark, from
		/// the first DateTime to the last DateTime:
		/// each interval begins at a given market day "i" and ends at
		/// market day "i + intervalLength"
		/// </summary>
		/// <param name="firstEndOfDayDateTime"></param>
		/// <param name="lastEndOfDayDateTime"></param>
		/// <param name="benchmark"></param>
		/// <param name="intervalLength"></param> 
		public CloseToCloseIntervals( DateTime firstDateTime ,
			DateTime lastDateTime , 
			string benchmark , int intervalLength ) :
			base( firstDateTime , lastDateTime , benchmark ,
			      intervalLength)
		{
			
		}
		
		#region setIntervals
		private void addInterval( int i )
		{
			DateTime dateTimeForIntervalBegin =
				HistoricalEndOfDayTimer.GetMarketClose(
					(DateTime)this.marketDaysForBenchmark.GetKey( i ) );
			DateTime dateTimeForIntervalEnd =
				HistoricalEndOfDayTimer.GetMarketClose(
					(DateTime)this.marketDaysForBenchmark.GetKey( i + this.intervalLength ) );
			ReturnInterval returnInterval = new ReturnInterval(
				dateTimeForIntervalBegin , dateTimeForIntervalEnd );
//				new EndOfDayDateTime( dateTimeForIntervalBegin ,
//				EndOfDaySpecificTime.MarketClose ) ,
//				new EndOfDayDateTime( dateTimeForIntervalEnd ,
//				EndOfDaySpecificTime.MarketClose ) );
			this.Add( returnInterval );
		}
		protected override void setIntervals()
		{
			for( int i = 0 ;
			     i < this.marketDaysForBenchmark.Count - this.intervalLength;
			     i = i + this.intervalLength )
				this.addInterval( i );
		}
		
		#endregion setIntervals
//		private History getTimeLineForOptimization( EndOfDayDateTime now )
//		{
//			DateTime firstInSampleDateForDrivingPositions =
//				now.DateTime.AddDays(
//				-( this.NumberDaysForInSampleOptimization - 1 ) );
//			DateTime lastInSampleOptimizationDate =
//				now.DateTime;
//			return Quotes.GetMarketDays( this.benchmark ,
//				firstInSampleDateForDrivingPositions , lastInSampleOptimizationDate );
//		}
	}
}
