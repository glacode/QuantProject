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
    
    public GenomeManagerForEfficientCTCPortfolio(DataTable setOfInitialTickers,
                                                 DateTime firstQuoteDate,
                                                 DateTime lastQuoteDate,
                                                 int numberOfTickersInPortfolio,
                                                 int numDaysOfPortfolioLife,
                                                 double targetPerformance)
                                                 :
                                                base(setOfInitialTickers,
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
      
    }
  
    protected override float[] getArrayOfRatesOfReturn(string ticker)
    {
      Quotes tickerQuotes = new Quotes(ticker, this.firstQuoteDate, this.lastQuoteDate);
      float[] allAdjValues = ExtendedDataTable.GetArrayOfFloatFromColumn(tickerQuotes, "quAdjustedClose");
      float[] ratesOfReturns = new float[allAdjValues.Length/this.numDaysOfPortfolioLife + 1];
      int i = 0; //index for ratesOfReturns array
      
      for(int idx = 0; idx + this.numDaysOfPortfolioLife < allAdjValues.Length; idx += this.numDaysOfPortfolioLife )
      {
        ratesOfReturns[i] = allAdjValues[idx+this.numDaysOfPortfolioLife]/
                            allAdjValues[idx] - 1;
        i++;
      }

      return ratesOfReturns;
    }
    
  }

}
