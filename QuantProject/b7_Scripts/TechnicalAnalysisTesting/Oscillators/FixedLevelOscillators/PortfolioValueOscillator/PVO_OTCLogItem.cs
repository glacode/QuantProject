/*
QuantProject - Quantitative Finance Library

PVO_OTCLogItem.cs
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
using QuantProject.Business.Financial.Accounting.AccountProviding;
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
  /// LogItem for the PVO_OTC strategy
  /// portfolio value oscillator strategy
  /// </summary>
  [Serializable]
  public class PVO_OTCLogItem : PVOLogItem
  {
		
		public PVO_OTCLogItem(EndOfDayDateTime endOfDayDateTime,
  	                      int numberOfInSampleDays)
			: base( endOfDayDateTime , numberOfInSampleDays )
		{
			
		}
		
		protected override void runStrategyClickEventHandler(object sender, System.EventArgs e)
		{
			//general
			int inSampleDays = 120;
			DateTime firstDateTime = this.SimulatedCreationTime.DateTime.AddDays(-inSampleDays);
			DateTime lastDateTime = this.SimulatedCreationTime.DateTime;
			double maxRunningHours = 1;
			Benchmark benchmark = new Benchmark( "^GSPC" );
			// definition for the Fitness Evaluator (for
			// objects that use it)
			int	numberOfPortfolioPositions =
				this.BestPVOPositionsInSample[0].WeightedPositions.Count;
			//cash and portfolio type
			double cashToStart = 30000;
			
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
			TestingPositions[] positionsToTest =
				new TestingPositions[this.bestPVOPositionsInSample.Length];
			positionsToTest = this.bestPVOPositionsInSample;
			PVO_OTCStrategy strategy =
				new PVO_OTCStrategy(eligiblesSelector,
				positionsToTest, inSampleDays,
				benchmark ,
				int.MaxValue ,
				((PVOPositions)positionsToTest[0]).OversoldThreshold,
				((PVOPositions)positionsToTest[0]).OverboughtThreshold,
				historicalQuoteProviderForStrategy);
			
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
				"PVO_OTC" , strategy ,
				historicalQuoteProviderForBackTester , 
				new SimpleAccountProvider(),
				firstDateTime ,
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
