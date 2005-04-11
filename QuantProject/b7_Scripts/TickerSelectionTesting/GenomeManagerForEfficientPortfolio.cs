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


namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
  /// <summary>
  /// This is the base class implementing IGenomeManager, in order to find
  /// efficient portfolios using the GeneticOptimizer
  /// </summary>
  public class GenomeManagerForEfficientPortfolio : IGenomeManager
  {
    protected int genomeSize;
    protected int minValueForGenes;
    protected int maxValueForGenes;

    protected DataTable setOfTickers;
    protected DateTime firstQuoteDate;
    protected DateTime lastQuoteDate;
    protected double targetPerformance;
    protected double variance;
    protected double rateOfReturn;
    protected PortfolioType portfolioType;
    
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
      set{this.portfolioType = value;}
    }
    
    public GenomeManagerForEfficientPortfolio(DataTable setOfInitialTickers,
																				      DateTime firstQuoteDate,
																				      DateTime lastQuoteDate,
																				      int numberOfTickersInPortfolio,
																				      double targetPerformance,
																				      PortfolioType portfolioType)
                          
    {
      this.setOfTickers = setOfInitialTickers;
 
      if(!this.setOfTickers.Columns.Contains("ArrayOfRatesOfReturn"))
        this.setOfTickers.Columns.Add("ArrayOfRatesOfReturn", System.Type.GetType("System.Array"));
      this.firstQuoteDate = firstQuoteDate;
      this.lastQuoteDate = lastQuoteDate;
      this.targetPerformance = targetPerformance;
      this.genomeSize = numberOfTickersInPortfolio;
      //each genes is the index for the setOfTickers table
      this.minValueForGenes = 0;
      this.maxValueForGenes = this.setOfTickers.Rows.Count - 1;
      this.portfolioType = portfolioType;
    }
    
    public virtual double GetFitnessValue(Genome genome)
    {
      double returnValue;
      double portfolioVariance = this.getPortfolioVariance(genome.Genes());
      double portfolioRateOfReturn = this.getPortfolioRateOfReturn(genome.Genes());
      this.variance = portfolioVariance;
      this.rateOfReturn = portfolioRateOfReturn;
      
      NormalDistribution normal = new NormalDistribution(portfolioRateOfReturn, Math.Sqrt(portfolioVariance));
      if(this.portfolioType == PortfolioType.OnlyLong)
         returnValue = normal.GetProbability(this.targetPerformance*0.75,this.targetPerformance*1.25);
      else//only short orders are permitted
      	returnValue = normal.GetProbability(-this.targetPerformance*1.25,-this.targetPerformance*0.75);
      
      return returnValue;
    }
    
    public Genome[] GetChilds(Genome parent1, Genome parent2)
    {
      return GenomeManagement.MixGenesWithoutDuplicates(parent1, parent2);
    }
    
    public int GetNewGeneValue(Genome genome)
    {
      // in this implementation new gene values must be different from
      // the others already stored in the given genome
      int returnValue = GenomeManagement.RandomGenerator.Next(genome.MinValueForGenes,
        genome.MaxValueForGenes + 1);
      while(genome.HasGene(returnValue))
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
      while(genome.HasGene(newValueForGene))
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

    protected double getPortfolioVariance(int[] tickerIdx)
    {
      double sumOfVariances = this.getSumOfVariances(tickerIdx);
      double sumOfCovariances = this.getSumOfCovariances(tickerIdx);
      double returnValue = sumOfVariances + sumOfCovariances; 
      return returnValue;
    }
    
    protected double getSumOfVariances(int[] tickerIdx)
    {
      double returnValue = 0;
      double tickerCoeff = 1.0/this.genomeSize;
      foreach(int idx in tickerIdx)
      {
        returnValue += BasicFunctions.Variance((float[])this.setOfTickers.Rows[idx]["ArrayOfRatesOfReturn"]);
      }
      returnValue = returnValue * tickerCoeff * tickerCoeff;
      return returnValue;
    }

    protected double getSumOfCovariances(int[] tickerIdx)
    {
      double returnValue = 0;
      float[] ticker_i;
      float[] ticker_j;
      double tickerCoeff = 1/this.genomeSize;
      for(int i = 0; i<this.genomeSize ; i++)
      {
        ticker_i = (float[])this.setOfTickers.Rows[i]["ArrayOfRatesOfReturn"];
        for(int j = 0 ; j<this.genomeSize ; j++)
        {
          if(j != i)
          {
            ticker_j = (float[])this.setOfTickers.Rows[j]["ArrayOfRatesOfReturn"];
            returnValue += BasicFunctions.CoVariance(ticker_i, ticker_j);
          }
        }
      }
      returnValue = returnValue * tickerCoeff * tickerCoeff;
      return returnValue;
    }

    protected void retrieveData()
    {
      foreach(DataRow row in this.setOfTickers.Rows)
      {
        //
        float[] arrayOfRatesOfReturn = this.getArrayOfRatesOfReturn((string)row[0]);
        row["ArrayOfRatesOfReturn"] = arrayOfRatesOfReturn;
      }
    }
    
    //this protected method can be overriden by inherited classes
    //which can specify the type of rates of return
    
    //in this basic implementation rates of returns
    //are based on daily close to close
    protected virtual float[] getArrayOfRatesOfReturn(string ticker)
    {
      Quotes tickerQuotes = new Quotes(ticker, this.firstQuoteDate, this.lastQuoteDate);
      float[] allAdjValues = ExtendedDataTable.GetArrayOfFloatFromColumn(tickerQuotes, "quAdjustedClose");
      float[] ratesOfReturns = new float[allAdjValues.Length];
      int i = 0; //index for ratesOfReturns array
      
      for(int idx = 0; idx < allAdjValues.Length; idx++)
      {
        ratesOfReturns[i] = allAdjValues[idx+1]/
          allAdjValues[idx] - 1;
        i++;
      }
      
      return ratesOfReturns;
    }
    
    
    

    protected double getPortfolioRateOfReturn(int[] tickerIdx)
    {
      double returnValue = 0;
      foreach(int idx in tickerIdx)
      {
        returnValue += BasicFunctions.SimpleAverage((float[])this.setOfTickers.Rows[idx]["ArrayOfRatesOfReturn"]);
      }
      //the investment is assumed to be equally divided
      return (returnValue/this.GenomeSize);
    }
  }

}
