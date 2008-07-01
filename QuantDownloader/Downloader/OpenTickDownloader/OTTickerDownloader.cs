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
using System.Data;
using System.Threading;

using QuantProject.ADT;

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	public delegate void DownloadingStartedEventHandler(
		object sender , DownloadingStartedEventArgs e );
	
	public delegate void DatabaseUpdatedEventHandler(
		object sender , DatabaseUpdatedEventArgs e );
	
	public delegate void DownloadingCompletedEventHandler(
		object sender , DownloadingCompletedEventArgs e );
	
	/// <summary>
	/// Summary description for OTTickerDownloader.
	/// </summary>
  public class OTTickerDownloader
  {
  	private string[] tickersToDownload;
	  private DateTime startingNewYorkDateTime;
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
    	//the two options have to be different in value or both false
    		throw new Exception("Downloading only quotes successive to the last quote in the DB " + 
    		                    "implies that missing quotes will not be checked" );
    	if( ( openTickUser == null || openTickUser == "" ) || 
    	    ( openTickPassword == null || openTickPassword == "" ) )
    		throw new Exception("Type in user and password for logging to OpenTick" );
    }
    
  	public OTTickerDownloader( string[] tickersToDownload,
                               DateTime startingNewYorkDateTime,
	  													 DateTime dateTimeForOverWritingQuotes,
	  													 bool checkForMissingQuotes,
	  													 bool overwriteAllQuotesInDatabase,
	  													 bool downloadOnlySuccessiveQuotesToTheLastQuoteInDatabase,
	  													 string openTickUser,
	  													 string openTickPassword)
    {
    	this.otTickerDownloader_checkParameters(tickersToDownload,
                               startingNewYorkDateTime,
	  													 dateTimeForOverWritingQuotes,
	  													 checkForMissingQuotes,
	  													 overwriteAllQuotesInDatabase,
	  													 downloadOnlySuccessiveQuotesToTheLastQuoteInDatabase,
	  													 openTickUser,
	  													 openTickPassword);
    	this.tickersToDownload = tickersToDownload;
      this.startingNewYorkDateTime = startingNewYorkDateTime;
      this.dateTimeForOverWritingQuotes = dateTimeForOverWritingQuotes;
      this.checkForMissingQuotes = checkForMissingQuotes;
      this.overwriteAllQuotesInDatabase = overwriteAllQuotesInDatabase;
      this.downloadOnlySuccessiveQuotesToTheLastQuoteInDatabase = downloadOnlySuccessiveQuotesToTheLastQuoteInDatabase;
      this.openTickUser = openTickUser;
      this.openTickPassword = openTickPassword;
    }
		
    public event DownloadingStartedEventHandler DownloadingStarted;
  	public event DatabaseUpdatedEventHandler DatabaseUpdated;
  	public event DownloadingCompletedEventHandler DownloadingCompleted;
  	
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
  	
  	public void DownloadTickers()
  	{
  		this.downloadTickers_dummyDownloadingStartedEventRiser();
  		this.downloadTickers_dummyDatabaseUpdatedEventRiser();
  		this.downloadTickers_dummyDownloadingCompletedEventRiser();
  	}
  }  
}
