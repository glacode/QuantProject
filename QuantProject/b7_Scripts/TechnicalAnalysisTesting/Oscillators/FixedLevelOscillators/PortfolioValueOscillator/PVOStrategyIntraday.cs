/*
QuantProject - Quantitative Finance Library

PVOStrategyIntraday.cs
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
using QuantProject.ADT.Histories;
using QuantProject.ADT.Messaging;
using QuantProject.ADT.Timing;
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
	public class PVOStrategyIntraday : IStrategyForBacktester
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
		
		protected DateTime lastTimerDateTimeAfterLastExit;
		protected DateTime lastOptimizationDateTime;
		protected Account account;
		public Account Account
		{
			get { return this.account; }
			set { this.account = value; }
		}
		
		private int minimumNumberOfEligiblesForValidOptimization;
//		private bool optimalPositionsHaveBeenUpdated;
		
		protected Time timeForCheckingCrossingThresholds_begin;
		protected Time timeForCheckingCrossingThresholds_end;
		protected Time timeForEntry;
		protected Time timeForExit;
				
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
					"PVO_OTC\n" +
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
		
		private void pvoStrategyIntraday(IEligiblesSelector eligiblesSelector,
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
		                             Time timeForCheckingCrossingThresholds_begin,
		                             Time timeForCheckingCrossingThresholds_end,
																 Time timeForEntry,
															   Time timeForExit)
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
			this.historicalMarketValueProviderForInSample = historicalMarketValueProviderForInSample;
			this.historicalMarketValueProviderForOutOfSample = historicalMarketValueProviderForOutOfSample;
			this.timeForCheckingCrossingThresholds_begin = timeForCheckingCrossingThresholds_begin;
			this.timeForCheckingCrossingThresholds_end = timeForCheckingCrossingThresholds_end;
			this.timeForEntry = timeForEntry;
			this.timeForExit = timeForExit;
			this.lastOptimizationDateTime = DateTime.MinValue;
			this.pvoStrategyIntraday_checkTimeParameters();
//			this.optimalPositionsHaveBeenUpdated = false;
		}

		public PVOStrategyIntraday(IEligiblesSelector eligiblesSelector,
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
		                       Time timeForCheckingCrossingThresholds_begin,
                           Time timeForCheckingCrossingThresholds_end,
													 Time timeForEntry,
												   Time timeForExit)
			
		{
			this.pvoStrategyIntraday(eligiblesSelector, minimumNumberOfEligiblesForValidOptimization,
			                     inSampleDays , benchmark , numDaysBetweenEachOptimization ,
			                     oversoldThreshold, overboughtThreshold,
			                     oversoldThresholdMAX, overboughtThresholdMAX,
			                     historicalMarketValueProviderForInSample,
		                       historicalMarketValueProviderForOutOfSample,
		                       timeForCheckingCrossingThresholds_begin,
		                       timeForCheckingCrossingThresholds_end,
													 timeForEntry, timeForExit);
			this.inSampleChooser = inSampleChooser;
		}
		
		public PVOStrategyIntraday(IEligiblesSelector eligiblesSelector,
		                       int minimumNumberOfEligiblesForValidOptimization,
		                       IInSampleChooser inSampleChooser,
		                       int inSampleDays,
		                       Benchmark benchmark,
		                       int numDaysBetweenEachOptimization,
		                       double oversoldThreshold,
		                       double overboughtThreshold,
		                       HistoricalMarketValueProvider historicalMarketValueProviderForInSample,
		                       HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample,
		                       Time timeForCheckingCrossingThresholds_begin,
		                       Time timeForCheckingCrossingThresholds_end,
													 Time timeForEntry,
													 Time timeForExit)
			
		{
			this.pvoStrategyIntraday(eligiblesSelector, minimumNumberOfEligiblesForValidOptimization,
			                     inSampleDays , benchmark , numDaysBetweenEachOptimization ,
			                     oversoldThreshold, overboughtThreshold,
			                     double.MaxValue, double.MaxValue,
			                     historicalMarketValueProviderForInSample,
		                       historicalMarketValueProviderForOutOfSample,
		                       timeForCheckingCrossingThresholds_begin,
		                       timeForCheckingCrossingThresholds_end,
													 timeForEntry, timeForExit);
			this.inSampleChooser = inSampleChooser;
		}
		public PVOStrategyIntraday(IEligiblesSelector eligiblesSelector,
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
		                       Time timeForCheckingCrossingThresholds_begin,
		                       Time timeForCheckingCrossingThresholds_end,
													 Time timeForEntry,
													 Time timeForExit)
			
		{
			this.pvoStrategyIntraday(eligiblesSelector, minimumNumberOfEligiblesForValidOptimization,
			                     inSampleDays , benchmark , numDaysBetweenEachOptimization ,
			                     oversoldThreshold, overboughtThreshold,
			                     oversoldThresholdMAX, overboughtThresholdMAX,
			                     historicalMarketValueProviderForInSample,
		                       historicalMarketValueProviderForOutOfSample,
		                       timeForCheckingCrossingThresholds_begin,
		                       timeForCheckingCrossingThresholds_end,
													 timeForEntry, timeForExit);
			this.chosenPVOPositions = chosenPVOPositions;
		}
		public PVOStrategyIntraday(IEligiblesSelector eligiblesSelector,
		                       int minimumNumberOfEligiblesForValidOptimization,
		                       TestingPositions[] chosenPVOPositions,
		                       int inSampleDays,
		                       Benchmark benchmark,
		                       int numDaysBetweenEachOptimization,
		                       double oversoldThreshold,
		                       double overboughtThreshold,
		                       HistoricalMarketValueProvider historicalMarketValueProviderForInSample,
		                       HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample,
		                       Time timeForCheckingCrossingThresholds_begin,
		                       Time timeForCheckingCrossingThresholds_end,
													 Time timeForEntry,
													 Time timeForExit)
			
		{
			this.pvoStrategyIntraday(eligiblesSelector, minimumNumberOfEligiblesForValidOptimization,
			                     inSampleDays , benchmark , numDaysBetweenEachOptimization ,
			                     oversoldThreshold, overboughtThreshold,
			                     double.MaxValue, double.MaxValue,
			                     historicalMarketValueProviderForInSample,
		                       historicalMarketValueProviderForOutOfSample,
		                       timeForCheckingCrossingThresholds_begin,
		                       timeForCheckingCrossingThresholds_end,
													 timeForEntry,
													 timeForExit);
			this.chosenPVOPositions = chosenPVOPositions;
		}
		
		
		#region marketOpenEventHandler
		
//		protected virtual DateTime getBeginOfOscillatingPeriod(IndexBasedEndOfDayTimer timer)
//		{
//			DateTime beginOfOscillatingPeriod =
//				HistoricalEndOfDayTimer.GetMarketClose(
//					(DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition-1]["quDate"] );
//			return beginOfOscillatingPeriod;
////			return new EndOfDayDateTime(	(DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition-1]["quDate"],
////			                            EndOfDaySpecificTime.MarketClose );
//		}
//		
//		private PVOPositionsStatus marketOpenEventHandler_openPositions_getStatus(IndexBasedEndOfDayTimer timer)
//		{
//			DateTime today = timer.GetCurrentDateTime();
//			DateTime beginOfOscillatingPeriod =
//				this.getBeginOfOscillatingPeriod(timer);
//			PVOPositionsStatus currentStatus =
//				PVOPositionsStatus.InTheMiddle;
//			for(int i = 0; i<this.chosenPVOPositions.Length; i++)
//			{
//				if(this.chosenPVOPositions[i] != null)
//					currentStatus =
//						((PVOPositions)this.chosenPVOPositions[i]).GetStatus(beginOfOscillatingPeriod, today,
//						                                                     this.benchmark.Ticker, this.historicalMarketValueProviderForInSample,
//						                                                     this.oversoldThresholdMAX, this.overboughtThresholdMAX);
//				if(currentStatus == PVOPositionsStatus.Oversold ||
//				   currentStatus == PVOPositionsStatus.Overbought )
//				{
//					this.positionsForOutOfSample = (PVOPositions)this.chosenPVOPositions[i];
//					i = this.chosenPVOPositions.Length;//exit from for
//				}
//			}
//			return currentStatus;
//		}
//		
//		protected void marketOpenEventHandler_openPositions(IndexBasedEndOfDayTimer timer)
//		{
//			PVOPositionsStatus pvoPositionsStatus = PVOPositionsStatus.InTheMiddle;
//			if(timer.CurrentDateArrayPosition >= 1)
//				pvoPositionsStatus =
//					this.marketOpenEventHandler_openPositions_getStatus(timer);
//			switch (pvoPositionsStatus)
//			{
//				case PVOPositionsStatus.Overbought:
//					{
//						#region manage Overbought case
//						this.positionsForOutOfSample.WeightedPositions.Reverse();
//						try
//						{
//							AccountManager.OpenPositions( this.positionsForOutOfSample.WeightedPositions,
//							                             this.account );
//						}
//						catch(Exception ex)
//						{
//							string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
//						}
//						finally
//						{
//							this.positionsForOutOfSample.WeightedPositions.Reverse();
//						}
//						#endregion
//						break;
//					}
//				case PVOPositionsStatus.Oversold:
//					{
//						AccountManager.OpenPositions( this.positionsForOutOfSample.WeightedPositions,
//						                             this.account );
//						break;
//					}
//				case PVOPositionsStatus.InTheMiddle://that is
//					{  //pvoPositionsForOutOfSample has not been set
//						
//						break;
//					}
//				default:
//					{
//						//it should never been reached
//						break;
//					}
//			}
//		}
//
//		protected void marketOpenEventHandler(
//			Object sender , DateTime dateTime )
//		{
//			if ( this.account.Portfolio.Count == 0 &&
//			    this.chosenPVOPositions != null &&
//			    this.optimalPositionsHaveBeenUpdated == true)
//				//portfolio is empty and optimization has
//				//been already successfully launched
//			{
//				try{
//					this.marketOpenEventHandler_openPositions( (IndexBasedEndOfDayTimer)sender );
//				}
//				catch(TickerNotExchangedException ex)
//				{
//					string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
//				}
//			}
//		}
		
		#endregion
		
		#region newDateTimeEventHandler_closePositions
		private void newDateTimeEventHandler_closePositions()
		{
			AccountManager.ClosePositions( this.account );
		}
		#endregion newDateTimeEventHandler_closePositions
		
		#region newDateTimeEventHandler_openPositions
		private void newDateTimeEventHandler_openPositions()
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
						                             	this.account );
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
					AccountManager.OpenPositions( this.positionsForOutOfSample.WeightedPositions,
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
		#endregion newDateTimeEventHandler_openPositions
			
		#region newDateTimeEventHandler_updateStatus
		private void newDateTimeEventHandler_setPositionsAndStatus()
		{
			DateTime beginOfOscillatingPeriod = 
				Time.GetDateTimeFromMerge(this.now() , this.timeForCheckingCrossingThresholds_begin);
			DateTime endOfOscillatingPeriod = 
				Time.GetDateTimeFromMerge(this.now() , this.timeForCheckingCrossingThresholds_end);
			for(int i = 0; i<this.chosenPVOPositions.Length; i++)
			{
				if(this.chosenPVOPositions[i] != null)
					this.positionsForOutOfSampleStatus =
						((PVOPositions)this.chosenPVOPositions[i]).GetStatus(beginOfOscillatingPeriod, endOfOscillatingPeriod,
						                                                     this.benchmark.Ticker, this.historicalMarketValueProviderForOutOfSample,
						                                                     this.oversoldThresholdMAX, this.overboughtThresholdMAX);
				if(this.positionsForOutOfSampleStatus == PVOPositionsStatus.Oversold ||
				   this.positionsForOutOfSampleStatus == PVOPositionsStatus.Overbought )
				{
					this.positionsForOutOfSample = (PVOPositions)this.chosenPVOPositions[i];
					i = this.chosenPVOPositions.Length;//exit from for
				}
			}
		}
		#endregion newDateTimeEventHandler_updateStatus
		
		public virtual void NewDateTimeEventHandler(
			Object sender , DateTime dateTime )
		{
			Time currentTime = new Time( dateTime );
			if( timeForCheckingCrossingThresholds_end == currentTime )
				this.newDateTimeEventHandler_setPositionsAndStatus();
			else if( timeForEntry == currentTime )
				this.newDateTimeEventHandler_openPositions();
			else if( timeForExit == currentTime )
				this.newDateTimeEventHandler_closePositions();
			else if( currentTime > timeForExit )
				this.newDateTimeEventHandler_updateTestingPositions( dateTime );
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
			this.updateReturnsManager(history.FirstDateTime,
			                          history.LastDateTime);
			if( (  this.eligiblesSelector is DummyEligibleSelector &&
			     this.inSampleChooser != null )    ||
			   (  eligibles.Count > this.minimumNumberOfEligiblesForValidOptimization &&
			    this.inSampleChooser != null )  )
			{
				this.chosenPVOPositions = (TestingPositions[])inSampleChooser.AnalyzeInSample(eligibles, this.returnsManager);
				this.updateTestingPositions_updateThresholds();
				this.logOptimizationInfo(eligibles);
//				this.optimalPositionsHaveBeenUpdated = true;
			}
//			else
//			{
//				this.optimalPositionsHaveBeenUpdated = false;
//			}
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
			this.lastTimerDateTimeAfterLastExit = dateTime;
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
				this.updateTestingPositions( dateTime );
				//sets tickers to be chosen next time
				this.numDaysElapsedSinceLastOptimization = 0;
				this.lastOptimizationDateTime = this.now();
			}
		}
		#endregion UpdateTestingPositions
		
		private DateTime now()
		{
			return this.account.Timer.GetCurrentDateTime();
		}
		
		protected virtual void updateReturnsManager(DateTime firstDateTime,
		                                            DateTime lastDayDateTime)
		{
			ReturnIntervals returnIntervals =
				new DailyOpenToCloseIntervals( firstDateTime, lastDayDateTime,
				                              this.benchmark.Ticker );
			if( this.inSampleChooser is PVOCorrelationChooser )
			{
				switch ( ((PVOCorrelationChooser)this.inSampleChooser).IntervalsType )
				{
					case IntervalsType.CloseToCloseIntervals:
						returnIntervals = new CloseToCloseIntervals(firstDateTime, lastDayDateTime,
						                                            this.benchmark.Ticker, ((PVOCorrelationChooser)this.inSampleChooser).ReturnIntervalLength);
						break;
					case IntervalsType.OpenToOpenIntervals:
						returnIntervals = new OpenToOpenIntervals(firstDateTime, lastDayDateTime,
						                                          this.benchmark.Ticker, ((PVOCorrelationChooser)this.inSampleChooser).ReturnIntervalLength);
						break;
					case IntervalsType.CloseToOpenIntervals:
						returnIntervals = new CloseToOpenIntervals(firstDateTime, lastDayDateTime,
						                                           this.benchmark.Ticker);
						break;
					case IntervalsType.OpenToCloseIntervals:
						returnIntervals = new DailyOpenToCloseIntervals(firstDateTime, lastDayDateTime,
						                                                this.benchmark.Ticker );
						break;
					case IntervalsType.OpenToCloseCloseToOpenIntervals:
						returnIntervals = new OpenToCloseCloseToOpenIntervals(
							firstDateTime, lastDayDateTime, this.benchmark.Ticker);
						break;
					default:
						// it should never be reached
						returnIntervals = new DailyOpenToCloseIntervals(firstDateTime, lastDayDateTime,
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
			string message = "Number of Eligible tickers: " +
				eligibleTickers.Count;
			NewMessageEventArgs newMessageEventArgs =
				new NewMessageEventArgs( message );
			if ( this.NewMessage != null )
				this.NewMessage( this , newMessageEventArgs );
		}
		private void logOptimizationInfo( EligibleTickers eligibleTickers )
		{
			if(eligibleTickers.Count > 0)
				this.raiseNewLogItem( eligibleTickers );
			
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
