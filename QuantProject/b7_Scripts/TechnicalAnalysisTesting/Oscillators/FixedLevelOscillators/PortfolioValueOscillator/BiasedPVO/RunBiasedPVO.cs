/*
QuantProject - Quantitative Finance Library

RunBiasedPVO.cs
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
using System.IO;

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
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator;


namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.BiasedPVO
{
	/// <summary>
	/// Script that implements the PVO strategy,
	/// with this difference:
	/// when it is time to open new positions,
	/// it is not simply chosen the genome with the highest fitness
	/// (as in the PVO base), but it is chosen the genome (from a
	/// given set of genomes with a good fitness) that
	/// has the highest degree of deviation from a threshold
	/// </summary>
	[Serializable]
	public class RunBiasedPVO : RunPVO
	{
		protected double minPriceForTickersToBeChosen;
		protected double maxPriceForTickersToBeChosen;
		protected int numOfDifferentGenomesToEvaluateOutOfSample;
		protected int numDaysOfStayingOnTheMarket;
		protected double maxCoefficientForDegreeComputationOfCrossingThreshold;
		protected double numOfStdDevForThresholdsComputation;
		protected bool resetThresholdsBeforeCheckingOutOfSample;
		protected bool buyOnlyPositionsThatAreMovingTogether;
		protected bool doNotOpenReversedPositionsThatHaveJustBeenClosed;
		protected int numDaysForThresholdsReComputation;
		
		protected string pathOfFileContainingGenomes;
		public string PathOfFileContainingGenomes
		{
			get { return pathOfFileContainingGenomes; }
			set { pathOfFileContainingGenomes = value; }
		}
		
		public RunBiasedPVO(string tickerGroupID, int maxNumOfEligibleTickersForOptimization,
		                    double minPriceForTickersToBeChosen,
		                    double maxPriceForTickersToBeChosen,
		                    int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
		                    int generationNumberForGeneticOptimizer,
		                    int populationSizeForGeneticOptimizer, string benchmark,
		                    DateTime startDate, DateTime endDate,
		                    int numOfDifferentGenomesToEvaluateOutOfSample,
		                    bool resetThresholdsBeforeCheckingOutOfSample,
		                    int numDaysForThresholdsReComputation,
		                    double numOfStdDevForThresholdsComputation,
		                    double maxCoefficientForDegreeComputationOfCrossingThreshold,
		                    bool buyOnlyPositionsThatAreMovingTogether,
		                    bool doNotOpenReversedPositionsThatHaveJustBeenClosed,
		                    int numDaysForOscillatingPeriod,
		                    int numDaysOfStayingOnTheMarket,
		                    int minLevelForOversoldThreshold,
		                    int maxLevelForOversoldThreshold,
		                    int minLevelForOverboughtThreshold,
		                    int maxLevelForOverboughtThreshold,
		                    int divisorForThresholdComputation,
		                    bool symmetricalThresholds,
		                    bool overboughtMoreThanOversoldForFixedPortfolio,
		                    int numDaysBetweenEachOptimization,
		                    PortfolioType inSamplePortfolioType, double maxAcceptableCloseToCloseDrawdown,
		                    double minimumAcceptableGain, double maxRunningHours):
			base(tickerGroupID, maxNumOfEligibleTickersForOptimization,
			     numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
			     generationNumberForGeneticOptimizer,
			     populationSizeForGeneticOptimizer, benchmark,
			     startDate, endDate,
			     numDaysForOscillatingPeriod,
			     minLevelForOversoldThreshold,
			     maxLevelForOversoldThreshold,
			     minLevelForOverboughtThreshold,
			     maxLevelForOverboughtThreshold,
			     divisorForThresholdComputation,
			     symmetricalThresholds,
			     overboughtMoreThanOversoldForFixedPortfolio,
			     numDaysBetweenEachOptimization,
			     inSamplePortfolioType, maxAcceptableCloseToCloseDrawdown,
			     minimumAcceptableGain,
			     maxRunningHours)
		{
			this.minPriceForTickersToBeChosen = minPriceForTickersToBeChosen;
			this.maxPriceForTickersToBeChosen = maxPriceForTickersToBeChosen;
			this.numOfDifferentGenomesToEvaluateOutOfSample = numOfDifferentGenomesToEvaluateOutOfSample;
			this.minimumAcceptableGain = minimumAcceptableGain;
			//this.ScriptName = "PVO_Biased_WithWeightsPriceSel";
			this.ScriptName = "PVO_Biased_NoWeightsPriceSel";
			this.numDaysOfStayingOnTheMarket = numDaysOfStayingOnTheMarket;
			this.maxCoefficientForDegreeComputationOfCrossingThreshold = maxCoefficientForDegreeComputationOfCrossingThreshold;
			this.resetThresholdsBeforeCheckingOutOfSample = resetThresholdsBeforeCheckingOutOfSample;
			this.numDaysForThresholdsReComputation = numDaysForThresholdsReComputation;
			this.numOfStdDevForThresholdsComputation = numOfStdDevForThresholdsComputation;
			this.buyOnlyPositionsThatAreMovingTogether = buyOnlyPositionsThatAreMovingTogether;
			this.doNotOpenReversedPositionsThatHaveJustBeenClosed = doNotOpenReversedPositionsThatHaveJustBeenClosed;
			this.pathOfFileContainingGenomes = null;
			//if this field is set to null, selections of tickers (with
			//optimization), takes place; otherwise, tickers for
			//out of sample testing are chosen from a given
			//set of genomes saved to disk (representing a set of
			//a certain number of optimizations run over a given period)
		}

		#region auxiliary overriden methods for Run
		
		protected override void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler = new EndOfDayTimerHandlerBiasedPVO(this.tickerGroupID, this.numberOfEligibleTickers,
			                                                              this.minPriceForTickersToBeChosen,
			                                                              this.maxPriceForTickersToBeChosen,
			                                                              this.numberOfTickersToBeChosen, this.numDaysForOptimizationPeriod,
			                                                              this.account,
			                                                              this.PathOfFileContainingGenomes,
			                                                              this.generationNumberForGeneticOptimizer,
			                                                              this.populationSizeForGeneticOptimizer, this.benchmark,
			                                                              this.numOfDifferentGenomesToEvaluateOutOfSample,
			                                                              this.resetThresholdsBeforeCheckingOutOfSample,
			                                                              this.numDaysForThresholdsReComputation,
			                                                              this.numOfStdDevForThresholdsComputation,
			                                                              this.maxCoefficientForDegreeComputationOfCrossingThreshold,
			                                                              this.buyOnlyPositionsThatAreMovingTogether,
			                                                              this.doNotOpenReversedPositionsThatHaveJustBeenClosed,
			                                                              this.numDaysForOscillatingPeriod,
			                                                              this.numDaysOfStayingOnTheMarket,
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
		
		
		protected void saveScriptResults_saveScriptFeaturesToLogFile(string nameForScriptFiles)
		{
			string pathFile =
				System.Configuration.ConfigurationManager.AppSettings["ReportsArchive"] +
				"\\" + this.ScriptName + "\\" + nameForScriptFiles + ".txt";
			StreamWriter w = File.AppendText(pathFile);
			w.WriteLine ("\n----------------------------------------------\r\n");
			w.Write("\r\nScript file: {0}\r", nameForScriptFiles);
			w.Write("\r\nSelection of tickers from group: {0}\r", this.tickerGroupID);
			w.Write("\r\nSelect eligible from tickers with minimum price (in the last 30 days): {0}\r", this.minPriceForTickersToBeChosen.ToString());
			w.Write("\r\nSelect eligible from tickers with maximum price (in the last 30 days): {0}\r", this.maxPriceForTickersToBeChosen.ToString());
			w.Write("\r\nMax num of eligible tickers for optimization (most liquid): {0}\r", this.numberOfEligibleTickers.ToString());
			w.Write("\r\nNum of tickers in portfolio: {0}\r", this.numberOfTickersToBeChosen.ToString());
			w.Write("\r\nPath name file of already optimized genomes: {0}\r", this.PathOfFileContainingGenomes);
			w.Write("\r\nLength in days for optimization period: {0}\r", this.numDaysForOptimizationPeriod.ToString());
			w.Write("\r\nGeneration number for genetic optimizer: {0}\r", this.generationNumberForGeneticOptimizer.ToString());
			w.Write("\r\nPopulation size for genetic optimizer: {0}\r", this.populationSizeForGeneticOptimizer.ToString());
			w.Write("\r\nOptimization each (num of days): {0}\r", this.numDaysBetweenEachOptimization.ToString());
			w.Write("\r\nBenchmark: {0}\r", this.benchmark);
			w.Write("\r\nStart date: {0}\r", this.startDateTime.ToLongDateString());
			w.Write("\r\nEnd date: {0}\r", this.endDateTime.ToLongDateString());
			w.Write("\r\nNum of genomes to check out of sample: {0}\r", this.numOfDifferentGenomesToEvaluateOutOfSample.ToString());
			w.Write("\r\nReset thresholds out of sample: {0}\r", this.resetThresholdsBeforeCheckingOutOfSample.ToString());
			w.Write("\r\nNum of Days for thresholds recomputation out of sample: {0}\r", this.numDaysForThresholdsReComputation.ToString());
			w.Write("\r\nNum of standard deviation from average for setting thresholds out of sample: {0}\r", this.numOfStdDevForThresholdsComputation.ToString());
			w.Write("\r\nDiscard genome if degree of crossing threshold is {0} * threshold away\r", this.maxCoefficientForDegreeComputationOfCrossingThreshold.ToString());
			w.Write("\r\nBuy only positions that are moving together: {0}\r", this.buyOnlyPositionsThatAreMovingTogether.ToString());
			w.Write("\r\nDo not reverse positions that have just been closed: {0}\r", this.doNotOpenReversedPositionsThatHaveJustBeenClosed.ToString());
			w.Write("\r\nLength return (oscillating period): {0}\r", this.numDaysForOscillatingPeriod.ToString());
			w.Write("\r\nNumDays of staying on the market: {0}\r", this.numDaysOfStayingOnTheMarket.ToString());
			w.Write("\r\nMin level for oversold threshold: {0}\r", this.minLevelForOversoldThreshold.ToString());
			w.Write("\r\nMax level for oversold threshold: {0}\r", this.maxLevelForOversoldThreshold.ToString());
			w.Write("\r\nMin level for overbought threshold: {0}\r", this.minLevelForOverboughtThreshold.ToString());
			w.Write("\r\nMax level for overbought threshold: {0}\r", this.maxLevelForOverboughtThreshold.ToString());
			w.Write("\r\nDivisor for threshold computation: {0}\r", this.divisorForThresholdComputation.ToString());
			w.Write("\r\nSymmetrical thresholds: {0}\r", this.symmetricalThresholds.ToString());
			w.Write("\r\nOverbought more than oversold: {0}\r", this.overboughtMoreThanOversoldForFixedPortfolio.ToString());
			w.Write("\r\nSymmetrical thresholds: {0}\r", this.symmetricalThresholds.ToString());
			w.Write("\r\nNum days between each optimization: {0}\r", this.numDaysBetweenEachOptimization.ToString());
			w.Write("\r\nIn sample portfolio type: {0}\r", this.portfolioType.ToString());
			w.Write("\r\nMax acceptable draw down: {0}\r", this.maxAcceptableCloseToCloseDrawdown.ToString());
			w.Write("\r\nMinimum acceptable gain: {0}\r", this.minimumAcceptableGain.ToString());
			w.Write("\r\nMax running hours: {0}\r", this.maxRunningHours.ToString());
			w.WriteLine ("\n----------------------------------------------");
			//w.Write("\r\nFitnesses compared (sharpe r. OTC): {0}\r", this.fitnessesInSample.Length.ToString());
			// Update the underlying file.
			w.Flush();
			w.Close();
		}

		public override void SaveScriptResults()
		{
			string fileName = DateTime.Now.Hour.ToString().PadLeft(2,'0') + "_" +
				DateTime.Now.Minute.ToString().PadLeft(2,'0') + "_" +
				DateTime.Now.Second.ToString().PadLeft(2,'0') + "_" +
				this.scriptName +  "GenOS_" + this.numOfDifferentGenomesToEvaluateOutOfSample +
				"_OsDays_" + numDaysForOscillatingPeriod +
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
			if( this.PathOfFileContainingGenomes == null )
			{
				OptimizationOutput optimizationOutput = new OptimizationOutput();
				foreach(GenomeRepresentation genomeRepresentation in this.endOfDayTimerHandler.BestGenomes)
					optimizationOutput.Add(genomeRepresentation);
				ObjectArchiver.Archive(optimizationOutput,
				                       dirNameWhereToSaveBestGenomes +
				                       fileName + ".bgn");
			}
			this.checkDateForReport_createDirIfNotPresent(dirNameWhereToSaveReports);
			AccountReport accountReport = this.account.CreateReport(fileName,1,
			                                                        this.endOfDayTimer.GetCurrentDateTime(),
			                                                        this.benchmark,
			                                                        new HistoricalAdjustedQuoteProvider());
			ObjectArchiver.Archive(accountReport,
			                       dirNameWhereToSaveReports +
			                       fileName + ".qPr");
			this.saveScriptResults_saveScriptFeaturesToLogFile(fileName);
			this.endOfDayTimer.Stop();
		}
		
		
		#endregion
	}
}
