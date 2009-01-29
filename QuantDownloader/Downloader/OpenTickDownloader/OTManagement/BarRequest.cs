/*
QuantProject - Quantitative Finance Library

BarRequest.cs
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
	/// A bar to be requested to the OTClient
	/// </summary>
	public class BarRequest
	{
		private string exchange;
		private string symbol;
		private DateTime dateTimeForOpenInUTC;
		private short intervalValueInSeconds;
		
		public string Exchange {
			get { return exchange; }
		}
		public string Symbol {
			get { return symbol; }
		}
		public DateTime DateTimeForOpenInUTC {
			get { return dateTimeForOpenInUTC; }
		}
		public short IntervalValueInSeconds {
			get { return this.intervalValueInSeconds; }
		}
		public BarRequest(
			string exchange ,
			string symbol ,
			DateTime dateTimeForOpenInUTC ,
			short intervalValueInSeconds )
		{
			this.exchange = exchange;
			this.symbol = symbol;
			this.dateTimeForOpenInUTC = dateTimeForOpenInUTC;
			this.intervalValueInSeconds = intervalValueInSeconds;
		}
	}
}
