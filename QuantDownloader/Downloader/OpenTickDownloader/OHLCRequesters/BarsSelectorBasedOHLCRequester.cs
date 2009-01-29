/*
QuantProject - Quantitative Finance Library

BarsSelectorBasedOHLCRequester.cs
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
	/// Throws out a single OHLCRequest for every single bar selected by
	/// the IBarSelector
	/// </summary>
	public class BarsSelectorBasedOHLCRequester : IOHLCRequester
	{
		private IBarsSelector barsSelector;
		
		public bool AreAllOHLCRequestsAlredyGiven
		{
			get
			{
				bool areAllAlreadyGiven = this.barsSelector.AreAllBarsAlredyGiven;
				return areAllAlreadyGiven;
			}
		}
		
		public BarsSelectorBasedOHLCRequester( IBarsSelector barsSelector )
		{
			this.barsSelector = barsSelector;
		}
		
		public OHLCRequest GetNextOHLCRequest()
		{
			BarIdentifier barIdentifier = this.barsSelector.GetNextBarIdentifier();
			OHLCRequest ohlcRequest = new OHLCRequest(
				barIdentifier.Ticker ,
				barIdentifier.DateTimeForOpenInNewYorkTimeZone ,
				barIdentifier.DateTimeForOpenInNewYorkTimeZone ,
				barIdentifier.BarIntervalInSeconds );
			return ohlcRequest;
		}
	}
}
