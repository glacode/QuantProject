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
  public class GenomeManagerForEfficientCTCPortfolio : GenomeManagerForEfficientPortfolio
  {
    private int numDaysOfPortfolioLife;
    private int numDaysForReturnCalculation;
    
    public GenomeManagerForEfficientCTCPortfolio(DataTable setOfInitialTickers,
                                                 DateTime firstQuoteDate,
                                                 DateTime lastQuoteDate,
                                                 int numberOfTickersInPortfolio,
                                                 int numDaysOfPortfolioLife,
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
      this.numDaysOfPortfolioLife = numDaysOfPortfolioLife;
      this.numDaysForReturnCalculation = numDaysForReturnCalculation;
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
      
    }
  
    protected override float[] getArrayOfRatesOfReturn(string ticker)
    {
      float coefficient = this.getCoefficient(ticker);
      string tickerCode = GenomeManagerForEfficientPortfolio.GetCleanTickerCode(ticker);
    	Quotes tickerQuotes = new Quotes(tickerCode, this.firstQuoteDate, this.lastQuoteDate);
      float[] allAdjValues = ExtendedDataTable.GetArrayOfFloatFromColumn(tickerQuotes, "quAdjustedClose");
      float[] ratesOfReturns = new float[allAdjValues.Length/this.numDaysForReturnCalculation + 1];
      int i = 0; //index for ratesOfReturns array
      
      for(int idx = 0; idx + this.numDaysForReturnCalculation < allAdjValues.Length; idx += this.numDaysForReturnCalculation )
      {
        ratesOfReturns[i] = (allAdjValues[idx+this.numDaysForReturnCalculation]/
      	                     allAdjValues[idx] - 1)*coefficient;
        i++;
      }

      return ratesOfReturns;
    }
    
  }

}
