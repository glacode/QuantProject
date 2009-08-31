/*
QuantProject - Quantitative Finance Library

MarketDaysManager.cs
Copyright (C) 2009
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
using System.Collections.Generic;

using QuantProject.Data.DataTables;
using QuantProject.Business.Strategies;
using QuantProject.ADT;
using QuantProject.ADT.Timing;
using QuantProject.ADT.Histories;

namespace QuantProject.Business.Timing.TimingManagement
{
	/// <summary>
	/// Class to be used for managing market date times
	/// provided by a given benchmark
	/// </summary>
	[Serializable]
	public class MarketDaysManager : MarketIntervalsManager
	{
		private Time timeOfDay;
		/// <summary>
		/// Creates a MarketDayManager, for managing
		/// operations on the date times a given benchmark
		/// was quoted at
		/// </summary>
		/// <param name="benchmark"></param>
		/// <param name="firstDateTime"></param>
		/// <param name="lastDateTime"></param>
		public MarketDaysManager(Benchmark benchmark,
                             DateTime firstDateTime,
                             DateTime lastDateTime,
                             Time timeOfDay) :
													 base(benchmark,
                                firstDateTime,
                                lastDateTime)
		{
			this.timeOfDay = timeOfDay;
		}
		protected override List<DateTime> getDateTimesHistory()
		{
			History history = Quotes.GetMarketDays(this.benchmark.Ticker,
				                       		this.firstDateTime,
				                       		this.lastDateTime);
			for(int i = 0; i < history.Count; i++)
				history.SetByIndex(i, Time.GetDateTimeFromMerge((DateTime)history.GetByIndex(i), this.timeOfDay));
			return history.DateTimes;
		}
	}
}
