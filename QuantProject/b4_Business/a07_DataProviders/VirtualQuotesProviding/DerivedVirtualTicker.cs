/*
QuantProject - Quantitative Finance Library

DerivedVirtualTicker.cs
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

using QuantProject.DataAccess.Tables;
using QuantProject.Business.Timing;

namespace QuantProject.Business.DataProviders.VirtualQuotesProviding
{
  /// <summary>
  /// Struct used for the definition of a virtual ticker derived
  /// from an underlying ticker (real)
  /// </summary>
  [Serializable]
  public struct DerivedVirtualTicker
  {
    private string virtualCodeForTicker;
		public string VirtualCodeForTicker
		{
			get
			{
				return this.virtualCodeForTicker;
			}
		}
    
		private string underlyingRealTicker;
		public string UnderlyingRealTicker
		{
			get
			{
				return this.underlyingRealTicker;
			}
		}

    private double firstVirtualQuote;
		public double FirstVirtualQuote
		{
			get
			{
				return this.firstVirtualQuote;
			}
		}
		
		private void derivedVirtualTicker_checkParameters(string virtualCodeForTicker,
			                                          			string underlyingRealTicker,
			                                          			double firstVirtualQuote)
		{
			int numberOfQuotesOfVirtualTickerCodeInDB =
				Quotes.GetNumberOfQuotes(virtualCodeForTicker);
			if(numberOfQuotesOfVirtualTickerCodeInDB > 0)
				throw new Exception("The code for virtual ticker is the same " +
				                    "as a real ticker - with some quotes in DB: " +
				                    "it's necessary to choose a different virtualCode " +
				                    "for the creation of a valid DerivedVirtualTicker instance");
			int numberOfQuotesOfUnderlyingRealTickerInDB =
				Quotes.GetNumberOfQuotes(underlyingRealTicker);
			if(numberOfQuotesOfUnderlyingRealTickerInDB == 0)
				throw new Exception("The underlyingRealTicker has no quotes " +
				                    "in the DB: " +
				                    "it's necessary to choose another underlyingRealTicker code " + 
				                    "for the creation of a valid DerivedVirtualTicker instance");
			if(firstVirtualQuote <= 0)
				throw new Exception("The first - conventional - quote " +
				                    "for the virtual ticker " +
				                    "has to be greater than 0!");
		}
		
		public DerivedVirtualTicker( string virtualCodeForTicker,
					 											 string underlyingRealTicker,
					 											 double firstVirtualQuote)
    {
			this.virtualCodeForTicker = virtualCodeForTicker;
    	this.underlyingRealTicker = underlyingRealTicker;
    	this.firstVirtualQuote = firstVirtualQuote;
    	this.derivedVirtualTicker_checkParameters(virtualCodeForTicker,
			                                          underlyingRealTicker,
			                                          firstVirtualQuote);
    }
  }
}
