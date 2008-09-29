/*
QuantProject - Quantitative Finance Library

RunEfficientOTC_CTOTrendFollower.cs
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


namespace QuantProject.Scripts.TechnicalAnalysisTesting.TrendFollowing.OTC_CTOTrendFollower
{
	/// <summary>
	/// Script that aims to anticipate the growing trend
	/// of a portfolio using the OTC_CTO indicator
	/// In other words: if a portfolio, in sample, has a
	/// a good performance with the OTC_CTO strategy, then it should
	/// gain also (we hope ...) out of sample
	/// </summary>
	[Serializable]
	public class RunEfficientOTC_CTOTrendFollower : RunEfficientPortfolio
	{
		protected int numDaysBetweenEachOptimization;
		private double stopLossLevel;
		
		public RunEfficientOTC_CTOTrendFollower(string tickerGroupID, int numberOfEligibleTickers,
		                                        int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
		                                        int generationNumberForGeneticOptimizer,
		                                        int populationSizeForGeneticOptimizer, string benchmark,
		                                        DateTime startDate, DateTime endDate, double targetReturn,
		                                        double stopLossLevel, PortfolioType portfolioType, double maxRunningHours,
		                                        int numDaysBetweenEachOptimization):
			base(tickerGroupID, numberOfEligibleTickers,
			     numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
			     generationNumberForGeneticOptimizer,
			     populationSizeForGeneticOptimizer, benchmark,
			     startDate, endDate, targetReturn,
			     portfolioType, maxRunningHours)
		{
			//this.ScriptName = "OTC_CTOTrendFollowerSharpeRatioNoCoeff";
			this.ScriptName = "OTC_CTOTrendFollowerSharpeRatioWithCoeffOnlyMutation";
			this.numDaysBetweenEachOptimization = numDaysBetweenEachOptimization;
			this.stopLossLevel = stopLossLevel;
		}
		
		
		// delete remark delimitations for having ib commission
		// and a fixed percentage calculation of slippage
		protected override void run_initializeAccount()
		{
			this.account = new Account(this.ScriptName,
			                           this.endOfDayTimer ,
			                           new HistoricalEndOfDayDataStreamer(this.endOfDayTimer ,
			                                                              this.historicalMarketValueProvider ) ,
			                           new HistoricalEndOfDayOrderExecutor(this.endOfDayTimer ,
			                                                               this.historicalMarketValueProvider)//,
			                           // new FixedPercentageSlippageManager(this.historicalQuoteProvider,
			                           // this.endOfDayTimer,0.08)),
			                          );
			//new IBCommissionManager());
			
		}
		
		
		protected override void run_initializeEndOfDayTimerHandler()
		{
			this.endOfDayTimerHandler = new EndOfDayTimerHandlerOTC_CTOTrendFollower(this.tickerGroupID,
			                                                                         this.numberOfEligibleTickers,
			                                                                         this.numberOfTickersToBeChosen,
			                                                                         this.numDaysForOptimizationPeriod,
			                                                                         this.account,
			                                                                         this.generationNumberForGeneticOptimizer,
			                                                                         this.populationSizeForGeneticOptimizer,
			                                                                         this.benchmark,
			                                                                         this.targetReturn,
			                                                                         this.stopLossLevel,
			                                                                         this.portfolioType, this.numDaysBetweenEachOptimization);
		}
		
		protected override void run_initializeHistoricalQuoteProvider()
		{
			//this.historicalQuoteProvider = new HistoricalRawQuoteProvider();
			this.historicalMarketValueProvider = new HistoricalAdjustedQuoteProvider();
		}
		
		protected override void run_addEventHandlers()
		{
			
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.endOfDayTimerHandler.NewDateTimeEventHandler );
			this.endOfDayTimer.NewDateTime +=
				new NewDateTimeEventHandler( this.checkDateForReport );

//			this.endOfDayTimer.MarketOpen +=
//				new MarketOpenEventHandler(
//					this.endOfDayTimerHandler.MarketOpenEventHandler);
//			
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
//					this.endOfDayTimerHandler.OneHourAfterMarketCloseEventHandler );
		}
		
		//necessary far calling RunEfficientPortfolio.Run()
		//in classes that inherit from this class
		public override void Run()
		{
			base.Run();
		}
	}
}
