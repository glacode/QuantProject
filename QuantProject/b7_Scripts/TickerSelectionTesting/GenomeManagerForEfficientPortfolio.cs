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
using QuantProject.ADT.Optimizing.BruteForce;
using QuantProject.Data;
using QuantProject.Data.DataTables;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
  /// <summary>
  /// This is the base class implementing IGenomeManager, in order to find
  /// efficient portfolios using the GeneticOptimizer
  /// </summary>
  [Serializable]
  public abstract class GenomeManagerForEfficientPortfolio : IGenomeManager
  {
    protected int genomeSize;
    protected int minValueForGenes;
    protected int maxValueForGenes;
    protected DataTable setOfTickers;
    protected int originalNumOfTickers;
    protected DateTime firstQuoteDate;
    protected DateTime lastQuoteDate;
    protected double targetPerformance;
    protected double varianceOfStrategyReturns;
    protected double averageOfStrategyReturns;
    protected PortfolioType portfolioType;
    protected string benchmark;
    protected float[] strategyReturns;
    protected GeneticOptimizer currentGeneticOptimizer;
    protected WeightedPositions weightedPositionsFromGenome;
    
    public virtual int GenomeSize
    {
      get{return this.genomeSize;}
    }
    public virtual int GetMinValueForGenes(int genePosition)
    {
      return this.minValueForGenes;
    }
    public virtual int GetMaxValueForGenes(int genePosition)
    {
      return this.maxValueForGenes;
    }
    public double VarianceOfStrategyReturns
    {
      get{return this.varianceOfStrategyReturns;}
    }
    public double AverageOfStrategyReturns
    {
      get{return this.averageOfStrategyReturns;}
    }
    public PortfolioType PortfolioType
    {
      get{return this.portfolioType;}
    }
    public float[] StrategyReturns
    {
      get{return this.strategyReturns;}
    }
    public DateTime FirstQuoteDate
    {
      get{return this.firstQuoteDate;}
    }
    public DateTime LastQuoteDate
    {
      get{return this.lastQuoteDate;}
    }
    
    //setOfInitialTickers has to contain the
    //ticker's symbol in the first column !
    public GenomeManagerForEfficientPortfolio(DataTable setOfInitialTickers,
										      DateTime firstQuoteDate,
										      DateTime lastQuoteDate,
										      int numberOfTickersInPortfolio,
										      double targetPerformance,
										      PortfolioType portfolioType,
										     	string benchmark)
                          
    {
 			this.setOfTickers = setOfInitialTickers;
			if ( setOfInitialTickers.Rows.Count == 0 )
				throw new Exception( "setOfInitialTickers cannot be empty!" );
      this.originalNumOfTickers = setOfInitialTickers.Rows.Count;
      this.firstQuoteDate = firstQuoteDate;
      this.lastQuoteDate = lastQuoteDate;
      this.targetPerformance = targetPerformance;
      this.genomeSize = numberOfTickersInPortfolio;
      this.portfolioType = portfolioType;
      this.benchmark = benchmark;
      this.setMinAndMaxValueForGenes();
      GenomeManagement.SetRandomGenerator(QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator);
    }
    
    private void setMinAndMaxValueForGenes()
    {
      switch (this.portfolioType) 
      {
        case QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios.PortfolioType.OnlyLong :        
        //OnlyLong orders are admitted
          this.minValueForGenes = 0;
          this.maxValueForGenes = this.originalNumOfTickers - 1;
          break;
        case QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios.PortfolioType.OnlyShort :        
        //OnlyShort orders are admitted
          this.minValueForGenes = - this.originalNumOfTickers;
          //if gene g is negative, it refers to the ticker |g|-1 to be shorted
          this.maxValueForGenes = - 1;
          break;
        default :        
        //Both Long and Short orders are admitted
          this.minValueForGenes = - this.originalNumOfTickers;
          this.maxValueForGenes = this.originalNumOfTickers - 1;
          break;
      }
    }
    
    #region GetFitnessValue

    protected virtual double getFitnessValue_calculate()
    {
				return this.AverageOfStrategyReturns/Math.Sqrt(this.VarianceOfStrategyReturns);
    }
    
		protected abstract float[] getStrategyReturns();

		private void setWeightedPositionsFromGenome(Genome genome)
		{
			GenomeMeaning genomeMeaning = (GenomeMeaning)genome.Meaning;
			this.weightedPositionsFromGenome = new WeightedPositions(
				genomeMeaning.TickersPortfolioWeights,
				new SignedTickers(genomeMeaning.Tickers) );
		}

    public virtual double GetFitnessValue(Genome genome)
    {
      double returnValue = 0;
			this.setWeightedPositionsFromGenome(genome);
      this.strategyReturns = this.getStrategyReturns();
      this.averageOfStrategyReturns = 
            BasicFunctions.SimpleAverage(this.strategyReturns);
      this.varianceOfStrategyReturns = 
            BasicFunctions.Variance(this.strategyReturns);

      if(!Double.IsInfinity(this.VarianceOfStrategyReturns) &&
         !Double.IsInfinity(this.AverageOfStrategyReturns) &&
         !Double.IsNaN(this.VarianceOfStrategyReturns) &&
         !Double.IsNaN(this.AverageOfStrategyReturns) &&
        	this.VarianceOfStrategyReturns > 0.0)
      //both varianceOfStrate and rate of return are 
      //double values computed in the right way:
      // so it's possible to assign fitness
      {
      	if( this.portfolioType == PortfolioType.OnlyMixed &&
					  ( this.weightedPositionsFromGenome.NumberOfLongPositions == 0 ||
      	      this.weightedPositionsFromGenome.NumberOfShortPositions == 0  )  )
				// if both long and short positions have to be taken and
				// there aren't both long and short positions in portfolio
							returnValue = -1.0;
				else//short and long, only long or only short portfolio OR
					  //Only Mixed portfolio with both long and short position
							returnValue = this.getFitnessValue_calculate();
      }
      return returnValue;
    }
    
//    public virtual double GetFitnessValue(BruteForceOptimizableParameters bruteForceOptimizableParameters)
//    {
//      double returnValue = 0;
//      this.portfolioRatesOfReturn = 
//      	this.getPortfolioRatesOfReturn(bruteForceOptimizableParameters.GetValues());
//      double averagePortfolioRateOfReturn = 
//            BasicFunctions.SimpleAverage(this.portfolioRatesOfReturn);
//        
//      double portfolioVariance = 
//            BasicFunctions.Variance(this.portfolioRatesOfReturn);
//
//      if(!Double.IsInfinity(portfolioVariance) &&
//         !Double.IsInfinity(averagePortfolioRateOfReturn) &&
//         !Double.IsNaN(portfolioVariance) &&
//         !Double.IsNaN(averagePortfolioRateOfReturn) &&
//        	portfolioVariance > 0.0)
//      //both variance and rate of return are 
//      //double values computed in the right way:
//      // so it's possible to assign fitness
//      {
//	      this.variance = portfolioVariance;
//      	this.rateOfReturn = averagePortfolioRateOfReturn;
//      	returnValue = this.getFitnessValue_calculate();
//      }
//      
//      return returnValue;
//    }
    
    #endregion

    public virtual Genome[] GetChilds(Genome parent1, Genome parent2)
    {
      return
      	GenomeManipulator.MixGenesWithoutDuplicates(parent1, parent2);
    }
    
    public virtual int GetNewGeneValue(Genome genome, int genePosition)
    {
      // in this implementation new gene values must be different from
      // the others already stored in the given genome
      int returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
                           genome.GetMaxValueForGenes(genePosition) + 1);
      while( GenomeManipulator.IsTickerContainedInGenome(returnValue,genome) )
      //the portfolio can't have a long position and a short one for the same ticker 
      {
        returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
                           genome.GetMaxValueForGenes(genePosition) + 1);
      }
      return returnValue;
    }
        
    public virtual void Mutate(Genome genome)
    {
      // in this implementation only one gene is mutated
      // the new value has to be different from all the other genes of the genome
      int genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
      int newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
                            genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);
     	while( GenomeManipulator.IsTickerContainedInGenome(newValueForGene,genome) )
			//the portfolio can't have a long position and a short one for the same ticker
      {
        newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
                            genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);
      }
      GenomeManagement.MutateOneGene(genome, genePositionToBeMutated, newValueForGene);
    }
    
    #region Decode

    protected virtual string decode_getTickerCodeForLongOrShortTrade(int geneValue)
    {
      string initialCharForTickerCode = "";
      int position = geneValue;//for geneValue >= 0 the coded ticker is for long
      if(geneValue < 0)
      {
        position = Math.Abs(geneValue) - 1;
        initialCharForTickerCode = "-";
      }  
      return initialCharForTickerCode + (string)setOfTickers.Rows[position][0];
    }

    public virtual object Decode(BruteForceOptimizableParameters bruteForceOptimizableParameters)
    {
     	string[] arrayOfTickers = 
    		new string[bruteForceOptimizableParameters.GetValues().Length];
      int indexOfTicker;
      for(int index = 0; index < bruteForceOptimizableParameters.GetValues().Length; index++)
      {
      	indexOfTicker = bruteForceOptimizableParameters.GetValues()[index];
        arrayOfTickers[index] = this.decode_getTickerCodeForLongOrShortTrade(indexOfTicker);
      }
      GenomeMeaning meaning = new GenomeMeaning(arrayOfTickers);
      return meaning;
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
      GenomeMeaning meaning = new GenomeMeaning(arrayOfTickers);
      return meaning;
    }
    #endregion

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
         
    protected virtual double getTickerWeight(int[] genes, int tickerPositionInGenes)
    {
      return 1.0/genes.Length;
      //weights for tickers are all the same in this implementation
    }
  }
}
