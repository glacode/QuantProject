/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerSimplePT.cs
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
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;
using QuantProject.Business.DataProviders;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.Data.DataTables;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.ArbitrageTesting.PairTrading.SimplePairTrading.InSample;

namespace QuantProject.Scripts.ArbitrageTesting.PairTrading.SimplePairTrading
{
	
	/// <summary>
	/// Implements MarketOpenEventHandler,
	/// MarketCloseEventHandler and OneHourAfterMarketCloseEventHandler
	/// for the implementation of the simple pair trading strategy
	/// </summary>
	[Serializable]
	public class EndOfDayTimerHandlerSimplePT : EndOfDayStrategy
	{
		private string tickerGroupID;
		protected string[] chosenTickers = new string[10];
		//this array is designed for containing
		//five couples of eligible tickers for
		//the pair trading strategy
		protected double[] averageGapsOfChosenTickers = new double[10];
		protected double[] stdDevGapsOfChosenTickers = new double[10];
		protected string[] lastOrderedTickers = new string[2];
		protected int addedTickersCounter = 0;
		protected float currentPair_firstTickerGain = 0.0F;
		protected float currentPair_secondTickerGain = 0.0F;
		
		private int numberOfEligibleTickers;
		private int numDaysForOptimizationPeriod;
		private int generationNumberForGeneticOptimizer;
		private int populationSizeForGeneticOptimizer;
		protected string benchmark;
		protected DateTime startDate;
		protected DateTime endDate;
		protected double maxNumOfStdDevForNormalGap;
		private double maxLevelForNormalGap;
		private int minNumOfDaysForGapComputation;
		private int maxNumOfDaysForGapComputation;
		private int numDaysBetweenEachOptimization;
		private int numDaysElapsedSinceLastOptimization;
		private double maxRunningHours;
//		protected Account account;
		private int seedForRandomGenerator;
		protected double minimumGainForClosingPositions;
		protected double maximumToleratedLoss;
		protected double initialAccountValue = 0.0;
		protected int counterForDaysWithPositions = 0;
		protected int currentNumDaysForGapComputation;
		//it stores the value for the new account
		protected HistoricalAdjustedQuoteProvider historicalAdjustedQuoteProvider;
		private OptimizationOutput optimizationOutput;
		
		public OptimizationOutput OptimizationOutput
		{
			get{return this.optimizationOutput;}
		}

		public EndOfDayTimerHandlerSimplePT(string tickerGroupID, int numberOfEligibleTickers,
		                                    int numDaysForOptimizationPeriod,
		                                    int generationNumberForGeneticOptimizer,
		                                    int populationSizeForGeneticOptimizer,
		                                    string benchmark,
		                                    DateTime startDate, DateTime endDate,
		                                    double maxNumOfStdDevForNormalGap, int minNumOfDaysForGapComputation,
		                                    int maxNumOfDaysForGapComputation, int numDaysBetweenEachOptimization,
		                                    double maxRunningHours,
		                                    Account account)
		{
			this.tickerGroupID = tickerGroupID;
			this.numberOfEligibleTickers = numberOfEligibleTickers;
			this.numDaysForOptimizationPeriod = numDaysForOptimizationPeriod;
			this.generationNumberForGeneticOptimizer = generationNumberForGeneticOptimizer;
			this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
			this.benchmark = benchmark;
			this.startDate = startDate;
			this.endDate = endDate;
			this.maxNumOfStdDevForNormalGap = maxNumOfStdDevForNormalGap;
			this.minNumOfDaysForGapComputation = minNumOfDaysForGapComputation;
			this.maxNumOfDaysForGapComputation = maxNumOfDaysForGapComputation;
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
			this.maxRunningHours = maxRunningHours;
			this.account = account;
			this.numDaysElapsedSinceLastOptimization = 0;
			this.seedForRandomGenerator = ConstantsProvider.SeedForRandomGenerator;
			this.historicalAdjustedQuoteProvider = new HistoricalAdjustedQuoteProvider();
			this.minimumGainForClosingPositions = 0.002;
			this.maximumToleratedLoss = 0.02;
			this.optimizationOutput = new OptimizationOutput();
		}

		
		protected override void marketOpenEventHandler(
			Object sender , DateTime dateTime )
		{
			
		}
		
		#region marketCloseEventHandler
		
		public void marketCloseEventHandler_closePositions()
		{
			if(this.account.Portfolio[ this.lastOrderedTickers[0] ] != null)
				this.account.ClosePosition( this.lastOrderedTickers[0] );

			if(this.account.Portfolio[ this.lastOrderedTickers[1] ] != null)
				this.account.ClosePosition( this.lastOrderedTickers[1] );
		}
		
		private bool marketCloseEventHandler_isCurrentGainGoodEnoughOrStopLossConditionReached(
			DateTime currentDateTime )
		{
			bool returnValue = false;
			double currentGain =
				(this.account.GetMarketValue() - this.initialAccountValue)/
				this.initialAccountValue;
			if(currentGain >= this.minimumGainForClosingPositions ||
			   -currentGain >= this.maximumToleratedLoss)
				//gain is good enough or loss is too high ...
				returnValue = true;

			return returnValue;
		}
		
		private float marketCloseEventHandler_openPositions_getGap(int indexForPair,
		                                                           DateTime firstDate,
		                                                           DateTime lastDate)
		{
			Quotes firstTickerQuotes = new Quotes(this.chosenTickers[indexForPair], firstDate, lastDate);
			Quotes secondTickerQuotes = new Quotes(this.chosenTickers[indexForPair+1], firstDate, lastDate);
			this.currentPair_firstTickerGain =
				(float)firstTickerQuotes.Rows[1]["quClose"]/(float)firstTickerQuotes.Rows[0]["quClose"];
			this.currentPair_secondTickerGain =
				(float)secondTickerQuotes.Rows[1]["quClose"]/(float)secondTickerQuotes.Rows[0]["quClose"];
			return  (this.currentPair_firstTickerGain - this.currentPair_secondTickerGain);
		}
		
		private void marketCloseEventHandler_openPositions_open(int indexForPair)
			
		{
			double cashForSinglePosition = this.account.CashAmount/2;
			long quantityForFirstTicker =
				Convert.ToInt64( Math.Floor( cashForSinglePosition / this.account.DataStreamer.GetCurrentBid( this.chosenTickers[indexForPair] ) ) );
			long quantityForSecondTicker =
				Convert.ToInt64( Math.Floor( cashForSinglePosition / this.account.DataStreamer.GetCurrentBid( this.chosenTickers[indexForPair+1] ) ) );
			OrderType orderTypeForFirstTicker = OrderType.MarketBuy;
			OrderType orderTypeForSecondTicker = OrderType.MarketSellShort;
			if(this.currentPair_firstTickerGain > this.currentPair_secondTickerGain)
			{
				orderTypeForFirstTicker = OrderType.MarketSellShort;
				orderTypeForSecondTicker = OrderType.MarketBuy;
			}
			this.account.AddOrder(new Order( orderTypeForFirstTicker,
			                                new Instrument( this.chosenTickers[indexForPair] ) ,
			                                quantityForFirstTicker ));
			this.account.AddOrder(new Order( orderTypeForSecondTicker,
			                                new Instrument( this.chosenTickers[indexForPair+1] ) ,
			                                quantityForSecondTicker ));
			this.lastOrderedTickers[0] = this.chosenTickers[indexForPair];
			this.lastOrderedTickers[1] = this.chosenTickers[indexForPair+1];
		}
		
		private void marketCloseEventHandler_openPositions_setMaxLevelForNormalGap(int indexForPair)
			
		{
			this.maxLevelForNormalGap = this.averageGapsOfChosenTickers[indexForPair] +
				this.maxNumOfStdDevForNormalGap * 
				this.stdDevGapsOfChosenTickers[indexForPair];
		}
		
		private void marketCloseEventHandler_openPositions(
			DateTime currentDateTime , IndexBasedEndOfDayTimer timer )
		{
			if(this.chosenTickers[0] != null)
				//potential tickers for pair trading have been set
			{
				for(int i = 0; i<this.chosenTickers.Length - 1 &&
				    this.account.Portfolio.Count == 0; i++)
				{
					this.marketCloseEventHandler_openPositions_setMaxLevelForNormalGap(i);
					if(Math.Abs(this.marketCloseEventHandler_openPositions_getGap(
						i, timer.GetPreviousDateTime(),
						timer.GetCurrentDateTime())) >= (float)this.maxLevelForNormalGap)
						//currentGap is too high for being considered rational
						//so there is an arbitrage opportunity
						this.marketCloseEventHandler_openPositions_open(i);
				}
				this.initialAccountValue = this.account.GetMarketValue();
				//this.account.Portfolio.GetMarketValue(currentEndOfDayDateTime,this.historicalAdjustedQuoteProvider);
			}
		}
		
		protected override void marketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			if(this.account.Portfolio.Count == 0)
				//portfolio is empty
			{
				if(this.account.Transactions.Count == 0)
					this.account.AddCash(30000);
				this.marketCloseEventHandler_openPositions(dateTime,
				                                           (IndexBasedEndOfDayTimer)sender);
			}
			else // portfolio is not empty
			{
				this.counterForDaysWithPositions++;
				if(this.counterForDaysWithPositions == this.currentNumDaysForGapComputation ||
				   this.marketCloseEventHandler_isCurrentGainGoodEnoughOrStopLossConditionReached(dateTime))
					//a number of days equal to the number of days
					//used for gap computation has elapsed or
					//the current gain is good enough or
					//loss is too high
				{
					this.marketCloseEventHandler_closePositions();
					this.counterForDaysWithPositions = 0;
				}
				
			}
		}
		#endregion

		#region oneHourAfterMarketCloseEventHandler
		
		protected DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
		{
			SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID, currentDate);
			SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromEligible =
				new SelectorByQuotationAtEachMarketDay( temporizedGroup.GetTableOfSelectedTickers(),
				                                       false, currentDate.AddDays(-this.numDaysForOptimizationPeriod),
				                                       currentDate, this.numberOfEligibleTickers, this.benchmark);
			return quotedAtEachMarketDayFromEligible.GetTableOfSelectedTickers();
		}
		
		private void setTickers_setChosenTickers_addGenomeForOptimizationOutput(Genome genome, DateTime currentDate)
		{
			this.optimizationOutput.Add(new GenomeRepresentation(this.maxNumOfStdDevForNormalGap,
			                                                     genome,
			                                                     currentDate.AddDays(-this.numDaysForOptimizationPeriod),
			                                                     currentDate));
		}

		private void setTickers_setChosenTickers_addTickers(Genome genome)
		{
			this.chosenTickers[this.addedTickersCounter] =
				((GenomeMeaningSimplePT)genome.Meaning).FirstTicker;
			this.chosenTickers[this.addedTickersCounter+1] =
				((GenomeMeaningSimplePT)genome.Meaning).SecondTicker;
			
			this.averageGapsOfChosenTickers[this.addedTickersCounter] =
				((GenomeMeaningSimplePT)genome.Meaning).AverageGap;
			this.stdDevGapsOfChosenTickers[this.addedTickersCounter] =
				((GenomeMeaningSimplePT)genome.Meaning).StdDevGap;

			this.currentNumDaysForGapComputation = ((GenomeMeaningSimplePT)genome.Meaning).NumOfDaysForGap;
			
			this.addedTickersCounter += 2;
		}
		
		private bool setTickers_setChosenTickers_isAnyTickerOfGenomeInChosenTickers(Genome genome)
		{
			bool returnValue = false;
			foreach(string ticker in this.chosenTickers)
			{
				if(ticker == ((GenomeMeaningSimplePT)genome.Meaning).FirstTicker ||
				   ticker == ((GenomeMeaningSimplePT)genome.Meaning).SecondTicker)
					returnValue = true;
			}
			return returnValue;
		}
		
		private void setTickers_setChosenTickers(GeneticOptimizer GO, DateTime currentDate)
		{
			this.addedTickersCounter = 0;
			for(int i = 0;
			    i<GO.CurrentGeneration.Count -1 &&
			    addedTickersCounter<this.chosenTickers.Length;
			    i++)
			{
				Genome currentGenome = (Genome)GO.CurrentGeneration[GO.CurrentGeneration.Count-i-1];
				if(!this.setTickers_setChosenTickers_isAnyTickerOfGenomeInChosenTickers(currentGenome))
				{
					this.setTickers_setChosenTickers_addTickers(currentGenome);
					this.setTickers_setChosenTickers_addGenomeForOptimizationOutput(currentGenome,
					                                                                currentDate);
				}
			}
		}

		private void setTickers(DateTime currentDate)
		{
			
			DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
			IGenomeManager genManForSimplePT =
				new GenomeManagerForSimplePT(setOfTickersToBeOptimized,
				                             currentDate.AddDays(-this.numDaysForOptimizationPeriod),
				                             currentDate,
				                             this.minNumOfDaysForGapComputation,
				                             this.maxNumOfDaysForGapComputation,
				                             this.maxNumOfStdDevForNormalGap);
			
			GeneticOptimizer GO = new GeneticOptimizer(genManForSimplePT,
			                                           this.populationSizeForGeneticOptimizer,
			                                           this.generationNumberForGeneticOptimizer,
			                                           this.seedForRandomGenerator);
			GO.Run(false);
			this.setTickers_setChosenTickers(GO, currentDate);
		}

		/// <summary>
		/// Handles a "One hour after market close" event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void oneHourAfterMarketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			this.seedForRandomGenerator++;
			if(this.numDaysElapsedSinceLastOptimization ==
			   this.numDaysBetweenEachOptimization - 1)
			{
				this.setTickers( dateTime );
				//sets tickers to be chosen at next Market Open event
				this.numDaysElapsedSinceLastOptimization = 0;
			}
			else
			{
				this.numDaysElapsedSinceLastOptimization++;
			}
		}
		
		#endregion
		
	}
}
