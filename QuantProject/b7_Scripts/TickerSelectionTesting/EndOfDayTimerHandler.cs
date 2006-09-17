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
using QuantProject.Data.DataTables;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.ADT.Optimizing.BruteForce;
using QuantProject.Scripts.WalkForwardTesting.LinearCombination;

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
    protected string[] reversedTickersForOpenedPositions;
    protected double[] chosenTickersPortfolioWeights;
    protected string[] lastOrderedTickers;
    
    protected string tickerGroupID;
    protected int numberOfEligibleTickers;
    protected int numberOfTickersToBeChosen;
    protected int numDaysForOptimizationPeriod;
    protected int generationNumberForGeneticOptimizer;
    protected int populationSizeForGeneticOptimizer;
		
    protected Account account = null;
    protected ArrayList orders;

    protected string benchmark;
    //these two values have to be updated during
    //backtest, for minimizing commission amount,
    //according to broker's commission scheme 
    protected double minPriceForMinimumCommission = 30;
    protected double maxPriceForMinimumCommission = 500;
    
    protected double targetReturn;
    
    protected PortfolioType portfolioType;
    
    protected GenomeCounter genomeCounter;
    public GenomeCounter GenomeCounter
    {
      get{return this.genomeCounter;}
    }
    
    public string[] LastOrderedTickers
    {
      get { return this.lastOrderedTickers; }
    }
    public int NumberOfEligibleTickers
    {
      get { return this.numberOfEligibleTickers; }
    }
		
    public Account Account
    {
      get { return this.account; }
    }
		
    /// <summary>
    /// bestGenomes[ i ] contains an array list with the best genomes
    /// for generation i
    /// </summary>
    protected ArrayList bestGenomes;
    public ArrayList BestGenomes
    {
      get { return this.bestGenomes; }
    }
    
    private void endOfDayTimerHandler_initializeBasic(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark, double targetReturn,
                                PortfolioType portfolioType)
    {
      this.tickerGroupID = tickerGroupID;
      this.numberOfEligibleTickers = numberOfEligibleTickers;
      this.numberOfTickersToBeChosen = numberOfTickersToBeChosen;
      this.numDaysForOptimizationPeriod = numDaysForOptimizationPeriod;
      this.generationNumberForGeneticOptimizer = generationNumberForGeneticOptimizer;
      this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
      this.benchmark = benchmark;
      this.orders = new ArrayList();
      this.chosenTickers = new string[numberOfTickersToBeChosen];
      this.lastOrderedTickers = new string[numberOfTickersToBeChosen];
      this.targetReturn = targetReturn;
      this.portfolioType = portfolioType;
      this.setDefaultChosenTickersPortfolioWeights();
      
    }
    
    public EndOfDayTimerHandler(string tickerGroupID, int numberOfEligibleTickers,
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod, Account account,
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark, double targetReturn,
                                PortfolioType portfolioType)
    {
      this.endOfDayTimerHandler_initializeBasic(tickerGroupID, numberOfEligibleTickers, 
                                numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
                                generationNumberForGeneticOptimizer,
                                populationSizeForGeneticOptimizer,
                                benchmark, targetReturn,
                                portfolioType);
    	this.account = account;
      
    }
		public EndOfDayTimerHandler(string tickerGroupID, int numberOfEligibleTickers,
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark, double targetReturn,
                                PortfolioType portfolioType)
    {
      this.endOfDayTimerHandler_initializeBasic(tickerGroupID, numberOfEligibleTickers, 
                                numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
                                generationNumberForGeneticOptimizer,
                                populationSizeForGeneticOptimizer,
                                benchmark, targetReturn,
                                portfolioType);
    }
    
    
    
    public EndOfDayTimerHandler(string[] chosenTickers,
                                PortfolioType portfolioType,
                                Account account,
                                string benchmark)
    {
      
      this.account = account;
      this.benchmark = benchmark;
      this.orders = new ArrayList();
      this.chosenTickers = chosenTickers;
      this.numberOfTickersToBeChosen = chosenTickers.Length;
      this.lastOrderedTickers = new string[chosenTickers.Length];
      this.portfolioType = portfolioType;
      this.setDefaultChosenTickersPortfolioWeights();
    }
           
    private void setDefaultChosenTickersPortfolioWeights()
    {
    	this.chosenTickersPortfolioWeights = new double[this.chosenTickers.Length];
    	for(int i = 0;i<this.chosenTickers.Length;i++)
    		this.chosenTickersPortfolioWeights[i]=1.0/this.chosenTickers.Length;
		}
     
    protected virtual void addOrderForTicker(string[] tickers, 
                                             int tickerPosition )
    {
    	string tickerCode = 
    		GenomeManagerForEfficientPortfolio.GetCleanTickerCode(tickers[tickerPosition]);
      double cashForSinglePosition = 
      	this.account.CashAmount * this.chosenTickersPortfolioWeights[tickerPosition];
      long quantity =
        Convert.ToInt64( Math.Floor( cashForSinglePosition / this.account.DataStreamer.GetCurrentBid( tickerCode ) ) );
      Order order;
      if(this.portfolioType == PortfolioType.OnlyShort ||
         		(this.portfolioType == PortfolioType.ShortAndLong &&
          tickers[tickerPosition] != tickerCode))
        order = new Order( OrderType.MarketSellShort, new Instrument( tickerCode ) , quantity );  
      else      		
      	order = new Order( OrderType.MarketBuy, new Instrument( tickerCode ) , quantity );
      
      this.orders.Add(order);
    }
    
    protected virtual void closePosition(
      string ticker )
    {
      this.account.ClosePosition( ticker );
    }
    
    protected virtual void closePositions()
    {
      
      if(this.lastOrderedTickers != null)
      {
        foreach( string ticker in this.lastOrderedTickers)
        {
          for(int i = 0; i<this.account.Portfolio.Keys.Count; i++)
          {
            if(this.account.Portfolio[ticker]!=null)
              closePosition( ticker );
          }
        }
      }
      
      
    }
    
    protected virtual void addChosenTickersToOrderList(string[] tickers)
    {
      for( int i = 0; i<tickers.Length; i++)
      {
      	if(tickers[i] != null)
        {  
          this.addOrderForTicker( tickers, i );
          this.lastOrderedTickers[i] = 
          	GenomeManagerForEfficientPortfolio.GetCleanTickerCode(tickers[i]);
        }
      }
    }
    
    protected bool openPositions_allTickersToOpenQuotedAtCurrentDate(string[] tickers)
    {
      bool returnValue = true;
      DateTime currentDate = this.Account.EndOfDayTimer.GetCurrentTime().DateTime;
      foreach(string ticker in tickers)
      {
        if(ticker != null)
        {
          Quotes tickerQuotes = new Quotes(GenomeManagerForEfficientPortfolio.GetCleanTickerCode(ticker),
                                          currentDate, currentDate);
          if(tickerQuotes.Rows.Count == 0)
          //no quote available for the current ticker
            returnValue = false;
        }                       
      }
      return returnValue;
    }
    
    protected virtual void openPositions(string[] tickers)
    {
      //add cash first
    	if(this.orders.Count == 0 && this.account.Transactions.Count == 0)
        this.account.AddCash(30000);     
      if(this.openPositions_allTickersToOpenQuotedAtCurrentDate(tickers))
        //all tickers have quotes at the current date, so orders can be filled
      {
        this.addChosenTickersToOrderList(tickers);
        //execute orders actually
        foreach(object item in this.orders)
          this.account.AddOrder((Order)item);
      }
    }
    
    protected void addGenomeToBestGenomes(Genome genome,
                                          DateTime firstOptimizationDate,
                                          DateTime secondOptimizationDate,
                                          int eligibleTickers)
    {
      if(this.bestGenomes == null)
        this.bestGenomes = new ArrayList();
      
      this.bestGenomes.Add(new GenomeRepresentation(genome,
                                                    firstOptimizationDate,
                                                    secondOptimizationDate,
                                                    genome.Generation,
                                                   	eligibleTickers) );
    }
    
    protected void addGenomeToBestGenomes(BruteForceOptimizableParameters BFOptimizableParameters,
                                          string[] signedTickers,
                                          DateTime firstOptimizationDate,
                                          DateTime secondOptimizationDate,
                                          int eligibleTickers)
    {
      if(this.bestGenomes == null)
        this.bestGenomes = new ArrayList();
      
      this.bestGenomes.Add(new GenomeRepresentation(BFOptimizableParameters,
                                                    signedTickers,
                                                    firstOptimizationDate,
                                                    secondOptimizationDate,
                                                    eligibleTickers) );
    }
    
    protected void addGenomeToBestGenomes(Genome genome,
                                          DateTime firstOptimizationDate,
                                          DateTime secondOptimizationDate,
                                          int eligibleTickers, int halfPeriodDays)
    {
      if(this.bestGenomes == null)
        this.bestGenomes = new ArrayList();
      
      this.bestGenomes.Add(new GenomeRepresentation(genome,
                                    firstOptimizationDate,
                                    secondOptimizationDate,
                                    genome.Generation,
                                    eligibleTickers,
                                    halfPeriodDays));
    }

    protected void addGenomeToBestGenomes(Genome genome,
                                          DateTime firstOptimizationDate,
                                          DateTime secondOptimizationDate,
                                          int eligibleTickers, int halfPeriodDays,
                                          PortfolioType portfolioType)
    {
      if(this.bestGenomes == null)
        this.bestGenomes = new ArrayList();
      
      this.bestGenomes.Add(new GenomeRepresentation(genome,
        firstOptimizationDate,
        secondOptimizationDate,
        genome.Generation,
        eligibleTickers,
        halfPeriodDays, portfolioType));
    }

    protected void addGenomeToBestGenomes(Genome genome,
                                          DateTime firstOptimizationDate,
                                          DateTime secondOptimizationDate,
                                          int eligibleTickers, int halfPeriodDays,
                                          PortfolioType portfolioType,
                                          int createdGenerations)
    {
      if(this.bestGenomes == null)
        this.bestGenomes = new ArrayList();
      
      this.bestGenomes.Add(new GenomeRepresentation(genome,
        firstOptimizationDate,
        secondOptimizationDate,
        genome.Generation,
        eligibleTickers,
        halfPeriodDays, portfolioType, createdGenerations));
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
    
    #region reversePositions
    private void reversePositions_addReversedTickersToOrderList_addOrderForTicker(int tickerPosition )
    {
      string tickerCode = 
        GenomeManagerForEfficientPortfolio.GetCleanTickerCode(this.reversedTickersForOpenedPositions[tickerPosition]);
      double cashForSinglePosition = 
        this.account.CashAmount * this.chosenTickersPortfolioWeights[tickerPosition];
      long quantity =
        Convert.ToInt64( Math.Floor( cashForSinglePosition / this.account.DataStreamer.GetCurrentBid( tickerCode ) ) );
      Order order;
      if(this.portfolioType == PortfolioType.OnlyShort ||
        (this.portfolioType == PortfolioType.ShortAndLong &&
        this.reversedTickersForOpenedPositions[tickerPosition] != tickerCode))
        order = new Order( OrderType.MarketSellShort, new Instrument( tickerCode ) , quantity );  
      else      		
        order = new Order( OrderType.MarketBuy, new Instrument( tickerCode ) , quantity );
      
      this.orders.Add(order);
    }

    private void reversePositions_addReversedTickersToOrderList()
    {
      for( int i = 0; i<this.reversedTickersForOpenedPositions.Length; i++)
      {
        if(this.reversedTickersForOpenedPositions[i] != null)
        {  
          this.reversePositions_addReversedTickersToOrderList_addOrderForTicker( i );
          this.lastOrderedTickers[i] = 
            GenomeManagerForEfficientPortfolio.GetCleanTickerCode(this.reversedTickersForOpenedPositions[i]);
        }
      }
    }
    
    
    private void reversePositions_setReversedTickers()
    {
      string sign = "";
      string instrumentKey = "";
      Position[] positions = new Position[this.chosenTickers.Length];
      this.account.Portfolio.Positions.CopyTo(positions, 0);
      for(int i = 0; i<this.account.Portfolio.Count; i++)
      {  
        instrumentKey = positions[i].Instrument.Key;
        if(positions[i].IsShort)
          sign = "";
        else
          sign = "-";
        this.reversedTickersForOpenedPositions[i] = sign + instrumentKey;
      } 
    }  

    protected void reversePositions()
    {
      this.reversedTickersForOpenedPositions = 
        new string[this.chosenTickers.Length];
      this.reversePositions_setReversedTickers();
      this.closePositions();
      this.orders.Clear();
      this.openPositions(this.reversedTickersForOpenedPositions);
    }   
    #endregion

  } // end of class
}
