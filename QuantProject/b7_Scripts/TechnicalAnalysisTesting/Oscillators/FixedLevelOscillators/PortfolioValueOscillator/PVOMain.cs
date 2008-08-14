/*
QuantProject - Quantitative Finance Library

PVOMain.cs
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
	/// Entry point for the PVO strategy. If any strategy
	/// parameter had to be changed, this is the place where it should
	/// be done
	/// </summary>
	public class PVOMain : BasicScriptForBacktesting
	{
		private Benchmark benchmark;
		private HistoricalQuoteProvider historicalQuoteProvider;
		
		public PVOMain()
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
				new ByPriceMostLiquidLessVolatileCTCAlwaysQuoted(
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
			int numberOfBestTestingPositionsToBeReturned = 50;
			numberOfBestTestingPositionsToBeReturned = 10;
			bool balancedWeightsOnVolatilityBase = true;
			float minimumAbsoluteReturnValue = 0.000001f;
			float maximumAbsoluteReturnValue = 100000f;
			int closeToCloseIntervalLengthForCorrelation = 2;
			//correlation is computed only for returns
			//between minimum and maximum
			IInSampleChooser inSampleChooser = 
//				new PVO_CTCCorrelationChooser(numberOfBestTestingPositionsToBeReturned, 
//				closeToCloseIntervalLengthForCorrelation , maxCorrelationAllowed , balancedWeightsOnVolatilityBase,
//				minimumAbsoluteReturnValue , maximumAbsoluteReturnValue, this.benchmark.Ticker );
				new PVO_CTCStrongCorrelationChooser(numberOfBestTestingPositionsToBeReturned, 
				maxCorrelationAllowed , minimumAbsoluteReturnValue , maximumAbsoluteReturnValue,
				balancedWeightsOnVolatilityBase, this.benchmark.Ticker );
			//			IInSampleChooser inSampleChooser = 
			//				new PVO_OTCCTOCorrelationChooser(numberOfBestTestingPositionsToBeReturned, 
			//						maxCorrelationAllowed , balancedWeightsOnVolatilityBase,
			//					  minimumAbsoluteReturnValue , maximumAbsoluteReturnValue);
			//			//office
//			inSampleChooser =
//				new PVOChooserFromSavedBackTestLog(
//				@"C:\Utente\MarcoVarie\Vari\qP\LogArchive\2008_05_23_15_46_38_PVO_CTC_from_2001_01_01_to_2001_12_31_annlRtrn_8,61_maxDD_17,21\2008_05_23_15_46_38_PVO_CTC_from_2001_01_01_to_2001_12_31_annlRtrn_8,61_maxDD_17,21.qpL",
//				numberOfBestTestingPositionsToBeReturned);
			//home
						inSampleChooser =
							new PVOChooserFromSavedBackTestLog(
							@"C:\Utente\MarcoVarie\Vari\qP\LogArchive\2008_06_03_18_33_29_PVO_CTC_from_2001_01_01_to_2004_12_31_annlRtrn_4.72_maxDD_13.57\2008_06_03_18_33_29_PVO_CTC_from_2001_01_01_to_2004_12_31_annlRtrn_4.72_maxDD_13.57.qpL",
							numberOfBestTestingPositionsToBeReturned);
			return inSampleChooser;
		}

		protected override IEndOfDayStrategyForBacktester getEndOfDayStrategy()
		{
			int inSampleDays = 180;
			// uncomment the following line for a faster script
			//inSampleDays = 50;
			int numDaysBetweenEachOptimization = 5;
			double oversoldThreshold = 0.002;
			double overboughtThreshold = 0.002;
			double oversoldThresholdMAX = 0.02;
			double overboughtThresholdMAX = 0.02;
			double maxAcceptableDrawDown = 0.01;
			double minimumAcceptableGain = 0.002;
			int closeToCloseIntervalLength = 1;
			IEndOfDayStrategyForBacktester endOfDayStrategy
				//				 = new PVO_OTCStrategyLessCorrelated(eligiblesSelector ,inSampleChooser ,
				//				inSampleDays , benchmark , numDaysBetweenEachOptimization ,
				//				oversoldThreshold , overboughtThreshold , historicalQuoteProvider);
				//			
				= new PVOStrategy(eligiblesSelector , inSampleChooser ,
				inSampleDays , closeToCloseIntervalLength , 2 , benchmark , numDaysBetweenEachOptimization ,
				oversoldThreshold , overboughtThreshold ,
				oversoldThresholdMAX , overboughtThresholdMAX , historicalQuoteProvider, 
				maxAcceptableDrawDown, minimumAcceptableGain );
			return endOfDayStrategy;
		}
		protected override EndOfDayStrategyBackTester getEndOfDayStrategyBackTester()
		{
			string backTestId = "PVO_CTC";
			IAccountProvider accountProvider;
			accountProvider =	new SimpleAccountProvider();
			//			double fixedPercentageSlippage = 0.05;
			//			accountProvider =
			//				new InteractiveBrokerAccountProvider(fixedPercentageSlippage);
			double cashToStart = 25000;

			DateTime firstDateTime = new DateTime( 2001 , 1 , 1 );
			DateTime lastDateTime = new DateTime( 2004 , 12, 31 );
			double maxRunningHours = 8;
			HistoricalQuoteProvider quoteProviderForBackTester =
				this.historicalQuoteProvider;
			quoteProviderForBackTester =
				new HistoricalAdjustedQuoteProvider();
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
				backTestId , this.endOfDayStrategy ,
				quoteProviderForBackTester , accountProvider ,
				firstDateTime ,	lastDateTime ,
				this.benchmark , cashToStart , maxRunningHours );
			return endOfDayStrategyBackTester;
		}

		protected override string getPathForTheMainFolderWhereScriptsResultsAreToBeSaved()
		{
			string pathForTheMainFolderWhereScriptsResultsAreToBeSaved =
				System.Configuration.ConfigurationSettings.AppSettings["LogArchive"];
			return pathForTheMainFolderWhereScriptsResultsAreToBeSaved;
		}

		protected override string getCustomSmallTextForFolderName()
		{
			return "PVO_CTC";
		}

		protected override string getFullPathFileNameForMain()
		{
			string returnValue;
			string fullPathFileNameForMainAtHome = 
				@"C:\Quant\QuantProject\b7_Scripts\TechnicalAnalysisTesting\Oscillators\FixedLevelOscillators\PortfolioValueOscillator\PVOMain.cs";
			if( File.Exists(fullPathFileNameForMainAtHome) )
				returnValue = fullPathFileNameForMainAtHome;
			else
				returnValue = 
					@"C:\Utente\MarcoVarie\Vari\qP\QuantProject\b7_Scripts\TechnicalAnalysisTesting\Oscillators\FixedLevelOscillators\PortfolioValueOscillator\PVOMain.cs";
			
			return returnValue;
		}
	}
}
