/*
QuantProject - Quantitative Finance Library

BuyAndHoldTesterForTestingPositions.cs
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
using System.Windows.Forms;

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting.AccountProviding;
using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;
using QuantProject.Scripts.General.Reporting;

namespace QuantProject.Scripts.General.Logging
{
	[Serializable]
	/// <summary>
	/// It runs a simple buy and hold test on the given TestingPositions object
	/// </summary>
	public class BuyAndHoldTesterForTestingPositions : IExecutable
	{
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		private Benchmark benchmark;
		private TestingPositions testingPositions;
		private int numberOfInSampleDays;
		private DateTime simulatedCreationDateTime;
		
		/// <summary>
		/// Generation when the TestingPositions object has been created
		/// (if genetically optimized)
		/// </summary>
		public int Generation
		{
			get
			{
				int generation = -1;
				if( this.testingPositions is IGeneticallyOptimizable )
					generation = 
						((IGeneticallyOptimizable)this.testingPositions).Generation;
				return generation;
			}
		}
		public double FitnessInSample
		{
			get { return this.testingPositions.FitnessInSample; }
		}
		public string ShortDescription
		{
			get { return this.testingPositions.WeightedPositions.Description; }
		}
		public BuyAndHoldTesterForTestingPositions( HistoricalMarketValueProvider historicalMarketValueProvider,
			Benchmark benchmark,
		  TestingPositions testingPositions ,
			int numberOfInSampleDays ,
			DateTime simulatedCreationDateTime )
		{
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.benchmark = benchmark;
			this.testingPositions = testingPositions;
			this.numberOfInSampleDays = numberOfInSampleDays;
			this.simulatedCreationDateTime =
				simulatedCreationDateTime;
		}
		
		#region runStrategyClickEventHandlerInAndOutOfSample
		protected virtual void runStrategyClickEventHandlerInAndOutOfSample(object sender, System.EventArgs e)
		{
			DateTime firstDateTime = this.simulatedCreationDateTime.AddDays(-this.numberOfInSampleDays);
			DateTime lastDateTime = this.simulatedCreationDateTime;
			double maxRunningHours = 1;
			int	numberOfPortfolioPositions = this.testingPositions.WeightedPositions.Count;
			//cash and portfolio type
			double cashToStart = 10000;
			HistoricalMarketValueProvider historicalQuoteProviderForBackTester,
				historicalQuoteProviderForInSampleChooser,
				historicalQuoteProviderForStrategy;
			historicalQuoteProviderForBackTester = this.historicalMarketValueProvider;
			historicalQuoteProviderForInSampleChooser = historicalQuoteProviderForBackTester;
			historicalQuoteProviderForStrategy = historicalQuoteProviderForInSampleChooser;
			
			BuyAndHoldStrategy strategy =
					new BuyAndHoldStrategy(this.testingPositions);
		
			QuantProject.Business.Timing.Timer timer = 
				new IndexBasedEndOfDayTimer( firstDateTime,
				                             lastDateTime.AddDays(this.numberOfInSampleDays),
				                             this.benchmark.Ticker);
			
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
					"BuyAndHold" , timer	 , strategy, 
					historicalQuoteProviderForBackTester ,
					new SimpleAccountProvider(), firstDateTime ,
					lastDateTime.AddDays(this.numberOfInSampleDays) ,
					this.benchmark , cashToStart , maxRunningHours );
			
			endOfDayStrategyBackTester.Run();
			BackTesterReportViewer.ShowReport( lastDateTime.AddDays(this.numberOfInSampleDays) ,
			                                  endOfDayStrategyBackTester );
			                               
		}
		#endregion runStrategyClickEventHandlerInAndOutOfSample
		
		#region runStrategyClickEventHandlerOnlyInSample
		protected virtual void runStrategyClickEventHandlerOnlyInSample(object sender, System.EventArgs e)
		{
			DateTime firstDateTime = this.simulatedCreationDateTime.AddDays(-this.numberOfInSampleDays);
			DateTime lastDateTime = this.simulatedCreationDateTime;
			double maxRunningHours = 1;
			int	numberOfPortfolioPositions = this.testingPositions.WeightedPositions.Count;
			//cash and portfolio type
			double cashToStart = 10000;
			HistoricalMarketValueProvider historicalQuoteProviderForBackTester,
				historicalQuoteProviderForInSampleChooser,
				historicalQuoteProviderForStrategy;
			historicalQuoteProviderForBackTester = this.historicalMarketValueProvider;
			historicalQuoteProviderForInSampleChooser = historicalQuoteProviderForBackTester;
			historicalQuoteProviderForStrategy = historicalQuoteProviderForInSampleChooser;
			
			BuyAndHoldStrategy strategy =
					new BuyAndHoldStrategy(this.testingPositions);
		
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
			DateTime firstDateTime = this.simulatedCreationDateTime;
			DateTime lastDateTime = this.simulatedCreationDateTime.AddDays(this.numberOfInSampleDays);
			double maxRunningHours = 1;
			int	numberOfPortfolioPositions = this.testingPositions.WeightedPositions.Count;
			//cash and portfolio type
			double cashToStart = 10000;
			HistoricalMarketValueProvider historicalQuoteProviderForBackTester,
				historicalQuoteProviderForInSampleChooser,
				historicalQuoteProviderForStrategy;
			historicalQuoteProviderForBackTester = this.historicalMarketValueProvider;
			historicalQuoteProviderForInSampleChooser = historicalQuoteProviderForBackTester;
			historicalQuoteProviderForStrategy = historicalQuoteProviderForInSampleChooser;
			
			BuyAndHoldStrategy strategy =
					new BuyAndHoldStrategy(this.testingPositions);
		
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
		
		protected virtual void createAndShowContextMenu()
		{
			MenuItem[] menuItems = new MenuItem[3];
			menuItems[0] = new MenuItem("Run Buy&Hold Strategy In Sample And Out of S.");
			menuItems[1] = new MenuItem("Run Buy&Hold Strategy Only In Sample");
			menuItems[2] = new MenuItem("Run Buy&Hold Strategy Only Out of S.");
			menuItems[0].Click +=
				new System.EventHandler(this.runStrategyClickEventHandlerInAndOutOfSample);
			menuItems[1].Click +=
				new System.EventHandler(this.runStrategyClickEventHandlerOnlyInSample);
			menuItems[2].Click +=
				new System.EventHandler(this.runStrategyClickEventHandlerOnlyOutOfSample);
			ContextMenu contextMenu = new ContextMenu(menuItems);
			contextMenu.Show(Form.ActiveForm, Form.MousePosition);
		}
		
		public void Run()
		{
			this.createAndShowContextMenu();
		}
	}
}
