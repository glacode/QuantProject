/*
QuantProject - Quantitative Finance Library

TesterForPairsTradingTestingPositions.cs
Copyright (C) 2009
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

using QuantProject.ADT.Histories;
using QuantProject.Data.DataProviders.Bars.Caching;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting.AccountProviding;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors;
using QuantProject.Business.Timing;
using QuantProject.Presentation;
using QuantProject.Presentation.Reporting.WindowsForm;
using QuantProject.Scripts.General.Strategies;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	[Serializable]
	/// <summary>
	/// Tests a PairsTradingTestingPositions object
	/// </summary>
	public class TesterForPairsTradingTestingPositions : IExecutable
	{
		private PairsTradingTestingPositions testingPositions;
		private int numberOfInSampleDays;
		private DateTime dateTimeWhenThisObjectWasLogged;
		
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		
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
		public TesterForPairsTradingTestingPositions(
			TestingPositions testingPositions ,
			int numberOfInSampleDays ,
			DateTime dateTimeWhenThisObjectWasLogged )
		{
			this.checkParameters( testingPositions );
			this.testingPositions = (PairsTradingTestingPositions)testingPositions;
			this.numberOfInSampleDays = numberOfInSampleDays;
			this.dateTimeWhenThisObjectWasLogged =	dateTimeWhenThisObjectWasLogged;
		}
		private void checkParameters( TestingPositions testingPositions )
		{
			if ( ! ( testingPositions is PairsTradingTestingPositions ) )
				throw new Exception(
					"The given testingPositions is NOT a " +
					"PairsTradingTestingPositions!" );
		}

		#region Run
		
		#region getHistory
		
		#region addItemsToHistory
		private DateTime getFirstDateTime()
		{
			DateTime firstDateTime =
				HistoricalEndOfDayTimer.GetMarketOpen(
					this.dateTimeWhenThisObjectWasLogged.AddDays( 1 ) );
			return firstDateTime;
		}
		private DateTime getLastDateTime()
		{
			DateTime firstDateTime = this.getFirstDateTime();
			DateTime lastDateTime =
				HistoricalEndOfDayTimer.GetMarketClose( firstDateTime );
			return lastDateTime;
		}
		private void addItemToHistory(
			WeightedPosition weightedPosition , DateTime dateTime , History history )
		{
			try
			{
			double marketValue = this.historicalMarketValueProvider.GetMarketValue(
				weightedPosition.Ticker , dateTime );
			history.Add( dateTime , marketValue );
			}
			catch( TickerNotExchangedException tickerNotExchangedException )
			{
				string toAvoidWarning = tickerNotExchangedException.Message;
			}
		}
		private void addItemsToHistory( WeightedPosition weightedPosition , History history )
		{
			DateTime currentDateTime = this.getFirstDateTime();
			DateTime lastDateTime = this.getLastDateTime();
			while ( currentDateTime <= lastDateTime )
			{
				this.addItemToHistory( weightedPosition , currentDateTime , history );
				currentDateTime = currentDateTime.AddMinutes( 1 );
			}
		}
		#endregion addItemsToHistory
		
		private History getHistory( WeightedPosition weightedPosition )
		{
			History history = new History();
			this.addItemsToHistory( weightedPosition , history );
			return history;
		}
		#endregion getHistory
		
		private void showHistoriesPlots(
			History historyForFirstPosition , History historyForSecondPosition )
		{
			HistoriesViewer historiesViewer = new HistoriesViewer();
			historiesViewer.Add( historyForFirstPosition , Color.Green );
			historiesViewer.Add( historyForSecondPosition , Color.Red );
			historiesViewer.ShowDialog();
		}
		
		public void Run()
		{
			this.historicalMarketValueProvider = new HistoricalBarProvider(
				new SimpleBarCache( 60 ) );
			History historyForFirstPosition = this.getHistory(
				this.testingPositions.WeightedPositions[ 0 ] );
			History historyForSecondPosition = this.getHistory(
				this.testingPositions.WeightedPositions[ 1 ] );
			this.showHistoriesPlots( historyForFirstPosition , historyForSecondPosition );
		}
		#endregion Run


	}
}
