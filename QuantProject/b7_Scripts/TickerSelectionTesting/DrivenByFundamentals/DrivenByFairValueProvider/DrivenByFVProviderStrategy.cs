/*
QuantProject - Quantitative Finance Library

DrivenByFVProviderStrategy.cs
Copyright (C) 2010
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
//using QuantProject.Business.Strategies.ReturnsManagement.Time.IntervalsSelectors
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

namespace QuantProject.Scripts.TickerSelectionTesting.DrivenByFundamentals.DrivenByFairValueProvider
{
	/// <summary>
	/// Implements a strategy driven by a fair value provider
	/// which uses the available fundamentals
	/// </summary>
	[Serializable]
	public class DrivenByFVProviderStrategy : IStrategyForBacktester
	{
		public event NewLogItemEventHandler NewLogItem;
		public event NewMessageEventHandler NewMessage;

		protected int numDaysForFundamentalAnalysis;
		protected int inSamplePeriodLengthInDays;
		protected int numDaysBetweenEachOptimization;
		protected IInSampleChooser inSampleChooser;
		protected IEligiblesSelector eligiblesSelector;
		protected Benchmark benchmark;
		protected HistoricalMarketValueProvider 
			historicalMarketValueProviderForInSample;
		protected HistoricalMarketValueProvider
			historicalMarketValueProviderForOutOfSample;
		//initialized after constructor's call
		protected int numDaysElapsedSinceLastOptimization;
		protected ReturnsManager returnsManager;
		protected TestingPositions[] chosenPositions;
		//chosen in sample by the chooser or passed
		//directly by the user using a form:
		//these are the positions to test out of sample
		protected PortfolioType portfolioType;
		protected DateTime lastEntryTime;
		protected DateTime lastOptimizationDateTime;
		protected Account account;
		public Account Account
		{
			get { return this.account; }
			set { this.account = value; }
		}
		
		private int minimumNumberOfEligiblesForValidOptimization;
//		private bool optimalPositionsHaveBeenUpdated;
		protected EligibleTickers currentEligibles;
		protected bool stopLossConditionReached;
		protected bool takeProfitConditionReached;
		protected double currentAccountValue;
		protected double previousAccountValue;
		protected double stopLoss;
		protected double percentageOfTheoreticalProfit;
		protected double theoreticalProfit;
		//portfolio's profit if each position reaches its theoretical price
		protected double takeProfitLevelInAnyCase;
		//if current profit of the portfolio crosses this level,
		//the portfolio is sold, no matter what the current value
		//of percentageOfTheoreticalProfit is
		
		private string description_GetDescriptionForChooser()
		{
			if(this.inSampleChooser == null)
				return "NoChooserDefined";
			else
				return this.inSampleChooser.Description;
		}
	
		public string Description
		{
			get
			{
				string description =
					"DrivenByFairValueProvider";
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
		
		public DrivenByFVProviderStrategy(IEligiblesSelector eligiblesSelector ,
				int minNumOfEligiblesForValidOptimization, IInSampleChooser inSampleChooser ,
				int numDaysForFundamentalAnalysis , int numDaysForPortfolioVolatilityAnalysis ,
				Benchmark benchmark , int numDaysBetweenEachOptimization ,
				HistoricalMarketValueProvider historicalMarketValueProviderForInSample ,
			  HistoricalMarketValueProvider historicalMarketValueProviderForOutOfSample ,
			  PortfolioType portfolioType,  double stopLoss, double percentageOfTheoreticalProfit,
			  double takeProfitLevelInAnyCase)
		{
			this.eligiblesSelector = eligiblesSelector;
			this.minimumNumberOfEligiblesForValidOptimization = minNumOfEligiblesForValidOptimization;
			this.inSampleChooser = inSampleChooser;
			this.numDaysForFundamentalAnalysis = numDaysForFundamentalAnalysis;
			this.inSamplePeriodLengthInDays = numDaysForPortfolioVolatilityAnalysis;
			this.benchmark = benchmark;
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
			this.historicalMarketValueProviderForInSample = historicalMarketValueProviderForInSample;
			this.historicalMarketValueProviderForOutOfSample = historicalMarketValueProviderForOutOfSample;
			this.portfolioType = portfolioType;
			this.stopLoss = stopLoss;
			this.percentageOfTheoreticalProfit = percentageOfTheoreticalProfit;
			this.takeProfitLevelInAnyCase = takeProfitLevelInAnyCase;
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
		
		#region newDateTimeEventHandler_closePositions
				
		private void newDateTimeEventHandler_closePositions()
		{
			DateTime currentDateTime = this.now();
			if( allTickersAreExchanged( currentDateTime, AccountManager.GetTickersInOpenedPositions(this.account) ) )
			{
		  	AccountManager.ClosePositions( this.account );
		  	this.lastEntryTime = new DateTime(1900,1,1);
			}
		}
		#endregion newDateTimeEventHandler_closePositions
		
		#region newDateTimeEventHandler_openPositions
	
		private void newDateTimeEventHandler_openPositions()
		{
			if(	this.chosenPositions != null &&
			    this.allTickersAreExchanged( this.now(), this.chosenPositions[0].WeightedPositions.SignedTickers.Tickers) 
			   )
			{
				try
				{
					AccountManager.OpenPositions( this.chosenPositions[0].WeightedPositions,
					                             	this.account );
					this.lastEntryTime = this.now();
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
		protected virtual void newDateTimeEventHandler_updateStopLossAndTakeProfitConditions()
		{
			//this.previousAccountValue has been set at opening positions
			this.stopLossConditionReached = false;
			this.takeProfitConditionReached = false;
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
					}
					else if (!double.IsInfinity(portfolioGainOrLoss) && (
//					          portfolioGainOrLoss >= (this.percentageOfTheoreticalProfit * this.theoreticalProfit) ||
					          portfolioGainOrLoss >= this.takeProfitLevelInAnyCase) )
						
					{
						this.takeProfitConditionReached = true;
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
			this.newDateTimeEventHandler_updateStopLossAndTakeProfitConditions();
			bool timeToProfitOrToStopLoss = this.takeProfitConditionReached ||
																	this.stopLossConditionReached;
			
			if( this.account.Portfolio.Count == 0 )
				this.newDateTimeEventHandler_openPositions();
			
			if( (this.account.Portfolio.Count > 0 && timeToProfitOrToStopLoss) ||
			    this.optimalTestingPositionsAreToBeUpdated() )
				this.newDateTimeEventHandler_closePositions();
		
			this.newDateTimeEventHandler_updateTestingPositions( dateTime );
		}
				
		private void findPositionsForToday_writePositionsToLogFile(DateTime today, EligibleTickers eligibles,
		                                                           TestingPositions[] positionsToWrite)
		{
			string pathFile =
				System.Configuration.ConfigurationManager.AppSettings["LogArchive"] +
				"\\PositionsForDrivenByFVProviderStrategy" + today.Day.ToString() + "_" +
				today.Month.ToString() + "_" + today.Year.ToString() + ".txt";
			StreamWriter w = File.AppendText(pathFile);
			w.WriteLine ("\n----------------------------------------------\r\n");
			w.Write("\r\nPositions for PositionsForDrivenByFVProviderStrategy on date: {0}\r", today.ToLongDateString() );
			w.Write("\r\nNum days for fundamental analysis {0}\r", this.numDaysForFundamentalAnalysis.ToString());
			w.Write("\r\nNum days for portofolio analysis for volatility {0}\r", this.inSamplePeriodLengthInDays.ToString());
			w.Write("\r\nEligibles: {0}\r", eligibles.Count.ToString() );
			w.WriteLine ("\n----------------------------------------------");
			//
			for(int i = 0; i<positionsToWrite.Length; i++)
				if(positionsToWrite[i] != null && positionsToWrite[i].FitnessInSample != double.MinValue)
					w.WriteLine("\n{0}-Positions: {1} --> fitness {2}", i.ToString(), 
				            	positionsToWrite[i].WeightedPositions.Description,
				            	positionsToWrite[i].FitnessInSample.ToString() );
			// Update the underlying file.
			w.Flush();
			w.Close();
		}
		
		#region UpdateTestingPositions
		private double updateTheoreticalProfit_getBuyPrice(WeightedPosition position)
		{
			double returnValue;
			returnValue = 
				this.historicalMarketValueProviderForOutOfSample.GetMarketValue(position.Ticker,
				                                                                this.now() );
			return returnValue;
		}
		
		private double updateTheoreticalProfit_getFairPrice(WeightedPosition position)
		{
			double returnValue;
			object[] keys = new object[1];
			keys[0] = position.Ticker;
			DataRow foundRow = 
				this.currentEligibles.SourceDataTable.Rows.Find(keys);
			returnValue = (double)foundRow["FairPrice"];
			
			return returnValue;
		}
		
		private void updateTheoreticalProfit()
		{
			double buyPriceOfCurrentPosition;
			double fairPriceOfCurrentPosition;
			double weightOfCurrentPosition;
			foreach( WeightedPosition position in this.chosenPositions[0].WeightedPositions )
			{
				buyPriceOfCurrentPosition = this.updateTheoreticalProfit_getBuyPrice(position);
				fairPriceOfCurrentPosition = this.updateTheoreticalProfit_getFairPrice(position);
				weightOfCurrentPosition = position.Weight;
				this.theoreticalProfit +=
					weightOfCurrentPosition*(fairPriceOfCurrentPosition-buyPriceOfCurrentPosition)/
					buyPriceOfCurrentPosition;
			}
		}
		
		protected void updateTestingPositions(DateTime currentDateTime)
		{
			History historyForEligiblesSelector =
				this.benchmark.GetEndOfDayHistory(
					HistoricalEndOfDayTimer.GetMarketClose(
						currentDateTime.AddDays( -this.numDaysForFundamentalAnalysis ) ) ,
					HistoricalEndOfDayTimer.GetMarketClose(
						currentDateTime.AddDays(-1) ) );
			
			History historyForReturnsManager =
				this.benchmark.GetEndOfDayHistory(
					HistoricalEndOfDayTimer.GetMarketClose(
						currentDateTime.AddDays( -this.inSamplePeriodLengthInDays ) ) ,
					HistoricalEndOfDayTimer.GetMarketClose(
						currentDateTime.AddDays(-1) ) );
			
			this.currentEligibles =
				this.eligiblesSelector.GetEligibleTickers(historyForEligiblesSelector);
			
			this.updateReturnsManager(historyForReturnsManager.FirstDateTime,
			                          historyForReturnsManager.LastDateTime);
			
			if( (  this.eligiblesSelector is DummyEligibleSelector &&
			     this.inSampleChooser != null )    ||
			   (  this.currentEligibles.Count > this.minimumNumberOfEligiblesForValidOptimization &&
			    this.inSampleChooser != null )  )
			{
				this.chosenPositions = (TestingPositions[])inSampleChooser.AnalyzeInSample(this.currentEligibles, this.returnsManager);
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
				this.chosenPositions = null;
				this.updateTestingPositions( dateTime );
				this.lastOptimizationDateTime = this.now();
//				if(this.chosenPositions != null)
//					this.updateTheoreticalProfit();
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
				new CloseToCloseIntervals( firstDateTime, lastDayDateTime,
				                           this.benchmark.Ticker );
			
			this.returnsManager =
				new ReturnsManager( returnIntervals , this.historicalMarketValueProviderForInSample);
		}
				
		private DrivenByFVProviderLogItem getLogItem( EligibleTickers eligibleTickers )
		{
			DrivenByFVProviderLogItem logItem =	
				new DrivenByFVProviderLogItem( this.now(), this.numDaysForFundamentalAnalysis,
				                               this.inSamplePeriodLengthInDays);
			logItem.BestPositionsInSample =
				this.chosenPositions;
			logItem.NumberOfEligibleTickers =
				eligibleTickers.Count;
			logItem.Fitness =
				this.chosenPositions[0].FitnessInSample;
			logItem.Generation = ((IGeneticallyOptimizable)this.chosenPositions[0]).Generation;
			logItem.Tickers =
				this.chosenPositions[0].HashCodeForTickerComposition;
			
			return logItem;
		}
		
		private void raiseNewLogItem( EligibleTickers eligibleTickers )
		{
			DrivenByFVProviderLogItem logItem =
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
