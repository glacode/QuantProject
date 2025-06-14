/*
QuantProject - Quantitative Finance Library

TesterForPairsTradingTestingPositions_old.cs
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
using System.Drawing;

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting.AccountProviding;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;
using QuantProject.Presentation.Reporting.WindowsForm;
using QuantProject.Scripts.General.Strategies;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	[Serializable]
	/// <summary>
	/// Tests a PairsTradingTestingPositions object looking in sample and using end of day quotes
	/// </summary>
	public class TesterForPairsTradingTestingPositionsEndOfDayInSample : IExecutable
	{
		private PairsTradingTestingPositions testingPositions;
		private int numberOfInSampleDays;
		private DateTime dateTimeWhenThisObjectWasLogged;
		
		/// <summary>
		/// Generation when the TestingPositions object has been created
		/// (if genetically optimized)
		/// </summary>
		public int Generation
		{
			get
			{
				return this.testingPositions.Generation;
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
		public PairsTradingTestingPositions TestingPositions
		{
			get { return this.testingPositions; }
		}
		public TesterForPairsTradingTestingPositionsEndOfDayInSample(
			TestingPositions testingPositions ,
			int numberOfInSampleDays ,
			DateTime dateTimeWhenThisObjectWasLogged )
		{
			this.checkParameters( testingPositions );
			this.testingPositions = (PairsTradingTestingPositions)testingPositions;
			this.numberOfInSampleDays = numberOfInSampleDays;
			this.dateTimeWhenThisObjectWasLogged =
				dateTimeWhenThisObjectWasLogged;
		}
		private void checkParameters( TestingPositions testingPositions )
		{
			if ( ! ( testingPositions is PairsTradingTestingPositions ) )
				throw new Exception(
					"The given testingPositions is NOT a " +
					"PairsTradingTestingPositions!" );
		}
		
		#region Run
		private AccountReport getAccountReport(
			WeightedPositions weightedPositions ,
			IIntervalsSelector intervalsSelector ,
			HistoricalMarketValueProvider historicalMarketValueProvider ,
			Benchmark benchmark ,
			double cashToStart )
		{
			SimpleStrategy simpleStrategy =
				new SimpleStrategy( weightedPositions ,
				                   intervalsSelector , historicalMarketValueProvider );
			IAccountProvider accountProvider =
				new SimpleAccountProvider();

			DateTime firstDateTime =
				this.dateTimeWhenThisObjectWasLogged.AddDays(
					- this.numberOfInSampleDays );
			DateTime lastDateTime =
				this.dateTimeWhenThisObjectWasLogged;
			double maxRunningHours = 0.3;
			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
				new EndOfDayStrategyBackTester(
					"SinglePosition" ,
					new QuantProject.Business.Timing.IndexBasedEndOfDayTimer(
						HistoricalEndOfDayTimer.GetMarketOpen( firstDateTime ) ,
						benchmark.Ticker ) ,
					simpleStrategy ,
					historicalMarketValueProvider , accountProvider ,
					firstDateTime , lastDateTime ,
					benchmark , cashToStart , maxRunningHours );

//			simpleStrategy.Account = endOfDayStrategyBackTester.Account;

			endOfDayStrategyBackTester.Run();
			return endOfDayStrategyBackTester.AccountReport;
		}
		private double getWeightedPositions_getWeight(
			WeightedPosition weightedPosition )
		{
			double weight = 1;
			if ( weightedPosition.IsShort )
				// the current WeightedPosition is short in the couple correlation
				weight = -1;
			return weight;
		}
		private WeightedPositions getWeightedPositions(
			WeightedPosition weightedPosition )
		{
//			double[] weights = { 1 };
			double[] weights = { this.getWeightedPositions_getWeight( weightedPosition ) };
			string[] tickers = { weightedPosition.Ticker };
			WeightedPositions weightedPositions =
				new WeightedPositions( weights , tickers );
			return weightedPositions;
		}
		public void Run()
		{
			//			string backTestId = "SimplePairsTrading";
			//			double cashToStart = 30000;

			Benchmark benchmark = new Benchmark( "MSFT" );

			HistoricalMarketValueProvider historicalMarketValueProvider =
				new HistoricalAdjustedQuoteProvider();

			//			IInSampleChooser inSampleChooser =
			//				(IInSampleChooser)new ConstantWeightedPositionsChooser(
			//				this.BestWeightedPositionsInSample );

			IIntervalsSelector intervalsSelector =
				new OddIntervalsSelector(	1 , 1 , benchmark );
			IEligiblesSelector eligiblesSelector = new DummyEligibleSelector();

			WeightedPositions weightedPositions =
				this.testingPositions.WeightedPositions;
			
			WeightedPositions firstPosition =
				this.getWeightedPositions( weightedPositions[ 0 ] );
			WeightedPositions secondPosition =
				this.getWeightedPositions( weightedPositions[ 1 ] );
			AccountReport accountReportForFirstPosition =
				this.getAccountReport( firstPosition , intervalsSelector ,
				                      historicalMarketValueProvider ,
				                      benchmark , 30000 );
			AccountReport accountReportForSecondPosition =
				this.getAccountReport( secondPosition , intervalsSelector ,
				                      historicalMarketValueProvider ,
				                      benchmark ,
				                      Math.Abs(	30000 * weightedPositions[ 1 ].Weight /
				                               weightedPositions[ 0 ].Weight ) );
			
			Report report =
				new Report( accountReportForFirstPosition , false );
			DateTime lastDateTimeForReport =
				HistoricalEndOfDayTimer.GetOneHourAfterMarketClose(
					accountReportForFirstPosition.EquityLine.LastDateTime );
//				new EndOfDayDateTime(
//				accountReportForFirstPosition.EquityLine.LastDateTime ,
//				EndOfDaySpecificTime.OneHourAfterMarketClose );
			
			//			report.Create( "PearsonDebug" , 1 ,
			//			              lastEndOfDayDateTimeForReport ,
			//			              benchmark.Ticker , false );
			report.AddEquityLine( accountReportForSecondPosition.EquityLine ,
			                     Color.Brown );
			report.ShowDialog();
		}
		#endregion Run

	}
}
