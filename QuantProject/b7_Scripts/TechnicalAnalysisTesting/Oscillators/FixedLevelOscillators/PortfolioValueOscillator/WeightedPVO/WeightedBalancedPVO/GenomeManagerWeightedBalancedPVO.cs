/*
QuantProject - Quantitative Finance Library

GenomeManagerWeightedBalancedPVO.cs
Copyright (C) 2006
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
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.WeightedPVO;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.WeightedPVO.WeightedBalancedPVO
{
	/// <summary>
	/// Implements what needed to use the Genetic Optimizer
	/// for finding the portfolio that best suites
	/// the Portfolio Value Oscillator strategy (with weights
	/// that balance the portfolio between long and short positions) 
	/// </summary>
	[Serializable]
  public class GenomeManagerWeightedBalancedPVO : GenomeManagerWeightedPVO
  {
    
  	private bool arePrecedingTickersOnlyLong(Genome genome, int genePositionOfCurrentTicker)
    {
      bool returnValue = true;
      for(int i = 0; i<genePositionOfCurrentTicker ; i++)
      {
        if(this.genePositionPointsToATicker(i) &&
           genome.GetGeneValue(i) < 0 )
        // current i points to a short ticker
            returnValue = false;
      }
      return returnValue;
    }

    private bool arePrecedingTickersOnlyShort(Genome genome, int genePositionOfCurrentTicker)
    {
      bool returnValue = true;
      for(int i = 0; i<genePositionOfCurrentTicker ; i++)
      {
        if(this.genePositionPointsToATicker(i) &&
          genome.GetGeneValue(i) >= 0 )
          // current i points to a long ticker
          returnValue = false;
      }
      return returnValue;
    }

  	
    public GenomeManagerWeightedBalancedPVO(DataTable setOfInitialTickers,
                           DateTime firstQuoteDate,
                           DateTime lastQuoteDate,
                           int numberOfTickersInPortfolio,
                           int numDaysForOscillatingPeriod,
                           int minLevelForOversoldThreshold,
	                         int maxLevelForOversoldThreshold,
	                         int minLevelForOverboughtThreshold,
	                         int maxLevelForOverboughtThreshold,
                           int divisorForThresholdComputation,
                           bool symmetricalThresholds,
                           bool overboughtMoreThanOversoldForFixedPortfolio,
                           PortfolioType inSamplePortfolioType,
                           string benchmark)
                           :
                          base(setOfInitialTickers,
                           firstQuoteDate,
                           lastQuoteDate,
                           numberOfTickersInPortfolio,
                           numDaysForOscillatingPeriod,
                           minLevelForOversoldThreshold,
	                         maxLevelForOversoldThreshold,
	                         minLevelForOverboughtThreshold,
	                         maxLevelForOverboughtThreshold,
                           divisorForThresholdComputation,
                           symmetricalThresholds,
                           overboughtMoreThanOversoldForFixedPortfolio,
                           inSamplePortfolioType,
                           benchmark)
    
    {
    	
    }
    
    private bool getTickerWeight_currentIndexPointsToATickerOfTheSameSign( int[]genes,
                                                                           int currentIndexOfTicker,
                                                                           int tickerPositionForComparison )
    {
    	//return true iif both point to long tickers or both point to short tickers
    	return (genes[currentIndexOfTicker] >= 0 && genes[tickerPositionForComparison] >= 0) ||
    		     (genes[currentIndexOfTicker] < 0 && genes[tickerPositionForComparison] < 0);
    }
    
    protected override double getTickerWeight(int[] genes, int tickerPositionInGenes)
    {
    	double totalOfWeightsForTickersOfTheSameSign = 0.0;
    	int numOfTickersOfTheSameSign = 0;
    	for(int j = this.numOfGenesDedicatedToThresholds; j < genes.Length; j++)
      {
        if( this.numOfGenesDedicatedToThresholds == 2 && j%2==0 &&
    		    this.getTickerWeight_currentIndexPointsToATickerOfTheSameSign(genes,j+1,tickerPositionInGenes) )
        //ticker weight is contained in genes at even position when 
      	//thresholds are asymmetrical and current Index Points To A Ticker Of The Same Sign
        //0 has to be avoided !
    		{						
    			totalOfWeightsForTickersOfTheSameSign += Math.Abs(genes[j]) + 1.0;
 				  numOfTickersOfTheSameSign++;    	
    		}
    		else if ( this.numOfGenesDedicatedToThresholds == 1 && j%2!=0 &&
        	       this.getTickerWeight_currentIndexPointsToATickerOfTheSameSign(genes,j+1,tickerPositionInGenes) )
     		//ticker weight is contained in genes at odd position when 
      	//thresholds are symmetrical and current Index Points To A Ticker Of The Same Sign
        //0 has to be avoided !
    		{
    			totalOfWeightsForTickersOfTheSameSign += Math.Abs(genes[j]) + 1.0;
    			numOfTickersOfTheSameSign++;
    		}
      }
      double minimumWeight = 0.75*(0.5/numOfTickersOfTheSameSign);
      //with 0.75 the minimum weight for 4 ticker can be 0.1875 (2 long and 2 short)
      //or 0.125 (3 long and 1 short or viceversa)
      double normalizingWeight = (  ( Math.Abs(genes[tickerPositionInGenes-1]) + 1.0 ) /
                    												totalOfWeightsForTickersOfTheSameSign   ) * 
                 								 (0.5 - numOfTickersOfTheSameSign*minimumWeight);
      
      return minimumWeight + normalizingWeight;
    }
  
    private int getNewGeneValue_getGeneValue(Genome genome, int genePosition)
    {
      int returnValue;
      int minValueForGene = genome.GetMinValueForGenes(genePosition);
      int maxValueForGene = genome.GetMaxValueForGenes(genePosition);
      if     ( this.numOfGenesDedicatedToThresholds == 2 &&
              this.overboughtMoreThanOversoldForFixedPortfolio && genePosition == 1 )
      //genePosition points to overbought threshold,
      //dipendent from the oversold one such that the portfolio tends to be fix
                  returnValue = Convert.ToInt32(Convert.ToDouble(genome.GetGeneValue(0)) * Convert.ToDouble(this.divisorForThresholdComputation) /
                        (Convert.ToDouble(this.divisorForThresholdComputation) - Convert.ToDouble(genome.GetGeneValue(0))));
      else if( genePosition == genome.Size - 1 &&
            this.arePrecedingTickersOnlyLong(genome, genePosition) )
      //genePosition points to the last ticker and the preceding ones
      //point only to long tickers. In this case it has to be produced
      //a value pointing to a short ticker
                  returnValue = GenomeManagement.RandomGenerator.Next(minValueForGene,0);
      else if( genePosition == genome.Size - 1 &&
               this.arePrecedingTickersOnlyShort(genome, genePosition) )
        //genePosition points to the last ticker and the preceding ones
        //point only to short tickers. In this case it has to be produced
        //a value pointing to a long ticker
                  returnValue = GenomeManagement.RandomGenerator.Next(0,maxValueForGene + 1);
      else // genePosition doesn't point to the last ticker
        	   // or it points to the last ticker but the preceding tickers
        		 // are not of the same sign
                  returnValue = GenomeManagement.RandomGenerator.Next(minValueForGene,
                                  maxValueForGene + 1);
      return returnValue;
    }
    
    public override int GetNewGeneValue(Genome genome, int genePosition)
    {
      // in this implementation only new gene values pointing to tickers
      // must be different from the others already stored
      int returnValue = this.getNewGeneValue_getGeneValue(genome,genePosition);
    	while(  this.genePositionPointsToATicker(genePosition) &&
              GenomeManipulator.IsTickerContainedInGenome( returnValue,
              genome,
              this.GetGenePositionsPointingToTickers(genome) )  )
      //while in the given position has to be stored
      //a new gene pointing to a ticker and
      //the proposed returnValue points to a ticker
      //already stored in the given genome, at the given genePositionsPointingToTickers
                  returnValue = this.getNewGeneValue_getGeneValue(genome,genePosition);
      return returnValue;
    }
  }
}
