/*
QuantProject - Quantitative Finance Library

HistoricalDataStreamer.cs
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
using System.Collections;

using QuantProject.ADT;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;
using QuantProject.Data.DataProviders.Quotes;

namespace QuantProject.Business.DataProviders
{
	/// <summary>
	/// IDataStreamer implementation using end of day historical data
	/// </summary>
	[Serializable]
	public class HistoricalDataStreamer : IDataStreamer
	{
		private Timer timer;
		private HistoricalMarketValueProvider historicalMarketValueProvider;

		public HistoricalDataStreamer(
			Timer timer ,
			HistoricalMarketValueProvider historicalMarketValueProvider )
		{
			this.timer = timer;
			this.historicalMarketValueProvider = historicalMarketValueProvider;
		}

		/// <summary>
		/// Returns the current bid for the given ticker
		/// </summary>
		/// <param name="ticker"></param>
		/// <returns></returns>
		public double GetCurrentBid( string ticker )
		{
			double currentBid =
				this.historicalMarketValueProvider.GetMarketValue(
					ticker ,
					this.timer.GetCurrentDateTime() );
			return currentBid;
		}

		/// <summary>
		/// Returns the current ask for the given ticker
		/// </summary>
		/// <param name="ticker"></param>
		/// <returns></returns>
		public double GetCurrentAsk( string ticker )
		{
			return this.GetCurrentBid( ticker );
		}

		/// <summary>
		/// true iif the ticker was exchanged at the given date time
		/// </summary>
		/// <param name="ticker"></param>
		/// <returns></returns>
		public bool IsExchanged( string ticker )
		{
			return this.historicalMarketValueProvider.WasExchanged(
				ticker , this.timer.GetCurrentDateTime() );
		}
	}
}
