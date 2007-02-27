/*
QuantProject - Quantitative Finance Library

GenomeManagerBiasedOTC_PVONoThresholds.cs
Copyright (C) 2007 
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
using QuantProject.ADT;
using QuantProject.ADT.Statistics;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Data;
using QuantProject.Data.DataTables;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.BiasedPVO.BiasedOTC_PVONoThresholds
{
	/// <summary>
	/// Implements what needed to use the Genetic Optimizer
	/// for finding the portfolio that best suites
	/// the Biased Portfolio Value Oscillator strategy, with no thresholds
	/// </summary>
	[Serializable]
  public class GenomeManagerBiasedOTC_PVONoThresholds : GenomeManagerForEfficientPortfolio
  {

    public GenomeManagerBiasedOTC_PVONoThresholds(DataTable setOfInitialTickers,
                           DateTime firstQuoteDate,
                           DateTime lastQuoteDate,
                           int numberOfTickersInPortfolio,
                           PortfolioType inSamplePortfolioType)
                           :
                          base(setOfInitialTickers,
                          firstQuoteDate,
                          lastQuoteDate,
                          numberOfTickersInPortfolio,
                          0.0,
                          inSamplePortfolioType)
                                
                          
    {
    	this.retrieveData();
    }
    
    #region Get Min and Max Value

    public override int GetMinValueForGenes(int genePosition)
    {
    	int returnValue;
      switch (this.portfolioType)
      {
        case PortfolioType.OnlyLong :
          returnValue = 0;
          break;
        default://For ShortAndLong or OnlyShort portfolios
          returnValue = - this.originalNumOfTickers;
          break;
      }
    	return returnValue;
    }

    public override int GetMaxValueForGenes(int genePosition)
    {
      int returnValue;
      switch (this.portfolioType)
      {
        case PortfolioType.OnlyShort :
          returnValue = - 1;
          break;
        default ://For ShortAndLong or OnlyLong portfolios
          returnValue = this.originalNumOfTickers - 1;
          break;
      }
    	return returnValue;
    }																
  	
    #endregion
												
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
            ( (float)tickerQuotes.Rows[i+1]["quOpen"]*
            (float)tickerQuotes.Rows[i+1]["quAdjustedClose"]/
            (float)tickerQuotes.Rows[i+1]["quClose"] )
            /(float)tickerQuotes.Rows[i]["quAdjustedClose"] - 1;
        }
        j += 2 ;
      }
      this.numberOfExaminedReturns = returnValue.Length;
      return returnValue;
    }
    
    //fitness is a number that indicates how much the portfolio
    //tends to preserve equity (no gain and no loss), with a low std dev
	  public override double GetFitnessValue(Genome genome)
    {
      double returnValue = -1.0;
	  	this.portfolioRatesOfReturn = this.getPortfolioRatesOfReturn( genome.Genes() );
	  	//double[] asbolutePortfolioRatesOfReturns = new double[this.portfolioRatesOfReturn.Length];
//	  	for( int i = 0; i<asbolutePortfolioRatesOfReturns.Length; i++ )
//	  		asbolutePortfolioRatesOfReturns[i] = Math.Abs(this.portfolioRatesOfReturn[i]);
	  	returnValue = 1.0/
        //(  BasicFunctions.SimpleAverage(asbolutePortfolioRatesOfReturns) *
          (  
            Math.Abs( BasicFunctions.SimpleAverage(this.portfolioRatesOfReturn)));//) *
            //BasicFunctions.StdDev(this.portfolioRatesOfReturn)  );  //);
      return returnValue;
    }
    
    public override object Decode(Genome genome)
    {
      string[] arrayOfTickers = new string[genome.Genes().Length];
      int indexOfTicker;
      for(int index = 0; index < genome.Genes().Length; index++)
      {
        indexOfTicker = (int)genome.Genes().GetValue(index);
        arrayOfTickers[index] = this.decode_getTickerCodeForLongOrShortTrade(indexOfTicker);
      }
      GenomeMeaningPVO meaning = new GenomeMeaningPVO(arrayOfTickers,
                                                      0.0, 0.0, 2);
      return meaning;
    }
  }
}
