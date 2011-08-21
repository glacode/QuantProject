/*
QuantProject - Quantitative Finance Library

DrivenBySharpeRatioBruteForceChooser.cs
Copyright (C) 2011
Marco Milletti

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

using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.ADT.Optimizing.BruteForce;
using QuantProject.Business.Strategies;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.Optimizing.GenomeManagers;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;


namespace QuantProject.Scripts.TickerSelectionTesting.DrivenBySharpeRatio.InSampleChoosers.BruteForce
{
	/// <summary>
	/// brute force IInSampleChooser with End Of Day data for the strategy
	/// driven by SharpeRatio
	/// </summary>
	[Serializable]
	public class DrivenBySharpeRatioBruteForceChooser : BruteForceChooser
	{
		private int numberOfPortfolioPositions;
		private PortfolioType portfolioType;
		
		public DrivenBySharpeRatioBruteForceChooser( PortfolioType portfolioType,
			int numberOfPortfolioPositions , int numberOfBestTestingPositionsToBeReturned ,
			Benchmark benchmark ,
			IDecoderForTestingPositions decoderForTestingPositions ,
			IFitnessEvaluator fitnessEvaluator ,
			HistoricalMarketValueProvider historicalMarketValueProvider) :
			base (
			numberOfBestTestingPositionsToBeReturned ,
			decoderForTestingPositions ,
			fitnessEvaluator )
		{
			this.portfolioType = portfolioType;
			this.numberOfPortfolioPositions = numberOfPortfolioPositions;
		}
		protected override IBruteForceOptimizableParametersManager
			getBruteForceOptimizableParametersManager(
			EligibleTickers eligibleTickers ,
			ReturnsManager returnsManager )
		{
			IBruteForceOptimizableParametersManager parametersManager;
			parametersManager = new DrivenBySharpeROptimizableParametersManager(
				eligibleTickers ,
				this.numberOfPortfolioPositions	,
				this.decoderForTestingPositions ,
				this.fitnessEvaluator ,
				returnsManager );
							
			return parametersManager;
		}
	}
}
