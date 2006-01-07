/*
QuantProject - Quantitative Finance Library

GenomeManagerForEfficientOTCTypes.cs
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
  public class GenomeManagerForEfficientOTCTypes : GenomeManagerForWeightedEfficientPortfolio
  {
//    private GenomeManagerForEfficientCTOPortfolio genManCTO;
    public GenomeManagerForEfficientOTCTypes(DataTable setOfInitialTickers,
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
//      this.genManCTO = new GenomeManagerForEfficientCTOPortfolio(setOfInitialTickers,
//                                     firstQuoteDate,
//                                     lastQuoteDate,
//                                     numberOfTickersInPortfolio,
//                                     targetPerformance,
//                                     portfolioType);
    }
    //rate of return = rawClose/rawOpen - 1
    protected override float[] getArrayOfRatesOfReturn(string ticker)
    {
      float[] returnValue = null;
      Quotes tickerQuotes = new Quotes(ticker, this.firstQuoteDate, this.lastQuoteDate);
      returnValue = ExtendedDataTable.GetRatesOfReturnsFromColumns(tickerQuotes, "quClose", "quOpen");
      this.numberOfExaminedReturns = returnValue.Length;
      
      return returnValue;
    }
    
    public override double GetFitnessValue(Genome genome)
    {
      double returnValue = 0;
      this.portfolioRatesOfReturn = this.getPortfolioRatesOfReturn(genome.Genes());
      double averagePortfolioRateOfReturn = 
        BasicFunctions.SimpleAverage(this.portfolioRatesOfReturn);
        
      double portfolioVariance = 
        BasicFunctions.Variance(this.portfolioRatesOfReturn);

      if(!Double.IsInfinity(portfolioVariance) &&
        !Double.IsInfinity(averagePortfolioRateOfReturn) &&
        !Double.IsNaN(portfolioVariance) &&
        !Double.IsNaN(averagePortfolioRateOfReturn) &&
        portfolioVariance > 0.0)
        //both variance and rate of return are 
        //double values computed in the right way:
        // so it's possible to assign fitness
      {
        this.variance = portfolioVariance;
        this.rateOfReturn = averagePortfolioRateOfReturn;
        returnValue = this.getFitnessValue_calculate();
//        returnValue = this.getFitnessValue_calculate() -
//                      this.genManCTO.GetFitnessValue(genome);
        
      }
      
      return returnValue;
    }

    
    protected override double getFitnessValue_calculate()
    {
      return this.RateOfReturn/Math.Sqrt(this.Variance);
    }
    
    
  }

}
