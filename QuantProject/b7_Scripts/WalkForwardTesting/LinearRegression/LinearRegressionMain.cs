/*
QuantProject - Quantitative Finance Library

LinearRegressionMain.cs
Copyright (C) 2010
Glauco Siliprandi

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

using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.ADT.Timing;
using QuantProject.Data.Selectors;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting.AccountProviding;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;
using QuantProject.Scripts.General;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Entry point for the LinearRegression strategy. If any strategy
	/// parameter had to be changed, this is the place where it should
	/// be done
	/// </summary>
	public class LinearRegressionMain : BasicScriptForBacktesting
	{
		private Benchmark benchmark;
		private HistoricalMarketValueProvider historicalMarketValueProviderForInSample;
		private HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample;
		private HistoricalMarketValueProvider
			historicalMarketValueProviderForTheBacktesterAccount;
		private LinearRegressionFitnessEvaluator fitnessEvaluator;
		private IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling;
//		protected IEligiblesSelector eligiblesSelectorForSignalingTickers;

		public LinearRegressionMain()
		{
			this.benchmark = new Benchmark( "CCE" );
			this.historicalMarketValueProviderForInSample =
				new HistoricalAdjustedQuoteProvider();
			this.historicalMarketValueProviderForChosingPositionsOutOfSample =
				this.historicalMarketValueProviderForInSample;
			this.historicalMarketValueProviderForTheBacktesterAccount =
				this.historicalMarketValueProviderForChosingPositionsOutOfSample;
			this.returnIntervalSelectorForSignaling =
				new ShiftedTimeIntervalSelectorForSignaling( new TimeSpan( -24 , 0 , 0 ) );

		}
		
		protected override string getPathForTheMainFolderWhereScriptsResultsAreToBeSaved()
		{
			string pathForTheMainFolderWhereScriptsResultsAreToBeSaved =
				@"T:\senzaBackup\qpReports\linearRegression\";
			return pathForTheMainFolderWhereScriptsResultsAreToBeSaved;
		}
		
		protected override string getCustomSmallTextForFolderName()
		{
			return "lnrRgrssn";
		}
		
		protected override string getFullPathFileNameForMain()
		{
			string fullPathFileNameForMain =
				@"T:\QuantProject\QuantProject\b7_Scripts\WalkForwardTesting\LinearRegression\LinearRegressionMain.cs";
			return fullPathFileNameForMain;
		}
		
		protected override IEligiblesSelector getEligiblesSelector()
		{
			string groupIdForTradingTickers = "SP500";
			string groupIdForAdditionalSignalingTickers = "usIndxs";
			ITickerSelectorByGroup tickerSelectorByGroup = new TickerSelectorByGroup();
//			double minPercentageOfAvailableValues = 0.8;
			double minPercentageOfAvailableValues = 1;
			double minPriceForTradingTicker = 1;
			double maxPriceForTradingTicker = 9000;
			int maxNumberOfEligiblesForTrading = 150;
			
			// uncomment the followings lines for a faster script
			groupIdForTradingTickers = "fastTest";
			groupIdForAdditionalSignalingTickers = "fastTest";
			maxNumberOfEligiblesForTrading = 8;

			IEligiblesSelector eligiblesSelector =
				new EligiblesSelectorForLinearRegression(
					groupIdForTradingTickers ,
					groupIdForAdditionalSignalingTickers ,
					tickerSelectorByGroup ,
					this.benchmark ,
					this.historicalMarketValueProviderForInSample ,
					minPercentageOfAvailableValues ,
					minPriceForTradingTicker ,
					maxPriceForTradingTicker ,
					maxNumberOfEligiblesForTrading );

//			this.eligiblesSelectorForSignalingTickers =
//				new SP550andIndexes();

//			uncomment the following line for a (logbased) log based in sample chooser
//			eligiblesSelector = new DummyEligibleSelector();

			return eligiblesSelector;
		}
		
		protected override IInSampleChooser getInSampleChooser()
		{
			int numberOfBestTestingPositionsToBeReturned = 50;
			// uncomment the following line for a faster script
//			numberOfBestTestingPositionsToBeReturned = 20;
			numberOfBestTestingPositionsToBeReturned = 3;
			
			int numberOfTickersForTrading = 1;
			int[] numberOfTickersInEachSignalingPortfolio =
				new int[] { 1 , 1 , 1 };
			
			DecoderForLinearRegressionTestingPositions decoderForWeightedPositions =
				new DecoderForLinearRegressionTestingPositions(
					numberOfTickersForTrading , numberOfTickersInEachSignalingPortfolio );

//			double maxCorrelationAllowed = 0.96; copypasted
//			IReturnIntervalFilter returnIntervalFilterForTrading =
//				new ReturnIntervalFilterForDaysOfTheWeek(
//					new List<DayOfWeek>( new DayOfWeek[] {
//					                    	DayOfWeek.Tuesday , DayOfWeek.Wednesday ,
//					                    	DayOfWeek.Thursday , DayOfWeek.Friday } ) );
//			IReturnIntervalFilter returnIntervalFilterForSignaling =
//				new ReturnIntervalFilterForDaysOfTheWeek(
//					new List<DayOfWeek>( new DayOfWeek[] {
//					                    	DayOfWeek.Monday , DayOfWeek.Tuesday ,
//					                    	DayOfWeek.Wednesday , DayOfWeek.Thursday } ) );
			IReturnIntervalsBuilderForTradingAndForSignaling
				returnIntervalsBuilderForTradingAndForSignaling =
				new ReturnIntervalsBuilderForTradingAndForSignaling();
//					returnIntervalFilterForTrading ,
//					returnIntervalFilterForSignaling );
			
			this.fitnessEvaluator =
				new LinearRegressionFitnessEvaluator( new LinearRegressionSetupManager() );
			
			// parameters for the genetic optimizer
			double crossoverRate = 0.5;
			double mutationRate = 0.02;
//			double elitismRate = 0.00001;
			double elitismRate = 0.2;
			int populationSizeForGeneticOptimizer = 60000;
			int generationNumberForGeneticOptimizer = 15;
			
			// uncomment the followings line for a faster script
			populationSizeForGeneticOptimizer = 300;
			generationNumberForGeneticOptimizer = 2;
			
			int seedForRandomGeneratorForTheGeneticOptimizer =
				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator;
			int seedForRandomGeneratorForGenomeManagerForTradingTickers =
				seedForRandomGeneratorForTheGeneticOptimizer + 1;
			int seedForRandomGeneratorForGenomeManagerForSignalingTickers =
				seedForRandomGeneratorForTheGeneticOptimizer + 2;
			
//			seedForRandomGeneratorForGenomeManagerForTradingTickers += 1234;
//			
//			seedForRandomGeneratorForGenomeManagerForSignalingTickers += 97134;
			
			ArrayList currentGeneration = new GenerationWithoutDuplicatedFitness(
				populationSizeForGeneticOptimizer , new FitnessApproximator( 9 ) );
			ArrayList nextGeneration = new GenerationWithoutDuplicatedFitness(
				populationSizeForGeneticOptimizer , new FitnessApproximator( 9 ) );
			
			// comment the following two lines to use GenerationWithoutDuplicatedFitness
//			currentGeneration = new GenerationWithDuplicatedGenomes(
//				populationSizeForGeneticOptimizer );
//			nextGeneration = new GenerationWithDuplicatedGenomes(
//				populationSizeForGeneticOptimizer );
			
			IInSampleChooser inSampleChooser =
				new LinearRegressionGeneticChooser(
					numberOfBestTestingPositionsToBeReturned ,
					this.benchmark ,
					decoderForWeightedPositions ,
					this.returnIntervalSelectorForSignaling ,
					returnIntervalsBuilderForTradingAndForSignaling ,
					fitnessEvaluator ,
					this.historicalMarketValueProviderForInSample ,
					this.eligiblesSelector ,
					crossoverRate , mutationRate , elitismRate ,
					populationSizeForGeneticOptimizer ,
					generationNumberForGeneticOptimizer ,
					seedForRandomGeneratorForTheGeneticOptimizer ,
					seedForRandomGeneratorForGenomeManagerForTradingTickers ,
					seedForRandomGeneratorForGenomeManagerForSignalingTickers ,
					currentGeneration ,
					nextGeneration );
			
//			uncomment the following line for a (logbased) log based in sample chooser
//			inSampleChooser =
//				new PairsTradingChooserFromSavedBackTestLog(
//					@"C:\qpReports\pairsTrading\2008_05_08_23_49_18_pairsTrdng_from_2005_01_01_to_2008_04_30_annlRtrn_90.70_maxDD_5.43\2008_05_08_23_49_18_pairsTrdng_from_2005_01_01_to_2008_04_30_annlRtrn_90.70_maxDD_5.43.qpL",
//				  numberOfBestTestingPositionsToBeReturned);

			
			return inSampleChooser;
		}
		
		protected override IStrategyForBacktester getStrategyForBacktester()
		{
			int inSampleDays = 180;
//			inSampleDays = 360;
			// uncomment the following line for a faster script
//			inSampleDays = 20;
			inSampleDays = 60;
			
//			IIntervalsSelector intervalsSelectorForOutOfSample =
//				new OddIntervalsSelector( 1 , 1 , this.benchmark );
			Time timeForTrading = new Time( HistoricalEndOfDayTimer.GetMarketClose( DateTime.Now ) );
			List<DayOfWeek> acceptableDaysOfTheWeekForTheEndOfEachInterval =
				new List<DayOfWeek>(
					new DayOfWeek[] {
						DayOfWeek.Wednesday , DayOfWeek.Thursday , DayOfWeek.Friday } );
			TimeSpan maxTimeSpanToLookAhead = TimeSpan.FromDays( 20 );
			IIntervalsSelector intervalsSelectorForOutOfSample =
				new SingleDayIntervalsSelector(
					this.benchmark , timeForTrading ,
					acceptableDaysOfTheWeekForTheEndOfEachInterval ,
					maxTimeSpanToLookAhead );

//			IIntervalsSelector intervalsSelectorForInSample =
//				new OddIntervalsSelector( 1 , 1 , this.benchmark );
			IIntervalsSelector intervalsSelectorForInSample =
				new SingleDayIntervalsSelector(
					this.benchmark , timeForTrading ,
					acceptableDaysOfTheWeekForTheEndOfEachInterval ,
					maxTimeSpanToLookAhead );

			double minForecastedReturn = 0.002F;
			IEntryStrategy longAndShortBasedOnAverageExpectedReturn =
				new EntryStrategyBasedOnForecastedReturn(
					minForecastedReturn , this.fitnessEvaluator ,
					this.returnIntervalSelectorForSignaling ,
					this.historicalMarketValueProviderForChosingPositionsOutOfSample );
			
			IExitStrategy exitStrategy = new ExitOnIntervalEnd();

			IStrategyForBacktester strategyForBacktester =
				new LinearRegressionStrategy(
					7 , inSampleDays ,
					intervalsSelectorForInSample , intervalsSelectorForOutOfSample ,
					this.returnIntervalSelectorForSignaling ,
					eligiblesSelector ,		// eligible selector for trading tickers
//					this.eligiblesSelectorForSignalingTickers ,
					this.inSampleChooser ,
					this.historicalMarketValueProviderForInSample ,
					this.historicalMarketValueProviderForChosingPositionsOutOfSample ,
					longAndShortBasedOnAverageExpectedReturn , exitStrategy );

			return strategyForBacktester;
		}
		
		#region getEndOfDayStrategyBackTester
		private Timer getTimer(
			DateTime firstDateTime , DateTime lastDateTime)
		{
//			List< Time > dailyTimes = this.getDailyTimes();
//			IndexBasedHistoricalTimer indexBasedTimer =
//				new IndexBasedHistoricalTimer(
//					this.benchmark.Ticker ,
//					firstDateTime , lastDateTime , dailyTimes , 60 );
			IndexBasedEndOfDayTimer timer = new IndexBasedEndOfDayTimer(
				firstDateTime , this.benchmark.Ticker );
			return timer;
		}
		protected override EndOfDayStrategyBackTester
			getEndOfDayStrategyBackTester()
		{
			string backTestId = "LinearRegression";
			IAccountProvider accountProvider = new SimpleAccountProvider();
			double cashToStart = 30000;

			DateTime firstDateTime = new DateTime( 2001 , 1 , 1 );
			firstDateTime = new DateTime( 2003 , 1 , 1 );
			firstDateTime = new DateTime( 2004 , 1 , 1 );
			DateTime lastDateTime = new DateTime( 2005 , 1 , 1 );
			lastDateTime = new DateTime( 2006 , 1 , 1 );
			lastDateTime = new DateTime( 2004 , 1 , 7 );	// to test a single optimization with different seeds

			// uncomment the following two lines for a faster script
			firstDateTime = new DateTime( 2006 , 2 , 26 );
			lastDateTime = new DateTime( 2006 , 4 , 5 );

			double maxRunningHours = 2;
			
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
					backTestId ,
					this.getTimer( firstDateTime , lastDateTime ) ,
					this.strategyForBacktester ,
					this.historicalMarketValueProviderForTheBacktesterAccount ,
					accountProvider ,
					firstDateTime ,	lastDateTime ,
					this.benchmark , cashToStart , maxRunningHours );
			return endOfDayStrategyBackTester;
		}
		#endregion getEndOfDayStrategyBackTester
	}
}
