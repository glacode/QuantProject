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
using System.Collections.Generic;

using QuantProject.ADT;
using QuantProject.ADT.Histories;
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
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.EntryConditions;
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
	public class PVOStrategy : IStrategyForBacktester
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
		protected HistoricalMarketValueProvider historicalQuoteProvider;
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
		protected Account account;
		public Account Account
		{
			get { return this.account; }
			set { this.account = value; }
		}
		protected bool stopLossConditionReached;
		protected bool takeProfitConditionReached;
		protected bool maxNumberOfDaysOnTheMarketReached;
		protected int maxNumberOfDaysOnTheMarket;
		protected int daysOnTheMarket;
		protected double currentAccountValue;
		protected double previousAccountValue;
		protected double oversoldThreshold;
		protected double overboughtThreshold;
		protected double oversoldThresholdMAX;
		protected double overboughtThresholdMAX;
		protected double numOfStdDevForSignificantPriceMovements;
		protected double leverage;
		protected bool openOnlyLongPositions;
		protected List<IEntryCondition> entryConditions;
		protected bool allEntryConditionsHaveToBeSatisfied;
		
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
		                         double oversoldThreshold,
		                         double overboughtThreshold,
		                         double oversoldThresholdMAX,
		                         double overboughtThresholdMAX,
		                         double numOfStdDevForSignificantPriceMovements,
		                   			 double leverage,
		                   			 bool openOnlyLongPositions,
		                   			 int maxNumberOfDaysOnTheMarket,
		                         HistoricalMarketValueProvider historicalQuoteProvider,
		                         double maxAcceptableCloseToCloseDrawdown,
		                         double minimumAcceptableGain,
					                   List<IEntryCondition> entryConditions,
														 bool allEntryConditionsHaveToBeSatisfied)
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
			this.oversoldThreshold = oversoldThreshold;
			this.overboughtThreshold = overboughtThreshold;
			this.oversoldThresholdMAX = oversoldThresholdMAX;
			this.overboughtThresholdMAX = overboughtThresholdMAX;
			this.numOfStdDevForSignificantPriceMovements = numOfStdDevForSignificantPriceMovements;
			this.leverage = leverage;
			this.openOnlyLongPositions = openOnlyLongPositions;
			this.maxNumberOfDaysOnTheMarket = maxNumberOfDaysOnTheMarket;
			this.stopLossConditionReached = false;
			this.currentAccountValue = 0.0;
			this.previousAccountValue = 0.0;
			this.entryConditions = entryConditions;
			this.allEntryConditionsHaveToBeSatisfied = allEntryConditionsHaveToBeSatisfied;
		}

		public PVOStrategy(IEligiblesSelector eligiblesSelector,
		                   IInSampleChooser inSampleChooser,
		                   int inSampleDays,
		                   int numDaysForOscillatingPeriod,
		                   int numberOfTickersToBeChosen,
		                   Benchmark benchmark,
		                   int numDaysBetweenEachOptimization,
		                   double oversoldThreshold,
		                   double overboughtThreshold,
		                   double oversoldThresholdMAX,
		                   double overboughtThresholdMAX,
		                   double numOfStdDevForSignificantPriceMovements,
		                   double leverage,
		                   bool openOnlyLongPositions,
		                   int maxNumberOfDaysOnTheMarket,
		                   HistoricalMarketValueProvider historicalQuoteProvider,
		                   double maxAcceptableCloseToCloseDrawdown,
		                   double minimumAcceptableGain,
		                   List<IEntryCondition> entryConditions,
											 bool allEntryConditionsHaveToBeSatisfied)
			
		{
			this.pvoStrategy(eligiblesSelector, inSampleDays , numDaysForOscillatingPeriod ,
			                 numberOfTickersToBeChosen , benchmark , numDaysBetweenEachOptimization ,
			                 oversoldThreshold, overboughtThreshold,
			                 oversoldThresholdMAX, overboughtThresholdMAX,
			                 numOfStdDevForSignificantPriceMovements,
		                   leverage,
		                   openOnlyLongPositions,
		                   maxNumberOfDaysOnTheMarket,
		                   historicalQuoteProvider , maxAcceptableCloseToCloseDrawdown ,
			                 minimumAcceptableGain,
			                 entryConditions,
											 allEntryConditionsHaveToBeSatisfied);
			this.inSampleChooser = inSampleChooser;
		}
		
		public PVOStrategy(IEligiblesSelector eligiblesSelector,
		                   IInSampleChooser inSampleChooser,
		                   int inSampleDays,
		                   int numDaysForOscillatingPeriod,
		                   int numberOfTickersToBeChosen,
		                   Benchmark benchmark,
		                   int numDaysBetweenEachOptimization,
		                   double oversoldThreshold,
		                   double overboughtThreshold,
		                   double numOfStdDevForSignificantPriceMovements,
		                   double leverage,
		                   bool openOnlyLongPositions,
		                   int maxNumberOfDaysOnTheMarket,
		                   HistoricalMarketValueProvider historicalQuoteProvider,
		                   double maxAcceptableCloseToCloseDrawdown,
		                   double minimumAcceptableGain,
		                   List<IEntryCondition> entryConditions,
											 bool allEntryConditionsHaveToBeSatisfied)
			
		{
			this.pvoStrategy(eligiblesSelector, inSampleDays , numDaysForOscillatingPeriod ,
			                 numberOfTickersToBeChosen , benchmark , numDaysBetweenEachOptimization ,
			                 oversoldThreshold, overboughtThreshold,
			                 double.MaxValue, double.MaxValue,
			                 numOfStdDevForSignificantPriceMovements,
		                   leverage,
		                   openOnlyLongPositions,
		                   maxNumberOfDaysOnTheMarket,
			                 historicalQuoteProvider , maxAcceptableCloseToCloseDrawdown ,
			                 minimumAcceptableGain,
			                 entryConditions,
											 allEntryConditionsHaveToBeSatisfied );
			this.inSampleChooser = inSampleChooser;
		}

		public PVOStrategy(IEligiblesSelector eligiblesSelector,
		                   TestingPositions[] chosenPVOPositions,
		                   int inSampleDays,
		                   int numDaysForOscillatingPeriod,
		                   int numberOfTickersToBeChosen,
		                   Benchmark benchmark,
		                   int numDaysBetweenEachOptimization,
		                   double oversoldThreshold,
		                   double overboughtThreshold,
		                   double oversoldThresholdMAX,
		                   double overboughtThresholdMAX,
		                   double numOfStdDevForSignificantPriceMovements,
		                   double leverage,
		                   bool openOnlyLongPositions,
		                   int maxNumberOfDaysOnTheMarket,
		                   HistoricalMarketValueProvider historicalQuoteProvider,
		                   double maxAcceptableCloseToCloseDrawdown,
		                   double minimumAcceptableGain,
		                   List<IEntryCondition> entryConditions,
											 bool allEntryConditionsHaveToBeSatisfied)
			
		{
			this.pvoStrategy(eligiblesSelector, inSampleDays , numDaysForOscillatingPeriod ,
			                 numberOfTickersToBeChosen , benchmark , numDaysBetweenEachOptimization ,
			                 oversoldThreshold, overboughtThreshold,
			                 oversoldThresholdMAX, overboughtThresholdMAX,
			                 numOfStdDevForSignificantPriceMovements,
		                   leverage,
		                   openOnlyLongPositions,
		                   maxNumberOfDaysOnTheMarket,
			                 historicalQuoteProvider , maxAcceptableCloseToCloseDrawdown ,
			                 minimumAcceptableGain,
			                 entryConditions,
											 allEntryConditionsHaveToBeSatisfied );
			this.chosenPVOPositions = chosenPVOPositions;
		}

		public PVOStrategy(IEligiblesSelector eligiblesSelector,
		                   TestingPositions[] chosenPVOPositions,
		                   int inSampleDays,
		                   int numDaysForOscillatingPeriod,
		                   int numberOfTickersToBeChosen,
		                   Benchmark benchmark,
		                   int numDaysBetweenEachOptimization,
		                   double oversoldThreshold,
		                   double overboughtThreshold,
		                   double numOfStdDevForSignificantPriceMovements,
		                   double leverage,
		                   bool openOnlyLongPositions,
		                   int maxNumberOfDaysOnTheMarket,
		                   HistoricalMarketValueProvider historicalQuoteProvider,
		                   double maxAcceptableCloseToCloseDrawdown,
		                   double minimumAcceptableGain,
		                   List<IEntryCondition> entryConditions,
											 bool allEntryConditionsHaveToBeSatisfied)
			
		{
			this.pvoStrategy(eligiblesSelector, inSampleDays , numDaysForOscillatingPeriod ,
			                 numberOfTickersToBeChosen , benchmark , numDaysBetweenEachOptimization ,
			                 oversoldThreshold, overboughtThreshold,
			                 double.MaxValue , double.MaxValue ,
			                 numOfStdDevForSignificantPriceMovements,
		                   leverage,
		                   openOnlyLongPositions,
		                   maxNumberOfDaysOnTheMarket,
			                 historicalQuoteProvider , maxAcceptableCloseToCloseDrawdown ,
			                 minimumAcceptableGain,
			                 entryConditions,
											 allEntryConditionsHaveToBeSatisfied );
			this.chosenPVOPositions = chosenPVOPositions;
		}

		private void marketOpenEventHandler(
			Object sender , DateTime dateTime )
		{
			;
		}

		private void fiveMinutesBeforeMarketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
		}

		private DateTime now()
		{
			return this.account.Timer.GetCurrentDateTime();
		}

		#region MarketCloseEventHandler
		//forOutOfSampleTesting
		protected virtual DateTime getBeginOfOscillatingPeriod(IndexBasedEndOfDayTimer timer)
		{
			DateTime beginOfOscillatingPeriod =
				HistoricalEndOfDayTimer.GetMarketClose(
					(DateTime)timer.IndexQuotes.Rows[
						timer.CurrentDateArrayPosition-this.numDaysForOscillatingPeriod]["quDate"] );
			return beginOfOscillatingPeriod;
		}
	
		private PVOPositionsStatus marketCloseEventHandler_openPositions_getStatus(IndexBasedEndOfDayTimer timer)
		{
			DateTime today = timer.GetCurrentDateTime();
			DateTime beginOfOscillatingPeriod =
				this.getBeginOfOscillatingPeriod(timer);
			PVOPositionsStatus currentStatus =
				PVOPositionsStatus.InTheMiddle;
			for(int i = 0; i<this.chosenPVOPositions.Length; i++)
			{
				if(this.chosenPVOPositions[i] != null)
					currentStatus =
						((PVOPositions)this.chosenPVOPositions[i]).GetStatus(beginOfOscillatingPeriod, today,
						                                                     this.benchmark.Ticker, this.historicalQuoteProvider,
						                                                     double.MaxValue, double.MaxValue);
				if(currentStatus == PVOPositionsStatus.Oversold ||
				   currentStatus == PVOPositionsStatus.Overbought )
				{
					this.pvoPositionsForOutOfSample = (PVOPositions)this.chosenPVOPositions[i];
					i = this.chosenPVOPositions.Length;//exit from for
				}
			}
			return currentStatus;
		}
		
		private bool areEntryConditionsSatisfied()
		{
			bool returnValue = true;
			if( this.allEntryConditionsHaveToBeSatisfied )
			{	
				for(int i = 0; i<this.entryConditions.Count; i++)
					if(	!this.entryConditions[i].IsConditionSatisfiedByGivenPVOPositions(this.now(),
				   		this.pvoPositionsForOutOfSample) )
					{	
						returnValue = false;
						i = this.entryConditions.Count;//returnValue == false -> exit
					}
			}
			else // !this.allEntryConditionsHaveToBeSatisfied
			{	
				for(int i = 0; i<this.entryConditions.Count; i++)
					if(	this.entryConditions[i].IsConditionSatisfiedByGivenPVOPositions(this.now(),
				   		this.pvoPositionsForOutOfSample) )
						i = this.entryConditions.Count;//returnValue == true -> exit
			}	
			return returnValue;
		}
		
		private WeightedPositions marketCloseEventHandler_openPositionsIfTheCase_getWeightedPositionsToInvest()
		{
			WeightedPositions weightedPositions = 
				this.pvoPositionsForOutOfSample.WeightedPositions;
			if(this.openOnlyLongPositions)
			{
				SignedTickers signedTickers = new SignedTickers();
				foreach(WeightedPosition position in this.pvoPositionsForOutOfSample.WeightedPositions.GetValueList())
					if(position.IsLong)
					{
						signedTickers.Add(new SignedTicker(position.Ticker));
						weightedPositions =
							new WeightedPositions(signedTickers);
					}
			}
			return weightedPositions;
		}
			
		
		protected void marketCloseEventHandler_openPositionsIfTheCase(IndexBasedEndOfDayTimer timer)
		{
			PVOPositionsStatus pvoPositionsStatus = PVOPositionsStatus.InTheMiddle;
			if(timer.CurrentDateArrayPosition >= this.numDaysForOscillatingPeriod)
			{	
				pvoPositionsStatus =
					this.marketCloseEventHandler_openPositions_getStatus(timer);
			}
			switch (pvoPositionsStatus){
				case PVOPositionsStatus.Overbought:
					{
						#region manage Overbought case
						try{
							if( this.areEntryConditionsSatisfied() )
							{
								this.pvoPositionsForOutOfSample.WeightedPositions.Reverse();
								AccountManager.OpenPositions( marketCloseEventHandler_openPositionsIfTheCase_getWeightedPositionsToInvest(),
							                             		this.account, 10000.0, this.leverage );
							}
							this.previousAccountValue = this.account.GetMarketValue();
						}
						catch(Exception ex)
						{
							string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
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
						if( this.areEntryConditionsSatisfied() )
							AccountManager.OpenPositions( marketCloseEventHandler_openPositionsIfTheCase_getWeightedPositionsToInvest(),
						                              this.account, 10000.0, this.leverage );
						
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
			if( this.stopLossConditionReached ||
			    this.takeProfitConditionReached ||
			    this.maxNumberOfDaysOnTheMarketReached )
			{
				AccountManager.ClosePositions(this.account);
				this.currentAccountValue = 0.0;
				this.previousAccountValue = 0.0;
				this.daysOnTheMarket = 0;
			}
		}
		
		protected virtual void marketCloseEventHandler_updateClosingConditions()
		{
			//this.previousAccountValue has been set at opening positions
			this.stopLossConditionReached = false;
			this.takeProfitConditionReached = false;
			this.maxNumberOfDaysOnTheMarketReached = false;
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
			if ( this.maxNumberOfDaysOnTheMarket == this.daysOnTheMarket )
				this.maxNumberOfDaysOnTheMarketReached = true;
		}
		
		private void marketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			try{
				this.marketCloseEventHandler_updateClosingConditions();
				//this.marketCloseEventHandler_closePositionsIfNeeded();
				if( this.chosenPVOPositions != null )
					//PVOPositions have been chosen by the chooser
				{
					if(this.account.Portfolio.Count == 0)
						this.marketCloseEventHandler_openPositionsIfTheCase((IndexBasedEndOfDayTimer)sender);
					//positions are opened only if thresholds are reached
					else//there are some opened positions
						this.marketCloseEventHandler_closePositionsIfNeeded();
				}
			}
			catch(TickerNotExchangedException ex)
			{
				string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
			}
		}
		#endregion
		
		#region OneHourAfterMarketCloseEventHandler
		
		protected virtual void updateReturnsManager(DateTime firstDateTime,
		                                            DateTime lastDateTime)
		{
			this.returnsManager =
				new ReturnsManager(new CloseToCloseIntervals(firstDateTime, lastDateTime,
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
		private void logOptimizationInfo( EligibleTickers eligibleTickers )
		{
			this.raiseNewLogItem( eligibleTickers );
			this.notifyMessage( eligibleTickers );
		}

		protected virtual void updateTestingPositions(DateTime currentDate)
		{
			History history =
				this.benchmark.GetEndOfDayHistory(
					HistoricalEndOfDayTimer.GetMarketClose(
						currentDate.AddDays( -this.inSampleDays ) ) ,
					HistoricalEndOfDayTimer.GetMarketClose( currentDate ) );
//					new EndOfDayDateTime(currentDate.AddDays(-this.inSampleDays),
//					                     EndOfDaySpecificTime.MarketClose),
//					new EndOfDayDateTime(currentDate,
//					                     EndOfDaySpecificTime.MarketClose));
			EligibleTickers eligibles =
				this.eligiblesSelector.GetEligibleTickers(history);
			this.updateReturnsManager(history.FirstDateTime,
			                          history.LastDateTime);
			if(this.inSampleChooser != null)
				this.chosenPVOPositions = (TestingPositions[])inSampleChooser.AnalyzeInSample(eligibles, this.returnsManager );
			this.updateTestingPositions_updateThresholds();
			this.logOptimizationInfo(eligibles);
		}
		
		/// <summary>
		/// Handles a "One hour after market close" event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		private void oneHourAfterMarketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			this.lastCloseDate = dateTime;
			this.numDaysElapsedSinceLastOptimization++;
			if((this.numDaysElapsedSinceLastOptimization ==
			    this.numDaysBetweenEachOptimization))
				//num days without optimization has elapsed
			{
				this.updateTestingPositions(dateTime);
				//sets tickers to be chosen next Market Close event
				this.numDaysElapsedSinceLastOptimization = 0;
			}
			if( this.account.Portfolio.Count > 0 )
				this.daysOnTheMarket++;
		}
		#endregion
		
		public virtual void NewDateTimeEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( HistoricalEndOfDayTimer.IsMarketOpen( dateTime ) )
				this.marketOpenEventHandler( sender , dateTime );
			if ( HistoricalEndOfDayTimer.IsMarketClose( dateTime ) )
				this.marketCloseEventHandler( sender , dateTime );
			if ( HistoricalEndOfDayTimer.IsOneHourAfterMarketClose( dateTime ) )
				this.oneHourAfterMarketCloseEventHandler( sender , dateTime );
		}
	}
}
