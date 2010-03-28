/*
QuantProject - Quantitative Finance Library

ShiftedTimeIntervalSelectorForSignaling.cs
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

using QuantProject.Business.Strategies.ReturnsManagement.Time;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Description of ShiftedTimeIntervalSelectorForSignaling.
	/// </summary>
	[Serializable]
	public class ShiftedTimeIntervalSelectorForSignaling :
		IReturnIntervalSelectorForSignaling
	{
		TimeSpan timeSpanToBeShifted;
		
		/// <summary>
		/// Given a trading ReturnInterval, builds a signaling ReturnInterval
		/// that is shifted the given time span
		/// </summary>
		/// <param name="timeSpanToBeShifted">time span used to build the
		/// signaling ReturnInterval; use a negative value in order to build
		/// a signaling ReturnInterval that is before the trading
		/// ReturnInterval</param>
		public ShiftedTimeIntervalSelectorForSignaling(
		TimeSpan timeSpanToBeShifted )
		{
			this.timeSpanToBeShifted = timeSpanToBeShifted;
		}
		
		public ReturnInterval GetReturnIntervalUsedForSignaling(
			ReturnInterval returnIntervalForTrading )
		{
			DateTime signalingReturnIntervalBegin =
				returnIntervalForTrading.Begin.Add( this.timeSpanToBeShifted );
			DateTime signalingReturnIntervalEnd =
				returnIntervalForTrading.End.Add( this.timeSpanToBeShifted );
			ReturnInterval returnIntervalUsedForSignaling = new ReturnInterval(
				signalingReturnIntervalBegin , signalingReturnIntervalEnd );
			return returnIntervalUsedForSignaling;
		}
	}
}
