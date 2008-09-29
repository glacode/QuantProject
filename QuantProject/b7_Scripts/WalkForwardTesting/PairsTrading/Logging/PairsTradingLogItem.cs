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
			DateTime now ,
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
			DateTime now )
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
			DateTime now )
		{
			this.testersForBestTestingPositionsInSample =
				new TesterForPairsTradingTestingPositions[
					bestTestingPositionsInSample.Length ];
			for ( int i = 0 ; i < bestTestingPositionsInSample.Length ; i++ )
				this.setTesterForPairstTradingTestingPositions(
					i ,
					bestTestingPositionsInSample[ i ] ,
					now );
		}
		#endregion setTestersForPairstTradingTestingPositions

		/// <summary>
		/// We don't use a property instead of this method,
		/// to avoid it being shown in the log viewer list
		/// (it would be meaningless in the grid)
		/// </summary>
		public PairsTradingTestingPositions[]
			GetTestingPositions()
		{
			PairsTradingTestingPositions[] testingPositions =
				new PairsTradingTestingPositions[
				this.testersForBestTestingPositionsInSample.Length ];
			for ( int i = 0 ;
				i < this.testersForBestTestingPositionsInSample.Length ; i++ )
				testingPositions[ i ] =
					this.testersForBestTestingPositionsInSample[ i ].TestingPositions;
			return testingPositions;
		}
		
		public override void Run()
		{
			QuantProject.Presentation.ExecutablesListViewer executablesListViewer =
				new ExecutablesListViewer(
					this.testersForBestTestingPositionsInSample );
			executablesListViewer.Show();
		}
	}
}
