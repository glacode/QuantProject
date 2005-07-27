/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerCTO.cs
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
  public class EndOfDayTimerHandlerCTO : EndOfDayTimerHandler
  {
    protected int numDaysBetweenEachOptimization;
    private int numDaysElapsedSinceLastOptimization;
    
    public EndOfDayTimerHandlerCTO(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysForLiquidity, Account account,
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark, double targetReturn,
                                PortfolioType portfolioType, int numDaysBetweenEachOptimization):
  															base(tickerGroupID, numberOfEligibleTickers, 
                                numberOfTickersToBeChosen, numDaysForLiquidity, account,
                                generationNumberForGeneticOptimizer,
                                populationSizeForGeneticOptimizer,
                                benchmark, targetReturn,
                                portfolioType)
    {
    	this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;  
    	this.numDaysElapsedSinceLastOptimization = 0;
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
    		this.openPositions();
    }
		
                
    public override void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
    	
    	//temporarily
    	//if(this.numDaysElapsedSinceLastOptimization ==
    	//   this.numDaysBetweenEachOptimization)
    	 		this.closePositions();
    	  	
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
     	
     	SelectorByLiquidity mostLiquid = new SelectorByLiquidity(this.tickerGroupID, false,
                                      currentDate.AddDays(-this.numDaysForLiquidity), currentDate,
                                      this.numberOfEligibleTickers);
      /*SelectorByOpenToCloseVolatility lessVolatile = 
      	new SelectorByOpenToCloseVolatility(mostLiquid.GetTableOfSelectedTickers(),
      	                                    true, currentDate.AddDays(-5),
      	                                    currentDate,
      	                                    this.numberOfEligibleTickers/2);*/
      
      this.eligibleTickers = mostLiquid.GetTableOfSelectedTickers();
      SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromEligible = 
        new SelectorByQuotationAtEachMarketDay( this.eligibleTickers,
                                   false, currentDate.AddDays(-this.numDaysForLiquidity),
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
      if(setOfTickersToBeOptimized.Rows.Count > this.chosenTickers.Length*2)
        //the optimization process is possible only if the initial set of tickers is 
        //as large as the number of tickers to be chosen                     
      
      {
        IGenomeManager genManEfficientCTOPortfolio = 
          new GenomeManagerForEfficientCTOPortfolio(setOfTickersToBeOptimized,
        	                                          currentDate.AddDays(-this.numDaysForLiquidity),
        	                                          currentDate,
        	                                          this.numberOfTickersToBeChosen,
        	                                          this.targetReturn,
        	                                         	this.portfolioType);
        
        GeneticOptimizer GO = new GeneticOptimizer(genManEfficientCTOPortfolio,
                                                    this.populationSizeForGeneticOptimizer,
                                                    this.generationNumberForGeneticOptimizer,
                                                   ConstantsProvider.SeedForRandomGenerator);
        if(setGenomeCounter)
        	this.genomeCounter = new GenomeCounter(GO);
        
        GO.Run(false);
        
        this.chosenTickers = (string[])GO.BestGenome.Meaning;
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
    	this.orders.Clear();
    	//this.oneHourAfterMarketCloseEventHandler_updatePrices();
      if(this.numDaysElapsedSinceLastOptimization == 
    	   this.numDaysBetweenEachOptimization)
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
