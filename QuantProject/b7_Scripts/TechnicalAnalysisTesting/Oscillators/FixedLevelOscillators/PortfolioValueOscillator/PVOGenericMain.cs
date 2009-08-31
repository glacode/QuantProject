/*
QuantProject - Quantitative Finance Library

PVOGenericMain.cs
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
using QuantProject.ADT.FileManaging;
using QuantProject.ADT.Timing;
using QuantProject.Business.DataProviders;
using QuantProject.Data.DataProviders.Bars.Caching;
using QuantProject.Business.Strategies;
using QuantProject.Business.Financial.Accounting.AccountProviding;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;
using QuantProject.Business.Timing.TimingManagement;
using QuantProject.Presentation;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.InSampleChoosers;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.Decoding;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.FitnessEvaluators;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.EntryConditions;
using QuantProject.Scripts.General;
using QuantProject.Scripts.General.Logging;
using QuantProject.Scripts.General.Reporting;


namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator
{
	/// <summary>
	/// Entry point for the PVOGenericMain. If any strategy
	/// parameter had to be changed, this is the place where it should
	/// be done
	/// </summary>
	public class PVOGenericMain : BasicScriptForBacktesting
	{
		private Benchmark benchmark;
		private HistoricalMarketValueProvider historicalMarketValueProviderForInSample;
		private HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample;
		private HistoricalMarketValueProvider historicalMarketValueProviderForTheBackTester;
		private double inefficiencyLengthInMarketIntervals;
		private int numberOfPreviousEfficientPeriods;
		private int numberOfDaysForPriceRatioAnalysis;
		private double numberOfStdDeviationForSignificantPriceRatioMovements;
		private double maxOpeningLengthInMarketIntervals;
		private double marketIntervalsFromLastInefficiencyToWaitBeforeOpening;
		private double marketIntervalsFromLastLossOrProfitToWaitBeforeClosing;
		private int stepInMinutesForTimer;
		private Timer timerForBackTester;
		private DateTime firstDateTime;
		private DateTime lastDateTime;
		private int intervalFrameInSeconds;
		private MarketIntervalsManager marketIntervalsManagerForOutOfSample;
		
		public PVOGenericMain()
		{
			this.inefficiencyLengthInMarketIntervals = 30;
			this.numberOfPreviousEfficientPeriods = 1;
			
			this.numberOfDaysForPriceRatioAnalysis = 90;
			this.numberOfStdDeviationForSignificantPriceRatioMovements = 0.0;
			
			this.maxOpeningLengthInMarketIntervals = 1800;
			this.marketIntervalsFromLastInefficiencyToWaitBeforeOpening = 1;
			this.marketIntervalsFromLastLossOrProfitToWaitBeforeClosing = 0;
			this.stepInMinutesForTimer = 1;
			this.intervalFrameInSeconds = 60;
			
			this.firstDateTime = new DateTime( 2006 , 1, 1 );
			this.lastDateTime = new DateTime( 2006 , 12, 31 );
			
			this.benchmark = new Benchmark( "CCE" );
			
			this.marketIntervalsManagerForOutOfSample = 
				new MarketMinutesManager(this.benchmark, this.firstDateTime,
                                 this.lastDateTime);
//				  new MarketDaysManager(this.benchmark, this.firstDateTime,
//				                      this.lastDateTime, new Time("16:00:00"));
			
			this.historicalMarketValueProviderForOutOfSample =
				this.getHistoricalBarProvider();
//					new HistoricalAdjustedQuoteProvider();
			this.historicalMarketValueProviderForInSample =
//				this.historicalMarketValueProviderForOutOfSample;
				new HistoricalAdjustedQuoteProvider();
//				new HistoricalBarInterpolatingProvider( this.getBarCache() );
						
			this.historicalMarketValueProviderForTheBackTester =
//				new HistoricalBarProvider(
//					new SimpleBarCache( intervalFrameInSeconds,
//					                    BarComponent.Open ) );
//				new HistoricalBarProvider(
//					new DailyBarCache( intervalFrameInSeconds,
//					                  this.getDailyTimes() ) );
				  this.historicalMarketValueProviderForOutOfSample;
					
			this.timerForBackTester =
				new IndexBasedHistoricalTimer(this.benchmark.Ticker,
				                              this.firstDateTime,
				                              this.lastDateTime ,
				                            	this.getDailyTimes(),
				                              this.intervalFrameInSeconds);
			
//				new IndexBasedEndOfDayTimer( this.firstDateTime,
//				                             this.lastDateTime,
//				                             this.benchmark.Ticker );
		}

		protected override IEligiblesSelector getEligiblesSelector()
		{
			
			int maxNumberOfEligiblesToBeChosen = 100;
//			int numOfTopRowsToDeleteFromSelectorByLiquidity = 0;
			
			string tickersGroupId = "SP500";
			
			bool temporizedGroup = true;
			int numDaysForAverageRawOpenPriceComputation = 10;
			double minPrice = 15;
			double maxPrice = 1500;
			
//			int maxNumberOfMostLiquidTickersToBeChosen = 150;
//			int numDaysForVolatility = 10;
//			int minimumNumberOfMinutelyBarsForEachDayForInSample = 100;
						
//			IEligiblesSelector eligiblesSelector =
//				new ByPriceMostLiquidQuotedAtEachDateTime(
//				tickersGroupId , temporizedGroup ,
//				maxNumberOfEligiblesToBeChosen ,
//			  numDaysForAverageRawOpenPriceComputation ,
//			 	minPrice , maxPrice, intervalFrameInSeconds ,
//			 	this.benchmark.Ticker );
//		LAST	GOOD
//			IEligiblesSelector eligiblesSelector =
//				new ByPriceMostLiquidLessVolatileOTCAlwaysQuoted(
//				tickersGroupId , temporizedGroup ,
//				maxNumberOfEligiblesToBeChosen ,
//				maxNumberOfMostLiquidTickersToBeChosen ,
//			  numDaysForAverageRawOpenPriceComputation ,
//				numDaysForVolatility ,
//			 	minPrice , maxPrice );
			
//			IEligiblesSelector eligiblesSelector =
//				new ByPriceMostLiquidAlwaysIntradayQuoted(
//				tickersGroupId , this.benchmark, temporizedGroup ,
//				maxNumberOfEligiblesToBeChosen ,
//				numOfTopRowsToDeleteFromSelectorByLiquidity,
//				numDaysForAverageRawOpenPriceComputation ,
//				minPrice , maxPrice,
//				minimumNumberOfMinutelyBarsForEachDayForInSample  );
			
//			new ByPriceLessLiquidAlwaysIntradayQuoted(
//				tickersGroupId , this.benchmark, temporizedGroup ,
//				maxNumberOfEligiblesToBeChosen ,
//				numOfTopRowsToDeleteFromSelectorByLiquidity,
//				numDaysForAverageRawOpenPriceComputation ,
//				minPrice , maxPrice,
//				minimumNumberOfMinutelyBarsForEachDayForInSample  );
			
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

		protected override IInSampleChooser getInSampleChooser()
		{
			// parameters for the genetic optimizer
//			double crossoverRate = 0.85;
//			double mutationRate = 0.02;
//			double elitismRate = 0.001;
//			int populationSizeForGeneticOptimizer = 3000;
//			int generationNumberForGeneticOptimizer = 4;
//			int seedForRandomGenerator =
//				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator;

//			IDecoderForTestingPositions decoderForWeightedPositions =
//				new DecoderForPairsTradingTestingPositionsWithBalancedWeights();

			double maxCorrelationAllowed = 0.96;
			int numberOfBestTestingPositionsToBeReturned = 50;
			bool balancedWeightsOnVolatilityBase = true;
			float minimumAbsoluteReturnValue = 0.00005f;
			float maximumAbsoluteReturnValue = 100000f;
//			int returnIntervalLengthInMinutesForCorrelationProvider = 120;
			//correlation is computed only for returns
			//between minimum and maximum
//			IInSampleChooser inSampleChooser = 
//				new PVO_OTCCorrelationChooser(numberOfBestTestingPositionsToBeReturned, 
//						maxCorrelationAllowed , balancedWeightsOnVolatilityBase,
//					  minimumAbsoluteReturnValue , maximumAbsoluteReturnValue, this.benchmark.Ticker);
//			IInSampleChooser inSampleChooser = 
//				new PVO_OTCCTOCorrelationChooser(numberOfBestTestingPositionsToBeReturned, 
//						maxCorrelationAllowed , balancedWeightsOnVolatilityBase,
//					  minimumAbsoluteReturnValue , maximumAbsoluteReturnValue, this.benchmark.Ticker);
			IInSampleChooser inSampleChooser = 
				new PVO_CTCCorrelationChooser(numberOfBestTestingPositionsToBeReturned, 
						2 , maxCorrelationAllowed , balancedWeightsOnVolatilityBase,
						minimumAbsoluteReturnValue , maximumAbsoluteReturnValue, this.benchmark.Ticker);
//					new PVOIntradayCorrelationChooser(numberOfBestTestingPositionsToBeReturned, 
//						returnIntervalLengthInMinutesForCorrelationProvider ,
//						maxCorrelationAllowed , balancedWeightsOnVolatilityBase,
//						minimumAbsoluteReturnValue , maximumAbsoluteReturnValue, this.benchmark.Ticker);

//			//office
//			inSampleChooser =
//				new PVOChooserFromSavedBackTestLog(
//					@"C:\Utente\MarcoVarie\Vari\qP\LogArchive\2009_01_05_13_40_28_PVOIntraday_from_2006_01_01_to_2007_12_31_annlRtrn_17,84_maxDD_4,52\2009_01_05_13_40_28_PVOIntraday_from_2006_01_01_to_2007_12_31_annlRtrn_17,84_maxDD_4,52.qpL",
//				  numberOfBestTestingPositionsToBeReturned);
			//home
//			inSampleChooser =
//				new PVOChooserFromSavedBackTestLog(
//				@"C:\Utente\MarcoVarie\Vari\qP\LogArchive\2009_03_31_0_34_14_PVOIntraday_from_2006_03_01_to_2007_03_01_annlRtrn_35.76_maxDD_2.34\2009_03_31_0_34_14_PVOIntraday_from_2006_03_01_to_2007_03_01_annlRtrn_35.76_maxDD_2.34.qpL",
//				numberOfBestTestingPositionsToBeReturned);
			return inSampleChooser;
		}

		protected override IStrategyForBacktester getStrategyForBacktester()
		{
			//int inSampleDays = 90;
			int inSampleDays = 180;
			int numDaysBetweenEachOptimization = 10;
			int minNumOfEligiblesForValidOptimization = 20;
			double oversoldThreshold = 0.008;//0.006
			double overboughtThreshold = 0.008;//0.006
			double oversoldThresholdMAX = 0.03;
			double overboughtThresholdMAX = 0.03;
			double stopLoss = 0.005;
			double takeProfit = 0.005;
			double leverage = 1.0;
			bool openOnlyLongPositions = false;
		  List<IEntryCondition> entryConditions = new List<IEntryCondition>();
		  IEntryCondition previousPeriodEfficientEntryCondition = 
//		  	new AlwaysTrueEntryCondition();
//		  	new PriceRatioEntryCondition(45, this.historicalQuoteProvider, 1.0);
		  	new PreviousPeriodsWereEfficientEntryCondition(this.numberOfPreviousEfficientPeriods,
		  	                                               this.historicalMarketValueProviderForOutOfSample,
		  	                                               this.inefficiencyLengthInMarketIntervals, this.marketIntervalsManagerForOutOfSample);
		  IEntryCondition priceRatioEntryCondition = 
		  	new PriceRatioEntryCondition(this.numberOfDaysForPriceRatioAnalysis, 
		  	    this.historicalMarketValueProviderForOutOfSample, 1.5);
		  
		  IEntryCondition inefficiencyMovingBackEntryCondition =
		  	new InefficiencyMovingBackEntryCondition(0.2,
		  	     this.historicalMarketValueProviderForOutOfSample,
		  	     this.benchmark);
		  
//		  entryConditions.Add(previousPeriodEfficientEntryCondition);
//		  entryConditions.Add(priceRatioEntryCondition);
		  entryConditions.Add(inefficiencyMovingBackEntryCondition);
		  bool allEntryConditionsHaveToBeTrue = true;
		  
			IStrategyForBacktester strategyForBacktester
//				 = new PVO_OTCStrategyLessCorrelated(eligiblesSelector ,inSampleChooser ,
//				inSampleDays , benchmark , numDaysBetweenEachOptimization ,
//				oversoldThreshold , overboughtThreshold , historicalQuoteProvider);
//			
				= new PVOGenericStrategy(eligiblesSelector ,
				minNumOfEligiblesForValidOptimization, inSampleChooser ,
				inSampleDays , benchmark , numDaysBetweenEachOptimization ,
				oversoldThreshold , overboughtThreshold ,
				oversoldThresholdMAX , overboughtThresholdMAX ,
				historicalMarketValueProviderForInSample,
			  historicalMarketValueProviderForOutOfSample,
			  inefficiencyLengthInMarketIntervals ,
			  numberOfPreviousEfficientPeriods,
			  numberOfDaysForPriceRatioAnalysis, 
			  numberOfStdDeviationForSignificantPriceRatioMovements, 
			  marketIntervalsFromLastInefficiencyToWaitBeforeOpening,
			  marketIntervalsFromLastLossOrProfitToWaitBeforeClosing,
			  maxOpeningLengthInMarketIntervals, this.getDailyTimes(),
			  stopLoss , takeProfit , leverage,
			  openOnlyLongPositions, entryConditions, allEntryConditionsHaveToBeTrue,
			  this.marketIntervalsManagerForOutOfSample);
			return strategyForBacktester;
		}
		#region getHistoricalBarProvider
		
		#region getBarCache
		private List<Time> getDailyTimes()
		{
			return Time.GetIntermediateTimes(new Time("09:45:00"),
			                                 new Time("16:00:00"),
			                                 this.stepInMinutesForTimer);
		}
		private IBarCache getBarCache()
		{
			List<Time> dailyTimes = this.getDailyTimes();
			IBarCache barCache = new DailyBarCache( this.intervalFrameInSeconds , dailyTimes );
			return barCache;
		}
		#endregion getBarCache
		
		private HistoricalMarketValueProvider getHistoricalBarProvider()
		{
			IBarCache barCache = getBarCache();
			HistoricalMarketValueProviderWithQuoteBackupOnClose historicalBarProvider;
			HistoricalAdjustedQuoteProvider histAdjQuoteProvider = 
				new HistoricalAdjustedQuoteProvider();
			HistoricalAdjustedBarProvider historicalAdjustedBarProvider =
				new HistoricalAdjustedBarProvider( new HistoricalBarProvider( barCache ),
				                                   new HistoricalRawQuoteProvider(),
				                                   histAdjQuoteProvider );
			historicalBarProvider = 
				new HistoricalMarketValueProviderWithQuoteBackupOnClose(historicalAdjustedBarProvider,
				                                                        histAdjQuoteProvider);
			return historicalBarProvider;
		}
		#endregion getHistoricalBarProvider
		
		protected override EndOfDayStrategyBackTester getEndOfDayStrategyBackTester()
		{
			string backTestId = "PVOGeneric";
			IAccountProvider accountProvider;
			accountProvider =
//				new InteractiveBrokerAccountProvider();
					new SimpleAccountProvider();
			double cashToStart = 25000;
			double maxRunningHours = 24;
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
				backTestId , this.timerForBackTester, 
				this.strategyForBacktester ,
				this.historicalMarketValueProviderForTheBackTester , accountProvider ,
				firstDateTime ,	lastDateTime ,
				this.benchmark , cashToStart , maxRunningHours );
			return endOfDayStrategyBackTester;
		}
		
		protected override string getCustomSmallTextForFolderName()
		{
			return "PVOGeneric";
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
				@"C:\Quant\QuantProject\b7_Scripts\TechnicalAnalysisTesting\Oscillators\FixedLevelOscillators\PortfolioValueOscillator\PVOGenericMain.cs";
			if( File.Exists(fullPathFileNameForMainAtHome) )
				returnValue = fullPathFileNameForMainAtHome;
			else
				returnValue = 
					@"C:\Utente\MarcoVarie\Vari\qP\QuantProject\b7_Scripts\TechnicalAnalysisTesting\Oscillators\FixedLevelOscillators\PortfolioValueOscillator\PVOGenericMain.cs";
			
			return returnValue;
		}
	}
}
