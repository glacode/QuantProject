/*
QuantProject - Quantitative Finance Library

PVOLogItem.cs
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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;
using QuantProject.Scripts.General.Reporting;



namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator
{
  /// <summary>
  /// LogItem for the PVO strategy
  /// portfolio value oscillator strategy
  /// </summary>
  [Serializable]
  public class PVOLogItem : LogItem
  {
//		static public Random rand = new Random(4676);
		private TestingPositions[] bestPVOPositionsInSample;
		private int numberOfEligibleTickers;
		private double fitnessOfFirstPVOPositionsInSample;
		private double fitnessOfLastPVOPositionsInSample;
		private int generationOfFirstPVOPositionsInSample;
		private int generationOfLastPVOPositionsInSample;
		private string thresholdsOfFirst;
		private string thresholdsOfLast;
		private string tickersOfFirst;
		private string tickersOfLast;
		
		public TestingPositions[] BestPVOPositionsInSample
		{
			get
			{
				if ( this.bestPVOPositionsInSample == null )
					throw new Exception( "This property has not " +
						"been assigned yet! If you are loading the LogItem from " +
						"a log, this property was not set before logging the LogItem." );
				return this.bestPVOPositionsInSample;
			}
			set { this.bestPVOPositionsInSample = value; }
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
		public double FitnessOfFirst
		{
			get
			{
				if ( this.fitnessOfFirstPVOPositionsInSample == double.MinValue )
					throw new Exception( "This property has not " +
						"been assigned yet! If you are loading the LogItem from " +
						"a log, this property was not set before logging the LogItem." );
				return this.fitnessOfFirstPVOPositionsInSample;
			}
			set { this.fitnessOfFirstPVOPositionsInSample = value; }
		}
		public double FitnessOfLast
		{
			get
			{
				if ( this.fitnessOfLastPVOPositionsInSample == double.MinValue )
					throw new Exception( "This property has not " +
						"been assigned yet! If you are loading the LogItem from " +
						"a log, this property was not set before logging the LogItem." );
				return this.fitnessOfLastPVOPositionsInSample;
			}
			set { this.fitnessOfLastPVOPositionsInSample = value; }
		}
		public int GenerationOfFirst
		{
			get{return this.generationOfFirstPVOPositionsInSample;}
			set{this.generationOfFirstPVOPositionsInSample = value;}
		}
		public int GenerationOfLast
		{
			get{return this.generationOfLastPVOPositionsInSample;}
			set{this.generationOfLastPVOPositionsInSample = value;}
		}
		public string ThresholdsOfFirst
		{
			get{return this.thresholdsOfFirst;}
			set{this.thresholdsOfFirst = value;}
		}
		public string ThresholdsOfLast
		{
			get{return this.thresholdsOfLast;}
			set{this.thresholdsOfLast = value;}
		}
		public string TickersOfFirst
		{
			get{return this.tickersOfFirst;}
			set{this.tickersOfFirst = value;}
		}
		public string TickersOfLast
		{
			get{return this.tickersOfLast;}
			set{this.tickersOfLast = value;}
		}

		public PVOLogItem(EndOfDayDateTime endOfDayDateTime )
			: base( endOfDayDateTime )
		{
			this.numberOfEligibleTickers = int.MinValue;
			this.fitnessOfFirstPVOPositionsInSample = double.MinValue;
			this.fitnessOfLastPVOPositionsInSample = double.MinValue;
		}
		
		public override void Run()
		{
			//general
			int inSampleDays = 120;
			DateTime firstDateTime = this.SimulatedCreationTime.DateTime.AddDays(-inSampleDays);
			DateTime lastDateTime = this.SimulatedCreationTime.DateTime;
			double maxRunningHours = 1;
			Benchmark benchmark = new Benchmark( "^GSPC" );
			// definition for the Fitness Evaluator (for
			// objects that use it)
			int numDaysForOscillatingPeriodForChooser =
				((PVOPositions)this.BestPVOPositionsInSample[0]).NumDaysForOscillatingPeriod;
			int	numberOfPortfolioPositions =
				this.BestPVOPositionsInSample[0].WeightedPositions.Count;
			//cash and portfolio type
			double cashToStart = 30000;
			double maxAcceptableCloseToCloseDrawdown = 0.02;
			double minimumAcceptableGain = 0.007;
			HistoricalQuoteProvider historicalQuoteProviderForBackTester,
				historicalQuoteProviderForInSampleChooser,
				historicalQuoteProviderForStrategy;
			historicalQuoteProviderForBackTester =
				new HistoricalAdjustedQuoteProvider();
			historicalQuoteProviderForInSampleChooser = historicalQuoteProviderForBackTester;
			historicalQuoteProviderForStrategy = historicalQuoteProviderForInSampleChooser;
						
			IEligiblesSelector eligiblesSelector =
				new DummyEligibleSelector();
			//strategyParameters
			int numDaysForOscillatingPeriodForOutOfSample =
				numDaysForOscillatingPeriodForChooser;
			TestingPositions[] positionsToTest =
				new TestingPositions[1];
//			int idx = PVOLogItem.rand.Next(bestPVOPositionsInSample.Length);
			positionsToTest[0] = this.bestPVOPositionsInSample[0];
			PVOStrategy strategy =
				new PVOStrategy(eligiblesSelector,
				positionsToTest, inSampleDays,
				numDaysForOscillatingPeriodForOutOfSample,
				numberOfPortfolioPositions , benchmark ,
				int.MaxValue ,
				historicalQuoteProviderForStrategy ,
				maxAcceptableCloseToCloseDrawdown , minimumAcceptableGain );
			
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
				"PVO" , strategy ,
				historicalQuoteProviderForBackTester , firstDateTime ,
				lastDateTime , benchmark , cashToStart , maxRunningHours );

			// TO DO check if you can do this assign in the EndOfDayStrategyBackTester
			// constructor
			strategy.Account = endOfDayStrategyBackTester.Account;
			endOfDayStrategyBackTester.Run();
			BackTesterReportViewer.ShowReport( lastDateTime ,
				endOfDayStrategyBackTester );
		}
	}
}
