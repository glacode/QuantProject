/*
QuantProject - Quantitative Finance Library

OTCEndOfDayBruteForceChooser.cs
Copyright (C) 2009
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


namespace QuantProject.Scripts.TickerSelectionTesting.OTC.InSampleChoosers.BruteForce
{
	/// <summary>
	/// brute force IInSampleChooser with End Of Day data for the OTC strategy
	/// </summary>
	[Serializable]
	public class OTCEndOfDayBruteForceChooser : BruteForceChooser
	{
		private int numberOfPortfolioPositions;
		private PortfolioType portfolioType;
		
		public OTCEndOfDayBruteForceChooser( PortfolioType portfolioType,
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
			if(this.portfolioType == PortfolioType.OnlyLong)
				parametersManager = new OTCOnlyLongEndOfDayBruteForceOptimizableParametersManager(
				eligibleTickers ,
				this.numberOfPortfolioPositions	,
				this.decoderForTestingPositions ,
				this.fitnessEvaluator ,
				returnsManager );
			else if(this.portfolioType == PortfolioType.OnlyShort)
				parametersManager = new OTCOnlyShortEndOfDayBruteForceOptimizableParametersManager(
				eligibleTickers ,
				this.numberOfPortfolioPositions	,
				this.decoderForTestingPositions ,
				this.fitnessEvaluator ,
				returnsManager );
			else//short and long or only mixed portfolio types
				parametersManager = new OTCEndOfDayBruteForceOptimizableParametersManager(
				eligibleTickers ,
				this.numberOfPortfolioPositions	,
				this.decoderForTestingPositions ,
				this.fitnessEvaluator ,
				returnsManager );
				
			return parametersManager;
		}
	}
}


