/*
QuantProject - Quantitative Finance Library

InteractiveBrokerAccountProvider.cs
Copyright (C) 2008
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

using QuantProject.ADT;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting.Commissions;
using QuantProject.Business.Financial.Accounting.Slippage;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;


namespace QuantProject.Business.Financial.Accounting.AccountProviding
{
	/// <summary>
	/// IAccountProvider object that provides a typical account
	/// with Interactive Broker for individual traders,
	/// where commissions are managed and slippage can be simulated
	/// through a fixed percentage amount (on stock-price)
	/// lost at each stock-transaction
	/// </summary>
	public class InteractiveBrokerAccountProvider : IAccountProvider
	{
		private double slippageFixedPercentage;
		
		public InteractiveBrokerAccountProvider()
		{
			this.slippageFixedPercentage = 0.0;
		}
		
		public InteractiveBrokerAccountProvider(double slippageFixedPercentage)
		{
			if( slippageFixedPercentage < 0.0 ||
			   slippageFixedPercentage > 100.0 )
				throw new OutOfRangeException("slippageFixedPercentage", 0.0, 100.0);
			
			this.slippageFixedPercentage = slippageFixedPercentage;
		}
		
		private ISlippageManager getAccount_getSlippageManager(
			Timer timer , HistoricalMarketValueProvider historicalMarketValueProvider)
		{
			ISlippageManager slippageManager;
			if(this.slippageFixedPercentage == 0.0)
				slippageManager = new ZeroSlippageManager();
			else//this.slippageFixedPercentage > 0.0
				slippageManager = new FixedPercentageSlippageManager(
					historicalMarketValueProvider , timer ,
					this.slippageFixedPercentage );
			return slippageManager;
		}
		
		public Account GetAccount(
			Timer timer , HistoricalMarketValueProvider historicalMarketValueProvider )
		{
			Account account =
				new Account(
					"IBAccount" , timer ,
					new HistoricalDataStreamer(
						timer , historicalMarketValueProvider ) ,
					new HistoricalOrderExecutor(
						timer , historicalMarketValueProvider ,
						this.getAccount_getSlippageManager(
							timer , historicalMarketValueProvider ) ),
					new IBCommissionManager() );
			return account;
		}
		
		public string Description
		{
			get
			{
				string description =
					"IBAccountForIndividualTraders";
				return description;
			}
		}
	}
}
