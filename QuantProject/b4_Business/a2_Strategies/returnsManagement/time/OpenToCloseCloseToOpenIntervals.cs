/*
QuantProject - Quantitative Finance Library

OpenToCloseCloseToOpenIntervals.cs
Copyright (C) 2007
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
	/// Open to Close - Close to Open intervals, to be used
	/// to compute returns where an open to close return is followed
	/// by the next close to open return, and this is followed
	/// by the next open to close return, and so on
	/// </summary>
	[Serializable]
	public class OpenToCloseCloseToOpenIntervals : ReturnIntervals
	{
		/// <summary>
		/// Creates the OTC-CTO intervals for the given benchmark, from
		/// the first DateTime to the last DateTime
		/// </summary>
		/// <param name="firstEndOfDayDateTime"></param>
		/// <param name="lastEndOfDayDateTime"></param>
		/// <param name="benchmark"></param>
		public OpenToCloseCloseToOpenIntervals( DateTime firstDateTime ,
			DateTime lastDateTime , string benchmark ) :
			base( firstDateTime , lastDateTime , benchmark )
		{
		}
		#region setIntervals
		private void addInterval( int i )
		{
			//adds the open to close interval
			DateTime dateTimeForOTC =
				(DateTime)this.marketDaysForBenchmark.GetKey( i );
			ReturnInterval returnOTCInterval = new ReturnInterval(
				HistoricalEndOfDayTimer.GetMarketOpen( dateTimeForOTC ) ,
				HistoricalEndOfDayTimer.GetMarketClose( dateTimeForOTC ) );
//				new EndOfDayDateTime( dateTimeForOTC ,
//				EndOfDaySpecificTime.MarketOpen ) ,
//				new EndOfDayDateTime( dateTimeForOTC ,
//				EndOfDaySpecificTime.MarketClose ) );
			this.Add( returnOTCInterval );
			//adds the following close to open interval
			DateTime dateTimeForCTOEnd = 
				(DateTime)this.marketDaysForBenchmark.GetKey( i + 1 );
			ReturnInterval returnCTOInterval = new ReturnInterval(
				HistoricalEndOfDayTimer.GetMarketClose( dateTimeForOTC ) ,
				HistoricalEndOfDayTimer.GetMarketOpen( dateTimeForCTOEnd ) );
//				new EndOfDayDateTime( dateTimeForOTC ,
//				EndOfDaySpecificTime.MarketClose ) ,
//				new EndOfDayDateTime( dateTimeForCTOEnd ,
//				EndOfDaySpecificTime.MarketOpen ) );
			this.Add( returnCTOInterval );
		}
		protected override void setIntervals()
		{
			for( int i = 0 ; i < marketDaysForBenchmark.Count - 1 ; i++ )
				this.addInterval( i );
		}
		
		#endregion setIntervals
	}
}
