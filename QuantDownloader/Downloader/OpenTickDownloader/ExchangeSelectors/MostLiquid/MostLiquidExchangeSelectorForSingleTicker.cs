/*
QuantProject - Quantitative Finance Library

MostLiquidExchangeSelectorForSingleTicker.cs
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
using System.Collections.Generic;

using OTFeed_NET;

using QuantProject.ADT.Messaging;

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	public delegate void MostLiquidExchangeFoundEventHandler(
		object sender , string ticker , string mostLiquidExchange );
	public delegate void MostLiquidExchangeNotFoundEventHandler(
		object sender , string ticker );
		
	/// <summary>
	/// Finds the most liquid exchange for a ticker
	/// </summary>
	public class MostLiquidExchangeSelectorForSingleTicker :
		IMessageSender
	{
		/// <summary>
		/// the main exchange for the ticker has been found
		/// </summary>
		public event MostLiquidExchangeFoundEventHandler
			MostLiquidExchangeFound;
		
		/// <summary>
		/// the ticker is not exchanged in any of the considered exchanges
		/// </summary>
		public event MostLiquidExchangeNotFoundEventHandler
			MostLiquidExchangeNotFound;
		
		public event NewOHLCRequestEventHandler NewOHLCRequest;
		
		public event NewMessageEventHandler NewMessage;
		
		private OTManager oTManager;
		private string ticker;
		
		private DateTime startingDate;
		private DateTime endingDate;
		private Dictionary<string , long> volumeForExchange;
		private List<int> requestIdsFromThisObject;
		
		private bool isSearchComplete;
		private bool isMostLiquidExchangeFound;
		private string mostLiquidExchange;

		
		public bool IsSearchComplete
		{
			get { return this.isSearchComplete; }
		}

		public bool IsMostLiquidExchangeFound
		{
			get
			{
				if ( !this.IsSearchComplete )
					throw new Exception( "The search is not complete, yet!" );
				return this.isMostLiquidExchangeFound;
			}
		}

		public string MostLiquidExchange
		{
			get
			{
				if ( !this.IsSearchComplete )
					throw new Exception( "The search is not complete, yet!" );
				return this.mostLiquidExchange;
			}
		}
		
		/// <summary>
		/// Finds the most liquid exchange for a ticker. Events are logged
		/// to the given text file
		/// </summary>
		/// <param name="ticker">ticker for whom the most liquid exchange is
		/// to be found</param>
		/// <param name="logFileName">full path for the file where events will
		/// be logged in</param>
//		public MostLiquidExchangeSelectorForSingleTicker(
//			string ticker , string logFileName )
//		{
//			this.oTManager = new OTManager();
//			this.commonInitialization( ticker );
//		}
		public MostLiquidExchangeSelectorForSingleTicker(
			string ticker )
		{
			this.oTManager = new OTManager();
			this.oTManager.NewMessage +=
				new NewMessageEventHandler(
					this.newMessageEventHandler );
			
			this.commonInitialization( ticker );
		}
		private void commonInitialization( string ticker )
		{
			this.ticker = ticker;
			this.isSearchComplete = false;
		}
		private void newMessageEventHandler(
			object sender , NewMessageEventArgs eventArgs )
		{
			if ( this.NewMessage != null )
				this.NewMessage( this , eventArgs );
		}
		
		#region SelectMostLiquidExchange
		private void setRequestParameters()
		{
			this.startingDate = new DateTime( 2007 , 1 , 1 );
			this.endingDate = new DateTime( 2007 , 1 , 10 );
		}
		
		#region setEventsHandlers
		
		#region compareVolumesSetResultsAndRiseEvents
		
		#region setMostLiquidExchange
		private void setMostLiquidExchange( out long volumeForMostLiquidExchange )
		{
			volumeForMostLiquidExchange = long.MinValue;
			this.mostLiquidExchange = "";
			foreach ( KeyValuePair< string , long > exchangeWithVolume in
			         this.volumeForExchange )
				if ( exchangeWithVolume.Value > volumeForMostLiquidExchange )
			{
				volumeForMostLiquidExchange = exchangeWithVolume.Value;
				this.mostLiquidExchange = exchangeWithVolume.Key;
			}
		}
		#endregion setMostLiquidExchange
		
		private void setResultsForExchangeNotFound()
		{
			this.isMostLiquidExchangeFound = false;
			if ( this.MostLiquidExchangeNotFound != null )
				this.MostLiquidExchangeNotFound( this , this.ticker );
		}
		private void setResultsForMainExchangeFound()
		{
			this.isMostLiquidExchangeFound = true;
//			MainExchangeFoundEventArgs eventArgs =
//				new MainExchangeFoundEventArgs(
//					this.ticker , this.mostLiquidExchange );
			if ( this.MostLiquidExchangeFound != null )
				this.MostLiquidExchangeFound(
					this , this.ticker , this.mostLiquidExchange );

		}
		private void compareVolumesSetResultsAndRiseEvents()
		{
			long volumeForMostLiquidExchange;
			this.setMostLiquidExchange( out volumeForMostLiquidExchange );
			if ( volumeForMostLiquidExchange <= 0 )
				// the ticker is not exchanged in any of the considered exchanges
				this.setResultsForExchangeNotFound();
			else
				// the main exchange for the ticker has been found
				this.setResultsForMainExchangeFound();
		}
		#endregion compareVolumesSetResultsAndRiseEvents

		private void addVolumeForExchange(
			string exchange , long volume )
		{
			this.volumeForExchange.Add( exchange , volume );
			if ( this.volumeForExchange.Count == 3 )
				// all three bars have been analyzed
			{
				this.compareVolumesSetResultsAndRiseEvents();
				this.isSearchComplete = true;
				this.oTManager.OnHistoricalOHLC -=
					this.onHistoricalOHLCeventHandler;
				this.oTManager.OnNoData -= this.onNoDataEventHandler;
			}
		}

		#region onHistoricalOHLCeventHandler_addVolumeForExchange
		private void onHistoricalOHLCeventHandler_addVolumeForExchangeActually(
			string exchange , long volume )
		{
//			OTDataEntity oTDataEntity = this.oTClient.getEntityById(
//				ohlc.RequestId );
			this.addVolumeForExchange(
				exchange , volume );
		}
		private void onHistoricalOHLCeventHandler_addVolumeForExchange(
			int requestId , string exchange , long volume )
		{
			if ( this.requestIdsFromThisObject.Contains( requestId ) )
			{
				this.onHistoricalOHLCeventHandler_addVolumeForExchangeActually(
					exchange , volume );				
			}
		}
		#endregion onHistoricalOHLCeventHandler_addVolumeForExchange
		
		private void onHistoricalOHLCeventHandler(
			OTOHLC ohlc , BarRequest barRequest )
		{
			this.onHistoricalOHLCeventHandler_addVolumeForExchange(
				ohlc.RequestId , barRequest.Exchange , ohlc.Volume );
		}
		private void onNoDataEventHandler(
			int requestId , BarRequest barRequest )
		{
			if ( this.requestIdsFromThisObject.Contains( requestId ) )
				// the request had been submitted by this object
//				OTDataEntity oTDataEntity =
//					this.oTClient.getEntityById( error.RequestId );
				this.addVolumeForExchange( barRequest.Exchange , 0 );
		}
		private void onEndOfDataEventHandler(
			int requestId , BarRequest barRequest )
		{
			if ( this.requestIdsFromThisObject.Contains( requestId ) )
			{
				// the request had been submitted by this object
//				OTDataEntity oTDataEntity =
//					this.oTClient.getEntityById( oTMessage.RequestId );
				if ( !this.volumeForExchange.ContainsKey(
					barRequest.Exchange ) )
				    // no data is available for this exchange
				    this.addVolumeForExchange(
				    	barRequest.Exchange , 0 );
			}
		}
		private void setEventsHandlers()
		{
			this.oTManager.OnHistoricalOHLC +=
				new OnHistoricalOHLCEventHandler(
					this.onHistoricalOHLCeventHandler );
			this.oTManager.OnNoData +=
				new OnNoDataEventHandler( this.onNoDataEventHandler );
			this.oTManager.OnEndOfData += new OnEndOfDataEventHandler(
				this.onEndOfDataEventHandler );
		}
		#endregion setEventsHandlers
		
		#region request
		
		#region riseNewOHLCRequestEvent
		private long getBarInterval()
		{
			TimeSpan timeSpan =
				this.endingDate.Subtract( this.startingDate );
			long barInterval = timeSpan.Seconds;
			return barInterval;
		}
		private void riseNewOHLCRequestEvent( int requestId )
		{
			long barInterval = this.getBarInterval();
			if ( this.NewOHLCRequest != null )
				this.NewOHLCRequest(
					requestId , this.startingDate , barInterval );
		}
		#endregion riseNewOHLCRequestEvent
		
		private void request( string exchangeCode )
		{
//			OTDataEntity oTDataEntity = new OTDataEntity(
//				exchangeCode , this.ticker );
			int requestId =
				this.oTManager.RequestHistData(
					exchangeCode , this.ticker
					, this.startingDate , this.endingDate ,
					OTHistoricalType.OhlcDaily , 10 );
			this.requestIdsFromThisObject.Add( requestId );
			this.riseNewOHLCRequestEvent( requestId );
		}
		#endregion request
		
		public void SelectMostLiquidExchange()
		{
			this.volumeForExchange = new Dictionary<string , long>();
			this.requestIdsFromThisObject = new List<int>();
			this.setRequestParameters();
			this.setEventsHandlers();
			this.request( "A" );
			this.request( "N" );
			this.request( "Q" );
		}
		#endregion SelectMostLiquidExchange
		
	}
}
