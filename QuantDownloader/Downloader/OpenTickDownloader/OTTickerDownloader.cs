/*
QuantProject - Quantitative Finance Library

OTTickerDownloader.cs
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
using System.Data;
using System.Threading;
using System.Windows.Forms;

using QuantProject.ADT;
using QuantProject.ADT.Timing;
using QuantProject.Presentation;

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	public delegate void DownloadingStartedEventHandler(
		object sender , DownloadingStartedEventArgs e );
	
	public delegate void DownloadingCompletedEventHandler(
		object sender , DownloadingCompletedEventArgs e );
	
	/// <summary>
	/// Summary description for OTTickerDownloader.
	/// </summary>
	public class OTTickerDownloader
	{
		private string[] tickersToDownload;
		
		private List<Time> dailyTimes;
		private DateTime firstDate;
		private DateTime lastDate;
		
		private DateTime dateTimeForFirstBarOpenInNewYorkTimeZone;
		private DateTime dateTimeForLastBarOpenInNewYorkTimeZone;
		
		/// <summary>
		/// number of seconds in each bar
		/// </summary>
		private int barIntervalInSeconds;
		private DateTime dateTimeForOverWritingQuotes;//before this
		//date quotes should be overwritten automatically
		private bool checkForMissingQuotes;//it downloads and writes
		//to db all the missing quotes
		private bool overwriteAllQuotesInDatabase;//for overWriting quotes
		//also after dateTimeForOverWritingQuotes
		private bool downloadOnlySuccessiveQuotesToTheLastQuoteInDatabase;
		//the starting date for download is just the next day to the
		//last one stored in database. If no quote is stored,
		//the starting date for download is startingNewYorkDateTime
		private string openTickUser;
		private string openTickPassword;
		
//		private OTManager oTManager;
		private IExchangeSelector exchangeSelector;
		private IOHLCRequester oHLCRequester;
		private BarsDownloader barsDownloader;
		private MessageManager messageManager;
		
		private Thread downloadBarsThread;
		
		private void otTickerDownloader_checkParameters( string[] tickersToDownload,
		                                                DateTime startingNewYorkDateTime,
		                                                DateTime dateTimeForOverWritingQuotes,
		                                                bool checkForMissingQuotes,
		                                                bool overwriteAllQuotesInDatabase,
		                                                bool downloadOnlySuccessiveQuotesToTheLastQuoteInDatabase,
		                                                string openTickUser,
		                                                string openTickPassword )
		{
			if(tickersToDownload == null)
				throw new Exception("No ticker has been indicated for download!");
			if( !downloadOnlySuccessiveQuotesToTheLastQuoteInDatabase &&
			   dateTimeForOverWritingQuotes.CompareTo(startingNewYorkDateTime) < 0 )
				//the user has not requested only quotes successive to the last
				//stored in the DB and date time for overwriting quotes precedes
				//the date time for the first quote to download
				throw new Exception("Date Time for OverWriting Quotes can't precede " +
				                    "starting Date for download!");
			if( checkForMissingQuotes && downloadOnlySuccessiveQuotesToTheLastQuoteInDatabase  )
				//the two options have to be different in value or both false. They can't be both true
				throw new Exception("Downloading only quotes successive to the last quote in the DB " +
				                    "implies that missing quotes will not be checked" );
			if( ( openTickUser == null || openTickUser == "" ) ||
			   ( openTickPassword == null || openTickPassword == "" ) )
				throw new Exception("Type in user and password for logging to OpenTick" );
		}
		
		public OTTickerDownloader(
			string[] tickersToDownload,
			List<Time> dailyTimes ,
			DateTime firstDate ,
			DateTime lastDate ,
			int barIntervalInSeconds ,
			DateTime dateTimeForOverWritingQuotes,
			bool checkForMissingQuotes,
			bool overwriteAllQuotesInDatabase,
			bool downloadOnlySuccessiveQuotesToTheLastQuoteInDatabase,
			string openTickUser,
			string openTickPassword)
		{
			this.otTickerDownloader_checkParameters(tickersToDownload,
			                                        firstDate,
			                                        dateTimeForOverWritingQuotes,
			                                        checkForMissingQuotes,
			                                        overwriteAllQuotesInDatabase,
			                                        downloadOnlySuccessiveQuotesToTheLastQuoteInDatabase,
			                                        openTickUser,
			                                        openTickPassword);
			this.tickersToDownload = tickersToDownload;
			this.dailyTimes = dailyTimes;
			this.firstDate = firstDate;
			this.lastDate = lastDate;
			this.barIntervalInSeconds = barIntervalInSeconds;
			this.dateTimeForOverWritingQuotes = dateTimeForOverWritingQuotes;
			this.checkForMissingQuotes = checkForMissingQuotes;
			this.overwriteAllQuotesInDatabase = overwriteAllQuotesInDatabase;
			this.downloadOnlySuccessiveQuotesToTheLastQuoteInDatabase =
				downloadOnlySuccessiveQuotesToTheLastQuoteInDatabase;
			this.openTickUser = openTickUser;
			this.openTickPassword = openTickPassword;
		}
		
		public OTTickerDownloader(
			string[] tickersToDownload,
			DateTime dateTimeForFirstBarOpenInNewYorkTimeZone,
			DateTime dateTimeForLastBarOpenInNewYorkTimeZone,
			int barInterval ,
			string openTickUser,
			string openTickPassword)
		{
			this.otTickerDownloader_checkParameters(tickersToDownload,
			                                        firstDate,
			                                        dateTimeForFirstBarOpenInNewYorkTimeZone,
			                                        false,
			                                        false,
			                                        false,
			                                        openTickUser,
			                                        openTickPassword);
			this.tickersToDownload = tickersToDownload;
			this.dailyTimes = null;
			this.dateTimeForFirstBarOpenInNewYorkTimeZone = dateTimeForFirstBarOpenInNewYorkTimeZone;
			this.dateTimeForLastBarOpenInNewYorkTimeZone = dateTimeForLastBarOpenInNewYorkTimeZone;
			this.barIntervalInSeconds = barInterval;
			this.dateTimeForOverWritingQuotes = DateTime.MaxValue;
			this.checkForMissingQuotes = false;
			this.overwriteAllQuotesInDatabase = false;
			this.downloadOnlySuccessiveQuotesToTheLastQuoteInDatabase =	false;
			this.openTickUser = openTickUser;
			this.openTickPassword = openTickPassword;
		}
		
		public event DownloadingStartedEventHandler DownloadingStarted;
		public event DatabaseUpdatedEventHandler DatabaseUpdated;
		public event DownloadingCompletedEventHandler DownloadingCompleted;
		
		#region DownloadTickers
		private void downloadTickers_dummyDatabaseUpdatedEventRiser()
		{
			DateTime dateTimeOfLastBarUpdated = DateTime.Now;
			foreach(string ticker in this.tickersToDownload)
			{
				Thread.Sleep(500);
				DatabaseUpdatedEventArgs databaseUpdatedEventArgs =
					new DatabaseUpdatedEventArgs(ticker, dateTimeOfLastBarUpdated);
				if(this.DatabaseUpdated != null)
					this.DatabaseUpdated(this, databaseUpdatedEventArgs);
			}
		}
		
		private void downloadTickers_dummyDownloadingStartedEventRiser()
		{
			DownloadingStartedEventArgs eventArgs =
				new DownloadingStartedEventArgs(DateTime.Now);
			if(this.DownloadingStarted != null)
				this.DownloadingStarted( this, eventArgs );
		}
		
		private void downloadTickers_dummyDownloadingCompletedEventRiser()
		{
			DownloadingCompletedEventArgs eventArgs =
				new DownloadingCompletedEventArgs(DateTime.Now);
			if(this.DownloadingCompleted != null)
				this.DownloadingCompleted( this, eventArgs );
		}

		#region downloadTickersActually
		private DateTime setBarsSelector_getDate( DateTime dateTime )
		{
			DateTime date = new DateTime(
				dateTime.Year , dateTime.Month , dateTime.Day );
			return date;
		}
		#region oHLCRequester
		private void setOHLCRequester_forSpecificDailyTimes()
		{
			DateTime firstDateForBarsSelector =
				this.setBarsSelector_getDate( this.firstDate );
			DateTime lastDateForBarsSelector =
				this.setBarsSelector_getDate( DateTime.Now );
			if ( this.checkForMissingQuotes )
				// only missing quotes are to be downloaded
				this.oHLCRequester =
					new BarsSelectorBasedOHLCRequester(
						new MissingDailyBarsSelector(
							this.tickersToDownload ,
							this.setBarsSelector_getDate( this.firstDate ) ,
							this.setBarsSelector_getDate( this.lastDate ) ,
							this.barIntervalInSeconds ,
							this.dailyTimes ) );
			else
				// all quotes are to be downloaded, even if they
				// are in the database already
				this.oHLCRequester =
					new BarsSelectorBasedOHLCRequester(
						new DailyBarsSelector(
							this.tickersToDownload ,
							this.setBarsSelector_getDate( this.firstDate ) ,
							this.setBarsSelector_getDate( DateTime.Now ) ,
							this.barIntervalInSeconds ,
							this.dailyTimes ) );
		}
		private void setOHLCRequester_forAllDailyBars()
		{
			this.oHLCRequester =
				new OHLCRequesterForConsecutiveBars(
							this.tickersToDownload ,
							this.dateTimeForFirstBarOpenInNewYorkTimeZone ,
							this.dateTimeForLastBarOpenInNewYorkTimeZone ,
							this.barIntervalInSeconds );
		}
		private void setOHLCRequester()
		{
			if ( this.dailyTimes != null )
				// only some specific daily times have been requested
				this.setOHLCRequester_forSpecificDailyTimes();
			else
				// all daily bars have been requested
				this.setOHLCRequester_forAllDailyBars();
		}
		#endregion oHLCRequester
		private void setExchangeSelector()
		{
			this.exchangeSelector =
				new MostLiquidExchangeSelector();
		}
		#region setBarsDownloaderAndRunIt
		
		#region setBarsDownloader
		
		#region setBarsDownloader_setEventHandlers
		private void databaseUpdatedEventHandler(
			object sender , DatabaseUpdatedEventArgs eventArgs )
		{
			if ( this.DatabaseUpdated != null )
				this.DatabaseUpdated( this , eventArgs );
		}
		private void setBarsDownloader_setEventHandlers()
		{
			this.barsDownloader.DatabaseUpdated +=
				new DatabaseUpdatedEventHandler(
					this.databaseUpdatedEventHandler );
		}
		#endregion setBarsDownloader_setEventHandlers

		private void setBarsDownloader()
		{
			this.barsDownloader =
				new BarsDownloader(
					this.oHLCRequester ,
					this.exchangeSelector ,
					this.openTickUser ,
					this.openTickPassword );
			this.setBarsDownloader_setEventHandlers();
		}
		#endregion setBarsDownloader
		
		#region setMessageManager
		private string getLogFileName()
		{
//			string logFileName =
//				@"C:\Quant\OpenTickDownloader\textFilesForLoggingNotification\textFileForLoggingNotification";
			string logFileName =
				Application.StartupPath + "\\textFileForLoggingNotification";
			logFileName = logFileName + "_" +
				ExtendedDateTime.GetCompleteShortDescriptionForFileName(
					DateTime.Now ) + ".txt";
			return logFileName;
		}
		private void setMessageManager()
		{
			this.messageManager =
				new MessageManager( this.getLogFileName() );
			// uncomment the following line if you want to see the log file
//			this.messageManager.Monitor( this.barsDownloader );
		}
		#endregion setMessageManager
		
		private void downloadBarsInANewThread()
		{
			this.downloadBarsThread = new Thread(
				new ThreadStart( this.barsDownloader.DownloadBars ) );
			this.downloadBarsThread.Start();
		}
		private void setBarsDownloaderAndRunIt()
		{
			this.setBarsDownloader();
			this.setMessageManager();
			this.downloadBarsInANewThread();
		}
		#endregion setBarsDownloaderAndRunIt
		private void downloadTickersActually()
		{
			this.setOHLCRequester();
			this.setExchangeSelector();
			this.setBarsDownloaderAndRunIt();
		}
		#endregion downloadTickersActually
		
		public void DownloadTickers()
		{
			this.downloadTickersActually();
//			this.downloadTickers_dummyDownloadingStartedEventRiser();
//			this.downloadTickers_dummyDatabaseUpdatedEventRiser();
//			this.downloadTickers_dummyDownloadingCompletedEventRiser();
		}
		#endregion DownloadTickers
	}
}
