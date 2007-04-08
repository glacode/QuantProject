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
    private int numOfDifferentGenomesToEvaluateOutOfSample;
    private double minimumAcceptableGain;
    
    public RunBiasedPVO(string tickerGroupID, int maxNumOfEligibleTickersForOptimization, 
                                    int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod, 
                                    int generationNumberForGeneticOptimizer,
                                    int populationSizeForGeneticOptimizer, string benchmark,
                                    DateTime startDate, DateTime endDate,
                                    int numOfDifferentGenomesToEvaluateOutOfSample,
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
                            maxRunningHours)
		{
      this.numOfDifferentGenomesToEvaluateOutOfSample = numOfDifferentGenomesToEvaluateOutOfSample;
      this.minimumAcceptableGain = minimumAcceptableGain;
      this.ScriptName = "PVO_Biased_WithWeightsPriceSel";
		}

    #region auxiliary overriden methods for Run
    
    protected override void run_initializeEndOfDayTimerHandler()
    {
      this.endOfDayTimerHandler = new EndOfDayTimerHandlerBiasedPVO(this.tickerGroupID, this.numberOfEligibleTickers,
    	                                                        this.numberOfTickersToBeChosen, this.numDaysForOptimizationPeriod,
                                                              this.account,
    	                                                        this.generationNumberForGeneticOptimizer,
    	                                                        this.populationSizeForGeneticOptimizer, this.benchmark,
    	                                                        this.numOfDifferentGenomesToEvaluateOutOfSample,
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
    
   

    public override void SaveScriptResults()
    {
    	string fileName = DateTime.Now.Hour.ToString().PadLeft(2,'0') + "_" + 
    										DateTime.Now.Minute.ToString().PadLeft(2,'0') + "_" +
    		        				this.scriptName +  "GenOS_" + this.numOfDifferentGenomesToEvaluateOutOfSample +
                        "_OsDays_" + numDaysForOscillatingPeriod +
      								"_From_" + this.tickerGroupID + "_" + this.numberOfEligibleTickers +
                      "_DaysForOpt" + this.numDaysForOptimizationPeriod + "Tick" +
                      this.numberOfTickersToBeChosen + "GenN°" + 
                      this.generationNumberForGeneticOptimizer +
                      "PopSize" + this.populationSizeForGeneticOptimizer +
        							Convert.ToString(this.portfolioType);
      string dirNameWhereToSaveReports = System.Configuration.ConfigurationSettings.AppSettings["ReportsArchive"] +
                         								"\\" + this.ScriptName + "\\";
      string dirNameWhereToSaveTransactions = System.Configuration.ConfigurationSettings.AppSettings["TransactionsArchive"] +
                       									"\\" + this.ScriptName + "\\";
      string dirNameWhereToSaveBestGenomes = System.Configuration.ConfigurationSettings.AppSettings["GenomesArchive"] +
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
                                    		this.endOfDayTimer.GetCurrentTime(),
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
