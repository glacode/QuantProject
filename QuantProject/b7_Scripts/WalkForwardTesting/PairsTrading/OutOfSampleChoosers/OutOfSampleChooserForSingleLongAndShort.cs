/*
QuantProject - Quantitative Finance Library

OutOfSampleChooserForSingleLongAndShort.cs
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

using QuantProject.ADT.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Selects a single couple with a long and a short position
	/// </summary>
	[Serializable]
	public class OutOfSampleChooserForSingleLongAndShort :
		OutOfSampleChooser
	{
		/// <summary>
		/// Selects a single couple with a long and a short position
		/// </summary>
		public OutOfSampleChooserForSingleLongAndShort(
			Time firstTimeToTestInefficiency ,
			double minThresholdForGoingLong ,
			double maxThresholdForGoingLong ,
			double minThresholdForGoingShort ,
			double maxThresholdForGoingShort ) :
			base(
				firstTimeToTestInefficiency ,
				minThresholdForGoingLong ,
				maxThresholdForGoingLong ,
				minThresholdForGoingShort ,
				maxThresholdForGoingShort )
		{
		}
		protected override WeightedPositions getPositionsToBeOpened(
			WeightedPositions[] inefficientCouples ,
			ReturnsManager inSampleReturnsManager )
		{
			WeightedPositions positionsToBeOpened = null;
			if ( ( inefficientCouples != null ) &&
			    ( inefficientCouples.Length > 0 ) )
				// at least an inefficient couple has been found
				positionsToBeOpened = inefficientCouples[ 0 ];
			return positionsToBeOpened;
		}
	}
}
