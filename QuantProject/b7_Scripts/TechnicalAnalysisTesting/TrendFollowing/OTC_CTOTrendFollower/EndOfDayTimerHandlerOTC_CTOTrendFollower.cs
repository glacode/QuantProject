/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerOTC_CTOTrendFollower.cs
Copyright (C) 2007
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
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.TrendFollowing.OTC_CTOTrendFollower
{
	
	/// <summary>
	/// Implements MarketOpenEventHandler,
	/// TwoMinutesBeforeMarketCloseEventHandler and OneHourAfterMarketCloseEventHandler
	/// These handlers contain the core strategy for the efficient close to open portfolio!
	/// </summary>
	[Serializable]
	public class EndOfDayTimerHandlerOTC_CTOTrendFollower :
		QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios.EndOfDayTimerHandler
	{
		protected int seedForRandomGenerator;
		private double stopLossLevel;
		private double accountValueAtOpeningPositions;
		
		public EndOfDayTimerHandlerOTC_CTOTrendFollower(string tickerGroupID, int numberOfEligibleTickers,
		                                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod, Account account,
		                                                int generationNumberForGeneticOptimizer,
		                                                int populationSizeForGeneticOptimizer,
		                                                string benchmark, double targetReturn,
		                                                double stopLossLevel, PortfolioType portfolioType, int numDaysBetweenEachOptimization):
			base(tickerGroupID, numberOfEligibleTickers,
			     numberOfTickersToBeChosen, numDaysForOptimizationPeriod, account,
			     generationNumberForGeneticOptimizer,
			     populationSizeForGeneticOptimizer,
			     benchmark, targetReturn,
			     portfolioType)
		{
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
			this.numDaysElapsedSinceLastOptimization = 0;
			this.seedForRandomGenerator = ConstantsProvider.SeedForRandomGenerator;
			this.stopLossLevel = stopLossLevel;
		}
		
		/// <summary>
		/// Handles a "Market Open" event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void marketOpenEventHandler(
			Object sender , DateTime dateTime )
		{
			if(this.account.Transactions.Count == 0)
				this.account.AddCash(15000);
			if(this.account.Portfolio.Count == 0)
			{
				AccountManager.OpenPositions(this.chosenWeightedPositions, this.account);
				this.accountValueAtOpeningPositions = this.account.GetMarketValue();
			}
		}
		
		private void marketCloseEventHandler_updateTakeProfitCondition()
		{
			if( (this.currentAccountValue - this.accountValueAtOpeningPositions)/
			   this.accountValueAtOpeningPositions > this.targetReturn         )
				this.takeProfitConditionReached = true;
			else
				this.takeProfitConditionReached = false;
		}
		private void marketCloseEventHandler_updateStopLossCondition()
		{
			if( (this.currentAccountValue - this.accountValueAtOpeningPositions)/
			   this.accountValueAtOpeningPositions < -this.stopLossLevel       )
				this.stopLossConditionReached = true;
			else
				this.stopLossConditionReached = false;
		}
		protected override void marketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			this.currentAccountValue = this.account.GetMarketValue();
			this.marketCloseEventHandler_updateStopLossCondition();
			this.marketCloseEventHandler_updateTakeProfitCondition();
			if( (this.numDaysElapsedSinceLastOptimization ==
			     this.numDaysBetweenEachOptimization - 1) ||
			   this.stopLossConditionReached            ||
			   this.takeProfitConditionReached             )
				AccountManager.ClosePositions(this.account);
		}
		
		#region oneHourAfterMarketCloseEventHandler
		protected DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
		{
			SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID, currentDate);
			DataTable tickersFromGroup = temporizedGroup.GetTableOfSelectedTickers();
			SelectorByLiquidity mostLiquid =
				new SelectorByLiquidity(tickersFromGroup,
				                        false,currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
				                        this.numberOfEligibleTickers);
			
			SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromMostLiquid =
				new SelectorByQuotationAtEachMarketDay(mostLiquid.GetTableOfSelectedTickers(),
				                                       false, currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
				                                       this.numberOfEligibleTickers, this.benchmark);
			
			SelectorByAverageRawOpenPrice byPrice =
				new SelectorByAverageRawOpenPrice(quotedAtEachMarketDayFromMostLiquid.GetTableOfSelectedTickers(),
				                                  false,currentDate.AddDays(-30),
				                                  currentDate,
				                                  this.numberOfEligibleTickers,
				                                  20);
			DataTable tickersByPrice = byPrice.GetTableOfSelectedTickers();
			
			SelectorByOpenCloseCorrelationToBenchmark tickersLessCorrelatedToBenchmark =
				new SelectorByOpenCloseCorrelationToBenchmark(tickersByPrice,
				                                              "^GSPC",true,
				                                              currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
				                                              tickersByPrice.Rows.Count/2);
			
			return tickersLessCorrelatedToBenchmark.GetTableOfSelectedTickers();

		}
		
		protected virtual void setTickers(DateTime currentDate,
		                                  bool setGenomeCounter)
		{
			
			DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
			if(setOfTickersToBeOptimized.Rows.Count > this.numberOfTickersToBeChosen*2)
				//the optimization process is possible only if the initial set of tickers is
				//as large as the number of tickers to be chosen
				
			{
				IGenomeManager genManEfficientOTCPortfolio =
					new GenomeManagerForEfficientOTCCTOPortfolio(setOfTickersToBeOptimized,
					                                             currentDate.AddDays(-this.numDaysForOptimizationPeriod),
					                                             currentDate,
					                                             this.numberOfTickersToBeChosen,
					                                             this.targetReturn,
					                                             this.portfolioType, this.benchmark);

				GeneticOptimizer GO = new GeneticOptimizer(genManEfficientOTCPortfolio,
				                                           this.populationSizeForGeneticOptimizer,
				                                           this.generationNumberForGeneticOptimizer,
				                                           this.seedForRandomGenerator);
				if(setGenomeCounter)
					this.genomeCounter = new GenomeCounter(GO);
				GO.CrossoverRate = 0.0;
				GO.MutationRate = 0.70;
				GO.Run(false);
				this.addGenomeToBestGenomes(GO.BestGenome,currentDate.AddDays(-this.numDaysForOptimizationPeriod),
				                            currentDate, setOfTickersToBeOptimized.Rows.Count);
				this.chosenWeightedPositions = new WeightedPositions( ((GenomeMeaning)GO.BestGenome.Meaning).TickersPortfolioWeights,
				                                                     new SignedTickers( ((GenomeMeaning)GO.BestGenome.Meaning).Tickers) );
			}
			//else it will be buyed again the previous optimized portfolio
			//that's it the actual chosenTickers member
		}

		protected void oneHourAfterMarketCloseEventHandler_updatePrices()
		{
			//min price for minimizing commission amount
			//according to IB Broker's commission scheme
			this.minPriceForMinimumCommission = this.account.CashAmount/(this.numberOfTickersToBeChosen*100);
			this.maxPriceForMinimumCommission = this.maxPriceForMinimumCommission;
			//just to avoid warning message
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
			//this.oneHourAfterMarketCloseEventHandler_updatePrices();
			if( (this.numDaysElapsedSinceLastOptimization ==
			     this.numDaysBetweenEachOptimization - 1) ||
			   this.account.Portfolio.Count == 0           )
				//it is time to optimize again or
				//portfolio is empty
			{
				this.setTickers( dateTime , false);
				//sets tickers to be chosen next Market Open event
				this.numDaysElapsedSinceLastOptimization = 0;
			}
			else
			{
				this.numDaysElapsedSinceLastOptimization++;
			}
			
		}
		
		#endregion oneHourAfterMarketCloseEventHandler
		
	}
}
