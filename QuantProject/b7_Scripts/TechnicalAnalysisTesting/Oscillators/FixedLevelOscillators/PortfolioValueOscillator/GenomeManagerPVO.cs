/*
QuantProject - Quantitative Finance Library

GenomeManagerPVO.cs
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
using QuantProject.ADT;
using QuantProject.ADT.Statistics;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Data;
using QuantProject.Data.DataTables;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator
{
	/// <summary>
	/// Implements what needed to use the Genetic Optimizer
	/// for finding the portfolio that best suites
	/// the Portfolio Value Oscillator strategy
	/// </summary>
	[Serializable]
  public class GenomeManagerPVO : GenomeManagerForEfficientPortfolio
  {
    private int minLevelForOversoldThreshold;
    private int maxLevelForOversoldThreshold;
    private int minLevelForOverboughtThreshold;
    private int maxLevelForOverboughtThreshold;
    private int divisorForThresholdComputation;
   	private double currentOversoldThreshold = 0.0;
    private double currentOverboughtThreshold = 0.0;
    private int numDaysForOscillatingPeriod;
    
    private double[] portfolioValues;//the values for each unit, invested
                                     //at the beginning of the optimization period,
    																//throughout the period itself
    
    private void genomeManagerPVO_checkParametersForThresholdComputation()
    {
      if(this.maxLevelForOverboughtThreshold < this.minLevelForOverboughtThreshold || 
         this.maxLevelForOversoldThreshold < this.minLevelForOversoldThreshold ||
         this.divisorForThresholdComputation < this.maxLevelForOverboughtThreshold ||
         this.divisorForThresholdComputation < this.maxLevelForOversoldThreshold)

          throw new Exception("Bad parameters for threshold computation!");
    }

    public GenomeManagerPVO(DataTable setOfInitialTickers,
                           DateTime firstQuoteDate,
                           DateTime lastQuoteDate,
                           int numberOfTickersInPortfolio,
                           int numDaysForOscillatingPeriod,
                           int minLevelForOversoldThreshold,
	                         int maxLevelForOversoldThreshold,
	                         int minLevelForOverboughtThreshold,
	                         int maxLevelForOverboughtThreshold,
                           int divisorForThresholdComputation,
                           PortfolioType portfolioType)
                           :
                          base(setOfInitialTickers,
                          firstQuoteDate,
                          lastQuoteDate,
                          numberOfTickersInPortfolio,
                          0.0,
                          portfolioType)
                                
                          
    {
    	this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
    	this.minLevelForOversoldThreshold  = minLevelForOversoldThreshold;
      this.maxLevelForOversoldThreshold = maxLevelForOversoldThreshold;
      this.minLevelForOverboughtThreshold = minLevelForOverboughtThreshold;
      this.maxLevelForOverboughtThreshold = maxLevelForOverboughtThreshold;
      this.divisorForThresholdComputation = divisorForThresholdComputation;
      this.genomeManagerPVO_checkParametersForThresholdComputation();
      this.retrieveData();
    }
    
    public override int GenomeSize
    {
      get{return this.genomeSize + 2;}
      //two initial genes are for oversold and overbought thresholds
    }

    public override int GetMinValueForGenes(int genePosition)
    {
    	int returnValue;
    	switch (genePosition)
    	{
    		case 0 ://gene for oversold threshold
					returnValue = this.minLevelForOversoldThreshold;
					break;
				case 1 ://gene for overbought threshold
					returnValue = this.minLevelForOverboughtThreshold;
					break;
				default://gene for ticker
					returnValue = - this.originalNumOfTickers;
					break;
      }
    	return returnValue;
    }
    
    public override int GetMaxValueForGenes(int genePosition)
    {
      int returnValue;
    	switch (genePosition)
    	{
    		case 0 ://gene for oversold threshold
					returnValue = this.maxLevelForOversoldThreshold;
					break;
				case 1 ://gene for overbought threshold
					returnValue = this.maxLevelForOverboughtThreshold;
					break;
				default://gene for ticker
					returnValue = this.originalNumOfTickers - 1;
					break;
      }
    	return returnValue;
    }																
  															
    protected override float[] getArrayOfRatesOfReturn(string ticker)
    {
      float[] returnValue = null;
      Quotes tickerQuotes = new Quotes(ticker, this.firstQuoteDate, this.lastQuoteDate);
      returnValue = QuantProject.ADT.ExtendedDataTable.GetArrayOfFloatFromColumn(tickerQuotes,
    	                                                          Quotes.AdjustedCloseToCloseRatio);
      for(int i = 0; i<returnValue.Length; i++)
        returnValue[i] = returnValue[i] - 1.0f;
      
      this.numberOfExaminedReturns = returnValue.Length;
      
      return returnValue;
    }
    //starting from this.numDaysForOscillatingPeriod day,
    //it computes for each day ahead the value of the
    //portfolio opened numDaysForOscillatingPeriod days ago
    private double[] getPortfolioMovingValues()
    {
    	double[] returnValue = new double[this.portfolioRatesOfReturn.Length];
      double[] valuesInOscillatingPeriod = new double[this.numDaysForOscillatingPeriod];
      valuesInOscillatingPeriod[0] = 1.0;
      for(int i = this.numDaysForOscillatingPeriod; i<returnValue.Length; i++)
      {
        for(int j = 1;
            j < this.numDaysForOscillatingPeriod; j++)
      			  valuesInOscillatingPeriod[j] = 
      			    valuesInOscillatingPeriod[j - 1] + 
                valuesInOscillatingPeriod[j - 1] * 
                  this.portfolioRatesOfReturn[i-this.numDaysForOscillatingPeriod+j];
        returnValue[i-1] = valuesInOscillatingPeriod[this.numDaysForOscillatingPeriod -1];
      }
      return returnValue;
    }

    private void getFitnessValue_setCurrentThresholds(Genome genome)
    {
    	this.currentOversoldThreshold = Convert.ToDouble(genome.Genes()[0])/
                                      Convert.ToDouble(this.divisorForThresholdComputation);
    	this.currentOverboughtThreshold = Convert.ToDouble(genome.Genes()[1])/
                                        Convert.ToDouble(this.divisorForThresholdComputation);
    }
    
    private int getFitnessValue_getDaysOnTheMarket(double[] equityLine)
    {
      int returnValue = 0;
      foreach(double equityReturn in equityLine)
        if(equityReturn > 0.0)
          returnValue++;

      return returnValue;
    }

		//fitness is a sharpe-ratio based indicator for the equity line resulting
		//from applying the strategy
	  public override double GetFitnessValue(Genome genome)
    {
      double returnValue = -1.0;
	  	this.portfolioRatesOfReturn = this.getPortfolioRatesOfReturn(genome.Genes());
      this.portfolioValues = this.getPortfolioMovingValues();
      this.getFitnessValue_setCurrentThresholds(genome);
      double[] equityLine = this.getFitnessValue_getEquityLineRates();
      double fitness = Double.NaN;
      if(this.getFitnessValue_getDaysOnTheMarket(equityLine) >
          equityLine.Length / 2)
      //if the genome represents a portfolio that stays on the market
      //at least half of the theoretical days
        fitness = AdvancedFunctions.GetSharpeRatio(equityLine);
      if(!double.IsNaN(fitness) && !double.IsInfinity(fitness))
      	returnValue = fitness;
      
      return returnValue;
    }
    
    private double[] getFitnessValue_getEquityLineRates()
    {
    	double[] returnValue = new double[this.PortfolioRatesOfReturn.Length];
    	double coefficient = 0.0;
    	for(int i = 0; i<this.PortfolioRatesOfReturn.Length - 1;i++)
      {
    		if(this.portfolioValues[i] >= 
    		   1.0 + this.currentOverboughtThreshold)
    		//portfolio is overbought
    			coefficient = -1.0;
        else if(this.portfolioValues[i] <= 
    		        1.0 - this.currentOversoldThreshold &&
                this.portfolioValues[i] > 0.0)
 				//portfolio is oversold   			
        	coefficient = 1.0;
    		  		
    		returnValue[i + 1] = 
    			coefficient * this.PortfolioRatesOfReturn[i + 1];
      }
      return returnValue;
    }
	  
    protected override double getTickerWeight(int[] genes, int tickerPositionInGenes)
    {
      return 1.0/(genes.Length - 2);
      //two genes are for thresholds
    }

    protected override double[] getPortfolioRatesOfReturn(int[] genes)
    {
      double[] returnValue = new double[this.numberOfExaminedReturns];
      for(int i = 0; i<returnValue.Length; i++)    
      {  
        for(int j=2; j<genes.Length; j++)//the first two genes are for thresholds
          returnValue[i] +=
            this.getPortfolioRatesOfReturn_getRateOfTickerToBeAddedToTheArray(genes,j,i);
      }
      return returnValue;
    }

    public override object Decode(Genome genome)
    {
      string[] arrayOfTickers = new string[genome.Genes().Length - 2];
      int geneForTicker;
      for(int genePosition = 2; genePosition < genome.Genes().Length; genePosition++)
      {
        geneForTicker = (int)genome.Genes().GetValue(genePosition);
        arrayOfTickers[genePosition - 2] = 
                  this.decode_getTickerCodeForLongOrShortTrade(geneForTicker);
      }
      GenomeMeaningPVO meaning = new GenomeMeaningPVO(
                                      arrayOfTickers,
                                      Convert.ToDouble(genome.Genes()[0])/Convert.ToDouble(this.divisorForThresholdComputation),
                                      Convert.ToDouble(genome.Genes()[1])/Convert.ToDouble(this.divisorForThresholdComputation),
                                     	this.numDaysForOscillatingPeriod);
      return meaning;
    }

    public override Genome[] GetChilds(Genome parent1, Genome parent2)
    {
    	//in this simple implementation
    	//child have the tickers of one parent
    	//and the thresholds of the other
    	Genome[] childs = new Genome[2];
    	childs[0] = parent1.Clone();
    	childs[1] = parent2.Clone();
    	//exchange of genes coding thresholds
    	childs[0].SetGeneValue(parent2.GetGeneValue(0),0);
    	childs[0].SetGeneValue(parent2.GetGeneValue(1),1);
    	childs[1].SetGeneValue(parent1.GetGeneValue(0),0);
    	childs[1].SetGeneValue(parent1.GetGeneValue(1),1);
    	return childs;
    }

    public override int GetNewGeneValue(Genome genome, int genePosition)
    {
      // in this implementation only new gene values pointing to tickers
      // must be different from the others already stored
      int minValueForGene = genome.GetMinValueForGenes(genePosition);
      int maxValueForGene = genome.GetMaxValueForGenes(genePosition);
      int returnValue = minValueForGene;
      if( minValueForGene != maxValueForGene)
      {
	      returnValue = GenomeManagement.RandomGenerator.Next(minValueForGene,
	        															maxValueForGene + 1);
	      while(genePosition > 1
	        && GenomeManipulator.IsTickerContainedInGenome(returnValue,genome,2,genome.Size - 1))
	        //while in the given position has to be stored
	        //a new gene pointing to a ticker and
	        //the proposed returnValue points to a ticker
	        //already stored in the given genome
	      {
	        // a new returnValue has to be generated
	        returnValue = GenomeManagement.RandomGenerator.Next(minValueForGene,
	        															maxValueForGene + 1);
	      }
      }
      return returnValue;
    }
        
    public override void Mutate(Genome genome, double mutationRate)
    {
      // in this implementation only one gene is mutated
      int genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
      int minValueForGene = genome.GetMinValueForGenes(genePositionToBeMutated);
      int maxValueForGene = genome.GetMaxValueForGenes(genePositionToBeMutated);
      if(minValueForGenes != maxValueForGenes)
      {
	      int newValueForGene = GenomeManagement.RandomGenerator.Next(minValueForGene,
	        																		maxValueForGene +1);
	       
	      while(genePositionToBeMutated > 1 &&
	        GenomeManipulator.IsTickerContainedInGenome(newValueForGene,genome,2,genome.Size - 1))
	        //while in the proposed genePositionToBeMutated has to be stored
	        //a new gene pointing to a ticker and
	        //the proposed newValueForGene points to a ticker
	        //already stored in the given genome
	      {
	        newValueForGene = GenomeManagement.RandomGenerator.Next(minValueForGene,
	        																		maxValueForGene +1);
	      }
	      GenomeManagement.MutateOneGene(genome, mutationRate,
	        genePositionToBeMutated, newValueForGene);
      }
    }
  }
}
