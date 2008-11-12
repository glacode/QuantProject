/*
QuantProject - Quantitative Finance Library

TesterForFLTPTestingPositions.cs
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
using QuantProject.Scripts.General.Reporting;

namespace QuantProject.Scripts.WalkForwardTesting.FixedLengthTwoPhases
{
	[Serializable]
	/// <summary>
	/// Tests a FLTPTestingPositions object
	/// </summary>
	public class TesterForFLTPTestingPositions : IExecutable
	{
		private FLTPTestingPositions testingPositions;
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
//		public PairsTradingTestingPositions TestingPositions
//		{
//			get { return this.testingPositions; }
//		}
		
		public TesterForFLTPTestingPositions(
			TestingPositions testingPositions ,
			int numberOfInSampleDays ,
			DateTime dateTimeWhenThisObjectWasLogged )
		{
			this.checkParameters( testingPositions );
			this.testingPositions =
				(FLTPTestingPositions)testingPositions;
			this.numberOfInSampleDays = numberOfInSampleDays;
			this.dateTimeWhenThisObjectWasLogged =
				dateTimeWhenThisObjectWasLogged;
		}
		private void checkParameters( TestingPositions testingPositions )
		{
			if ( ! ( testingPositions is FLTPTestingPositions ) )
				throw new Exception(
					"The given testingPositions is NOT a " +
					"FLTPTestingPositions!" );
		}
		
		#region Run
		private AccountReport getAccountReport(
			WeightedPositions weightedPositions ,
			IIntervalsSelector intervalsSelector ,
			HistoricalMarketValueProvider HistoricalMarketValueProvider ,
			Benchmark benchmark ,
			double cashToStart )
		{
			FLTPSimpleStrategy fLTPSimpleStrategy =
				new FLTPSimpleStrategy( weightedPositions ,
				                       intervalsSelector , HistoricalMarketValueProvider );
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
					"SimpleFLTP" ,
					new QuantProject.Business.Timing.IndexBasedEndOfDayTimer(
						HistoricalEndOfDayTimer.GetMarketOpen( firstDateTime ) ,
						benchmark.Ticker ) ,
					fLTPSimpleStrategy ,
					HistoricalMarketValueProvider , accountProvider ,
					firstDateTime , lastDateTime ,
					benchmark , cashToStart , maxRunningHours );

//			simpleStrategy.Account = endOfDayStrategyBackTester.Account;

			endOfDayStrategyBackTester.Run();
			return endOfDayStrategyBackTester.AccountReport;
		}
//		private double getWeightedPositions_getWeight(
//			WeightedPosition weightedPosition )
//		{
//			double weight = 1;
//			if ( weightedPosition.IsShort )
//				// the current WeightedPosition is short in the couple correlation
//				weight = -1;
//			return weight;
//		}
//		private WeightedPositions getWeightedPositions(
//			WeightedPosition weightedPosition )
//		{
		////			double[] weights = { 1 };
//			double[] weights = { this.getWeightedPositions_getWeight( weightedPosition ) };
//			string[] tickers = { weightedPosition.Ticker };
//			WeightedPositions weightedPositions =
//				new WeightedPositions( weights , tickers );
//			return weightedPositions;
//		}
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
			// uncomment the following line if the optimization
			// was made on both phases
			intervalsSelector =
				new FixedLengthTwoPhasesIntervalsSelector( 1 , 1 , benchmark );
			
			IEligiblesSelector eligiblesSelector = new DummyEligibleSelector();

			WeightedPositions weightedPositions =
				this.testingPositions.WeightedPositions;
			
			AccountReport accountReport =
				this.getAccountReport( weightedPositions , intervalsSelector ,
				                      historicalMarketValueProvider ,
				                      benchmark , 30000 );
			
			Report report =
				new Report( accountReport , false );
//			EndOfDayDateTime lastEndOfDayDateTimeForReport =
//				new EndOfDayDateTime(
//				accountReportForFirstPosition.EquityLine.LastDateTime ,
//				EndOfDaySpecificTime.OneHourAfterMarketClose );
			
			//			report.Create( "PearsonDebug" , 1 ,
			//			              lastEndOfDayDateTimeForReport ,
			//			              benchmark.Ticker , false );
//			report.AddEquityLine( accountReportForSecondPosition.EquityLine ,
//				Color.Brown );
			report.ShowDialog();
		}
		#endregion Run

	}
}
