/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerOTC_WorstAtNight.cs
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
using System.Data;
using System.Collections;

using QuantProject.ADT;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.ADT.Optimizing.Genetic;

namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	
  /// <summary>
  /// Implements MarketOpenEventHandler,
  /// TwoMinutesBeforeMarketCloseEventHandler and OneHourAfterMarketCloseEventHandler
  /// These handlers contain the core strategy for the efficient close to open portfolio!
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerOTC_WorstAtNight : EndOfDayTimerHandler
  {
    protected int seedForRandomGenerator;
		protected GeneticOptimizer currentGO;
		protected int numOfGenomesForCTOScanning;
    
    public EndOfDayTimerHandlerOTC_WorstAtNight(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod, Account account,
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark, double targetReturn,
                                PortfolioType portfolioType, int numDaysBetweenEachOptimization,
																int numOfGenomesForCTOScanning):
  															base(tickerGroupID, numberOfEligibleTickers, 
                                numberOfTickersToBeChosen, numDaysForOptimizationPeriod, account,
                                generationNumberForGeneticOptimizer,
                                populationSizeForGeneticOptimizer,
                                benchmark, targetReturn,
                                portfolioType)
    {
    	this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;  
    	this.numDaysElapsedSinceLastOptimization = 0;
    	this.seedForRandomGenerator = ConstantsProvider.SeedForRandomGenerator;
			this.numOfGenomesForCTOScanning = numOfGenomesForCTOScanning;
    }
  
		private void marketOpenEventHandler_chooseTheWorstCTOGenome()
		{
			int populationSize = this.currentGO.PopulationSize;
			int indexOfWorstCombination = populationSize - 1;
			double lossOfCurrentCombination;
			double lossOfCurrentWorstCombination = 0.0;
			double fitnessOfPreviousCombination = 0.0;
			IndexBasedEndOfDayTimer currentTimer = (IndexBasedEndOfDayTimer)this.account.EndOfDayTimer;
		  DateTime today = currentTimer.GetCurrentTime().DateTime;
		  DateTime lastMarketDay = currentTimer.GetPreviousDateTime();
		  int numOfGenomesScanned = 0; 
		  for(int i = 0;
					numOfGenomesScanned < this.numOfGenomesForCTOScanning &&
					i < populationSize - 1;
					i++)
		  {
		  	if(  i == 0 ||
		  	     ( ((Genome)this.currentGO.CurrentGeneration[populationSize - i - 1]).Fitness <
		  	     fitnessOfPreviousCombination )  )
		  	//it is the best genome or the current genome is different from - and
		  	//so it has to be strictly less of - the previous scanned genome
		  	{
		  		fitnessOfPreviousCombination =
				  		((Genome)this.currentGO.CurrentGeneration[populationSize - i - 1]).Fitness;
				  try{
		  		//tries to retrieve loss at night for the CurrentCombination
			  		SignedTickers signedTickers = 
							new SignedTickers( ((GenomeMeaning)((Genome)this.currentGO.CurrentGeneration[populationSize - i - 1]).Meaning).Tickers);
		  			WeightedPositions weightedPositions =
		  				new WeightedPositions( ((GenomeMeaning)((Genome)this.currentGO.CurrentGeneration[populationSize - i - 1]).Meaning).TickersPortfolioWeights, 
		  				                      	signedTickers);
		  			lossOfCurrentCombination =
			  			weightedPositions.GetLastNightReturn(lastMarketDay,today);
				  	numOfGenomesScanned++;
				  	if(lossOfCurrentCombination < lossOfCurrentWorstCombination)
				  	{
				  		indexOfWorstCombination = populationSize - i - 1;
				  		lossOfCurrentWorstCombination = lossOfCurrentCombination;
				  	}
		  		}
		  		catch(Exception ex){
		  		//retrieve of loss of current combination fails
		  			ex = ex;
		  		}
		  	}
		  }
		 this.chosenWeightedPositions = new WeightedPositions( ((GenomeMeaning)((Genome)this.currentGO.CurrentGeneration[indexOfWorstCombination]).Meaning).TickersPortfolioWeights,
				new SignedTickers( ((GenomeMeaning)((Genome)this.currentGO.CurrentGeneration[indexOfWorstCombination]).Meaning).Tickers));
		}
   
    /// <summary>
    /// Handles a "Market Open" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public override void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
     	if(this.currentGO != null)
			//so a list of genomes is available
					this.marketOpenEventHandler_chooseTheWorstCTOGenome();
			
			this.openPositions();
    }
		                
    public override void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
    	AccountManager.ClosePositions(this.account);
    }
    
		#region OneHourAfterMarketCloseEventHandler
      
    protected DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
    {
			SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID, currentDate);
			DataTable tickersFromGroup = temporizedGroup.GetTableOfSelectedTickers();
			SelectorByLiquidity mostLiquid =
				new SelectorByLiquidity(tickersFromGroup,
				false,currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
				this.numberOfEligibleTickers);
      
			SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromMostLiquid = 
				new SelectorByQuotationAtEachMarketDay(mostLiquid.GetTableOfSelectedTickers(),
				false, currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
				this.numberOfEligibleTickers, this.benchmark);
      
			SelectorByAverageRawOpenPrice byPrice = 
				new SelectorByAverageRawOpenPrice(quotedAtEachMarketDayFromMostLiquid.GetTableOfSelectedTickers(),
				false,currentDate.AddDays(-30),
				currentDate,
				this.numberOfEligibleTickers,
				35);
			DataTable tickersByPrice = byPrice.GetTableOfSelectedTickers();
      
			SelectorByOpenCloseCorrelationToBenchmark tickersLessCorrelatedToBenchmark = 
				new SelectorByOpenCloseCorrelationToBenchmark(tickersByPrice,
				"^GSPC",true,
				currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
				tickersByPrice.Rows.Count/2);
      
			return tickersLessCorrelatedToBenchmark.GetTableOfSelectedTickers();

    }
    
    protected virtual void setTickers(DateTime currentDate,
                                     	bool setGenomeCounter)
    {
      
      DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
      if(setOfTickersToBeOptimized.Rows.Count > this.numberOfTickersToBeChosen*2)
        //the optimization process is possible only if the initial set of tickers is 
        //as large as the number of tickers to be chosen                     
      
      {
        IGenomeManager genManEfficientOTCPortfolio = 
//          new GenomeManagerForEfficientOTCPortfolio(setOfTickersToBeOptimized,
//        	                                          currentDate.AddDays(-this.numDaysForOptimizationPeriod),
//        	                                          currentDate,
//        	                                          this.numberOfTickersToBeChosen,
//        	                                          this.targetReturn,
//        	                                         	this.portfolioType);
          new GenomeManagerForEfficientOTCCTOPortfolio(setOfTickersToBeOptimized,
                                                    currentDate.AddDays(-this.numDaysForOptimizationPeriod),
                                                    currentDate,
                                                    this.numberOfTickersToBeChosen,
                                                    this.targetReturn,
                                                    this.portfolioType);

        GeneticOptimizer GO = new GeneticOptimizer(genManEfficientOTCPortfolio,
                                                    this.populationSizeForGeneticOptimizer,
                                                    this.generationNumberForGeneticOptimizer,
                                                   this.seedForRandomGenerator);
        if(setGenomeCounter)
        	this.genomeCounter = new GenomeCounter(GO);
				GO.CrossoverRate = 0.0;
				GO.MutationRate = 0.70;
        GO.Run(false);
        this.addGenomeToBestGenomes(GO.BestGenome,currentDate.AddDays(-this.numDaysForOptimizationPeriod),
                                    currentDate, setOfTickersToBeOptimized.Rows.Count);
        this.chosenWeightedPositions = new WeightedPositions( ((GenomeMeaning)GO.BestGenome.Meaning).TickersPortfolioWeights,
					new SignedTickers( ((GenomeMeaning)GO.BestGenome.Meaning).Tickers) );
				this.currentGO = GO;
      }
      //else it will be buyed again the previous optimized portfolio
      //that's it the actual chosenTickers member
    }

    protected void oneHourAfterMarketCloseEventHandler_updatePrices()
    {
    	//min price for minimizing commission amount
    	//according to IB Broker's commission scheme
    	this.minPriceForMinimumCommission = this.account.CashAmount/(this.numberOfTickersToBeChosen*100);
      this.maxPriceForMinimumCommission = this.maxPriceForMinimumCommission;
      //just to avoid warning message
    }
    
    /// <summary>
    /// Handles a "One hour after market close" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public override void OneHourAfterMarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
    	this.seedForRandomGenerator++;
    	//this.oneHourAfterMarketCloseEventHandler_updatePrices();
      if(this.numDaysElapsedSinceLastOptimization == 
    	   this.numDaysBetweenEachOptimization - 1)
    	{
    		this.setTickers(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime, false);
      	//sets tickers to be chosen next Market Open event
      	this.numDaysElapsedSinceLastOptimization = 0;
    	}
      else
      {
        this.numDaysElapsedSinceLastOptimization++;
      }
    	
    }
		   
    #endregion
		
  }
}
