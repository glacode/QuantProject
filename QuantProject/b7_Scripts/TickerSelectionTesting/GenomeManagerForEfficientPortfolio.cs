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

    protected DataTable setOfTickers;
    protected int originalNumOfTickers;
    protected DateTime firstQuoteDate;
    protected DateTime lastQuoteDate;
    protected double targetPerformance;
    protected double variance;
    protected double lowerPartialMoment;
    protected double rateOfReturn;
    protected PortfolioType portfolioType;
    protected double[] portfolioRatesOfReturn;
    protected int numberOfExaminedReturns;
    
    static public string GetCleanTickerCode(string tickerModifiedCode)
    {
    	if(tickerModifiedCode.StartsWith("-"))
    	//if the first char is "-"
    	//each element of the array of rates of return is
    	//multiplied by -1
    		return tickerModifiedCode.Substring(1,tickerModifiedCode.Length -1);
    	else
    		return tickerModifiedCode;
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
    
    public GenomeManagerForEfficientPortfolio(DataTable setOfInitialTickers,
																				      DateTime firstQuoteDate,
																				      DateTime lastQuoteDate,
																				      int numberOfTickersInPortfolio,
																				      double targetPerformance,
																				      PortfolioType portfolioType)
                          
    {
      this.setOfTickers = setOfInitialTickers;
 			this.originalNumOfTickers = setOfInitialTickers.Rows.Count;
      if(!this.setOfTickers.Columns.Contains("ArrayOfRatesOfReturn"))
        this.setOfTickers.Columns.Add("ArrayOfRatesOfReturn", System.Type.GetType("System.Array"));
      this.firstQuoteDate = firstQuoteDate;
      this.lastQuoteDate = lastQuoteDate;
      this.targetPerformance = targetPerformance;
      this.genomeSize = numberOfTickersInPortfolio;
      this.portfolioType = portfolioType;
      this.setMinAndMaxValueForGenes();
      this.set_SetOfInitialTickers();
  
    }
    
    
    private void set_SetOfInitialTickers()
    {
      
      if(this.portfolioType == PortfolioType.ShortAndLong)
      {
      	for(int i = 0;i<this.originalNumOfTickers;i++)
      	{
      		string ticker = (string)this.setOfTickers.Rows[i][0];
      		DataRow newRow = this.setOfTickers.NewRow();
      		newRow[0] = "-" + ticker;
      		this.setOfTickers.Rows.Add(newRow);
      		//so, if row[i][0]="TICKER" 
      		//row[i+originalNumOfTickers][0]="-TICKER"
      	}
      }
    }
    
    private void setMinAndMaxValueForGenes()
    {
      //each genes is the index for the setOfTickers table
      this.minValueForGenes = 0;
      
      if(this.portfolioType == PortfolioType.OnlyLong ||
         this.portfolioType == PortfolioType.OnlyShort)
            this.maxValueForGenes = this.setOfTickers.Rows.Count - 1;
      else//ShortAndLong
            this.maxValueForGenes = this.setOfTickers.Rows.Count*2 - 1;
    }
    
    
    
    protected float getCoefficient(string ticker)
    {
    	float returnValue;
    	if(ticker.StartsWith("-"))
    	//if the first char is "-"
    	//each element of the array of rates of return is
    	//multiplied by -1
    		returnValue = -1;
    	else
    		returnValue = 1;
    	
    	return returnValue;
    		
    }
    
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
    
    public Genome[] GetChilds(Genome parent1, Genome parent2)
    {
      return 
      	GenomeManagement.MixGenesWithoutDuplicates(parent1, parent2,
      	                                           this.originalNumOfTickers);
    }
    
    public int GetNewGeneValue(Genome genome)
    {
      // in this implementation new gene values must be different from
      // the others already stored in the given genome
      int returnValue = GenomeManagement.RandomGenerator.Next(genome.MinValueForGenes,
        genome.MaxValueForGenes + 1);
      while(genome.HasGene(returnValue) ||
            genome.HasGene(returnValue + this.originalNumOfTickers) ||
            genome.HasGene(returnValue - this.originalNumOfTickers) )
      //the portfolio can't have a long position and a short position
      // for the same ticker
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
            genome.HasGene(newValueForGene + this.originalNumOfTickers) ||
            genome.HasGene(newValueForGene - this.originalNumOfTickers) )
        //the efficient portfolio, in this implementation, 
        // can't have a long position and a short position
        // for the same ticker
      {
        newValueForGene = GenomeManagement.RandomGenerator.Next(genome.MinValueForGenes,
          genome.MaxValueForGenes +1);
      }
      GenomeManagement.MutateOneGene(genome, mutationRate,
        genePositionToBeMutated, newValueForGene);
    }
    
    public virtual object Decode(Genome genome)
    {
      string sequenceOfTickers = ""; 
      object returnValue;
      foreach(int index in genome.Genes())
      {
        sequenceOfTickers += (string)this.setOfTickers.Rows[index][0] + ";" ;
      }
      returnValue = sequenceOfTickers;
      returnValue += "(rate: " + this.RateOfReturn + " std: " +
        System.Math.Sqrt(this.Variance) + ")";
      return returnValue;
    }
    // end of implementation of IGenomeManager

    #region old implementation for variance computation
    
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

    #endregion
    
    
    protected void retrieveData()
    {
      foreach(DataRow row in this.setOfTickers.Rows)
      {
        //
        float[] arrayOfRatesOfReturn = this.getArrayOfRatesOfReturn((string)row[0]);
        if(arrayOfRatesOfReturn == null)
        	row["ArrayOfRatesOfReturn"] = DBNull.Value;
        else
        	row["ArrayOfRatesOfReturn"] = arrayOfRatesOfReturn;
      }
    }
    
    //this protected method must be overriden by inherited classes
    //specifing the type of rates of return that have to 
    //be analyzed

    protected virtual float[] getArrayOfRatesOfReturn(string ticker)
    {
    	float[] returnValue = null;
    	return returnValue;
    }
   
        
    protected double[] getPortfolioRatesOfReturn(int[] tickerIdx)
    {
      double[] returnValue = new double[this.numberOfExaminedReturns];
      float[] tickerRatesOfReturn;
      for(int i = 0; i<returnValue.Length; i++)    
      {  
        foreach(int idx in tickerIdx)
        {
        	if(this.setOfTickers.Rows[idx]["ArrayOfRatesOfReturn"] is System.DBNull)
          //the idx points to a ticker for which short returns are
          //to be examined
        		tickerRatesOfReturn =
        					(float[])this.setOfTickers.Rows[idx - this.originalNumOfTickers]["ArrayOfRatesOfReturn"];
        	else
        		tickerRatesOfReturn = (float[])this.setOfTickers.Rows[idx]["ArrayOfRatesOfReturn"];
          
          returnValue[i] += 
          		this.getCoefficient((string)this.setOfTickers.Rows[idx][0])*
          		tickerRatesOfReturn[i]/this.genomeSize;
          //the investment is assumed to be equally divided for each ticker
        }
      }
      return returnValue;
      
    }
    
  }

}
