/*
QuantProject - Quantitative Finance Library

PVOStrategy.cs
Copyright (C) 2008 
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
using QuantProject.ADT.Messaging;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies.TickersRelationships;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Data;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.Data.DataTables;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.WalkForwardTesting.LinearCombination;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator
{
	/// <summary>
  /// Implements MarketOpenEventHandler and MarketCloseEventHandler
  /// These handlers contain the core strategy for the Portfolio Value
  /// Oscillator
  /// </summary>
  [Serializable]
  public class PVOStrategy : IEndOfDayStrategyForBacktester
  {
		public event NewLogItemEventHandler NewLogItem;
		public event NewMessageEventHandler NewMessage;

		//initialized by the constructor
  	protected int numberOfTickersToBeChosen;
    protected int inSampleDays;
    protected int numDaysBetweenEachOptimization;
    protected IInSampleChooser inSampleChooser;
    protected IEligiblesSelector eligiblesSelector;
    protected Benchmark benchmark;
    protected HistoricalQuoteProvider historicalQuoteProvider;
    protected double maxAcceptableCloseToCloseDrawdown;
    protected double minimumAcceptableGain;
    protected int numDaysForOscillatingPeriod;
    //initialized after constructor's call
    protected int numDaysElapsedSinceLastOptimization;
    protected ReturnsManager returnsManager;
		protected TestingPositions[] chosenPVOPositions;
		//chosen in sample: these are the eligible positions for out
		//of sample testing
		protected PVOPositions pvoPositionsForOutOfSample;
    protected DateTime lastCloseDate;
    protected bool portfolioHasBeenOverbought;
    protected bool portfolioHasBeenOversold;
    protected Account account;
    public Account Account
		{
			get { return this.account; }
    	set { this.account = value; }
		}
    protected bool stopLossConditionReached;
    protected bool takeProfitConditionReached;
    protected double currentAccountValue;
    protected double previousAccountValue;
    
    private string description_GetDescriptionForChooser()
    {
    	if(this.inSampleChooser == null)
    		return "ConstantChooser";
    	else
    		return this.inSampleChooser.Description;	
    }
    
		public string Description
		{
			get
			{
				string description =
					"PVO_" +
					"nmbrTckrs_" + this.numberOfTickersToBeChosen.ToString() +
					"_inSmplDays_" + this.inSampleDays.ToString() + "_" +
					this.eligiblesSelector.Description + "_" +
					this.description_GetDescriptionForChooser();
				return description;
			}
		}
		
		public bool StopBacktestIfMaxRunningHoursHasBeenReached
		{
			get
			{
				return true;
			}
		}

		private void pvoStrategy(IEligiblesSelector eligiblesSelector,
			int inSampleDays,
			int numDaysForOscillatingPeriod,
			int numberOfTickersToBeChosen,
			Benchmark benchmark,
			int numDaysBetweenEachOptimization,
			HistoricalQuoteProvider historicalQuoteProvider,
			double maxAcceptableCloseToCloseDrawdown,
			double minimumAcceptableGain)
		{
			this.eligiblesSelector = eligiblesSelector;
			this.inSampleDays = inSampleDays;
			this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
			this.numberOfTickersToBeChosen = numberOfTickersToBeChosen;
			this.benchmark = benchmark;
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
			this.historicalQuoteProvider = historicalQuoteProvider;
			this.maxAcceptableCloseToCloseDrawdown = maxAcceptableCloseToCloseDrawdown;
			this.minimumAcceptableGain = minimumAcceptableGain;
      
			this.stopLossConditionReached = false;
			this.currentAccountValue = 0.0;
			this.previousAccountValue = 0.0;
			this.portfolioHasBeenOverbought = false;
			this.portfolioHasBeenOversold = false;
		}

    public PVOStrategy(IEligiblesSelector eligiblesSelector,
																IInSampleChooser inSampleChooser,
																int inSampleDays,
																int numDaysForOscillatingPeriod,
                                int numberOfTickersToBeChosen,
                                Benchmark benchmark,
																int numDaysBetweenEachOptimization,
                                HistoricalQuoteProvider historicalQuoteProvider,
                                double maxAcceptableCloseToCloseDrawdown,
                                double minimumAcceptableGain)
    														
    {
			this.pvoStrategy(eligiblesSelector, inSampleDays , numDaysForOscillatingPeriod ,
				numberOfTickersToBeChosen , benchmark , numDaysBetweenEachOptimization ,
				historicalQuoteProvider , maxAcceptableCloseToCloseDrawdown , 
				minimumAcceptableGain );
			this.inSampleChooser = inSampleChooser;
    }
	
		public PVOStrategy(IEligiblesSelector eligiblesSelector,
			TestingPositions[] chosenPVOPositions,
			int inSampleDays,
			int numDaysForOscillatingPeriod,
			int numberOfTickersToBeChosen,
			Benchmark benchmark,
			int numDaysBetweenEachOptimization,
			HistoricalQuoteProvider historicalQuoteProvider,
			double maxAcceptableCloseToCloseDrawdown,
			double minimumAcceptableGain)
    														
		{
			this.pvoStrategy(eligiblesSelector, inSampleDays , numDaysForOscillatingPeriod ,
				numberOfTickersToBeChosen , benchmark , numDaysBetweenEachOptimization ,
				historicalQuoteProvider , maxAcceptableCloseToCloseDrawdown , 
				minimumAcceptableGain );
			this.chosenPVOPositions = chosenPVOPositions;
		}

    public virtual void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
    	;
    }

		public void FiveMinutesBeforeMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
		}

		private EndOfDayDateTime now()
		{
			return this.account.EndOfDayTimer.GetCurrentTime();
		}

    #region MarketCloseEventHandler
    //forOutOfSampleTesting
		protected virtual EndOfDayDateTime getBeginOfOscillatingPeriod(IndexBasedEndOfDayTimer timer)
		{
			return new EndOfDayDateTime(	(DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition-this.numDaysForOscillatingPeriod]["quDate"],
				EndOfDaySpecificTime.MarketClose );
		}

    private void marketCloseEventHandler_reverseIfNeeded_reverse(PVOPositionsStatus currentStatus)
    {
    	if(currentStatus == PVOPositionsStatus.Overbought)
    	{
    		this.portfolioHasBeenOversold = false;
  			this.portfolioHasBeenOverbought = true;
    	}
    	else if(currentStatus == PVOPositionsStatus.Oversold)
    	{
    		this.portfolioHasBeenOversold = true;
  			this.portfolioHasBeenOverbought = false;
    	}
    	AccountManager.ReversePositions(this.account);
    	this.previousAccountValue = this.account.GetMarketValue();
    }

    private void marketCloseEventHandler_reverseIfNeeded(IndexBasedEndOfDayTimer timer)
    {
			EndOfDayDateTime today = timer.GetCurrentTime(); 
			EndOfDayDateTime beginOfOscillatingPeriod = this.getBeginOfOscillatingPeriod(timer);
			PVOPositionsStatus pvoPositionsStatus = 
      	this.pvoPositionsForOutOfSample.GetStatus(beginOfOscillatingPeriod, today , this.benchmark.Ticker, this.historicalQuoteProvider);
  		if(pvoPositionsStatus == PVOPositionsStatus.Overbought &&
    	   this.portfolioHasBeenOversold)
  		//open positions derive from an overSold period but now
  		//the overbought threshold has been reached
  		   		this.marketCloseEventHandler_reverseIfNeeded_reverse(pvoPositionsStatus);
  		
  		if(pvoPositionsStatus == PVOPositionsStatus.Oversold &&
    	   this.portfolioHasBeenOverbought)
  		//open positions derive from an overBought period but now
  		//the overSold threshold has been reached
  					this.marketCloseEventHandler_reverseIfNeeded_reverse(pvoPositionsStatus);
    }
    private PVOPositionsStatus marketCloseEventHandler_openPositions_getStatus(IndexBasedEndOfDayTimer timer)
    {
    	EndOfDayDateTime today = timer.GetCurrentTime();
    	EndOfDayDateTime beginOfOscillatingPeriod =
    		this.getBeginOfOscillatingPeriod(timer);
    	PVOPositionsStatus currentStatus = 
    		PVOPositionsStatus.InTheMiddle;
    	for(int i = 0; i<this.chosenPVOPositions.Length; i++)
    	{
    		if(this.chosenPVOPositions[i] != null)
    			currentStatus = 
    				((PVOPositions)this.chosenPVOPositions[i]).GetStatus(beginOfOscillatingPeriod, today,
    				                                     this.benchmark.Ticker, this.historicalQuoteProvider);
    		if(currentStatus == PVOPositionsStatus.Oversold ||
    		   currentStatus == PVOPositionsStatus.Overbought )
    		{	
					this.pvoPositionsForOutOfSample = (PVOPositions)this.chosenPVOPositions[i];
					i = this.chosenPVOPositions.Length;//exit from for
				}
     	}
    		return currentStatus;
    }
    
    protected void marketCloseEventHandler_openPositions(IndexBasedEndOfDayTimer timer)
    {
      PVOPositionsStatus pvoPositionsStatus = PVOPositionsStatus.InTheMiddle;
      if(timer.CurrentDateArrayPosition >= this.numDaysForOscillatingPeriod)
      	pvoPositionsStatus = 
      		this.marketCloseEventHandler_openPositions_getStatus(timer);
			switch (pvoPositionsStatus){
        case PVOPositionsStatus.Overbought:
    		{
    			#region manage Overbought case
      		this.pvoPositionsForOutOfSample.WeightedPositions.Reverse();
          try{
            AccountManager.OpenPositions( this.pvoPositionsForOutOfSample.WeightedPositions,
          	                            	this.account );
            this.portfolioHasBeenOverbought = true;
          	this.portfolioHasBeenOversold = false;
          	this.previousAccountValue = this.account.GetMarketValue();
          }
          catch(Exception ex){
            ex = ex;
          }
          finally{
            this.pvoPositionsForOutOfSample.WeightedPositions.Reverse();
          }
      		#endregion
      		break;
    		}
        case PVOPositionsStatus.Oversold:
        {
    			#region manage Oversold case
      		AccountManager.OpenPositions( this.pvoPositionsForOutOfSample.WeightedPositions,
          	                            this.account );
          this.portfolioHasBeenOverbought = false;
          this.portfolioHasBeenOversold = true;
          this.previousAccountValue = this.account.GetMarketValue();
          #endregion
          break;
    		}
        case PVOPositionsStatus.InTheMiddle://that is
      	{  //pvoPositionsForOutOfSample has not been set
      		
          this.previousAccountValue = this.account.GetMarketValue();
					break;
      	}
				default:
        {
      		//it should never been reached
          this.previousAccountValue = this.account.GetMarketValue();
					break;
      	}
      }
    }
    
    protected virtual void marketCloseEventHandler_closePositionsIfNeeded()
    {
      if(this.stopLossConditionReached ||
    	   this.takeProfitConditionReached ||
         this.numDaysElapsedSinceLastOptimization + 1 == this.numDaysBetweenEachOptimization )
      {    
    		AccountManager.ClosePositions(this.account);
        this.portfolioHasBeenOverbought = false;
        this.portfolioHasBeenOversold = false;
      }
    }    
    
    protected virtual void marketCloseEventHandler_updateStopLossAndTakeProfitConditions()
    {
      //this.previousAccountValue has been set at opening positions
      this.stopLossConditionReached = false;
      this.takeProfitConditionReached = false;
      this.currentAccountValue = this.account.GetMarketValue();
      double portfolioGainOrLoss = (this.currentAccountValue - this.previousAccountValue)
           													/this.previousAccountValue;
      
      if(!double.IsInfinity(portfolioGainOrLoss) &&
         portfolioGainOrLoss <= -this.maxAcceptableCloseToCloseDrawdown )
      {
        this.stopLossConditionReached = true;
        this.takeProfitConditionReached = false;
      }
      else if (!double.IsInfinity(portfolioGainOrLoss) &&
      				 portfolioGainOrLoss >= this.minimumAcceptableGain )
               
      {
        this.stopLossConditionReached = false;
        this.takeProfitConditionReached = true;
      }
    }
    
    public virtual void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
    	try{
	    	this.marketCloseEventHandler_updateStopLossAndTakeProfitConditions();
	      this.marketCloseEventHandler_closePositionsIfNeeded();
	      if( this.chosenPVOPositions != null )
	      //PVOPositions have been chosen by the chooser
	      {
	      	if(this.account.Portfolio.Count == 0)
	      			this.marketCloseEventHandler_openPositions((IndexBasedEndOfDayTimer)sender);
	        		//positions are opened only if thresholds are reached
	      	else//there are some opened positions
	            this.marketCloseEventHandler_reverseIfNeeded((IndexBasedEndOfDayTimer)sender);
	      }
    	}
    	catch(TickerNotExchangedException ex)
    	{ex=ex;}
    }

    #endregion
 
    #region OneHourAfterMarketCloseEventHandler
    
		protected virtual void updateReturnsManager(EndOfDayDateTime firstEndOfDayDateTime,
			EndOfDayDateTime lastEndOfDayDateTime)
		{
			this.returnsManager = 
				new ReturnsManager(new CloseToCloseIntervals(firstEndOfDayDateTime, lastEndOfDayDateTime,
				this.benchmark.Ticker, this.numDaysForOscillatingPeriod),
				this.historicalQuoteProvider);
		}

		private PVOLogItem getLogItem( EligibleTickers eligibleTickers )
		{
			PVOLogItem logItem =
				new PVOLogItem( this.now() , this.inSampleDays );
			logItem.BestPVOPositionsInSample =
				this.chosenPVOPositions;
			logItem.NumberOfEligibleTickers =
				eligibleTickers.Count;
			logItem.FitnessOfFirst = 
				this.chosenPVOPositions[0].FitnessInSample;
			logItem.FitnessOfLast = 
				this.chosenPVOPositions[this.chosenPVOPositions.Length - 1].FitnessInSample;
			logItem.GenerationOfFirst = 
				((IGeneticallyOptimizable)this.chosenPVOPositions[0]).Generation;
			logItem.GenerationOfLast = 
				((IGeneticallyOptimizable)this.chosenPVOPositions[this.chosenPVOPositions.Length - 1]).Generation;
			logItem.ThresholdsOfFirst = 
				((PVOPositions)this.chosenPVOPositions[0]).OversoldThreshold.ToString() + ";" +
				((PVOPositions)this.chosenPVOPositions[0]).OverboughtThreshold.ToString();
			logItem.ThresholdsOfLast = 
				((PVOPositions)this.chosenPVOPositions[this.chosenPVOPositions.Length - 1]).OversoldThreshold.ToString() + ";" +
				((PVOPositions)this.chosenPVOPositions[this.chosenPVOPositions.Length - 1]).OverboughtThreshold.ToString();
			logItem.TickersOfFirst = 
				this.chosenPVOPositions[0].HashCodeForTickerComposition;
			logItem.TickersOfLast = 
				this.chosenPVOPositions[this.chosenPVOPositions.Length - 1].HashCodeForTickerComposition;
			return logItem;
		}
		private void raiseNewLogItem( EligibleTickers eligibleTickers )
		{
			PVOLogItem logItem =
				this.getLogItem( eligibleTickers );
			NewLogItemEventArgs newLogItemEventArgs =
				new NewLogItemEventArgs( logItem );
			this.NewLogItem( this , newLogItemEventArgs );
		}
		private void notifyMessage( EligibleTickers eligibleTickers )
		{
			string message = "Number of Eligible tickers: " +
				eligibleTickers.Count;
			NewMessageEventArgs newMessageEventArgs =
				new NewMessageEventArgs( message );
			if ( this.NewMessage != null )
				this.NewMessage( this , newMessageEventArgs );
		}
		private void logOptimizationInfo( EligibleTickers eligibleTickers )
		{
			this.raiseNewLogItem( eligibleTickers );
			this.notifyMessage( eligibleTickers );
		}

    protected virtual void updateTestingPositions(DateTime currentDate)
    {
			EndOfDayHistory endOfDayHistory =
				this.benchmark.GetEndOfDayHistory(
				new EndOfDayDateTime(currentDate.AddDays(-this.inSampleDays),
				EndOfDaySpecificTime.MarketClose),
				new EndOfDayDateTime(currentDate,
				EndOfDaySpecificTime.MarketClose));
			EligibleTickers eligibles = 
				this.eligiblesSelector.GetEligibleTickers(endOfDayHistory);
			this.updateReturnsManager(endOfDayHistory.FirstEndOfDayDateTime,
			                          endOfDayHistory.LastEndOfDayDateTime);
			if(this.inSampleChooser != null)
				this.chosenPVOPositions = (TestingPositions[])inSampleChooser.AnalyzeInSample(eligibles, this.returnsManager );
			this.logOptimizationInfo(eligibles);
    }
    
    /// <summary>
    /// Handles a "One hour after market close" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public virtual void OneHourAfterMarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      this.lastCloseDate = endOfDayTimingEventArgs.EndOfDayDateTime.DateTime;
      this.numDaysElapsedSinceLastOptimization++;
      if((this.numDaysElapsedSinceLastOptimization == 
            this.numDaysBetweenEachOptimization))
      //num days without optimization has elapsed
      {
        this.updateTestingPositions(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime);
        //sets tickers to be chosen next Market Close event
        this.numDaysElapsedSinceLastOptimization = 0;
      }
		      
    }
		#endregion
  }
}
