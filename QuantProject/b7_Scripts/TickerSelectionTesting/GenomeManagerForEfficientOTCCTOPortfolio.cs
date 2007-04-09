/*
QuantProject - Quantitative Finance Library

GenomeManagerForEfficientOTCCTOPortfolio.cs
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
	/// portfolios based on the OTC and CTO strategy, using the
	/// GeneticOptimizer
	/// </summary>
	[Serializable]
  public class GenomeManagerForEfficientOTCCTOPortfolio : GenomeManagerForEfficientPortfolio
  {
    
    public GenomeManagerForEfficientOTCCTOPortfolio(DataTable setOfInitialTickers,
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
      Quotes tickerQuotes = new Quotes(ticker, this.firstQuoteDate, this.lastQuoteDate);
      float[] returnValue = new float[2*tickerQuotes.Rows.Count - 1];
      int j = 0;
      for(int i = 0;i<tickerQuotes.Rows.Count; i++)
      {
        //open to close
        returnValue[j] = (float)tickerQuotes.Rows[i]["quClose"]/
                         (float)tickerQuotes.Rows[i]["quOpen"] - 1;
        //close to open
        if(i<tickerQuotes.Rows.Count-1)
        {
          returnValue[j+1] = 
            -(( (float)tickerQuotes.Rows[i+1]["quOpen"]*
            (float)tickerQuotes.Rows[i+1]["quAdjustedClose"]/
            (float)tickerQuotes.Rows[i+1]["quClose"] )
            /(float)tickerQuotes.Rows[i]["quAdjustedClose"] - 1);
        }
        j += 2 ;
      }
      this.numberOfExaminedReturns = returnValue.Length;
      return returnValue;
    }
    
    
    protected override double getFitnessValue_calculate()
    {
      return this.RateOfReturn/Math.Sqrt(this.Variance);
      //return AdvancedFunctions.GetExpectancyScore(this.PortfolioRatesOfReturn);
    }
    
    
  }

}
