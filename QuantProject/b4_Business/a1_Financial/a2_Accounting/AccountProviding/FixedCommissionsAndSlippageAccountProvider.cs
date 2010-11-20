/*
QuantProject - Quantitative Finance Library

FixedCommissionsAndSlippageAccountProvider.cs
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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using System;

using QuantProject.ADT;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Financial.Accounting.Commissions;
using QuantProject.Business.Financial.Accounting.Slippage;
using QuantProject.Business.Timing;


namespace QuantProject.Business.Financial.Accounting.AccountProviding
{
	/// <summary>
	/// IAccountProvider object that provides an account
	/// with fixed commissions (also 0) and fixed percentage slippage (also 0)
	/// on orders
	/// </summary>
	[Serializable]
	public class FixedCommissionsAndSlippageAccountProvider : IAccountProvider
	{
		private double slippageFixedPercentage;
		private double fixedCommission;
		
		public FixedCommissionsAndSlippageAccountProvider(double fixedCommission, 
		                                                  double slippageFixedPercentage)
		{
			this.fixedCommission = fixedCommission;
			
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
					new FixedCommissionManager(this.fixedCommission) );
			return account;
		}
		
		public string Description
		{
			get
			{
				string description =
					"SimpleAccount_FixedComm_Fixed%Slippage";
				return description;
			}
		}
	}
}

