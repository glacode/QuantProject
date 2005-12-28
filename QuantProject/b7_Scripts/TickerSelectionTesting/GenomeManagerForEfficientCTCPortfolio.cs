/*
QuantProject - Quantitative Finance Library

GenomeManagerForEfficientCTCPortfolio.cs
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


namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	/// <summary>
	/// Class to find efficient 
	/// portfolios based on tickers' CloseToClose rates (adjusted values),
	/// using the GeneticOptimizer
	/// </summary>
	[Serializable]
  public class GenomeManagerForEfficientCTCPortfolio : GenomeManagerForEfficientPortfolio
  {
    private int numDaysForReturnCalculation;
    private double shiftedPortfolioRateOfReturn;
    private float[] shiftedPortfolioRatesOfReturn;
    //private double shiftedPortfolioVariance;
    //rate of return and variance of portfolio 
    //shifted ahead of numDaysForReturnCalculation
    
    /// <summary>
    /// Rates of returns of the portfolio shifted ahead of numDaysForReturnCalculation
    /// </summary>
    public float[] ShiftedPortfolioRatesOfReturn
    {
      get{return this.shiftedPortfolioRatesOfReturn;}
    }

    public GenomeManagerForEfficientCTCPortfolio(DataTable setOfInitialTickers,
                                                 DateTime firstQuoteDate,
                                                 DateTime lastQuoteDate,
                                                 int numberOfTickersInPortfolio,
                                                 int numDaysForReturnCalculation,
                                                 double targetPerformance,
                                                 PortfolioType portfolioType)
                                                 :
                                                base(setOfInitialTickers,
                                                firstQuoteDate,
                                                lastQuoteDate,
                                                numberOfTickersInPortfolio,
                                                targetPerformance,
                                                portfolioType)
                                
                          
    {
      this.numDaysForReturnCalculation = numDaysForReturnCalculation;
      this.retrieveData();
    }
// old implementation, where a "continuos" adjusted close to close ratio,
// based on a particular fixed interval of days, is considered
// In this case, there is no discontinuity between the returned ratesOfReturn
//
//    protected override float[] getArrayOfRatesOfReturn(string ticker)
//    {
//      float[] returnValue = null;
//      Quotes tickerQuotes = new Quotes(ticker, this.firstQuoteDate, this.lastQuoteDate);
//      float[] allAdjValues = ExtendedDataTable.GetArrayOfFloatFromColumn(tickerQuotes, "quAdjustedClose");
//      returnValue = new float[allAdjValues.Length/this.numDaysForReturnCalculation + 1];
//      int i = 0; //index for ratesOfReturns array
//	    for(int idx = 0; idx + this.numDaysForReturnCalculation < allAdjValues.Length; idx += this.numDaysForReturnCalculation )
//	    {
//	      returnValue[i] = (allAdjValues[idx+this.numDaysForReturnCalculation]/
//	      	                    allAdjValues[idx] - 1);
//	      i++;
//	    }	
//      this.numberOfExaminedReturns = returnValue.Length;
//      
//      return returnValue;
//    }
    
    // new implementation, where a "discontinuos" adjusted close to close ratio,
    // based on a particular fixed interval of days, is considered
    // In this case, there is a discontinuity between each pair of ratesOfReturn,
    // equal to the given interval of days
    protected override float[] getArrayOfRatesOfReturn(string ticker)
    {
      this.calculateShiftedRateOfReturn(ticker);

      float[] returnValue = null;
      returnValue = 
      	QuantProject.Data.DataTables.Quotes.GetArrayOfCloseToCloseRatios(ticker,
                                                            this.firstQuoteDate,
                                                            this.lastQuoteDate,
                                                            this.numDaysForReturnCalculation);
      	
      this.numberOfExaminedReturns = returnValue.Length;
      
      return returnValue;
                                                                        
    }
    
    /*LPM as fitness
    protected override double getFitnessValue_calculate()
    {
      double returnValue = 0;                                            
      
      double a, b, c;
      a = 0.002; b = 2.0; c = 2.0;
      
      //returnValue = Math.Pow((a/this.Variance),b) *
      //                 Math.Pow((this.rateOfReturn - this.targetPerformance),
      //                          c);
      //this.lowerPartialMoment = AdvancedFunctions.LowerPartialMoment(this.portfolioRatesOfReturn,
      //                                                      BasicFunctions.SimpleAverage(this.portfolioRatesOfReturn),
      //                                                      3.0);
      this.lowerPartialMoment = AdvancedFunctions.NegativeSemiVariance(this.portfolioRatesOfReturn);
      a = 1.0;
      returnValue = Math.Pow((a/this.lowerPartialMoment),b) *
        Math.Pow((this.rateOfReturn - this.targetPerformance),
        c);
      
      if(this.portfolioType == PortfolioType.OnlyShort)
        returnValue = - returnValue; 
      
      if(Double.IsInfinity(returnValue) || Double.IsNaN(returnValue))
        throw new Exception("Fitness value not computed correctly!");
      
      return returnValue;
    }
		*/
    private void calculateShiftedRateOfReturn(string ticker)
    {
      try
      {
        float[] closeToCloseRatios = Quotes.GetArrayOfCloseToCloseRatios(ticker, this.firstQuoteDate,
                                        this.lastQuoteDate,
                                        this.numDaysForReturnCalculation,
                                        this.numDaysForReturnCalculation);
        this.shiftedPortfolioRatesOfReturn = closeToCloseRatios;
        this.shiftedPortfolioRateOfReturn =
            BasicFunctions.SimpleAverage(closeToCloseRatios);
        //this.shiftedPortfolioVariance = 
          //BasicFunctions.Variance(closeToCloseRatios);
      }
      catch(Exception ex)
      {
        ex = ex;
      }
    }
		
		protected override double getFitnessValue_calculate()
    {
			return (this.RateOfReturn/Math.Sqrt(this.Variance))*
              -this.shiftedPortfolioRateOfReturn * 
              -this.ShiftedPortfolioRatesOfReturn[this.ShiftedPortfolioRatesOfReturn.Length -1];
    }
		
  }

}
