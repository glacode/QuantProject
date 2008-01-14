/*
QuantProject - Quantitative Finance Library

GenomeManagerForFPOscillatorCTC.cs
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

using QuantProject.ADT.Statistics;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Data;
using QuantProject.Data.DataTables;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedPeriodOscillators
{
	/// <summary>
	/// Implements what needed to use the Genetic Optimizer
	/// for finding the portfolio that best suites
	/// the fixed period Oscillator strategy
	/// </summary>
	[Serializable]
  public class GenomeManagerForFPOscillatorCTC : GenomeManagerForEfficientPortfolio
  {
    private int numDaysForReturnCalculation;
    private ReturnsManager returnsManager;
    
    public GenomeManagerForFPOscillatorCTC(DataTable setOfInitialTickers,
                                                 DateTime firstQuoteDate,
                                                 DateTime lastQuoteDate,
                                                 int numberOfTickersInPortfolio,
                                                 int numDaysForReturnCalculation,
                                                 PortfolioType portfolioType,
                                                 string benchmark)
                                                 :
                                                base(setOfInitialTickers,
                                                firstQuoteDate,
                                                lastQuoteDate,
                                                numberOfTickersInPortfolio,
                                                0.0,
                                                portfolioType,
                                               	benchmark)
                                
                          
    {
      this.numDaysForReturnCalculation = numDaysForReturnCalculation;
      this.setReturnsManager(firstQuoteDate , lastQuoteDate);
    }
    
    private void setReturnsManager(DateTime firstQuoteDate,
                                   DateTime lastQuoteDate)
    {
    	EndOfDayDateTime firstEndOfDayDateTime =
				new EndOfDayDateTime(firstQuoteDate, EndOfDaySpecificTime.MarketOpen);
			EndOfDayDateTime lastEndOfDayDateTime =
				new EndOfDayDateTime(lastQuoteDate, EndOfDaySpecificTime.MarketClose);
    	this.returnsManager = 
    		new ReturnsManager( new CloseToCloseIntervals(
															  firstEndOfDayDateTime, 
																lastEndOfDayDateTime, 
																this.benchmark,
															  this.numDaysForReturnCalculation),
														new HistoricalAdjustedQuoteProvider() );
    }
    
    private float[] getStrategyReturns_getReturnsActually(
    									float[] plainReturns)
		{
			float[] returnValue = new float[plainReturns.Length];
    	for(int i = 0; i < returnValue.Length; i++)
      {
    		if( i%2 == 0 )
    		//even periods are expected to have the same sign as plain ctc returns
    			returnValue[i] = plainReturns[i];
    		else//odd periods are reversed
    			returnValue[i] = - plainReturns[i];
      }
      return returnValue;
		}
    
    protected override float[] getStrategyReturns()
		{
			float[] plainReturns = 
				this.weightedPositionsFromGenome.GetReturns(
        this.returnsManager);
			return this.getStrategyReturns_getReturnsActually(plainReturns);
		}
  }  
}
