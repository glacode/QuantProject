/*
QuantProject - Quantitative Finance Library

DrivenBySharpeRatioLogItem.cs
Copyright (C) 2011
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

namespace QuantProject.Scripts.TickerSelectionTesting.DrivenBySharpeRatio
{
	/// <summary>
	/// LogItem for the Driven Sharpe Ratio strategy
	/// </summary>
	[Serializable]
	public class DrivenBySharpeRatioLogItem : LogItem
	{
		protected BuyAndHoldTesterForTestingPositions[]
			buyAndHoldTestersForBestTestingPositionsInSample;
		protected TestingPositions[] bestPositionsInSample;
		protected int numberOfEligibleTickers;
		protected double fitness;
		protected int generation;
		protected string tickers;
		protected int inSamplePeriodLengthInDays;
		protected Benchmark benchmark;
		protected HistoricalMarketValueProvider historicalMarketValueProvider;		
		
		public TestingPositions[] BestPositionsInSample
		{
			get
			{
				if ( this.bestPositionsInSample == null )
					throw new Exception( "This property has not " +
					                    "been assigned yet! If you are loading the LogItem from " +
					                    "a log, this property was not set before logging the LogItem." );
				return this.bestPositionsInSample;
			}
			set { this.bestPositionsInSample = value; }
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
		public DrivenBySharpeRatioLogItem(DateTime dateTime,
		                  				 			  int inSamplePeriodLengthInDays,
		                  				 			  Benchmark benchmark,
		                  				 			  HistoricalMarketValueProvider historicalMarketValueProvider)
			: base( dateTime )
		{
			this.inSamplePeriodLengthInDays = inSamplePeriodLengthInDays;
			this.numberOfEligibleTickers = int.MinValue;
			this.fitness = double.MinValue;
			this.benchmark = benchmark;
			this.historicalMarketValueProvider = historicalMarketValueProvider;
		}
		
		#region runStrategyClickEventHandlerOnlyInSample
		protected virtual void runStrategyClickEventHandlerOnlyInSample(object sender, System.EventArgs e)
		{
			DateTime firstDateTime = this.SimulatedCreationDateTime.AddDays(-this.inSamplePeriodLengthInDays);
			DateTime lastDateTime = this.SimulatedCreationDateTime;
			double maxRunningHours = 1;
			int	numberOfPortfolioPositions = this.bestPositionsInSample[0].WeightedPositions.Count;
			//cash and portfolio type
			double cashToStart = 10000;
			HistoricalMarketValueProvider historicalQuoteProviderForBackTester,
				historicalQuoteProviderForInSampleChooser,
				historicalQuoteProviderForStrategy;
			historicalQuoteProviderForBackTester = this.historicalMarketValueProvider;
			historicalQuoteProviderForInSampleChooser = historicalQuoteProviderForBackTester;
			historicalQuoteProviderForStrategy = historicalQuoteProviderForInSampleChooser;
			//strategyParameters
			TestingPositions[] positionsToTest = this.BestPositionsInSample;
			
			DrivenBySharpeRatioStrategy strategy =
					new DrivenBySharpeRatioStrategy(positionsToTest,  
				         this.inSamplePeriodLengthInDays, this.benchmark,
				         historicalQuoteProviderForBackTester,
				         PortfolioType.OnlyLong);
		
			QuantProject.Business.Timing.Timer timer = 
				new IndexBasedEndOfDayTimer( firstDateTime,
				                             lastDateTime,
				                             this.benchmark.Ticker);
			
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
					"BuyAndHold" , timer	 , strategy, 
					historicalQuoteProviderForBackTester ,
					new SimpleAccountProvider(), firstDateTime ,
					lastDateTime ,
					this.benchmark , cashToStart , maxRunningHours );
			
			endOfDayStrategyBackTester.Run();
			BackTesterReportViewer.ShowReport( lastDateTime ,
			                                  endOfDayStrategyBackTester );
		}
		#endregion runStrategyClickEventHandlerOnlyInSample
		
		#region runStrategyClickEventHandlerOnlyOutOfSample
		protected virtual void runStrategyClickEventHandlerOnlyOutOfSample(object sender, System.EventArgs e)
		{
			DateTime firstDateTime = this.SimulatedCreationDateTime;
			DateTime lastDateTime = this.SimulatedCreationDateTime.AddDays(this.inSamplePeriodLengthInDays);
			double maxRunningHours = 1;
			int	numberOfPortfolioPositions = this.bestPositionsInSample[0].WeightedPositions.Count;
			//cash and portfolio type
			double cashToStart = 10000;
			HistoricalMarketValueProvider historicalQuoteProviderForBackTester,
				historicalQuoteProviderForInSampleChooser,
				historicalQuoteProviderForStrategy;
			historicalQuoteProviderForBackTester = this.historicalMarketValueProvider;
			historicalQuoteProviderForInSampleChooser = historicalQuoteProviderForBackTester;
			historicalQuoteProviderForStrategy = historicalQuoteProviderForInSampleChooser;
			//strategyParameters
			TestingPositions[] positionsToTest = this.BestPositionsInSample;
			
			DrivenBySharpeRatioStrategy strategy =
					new DrivenBySharpeRatioStrategy(positionsToTest,  
				         this.inSamplePeriodLengthInDays, this.benchmark,
				         historicalQuoteProviderForBackTester,
				         PortfolioType.OnlyLong);
		
			QuantProject.Business.Timing.Timer timer = 
				new IndexBasedEndOfDayTimer( firstDateTime,
				                             lastDateTime,
				                             this.benchmark.Ticker);
			
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
					"BuyAndHold" , timer	 , strategy, 
					historicalQuoteProviderForBackTester ,
					new SimpleAccountProvider(), firstDateTime ,
					lastDateTime ,
					this.benchmark , cashToStart , maxRunningHours );
			
			endOfDayStrategyBackTester.Run();
			BackTesterReportViewer.ShowReport( lastDateTime ,
			                                  endOfDayStrategyBackTester );
			                               
		}
		#endregion runStrategyClickEventHandlerOnlyOutOfSample
		
		#region runStrategyClickEventHandlerInAndOutOfSample
		protected virtual void runStrategyClickEventHandlerInAndOutOfSample(object sender, System.EventArgs e)
		{
			DateTime firstDateTime = this.SimulatedCreationDateTime.AddDays(-this.inSamplePeriodLengthInDays);
			DateTime lastDateTime = this.SimulatedCreationDateTime;
			double maxRunningHours = 1;
			int	numberOfPortfolioPositions = this.bestPositionsInSample[0].WeightedPositions.Count;
			//cash and portfolio type
			double cashToStart = 10000;
			HistoricalMarketValueProvider historicalQuoteProviderForBackTester,
				historicalQuoteProviderForInSampleChooser,
				historicalQuoteProviderForStrategy;
			historicalQuoteProviderForBackTester = this.historicalMarketValueProvider;
			historicalQuoteProviderForInSampleChooser = historicalQuoteProviderForBackTester;
			historicalQuoteProviderForStrategy = historicalQuoteProviderForInSampleChooser;
			//strategyParameters
			TestingPositions[] positionsToTest = this.BestPositionsInSample;
			
			DrivenBySharpeRatioStrategy strategy =
					new DrivenBySharpeRatioStrategy(positionsToTest,  
				         this.inSamplePeriodLengthInDays, this.benchmark,
				         historicalQuoteProviderForBackTester,
				         PortfolioType.OnlyLong);
		
			QuantProject.Business.Timing.Timer timer = 
				new IndexBasedEndOfDayTimer( firstDateTime,
				                             lastDateTime.AddDays(this.inSamplePeriodLengthInDays),
				                             this.benchmark.Ticker);
			
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
					"BuyAndHold" , timer	 , strategy, 
					historicalQuoteProviderForBackTester ,
					new SimpleAccountProvider(), firstDateTime ,
					lastDateTime.AddDays(this.inSamplePeriodLengthInDays) ,
					this.benchmark , cashToStart , maxRunningHours );
			
			endOfDayStrategyBackTester.Run();
			BackTesterReportViewer.ShowReport( lastDateTime.AddDays(this.inSamplePeriodLengthInDays) ,
			                                  endOfDayStrategyBackTester );
			                               
		}
		#endregion runStrategyClickEventHandlerInAndOutOfSample
		
		private void showTestingPositionsClickEventHandler_setTesters_setTester(
			int currentIndex ,
			TestingPositions testingPositions ,
			DateTime simulatedCreationDateTime )
		{
			this.buyAndHoldTestersForBestTestingPositionsInSample[ currentIndex ] =
				new BuyAndHoldTesterForTestingPositions(this.historicalMarketValueProvider,
					this.benchmark, testingPositions ,
					this.inSamplePeriodLengthInDays ,
					simulatedCreationDateTime );
		}
		
		private void showTestingPositionsClickEventHandler_setTesters()
		{
			this.buyAndHoldTestersForBestTestingPositionsInSample =
				new BuyAndHoldTesterForTestingPositions[this.BestPositionsInSample.Length];
			for ( int i = 0 ;
			      i < BestPositionsInSample.Length && this.BestPositionsInSample[ i ] != null;
			      i++ )
				this.showTestingPositionsClickEventHandler_setTesters_setTester(
					i ,
					BestPositionsInSample[ i ] ,
					this.SimulatedCreationDateTime );
		}
		
		protected virtual void showTestingPositionsClickEventHandler(object sender, System.EventArgs e)
		{
			this.showTestingPositionsClickEventHandler_setTesters();
			QuantProject.Presentation.ExecutablesListViewer executablesListViewer =
				new ExecutablesListViewer(
					this.buyAndHoldTestersForBestTestingPositionsInSample );
			executablesListViewer.Show();
		}
		
		protected virtual void createAndShowContextMenu()
		{
			MenuItem[] menuItems = new MenuItem[4];
			menuItems[0] = new MenuItem("Run Strategy In Sample And Out of S.");
			menuItems[1] = new MenuItem("Run Strategy Only In Sample");
			menuItems[2] = new MenuItem("Run Strategy Only Out of S.");
			menuItems[3] = new MenuItem("Show TestingPositions");
			menuItems[0].Click +=
				new System.EventHandler(this.runStrategyClickEventHandlerInAndOutOfSample);
			menuItems[1].Click +=
				new System.EventHandler(this.runStrategyClickEventHandlerOnlyInSample);
			menuItems[2].Click +=
				new System.EventHandler(this.runStrategyClickEventHandlerOnlyOutOfSample);
			menuItems[3].Click +=
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
