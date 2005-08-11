/*
QuantProject - Quantitative Finance Library

RunWalkForwardLinearCombination.cs
Copyright (C) 2003 
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
using System.Collections;

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;
using QuantProject.Presentation.Reporting.WindowsForm;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;


namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Walk Forward test for the linear combination strategy
	/// </summary>
	public class RunWalkForwardLinearCombination
	{
		private string tickerGroupID;
		private int numDaysForInSampleOptimization;
		private int numberOfEligibleTickers;
		private int numberOfTickersToBeChosen;
		private int numDaysForLiquidity;
		private int generationNumberForGeneticOptimizer;
		private int populationSizeForGeneticOptimizer;
		private string benchmark;
		private DateTime firstDate;
		private DateTime lastDate;
		private double targetReturn;
		private PortfolioType portfolioType;

		private bool openToCloseDaily;

		private IHistoricalQuoteProvider historicalQuoteProvider;
		private HistoricalEndOfDayTimer historicalEndOfDayTimer;
		private Account account;
		private IEndOfDayStrategy endOfDayStrategy;

		public RunWalkForwardLinearCombination(string tickerGroupID,
			int numDaysForInSampleOptimization ,
			int numberOfEligibleTickers, 
			int numberOfTickersToBeChosen, int numDaysForLiquidity, 
			int generationNumberForGeneticOptimizer,
			int populationSizeForGeneticOptimizer, string benchmark,
			DateTime firstDate, DateTime lastDate, double targetReturn,
			PortfolioType portfolioType ,
			bool openToCloseDaily )
		{
			this.tickerGroupID = tickerGroupID;
			this.numDaysForInSampleOptimization = numDaysForInSampleOptimization;
			this.numberOfEligibleTickers = numberOfEligibleTickers;
			this.numberOfTickersToBeChosen = numberOfTickersToBeChosen;
			this.numDaysForLiquidity = numDaysForLiquidity;
			this.generationNumberForGeneticOptimizer =
				generationNumberForGeneticOptimizer;
			this.populationSizeForGeneticOptimizer =
				populationSizeForGeneticOptimizer;
			this.benchmark = benchmark;
			this.firstDate = firstDate;
			this.lastDate = lastDate;
			this.targetReturn = targetReturn;
			this.portfolioType = portfolioType;
			this.openToCloseDaily = openToCloseDaily;
		}

		private void oneHourAfterMarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			if ( this.account.EndOfDayTimer.GetCurrentTime().DateTime >=
				this.lastDate )
				this.account.EndOfDayTimer.Stop();
		}

		private void run_setHistoricalQuoteProvider()
		{
			if ( this.openToCloseDaily )
				this.historicalQuoteProvider = new HistoricalRawQuoteProvider();
			else
				this.historicalQuoteProvider = new HistoricalAdjustedQuoteProvider();
		}
		private void run_setStrategy()
		{
			if ( this.openToCloseDaily )
				this.endOfDayStrategy =
					new WalkForwardOpenToCloseDailyStrategy(
					this.account , this.tickerGroupID ,
					this.numDaysForInSampleOptimization ,
					this.numberOfEligibleTickers ,
					this.numberOfTickersToBeChosen ,
					this.benchmark , this.targetReturn , this.portfolioType ,
					this.populationSizeForGeneticOptimizer ,
					this.generationNumberForGeneticOptimizer );
//			else
//				this.endOfDayStrategy = new OpenToCloseWeeklyStrategy(
//					this.account , this.signedTickers );
		}
		public void Run()
		{
			this.historicalEndOfDayTimer =
				new IndexBasedEndOfDayTimer(
				new EndOfDayDateTime( this.firstDate ,
				EndOfDaySpecificTime.MarketOpen ) , "DYN" );
			run_setHistoricalQuoteProvider();
			this.account = new Account( "LinearCombination" , historicalEndOfDayTimer ,
				new HistoricalEndOfDayDataStreamer( historicalEndOfDayTimer ,
				this.historicalQuoteProvider ) ,
				new HistoricalEndOfDayOrderExecutor( historicalEndOfDayTimer ,
				this.historicalQuoteProvider ) );
			run_setStrategy();
//			OneRank oneRank = new OneRank( account ,
//				this.endDateTime );
			this.historicalEndOfDayTimer.MarketOpen +=
				new MarketOpenEventHandler(
				this.endOfDayStrategy.MarketOpenEventHandler );
			this.historicalEndOfDayTimer.FiveMinutesBeforeMarketClose +=
				new FiveMinutesBeforeMarketCloseEventHandler(
				this.endOfDayStrategy.FiveMinutesBeforeMarketCloseEventHandler );
			this.historicalEndOfDayTimer.OneHourAfterMarketClose +=
				new OneHourAfterMarketCloseEventHandler(
				this.endOfDayStrategy.OneHourAfterMarketCloseEventHandler );
			this.historicalEndOfDayTimer.OneHourAfterMarketClose +=
				new OneHourAfterMarketCloseEventHandler(
				this.oneHourAfterMarketCloseEventHandler );
			this.account.EndOfDayTimer.Start();

			Report report = new Report( this.account , this.historicalQuoteProvider );
			report.Create( "Linear Combination" , 1 ,
				new EndOfDayDateTime( this.lastDate , EndOfDaySpecificTime.MarketClose ) ,
				"^SPX" );
			//			ObjectArchiver.Archive( report.AccountReport ,
			//				@"C:\Documents and Settings\Glauco\Desktop\reports\runOneRank.qPr" );
			report.Show();
		}
	}
}
