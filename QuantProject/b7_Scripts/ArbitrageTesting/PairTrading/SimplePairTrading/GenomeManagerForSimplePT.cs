/*
QuantProject - Quantitative Finance Library

GenomeManagerForSimplePT.cs
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


namespace QuantProject.Scripts.ArbitrageTesting.PairTrading.SimplePairTrading
{
	/// <summary>
	/// This class implements IGenomeManager interface, in order to find 
	/// tickers that best suite the pair trading strategy
	/// </summary>
	[Serializable]
  public class GenomeManagerForSimplePT : IGenomeManager
  {
    private DataTable setOfInitialTickers;
    private PairTradingCandidate[] candidates;
    private DateTime firstQuoteDate;
    private DateTime lastQuoteDate;
    private int minNumOfDaysForGapComputation;
    private int maxNumOfDaysForGapComputation;
    private double maxNumOfStdDevForNormalGap;
    private int genomeSize;
    private int minValueForGenes;
    private int maxValueForGenes;
    private float[] gaps;
    //it will contain absolute gaps of gain for tickers
    private double[] pairTradingPortfolioGains;

    //IGenomeManager implementation for properties 
    public int GenomeSize
    {
      get{return this.genomeSize;}
    }
    
    public int GetMinValueForGenes(int genePosition)
    {
      return this.minValueForGenes;
    }
    
    public int GetMaxValueForGenes(int genePosition)
    {
      return this.maxValueForGenes;
    }
    
    //end of interface implementation for properties

    public GenomeManagerForSimplePT(DataTable setOfInitialTickers,
                                    DateTime firstQuoteDate,
                                    DateTime lastQuoteDate,
                                    int minNumOfDaysForGapComputation,
                                    int maxNumOfDaysForGapComputation,
                                    double maxNumOfStdDevForNormalGap)                          
    {
      this.genomeSize = 3;//1° pos for numIntervalDays;
      //2° and 3° positions for tickers
      this.setOfInitialTickers = setOfInitialTickers;
      this.setMinAndMaxValueForGenes();
      this.firstQuoteDate = firstQuoteDate;
      this.lastQuoteDate = lastQuoteDate;
      this.minNumOfDaysForGapComputation = minNumOfDaysForGapComputation;
      this.maxNumOfDaysForGapComputation = maxNumOfDaysForGapComputation;
      this.maxNumOfStdDevForNormalGap = maxNumOfStdDevForNormalGap;
      this.candidates = new PairTradingCandidate[this.setOfInitialTickers.Rows.Count];
      this.retrieveData();
      this.pairTradingPortfolioGains = new double[this.candidates[0].ArrayOfAdjustedCloseQuotes.Length];
    }
    
    private void setMinAndMaxValueForGenes()
    {
      this.minValueForGenes = 0;
      this.maxValueForGenes = this.setOfInitialTickers.Rows.Count - 1;
    }

    private float[] retrieveData_getArrayOfAdjustedCloseQuotes(string ticker)
    {
      float[] returnValue = null;
      Quotes tickerQuotes = new Quotes(ticker, this.firstQuoteDate, this.lastQuoteDate);
      returnValue = ExtendedDataTable.GetArrayOfFloatFromColumn(tickerQuotes,"quAdjustedClose");
      return returnValue;
    }

    private void retrieveData()
    {
      for(int i = 0; i<this.setOfInitialTickers.Rows.Count; i++)
      {
        string ticker = (string)setOfInitialTickers.Rows[i][0];
        this.candidates[i] = new PairTradingCandidate(ticker,
                                  this.retrieveData_getArrayOfAdjustedCloseQuotes(ticker));
      }
    }

    private void getFitnessValue_setGaps(Genome genome)
    {
      int numOfDaysForGap = genome.GetGeneValue(0);
      int gapsLength = 
        (this.candidates[0].ArrayOfAdjustedCloseQuotes.Length - 1)/numOfDaysForGap;
      this.gaps = new float[gapsLength];
      int j = 0;//counter for gaps array
      for(int i = 0;i<gapsLength-numOfDaysForGap;i += numOfDaysForGap)
      {
        this.gaps[j] = 
        this.candidates[genome.GetGeneValue(0)].ArrayOfAdjustedCloseQuotes[i+numOfDaysForGap]/
                  this.candidates[genome.GetGeneValue(0)].ArrayOfAdjustedCloseQuotes[i] - 
                  this.candidates[genome.GetGeneValue(1)].ArrayOfAdjustedCloseQuotes[i+numOfDaysForGap]/
                  this.candidates[genome.GetGeneValue(1)].ArrayOfAdjustedCloseQuotes[i] ;
        j++;
      }
    }

    public double GetFitnessValue(Genome genome)
    {
      double returnValue = 0.0;
      this.getFitnessValue_setGaps(genome);
      //float maxGap = this.maxNumOfStdDevForNormalGap * 
//      foreach(float gap in this.gaps)
      
       
      //utilizzare l'equity line della strategia

      //individuare i giorni in cui si entra nel mercato
      //in base al segnale (gap > maxGap) ? ottimizzare il maxGap?
      //crea portafoglio di pair trading
      //calcola gain o stop loss ad ogni chiusura in base agli adjCloses
      //se gain o stopLoss non fanno uscire,
      //aggiungi ritorno al portafoglio di pair trading
      //se gain o stopLoss fanno uscire, azzera gain e stopLoss
      //ripeti ciclo esaminando primo gap utile
      //esauriti i gap, 
      //calcolare sharpeRatio di equityLine
      
      returnValue = (1/Math.Abs(BasicFunctions.SimpleAverage(this.gaps)));// /BasicFunctions.StdDev(this.gaps);
      if(Double.IsNaN(returnValue) || Double.IsInfinity(returnValue))
        returnValue = 0.0;
      return returnValue;
    }
   
    public Genome[] GetChilds(Genome parent1, Genome parent2)
    {
      return
        SimplePTGenomeManipulator.MixGenesWithoutDuplicates(parent1, parent2);
    }
    
    public int GetNewGeneValue(Genome genome, int genePosition)
    {
      int returnValue;
      if(genePosition == 0)
        //this positions needs the n° of days for the gap 
        //between tickers'gains 
      {
        returnValue = GenomeManagement.RandomGenerator.Next(this.minNumOfDaysForGapComputation,
          this.maxNumOfDaysForGapComputation + 1);
      }
      else
        //in 2° or 3° position there must be a ticker different from
        // the one contained in 3° or 2° position
      {
      	returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
      	                                                    genome.GetMaxValueForGenes(genePosition) + 1);
        while(genePosition>0 
          && SimplePTGenomeManipulator.IsTickerContainedInGenome(returnValue,
                                                                 genome))
          //while in the given position has to be stored
          //a new gene pointing to a ticker and
          //the proposed returnValue points to a ticker
          //already stored in the given genome
        {
          // a new returnValue has to be generated
          returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
                                                              genome.GetMaxValueForGenes(genePosition) + 1);
        }
      }
      return returnValue;
    }
        
    public void Mutate(Genome genome, double mutationRate)
    {
      int newValueForGene;
      int genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size); 
      if(genePositionToBeMutated == 0)
        //this positions needs the n° of days for the gap 
        //between tickers'gains 
      {
        newValueForGene = GenomeManagement.RandomGenerator.Next(this.minNumOfDaysForGapComputation,
          this.maxNumOfDaysForGapComputation + 1);
      }
      else
      {
      	newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
      	                                                        genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);
        while(genePositionToBeMutated>0 &&
                SimplePTGenomeManipulator.IsTickerContainedInGenome(newValueForGene,
                                                                    genome))
                //while in the proposed genePositionToBeMutated has to be stored
                //a new gene pointing to a ticker and
                //the proposed newValueForGene points to a ticker
                //already stored in the given genome
        {
          newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
      	                                                          genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);
        }

      }
      GenomeManagement.MutateOneGene(genome, mutationRate,
        genePositionToBeMutated, newValueForGene);
    }

    public object Decode(Genome genome)
    {
      int numOfDaysForGap = genome.GetGeneValue(0);
      string firstTicker = this.candidates[genome.GetGeneValue(1)].Ticker;
      string secondTicker = this.candidates[genome.GetGeneValue(2)].Ticker;
      double averageGap = BasicFunctions.SimpleAverage(this.gaps);
      double stdDevGap = BasicFunctions.StdDev(this.gaps);
      return new GenomeMeaningSimplePT(numOfDaysForGap, averageGap,
                                       stdDevGap,
                                       firstTicker, secondTicker);
    }

  }

}
