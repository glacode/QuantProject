/*
QuantProject - Quantitative Finance Library

HistoricalDataStreamer.cs
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


namespace QuantProject.Data.DataProviders
{
	/// <summary>
	/// IDataStreamer implementation using historical data
	/// </summary>
	public class HistoricalDataStreamer : IDataStreamer
	{
		private Hashtable tickers;

		private Timer timer;

		private ExtendedDateTime startDateTime;
		private ExtendedDateTime endDateTime;

		public ExtendedDateTime StartDateTime
		{
			get	{	return this.startDateTime;	}
			set	{	this.startDateTime = value;	}
		}

		public ExtendedDateTime EndDateTime
		{
			get	{	return this.endDateTime;	}
			set	{	this.endDateTime = value;	}
		}

		public HistoricalDataStreamer()
		{
			this.tickers = new Hashtable();
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

		public event NewQuoteEventHandler NewQuote;

		/// <summary>
		/// Returns the current bid for the given ticker
		/// </summary>
		/// <param name="ticker"></param>
		/// <returns></returns>
		public double GetCurrentBid( string ticker )
		{
			return HistoricalDataProvider.GetMarketValue( ticker ,
				this.timer.CurrentDateTime );
		}
		/// <summary>
		/// Returns the current ask for the given ticker
		/// </summary>
		/// <param name="ticker"></param>
		/// <returns></returns>
		public double GetCurrentAsk( string ticker )
		{
			return HistoricalDataProvider.GetMarketValue( ticker ,
				this.timer.CurrentDateTime );
		}
		/// <summary>
		/// Starts the time walking simulation
		/// </summary>
		public void GoSimulate()
		{
			this.timer = new Timer( this.startDateTime , this.endDateTime );
			timer.NewExtendedDateTime += new NewExtendedDateTimeHandler(
				this.newExtendedDateTimeHandler );
			timer.Start();
		}

		private void newExtendedDateTimeHandler(
			Object sender , NewExtendedDateTimeEventArgs eventArgs )
		{
			Hashtable quotes =
				HistoricalDataProvider.GetQuotes( this.tickers , eventArgs.ExtendedDateTime );
      foreach ( Quote quote in quotes )
				this.NewQuote( this , new NewQuoteEventArgs( quote ) );
		}
	}
}
