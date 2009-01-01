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
		private double inefficiencyLengthInMinutes;
		private double maxOpeningLengthInMinutes;
		private Timer timerForBackTester;
		private DateTime firstDateTime;
		private DateTime lastDateTime;
		private int intervalFrameInSeconds;
		
		public PVOStrategyIntradayMain()
		{
			//this.benchmark = new Benchmark( "^GSPC" );
			this.inefficiencyLengthInMinutes = 30;
			this.maxOpeningLengthInMinutes = 120;
			
			this.firstDateTime = new DateTime( 2006 , 1 , 1 );
			this.lastDateTime = new DateTime( 2007 , 12, 31 );
			this.intervalFrameInSeconds = 60;
			this.benchmark = new Benchmark( "MSFT" );
			this.historicalMarketValueProviderForInSample =
				new HistoricalAdjustedQuoteProvider();
			this.historicalMarketValueProviderForOutOfSample =
				this.getHistoricalBarProvider();
			this.timerForBackTester = 
				new IndexBasedHistoricalTimer(this.benchmark.Ticker,
				                              this.firstDateTime,
				                              this.lastDateTime ,
				                            	this.getDailyTimes(),
				                              this.intervalFrameInSeconds);
			// definition for the Fitness Evaluator
			//      IEquityEvaluator equityEvaluator = new SharpeRatio();
		}

		protected override IEligiblesSelector getEligiblesSelector()
		{
			
			int maxNumberOfEligiblesToBeChosen = 100;
						
			string tickersGroupId = "SP500";
			
			bool temporizedGroup = true;
			int numDaysForAverageRawOpenPriceComputation = 10;
			double minPrice = 30;
			double maxPrice = 75;
			
			int maxNumberOfMostLiquidTickersToBeChosen = 150;
			int numDaysForVolatility = 10;
						
//			IEligiblesSelector eligiblesSelector =
//				new ByPriceMostLiquidQuotedAtEachDateTime(
//				tickersGroupId , temporizedGroup ,
//				maxNumberOfEligiblesToBeChosen ,
//			  numDaysForAverageRawOpenPriceComputation ,
//			 	minPrice , maxPrice, intervalFrameInSeconds ,
//			 	this.benchmark.Ticker );
//			
			IEligiblesSelector eligiblesSelector =
				new ByPriceMostLiquidLessVolatileOTCAlwaysQuoted(
				tickersGroupId , temporizedGroup ,
				maxNumberOfEligiblesToBeChosen ,
				maxNumberOfMostLiquidTickersToBeChosen ,
			  numDaysForAverageRawOpenPriceComputation ,
				numDaysForVolatility ,
			 	minPrice , maxPrice );
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
			float minimumAbsoluteReturnValue = 0.000001f;
			float maximumAbsoluteReturnValue = 100000f;
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
						1 , maxCorrelationAllowed , balancedWeightsOnVolatilityBase,
						minimumAbsoluteReturnValue , maximumAbsoluteReturnValue, this.benchmark.Ticker);
//			//office
//			inSampleChooser =
//				new PVOChooserFromSavedBackTestLog(
//					@"C:\Utente\MarcoVarie\Vari\qP\LogArchive\2008_08_06_16_57_34_PVO_OTC_from_2003_06_01_to_2008_04_28_annlRtrn_3,34_maxDD_11,36\2008_08_06_16_57_34_PVO_OTC_from_2003_06_01_to_2008_04_28_annlRtrn_3,34_maxDD_11,36.qpL",
//				  numberOfBestTestingPositionsToBeReturned);
			//home
//			inSampleChooser =
//				new PVOChooserFromSavedBackTestLog(
//				@"C:\Utente\MarcoVarie\Vari\qP\LogArchive\2008_05_04_18_54_45_PVO_OTC_from_2006_01_01_to_2008_04_28_annlRtrn_93.08_maxDD_5.18\2008_05_04_18_54_45_PVO_OTC_from_2006_01_01_to_2008_04_28_annlRtrn_93.08_maxDD_5.18.qpL",
//				numberOfBestTestingPositionsToBeReturned);
			return inSampleChooser;
		}

		protected override IStrategyForBacktester getStrategyForBacktester()
		{
			//int inSampleDays = 90;
			int inSampleDays = 150;
			int numDaysBetweenEachOptimization = 5;
			int minNumOfEligiblesForValidOptimization = 20;
			double oversoldThreshold = 0.0065;
			double overboughtThreshold = 0.0065;
			double oversoldThresholdMAX = 0.02;
			double overboughtThresholdMAX = 0.02;
			double stopLoss = 0.02;
			double takeProfit = 0.005;
			
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
			  inefficiencyLengthInMinutes , maxOpeningLengthInMinutes, this.getDailyTimes(),
			  stopLoss , takeProfit );
			return strategyForBacktester;
		}
		#region getHistoricalBarProvider
		
		#region getBarCache
		private List<Time> getDailyTimes()
		{
			List<Time> dailyTimes = new List<Time>();
			dailyTimes.Add( new Time("10:00:00") );
			dailyTimes.Add( new Time("10:30:00") );
			dailyTimes.Add( new Time("11:00:00") );
			dailyTimes.Add( new Time("11:30:00") );
			dailyTimes.Add( new Time("12:00:00") );
			dailyTimes.Add( new Time("12:30:00") );
			dailyTimes.Add( new Time("13:00:00") );
			dailyTimes.Add( new Time("13:30:00") );
			dailyTimes.Add(new Time("14:00:00") );
			dailyTimes.Add(new Time("14:30:00") );
			dailyTimes.Add(new Time("15:00:00") );
			dailyTimes.Add(new Time("16:00:00") );
			return dailyTimes;
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
			double maxRunningHours = 5;
			HistoricalMarketValueProvider quoteProviderForBackTester =
				this.historicalMarketValueProviderForOutOfSample;
//			quoteProviderForBackTester =
//				new HistoricalRawQuoteProvider();
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
				backTestId , this.timerForBackTester, 
				this.strategyForBacktester ,
				quoteProviderForBackTester , accountProvider ,
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
