/*
QuantProject - Quantitative Finance Library

BasicDerivedVirtualQuoteProvider.cs
Copyright (C) 2011
Marco Milletti

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
using QuantProject.DataAccess.Tables;
using QuantProject.Business.DataProviders;

namespace QuantProject.Business.DataProviders.VirtualQuotesProviding
{
	/// <summary>
	/// This abstract class implements IVirtualQuotesProvider interface in such a way
	/// that there is a function that returns a quote for each given virtual ticker.
	/// The input data for the (abstract) function are:
	/// - firstQuoteOfUnderlyingRealTicker,
	/// - currentQuoteOfUnderlyingRealTicker,
	/// - firstQuoteOfVirtualTicker 
	/// </summary>
	[Serializable]
	public abstract class BasicDerivedVirtualQuoteProvider : IVirtualQuotesProvider
	{
		protected HistoricalMarketValueProvider historicalMarketValueProvider;
		protected IList<DerivedVirtualTicker> derivedVirtualTickers;
		
		public BasicDerivedVirtualQuoteProvider(IList<DerivedVirtualTicker> derivedVirtualTickers,
		                                        HistoricalMarketValueProvider historicalMarketValueProvider)
		{
			this.derivedVirtualTickers = derivedVirtualTickers;
			this.historicalMarketValueProvider = historicalMarketValueProvider;
		}
		
		protected virtual string getDescription()
		{
			return "drvdVrtlQuotesPrvdr";
		}
		
		//returns -1 if no item in the IList member
		//contains virtualTicker; otherwise
		//returns the index of the first item
		//that contains virtualTicker
		private int getIndexOfTheFirstItemContainingVirtualTicker(string virtualTicker)
		{
			int returnValue = -1;
			for( int i = 0; 
			     i < this.derivedVirtualTickers.Count && returnValue == -1;
			     i++ )
				if( this.derivedVirtualTickers[i].VirtualCodeForTicker == virtualTicker )
					returnValue = i;
			return returnValue;
		}
		
		public virtual bool Contains(	string virtualTicker )
		{
			bool returnValue = false;
			int idxOfVirtualTicker =
				this.getIndexOfTheFirstItemContainingVirtualTicker(virtualTicker);
			if( idxOfVirtualTicker >= 0 )
					returnValue = true;
			return returnValue;
		}
		
		//analogous to WasExchanged method in HistoricalMarketValueProvider class
		public virtual bool IsAvailable(	string virtualTicker, 
		                                  DateTime dateTime )
		{
			bool returnValue = false;
			int idxOfItemContainingVirtualTicker = 
				this.getIndexOfTheFirstItemContainingVirtualTicker(virtualTicker);
			if( idxOfItemContainingVirtualTicker >= 0 )
				returnValue = 
					this.historicalMarketValueProvider.WasExchanged(
						this.derivedVirtualTickers[idxOfItemContainingVirtualTicker].UnderlyingRealTicker,
						dateTime);
			return returnValue;			
		}
		
		protected abstract double getVirtualQuoteActually(double firstQuoteOfUnderlyingRealTicker,
		                                                  double currentQuoteOfUnderlyingRealTicker,
		                                                  double firstQuoteOfVirtualTicker);
		
		protected abstract DateTime getCloseDateTimeAsBase(string realTicker,
		                                                        DateTime currentDateTime);
		
		public virtual double GetVirtualQuote( string virtualTicker ,
		                        								DateTime dateTime,
		                        								HistoricalMarketValueProvider historicalMarketValueProvider)
		{
			double returnValue;
			int idxOfDerivedVirtualTicker =
				this.getIndexOfTheFirstItemContainingVirtualTicker(virtualTicker);
			string realTicker = 
				this.derivedVirtualTickers[idxOfDerivedVirtualTicker].UnderlyingRealTicker;
			
			DateTime closeDateTimeAsBase = this.getCloseDateTimeAsBase(realTicker, dateTime);
				
			double firstQuoteOfUnderlyingRealTicker =
				this.historicalMarketValueProvider.GetMarketValue(realTicker,
				                                                  closeDateTimeAsBase);
		  double currentQuoteOfUnderlyingRealTicker = 
		  	this.historicalMarketValueProvider.GetMarketValue(realTicker,
				                                                  dateTime);
		  double firstQuoteOfVirtualTicker = 
		  	this.derivedVirtualTickers[idxOfDerivedVirtualTicker].FirstVirtualQuote;
			returnValue = this.getVirtualQuoteActually(firstQuoteOfUnderlyingRealTicker,
		                                             currentQuoteOfUnderlyingRealTicker,
		                                             firstQuoteOfVirtualTicker);
		  
			return returnValue;
		}
	}
}
