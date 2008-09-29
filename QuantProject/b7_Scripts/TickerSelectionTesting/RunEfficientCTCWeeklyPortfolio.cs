/*
QuantProject - Quantitative Finance Library

RunEfficientCTCWeeklyPorfolio.cs
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
using System.Collections;
using System.Data;
using QuantProject.ADT;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Testing;
using QuantProject.Business.Timing;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Presentation.Reporting.WindowsForm;
using QuantProject.ADT.FileManaging;



namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	/// <summary>
	/// Script to buy at close on Monday and sell at close
	/// on next Friday (or before, if drawdown crosses a given level)
	/// The efficient portfolio's generation rules are
	/// contained in EndOfDayTimerHandlerCTCWeekly class
	/// </summary>
	[Serializable]
	public class RunEfficientCTCWeeklyPortfolio : RunEfficientPortfolio
	{
		protected int numDaysForReturnCalculation;
		protected double maxAcceptableCloseToCloseDrawdown;
		
		public RunEfficientCTCWeeklyPortfolio(string tickerGroupID, int numberOfEligibleTickers,
		                                      int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
		                                      int generationNumberForGeneticOptimizer,
		                                      int populationSizeForGeneticOptimizer, string benchmark,
		                                      DateTime startDate, DateTime endDate,
		                                      int numDaysForReturnCalculation,
		                                      double targetReturn,
		                                      PortfolioType portfolioType, double maxAcceptableCloseToCloseDrawdown,
		                                      double maxRunningHours):
			base(tickerGroupID, numberOfEligibleTickers,
			     numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
			     generationNumberForGeneticOptimizer,
			     populationSizeForGeneticOptimizer, benchmark,
			     startDate, endDate, targetReturn,
			     portfolioType, maxRunningHours)
		{
			this.ScriptName = "CloseToCloseScriptsWeeklyEfficientPortfolio";
			this.numDaysForReturnCalculation = numDaysForReturnCalculation;
			this.maxAcceptableCloseToCloseDrawdown = maxAcceptableCloseToCloseDrawdown;
		}

		#region auxiliary overriden methods for Run
		
		protected override void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler = new EndOfDayTimerHandlerCTCWeekly(this.tickerGroupID, this.numberOfEligibleTickers,
			                                                              this.numberOfTickersToBeChosen, this.numDaysForOptimizationPeriod,
			                                                              this.account,
			                                                              this.generationNumberForGeneticOptimizer,
			                                                              this.populationSizeForGeneticOptimizer, this.benchmark,
			                                                              this.numDaysForReturnCalculation,
			                                                              this.targetReturn,
			                                                              this.portfolioType, this.maxAcceptableCloseToCloseDrawdown);
		}
		
		protected override void run_initializeHistoricalQuoteProvider()
		{
			this.historicalMarketValueProvider = new HistoricalAdjustedQuoteProvider();
		}
		
		protected override void run_addEventHandlers()
		{
			
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.endOfDayTimerHandler.NewDateTimeEventHandler );
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.checkDateForReport );

//			this.endOfDayTimer.MarketClose +=
//				new MarketCloseEventHandler(
//					this.endOfDayTimerHandler.MarketCloseEventHandler);
//			
//			this.endOfDayTimer.MarketClose +=
//				new MarketCloseEventHandler(
//					this.checkDateForReport);
//			
//			this.endOfDayTimer.OneHourAfterMarketClose +=
//				new OneHourAfterMarketCloseEventHandler(
//					this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler);
		}

		
		#endregion
	}
}
