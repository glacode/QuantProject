/*
QuantProject - Quantitative Finance Library

OHLCRequest.cs
Copyright (C) 2009
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

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	/// <summary>
	/// Identifies a set of consecutive OpenHighLowClose bar to be requested
	/// (the exchange is not considered)
	/// </summary>
	public class OHLCRequest
	{
		private string ticker;
		private DateTime dateTimeForFirstBarOpenInNewYorkTimeZone;
		private DateTime dateTimeForLastBarOpenInNewYorkTimeZone;
		private long barIntervalInSeconds;
		
		public string Ticker {
			get { return this.ticker; }
		}
		/// <summary>
		/// date time for the first bar requested
		/// </summary>
		public DateTime DateTimeForFirstBarOpenInNewYorkTimeZone {
			get { return this.dateTimeForFirstBarOpenInNewYorkTimeZone; }
		}
		/// <summary>
		/// date time for the last bar requested
		/// </summary>
		public DateTime DateTimeForLastBarOpenInNewYorkTimeZone {
			get { return this.dateTimeForLastBarOpenInNewYorkTimeZone; }
		}
		public long BarIntervalInSeconds {
			get { return this.barIntervalInSeconds; }
		}

		/// <summary>
		/// Identifies an OpenHighLowClose bar bar to be requested
		/// (the exchange is not considered)
		/// </summary>
		/// <param name="ticker">the ticker the bar is referred to</param>
		/// <param name="dateTimeForFirstBarOpenInNewYorkTimeZone">date time when the first bar begins</param>
		/// <param name="dateTimeForLastBarOpenInNewYorkTimeZone">date time when the last bar begins</param>
		/// <param name="barIntervalInSeconds">number or seconds between the
		/// bar's open and the bar's close (in other words, the bar's
		/// length, in seconds)</param>
		public OHLCRequest(
			string ticker ,
			DateTime dateTimeForFirstBarOpenInNewYorkTimeZone ,
			DateTime dateTimeForLastBarOpenInNewYorkTimeZone ,
			long barIntervalInSeconds )
		{
			this.ticker = ticker;
			this.dateTimeForFirstBarOpenInNewYorkTimeZone =
				dateTimeForFirstBarOpenInNewYorkTimeZone;
			this.dateTimeForLastBarOpenInNewYorkTimeZone =
				dateTimeForLastBarOpenInNewYorkTimeZone;
			this.barIntervalInSeconds = barIntervalInSeconds;
		}
	}
}
