/*
QuantProject - Quantitative Finance Library

IEligiblesSelector.cs
Copyright (C) 2007
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
using System.Data;

using QuantProject.ADT.Messaging;
using QuantProject.Business.Timing;

namespace QuantProject.Business.Strategies.Eligibles
{
	/// <summary>
	/// Interface for classes that narrow down the number of tickers on which
	/// the in sample optimization will be performed
	/// </summary>
	public interface IEligiblesSelector : IMessageSender
	{
		/// <summary>
		/// Returns a set of eligible tickers. A ReturnIntervals object is
		/// given as a parameter: it may be that, for efficiency, it will
		/// not be used by the implementation
		/// </summary>
		/// <param name="endOfDayHistory">usually, eligible
		/// tickers require to be traded on specific
		/// market days, thus this parameter
		/// is given</param>
		/// <returns></returns>
		EligibleTickers GetEligibleTickers(
			EndOfDayHistory endOfDayHistory );
	}
}
