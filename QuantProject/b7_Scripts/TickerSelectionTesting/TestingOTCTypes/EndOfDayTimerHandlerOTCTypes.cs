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
    private int numDaysBetweenEachOptimization;
    private int numDaysElapsedSinceLastOptimization;
    private int seedForRandomGenerator;
    private Account[] accounts;
    //private ArrayList[] ordersForAccounts;
    
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
      //for(int i = 0; i<this.accounts.Length;i++)
        //ordersForAccounts[i] = new ArrayList();
    }
   
//    protected override void addChosenTickersToOrderList()
//    {
//      for(int i = 0; i<this.accounts.Length; i++)
//      {
//        if(i==0)//OTC daily
//        {}
//        if(i==1)
//        {}
//        if(i==2)
//        {}
//        for(int j = 0; j<this.numberOfTickersToBeChosen; j++) 
//        {
//          string ticker = this.chosenTickersForAccounts[i,j];
//          if( ticker != null)
//          {  
//            this.addOrderForTickerForEachAccount(i, ticker );
//            this.lastOrderedTickersForAccounts[i,j] = 
//              GenomeManagerForEfficientPortfolio.GetCleanTickerCode(ticker);
//          }
//        }
//      }
//    }
    private void openPositions_openWhenPortfolioIsEmpty(int accountNumber)
    {
      if(this.accounts[accountNumber].Portfolio.Count == 0)
      {
        foreach(object item in this.orders)
          this.accounts[accountNumber].AddOrder((Order)item);
      }
    }

    protected void addOrderForTickerForTheGivenAccount(int tickerPosition,
                                                       int accountNumber )
    {
    	string tickerCode = 
    		GenomeManagerForEfficientPortfolio.GetCleanTickerCode(this.chosenTickers[tickerPosition]);
      double cashForSinglePosition = 
      	this.accounts[accountNumber].CashAmount * this.chosenTickersPortfolioWeights[tickerPosition];
      long quantity =
      	Convert.ToInt64( Math.Floor( cashForSinglePosition / this.accounts[accountNumber].DataStreamer.GetCurrentBid( tickerCode ) ) );
      Order order;
      if(this.portfolioType == PortfolioType.OnlyShort ||
         		(this.portfolioType == PortfolioType.ShortAndLong &&
          this.chosenTickers[tickerPosition] != tickerCode))
        order = new Order( OrderType.MarketSellShort, new Instrument( tickerCode ) , quantity );  
      else      		
      	order = new Order( OrderType.MarketBuy, new Instrument( tickerCode ) , quantity );
      
      this.orders.Add(order);
    }
    protected void addChosenTickersToOrderListForTheGivenAccount(int accountNumber)
    {
      for( int i = 0; i<this.chosenTickers.Length; i++)
      {
      	if(this.chosenTickers[i] != null)
        {  
          this.addOrderForTickerForTheGivenAccount( i, accountNumber );
          this.lastOrderedTickers[i] = 
          	GenomeManagerForEfficientPortfolio.GetCleanTickerCode(this.chosenTickers[i]);
        }
      }
    }
    protected override void openPositions()
    {
      
      for(int i = 0; i<this.accounts.Length; i++)
      {
        //add cash first for each account
        if(this.orders.Count == 0 && this.accounts[i].Transactions.Count == 0)
              this.accounts[i].AddCash(30000);  
        
        if(i<=1)//daily classical, and multiday
        {
        	this.orders.Clear();
        	this.addChosenTickersToOrderListForTheGivenAccount(i);
        	this.openPositions_openWhenPortfolioIsEmpty(i);
        }
        	else if(i==2)//for the CTO OTC
        {
          this.closePositions_close(i);
          this.orders.Clear();
          this.addChosenTickersToOrderListForTheGivenAccount(i);
          foreach(object item in this.orders)
            this.accounts[i].AddOrder((Order)item);
        }
        else if(i==3)//for the CTO, no position is opened
        	//at market open. Any open position is closed, instead
        {
          this.closePositions_close(i);	
        }
      }
    }
 
   
    private void closePositions_close(int accountNumber)
    {
      string ticker;
      if(this.accounts[accountNumber].Portfolio.Count >0)
      {
        for(int j = 0; j<this.lastOrderedTickers.Length; j++)
        {
          ticker = this.lastOrderedTickers[j];
          if( ticker != null)
          {
            if(this.accounts[accountNumber].Portfolio[ticker]!=null)
              this.accounts[accountNumber].ClosePosition(ticker); 
          }
        }
      }
    }


    
    protected override void closePositions()
    {
      for(int i=0; i<this.accounts.Length; i++)
      {
        if(i==0)//OTC daily account
            this.closePositions_close(i);
        if(i==1)//OTC 2 days 
        {
          if(this.numDaysElapsedSinceLastOptimization ==
    	        this.numDaysBetweenEachOptimization - 1)
            this.closePositions_close(i);
        }
        if(i==2)//OTC-CTO
        {
          this.closePositions_close(i);
          if(this.numDaysElapsedSinceLastOptimization < 
            this.numDaysBetweenEachOptimization - 1)
          //open reverse positions at night
          {
            this.reverseSignOfChosenTickers();
            this.orders.Clear();
            this.addChosenTickersToOrderListForTheGivenAccount(i);
            this.openPositions_openWhenPortfolioIsEmpty(i);
            this.reverseSignOfChosenTickers();
            this.orders.Clear();
          }
        }
        if(i==3)//CTO, only at night
        {
//          this.closePositions_close(i);
          if(this.numDaysElapsedSinceLastOptimization < 
            this.numDaysBetweenEachOptimization - 1)
            //open reverse positions at night
          {
            this.reverseSignOfChosenTickers();
            this.orders.Clear();
            this.addChosenTickersToOrderListForTheGivenAccount(i);
            this.openPositions_openWhenPortfolioIsEmpty(i);
            this.reverseSignOfChosenTickers();
            this.orders.Clear();
          }
        }
      }
    }
    private void reverseSignOfChosenTickers()
    {
      for(int i = 0; i<this.chosenTickers.Length; i++)
      {
        if(this.chosenTickers[i] != null)
        {
          if(this.chosenTickers[i].StartsWith("-"))
            this.chosenTickers[i] =
              GenomeManagerForEfficientPortfolio.GetCleanTickerCode(this.chosenTickers[i]);
          else
            this.chosenTickers[i] = "-" + this.chosenTickers[i];
        }
      }
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
      if(setOfTickersToBeOptimized.Rows.Count > this.chosenTickers.Length*2)
        //the optimization process is possible only if the initial set of tickers is 
        //as large as the number of tickers to be chosen                     
      
      {
        IGenomeManager genManEfficientOTCTypes = 
          new GenomeManagerForEfficientOTCTypes(setOfTickersToBeOptimized,
        	                                          currentDate.AddDays(-this.numDaysForOptimizationPeriod),
        	                                          currentDate,
        	                                          this.numberOfTickersToBeChosen,
        	                                          this.targetReturn,
        	                                         	this.portfolioType);
        
        GeneticOptimizer GO = new GeneticOptimizer(genManEfficientOTCTypes,
                                                    this.populationSizeForGeneticOptimizer,
                                                    this.generationNumberForGeneticOptimizer,
                                                   this.seedForRandomGenerator);
        if(setGenomeCounter)
        	this.genomeCounter = new GenomeCounter(GO);
        
        GO.Run(false);
        this.addGenomeToBestGenomes(GO.BestGenome,currentDate.AddDays(-this.numDaysForOptimizationPeriod),
                                    currentDate);
        this.chosenTickers = ((GenomeMeaning)GO.BestGenome.Meaning).Tickers;
        this.chosenTickersPortfolioWeights = ((GenomeMeaning)GO.BestGenome.Meaning).TickersPortfolioWeights;
        
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
    	this.orders.Clear();
    	//this.oneHourAfterMarketCloseEventHandler_updatePrices();
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
