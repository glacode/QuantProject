/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerCTOMultiAccount.cs
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
  public class EndOfDayTimerHandlerCTOMultiAccount : EndOfDayTimerHandler
  {
    protected int numDaysBetweenEachOptimization;
    private int numDaysElapsedSinceLastOptimization;
    protected int seedForRandomGenerator;
    private string[,] chosenTickersForAccounts;
    private int distanceForEachGenomeToTest;
    private Account[] accounts;
    private ArrayList[] ordersForAccounts;
    private string[,] lastOrderedTickersForAccounts;
    
    public EndOfDayTimerHandlerCTOMultiAccount(string tickerGroupID, int numberOfEligibleTickers, 
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
      this.chosenTickersForAccounts = new string[numberOfAccounts,numberOfTickersToBeChosen];
      this.lastOrderedTickersForAccounts = new string[numberOfAccounts,numberOfTickersToBeChosen];
      this.ordersForAccounts = new ArrayList[numberOfAccounts];
      for(int i = 0; i<numberOfAccounts;i++)
        ordersForAccounts[i] = new ArrayList();
      this.distanceForEachGenomeToTest = distanceForEachGenomeToTest;
      this.accounts = accounts;
    }
		
    private void addOrderForTickerForEachAccount(int accountNumber, string ticker )
    {
      string tickerCode = 
        GenomeManagerForEfficientPortfolio.GetCleanTickerCode(ticker);
      double cashForSinglePosition = this.accounts[accountNumber].CashAmount / this.numberOfTickersToBeChosen;
      long quantity =
        Convert.ToInt64( Math.Floor( cashForSinglePosition / this.accounts[accountNumber].DataStreamer.GetCurrentBid( tickerCode ) ) );
      Order order;
      if(this.portfolioType == PortfolioType.OnlyShort ||
        (this.portfolioType == PortfolioType.ShortAndLong &&
        ticker != tickerCode))
        order = new Order( OrderType.MarketSellShort, new Instrument( tickerCode ) , quantity );  
      else      		
        order = new Order( OrderType.MarketBuy, new Instrument( tickerCode ) , quantity );
      
      this.ordersForAccounts[accountNumber].Add(order);
    }

    protected override void addChosenTickersToOrderList(string[] tickers)
    {
      for(int i = 0; i<this.accounts.Length; i++)
      {
        for(int j = 0; j<this.numberOfTickersToBeChosen; j++) 
        {
          string ticker = this.chosenTickersForAccounts[i,j];
          if( ticker != null)
          {  
            this.addOrderForTickerForEachAccount(i, ticker );
            this.lastOrderedTickersForAccounts[i,j] = 
              GenomeManagerForEfficientPortfolio.GetCleanTickerCode(ticker);
          }
        }
      }
    }


    protected override void openPositions()
    {
      //add cash first
      for(int i = 0; i<this.accounts.Length; i++)
      {
        if(this.ordersForAccounts[i].Count == 0 && this.accounts[i].Transactions.Count == 0)
              this.accounts[i].AddCash(17000);  
      }

      this.addChosenTickersToOrderList();
      //execute orders actually
      for(int i = 0; i<this.accounts.Length; i++)
      {
        foreach(object item in this.ordersForAccounts[i])
           this.accounts[i].AddOrder((Order)item);
      }
       
    }
    
    private void closePositions_closePositionForAccount(int accountNumber,
                                                        string ticker)
    {
      this.accounts[accountNumber].ClosePosition(ticker);   
    }
    protected override void closePositions()
    {
      string ticker;
      for(int i = 0; i<this.accounts.Length; i++)
      {
        for(int j = 0; j<this.numberOfTickersToBeChosen; j++)
        {
          ticker = this.lastOrderedTickersForAccounts[i,j];
          if( ticker != null)
          {
            if(this.accounts[i].Portfolio[ticker]!=null)
                closePositions_closePositionForAccount(i, ticker );
          }
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
        IGenomeManager genManEfficientCTOPortfolio = 
          new GenomeManagerForEfficientCTOPortfolio(setOfTickersToBeOptimized,
        	                                          currentDate.AddDays(-this.numDaysForOptimizationPeriod),
        	                                          currentDate,
        	                                          this.numberOfTickersToBeChosen,
        	                                          this.targetReturn,
        	                                         	this.portfolioType);
        
        GeneticOptimizer GO = new GeneticOptimizer(genManEfficientCTOPortfolio,
                                                    this.populationSizeForGeneticOptimizer,
                                                    this.generationNumberForGeneticOptimizer,
                                                   this.seedForRandomGenerator);
        if(setGenomeCounter)
        	this.genomeCounter = new GenomeCounter(GO);
        
        GO.Run(false);
        //this.addGenomeToBestGenomes(GO.BestGenome,currentDate.AddDays(-this.numDaysForOptimizationPeriod),
        //                            currentDate);
        for(int i = 0; i<this.accounts.Length; i++)
        {
          for(int j = 0; j<this.numberOfTickersToBeChosen; j++)
            this.chosenTickersForAccounts[i,j] =
                ((string[])((Genome)GO.CurrentGeneration[GO.CurrentGeneration.Count - 1 -i*this.distanceForEachGenomeToTest]).Meaning)[j];
        }
        
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
    	foreach(ArrayList arrayList in this.ordersForAccounts)
        arrayList.Clear();
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
