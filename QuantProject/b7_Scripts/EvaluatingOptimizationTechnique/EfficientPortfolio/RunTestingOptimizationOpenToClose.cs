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
using System.IO;

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
    private int numDaysForOptimization;
    private int populationSizeForGeneticOptimizer;
    private int generationNumberForGeneticOptimizer;
  	private string benchmark;
  	private DateTime marketDate;
  	private double targetReturn;
  	private PortfolioType portfolioType;
  	private int numDaysAfterLastOptimizationDay;
  	private int numberOfSubsets;
  	private Genome[] genomesToTestOutOfSample;
  	private int numberOfGenomesToTest;
      	
    public RunTestingOptimizationOpenToClose(string tickerGroupID, int numberOfEligibleTickers,
      				int numberOfTickersToBeChosen, int numDaysForOptimization,
      				int generationNumberForGeneticOptimizer, int populationSizeForGeneticOptimizer, 
      				string benchmark,
      				DateTime marketDate, double targetReturn,
      				PortfolioType portfolioType, int numDaysAfterLastOptimizationDay,
      				int numberOfSubsets, int numberOfGenomesToTest)
    {
  		this.numberOfGenomesToTest = numberOfGenomesToTest;
  		this.genomesToTestOutOfSample = new Genome[numberOfGenomesToTest];
  		this.fitnessesInSample = new double[numberOfGenomesToTest];
  		this.fitnessesOutOfSample = new double[numberOfGenomesToTest];
  		this.tickerGroupID = tickerGroupID;
  		this.numberOfEligibleTickers = numberOfEligibleTickers;
  		this.numberOfTickersToBeChosen = numberOfTickersToBeChosen;
  		this.numDaysForOptimization = numDaysForOptimization;
  		this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
  		this.generationNumberForGeneticOptimizer = generationNumberForGeneticOptimizer;
  		this.benchmark = benchmark;
  		this.marketDate = marketDate;
  		this.targetReturn = targetReturn;
  		this.portfolioType = portfolioType;
  		this.numDaysAfterLastOptimizationDay = numDaysAfterLastOptimizationDay;
  		this.numberOfSubsets = numberOfSubsets;
   	}
    
  	private DataTable getSetOfTickersToBeOptimized(DateTime date)
    {
      /*
      SelectorByAverageRawOpenPrice selectorByOpenPrice = 
                  new SelectorByAverageRawOpenPrice(this.tickerGroupID, false,
                          currentDate.AddDays(-this.numDaysForOptimization), currentDate,
                          this.numberOfEligibleTickers, this.minPriceForMinimumCommission,
                          this.maxPriceForMinimumCommission, 0, 2);
      DataTable tickersByPrice = selectorByOpenPrice.GetTableOfSelectedTickers();
      */
     	
     	SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID,
      	                                                    date);
      
      SelectorByOpenCloseCorrelationToBenchmark lessCorrelatedFromTemporizedGroup = 
        new SelectorByOpenCloseCorrelationToBenchmark(temporizedGroup.GetTableOfSelectedTickers(),
                                          this.benchmark,true,
                                          date.AddDays(-this.numDaysForOptimization ),
                                          date,
                                         this.numberOfEligibleTickers);
      
      DataTable eligibleTickers;
      eligibleTickers = lessCorrelatedFromTemporizedGroup.GetTableOfSelectedTickers();
      //eligibleTickers = temporizedGroup.GetTableOfSelectedTickers();
      SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromEligible = 
        new SelectorByQuotationAtEachMarketDay( eligibleTickers,
                                   false, date.AddDays(-this.numDaysForOptimization),
                                    date, this.numberOfEligibleTickers, this.benchmark);
      //SelectorByWinningOpenToClose winners =
      	//new SelectorByWinningOpenToClose(quotedAtEachMarketDayFromEligible.GetTableOfSelectedTickers(),
      	  //                               false, date.AddDays(-1),
      	    //                             date.AddDays(-1), this.numberOfEligibleTickers/2,
              //                            true);      	                                 
      //return winners.GetTableOfSelectedTickers();
     
      return quotedAtEachMarketDayFromEligible.GetTableOfSelectedTickers();
      //return lessCorrelated.GetTableOfSelectedTickers();
    }
  	
    private double setFitnesses_setFitnessesActually_getFitnessOutOfSample(Genome genome)
  		
    {
      double returnValue = 0;
      foreach(string tickerCode in ((GenomeMeaning)genome.Meaning).Tickers)
      {
        double coefficient = 1.0;
        string ticker = tickerCode;
        if(ticker.StartsWith("-"))
        {
          ticker = ticker.Substring(1,ticker.Length -1);
          coefficient = -1.0;
        }
        DateTime dateOutOfSample = this.marketDate.AddDays(this.numDaysAfterLastOptimizationDay);
        //returnValue is the single return for the numDaysAfterLastOptimizationDay - th day
        //after the given market date
        
        //Quotes tickerQuotes = new Quotes(ticker, dateOutOfSample,
        //  															 dateOutOfSample);
        
        //returnValue +=
        //  (tickerQuotes.GetFirstValidRawClose(dateOutOfSample)/
        //  tickerQuotes.GetFirstValidRawOpen(dateOutOfSample) - 1.0)*coefficient;
	 			//
        //returnValue is the average return for the interval between
	 			//the given market date and the numDaysAfterLastOptimizationDay - th
	 			//day after the given market date
	 			Quotes tickerQuotes = new Quotes(ticker, this.marketDate,
          															 dateOutOfSample);
        double close, open;
	 			for(int i = 0; i<this.numDaysAfterLastOptimizationDay; i++)
	 			{
		      close = tickerQuotes.GetFirstValidRawClose(this.marketDate.AddDays(i));
		      open = tickerQuotes.GetFirstValidRawOpen(this.marketDate.AddDays(i));
	 				returnValue +=
		      	(close/open - 1.0)*coefficient/this.numDaysAfterLastOptimizationDay;
		      	
	 			}
      }
      return returnValue/genome.Size;
      
    }
    
    private bool setFitnesses_setFitnessesActually_setGenomesToTestOutOfSample_addGenomes_sharesNoGeneWithGenomesAlreadyAdded(Genome genomeToBeAdded,
                                                                                                                              bool addWorstGenomes)
  		
    {
      bool returnValue = true;
      if(addWorstGenomes)
      //the first half containing the worst genomes has to be checked
      {
        for(int i = 0; i<this.numberOfGenomesToTest/2; i++)
        {
          if(this.genomesToTestOutOfSample[i]==null)
            return true;
          if(!genomeToBeAdded.SharesNoGeneWith(this.genomesToTestOutOfSample[i]))
            return false;
        }
      }
      else
      //the second half containing the best genomes has to be checked
      {
        for(int i = 0; i<this.numberOfGenomesToTest/2; i++)
        {
          if(this.genomesToTestOutOfSample[this.numberOfGenomesToTest-1-i]==null)
            return true;
          if(!genomeToBeAdded.SharesNoGeneWith(this.genomesToTestOutOfSample[this.numberOfGenomesToTest-1-i]))
            return false;
        }
      }
      return returnValue;
    }
    private void setFitnesses_setFitnessesActually_setGenomesToTestOutOfSample_addGenomes(GeneticOptimizer optimizer,
                                                                                      bool addWorstGenomes)
  		
    {
      Genome currentGenome;
      Genome previousGenome = null;
      int numOfDifferentGenomesFound = 0;
      for(int j = 0;
        j<this.populationSizeForGeneticOptimizer && numOfDifferentGenomesFound<this.numberOfGenomesToTest/2;
        j++)
      {
        if(addWorstGenomes == true)
          currentGenome = (Genome)optimizer.CurrentGeneration[j];
        else
          currentGenome = (Genome)optimizer.CurrentGeneration[this.populationSizeForGeneticOptimizer-j-1];

        if(this.setFitnesses_setFitnessesActually_setGenomesToTestOutOfSample_addGenomes_sharesNoGeneWithGenomesAlreadyAdded(currentGenome, addWorstGenomes))
        //no genes of the current genome are present in the relative half
        {
          if(this.genomesToTestOutOfSample[numOfDifferentGenomesFound]!= null)
          //the first half of the array has already been filled
            this.genomesToTestOutOfSample[this.numberOfGenomesToTest-1-numOfDifferentGenomesFound]=currentGenome;
          else//the first half is still empty
            this.genomesToTestOutOfSample[numOfDifferentGenomesFound] = currentGenome;
          previousGenome = currentGenome;
          numOfDifferentGenomesFound++;
        }
        
      }
     
    }
  	
    private void setFitnesses_setFitnessesActually_setGenomesToTestOutOfSample(IGenomeManager genomeManager)
  		
    {
      GeneticOptimizer optimizer = new GeneticOptimizer(genomeManager,
                                              this.populationSizeForGeneticOptimizer,
                                              this.generationNumberForGeneticOptimizer,
                                              ConstantsProvider.SeedForRandomGenerator);
      optimizer.Run(false);
      this.setFitnesses_setFitnessesActually_setGenomesToTestOutOfSample_addGenomes(optimizer,
                                                                                 true);
      this.setFitnesses_setFitnessesActually_setGenomesToTestOutOfSample_addGenomes(optimizer,
                                                                                false);
      Array.Sort(this.genomesToTestOutOfSample);
    }
    
    private void setFitnesses_setFitnessesActually(IGenomeManager genomeManager)
  		
    {
      this.setFitnesses_setFitnessesActually_setGenomesToTestOutOfSample(genomeManager);
      for(int i = 0; i<this.numberOfGenomesToTest; i++)
      {
        this.fitnessesInSample[i]=(this.genomesToTestOutOfSample[i]).Fitness;
        this.fitnessesOutOfSample[i]= 
          this.setFitnesses_setFitnessesActually_getFitnessOutOfSample(this.genomesToTestOutOfSample[i]);
      }
      
    }

    private void setFitnesses()
  		
    {
      
      DataTable setOfTickersToBeOptimized = 
      	this.getSetOfTickersToBeOptimized(this.marketDate);
       IGenomeManager genManEfficientCTOPortfolio = 
        new GenomeManagerForEfficientCTOPortfolio(setOfTickersToBeOptimized,
      	                                          this.marketDate.AddDays(-this.numDaysForOptimization),
      	                                          this.marketDate,
      	                                          this.numberOfTickersToBeChosen,
      	                                          this.targetReturn,
      	                                         	this.portfolioType);
    
      this.setFitnesses_setFitnessesActually(genManEfficientCTOPortfolio);
      
    }
  	
    public void Run()
    {
      try
      {
        this.setFitnesses();
        OptimizationTechniqueEvaluator evaluator = 
          new OptimizationTechniqueEvaluator(this.fitnessesInSample,
          this.fitnessesOutOfSample);
        this.run_writeToLogFile(evaluator);
      }
      catch(Exception ex)
      {
        ex = ex;
      }
    }
    
    private void run_writeToLogFile(OptimizationTechniqueEvaluator evaluator)
    {
    	double[] averagesInSample = 
    		evaluator.GetAveragesOfSubsetsInSample(this.numberOfSubsets);
    	double[] averagesOutOfSample = 
    		evaluator.GetAveragesOfSubsetsOutOfSample(this.numberOfSubsets);
    	double r = evaluator.GetCorrelationBetweenFitnesses();
    	GenomeCounter genomeCounter = new GenomeCounter(this.genomesToTestOutOfSample);
    	int differentEvaluatedGenomes = genomeCounter.TotalEvaluatedGenomes;
     	string pathFile = System.Configuration.ConfigurationSettings.AppSettings["GenericArchive"] +
                    "\\OpenToCloseOptimizationEvaluation.txt";
  	  StreamWriter w = File.AppendText(pathFile);
  	  w.WriteLine ("\n----------------------------------------------\r\n");
  	  w.Write("\r\nNew Test for Evaluation of Open To Close Optimization {0}\r", DateTime.Now.ToLongDateString()+ " " +DateTime.Now.ToLongTimeString());
  	  w.Write("\r\nNum days for optimization {0}\r", this.numDaysForOptimization.ToString());
      w.Write("\r\nOptimizing market date {0}\r", this.marketDate.ToLongDateString());
      w.Write("\r\nMarket date for test (out of sample){0}\r",
                        this.marketDate.AddDays(this.numDaysAfterLastOptimizationDay).ToLongDateString());
      w.Write("\r\nNumber of tickers: {0}\r", this.numberOfTickersToBeChosen.ToString());
      w.WriteLine ("\n----------------------------------------------");
  	  w.Write("\r\nFitnesses compared: {0}\r", this.fitnessesInSample.Length.ToString());
  	  w.Write("\r\nDifferent evaluated genomes: {0}\r", differentEvaluatedGenomes.ToString());
      w.Write("\r\nAverages of the {0} sub sets of fitnesses In Sample:\r",
  	          this.numberOfSubsets);
      //
      for(int i = 0; i<averagesInSample.Length; i++)
        	w.WriteLine("\n{0}-->{1}", i.ToString(), averagesInSample[i].ToString());
      
      w.WriteLine ("\n\n----------------------------------------------");
      w.Write("\r\nAverages of the {0} sub sets of fitnesses Out of Sample:\r",
  	          this.numberOfSubsets);
      //
      for(int i = 0; i<averagesOutOfSample.Length; i++)
        	w.WriteLine("\n{0}-->{1}", i.ToString(), averagesOutOfSample[i].ToString());	
      w.WriteLine ("\n\n----------------------------------------------");
      //
      w.Write("\r\nCorrelation coefficient between fitnesses: {0}\r", r.ToString());
      w.WriteLine ("\n-----------------End of Test------------------\r\n");
      // Update the underlying file.
      w.Flush();
      w.Close();
    }	
	}
}
