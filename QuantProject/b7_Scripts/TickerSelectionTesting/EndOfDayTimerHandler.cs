/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandler.cs
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
  /// Base class for EndOfDayTimerHandlers for efficient portfolios
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandler
  {
    protected DataTable eligibleTickers;
    protected string[] chosenTickers;
    protected string[] lastChosenTickers;
    
    protected string tickerGroupID;
    protected int numberOfEligibleTickers;
    protected int numberOfTickersToBeChosen;
    protected int numDaysForLiquidity;
    protected int generationNumberForGeneticOptimizer;
    protected int populationSizeForGeneticOptimizer;
		
    protected Account account;
    protected ArrayList orders;

    protected string benchmark;
    //these two values have to be updated during
    //backtest, for minimizing commission amount,
    //according to broker's commission scheme 
    protected double minPriceForMinimumCommission = 0;
    protected double maxPriceForMinimumCommission = 500;
    
    protected double targetReturn;
    
    protected PortfolioType portfolioType;
    

    public int NumberOfEligibleTickers
    {
      get { return this.numberOfEligibleTickers; }
    }
		
    public Account Account
    {
      get { return this.account; }
    }
		
    public EndOfDayTimerHandler(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysForLiquidity, Account account,
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark, double targetReturn,
                                PortfolioType portfolioType)
    {
      this.tickerGroupID = tickerGroupID;
      this.numberOfEligibleTickers = numberOfEligibleTickers;
      this.numberOfTickersToBeChosen = numberOfTickersToBeChosen;
      this.numDaysForLiquidity = numDaysForLiquidity;
      this.account = account;
      this.generationNumberForGeneticOptimizer = generationNumberForGeneticOptimizer;
      this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
      this.benchmark = benchmark;
      this.orders = new ArrayList();
      this.chosenTickers = new string[numberOfTickersToBeChosen];
      this.lastChosenTickers = new string[numberOfTickersToBeChosen];
      this.targetReturn = targetReturn;
      this.portfolioType = portfolioType;
    }
		
    public virtual void AddOrderForTicker(string ticker )
    {
 
      double cashForSinglePosition = this.account.CashAmount / this.numberOfTickersToBeChosen;
      long quantity =
        Convert.ToInt64( Math.Floor( cashForSinglePosition / this.account.DataStreamer.GetCurrentBid( ticker ) ) );
      Order order;
      if(this.portfolioType == PortfolioType.OnlyLong)
          order = new Order( OrderType.MarketBuy, new Instrument( ticker ) , quantity );
      else// only short orders are permitted
      		order = new Order( OrderType.MarketSellShort, new Instrument( ticker ) , quantity );
      
      this.orders.Add(order);
    }
    
    public virtual void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      ;
    }
    public virtual void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      ;
    }
    public virtual void OneHourAfterMarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      ;
    }
  } // end of class
}
