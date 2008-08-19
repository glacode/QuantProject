/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerFPOscillatorCTC.cs
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
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Data;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.Data.DataTables;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedPeriodOscillators
{
	/// <summary>
	/// Implements MarketOpenEventHandler and MarketCloseEventHandler
	/// These handlers contain the core strategy for the oscillator
	/// close to close strategy!
	/// </summary>
	[Serializable]
	public class EndOfDayTimerHandlerFPOscillatorCTC : EndOfDayTimerHandler
	{
		private int numDaysForReturnCalculation;
		private double maxAcceptableCloseToCloseDrawdown;
		private int daysCounterWithPositions;
		private int daysCounterWithRightPositions;
		private int daysCounterWithReversalPositions;
		private bool isReversalPeriodOn = false;
		//  private bool isTheFirstClose = false;
		private DateTime lastCloseDate;
		private IGenomeManager iGenomeManager;
		private int seedForRandomGenerator;
		
		public EndOfDayTimerHandlerFPOscillatorCTC(string tickerGroupID, int numberOfEligibleTickers,
		                                           int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
		                                           Account account,
		                                           int generationNumberForGeneticOptimizer,
		                                           int populationSizeForGeneticOptimizer,
		                                           string benchmark,
		                                           int numDaysForReturnCalculation,
		                                           PortfolioType portfolioType, double maxAcceptableCloseToCloseDrawdown):
			base(tickerGroupID, numberOfEligibleTickers,
			     numberOfTickersToBeChosen, numDaysForOptimizationPeriod, account,
			     generationNumberForGeneticOptimizer,
			     populationSizeForGeneticOptimizer,
			     benchmark, 0.0,
			     portfolioType)
		{
			this.numDaysForReturnCalculation = numDaysForReturnCalculation;
			this.daysCounterWithRightPositions = 0;
			this.daysCounterWithReversalPositions = 0;
			this.isReversalPeriodOn = false;
			this.maxAcceptableCloseToCloseDrawdown = maxAcceptableCloseToCloseDrawdown;
			this.stopLossConditionReached = false;
			this.currentAccountValue = 0.0;
			this.previousAccountValue = 0.0;
			this.numDaysBetweenEachOptimization = 2* numDaysForReturnCalculation;
			this.numDaysBetweenEachOptimization = numDaysForReturnCalculation;
			this.seedForRandomGenerator = ConstantsProvider.SeedForRandomGenerator;
		}
		
		public override void MarketOpenEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			;
		}

		#region MarketCloseEventHandler
		
		protected void marketCloseEventHandler_updateStopLossCondition()
		{
			this.previousAccountValue = this.currentAccountValue;
			this.currentAccountValue = this.account.GetMarketValue();
			if((this.currentAccountValue - this.previousAccountValue)
			   /this.previousAccountValue < -this.maxAcceptableCloseToCloseDrawdown)
			{
				this.stopLossConditionReached = true;
			}
			else
			{
				this.stopLossConditionReached = false;
			}
		}
		
		private void marketCloseEventHandler_updateCounters(bool isTheFirstClose)
		{
			if(this.account.Portfolio.Count > 0 && isTheFirstClose == false)
			{
				if(this.isReversalPeriodOn)
					this.daysCounterWithReversalPositions++ ;
				else
					this.daysCounterWithRightPositions++ ;
			}
		}

		private bool marketCloseEventHandler_openPositionsIfTuned_isTuned(IndexBasedEndOfDayTimer timer)
		{
			bool returnValue = false;
			try
			{
				double gainForTheLastHalfPeriod;
				DateTime initialDateForHalfPeriod =
					(DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition - this.numDaysForReturnCalculation + 1]["quDate"];
				DateTime finalDateForHalfPeriod =
					(DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition]["quDate"];
				gainForTheLastHalfPeriod =
					chosenWeightedPositions.GetCloseToCloseReturn(initialDateForHalfPeriod,finalDateForHalfPeriod);
				if(gainForTheLastHalfPeriod < 0.0)
					//in the last periods the combination has lost, so
					//it should gain the next days
					returnValue = true;
				
				return returnValue;
			}
			catch(MissingQuotesException ex)
			{
				string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
				return returnValue;
			}
		}

		private void marketCloseEventHandler_openPositionsIfTuned(IndexBasedEndOfDayTimer timer)
		{
			if(this.marketCloseEventHandler_openPositionsIfTuned_isTuned(timer))
				AccountManager.OpenPositions(this.chosenWeightedPositions, this.account);
		}
		
		private void marketCloseEventHandler_closePositions()
		{
			this.daysCounterWithPositions++;
			if(this.daysCounterWithPositions == this.numDaysForReturnCalculation ||
			   this.stopLossConditionReached)
			{
				//Close if halfPeriod has elapsed or stop loss condition reached
				AccountManager.ClosePositions(this.account);
				this.daysCounterWithPositions = 0;
			}
		}
		
		public override void MarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			this.marketCloseEventHandler_updateStopLossCondition();
			
			if(this.account.Portfolio.Count == 0 &&
			   this.chosenWeightedPositions != null)
				//portfolio is empty and tickers to buy have been chosen
				this.marketCloseEventHandler_openPositionsIfTuned((IndexBasedEndOfDayTimer)sender);
			else
				this.marketCloseEventHandler_closePositions();
			
			//OLD IMPLEMENTATION: always on the market
			//    	//update isTheFirstClose and optimize after adding cash
			//    	//(this first optimization could be done also after market close)
			//      if (this.account.Transactions.Count == 0)
			//      {
			//        this.isTheFirstClose = true;
			//        this.marketCloseEventHandler_optimize(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime);
			//      }
			//      else
			//      {
			//        this.isTheFirstClose = false;
			//      }
			//      this.marketCloseEventHandler_updateCounters(this.isTheFirstClose);
			//      this.marketCloseEventHandler_updateStopLossCondition();
//
			//      if(this.stopLossConditionReached)
			//      {
			//      	this.orders.Clear();
			//      	this.closePositions();
			//        this.daysCounterWithReversalPositions = 0;
			//        this.daysCounterWithRightPositions = 0;
			//        this.marketCloseEventHandler_optimize(this.lastCloseDate);
			//        this.openPositions();
			//      }
			//      else
			//      {
			//        if(this.account.Portfolio.Count == 0)
			//        {
			//        	this.orders.Clear();
			//        	this.openPositions();
			//        }
//
			//        if((this.isTheFirstClose == false && this.isReversalPeriodOn == false &&
			//          this.daysCounterWithRightPositions == this.numDaysForReturnCalculation))
			//        {
			//          this.orders.Clear();
			//        	this.closePositions();
			//          this.daysCounterWithRightPositions = 0;
			//          this.marketCloseEventHandler_reverseSignOfTickers(this.chosenTickers);
			//          this.openPositions();
			//          this.isReversalPeriodOn = true;
			//        }
//
			//        if((this.isReversalPeriodOn == true &&
			//          this.daysCounterWithReversalPositions == this.numDaysForReturnCalculation))
			//        {
			//          this.orders.Clear();
			//        	this.closePositions();
			//          this.daysCounterWithReversalPositions = 0;
			//          this.isReversalPeriodOn = false;
			//          //code for only one optimization
			////          this.marketCloseEventHandler_reverseSignOfTickers(this.chosenTickers);
			////          this.openPositions();
			//          //
			//          //normal strategy
			//          this.marketCloseEventHandler_optimize(this.lastCloseDate);
			//          this.openPositions();
			//        }
//
			//      } //END OF OLD IMPLEMENTATION
		}

		#endregion
		
		#region OneHourAfterMarketCloseEventHandler
		
		protected DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
		{
			SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID,
			                                                      currentDate);
			
			SelectorByQuotationAtEachMarketDay quotedAtEachMarketFromTemporized =
				new SelectorByQuotationAtEachMarketDay(temporizedGroup.GetTableOfSelectedTickers(),
				                                       false, currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
				                                       this.numberOfEligibleTickers, this.benchmark);

			//      SelectorByCloseToCloseVolatility lessVolatile = new SelectorByCloseToCloseVolatility(
			//        quotedAtEachMarketFromTemporized.GetTableOfSelectedTickers(),true,
			//        currentDate.AddDays(-15), currentDate,
			//        this.numberOfEligibleTickers);
//
			//      return lessVolatile.GetTableOfSelectedTickers();
			
			return quotedAtEachMarketFromTemporized.GetTableOfSelectedTickers();
			
		}
		
		
		protected virtual void setTickers(DateTime currentDate,
		                                  bool setGenomeCounter)
		{
			DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
			if(setOfTickersToBeOptimized.Rows.Count > this.numberOfTickersToBeChosen*2)
				//the optimization process is meaningful only if the initial set of tickers is
				//larger than the number of tickers to be chosen
				
			{
				
				//double targetReturnForEachPeriodOfPortfolioLife =
				//	Math.Pow(1.60,(double)(1.0/(360.0/this.numDaysOfPortfolioLife))) - 1.0;
				//the target has to be such that annual system return is minimum 50%
				//(with no commissions and bid-ask spreads)

				//     	IGenomeManager genManOscillatorCTC =
				//          new GenomeManagerForEfficientCTCPortfolio(setOfTickersToBeOptimized,
				//          currentDate.AddDays(-this.numDaysForOptimizationPeriod),
				//          currentDate, this.numberOfTickersToBeChosen,
				//          this.numDaysForReturnCalculation, 0.0,
				//          this.portfolioType);
				this.iGenomeManager =
					new GenomeManagerForFPOscillatorCTC(setOfTickersToBeOptimized,
					                                    currentDate.AddDays(-this.numDaysForOptimizationPeriod),
					                                    currentDate, this.numberOfTickersToBeChosen,
					                                    this.numDaysForReturnCalculation,
					                                    this.portfolioType, this.benchmark);
				GeneticOptimizer GO = new GeneticOptimizer(this.iGenomeManager,
				                                           this.populationSizeForGeneticOptimizer,
				                                           this.generationNumberForGeneticOptimizer,
				                                           this.seedForRandomGenerator);
				if(setGenomeCounter)
					this.genomeCounter = new GenomeCounter(GO);
				
				GO.Run(false);
				this.addGenomeToBestGenomes(GO.BestGenome,((GenomeManagerForEfficientPortfolio)this.iGenomeManager).FirstQuoteDate,
				                            ((GenomeManagerForEfficientPortfolio)this.iGenomeManager).LastQuoteDate, setOfTickersToBeOptimized.Rows.Count,
				                            this.numDaysForReturnCalculation);
				this.chosenWeightedPositions = new WeightedPositions( ((GenomeMeaning)GO.BestGenome.Meaning).TickersPortfolioWeights,
				                                                     new SignedTickers( ((GenomeMeaning)GO.BestGenome.Meaning).Tickers) );
			}
			//else it will be buyed again the previous optimized portfolio
			//that's it the actual chosenTickers member
		}

		/// <summary>
		/// Handles a "One hour after market close" event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		public override void OneHourAfterMarketCloseEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
			this.lastCloseDate = endOfDayTimingEventArgs.EndOfDayDateTime.DateTime;
			this.seedForRandomGenerator++;
			this.numDaysElapsedSinceLastOptimization++;
			if((this.numDaysElapsedSinceLastOptimization - 1 ==
			    this.numDaysBetweenEachOptimization)) //|| this.isTheFirstClose )
				//num days without optimization has elapsed or
				//it is the first close (OLD IMPLEMENTATION)
			{
				this.setTickers(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime, false);
				//sets tickers to be chosen next Market Close event
				this.numDaysElapsedSinceLastOptimization = 0;
			}
			
		}
		#endregion
	}
}
