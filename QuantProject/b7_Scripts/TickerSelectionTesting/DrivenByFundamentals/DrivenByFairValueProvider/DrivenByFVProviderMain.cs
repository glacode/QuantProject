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
using System.IO;
using QuantProject.ADT.Timing;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting.AccountProviding;
using QuantProject.Business.Financial.Fundamentals;
using QuantProject.Business.Financial.Fundamentals.FairValueProviders;
using QuantProject.Business.Financial.Fundamentals.FairValueProviders.LinearRegression;
using QuantProject.Business.Financial.Fundamentals.RatioProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.Optimizing.GenomeManagers;
using QuantProject.Business.Timing;
using QuantProject.Data.Selectors;
using QuantProject.Scripts.General;
using QuantProject.Scripts.TickerSelectionTesting.DrivenByFundamentals.DrivenByFairValueProvider.InSampleChoosers.Genetic;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.TickerSelectionTesting.DrivenByFundamentals.DrivenByFairValueProvider.InSampleChoosers;

namespace QuantProject.Scripts.TickerSelectionTesting.DrivenByFundamentals.DrivenByFairValueProvider
{
	/// <summary>
	/// Entry point for the DrivenByFairValueProviderStrategy. If any strategy
	/// parameter had to be changed, this is the place where it should
	/// be done
	/// </summary>
	[Serializable]
	public class DrivenByFVProviderMain : BasicScriptForBacktesting
	{
		private int numberOfPortfolioPositions;
		private double stopLoss;
		private double percentageOfTheoreticalProfitForExit;
		private double takeProfitLevelInAnyCase;
		private PortfolioType portfolioType;
		private int maxNumberOfEligiblesToBeChosen;
		private IFairValueProvider fairValueProvider;
		private int numDaysForPortfolioVolatilityAnalysis;
		private int numDaysForFundamentalDataAvailability;
		private string tickersGroupId;
		private Benchmark benchmark;
		private DateTime firstDateTime;
		private DateTime lastDateTime;
		private HistoricalMarketValueProvider historicalMarketValueProviderForInSample;
		private HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample;
		private HistoricalMarketValueProvider historicalMarketValueProviderForTheBackTester;
		private Timer timerForBackTester;
//		private GenomeManagerType genomeManagerType;
		private ITickerSelectorByDate tickerSelectorByDate;
		private bool temporizedGroup;
//		private int numDayForAveragePriceComputation;
		private string hedgingTicker;
		private double hedgingTickerWeight;
		
		#region main
		public DrivenByFVProviderMain()
		{
			this.numberOfPortfolioPositions = 6;
			this.hedgingTicker = "SH";
//			this.hedgingTicker ="MYY";
//			this.hedgingTicker = null;
			this.hedgingTickerWeight = 0.5;
			this.benchmark = new Benchmark( "^GSPC" );
//			this.benchmark = new Benchmark( "FTSEMIB.MI" );
			this.tickersGroupId = "SP500";
//			this.tickersGroupId = "ticUSFin";
//			this.tickersGroupId = "STOCKMI";
			this.temporizedGroup = true;
			this.numDaysForPortfolioVolatilityAnalysis = 90;
			this.maxNumberOfEligiblesToBeChosen = 500;
//			int numOfTopRowsToDelete = 500;
//			this.numDayForAveragePriceComputation = 10;
//			double minPriceForTickersToBeSelected = 0.5;
//			string benchmarkForCorrelation = "C"; //citigroup
			int numOfDaysForCorrelation = this.numDaysForPortfolioVolatilityAnalysis;
			this.tickerSelectorByDate =
//				new SelectorByGroupLiquidityAndPrice(this.tickersGroupId, temporizedGroup,
//				                                     false, this.numDaysForPortfolioVolatilityAnalysis,
//				                                     this.maxNumberOfEligiblesToBeChosen,
//				                                     numOfTopRowsToDelete,
//																						 numDayForAveragePriceComputation,
//																						 minPriceForTickersToBeSelected);
				new SelectorByGroup(this.tickersGroupId, temporizedGroup);
//			new SelectorByCloseToCloseCorrelationToBenchmark(
//					new SelectorByGroupLiquidityAndPrice(this.tickersGroupId, temporizedGroup,
//                                              false, numOfDaysForCorrelation,
//                                              3000, numDayForAveragePriceComputation,
//                                              minPriceForTickersToBeSelected),
//					new SelectorByGroup(this.tickersGroupId, temporizedGroup),
//					numOfDaysForCorrelation,
//					benchmarkForCorrelation, false,
//					this.maxNumberOfEligiblesToBeChosen, false);
			
			this.portfolioType = PortfolioType.ShortAndLong;//filter for out of sample
//			this.genomeManagerType = GenomeManagerType.ShortAndLong;//filter for the genetic chooser
//			this.benchmark = new Benchmark( "ENI.MI" );
			this.firstDateTime = new DateTime( 2003 , 1 , 1 );
			this.lastDateTime = new DateTime( 2009 , 12, 31 );
			
			this.historicalMarketValueProviderForInSample =
//				new HistoricalRawQuoteProvider();
			  new HistoricalAdjustedQuoteProvider();
			this.historicalMarketValueProviderForOutOfSample =
				this.historicalMarketValueProviderForInSample;
			this.historicalMarketValueProviderForTheBackTester =
				this.historicalMarketValueProviderForOutOfSample;
				//ricordarsi di togliere - mettere
				//commento nel gestore evento tempo
			this.numDaysForFundamentalDataAvailability = 60;
//			double optimalDebtEquityRatioLevel = 0.1;
//			int maxNumOfGrowthRatesToTakeIntoAccount = 4;
//			IGrowthRateProvider growthProvider =
//				new AverageAndDebtAdjustedGrowthRateProvider(numDaysForFundamentalDataAvailability,
//				                                             maxNumOfGrowthRatesToTakeIntoAccount,
//				                                             optimalDebtEquityRatioLevel);
//				new LastAvailableGrowthRateProvider(numDaysForFundamentalDataAvailability);
			FundamentalDataProvider[] fundamentalDataProviders =
				new FundamentalDataProvider[1]{
					new BookValueProvider(numDaysForFundamentalDataAvailability)
				};
				
			ILinearRegressionValuesProvider linearRegressionValuesProvider =
				new BasicLinearRegressionValuesProvider(this.tickerSelectorByDate,
				                                        fundamentalDataProviders,
				                                        new DayOfMonth(12, 31) );
			
			IIndipendentValuesProvider indipendentValuesProvider = 
				new BasicIndipendentValuesProvider(fundamentalDataProviders);
//			IRatioProvider_PE PEProvider =
//				new LastAvailablePEProvider(this.historicalMarketValueProviderForInSample,
//				                            numDaysForFundamentalDataAvailability);
//			double fairPEGRatioLevel = 1.0;
			this.fairValueProvider = 
//				new PEGRatioFairValueProvider(fairPEGRatioLevel,PEProvider,
//				                              growthProvider,this.historicalMarketValueProviderForInSample);
				new LinearRegressionFairValueProvider(linearRegressionValuesProvider,
				                                      indipendentValuesProvider);
//			this.genomeManagerType = GenomeManagerType.ShortAndLong;

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
//			double minimumIncome = 10000000.0;//10 mln
//			int numOfMinIncomeInARow = 3;
//			double minimumRelativeDifferenceBetweenFairAndAverageMarketPrice = 0.01;
			int numDaysForAveragePriceComputation = 5;
			int firstPercentileOfMostDiscountedToExclude = 0;
			int firstPercentileOfMostExpensiveToExclude = 100;
			//the strategy is only long: over-valued tickers are discarded
			
			IEligiblesSelector eligiblesSelector =
					new ByRelativeDifferenceBetweenPriceAndFairValue( this.fairValueProvider,
				             this.tickerSelectorByDate , 
										 maxNumberOfEligiblesToBeChosen ,
										 firstPercentileOfMostDiscountedToExclude ,
										 firstPercentileOfMostExpensiveToExclude ,
										 numDaysForAveragePriceComputation );                                    
					
//					new ByPriceMostLiquidAlwaysQuoted(tickersGroupId,
//									temporizedGroup, maxNumberOfEligiblesToBeChosen,
//									numDaysForAveragePriceComputation, 1.0, 5000.0);
			
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
//			double crossoverRate = 0.85;
//			double mutationRate = 0.02;
//			double elitismRate = 0.001;
//			int populationSizeForGeneticOptimizer = 1000;
//			int generationNumberForGeneticOptimizer = 10;
//			int seedForRandomGenerator =
//				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator;
			
//			BuyAndHoldFitnessEvaluator fitnessEvaluator = 
//				new BuyAndHoldFitnessEvaluator( new Variance() );
//				new BuyAndHoldFitnessEvaluator( new SharpeRatio() );
			
//			bool mixPastReturnsEvaluationWithFundamentalEvaluation = true;
			
			BasicDecoderForGeneticallyOptimizableTestingPositions decoderForFVProvider = 
				new DecoderForFVProviderStrategy();
			
			IInSampleChooser inSampleChooser = 
//				new DrivenByFVProviderInSampleChooser(this.numberOfPortfolioPositions,
//				                                      numberOfBestTestingPositionsToBeReturned,
//						benchmark, decoderForFVProvider, this.genomeManagerType ,
//						fitnessEvaluator , mixPastReturnsEvaluationWithFundamentalEvaluation,
//						historicalMarketValueProviderForInSample,	this.timerForBackTester, 
//						crossoverRate, mutationRate, elitismRate ,
//						populationSizeForGeneticOptimizer, generationNumberForGeneticOptimizer,
//						seedForRandomGenerator);
			
				new SelectTopBottomEligiblesWithSignInSampleChooser( this.numberOfPortfolioPositions,
			                                         numberOfBestTestingPositionsToBeReturned,
			                                         this.historicalMarketValueProviderForInSample,
			                                         this.timerForBackTester);
			
			
			return inSampleChooser;
		}
		#endregion inSampleChooser
		
		#region strategy
		protected override IStrategyForBacktester getStrategyForBacktester()
		{
			int numDaysBetweenEachOptimization = 180;
			int minNumOfEligiblesForValidOptimization = 20;
			
			IStrategyForBacktester strategyForBacktester
				= new DrivenByFVProviderStrategy(eligiblesSelector ,
				minNumOfEligiblesForValidOptimization, inSampleChooser ,
				this.numDaysForFundamentalDataAvailability, numDaysForPortfolioVolatilityAnalysis ,
				benchmark , numDaysBetweenEachOptimization ,
				historicalMarketValueProviderForInSample ,
			  historicalMarketValueProviderForOutOfSample ,
			  this.portfolioType, this.stopLoss, this.percentageOfTheoreticalProfitForExit,
			  this.takeProfitLevelInAnyCase, this.hedgingTicker, this.hedgingTickerWeight);
			
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
