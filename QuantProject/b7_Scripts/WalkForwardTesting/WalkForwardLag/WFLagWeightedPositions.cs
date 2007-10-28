/*
QuantProject - Quantitative Finance Library

WFLagWeightedPositions.cs
Copyright (C) 2003 
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

using QuantProject.Business.Strategies;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// This class identifies all the weighted positions to apply
	/// the lag strategy out of sample: it contains both the
	/// driving positions and the portfolio positions. Each genome
	/// can be decoded to an instance of this class
	/// </summary>
	[Serializable]
	public class WFLagWeightedPositions
	{
		private WeightedPositions drivingWeightedPositions;
		private WeightedPositions portfolioWeightedPositions;

		public WeightedPositions DrivingWeightedPositions
		{
			get { return this.drivingWeightedPositions; }
		}
		public WeightedPositions PortfolioWeightedPositions
		{
			get { return this.portfolioWeightedPositions; }
		}

		public static WFLagWeightedPositions TestInstance
		{
			get
			{
				WFLagWeightedPositions  testInstance = new WFLagWeightedPositions(
					WeightedPositions.TestInstance , WeightedPositions.TestInstance );
				return testInstance;
			}
		}
		public WFLagWeightedPositions(
			WeightedPositions drivingWeightedPositions ,
			WeightedPositions portfolioWeightedPositions )
		{
			this.drivingWeightedPositions = drivingWeightedPositions;
			this.portfolioWeightedPositions = portfolioWeightedPositions;
		}
	}
}
