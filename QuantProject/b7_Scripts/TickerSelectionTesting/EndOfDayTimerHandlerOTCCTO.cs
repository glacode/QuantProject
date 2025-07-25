/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerOTCCTO.cs
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
	/// Implements MarketOpenEventHandler,
	/// TwoMinutesBeforeMarketCloseEventHandler and OneHourAfterMarketCloseEventHandler
	/// These handlers contain the core strategy for the efficient close to open portfolio!
	/// </summary>
	[Serializable]
	public class EndOfDayTimerHandlerOTCCTO : EndOfDayTimerHandler
	{
		protected int seedForRandomGenerator;
		
		public EndOfDayTimerHandlerOTCCTO(string tickerGroupID, int numberOfEligibleTickers,
		                                  int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod, Account account,
		                                  int generationNumberForGeneticOptimizer,
		                                  int populationSizeForGeneticOptimizer,
		                                  string benchmark, double targetReturn,
		                                  PortfolioType portfolioType, int numDaysBetweenEachOptimization):
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
		}
		
		
		/// <summary>
		/// Handles a "Market Open" event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		protected override void marketOpenEventHandler(
			Object sender , DateTime dateTime )
		{
			//temporarily the if condition
			//if(this.numDaysElapsedSinceLastOptimization == 0)
			AccountManager.ClosePositions(this.account);
			this.openPositions();
		}
		
		protected override void marketCloseEventHandler(
			Object sender , DateTime dateTime )
		{
			
			//temporarily
			//if(this.numDaysElapsedSinceLastOptimization ==
			//   this.numDaysBetweenEachOptimization)
			AccountManager.ClosePositions(this.account);
			try{
				this.chosenWeightedPositions.ReverseSigns();
				this.openPositions();
			}
			catch(Exception ex)
			{
				string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
			}
			this.chosenWeightedPositions.ReverseSigns();
		}
		
		

		#region oneHourAfterMarketCloseEventHandler
		
		protected DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
		{
			/*
      SelectorByAverageRawOpenPrice selectorByOpenPrice =
                  new SelectorByAverageRawOpenPrice(this.tickerGroupID, false,
                          currentDate.AddDays(-this.numDaysForLiquidity), currentDate,
                          this.numberOfEligibleTickers, this.minPriceForMinimumCommission,
                          this.maxPriceForMinimumCommission, 0, 2);
      DataTable tickersByPrice = selectorByOpenPrice.GetTableOfSelectedTickers();
			 */
			
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
			
			return quotedAtEachMarketDayFromMostLiquid.GetTableOfSelectedTickers();
			
		}
		
		protected virtual void setTickers(DateTime currentDate,
		                                  bool setGenomeCounter)
		{
			
			DataTable setOfTickersToBeOptimized = this.getSetOfTickersToBeOptimized(currentDate);
			if(setOfTickersToBeOptimized.Rows.Count > this.numberOfTickersToBeChosen*2)
				//the optimization process is possible only if the initial set of tickers is
				//as large as the number of tickers to be chosen
				
			{
				IGenomeManager genManEfficientOTCCTOPortfolio =
					new GenomeManagerForEfficientOTCCTOPortfolio(setOfTickersToBeOptimized,
					                                             currentDate.AddDays(-this.numDaysForOptimizationPeriod),
					                                             currentDate,
					                                             this.numberOfTickersToBeChosen,
					                                             this.targetReturn,
					                                             this.portfolioType,
					                                             this.benchmark);
				
				GeneticOptimizer GO = new GeneticOptimizer(genManEfficientOTCCTOPortfolio,
				                                           this.populationSizeForGeneticOptimizer,
				                                           this.generationNumberForGeneticOptimizer,
				                                           this.seedForRandomGenerator);
				if(setGenomeCounter)
					this.genomeCounter = new GenomeCounter(GO);
				
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
			if(this.numDaysElapsedSinceLastOptimization ==
			   this.numDaysBetweenEachOptimization - 1)
			{
				this.setTickers(dateTime, false);
				//sets tickers to be chosen next Market Open event
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
