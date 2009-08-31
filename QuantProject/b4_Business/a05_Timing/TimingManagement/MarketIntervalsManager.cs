/*
QuantProject - Quantitative Finance Library

MarketIntervalsManager.cs
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
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.ADT;
using QuantProject.ADT.Histories;

namespace QuantProject.Business.Timing.TimingManagement
{
	/// <summary>
	/// Class to be used for managing market intervals
	/// provided by a given benchmark
	/// </summary>
	[Serializable]
	public abstract class MarketIntervalsManager
	{
		protected Benchmark benchmark;
		protected List<DateTime> marketDateTimesForBenchmark;
		protected DateTime firstDateTime;
		protected DateTime lastDateTime;
		
		public Benchmark Benchmark
		{
			get	{	return this.benchmark;	}
		}
		
		/// <summary>
		/// Creates a MarketIntervalsManager, for managing
		/// operations on the date times a given benchmark
		/// was quoted at
		/// </summary>
		/// <param name="benchmark"></param>
		/// <param name="firstDateTime"></param>
		/// <param name="lastDateTime"></param>
		public MarketIntervalsManager(Benchmark benchmark,
		                             DateTime firstDateTime,
		                             DateTime lastDateTime)
		{
			this.benchmark = benchmark;
			this.firstDateTime = firstDateTime;
			this.lastDateTime = lastDateTime;
		}
		
		protected abstract List<DateTime> getDateTimesHistory();
		
		private int addMarketIntervals_getIndexOfStartingDateTime( DateTime startingDateTime, 
		                                                           int marketIntervalsToAdd )
		{
			int returnValue;
			if( this.marketDateTimesForBenchmark.Contains( startingDateTime ) )
			   returnValue = this.marketDateTimesForBenchmark.IndexOf( startingDateTime );
			else
				throw new Exception("The benchmark '" +
				                    this.benchmark.Ticker + "' is not quoted at the given startingDateTime '" +
				                    startingDateTime.ToShortDateString() + " " +
				                    startingDateTime.ToShortTimeString() + "'!");
			return returnValue;
		}
		
//		public ReturnIntervals GetReturnIntervals(DateTime firstDateTime,
//		                                  				DateTime lastDateTime,
//		                                  			  int numberOfMarketIntervalsForEachReturnInterval)
//		{
//			if( this.marketDateTimesForBenchmark == null )
//				this.marketDateTimesForBenchmark = this.getDateTimesHistory();
//			ReturnIntervals returnValue = new ReturnIntervals();
//			foreach(DateTime dateTime in this.marketDateTimesForBenchmark)
//				if(dateTime >= firstDateTime && dateTime <= lastDateTime)
//					returnValue.Add(dateTime, dateTime);
//			
//			return returnValue;
//		}
		
		public DateTime AddMarketIntervals(DateTime startingDateTime,
		                                 	 int marketIntervalsToAdd)
		{
			if(this.marketDateTimesForBenchmark == null)
				this.marketDateTimesForBenchmark = this.getDateTimesHistory();
			if(this.marketDateTimesForBenchmark.Count == 0)
				throw new Exception("The given benchmark is not quoted at no " +
				                    "date time between firstDateTime and lastDateTime");
			DateTime returnValue;
			int indexForStartingDateTime = 
				this.addMarketIntervals_getIndexOfStartingDateTime( startingDateTime, 
				                                                    marketIntervalsToAdd );
			int indexOfReturnValue = indexForStartingDateTime + marketIntervalsToAdd;
			if ( indexOfReturnValue < 0 || 
			     indexOfReturnValue >= this.marketDateTimesForBenchmark.Count )
				throw new OutOfRangeException("parameter marketPeriodsToAdd",
				                              -indexForStartingDateTime,
				                              this.marketDateTimesForBenchmark.Count - marketIntervalsToAdd - 1);
			else
				returnValue = this.marketDateTimesForBenchmark[ indexOfReturnValue ];
			
			return returnValue;
		}
	}
}

