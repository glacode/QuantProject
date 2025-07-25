/*
QuantProject - Quantitative Finance Library

RunEfficientCTOPorfolio.cs
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
using QuantProject.ADT.FileManaging;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Testing;
using QuantProject.Business.Timing;
using QuantProject.Business.Financial.Accounting.Commissions;
using QuantProject.Business.Financial.Accounting.Slippage;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Presentation.Reporting.WindowsForm;


namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	/// <summary>
	/// Script to buy at open and sell at close
	/// the efficient close to open daily portfolio
	/// </summary>
	[Serializable]
	public class RunEfficientCTOPortfolio : RunEfficientPortfolio
	{
		protected int numDaysBetweenEachOptimization;
		public RunEfficientCTOPortfolio(string tickerGroupID, int numberOfEligibleTickers,
		                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
		                                int generationNumberForGeneticOptimizer,
		                                int populationSizeForGeneticOptimizer, string benchmark,
		                                DateTime startDate, DateTime endDate, double targetReturn,
		                                PortfolioType portfolioType, double maxRunningHours,
		                                int numDaysBetweenEachOptimization):
			base(tickerGroupID, numberOfEligibleTickers,
			     numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
			     generationNumberForGeneticOptimizer,
			     populationSizeForGeneticOptimizer, benchmark,
			     startDate, endDate, targetReturn,
			     portfolioType, maxRunningHours)
		{
			this.ScriptName = "CloseToOpenScriptsNoCoeffSharpeRatioCombined";
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
		}
		
		
		// delete remark delimitations for having ib commission
		// and a fixed percentage calculation of slippage
		protected override void run_initializeAccount()
		{
			this.account = new Account(this.ScriptName,
			                           this.endOfDayTimer ,
			                           new HistoricalDataStreamer(this.endOfDayTimer ,
			                                                              this.historicalMarketValueProvider ) ,
			                           new HistoricalOrderExecutor(this.endOfDayTimer ,
			                                                               this.historicalMarketValueProvider));
			//, new FixedPercentageSlippageManager(this.historicalQuoteProvider,
			// this.endOfDayTimer,0.08)),
			//new IBCommissionManager());
			
		}
		
		
		protected override void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler = new EndOfDayTimerHandlerCTO(this.tickerGroupID,
			                                                        this.numberOfEligibleTickers,
			                                                        this.numberOfTickersToBeChosen,
			                                                        this.numDaysForOptimizationPeriod,
			                                                        this.account,
			                                                        this.generationNumberForGeneticOptimizer,
			                                                        this.populationSizeForGeneticOptimizer,
			                                                        this.benchmark,
			                                                        this.targetReturn,
			                                                        this.portfolioType,
			                                                        this.numDaysBetweenEachOptimization);
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
			
//			this.endOfDayTimer.MarketOpen +=
			//        new MarketOpenEventHandler(
			//        this.endOfDayTimerHandler.MarketOpenEventHandler);
//
			//      this.endOfDayTimer.MarketClose +=
			//        new MarketCloseEventHandler(
			//        this.endOfDayTimerHandler.MarketCloseEventHandler);
//
			//      this.endOfDayTimer.MarketClose +=
			//        new MarketCloseEventHandler(
			//        this.checkDateForReport);
//
			//      this.endOfDayTimer.OneHourAfterMarketClose +=
			//        new OneHourAfterMarketCloseEventHandler(
			//        this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler );
		}
		
		
		
		//necessary far calling RunEfficientPortfolio.Run()
		//in classes that inherit from this class
		public override void Run()
		{
			base.Run();
		}
	}
}
