/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerCTCWeekly.cs
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
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.ADT.Optimizing.Genetic;

namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	
	/// <summary>
	/// Implements MarketOpenEventHandler and MarketCloseEventHandler
	/// These handlers contain the core strategy for the efficient close to close
	/// weekly portfolio (with a given days of life)!
	/// </summary>
	[Serializable]
	public class EndOfDayTimerHandlerCTCWeekly : EndOfDayTimerHandler
	{
		protected int numDaysForReturnCalculation;
		protected double maxAcceptableCloseToCloseDrawdown;
		
		public EndOfDayTimerHandlerCTCWeekly(string tickerGroupID, int numberOfEligibleTickers,
		                                     int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
		                                     Account account,
		                                     int generationNumberForGeneticOptimizer,
		                                     int populationSizeForGeneticOptimizer,
		                                     string benchmark,
		                                     int numDaysForReturnCalculation,
		                                     double targetReturn,
		                                     PortfolioType portfolioType, double maxAcceptableCloseToCloseDrawdown):
			base(tickerGroupID, numberOfEligibleTickers,
			     numberOfTickersToBeChosen, numDaysForOptimizationPeriod, account,
			     generationNumberForGeneticOptimizer,
			     populationSizeForGeneticOptimizer,
			     benchmark, targetReturn,
			     portfolioType)
		{
			this.numDaysForReturnCalculation = numDaysForReturnCalculation;
			this.maxAcceptableCloseToCloseDrawdown = maxAcceptableCloseToCloseDrawdown;
		}

		protected override void marketOpenEventHandler(
			Object sender , DateTime dateTime )
		{
			;
		}
		
		#region marketCloseEventHandler
		protected void updateStopLossCondition()
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
		
		protected override void marketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			this.updateStopLossCondition();
			
			if(dateTime.DayOfWeek ==
			   DayOfWeek.Monday)
				this.openPositions();
			
			if(this.stopLossConditionReached ||
			   dateTime.DayOfWeek ==
			   DayOfWeek.Friday)
				AccountManager.ClosePositions(this.account);
			
		}
		
		#endregion marketCloseEventHandler
		
		#region oneHourAfterMarketCloseEventHandler
		
		protected DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
		{
			SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID,
			                                                      currentDate);
			
			this.eligibleTickers = temporizedGroup.GetTableOfSelectedTickers();
			SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromEligible =
				new SelectorByQuotationAtEachMarketDay(this.eligibleTickers,
				                                       false, currentDate.AddDays(-this.numDaysForOptimizationPeriod),currentDate,
				                                       this.numberOfEligibleTickers, this.benchmark);
			return quotedAtEachMarketDayFromEligible.GetTableOfSelectedTickers();
		}
		
		
		protected virtual void setTickers(DateTime currentDate)
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
				IGenomeManager genManEfficientCTCPortfolio =
					new GenomeManagerForEfficientCTCPortfolio(setOfTickersToBeOptimized,
					                                          currentDate.AddDays(-this.numDaysForOptimizationPeriod),
					                                          currentDate, this.numberOfTickersToBeChosen,
					                                          this.numDaysForReturnCalculation,
					                                          this.targetReturn,
					                                          this.portfolioType, this.benchmark);
				GeneticOptimizer GO = new GeneticOptimizer(genManEfficientCTCPortfolio,
				                                           this.populationSizeForGeneticOptimizer,
				                                           this.generationNumberForGeneticOptimizer);
				//GO.KeepOnRunningUntilConvergenceIsReached = true;
				GO.Run(false);
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
		protected override void oneHourAfterMarketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			
			if(this.account.Portfolio.Count == 0 &&
			   dateTime.DayOfWeek ==
			   DayOfWeek.Friday)
			{
				this.setTickers(dateTime);
				//it sets tickers to be chosen at next Monday
			}
		}
		#endregion oneHourAfterMarketCloseEventHandler
	}
}
