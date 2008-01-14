/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerOTCMultiAccount.cs
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
  /// TwoMinutesBeforeMarketCloseEventHandler and OneHourAfterMarketCloseEventHandler
  /// These handlers contain the core strategy for the efficient open to close portfolio!
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerOTCMultiAccount : EndOfDayTimerHandler
  {
    protected int seedForRandomGenerator;
    private WeightedPositions[] chosenWeightedPositionsForAccounts;
    private int distanceForEachGenomeToTest;
    private Account[] accounts;
    
    public EndOfDayTimerHandlerOTCMultiAccount(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
                                Account[] accounts,
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark, double targetReturn,
                                PortfolioType portfolioType, int numDaysBetweenEachOptimization, int numberOfAccounts,
                                int distanceForEachGenomeToTest):
  															base(tickerGroupID, numberOfEligibleTickers, 
                                numberOfTickersToBeChosen, numDaysForOptimizationPeriod, accounts[0],
                                generationNumberForGeneticOptimizer,
                                populationSizeForGeneticOptimizer,
                                benchmark, targetReturn,
                                portfolioType)
    {
    	this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;  
    	this.numDaysElapsedSinceLastOptimization = 0;
    	this.seedForRandomGenerator = ConstantsProvider.SeedForRandomGenerator;
      this.chosenWeightedPositionsForAccounts = new WeightedPositions[numberOfAccounts];
      this.distanceForEachGenomeToTest = distanceForEachGenomeToTest;
      this.accounts = accounts;
    }
    
    private void openPositions(WeightedPositions[] chosenWeightedPositionsForAccounts)
    {
      //add cash first
      for ( int i = 0; i<this.accounts.Length; i++ )
        if ( this.accounts[i].Transactions.Count == 0 )
					this.accounts[i].AddCash(15000);  
      //execute orders actually
      for(int i = 0; i<this.accounts.Length; i++)
        AccountManager.OpenPositions(this.chosenWeightedPositionsForAccounts[i],
																		this.accounts[i]);
    }
    
		/// <summary>
    /// Handles a "Market Open" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public override void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
    	//temporarily the if condition
    	//if(this.numDaysElapsedSinceLastOptimization == 0)
    		this.openPositions(this.chosenWeightedPositionsForAccounts);
    }
		
                
    public override void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
    	
    	//temporarily
    	//if(this.numDaysElapsedSinceLastOptimization ==
    	//   this.numDaysBetweenEachOptimization)
			for(int i = 0; i<this.accounts.Length; i++)
				AccountManager.ClosePositions( this.accounts[i] );
    }

		#region OneHourAfterMarketCloseEventHandler
      
    protected DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
    {
      /*
      SelectorByAverageRawOpenPrice selectorByOpenPrice = 
                  new SelectorByAverageRawOpenPrice(this.tickerGroupID, false,
                          currentDate.AddDays(-this.numDaysForLiquidity), currentDate,
                          this.numberOfEligibleTickers, this.minPriceForMinimumCommission,
                          this.maxPriceForMinimumCommission, 0, 2);
      DataTable tickersByPrice = selectorByOpenPrice.GetTableOfSelectedTickers();
      */
     	
     	SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID, currentDate);
      SelectorByOpenCloseCorrelationToBenchmark lessCorrelatedFromTemporizedGroup = 
      	new SelectorByOpenCloseCorrelationToBenchmark(temporizedGroup.GetTableOfSelectedTickers(),
      	                                              this.benchmark,true,
      	                                              currentDate.AddDays(-this.numDaysForOptimizationPeriod ),
      	                                    					currentDate,
      	                                    					this.numberOfEligibleTickers);
      
      this.eligibleTickers = lessCorrelatedFromTemporizedGroup.GetTableOfSelectedTickers();
      SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromEligible = 
        new SelectorByQuotationAtEachMarketDay( this.eligibleTickers,
                                   false, currentDate.AddDays(-this.numDaysForOptimizationPeriod),
                                    currentDate, this.numberOfEligibleTickers, this.benchmark);
      //SelectorByWinningOpenToClose winners =
      //	new SelectorByWinningOpenToClose(quotedAtEachMarketDayFromMostLiquid.GetTableOfSelectedTickers(),
      //	                                 false, currentDate.AddDays(-2),
      //	                                 currentDate, this.numberOfEligibleTickers/4);      	                                 
      //return winners.GetTableOfSelectedTickers();
      //SelectorByOpenCloseCorrelationToBenchmark lessCorrelated = 
      //  new SelectorByOpenCloseCorrelationToBenchmark(quotedAtEachMarketDayFromEligible.GetTableOfSelectedTickers(),
      //                                                this.benchmark, true,
      //                                                currentDate.AddDays(-this.numDaysForLiquidity),
      //                                                currentDate, this.numberOfEligibleTickers/2);
      return quotedAtEachMarketDayFromEligible.GetTableOfSelectedTickers();
      //return lessCorrelated.GetTableOfSelectedTickers();
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
          new GenomeManagerForEfficientOTCPortfolio(setOfTickersToBeOptimized,
        	                                          currentDate.AddDays(-this.numDaysForOptimizationPeriod),
        	                                          currentDate,
        	                                          this.numberOfTickersToBeChosen,
        	                                          this.targetReturn,
        	                                         	this.portfolioType,
																										this.benchmark);
        
        GeneticOptimizer GO = new GeneticOptimizer(genManEfficientOTCPortfolio,
                                                    this.populationSizeForGeneticOptimizer,
                                                    this.generationNumberForGeneticOptimizer,
                                                   this.seedForRandomGenerator);
        if(setGenomeCounter)
        	this.genomeCounter = new GenomeCounter(GO);
        
        GO.Run(false);
        //this.addGenomeToBestGenomes(GO.BestGenome,currentDate.AddDays(-this.numDaysForOptimizationPeriod),
        //                            currentDate);
        SignedTickers signedTickers;
				for(int i = 0; i<this.accounts.Length; i++)
        {
          signedTickers = new SignedTickers( (string[])((Genome)GO.CurrentGeneration[GO.CurrentGeneration.Count - 1 -i*this.distanceForEachGenomeToTest]).Meaning );
					this.chosenWeightedPositionsForAccounts[i] = new WeightedPositions( signedTickers );
				}
      }
      //else it will be buyed again the previous optimized portfolio
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
