/*
QuantProject - Quantitative Finance Library

GenomeManagerITF.cs
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
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.TrendFollowing.ImmediateTrendFollower
{
	/// <summary>
	/// Implements what needed to use the Genetic Optimizer
	/// for finding the portfolio that best suites
	/// the immediate trend follower strategy
	/// </summary>
	[Serializable]
  public class GenomeManagerITF : GenomeManagerForEfficientPortfolio
  {
    private int numDaysForReturnCalculation;
    
    public GenomeManagerITF(DataTable setOfInitialTickers,
                            DateTime firstQuoteDate,
                            DateTime lastQuoteDate,
                            int numberOfTickersInPortfolio,
                            int numDaysForReturnCalculation,
                            PortfolioType portfolioType)
                            :
            base(setOfInitialTickers,
            firstQuoteDate,
            lastQuoteDate,
            numberOfTickersInPortfolio,
            0.0,
            portfolioType)
                                
                          
    {
      this.numDaysForReturnCalculation = numDaysForReturnCalculation;
      this.retrieveData();
    }
    
    protected override float[] getArrayOfRatesOfReturn(string ticker)
    {
      float[] returnValue = null;
      Quotes tickerQuotes = new Quotes(ticker, this.firstQuoteDate, this.lastQuoteDate);
      returnValue = ExtendedDataTable.GetArrayOfFloatFromColumn(tickerQuotes,
    	                                                          Quotes.AdjustedCloseToCloseRatio);
      for(int i = 0; i<returnValue.Length; i++)
        returnValue[i] = returnValue[i] - 1.0f;
      
      this.numberOfExaminedReturns = returnValue.Length;
      
      return returnValue;
    }


		//fitness is a sharpe-ratio based indicator for the equity line resulting
		//from applying the strategy
	  public override double GetFitnessValue(Genome genome)
    {
      this.portfolioRatesOfReturn = this.getPortfolioRatesOfReturn(genome.Genes());
      
      double[] equityLine = this.getFitnessValue_getEquityLineRates();
      //return AdvancedFunctions.GetExpectancyScore(equityLine);
      return AdvancedFunctions.GetSharpeRatio(equityLine);
    }
    
    private double[] getFitnessValue_getEquityLineRates()
    {
    	double[] returnValue = new double[this.PortfolioRatesOfReturn.Length];
      double gainForTheLastHalfPeriod = 0.0;
      for(int i = this.numDaysForReturnCalculation - 1;
          i<this.PortfolioRatesOfReturn.Length - this.numDaysForReturnCalculation;
          i += this.numDaysForReturnCalculation)
      {
        gainForTheLastHalfPeriod = 0.0;
        for(int j=this.numDaysForReturnCalculation - 1;
            j > -1; j--)
        {
          gainForTheLastHalfPeriod = 
            (1.0+gainForTheLastHalfPeriod) * this.PortfolioRatesOfReturn[i-j];
        }
      
        for(int t=1;t<this.numDaysForReturnCalculation + 1;t++)
        {  
          if(gainForTheLastHalfPeriod<0.0)
            // if gain of first half period is negative 
             returnValue[i+t] = - this.PortfolioRatesOfReturn[i+t];
            //return of the next half period is still negative
          else// if gain of first half period is positive
             returnValue[i+t] = this.PortfolioRatesOfReturn[i+t]; 
          //return of the next half period is still positive
        }  
           
      }
      return returnValue;
    }
	
  }

}
