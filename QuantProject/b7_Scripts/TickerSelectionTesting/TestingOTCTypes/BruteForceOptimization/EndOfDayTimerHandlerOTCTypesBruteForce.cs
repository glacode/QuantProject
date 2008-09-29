/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerOTCTypesBruteForce.cs
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
using QuantProject.ADT.Optimizing.BruteForce;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TickerSelectionTesting.TestingOTCTypes.BruteForceOptimization
{
	
  /// <summary>
  /// Implements MarketOpenEventHandler,
  /// MarketCloseEventHandler and OneHourAfterMarketCloseEventHandler
  /// These handlers run all the 
  /// OTC strategy types after a common optimization, for testing purposes
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerOTCTypesBruteForce :
  	QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios.EndOfDayTimerHandler
  {
    private int seedForRandomGenerator;
    private Account[] accounts;
    int numOfClosesWithOpenPositionsFor2DaysStrategy;
    
    
    public EndOfDayTimerHandlerOTCTypesBruteForce(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
                                string benchmark, int numDaysBetweenEachOptimization,
                                Account[] accounts):
                                base(tickerGroupID, numberOfEligibleTickers, 
                                numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
                                1,
                                100,
                                benchmark, 0.0,
                                PortfolioType.ShortAndLong)
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

    private void marketCloseEventHandler_reversePositionsForTheAccount(int accountNumber)
    {
      AccountManager.ReversePositions(this.accounts[accountNumber]);
    }

    /// <summary>
    /// Handles a "Market Open" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    protected override void marketOpenEventHandler(
      Object sender , DateTime dateTime )
    {
      for(int i = 0; i<this.accounts.Length; i++)
      {
        //add cash first for each account
        if( this.accounts[i].Transactions.Count == 0)
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
	          
    protected override void marketCloseEventHandler(
      Object sender , DateTime dateTime )
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
          this.marketCloseEventHandler_reversePositionsForTheAccount(i);
      }
    }
   
		#region oneHourAfterMarketCloseEventHandler
      
    protected DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
    {
      SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID, currentDate);
      DataTable tickersFromGroup = temporizedGroup.GetTableOfSelectedTickers();
      
      SelectorByAverageRawOpenPrice byPrice = 
      		new SelectorByAverageRawOpenPrice(tickersFromGroup,false,currentDate.AddDays(-30),
      	                                  	currentDate,
      	                                  	tickersFromGroup.Rows.Count,
      	                                  	25,500, 0.0001,100);
      	                                  
      
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
    
    private double[] setTickers_getWeights(string[] tickers)
    {
    	double[] returnValue = new double[tickers.Length];
    	for(int i = 0;i<tickers.Length; i++)
    		returnValue[i] = 1.0/tickers.Length;
    	return returnValue;
    }
    
    protected virtual void setTickers(DateTime currentDate,
                                     	bool setGenomeCounter)
    {
      
      DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
      if(setOfTickersToBeOptimized.Rows.Count > this.numberOfTickersToBeChosen*2)
        //the optimization process is possible only if the initial set of tickers is 
        //as large as the number of tickers to be chosen                     
      
      {
        
      	OTCBruteForceOptimizableParametersManager otcBruteForceParamManager = 
      		new OTCBruteForceOptimizableParametersManager(
      		           setOfTickersToBeOptimized,
      		           currentDate.AddDays(-this.numDaysForOptimizationPeriod),
      		           currentDate,this.numberOfTickersToBeChosen);
        
      	BruteForceOptimizer BFO = new BruteForceOptimizer(otcBruteForceParamManager,1);
      	BFO.Run();
        	//this.setTickers_getChosenTickers(BFO.BestParameters);
      	this.addGenomeToBestGenomes(BFO.BestParameters,this.chosenWeightedPositions.SignedTickers,
      	                            currentDate.AddDays(-this.numDaysForOptimizationPeriod),
      	                            currentDate,setOfTickersToBeOptimized.Rows.Count);
				this.chosenWeightedPositions = 
					new WeightedPositions(new SignedTickers(((GenomeMeaning)otcBruteForceParamManager.Decode(BFO.BestParameters)).Tickers));
        
      }
      //else it will be buyed again the previous optimized portfolio
      //that's it the actual chosenTickers member
    }

    /// <summary>
    /// Handles a "One hour after market close" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    protected override void oneHourAfterMarketCloseEventHandler(
      Object sender , DateTime dateTime )
    {
    	this.seedForRandomGenerator++;
      if(this.numDaysElapsedSinceLastOptimization == 
    	   this.numDaysBetweenEachOptimization - 1)
    	{
    		this.setTickers(dateTime, false);
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
