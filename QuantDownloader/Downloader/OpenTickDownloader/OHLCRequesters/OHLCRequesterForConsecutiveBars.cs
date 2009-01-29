/*
QuantProject - Quantitative Finance Library

OHLCRequesterForConsecutiveBars.cs
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
	/// Throws out a single OHLCRequest for every ticker in the given array: such request requests
	/// all available bars in the given period
	/// </summary>
	public class OHLCRequesterForConsecutiveBars : IOHLCRequester
	{
		private string[] tickersToBeDownloaded;
		private DateTime dateTimeForFirstBarOpenInNewYorkTimeZone;
		private DateTime dateTimeForLastBarOpenInNewYorkTimeZone;
		private int barIntervalInSeconds;
		
		private int indexForTheNextTickerToRequestFor;
		
		public bool AreAllOHLCRequestsAlredyGiven
		{
			get
			{
				bool areAllAlreadyGiven =
					( indexForTheNextTickerToRequestFor >= this.tickersToBeDownloaded.Length );
				return areAllAlreadyGiven;
			}
		}
		
		/// <summary>
		/// Throws out a single OHLCRequest for every ticker in the given array: such request requests
		/// all available bars in the given period
		/// </summary>
		/// <param name="ticker"></param>
		/// <returns></returns>
		public OHLCRequesterForConsecutiveBars(
			string[] tickersToDownload ,
			DateTime dateTimeForFirstBarOpenInNewYorkTimeZone ,
			DateTime dateTimeForLastBarOpenInNewYorkTimeZone ,
			int barIntervalInSeconds )
		{
			this.tickersToBeDownloaded = tickersToDownload;
			this.dateTimeForFirstBarOpenInNewYorkTimeZone =
				dateTimeForFirstBarOpenInNewYorkTimeZone;
			this.dateTimeForLastBarOpenInNewYorkTimeZone =
				dateTimeForLastBarOpenInNewYorkTimeZone;
			this.barIntervalInSeconds = barIntervalInSeconds;
			
			this.indexForTheNextTickerToRequestFor = 0;
		}
		
		#region GetNextOHLCRequest
		private OHLCRequest getOHLCRequest( string ticker )
		{
			OHLCRequest ohlcRequest = new OHLCRequest(
				ticker ,
				this.dateTimeForFirstBarOpenInNewYorkTimeZone ,
				this.dateTimeForLastBarOpenInNewYorkTimeZone ,
				this.barIntervalInSeconds );
			return ohlcRequest;
		}
		public OHLCRequest GetNextOHLCRequest()
		{
			string ticker = this.tickersToBeDownloaded[ this.indexForTheNextTickerToRequestFor ];
			OHLCRequest ohlcRequest = this.getOHLCRequest( ticker );
			this.indexForTheNextTickerToRequestFor++;
			return ohlcRequest;
		}
		#endregion GetNextOHLCRequest
	}
}
