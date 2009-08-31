/*
QuantProject - Quantitative Finance Library

OTCIntradayStrategy.cs
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
using System.IO;

using QuantProject.ADT;
using QuantProject.ADT.Statistics;
using QuantProject.ADT.Histories;
using QuantProject.ADT.Messaging;
using QuantProject.ADT.Timing;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Business.Strategies.InSample.InSampleFitnessDistributionEstimation;
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
using QuantProject.Scripts.TickerSelectionTesting.OTC;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.WalkForwardTesting.LinearCombination;

namespace QuantProject.Scripts.TickerSelectionTesting.OTC.OTC_Intraday
{
	/// <summary>
	/// Implements the open to close strategy (buy a portfolio at open
	/// and sell it at close) next to opening and closing time,
	/// using intraday bars
	/// </summary>
	[Serializable]
	public class OTCIntradayStrategy : IStrategyForBacktester
	{
		public event NewLogItemEventHandler NewLogItem;
		public event NewMessageEventHandler NewMessage;

		//initialized by the constructor
		protected int inSampleDays;
		protected int numDaysBetweenEachOptimization;
		protected int numDaysBeforeCurrentDateForRetrievingInSampleData;
		protected IInSampleChooser inSampleChooser;
		protected GeneticChooser geneticChooserForFitnessDistributionEstimator;
		protected double numberOfMinStdDeviationForOpeningPositions;
		protected double numberOfMaxStdDeviationForOpeningPositions;
		protected IEligiblesSelector eligiblesSelector;
		protected IInSampleFitnessDistributionEstimator estimator;
		protected int sampleLength;
		protected Benchmark benchmark;
		protected HistoricalMarketValueProvider 
			historicalMarketValueProviderForInSample;
		protected HistoricalMarketValueProvider
			historicalMarketValueProviderForOutOfSample;
		//initialized after constructor's call
		protected int numDaysElapsedSinceLastOptimization;
		protected ReturnsManager returnsManager;
		protected TestingPositions[] chosenOTCPositions;
		//chosen in sample by the chooser or passed
		//directly by the user using a form:
		//these are the positions to test out of sample
		protected PortfolioType portfolioType;
		protected DateTime lastOptimizationDateTime;
		protected Account account;
		public Account Account
		{
			get { return this.account; }
			set { this.account = value; }
		}
		
		private int minimumNumberOfEligiblesForValidOptimization;
//		private bool optimalPositionsHaveBeenUpdated;
		protected int idxForBestPositionsCompatibleWithPortfolioType;
		protected EligibleTickers currentEligibles;
		protected bool stopLossConditionReached;
		protected bool takeProfitConditionReached;
		protected double maxOpeningLengthInMinutes;
		protected Time lastEntryTime;
		protected Time lastProfitOrLossTime;
		protected List<Time> openingTimesForAvailableBars;
		protected Time nearToOpeningTimeFrom;
    protected Time nearToOpeningTimeTo;
    protected Time nearToClosingTimeFrom;
    protected Time nearToClosingTimeTo;
		protected double currentAccountValue;
		protected double previousAccountValue;
		protected double stopLoss;
		protected double takeProfit;
				
		private string description_GetDescriptionForChooser()
		{
			if(this.inSampleChooser == null)
				return "NoChooserDefined";
			else
				return this.inSampleChooser.Description;
		}
		
		private Time getLastEventTimeWithCachedBars()
		{
			return this.openingTimesForAvailableBars[openingTimesForAvailableBars.Count - 1];
		}
		private Time getFirstEventTimeWithCachedBars()
		{
			return this.openingTimesForAvailableBars[0];
		}
		
		public string Description
		{
			get
			{
				string description =
					"OTC_Intraday";
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

		private void otcIntradayStrategy_commonInitialization(IEligiblesSelector eligiblesSelector,
		                       int minimumNumberOfEligiblesForValidOptimization,
		                       int inSampleDays,
		                       Benchmark benchmark,
		                       int numDaysBetweenEachOptimization,
		                       int numDaysBeforeCurrentDateForRetrievingInSampleData,
		                       HistoricalMarketValueProvider historicalMarketValueProviderForInSample,
		                       HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample,
		                       List<Time> openingTimesForAvailableBars,
		                       Time nearToOpeningTimeFrom,
		                       Time nearToOpeningTimeTo,
		                       Time nearToClosingTimeFrom,
		                       Time nearToClosingTimeTo,
													 double stopLoss, double takeProfit,
													 PortfolioType portfolioType,
													 GeneticChooser geneticChooserForFitnessDistributionEstimator,
													 double numberOfMinimumStdDeviationForOpeningPositions,
													 double numberOfMaxStdDeviationForOpeningPositions,
													 IInSampleFitnessDistributionEstimator estimator, 
													 int sampleLength)
		{
			this.eligiblesSelector = eligiblesSelector;
			this.minimumNumberOfEligiblesForValidOptimization = minimumNumberOfEligiblesForValidOptimization;
			this.inSampleDays = inSampleDays;
			this.benchmark = benchmark;
			this.idxForBestPositionsCompatibleWithPortfolioType = 0;
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
			this.numDaysBeforeCurrentDateForRetrievingInSampleData = 
				numDaysBeforeCurrentDateForRetrievingInSampleData;
			this.historicalMarketValueProviderForInSample =
				historicalMarketValueProviderForInSample;
			this.historicalMarketValueProviderForOutOfSample = 
				historicalMarketValueProviderForOutOfSample;
			this.openingTimesForAvailableBars =
				openingTimesForAvailableBars;
			this.nearToOpeningTimeFrom = nearToOpeningTimeFrom;
			this.nearToOpeningTimeTo = nearToOpeningTimeTo;
			this.nearToClosingTimeFrom = nearToClosingTimeFrom;
			this.nearToClosingTimeTo = nearToClosingTimeTo;
			this.stopLoss = stopLoss;
			this.takeProfit = takeProfit;
			this.portfolioType = portfolioType;
			this.geneticChooserForFitnessDistributionEstimator = geneticChooserForFitnessDistributionEstimator;
			this.numberOfMinStdDeviationForOpeningPositions = numberOfMinimumStdDeviationForOpeningPositions;
			this.numberOfMaxStdDeviationForOpeningPositions = numberOfMaxStdDeviationForOpeningPositions;
			this.estimator = estimator;
			this.sampleLength = sampleLength;
		}
		
		public OTCIntradayStrategy(IEligiblesSelector eligiblesSelector,
		                       int minimumNumberOfEligiblesForValidOptimization,
		                       IInSampleChooser inSampleChooser,
		                       int inSampleDays,
		                       Benchmark benchmark,
		                       int numDaysBetweenEachOptimization,
		                       int numDaysBeforeCurrentDateForRetrievingInSampleData,
		                       HistoricalMarketValueProvider historicalMarketValueProviderForInSample,
		                       HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample,
		                       List<Time> openingTimesForAvailableBars,
		                       Time nearToOpeningTimeFrom,
		                       Time nearToOpeningTimeTo,
		                       Time nearToClosingTimeFrom,
		                       Time nearToClosingTimeTo,
													 double stopLoss, double takeProfit,
													 PortfolioType portfolioType,
													 GeneticChooser geneticChooserForFitnessDistributionEstimator,
													 double numberOfMinimumStdDeviationForOpeningPositions,
													 double numberOfMaxStdDeviationForOpeningPositions,
													 IInSampleFitnessDistributionEstimator estimator,
													 int sampleLength)
			
		{
			this.inSampleChooser = inSampleChooser;
			this.otcIntradayStrategy_commonInitialization(eligiblesSelector,
			                                              minimumNumberOfEligiblesForValidOptimization,
			                                              inSampleDays, benchmark,
															                      numDaysBetweenEachOptimization,
															                      numDaysBeforeCurrentDateForRetrievingInSampleData,
															                      historicalMarketValueProviderForInSample,
															                      historicalMarketValueProviderForOutOfSample,
															                      openingTimesForAvailableBars,
															                      nearToOpeningTimeFrom,
															                      nearToOpeningTimeTo,
															                      nearToClosingTimeFrom,
															                      nearToClosingTimeTo,
															                      stopLoss, takeProfit,
													 													portfolioType,
													 													geneticChooserForFitnessDistributionEstimator,
																									  numberOfMinimumStdDeviationForOpeningPositions,
																									  numberOfMaxStdDeviationForOpeningPositions,
																									  estimator, sampleLength);
		}
		
		public OTCIntradayStrategy(IEligiblesSelector eligiblesSelector,
		                       TestingPositions[] chosenOTCPositions,
		                       Benchmark benchmark,
		                       HistoricalMarketValueProvider historicalMarketValueProviderForInSample,
		                       HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample,
		                       List<Time> openingTimesForAvailableBars,
													 double stopLoss, double takeProfit,
													 PortfolioType portfolioType,
													 GeneticChooser geneticChooserForFitnessDistributionEstimator,
													 double numberOfMinimumStdDeviationForOpeningPositions,
													 double numberOfMaxStdDeviationForOpeningPositions,
													 IInSampleFitnessDistributionEstimator estimator,
													 int sampleLength)
			
		{
			this.chosenOTCPositions = chosenOTCPositions;
			this.otcIntradayStrategy_commonInitialization(eligiblesSelector,
			                                              5, 5, benchmark,
															                      numDaysBetweenEachOptimization,
															                      0,
															                      historicalMarketValueProviderForInSample,
															                      historicalMarketValueProviderForOutOfSample,
															                      openingTimesForAvailableBars,
															                      nearToOpeningTimeFrom,
															                      nearToOpeningTimeTo,
															                      nearToClosingTimeFrom,
															                      nearToClosingTimeTo,
															                      stopLoss, takeProfit,
													 													portfolioType,
													 													geneticChooserForFitnessDistributionEstimator,
																									  numberOfMinimumStdDeviationForOpeningPositions,
																									  numberOfMaxStdDeviationForOpeningPositions,
																									  estimator, sampleLength);
		}		
		private bool allTickersAreExchanged(DateTime dateTime,
		                                    string[] tickers)
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
		private bool allTickersAreExchangedInTheLastFiveMinutelyBars(DateTime fromDateTime,
		                                                           	 string[] tickers)
		{
			bool returnValue = true;
			Time lastFiveMinutelyBarTime = 
				this.openingTimesForAvailableBars[this.openingTimesForAvailableBars.Count-1].AddMinutes(-5);
			try{
				for( int i = 0; i < tickers.Length; i++ )
				{
					Bars currentTickerBars =
						new Bars(tickers[i], Time.GetDateTimeFromMerge(fromDateTime, lastFiveMinutelyBarTime),
						         Time.GetDateTimeFromMerge(fromDateTime,
						                                   this.openingTimesForAvailableBars[this.openingTimesForAvailableBars.Count-1]),
						         60);
					if( currentTickerBars.Rows.Count < 1 )
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
				
		private void newDateTimeEventHandler_closePositions(Time currentDailyTime)
		{
			if( allTickersAreExchanged( this.now(), AccountManager.GetTickersInOpenedPositions(this.account) ) )
			{
		  	AccountManager.ClosePositions( this.account );
		  	this.lastEntryTime = new Time("00:00:00");
			}
		}
		#endregion newDateTimeEventHandler_closePositions
		
		#region newDateTimeEventHandler_openPositions
		private bool newDateTimeEventHandler_openPositions_bestFitnessIsSignificantlyHigh()
		{
			bool returnValue = false;
			double average, stdDev;
			if( this.geneticChooserForFitnessDistributionEstimator != null &&
			    this.estimator != null )
			{
				average =
					estimator.GetAverage(this.geneticChooserForFitnessDistributionEstimator,
				                     this.currentEligibles, this.returnsManager, 100);
			  stdDev = 
					Math.Sqrt(estimator.GetVariance(this.geneticChooserForFitnessDistributionEstimator,
				                               this.currentEligibles, this.returnsManager, 100));
				double bestFitness = 
					this.chosenOTCPositions[idxForBestPositionsCompatibleWithPortfolioType].FitnessInSample;
			  double minNumOfStdDev = this.numberOfMinStdDeviationForOpeningPositions;
			  double maxNumOfStdDev = this.numberOfMaxStdDeviationForOpeningPositions;
			  returnValue =  bestFitness >= (average +  minNumOfStdDev * stdDev) &&
			  	             bestFitness <= (average +  maxNumOfStdDev * stdDev);
			}
			return returnValue;
		}
		private void newDateTimeEventHandler_openPositions(Time currentDailyTime)
		{
			if(	this.chosenOTCPositions != null &&
			    this.allTickersAreExchanged( this.now(), this.chosenOTCPositions[idxForBestPositionsCompatibleWithPortfolioType].WeightedPositions.SignedTickers.Tickers) 
			    &&
			    this.newDateTimeEventHandler_openPositions_bestFitnessIsSignificantlyHigh() )
//			   &&
//			   this.allTickersAreExchangedInTheLastFiveMinutelyBars( this.now(), this.chosenOTCPositions[idxForBestPositionsCompatibleWithPortfolioType].WeightedPositions.SignedTickers.Tickers ) 
//			  )
			{
				try
				{
					AccountManager.OpenPositions( this.chosenOTCPositions[idxForBestPositionsCompatibleWithPortfolioType].WeightedPositions,
					                             	this.account );
					this.lastEntryTime = currentDailyTime;
					this.previousAccountValue = this.account.GetMarketValue();
				}
				catch(Exception ex)
				{
					string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
				}
			}
		}
		#endregion newDateTimeEventHandler_openPositions
						
		#region newDateTimeEventHandler_updateStopLossAndTakeProfitConditions
		protected virtual void newDateTimeEventHandler_updateStopLossAndTakeProfitConditions(Time currentDailyTime)
		{
			//this.previousAccountValue has been set at opening positions
			this.stopLossConditionReached = false;
			this.takeProfitConditionReached = false;
			this.lastProfitOrLossTime = new Time("00:00:00");
			if(this.account.Portfolio.Count > 0)
			{
				try{
					this.currentAccountValue = this.account.GetMarketValue();
					double portfolioGainOrLoss = (this.currentAccountValue - this.previousAccountValue)
						/this.previousAccountValue;
					
					if(!double.IsInfinity(portfolioGainOrLoss) &&
					   portfolioGainOrLoss <= -this.stopLoss )
					{
						this.stopLossConditionReached = true;
						this.lastProfitOrLossTime = currentDailyTime;
					}
					else if (!double.IsInfinity(portfolioGainOrLoss) &&
					         portfolioGainOrLoss >= this.takeProfit )
						
					{
						this.takeProfitConditionReached = true;
						this.lastProfitOrLossTime = currentDailyTime;
					}
				}
				catch(Exception ex)
					{string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";}
			}
		}
		
		#endregion newDateTimeEventHandler_updateStopLossAndTakeProfitConditions
		
		public virtual void NewDateTimeEventHandler(
			Object sender , DateTime dateTime )
		{
			Time currentTime = new Time( dateTime );
			bool nearToOpeningTime = currentTime >= this.nearToOpeningTimeFrom && 
			    										 currentTime <= this.nearToOpeningTimeTo;
			bool nearToClosingTime = currentTime >= this.nearToClosingTimeFrom && 
			    										 currentTime <= this.nearToClosingTimeTo;
			this.newDateTimeEventHandler_updateStopLossAndTakeProfitConditions(currentTime);
			bool timeToProfitOrToStopLoss = this.takeProfitConditionReached ||
																	this.stopLossConditionReached;
			if( this.account.Portfolio.Count == 0 && nearToOpeningTime )
				this.newDateTimeEventHandler_openPositions(currentTime);
			
			if( this.account.Portfolio.Count > 0 && 
			    ( nearToClosingTime || timeToProfitOrToStopLoss ) )
				this.newDateTimeEventHandler_closePositions(currentTime);
			
			if( currentTime == getLastEventTimeWithCachedBars() )
			//it's time for new optimization, if the case
				this.newDateTimeEventHandler_updateTestingPositions( dateTime );
		}
		
		#region FindPositionsForToday
//		private double[] findPositionsForToday_writePositionsToLogFile_getFitnesses(TestingPositions[] positionsToWrite)
//		{
//			int numOfValidFitnesses = 0;
//			double[] validFitnesses;
//			for(int i=0; i<positionsToWrite.Length; i++)
//				if( positionsToWrite[i] != null &&
//				   	!double.IsInfinity(positionsToWrite[i].FitnessInSample) &&
//				    !double.IsNaN(positionsToWrite[i].FitnessInSample) &&
//				    positionsToWrite[i].FitnessInSample != double.MinValue )
//					numOfValidFitnesses++;
//			
//			validFitnesses = new double[numOfValidFitnesses];
//			int addedValidFitnesses = 0;
//			for(int i=0; i<positionsToWrite.Length; i++)
//				if( positionsToWrite[i] != null &&
//				    !double.IsInfinity(positionsToWrite[i].FitnessInSample) &&
//				    !double.IsNaN(positionsToWrite[i].FitnessInSample) &&
//				    positionsToWrite[i].FitnessInSample != double.MinValue )
//				{
//					validFitnesses[addedValidFitnesses] = positionsToWrite[i].FitnessInSample;
//					addedValidFitnesses++;
//				}
//			return validFitnesses;
//		}
		
		private void findPositionsForToday_writePositionsToLogFile(DateTime today, EligibleTickers eligibles,
		                                                           TestingPositions[] positionsToWrite)
		{
			double averageRandomFitness =
				estimator.GetAverage(this.geneticChooserForFitnessDistributionEstimator,
				                     this.currentEligibles, this.returnsManager, this.sampleLength);
			double stdDevForRandomFitness = 
				Math.Sqrt(estimator.GetVariance(this.geneticChooserForFitnessDistributionEstimator,
				                               this.currentEligibles, this.returnsManager, this.sampleLength));
			string pathFile =
				System.Configuration.ConfigurationManager.AppSettings["LogArchive"] +
				"\\PositionsForOTCStrategyOn" + today.Day.ToString() + "_" +
				today.Month.ToString() + "_" + today.Year.ToString() + ".txt";
			StreamWriter w = File.AppendText(pathFile);
			w.WriteLine ("\n----------------------------------------------\r\n");
			w.Write("\r\nPositions for OTC Strategy on date: {0}\r", today.ToLongDateString() );
			w.Write("\r\nNum days for optimization {0}\r", this.inSampleDays.ToString());
			w.Write("\r\nEligibles: {0}\r", eligibles.Count.ToString() );
			w.WriteLine ("\n----------------------------------------------");
			//
			for(int i = 0; i<positionsToWrite.Length; i++)
				if(positionsToWrite[i] != null && positionsToWrite[i].FitnessInSample != double.MinValue)
					w.WriteLine("\n{0}-Positions: {1} --> fitness {2}", i.ToString(), 
				            	positionsToWrite[i].WeightedPositions.Description,
				            	positionsToWrite[i].FitnessInSample.ToString() );
			w.WriteLine ("\n\n----------------------------------------------");
			w.WriteLine ("\n\nBest testing positions is {0}", this.idxForBestPositionsCompatibleWithPortfolioType.ToString());
			w.Write("\r\nAverage random fitness (sample: {0}): {1}\r",
			        this.sampleLength.ToString(), averageRandomFitness.ToString() );
			w.Write("\r\nStandard deviation for random fitness: {0}\r",
			        stdDevForRandomFitness.ToString());
			w.Write("\r\nAverage + min num of std deviation ({0}) is: {1}\r", this.numberOfMinStdDeviationForOpeningPositions.ToString() ,
			        (averageRandomFitness + this.numberOfMinStdDeviationForOpeningPositions * stdDevForRandomFitness).ToString());
						w.Write("\r\nAverage + max num of std deviation ({0}) is: {1}\r", this.numberOfMaxStdDeviationForOpeningPositions.ToString() ,
			        (averageRandomFitness + this.numberOfMaxStdDeviationForOpeningPositions * stdDevForRandomFitness).ToString());

			// Update the underlying file.
			w.Flush();
			w.Close();
		}
		
		public void FindPositionsForToday(DateTime today, DateTime lastMarketDay)
		{
			History history =
				this.benchmark.GetEndOfDayHistory(
					HistoricalEndOfDayTimer.GetMarketOpen(
						lastMarketDay.AddDays( -this.inSampleDays ) ) ,
					HistoricalEndOfDayTimer.GetMarketClose(
						lastMarketDay ) );
			this.currentEligibles =
				this.eligiblesSelector.GetEligibleTickers(history);
			this.updateReturnsManager(history.FirstDateTime,
			                          history.LastDateTime);
			if( (  this.eligiblesSelector is DummyEligibleSelector &&
			     this.inSampleChooser != null )    ||
			   (  this.currentEligibles.Count > this.minimumNumberOfEligiblesForValidOptimization &&
			    this.inSampleChooser != null )  )
			{	
				this.chosenOTCPositions = (TestingPositions[])inSampleChooser.AnalyzeInSample(this.currentEligibles, this.returnsManager);
				this.updateTestingPositions_updateIdxForBestPositionsCompatibleWithPortfolioType();
				this.findPositionsForToday_writePositionsToLogFile(today, this.currentEligibles,
					 																								 this.chosenOTCPositions);
				this.chosenOTCPositions = null;
			}	
		}
		#endregion FindPositionsForToday
		
		#region UpdateTestingPositions
		private void updateTestingPositions_updateIdxForBestPositionsCompatibleWithPortfolioType()
		{
			for(int i = 0; i<this.chosenOTCPositions.Length; i++)
			{
				if(this.chosenOTCPositions[i] != null &&
				   ( (this.chosenOTCPositions[i].BothLongAndShortPositions &&
				      (this.portfolioType == PortfolioType.ShortAndLong || this.portfolioType == PortfolioType.OnlyMixed) ) ||
				      (this.chosenOTCPositions[i].OnlyLongPositions && this.portfolioType == PortfolioType.OnlyLong) ||
				      (this.chosenOTCPositions[i].OnlyShortPositions && this.portfolioType == PortfolioType.OnlyShort) ) )
				{
					this.idxForBestPositionsCompatibleWithPortfolioType = i;
					i = this.chosenOTCPositions.Length;//exit from for
				}
			}
		}
		
		protected void updateTestingPositions(DateTime currentDateTime)
		{
			History history =
				this.benchmark.GetEndOfDayHistory(
					HistoricalEndOfDayTimer.GetMarketOpen(
						currentDateTime.AddDays( -this.inSampleDays ) ) ,
					HistoricalEndOfDayTimer.GetMarketClose(
						currentDateTime.AddDays(-this.numDaysBeforeCurrentDateForRetrievingInSampleData) ) );
			this.currentEligibles =
				this.eligiblesSelector.GetEligibleTickers(history);
			this.updateReturnsManager(history.FirstDateTime,
			                          history.LastDateTime);
			if( (  this.eligiblesSelector is DummyEligibleSelector &&
			     this.inSampleChooser != null )    ||
			   (  this.currentEligibles.Count > this.minimumNumberOfEligiblesForValidOptimization &&
			    this.inSampleChooser != null )  )
			{
				this.chosenOTCPositions = (TestingPositions[])inSampleChooser.AnalyzeInSample(this.currentEligibles, this.returnsManager);
				this.updateTestingPositions_updateIdxForBestPositionsCompatibleWithPortfolioType();
				this.logOptimizationInfo(this.currentEligibles);
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
//				new DailyOpenToCloseIntervals( firstDateTime, lastDayDateTime,
//				                              this.benchmark.Ticker );
				new OpenToCloseCloseToOpenIntervals( firstDateTime, lastDayDateTime,
				                              			 this.benchmark.Ticker );
			
			this.returnsManager =
				new ReturnsManager( returnIntervals , this.historicalMarketValueProviderForInSample);
		}
				
		private OTCIntradayLogItem getLogItem( EligibleTickers eligibleTickers )
		{
			OTCIntradayLogItem logItem =
				new OTCIntradayLogItem(this.now(), this.inSampleDays);
			logItem.BestOTCPositionsInSample =
				this.chosenOTCPositions;
			logItem.NumberOfEligibleTickers =
				eligibleTickers.Count;
			logItem.Fitness =
				this.chosenOTCPositions[idxForBestPositionsCompatibleWithPortfolioType].FitnessInSample;
			logItem.Generation = ((IGeneticallyOptimizable)this.chosenOTCPositions[idxForBestPositionsCompatibleWithPortfolioType]).Generation;
			logItem.Tickers =
				this.chosenOTCPositions[idxForBestPositionsCompatibleWithPortfolioType].HashCodeForTickerComposition;
			
			return logItem;
		}
		
		private void raiseNewLogItem( EligibleTickers eligibleTickers )
		{
			OTCIntradayLogItem logItem =
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
	}
}
