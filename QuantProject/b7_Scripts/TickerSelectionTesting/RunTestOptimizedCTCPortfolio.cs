/*
QuantProject - Quantitative Finance Library

RunTestOptimizedCTCPortfolio.cs
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
	/// Script to buy at close and sell at close
	/// after a specified number of market days  
	/// the efficient portfolio
	/// The efficient portfolio's generation rules
	/// (contained in the EndOfDayTimerHandler) are:
	/// - choose the most liquid tickers;
	/// - choose only tickers quoted at each market day
	///   during a given previous interval of days;
	/// - choose the most efficient portfolio among these tickers
	/// </summary>
	[Serializable]
	public class RunTestOptimizedCTCPortfolio : RunEfficientCTCPorfolio
	{
   
    public RunTestOptimizedCTCPortfolio(string tickerGroupID, int numberOfEligibleTickers, 
                                    int numberOfTickersToBeChosen, int numDaysForLiquidity, 
                                    int generationNumberForGeneticOptimizer,
                                    int populationSizeForGeneticOptimizer, string benchmark,
                                    DateTime startDate, DateTime endDate,
                                   	int numDaysOfPortfolioLife, int numDaysForReturnCalculation, 
                                   	double targetReturn,
                                    PortfolioType portfolioType, double maxAcceptableCloseToCloseDrawdown, 
                                    double maxRunningHours):
																base(tickerGroupID, numberOfEligibleTickers, 
                                    numberOfTickersToBeChosen, numDaysForLiquidity, 
                                    generationNumberForGeneticOptimizer,
                                    populationSizeForGeneticOptimizer, benchmark,
                                    startDate, endDate,
                                   	numDaysOfPortfolioLife, numDaysForReturnCalculation, 
                                   	targetReturn,
                                    portfolioType, maxAcceptableCloseToCloseDrawdown, 
                                    maxRunningHours)
		{
      this.ScriptName = "TestOptimizedCTCPortfolio";
      
		}
  
    protected override void run_initializeEndOfDayTimerHandler()
    {
      this.endOfDayTimerHandler = new EndOfDayTimerHandlerCTCTest(this.tickerGroupID, this.numberOfEligibleTickers,
    	                                                        this.numberOfTickersToBeChosen, this.numDaysForLiquidity,
    	                                                        this.account,
    	                                                        this.generationNumberForGeneticOptimizer,
    	                                                        this.populationSizeForGeneticOptimizer, this.benchmark,
    	                                                        this.numDayOfPortfolioLife, 
    	                                                        this.numDaysForReturnCalculation,
    	                                                        this.targetReturn,
    	                                                       	this.portfolioType, this.maxAcceptableCloseToCloseDrawdown);
    }
    
        
    public override void Run()
    {
    	base.Run();
      ((EndOfDayTimerHandlerCTCTest)this.endOfDayTimerHandler).Reset();
    }
   
	}
}
