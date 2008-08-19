/*
QuantProject - Quantitative Finance Library

RunBiasedPVO_OTC.cs
Copyright (C) 2008
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
using QuantProject.Business.Financial.Accounting.Commissions;
using QuantProject.Business.Financial.Accounting.Slippage;
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
	/// Script that implements the PVO strategy using OTC returns,
	/// with this difference:
	/// when it is time to open new positions,
	/// the first genome (from a given set of genomes with a good fitness) that
	/// crosses a threshold is chosen
	/// </summary>
	[Serializable]
	public class RunBiasedPVO_OTC : RunBiasedPVO
	{
    		
    public RunBiasedPVO_OTC(string tickerGroupID, int maxNumOfEligibleTickersForOptimization,
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
		                    						minPriceForTickersToBeChosen,
		                    						maxPriceForTickersToBeChosen,
                                    numberOfTickersToBeChosen, numDaysForOptimizationPeriod, 
                                    generationNumberForGeneticOptimizer,
                                    populationSizeForGeneticOptimizer, benchmark,
                                    startDate, endDate,
                                    numOfDifferentGenomesToEvaluateOutOfSample,
                                    resetThresholdsBeforeCheckingOutOfSample,
                                    numDaysForThresholdsReComputation,
                                    numOfStdDevForThresholdsComputation,
                                    maxCoefficientForDegreeComputationOfCrossingThreshold,
                                    buyOnlyPositionsThatAreMovingTogether,
                                    doNotOpenReversedPositionsThatHaveJustBeenClosed, 1,
                                    numDaysOfStayingOnTheMarket,
                                    minLevelForOversoldThreshold,
                                    maxLevelForOversoldThreshold,
                                    minLevelForOverboughtThreshold,
                                    maxLevelForOverboughtThreshold,
                                    divisorForThresholdComputation,
                                    symmetricalThresholds,
                                    overboughtMoreThanOversoldForFixedPortfolio,
                                   	numDaysBetweenEachOptimization,
                                    inSamplePortfolioType, maxAcceptableCloseToCloseDrawdown, 
                                    minimumAcceptableGain, maxRunningHours)
		{
      this.ScriptName = "PVO_OTC_Biased_NoWeightsPriceSel";
    }
    
		protected override void run_initializeHistoricalQuoteProvider()
    {
    	this.historicalQuoteProvider = new HistoricalRawQuoteProvider();
    }
		
		protected override void run_initializeAccount()
		{
			this.account = new Account( this.scriptName , this.endOfDayTimer ,
				new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ,
				this.historicalQuoteProvider ) ,
				new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ,
				this.historicalQuoteProvider, 
				new FixedPercentageSlippageManager(
						this.historicalQuoteProvider, this.endOfDayTimer, 0.05 ) ),
				new IBCommissionManager() );
 		}
//		//no slippage, only commissions
//		protected override void run_initializeAccount()
//		{
//			this.account = new Account( this.scriptName , this.endOfDayTimer ,
//				new HistoricalEndOfDayDataStreamer( this.endOfDayTimer ,
//				this.historicalQuoteProvider ) ,
//				new HistoricalEndOfDayOrderExecutor( this.endOfDayTimer ,
//				this.historicalQuoteProvider ),
//				new IBCommissionManager() );
//		}

    protected override void run_initializeEndOfDayTimerHandler()
    {
      this.endOfDayTimerHandler = new EndOfDayTimerHandlerBiasedPVO_OTC(this.tickerGroupID, this.numberOfEligibleTickers,
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
		
		public override void SaveScriptResults()
		{
			string fileName = DateTime.Now.Hour.ToString().PadLeft(2,'0') + "_" + 
				DateTime.Now.Minute.ToString().PadLeft(2,'0') + "_" +
				DateTime.Now.Second.ToString().PadLeft(2,'0') + "_" +
				this.scriptName +  "GenOS_" + this.numOfDifferentGenomesToEvaluateOutOfSample +
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
				this.endOfDayTimer.GetCurrentTime(),
				this.benchmark,
				this.historicalQuoteProvider);
			ObjectArchiver.Archive(accountReport,
				dirNameWhereToSaveReports + 
				fileName + ".qPr");
			this.saveScriptResults_saveScriptFeaturesToLogFile(fileName);      
			this.endOfDayTimer.Stop();
		}

  }
}
