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
                                                 int numDaysOfPortfolioLife,
                                                 double targetPerformance)
                                :base(setOfInitialTickers,
                                     firstQuoteDate,
                                     lastQuoteDate,
                                     numberOfTickersInPortfolio,
                                     numDaysOfPortfolioLife,
                                     targetPerformance)
                          
    {
      
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
      Quotes tickerQuotes = new Quotes(ticker, this.firstQuoteDate, this.lastQuoteDate);
      returnValue = ExtendedDataTable.GetArrayOfFloatFromRatioOfColumns(tickerQuotes, "quClose", "quOpen");
      for(int idx = 0; idx!= returnValue.Length; idx++)
      {
        returnValue[idx] = returnValue[idx] - 1;
      }
      return returnValue; 
    }

    public override double GetFitnessValue(Genome genome)
    {
      double returnValue;
      double portfolioVariance = this.getPortfolioVariance(genome.Genes());
      double portfolioRateOfReturn = this.getPortfolioRateOfReturn(genome.Genes());
      this.variance = portfolioVariance;
      this.rateOfReturn = portfolioRateOfReturn;
      
      NormalDistribution normal = new NormalDistribution(portfolioRateOfReturn, Math.Sqrt(portfolioVariance));
      //for long portfolio
      returnValue = normal.GetProbability(this.targetPerformance*0.75,this.targetPerformance*1.25);
      //for short portfolio
      //returnValue = normal.GetProbability(-this.targetPerformance*1.25,-this.targetPerformance*0.75);
      return returnValue;
    }
    
  }

}
