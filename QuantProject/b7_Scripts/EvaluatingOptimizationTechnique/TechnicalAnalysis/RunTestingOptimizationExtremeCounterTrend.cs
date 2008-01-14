/*
QuantProject - Quantitative Finance Library

RunTestingOptimizationExtremeCounterTrend.cs
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
using QuantProject.ADT.Statistics;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.ExtremeCounterTrend;

namespace QuantProject.Scripts.EvaluatingOptimizationTechnique.TechnicalAnalysis
{
	/// <summary>
	/// For the evaluation of the optimization technique used by the script
	/// for the extreme counter trend strategy
	/// </summary>
  [Serializable]
  public class RunTestingOptimizationExtremeCounterTrend
  {
  	private double[] fitnessesInSample;
  	private double[] fitnessesOutOfSample;
  	private string tickerGroupID;
    private int numberOfEligibleTickers;
    private int numberOfTickersToBeChosen;
    private int numDaysForOptimizationPeriod;
    private int populationSizeForGeneticOptimizer;
    private int generationNumberForGeneticOptimizer;
  	private string benchmark;
  	private DateTime marketDate;
  	private int numDaysForReturnCalculation;
  	private PortfolioType portfolioType;
  	private int numDaysAfterLastOptimizationDay;
  	private int numberOfSubsets;
  	private Genome[] genomesToTestOutOfSample;
  	private int numberOfGenomesToTest;
      	
    public RunTestingOptimizationExtremeCounterTrend(string tickerGroupID, int numberOfEligibleTickers,
      				int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod,
      				int generationNumberForGeneticOptimizer, int populationSizeForGeneticOptimizer, 
      				string benchmark,
      				DateTime marketDate, int numDaysForReturnCalculation,
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
  		this.numDaysForOptimizationPeriod = numDaysForOptimizationPeriod;
  		this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
  		this.generationNumberForGeneticOptimizer = generationNumberForGeneticOptimizer;
  		this.benchmark = benchmark;
  		this.marketDate = marketDate;
  		this.numDaysForReturnCalculation = numDaysForReturnCalculation;
  		this.portfolioType = portfolioType;
  		this.numDaysAfterLastOptimizationDay = numDaysAfterLastOptimizationDay;
  		this.numberOfSubsets = numberOfSubsets;
   	}
    
  	private DataTable getSetOfTickersToBeOptimized(DateTime currentDate)
    {
           	
     	SelectorByGroup temporizedGroup = new SelectorByGroup(this.tickerGroupID,
        																										currentDate);
      
      SelectorByQuotationAtEachMarketDay quotedAtEachMarketDayFromTemporized = 
        new SelectorByQuotationAtEachMarketDay(temporizedGroup.GetTableOfSelectedTickers(),
        false, currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
        600, this.benchmark);
      // filter to be used with plain stocks
//      DataTable tickersQuotedAtEachMarketDay = quotedAtEachMarketDayFromTemporized.GetTableOfSelectedTickers();
//      SelectorByLiquidity mostLiquid =
//      	new SelectorByLiquidity(tickersQuotedAtEachMarketDay,
//      	                        false,currentDate.AddDays(-this.numDaysForOptimizationPeriod), currentDate,
//      	                        tickersQuotedAtEachMarketDay.Rows.Count/2);
//      
//      DataTable mostLiquidTickers = mostLiquid.GetTableOfSelectedTickers();
//      	                        
//      SelectorByCloseToCloseVolatility lessVolatile =
//      	new SelectorByCloseToCloseVolatility(mostLiquidTickers,
//      	                                     true,currentDate.AddDays(-30), currentDate,
//      	                                     Math.Min(this.numberOfEligibleTickers, mostLiquidTickers.Rows.Count/2));
////      return mostLiquid.GetTableOfSelectedTickers();
//      return lessVolatile.GetTableOfSelectedTickers();
      //
      return quotedAtEachMarketDayFromTemporized.GetTableOfSelectedTickers();
    }
  	
    private double setFitnesses_setFitnessesActually_getFitnessOutOfSample_PortfolioClose(Genome portfolioGenome, DateTime date)
  		
    {
      double returnValue = 0.0;
      foreach(string tickerCode in ((GenomeMeaning)portfolioGenome.Meaning).Tickers)
      {
        double coefficient = 1.0;
        string ticker = tickerCode;
        if(ticker.StartsWith("-"))
        {
          ticker = ticker.Substring(1,ticker.Length -1);
          coefficient = -1.0;
        }
        Quotes tickerQuotes = new Quotes(ticker, date,
                                         date);
        returnValue += (tickerQuotes.GetFirstValidCloseToCloseRatio(date) - 1.0)*coefficient;
      }
      return returnValue/portfolioGenome.Size;
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
        DateTime endDateOutOfSample = this.marketDate.AddDays(this.numDaysAfterLastOptimizationDay);
        
        Quotes tickerQuotes = new Quotes(ticker, this.marketDate,
          															 endDateOutOfSample);
        double[] returns = new double[tickerQuotes.Rows.Count];
        for(int i = 1; i<returns.Length; i++)
        {
          if(this.setFitnesses_setFitnessesActually_getFitnessOutOfSample_PortfolioClose(
                genome, (DateTime)tickerQuotes.Rows[i-1]["quDate"])>0.0)
            coefficient = -1.0 * coefficient;
        	returns[i] = (tickerQuotes.GetFirstValidCloseToCloseRatio(
                          (DateTime)tickerQuotes.Rows[i]["quDate"] ) - 1.0)*coefficient;
        }
        returnValue += BasicFunctions.SimpleAverage(returns) / BasicFunctions.StdDev(returns);
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
      
      IGenomeManager genManExtremeCounterTrend =
        new GenomeManagerECT(setOfTickersToBeOptimized,
	                            this.marketDate.AddDays(-this.numDaysForOptimizationPeriod),
	                            this.marketDate,
	                            this.numberOfTickersToBeChosen,
	                            this.numDaysForReturnCalculation,
	                           	this.portfolioType,
	                            this.benchmark);
    
      this.setFitnesses_setFitnessesActually(genManExtremeCounterTrend);
      
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
                    "\\OptimizationEvaluation.txt";
  	  StreamWriter w = File.AppendText(pathFile);
  	  w.WriteLine ("\n----------------------------------------------\r\n");
  	  w.Write("\r\nNew Test for Evaluation of Extreme Counter Trend Optimization {0}\r", DateTime.Now.ToLongDateString()+ " " +DateTime.Now.ToLongTimeString());
  	  w.Write("\r\nNum days for optimization {0}\r", this.numDaysForOptimizationPeriod.ToString());
      w.Write("\r\nOptimizing market date {0}\r", this.marketDate.ToLongDateString());
      w.Write("\r\nMarket date for test out of sample (sharpe ratio as fitness OS){0}\r",
                        this.marketDate.AddDays(this.numDaysAfterLastOptimizationDay).ToLongDateString());
      w.Write("\r\nNumber of tickers: {0}\r", this.numberOfTickersToBeChosen.ToString());
      w.WriteLine ("\n----------------------------------------------");
  	  w.Write("\r\nFitnesses compared (sharpe r. OTC - sharpe r. CTO): {0}\r", this.fitnessesInSample.Length.ToString());
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
