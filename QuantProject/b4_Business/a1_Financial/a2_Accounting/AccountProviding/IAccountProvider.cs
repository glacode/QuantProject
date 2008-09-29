/*
QuantProject - Quantitative Finance Library

IAccountProvider.cs
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
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Timing;


namespace QuantProject.Business.Financial.Accounting.AccountProviding
{
	/// <summary>
	/// Interface to be implemented by objects
	/// that provide a new account through the
	/// GetAccount method.
	/// These objects are used at the moment
	/// by the EndOfDaysStrategyBackTester
	/// </summary>
	public interface IAccountProvider : ILogDescriptor
	{
		/// <summary>
		/// Returns a new account
		/// </summary>
		/// <param name="instrumentKey">instrument identifier</param>
		/// <param name="endOfDayDateTime">end of day date time for the market evaluation</param>
		/// <returns></returns>
		Account GetAccount(
			Timer timer ,
			HistoricalMarketValueProvider historicalMarketValueProvider );
	}
}
