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
using System.Collections.Generic;
using System.Windows.Forms;

using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Financial.Accounting.AccountProviding;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.EntryConditions;
using QuantProject.Business.Timing;
using QuantProject.Presentation;
using QuantProject.Scripts.General.Reporting;
using QuantProject.Scripts.General.Logging;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator
{
	/// <summary>
	/// LogItem for the PVO strategy
	/// portfolio value oscillator strategy
	/// </summary>
	[Serializable]
	public class PVOLogItem : LogItem
	{
		protected DummyTesterForTestingPositions[]
			dummyTestersForBestTestingPositionsInSample;
		protected TestingPositions[] bestPVOPositionsInSample;
		protected int numberOfEligibleTickers;
		protected double fitnessOfFirstPVOPositionsInSample;
		protected double fitnessOfLastPVOPositionsInSample;
		protected int generationOfFirstPVOPositionsInSample;
		protected int generationOfLastPVOPositionsInSample;
		protected string thresholdsOfFirst;
		protected string thresholdsOfLast;
		protected string tickersOfFirst;
		protected string tickersOfLast;
		protected int numberOfInSampleDays;
		
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

		public PVOLogItem(DateTime dateTime,
		                  int numberOfInSampleDays)
			: base( dateTime )
		{
			this.numberOfInSampleDays = numberOfInSampleDays;
			this.numberOfEligibleTickers = int.MinValue;
			this.fitnessOfFirstPVOPositionsInSample = double.MinValue;
			this.fitnessOfLastPVOPositionsInSample = double.MinValue;
		}
		
		protected virtual void runStrategyClickEventHandler(object sender, System.EventArgs e)
		{
			//general
			DateTime firstDateTime = this.SimulatedCreationDateTime.AddDays(-this.numberOfInSampleDays);
			DateTime lastDateTime = this.SimulatedCreationDateTime;
			double maxRunningHours = 1;
			Benchmark benchmark = new Benchmark( "^GSPC" );
			// definition for the Fitness Evaluator (for
			// objects that use it)
			int numDaysForOscillatingPeriodForChooser =
				((PVOPositions)this.BestPVOPositionsInSample[0]).NumOfDaysOrMinutesForOscillatingPeriod;
			int	numberOfPortfolioPositions =
				this.BestPVOPositionsInSample[0].WeightedPositions.Count;
			//cash and portfolio type
			double cashToStart = 30000;
			double maxAcceptableCloseToCloseDrawdown = 0.02;
			double minimumAcceptableGain = 0.007;
			HistoricalMarketValueProvider historicalQuoteProviderForBackTester,
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
				//new TestingPositions[1];
				//			int idx = PVOLogItem.rand.Next(bestPVOPositionsInSample.Length);
				//positionsToTest[0] = this.bestPVOPositionsInSample[0];
				positionsToTest = this.BestPVOPositionsInSample;
			List<IEntryCondition> entryConditions = new List<IEntryCondition>();
		  IEntryCondition entryCondition = 
		  	new AlwaysTrueEntryCondition();
//		  	new PreviousPeriodsWereEfficientEntryCondition(1, historicalQuoteProviderForStrategy,
//		  	                                               numDaysForOscillatingPeriodForOutOfSample * 24 *60,
//		  	                                               new MarketDateTimeManager(benchmark, firstDateTime, lastDateTime, 60) );
		  entryConditions.Add(entryCondition);
		  bool allEntryConditionsHaveToBeTrue = true;
			PVOStrategy strategy =
				new PVOStrategy(eligiblesSelector,
				                positionsToTest, this.numberOfInSampleDays,
				                numDaysForOscillatingPeriodForOutOfSample,
				                numberOfPortfolioPositions , benchmark ,
				                int.MaxValue ,
				                ((PVOPositions)positionsToTest[0]).OversoldThreshold,
				                ((PVOPositions)positionsToTest[0]).OverboughtThreshold,
				                0.0, 1.0, false, 1,
				                historicalQuoteProviderForStrategy ,
				                maxAcceptableCloseToCloseDrawdown , minimumAcceptableGain,
				               	entryConditions, allEntryConditionsHaveToBeTrue);
			
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
					"PVO" ,
					new QuantProject.Business.Timing.IndexBasedEndOfDayTimer(
						HistoricalEndOfDayTimer.GetMarketClose( firstDateTime ) ,
						benchmark.Ticker ) ,
					strategy ,
					historicalQuoteProviderForBackTester ,
					new SimpleAccountProvider(), firstDateTime ,
					lastDateTime , benchmark , cashToStart , maxRunningHours );
			
			endOfDayStrategyBackTester.Run();
			BackTesterReportViewer.ShowReport( lastDateTime ,
			                                  endOfDayStrategyBackTester );
		}
		
		private void showTestingPositionsClickEventHandler_setDummyTesters_setTester(
			int currentIndex ,
			TestingPositions testingPositions ,
			DateTime simulatedCreationDateTime )
		{
			this.dummyTestersForBestTestingPositionsInSample[ currentIndex ] =
				new DummyTesterForTestingPositions(
					testingPositions ,
					this.numberOfInSampleDays ,
					simulatedCreationDateTime );
		}
		
		private void showTestingPositionsClickEventHandler_setDummyTesters()
		{
			this.dummyTestersForBestTestingPositionsInSample =
				new DummyTesterForTestingPositions[this.BestPVOPositionsInSample.Length];
			for ( int i = 0 ; i < BestPVOPositionsInSample.Length; i++ )
				this.showTestingPositionsClickEventHandler_setDummyTesters_setTester(
					i ,
					BestPVOPositionsInSample[ i ] ,
					this.SimulatedCreationDateTime );
		}
		
		protected virtual void showTestingPositionsClickEventHandler(object sender, System.EventArgs e)
		{
			this.showTestingPositionsClickEventHandler_setDummyTesters();
			QuantProject.Presentation.ExecutablesListViewer executablesListViewer =
				new ExecutablesListViewer(
					this.dummyTestersForBestTestingPositionsInSample );
			executablesListViewer.Show();
		}
		
		protected virtual void createAndShowContextMenu()
		{
			MenuItem[] menuItems = new MenuItem[2];
			menuItems[0] = new MenuItem("Run Strategy");
			menuItems[1] = new MenuItem("Show TestingPositions");
			menuItems[0].Click +=
				new System.EventHandler(this.runStrategyClickEventHandler);
			menuItems[1].Click +=
				new System.EventHandler(this.showTestingPositionsClickEventHandler);
			ContextMenu contextMenu = new ContextMenu(menuItems);
			contextMenu.Show(Form.ActiveForm, Form.MousePosition);
		}
		
		public override void Run()
		{
			this.createAndShowContextMenu();
		}
	}
}
