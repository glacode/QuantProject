/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerCTCTest.cs
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
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.ADT.Optimizing.Genetic;

namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	
  /// <summary>
  /// Implements MarketOpenEventHandler and MarketCloseEventHandler
  /// These handlers contain the core strategy for the efficient close to close
  /// portfolio (with a given days of life)!
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerCTCTest : EndOfDayTimerHandlerCTC
  {
   
    private static bool optimized;

    public EndOfDayTimerHandlerCTCTest(string tickerGroupID, int numberOfEligibleTickers, 
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
                                Account account,                                
                                int generationNumberForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string benchmark,
                                int numDaysOfPortfolioLife,
                                int numDaysForReturnCalculation,
                                double targetReturn,
                               	PortfolioType portfolioType, double maxAcceptableCloseToCloseDrawdown):
    														base(tickerGroupID, numberOfEligibleTickers, 
                                numberOfTickersToBeChosen, numDaysForOptimizationPeriod,
                                account,                                
                                generationNumberForGeneticOptimizer,
                                populationSizeForGeneticOptimizer,
                                benchmark,
                                numDaysOfPortfolioLife,
                                numDaysForReturnCalculation,
                                targetReturn,
                               	portfolioType, maxAcceptableCloseToCloseDrawdown)
    {
     
    }
		    
    protected override void setTickers(DateTime currentDate,
                                        bool setGenomeCounter)
    {
      //setGenomeCounter never used; it is necessary for overriding 
      if(!EndOfDayTimerHandlerCTCTest.optimized)
      {
      	base.setTickers(currentDate.AddDays(this.numDaysForOptimizationPeriod),
                        true);
      	EndOfDayTimerHandlerCTCTest.optimized = true;
      }
    }

    public void Reset()
    {
      EndOfDayTimerHandlerCTCTest.optimized = false;
    }
  
  }

}
