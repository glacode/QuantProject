/*
QuantProject - Quantitative Finance Library

BarsDownloader.cs
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
using System.Threading;

using OTFeed_NET;

using QuantProject.ADT.Messaging;

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	/// <summary>
	/// Downloads bars and stores them in the
	/// database (in the bars table)
	/// </summary>
	public class BarsDownloader : IMessageSender
	{
		public event NewOHLCRequestEventHandler NewOHLCRequest;
		public event NewMessageEventHandler NewMessage;
		public event DatabaseUpdatedEventHandler DatabaseUpdated;

		private IBarsSelector barsSelector;
		private IExchangeSelector exchangeSelector;
		private string openTickUser;
		private string openTickPassword;
		
		private OTManager oTManager;
		
//		private string ticker;
//		private DateTime firstDate;
//		private DateTime lastDate;
//		private int barInterval;
//		private DateTime firstBarOpenTime;
//		private int numberOfBarsToBeDownloadedForEachDay;
		
		private BarQueue barQueue;
		private BarQueueFiller barQueueFiller;
		private DataBaseWriter dataBaseWriter;
		
//		private MainExchangeFinder mainExchangeFinder;
		
//		public event ExchangeNotFoundEventHandler ExchangeNotFound;

		/// <summary>
		/// Downloads and writes to database the requested bars
		/// </summary>
		/// <param name="oTClient">OTClient to be used for downloading</param>
		/// <param name="ticker">the ticker whose bars have to be downloaded</param>
		/// <param name="firstDate">first date for the days to be considered</param>
		/// <param name="lastDate">last date for the days to be considered</param>
		/// <param name="barInterval">lenght, in seconds, for a bar (60 for
		/// a one minute bar)</param>
		/// <param name="firstBarOpenTime">time for the open of the first bar
		/// that has to be downloaded, for every day; use New York time zone
		/// for this parameter</param>
		/// <param name="numberOfBarsToBeDownloadedForEachDay">number of bars
		/// to be downloaded every day</param>
		public BarsDownloader(
			IBarsSelector barsSelector ,
			IExchangeSelector exchangeSelector ,
			string openTickUser ,
			string openTickPassword
//			,
//			string ticker ,
//			DateTime firstDate ,
//			DateTime lastDate ,
//			int barInterval ,
//			DateTime firstBarOpenTime ,
//			int numberOfBarsToBeDownloadedForEachDay
		)
		{
			this.barsSelector = barsSelector;
			this.exchangeSelector = exchangeSelector;
			this.openTickUser = openTickUser;
			this.openTickPassword = openTickPassword;
			this.oTManager = new OTManager();
//			this.ticker = ticker;
//			this.firstDate = firstDate;
//			this.lastDate = lastDate;
//			this.barInterval = barInterval;
//			this.firstBarOpenTime = firstBarOpenTime;
//			this.numberOfBarsToBeDownloadedForEachDay =
//				numberOfBarsToBeDownloadedForEachDay;
		}
		
		#region DownloadBars
		
//		private void runMainExchangeFinder()
//		{
//			// consider using this.oTClient.requestListSymbols( string exchange );
////			this.mainExchangeFinder =
////				new MainExchangeFinder( this.oTClient , this.ticker );
//			this.mainExchangeFinder.NewOHLCRequest +=
//				new NewOHLCRequestEventHandler( this.newOHLCRequestEventHandler );
//			mainExchangeFinder.FindMainExchange();
//			while ( !mainExchangeFinder.IsSearchComplete )
//				// the main exchange has not been found, yet
//				Thread.Sleep( 200 );
//		}

		private void initializeBarQueue()
		{
			this.barQueue = new BarQueue(
				DownloaderConstants.MAX_NUMBER_OF_BARS_TO_BE_WRITTEN_WITH_A_SINGLE_SQL_COMMAND );
		}
		
		#region initializeBarQueueFiller
		private void newOHLCRequestEventHandler(
			int requestID , DateTime dateTimeForRequestInUTC , long barInterval )
		{
			if ( this.NewOHLCRequest != null )
				this.NewOHLCRequest(
					requestID , dateTimeForRequestInUTC , barInterval );
		}
		private void newMessageEventHandler(
			object sender , NewMessageEventArgs eventArgs )
		{
			if ( this.NewMessage != null )
				this.NewMessage( this , eventArgs );
		}
		private void initializeBarQueueFiller()
		{
			this.barQueueFiller =
				new BarQueueFiller(
					this.barsSelector ,
					this.exchangeSelector ,
					this.oTManager ,
//					this.ticker ,
//					this.mainExchangeFinder.MainExchange ,
//					this.firstDate ,
//					this.lastDate ,
//					this.barInterval ,
//					this.firstBarOpenTime ,
//					this.numberOfBarsToBeDownloadedForEachDay ,
					this.barQueue 
				);
			this.barQueueFiller.NewOHLCRequest +=
				new NewOHLCRequestEventHandler( this.newOHLCRequestEventHandler );
			this.barQueueFiller.NewMessage +=
				new NewMessageEventHandler( this.newMessageEventHandler );
		}
		#endregion initializeBarQueueFiller
		
		#region initializeDatabaseWriter
		private void databaseUpdatedEventHandler(
			object sender , DatabaseUpdatedEventArgs eventArgs )
		{
			if ( this.DatabaseUpdated != null )
				this.DatabaseUpdated( this , eventArgs );
		}
		private void initializeDatabaseWriter()
		{
			this.dataBaseWriter = new DataBaseWriter(
				this.barQueue ,
				DownloaderConstants.MAX_NUMBER_OF_BARS_TO_BE_WRITTEN_WITH_A_SINGLE_SQL_COMMAND );
			this.dataBaseWriter.DatabaseUpdated +=
				new DatabaseUpdatedEventHandler(
					this.databaseUpdatedEventHandler );
		}
		#endregion initializeDatabaseWriter
		
		private void initializeDownloadingObjects()
		{
			this.initializeBarQueue();
			this.initializeBarQueueFiller();
			this.initializeDatabaseWriter();
		}
		private void startThreadToFillBarQueue()
		{
			this.barQueueFiller.StartFillingQueue();
		}
		private void startThreadToWriteBarsFromBuffersToTheDatabase()
		{
			this.dataBaseWriter.StartWritingBuffersToDatabase();
		}
		
		private void onLoginEventHandler()
		{
//			this.runMainExchangeFinder();
//			if ( this.mainExchangeFinder.IsMainExchangeFound )
//				// the main exchange has been found and
//			{
				this.initializeDownloadingObjects();
				this.startThreadToFillBarQueue();
				this.startThreadToWriteBarsFromBuffersToTheDatabase();
//			}
//			else
//				this.ExchangeNotFound( this , this.ticker );

//			this.downloadBarsThread = new Thread(
//				new ThreadStart( this.downloadBars ) );
//			this.downloadBarsThread.Start();
			
//			this.testForDaylightSavingTime();
		}
		
		public void DownloadBars()
		{
			OTManager.OnLogin += new OnLoginEventHandler(
				this.onLoginEventHandler );

			OTManager.SubmitLogin(
				this.openTickUser , this.openTickPassword );
		}
		#endregion DownloadBars
	}
}
