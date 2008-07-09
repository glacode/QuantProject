/*
QuantProject - Quantitative Finance Library

Bar.cs
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
	/// A single Bar
	/// </summary>
	public struct Bar
	{
		private string ticker;
		private string exchange;
		private DateTime dateTimeForOpenInUTCTime;
		private long interval;
		private double open;
		private double high;
		private double low;
		private double close;
		private long volume;
		
		public string Ticker
		{
			get { return this.ticker; }
		}

		public string Exchange
		{
			get { return this.exchange; }
		}
		public DateTime DateTimeForOpenInUTCTime
		{
			get { return this.dateTimeForOpenInUTCTime; }
		}
		public long Interval
		{
			get { return this.interval; }
		}
		public double Open
		{
			get { return this.open; }
		}
		public double High
		{
			get { return this.high; }
		}
		public double Low
		{
			get { return this.low; }
		}
		public double Close
		{
			get { return this.close; }
		}
		
		public long Volume
		{
			get { return this.volume; }
		}

		public Bar(
			string ticker ,
			string exchange ,
			DateTime dateTimeForOpenInUTCTime ,
			long interval ,
			double open ,
			double high ,
			double low ,
			double close ,
			long volume	)
		{
			this.ticker = ticker;
			this.exchange = exchange;
			this.dateTimeForOpenInUTCTime = dateTimeForOpenInUTCTime;
			this.interval = interval;
			this.open = open;
			this.high = high;
			this.low = low;
			this.close = close;
			this.volume = volume;
		}
	}
}
