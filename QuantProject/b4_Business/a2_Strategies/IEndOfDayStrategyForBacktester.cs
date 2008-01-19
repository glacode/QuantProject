/*
QuantProject - Quantitative Finance Library

IEndOfDayStrategyForBacktester.cs
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

namespace QuantProject.Business.Strategies
{
	/// <summary>
	/// Interface to be implemented by end of day strategies that are
	/// to be used by the EndOfDayStrategyBackTester
	/// </summary>
	public interface IEndOfDayStrategyForBacktester : IEndOfDayStrategy
	{
		/// <summary>
		/// use this property to signal to the end of day backtester that at current time
		/// a long execution is expected, so that it will decide to stop the execution if
		/// maxRunningHours have already expired. If this property returns false, the
		/// end of day backtesters goes on with its work (that should be all out
		/// of sample and then pretty fast). If you want the backtester to stop
		/// whenever maxRunningHours have elapsed, then set this property
		/// to always return true
		/// </summary>
		bool StopBacktestIfMaxRunningHoursHasBeenReached{ get; }
		event NewLogItemEventHandler NewLogItem;
		/// <summary>
		/// short description to be used as part of the log file name
		/// </summary>
		string DescriptionForLogFileName{ get; }
	}
}
