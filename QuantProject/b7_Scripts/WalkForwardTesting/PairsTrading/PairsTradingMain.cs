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
using QuantProject.Scripts.General.Logging;
using QuantProject.Scripts.General.Reporting;


namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Entry point for the PairsTradingMain strategy. If any strategy
	/// parameter had to be changed, this is the place where it should
	/// be done
	/// </summary>
	public class PairsTradingMain
	{
		public PairsTradingMain()
		{
		}
		#region Run
		private MessageManager setMessageManager(
			IEligiblesSelector eligiblesSelector ,
			IInSampleChooser inSampleChooser ,
			IEndOfDayStrategyForBacktester endOfDayStrategy ,
			EndOfDayStrategyBackTester endOfDayStrategyBackTester )
		{
			string dateStamp =
				ExtendedDateTime.GetCompleteShortDescriptionForFileName( DateTime.Now );
			MessageManager messageManager =
				new MessageManager( "NotificationMessagesForCurrentStrategy_" +
				dateStamp + ".Txt" );
			messageManager.Monitor( eligiblesSelector );
			messageManager.Monitor( inSampleChooser );
			messageManager.Monitor( endOfDayStrategy );
			messageManager.Monitor( endOfDayStrategyBackTester );
			return messageManager;
		}

		private void saveLog( BackTestLog backTestLog ,
		                    string suggestedLogFileName )
		{
			string defaultFolderPath =
				"C:\\qpReports\\";
//			this.wFLagLog.TransactionHistory = this.account.Transactions;
			LogArchiver.Save( backTestLog ,
			              suggestedLogFileName , defaultFolderPath );
		}



		public void Run1()
		{
			BackTestLog backTestLog = LogArchiver.Load( "C:\\qpReports\\" );
			LogViewer logViewer =
				new LogViewer( backTestLog );
			logViewer.Show();
		}

		public void Run()
		{
			string backTestId = "PairsTrading";
			double cashToStart = 30000;

			int inSampleDays = 90;
			string tickersGroupId = "SP500";
			
			// uncomment the following two lines for faster scripts
//			int inSampleDays = 30;
//			string tickersGroupId = "fastTest";

      Benchmark benchmark = new Benchmark( "MSFT" );
			int maxNumberOfEligiblesToBeChosen = 100;
			IDecoderForTestingPositions decoderForWeightedPositions
				= new DecoderForTestingPositionsWithBalancedWeights();
			IHistoricalQuoteProvider historicalQuoteProvider =
				new HistoricalAdjustedQuoteProvider();

			// definition for the Fitness Evaluator
//      IEquityEvaluator equityEvaluator = new SharpeRatio();
			IFitnessEvaluator	fitnessEvaluator =
				new PairsTradingFitnessEvaluator();

			// parameters for the genetic optimizer
			double crossoverRate = 0.85;
			double mutationRate = 0.02;
			double elitismRate = 0.001;
			int populationSizeForGeneticOptimizer = 20000;
			int generationNumberForGeneticOptimizer = 8;
			int seedForRandomGenerator =
				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator;
			IInSampleChooser inSampleChooser =
				new PairsTradingGeneticChooser(
				10 ,
				benchmark ,
				decoderForWeightedPositions , fitnessEvaluator ,
				historicalQuoteProvider ,
				crossoverRate , mutationRate , elitismRate ,
				populationSizeForGeneticOptimizer , generationNumberForGeneticOptimizer ,
				seedForRandomGenerator );

//			IIntervalsSelector intervalsSelector =
//				new FixedLengthTwoPhasesIntervalsSelector(
//				1 , 1 , benchmark );
			IIntervalsSelector intervalsSelector =
				new OddIntervalsSelector( 1 , 1 , benchmark );
			IEligiblesSelector eligiblesSelector =
				new MostLiquidAndLessVolatile(
				tickersGroupId , maxNumberOfEligiblesToBeChosen );

			PairsTradingStrategy pairsTradingStrategy =
				new PairsTradingStrategy(
				7 , inSampleDays , intervalsSelector ,
				eligiblesSelector , inSampleChooser , historicalQuoteProvider ,
				0.003 , 0.99 , 0.003 , 0.99 );

			DateTime firstDateTime = new DateTime( 2002 , 1 , 29 );
			DateTime lastDateTime = new DateTime( 2002 , 2 , 28 );
			double maxRunningHours = 8;
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
					backTestId , pairsTradingStrategy ,
					historicalQuoteProvider , firstDateTime ,
					lastDateTime , benchmark , cashToStart , maxRunningHours );

			// TO DO check if you can do this assign in the EndOfDayStrategyBackTester
			// constructor
			pairsTradingStrategy.Account = endOfDayStrategyBackTester.Account;

			MessageManager messageManager = this.setMessageManager(
				eligiblesSelector , inSampleChooser ,
				pairsTradingStrategy , endOfDayStrategyBackTester );
			endOfDayStrategyBackTester.Run();
			BackTesterReportViewer.ShowReport( lastDateTime ,
				endOfDayStrategyBackTester );
			this.saveLog(
				endOfDayStrategyBackTester.Log ,
				endOfDayStrategyBackTester.Description );
		}

		#endregion Run
	}
}
