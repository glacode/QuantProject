/*
QuantProject - Quantitative Finance Library

PVO_CTOMain.cs
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
using System.IO;

using QuantProject.ADT;
using QuantProject.ADT.FileManaging;
using QuantProject.Business.DataProviders;
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
	/// Entry point for the PVO_OTC strategy. If any strategy
	/// parameter had to be changed, this is the place where it should
	/// be done
	/// </summary>
	public class PVO_CTOMain : BasicScriptForBacktesting
	{
		private Benchmark benchmark;
		private HistoricalMarketValueProvider historicalQuoteProvider;
		
		public PVO_CTOMain()
		{
			this.benchmark = new Benchmark( "BMC" );

			this.historicalQuoteProvider =
				new HistoricalAdjustedQuoteProvider();

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
			IEligiblesSelector eligiblesSelector =
				new ByPriceMostLiquidLessVolatileCTOAlwaysQuoted(
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
			eligiblesSelector =
				new DummyEligibleSelector();
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
			int numberOfBestTestingPositionsToBeReturned = 100;
			numberOfBestTestingPositionsToBeReturned = 100;
			bool balancedWeightsOnVolatilityBase = true;
			int returnIntervalLength = 1;
			float minimumAbsoluteReturnValue = 0.000001f;
			float maximumAbsoluteReturnValue = 100000f;
			//correlation is computed only for returns
			//between minimum and maximum
			IInSampleChooser inSampleChooser =
				new PVO_CTCCorrelationChooser(numberOfBestTestingPositionsToBeReturned,
				                              returnIntervalLength, maxCorrelationAllowed , balancedWeightsOnVolatilityBase,
				                              minimumAbsoluteReturnValue , maximumAbsoluteReturnValue, this.benchmark.Ticker);
//			IInSampleChooser inSampleChooser =
//					new PVO_OTOCorrelationChooser(numberOfBestTestingPositionsToBeReturned,
//							returnIntervalLength, maxCorrelationAllowed , balancedWeightsOnVolatilityBase,
//						  minimumAbsoluteReturnValue , maximumAbsoluteReturnValue);

//			IInSampleChooser inSampleChooser =
//				new PVO_OTCCorrelationChooser(numberOfBestTestingPositionsToBeReturned,
//						maxCorrelationAllowed , balancedWeightsOnVolatilityBase,
//					  minimumAbsoluteReturnValue , maximumAbsoluteReturnValue);
//			IInSampleChooser inSampleChooser =
//				new PVO_CTOCorrelationChooser(numberOfBestTestingPositionsToBeReturned,
//						maxCorrelationAllowed , balancedWeightsOnVolatilityBase,
//					  minimumAbsoluteReturnValue , maximumAbsoluteReturnValue);
			
//			IInSampleChooser inSampleChooser =
//				new PVO_OTCCTOCorrelationChooser(numberOfBestTestingPositionsToBeReturned,
//						maxCorrelationAllowed , balancedWeightsOnVolatilityBase,
//					  minimumAbsoluteReturnValue , maximumAbsoluteReturnValue);
//			//office
//			inSampleChooser =
//				new PVOChooserFromSavedBackTestLog(
//					@"C:\Utente\MarcoVarie\Vari\qP\LogArchive\2008_05_02_15_55_32_PVO_OTC_from_2001_01_01_to_2008_04_28_annlRtrn_101,19_maxDD_6,30\2008_05_02_15_55_32_PVO_OTC_from_2001_01_01_to_2008_04_28_annlRtrn_101,19_maxDD_6,30.qpL",
//				  numberOfBestTestingPositionsToBeReturned);
			
			//office - CTO
//			inSampleChooser =
//				new PVOChooserFromSavedBackTestLog(
//					@"C:\Utente\MarcoVarie\Vari\qP\LogArchive\2008_05_14_10_12_30_PVO_CTO_from_2001_01_01_to_2004_12_31_annlRtrn_16,08_maxDD_8,34\2008_05_14_10_12_30_PVO_CTO_from_2001_01_01_to_2004_12_31_annlRtrn_16,08_maxDD_8,34.qpL",
//				  numberOfBestTestingPositionsToBeReturned);
			
			//home
			inSampleChooser =
				new PVOChooserFromSavedBackTestLog(
					@"C:\Utente\MarcoVarie\Vari\qP\LogArchive\2008_05_18_12_13_22_PVO_CTO_from_2006_01_01_to_2008_04_28_annlRtrn_4.15_maxDD_17.07\2008_05_18_12_13_22_PVO_CTO_from_2006_01_01_to_2008_04_28_annlRtrn_4.15_maxDD_17.07.qpL",
					numberOfBestTestingPositionsToBeReturned);
			return inSampleChooser;
		}

		protected override IStrategyForBacktester getStrategyForBacktester()
		{
			int inSampleDays = 180;
			// uncomment the following line for a faster script
			//inSampleDays = 50;
			int numDaysBetweenEachOptimization = 5;
			int numOfClosingsBeforeExit = 0;
			int minNumOfEligiblesForValidOptimization = 10;
			double oversoldThreshold = 0.007;
			double overboughtThreshold = 0.007;
			double oversoldThresholdMAX = 0.02;
			double overboughtThresholdMAX = 0.02;
			IStrategyForBacktester strategyForBacktester
				= new PVO_OTCStrategy(eligiblesSelector ,
				                      minNumOfEligiblesForValidOptimization, inSampleChooser ,
				                      inSampleDays , benchmark , numDaysBetweenEachOptimization ,
				                      numOfClosingsBeforeExit, oversoldThreshold , overboughtThreshold ,
				                      oversoldThresholdMAX , overboughtThresholdMAX , historicalQuoteProvider);
			return strategyForBacktester;
		}
		protected override EndOfDayStrategyBackTester getEndOfDayStrategyBackTester()
		{
			string backTestId = "PVO_CTO";
			IAccountProvider accountProvider;
			accountProvider =	new SimpleAccountProvider();
//			double fixedPercentageSlippage = 0.05;
//			accountProvider =
//				new InteractiveBrokerAccountProvider(fixedPercentageSlippage);
			double cashToStart = 25000;

			DateTime firstDateTime = new DateTime( 2006 , 1 , 1 );
			DateTime lastDateTime = new DateTime( 2008 , 4, 28 );
			double maxRunningHours = 3;
			HistoricalMarketValueProvider quoteProviderForBackTester =
				this.historicalQuoteProvider;
			quoteProviderForBackTester =
				new HistoricalAdjustedQuoteProvider();
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
					backTestId ,
					new QuantProject.Business.Timing.IndexBasedEndOfDayTimer(
						HistoricalEndOfDayTimer.GetMarketOpen( firstDateTime ) ,
						this.benchmark.Ticker ) ,
					this.strategyForBacktester ,
					quoteProviderForBackTester , accountProvider ,
					firstDateTime ,	lastDateTime ,
					this.benchmark , cashToStart , maxRunningHours );
			return endOfDayStrategyBackTester;
		}

		protected override string getPathForTheMainFolderWhereScriptsResultsAreToBeSaved()
		{
			string pathForTheMainFolderWhereScriptsResultsAreToBeSaved =
				System.Configuration.ConfigurationManager.AppSettings["LogArchive"];
			return pathForTheMainFolderWhereScriptsResultsAreToBeSaved;
		}

		protected override string getCustomSmallTextForFolderName()
		{
			return "PVO_CTO";
		}

		protected override string getFullPathFileNameForMain()
		{
			string returnValue;
			string fullPathFileNameForMainAtHome =
				@"C:\Quant\QuantProject\b7_Scripts\TechnicalAnalysisTesting\Oscillators\FixedLevelOscillators\PortfolioValueOscillator\PVO_CTOMain.cs";
			if( File.Exists(fullPathFileNameForMainAtHome) )
				returnValue = fullPathFileNameForMainAtHome;
			else
				returnValue =
					@"C:\Utente\MarcoVarie\Vari\qP\QuantProject\b7_Scripts\TechnicalAnalysisTesting\Oscillators\FixedLevelOscillators\PortfolioValueOscillator\PVO_CTOMain.cs";
			
			return returnValue;
		}
	}
}
