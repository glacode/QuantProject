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
		private IHistoricalQuoteProvider historicalQuoteProviderForInSample;
		private IHistoricalQuoteProvider
			historicalQuoteProviderForChosingPositionsOutOfSample;
		private IHistoricalQuoteProvider
			historicalQuoteProviderForTheBacktesterAccount;

		
		public PairsTradingMain()
		{
			this.benchmark = new Benchmark( "BMC" );

			this.historicalQuoteProviderForInSample =
				new HistoricalRawQuoteProvider();
			this.historicalQuoteProviderForChosingPositionsOutOfSample =
				new HistoricalAdjustedQuoteProvider();
			this.historicalQuoteProviderForTheBacktesterAccount =
				this.historicalQuoteProviderForInSample;

			// definition for the Fitness Evaluator
			//      IEquityEvaluator equityEvaluator = new SharpeRatio();
		}

		protected override void setEligiblesSelector()
		{
			int maxNumberOfEligiblesToBeChosen = 100;
			
			string tickersGroupId = "SP500";
			// uncomment the following line for a faster script
//			tickersGroupId = "fastTest";

			this.eligiblesSelector =
				new MostLiquidAndLessVolatile(
				tickersGroupId ,
				maxNumberOfEligiblesToBeChosen );
			this.eligiblesSelector =
				new ByPriceMostLiquidAlwaysQuoted(
					tickersGroupId ,
					true ,
					maxNumberOfEligiblesToBeChosen ,
					10 , 30 , 99999 );					
		}

		protected override void setInSampleChooser()
		{
			int numberOfBestTestingPositionsToBeReturned = 10;
			
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
			this.inSampleChooser =
				new PairsTradingGeneticChooser(
				numberOfBestTestingPositionsToBeReturned ,
				this.benchmark ,
				decoderForWeightedPositions , fitnessEvaluator ,
				this.historicalQuoteProviderForInSample ,
				crossoverRate , mutationRate , elitismRate ,
				populationSizeForGeneticOptimizer ,
				generationNumberForGeneticOptimizer ,
				seedForRandomGenerator );
			
			this.inSampleChooser =
				new PairsTradingBruteForceChooser(
					numberOfBestTestingPositionsToBeReturned ,
					decoderForWeightedPositions ,
					fitnessEvaluator );
		}

		protected override void setEndOfDayStrategy()
		{
			int inSampleDays = 180;
			// uncomment the following line for a faster script
//			inSampleDays = 5; // inSampleDays = 60;
			
			IIntervalsSelector intervalsSelector =
				new OddIntervalsSelector( 1 , 1 , this.benchmark );

			this.endOfDayStrategy =
				new PairsTradingStrategy(
				7 , inSampleDays , intervalsSelector ,
				eligiblesSelector , inSampleChooser ,
				this.historicalQuoteProviderForInSample ,
				this.historicalQuoteProviderForChosingPositionsOutOfSample ,
				0.005 , 0.99 , 0.005 , 0.99 );
		}
		protected override void setEndOfDayStrategyBackTester()
		{
			string backTestId = "PairsTrading";
			IAccountProvider accountProvider = new SimpleAccountProvider();
			double cashToStart = 30000;

			DateTime firstDateTime = new DateTime( 2001 , 1 , 1 );
			DateTime lastDateTime = new DateTime( 2004 , 12 , 31 );
			double maxRunningHours = 8;
			
			this.endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
				backTestId , this.endOfDayStrategy ,
				this.historicalQuoteProviderForTheBacktesterAccount ,
				accountProvider ,
				firstDateTime ,	lastDateTime ,
				this.benchmark , cashToStart , maxRunningHours );
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
