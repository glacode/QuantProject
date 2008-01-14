/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerWeightedBalancedPVO.cs
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
using System.Data;
using System.Collections;

using QuantProject.ADT;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Data;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.Data.DataTables;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.WalkForwardTesting.LinearCombination;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.WeightedPVO.WeightedBalancedPVO
{
	/// <summary>
  /// Implements MarketOpenEventHandler and MarketCloseEventHandler
  /// These handlers contain the core strategy for the Portfolio Value
  /// Oscillator using different weights for tickers, in such a way that
  /// portfolio is invested for the 50% in long positions,
  /// and the other 50% in short positions (also with an odd number
  /// of tickers in portfolio)
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerWeightedBalancedPVO : EndOfDayTimerHandlerPVO
  {
            
    public EndOfDayTimerHandlerWeightedBalancedPVO(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
                                Account account,                                
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark,
                                int numDaysForOscillatingPeriod,
                                int minLevelForOversoldThreshold,
                                int maxLevelForOversoldThreshold,
                                int minLevelForOverboughtThreshold,
                                int maxLevelForOverboughtThreshold,
                                int divisorForThresholdComputation,
                                bool symmetricalThresholds,
                                bool overboughtMoreThanOversoldForFixedPortfolio,
                                int numDaysBetweenEachOptimization,
                                PortfolioType portfolioType, double maxAcceptableCloseToCloseDrawdown,
                                double minimumAcceptableGain):
    														base(tickerGroupID, numberOfEligibleTickers, 
                                numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
                                account,                                
                                generationNumberForGeneticOptimizer,
                                populationSizeForGeneticOptimizer,
                                benchmark,
                                numDaysForOscillatingPeriod,
                                minLevelForOversoldThreshold,
                                maxLevelForOversoldThreshold,
                                minLevelForOverboughtThreshold,
                                maxLevelForOverboughtThreshold,
                                divisorForThresholdComputation,
                                symmetricalThresholds,
                                overboughtMoreThanOversoldForFixedPortfolio,
                                numDaysBetweenEachOptimization,
                                portfolioType, maxAcceptableCloseToCloseDrawdown,
                                minimumAcceptableGain)
    {
      
    }
  	
    protected override DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
    {
			SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID, currentDate);
      DataTable tickersFromGroup = temporizedGroup.GetTableOfSelectedTickers();
      int numOfTickersInGroupAtCurrentDate = tickersFromGroup.Rows.Count;
      
      SelectorByAverageRawOpenPrice byPrice =
      		new SelectorByAverageRawOpenPrice(tickersFromGroup,false,currentDate.AddDays(-30),
      	                                  currentDate,
      	                                  numOfTickersInGroupAtCurrentDate,
      	                                  20,500, 0.0001,100);
 
      SelectorByLiquidity mostLiquidSelector =
      	new SelectorByLiquidity(byPrice.GetTableOfSelectedTickers(),
        false,currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
        this.numberOfEligibleTickers);
      
//      SelectorByCloseToCloseCorrelationToBenchmark byCorrelationToBenchmark =
//      	new SelectorByCloseToCloseCorrelationToBenchmark(mostLiquidSelector.GetTableOfSelectedTickers(),
//      	                                                 "^GSPC",false,
//      	                                                 currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
//      	                                                 this.numberOfEligibleTickers/2,false);
//      
      
      SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromLastSelection = 
        new SelectorByQuotationAtEachMarketDay(mostLiquidSelector.GetTableOfSelectedTickers(),
        false, currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
        this.numberOfEligibleTickers, this.benchmark);
     
      return quotedAtEachMarketDayFromLastSelection.GetTableOfSelectedTickers(); 
    	
    }
    
    
    protected override void setTickers(DateTime currentDate,
      bool setGenomeCounter)
    {
      DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
      if(setOfTickersToBeOptimized.Rows.Count > this.numberOfTickersToBeChosen*2)
        //the optimization process is meaningful only if the initial set of tickers is 
        //larger than the number of tickers to be chosen                     
      
      {
        this.iGenomeManager =
          new GenomeManagerWeightedBalancedPVO(setOfTickersToBeOptimized,
          currentDate.AddDays(-this.numDaysForOptimizationPeriod), 
          currentDate, this.numberOfTickersToBeChosen,
          this.numDaysForOscillatingPeriod,
          this.minLevelForOversoldThreshold,
          this.maxLevelForOversoldThreshold,
          this.minLevelForOverboughtThreshold,
          this.maxLevelForOverboughtThreshold,
          this.divisorForThresholdComputation,
          this.symmetricalThresholds,
          this.overboughtMoreThanOversoldForFixedPortfolio,
          this.portfolioType, this.benchmark);
        GeneticOptimizer GO = new GeneticOptimizer(this.iGenomeManager,
          this.populationSizeForGeneticOptimizer, 
          this.generationNumberForGeneticOptimizer,
          this.seedForRandomGenerator);
        if(setGenomeCounter)
          this.genomeCounter = new GenomeCounter(GO);
        GO.MutationRate = 0.2;
        GO.Run(false);
        
				this.chosenWeightedPositions = new WeightedPositions( ((GenomeMeaningPVO)GO.BestGenome.Meaning).TickersPortfolioWeights,
					new SignedTickers( ((GenomeMeaningPVO)GO.BestGenome.Meaning).Tickers) );
        this.currentOversoldThreshold = ((GenomeMeaningPVO)GO.BestGenome.Meaning).OversoldThreshold;
        this.currentOverboughtThreshold = ((GenomeMeaningPVO)GO.BestGenome.Meaning).OverboughtThreshold;
        this.addPVOGenomeToBestGenomes(GO.BestGenome,((GenomeManagerForEfficientPortfolio)this.iGenomeManager).FirstQuoteDate,
          ((GenomeManagerForEfficientPortfolio)this.iGenomeManager).LastQuoteDate, setOfTickersToBeOptimized.Rows.Count,
           this.numDaysForOscillatingPeriod, this.portfolioType, GO.GenerationCounter,
           this.currentOversoldThreshold, this.currentOverboughtThreshold);
      }
      //else it will be buyed again the previous optimized portfolio
      //that's it the actual chosenTickers member
    }
  }
}
