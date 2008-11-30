/*
QuantProject - Quantitative Finance Library

OTManager.cs
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
using System.Threading;

using OTFeed_NET;

using QuantProject.ADT.Messaging;
//using QuantProject.Presentation;

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	public delegate void OnErrorEventHandler(
		OTError otError );
	public delegate void OnLoginEventHandler();
	public delegate void OnNoDataEventHandler(
		int requestId , BarRequest barRequest );
	public delegate void OnEndOfDataEventHandler(
		int requestId , BarRequest barRequest );
	public delegate void OnHistoricalOHLCEventHandler(
		OTOHLC ohlc , BarRequest barRequest );
	
	/// <summary>
	/// Encapsulates the OTClient and extends its features
	/// </summary>
	public class OTManager : IMessageSender
	{
		public event OnErrorEventHandler OnError;
		public static OnLoginEventHandler OnLogin;
		public event OnNoDataEventHandler OnNoData;
		public event OnEndOfDataEventHandler OnEndOfData;
		public event OnHistoricalOHLCEventHandler OnHistoricalOHLC;
		
		public event NewMessageEventHandler NewMessage;
		
		/// <summary>
		/// requests for wich an answering event has not been received yet;
		/// when such an event returns, it is forwarded and then the request
		/// is removed
		/// </summary>
		private Hashtable pendingBarRequests;
		/// <summary>
		/// full path for the file where messages will be logged; if
		/// this member is "" then no logging is required
		/// </summary>
//		private string logFileName;
		
		private static OTClient oTClient;
		/// <summary>
		/// used to lock OTManager.oTClient: we use a dummy BarRequest
		/// but any reference type would have done the job
		/// </summary>
//		private static BarRequest oTClientCreatorLocker =
//			new BarRequest( "A" , "A" , new DateTime( 1 , 1 , 2008 ) );
		
		static OTManager()
		{
			OTManager.oTClient = new OTClient();
			OTManager.oTClient.onLogin += new OTLoginEvent(
				OTManager.onLoginEventHandler );
			OTManager.oTClient.onStatusChanged += new OTStatusChangedEvent(
				OTManager.onStatusChangedEventHandler );
		}
		private static void onLoginEventHandler()
		{
			if ( OTManager.OnLogin != null )
				OTManager.OnLogin();
		}
		private static void onStatusChangedEventHandler( int status )
		{
			string currentStatusHasChangedTo =
				OTStatus.GetName(
					typeof( OTStatus ) , status );
			string message =
				"Current status has changed to: " +	currentStatusHasChangedTo;
//			this.riseNewMessageEvent( message );
		}

		
		/// <summary>
		/// Encapsulates the OTClient and extends its features. Using
		/// this constructor, no log is done
		/// </summary>
		public OTManager()
		{
//			this.logFileName = "";
			this.commonInitialization();
		}
		
		#region commonInitialization
		#region setOTCLientEventHandlers
		private bool wasThisRequestSubmittedByThisOTManagerInstance( int requestId )
		{
			bool wasSubmittedByThisOTManagerInstance;
			lock( this.pendingBarRequests )
			{
				wasSubmittedByThisOTManagerInstance =
					this.pendingBarRequests.ContainsKey( requestId );
			}
			return wasSubmittedByThisOTManagerInstance;
		}
		
		#region onOTClientError
		
		#region onOTClientError_actually
		private void onOTClientError_riseOnNoDataIfTheCase( OTError error )
		{
			if ( error.Code == Convert.ToInt32( OTErrorCodes.NoData ) )
				// current error event signals no data available for
				// the given ticker, for the given exchange
			{
				BarRequest barRequest = this.RemoveBarRequest(
					error.RequestId );
				if ( this.OnNoData != null )
					this.OnNoData( error.RequestId , barRequest );
			}
		}
		private void onOTClientError_logMessage( OTError error )
		{
			string message =
				"onOTClientError " + "---" +
				"Code: " + error.Code + "---" +
				"Description: " + error.Description + "---" +
				"RequestId: " + error.RequestId + "---" +
				"Type: " + error.Type;
			OTDataEntity oTDataEntity =
				OTManager.oTClient.getEntityById( error.RequestId );
			if ( oTDataEntity != null )
				// RequestId was refered to a OTDataEntity
				message = message + "---" +
					"Ticker: " + oTDataEntity.Symbol + "---" +
					"Exchange: " + oTDataEntity.Exchange;
			this.riseNewMessageEvent( message );
		}
		private void onOTClientError_actually( OTError error )
		{
			this.onOTClientError_riseOnNoDataIfTheCase( error );
			this.onOTClientError_logMessage( error );
			if ( this.OnError != null )
				this.OnError( error );			
		}
		#endregion onOTClientError_actually
		
		private void onOTClientError( OTError error )
		{
			if ( this.wasThisRequestSubmittedByThisOTManagerInstance(
				error.RequestId ) )
				// the error refers to a request submitted by this OTManager instance
				this.onOTClientError_actually( error );
		}
		#endregion onOTClientError
		
		#region onMessageEventHandler
		
		#region onMessageEventHandler_actually
		
		#region onMessageEventHandler_logMessage
		private string getCompleteMessage(
			string message , OTDataEntity oTDataEntity , int requestId )
		{
			BarRequest barRequest =	this.GetBarRequest( requestId );
//			DateTime dateTimeForRequest = (DateTime)
//				this.requestsThatAreNotCompletedYet[ requestId ];
//			this.requestsThatAreNotCompletedYet.Remove( requestId );
			string completeMessage = message + "---" +
				"Ticker: " + oTDataEntity.Symbol + "---" +
				"Exchange: " + oTDataEntity.Exchange + "---" +
				"DateTime: " + barRequest.DateTimeForOpenInUTC;
			return completeMessage;
		}
		private void onMessageEventHandler_logMessage( OTMessage oTMessage )
		{
			string message =
				"onMessageEventHandler " + "---" +
				"Code: " + oTMessage.Code + "---" +
				"Description: " + oTMessage.Description + "---" +
				"RequestId: " + oTMessage.RequestId;
			OTDataEntity oTDataEntity =
				OTManager.oTClient.getEntityById( oTMessage.RequestId );
			if ( oTDataEntity != null )
				// RequestId was refered to a OTDataEntity
				message = this.getCompleteMessage(
					message , oTDataEntity , oTMessage.RequestId );
			this.riseNewMessageEvent( message );
//			System.Windows.Forms.MessageBox.Show( message );
		}
		#endregion onMessageEventHandler_logMessage
		
		private void onMessageEventHandler_riseOnEndOfDataIfTheCase(
			OTMessage oTMessage )
		{
			if ( oTMessage.Code ==
			    Convert.ToInt32( OTMessageCodes.EndOfData ) )
				// a request has all been satisfied
			{
				BarRequest barRequest = this.RemoveBarRequest(
					oTMessage.RequestId );
				if ( this.OnEndOfData != null )
					this.OnEndOfData( oTMessage.RequestId , barRequest );
			}
		}
		private void onMessageEventHandler_actually( OTMessage oTMessage )
		{
			this.onMessageEventHandler_logMessage( oTMessage );
			this.onMessageEventHandler_riseOnEndOfDataIfTheCase( oTMessage );			
		}
		#endregion onMessageEventHandler_actually
		
		private void onMessageEventHandler( OTMessage oTMessage )
		{
			if ( this.wasThisRequestSubmittedByThisOTManagerInstance(
				oTMessage.RequestId ) )
				this.onMessageEventHandler_actually( oTMessage );
		}
		#endregion onMessageEventHandler
		
		#region onHistoricalOHLC
		#region onHistoricalOHLC_actually
		private void onHistoricalOHLC_logMessage( OTOHLC ohlc )
		{
			OTDataEntity oTDataEntity =
				OTManager.oTClient.getEntityById( ohlc.RequestId );
			string message = String.Format(
				"OHLC({7}):time={0} o={1,-6} h={2,-6} " +
				"l={3,-6} c={4,-6} v={5,-8} now is {6} " +
				"- ticker:{8} - exchange:{9}" ,
				ohlc.Timestamp,
				ohlc.OpenPrice, ohlc.HighPrice,
				ohlc.LowPrice, ohlc.ClosePrice,
				ohlc.Volume,
				DateTime.Now,
				ohlc.RequestId ,
				oTDataEntity.Symbol ,
				oTDataEntity.Exchange
			);
			this.riseNewMessageEvent( message );
		}
		private void onHistoricalOHLC_actually( OTOHLC ohlc )
		{
			this.onHistoricalOHLC_logMessage( ohlc );
			if ( this.OnHistoricalOHLC != null )
			{
				BarRequest barRequest =
					this.GetBarRequest( ohlc.RequestId );
				this.OnHistoricalOHLC( ohlc , barRequest );
			}
		}
		#endregion onHistoricalOHLC_actually
		private void onHistoricalOHLC( OTOHLC ohlc )
		{
			if ( this.wasThisRequestSubmittedByThisOTManagerInstance(
				ohlc.RequestId ) )
				this.onHistoricalOHLC_actually( ohlc );
		}
		#endregion onHistoricalOHLC
		
		private void setOTClientEventHandlers()
		{
			OTManager.oTClient.onError += new OTErrorEvent( this.onOTClientError );
			OTManager.oTClient.onMessage += new OTMessageEvent(
				this.onMessageEventHandler );
//			if ( oTClientWasJustCreated )
//				// oTClient was null. We want only one OTManager instance
//				// to monitor (and possibly log) oTClient status changes
//			{
//				OTManager.oTClient.onLogin += new OTLoginEvent(
//					OTManager.onLoginEventHandler );
//				OTManager.oTClient.onStatusChanged += new OTStatusChangedEvent(
//					OTManager.onStatusChangedEventHandler );
//			}
			OTManager.oTClient.onHistoricalOHLC +=
				new OTOHLCEvent( this.onHistoricalOHLC );
		}
		#endregion setOTCLientEventHandlers
		
		private void commonInitialization()
		{
			this.pendingBarRequests = new Hashtable();
			this.setOTClientEventHandlers();
		}
		#endregion commonInitialization


		

		public static void SubmitLogin(
			string openTickUser , string openTickPassword )
		{
			OTManager.oTClient.addHost( "feed1.opentick.com" , 10010 );
			//OTManager.oTClient.addHost( "delayed1.opentick.com" , 10010 );
//			string username =
//				System.Configuration.ConfigurationManager.AppSettings[
//					"usrnm" ];
//			string password =
//				System.Configuration.ConfigurationManager.AppSettings[
//					"pswd" ];
			OTManager.oTClient.login( openTickUser , openTickPassword );
		}
//		/// <summary>
//		/// This method can be 
//		/// </summary>
//		public void SubmitLogin()
//		{
//			bool oTCLientWasNullWhenThisMethodBegan;
//			// TO DO add a lock to the following two statements
////			lock( ??? what could I use here??? )
////			{
////				oTCLientWasNullWhenThisMethodBegan =
////					( OTManager.oTClient == null );
////				if ( oTCLientWasNullWhenThisMethodBegan )
////					OTManager.oTClient = new OTClient();
////			}
//			this.setOTClientEventHandlers( oTCLientWasNullWhenThisMethodBegan );
//			if ( oTCLientWasNullWhenThisMethodBegan )
//				this.submitLogin();
//		}



		
		public BarRequest GetBarRequest( int requestId )
		{
			BarRequest barRequest;
			lock ( this.pendingBarRequests )
			{
				barRequest = (BarRequest)this.pendingBarRequests[ requestId ];
			}
			if ( barRequest == null )
				throw new Exception(
					"There is no request for the given requestId" );
			return barRequest;
		}
		
		public BarRequest RemoveBarRequest( int requestId )
		{
			BarRequest barRequest =
				this.GetBarRequest( requestId );
			lock ( this.pendingBarRequests )
			{
				this.pendingBarRequests.Remove( requestId );
			}
			return barRequest;
		}
		
		#region RequestHistData
		private bool areThereTooManyPendentBarRequests()
		{
			bool areThereTooMany;
			lock( this.pendingBarRequests )
			{
				areThereTooMany =
					( this.pendingBarRequests.Count >=
					 DownloaderConstants.MAX_NUMBER_OF_PENDING_REQUESTS_FOR_A_SINGLE_OTMANAGER );
			}
			return areThereTooMany;				
		}
		
		#region requestHistDataWithoutTooManyPendentBarRequests
		private int requestHistDataActually(
			string exchange ,
			string symbol ,
			DateTime startingDateInUTC ,
			DateTime endingDateInUTC ,
			OTHistoricalType oTHistoricalType ,
			short intervalValue )
		{
			OTDataEntity oTDataEntity =
				new OTDataEntity( exchange , symbol );
			int requestId = OTManager.oTClient.requestHistData(
				oTDataEntity , startingDateInUTC , endingDateInUTC ,
				oTHistoricalType , intervalValue );
			return requestId;
		}
		private void update_barRequests(
			int requestId ,
			string exchange ,
			string symbol ,
			DateTime startingDateInUTC ,
			DateTime endingDateInUTC ,
			OTHistoricalType oTHistoricalType ,
			short intervalValue)
		{
			BarRequest barRequest =
				new BarRequest(
					exchange ,
					symbol ,
					startingDateInUTC );
			lock ( this.pendingBarRequests )
			{
				this.pendingBarRequests.Add( requestId , barRequest );
			}
		}
		public int requestHistDataWithoutTooManyPendentBarRequests(
			string exchange ,
			string symbol ,
			DateTime startingDateInUTC ,
			DateTime endingDateInUTC ,
			OTHistoricalType oTHistoricalType ,
			short intervalValue )
		{
			int requestId = this.requestHistDataActually(
				exchange ,
				symbol ,
				startingDateInUTC ,
				endingDateInUTC ,
				oTHistoricalType ,
				intervalValue );
			this.update_barRequests(
				requestId ,
				exchange ,
				symbol ,
				startingDateInUTC ,
				endingDateInUTC ,
				oTHistoricalType ,
				intervalValue);
			return requestId;
		}
		#endregion requestHistDataWithoutTooManyPendentBarRequests
		
		public int RequestHistData(
			string exchange ,
			string symbol ,
			DateTime startingDateInUTC ,
			DateTime endingDateInUTC ,
			OTHistoricalType oTHistoricalType ,
			short intervalValue )
		{
//			while ( this.areThereTooManyPendentBarRequests() )
//				Thread.Sleep( 10 );
			int	requestId =
					this.requestHistDataWithoutTooManyPendentBarRequests(
						exchange ,
						symbol ,
						startingDateInUTC ,
						endingDateInUTC ,
						oTHistoricalType ,
						intervalValue );
			return requestId;
		}
		#endregion RequestHistData
		
		private void riseNewMessageEvent( string message )
		{
			NewMessageEventArgs eArgs =
				new NewMessageEventArgs( message );
			if ( this.NewMessage != null )
				this.NewMessage( this , eArgs );
//			MessageManager.DisplayMessage(
//				message + "\n" , this.logFileName );
		}
	}
}
