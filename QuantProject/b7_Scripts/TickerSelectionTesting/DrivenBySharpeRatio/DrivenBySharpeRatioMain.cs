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
using QuantProject.Business.DataProviders.VirtualQuotesProviding;
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
using QuantProject.Scripts.TickerSelectionTesting.DrivenBySharpeRatio.InSampleChoosers.BruteForce;
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
		private int numDaysBetweenEachOptimization;
		private HistoricalMarketValueProvider historicalMarketValueProviderForInSample;
		private HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample;
		private HistoricalMarketValueProvider historicalMarketValueProviderForTheBackTester;
		private Timer timerForBackTester;
		private GenomeManagerType genomeManagerType;
		private string hedgingTicker;
		
		#region main
		public DrivenBySharpeRatioMain()
		{
			this.numberOfPortfolioPositions = 6;
//			this.benchmark = new Benchmark( "FTSEMIB.MI" );
			this.benchmark = new Benchmark( "^GSPC" );
//			this.hedgingTicker = "SH(FTSEMIB)";
			this.hedgingTicker = null;
			this.portfolioType = PortfolioType.ShortAndLong;//filter for out of sample
			this.genomeManagerType = GenomeManagerType.ShortAndLong;//filter for the genetic chooser
//			this.benchmark = new Benchmark( "ENI.MI" );
			this.firstDateTime = new DateTime( 2003 , 1 , 1 );
			this.lastDateTime =  new DateTime( 2009 , 12 , 31 );
			this.numDaysBetweenEachOptimization = 180;
			
			HistoricalAdjustedQuoteProvider historicalAdjustedQuoteProvider =
				new HistoricalAdjustedQuoteProvider();
			List<DerivedVirtualTicker> virtualTickerList = 
				new List<DerivedVirtualTicker>();
			DerivedVirtualTicker virtualTicker1 = 
				new DerivedVirtualTicker(this.hedgingTicker, this.benchmark.Ticker, 100);
			virtualTickerList.Add(virtualTicker1);
			ShortVirtualQuoteProvider shortVirtualQuoteProvider =
//				new ShortVirtualQuoteProvider(virtualTickerList, historicalAdjustedQuoteProvider,
//				                              2 * this.numDaysBetweenEachOptimization);
				new ShortVirtualQuoteProvider(virtualTickerList, historicalAdjustedQuoteProvider,
				                              new DateTime(2003,6,1,16,0,0) );
			this.historicalMarketValueProviderForInSample =
//				new HistoricalRawQuoteProvider();
			  new HistoricalAdjustedQuoteProvider();
//				new VirtualAndHistoricalAdjustedQuoteProvider(shortVirtualQuoteProvider);
			
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
			this.maxNumberOfEligiblesToBeChosen = 1000;
			string tickersGroupId = "ticUSFin";
//			string tickersGroupId = "USFunds";
//			string tickersGroupId = "SP500";
//			string tickersGroupId = "BLUECHIX";
//			string tickersGroupId = "ETFMI";
			
			bool temporizedGroup = false;//Attenzione!
			// by most discounted prices parameters
			int numDaysForFundamentalDataAvailability = 60;
//			int numDaysForFundamentalAnalysis = 365;
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
			IFairValueProvider fairValueProvider = 
				new PEGRatioFairValueProvider(fairPEGRatioLevel,PEProvider,
				                              growthProvider,this.historicalMarketValueProviderForInSample);
//			double minimumIncome = 1000000;
//			int numOfMinIncomeInARow = 2;
//			double minimumRelativeDifferenceBetweenFairAndAverageMarketPrice = 0.05;
//			int numDaysForAveragePriceComputation = 90;
			
			//end of by most discounted prices parameters
			
			IEligiblesSelector eligiblesSelector =
//				new ByGroup( tickersGroupId , temporizedGroup);
				
//				new ByLiquidity ( tickersGroupId , temporizedGroup ,
//				maxNumberOfEligiblesToBeChosen);
				
			    new ByPriceMostLiquidAlwaysQuoted(tickersGroupId,
									temporizedGroup, maxNumberOfEligiblesToBeChosen,
									6, 1.0, 5000.0);
					
//					new ByPriceLiquidityLowestPEQuotedAtAGivenPercentage(				
//							tickersGroupId , temporizedGroup ,
//							maxNumberOfEligiblesToBeChosen , 
//							maxNumberOfEligiblesToBeChosen * 2,
//							numDaysForAveragePriceComputation,
//							1, 1000, 10, 50, 0.9);
//					new ByMostDiscountedPrices( fairValueProvider ,
//							tickersGroupId , temporizedGroup ,
//							maxNumberOfEligiblesToBeChosen , numDaysForFundamentalAnalysis,
//							numDaysForFundamentalDataAvailability,
//							minimumIncome, numOfMinIncomeInARow,
//							minimumRelativeDifferenceBetweenFairAndAverageMarketPrice,
//							numDaysForAveragePriceComputation);

//			eligiblesSelector =
//				new DummyEligibleSelector();
//			
			return eligiblesSelector;
		}
		#endregion eligiblesSelector
		
		#region inSampleChooser
		protected override IInSampleChooser getInSampleChooser()
		{
			int numberOfBestTestingPositionsToBeReturned = 10;
			// parameters for the genetic optimizer
			double crossoverRate = 0.99;
			double mutationRate = 0.1;
			double elitismRate = 0.0;
			int populationSizeForGeneticOptimizer = 20000;
			int generationNumberForGeneticOptimizer = 35;
			int seedForRandomGenerator =
				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator;
			bool keepOnRunningUntilConvergence = false;
			double minConvergenceRate = 0.50;
			
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
						seedForRandomGenerator, keepOnRunningUntilConvergence,
						minConvergenceRate);
//					new DrivenBySharpeRatioBruteForceChooser(PortfolioType.OnlyLong,
//									this.numberOfPortfolioPositions, numberOfBestTestingPositionsToBeReturned,
//									this.benchmark, basicGenOptDecoder, fitnessEvaluator, 
//									this.historicalMarketValueProviderForInSample);
			
			return inSampleChooser;
		}
		#endregion inSampleChooser
		
		#region strategy
		protected override IStrategyForBacktester getStrategyForBacktester()
		{
//			ConstantsProvider.AmountOfVariableWeightToBeAssignedToTickers = 0.5;
			int minNumOfEligiblesForValidOptimization = 10;
			int popSizeForGeneticHedgingOptimization = 10;
			int genNumForGeneticHedgingOptimization = 1;
			double weightForHedgingTicker = 0.40; //set 0.0 if you want
			//this weight be set by the genetic optimizer
			IFitnessEvaluator fitnessEvaluatorForGeneticHedgingOptimization =
				new BuyAndHoldFitnessEvaluator( new SharpeRatio() );
			
			IStrategyForBacktester strategyForBacktester
				= new DrivenBySharpeRatioStrategy(eligiblesSelector ,
				minNumOfEligiblesForValidOptimization, inSampleChooser ,
				numDaysForInSample ,
				benchmark , this.hedgingTicker, weightForHedgingTicker, numDaysBetweenEachOptimization ,
				historicalMarketValueProviderForInSample ,
			  historicalMarketValueProviderForOutOfSample ,
			  popSizeForGeneticHedgingOptimization,
			  genNumForGeneticHedgingOptimization,
			  fitnessEvaluatorForGeneticHedgingOptimization,
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
