/*
QuantProject - Quantitative Finance Library

PortfolioReturnComputer.cs
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

using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.DataProviders;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Computes a return for a given portfolio
	/// </summary>
	public class PortfolioReturnComputer : IVirtualReturnComputer
	{
		private WeightedPositions portfolio;
		private IHistoricalMarketValueProvider historicalMarketValueProvider;
		
		public PortfolioReturnComputer(
			WeightedPositions portfolio ,
			IHistoricalMarketValueProvider historicalMarketValueProvider )
		{
			this.portfolio = portfolio;
			this.historicalMarketValueProvider = historicalMarketValueProvider;
		}
		
		#region ComputeReturn
		private ReturnsManager getReturnsManager( ReturnInterval returnInterval )
		{
			ReturnIntervals returnIntervals = new ReturnIntervals( returnInterval );
			ReturnsManager returnsManager = new ReturnsManager(
				returnIntervals , this.historicalMarketValueProvider );
			return returnsManager;
		}
		public double ComputeReturn( ReturnInterval returnInterval )
		{
			ReturnsManager returnsManager = this.getReturnsManager( returnInterval );
			double predictedReturn = this.portfolio.GetReturn( 0 , returnsManager );
			return predictedReturn;
		}
		#endregion ComputeReturn
	}
}
