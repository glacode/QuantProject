/*
QuantProject - Quantitative Finance Library

RunTestingOptimizationOpenToClose.cs
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

using QuantProject.ADT;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Data.Selectors;
using QuantProject.Data.DataTables;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.EvaluatingOptimizationTechnique.EfficientPortfolio
{
	/// <summary>
	/// For the evaluation of the optimization technique used by the script
	/// for the efficient open to close portfolio
	/// </summary>
  [Serializable]
  public class RunTestingOptimizationOpenToClose
  {
  	private double[] fitnessesInSample;
  	private double[] fitnessesOutOfSample;
  	private string tickerGroupID;
    private int numberOfEligibleTickers;
    private int numberOfTickersToBeChosen;
    private int numDaysForLiquidity;
    private int populationSizeForGeneticOptimizer;
  	private string benchmark;
  	private DateTime marketDate;
  	private double targetReturn;
  	private PortfolioType portfolioType;
  	private int numDaysAfterLastOptimizationDay;
  	private int numberOfSubsets;
  	
    public RunTestingOptimizationOpenToClose(string tickerGroupID, int numberOfEligibleTickers,
      				int numberOfTickersToBeChosen, int numDaysForLiquidity, 
      				int populationSizeForGeneticOptimizer, string benchmark,
      				DateTime marketDate, double targetReturn,
      				PortfolioType portfolioType, int numDaysAfterLastOptimizationDay)
    {
  		this.fitnessesInSample = new double[populationSizeForGeneticOptimizer];
  		this.fitnessesOutOfSample = new double[populationSizeForGeneticOptimizer];
  		this.tickerGroupID = tickerGroupID;
  		this.numberOfEligibleTickers = numberOfEligibleTickers;
  		this.numberOfTickersToBeChosen = numberOfTickersToBeChosen;
  		this.numDaysForLiquidity = numDaysForLiquidity;
  		this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
  		this.benchmark = benchmark;
  		this.marketDate = marketDate;
  		this.targetReturn = targetReturn;
  		this.portfolioType = portfolioType;
  		this.numDaysAfterLastOptimizationDay = numDaysAfterLastOptimizationDay;
  		this.numberOfSubsets = 5;
    }
    
  	private DataTable getSetOfTickersToBeOptimized(DateTime date)
    {
      /*
      SelectorByAverageRawOpenPrice selectorByOpenPrice = 
                  new SelectorByAverageRawOpenPrice(this.tickerGroupID, false,
                          currentDate.AddDays(-this.numDaysForLiquidity), currentDate,
                          this.numberOfEligibleTickers, this.minPriceForMinimumCommission,
                          this.maxPriceForMinimumCommission, 0, 2);
      DataTable tickersByPrice = selectorByOpenPrice.GetTableOfSelectedTickers();
      */
     	
     	SelectorByLiquidity mostLiquid = new SelectorByLiquidity(this.tickerGroupID, false,
                                      date.AddDays(-this.numDaysForLiquidity), date,
                                      this.numberOfEligibleTickers);
      /*SelectorByOpenToCloseVolatility lessVolatile = 
      	new SelectorByOpenToCloseVolatility(mostLiquid.GetTableOfSelectedTickers(),
      	                                    true, currentDate.AddDays(-5),
      	                                    currentDate,
      	                                    this.numberOfEligibleTickers/2);*/
      DataTable eligibleTickers;
      eligibleTickers = mostLiquid.GetTableOfSelectedTickers();
      SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromEligible = 
        new SelectorByQuotationAtEachMarketDay( eligibleTickers,
                                   false, date.AddDays(-this.numDaysForLiquidity),
                                    date, this.numberOfEligibleTickers, this.benchmark);
      //SelectorByWinningOpenToClose winners =
      //	new SelectorByWinningOpenToClose(quotedAtEachMarketDayFromMostLiquid.GetTableOfSelectedTickers(),
      //	                                 false, currentDate.AddDays(-2),
      //	                                 currentDate, this.numberOfEligibleTickers/4);      	                                 
      //return winners.GetTableOfSelectedTickers();
      //SelectorByOpenCloseCorrelationToBenchmark lessCorrelated = 
      //  new SelectorByOpenCloseCorrelationToBenchmark(quotedAtEachMarketDayFromEligible.GetTableOfSelectedTickers(),
      //                                                this.benchmark, true,
      //                                                currentDate.AddDays(-this.numDaysForLiquidity),
      //                                                currentDate, this.numberOfEligibleTickers/2);
      return quotedAtEachMarketDayFromEligible.GetTableOfSelectedTickers();
      //return lessCorrelated.GetTableOfSelectedTickers();
    }
  	
  	private double setFitnesses_setFitnessesActually_getFitnessOutOfSample(Genome genome)
  		
    {
 			double returnValue = 0;
 			foreach(string tickerCode in (string[])genome.Meaning)
 			{
 				double coefficient = 1.0;
 				string ticker = tickerCode;
 				if(ticker.StartsWith("-"))
 				{
 					ticker = ticker.Substring(1,ticker.Length -1);
 					coefficient = -1.0;
 				}
 				DateTime dateOutOfSample = this.marketDate.AddDays(this.numDaysAfterLastOptimizationDay);
 				Quotes tickerQuotes = new Quotes(ticker, dateOutOfSample,
 				                                 dateOutOfSample);
 				returnValue += 
 					(tickerQuotes.GetFirstValidRawClose(dateOutOfSample)/
 					 tickerQuotes.GetFirstValidRawOpen(dateOutOfSample) - 1.0)*coefficient;
	 			
 			}
 			return returnValue/genome.Size;
      
    }
  	
  	private void setFitnesses_setFitnessesActually(GeneticOptimizer GO)
  		
    {
  		
  		for(int i = 0; i<GO.CurrentGeneration.Count; i++)
  		{
  			this.fitnessesInSample[i]=((Genome)GO.CurrentGeneration[i]).Fitness;
  			this.fitnessesOutOfSample[i]= 
  				this.setFitnesses_setFitnessesActually_getFitnessOutOfSample((Genome)GO.CurrentGeneration[i]);
  		}
      
    }
  	
  	private void setFitnesses()
  		
    {
      
      DataTable setOfTickersToBeOptimized = 
      	this.getSetOfTickersToBeOptimized(this.marketDate);
       IGenomeManager genManEfficientCTOPortfolio = 
        new GenomeManagerForEfficientCTOPortfolio(setOfTickersToBeOptimized,
      	                                          this.marketDate.AddDays(-this.numDaysForLiquidity),
      	                                          this.marketDate,
      	                                          this.numberOfTickersToBeChosen,
      	                                          this.targetReturn,
      	                                         	this.portfolioType);
      
      GeneticOptimizer GO = new GeneticOptimizer(genManEfficientCTOPortfolio,
                                                  this.populationSizeForGeneticOptimizer,
                                                  0,
                                                 ConstantsProvider.SeedForRandomGenerator);
          
      GO.Run(false);
      this.setFitnesses_setFitnessesActually(GO);
      
    }
  	
    public void Run()
    {
    	this.setFitnesses();
    	OptimizationTechniqueEvaluator evaluator = 
    		new OptimizationTechniqueEvaluator(this.fitnessesInSample,
    		                                   this.fitnessesOutOfSample);
    	double[] averagesInSample = 
    		evaluator.GetAveragesOfSubsetsInSample(this.numberOfSubsets);
    	double[] averagesOutOfSample = 
    		evaluator.GetAveragesOfSubsetsOutOfSample(this.numberOfSubsets);
    	double r = evaluator.GetCorrelationBetweenFitnesses();
    }
	}
}
