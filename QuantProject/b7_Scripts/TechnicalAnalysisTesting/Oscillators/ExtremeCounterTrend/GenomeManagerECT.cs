/*
QuantProject - Quantitative Finance Library

GenomeManagerECT.cs
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

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.ExtremeCounterTrend
{
	/// <summary>
	/// Implements what needed to use the Genetic Optimizer
	/// for finding the portfolio that best suites
	/// the extreme counter trend strategy
	/// </summary>
	[Serializable]
  public class GenomeManagerECT : GenomeManagerForEfficientPortfolio
  {
    private int numDaysForReturnCalculation;
    private ReturnsManager returnsManager;
    
    public GenomeManagerECT(DataTable setOfInitialTickers,
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
      this.setReturnsManager();
    }
    
    private void setReturnsManager()
    {
			DateTime firstDateTime =
				HistoricalEndOfDayTimer.GetMarketClose( firstQuoteDate );
//				new EndOfDayDateTime(firstQuoteDate, EndOfDaySpecificTime.MarketClose);
			DateTime lastDateTime =
				HistoricalEndOfDayTimer.GetMarketClose( lastQuoteDate );
//				new EndOfDayDateTime(lastQuoteDate, EndOfDaySpecificTime.MarketClose);
			this.returnsManager =
				 new ReturnsManager( new CloseToCloseIntervals(
																 firstDateTime, 
																 lastDateTime, 
																 this.benchmark,
																 this.numDaysForReturnCalculation) ,
														 new HistoricalAdjustedQuoteProvider() );
    }
    
    private float[] getStrategyReturns_getReturnsActually(
    									float[] plainReturns)
		{
			float[] returnValue = new float[plainReturns.Length];
			returnValue[0] = 0; //a the very first day the
			//first strategy return is equal to 0 because no position
			//has been entered
			float coefficient = 0;
    	for(int i = 0; i < returnValue.Length - 1; i++)
      {
    		if( plainReturns[i] > 0 )
    		//portfolio is overbought
    			coefficient = -1;
    		else if( plainReturns[i] <= 0 )
 				//portfolio is oversold   			
        	coefficient = 1;
    		
    		returnValue[i + 1] = coefficient * plainReturns[i + 1];
      }
      return returnValue;
		}
    
    protected override float[] getStrategyReturns()
		{
			DateTime firstDateTime =
				HistoricalEndOfDayTimer.GetMarketClose( firstQuoteDate );
//				new EndOfDayDateTime(firstQuoteDate, EndOfDaySpecificTime.MarketClose);
			DateTime lastDateTime =
				HistoricalEndOfDayTimer.GetMarketClose( lastQuoteDate );
//				new EndOfDayDateTime(lastQuoteDate, EndOfDaySpecificTime.MarketClose);
	  	float[] plainReturns = this.weightedPositionsFromGenome.GetReturns(
              							 this.returnsManager);
			return this.getStrategyReturns_getReturnsActually(plainReturns);
		}
  }
}
