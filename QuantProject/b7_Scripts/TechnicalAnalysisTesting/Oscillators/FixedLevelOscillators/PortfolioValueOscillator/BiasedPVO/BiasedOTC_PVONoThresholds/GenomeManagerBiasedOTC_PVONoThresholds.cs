/*
QuantProject - Quantitative Finance Library

GenomeManagerBiasedOTC_PVONoThresholds.cs
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

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.BiasedPVO.BiasedOTC_PVONoThresholds
{
	/// <summary>
	/// Implements what needed to use the Genetic Optimizer
	/// for finding the portfolio that best suites
	/// the Biased Portfolio Value Oscillator strategy, with no thresholds
	/// </summary>
	[Serializable]
  public class GenomeManagerBiasedOTC_PVONoThresholds : GenomeManagerForEfficientPortfolio
  {
		private ReturnsManager returnsManager;
  	
    public GenomeManagerBiasedOTC_PVONoThresholds(DataTable setOfInitialTickers,
                           DateTime firstQuoteDate,
                           DateTime lastQuoteDate,
                           int numberOfTickersInPortfolio,
                           PortfolioType inSamplePortfolioType,
                           string benchmark)
                           :
                          base(setOfInitialTickers,
                          firstQuoteDate,
                          lastQuoteDate,
                          numberOfTickersInPortfolio,
                          0.0,
                          inSamplePortfolioType,
                          benchmark)
                                
                          
    {
			this.setReturnsManager();
    }
    
		private void setReturnsManager()
		{
			EndOfDayDateTime firstEndOfDayDateTime =
				new EndOfDayDateTime(firstQuoteDate, EndOfDaySpecificTime.MarketClose);
			EndOfDayDateTime lastEndOfDayDateTime =
				new EndOfDayDateTime(lastQuoteDate, EndOfDaySpecificTime.MarketClose);
			this.returnsManager = 
				new ReturnsManager( new OpenToCloseCloseToOpenIntervals(
																firstEndOfDayDateTime, 
																lastEndOfDayDateTime, 
																this.benchmark) ,
														new HistoricalAdjustedQuoteProvider() ); 
		}
		
  	private float[] getStrategyReturns_getReturnsActually(
    									float[] plainReturns)
		{
			float[] returnValue = new float[plainReturns.Length];
			returnValue[0] = 0; //at the very first day the
			//first strategy return is equal to 0 because no position
			//has been entered
			float coefficient = 0;
    	for(int i = 0; i < returnValue.Length - 1; i++)
      {
    		if( plainReturns[i] >= 0 )
    		//portfolio is overbought
    			coefficient = -1;
    		else if( plainReturns[i] <= 0 )
 				//portfolio is oversold   			
        	coefficient = 1;
    		//else 
    		// coefficient = coefficient; the previous coeff is kept
    		returnValue[i + 1] = coefficient * plainReturns[i + 1];
      }
      return returnValue;
		}
    
    protected override float[] getStrategyReturns()
		{
			EndOfDayDateTime firstEndOfDayDateTime =
				new EndOfDayDateTime(firstQuoteDate, EndOfDaySpecificTime.MarketClose);
			EndOfDayDateTime lastEndOfDayDateTime =
				new EndOfDayDateTime(lastQuoteDate, EndOfDaySpecificTime.MarketClose);
	  	float[] plainReturns = this.weightedPositionsFromGenome.GetReturns(
               							 this.returnsManager);
			return this.getStrategyReturns_getReturnsActually(plainReturns);
		}
  }
}
