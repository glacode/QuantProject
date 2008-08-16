/*
QuantProject - Quantitative Finance Library

RankBasedOutOfSampleChooser.cs
Copyright (C) 2008
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
using System.Collections;

using QuantProject.ADT.Collections;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.WalkForwardTesting.FixedLengthTwoPhases
{
	/// <summary>
	/// Given the in sample TestingPositions candidates,
	/// this class selects the positions to be opened.
	/// It selects the i_th best element, where i is given
	/// by the rank value (zero based)
	/// </summary>
	public class RankBasedOutOfSampleChooser
	{
		private int rank;
		
		public RankBasedOutOfSampleChooser(
			int rank )
		{
			this.rank = rank;
		}

		#region GetPositionsToBeOpened
		private void getPositionsToBeOpened_checkParameters(
			TestingPositions[] bestTestingPositionsInSample )
		{
			if ( bestTestingPositionsInSample.Length <=
			    this.rank )
				throw new Exception(
					"The out of sample chooser was set to " +
					"return the best position ranked " + this.rank +
					" but bestTestingPositionsInSample contains " +
					"just " + bestTestingPositionsInSample.Length +
					"elements!" );
			
		}
		/// <summary>
		/// Selects the WeghtedPositions to actually be opened
		/// </summary>
		/// <param name="bestTestingPositionsInSample">most correlated couples,
		/// in sample</param>
		/// <param name="outOfSampleReturnIntervals">return intervals for
		/// the current backtest</param>
		/// <param name="minThreshold">min requested inefficiency</param>
		/// <param name="maxThreshold">max allowed inefficiency</param>
		/// <param name="inSampleReturnsManager"></param>
		/// <returns></returns>
		public WeightedPositions GetPositionsToBeOpened(
			TestingPositions[] bestTestingPositionsInSample )
		{
			this.getPositionsToBeOpened_checkParameters(
				bestTestingPositionsInSample );
			WeightedPositions positionsToBeOpened =
				bestTestingPositionsInSample[
					this.rank ].WeightedPositions;
			return positionsToBeOpened;
		}
		#endregion GetPositionsToBeOpened
	}
}
