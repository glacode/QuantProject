/*
QuantProject - Quantitative Finance Library

GenomeManagerForEfficientCTOPortfolio.cs
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
	/// portfolios based on tickers' CloseToOpen rates, using the
	/// GeneticOptimizer
	/// </summary>
  public class GenomeManagerForEfficientCTOPortfolio : IGenomeManager
  {
    private int genomeSize;
    private int minValueForGenes;
    private int maxValueForGenes;

    private DataTable setOfTickers;
    private DateTime firstQuoteDate;
    private DateTime lastQuoteDate;
    private double targetPerformance;
    private double targetStdDev;
    private double variance;
    private double rateOfReturn;
    
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

    public GenomeManagerForEfficientCTOPortfolio(DataTable setOfInitialTickers,
                                                DateTime firstQuoteDate,
                                                DateTime lastQuoteDate,
                                                int numberOfTickersInPortfolio,
                                                double targetPerformance, 
                                                double targetStdDev)
                          
    {
      this.setOfTickers = setOfInitialTickers;
      if(!this.setOfTickers.Columns.Contains("ArrayOfCloseToOpenRates"))
          this.setOfTickers.Columns.Add("ArrayOfCloseToOpenRates", System.Type.GetType("System.Array"));
      this.firstQuoteDate = firstQuoteDate;
      this.lastQuoteDate = lastQuoteDate;
     	this.targetPerformance = targetPerformance;
      this.targetStdDev = targetStdDev;
      this.genomeSize = numberOfTickersInPortfolio;
       //each genes is the index for the setOfTickers table
      this.minValueForGenes = 0;
      this.maxValueForGenes = this.setOfTickers.Rows.Count - 1;
      
      this.retrieveData();
    }
    
    public double GetFitnessValue(Genome genome)
    {
      //parameters used to balance the rate of return against variance
      //double a=2.5, b=2.0; 
      double portofolioVariance = this.getPortfolioVariance(genome.Genes());
      double portfolioRateOfReturn = this.getPortfolioRateOfReturn(genome.Genes());
      this.variance = portofolioVariance;
      this.rateOfReturn = portfolioRateOfReturn;
      NormalDistribution normal = new NormalDistribution(portfolioRateOfReturn, Math.Sqrt(portofolioVariance));
      double returnValue = normal.GetProbability(this.targetPerformance*0.5,this.targetPerformance*1.5);
      //double returnValue = System.Math.Pow(((this.targetStdDev*this.targetStdDev)/portofolioVariance),a)*
                           //System.Math.Pow(System.Math.Max(0.0,(portfolioRateOfReturn/this.targetPerformance)),b); 
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
    
    public object Decode(Genome genome)
    {
      
      string[] arrayOfTickers = new string[genome.Genes().Length];
      int indexOfTicker;
      for(int index = 0; index < genome.Genes().Length; index++)
      {
        indexOfTicker = (int)genome.Genes().GetValue(index);
        arrayOfTickers[index] = (string)this.setOfTickers.Rows[indexOfTicker][0];
      }
      return arrayOfTickers;
      
      /*old implementation, to be used for output to console
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
      */
      
    }
    // end of implementation of IGenomeManager

    private double getPortfolioVariance(int[] tickerIdx)
    {
      double sumOfVariances = this.getSumOfVariances(tickerIdx);
      double sumOfCovariances = this.getSumOfCovariances(tickerIdx);
      double returnValue = sumOfVariances + sumOfCovariances; 
      return returnValue;
    }
    
    private double getSumOfVariances(int[] tickerIdx)
    {
      double returnValue = 0;
      double tickerCoeff = 1.0/this.genomeSize;
      foreach(int idx in tickerIdx)
      {
        returnValue += BasicFunctions.Variance((float[])this.setOfTickers.Rows[idx]["ArrayOfCloseToOpenRates"]);
      }
      returnValue = returnValue * tickerCoeff * tickerCoeff;
      return returnValue;
    }

    private double getSumOfCovariances(int[] tickerIdx)
    {
      double returnValue = 0;
      float[] ticker_i;
      float[] ticker_j;
      double tickerCoeff = 1/this.genomeSize;
      for(int i = 0; i<this.genomeSize ; i++)
      {
        ticker_i = (float[])this.setOfTickers.Rows[i]["ArrayOfCloseToOpenRates"];
        for(int j = 0 ; j<this.genomeSize ; j++)
        {
          if(j != i)
          {
            ticker_j = (float[])this.setOfTickers.Rows[j]["ArrayOfCloseToOpenRates"];
            returnValue += BasicFunctions.CoVariance(ticker_i, ticker_j);
          }
        }
      }
      returnValue = returnValue * tickerCoeff * tickerCoeff;
      return returnValue;
    }

    private void retrieveData()
    {
      foreach(DataRow row in this.setOfTickers.Rows)
      {
        float[] arrayOfCTORates = this.getArrayOfCTORates((string)row[0]);
        row["ArrayOfCloseToOpenRates"] = arrayOfCTORates;
      }
    }
    
    private float[] getArrayOfCTORates(string ticker)
    {
      Quotes tickerQuotes = new Quotes(ticker, this.firstQuoteDate, this.lastQuoteDate);
      return ExtendedDataTable.GetArrayOfFloatFromRatioOfColumns(tickerQuotes, "quClose", "quOpen");
    }
    
    

    private double getPortfolioRateOfReturn(int[] tickerIdx)
    {
      double returnValue = 0;
    	foreach(int idx in tickerIdx)
    	{
    		returnValue += (BasicFunctions.SimpleAverage((float[])this.setOfTickers.Rows[idx]["ArrayOfCloseToOpenRates"]) - 1);
    	}
      //the investement is assumed to be equally divided
      return (returnValue/this.GenomeSize);
    }
  }

}
