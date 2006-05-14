/*
QuantProject - Quantitative Finance Library

GenomeManagerForEfficientOTCPortfolio.cs
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
	/// This class implements IGenomeManager, in order to find efficient 
	/// portfolios based on tickers' OpenToClose rates, using the
	/// GeneticOptimizer
	/// </summary>
	[Serializable]
  public class GenomeManagerForEfficientOTCPortfolio : GenomeManagerForEfficientPortfolio
  {
    
    public GenomeManagerForEfficientOTCPortfolio(DataTable setOfInitialTickers,
                                                 DateTime firstQuoteDate,
                                                 DateTime lastQuoteDate,
                                                 int numberOfTickersInPortfolio,
                                                 double targetPerformance,
                                                 PortfolioType portfolioType)
                                :base(setOfInitialTickers,
                                     firstQuoteDate,
                                     lastQuoteDate,
                                     numberOfTickersInPortfolio,
                                     targetPerformance,
                                     portfolioType)
                          
    {
      this.retrieveData();
    }
    
    protected override float[] getArrayOfRatesOfReturn(string ticker)
    {
      float[] returnValue = null;
      Quotes tickerQuotes = new Quotes(ticker, this.firstQuoteDate, this.lastQuoteDate);
      returnValue = ExtendedDataTable.GetRatesOfReturnsFromColumns(tickerQuotes, "quClose", "quOpen");
      this.numberOfExaminedReturns = returnValue.Length;
      
      return returnValue;
    }
    /*using LPM
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
                       Math.Pow(Math.Max(0.0,(this.rateOfReturn - this.targetPerformance)),
                                c);
      
      if(this.portfolioType == PortfolioType.OnlyShort)
        returnValue = - returnValue; 
      
      if(Double.IsInfinity(returnValue) || Double.IsNaN(returnValue))
      		throw new Exception("Fitness value not computed correctly!");
      
      return returnValue;
    }
    */
    
    protected override double getFitnessValue_calculate()
    {
      return this.RateOfReturn/Math.Sqrt(this.Variance);
    }
    
    
  }

}
