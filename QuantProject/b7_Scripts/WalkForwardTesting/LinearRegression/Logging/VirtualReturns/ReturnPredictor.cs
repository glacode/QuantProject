/*
QuantProject - Quantitative Finance Library

ReturnPredictor.cs
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

using QuantProject.ADT.Econometrics;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Predicts a return using a linear regression estimated parameters
	/// </summary>
	public class ReturnPredictor : IVirtualReturnComputer
	{
		private LinearRegressionTestingPositions linearRegressionTestingPositions;
		private IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling;
		private IHistoricalMarketValueProvider historicalMarketValueProvider;
		
		public ReturnPredictor(
			LinearRegressionTestingPositions linearRegressionTestingPositions ,
			IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling ,
			IHistoricalMarketValueProvider historicalMarketValueProvider
		)
		{
			this.linearRegressionTestingPositions = linearRegressionTestingPositions;
			this.returnIntervalSelectorForSignaling = returnIntervalSelectorForSignaling;
			this.historicalMarketValueProvider = historicalMarketValueProvider;
		}
		
		#region ComputeReturn
		
		#region computeReturn
		
		#region getWeightedReturn
		
		#region getReturn
		private double getReturn(
			WeightedPositions signalingPortfolio , ReturnInterval signalingInterval )
		{
			ReturnsManager returnsManager = new ReturnsManager(
				new ReturnIntervals( signalingInterval ) ,
				this.historicalMarketValueProvider );
			double portfolioReturn = signalingPortfolio.GetReturn( 0 , returnsManager );
			return portfolioReturn;
		}
		#endregion getReturn
		
		private double getWeightedReturn(
			int signalingPortfolioIndex , WeightedPositions signalingPortfolio ,
			ReturnInterval signalingInterval )
		{
			double coefficient =
				this.linearRegressionTestingPositions.LinearRegression.EstimatedCoefficients[
					signalingPortfolioIndex + 1 ];
			double portfolioReturn = this.getReturn( signalingPortfolio , signalingInterval );
			double weightedReturn = coefficient * portfolioReturn;
			return weightedReturn;
		}
		#endregion getWeightedReturn
		
		private double computeReturn( ReturnInterval signalingInterval )
		{
			QuantProject.ADT.Econometrics.ILinearRegression linearRegression =
				this.linearRegressionTestingPositions.LinearRegression;
			WeightedPositions[] signalingPortfolios =
				this.linearRegressionTestingPositions.SignalingPortfolios;
//			double predictedReturn = linearRegression.EstimatedCoefficients[ 0 ];
			double predictedReturn = 0;
			for( int i = 0 ; i < linearRegressionTestingPositions.SignalingPortfolios.Length ; i++ )
				predictedReturn += this.getWeightedReturn(
					i , signalingPortfolios[ i ] , signalingInterval );
			return predictedReturn;
		}
		
		#endregion computeReturn
		
		public double ComputeReturn( ReturnInterval returnInterval )
		{
			ReturnInterval signalingInterval =
				this.returnIntervalSelectorForSignaling.GetReturnIntervalUsedForSignaling(
					returnInterval );
			double predictedReturn = this.computeReturn( signalingInterval );
			return predictedReturn;
		}
		#endregion ComputeReturn
	}
}
