/*
QuantProject - Quantitative Finance Library

PVO_OTCStrategyLessCorrelated.cs
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
using QuantProject.ADT.Optimizing.Genetic;
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
	public class PVO_OTCStrategyLessCorrelated : IStrategyForBacktester
	{
		public event NewLogItemEventHandler NewLogItem;
		public event NewMessageEventHandler NewMessage;

		//initialized by the constructor
		protected int inSampleDays;
		protected int numDaysBetweenEachOptimization;
		protected IInSampleChooser inSampleChooser;
		protected IEligiblesSelector eligiblesSelector;
		protected Benchmark benchmark;
		protected HistoricalMarketValueProvider historicalQuoteProvider;
		protected double oversoldThreshold;
		protected double overboughtThreshold;
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

		private void pvo_otcStrategy(IEligiblesSelector eligiblesSelector,
		                             int inSampleDays,
		                             Benchmark benchmark,
		                             int numDaysBetweenEachOptimization,
		                             double oversoldThreshold,
		                             double overboughtThreshold,
		                             HistoricalMarketValueProvider historicalQuoteProvider)
		{
			this.eligiblesSelector = eligiblesSelector;
			this.inSampleDays = inSampleDays;
			this.benchmark = benchmark;
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
			this.oversoldThreshold = oversoldThreshold;
			this.overboughtThreshold = overboughtThreshold;
			this.historicalQuoteProvider = historicalQuoteProvider;
		}

		public PVO_OTCStrategyLessCorrelated(IEligiblesSelector eligiblesSelector,
		                                     IInSampleChooser inSampleChooser,
		                                     int inSampleDays,
		                                     Benchmark benchmark,
		                                     int numDaysBetweenEachOptimization,
		                                     double oversoldThreshold,
		                                     double overboughtThreshold,
		                                     HistoricalMarketValueProvider historicalQuoteProvider)
			
		{
			this.pvo_otcStrategy(eligiblesSelector, inSampleDays , benchmark , numDaysBetweenEachOptimization ,
			                     oversoldThreshold, overboughtThreshold,
			                     historicalQuoteProvider);
			this.inSampleChooser = inSampleChooser;
		}
		
		public PVO_OTCStrategyLessCorrelated(IEligiblesSelector eligiblesSelector,
		                                     TestingPositions[] chosenPVOPositions,
		                                     int inSampleDays,
		                                     Benchmark benchmark,
		                                     int numDaysBetweenEachOptimization,
		                                     double oversoldThreshold,
		                                     double overboughtThreshold,
		                                     HistoricalMarketValueProvider historicalQuoteProvider)
			
		{
			this.pvo_otcStrategy(eligiblesSelector, inSampleDays , benchmark , numDaysBetweenEachOptimization ,
			                     oversoldThreshold, overboughtThreshold, historicalQuoteProvider);
			this.chosenPVOPositions = chosenPVOPositions;
		}
		
		#region MarketOpenEventHandler
		
		protected virtual DateTime getBeginOfOscillatingPeriod(IndexBasedEndOfDayTimer timer)
		{
			DateTime beginOfOscillatingPeriod =
				HistoricalEndOfDayTimer.GetMarketClose(
					(DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition-1]["quDate"] );
			return beginOfOscillatingPeriod;
//					
//				return new EndOfDayDateTime(	(DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition-1]["quDate"],
//				                            EndOfDaySpecificTime.MarketClose );
		}
		
		private PVOPositionsStatus marketOpenEventHandler_openPositions_getStatus(IndexBasedEndOfDayTimer timer)
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
		
		protected void marketOpenEventHandler_openPositions(IndexBasedEndOfDayTimer timer)
		{
			PVOPositionsStatus pvoPositionsStatus = PVOPositionsStatus.InTheMiddle;
			if(timer.CurrentDateArrayPosition >= 1)
				pvoPositionsStatus =
					this.marketOpenEventHandler_openPositions_getStatus(timer);
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
							string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
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

		private void marketOpenEventHandler(
			Object sender , DateTime dateTime )
		{
			if ( this.account.Portfolio.Count == 0 &&
			    this.chosenPVOPositions != null )
				//portfolio is empty and optimization has
				//been already launched
			{
				try{
					this.marketOpenEventHandler_openPositions( (IndexBasedEndOfDayTimer)sender );
				}
				catch(TickerNotExchangedException ex)
				{
					string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
				}
			}
		}
		
		#endregion
		
		private void fiveMinutesBeforeMarketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
		}

		private DateTime now()
		{
			return this.account.Timer.GetCurrentDateTime();
		}

		
		private void marketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			if(this.account.Portfolio.Count > 0)
				AccountManager.ClosePositions(this.account);
		}
		
		#region OneHourAfterMarketCloseEventHandler
		
		protected virtual void updateReturnsManager(DateTime firstDateTime,
		                                            DateTime lastDateTime)
		{
			this.returnsManager =
				new ReturnsManager( new DailyOpenToCloseIntervals(firstDateTime, lastDateTime,
				                                                  this.benchmark.Ticker ) ,
				                   this.historicalQuoteProvider);
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
		
		private TestingPositions[] getReversedTestingPositions()
		{
			int arrayLength = this.chosenPVOPositions.Length;
			TestingPositions[] reversedTestingPositions =
				new TestingPositions[arrayLength];
			
			for(int i = 0; i < arrayLength ; i++)
			{
				reversedTestingPositions[arrayLength - i - 1] =
					this.chosenPVOPositions[i];
			}
			return reversedTestingPositions;
		}

		protected virtual void updateTestingPositions(DateTime currentDate)
		{
			History history =
				this.benchmark.GetEndOfDayHistory(
					HistoricalEndOfDayTimer.GetMarketOpen(
						currentDate.AddDays(-this.inSampleDays) ) ,
					HistoricalEndOfDayTimer.GetMarketClose( currentDate ) );
//					new EndOfDayDateTime(currentDate.AddDays(-this.inSampleDays),
//					                     EndOfDaySpecificTime.MarketOpen),
//					new EndOfDayDateTime(currentDate,
//					                     EndOfDaySpecificTime.MarketClose));
			EligibleTickers eligibles =
				this.eligiblesSelector.GetEligibleTickers(history);
			this.updateReturnsManager(history.FirstDateTime,
			                          history.LastDateTime);
			if(this.inSampleChooser != null)
				this.chosenPVOPositions = (TestingPositions[])inSampleChooser.AnalyzeInSample(eligibles, this.returnsManager);
			this.updateTestingPositions_updateThresholds();
			this.chosenPVOPositions = this.getReversedTestingPositions();
			this.logOptimizationInfo(eligibles);
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
			//OLD - numDaysBetweenEachOptimization --> market days
//			if( this.account.Transactions.Count <= 1 ||
//				  (this.numDaysElapsedSinceLastOptimization ==
			//            this.numDaysBetweenEachOptimization) )
			//num days without optimization has elapsed or
			//no transaction, except for adding cash, has been executed
			//NEW - numDaysBetweenEachOptimization --> calendar days
			if ( this.optimalTestingPositionsAreToBeUpdated() )
			{
				this.updateTestingPositions(dateTime);
				//sets tickers to be chosen next Market Close event
				this.numDaysElapsedSinceLastOptimization = 0;
				this.lastOptimizationDateTime = this.now();
			}
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
