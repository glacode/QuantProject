/*
QuantProject - Quantitative Finance Library

GenomeManagerForEfficientPortfolio.cs
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

namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
  /// <summary>
  /// This is the base class implementing IGenomeManager, in order to find
  /// efficient portfolios using the GeneticOptimizer
  /// </summary>
  [Serializable]
  public class GenomeManagerForEfficientPortfolio : IGenomeManager
  {
    protected int genomeSize;
    protected int minValueForGenes;
    protected int maxValueForGenes;

    protected DataTable setOfTickers;//used only for keeping
                                     //the same signature for 
                                     //protected retrieveData() method
    protected CandidateProperties[] setOfCandidates;
    protected int originalNumOfTickers;
    protected int constToDiscoverDuplicateGenes;
    protected DateTime firstQuoteDate;
    protected DateTime lastQuoteDate;
    protected double targetPerformance;
    protected double variance;
    protected double lowerPartialMoment;
    protected double rateOfReturn;
    protected PortfolioType portfolioType;
    protected double[] portfolioRatesOfReturn;
    protected int numberOfExaminedReturns;
    
    static public string GetCleanTickerCode(string tickerCodeForLongOrShortTrade)
    {
    	if(tickerCodeForLongOrShortTrade.StartsWith("-"))
    		return tickerCodeForLongOrShortTrade.Substring(1,tickerCodeForLongOrShortTrade.Length -1);
    	else
    		return tickerCodeForLongOrShortTrade;
    }
    
    //IGenomeManager implementation for properties 
    public int GenomeSize
    {
      get{return this.genomeSize;}
    }
    
    public int MinValueForGenes
    {
      get{return this.minValueForGenes;}
    }
    
    public int MaxValueForGenes
    {
      get{return this.maxValueForGenes;}
    }
    //end of implementation for properties
    
    public double Variance
    {
      get{return this.variance;}
    }
    
    public double RateOfReturn
    {
      get{return this.rateOfReturn;}
    }

    public PortfolioType PortfolioType
    {
      get{return this.portfolioType;}
      //set{this.portfolioType = value;}
    }
    
    //setOfInitialTickers has to contain the
    //ticker's symbol in the first column !

    public GenomeManagerForEfficientPortfolio(DataTable setOfInitialTickers,
																				      DateTime firstQuoteDate,
																				      DateTime lastQuoteDate,
																				      int numberOfTickersInPortfolio,
																				      double targetPerformance,
																				      PortfolioType portfolioType)
                          
    {
 			this.setOfTickers = setOfInitialTickers;
      this.originalNumOfTickers = setOfInitialTickers.Rows.Count;
      this.constToDiscoverDuplicateGenes = this.originalNumOfTickers + 1;
      this.firstQuoteDate = firstQuoteDate;
      this.lastQuoteDate = lastQuoteDate;
      this.targetPerformance = targetPerformance;
      this.genomeSize = numberOfTickersInPortfolio;
      this.portfolioType = portfolioType;
      this.setMinAndMaxValueForGenes();
      GenomeManagement.SetRandomGenerator(QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator);
    }
    
    private void setMinAndMaxValueForGenes()
    {
      this.minValueForGenes = 0;
      this.maxValueForGenes = this.originalNumOfTickers - 1;
           
      if(this.portfolioType == PortfolioType.ShortAndLong)
        this.minValueForGenes = - this.originalNumOfTickers;
      //if gene g is negative, it refers to the ticker Abs(g+1) to be shorted
    }
    
    //this protected method has to be called by inherited genome
    //managers (open to close or close to close) 
    //only after all initializations provided
    //by their respective constructors
    protected void retrieveData()
    {
      this.setOfCandidates = new CandidateProperties[setOfTickers.Rows.Count];
      for(int i = 0; i<setOfTickers.Rows.Count; i++)
      {
        string ticker = (string)setOfTickers.Rows[i][0];
        this.setOfCandidates[i] = new CandidateProperties(ticker,
                                      this.getArrayOfRatesOfReturn(ticker));
      }
    }

    //implementation of IGenomeManager
  
    #region GetFitnessValue

    protected virtual double getFitnessValue_calculate()
    {
      double returnValue = 0;                                            
        
      NormalDistribution normal = 
        new NormalDistribution(this.rateOfReturn,
        Math.Sqrt(this.variance));
      if(this.portfolioType == PortfolioType.OnlyLong ||
        this.portfolioType == PortfolioType.ShortAndLong)
        //the genome fitness is evaluated as if
        //the portfolio was long
        //returnValue = normal.GetProbability(this.targetPerformance*0.75,this.targetPerformance*1.25);
        returnValue = 1.0 - normal.GetProbability(this.targetPerformance);
      else//only short orders are permitted
        //returnValue = normal.GetProbability(-this.targetPerformance*1.25,-this.targetPerformance*0.75);
        returnValue = normal.GetProbability(-this.targetPerformance);

      return returnValue;
      
    }
    
    public double GetFitnessValue(Genome genome)
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
      }
      
      return returnValue;
    }
    
    #endregion

    public Genome[] GetChilds(Genome parent1, Genome parent2)
    {
      return
      	GenomeManagement.MixGenesWithoutDuplicates(parent1, parent2,
      	                                           this.constToDiscoverDuplicateGenes);
    }
    
    public int GetNewGeneValue(Genome genome)
    {
      // in this implementation new gene values must be different from
      // the others already stored in the given genome
      int returnValue = GenomeManagement.RandomGenerator.Next(genome.MinValueForGenes,
                                                              genome.MaxValueForGenes + 1);
      while(genome.HasGene(returnValue) ||
            genome.HasGene(returnValue + this.constToDiscoverDuplicateGenes) ||
            genome.HasGene(returnValue - this.constToDiscoverDuplicateGenes) )
      //the portfolio can't have a long position and a short one for the same ticker
      {
        returnValue = GenomeManagement.RandomGenerator.Next(genome.MinValueForGenes,
          genome.MaxValueForGenes + 1);
      }

      return returnValue;
    }
        
    public void Mutate(Genome genome, double mutationRate)
    {
      // in this implementation only one gene is mutated
      // the new value has to be different from all the other genes of the genome
      int newValueForGene = GenomeManagement.RandomGenerator.Next(genome.MinValueForGenes,
        genome.MaxValueForGenes +1);
      int genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size); 
      while(genome.HasGene(newValueForGene) || 
            genome.HasGene(newValueForGene + this.constToDiscoverDuplicateGenes) ||
            genome.HasGene(newValueForGene - this.constToDiscoverDuplicateGenes) )
        //the efficient portfolio, in this implementation, 
        // can't have a long position and a short position
        // for the same ticker
      {
        newValueForGene = GenomeManagement.RandomGenerator.Next(genome.MinValueForGenes,
                                                         genome.MaxValueForGenes + 1);
      }
      GenomeManagement.MutateOneGene(genome, mutationRate,
                                     genePositionToBeMutated, newValueForGene);
    }
    
    #region Decode

    private string decode_getTickerCodeForLongOrShortTrade(int geneValue)
    {
      string initialCharForTickerCode = "";
      int position = geneValue;
      if(geneValue<0)
      {
        position = Math.Abs(geneValue + 1);
        initialCharForTickerCode = "-";
      }  
      return initialCharForTickerCode + this.setOfCandidates[position].Ticker;
    }

    public virtual object Decode(Genome genome)
    {
      string[] arrayOfTickers = new string[genome.Genes().Length];
      int indexOfTicker;
      for(int index = 0; index < genome.Genes().Length; index++)
      {
        indexOfTicker = (int)genome.Genes().GetValue(index);
        arrayOfTickers[index] = this.decode_getTickerCodeForLongOrShortTrade(indexOfTicker);
      }
      return arrayOfTickers;
      
    }
    #endregion

    // end of implementation of IGenomeManager

    #region old implementation for variance computation
    /*
    protected double getPortfolioVariance(int[] tickerIdx)
    {
      double sumOfVariances = this.getWeightedSumOfVariances(tickerIdx);
      double sumOfCovariances = this.getWeightedSumOfCovariances(tickerIdx);
      double returnValue = sumOfVariances + sumOfCovariances; 
      return returnValue;
    }
    
    protected double getWeightedSumOfVariances(int[] tickerIdx)
    {
      double returnValue = 0;
      double tickerCoeff = 1.0/this.genomeSize;  
      foreach(int idx in tickerIdx)
      {
        returnValue += tickerCoeff * tickerCoeff * 
        				BasicFunctions.Variance((float[])this.setOfTickers.Rows[idx]["ArrayOfRatesOfReturn"]);
      }
      return returnValue;
    }

    protected double getWeightedSumOfCovariances(int[] tickerIdx)
    {
      double returnValue = 0;
      float[] ticker_i;
      float[] ticker_j;
      double tickerCoeff = 1.0/this.genomeSize; 
      for(int i = 0; i<this.genomeSize ; i++)
      {
        ticker_i = (float[])this.setOfTickers.Rows[i]["ArrayOfRatesOfReturn"];
        for(int j = 0 ; j<this.genomeSize ; j++)
        {
          if(j != i)
          {
            ticker_j = (float[])this.setOfTickers.Rows[j]["ArrayOfRatesOfReturn"];
            returnValue += tickerCoeff * tickerCoeff * 
            				BasicFunctions.CoVariance(ticker_i, ticker_j);
          }
        }
      }
      return returnValue;
    }
    */
    #endregion
         
   
    //this protected method must be overriden by inherited classes
    //specifing the type of rates of return that have to 
    //be analyzed
    protected virtual float[] getArrayOfRatesOfReturn(string ticker)
    {
    	float[] returnValue = null;
    	return returnValue;
    }
    
    #region getPortfolioRatesOfReturn
    
    private int getPortfolioRatesOfReturn_getRateOfTickerToBeAddedToTheArray_getPositionInArray(int geneValueForTickerIdx)
    {
      int position = geneValueForTickerIdx;
      if(geneValueForTickerIdx<0)
        position = Math.Abs(geneValueForTickerIdx + 1);
      return position;
    }
    
    private float getPortfolioRatesOfReturn_getRateOfTickerToBeAddedToTheArray(int tickerIdx,
                                                                               int arrayElementPosition)
    {
      bool longReturns = false;
      if(tickerIdx > 0)
        //the tickerIdx points to a ticker for which long returns are to be examined
        longReturns = true;
      int position = this.getPortfolioRatesOfReturn_getRateOfTickerToBeAddedToTheArray_getPositionInArray(tickerIdx);
      this.setOfCandidates[position].LongRatesOfReturn = longReturns;
      float[] arrayOfRatesOfReturn = this.setOfCandidates[position].ArrayOfRatesOfReturn;
      return (arrayOfRatesOfReturn[arrayElementPosition]/this.GenomeSize);
      //the investment is assumed to be equally divided for each ticker
    }    
    
    protected double[] getPortfolioRatesOfReturn(int[] tickersIdx)
    {
      double[] returnValue = new double[this.numberOfExaminedReturns];
      for(int i = 0; i<returnValue.Length; i++)    
      {  
        foreach(int tickerIdx in tickersIdx)
          returnValue[i] +=
            this.getPortfolioRatesOfReturn_getRateOfTickerToBeAddedToTheArray(tickerIdx,i);
      }
      return returnValue;
    }

    #endregion
    
  }

}
