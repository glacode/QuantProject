/*
QuantProject - Quantitative Finance Library

OTCIntradayMain.cs
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
using System.Collections;
using System.Collections.Generic;
using System.IO;

using QuantProject.ADT;
using QuantProject.ADT.Statistics.Combinatorial;
using QuantProject.ADT.FileManaging;
using QuantProject.ADT.Timing;
using QuantProject.Business.DataProviders;
using QuantProject.Data.DataProviders.Bars.Caching;
using QuantProject.Business.Strategies;
using QuantProject.Business.Financial.Accounting.AccountProviding;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.InSample.InSampleFitnessDistributionEstimation;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.GenomeManagers;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;
using QuantProject.Presentation;
//using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.InSampleChoosers;
//using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.Decoding;
//using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.FitnessEvaluators;
using QuantProject.Scripts.TickerSelectionTesting.OTC.InSampleChoosers;
using QuantProject.Scripts.General;
using QuantProject.Scripts.General.Logging;
using QuantProject.Scripts.General.Reporting;
using QuantProject.Scripts.General.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.TickerSelectionTesting.OTC.InSampleChoosers.Genetic;
using QuantProject.Scripts.TickerSelectionTesting.OTC.InSampleChoosers.BruteForce;



namespace QuantProject.Scripts.TickerSelectionTesting.OTC.OTC_Intraday
{
	/// <summary>
	/// Entry point for the OTCIntradayMain. If any strategy
	/// parameter had to be changed, this is the place where it should
	/// be done
	/// </summary>
	public class OTCIntradayMain : BasicScriptForBacktesting
	{
		private int numberOfPortfolioPositions;
		private PortfolioType portfolioType;
		private int maxNumberOfEligiblesToBeChosen;
		private Benchmark benchmark;
		private DateTime firstDateTime;
		private DateTime lastDateTime;
		List<Time> dailyTimes;//intraday times for barCache and Timer
		private Time nearToOpeningTimeFrom;
    private Time nearToOpeningTimeTo;
    private Time nearToClosingTimeFrom;
    private Time nearToClosingTimeTo;
		private HistoricalMarketValueProvider historicalMarketValueProviderForInSample;
		private HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample;
		private HistoricalMarketValueProvider historicalMarketValueProviderForTheBackTester;
		//private int stepInMinutesForTimer;
		private Timer timerForBackTester;
		private int intervalFrameInSeconds;
		private GenomeManagerType genomeManagerType;
		
		#region main
		public OTCIntradayMain()
		{
			this.numberOfPortfolioPositions = 3;
//			this.benchmark = new Benchmark( "CCE" );
			this.portfolioType = PortfolioType.ShortAndLong;//filter for out of sample
			this.genomeManagerType = GenomeManagerType.ShortAndLong;//filter for the genetic chooser
			
			this.benchmark = new Benchmark( "ENI.MI" );
			this.firstDateTime = new DateTime( 2000 , 1 , 1 );
			this.lastDateTime = new DateTime( 2009 , 8, 15 );
			//this.stepInMinutesForTimer = 1;
			this.intervalFrameInSeconds = 60;
//			this.dailyTimes = Time.GetIntermediateTimes(new Time("09:30:00"),
//			                                 new Time("16:00:00"),
//			                                 this.stepInMinutesForTimer);
			this.dailyTimes = new List<Time>();
			this.dailyTimes.Add(new Time("09:30:00"));
			this.dailyTimes.Add(new Time("09:31:00"));
			this.dailyTimes.Add(new Time("09:32:00"));
			this.dailyTimes.Add(new Time("09:33:00"));
			this.dailyTimes.Add(new Time("09:34:00"));
			this.dailyTimes.Add(new Time("09:35:00"));
			this.dailyTimes.Add(new Time("15:55:00"));
			this.dailyTimes.Add(new Time("15:56:00"));
			this.dailyTimes.Add(new Time("15:57:00"));
			this.dailyTimes.Add(new Time("15:58:00"));
			this.dailyTimes.Add(new Time("15:59:00"));
			this.dailyTimes.Add(new Time("16:00:00"));
			
			this.nearToOpeningTimeFrom = new Time("09:30:00");
			this.nearToOpeningTimeTo = new Time("09:35:00");
			this.nearToClosingTimeFrom = new Time("15:55:00");
			this.nearToClosingTimeTo = new Time("16:00:00");
			
			this.historicalMarketValueProviderForInSample =
//				new HistoricalRawQuoteProvider();
			  new HistoricalAdjustedQuoteProvider();
			this.historicalMarketValueProviderForOutOfSample =
//				this.getHistoricalBarProvider();
//				this.historicalMarketValueProviderForInSample;
				new HistoricalRawQuoteProvider();
			this.historicalMarketValueProviderForTheBackTester =
				this.historicalMarketValueProviderForOutOfSample;
				//new HistoricalBarProvider(
					//new SimpleBarCache( intervalFrameInSeconds,
					                    //BarComponent.Open ) );
				//ricordarsi di togliere - mettere
				//commento nel gestore evento tempo
			this.timerForBackTester =
				new IndexBasedEndOfDayTimer( this.firstDateTime,
				                             this.lastDateTime,
				                             this.benchmark.Ticker);
//				new IndexBasedHistoricalTimer(this.benchmark.Ticker,
//				                              this.firstDateTime,
//				                              this.lastDateTime ,
//				                            	this.dailyTimes,
//				                              this.intervalFrameInSeconds);
		}
		#endregion main
		
		#region eligiblesSelector
		protected override IEligiblesSelector getEligiblesSelector()
		{
			this.maxNumberOfEligiblesToBeChosen = 50;
//			string tickersGroupId = "SP500";
			string tickersGroupId = "STOCKMI";
			bool temporizedGroup = true;
			int numDaysForAverageRawOpenPriceComputation = 10;
			double minPrice = 4;
			double maxPrice = 200;
			
//			int maxNumberOfMostLiquidTickersToBeChosen = 150;
//			int numDaysForVolatility = 10;
						
//			IEligiblesSelector eligiblesSelector =
//				new ByPriceMostLiquidQuotedAtEachDateTime(
//				tickersGroupId , temporizedGroup ,
//				maxNumberOfEligiblesToBeChosen ,
//			  numDaysForAverageRawOpenPriceComputation ,
//			 	minPrice , maxPrice, intervalFrameInSeconds ,
//			 	this.benchmark.Ticker );
//		
//			IEligiblesSelector eligiblesSelector =
//				new ByPriceMostLiquidLessVolatileOTCAlwaysQuoted(
//				tickersGroupId , temporizedGroup ,
//				maxNumberOfEligiblesToBeChosen ,
//				maxNumberOfMostLiquidTickersToBeChosen ,
//			  numDaysForAverageRawOpenPriceComputation ,
//				numDaysForVolatility ,
//			 	minPrice , maxPrice );
			IEligiblesSelector eligiblesSelector =
				new ByPriceMostLiquidAlwaysQuoted(
				tickersGroupId , temporizedGroup ,
				maxNumberOfEligiblesToBeChosen ,
			  numDaysForAverageRawOpenPriceComputation ,
			 	minPrice , maxPrice );
//			IEligiblesSelector eligiblesSelector =
//				new ByPriceLessVolatileOTCAlwaysQuoted(
//				tickersGroupId , temporizedGroup ,
//				maxNumberOfEligiblesToBeChosen ,
//			  numDaysForAverageRawOpenPriceComputation ,
//			 	minPrice , maxPrice );
//			eligiblesSelector = 
//				new DummyEligibleSelector();
//			
			return eligiblesSelector;
		}
		#endregion eligiblesSelector
		
		#region inSampleChooser
		protected override IInSampleChooser getInSampleChooser()
		{
//			Combination combinations;
//			if(this.genomeManagerType == GenomeManagerType.ShortAndLong ||
//			   this.genomeManagerType == GenomeManagerType.OnlyMixed)
//				combinations = new Combination(-this.maxNumberOfEligiblesToBeChosen, this.maxNumberOfEligiblesToBeChosen - 1, this.numberOfPortfolioPositions);
//			else
//				combinations = new Combination(0, this.maxNumberOfEligiblesToBeChosen - 1, this.numberOfPortfolioPositions);
//
//			int numberOfBestTestingPositionsToBeReturned = 
//				(int)combinations.TotalNumberOfCombinations;
			int numberOfBestTestingPositionsToBeReturned = 50;
			// parameters for the genetic optimizer
			double crossoverRate = 0.85;
			double mutationRate = 0.02;
			double elitismRate = 0.001;
			int populationSizeForGeneticOptimizer = 10000;
			int generationNumberForGeneticOptimizer = 30;
			int seedForRandomGenerator =
				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator;

			IDecoderForTestingPositions decoderForTestingPositions =
				new BasicDecoderForOTCPositions();
			IFitnessEvaluator fitnessEvaluator = 
//				new OTCFitnessEvaluator( new SharpeRatio() );
				new OTCCTOFitnessEvaluator( new SharpeRatio() );
//				new DummyRandomFitnessEvaluator();

//			IInSampleChooser inSampleChooser =
//				new OTCIntradayGeneticChooser(numberOfBestTestingPositionsToBeReturned, 
//						1 ,  balancedWeightsOnVolatilityBase,
//						minimumAbsoluteReturnValue , maximumAbsoluteReturnValue, this.benchmark.Ticker);
			
			ADT.ConstantsProvider.AmountOfVariableWeightToBeAssignedToTickers = 0.40;
			IInSampleChooser inSampleChooser = 
//				new OTCEndOfDayGeneticChooser(this.numberOfPortfolioPositions, numberOfBestTestingPositionsToBeReturned, 
//						benchmark, decoderForTestingPositions , 
//						this.genomeManagerType ,
//						fitnessEvaluator ,
//						historicalMarketValueProviderForInSample, crossoverRate, 
//					  mutationRate, elitismRate , populationSizeForGeneticOptimizer, 
//					  generationNumberForGeneticOptimizer, seedForRandomGenerator);
				new OTCEndOfDayGeneticChooserWithWeights(this.numberOfPortfolioPositions, numberOfBestTestingPositionsToBeReturned, 
						benchmark, 
						this.genomeManagerType ,
						fitnessEvaluator ,
						historicalMarketValueProviderForInSample, crossoverRate, 
					  mutationRate, elitismRate , populationSizeForGeneticOptimizer, 
					  generationNumberForGeneticOptimizer, seedForRandomGenerator);
			
//			IInSampleChooser inSampleChooser =
//				new OTCEndOfDayBruteForceChooser(this.portfolioType,
//				                                 this.numberOfPortfolioPositions,
//				                                 numberOfBestTestingPositionsToBeReturned,
//				                                 this.benchmark, decoderForTestingPositions , fitnessEvaluator,
//				                                 historicalMarketValueProviderForInSample);
//			//office
//			inSampleChooser =
//				new PVOChooserFromSavedBackTestLog(
//					@"C:\Utente\MarcoVarie\Vari\qP\LogArchive\2009_01_05_13_40_28_PVOIntraday_from_2006_01_01_to_2007_12_31_annlRtrn_17,84_maxDD_4,52\2009_01_05_13_40_28_PVOIntraday_from_2006_01_01_to_2007_12_31_annlRtrn_17,84_maxDD_4,52.qpL",
//				  numberOfBestTestingPositionsToBeReturned);
			//home
//			inSampleChooser =
//				new PVOChooserFromSavedBackTestLog(
//				@"C:\Utente\MarcoVarie\Vari\qP\LogArchive\2009_01_01_18_57_36_PVOIntraday_from_2006_01_01_to_2007_12_31_annlRtrn_17.11_maxDD_3.80\2009_01_01_18_57_36_PVOIntraday_from_2006_01_01_to_2007_12_31_annlRtrn_17.11_maxDD_3.80.qpL",
//				numberOfBestTestingPositionsToBeReturned);
			return inSampleChooser;
		}
		#endregion inSampleChooser
		
		#region strategy
		protected override IStrategyForBacktester getStrategyForBacktester()
		{
			//int inSampleDays = 90;
			int inSampleDays = 90;
			int numDaysBetweenEachOptimization = 5;
			int numDaysBeforeCurrentDateForRetrievingInSampleData = 1;
			int minNumOfEligiblesForValidOptimization = 10;
			//disabilitato il controllo della fitness
			double minimumNumberOfStdDevForSignificantFitness = 0.0;
			double maximumNumberOfStdDevForSignificantFitness = 0.0;
			int sampleLengthForFitnessDistributionEstimation = 150;
			//disabilitato il controllo della fitness
			
			IInSampleFitnessDistributionEstimator estimator =
				new BasicInSampleFitnessDistributionEstimator();
			GeneticChooser geneticChooserForEstimator = 
				new OTCEndOfDayGeneticChooser(this.numberOfPortfolioPositions, 50,
				    this.benchmark, new BasicDecoderForOTCPositions(),
						this.genomeManagerType ,
						new OTCCTOFitnessEvaluator( new SharpeRatio() ) ,
						historicalMarketValueProviderForInSample, 0.85, 
					  0.01, 0.001 , 100, 0, QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator);
			double stopLoss = 0.015;
			double takeProfit = 0.03;
			object[] daysForPlayingTheStrategy = 
				new object[5]{ DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
											DayOfWeek.Thursday, DayOfWeek.Friday};
			IStrategyForBacktester strategyForBacktester
//				 = new PVO_OTCStrategyLessCorrelated(eligiblesSelector ,inSampleChooser ,
//				inSampleDays , benchmark , numDaysBetweenEachOptimization ,
//				oversoldThreshold , overboughtThreshold , historicalQuoteProvider);
//			
				= new OTCIntradayStrategy(eligiblesSelector ,
				minNumOfEligiblesForValidOptimization, inSampleChooser ,
				inSampleDays , benchmark , numDaysBetweenEachOptimization ,
				numDaysBeforeCurrentDateForRetrievingInSampleData,
				historicalMarketValueProviderForInSample,
			  historicalMarketValueProviderForOutOfSample,
			  this.dailyTimes,
			  this.nearToOpeningTimeFrom,
			  this.nearToOpeningTimeTo,
			  this.nearToClosingTimeFrom,
			  this.nearToClosingTimeTo,
			  stopLoss , takeProfit, this.portfolioType, geneticChooserForEstimator, 
			  minimumNumberOfStdDevForSignificantFitness, 
			  maximumNumberOfStdDevForSignificantFitness, 
			  estimator,
			  sampleLengthForFitnessDistributionEstimation, daysForPlayingTheStrategy);
			
//			((OTCIntradayStrategy)strategyForBacktester).FindPositionsForToday(
//				new DateTime(2009,9,1), new DateTime(2009,8,28) );
			
			return strategyForBacktester;
		}
		#endregion strategy
		
		#region backTester
		protected override EndOfDayStrategyBackTester getEndOfDayStrategyBackTester()
		{
			string backTestId = "OTCIntradayStrategy";
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
				
		#region getHistoricalBarProvider
		
		#region getBarCache
		
		private IBarCache getBarCache()
		{
			IBarCache barCache = new DailyBarCache( this.intervalFrameInSeconds , dailyTimes );
			return barCache;
		}
		#endregion getBarCache
		
		private HistoricalBarProvider getHistoricalBarProvider()
		{
			IBarCache barCache = getBarCache();
			HistoricalBarProvider historicalBarProvider =
				new HistoricalBarProvider( barCache );
			return historicalBarProvider;
		}
		#endregion getHistoricalBarProvider
		
		protected override string getCustomSmallTextForFolderName()
		{
			return "OTCIntradayStrategy";
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
				@"C:\Quant\QuantProject\b7_Scripts\TickerSelectionTesting\OTC\OTC_Intraday\OTCIntradayMain.cs";
			if( File.Exists(fullPathFileNameForMainAtHome) )
				returnValue = fullPathFileNameForMainAtHome;
			else
				returnValue = 
					@"C:\Utente\MarcoVarie\Vari\qP\QuantProject\b7_Scripts\TickerSelectionTesting\OTC\OTC_Intraday\OTCIntradayMain.cs";
			
			return returnValue;
		}
	}
}
