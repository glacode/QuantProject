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

using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Strategies;
using	QuantProject.Business.Timing;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.ADT.Optimizing.BruteForce;
using QuantProject.Scripts.WalkForwardTesting.LinearCombination;

namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
  /// <summary>
  /// Base class for EndOfDayTimerHandlers for efficient portfolios
  /// </summary>
  [Serializable]
  public abstract class EndOfDayTimerHandler
  {
    protected DataTable eligibleTickers;
		protected WeightedPositions chosenWeightedPositions = null;
    protected string tickerGroupID;
    protected int numberOfEligibleTickers;
    protected int numberOfTickersToBeChosen;
    protected int numDaysForOptimizationPeriod;
    protected int numDaysBetweenEachOptimization = 0;
    protected int numDaysElapsedSinceLastOptimization = 0;
    protected int generationNumberForGeneticOptimizer;
    protected int populationSizeForGeneticOptimizer;
    protected Account account = null;
    protected string benchmark;
    //these two values have to be updated during
    //backtest, for minimizing commission amount,
    //according to broker's commission scheme 
    protected double minPriceForMinimumCommission = 30;
    protected double maxPriceForMinimumCommission = 500;
    protected double targetReturn;
    protected PortfolioType portfolioType;
    
    protected bool stopLossConditionReached = false;
    protected bool takeProfitConditionReached = false;
    protected double currentAccountValue = 0.0;
    protected double previousAccountValue = 0.0;
    
		protected GenomeCounter genomeCounter;
    public GenomeCounter GenomeCounter
    {
      get{return this.genomeCounter;}
    }
 
    public int NumberOfEligibleTickers
    {
      get { return this.numberOfEligibleTickers; }
    }
		
    public Account Account
		{
			get { return this.account; }
			set { this.account = value; }
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
      this.targetReturn = targetReturn;
      this.portfolioType = portfolioType;
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
    public EndOfDayTimerHandler(WeightedPositions chosenWeightedPositions,
                                PortfolioType portfolioType,
                                Account account,
                                string benchmark)
    {
      this.account = account;
      this.benchmark = benchmark;
      this.chosenWeightedPositions = chosenWeightedPositions;
      this.numberOfTickersToBeChosen = this.chosenWeightedPositions.Count;
      this.portfolioType = portfolioType;
    }
		
		#region addGenomeToBestGenomes

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
			SignedTickers signedTickers,
			DateTime firstOptimizationDate,
			DateTime secondOptimizationDate,
			int eligibleTickers)
		{
			if(this.bestGenomes == null)
				this.bestGenomes = new ArrayList();
      
			this.bestGenomes.Add(new GenomeRepresentation(BFOptimizableParameters,
				signedTickers.ArrayForSignedTickers,
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
		#endregion
		
		protected virtual void openPositions()
    {
      //add cash first
    	if(this.account.Transactions.Count == 0)
        this.account.AddCash(15000);
    	if(this.chosenWeightedPositions != null)
      	AccountManager.OpenPositions(this.chosenWeightedPositions, this.account);
    }
	
		protected void reversePositions()
		{
			if ( this.portfolioType == PortfolioType.OnlyLong ||
				this.portfolioType == PortfolioType.OnlyShort )
				throw new Exception ("It's not possible to reverse positions " +
					"when portfolio has to contain only " +
					"long or short positions!");
			AccountManager.ReversePositions(this.account);
		}   
		
    public abstract void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
			;
    public abstract void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    	;
    public abstract void OneHourAfterMarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
      ;
  } 
}
