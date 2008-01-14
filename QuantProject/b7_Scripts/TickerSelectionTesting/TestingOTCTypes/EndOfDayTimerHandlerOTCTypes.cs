/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerOTCTypes.cs
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
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.ADT.Optimizing.Genetic;

namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	/// <summary>
  /// Implements MarketOpenEventHandler,
  /// MarketCloseEventHandler and OneHourAfterMarketCloseEventHandler
  /// These handlers run all the 
  /// OTC strategy types after a common optimization, for testing purposes
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerOTCTypes : EndOfDayTimerHandler
  {
    private int seedForRandomGenerator;
    private Account[] accounts;
    int numOfClosesWithOpenPositionsFor2DaysStrategy;
       
    public EndOfDayTimerHandlerOTCTypes(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark, double targetReturn,
                                PortfolioType portfolioType, int numDaysBetweenEachOptimization,
                                Account[] accounts):
                                base(tickerGroupID, numberOfEligibleTickers, 
                                numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
                                generationNumberForGeneticOptimizer,
                                populationSizeForGeneticOptimizer,
                                benchmark, targetReturn,
                                portfolioType)
    {
    	this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;  
    	this.numDaysElapsedSinceLastOptimization = 0;
    	this.seedForRandomGenerator = ConstantsProvider.SeedForRandomGenerator;
      this.accounts = accounts;
    }
 
    private void openPositionsForTheAccountWhenPortfolioIsEmpty(int accountNumber)
    {
      if(this.accounts[accountNumber].Portfolio.Count == 0)
      	AccountManager.OpenPositions(this.chosenWeightedPositions, this.accounts[accountNumber]);
    }

    private void closePositionsForTheAccount(int accountNumber)
    {
    	AccountManager.ClosePositions(this.accounts[accountNumber]);
    }
    
    /// <summary>
    /// Handles a "Market Open" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public override void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      for(int i = 0; i<this.accounts.Length; i++)
      {
        //add cash first for each account
        if(this.accounts[i].Transactions.Count == 0)
          this.accounts[i].AddCash(15000);  
        
        if(i<=1)//daily classical and multiday
           this.openPositionsForTheAccountWhenPortfolioIsEmpty(i);
   
        if(i==2)//for the CTO OTC
        {
          this.closePositionsForTheAccount(i);
          this.openPositionsForTheAccountWhenPortfolioIsEmpty(i);
        }
        if(i==3)//for the CTO, no position is opened
          //at market open. Any open position is closed, instead
          this.closePositionsForTheAccount(i);	
      }
    }
	          
    public override void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
     	if(this.accounts[1].Portfolio.Count > 0)
 		    numOfClosesWithOpenPositionsFor2DaysStrategy++;
      
      for(int i=0; i<this.accounts.Length; i++)
      {
        if(i==0)//OTC daily account
          this.closePositionsForTheAccount(i);
        
        if(i==1)//OTC 2 days 
        {
          if(this.numOfClosesWithOpenPositionsFor2DaysStrategy == 2)
          {
            this.closePositionsForTheAccount(i);
            this.numOfClosesWithOpenPositionsFor2DaysStrategy = 0;
          }
        }
        
        if(i>=2)//for the OTC-CTO and CTO
        	AccountManager.ReversePositions(this.accounts[i]);
      }
    }
    
		#region OneHourAfterMarketCloseEventHandler
      
    protected DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
    {
      SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID, currentDate);
      DataTable tickersFromGroup = temporizedGroup.GetTableOfSelectedTickers();
      
      SelectorByAverageRawOpenPrice byPrice = 
      		new SelectorByAverageRawOpenPrice(tickersFromGroup,false,currentDate.AddDays(-30),
      	                                  currentDate,
      	                                  tickersFromGroup.Rows.Count,
      	                                  25);
      
      SelectorByLiquidity mostLiquidSelector =
      	new SelectorByLiquidity(byPrice.GetTableOfSelectedTickers(),
        false,currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
        this.numberOfEligibleTickers);
      
      SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromMostLiquid = 
        new SelectorByQuotationAtEachMarketDay(mostLiquidSelector.GetTableOfSelectedTickers(),
        false, currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
        this.numberOfEligibleTickers, this.benchmark);
     
      return quotedAtEachMarketDayFromMostLiquid.GetTableOfSelectedTickers();
    }
    
    protected virtual void setTickers(DateTime currentDate,
                                     	bool setGenomeCounter)
    {
      DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
      if(setOfTickersToBeOptimized.Rows.Count > this.numberOfTickersToBeChosen*2)
        //the optimization process is possible only if the initial set of tickers is 
        //as large as the number of tickers to be chosen                     
      
      {
        IGenomeManager genManEfficientOTCTypes = 
          new GenomeManagerForEfficientOTCCTOPortfolio(setOfTickersToBeOptimized,
        	                                          currentDate.AddDays(-this.numDaysForOptimizationPeriod),
        	                                          currentDate,
        	                                          this.numberOfTickersToBeChosen,
        	                                          this.targetReturn,
        	                                         	this.portfolioType,
        	                                         	this.benchmark);
        
        GeneticOptimizer GO = new GeneticOptimizer(genManEfficientOTCTypes,
                                                    this.populationSizeForGeneticOptimizer,
                                                    this.generationNumberForGeneticOptimizer,
                                                   this.seedForRandomGenerator);
        if(setGenomeCounter)
        	this.genomeCounter = new GenomeCounter(GO);
        GO.MutationRate = 0.25;
        GO.CrossoverRate = 0.90;
        GO.Run(false);
        this.addGenomeToBestGenomes(GO.BestGenome,currentDate.AddDays(-this.numDaysForOptimizationPeriod),
                                    currentDate,setOfTickersToBeOptimized.Rows.Count,-1, this.portfolioType,
                                    GO.GenerationCounter);
        this.chosenWeightedPositions = new WeightedPositions( ((GenomeMeaning)GO.BestGenome.Meaning).TickersPortfolioWeights,
					new SignedTickers( ((GenomeMeaning)GO.BestGenome.Meaning).Tickers) );
      }
      //else it will be buyed again the previous optimized portfolio
      //that's it the actual chosenTickers member
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
    	if(this.numDaysElapsedSinceLastOptimization == 
    	   this.numDaysBetweenEachOptimization - 1)
    	{
    		this.setTickers(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime, false);
      	//sets tickers to be chosen at next Market Open event
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
