/*
QuantProject - Quantitative Finance Library

PVO_CTOStrategy.cs
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
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.InSampleChoosers;
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
  public class PVO_CTOStrategy : IEndOfDayStrategyForBacktester
  {
		public event NewLogItemEventHandler NewLogItem;
		public event NewMessageEventHandler NewMessage;

		//initialized by the constructor
  	protected int inSampleDays;
    protected int numDaysBetweenEachOptimization;
    protected IInSampleChooser inSampleChooser;
    protected IEligiblesSelector eligiblesSelector;
    protected Benchmark benchmark;
    protected HistoricalQuoteProvider historicalQuoteProvider;
    protected double oversoldThreshold;
    protected double overboughtThreshold;
    protected double oversoldThresholdMAX;
    protected double overboughtThresholdMAX;
    //initialized after constructor's call
    protected int numDaysElapsedSinceLastOptimization;
    protected ReturnsManager returnsManager;
		protected TestingPositions[] chosenPVOPositions;
		//chosen in sample: these are the eligible positions for out
		//of sample testing
		protected PVOPositions pvoPositionsForOutOfSample;
    protected DateTime lastCloseDate;
		protected DateTime lastOptimizationDateTime;
    protected Account account;
    public Account Account
		{
    	get { return this.account; }
    	set { this.account = value; }
		}
    private int numOfOpeningsToCrossBeforeExit;
    private int numOfOpeningsWithOpenPositions;
    
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
					"PVO_CTO\n" +
					"Tickers_" + "2\n" +
					"_inSampleDays_" + this.inSampleDays.ToString() + "\n" +
					this.eligiblesSelector.Description + "\n" +
					"oversoldThreshold_" + this.oversoldThreshold.ToString() + "\n" +
					"overboughtThreshold_" + this.overboughtThreshold.ToString() + "\n" +
					this.description_GetDescriptionForChooser() + "\n" +
					"Optimization each " + this.numDaysBetweenEachOptimization.ToString() + " days";
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

		private void pvo_ctoStrategy(IEligiblesSelector eligiblesSelector,
			int inSampleDays,
			Benchmark benchmark,
			int numDaysBetweenEachOptimization,
			int numOfOpeningsToCrossBeforeExit,
			double oversoldThreshold,
			double overboughtThreshold,
			double oversoldThresholdMAX,
			double overboughtThresholdMAX,
			HistoricalQuoteProvider historicalQuoteProvider)
		{
			this.numOfOpeningsWithOpenPositions = 0;
			this.eligiblesSelector = eligiblesSelector;
			this.inSampleDays = inSampleDays;
			this.benchmark = benchmark;
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
			this.numOfOpeningsToCrossBeforeExit = numOfOpeningsToCrossBeforeExit;
			this.oversoldThreshold = oversoldThreshold;
			this.overboughtThreshold = overboughtThreshold;
			this.oversoldThresholdMAX = oversoldThresholdMAX;
			this.overboughtThresholdMAX = overboughtThresholdMAX;
			this.historicalQuoteProvider = historicalQuoteProvider;
		}

    public PVO_CTOStrategy(IEligiblesSelector eligiblesSelector,
																IInSampleChooser inSampleChooser,
																int inSampleDays,
																Benchmark benchmark,
																int numDaysBetweenEachOptimization,
																int numOfOpeningsToCrossBeforeExit,
																double oversoldThreshold,
																double overboughtThreshold,
																double oversoldThresholdMAX,
																double overboughtThresholdMAX,
                                HistoricalQuoteProvider historicalQuoteProvider)
    														
    {
			this.pvo_ctoStrategy(eligiblesSelector, inSampleDays , benchmark , numDaysBetweenEachOptimization ,
				numOfOpeningsToCrossBeforeExit, oversoldThreshold, overboughtThreshold,
				oversoldThresholdMAX, overboughtThresholdMAX,
				historicalQuoteProvider);
			this.inSampleChooser = inSampleChooser;
    }
	 	
		public PVO_CTOStrategy(IEligiblesSelector eligiblesSelector,
																IInSampleChooser inSampleChooser,
																int inSampleDays,
																Benchmark benchmark,
																int numDaysBetweenEachOptimization,
																int numOfOpeningsToCrossBeforeExit,
																double oversoldThreshold,
																double overboughtThreshold,
                                HistoricalQuoteProvider historicalQuoteProvider)
    														
    {
			this.pvo_ctoStrategy(eligiblesSelector, inSampleDays , benchmark , numDaysBetweenEachOptimization ,
				numOfOpeningsToCrossBeforeExit, oversoldThreshold, overboughtThreshold,
				double.MaxValue, double.MaxValue,
				historicalQuoteProvider);
			this.inSampleChooser = inSampleChooser;
    }
		public PVO_CTOStrategy(IEligiblesSelector eligiblesSelector,
			TestingPositions[] chosenPVOPositions,
			int inSampleDays,
			Benchmark benchmark,
			int numDaysBetweenEachOptimization,
			int numOfOpeningsToCrossBeforeExit,
			double oversoldThreshold,
			double overboughtThreshold,
			double oversoldThresholdMAX,
			double overboughtThresholdMAX,
			HistoricalQuoteProvider historicalQuoteProvider)
    														
		{
			this.pvo_ctoStrategy(eligiblesSelector, inSampleDays , benchmark , numDaysBetweenEachOptimization ,
				numOfOpeningsToCrossBeforeExit,
				oversoldThreshold, overboughtThreshold,
				oversoldThresholdMAX, overboughtThresholdMAX,
				historicalQuoteProvider);
			this.chosenPVOPositions = chosenPVOPositions;
		}
		public PVO_CTOStrategy(IEligiblesSelector eligiblesSelector,
			TestingPositions[] chosenPVOPositions,
			int inSampleDays,
			Benchmark benchmark,
			int numDaysBetweenEachOptimization,
			int numOfOpeningsToCrossBeforeExit,
			double oversoldThreshold,
			double overboughtThreshold,
			HistoricalQuoteProvider historicalQuoteProvider)
    														
		{
			this.pvo_ctoStrategy(eligiblesSelector, inSampleDays , benchmark , numDaysBetweenEachOptimization ,
				numOfOpeningsToCrossBeforeExit,
				oversoldThreshold, overboughtThreshold,
				double.MaxValue, double.MaxValue,
				historicalQuoteProvider);
			this.chosenPVOPositions = chosenPVOPositions;
		}
		
		public virtual void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
			if(this.account.Portfolio.Count > 0)
			{
				this.numOfOpeningsWithOpenPositions++;
				if(this.numOfOpeningsWithOpenPositions > this.numOfOpeningsToCrossBeforeExit)
				{
					AccountManager.ClosePositions(this.account);
					this.numOfOpeningsWithOpenPositions = 0;
				}
			}
    }
		
		public void FiveMinutesBeforeMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
		}

		private EndOfDayDateTime now()
		{
			return this.account.EndOfDayTimer.GetCurrentTime();
		}

    
		protected virtual EndOfDayDateTime getBeginOfOscillatingPeriod(IndexBasedEndOfDayTimer timer)
		{
			return new EndOfDayDateTime(	(DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition]["quDate"],
				EndOfDaySpecificTime.MarketOpen );
		}
		
		#region MarketCloseEventHandler
		
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
    				                                     this.benchmark.Ticker, this.historicalQuoteProvider,
    				                                     this.oversoldThresholdMAX, this.overboughtThresholdMAX);
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
			if(timer.CurrentDateArrayPosition >= 1)
				pvoPositionsStatus = 
					this.marketCloseEventHandler_openPositions_getStatus(timer);
			switch (pvoPositionsStatus)
			{
				case PVOPositionsStatus.Overbought:
				{
    			#region manage Overbought case
					this.pvoPositionsForOutOfSample.WeightedPositions.Reverse();
					try
					{
						AccountManager.OpenPositions( this.pvoPositionsForOutOfSample.WeightedPositions,
							this.account );
					}
					catch(Exception ex)
					{
						ex = ex;
					}
					finally
					{
						this.pvoPositionsForOutOfSample.WeightedPositions.Reverse();
					}
      		#endregion
					break;
				}
				case PVOPositionsStatus.Oversold:
				{
    			AccountManager.OpenPositions( this.pvoPositionsForOutOfSample.WeightedPositions,
						this.account );
					break;
				}
				case PVOPositionsStatus.InTheMiddle://that is
				{  //pvoPositionsForOutOfSample has not been set
      		
					break;
				}
				default:
				{
					//it should never been reached
					break;
				}
			}
		}
		
		
    public virtual void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
			if ( this.account.Portfolio.Count == 0 &&
					 this.chosenPVOPositions != null )
					//portfolio is empty and optimization has
				  //been already launched
			{
				try{
					this.marketCloseEventHandler_openPositions( (IndexBasedEndOfDayTimer)sender );
				}
				catch(TickerNotExchangedException ex)
				{ex=ex;}
			}
		}
 
		#endregion
		
    #region OneHourAfterMarketCloseEventHandler
    
		protected virtual void updateReturnsManager(EndOfDayDateTime firstEndOfDayDateTime,
			EndOfDayDateTime lastEndOfDayDateTime)
		{
			ReturnIntervals returnIntervals = 
				new CloseToOpenIntervals( firstEndOfDayDateTime, lastEndOfDayDateTime,
																	this.benchmark.Ticker );
			if( this.inSampleChooser is PVOCorrelationChooser )
			{
				switch ( ((PVOCorrelationChooser)this.inSampleChooser).IntervalsType )
				{
					case IntervalsType.CloseToCloseIntervals:
						returnIntervals = new CloseToCloseIntervals(firstEndOfDayDateTime, lastEndOfDayDateTime,
							this.benchmark.Ticker, ((PVOCorrelationChooser)this.inSampleChooser).ReturnIntervalLength);
						break;
					case IntervalsType.OpenToOpenIntervals:
						returnIntervals = new OpenToOpenIntervals(firstEndOfDayDateTime, lastEndOfDayDateTime,
							this.benchmark.Ticker, ((PVOCorrelationChooser)this.inSampleChooser).ReturnIntervalLength);
						break;
					case IntervalsType.CloseToOpenIntervals:
						returnIntervals = new CloseToOpenIntervals(firstEndOfDayDateTime, lastEndOfDayDateTime,
							this.benchmark.Ticker);
						break;
					case IntervalsType.OpenToCloseIntervals:
						returnIntervals = new DailyOpenToCloseIntervals(firstEndOfDayDateTime, lastEndOfDayDateTime,
							this.benchmark.Ticker );
						break;
					case IntervalsType.OpenToCloseCloseToOpenIntervals:
						returnIntervals = new OpenToCloseCloseToOpenIntervals(
							firstEndOfDayDateTime, lastEndOfDayDateTime, this.benchmark.Ticker);
						break;
					default:
						// it should never be reached
						returnIntervals = new DailyOpenToCloseIntervals(firstEndOfDayDateTime, lastEndOfDayDateTime,
							this.benchmark.Ticker );
						break;
				}
			}
			
			this.returnsManager =
					new ReturnsManager( returnIntervals , this.historicalQuoteProvider);	
			
		}

		private PVO_CTOLogItem getLogItem( EligibleTickers eligibleTickers )
		{
			PVO_CTOLogItem logItem =
				new PVO_CTOLogItem( this.now() , this.inSampleDays );
			logItem.BestPVOPositionsInSample =
				this.chosenPVOPositions;
			logItem.NumberOfEligibleTickers =
				eligibleTickers.Count;
			logItem.FitnessOfFirst = 
				this.chosenPVOPositions[0].FitnessInSample;
			logItem.FitnessOfLast = 
				this.chosenPVOPositions[this.chosenPVOPositions.Length - 1].FitnessInSample;
//			logItem.GenerationOfFirst = 
//				((IGeneticallyOptimizable)this.chosenPVOPositions[0]).Generation;
//			logItem.GenerationOfLast = 
//				((IGeneticallyOptimizable)this.chosenPVOPositions[this.chosenPVOPositions.Length - 1]).Generation;
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
			PVO_CTOLogItem logItem =
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

		private void updateTestingPositions_updateThresholds()
		{
			for(int i = 0; i<this.chosenPVOPositions.Length; i++)
			{
				((PVOPositions)this.chosenPVOPositions[i]).OversoldThreshold =
					this.oversoldThreshold;
				((PVOPositions)this.chosenPVOPositions[i]).OverboughtThreshold =
					this.overboughtThreshold;
			}
		}
		
    protected virtual void updateTestingPositions(DateTime currentDate)
    {
			EndOfDayHistory endOfDayHistory =
				this.benchmark.GetEndOfDayHistory(
				new EndOfDayDateTime(currentDate.AddDays(-this.inSampleDays),
				EndOfDaySpecificTime.MarketOpen),
				new EndOfDayDateTime(currentDate,
				EndOfDaySpecificTime.MarketClose));
			EligibleTickers eligibles = 
				this.eligiblesSelector.GetEligibleTickers(endOfDayHistory);
			try{
				this.updateReturnsManager(endOfDayHistory.FirstEndOfDayDateTime,
				                          endOfDayHistory.LastEndOfDayDateTime);
				if(this.inSampleChooser != null)
					this.chosenPVOPositions = (TestingPositions[])inSampleChooser.AnalyzeInSample(eligibles, this.returnsManager);
				this.updateTestingPositions_updateThresholds();
				this.logOptimizationInfo(eligibles);
			}
			catch(TickerNotExchangedException ex)
			{	ex = ex;}
    }
    
		private bool optimalTestingPositionsAreToBeUpdated()
		{
			bool areToBeUpdated = false;
			if(this.inSampleChooser != null)
			{
				DateTime dateTimeForNextOptimization =
					this.lastOptimizationDateTime.AddDays(
					this.numDaysBetweenEachOptimization );
				areToBeUpdated =
					( ( ( this.account.Portfolio.Count == 0 )
					&& ( ( this.lastOptimizationDateTime == DateTime.MinValue ) ) ) ||
					( this.now().DateTime >= dateTimeForNextOptimization ) );
			}
			return areToBeUpdated;
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
//OLD - numDaysBetweenEachOptimization --> market days      
//			if( this.account.Transactions.Count <= 1 || 
//				  (this.numDaysElapsedSinceLastOptimization == 
//            this.numDaysBetweenEachOptimization) )
				//num days without optimization has elapsed or
				//no transaction, except for adding cash, has been executed	
//NEW - numDaysBetweenEachOptimization --> calendar days 
			if ( this.optimalTestingPositionsAreToBeUpdated() )
      {
        this.updateTestingPositions(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime);
        //sets tickers to be chosen next Market Close event
        this.numDaysElapsedSinceLastOptimization = 0;
				this.lastOptimizationDateTime = this.now().DateTime;
      }
    }
		#endregion
  }
}
