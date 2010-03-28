/*
QuantProject - Quantitative Finance Library

ExitOnIntervalEnd.cs
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

using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement.Time;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Positions are closed when the current out of sample return interval ends.
	/// That is, if for instance close to close intervals are considered, then
	/// positions are closed on market close
	/// </summary>
	[Serializable]
	public class ExitOnIntervalEnd : IExitStrategy
	{
//		private bool haveParametersBeenSetAfterLastRequest;
//		private ReturnIntervals returnIntervals;
		
		public ExitOnIntervalEnd()
		{
//			this.haveParametersBeenSetAfterLastRequest = false;
//			this.returnIntervals = returnIntervals;
		}

		#region ArePositionsToBeClosed
		/// <summary>
		/// returns true iif now is at the end of the second last interval
		/// </summary>
		/// <param name="now"></param>
		/// <param name="returnIntervals"></param>
		/// <returns></returns>
		public bool ArePositionsToBeClosed( DateTime now , ReturnIntervals returnIntervals )
		{
			bool areToBeClosed = false;
			if ( returnIntervals.Count >= 2 )
			{
				ReturnInterval secondLastInterval = returnIntervals.SeconLastInterval;
				areToBeClosed = ( now == secondLastInterval.End );
			}
			return areToBeClosed;
		}
		#endregion ArePositionsToBeClosed
	}
}
