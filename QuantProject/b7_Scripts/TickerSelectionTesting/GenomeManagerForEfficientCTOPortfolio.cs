/*
QuantProject - Quantitative Finance Library

GenomeManagerForEfficientCTOPortfolio.cs
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
	/// portfolios based on tickers' CloseToOpen rates, using the
	/// GeneticOptimizer
	/// </summary>
  public class GenomeManagerForEfficientCTOPortfolio : GenomeManagerForEfficientPortfolio
  {
    
    public GenomeManagerForEfficientCTOPortfolio(DataTable setOfInitialTickers,
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
    
    
    public override object Decode(Genome genome)
    {
      
      string[] arrayOfTickers = new string[genome.Genes().Length];
      int indexOfTicker;
      for(int index = 0; index < genome.Genes().Length; index++)
      {
        indexOfTicker = (int)genome.Genes().GetValue(index);
        arrayOfTickers[index] = (string)this.setOfTickers.Rows[indexOfTicker][0];
      }
      return arrayOfTickers;
      
      /*old implementation, to be used for output to console
      string sequenceOfTickers = ""; 
      object returnValue;
      foreach(int index in genome.Genes())
      {
        sequenceOfTickers += (string)this.setOfTickers.Rows[index][0] + ";" ;
      }
      returnValue = sequenceOfTickers;
      returnValue += "(rate: " + this.RateOfReturn + " std: " +
                        System.Math.Sqrt(this.Variance) + ")";
      return returnValue;
      */
    }
  
    protected override float[] getArrayOfRatesOfReturn(string ticker)
    {
      float[] returnValue;
      float coefficient = this.getCoefficient(ticker);
      string tickerCode = GenomeManagerForEfficientPortfolio.GetCleanTickerCode(ticker);
      Quotes tickerQuotes = new Quotes(tickerCode, this.firstQuoteDate, this.lastQuoteDate);
      returnValue = ExtendedDataTable.GetArrayOfFloatFromRatioOfColumns(tickerQuotes, "quClose", "quOpen");
      
      for(int idx = 0; idx!= returnValue.Length; idx++)
      {
      	returnValue[idx] = coefficient*(returnValue[idx] - 1);
      }
      return returnValue; 
    }
    /*
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
  }

}
