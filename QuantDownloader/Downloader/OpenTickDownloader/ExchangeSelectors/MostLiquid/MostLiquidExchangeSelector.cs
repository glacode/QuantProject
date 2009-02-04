/*
QuantProject - Quantitative Finance Library

MostLiquidExchangeSelector.cs
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
using System.Collections.Generic;
using System.Threading;

using OTFeed_NET;

using QuantProject.ADT.Messaging;

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	/// <summary>
	/// Finds the most liquid exchange for given tickers
	/// </summary>
	public class MostLiquidExchangeSelector : IExchangeSelector
	{
		public event NewMessageEventHandler NewMessage;
		
//		/// <summary>
//		/// the main exchange for the ticker has been found
//		/// </summary>
//		public event MainExchangeFoundEventHandler MainExchangeFound;
//		
//		/// <summary>
//		/// the ticker is not exchanged in any of the considered exchanges
//		/// </summary>
//		public event ExchangeNotFoundEventHandler ExchangeNotFound;
//		
//		public event NewOHLCRequestEventHandler NewOHLCRequest;
		
		private OTManager oTManager;
		/// <summary>
		/// if non empty, contains the full path to the file where
		/// OTManager's events will be logged 
		/// </summary>
//		private string logFileName;
		
		/// <summary>
		/// keys are the tickers; mostLiquidExchange[ ticker ] is the
		/// most liquid exchange for ticker
		/// </summary>
		private Hashtable mostLiquidExchange;
		
//		private DateTime startingDate;
//		private DateTime endingDate;
		
//		public MostLiquidExchangeSelector( string logFileName )
//		{
////			this.oTManager = oTManager;
//			this.logFileName = logFileName;
//			this.commonInitialization();
//		}
		public MostLiquidExchangeSelector()
		{
			this.oTManager = new OTManager();
//			this.logFileName = "";
			this.mostLiquidExchange = new Hashtable();
		}
//		private void commonInitialization()
//		{
//			this.mostLiquidExchange = new Hashtable();
//		}
		
		#region SelectExchange
		
		#region setExchange
		
		#region setExchange_setMostLiquidExchangeSelectorForSingleTicker
		private void newMessageEventHandler(
			object sender , NewMessageEventArgs eventArgs )
		{
			if ( this.NewMessage != null )
				this.NewMessage( this , eventArgs );
		}
		private MostLiquidExchangeSelectorForSingleTicker
			setExchange_setMostLiquidExchangeSelectorForSingleTicker( string ticker )
		{
			MostLiquidExchangeSelectorForSingleTicker
				mostLiquidExchangeSelectorForSingleTicker =
				new MostLiquidExchangeSelectorForSingleTicker(
					this.oTManager ,
					ticker );
			mostLiquidExchangeSelectorForSingleTicker.NewMessage +=
				new NewMessageEventHandler(
					this.newMessageEventHandler );
//			if ( this.logFileName == "" )
//				// no logging is required
//				mostLiquidExchangeSelectorForSingleTicker =
//					new MostLiquidExchangeSelectorForSingleTicker( ticker );
//			else
//				// logging is required
//				mostLiquidExchangeSelectorForSingleTicker =
//					new MostLiquidExchangeSelectorForSingleTicker(
//						ticker , this.logFileName );
			return mostLiquidExchangeSelectorForSingleTicker;
		}
		#endregion setExchange_setMostLiquidExchangeSelectorForSingleTicker

		private void setExchange( string ticker )
		{
			MostLiquidExchangeSelectorForSingleTicker
				mostLiquidExchangeSelectorForSingleTicker =
				this.setExchange_setMostLiquidExchangeSelectorForSingleTicker( ticker );
//			MostLiquidExchangeSelectorForSingleTicker
//				mostLiquidExchangeSelectorForSingleTicker =
//				new MostLiquidExchangeSelectorForSingleTicker(
//					ticker , this.logFileName );
			mostLiquidExchangeSelectorForSingleTicker.SelectMostLiquidExchange();
			while ( !mostLiquidExchangeSelectorForSingleTicker.IsSearchComplete )
				// the most liquid exchange has not been found, yet
				Thread.Sleep( 200 );
			this.mostLiquidExchange.Add(
				ticker ,
				mostLiquidExchangeSelectorForSingleTicker.MostLiquidExchange );
		}
		#endregion setExchange

		public string SelectExchange( string ticker )
		{
			string exchange;
			if ( !this.mostLiquidExchange.ContainsKey( ticker ) )
				// the most liquid exchang for ticker has not been computed, yet
				this.setExchange( ticker );
			exchange = (string)this.mostLiquidExchange[ ticker ];
			return exchange;
		}
		#endregion SelectExchange
	}
}
