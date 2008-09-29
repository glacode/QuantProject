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
using QuantProject.Business.DataProviders;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.TickersRelationships;
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
		protected int minLevelForOversoldThreshold;
		protected int maxLevelForOversoldThreshold;
		protected int minLevelForOverboughtThreshold;
		protected int maxLevelForOverboughtThreshold;
		protected int divisorForThresholdComputation;
		protected bool symmetricalThresholds = false;
		protected bool overboughtMoreThanOversoldForFixedPortfolio = false;
		protected int numOfGenesDedicatedToThresholds;
		protected double currentOversoldThreshold = 0.0;
		protected double currentOverboughtThreshold = 0.0;
		protected int numDaysForOscillatingPeriod;
		protected ReturnsManager returnsManager;
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
		                        bool symmetricalThresholds,
		                        bool overboughtMoreThanOversoldForFixedPortfolio,
		                        PortfolioType inSamplePortfolioType,
		                        string benchmark)
			:
			base(setOfInitialTickers,
			     firstQuoteDate,
			     lastQuoteDate,
			     numberOfTickersInPortfolio,
			     0.0,
			     inSamplePortfolioType,
			     benchmark)
			
			
		{
			this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
			this.divisorForThresholdComputation = divisorForThresholdComputation;
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
			this.setReturnsManager(firstQuoteDate , lastQuoteDate);
		}
		
		protected virtual void setReturnsManager(DateTime firstQuoteDate,
		                                         DateTime lastQuoteDate)
		{
			DateTime firstDateTime =
				HistoricalEndOfDayTimer.GetMarketClose( firstQuoteDate );
//				new EndOfDayDateTime(firstQuoteDate,
			//    		EndOfDaySpecificTime.MarketClose);
			DateTime lastDateTime =
				HistoricalEndOfDayTimer.GetMarketClose( lastQuoteDate );
//			new EndOfDayDateTime(lastQuoteDate,
//			                     EndOfDaySpecificTime.MarketClose);
			this.returnsManager =
				new ReturnsManager( new CloseToCloseIntervals(
					firstDateTime,
					lastDateTime,
					this.benchmark,
					this.numDaysForOscillatingPeriod),
				                   new HistoricalAdjustedQuoteProvider() );
		}
		
		public override int GenomeSize
		{
			get{return this.genomeSize + this.numOfGenesDedicatedToThresholds;}
		}

		#region Get Min and Max Value

		private int getMinValueForGenes_getMinValueForTicker()
		{
			int returnValue;
			switch (this.portfolioType)
			{
				case PortfolioType.OnlyLong :
					returnValue = 0;
					break;
				default://For ShortAndLong or OnlyShort portfolios
					returnValue = - this.originalNumOfTickers;
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
			switch (this.portfolioType)
			{
				case PortfolioType.OnlyShort :
					returnValue = - 1;
					break;
					default ://For ShortAndLong or OnlyLong portfolios
						returnValue = this.originalNumOfTickers - 1;
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
		
		
		#region getStrategyReturns
		
		private float[] getStrategyReturns_getReturnsActually(
			float[] plainReturns)
		{
			
			float[] returnValue = new float[plainReturns.Length];
			returnValue[0] = 0; //a the very first day the
			//strategy return is equal to 0 because no position
			//has been entered
			float coefficient = 0;
			for(int i = 0; i < returnValue.Length - 1; i++)
			{
				if( plainReturns[i] >= (float)this.currentOverboughtThreshold )
					//portfolio has been overbought
					coefficient = -1;
				else if( plainReturns[i] <= - (float)this.currentOversoldThreshold )
					//portfolio has been oversold
					coefficient = 1;
				//else the previous coeff is kept or, if no threshold has been
				//reached, then no positions will be opened (coefficient = 0)
				returnValue[i + 1] = coefficient * plainReturns[i + 1];
			}
			return returnValue;
			
			//return plainReturns;
		}
		
		protected override float[] getStrategyReturns()
		{
			float[] plainReturns = this.weightedPositionsFromGenome.GetReturns(
				this.returnsManager);
			return this.getStrategyReturns_getReturnsActually(plainReturns);
		}
		
		#endregion
		

		private void getFitnessValue_setCurrentThresholds(Genome genome)
		{
			this.currentOversoldThreshold = Convert.ToDouble(genome.Genes()[0])/
				Convert.ToDouble(this.divisorForThresholdComputation);
			
			if(this.symmetricalThresholds)
				this.currentOverboughtThreshold = this.currentOversoldThreshold;
			else
				this.currentOverboughtThreshold = Convert.ToDouble(genome.Genes()[1])/
					Convert.ToDouble(this.divisorForThresholdComputation);
		}
		
		protected int getFitnessValue_getDaysOnTheMarket()
		{
			int returnValue = 0;
			foreach(float strategyReturn in this.strategyReturns)
				if(strategyReturn != 0)
				//the applied strategy gets positions on the market
				returnValue++;

			return returnValue;
		}
		
		private double getFitnessValue_calculate_calculateActually()
		{
			return this.AverageOfStrategyReturns/Math.Sqrt(this.VarianceOfStrategyReturns);
			//return Math.Sqrt(this.VarianceOfStrategyReturns);
		}
		
		
		protected override double getFitnessValue_calculate()
		{
			double returnValue = -1.0;
			if(this.getFitnessValue_getDaysOnTheMarket() >
			   this.strategyReturns.Length / 2)
				//if the genome represents a portfolio that stays on the market
				//at least half of the theoretical days
				returnValue =
					this.getFitnessValue_calculate_calculateActually();
			return returnValue;
		}
		
		protected string getFitnessValue_getFirstTickerFromGenome(Genome genome)
		{
			GenomeMeaningPVO genomeMeaning = (GenomeMeaningPVO)genome.Meaning;
			return genomeMeaning.Tickers[0];
		}
		protected string getFitnessValue_getSecondTickerFromGenome(Genome genome)
		{
			GenomeMeaningPVO genomeMeaning = (GenomeMeaningPVO)genome.Meaning;
			return genomeMeaning.Tickers[1];
		}
		
		//fitness is a sharpe-ratio based indicator for the equity line resulting
		//from applying the strategy
		public override double GetFitnessValue(Genome genome)
		{
			//OLD CLASSICAL IMPLEMENTATION (sharpeRatio applied to strategyReturns)
			//      this.getFitnessValue_setCurrentThresholds(genome);
			//      return base.GetFitnessValue(genome);

			//NEW implementation: fitness is just the pearson correlation
			//applied to two tickers. This kind of fitness is only valid
			//for experimental tests with 2-tickers portfolios
			double returnValue = -2.0;
			if(this.correlationProvider == null)
				this.correlationProvider = new CloseToCloseCorrelationProvider(
					QuantProject.ADT.ExtendedDataTable.GetArrayOfStringFromColumn(this.setOfTickers, 0),
					this.returnsManager, 0.0001f, 0.5f);
			string firstTicker = this.getFitnessValue_getFirstTickerFromGenome(genome);
			string secondTicker = this.getFitnessValue_getSecondTickerFromGenome(genome);
			if(  ( firstTicker.StartsWith("-") && !secondTicker.StartsWith("-") ) ||
			   ( secondTicker.StartsWith("-") && !firstTicker.StartsWith("-") )     )
				//tickers have to be opposite in sign
			{
				double correlationIndex = correlationProvider.GetPearsonCorrelation(
					SignedTicker.GetTicker(firstTicker),
					SignedTicker.GetTicker(secondTicker) );
				if(correlationIndex < 0.96)
					//	if correlation index is not too high to be
					//  probably originated by the same instrument
					returnValue = correlationIndex;
			}
			return returnValue;
		}
		
		public override object Decode(Genome genome)
		{
			string[] arrayOfTickers =
				new string[genome.Genes().Length - this.numOfGenesDedicatedToThresholds];
			int geneForTicker;
			GenomeMeaningPVO meaning;
			for(int genePosition = this.numOfGenesDedicatedToThresholds;
			    genePosition < genome.Genes().Length;
			    genePosition++)
			{
				geneForTicker = (int)genome.Genes().GetValue(genePosition);
				arrayOfTickers[genePosition - this.numOfGenesDedicatedToThresholds] =
					this.decode_getTickerCodeForLongOrShortTrade(geneForTicker);
			}
			
			double[] arrayOfWeights = ExtendedMath.ArrayOfAbs(
				WeightedPositions.GetBalancedWeights(new SignedTickers(arrayOfTickers),
				                                     this.returnsManager) );
			
			if(this.symmetricalThresholds)
				meaning = new GenomeMeaningPVO(arrayOfTickers,
				                               arrayOfWeights,
				                               Convert.ToDouble(genome.Genes()[0])/Convert.ToDouble(this.divisorForThresholdComputation),
				                               Convert.ToDouble(genome.Genes()[0])/Convert.ToDouble(this.divisorForThresholdComputation),
				                               this.numDaysForOscillatingPeriod);
			else
				meaning = new GenomeMeaningPVO(arrayOfTickers,
				                               arrayOfWeights,
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
