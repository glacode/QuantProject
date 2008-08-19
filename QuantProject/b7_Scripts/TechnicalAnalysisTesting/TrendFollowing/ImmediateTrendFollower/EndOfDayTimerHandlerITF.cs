/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerITF.cs
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

namespace QuantProject.Scripts.TechnicalAnalysisTesting.TrendFollowing.ImmediateTrendFollower
{
	/// <summary>
	/// Implements MarketOpenEventHandler and MarketCloseEventHandler
	/// These handlers contain the core strategy for the extreme
	/// counter trend strategy!
	/// </summary>
	[Serializable]
	public class EndOfDayTimerHandlerITF : EndOfDayTimerHandler
	{
		private int numDaysForReturnCalculation;
		private double maxAcceptableCloseToCloseDrawdown;
		private int daysCounterWithPositions;
		private DateTime lastCloseDate;
		private IGenomeManager iGenomeManager;
		private int seedForRandomGenerator;
		
		public EndOfDayTimerHandlerITF(string tickerGroupID, int numberOfEligibleTickers,
		                               int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
		                               Account account,
		                               int generationNumberForGeneticOptimizer,
		                               int populationSizeForGeneticOptimizer,
		                               string benchmark,
		                               int numDaysForReturnCalculation,
		                               int numDaysBetweenEachOptimization,
		                               PortfolioType portfolioType, double maxAcceptableCloseToCloseDrawdown):
			base(tickerGroupID, numberOfEligibleTickers,
			     numberOfTickersToBeChosen, numDaysForOptimizationPeriod, account,
			     generationNumberForGeneticOptimizer,
			     populationSizeForGeneticOptimizer,
			     benchmark, 0.0,
			     portfolioType)
		{
			this.numDaysForReturnCalculation = numDaysForReturnCalculation;
			this.maxAcceptableCloseToCloseDrawdown = maxAcceptableCloseToCloseDrawdown;
			this.stopLossConditionReached = false;
			this.currentAccountValue = 0.0;
			this.previousAccountValue = 0.0;
			//      this.numDaysBetweenEachOptimization = 2* numDaysForReturnCalculation;
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
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

		private double marketCloseEventHandler_openPositions_getLastHalfPeriodGain(IndexBasedEndOfDayTimer timer)
		{
			double returnValue = 999.0;
			try
			{
				DateTime initialDateForHalfPeriod =
					(DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition - this.numDaysForReturnCalculation + 1]["quDate"];
				DateTime finalDateForHalfPeriod =
					(DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition]["quDate"];
				returnValue =
					chosenWeightedPositions.GetCloseToCloseReturn(initialDateForHalfPeriod,finalDateForHalfPeriod);
			}
			catch(MissingQuotesException ex)
			{
				string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
			}
			return returnValue;
		}

		private void marketCloseEventHandler_openPositions(IndexBasedEndOfDayTimer timer)
		{
			double lastHalfPeriodGain =
				this.marketCloseEventHandler_openPositions_getLastHalfPeriodGain(timer);
			if(lastHalfPeriodGain != 999.0)
				//last half period gain has been properly computed
			{
				if(lastHalfPeriodGain > 0.0)
					AccountManager.OpenPositions(chosenWeightedPositions, this.account);
				else//the last HalfPeriodGain has been negative
				{
					chosenWeightedPositions.Reverse();
					//short the portfolio
					try{
						AccountManager.OpenPositions(chosenWeightedPositions, this.account);
					}
					catch(Exception ex)
					{
						string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
					}
					finally{
						chosenWeightedPositions.Reverse();
					}
				}
			}
		}
		
		private void marketCloseEventHandler_closePositions()
		{
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
			//this.marketCloseEventHandler_updateStopLossCondition();
			if(this.account.Portfolio.Count > 0)
				this.daysCounterWithPositions++;
			this.marketCloseEventHandler_closePositions();
			if(chosenWeightedPositions != null)
				//tickers to buy have been chosen
			{
				if(this.account.Portfolio.Count == 0)
					this.marketCloseEventHandler_openPositions((IndexBasedEndOfDayTimer)sender);
			}
			
		}

		#endregion
		
		#region OneHourAfterMarketCloseEventHandler
		

		protected DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
		{
			SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID, currentDate);
			DataTable tickersFromGroup = temporizedGroup.GetTableOfSelectedTickers();
			int numOfTickersInGroupAtCurrentDate = tickersFromGroup.Rows.Count;
			
			SelectorByAverageRawOpenPrice byPrice =
				new SelectorByAverageRawOpenPrice(tickersFromGroup,false,currentDate,
				                                  currentDate.AddDays(-30),
				                                  numOfTickersInGroupAtCurrentDate,
				                                  30,500, 0.0001,100);
			
			SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromByPrice =
				new SelectorByQuotationAtEachMarketDay(byPrice.GetTableOfSelectedTickers(),
				                                       false, currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
				                                       numOfTickersInGroupAtCurrentDate, this.benchmark);
			
			//      SelectorByCloseToCloseVolatility lessVolatile =
			//      	new SelectorByCloseToCloseVolatility(quotedAtEachMarketDayFromByPrice.GetTableOfSelectedTickers(),
			//      	                                     true,currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
			//      	                                     this.numberOfEligibleTickers);
			
			SelectorByLiquidity mostLiquidSelector =
				new SelectorByLiquidity(quotedAtEachMarketDayFromByPrice.GetTableOfSelectedTickers(),
				                        true,currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
				                        this.numberOfEligibleTickers);
			
			
			return mostLiquidSelector.GetTableOfSelectedTickers();
			//OLD for etf
			//      SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID,
			//        																										currentDate);
//
			//      SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromTemporized =
			//        new SelectorByQuotationAtEachMarketDay(temporizedGroup.GetTableOfSelectedTickers(),
			//        false, currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
			//        600, this.benchmark);
			//      // filter to be used with plain stocks
			//      DataTable tickersQuotedAtEachMarketDay = quotedAtEachMarketDayFromTemporized.GetTableOfSelectedTickers();
			//      SelectorByLiquidity mostLiquid =
			//      	new SelectorByLiquidity(tickersQuotedAtEachMarketDay,
			//      	                        false,currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
			//      	                        tickersQuotedAtEachMarketDay.Rows.Count/2);
//
			//      DataTable mostLiquidTickers = mostLiquid.GetTableOfSelectedTickers();
//
			//      SelectorByCloseToCloseVolatility lessVolatile =
			//      	new SelectorByCloseToCloseVolatility(mostLiquidTickers,
			//      	                                     true,currentDate.AddDays(-30), currentDate,
			//      	                                     Math.Min(this.numberOfEligibleTickers, mostLiquidTickers.Rows.Count/2));
			//////      return mostLiquid.GetTableOfSelectedTickers();
			//      return lessVolatile.GetTableOfSelectedTickers();
			//      //
			////      return quotedAtEachMarketDayFromTemporized.GetTableOfSelectedTickers();
		}
		
		
		protected virtual void setTickers(DateTime currentDate,
		                                  bool setGenomeCounter)
		{
			DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
			if(setOfTickersToBeOptimized.Rows.Count > this.numberOfTickersToBeChosen*2)
				//the optimization process is meaningful only if the initial set of tickers is
				//larger than the number of tickers to be chosen
				
			{
				this.iGenomeManager =
					new GenomeManagerITF(setOfTickersToBeOptimized,
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
