/*
QuantProject - Quantitative Finance Library

BarIdentifier.cs
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

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	/// <summary>
	/// Identifies an OpenHighLowClose bar to be requested
	/// (the exchange is not considered)
	/// </summary>
	public class BarIdentifier
	{
		private string ticker;
		private DateTime dateTimeForOpenInNewYorkTimeZone;
		private long barIntervalInSeconds;
		
		public string Ticker {
			get { return this.ticker; }
		}
		public DateTime DateTimeForOpenInNewYorkTimeZone {
			get { return this.dateTimeForOpenInNewYorkTimeZone; }
		}
		public long BarIntervalInSeconds {
			get { return this.barIntervalInSeconds; }
		}

		/// <summary>
		/// Identifies an OpenHighLowClose bar bar to be requested
		/// (the exchange is not considered)
		/// </summary>
		/// <param name="ticker">the ticker the bar is referred to</param>
		/// <param name="dateTimeForOpenInNewYorkTimeZone">date time when the bar begins</param>
		/// <param name="interval">number or seconds between the
		/// bar's open and the bar's close (in other words, the bar's
		/// length, in seconds)</param>
		public BarIdentifier(
			string ticker ,
			DateTime dateTimeForOpenInNewYorkTimeZone ,
			long interval )
		{
			this.ticker = ticker;
			this.dateTimeForOpenInNewYorkTimeZone =
				dateTimeForOpenInNewYorkTimeZone;
			this.barIntervalInSeconds = interval;
		}
	}
}
