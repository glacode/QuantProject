/*
QuantProject - Quantitative Finance Library

IWFLagWeightedPositionsChooser.cs
Copyright (C) 2006
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

using QuantProject.ADT;
using QuantProject.Business.Timing;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger;


namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WeightedPositionsChoosers
{
	/// <summary>
	/// Interface to be implemented by any object used to chose in
	/// sample WeghtedPositions.
	/// </summary>
	public interface IWFLagWeightedPositionsChooser : IProgressNotifier
	{
		void ChosePositions(
			WFLagEligibleTickers eligibleTickersForDrivingPositions ,
			WFLagEligibleTickers eligibleTickersForPortfolioPositions ,
			DateTime now );
		WFLagWeightedPositions WFLagChosenPositions { get; }
		int NumberOfDrivingPositions { get; }
		int NumberOfPortfolioPositions { get; }
		int NumberDaysForInSampleOptimization { get; }
		/// <summary>
		/// If the chooser doesn't use a genetic optimizer, this property
		/// is meaningless, thus it will be set to a negative number
		/// </summary>
		int GenerationWhenChosenPositionsWereFound { get; }
		string Benchmark { get; }  // TO DO: remove this one from the interface
	}
}
