/*
QuantProject - Quantitative Finance Library

OTCIntradayLogItem.cs
Copyright (C) 2009
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

using QuantProject.ADT;
using QuantProject.ADT.Timing;
using QuantProject.Data.DataProviders.Bars.Caching;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Financial.Accounting.AccountProviding;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Scripts.TickerSelectionTesting.OTC.OTC_Intraday;
using QuantProject.Business.Timing;
using QuantProject.Presentation;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.General.Reporting;
using QuantProject.Scripts.General.Logging;

namespace QuantProject.Scripts.TickerSelectionTesting.OTC.OTC_Intraday
{
	/// <summary>
	/// LogItem for the Open To Close strategy
	/// It is "intraday" because the run method
	/// uses intraday bars
	/// </summary>
	[Serializable]
	public class OTCIntradayLogItem : LogItem
	{
		protected DummyTesterForTestingPositions[]
			dummyTestersForBestTestingPositionsInSample;
		protected TestingPositions[] bestOTCPositionsInSample;
		protected int numberOfEligibleTickers;
		protected double fitness;
		protected int generation;
		protected string tickers;
		protected int numberOfInSampleDays;
		
		public TestingPositions[] BestOTCPositionsInSample
		{
			get
			{
				if ( this.bestOTCPositionsInSample == null )
					throw new Exception( "This property has not " +
					                    "been assigned yet! If you are loading the LogItem from " +
					                    "a log, this property was not set before logging the LogItem." );
				return this.bestOTCPositionsInSample;
			}
			set { this.bestOTCPositionsInSample = value; }
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
		public double Fitness
		{
			get
			{
				if ( this.fitness == double.MinValue )
					throw new Exception( "This property has not " +
					                    "been assigned yet! If you are loading the LogItem from " +
					                    "a log, this property was not set before logging the LogItem." );
				return this.fitness;
			}
			set { this.fitness = value; }
		}
		public int Generation
		{
			get{return this.generation;}
			set{this.generation = value;}
		}
		public string Tickers
		{
			get{return this.tickers;}
			set{this.tickers = value;}
		}
		public OTCIntradayLogItem(DateTime dateTime,
		                  				int numberOfInSampleDays)
			: base( dateTime )
		{
			this.numberOfInSampleDays = numberOfInSampleDays;
			this.numberOfEligibleTickers = int.MinValue;
			this.fitness = double.MinValue;
		}
		
		protected virtual void runStrategyClickEventHandler(object sender, System.EventArgs e)
		{
			//general
			DateTime firstDateTime = this.SimulatedCreationDateTime.AddDays(-this.numberOfInSampleDays);
			DateTime lastDateTime = this.SimulatedCreationDateTime;
			double maxRunningHours = 1;
			Benchmark benchmark = new Benchmark( "CCE" );
			int	numberOfPortfolioPositions = this.BestOTCPositionsInSample[0].WeightedPositions.Count;
			//cash and portfolio type
			double cashToStart = 30000;
			HistoricalMarketValueProvider historicalQuoteProviderForBackTester,
			historicalQuoteProviderForInSampleChooser,
			historicalQuoteProviderForStrategy;
			historicalQuoteProviderForBackTester =
//				new HistoricalBarProvider(
//					new SimpleBarCache( 60,
//					                    BarComponent.Open ) );
				
				new HistoricalRawQuoteProvider();
			historicalQuoteProviderForInSampleChooser = historicalQuoteProviderForBackTester;
			historicalQuoteProviderForStrategy = historicalQuoteProviderForInSampleChooser;
			IEligiblesSelector eligiblesSelector =	new DummyEligibleSelector();
			//strategyParameters
			TestingPositions[] positionsToTest = this.BestOTCPositionsInSample;
//		  	new PreviousPeriodsWereEfficientEntryCondition(1, historicalQuoteProviderForStrategy,
//		  	                                               numDaysForOscillatingPeriodForOutOfSample * 24 *60,
//		  	                                                new MarketDateTimeManager(benchmark, firstDateTime, lastDateTime, 60) );
			OTCIntradayStrategy strategy =
				new OTCIntradayStrategy(eligiblesSelector,
				                        positionsToTest, benchmark,
				                historicalQuoteProviderForInSampleChooser,
				                historicalQuoteProviderForBackTester,
				                Time.GetIntermediateTimes(new Time("09:30:00"),
				                                          new Time("16:00:00"), 1 ),
				                0.5, 0.5, PortfolioType.ShortAndLong, null, 2, 3, null, 100);
		
			QuantProject.Business.Timing.Timer timer = 
//				new QuantProject.Business.Timing.IndexBasedHistoricalTimer(
//						 benchmark.Ticker, firstDateTime , lastDateTime, 
//						  Time.GetIntermediateTimes(new Time("09:30:00"),
//				                                          new Time("16:00:00"), 1 ) ,
//						 60);
				new IndexBasedEndOfDayTimer( firstDateTime,
				                             lastDateTime.AddDays(10),
				                             benchmark.Ticker);
			
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
					"OTC" , timer	 , strategy, 
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
				new DummyTesterForTestingPositions[this.BestOTCPositionsInSample.Length];
			for ( int i = 0 ; i < BestOTCPositionsInSample.Length; i++ )
				this.showTestingPositionsClickEventHandler_setDummyTesters_setTester(
					i ,
					BestOTCPositionsInSample[ i ] ,
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
