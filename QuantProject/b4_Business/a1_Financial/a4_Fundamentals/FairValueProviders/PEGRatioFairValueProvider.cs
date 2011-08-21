/*
QuantProject - Quantitative Finance Library

PEGRatioFairValueProvider.cs
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
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Fundamentals;
using QuantProject.Business.Financial.Fundamentals.RatioProviders;


namespace QuantProject.Business.Financial.Fundamentals.FairValueProviders
{
	/// <summary>
	/// Class implementing IFairValueProvider Interface
	/// using the popular PEG ratio (P/E ratio divided by an expected 
	/// growth rate)
	/// </summary>
	[Serializable]
	public class PEGRatioFairValueProvider : IFairValueProvider
	{
		private double fairPEGRatioLevel;
		private IRatioProvider_PE ratioProvider_PE;
		private IGrowthRateProvider growthRateProvider;
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		
		public PEGRatioFairValueProvider(double fairPEGRatioLevel,
		                                 IRatioProvider_PE ratioProvider_PE,
		 																 IGrowthRateProvider growthRateProvider,
		                                 HistoricalMarketValueProvider historicalMarketValueProvider)
		{
			this.fairPEGRatioLevel = fairPEGRatioLevel;
			this.ratioProvider_PE = ratioProvider_PE;
			this.growthRateProvider = growthRateProvider;
			this.historicalMarketValueProvider = historicalMarketValueProvider;
		}
		
		private double getPEGRatio(string ticker ,
		                           DateTime dateOfFairValueComputation)
		{
			double returnValue;
			double growthRate = this.growthRateProvider.GetGrowthRate(ticker,
				      dateOfFairValueComputation);
			double PE = this.ratioProvider_PE.GetPERatio(ticker,
				      dateOfFairValueComputation);
			if(growthRate < 0)
				returnValue = double.MaxValue;
			else
				returnValue = PE / (100.0 * growthRate);
			
			return returnValue;
		}
		
		public void Run(DateTime dateTime)
		{
			;
		}
		
		public string Description
		{
			get
			{
				return "PEGRatioFairValueProvider";	
			}
		}
		
		public double GetFairValue( string ticker ,
		                     				DateTime dateOfFairValueComputation )
		{
			double returnValue;
			double priceAtDateOfFairValueComputation = 
				this.historicalMarketValueProvider.GetMarketValue(ticker,
				                                            dateOfFairValueComputation);
			returnValue = priceAtDateOfFairValueComputation * this.fairPEGRatioLevel /
				            this.getPEGRatio(ticker,
				                             dateOfFairValueComputation);
			
			return returnValue;
		}
	}
}
