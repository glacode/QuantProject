/*
QuantProject - Quantitative Finance Library

FixedLengthTwoPhasesLogItem.cs
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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;
using QuantProject.Scripts.General.Reporting;

namespace QuantProject.Scripts.WalkForwardTesting.FixedLengthTwoPhases
{
	/// <summary>
	/// Log item for the FixedLengthTwoPhases strategy
	/// </summary>
	[Serializable]
	public class FixedLengthTwoPhasesLogItem : LogItem
	{
		private WeightedPositions bestWeightedPositionsInSample;
		private int numberOfEligibleTickers;
		
		public WeightedPositions BestWeightedPositionsInSample
		{
			get
			{
				if ( this.bestWeightedPositionsInSample == null )
					throw new Exception( "This property has not " +
						"been assigned yet! If you are loading the LogItem from " +
						"a log, this property was not set before logging the LogItem." );
				return this.bestWeightedPositionsInSample;
			}
			set { this.bestWeightedPositionsInSample = value; }
		}

		public int NumberOfEligibleTickers
		{
			get
			{
				if ( this.numberOfEligibleTickers == int.MinValue )
					throw new Exception( "This property has not " +
						"been assigned yet! If you are loading the LogItem from " +
						"a log, this property was not set before logging the LogItem." );
				return this.numberOfEligibleTickers;
			}
			set { this.numberOfEligibleTickers = value; }
		}

		public FixedLengthTwoPhasesLogItem( EndOfDayDateTime endOfDayDateTime )
			: base( endOfDayDateTime )
		{
			this.numberOfEligibleTickers = int.MinValue;
		}
		public override void Run()
		{
			string backTestId = "SimpleFLTP";
			double cashToStart = 30000;

			Benchmark benchmark = new Benchmark( "MSFT" );

//			int maxNumberOfEligiblesToBeChosen = 100;
//			IDecoderForWeightedPositions decoderForWeightedPositions
//				= new DecoderForBalancedWeightedPositions();
			IHistoricalQuoteProvider historicalQuoteProvider =
				new HistoricalAdjustedQuoteProvider();

//			// definition for the Fitness Evaluator
//			IEquityEvaluator equityEvaluator = new SharpeRatio();
//			FixedLengthTwoPhasesFitnessEvaluator
//				fixedLengthTwoPhasesFitnessEvaluator =
//				new FixedLengthTwoPhasesFitnessEvaluator(	equityEvaluator );

			// parameters for the genetic optimizer
//			double crossoverRate = 0.85;
//			double mutationRate = 0.02;
//			double elitismRate = 0.001;
//			int populationSizeForGeneticOptimizer = 3000;
//			int generationNumberForGeneticOptimizer = 5;
//			int seedForRandomGenerator =
//				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator;
//			IInSampleChooser inSampleChooser =
//				new FixedLengthTwoPhasesGeneticChooser(
//				numberOfPortfolioPositions , inSampleDays , benchmark ,
//				decoderForWeightedPositions , fixedLengthTwoPhasesFitnessEvaluator ,
//				historicalQuoteProvider ,
//				crossoverRate , mutationRate , elitismRate ,
//				populationSizeForGeneticOptimizer , generationNumberForGeneticOptimizer ,
//				seedForRandomGenerator );
			IInSampleChooser inSampleChooser =
				new ConstantWeightedPositionsChooser( this.BestWeightedPositionsInSample );

			IIntervalsSelector intervalsSelector =
				new FixedLengthTwoPhasesIntervalsSelector(
				1 , 1 , benchmark );
			IEligiblesSelector eligiblesSelector = new DummyEligibleSelector();

			FixedLengthTwoPhasesStrategy fixedLengthTwoPhasesStrategy =
				new FixedLengthTwoPhasesStrategy(
				this.BestWeightedPositionsInSample.Count ,
				9999 , 9 , benchmark , intervalsSelector ,
				eligiblesSelector , inSampleChooser , historicalQuoteProvider );

			DateTime firstDateTime = this.simulatedCreationTime.DateTime.AddDays( -90 );
			DateTime lastDateTime = this.simulatedCreationTime.DateTime;
			double maxRunningHours = 0.3;
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
				backTestId , fixedLengthTwoPhasesStrategy ,
				historicalQuoteProvider , firstDateTime ,
				lastDateTime , benchmark , cashToStart , maxRunningHours );

			// TO DO check if you can do this assign in the EndOfDayStrategyBackTester
			// constructor
			fixedLengthTwoPhasesStrategy.Account = endOfDayStrategyBackTester.Account;

//			MessageManager messageManager = this.setMessageManager(
//				eligiblesSelector , inSampleChooser ,
//				fixedLengthTwoPhasesStrategy , endOfDayStrategyBackTester );
			endOfDayStrategyBackTester.Run();
			BackTesterReportViewer.ShowReport( lastDateTime ,
				endOfDayStrategyBackTester );
//			this.saveLog(
//				endOfDayStrategyBackTester.Log ,
//				endOfDayStrategyBackTester.DescriptionForLogFileName );
		}
	}
}
