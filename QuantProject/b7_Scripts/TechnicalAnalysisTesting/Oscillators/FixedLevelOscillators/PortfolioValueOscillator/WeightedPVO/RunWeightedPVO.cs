/*
QuantProject - Quantitative Finance Library

RunWeightedPVO.cs
Copyright (C) 2006
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


namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.WeightedPVO
{
	/// <summary>
	/// Script that implements an oscillating strategy using weights for tickers,
	/// based on the classical technical indicator
	/// RelativeStrengthIndex - RSI
	/// </summary>
	[Serializable]
	public class RunWeightedPVO : RunEfficientPortfolio
	{
		protected int minLevelForOversoldThreshold;
		protected int maxLevelForOversoldThreshold;
		protected int minLevelForOverboughtThreshold;
		protected int maxLevelForOverboughtThreshold;
		protected int divisorForThresholdComputation;
		//to be used by the optimizer
		protected int numDaysBetweenEachOptimization;
		protected double maxAcceptableCloseToCloseDrawdown;
		protected double minimumAcceptableGain;
		protected int numDaysForOscillatingPeriod;
		protected bool symmetricalThresholds = false;
		protected bool overboughtMoreThanOversoldForFixedPortfolio = false;
		
		public RunWeightedPVO(string tickerGroupID, int maxNumOfEligibleTickersForOptimization,
		                      int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
		                      int generationNumberForGeneticOptimizer,
		                      int populationSizeForGeneticOptimizer, string benchmark,
		                      DateTime startDate, DateTime endDate,
		                      int numDaysForOscillatingPeriod,
		                      int minLevelForOversoldThreshold,
		                      int maxLevelForOversoldThreshold,
		                      int minLevelForOverboughtThreshold,
		                      int maxLevelForOverboughtThreshold,
		                      int divisorForThresholdComputation,
		                      bool symmetricalThresholds,
		                      bool overboughtMoreThanOversoldForFixedPortfolio,
		                      int numDaysBetweenEachOptimization,
		                      PortfolioType inSamplePortfolioType, double maxAcceptableCloseToCloseDrawdown,
		                      double minimumAcceptableGain,
		                      double maxRunningHours):
			base(tickerGroupID, maxNumOfEligibleTickersForOptimization,
			     numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
			     generationNumberForGeneticOptimizer,
			     populationSizeForGeneticOptimizer, benchmark,
			     startDate, endDate, 0.0,
			     inSamplePortfolioType, maxRunningHours)
		{
			this.ScriptName = "PVO_SR_WithWeights_PriceSel";
			this.minLevelForOversoldThreshold  = minLevelForOversoldThreshold;
			this.maxLevelForOversoldThreshold = maxLevelForOversoldThreshold;
			this.minLevelForOverboughtThreshold = minLevelForOverboughtThreshold;
			this.maxLevelForOverboughtThreshold = maxLevelForOverboughtThreshold;
			this.divisorForThresholdComputation = divisorForThresholdComputation;
			this.symmetricalThresholds = symmetricalThresholds;
			this.overboughtMoreThanOversoldForFixedPortfolio = overboughtMoreThanOversoldForFixedPortfolio;
			this.maxAcceptableCloseToCloseDrawdown = maxAcceptableCloseToCloseDrawdown;
			this.minimumAcceptableGain = minimumAcceptableGain;
			this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
		}

		#region auxiliary overriden methods for Run
		
		protected override void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler = new EndOfDayTimerHandlerWeightedPVO(this.tickerGroupID, this.numberOfEligibleTickers,
			                                                                this.numberOfTickersToBeChosen, this.numDaysForOptimizationPeriod,
			                                                                this.account,
			                                                                this.generationNumberForGeneticOptimizer,
			                                                                this.populationSizeForGeneticOptimizer, this.benchmark,
			                                                                this.numDaysForOscillatingPeriod,
			                                                                this.minLevelForOversoldThreshold,
			                                                                this.maxLevelForOversoldThreshold,
			                                                                this.minLevelForOverboughtThreshold,
			                                                                this.maxLevelForOverboughtThreshold,
			                                                                this.divisorForThresholdComputation,
			                                                                this.symmetricalThresholds,
			                                                                this.overboughtMoreThanOversoldForFixedPortfolio,
			                                                                this.numDaysBetweenEachOptimization,
			                                                                this.portfolioType, this.maxAcceptableCloseToCloseDrawdown,
			                                                                this.minimumAcceptableGain);
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
			string fileName = DateTime.Now.Hour.ToString().PadLeft(2,'0') + "_" +
				DateTime.Now.Minute.ToString().PadLeft(2,'0') + "_" +
				this.scriptName +  "OsDays_" + numDaysForOscillatingPeriod +
				"_From_" + this.tickerGroupID + "_" + this.numberOfEligibleTickers +
				"_DaysForOpt" + this.numDaysForOptimizationPeriod + "Tick" +
				this.numberOfTickersToBeChosen + "GenN°" +
				this.generationNumberForGeneticOptimizer +
				"PopSize" + this.populationSizeForGeneticOptimizer +
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
			
			this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveBestGenomes);
			OptimizationOutput optimizationOutput = new OptimizationOutput();
			foreach(GenomeRepresentation genomeRepresentation in this.endOfDayTimerHandler.BestGenomes)
				optimizationOutput.Add(genomeRepresentation);
			ObjectArchiver.Archive(optimizationOutput,
			                       dirNameWhereToSaveBestGenomes +
			                       fileName + ".bgn");
			
			//default report with numIntervalDays = 1
			AccountReport accountReport = this.account.CreateReport(fileName,1,
			                                                        this.endOfDayTimer.GetCurrentDateTime(),
			                                                        this.benchmark,
			                                                        new HistoricalAdjustedQuoteProvider());
			this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveReports);
			ObjectArchiver.Archive(accountReport,
			                       dirNameWhereToSaveReports +
			                       fileName + ".qPr");
			
			this.endOfDayTimer.Stop();
			
		}
		
		
		#endregion
	}
}
