/*
QuantProject - Quantitative Finance Library

DrivenBySharpeRatioMain.cs
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
using System.Collections;
using System.Collections.Generic;
using System.IO;

using QuantProject.ADT;
using QuantProject.ADT.Statistics.Combinatorial;
using QuantProject.ADT.FileManaging;
using QuantProject.ADT.Timing;
using QuantProject.Business.DataProviders;
//using QuantProject.Data.DataProviders.Bars.Caching;
using QuantProject.Business.Strategies;
using QuantProject.Business.Financial.Accounting.AccountProviding;
using QuantProject.Business.Financial.Fundamentals.FairValueProviders;
using QuantProject.Business.Financial.Fundamentals.RatioProviders;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.GenomeManagers;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;
using QuantProject.Presentation;
using QuantProject.Scripts.General;
using QuantProject.Scripts.General.Logging;
using QuantProject.Scripts.General.Reporting;
using QuantProject.Scripts.General.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Scripts.TickerSelectionTesting.DrivenBySharpeRatio.InSampleChoosers.Genetic;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
//using QuantProject.Scripts.TickerSelectionTesting.OTC.InSampleChoosers.Genetic;
//using QuantProject.Scripts.TickerSelectionTesting.OTC.InSampleChoosers.BruteForce;

namespace QuantProject.Scripts.TickerSelectionTesting.DrivenBySharpeRatio
{
	/// <summary>
	/// Entry point for the DrivenBySharpeRatioStrategy. If any strategy
	/// parameter had to be changed, this is the place where it should
	/// be done
	/// </summary>
	public class DrivenBySharpeRatioMain : BasicScriptForBacktesting
	{
		private int numberOfPortfolioPositions;
		private double stopLoss;
		private double takeProfit;
		private PortfolioType portfolioType;
		private int maxNumberOfEligiblesToBeChosen;
		private int numDaysForInSample;
		private Benchmark benchmark;
		private DateTime firstDateTime;
		private DateTime lastDateTime;
		private HistoricalMarketValueProvider historicalMarketValueProviderForInSample;
		private HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample;
		private HistoricalMarketValueProvider historicalMarketValueProviderForTheBackTester;
		private Timer timerForBackTester;
		private GenomeManagerType genomeManagerType;
		
		#region main
		public DrivenBySharpeRatioMain()
		{
			this.numberOfPortfolioPositions = 4;
			this.benchmark = new Benchmark( "^MIBTEL" );
//			this.benchmark = new Benchmark( "^GSPC" );
			this.portfolioType = PortfolioType.OnlyLong;//filter for out of sample
			this.genomeManagerType = GenomeManagerType.OnlyLong;//filter for the genetic chooser
//			this.benchmark = new Benchmark( "ENI.MI" );
			this.firstDateTime = new DateTime( 2001 , 1 , 1 );
			this.lastDateTime =  new DateTime( 2005 , 12 , 31 );
			
			this.historicalMarketValueProviderForInSample =
//				new HistoricalRawQuoteProvider();
			  new HistoricalAdjustedQuoteProvider();
			this.historicalMarketValueProviderForOutOfSample =
				this.historicalMarketValueProviderForInSample;
			this.historicalMarketValueProviderForTheBackTester =
				this.historicalMarketValueProviderForOutOfSample;
				//ricordarsi di togliere - mettere
				//commento nel gestore evento tempo
			
			this.numDaysForInSample = 180;

			this.timerForBackTester =
				new IndexBasedEndOfDayTimer( this.firstDateTime,
				                             this.lastDateTime,
				                             this.benchmark.Ticker);
			this.stopLoss = 1.50;//deactivated
			this.takeProfit = -0.20;//deactivated
		}
		#endregion main
		
		#region eligiblesSelector
		protected override IEligiblesSelector getEligiblesSelector()
		{
			this.maxNumberOfEligiblesToBeChosen = 200;
//			string tickersGroupId = "ticUSFin";
//			string tickersGroupId = "USFunds";
//			string tickersGroupId = "SP500";
			string tickersGroupId = "STOCKMI";
			
			bool temporizedGroup = false;//Attenzione!
			
			IEligiblesSelector eligiblesSelector =
//				new ByGroup( tickersGroupId , temporizedGroup);
				
				new ByLiquidity ( tickersGroupId , temporizedGroup ,
				maxNumberOfEligiblesToBeChosen);
//			eligiblesSelector = 
//				new DummyEligibleSelector();
//			
			return eligiblesSelector;
		}
		#endregion eligiblesSelector
		
		#region inSampleChooser
		protected override IInSampleChooser getInSampleChooser()
		{
			int numberOfBestTestingPositionsToBeReturned = 5;
			// parameters for the genetic optimizer
			double crossoverRate = 0.85;
			double mutationRate = 0.02;
			double elitismRate = 0.001;
			int populationSizeForGeneticOptimizer = 25000;
			int generationNumberForGeneticOptimizer = 25;
			int seedForRandomGenerator =
				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator;
			
			BuyAndHoldFitnessEvaluator fitnessEvaluator = 
				new BuyAndHoldFitnessEvaluator( new SharpeRatio() );
			
			BasicDecoderForGeneticallyOptimizableTestingPositions basicGenOptDecoder = 
				new BasicDecoderForGeneticallyOptimizableTestingPositions();
			
			IInSampleChooser inSampleChooser = 
				new DrivenBySharpeRatioInSampleChooser(this.numberOfPortfolioPositions,
				                                      numberOfBestTestingPositionsToBeReturned,
						benchmark, basicGenOptDecoder, this.genomeManagerType ,
						fitnessEvaluator , historicalMarketValueProviderForInSample,
						crossoverRate, mutationRate, elitismRate ,
						populationSizeForGeneticOptimizer, generationNumberForGeneticOptimizer,
						seedForRandomGenerator);
			
			return inSampleChooser;
		}
		#endregion inSampleChooser
		
		#region strategy
		protected override IStrategyForBacktester getStrategyForBacktester()
		{
			int numDaysBetweenEachOptimization = 60;
			int minNumOfEligiblesForValidOptimization = 15;
			
			IStrategyForBacktester strategyForBacktester
				= new DrivenBySharpeRatioStrategy(eligiblesSelector ,
				minNumOfEligiblesForValidOptimization, inSampleChooser ,
				numDaysForInSample ,
				benchmark , numDaysBetweenEachOptimization ,
				historicalMarketValueProviderForInSample ,
			  historicalMarketValueProviderForOutOfSample ,
			  this.portfolioType, this.stopLoss,
			  this.takeProfit);
			
			return strategyForBacktester;
		}
		#endregion strategy
		
		#region backTester
		protected override EndOfDayStrategyBackTester getEndOfDayStrategyBackTester()
		{
			string backTestId = "DrivenBySharpeRatioStrategy";
			IAccountProvider accountProvider;
			accountProvider =	new SimpleAccountProvider();
//			double fixedPercentageSlippage = 0.05;
//			accountProvider =
//				new InteractiveBrokerAccountProvider(fixedPercentageSlippage);
			double cashToStart = 10000;
			double maxRunningHours = 15;
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
				backTestId , this.timerForBackTester, 
				this.strategyForBacktester ,
				this.historicalMarketValueProviderForTheBackTester , accountProvider ,
				firstDateTime ,	lastDateTime ,
				this.benchmark , cashToStart , maxRunningHours );
			return endOfDayStrategyBackTester;
		}
		#endregion backTester
						
		protected override string getCustomSmallTextForFolderName()
		{
			return "SharpeRatio";
		}
		
		protected override string getPathForTheMainFolderWhereScriptsResultsAreToBeSaved()
		{
			string pathForTheMainFolderWhereScriptsResultsAreToBeSaved =
				System.Configuration.ConfigurationManager.AppSettings["LogArchive"];
			return pathForTheMainFolderWhereScriptsResultsAreToBeSaved;
		}
		
		protected override string getFullPathFileNameForMain()
		{
			string returnValue;
			string fullPathFileNameForMainAtHome = 
				@"C:\Quant\QuantProject\b7_Scripts\TickerSelectionTesting\DrivenBySharpeRatio\DrivenBySharpeRatioMain.cs";
			if( File.Exists(fullPathFileNameForMainAtHome) )
				returnValue = fullPathFileNameForMainAtHome;
			else
				returnValue = 
					@"C:\Utente\MarcoTributi\Vari\Vari\qP\QuantProject\b7_Scripts\TickerSelectionTesting\DrivenBySharpeRatio\DrivenBySharpeRatioMain.cs";
			
			return returnValue;
		}
	}
}
