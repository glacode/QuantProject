/*
QuantProject - Quantitative Finance Library

GenomeManagerForFPOscillatorCTC.cs
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

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedPeriodOscillators
{
	/// <summary>
	/// Implements what needed to use the Genetic Optimizer
	/// for finding the portfolio that best suites
	/// the fixed period Oscillator strategy
	/// </summary>
	[Serializable]
  public class GenomeManagerForFPOscillatorCTC : GenomeManagerForEfficientPortfolio
  {
    private int numDaysForReturnCalculation;
    
    public GenomeManagerForFPOscillatorCTC(DataTable setOfInitialTickers,
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
    
    private float[] getArrayOfRatesOfReturn_getAdjustedArray(Quotes sourceQuotes,
                                                             int numDaysForHalfPeriod,
                                                             ref DateTime firstQuoteDate)
    {
      float[] returnValue = ExtendedDataTable.GetArrayOfFloatFromColumn(sourceQuotes,
    	                                                                  Quotes.AdjustedCloseToCloseRatio);
      for(int i = 0;i<returnValue.Length;i++)
      	returnValue[i] = returnValue[i]-1;
      //in order to be alligned at the following market day,
      //the array has to be long n, where n is such that
      //n%(2 * hp) + 1 = 2 * hp (hp = half period)
      //if some rates are deleted, first quote day has to be updated
      while( (returnValue.Length + 1) % (2*numDaysForHalfPeriod) != 0)
      {
        float[] newReturnValue = new float[returnValue.Length - 1];
        for(int k = 0;k<returnValue.Length - 1;k++)
          newReturnValue[k] = returnValue[k + 1];
        returnValue = newReturnValue;
        firstQuoteDate = firstQuoteDate.AddDays(1);
        firstQuoteDate = sourceQuotes.GetQuoteDateOrFollowing(firstQuoteDate);
      }

      return returnValue;
    }

    protected override float[] getArrayOfRatesOfReturn(string ticker)
    {
      float[] returnValue = null;
      Quotes tickerQuotes = new Quotes(ticker, this.firstQuoteDate, this.lastQuoteDate);
      returnValue = this.getArrayOfRatesOfReturn_getAdjustedArray(tickerQuotes, this.numDaysForReturnCalculation,
                                                                  ref this.firstQuoteDate);
      this.numberOfExaminedReturns = returnValue.Length;
      
      return returnValue;
    }

//   	//implementation with fitness as average of modified sh for single tickers
//    private double getFitnessValue_getModifiedSharpeRatioForTicker(int[] genes,
//                                                           int tickerPositionInGenes)
//    {
//      bool longReturns = false;
//      if(genes[tickerPositionInGenes] > 0)
//        //genes[tickerPositionInGenes], the code for ticker, points to a ticker for which long returns are to be examined
//        longReturns = true;
//      int position = this.getPortfolioRatesOfReturn_getRateOfTickerToBeAddedToTheArray_getPositionInArray(genes[tickerPositionInGenes]);
//      this.setOfCandidates[position].LongRatesOfReturn = longReturns;
//      float[] arrayOfRatesOfReturn = this.setOfCandidates[position].ArrayOfRatesOfReturn;
//      float[] strategyReturns = new float[arrayOfRatesOfReturn.Length];
//      float sign = 1.0F;
//      for(int i=0; i<arrayOfRatesOfReturn.Length; i++)
//      {
//      	if(i > 0 && (i % this.numDaysForReturnCalculation  ==  0) )
//	          sign = -sign;
//	      	
//	      strategyReturns[i] = sign * arrayOfRatesOfReturn[i];
//	    }
//      double stdDev = BasicFunctions.StdDev(strategyReturns);
//      double powerForStdDev = 2.0;
//      
//      return  BasicFunctions.SimpleAverage(strategyReturns)/
//      				Math.Pow(stdDev, powerForStdDev);
//    }
//    
////    new implementation: fitness is the averaged sum of the "modified" sharpeRatios 
////    for each single ticker
//    public override double GetFitnessValue(Genome genome)
//    {
//      double returnValue = 0.0;
//      for(int i = 0; i<genome.Size; i++)
//      {
//      	returnValue += this.getFitnessValue_getModifiedSharpeRatioForTicker( genome.Genes(),i );
//      }
//      return returnValue/genome.Size;	
//    }
    
    public override object Decode(Genome genome)
    {
    	
    	string[] arrayOfTickers = new string[genome.Genes().Length];
      int indexOfTicker;
      for(int index = 0; index < genome.Genes().Length; index++)
      {
        indexOfTicker = (int)genome.Genes().GetValue(index);
        arrayOfTickers[index] = this.decode_getTickerCodeForLongOrShortTrade(indexOfTicker);
      }
      GenomeMeaning meaning = new GenomeMeaning(arrayOfTickers);
      return meaning;
    }
    
    //OLD IMPLEMENTATION - linear combination
    public override double GetFitnessValue(Genome genome)
    {
      double returnValue = 0.0;
      this.portfolioRatesOfReturn = this.getPortfolioRatesOfReturn(genome.Genes());
      
      double[] oscillatorPortfolioRatesOfReturn = this.getFitnessValue_getOscillatorRates();
      
      double averageOscillatorPortfolioRateOfReturn = 
            BasicFunctions.SimpleAverage(oscillatorPortfolioRatesOfReturn);
        
      double oscillatorPortfolioStdDev = 
            BasicFunctions.StdDev(oscillatorPortfolioRatesOfReturn);

      returnValue = averageOscillatorPortfolioRateOfReturn /
                    Math.Pow(oscillatorPortfolioStdDev,1.2);
     
      return returnValue; 
    }
    
    private double[] getFitnessValue_getOscillatorRates()
    {
    	double[] returnValue = new double[this.PortfolioRatesOfReturn.Length];
      double sign = 1.0;
      for(int i = 0;i<this.PortfolioRatesOfReturn.Length; i++)
      {
      	if(i > 0 && (i % this.numDaysForReturnCalculation  ==  0) )
          sign = -sign;
        returnValue[i] = sign*this.PortfolioRatesOfReturn[i];
      }

      return returnValue;
    }
	
		//new implementation: changed how fitness is computed
		//now it is the sharpe ratio for the equity line resulting
		//from applying the strategy
//	  public override double GetFitnessValue(Genome genome)
//    {
//      double returnValue = 0.0;
//      this.portfolioRatesOfReturn = this.getPortfolioRatesOfReturn(genome.Genes());
//      
//      double[] equityLine = this.getFitnessValue_getEquityLineRates();
//      
//      double averageEquityLineRateOfReturn = 
//            BasicFunctions.SimpleAverage(equityLine);
//        
//      double equityLineStdDev = 
//            BasicFunctions.StdDev(equityLine);
//
//      returnValue = averageEquityLineRateOfReturn /
//                    Math.Pow(equityLineStdDev,2.0);
//     
//      return returnValue; 
//    }
//    
//    private double[] getFitnessValue_getEquityLineRates()
//    {
//    	double[] returnValue = new double[this.PortfolioRatesOfReturn.Length];
//      double sign = 1.0;
//      for(int i = this.numDaysForReturnCalculation - 1;
//          i<this.PortfolioRatesOfReturn.Length;
//          i++)
//      {
//      	// if gain of last half period is negative, 
//      	// then add to the array the returns of the second half
//      }
//
//      return returnValue;
//    }
	
  }

}
