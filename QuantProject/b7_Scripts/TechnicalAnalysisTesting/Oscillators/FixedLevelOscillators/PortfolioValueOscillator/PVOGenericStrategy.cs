/*
QuantProject - Quantitative Finance Library

PVOGenericStrategy.cs
Copyright (C) 2009
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
using QuantProject.ADT.Timing;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Business.Timing.TimingManagement;
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
using QuantProject.Data.DataProviders.Bars.Caching;
using QuantProject.Data.Selectors;
using QuantProject.Data.DataTables;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.InSampleChoosers;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.EntryConditions;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.WalkForwardTesting.LinearCombination;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator
{
	/// <summary>
	/// This class contains the core strategy for the Portfolio Value
	/// Oscillator
	/// </summary>
	[Serializable]
	public class PVOGenericStrategy : IStrategyForBacktester
	{
		public event NewLogItemEventHandler NewLogItem;
		public event NewMessageEventHandler NewMessage;

		//initialized by the constructor
		protected int inSampleDays;
		protected int numDaysBetweenEachOptimization;
		protected IInSampleChooser inSampleChooser;
		protected IEligiblesSelector eligiblesSelector;
		protected Benchmark benchmark;
		protected HistoricalMarketValueProvider
			historicalMarketValueProviderForInSample;
		protected HistoricalMarketValueProvider
			historicalMarketValueProviderForOutOfSample;
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
		protected PVOPositions positionsForOutOfSample;
		protected PVOPositionsStatus positionsForOutOfSampleStatus;
		
		protected DateTime lastOptimizationDateTime;
		protected Account account;
		public Account Account
		{
			get { return this.account; }
			set { this.account = value; }
		}
		
		private int minimumNumberOfEligiblesForValidOptimization;
		
		protected bool stopLossConditionReached;
		protected bool takeProfitConditionReached;
		protected double inefficiencyLengthInMarketIntervals;
		protected int numberOfPreviousEfficientPeriods;
		protected int numberOfDaysForPriceRatioAnalysis;
		protected double numberOfStdDeviationForSignificantPriceRatioMovements;
		protected double maxOpeningLengthInMarketIntervals;
		protected double marketIntervalsFromLastInefficiencyTimeToWaitBeforeOpening;
		protected double marketIntervalsFromLastLossOrProfitToWaitBeforeClosing;
		protected DateTime lastEntryDateTime;
		protected DateTime lastProfitOrLossDateTime;
		protected DateTime lastInefficiencyDateTime;
		protected List<Time> openingTimesForAvailableBars;
		protected double currentAccountValue;
		protected double previousAccountValue;
		protected double stopLoss;
		protected double takeProfit;
		protected double leverage;
		protected bool openOnlyLongPositions;
		protected List<IEntryCondition> entryConditions;
		protected bool allEntryConditionsHaveToBeSatisfied;
		private MarketIntervalsManager marketIntervalsManagerForOutOfSample;

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
					"PVO_Intraday\n" +
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

		private void pvoStrategyIntraday_checkTimeParameters()
		{
			;//	to do
		}
		
		private void pvoGenericStrategy(IEligiblesSelector eligiblesSelector,
		                                 int minimumNumberOfEligiblesForValidOptimization,
		                                 int inSampleDays,
		                                 Benchmark benchmark,
		                                 int numDaysBetweenEachOptimization,
		                                 double oversoldThreshold,
		                                 double overboughtThreshold,
		                                 double oversoldThresholdMAX,
		                                 double overboughtThresholdMAX,
		                                 HistoricalMarketValueProvider historicalMarketValueProviderForInSample,
		                                 HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample,
		                                 double inefficiencyLengthInMarketIntervals,
		                                 int numberOfPreviousEfficientPeriods,
		                                 int numberOfDaysForPriceRatioAnalysis,
																		 double numberOfStdDeviationForSignificantPriceRatioMovements, 
		                                 double marketIntervalsFromLastInefficiencyTimeToWaitBeforeOpening,
		                                 double marketIntervalsFromLastLossOrProfitToWaitBeforeClosing,
		                                 double maxOpeningLengthInMarketIntervals,
		                                 List<Time> openingTimesForAvailableBars,
		                                 double stopLoss, double takeProfit, double leverage,
		                                 bool openOnlyLongPositions,
															 			 List<IEntryCondition> entryConditions,
															 			 bool allEntryConditionsHaveToBeSatisfied,
															 			 MarketIntervalsManager marketIntervalsManagerForOutOfSample)
		{
			this.eligiblesSelector = eligiblesSelector;
			this.minimumNumberOfEligiblesForValidOptimization =
				minimumNumberOfEligiblesForValidOptimization;
			this.inSampleDays = inSampleDays;
			this.benchmark = benchmark;
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
			this.oversoldThreshold = oversoldThreshold;
			this.overboughtThreshold = overboughtThreshold;
			this.oversoldThresholdMAX = oversoldThresholdMAX;
			this.overboughtThresholdMAX = overboughtThresholdMAX;
			this.historicalMarketValueProviderForInSample =
				historicalMarketValueProviderForInSample;
			this.historicalMarketValueProviderForOutOfSample = 
				historicalMarketValueProviderForOutOfSample;
			this.inefficiencyLengthInMarketIntervals = inefficiencyLengthInMarketIntervals;
			this.numberOfPreviousEfficientPeriods = numberOfPreviousEfficientPeriods;
			this.numberOfDaysForPriceRatioAnalysis = numberOfDaysForPriceRatioAnalysis;
			this.numberOfStdDeviationForSignificantPriceRatioMovements = 
				numberOfStdDeviationForSignificantPriceRatioMovements;
			this.marketIntervalsFromLastInefficiencyTimeToWaitBeforeOpening =
				marketIntervalsFromLastInefficiencyTimeToWaitBeforeOpening;
			this.marketIntervalsFromLastLossOrProfitToWaitBeforeClosing =
				marketIntervalsFromLastLossOrProfitToWaitBeforeClosing;
			this.maxOpeningLengthInMarketIntervals = maxOpeningLengthInMarketIntervals;
			this.openingTimesForAvailableBars = openingTimesForAvailableBars;
			this.stopLoss = stopLoss;
			this.takeProfit = takeProfit;
			this.leverage = leverage;
			this.lastOptimizationDateTime = DateTime.MinValue;
			this.pvoStrategyIntraday_checkTimeParameters();
			this.lastEntryDateTime = new DateTime(1900,1,1,0,0,0);
			this.lastInefficiencyDateTime = new DateTime(1900,1,1,0,0,0);
			this.lastProfitOrLossDateTime = new DateTime(1900,1,1,0,0,0);
			this.openOnlyLongPositions = openOnlyLongPositions;
			this.entryConditions = entryConditions;
			this.allEntryConditionsHaveToBeSatisfied = allEntryConditionsHaveToBeSatisfied;
			this.marketIntervalsManagerForOutOfSample = marketIntervalsManagerForOutOfSample;
		}

		public PVOGenericStrategy(IEligiblesSelector eligiblesSelector,
		                           int minimumNumberOfEligiblesForValidOptimization,
		                           IInSampleChooser inSampleChooser,
		                           int inSampleDays,
		                           Benchmark benchmark,
		                           int numDaysBetweenEachOptimization,
		                           double oversoldThreshold,
		                           double overboughtThreshold,
		                           double oversoldThresholdMAX,
		                           double overboughtThresholdMAX,
		                           HistoricalMarketValueProvider historicalMarketValueProviderForInSample,
		                           HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample,
		                           double inefficiencyLengthInMarketIntervals,
		                           int numberOfPreviousEfficientPeriods,
		                           int numberOfDaysForPriceRatioAnalysis,
															 double numberOfStdDeviationForSignificantPriceRatioMovements,
		                           double marketIntervalsFromLastInefficiencyTimeToWaitBeforeOpening,
		                           double marketIntervalsFromLastLossOrProfitToWaitBeforeClosing,
		                           double maxOpeningLengthInMarketIntervals,
		                           List<Time> openingTimesForAvailableBars,
		                           double stopLoss, double takeProfit,
		                           double leverage,
		                           bool openOnlyLongPositions,
															 List<IEntryCondition> entryConditions,
															 bool allEntryConditionsHaveToBeSatisfied,
															 MarketIntervalsManager marketIntervalsManager)
			
		{
			this.pvoGenericStrategy(eligiblesSelector, minimumNumberOfEligiblesForValidOptimization,
			                         inSampleDays , benchmark , numDaysBetweenEachOptimization ,
			                         oversoldThreshold, overboughtThreshold,
			                         oversoldThresholdMAX, overboughtThresholdMAX,
			                         historicalMarketValueProviderForInSample,
			                         historicalMarketValueProviderForOutOfSample,
			                         inefficiencyLengthInMarketIntervals,
			                         numberOfPreviousEfficientPeriods,
			                         numberOfDaysForPriceRatioAnalysis,
															 numberOfStdDeviationForSignificantPriceRatioMovements,
			                         marketIntervalsFromLastInefficiencyTimeToWaitBeforeOpening,
			                         marketIntervalsFromLastLossOrProfitToWaitBeforeClosing,
			                         maxOpeningLengthInMarketIntervals,
			                         openingTimesForAvailableBars,
			                         stopLoss, takeProfit, leverage,
			                         openOnlyLongPositions,
															 entryConditions,
															 allEntryConditionsHaveToBeSatisfied,
															 marketIntervalsManager);
			this.inSampleChooser = inSampleChooser;
		}
		
		public PVOGenericStrategy(IEligiblesSelector eligiblesSelector,
		                           int minimumNumberOfEligiblesForValidOptimization,
		                           IInSampleChooser inSampleChooser,
		                           int inSampleDays,
		                           Benchmark benchmark,
		                           int numDaysBetweenEachOptimization,
		                           double oversoldThreshold,
		                           double overboughtThreshold,
		                           HistoricalMarketValueProvider historicalMarketValueProviderForInSample,
		                           HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample,
		                           double inefficiencyLengthInMarketIntervals,
		                           int numberOfPreviousEfficientPeriods,
		                           int numberOfDaysForPriceRatioAnalysis,
															 double numberOfStdDeviationForSignificantPriceRatioMovements,
		                           double marketIntervalsFromLastInefficiencyTimeToWaitBeforeOpening,
		                           double marketIntervalsFromLastLossOrProfitToWaitBeforeClosing,
		                           double maxOpeningLengthInMarketIntervals,
		                           List<Time> openingTimesForAvailableBars,
		                           double stopLoss, double takeProfit, double leverage,
		                           bool openOnlyLongPositions,
															 List<IEntryCondition> entryConditions,
															 bool allEntryConditionsHaveToBeSatisfied,
															 MarketIntervalsManager marketIntervalsManager)
			
		{
			this.pvoGenericStrategy(eligiblesSelector, minimumNumberOfEligiblesForValidOptimization,
			                         inSampleDays , benchmark , numDaysBetweenEachOptimization ,
			                         oversoldThreshold, overboughtThreshold,
			                         double.MaxValue, double.MaxValue,
			                         historicalMarketValueProviderForInSample,
			                         historicalMarketValueProviderForOutOfSample,
			                         inefficiencyLengthInMarketIntervals,
			                         numberOfPreviousEfficientPeriods,
			                         numberOfDaysForPriceRatioAnalysis,
															 numberOfStdDeviationForSignificantPriceRatioMovements,
			                         marketIntervalsFromLastInefficiencyTimeToWaitBeforeOpening,
			                         marketIntervalsFromLastLossOrProfitToWaitBeforeClosing,
			                         maxOpeningLengthInMarketIntervals,
			                         openingTimesForAvailableBars,
			                         stopLoss, takeProfit, leverage,
			                         openOnlyLongPositions,
															 entryConditions,
															 allEntryConditionsHaveToBeSatisfied,
															 marketIntervalsManager);
			this.inSampleChooser = inSampleChooser;
		}
		public PVOGenericStrategy(IEligiblesSelector eligiblesSelector,
		                           int minimumNumberOfEligiblesForValidOptimization,
		                           TestingPositions[] chosenPVOPositions,
		                           int inSampleDays,
		                           Benchmark benchmark,
		                           int numDaysBetweenEachOptimization,
		                           double oversoldThreshold,
		                           double overboughtThreshold,
		                           double oversoldThresholdMAX,
		                           double overboughtThresholdMAX,
		                           HistoricalMarketValueProvider historicalMarketValueProviderForInSample,
		                           HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample,
		                           double inefficiencyLengthInMarketIntervals,
		                           int numberOfPreviousEfficientPeriods,
		                           int numberOfDaysForPriceRatioAnalysis,
															 double numberOfStdDeviationForSignificantPriceRatioMovements,
		                           double marketIntervalsFromLastInefficiencyTimeToWaitBeforeOpening,
		                           double marketIntervalsFromLastLossOrProfitToWaitBeforeClosing,
		                           double maxOpeningLengthInMarketIntervals,
		                           List<Time> openingTimesForAvailableBars,
		                           double stopLoss, double takeProfit, double leverage,
		                           bool openOnlyLongPositions,
															 List<IEntryCondition> entryConditions,
															 bool allEntryConditionsHaveToBeSatisfied,
															 MarketIntervalsManager marketIntervalsManager)
			
		{
			this.pvoGenericStrategy(eligiblesSelector, minimumNumberOfEligiblesForValidOptimization,
			                         inSampleDays , benchmark , numDaysBetweenEachOptimization ,
			                         oversoldThreshold, overboughtThreshold,
			                         oversoldThresholdMAX, overboughtThresholdMAX,
			                         historicalMarketValueProviderForInSample,
			                         historicalMarketValueProviderForOutOfSample,
			                         inefficiencyLengthInMarketIntervals,
			                         numberOfPreviousEfficientPeriods,
			                         numberOfDaysForPriceRatioAnalysis,
															 numberOfStdDeviationForSignificantPriceRatioMovements,
			                         marketIntervalsFromLastInefficiencyTimeToWaitBeforeOpening,
			                         marketIntervalsFromLastLossOrProfitToWaitBeforeClosing,
			                         maxOpeningLengthInMarketIntervals,
			                         openingTimesForAvailableBars,
			                         stopLoss, takeProfit, leverage,
			                         openOnlyLongPositions,
															 entryConditions,
															 allEntryConditionsHaveToBeSatisfied,
															 marketIntervalsManager);
			this.chosenPVOPositions = chosenPVOPositions;
		}
		public PVOGenericStrategy(IEligiblesSelector eligiblesSelector,
		                           int minimumNumberOfEligiblesForValidOptimization,
		                           TestingPositions[] chosenPVOPositions,
		                           int inSampleDays,
		                           Benchmark benchmark,
		                           int numDaysBetweenEachOptimization,
		                           double oversoldThreshold,
		                           double overboughtThreshold,
		                           HistoricalMarketValueProvider historicalMarketValueProviderForInSample,
		                           HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample,
		                           double inefficiencyLengthInMarketIntervals,
		                           int numberOfPreviousEfficientPeriods,
		                           int numberOfDaysForPriceRatioAnalysis,
															 double numberOfStdDeviationForSignificantPriceRatioMovements,
		                           double marketIntervalsFromLastInefficiencyTimeToWaitBeforeOpening,
		                           double marketIntervalsFromLastLossOrProfitToWaitBeforeClosing,
		                           double maxOpeningLengthInMarketIntervals,
		                           List<Time> openingTimesForAvailableBars,
		                           double stopLoss, double takeProfit, double leverage,
		                           bool openOnlyLongPositions,
															 List<IEntryCondition> entryConditions,
															 bool allEntryConditionsHaveToBeSatisfied,
															 MarketIntervalsManager marketIntervalsManager)
		{
			this.pvoGenericStrategy(eligiblesSelector, minimumNumberOfEligiblesForValidOptimization,
			                         inSampleDays , benchmark , numDaysBetweenEachOptimization ,
			                         oversoldThreshold, overboughtThreshold,
			                         double.MaxValue, double.MaxValue,
			                         historicalMarketValueProviderForInSample,
			                         historicalMarketValueProviderForOutOfSample,
			                         inefficiencyLengthInMarketIntervals,
			                         numberOfPreviousEfficientPeriods,
			                         numberOfDaysForPriceRatioAnalysis,
															 numberOfStdDeviationForSignificantPriceRatioMovements,
			                         marketIntervalsFromLastInefficiencyTimeToWaitBeforeOpening,
			                         marketIntervalsFromLastLossOrProfitToWaitBeforeClosing,
			                         maxOpeningLengthInMarketIntervals,
			                         openingTimesForAvailableBars,
			                         stopLoss, takeProfit, leverage,
			                         openOnlyLongPositions,
															 entryConditions,
															 allEntryConditionsHaveToBeSatisfied,
															 marketIntervalsManager);
			this.chosenPVOPositions = chosenPVOPositions;
		}
				
		private bool allTickersAreExchanged(DateTime dateTime, string[] tickers)
		{
			bool returnValue = true;
			try{
				for( int i = 0; i < tickers.Length; i++ )
				{
					if(!this.historicalMarketValueProviderForOutOfSample.WasExchanged( tickers[i], dateTime ) )
					{
						returnValue = false;
						i = tickers.Length; //exit from for
					}
				}
			}
			catch(Exception ex){
				string forBreakpoint = ex.Message;
				forBreakpoint = forBreakpoint + "";
				returnValue = false;
			}
			return returnValue;
		}
		
		#region newDateTimeEventHandler_closePositions
		
		private void newDateTimeEventHandler_closePositions(DateTime currentTime)
		{
			bool positionsHaveBeenOpenedEnough = false;
			bool isTimeToProfitOrToStopALoss = false;
			try{
				positionsHaveBeenOpenedEnough = (this.account.Portfolio.Count > 0 &&
				 	this.marketIntervalsManagerForOutOfSample.AddMarketIntervals(this.lastEntryDateTime,
					 	(int)this.maxOpeningLengthInMarketIntervals ) <= currentTime );
			
				isTimeToProfitOrToStopALoss = this.account.Portfolio.Count > 0 &&
					(this.takeProfitConditionReached  || this.stopLossConditionReached) &&
					(marketIntervalsManagerForOutOfSample.AddMarketIntervals(lastProfitOrLossDateTime,
				                                        (int)marketIntervalsFromLastLossOrProfitToWaitBeforeClosing) == currentTime );
			}
			catch{}
			
			if( (positionsHaveBeenOpenedEnough || isTimeToProfitOrToStopALoss ) &&
			   allTickersAreExchanged( this.now(), AccountManager.GetTickersInOpenedPositions(this.account) ) )
			{
				AccountManager.ClosePositions( this.account );
				this.currentAccountValue = 0.0;
				this.previousAccountValue = 0.0;
				this.lastEntryDateTime = new DateTime(1900,1,1,0,0,0);
				this.lastInefficiencyDateTime = new DateTime(1900,1,1,0,0,0);
				this.lastProfitOrLossDateTime = new DateTime(1900,1,1,0,0,0);
				this.takeProfitConditionReached = false;
				this.stopLossConditionReached = false;
			}
		}
		#endregion newDateTimeEventHandler_closePositions
		
		#region newDateTimeEventHandler_openPositions
		private void newDateTimeEventHandler_openPositions(DateTime currentDateTime)
		{
			DateTime maxDateTimeForClose;
			try{ 
				maxDateTimeForClose =
					marketIntervalsManagerForOutOfSample.AddMarketIntervals(currentDateTime, (int)maxOpeningLengthInMarketIntervals);
			}
			catch{
				return;
			}
			if(	this.account.Portfolio.Count == 0 &&
			   this.positionsForOutOfSample != null &&
			   this.allTickersAreExchanged( currentDateTime, this.positionsForOutOfSample.WeightedPositions.SignedTickers.Tickers ) &&
			   this.allTickersAreExchanged( maxDateTimeForClose, this.positionsForOutOfSample.WeightedPositions.SignedTickers.Tickers ) )
			{
				switch (this.positionsForOutOfSampleStatus)
				{
					case PVOPositionsStatus.Overbought:
					{
						#region manage Overbought case
						this.positionsForOutOfSample.WeightedPositions.Reverse();
						try
						{
							AccountManager.OpenPositions( this.positionsForOutOfSample.WeightedPositions,
							                             this.account, 10000, this.leverage );
							this.lastEntryDateTime = currentDateTime;
							this.previousAccountValue = this.account.GetMarketValue();
						}
						catch(Exception ex)
						{
							string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
						}
						finally
						{
							this.positionsForOutOfSample.WeightedPositions.Reverse();
						}
						#endregion
						break;
					}
					case PVOPositionsStatus.Oversold:
					{
						#region manage Oversold case
						try
						{
							AccountManager.OpenPositions( this.positionsForOutOfSample.WeightedPositions,
							                             this.account, 10000, this.leverage );
							this.lastEntryDateTime = currentDateTime;
							this.previousAccountValue = this.account.GetMarketValue();
						}
						catch(Exception ex)
						{
							string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
						}
						#endregion oversold case
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
		}
		#endregion newDateTimeEventHandler_openPositions
		
		#region newDateTimeEventHandler_updateStatus
		private void newDateTimeEventHandler_setPositionsAndStatus()
		{
			DateTime currentDateTime = this.now();
			DateTime beginOfOscillatingPeriod =
				marketIntervalsManagerForOutOfSample.AddMarketIntervals(currentDateTime, (int)-inefficiencyLengthInMarketIntervals);
			DateTime endOfOscillatingPeriod = currentDateTime;
			this.positionsForOutOfSampleStatus = PVOPositionsStatus.InTheMiddle;
			for(int i = 0; i<this.chosenPVOPositions.Length; i++)
			{
				if(this.chosenPVOPositions[i] != null)
					try{
					this.positionsForOutOfSampleStatus =
						((PVOPositions)this.chosenPVOPositions[i]).GetStatus(beginOfOscillatingPeriod, endOfOscillatingPeriod,
						                                                     this.benchmark.Ticker, this.historicalMarketValueProviderForOutOfSample,
						                                                     this.oversoldThresholdMAX, this.overboughtThresholdMAX);
					
				}catch(Exception ex){string str = ex.ToString();}
				
				if(this.positionsForOutOfSampleStatus == PVOPositionsStatus.Oversold ||
				   this.positionsForOutOfSampleStatus == PVOPositionsStatus.Overbought )
				{
					this.positionsForOutOfSample = (PVOPositions)this.chosenPVOPositions[i];
					this.lastInefficiencyDateTime = currentDateTime;
					this.positionsForOutOfSample.StatusAtLastInefficiencyTime = 
						this.positionsForOutOfSampleStatus;
					this.positionsForOutOfSample.LastInefficiencyDateTime = 
						this.lastInefficiencyDateTime;
					i = this.chosenPVOPositions.Length;//exit from for
				}
			}
		}
		#endregion newDateTimeEventHandler_updateStatus
		
		#region newDateTimeEventHandler_updateStopLossAndTakeProfitConditions
		protected virtual void newDateTimeEventHandler_updateStopLossAndTakeProfitConditions(DateTime currentDateTime)
		{
			//this.previousAccountValue has been set at opening positions
			if( this.account.Portfolio.Count > 0 &&
			   this.takeProfitConditionReached == false &&
			   this.stopLossConditionReached == false &&
			   this.lastProfitOrLossDateTime == new DateTime(1900,1,1,0,0,0) )
			{
				try{
					this.currentAccountValue = this.account.GetMarketValue();
					double portfolioGainOrLoss = (this.currentAccountValue - this.previousAccountValue)
						/this.previousAccountValue;
					
					if(!double.IsInfinity(portfolioGainOrLoss) &&
					   portfolioGainOrLoss <= -this.stopLoss )
					{
						this.stopLossConditionReached = true;
						this.lastProfitOrLossDateTime = currentDateTime;
					}
					else if (!double.IsInfinity(portfolioGainOrLoss) &&
					         portfolioGainOrLoss >= this.takeProfit )
						
					{
						this.takeProfitConditionReached = true;
						this.lastProfitOrLossDateTime = currentDateTime;
					}
				}
				catch(Exception ex)
				{string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";}
			}
		}
		
		#endregion newDateTimeEventHandler_updateStopLossAndTakeProfitConditions
		
		private void newDateTimeEventHandler_resetForNewResearch(bool areEntryConditionsSatisfied,
		                                                         DateTime currentDateTime)
		{
			try{
			if( currentDateTime >
			   	marketIntervalsManagerForOutOfSample.AddMarketIntervals(lastInefficiencyDateTime,
			                                          (int)this.maxOpeningLengthInMarketIntervals) ||
			    areEntryConditionsSatisfied == true )
			   this.lastInefficiencyDateTime = 
						new DateTime(1900,1,1,0,0,0); //it forces a new research of an inefficiency
			}
			catch(Exception ex)
			{
				string s = ex.ToString();
			}
		}
		
		private bool areEntryConditionsSatisfied_checkCondition(DateTime currentDateTime,
		                                                        int conditionIndex)
		{
			bool returnValue = false;
			try{
				if( this.entryConditions[conditionIndex].IsConditionSatisfiedByGivenPVOPositions(currentDateTime,
				                                                                    this.positionsForOutOfSample) )
					returnValue = true;
			}
			catch(Exception ex)
			{
				string s = ex.ToString();
			}
			return returnValue;
		}
		
		private bool areEntryConditionsSatisfied(DateTime currentDateTime)
		{
			bool returnValue = true;
			if( this.allEntryConditionsHaveToBeSatisfied )
			{	
				for(int i = 0; i<this.entryConditions.Count; i++)
					if(	! this.areEntryConditionsSatisfied_checkCondition( currentDateTime, i ) )
					{	
						returnValue = false;
						i = this.entryConditions.Count;//returnValue == false -> exit
					}
			}
			else // !this.allEntryConditionsHaveToBeSatisfied
			{	
				for(int i = 0; i<this.entryConditions.Count; i++)
					if(	this.areEntryConditionsSatisfied_checkCondition( currentDateTime, i ) )
						i = this.entryConditions.Count;//returnValue == true -> exit
			}	
			return returnValue;
		}
		
		private bool newDateTimeEventHandler_dateTimeIsInRange(DateTime dateTime)
		{
			bool returnValue = false;
			try{
				DateTime maxDate = 
					this.marketIntervalsManagerForOutOfSample.AddMarketIntervals(dateTime,
					                                            (int)this.maxOpeningLengthInMarketIntervals );
				DateTime beginningOfPreviousEfficientPeriods = 
					this.marketIntervalsManagerForOutOfSample.AddMarketIntervals(dateTime,
					                                            -(int)inefficiencyLengthInMarketIntervals -
					                                            (int)inefficiencyLengthInMarketIntervals * this.numberOfPreviousEfficientPeriods);
				returnValue = true; //if the two previous assignements can be done, dateTime is in range	
			}
			catch{}
			
			return returnValue;
		}
		
		public virtual void NewDateTimeEventHandler(
			Object sender , DateTime dateTime )
		{
			this.newDateTimeEventHandler_updateStopLossAndTakeProfitConditions(dateTime);
			this.newDateTimeEventHandler_closePositions(dateTime);
			if( this.account.Portfolio.Count == 0 &&
			   this.chosenPVOPositions != null &&
			   this.lastInefficiencyDateTime == new DateTime(1900,1,1,0,0,0) &&
			   this.newDateTimeEventHandler_dateTimeIsInRange(dateTime) )
				//portfolio empty, optimization done, no inefficiency found (or found but not deployed), time OK
						this.newDateTimeEventHandler_setPositionsAndStatus();
			
			bool areEntryConditionsSatisfied = this.areEntryConditionsSatisfied(dateTime);
			
			if( this.account.Portfolio.Count == 0 &&
				   this.lastInefficiencyDateTime != new DateTime(1900,1,1,0,0,0) &&
				   dateTime >=
				   marketIntervalsManagerForOutOfSample.AddMarketIntervals( lastInefficiencyDateTime,
				                                          (int)marketIntervalsFromLastInefficiencyTimeToWaitBeforeOpening ) &&
				   areEntryConditionsSatisfied
				)
						this.newDateTimeEventHandler_openPositions(dateTime);
			
			this.newDateTimeEventHandler_resetForNewResearch(areEntryConditionsSatisfied,
			                                                 dateTime);
			this.newDateTimeEventHandler_updateTestingPositions( dateTime );
			
			//one hour after market close
//			if( new Time(dateTime) == new Time("17:00:00") )
				//it's time for new optimization, if the case
//			{
//				this.newDateTimeEventHandler_updateTestingPositions( dateTime );
//				this.positionsForOutOfSample = null;
//				this.positionsForOutOfSampleStatus = PVOPositionsStatus.InTheMiddle;
//				this.lastInefficiencyDateTime = new DateTime(1900,1,1,0,0,0);
//			}
		}
		
		#region UpdateTestingPositions
		
		protected void updateTestingPositions(DateTime currentDateTime)
		{
			History history =
				this.benchmark.GetEndOfDayHistory(
					HistoricalEndOfDayTimer.GetMarketOpen(
						currentDateTime.AddDays( -this.inSampleDays ) ) ,
					HistoricalEndOfDayTimer.GetMarketClose(
						currentDateTime ) );
			EligibleTickers eligibles =
				this.eligiblesSelector.GetEligibleTickers(history);
			//To be changed how the returns manager is updated?
			this.updateReturnsManager(Time.GetDateTimeFromMerge(history.FirstDateTime, this.openingTimesForAvailableBars[0]),
			                          Time.GetDateTimeFromMerge(history.LastDateTime, this.openingTimesForAvailableBars[this.openingTimesForAvailableBars.Count - 1]));
			if( (  this.eligiblesSelector is DummyEligibleSelector &&
			     this.inSampleChooser != null )    ||
			   (  eligibles.Count > this.minimumNumberOfEligiblesForValidOptimization &&
			    this.inSampleChooser != null )  )
			{
				this.chosenPVOPositions = (TestingPositions[])inSampleChooser.AnalyzeInSample(eligibles, this.returnsManager);
				this.updateTestingPositions_updateThresholds();
				this.logOptimizationInfo(eligibles);
			}
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
					 ( this.now() >= dateTimeForNextOptimization ) );
			}
			return areToBeUpdated;
		}
		
		private void newDateTimeEventHandler_updateTestingPositions(
			DateTime dateTime )
		{
			if ( this.optimalTestingPositionsAreToBeUpdated() )
			{
				this.updateTestingPositions( dateTime );
				this.lastOptimizationDateTime = this.now();
				this.positionsForOutOfSample = null;
				this.positionsForOutOfSampleStatus = PVOPositionsStatus.InTheMiddle;
				this.lastInefficiencyDateTime = new DateTime(1900,1,1,0,0,0);

			}
		}
		#endregion UpdateTestingPositions
		
		private DateTime now()
		{
			return this.account.Timer.GetCurrentDateTime();
		}
		
		protected virtual void updateReturnsManager(DateTime firstDateTime,
		                                            DateTime lastDateTime)
		{
			ReturnIntervals returnIntervals = 
				new DailyOpenToCloseIntervals(firstDateTime, lastDateTime,
						                                                this.benchmark.Ticker );;
			
			if(this.inSampleChooser is PVOIntradayCorrelationChooser)
				returnIntervals =
					new IntradayIntervals(firstDateTime, lastDateTime,
					                      ((PVOIntradayCorrelationChooser)this.inSampleChooser).ReturnIntervalLengthInMinutes,
					                      this.benchmark.Ticker);
			
			if( this.inSampleChooser is PVOCorrelationChooser )
			{
				switch ( ((PVOCorrelationChooser)this.inSampleChooser).IntervalsType )
				{
					case IntervalsType.CloseToCloseIntervals:
						returnIntervals = new CloseToCloseIntervals(firstDateTime, lastDateTime,
						                                            this.benchmark.Ticker, ((PVOCorrelationChooser)this.inSampleChooser).ReturnIntervalLength);
						break;
					case IntervalsType.OpenToOpenIntervals:
						returnIntervals = new OpenToOpenIntervals(firstDateTime, lastDateTime,
						                                          this.benchmark.Ticker, ((PVOCorrelationChooser)this.inSampleChooser).ReturnIntervalLength);
						break;
					case IntervalsType.CloseToOpenIntervals:
						returnIntervals = new CloseToOpenIntervals(firstDateTime, lastDateTime,
						                                           this.benchmark.Ticker);
						break;
					case IntervalsType.OpenToCloseIntervals:
						returnIntervals = new DailyOpenToCloseIntervals(firstDateTime, lastDateTime,
						                                                this.benchmark.Ticker );
						break;
					case IntervalsType.OpenToCloseCloseToOpenIntervals:
						returnIntervals = new OpenToCloseCloseToOpenIntervals(
							firstDateTime, lastDateTime, this.benchmark.Ticker);
						break;
					default:
						// it should never be reached
						returnIntervals = new DailyOpenToCloseIntervals(firstDateTime, lastDateTime,
						                                                this.benchmark.Ticker );
						break;
				}
			}
			this.returnsManager =
				new ReturnsManager( returnIntervals , this.historicalMarketValueProviderForInSample);
		}
		
		private double getLogItem_getFitnessOfLast()
		{
			double returnValue = 0.0;
			for(int i = 1; i<=this.chosenPVOPositions.Length; i++)
			{
				if(this.chosenPVOPositions[this.chosenPVOPositions.Length - i] != null)
				{
					returnValue = this.chosenPVOPositions[this.chosenPVOPositions.Length - i].FitnessInSample;
					i = this.chosenPVOPositions.Length + 1; //exit from for
				}
			}
			return returnValue;
		}
		private string getLogItem_getHashCodeForTickerCompositionForLast()
		{
			string returnValue = "not set";
			for(int i = 1; i<=this.chosenPVOPositions.Length; i++)
			{
				if(this.chosenPVOPositions[this.chosenPVOPositions.Length - i] != null)
				{
					returnValue = this.chosenPVOPositions[this.chosenPVOPositions.Length - i].HashCodeForTickerComposition;
					i = this.chosenPVOPositions.Length + 1; //exit from for
				}
			}
			return returnValue;
		}
		
		private PVO_OTCLogItem getLogItem( EligibleTickers eligibleTickers )
		{
			PVO_OTCLogItem logItem =
				new PVO_OTCLogItem( this.now() , this.inSampleDays );
			logItem.BestPVOPositionsInSample =
				this.chosenPVOPositions;
			logItem.NumberOfEligibleTickers =
				eligibleTickers.Count;
			logItem.FitnessOfFirst =
				this.chosenPVOPositions[0].FitnessInSample;
			logItem.FitnessOfLast = this.getLogItem_getFitnessOfLast();
			
//			logItem.GenerationOfFirst =
//				((IGeneticallyOptimizable)this.chosenPVOPositions[0]).Generation;
//			logItem.GenerationOfLast =
//				((IGeneticallyOptimizable)this.chosenPVOPositions[this.chosenPVOPositions.Length - 1]).Generation;
			logItem.ThresholdsOfFirst =
				((PVOPositions)this.chosenPVOPositions[0]).OversoldThreshold.ToString() + ";" +
				((PVOPositions)this.chosenPVOPositions[0]).OverboughtThreshold.ToString();
			logItem.ThresholdsOfLast = logItem.ThresholdsOfFirst;
//				((PVOPositions)this.chosenPVOPositions[this.chosenPVOPositions.Length - 1]).OversoldThreshold.ToString() + ";" +
//				((PVOPositions)this.chosenPVOPositions[this.chosenPVOPositions.Length - 1]).OverboughtThreshold.ToString();
			logItem.TickersOfFirst =
				this.chosenPVOPositions[0].HashCodeForTickerComposition;
			logItem.TickersOfLast = this.getLogItem_getHashCodeForTickerCompositionForLast();
			
			return logItem;
		}
		private void raiseNewLogItem( EligibleTickers eligibleTickers )
		{
			PVO_OTCLogItem logItem =
				this.getLogItem( eligibleTickers );
			NewLogItemEventArgs newLogItemEventArgs =
				new NewLogItemEventArgs( logItem );
			this.NewLogItem( this , newLogItemEventArgs );
		}
		private void notifyMessage( EligibleTickers eligibleTickers )
		{
			string message = "Number of Eligible tickers (eligible selector: " + 
				this.eligiblesSelector.Description + "): " +
				eligibleTickers.Count;
			NewMessageEventArgs newMessageEventArgs =
				new NewMessageEventArgs( message );
			if ( this.NewMessage != null )
				this.NewMessage( this , newMessageEventArgs );
		}
		private void logOptimizationInfo( EligibleTickers eligibleTickers )
		{
//			if(eligibleTickers.Count > 0)
			try{
				this.raiseNewLogItem( eligibleTickers );
			}
			catch{}
			
			this.notifyMessage( eligibleTickers );
		}

		private void updateTestingPositions_updateThresholds()
		{
			for(int i = 0; i<this.chosenPVOPositions.Length; i++)
			{
				if(this.chosenPVOPositions[i] != null)
				{
					((PVOPositions)this.chosenPVOPositions[i]).OversoldThreshold =
						this.oversoldThreshold;
					((PVOPositions)this.chosenPVOPositions[i]).OverboughtThreshold =
						this.overboughtThreshold;
				}
			}
		}
	}
}
