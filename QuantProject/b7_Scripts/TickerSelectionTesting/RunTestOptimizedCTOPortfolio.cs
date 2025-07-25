/*
QuantProject - Quantitative Finance Library

RunTestOptimizedCTOPorfolio.cs
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
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors; 
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Presentation.Reporting.WindowsForm;
using QuantProject.Scripts.WalkForwardTesting.LinearCombination;


namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	/// <summary>
	/// Script to test optimization process
	/// in choosing the best open/close portfolio
	/// </summary>
	[Serializable]
  public class RunTestOptimizedCTOPortfolio : RunEfficientCTOPortfolio
	{
   
    public RunTestOptimizedCTOPortfolio(string tickerGroupID, int numberOfEligibleTickers, 
                                    int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod, 
                                    int generationNumberForGeneticOptimizer,
                                    int populationSizeForGeneticOptimizer, string benchmark,
                                    DateTime endDate, double targetReturn,
                                    PortfolioType portfolioType, double maxRunningHours, 
                                   	int numDaysBetweenEachOptimization):
  																base(tickerGroupID, numberOfEligibleTickers, 
                                     numberOfTickersToBeChosen, numDaysForOptimizationPeriod, 
                                    generationNumberForGeneticOptimizer,
                                    populationSizeForGeneticOptimizer, benchmark,
                                    endDate.AddDays(-numDaysForOptimizationPeriod), endDate, targetReturn,
                                   	portfolioType, maxRunningHours,
                                   	numDaysBetweenEachOptimization)
		{
      //this.ScriptName = "TestOptimizedCTOPortfolio";
      this.ScriptName = "TestOptimizedCTOWeightedPortfolio";
    }
  	
  	protected override void run_initializeEndOfDayTimerHandler()
    {
      this.endOfDayTimerHandler = new EndOfDayTimerHandlerCTOTest(this.tickerGroupID,
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
  	
  	public override void Run()
    {
  		base.Run();
      ((EndOfDayTimerHandlerCTOTest)this.endOfDayTimerHandler).Reset();
      
      Report report = new Report( this.account , this.historicalQuoteProvider );
      report.Create( "TestOptimizationOpenToCloseEfficientPortfolio", 1 ,
        new EndOfDayDateTime( this.endDateTime.DateTime ,
        EndOfDaySpecificTime.MarketClose ) ,
        this.benchmark );
      report.Show();
      report.Text = this.getGenomeCounterInfo();
    }
     
    protected override void checkDateForReport(Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs)
    {
      if(endOfDayTimingEventArgs.EndOfDayDateTime.DateTime>=this.endDateTime.DateTime ||
        DateTime.Now >= this.startingTimeForScript.AddHours(this.maxRunningHours))
        //last date is reached by the timer or maxRunning hours
        //are elapsed from the time script started
      {
        this.endOfDayTimer.Stop();
      }

    }
    
    

        
	}
}
