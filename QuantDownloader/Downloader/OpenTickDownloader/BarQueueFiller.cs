/*
QuantProject - Quantitative Finance Library

BarQueueFiller.cs
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
using System.Threading;

using OTFeed_NET;

using QuantProject.ADT.Messaging;

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	public delegate void NewOHLCRequestEventHandler(
		int requestId , DateTime dateTimeForRequest , long barInterval );
	
	/// <summary>
	/// Downloads all the bars for a given ticker and
	/// writes them into a queue
	/// </summary>
	public class BarQueueFiller : IMessageSender
	{
		public event NewOHLCRequestEventHandler NewOHLCRequest;
		
		public event NewMessageEventHandler NewMessage;
		
		private IBarsSelector barsSelector;
		
		private OTManager oTManager;
		
		private BarQueue barQueue;
		private DateTime minDateTimeForAcceptablesBars;
		
		private IExchangeSelector exchangeSelector;

		private Thread fillQueueThread;

		/// <summary>
		/// Downloads all the bars for a given ticker and
		/// writes them into a queue
		/// </summary>
		/// <param name="oTClient">OTClient to be used for downloading</param>
		/// <param name="ticker">the ticker whose bars have to be downloaded</param>
		/// <param name="exchange">exchange from which bars are to requested</param>
		/// <param name="firstDate">first date for the days to be considered</param>
		/// <param name="lastDate">last date for the days to be considered</param>
		/// <param name="barInterval">lenght, in seconds, for a bar (60 for
		/// a one minute bar)</param>
		/// <param name="firstBarOpenTime">time for the open of the first bar
		/// that has to be downloaded, for every day; use New York time zone
		/// for this parameter</param>
		/// <param name="numberOfBarsToBeDownloadedForEachDay">number of bars
		/// to be downloaded every day</param>
		/// <param name="barQueue">queue to be filled with the
		/// downloaded bars</param>
		public BarQueueFiller(
			IBarsSelector barsSelector ,
			IExchangeSelector exchangeSelector ,
			OTManager oTManager ,
			BarQueue barQueue
		)
		{
			this.barsSelector = barsSelector;
			this.exchangeSelector = exchangeSelector;
			this.exchangeSelector.NewMessage +=
				new NewMessageEventHandler(
					this.newMessageEventHandler );
			this.oTManager = oTManager;
			this.oTManager.NewMessage +=
				new NewMessageEventHandler(
					this.newMessageEventHandler );
			this.barQueue = barQueue;
			this.minDateTimeForAcceptablesBars = new DateTime( 1950 , 1 , 1 );
		}
		private void newMessageEventHandler(
			object sender , NewMessageEventArgs eventArgs )
		{
			if ( this.NewMessage != null )
				this.NewMessage( this , eventArgs );
		}
		
		#region fillQueue
		
		#region onHistoricalOHLCEventHandler
		
		#region isAcceptable
		private bool isAcceptableDateTime( OTOHLC ohlc )
		{
			bool isAcceptable =
				(
					( ohlc.Timestamp >= this.minDateTimeForAcceptablesBars ) &&
					( ohlc.Timestamp <= DateTime.Now )
				);
			return isAcceptable;
		}
		private bool areAcceptableOHLCvalues( OTOHLC ohlc )
		{
			bool areAcceptable =
				(
					( ohlc.LowPrice > 0 ) &&
					( ohlc.OpenPrice <= ohlc.HighPrice ) &&
					( ohlc.OpenPrice >= ohlc.LowPrice ) &&
					( ohlc.ClosePrice <= ohlc.HighPrice ) &&
					( ohlc.ClosePrice >= ohlc.LowPrice )
				);
			return areAcceptable;
		}
		private bool isAcceptable( OTOHLC ohlc )
		{
			bool isOk =
				( this.isAcceptableDateTime( ohlc ) ) &&
				( this.areAcceptableOHLCvalues( ohlc ) );
			return isOk;
		}
		#endregion isAcceptable
		
		#region enqueueBar
		#region getBar
		private long getBar_getInterval( OTOHLC ohlc )
		{
			int interval = 60; // TO DO use an internal list to handle this data
			return interval;
		}
		private Bar getBar( OTOHLC ohlc )
		{
			BarRequest barRequest =
				this.oTManager.GetBarRequest( ohlc.RequestId );
			string ticker = barRequest.Symbol;
			string exchange = barRequest.Exchange;
			long interval = this.getBar_getInterval( ohlc );
			Bar bar = new Bar(
				ticker ,
				exchange ,
				ohlc.Timestamp ,
				interval ,
				ohlc.OpenPrice ,
				ohlc.HighPrice ,
				ohlc.LowPrice ,
				ohlc.ClosePrice ,
				ohlc.Volume );
			return bar;
		}
		#endregion getBar
		
		private void enqueueBar( OTOHLC ohlc )
		{
				Bar bar = this.getBar( ohlc );
				this.barQueue.Enqueue( bar );
		}
		#endregion enqueueBar
		
		private void onHistoricalOHLCEventHandler(
			OTOHLC ohlc , BarRequest barRequest )
		{
			if ( this.isAcceptable( ohlc ) )
				// the bar is acceptable
				this.enqueueBar( ohlc );
		}
		
		#endregion onHistoricalOHLCEventHandler
		
		private void fillQueue_setEventHandlers()
		{
			this.oTManager.OnHistoricalOHLC +=
				new OnHistoricalOHLCEventHandler(
					this.onHistoricalOHLCEventHandler );
		}
		
		#region fillQueue_requestBarsForEachMarketDay
		
		#region fillQueue_requestBar
		private void fillQueue_requestBar_actually(
			BarIdentifier barIdentifier , string exchange )
		{
			short numberOfMinutesInEachBar =
				Convert.ToInt16( Math.Round(
					Convert.ToDouble( barIdentifier.Interval / 60 ) ) );
			DateTime dateTimeForBarOpenInUTC =
				TimeZoneManager.ConvertToUTC(
					barIdentifier.DateTimeForOpenInNewYorkTimeZone );
			int requestId = this.oTManager.RequestHistData(
				exchange , barIdentifier.Ticker ,
				dateTimeForBarOpenInUTC ,
				dateTimeForBarOpenInUTC ,
				OTHistoricalType.OhlcMinutely , numberOfMinutesInEachBar );
			if ( this.NewOHLCRequest != null )
				this.NewOHLCRequest(
					requestId , dateTimeForBarOpenInUTC ,
					barIdentifier.Interval );
		}
		private void fillQueue_requestBar(
			BarIdentifier barIdentifier )
		{
			string exchange =
				this.exchangeSelector.SelectExchange( barIdentifier.Ticker );
			if ( exchange != "" )
				// the exchange has been actually selected
				this.fillQueue_requestBar_actually( barIdentifier , exchange );
		}
		#endregion fillQueue_requestBar
		
		private void fillQueue_requestBarsForEachMarketDay()
		{
			while ( !this.barsSelector.AreAllBarsAlredyGiven )
				this.fillQueue_requestBar(
					this.barsSelector.GetNextBarIdentifier() );
		}
		#endregion fillQueue_requestBarsForEachMarketDay
		
		private void fillQueue()
		{
			this.fillQueue_setEventHandlers();
			this.fillQueue_requestBarsForEachMarketDay();
		}
		#endregion fillQueue
		
		public void StartFillingQueue()
		{
//			this.working = true;
			this.fillQueueThread = new Thread(
				new ThreadStart( this.fillQueue ) );
			this.fillQueueThread.Start();
		}
	}
}
