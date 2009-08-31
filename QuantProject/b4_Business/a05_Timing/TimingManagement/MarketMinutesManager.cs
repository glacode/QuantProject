/*
QuantProject - Quantitative Finance Library

MarketMinutesManager.cs
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
using QuantProject.ADT.Histories;

namespace QuantProject.Business.Timing.TimingManagement
{
	/// <summary>
	/// Class to be used for managing market date times
	/// provided by a given benchmark
	/// </summary>
	[Serializable]
	public class MarketMinutesManager : MarketIntervalsManager
	{
		/// <summary>
		/// Creates a MarketMinutesManager, for managing
		/// operations on the date times a given benchmark
		/// was quoted at
		/// </summary>
		/// <param name="benchmark"></param>
		/// <param name="firstDateTime"></param>
		/// <param name="lastDateTime"></param>
		/// <param name="intervalFrameInSeconds">Length in seconds between each Date Time</param>
		public MarketMinutesManager(Benchmark benchmark,
		                             DateTime firstDateTime,
		                             DateTime lastDateTime) : base(benchmark,
		                                   firstDateTime, lastDateTime)
		{
		}
		protected override List<DateTime> getDateTimesHistory()
		{
			History history = Bars.GetMarketDateTimes(this.benchmark.Ticker,
				                        this.firstDateTime,
				                        this.lastDateTime,
				                        60);
			return history.DateTimes;
		}
	}
}


