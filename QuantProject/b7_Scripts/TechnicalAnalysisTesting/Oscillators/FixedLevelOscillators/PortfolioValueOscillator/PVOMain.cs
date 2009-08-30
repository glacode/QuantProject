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
using System.Collections.Generic;
using System.IO;

using QuantProject.ADT;
using QuantProject.ADT.Timing;
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
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.FitnessEvaluators;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.InSampleChoosers.PriceRatioChooser;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.Decoding;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.EntryConditions;
using QuantProject.Scripts.General;
using QuantProject.Scripts.General.Logging;
using QuantProject.Scripts.General.Reporting;
using QuantProject.Business.Timing.TimingManagement;


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
		private HistoricalMarketValueProvider historicalQuoteProvider;
	
		private	double maxAcceptableDrawDown;
		private	double minimumAcceptableGain;
		private	int closeToCloseIntervalLength;
		private	int numberOfPortfolioPositions;
		private	int numberOfBestTestingPositionsToBeReturned;
		private IEquityEvaluator equityEvaluator;
		private DateTime firstDateTime;
		private DateTime lastDateTime;
		
		public PVOMain()
		{
			this.benchmark = new Benchmark( "ENI.MI" );
			this.historicalQuoteProvider =
				new HistoricalAdjustedQuoteProvider();
			this.maxAcceptableDrawDown = 0.03;
			this.minimumAcceptableGain = 0.01;
			this.closeToCloseIntervalLength = 2;
			this.numberOfPortfolioPositions = 2;
			this.numberOfBestTestingPositionsToBeReturned = 40;
			this.firstDateTime = new DateTime( 2004 , 1 , 1 );
			this.lastDateTime = new DateTime( 2004 , 6, 30 );
		}

		protected override IEligiblesSelector getEligiblesSelector()
		{
			int maxNumberOfEligiblesToBeChosen = 50;
			string tickersGroupId = "STOCKMI";
			
			bool temporizedGroup = true;
			int numDaysForAverageRawOpenPriceComputation = 10;
			double minPrice = 0.1;
			double maxPrice = 1500;
			
			int maxNumberOfMostLiquidTickersToBeChosen = 100;
			int numDaysForVolatility = 10;
			IEligiblesSelector eligiblesSelector =
				new ByPriceMostLiquidLessVolatileCTCAlwaysQuoted(
					tickersGroupId , temporizedGroup ,
					maxNumberOfEligiblesToBeChosen ,
					maxNumberOfMostLiquidTickersToBeChosen ,
					numDaysForAverageRawOpenPriceComputation ,
					numDaysForVolatility ,
					minPrice , maxPrice );

//			eligiblesSelector =
//				new DummyEligibleSelector();
			
			return eligiblesSelector;
		}

		protected override IInSampleChooser getInSampleChooser()
		{
			// parameters for the genetic optimizer
//			
			double crossoverRate = 0.85;
			double mutationRate = 0.1;
			double elitismRate = 0.01;
			int populationSizeForGeneticOptimizer = 5000;
			int generationNumberForGeneticOptimizer = 5;
			int seedForRandomGenerator =
					QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator;
			int divisorForThresholdComputation = 1000;
			bool symmetricalThreshold = true;
			bool overboughtMoreThanOversoldThresholdsForStationaryPortfolios = false;
			IDecoderForTestingPositions decoderForWeightedPositions =
					new BasicDecoderForPVOPositions(symmetricalThreshold, divisorForThresholdComputation,
				                                this.closeToCloseIntervalLength);
			this.equityEvaluator = new SharpeRatio();
			IFitnessEvaluator pvoFitnessEvaluator =
				new PVOFitnessEvaluator(this.equityEvaluator, this.minimumAcceptableGain,
				                        this.maxAcceptableDrawDown);
			IInSampleChooser inSampleChooser =
				new PVOGeneticChooser(this.closeToCloseIntervalLength, this.numberOfPortfolioPositions,
				                      this.numberOfBestTestingPositionsToBeReturned, this.benchmark,
				                      decoderForWeightedPositions, pvoFitnessEvaluator, 
				                      this.historicalQuoteProvider, crossoverRate, mutationRate,
				                      elitismRate, populationSizeForGeneticOptimizer, generationNumberForGeneticOptimizer,
				                      seedForRandomGenerator, 5, 5, 5, 5, divisorForThresholdComputation,
				                      symmetricalThreshold, overboughtMoreThanOversoldThresholdsForStationaryPortfolios);
//					
//			double maxCorrelationAllowed = 0.96;
//			bool balancedWeightsOnVolatilityBase = true;
//			float minimumAbsoluteReturnValue = 0.000001f;
//			float maximumAbsoluteReturnValue = 100000f;
//			int closeToCloseIntervalLengthForCorrelation = 1;
			//correlation is computed only for returns
			//between minimum and maximum
//			IInSampleChooser inSampleChooser =
//				new PVO_CTCCorrelationChooser(numberOfBestTestingPositionsToBeReturned,
//				closeToCloseIntervalLengthForCorrelation , maxCorrelationAllowed , balancedWeightsOnVolatilityBase,
//				minimumAbsoluteReturnValue , maximumAbsoluteReturnValue, this.benchmark.Ticker );
				
//				new PVOLessVolatilePriceRatioChooser(numberOfBestTestingPositionsToBeReturned, balancedWeightsOnVolatilityBase);
				                                       
//				new PVO_CTCStrongCorrelationChooser(numberOfBestTestingPositionsToBeReturned,
//				                                    maxCorrelationAllowed , minimumAbsoluteReturnValue , maximumAbsoluteReturnValue,
//				                                    balancedWeightsOnVolatilityBase, this.benchmark.Ticker );
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
//			inSampleChooser =
//				new PVOChooserFromSavedBackTestLog(
//					@"C:\Utente\MarcoVarie\Vari\qP\LogArchive\2008_06_03_18_33_29_PVO_CTC_from_2001_01_01_to_2004_12_31_annlRtrn_4.72_maxDD_13.57\2008_06_03_18_33_29_PVO_CTC_from_2001_01_01_to_2004_12_31_annlRtrn_4.72_maxDD_13.57.qpL",
//					numberOfBestTestingPositionsToBeReturned);
			return inSampleChooser;
		}

		protected override IStrategyForBacktester getStrategyForBacktester()
		{
			int inSampleDays = 180;
			// uncomment the following line for a faster script
			//inSampleDays = 50;
			int numDaysBetweenEachOptimization = 10;
			double oversoldThreshold = 0.015;
			double overboughtThreshold = 0.015;
			double oversoldThresholdMAX = 0.1;
			double overboughtThresholdMAX = 0.1;
			double numOfStdDevForSignificantPriceMovements = 1.5;
		  double leverage = 1.0;
		  bool openOnlyLongPositions = false;
		  int maxNumberOfDaysOnTheMarket = 5;
		  List<IEntryCondition> entryConditions = new List<IEntryCondition>();
		  IEntryCondition entryCondition = 
//		  	new AlwaysTrueEntryCondition();
//		  	new PriceRatioEntryCondition(45, this.historicalQuoteProvider, 1.0);
		  	new PreviousPeriodsWereEfficientEntryCondition(1, this.historicalQuoteProvider,
		  	                                               this.closeToCloseIntervalLength,
		  	                                               new MarketDaysManager(this.benchmark, 
		  	                                                                    this.firstDateTime, this.lastDateTime,
		  	                                                                    new Time("16:00:00")));
		  entryConditions.Add(entryCondition);
		  bool allEntryConditionsHaveToBeTrue = true;
			
			IStrategyForBacktester strategyForBacktester
			//pair trading	
//				= new PVOStrategy(eligiblesSelector , inSampleChooser ,
//				                  inSampleDays , closeToCloseIntervalLength , 2 , benchmark , numDaysBetweenEachOptimization ,
//				                  oversoldThreshold , overboughtThreshold ,
//				                  oversoldThresholdMAX , overboughtThresholdMAX ,
//				                  numOfStdDevForSignificantPriceMovements,
//				                  leverage , openOnlyLongPositions ,
//				                  maxNumberOfDaysOnTheMarket,
//				                  historicalQuoteProvider,
//				                  maxAcceptableDrawDown, minimumAcceptableGain );
				= new PVOStrategy(eligiblesSelector , inSampleChooser ,
				                  inSampleDays , closeToCloseIntervalLength , 
				                  this.numberOfPortfolioPositions , benchmark , numDaysBetweenEachOptimization ,
				                  oversoldThreshold , overboughtThreshold ,
				                  oversoldThresholdMAX , overboughtThresholdMAX ,
				                  numOfStdDevForSignificantPriceMovements,
				                  leverage , openOnlyLongPositions ,
				                  maxNumberOfDaysOnTheMarket,
				                  historicalQuoteProvider,
				                  maxAcceptableDrawDown, minimumAcceptableGain, entryConditions,
				                  allEntryConditionsHaveToBeTrue);
			
			return strategyForBacktester;
		}
		protected override EndOfDayStrategyBackTester getEndOfDayStrategyBackTester()
		{
			string backTestId = "PVO_CTC";
			IAccountProvider accountProvider;
			accountProvider =	new SimpleAccountProvider();
			//			double fixedPercentageSlippage = 0.05;
			//			accountProvider =
			//				new InteractiveBrokerAccountProvider(fixedPercentageSlippage);
			double cashToStart = 10000;

			double maxRunningHours = 10;
			HistoricalMarketValueProvider quoteProviderForBackTester =
				this.historicalQuoteProvider;
			quoteProviderForBackTester =
				new HistoricalAdjustedQuoteProvider();
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
					backTestId ,
					new QuantProject.Business.Timing.IndexBasedEndOfDayTimer(
						HistoricalEndOfDayTimer.GetMarketClose( firstDateTime ) ,
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
