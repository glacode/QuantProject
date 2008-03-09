/*
QuantProject - Quantitative Finance Library

PVOGenomeManager.cs
Copyright (C) 2008 
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
using QuantProject.Business.DataProviders;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.TickersRelationships;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.Optimizing.Decoding;
using QuantProject.Business.Strategies.Optimizing.GenomeManagers;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator
{
	/// <summary>
	/// Implements what needed to use the Genetic Optimizer
	/// for finding the portfolio that best suites
	/// the Portfolio Value Oscillator strategy
	/// </summary>
	[Serializable]
  public class PVOGenomeManager : BasicGenomeManager
  {
    protected int minLevelForOversoldThreshold;
    protected int maxLevelForOversoldThreshold;
    protected int minLevelForOverboughtThreshold;
    protected int maxLevelForOverboughtThreshold;
    protected int divisorForThresholdComputation;
    protected bool symmetricalThresholds;
    protected bool overboughtMoreThanOversoldForFixedPortfolio;
    protected int numOfGenesDedicatedToThresholds;
   	protected int numDaysForOscillatingPeriod;
    protected CorrelationProvider correlationProvider;//used for experimental
    //tests using 2 tickers and PearsonCorrelationCoefficient as fitness
        
    private void genomeManagerPVO_checkParametersForThresholdsComputation()
    {
      if(this.maxLevelForOverboughtThreshold < this.minLevelForOverboughtThreshold || 
         this.maxLevelForOversoldThreshold < this.minLevelForOversoldThreshold ||
         this.divisorForThresholdComputation < this.maxLevelForOverboughtThreshold ||
         this.divisorForThresholdComputation < this.maxLevelForOversoldThreshold ||
         (this.symmetricalThresholds && (this.minLevelForOversoldThreshold != this.minLevelForOverboughtThreshold ||
                                        this.maxLevelForOversoldThreshold != this.maxLevelForOverboughtThreshold) ) ||
         (this.overboughtMoreThanOversoldForFixedPortfolio && 
            (this.minLevelForOverboughtThreshold > Convert.ToInt32(Convert.ToDouble(this.minLevelForOversoldThreshold)* Convert.ToDouble(this.divisorForThresholdComputation) /
                                                     (Convert.ToDouble(this.divisorForThresholdComputation) - Convert.ToDouble(this.minLevelForOversoldThreshold) ) )  ||
              this.maxLevelForOverboughtThreshold < Convert.ToInt32(Convert.ToDouble(this.maxLevelForOversoldThreshold) * Convert.ToDouble(this.divisorForThresholdComputation) /
                                                     (Convert.ToDouble(this.divisorForThresholdComputation) - Convert.ToDouble(this.maxLevelForOversoldThreshold) ) ) ) ) ) 

          throw new Exception("Bad parameters for thresholds computation!");
    }

    public PVOGenomeManager(EligibleTickers eligibleTickers,
                           int numberOfTickersInPortfolio,
                           int numDaysForOscillatingPeriod,
                           int minLevelForOversoldThreshold,
	                         int maxLevelForOversoldThreshold,
	                         int minLevelForOverboughtThreshold,
	                         int maxLevelForOverboughtThreshold,
                           int divisorForThresholdComputation,
                           IDecoderForTestingPositions decoderForTestingPositions,
                           IFitnessEvaluator fitnessEvaluator,
                           bool symmetricalThresholds,
                           bool overboughtMoreThanOversoldForFixedPortfolio,
                           GenomeManagerType genomeManagerType,
													 ReturnsManager returnsManager,
													 int seedForRandomGenerator)
                           :
														base(eligibleTickers,
														numberOfTickersInPortfolio,
														decoderForTestingPositions,
														fitnessEvaluator,		
														genomeManagerType,
														returnsManager,
														seedForRandomGenerator)
                                
                          
    {
    	this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
    	this.divisorForThresholdComputation = divisorForThresholdComputation;
			this.decoderForTestingPositions = decoderForTestingPositions;
    	this.minLevelForOversoldThreshold  = minLevelForOversoldThreshold;
      this.maxLevelForOversoldThreshold = maxLevelForOversoldThreshold;
      this.minLevelForOverboughtThreshold = minLevelForOverboughtThreshold;
      this.maxLevelForOverboughtThreshold = maxLevelForOverboughtThreshold;
      this.symmetricalThresholds = symmetricalThresholds;
      this.overboughtMoreThanOversoldForFixedPortfolio = overboughtMoreThanOversoldForFixedPortfolio;
      if(this.symmetricalThresholds)//value for thresholds must be unique
 				numOfGenesDedicatedToThresholds = 1;
      else
      	numOfGenesDedicatedToThresholds = 2;
    	this.genomeManagerPVO_checkParametersForThresholdsComputation();
    }
   
    public override int GenomeSize
    {
      get{return this.genomeSize + this.numOfGenesDedicatedToThresholds;}
    }

    #region Get Min and Max Value

    private int getMinValueForGenes_getMinValueForTicker()
    {
      int returnValue;
      switch (this.genomeManagerType)
      {
        case GenomeManagerType.OnlyLong :
          returnValue = 0;
          break;
        default://For ShortAndLong or OnlyShort portfolios
          returnValue = - this.eligibleTickers.Count;
          break;
      }
      return returnValue;
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
					if(this.numOfGenesDedicatedToThresholds == 2)
						returnValue = this.minLevelForOverboughtThreshold;
					else
            returnValue = this.getMinValueForGenes_getMinValueForTicker();
				  break;
				default://gene for ticker
					returnValue = this.getMinValueForGenes_getMinValueForTicker();
					break;
      }
    	return returnValue;
    }
    
    private int getMaxValueForGenes_getMaxValueForTicker()
    {
      int returnValue;
      switch (this.genomeManagerType)
      {
        case GenomeManagerType.OnlyShort :
          returnValue = - 1;
          break;
        default ://For ShortAndLong or OnlyLong portfolios
          returnValue = this.eligibleTickers.Count - 1;
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
					if(this.numOfGenesDedicatedToThresholds == 2)
						returnValue = this.maxLevelForOverboughtThreshold;
					else
						returnValue = this.getMaxValueForGenes_getMaxValueForTicker();
					break;
				default://gene for ticker
					returnValue = this.getMaxValueForGenes_getMaxValueForTicker();
					break;
      }
    	return returnValue;
    }																
  	
    #endregion
		
       
    public override Genome[] GetChilds(Genome parent1, Genome parent2)
    {
    	//in this simple implementation
    	//child have the tickers of one parent
    	//and the thresholds of the other
    	Genome[] childs = new Genome[2];
    	childs[0] = parent1.Clone();
    	childs[1] = parent2.Clone();
    	//exchange of genes coding thresholds
     	
    	if(this.symmetricalThresholds)//unique value for thresholds
    	{
    		childs[0].SetGeneValue(parent2.GetGeneValue(0),0);
    		childs[1].SetGeneValue(parent1.GetGeneValue(0),0);
    	}
    	else//two different values for thresholds
    	{
    		childs[0].SetGeneValue(parent2.GetGeneValue(0),0);
    		childs[1].SetGeneValue(parent1.GetGeneValue(0),0);
    		childs[0].SetGeneValue(parent2.GetGeneValue(1),1);
    		childs[1].SetGeneValue(parent1.GetGeneValue(1),1);
    	}
    	return childs;
    }

    public override int GetNewGeneValue(Genome genome, int genePosition)
    {
      // in this implementation only new gene values pointing to tickers
      // must be different from the others already stored
      int minValueForGene = genome.GetMinValueForGenes(genePosition);
      int maxValueForGene = genome.GetMaxValueForGenes(genePosition);
      int returnValue = GenomeManagement.RandomGenerator.Next(minValueForGene,
                                      maxValueForGene + 1);
      
      if(this.numOfGenesDedicatedToThresholds == 2 &&
         this.overboughtMoreThanOversoldForFixedPortfolio && genePosition == 1)
      //genePosition points to overbought threshold,
      //dipendent from the oversold one such that the portfolio tends to be fix
      	  returnValue = Convert.ToInt32(Convert.ToDouble(genome.GetGeneValue(0)) * Convert.ToDouble(this.divisorForThresholdComputation) /
      					      (Convert.ToDouble(this.divisorForThresholdComputation) - Convert.ToDouble(genome.GetGeneValue(0))));
      
      while(genePosition > this.numOfGenesDedicatedToThresholds - 1
	          && GenomeManipulator.IsTickerContainedInGenome(returnValue,
      	                                                genome,
      	                                                this.numOfGenesDedicatedToThresholds,
      	                                                genome.Size - 1))
	    //while in the given position has to be stored
	    //a new gene pointing to a ticker and
	    //the proposed returnValue points to a ticker
	    //already stored in the given genome
	              returnValue = GenomeManagement.RandomGenerator.Next(minValueForGene,
	        														maxValueForGene + 1);
	    
      return returnValue;
    }
	  
    public override void Mutate(Genome genome)
    {
      // in this implementation only one gene is mutated
      int genePositionToBeMutated = GenomeManagement.RandomGenerator.Next(genome.Size);
      int minValueForGene = genome.GetMinValueForGenes(genePositionToBeMutated);
      int maxValueForGene = genome.GetMaxValueForGenes(genePositionToBeMutated);
      int newValueForGene = GenomeManagement.RandomGenerator.Next(minValueForGene,
	        																maxValueForGene +1);
      while(genePositionToBeMutated > this.numOfGenesDedicatedToThresholds - 1 &&
	          GenomeManipulator.IsTickerContainedInGenome(newValueForGene,
      	                                            genome,
      	                                            this.numOfGenesDedicatedToThresholds,
      	                                            genome.Size - 1))
	    //while in the proposed genePositionToBeMutated has to be stored
	    //a new gene pointing to a ticker and
	    //the proposed newValueForGene points to a ticker
	    //already stored in the given genome
	          newValueForGene = GenomeManagement.RandomGenerator.Next(minValueForGene,
	        																	maxValueForGene +1);
      //TODO add if when it is mutated a threshold 
      //(just a single threshold or the pair of thresholds)
	    if(genePositionToBeMutated > this.numOfGenesDedicatedToThresholds - 1)
	        GenomeManagement.MutateOneGene(genome,
	          genePositionToBeMutated, newValueForGene);
    }
  }
}
