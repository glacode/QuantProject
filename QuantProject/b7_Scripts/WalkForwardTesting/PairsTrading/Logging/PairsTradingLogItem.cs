/*
QuantProject - Quantitative Finance Library

PairsTradingLogItem.cs
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

using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Timing;
using QuantProject.Presentation;
//using QuantProject.Presentation.Reporting.WindowsForm;
//using QuantProject.Scripts.General.Reporting;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Log item for the Pairs Trading strategy
	/// </summary>
	[Serializable]
	public class PairsTradingLogItem : LogItem
	{
		private TesterForPairsTradingTestingPositions[]
			testersForBestTestingPositionsInSample;
		private int numberOfInSampleDays;
		private int numberOfEligibleTickers;
		
//		public TestingPositions BestTestingPositionsInSample
//		{
//			set { this.bestTestingPositionsInSample = value; }
//		}

//		public WeightedPositions BestWeightedPositionsInSample
//		{
//			get
//			{
//				return this.bestTestingPositionsInSample.WeightedPositions;
//			}
//		}

		public int NumberOfEligibleTickers
		{
			get
			{
				return this.numberOfEligibleTickers;
			}
		}

		/// <summary>
		/// Log item for the Pairs Trading strategy
		/// </summary>
		/// <param name="endOfDayDateTime"></param>
		/// <param name="bestTestingPositionsInSample"></param>
		/// <param name="numInSampleDays">number of days used for
		/// in sample optimization</param>
		/// <param name="numberOfEligibleTickers"></param>
		public PairsTradingLogItem(
			EndOfDayDateTime now ,
			TestingPositions[] bestTestingPositionsInSample ,
			int numberOfInSampleDays ,
			int numberOfEligibleTickers )
			: base( now )
		{
			this.numberOfEligibleTickers = int.MinValue;
//			this.bestTestingPositionsInSample = bestTestingPositionsInSample;
			this.numberOfInSampleDays = numberOfInSampleDays;
			this.numberOfEligibleTickers = numberOfEligibleTickers;
			this.setTestersForPairstTradingTestingPositions(
				bestTestingPositionsInSample ,
				now );
		}
		
		#region setTestersForPairstTradingTestingPositions
		private void setTestersForPairstTradingTestingPositions_checkParameters(
			TestingPositions testingPositions )
		{
			if ( ! ( testingPositions is PairsTradingTestingPositions ) )
				throw new Exception(
					"TestingPositions are all expected to be " +
					"PairsTradingTestingPositions. The current TestingPositions " +
					"is not a PairsTradingTestingPositions instead!" );
		}
		private void setTesterForPairstTradingTestingPositions(
			int currentIndex ,
			TestingPositions testingPositions ,
			EndOfDayDateTime now )
		{
			this.setTestersForPairstTradingTestingPositions_checkParameters(
				testingPositions );
			this.testersForBestTestingPositionsInSample[ currentIndex ] =
				new TesterForPairsTradingTestingPositions(
					testingPositions ,
					this.numberOfInSampleDays ,
					now );
		}
		private void setTestersForPairstTradingTestingPositions(
			TestingPositions[] bestTestingPositionsInSample ,
			EndOfDayDateTime now )
		{
			this.testersForBestTestingPositionsInSample =
				new TesterForPairsTradingTestingPositions[
					bestTestingPositionsInSample.Length ];
			for ( int i = 0 ; i < bestTestingPositionsInSample.Length - 1 ; i++ )
				this.setTesterForPairstTradingTestingPositions(
					i ,
					bestTestingPositionsInSample[ i ] ,
					now );
		}
		#endregion setTestersForPairstTradingTestingPositions
//		private AccountReport getAccountReport(
//			WeightedPositions weightedPositions ,
//			IIntervalsSelector intervalsSelector ,
//			IHistoricalQuoteProvider historicalQuoteProvider ,
//			Benchmark benchmark ,
//			double cashToStart )
//		{
//			SimpleStrategy simpleStrategy =
//				new SimpleStrategy( weightedPositions ,
//				intervalsSelector , historicalQuoteProvider );
//
//			DateTime firstDateTime = this.simulatedCreationTime.DateTime.AddDays( -90 );
//			DateTime lastDateTime = this.simulatedCreationTime.DateTime;
//			double maxRunningHours = 0.3;
//			EndOfDayStrategyBackTester endOfDayStrategyBackTester =
//				new EndOfDayStrategyBackTester(
//				"SinglePosition" , simpleStrategy ,
//				historicalQuoteProvider , firstDateTime ,
//				lastDateTime , benchmark , cashToStart , maxRunningHours );
//
//			simpleStrategy.Account = endOfDayStrategyBackTester.Account;
//
//			endOfDayStrategyBackTester.Run();
//			return endOfDayStrategyBackTester.AccountReport;
//		}
//		private WeightedPositions getWeightedPositions(
//			WeightedPosition weightedPosition )
//		{
//			double[] weights = { 1 };
//			string[] tickers = { weightedPosition.Ticker };
//			WeightedPositions weightedPositions =
//				new WeightedPositions( weights , tickers );
//			return weightedPositions;
//		}
//		public void Run3()
//		{
////			string backTestId = "SimplePairsTrading";
////			double cashToStart = 30000;
//
//			Benchmark benchmark = new Benchmark( "MSFT" );
//
//			IHistoricalQuoteProvider historicalQuoteProvider =
//				new HistoricalAdjustedQuoteProvider();
//
////			IInSampleChooser inSampleChooser =
////				(IInSampleChooser)new ConstantWeightedPositionsChooser(
////				this.BestWeightedPositionsInSample );
//
//			IIntervalsSelector intervalsSelector =
//				new OddIntervalsSelector(	1 , 1 , benchmark );
//			IEligiblesSelector eligiblesSelector = new DummyEligibleSelector();
//
//			WeightedPositions weightedPositions =
//				this.bestTestingPositionsInSample[ 0 ].WeightedPositions;
//				
//			WeightedPositions firstPosition =
//				this.getWeightedPositions( weightedPositions[ 0 ] );
//			WeightedPositions secondPosition =
//				this.getWeightedPositions( weightedPositions[ 1 ] );
//			AccountReport accountReportForFirstPosition =
//				this.getAccountReport( firstPosition , intervalsSelector ,
//				                     historicalQuoteProvider ,
//				                     benchmark , 30000 );
//			AccountReport accountReportForSecondPosition =
//				this.getAccountReport( secondPosition , intervalsSelector ,
//				                      historicalQuoteProvider ,
//				                      benchmark ,
//				                      30000 * weightedPositions[ 1 ].Weight /
//				                      weightedPositions[ 0 ].Weight );
//			
//			Report report =
//				new Report( accountReportForFirstPosition , false );
//			EndOfDayDateTime lastEndOfDayDateTimeForReport =
//				new EndOfDayDateTime(
//					accountReportForFirstPosition.EquityLine.LastDateTime ,
//					EndOfDaySpecificTime.OneHourAfterMarketClose );
//				
////			report.Create( "PearsonDebug" , 1 ,
////			              lastEndOfDayDateTimeForReport ,
////			              benchmark.Ticker , false );
//			report.AddEquityLine( accountReportForSecondPosition.EquityLine ,
//			                     Color.Brown );
//			report.ShowDialog();
//		}
		public override void Run()
		{
			QuantProject.Presentation.ExecutablesListViewer executablesListViewer =
				new ExecutablesListViewer(
					this.testersForBestTestingPositionsInSample );
			executablesListViewer.Show();
		}
	}
}
