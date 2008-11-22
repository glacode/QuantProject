/*
QuantProject - Quantitative Finance Library

SimpleAccountProvider.cs
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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;


namespace QuantProject.Business.Financial.Accounting.AccountProviding
{
	/// <summary>
	/// IAccountProvider object that provides an account
	/// with no commissions and slippage
	/// on orders
	/// </summary>
	[Serializable]
	public class SimpleAccountProvider : IAccountProvider
	{
		public SimpleAccountProvider()
		{
		}
		
		public Account GetAccount(
			Timer timer , HistoricalMarketValueProvider historicalMarketValueProvider )
		{
			Account account =
				new Account(
					"SimpleAccount" , timer ,
					new HistoricalDataStreamer(
						timer , historicalMarketValueProvider ) ,
					new HistoricalOrderExecutor(
						timer , historicalMarketValueProvider ) );
			return account;
		}
		
		public string Description
		{
			get
			{
				string description =
					"SimpleAccount_NoComm_NoSlippage";
				return description;
			}
		}
	}
}
