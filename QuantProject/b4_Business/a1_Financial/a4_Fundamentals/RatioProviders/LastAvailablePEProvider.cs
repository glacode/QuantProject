/*
QuantProject - Quantitative Finance Library

LastAvailablePEProvider.cs
Copyright (C) 2010
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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.*/

using System;
using QuantProject.DataAccess.Tables;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Fundamentals;
using QuantProject.Business.Financial.Fundamentals.RatioProviders;


namespace QuantProject.Business.Financial.Fundamentals.RatioProviders
{
	/// <summary>
	/// Class implementing IRatioProvider_PE Interface
	/// It provides the last available PE, using the last available price
	/// and the last available Earnings.
	/// Instead of the last available price, 
	/// the average of the last available prices can be used 
	/// (it depends on how the class has been instatiated)
	/// </summary>
	[Serializable]
	public class LastAvailablePEProvider : FundamentalDataProvider,
																				 IRatioProvider_PE
	{
		private HistoricalMarketValueProvider historicalmarketValueProvider;
		
		public LastAvailablePEProvider(HistoricalMarketValueProvider historicalmarketValueProvider,
		                               int daysForAvailabilityOfData) : 
																	 base(daysForAvailabilityOfData)
		{
			this.historicalmarketValueProvider = historicalmarketValueProvider;
		}
		
		private double getPERatio_getLastAvailableEarningsPerShare(string ticker , DateTime atDate)
		{
			double returnValue;
			DateTime limitDateForEndingPeriodDate = 
				atDate.AddDays(- this.daysForAvailabilityOfData);
			returnValue = 
				FinancialValues.GetLastFinancialValueForTicker(ticker, FinancialValueType.EarningsPerShare, 12,
			                                    limitDateForEndingPeriodDate);
			return returnValue;
		}
		
		public double GetPERatio( string ticker , DateTime atDate )
		{
			double returnValue;
			double priceAtDate = 
				this.historicalmarketValueProvider.GetMarketValue(ticker,
			 	                                            atDate);
			double earningsPerShareAtDate =
				this.getPERatio_getLastAvailableEarningsPerShare(ticker , atDate);
			
			returnValue = priceAtDate / earningsPerShareAtDate;
			if(returnValue > 1000.0)
			//if earnings is zero or near to zero ...
				returnValue = 1000.0;
			else if(returnValue < -1000.0)
				returnValue = -1000.0;
			
			return returnValue;
		}
		public override double GetValue( string ticker , DateTime atDate )
		{
			return this.GetPERatio(ticker, atDate);
		}
	}
}
