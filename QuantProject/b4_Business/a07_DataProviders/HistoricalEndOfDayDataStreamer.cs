/*
QuantProject - Quantitative Finance Library

HistoricalEndOfDayDataStreamer.cs
Copyright (C) 2003 
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

using QuantProject.ADT;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;
using QuantProject.Data.DataProviders.Quotes;

namespace QuantProject.Business.DataProviders
{
	/// <summary>
	/// IDataStreamer implementation using end of day historical data
	/// </summary>
	[Serializable]
	public class HistoricalEndOfDayDataStreamer : IDataStreamer
	{
		private Hashtable tickers;

		private Timer timer;
		private HistoricalMarketValueProvider historicalMarketValueProvider;

		private DateTime startDateTime;
		private DateTime endDateTime;

		public DateTime StartDateTime
		{
			get	{	return this.startDateTime;	}
			set	{	this.startDateTime = value;	}
		}

		public DateTime EndDateTime
		{
			get	{	return this.endDateTime;	}
			set	{	this.endDateTime = value;	}
		}

		public HistoricalEndOfDayDataStreamer(
			Timer timer ,
			HistoricalMarketValueProvider historicalMarketValueProvider )
		{
			this.timer = timer;
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.timer.NewDateTime +=
				new NewDateTimeEventHandler(
				this.newTimeEventHandler );
			this.tickers = new Hashtable();
		}

		#region newTimeEventHandler
		private void riseNewQuotesIfTheCase(
			DateTime dateTime , MarketStatusSwitch marketStatusSwitch )
		{
			if ( this.tickers.Count > 0 )
			{
				// the data streamer is monitoring some ticker
				Hashtable quotes =
					HistoricalQuotesProvider.GetAdjustedQuotes(
						this.tickers , dateTime , marketStatusSwitch );
				foreach ( Quote quote in quotes )
					this.NewQuote( this , new NewQuoteEventArgs( quote ) );
			}
		}
		private void newTimeEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( HistoricalEndOfDayTimer.IsMarketOpen( dateTime ) )
				this.riseNewQuotesIfTheCase( dateTime , MarketStatusSwitch.Open );
			if ( HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
				this.riseNewQuotesIfTheCase( dateTime , MarketStatusSwitch.Close );			
		}
		#endregion newTimeEventHandler

		#region GetCurrentBid
		private void getCurrentBid_checkValidTime()
		{
			DateTime currentDateTime = this.timer.GetCurrentDateTime();
			if ( !HistoricalEndOfDayTimer.IsMarketStatusSwitch( currentDateTime ) )
				throw new Exception(
					"With this data streamer, GetCurrentBid can be invoked only " +
					"if the current time is either on market open or on market " +
					"close." );
		}
		
		#region getCurrentBid_actually
		private double getCurrentBid_actually( string ticker )
		{
			double currentBid =
				this.historicalMarketValueProvider.GetMarketValue(
					ticker ,
					this.timer.GetCurrentDateTime() );
			return currentBid;
		}
		#endregion getCurrentBid_actually
		
		/// <summary>
		/// Returns the current bid for the given ticker
		/// </summary>
		/// <param name="ticker"></param>
		/// <returns></returns>
		public double GetCurrentBid( string ticker )
		{
			this.getCurrentBid_checkValidTime();
			double currentBid =
				this.getCurrentBid_actually( ticker );
			return currentBid;
		}
		#endregion GetCurrentBid

		/// <summary>
		/// Returns the current ask for the given ticker
		/// </summary>
		/// <param name="ticker"></param>
		/// <returns></returns>
		public double GetCurrentAsk( string ticker )
		{
			return this.GetCurrentBid( ticker );
		}

		/// <summary>
		/// true iif the ticker was exchanged at the given date time
		/// </summary>
		/// <param name="ticker"></param>
		/// <returns></returns>
		public bool IsExchanged( string ticker )
		{
			return HistoricalQuotesProvider.WasExchanged(
				ticker , this.timer.GetCurrentDateTime() );
		}
		/// <summary>
		/// Add a ticker whose quotes are to be monitored
		/// </summary>
		/// <param name="ticker"></param>
		public void Add( string ticker )
		{
			if ( !this.tickers.Contains( ticker ) )
				this.tickers.Add( ticker , 1 );
		}
    [field:NonSerialized]
		public event NewQuoteEventHandler NewQuote;

//		/// <summary>
//		/// Starts the time walking simulation
//		/// </summary>
//		public void GoSimulate()
//		{
//			this.timer = new Timer( this.startDateTime , this.endDateTime );
//			timer.NewExtendedDateTime += new NewExtendedDateTimeHandler(
//				this.newExtendedDateTimeHandler );
//			timer.Start();
//		}
	}
}
