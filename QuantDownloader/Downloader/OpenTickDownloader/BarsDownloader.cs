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

		private IOHLCRequester oHLCRequester;
		private IExchangeSelector exchangeSelector;
		private string openTickUser;
		private string openTickPassword;
		
		private OTManager oTManager;
		
		private BarQueue barQueue;
		private BarQueueFiller barQueueFiller;
		private DataBaseWriter dataBaseWriter;
		
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
			IOHLCRequester oHLCRequester ,
			IExchangeSelector exchangeSelector ,
			string openTickUser ,
			string openTickPassword
		)
		{
			this.oHLCRequester = oHLCRequester;
			this.exchangeSelector = exchangeSelector;
			this.openTickUser = openTickUser;
			this.openTickPassword = openTickPassword;
			
			this.oTManager = new OTManager();
		}
		
		#region DownloadBars
		private void initializeBarQueue()
		{
			this.barQueue = new BarQueue(
				DownloaderConstants.MAX_NUMBER_OF_BARS_TO_BE_WRITTEN_WITH_A_SINGLE_SQL_COMMAND );
		}
		
		#region initializeBarQueueFiller
		private void newOHLCRequestEventHandler(
			int requestID , DateTime dateTimeForFirstBarOpenInUTC ,
			DateTime dateTimeForLastBarOpenInUTC , long barInterval )
		{
			if ( this.NewOHLCRequest != null )
				this.NewOHLCRequest(
					requestID , dateTimeForFirstBarOpenInUTC ,
					dateTimeForLastBarOpenInUTC , barInterval );
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
					this.oHLCRequester ,
					this.exchangeSelector ,
					this.oTManager ,
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
			this.initializeDownloadingObjects();
			this.startThreadToFillBarQueue();
			this.startThreadToWriteBarsFromBuffersToTheDatabase();
		}
		
		public void DownloadBars()
		{
			this.oTManager.OnLogin += new OnLoginEventHandler(
				this.onLoginEventHandler );

			this.oTManager.SubmitLogin(
				this.openTickUser , this.openTickPassword );
		}
		#endregion DownloadBars
	}
}
