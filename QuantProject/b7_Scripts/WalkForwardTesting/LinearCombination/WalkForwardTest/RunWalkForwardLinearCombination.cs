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
using QuantProject.Presentation;
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
		private DateTime deadlineForScript;

		private HistoricalMarketValueProvider historicalMarketValueProvider;
		private HistoricalEndOfDayTimer historicalEndOfDayTimer;
		private Account account;
		private WalkForwardOpenToCloseDailyStrategy endOfDayStrategy;

		public RunWalkForwardLinearCombination(string tickerGroupID,
		                                       int numDaysForInSampleOptimization ,
		                                       int numberOfEligibleTickers,
		                                       int numberOfTickersToBeChosen, int numDaysForLiquidity,
		                                       int generationNumberForGeneticOptimizer,
		                                       int populationSizeForGeneticOptimizer, string benchmark,
		                                       DateTime firstDate, DateTime lastDate, double targetReturn,
		                                       PortfolioType portfolioType ,
		                                       bool openToCloseDaily ,
		                                       double maxRunningHours )
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
			this.deadlineForScript = DateTime.Now.AddHours( maxRunningHours );
		}

		private void oneHourAfterMarketCloseEventHandler( Object sender ,
		                                                 DateTime dateTime )
		{
			if ( ( this.account.Timer.GetCurrentDateTime() >=
			      this.lastDate ) ||
			    ( DateTime.Now > this.deadlineForScript ) )
				this.account.Timer.Stop();
		}

		private void run_setHistoricalQuoteProvider()
		{
			if ( this.openToCloseDaily )
				this.historicalMarketValueProvider = new HistoricalRawQuoteProvider();
			else
				this.historicalMarketValueProvider = new HistoricalAdjustedQuoteProvider();
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
					HistoricalEndOfDayTimer.GetMarketOpen( this.firstDate ) ,
//				new EndOfDayDateTime( this.firstDate ,
//				EndOfDaySpecificTime.MarketOpen ) ,
					"MSFT" );
			run_setHistoricalQuoteProvider();
			this.account = new Account( "LinearCombination" , historicalEndOfDayTimer ,
			                           new HistoricalDataStreamer( historicalEndOfDayTimer ,
			                                                              this.historicalMarketValueProvider ) ,
			                           new HistoricalOrderExecutor( historicalEndOfDayTimer ,
			                                                               this.historicalMarketValueProvider ) );
			run_setStrategy();
//			OneRank oneRank = new OneRank( account ,
//				this.endDateTime );
			this.historicalEndOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.endOfDayStrategy.NewDateTimeEventHandler );
//			this.historicalEndOfDayTimer.MarketOpen +=
//				new MarketOpenEventHandler(
//					this.endOfDayStrategy.MarketOpenEventHandler );
//			this.historicalEndOfDayTimer.FiveMinutesBeforeMarketClose +=
//				new FiveMinutesBeforeMarketCloseEventHandler(
//					this.endOfDayStrategy.FiveMinutesBeforeMarketCloseEventHandler );
//			this.historicalEndOfDayTimer.OneHourAfterMarketClose +=
//				new OneHourAfterMarketCloseEventHandler(
//					this.endOfDayStrategy.OneHourAfterMarketCloseEventHandler );
//			this.historicalEndOfDayTimer.OneHourAfterMarketClose +=
//				new OneHourAfterMarketCloseEventHandler(
//					this.oneHourAfterMarketCloseEventHandler );
			this.account.Timer.Start();

			Report report = new Report( this.account , this.historicalMarketValueProvider );
			report.Create(
				"Linear Combination" , 1 ,
				HistoricalEndOfDayTimer.GetMarketClose(	this.lastDate ) ,
//				new EndOfDayDateTime( this.lastDate , EndOfDaySpecificTime.MarketClose ) ,
				"^SPX" );
			//			ObjectArchiver.Archive( report.AccountReport ,
			//				@"C:\Documents and Settings\Glauco\Desktop\reports\runOneRank.qPr" );
			report.Show();
			string title = DateTime.Now.Year.ToString() + "_" +
				DateTime.Now.Month.ToString() + "_" +
				DateTime.Now.Day.ToString() + "_" +
				DateTime.Now.Hour.ToString() + "_" +
				DateTime.Now.Minute.ToString() + "_" +
				DateTime.Now.Second.ToString() + "_" +
				"numDayInSample" + this.numDaysForInSampleOptimization +
				"_numEligTick" + this.numberOfEligibleTickers +
				"_numDayLiq" + this.numDaysForLiquidity +
				"_numTickInPortfolio" + this.numberOfTickersToBeChosen;
			VisualObjectArchiver visualObjectArchiver =
				new VisualObjectArchiver();
			visualObjectArchiver.Save( this.endOfDayStrategy.OptimizationOutput ,
			                          "bgn" , title );
		}
	}
}
