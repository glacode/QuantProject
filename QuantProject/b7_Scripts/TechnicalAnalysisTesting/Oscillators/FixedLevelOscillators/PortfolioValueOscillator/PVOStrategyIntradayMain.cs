/*
QuantProject - Quantitative Finance Library

PVOStrategyIntradayMain.cs
Copyright (C) 2008
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
using QuantProject.Presentation;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.InSampleChoosers;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.Decoding;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.FitnessEvaluators;
using QuantProject.Scripts.General;
using QuantProject.Scripts.General.Logging;
using QuantProject.Scripts.General.Reporting;


namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator
{
	/// <summary>
	/// Entry point for the PVOStrategyIntradayMain. If any strategy
	/// parameter had to be changed, this is the place where it should
	/// be done
	/// </summary>
	public class PVOStrategyIntradayMain : BasicScriptForBacktesting
	{
		private Benchmark benchmark;
		private HistoricalMarketValueProvider historicalMarketValueProviderForInSample;
		private HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample;
		private HistoricalMarketValueProvider historicalMarketValueProviderForTheBackTester;
		private double inefficiencyLengthInMinutes;
		private double maxOpeningLengthInMinutes;
		private double minutesFromLastInefficiencyToWaitBeforeOpening;
		private double minutesFromLastLossOrProfitToWaitBeforeClosing;
		private int stepInMinutesForTimer;
		private Timer timerForBackTester;
		private DateTime firstDateTime;
		private DateTime lastDateTime;
		private int intervalFrameInSeconds;
		
		public PVOStrategyIntradayMain()
		{
			this.inefficiencyLengthInMinutes = 30;
			this.maxOpeningLengthInMinutes = 240;
			this.minutesFromLastInefficiencyToWaitBeforeOpening = 1;
			this.minutesFromLastLossOrProfitToWaitBeforeClosing = 0;
			this.stepInMinutesForTimer = 1;
			this.intervalFrameInSeconds = 60;
			
			this.firstDateTime = new DateTime( 2006 , 2 , 1 );
			this.lastDateTime = new DateTime( 2006 , 12, 31 );
			
			this.benchmark = new Benchmark( "CCE" );
			
			this.historicalMarketValueProviderForOutOfSample =
				this.getHistoricalBarProvider();
			
			this.historicalMarketValueProviderForInSample =
//				new HistoricalAdjustedQuoteProvider();
				new HistoricalBarInterpolatingProvider( this.getBarCache() );
			
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
		}

		protected override IEligiblesSelector getEligiblesSelector()
		{
			
			int maxNumberOfEligiblesToBeChosen = 100;
						
			string tickersGroupId = "SP500";
			
			bool temporizedGroup = true;
			int numDaysForAverageRawOpenPriceComputation = 10;
			double minPrice = 30;
			double maxPrice = 300;
			
			int maxNumberOfMostLiquidTickersToBeChosen = 150;
			int numDaysForVolatility = 10;
			int minimumNumberOfMinutelyBarsForEachDayForInSample = 20;
						
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
			
			IEligiblesSelector eligiblesSelector =
				new ByPriceMostLiquidAlwaysIntradayQuoted(
				tickersGroupId , this.benchmark, temporizedGroup ,
				maxNumberOfEligiblesToBeChosen ,
				numDaysForAverageRawOpenPriceComputation ,
				minPrice , maxPrice,
				minimumNumberOfMinutelyBarsForEachDayForInSample  );
			
//			IEligiblesSelector eligiblesSelector =
//				new ByPriceMostLiquidAlwaysQuoted(
//				tickersGroupId , temporizedGroup ,
//				maxNumberOfEligiblesToBeChosen ,
//			  numDaysForAverageRawOpenPriceComputation ,
//			 	minPrice , maxPrice );
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
			numberOfBestTestingPositionsToBeReturned = 50;
			bool balancedWeightsOnVolatilityBase = true;
			float minimumAbsoluteReturnValue = 0.00005f;
			float maximumAbsoluteReturnValue = 100000f;
			int returnIntervalLengthInMinutesForCorrelationProvider = 120;
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
//				new PVO_CTCCorrelationChooser(numberOfBestTestingPositionsToBeReturned, 
//						1 , maxCorrelationAllowed , balancedWeightsOnVolatilityBase,
//						minimumAbsoluteReturnValue , maximumAbsoluteReturnValue, this.benchmark.Ticker);
					new PVOIntradayCorrelationChooser(numberOfBestTestingPositionsToBeReturned, 
						returnIntervalLengthInMinutesForCorrelationProvider ,
						maxCorrelationAllowed , balancedWeightsOnVolatilityBase,
						minimumAbsoluteReturnValue , maximumAbsoluteReturnValue, this.benchmark.Ticker);

//			//office
//			inSampleChooser =
//				new PVOChooserFromSavedBackTestLog(
//					@"C:\Utente\MarcoVarie\Vari\qP\LogArchive\2009_01_05_13_40_28_PVOIntraday_from_2006_01_01_to_2007_12_31_annlRtrn_17,84_maxDD_4,52\2009_01_05_13_40_28_PVOIntraday_from_2006_01_01_to_2007_12_31_annlRtrn_17,84_maxDD_4,52.qpL",
//				  numberOfBestTestingPositionsToBeReturned);
			//home
//			inSampleChooser =
//				new PVOChooserFromSavedBackTestLog(
//				@"C:\Utente\MarcoVarie\Vari\qP\LogArchive\2009_02_17_23_17_03_PVOIntraday_from_2006_01_01_to_2007_12_31_annlRtrn_24.44_maxDD_3.05\2009_02_17_23_17_03_PVOIntraday_from_2006_01_01_to_2007_12_31_annlRtrn_24.44_maxDD_3.05.qpL",
//				numberOfBestTestingPositionsToBeReturned);
			return inSampleChooser;
		}

		protected override IStrategyForBacktester getStrategyForBacktester()
		{
			//int inSampleDays = 90;
			int inSampleDays = 30;
			int numDaysBetweenEachOptimization = 4;
			int minNumOfEligiblesForValidOptimization = 20;
			double oversoldThreshold = 0.005;//0.006
			double overboughtThreshold = 0.005;//0.006
			double oversoldThresholdMAX = 0.025;
			double overboughtThresholdMAX = 0.025;
			double stopLoss = 0.02;
			double takeProfit = 0.0025;//0.0045
			
			IStrategyForBacktester strategyForBacktester
//				 = new PVO_OTCStrategyLessCorrelated(eligiblesSelector ,inSampleChooser ,
//				inSampleDays , benchmark , numDaysBetweenEachOptimization ,
//				oversoldThreshold , overboughtThreshold , historicalQuoteProvider);
//			
				= new PVOStrategyIntraday(eligiblesSelector ,
				minNumOfEligiblesForValidOptimization, inSampleChooser ,
				inSampleDays , benchmark , numDaysBetweenEachOptimization ,
				oversoldThreshold , overboughtThreshold ,
				oversoldThresholdMAX , overboughtThresholdMAX ,
				historicalMarketValueProviderForInSample,
			  historicalMarketValueProviderForOutOfSample,
			  inefficiencyLengthInMinutes , minutesFromLastInefficiencyToWaitBeforeOpening,
			  this.minutesFromLastLossOrProfitToWaitBeforeClosing,
			  maxOpeningLengthInMinutes, this.getDailyTimes(),
			  stopLoss , takeProfit );
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
		
		private HistoricalBarProvider getHistoricalBarProvider()
		{
			IBarCache barCache = getBarCache();
			HistoricalBarProvider historicalBarProvider =
				new HistoricalBarProvider( barCache );
			return historicalBarProvider;
		}
		#endregion getHistoricalBarProvider
		
		protected override EndOfDayStrategyBackTester getEndOfDayStrategyBackTester()
		{
			string backTestId = "PVOIntraday";
			IAccountProvider accountProvider;
			accountProvider =	new SimpleAccountProvider();
//			double fixedPercentageSlippage = 0.05;
//			accountProvider =
//				new InteractiveBrokerAccountProvider(fixedPercentageSlippage);
			double cashToStart = 25000;
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
		
		protected override string getCustomSmallTextForFolderName()
		{
			return "PVOIntraday";
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
				@"C:\Quant\QuantProject\b7_Scripts\TechnicalAnalysisTesting\Oscillators\FixedLevelOscillators\PortfolioValueOscillator\PVOStrategyIntradayMain.cs";
			if( File.Exists(fullPathFileNameForMainAtHome) )
				returnValue = fullPathFileNameForMainAtHome;
			else
				returnValue = 
					@"C:\Utente\MarcoVarie\Vari\qP\QuantProject\b7_Scripts\TechnicalAnalysisTesting\Oscillators\FixedLevelOscillators\PortfolioValueOscillator\PVOStrategyIntradayMain.cs";
			
			return returnValue;
		}
	}
}
