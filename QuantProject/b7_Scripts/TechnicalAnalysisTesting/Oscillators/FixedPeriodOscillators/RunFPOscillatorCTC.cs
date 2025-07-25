/*
QuantProject - Quantitative Finance Library

RunFPOscillatorCTC.cs
Copyright (C) 2003
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
using System.Collections;
using System.Data;
using QuantProject.ADT;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Testing;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Presentation.Reporting.WindowsForm;
using QuantProject.ADT.FileManaging;
using QuantProject.Scripts.WalkForwardTesting.LinearCombination;


namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedPeriodOscillators
{
	/// <summary>
	/// Script that implements an oscillating strategy,
	/// for all market days (fixed period), using efficient portfolios
	/// </summary>
	[Serializable]
	public class RunFPOscillatorCTC : RunEfficientPortfolio
	{
		private int numDaysForReturnCalculation;
		private double maxAcceptableCloseToCloseDrawdown;
		private int numDaysBetweenEachOptimization;
		
		public RunFPOscillatorCTC(string tickerGroupID, int numberOfEligibleTickers,
		                          int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
		                          int generationNumberForGeneticOptimizer,
		                          int populationSizeForGeneticOptimizer, string benchmark,
		                          DateTime startDate, DateTime endDate,
		                          int numDaysForReturnCalculation,
		                          PortfolioType portfolioType, double maxAcceptableCloseToCloseDrawdown,
		                          double maxRunningHours):
			base(tickerGroupID, numberOfEligibleTickers,
			     numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
			     generationNumberForGeneticOptimizer,
			     populationSizeForGeneticOptimizer, benchmark,
			     startDate, endDate, 0.0,
			     portfolioType, maxRunningHours)
		{
			this.ScriptName = "FixedPeriodOscillatorCTCScriptWithSharpe";
			this.numDaysForReturnCalculation = numDaysForReturnCalculation;
			this.maxAcceptableCloseToCloseDrawdown = maxAcceptableCloseToCloseDrawdown;
			this.numDaysBetweenEachOptimization = numDaysForReturnCalculation;
		}

		#region auxiliary overriden methods for Run
		
		protected override void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler = new EndOfDayTimerHandlerFPOscillatorCTC(this.tickerGroupID, this.numberOfEligibleTickers,
			                                                                    this.numberOfTickersToBeChosen, this.numDaysForOptimizationPeriod,
			                                                                    this.account,
			                                                                    this.generationNumberForGeneticOptimizer,
			                                                                    this.populationSizeForGeneticOptimizer, this.benchmark,
			                                                                    this.numDaysForReturnCalculation,
			                                                                    this.portfolioType, this.maxAcceptableCloseToCloseDrawdown);
		}
		
		protected override void run_initializeHistoricalQuoteProvider()
		{
			this.historicalMarketValueProvider = new HistoricalAdjustedQuoteProvider();
		}
		
		protected override void run_addEventHandlers()
		{
			
			
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.endOfDayTimerHandler.NewDateTimeEventHandler );
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.checkDateForReport );

//			this.endOfDayTimer.MarketClose +=
//				new MarketCloseEventHandler(
//					this.endOfDayTimerHandler.MarketCloseEventHandler);
//			
//			this.endOfDayTimer.MarketClose +=
//				new MarketCloseEventHandler(
//					this.checkDateForReport);
//			
//			this.endOfDayTimer.OneHourAfterMarketClose +=
//				new OneHourAfterMarketCloseEventHandler(
//					this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler);
		}

		public override void SaveScriptResults()
		{
			string fileName = "From"+this.numberOfEligibleTickers +
				"OptDays" + this.numDaysForOptimizationPeriod + "Portfolio" +
				this.numberOfTickersToBeChosen + "GenNum" +
				this.generationNumberForGeneticOptimizer +
				"PopSize" + this.populationSizeForGeneticOptimizer +
				"HalfPeriodDays" + Convert.ToString(this.numDaysForReturnCalculation) +
				Convert.ToString(this.portfolioType);
			string dirNameWhereToSaveReports =
				System.Configuration.ConfigurationManager.AppSettings["ReportsArchive"] +
				"\\" + this.ScriptName + "\\";
			string dirNameWhereToSaveTransactions =
				System.Configuration.ConfigurationManager.AppSettings["TransactionsArchive"] +
				"\\" + this.ScriptName + "\\";
			string dirNameWhereToSaveBestGenomes =
				System.Configuration.ConfigurationManager.AppSettings["GenomesArchive"] +
				"\\" + this.ScriptName + "\\";
			//default report with numIntervalDays = 1
			AccountReport accountReport = this.account.CreateReport(fileName,1,
			                                                        this.endOfDayTimer.GetCurrentDateTime(),
			                                                        this.benchmark,
			                                                        new HistoricalAdjustedQuoteProvider());
			this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveReports);
			ObjectArchiver.Archive(accountReport,
			                       dirNameWhereToSaveReports +
			                       fileName + ".qPr");
			//
			//      this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveTransactions);
			//      ObjectArchiver.Archive(this.account.Transactions,
			//                             dirNameWhereToSaveTransactions +
			//                             fileName + ".qPt");
			//
			this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveBestGenomes);
			OptimizationOutput optimizationOutput = new OptimizationOutput();
			foreach(GenomeRepresentation genomeRepresentation in this.endOfDayTimerHandler.BestGenomes)
				optimizationOutput.Add(genomeRepresentation);
			ObjectArchiver.Archive(optimizationOutput,
			                       dirNameWhereToSaveBestGenomes +
			                       fileName + ".bgn");
			this.endOfDayTimer.Stop();
			
		}
		
		
		#endregion
	}
}
