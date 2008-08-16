/*
QuantProject - Quantitative Finance Library

FixedLengthTwoPhasesMain.cs
Copyright (C) 2007
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
using QuantProject.Scripts.General;
using QuantProject.Scripts.General.Logging;
using QuantProject.Scripts.General.Reporting;


namespace QuantProject.Scripts.WalkForwardTesting.FixedLengthTwoPhases
{
	/// <summary>
	/// Entry point for the FixedLengthTwoPhases strategy. If any strategy
	/// parameter had to be changed, this is the place where it should
	/// be done
	/// </summary>
	public class FixedLengthTwoPhasesMain : BasicScriptForBacktesting
	{
		private Benchmark benchmark;
		private int numberOfPortfolioPositions;
		private IHistoricalQuoteProvider historicalQuoteProviderForInSample;
		private IHistoricalQuoteProvider
			historicalQuoteProviderForTheBacktesterAccount;

		public FixedLengthTwoPhasesMain()
		{
			this.benchmark = new Benchmark( "CCE" );
			this.numberOfPortfolioPositions = 1;
			this.historicalQuoteProviderForInSample =
				new HistoricalRawQuoteProvider();
			this.historicalQuoteProviderForTheBacktesterAccount =
				new HistoricalRawQuoteProvider();
		}
		
		protected override IEligiblesSelector getEligiblesSelector()
		{
			string tickersGroupId = "SP500";
			// uncomment the following line for a faster script
			tickersGroupId = "fastTest";
			
			int maxNumberOfEligibleTickersToBeChosen = 100;
			int maxNumberOfMostLiquidTickersToBeChosen =
				maxNumberOfEligibleTickersToBeChosen + 50;
			int numOfDaysForAverageOpenRawPriceComputation = 10;
			int numOfDaysForVolatilityComputation = 10;
			double minPrice = 20;
			double maxPrice = 75;

			IEligiblesSelector eligiblesSelector =
				new ByPriceMostLiquidAlwaysQuoted(
					tickersGroupId ,
					true ,
					maxNumberOfEligibleTickersToBeChosen ,
					numOfDaysForAverageOpenRawPriceComputation ,
					minPrice ,
					maxPrice );
			eligiblesSelector =
				new ByPriceMostLiquidLessVolatileOTCAlwaysQuoted(
					tickersGroupId ,
					true ,
					maxNumberOfEligibleTickersToBeChosen ,
					maxNumberOfMostLiquidTickersToBeChosen ,
					numOfDaysForAverageOpenRawPriceComputation ,
					numOfDaysForVolatilityComputation ,
					minPrice ,
					maxPrice );

//			uncomment the following line for a (logbased) log based in sample chooser
//			eligiblesSelector = new DummyEligibleSelector();

			return eligiblesSelector;
		}
		
		protected override IInSampleChooser getInSampleChooser()
		{
			int numberOfBestTestingPositionsToBeReturned = 20;
			// uncomment the following line for a faster script
			 numberOfBestTestingPositionsToBeReturned = 2;
			
			IDecoderForTestingPositions decoderForWeightedPositions =
				new DecoderForFLTPTestingPositionsWithBalancedWeights();

			// definition for the Fitness Evaluator
			IEquityEvaluator equityEvaluator = new SharpeRatio();
			IFitnessEvaluator	fitnessEvaluator =
				new FixedLengthTwoPhasesFitnessEvaluator(
					equityEvaluator );

			// parameters for the genetic optimizer
			double crossoverRate = 0.85;
			double mutationRate = 0.02;
			double elitismRate = 0.001;
			int populationSizeForGeneticOptimizer = 10000;
			int generationNumberForGeneticOptimizer = 1;
			int seedForRandomGenerator =
				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator;
			IInSampleChooser inSampleChooser =
				new FixedLengthTwoPhasesGeneticChooser(
					this.numberOfPortfolioPositions ,
					numberOfBestTestingPositionsToBeReturned ,
					benchmark ,
					decoderForWeightedPositions , fitnessEvaluator ,
					this.historicalQuoteProviderForInSample ,
					crossoverRate , mutationRate , elitismRate ,
					populationSizeForGeneticOptimizer , generationNumberForGeneticOptimizer ,
					seedForRandomGenerator );
//			inSampleChooser =
//				new PairsTradingBruteForceChooser(
//					numberOfBestTestingPositionsToBeReturned ,
//					decoderForWeightedPositions ,
//					fitnessEvaluator );

//			uncomment the following line for a (logbased) log based in sample chooser
//			inSampleChooser =
//				new PairsTradingChooserFromSavedBackTestLog(
//					@"C:\qpReports\pairsTrading\2008_05_08_23_49_18_pairsTrdng_from_2005_01_01_to_2008_04_30_annlRtrn_90.70_maxDD_5.43\2008_05_08_23_49_18_pairsTrdng_from_2005_01_01_to_2008_04_30_annlRtrn_90.70_maxDD_5.43.qpL",
//				  numberOfBestTestingPositionsToBeReturned);

			return inSampleChooser;
		}
		
		protected override IEndOfDayStrategyForBacktester getEndOfDayStrategy()
		{
//			int numberOfPortfolioPositions = 2;
			int numDaysForInSampleOptimization = 180;
			// uncomment the following line for a faster script
			numDaysForInSampleOptimization = 5;
			numDaysForInSampleOptimization = 45;
			
			int numDaysBetweenEachOtpimization = 3;
			
			IIntervalsSelector intervalsSelector =
				new FixedLengthTwoPhasesIntervalsSelector(
					1 , 1 , benchmark );

			RankBasedOutOfSampleChooser outOfSampleChooser =
				new RankBasedOutOfSampleChooser( 0 );

			FixedLengthTwoPhasesStrategy fixedLengthTwoPhasesStrategy =
				new FixedLengthTwoPhasesStrategy(
					this.numberOfPortfolioPositions ,
					numDaysBetweenEachOtpimization ,
					numDaysForInSampleOptimization ,
					benchmark , intervalsSelector ,
					eligiblesSelector , inSampleChooser ,
					this.historicalQuoteProviderForInSample ,
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
			return fixedLengthTwoPhasesStrategy;
		}
		
		protected override EndOfDayStrategyBackTester
			getEndOfDayStrategyBackTester()
		{
			string backTestId = "FixedLengthTwoPhases";
			IAccountProvider accountProvider = new SimpleAccountProvider();
			double cashToStart = 30000;

			DateTime firstDateTime = new DateTime( 2006 , 1 , 2 );
			DateTime lastDateTime = new DateTime( 2006 , 1 , 6 );

			double maxRunningHours = 1;
			
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
					backTestId , this.endOfDayStrategy ,
					this.historicalQuoteProviderForTheBacktesterAccount ,
					accountProvider ,
					firstDateTime ,	lastDateTime ,
					this.benchmark , cashToStart , maxRunningHours );
			return endOfDayStrategyBackTester;
		}

		
		protected override string getPathForTheMainFolderWhereScriptsResultsAreToBeSaved()
		{
			string pathForTheMainFolderWhereScriptsResultsAreToBeSaved =
				"C:\\qpReports\\fixedLengthTwoPhases\\";
			return pathForTheMainFolderWhereScriptsResultsAreToBeSaved;
		}
		
		protected override string getCustomSmallTextForFolderName()
		{
			return "fltp";
		}
		
		protected override string getFullPathFileNameForMain()
		{
			string fullPathFileNameForMain =
				@"C:\QuantProject\QuantProject\b7_Scripts\WalkForwardTesting\FixedLengthTwoPhases\FixedLengthTwoPhasesMain.cs";
			return fullPathFileNameForMain;
		}
	}
}
