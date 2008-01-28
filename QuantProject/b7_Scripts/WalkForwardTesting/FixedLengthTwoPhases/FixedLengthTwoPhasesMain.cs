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
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;
using QuantProject.Presentation;
using QuantProject.Presentation.Reporting.WindowsForm;


namespace QuantProject.Scripts.WalkForwardTesting.FixedLengthTwoPhases
{
	/// <summary>
	/// Entry point for the FixedLengthTwoPhases strategy. If any strategy
	/// parameter had to be changed, this is the place where it should
	/// be done
	/// </summary>
	public class FixedLengthTwoPhasesMain
	{
		public FixedLengthTwoPhasesMain()
		{
		}
		#region Run
		private MessageManager setMessageManager(
			IEligiblesSelector eligiblesSelector ,
			IInSampleChooser inSampleChooser ,
			IEndOfDayStrategy endOfDayStrategy ,
			EndOfDayStrategyBackTester endOfDayStrategyBackTester )
		{
			MessageManager messageManager =
				new MessageManager( "FixedLengthUpDown.Txt" );
			messageManager.Monitor( eligiblesSelector );
			messageManager.Monitor( inSampleChooser );
			//			messageManager.Monitor( endOfDayStrategy );
			messageManager.Monitor( endOfDayStrategyBackTester );
			return messageManager;
		}

		// TO DO check if you can add this to QuantProject.Presentation.Reporting.WindowsForm.Report
		// as a public method or as a new constructor
		private void showReport(
			DateTime lastDateTimeRequestedForTheScript ,
			EndOfDayStrategyBackTester endOfDayStrategyBackTester )
		{			
//			DateTime lastReportDateTime = ExtendedDateTime.Min(
//				lastDateTimeRequestedForTheScript ,
//				endOfDayStrategyBackTester.EndOfDayTimer.GetCurrentTime().DateTime );
			DateTime lastReportDateTime =
				endOfDayStrategyBackTester.ActualLastDateTime;
			Report report = new Report(
				endOfDayStrategyBackTester.AccountReport ,
				true );
			report.Create( endOfDayStrategyBackTester.DescriptionForLogFileName , 1 ,
				new EndOfDayDateTime( lastReportDateTime ,
				EndOfDaySpecificTime.OneHourAfterMarketClose ) ,
				endOfDayStrategyBackTester.Benchmark.Ticker );
			report.Show();
		}

		public void Run()
		{
			string backTestId = "WFFLTP";
			double cashToStart = 30000;

			int numberOfPortfolioPositions = 2;
			int inSampleDays = 90;
			string tickersGroupId = "SP500";
			
			// uncomment the following three lines for faster scripts
//			int numberOfPortfolioPositions = 2;
//			int inSampleDays = 30;
////			string tickersGroupId = "millo";
//			string tickersGroupId = "fastTest";

      Benchmark benchmark = new Benchmark( "MSFT" );
			int maxNumberOfEligiblesToBeChosen = 100;
			IDecoderForWeightedPositions decoderForWeightedPositions
				= new DecoderForBalancedWeightedPositions();
			IHistoricalQuoteProvider historicalQuoteProvider =
				new HistoricalAdjustedQuoteProvider();

			// definition for the Fitness Evaluator
      IEquityEvaluator equityEvaluator = new SharpeRatio();
			FixedLengthTwoPhasesFitnessEvaluator
				fixedLengthTwoPhasesFitnessEvaluator =
				new FixedLengthTwoPhasesFitnessEvaluator(	equityEvaluator );

			// parameters for the genetic optimizer
			double crossoverRate = 0.85;
			double mutationRate = 0.02;
			double elitismRate = 0.001;
			int populationSizeForGeneticOptimizer = 30000;
			int generationNumberForGeneticOptimizer = 8;
			int seedForRandomGenerator =
				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator;
			IInSampleChooser inSampleChooser =
				new FixedLengthTwoPhasesGeneticChooser(
				numberOfPortfolioPositions , inSampleDays , benchmark ,
				decoderForWeightedPositions , fixedLengthTwoPhasesFitnessEvaluator ,
				historicalQuoteProvider ,
				crossoverRate , mutationRate , elitismRate ,
				populationSizeForGeneticOptimizer , generationNumberForGeneticOptimizer ,
				seedForRandomGenerator );

			IIntervalsSelector intervalsSelector =
				new FixedLengthTwoPhasesIntervalsSelector(
					1 , 1 , benchmark );
			IEligiblesSelector eligiblesSelector =
				new MostLiquidAndLessVolatile(
				tickersGroupId , maxNumberOfEligiblesToBeChosen );

			FixedLengthTwoPhasesStrategy fixedLengthTwoPhasesStrategy =
				new FixedLengthTwoPhasesStrategy(
					numberOfPortfolioPositions ,
					7 , 90 , benchmark , intervalsSelector ,
					eligiblesSelector , inSampleChooser , historicalQuoteProvider );

			DateTime firstDateTime = new DateTime( 2001 , 1 , 2 );
			DateTime lastDateTime = new DateTime( 2004 , 12 , 31 );
			double maxRunningHours = 7;
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
					backTestId , fixedLengthTwoPhasesStrategy ,
					historicalQuoteProvider , firstDateTime ,
					lastDateTime , benchmark , cashToStart , maxRunningHours );

			// TO DO check if you can do this assign in the EndOfDayStrategyBackTester
			// constructor
			fixedLengthTwoPhasesStrategy.Account = endOfDayStrategyBackTester.Account;

			MessageManager messageManager = this.setMessageManager(
				eligiblesSelector , inSampleChooser ,
				fixedLengthTwoPhasesStrategy , endOfDayStrategyBackTester );
			endOfDayStrategyBackTester.Run();
			this.showReport( lastDateTime ,
				endOfDayStrategyBackTester );
		}

		#endregion Run
	}
}
