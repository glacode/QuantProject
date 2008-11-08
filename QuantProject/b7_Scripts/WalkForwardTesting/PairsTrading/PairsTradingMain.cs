/*
QuantProject - Quantitative Finance Library

PairsTradingMain.cs
Copyright (C) 2008
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

using QuantProject.ADT;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting.AccountProviding;
using QuantProject.Business.Strategies;
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
using QuantProject.Data.DataProviders.Bars.Caching;
using QuantProject.Presentation;
using QuantProject.Scripts.General;
using QuantProject.Scripts.General.Logging;
using QuantProject.Scripts.General.Reporting;


namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Entry point for the PairsTradingMain strategy. If any strategy
	/// parameter had to be changed, this is the place where it should
	/// be done
	/// </summary>
	public class PairsTradingMain : BasicScriptForBacktesting
	{
		private Benchmark benchmark;
		private HistoricalMarketValueProvider historicalMarketValueProviderForInSample;
		private HistoricalMarketValueProvider
			historicalMarketValueProviderForChosingPositionsOutOfSample;
		private HistoricalMarketValueProvider
			historicalMarketValueProviderForTheBacktesterAccount;

		
		public PairsTradingMain()
		{
			this.benchmark = new Benchmark( "CCE" );

			this.historicalMarketValueProviderForInSample =
				new HistoricalRawQuoteProvider();

			this.historicalMarketValueProviderForChosingPositionsOutOfSample =
				this.getHistoricalBarProvider();
//			this.historicalMarketValueProviderForChosingPositionsOutOfSample =
//				new HistoricalAdjustedQuoteProvider();
//			this.historicalQuoteProviderForChosingPositionsOutOfSample =
//				new HistoricalRawQuoteProvider();

			this.historicalMarketValueProviderForTheBacktesterAccount =
				this.historicalMarketValueProviderForChosingPositionsOutOfSample;
//			this.historicalMarketValueProviderForTheBacktesterAccount =
//				new HistoricalRawQuoteProvider();
//			this.historicalQuoteProviderForTheBacktesterAccount =
//				new HistoricalAdjustedQuoteProvider();

			// definition for the Fitness Evaluator
			//      IEquityEvaluator equityEvaluator = new SharpeRatio();
		}
		
		#region getHistoricalBarProvider
		
		#region getBarCache
		private DateTime[] getDailyTimes()
		{
			DateTime[] dailyTimes = {
				new DateTime( 1900 , 1 , 1 , 10 , 0 , 0 ) ,
				new DateTime( 1900 , 1 , 1 , 10 , 30 , 0 ) ,
				new DateTime( 1900 , 1 , 1 , 11 , 0 , 0 )			
			};
			return dailyTimes;
		}
		private IBarCache getBarCache()
		{
			DateTime[] dailyTimes = this.getDailyTimes();
			IBarCache barCache = new DailyBarCache( 60 , dailyTimes );
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

		protected override IEligiblesSelector getEligiblesSelector()
		{
			int maxNumberOfEligiblesToBeChosen = 100;
			
			string tickersGroupId = "SP500";
			// uncomment the following line for a faster script
//			tickersGroupId = "fastTest";

//			IEligiblesSelector eligiblesSelector =
//				new MostLiquidAndLessVolatile(
//				tickersGroupId ,
//				maxNumberOfEligiblesToBeChosen );
			IEligiblesSelector eligiblesSelector =
				new ByPriceMostLiquidAlwaysQuoted(
					tickersGroupId ,
					true ,
					maxNumberOfEligiblesToBeChosen ,
					10 , 20 , 75 );
			eligiblesSelector =
				new ByPriceMostLiquidLessVolatileOTCAlwaysQuoted(
					tickersGroupId ,
					true ,
					maxNumberOfEligiblesToBeChosen ,
					maxNumberOfEligiblesToBeChosen + 50 ,
					10 , 10 , 20 , 75 );


//			uncomment the following line for a (logbased) log based in sample chooser
//			eligiblesSelector = new DummyEligibleSelector();

			return eligiblesSelector;
		}

		protected override IInSampleChooser getInSampleChooser()
		{
			int numberOfBestTestingPositionsToBeReturned = 50;
			// uncomment the following line for a faster script
//			 numberOfBestTestingPositionsToBeReturned = 5;
			
			IDecoderForTestingPositions decoderForWeightedPositions =
				new DecoderForPairsTradingTestingPositionsWithBalancedWeights();

			double maxCorrelationAllowed = 0.96;
			IFitnessEvaluator fitnessEvaluator =
				new PairsTradingFitnessEvaluator( maxCorrelationAllowed );

			// parameters for the genetic optimizer
			double crossoverRate = 0.85;
			double mutationRate = 0.02;
			double elitismRate = 0.001;
			int populationSizeForGeneticOptimizer = 3000;
			int generationNumberForGeneticOptimizer = 4;
			int seedForRandomGenerator =
				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator;
			IInSampleChooser inSampleChooser =
				new PairsTradingGeneticChooser(
					numberOfBestTestingPositionsToBeReturned ,
					this.benchmark ,
					decoderForWeightedPositions , fitnessEvaluator ,
					this.historicalMarketValueProviderForInSample ,
					crossoverRate , mutationRate , elitismRate ,
					populationSizeForGeneticOptimizer ,
					generationNumberForGeneticOptimizer ,
					seedForRandomGenerator );
			
			inSampleChooser =
				new PairsTradingBruteForceChooser(
					numberOfBestTestingPositionsToBeReturned ,
					decoderForWeightedPositions ,
					fitnessEvaluator );


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
			// uncomment the following line for a faster script
//			inSampleDays = 5;
//			 inSampleDays = 60;
			
			IIntervalsSelector intervalsSelectorForOutOfSample =
				new OddIntervalsSelector( 1 , 1 , this.benchmark );
			// uncomment the following statement in order to test a CTO strategy (out of sample)
//			intervalsSelectorForOutOfSample =
//				new EvenIntervalsSelector( 1 , 1 , this.benchmark );
			IIntervalsSelector intervalsSelectorForInSample =
				new OddIntervalsSelector( 1 , 1 , this.benchmark );

			// uncomment the following two statements in order to use an
			// OTC-CTO in sample optimization (night is considered also)
//			intervalsSelectorForInSample =
//				new FixedLengthTwoPhasesIntervalsSelector( 1 , 1 , this.benchmark );
//			this.historicalQuoteProviderForInSample =
//				new HistoricalAdjustedQuoteProvider();

			OutOfSampleChooser outOfSampleChooser =
				new OutOfSampleChooserForSingleLongAndShort(
					0.006 , 0.02 , 0.006 , 0.02 );
//			outOfSampleChooser =
//				new OutOfSampleChooserForExactNumberOfBestLongPositions(
//				2 ,	0.006 , 0.99 , 0.006 , 0.99 );

			IStrategyForBacktester strategyForBacktester =
				new PairsTradingStrategy(
					7 , inSampleDays ,
					intervalsSelectorForInSample , intervalsSelectorForOutOfSample ,
					eligiblesSelector , inSampleChooser ,
					this.historicalMarketValueProviderForInSample ,
					this.historicalMarketValueProviderForChosingPositionsOutOfSample ,
					outOfSampleChooser );
//			IEndOfDayStrategyForBacktester endOfDayStrategyForBacktester =
//				new PairsTradingStrategy(
//				7 , inSampleDays , intervalsSelector ,
//				eligiblesSelector , inSampleChooser ,
//				this.historicalQuoteProviderForInSample ,
//				this.historicalQuoteProviderForChosingPositionsOutOfSample ,
//				0.006 , 0.99 , 0.006 , 0.99 );
//			endOfDayStrategyForBacktester =
//				new LongOnlyPairsTradingStrategy(
//				7 , inSampleDays , intervalsSelector ,
//				eligiblesSelector , inSampleChooser ,
//				this.historicalQuoteProviderForInSample ,
//				this.historicalQuoteProviderForChosingPositionsOutOfSample ,
//				0.006 , 0.02 , 0.006 , 0.02 );
			return strategyForBacktester;
		}
		protected override EndOfDayStrategyBackTester
			getEndOfDayStrategyBackTester()
		{
			string backTestId = "PairsTrading";
			IAccountProvider accountProvider = new SimpleAccountProvider();
			double cashToStart = 30000;

			DateTime firstDateTime = new DateTime( 2001 , 1 , 1 );
			firstDateTime = new DateTime( 2006 , 8 , 1 );
			DateTime lastDateTime = new DateTime( 2008 , 4 , 30 );

			// uncomment the following two lines for a faster script
			firstDateTime = new DateTime( 2002 , 1 , 1 );
			lastDateTime = new DateTime( 2007 , 6 , 30 );

			double maxRunningHours = 0.05;
			
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
					backTestId , this.strategyForBacktester ,
					this.historicalMarketValueProviderForTheBacktesterAccount ,
					accountProvider ,
					firstDateTime ,	lastDateTime ,
					this.benchmark , cashToStart , maxRunningHours );
			return endOfDayStrategyBackTester;
		}

		protected override string getPathForTheMainFolderWhereScriptsResultsAreToBeSaved()
		{
			string pathForTheMainFolderWhereScriptsResultsAreToBeSaved =
				"C:\\qpReports\\pairsTrading\\";
			return pathForTheMainFolderWhereScriptsResultsAreToBeSaved;
		}

		protected override string getCustomSmallTextForFolderName()
		{
			return "pairsTrdng";
		}

		protected override string getFullPathFileNameForMain()
		{
			string fullPathFileNameForMain =
				@"C:\QuantProject\QuantProject\b7_Scripts\WalkForwardTesting\PairsTrading\PairsTradingMain.cs";
			return fullPathFileNameForMain;
		}
	}
}
