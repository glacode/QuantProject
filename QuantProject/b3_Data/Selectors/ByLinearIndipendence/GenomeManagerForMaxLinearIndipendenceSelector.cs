/*
QuantProject - Quantitative Finance Library

GenomeManagerForMaxLinearIndipendenceSelector.cs
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


namespace QuantProject.Data.Selectors.ByLinearIndipendence
{
	/// <summary>
	/// IGenomeManager used by SelectorByMaxLinearIndipendence
	/// </summary>
	[Serializable]
  public class GenomeManagerForMaxLinearIndipendenceSelector : IGenomeManager
  {
    private DataTable setOfInitialTickers;
    private Candidate[] candidates;
    private DateTime firstQuoteDate;
    private DateTime lastQuoteDate;
    private int genomeSize;
    private int minValueForGenes;
    private int maxValueForGenes;
    private double[,] correlationMatrix;
    
    //IGenomeManager implementation for properties 
    public int GenomeSize
    {
      get{return this.genomeSize;}
    }
    
    //end of interface implementation for properties

    public GenomeManagerForMaxLinearIndipendenceSelector(DataTable setOfInitialTickers,
                                    DateTime firstQuoteDate,
                                    DateTime lastQuoteDate,
                                    int numOfIndipendentTickersToBeReturned)                          
    {
      this.genomeSize = numOfIndipendentTickersToBeReturned;
      this.setOfInitialTickers = setOfInitialTickers;
      this.setMinAndMaxValueForGenes();
      this.firstQuoteDate = firstQuoteDate;
      this.lastQuoteDate = lastQuoteDate;
      this.candidates = new Candidate[this.setOfInitialTickers.Rows.Count];
      this.retrieveData();
    }
    
    public int GetMinValueForGenes(int genePosition)
    {
      return this.minValueForGenes;
    }
    
    public int GetMaxValueForGenes(int genePosition)
    {
      return this.maxValueForGenes;
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

    private void retrieveData_setCorrelationMatrix()
    {
      this.correlationMatrix = 
        new double[this.candidates.Length,this.candidates.Length];
      for(int i = 0;i<this.candidates.Length;i++)
      {
        for(int j = 0; j<i; j++)
        //matrix is symmetric, only the lower half is computed
          this.correlationMatrix[i,j] = 
            BasicFunctions.PearsonCorrelationCoefficient(this.candidates[i].ArrayOfRatesOfReturn,
                         this.candidates[j].ArrayOfRatesOfReturn);
      }

    }

    private void retrieveData()
    {
      for(int i = 0; i<this.setOfInitialTickers.Rows.Count; i++)
      {
        string ticker = (string)setOfInitialTickers.Rows[i][0];
        this.candidates[i] = new Candidate(ticker,
                                  this.retrieveData_getArrayOfAdjustedCloseQuotes(ticker));
      }
      this.retrieveData_setCorrelationMatrix();
    }

   
    public Genome[] GetChilds(Genome parent1, Genome parent2)
    {
      return
        GenomeManagement.MixGenesWithoutDuplicates(parent1, parent2);
    }
    
    public int GetNewGeneValue(Genome genome, int genePosition)
    {
    	int returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
    	                                                        genome.GetMaxValueForGenes(genePosition) + 1);
      while( genome.HasGene(returnValue) )
        //the genome to be examined shouldn't have a duplicate
      {
        returnValue = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePosition),
    	                                                        genome.GetMaxValueForGenes(genePosition) + 1);
      }
      return returnValue;
    }
        
    public void Mutate(Genome genome, double mutationRate)
    {
      
      int genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
      int newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
    	                                                        genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);
      while( genome.HasGene(newValueForGene) )
      //genome shouldn't have a duplicated gene
      {
        newValueForGene = GenomeManagement.RandomGenerator.Next(genome.GetMinValueForGenes(genePositionToBeMutated),
    	                                                        genome.GetMaxValueForGenes(genePositionToBeMutated) + 1);
      }
      GenomeManagement.MutateOneGene(genome, mutationRate,
        genePositionToBeMutated, newValueForGene);
    }

    public object Decode(Genome genome)
    {
      string[] returnValue = new string[genome.Size];
      for(int i = 0; i<genome.Size; i++)
        returnValue[i] = 
          (string)this.setOfInitialTickers.Rows[genome.GetGeneValue(i)][0];
      
      return returnValue;
    }

    private double getFitnessValue_getCorrelationCoefficient(int i, int j)
    {
      int row = i;
      int column = j;
      if( i < j )
      {
        row = j;
        column = i;
      }
      return this.correlationMatrix[row,column];
    }

    public virtual double GetFitnessValue(Genome genome)
    {
      double maximumCorrelation = 0.0;
      for( int i = 0; i <= genome.Size - 2; i++)
      {
        for( int j = 1; j<= genome.Size - i  - 1;j++)
           maximumCorrelation = Math.Max(maximumCorrelation,
        	                              Math.Abs(this.getFitnessValue_getCorrelationCoefficient(i,j)));
      }
      return 1.0 / maximumCorrelation;
    }

  }

}
