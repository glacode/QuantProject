/*
QuantProject - Quantitative Finance Library

DrivenByFVProviderMain.cs
Copyright (C) 2010
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
using QuantProject.Scripts.TickerSelectionTesting.DrivenByFundamentals.DrivenByFairValueProvider.InSampleChoosers.Genetic;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
//using QuantProject.Scripts.TickerSelectionTesting.OTC.InSampleChoosers.Genetic;
//using QuantProject.Scripts.TickerSelectionTesting.OTC.InSampleChoosers.BruteForce;

namespace QuantProject.Scripts.TickerSelectionTesting.DrivenByFundamentals.DrivenByFairValueProvider
{
	/// <summary>
	/// Entry point for the DrivenByFairValueProviderStrategy. If any strategy
	/// parameter had to be changed, this is the place where it should
	/// be done
	/// </summary>
	public class DrivenByFVProviderMain : BasicScriptForBacktesting
	{
		private int numberOfPortfolioPositions;
		private double stopLoss;
		private double percentageOfTheoreticalProfitForExit;
		private double takeProfitLevelInAnyCase;
		private PortfolioType portfolioType;
		private int maxNumberOfEligiblesToBeChosen;
		private IFairValueProvider fairValueProvider;
		private int numDaysForFundamentalAnalysis;
		private int numDaysForPortfolioVolatilityAnalysis;
		private int numDaysForFundamentalDataAvailability;
		private Benchmark benchmark;
		private DateTime firstDateTime;
		private DateTime lastDateTime;
		private HistoricalMarketValueProvider historicalMarketValueProviderForInSample;
		private HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample;
		private HistoricalMarketValueProvider historicalMarketValueProviderForTheBackTester;
		private Timer timerForBackTester;
		private GenomeManagerType genomeManagerType;
		
		#region main
		public DrivenByFVProviderMain()
		{
			this.numberOfPortfolioPositions = 4;
			this.benchmark = new Benchmark( "^GSPC" );
			this.portfolioType = PortfolioType.OnlyLong;//filter for out of sample
			this.genomeManagerType = GenomeManagerType.OnlyLong;//filter for the genetic chooser
//			this.benchmark = new Benchmark( "ENI.MI" );
			this.firstDateTime = new DateTime( 2002 , 4 , 1 );
			this.lastDateTime = new DateTime( 2009 , 3, 31 );
			
			this.historicalMarketValueProviderForInSample =
//				new HistoricalRawQuoteProvider();
			  new HistoricalAdjustedQuoteProvider();
			this.historicalMarketValueProviderForOutOfSample =
				this.historicalMarketValueProviderForInSample;
			this.historicalMarketValueProviderForTheBackTester =
				this.historicalMarketValueProviderForOutOfSample;
				//ricordarsi di togliere - mettere
				//commento nel gestore evento tempo
			this.numDaysForFundamentalDataAvailability = 30;
//			double optimalDebtEquityRatioLevel = 0.1;
//			int maxNumOfGrowthRatesToTakeIntoAccount = 4;
			IGrowthRateProvider growthProvider =
//				new AverageAndDebtAdjustedGrowthRateProvider(numDaysForFundamentalDataAvailability,
//				                                             maxNumOfGrowthRatesToTakeIntoAccount,
//				                                             optimalDebtEquityRatioLevel);
				new LastAvailableGrowthRateProvider(numDaysForFundamentalDataAvailability);
			
			IRatioProvider_PE PEProvider =
				new LastAvailablePEProvider(this.historicalMarketValueProviderForInSample,
				                            numDaysForFundamentalDataAvailability);
			double fairPEGRatioLevel = 1.0;
			this.fairValueProvider = 
				new PEGRatioFairValueProvider(fairPEGRatioLevel,PEProvider,
				                              growthProvider,this.historicalMarketValueProviderForInSample);
			this.numDaysForFundamentalAnalysis = 365;
			this.numDaysForPortfolioVolatilityAnalysis = 90;

			this.timerForBackTester =
				new IndexBasedEndOfDayTimer( this.firstDateTime,
				                             this.lastDateTime,
				                             this.benchmark.Ticker);
			this.stopLoss = 100.0;
			this.percentageOfTheoreticalProfitForExit = 0.50;
			// it determines the take profit level, by 
			// multiplying the theoretical profit that would
			// be gained if each position in portfolio reached its 
			// fair value computed by the IFairValueProvider
			this.takeProfitLevelInAnyCase = 100.0;
		}
		#endregion main
		
		#region eligiblesSelector
		protected override IEligiblesSelector getEligiblesSelector()
		{
			this.maxNumberOfEligiblesToBeChosen = 800;
			string tickersGroupId = "ticUSFin";
//			string tickersGroupId = "SP500";
//			string tickersGroupId = "STOCKMI";
			
			bool temporizedGroup = true;//Attenzione!
			double minimumIncome = 10000000.0;//10 mln
			int numOfMinIncomeInARow = 4;
			double minimumRelativeDifferenceBetweenFairAndAverageMarketPrice = 0.05;
			int numDaysForAveragePriceComputation = 10;
			
			IEligiblesSelector eligiblesSelector =
				new ByMostDiscountedPrices( this.fairValueProvider ,
				tickersGroupId , temporizedGroup ,
				maxNumberOfEligiblesToBeChosen , this.numDaysForFundamentalAnalysis,
				this.numDaysForFundamentalDataAvailability,
				minimumIncome, numOfMinIncomeInARow,
				minimumRelativeDifferenceBetweenFairAndAverageMarketPrice,
				numDaysForAveragePriceComputation);
//				new ByLiquidity ( tickersGroupId , temporizedGroup ,
//					maxNumberOfEligiblesToBeChosen );
			
			
//			eligiblesSelector = 
//				new DummyEligibleSelector();
//			
			return eligiblesSelector;
		}
		#endregion eligiblesSelector
		
		#region inSampleChooser
		protected override IInSampleChooser getInSampleChooser()
		{
			int numberOfBestTestingPositionsToBeReturned = 1;
			
			// parameters for the genetic optimizer
			double crossoverRate = 0.85;
			double mutationRate = 0.02;
			double elitismRate = 0.001;
			int populationSizeForGeneticOptimizer = 40000;
			int generationNumberForGeneticOptimizer = 50;
			int seedForRandomGenerator =
				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator;
			
			BuyAndHoldFitnessEvaluator fitnessEvaluator = 
//				new BuyAndHoldFitnessEvaluator( new Variance() );
				new BuyAndHoldFitnessEvaluator( new SharpeRatio() );
			
			bool mixPastReturnsEvaluationWithFundamentalEvaluation =
				false;
			
			BasicDecoderForGeneticallyOptimizableTestingPositions basicGenOptDecoder = 
				new BasicDecoderForGeneticallyOptimizableTestingPositions();
			
			IInSampleChooser inSampleChooser = 
				new DrivenByFVProviderInSampleChooser(this.numberOfPortfolioPositions,
				                                      numberOfBestTestingPositionsToBeReturned,
						benchmark, basicGenOptDecoder, this.genomeManagerType ,
						fitnessEvaluator , mixPastReturnsEvaluationWithFundamentalEvaluation,
						historicalMarketValueProviderForInSample,	this.timerForBackTester, 
						crossoverRate, mutationRate, elitismRate ,
						populationSizeForGeneticOptimizer, generationNumberForGeneticOptimizer,
						seedForRandomGenerator);
			
//				new SelectTopEligiblesInSampleChooser( this.numberOfPortfolioPositions,
//			                                         numberOfBestTestingPositionsToBeReturned);
			
			return inSampleChooser;
		}
		#endregion inSampleChooser
		
		#region strategy
		protected override IStrategyForBacktester getStrategyForBacktester()
		{
			int numDaysBetweenEachOptimization = 60;
			int minNumOfEligiblesForValidOptimization = 10;
			
			IStrategyForBacktester strategyForBacktester
				= new DrivenByFVProviderStrategy(eligiblesSelector ,
				minNumOfEligiblesForValidOptimization, inSampleChooser ,
				numDaysForFundamentalAnalysis , numDaysForPortfolioVolatilityAnalysis ,
				benchmark , numDaysBetweenEachOptimization ,
				historicalMarketValueProviderForInSample ,
			  historicalMarketValueProviderForOutOfSample ,
			  this.portfolioType, this.stopLoss, this.percentageOfTheoreticalProfitForExit,
			  this.takeProfitLevelInAnyCase);
			
			return strategyForBacktester;
		}
		#endregion strategy
		
		#region backTester
		protected override EndOfDayStrategyBackTester getEndOfDayStrategyBackTester()
		{
			string backTestId = "DrivenByFairValueProviderStrategy";
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
			return "FairValuePrvStr";
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
				@"C:\Quant\QuantProject\b7_Scripts\TickerSelectionTesting\DrivenByFundamentals\DrivenByFairValueProvider\DrivenByFVProviderMain.cs";
			if( File.Exists(fullPathFileNameForMainAtHome) )
				returnValue = fullPathFileNameForMainAtHome;
			else
				returnValue = 
					@"C:\Utente\MarcoTributi\Vari\Vari\qP\QuantProject\b7_Scripts\TickerSelectionTesting\DrivenByFundamentals\DrivenByFairValueProvider\DrivenByFVProviderMain.cs";
			
			return returnValue;
		}
	}
}
