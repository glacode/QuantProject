/*
QuantProject - Quantitative Finance Library

IReturnIntervalsBuilderForTradingAndForSignaling.cs
Copyright (C) 2010
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

using QuantProject.Business.Strategies.Eligibles;

namespace QuantProject.Business.Strategies.ReturnsManagement.Time
{
	/// <summary>
	/// Interface to be implemented by those classes that build intervals
	/// for trading and for signaling
	/// </summary>
	public interface IReturnIntervalsBuilderForTradingAndForSignaling
	{
		/// <summary>
		/// Builds returnIntervalsForTrading and returnIntervalsForSignaling
		/// </summary>
		/// <param name="returnsManager"></param>
		/// <param name="eligibleTickersForTrading"></param>
		/// <param name="eligibleTickersForSignaling"></param>
		/// <param name="returnIntervalsForTrading"></param>
		/// <param name="returnIntervalsForSignaling"></param>
		void BuildIntervals(
			IReturnsManager returnsManager ,
			IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling ,
			string[] eligibleTickersForTrading ,
			string[] eligibleTickersForSignaling ,
			out ReturnIntervals returnIntervalsForTrading ,
			out ReturnIntervals returnIntervalsForSignaling );
//		ReturnIntervals ReturnIntervalsForTrading { get; }
//		ReturnIntervals ReturnIntervalsForSignaling { get; }
	}
}
