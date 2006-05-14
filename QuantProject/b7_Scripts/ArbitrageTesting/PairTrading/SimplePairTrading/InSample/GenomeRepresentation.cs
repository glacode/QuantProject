/*
QuantProject - Quantitative Finance Library

GenomeRepresentation.cs
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

using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.ArbitrageTesting.PairTrading.SimplePairTrading;

namespace QuantProject.Scripts.ArbitrageTesting.PairTrading.SimplePairTrading.InSample
{
	/// <summary>
	/// Provides the genome relevant informations
	/// </summary>
	[Serializable]
	public class GenomeRepresentation
	{
		private double fitness;
		private string firstTicker;
    private string secondTicker;
		private DateTime firstOptimizationDate;
		private DateTime lastOptimizationDate;
    private int numDaysForGap;
    private double averageGap;
    private double stdDevGap;
    private double maxNumOfStdDevForNormalGap;
    private int generationCounter;

		public string FirstTicker
		{
			get{return this.firstTicker;}
		}
    
    public string SecondTicker
    {
      get{return this.secondTicker;}
    }

		public double Fitness
		{
			get { return this.fitness; }
		}
		public DateTime FirstOptimizationDate
		{
			get { return this.firstOptimizationDate; }
		}
		public DateTime LastOptimizationDate
		{
			get { return this.lastOptimizationDate; }
		}
		/// <summary>
		/// Number of the first generation containing the genome
		/// </summary>
		public int GenerationCounter
		{
			get { return this.generationCounter; }
		}

    public int NumDaysForGap
    {
      get { return this.numDaysForGap; }
    }

    public double AverageGap
    {
      get { return this.averageGap; }
    }
    
    public double StdDevGap
    {
      get { return this.stdDevGap; }
    }

    public double MaxNumOfStdDevForNormalGap
    {
      get { return this.maxNumOfStdDevForNormalGap; }
    }

    private void genomeRepresentation( double maxNumOfStdDevForNormalGap, Genome genome ,
			DateTime firstOptimizationDate , DateTime lastOptimizationDate ,
			int generationCounter )
		{
			this.maxNumOfStdDevForNormalGap = maxNumOfStdDevForNormalGap;
      this.fitness = genome.Fitness;
      this.averageGap = ((GenomeMeaningSimplePT)genome.Meaning).AverageGap;
      this.stdDevGap = ((GenomeMeaningSimplePT)genome.Meaning).StdDevGap;
      this.firstTicker = ((GenomeMeaningSimplePT)genome.Meaning).FirstTicker;
      this.secondTicker = ((GenomeMeaningSimplePT)genome.Meaning).SecondTicker;
			this.numDaysForGap = ((GenomeMeaningSimplePT)genome.Meaning).NumOfDaysForGap;
      this.firstOptimizationDate = firstOptimizationDate;
			this.lastOptimizationDate = lastOptimizationDate;
			this.generationCounter = generationCounter;
		}
		public GenomeRepresentation( double maxNumOfStdDevForNormalGap, Genome genome ,
			DateTime firstOptimizationDate , DateTime lastOptimizationDate )
		{
			this.genomeRepresentation( maxNumOfStdDevForNormalGap, genome ,
				firstOptimizationDate , lastOptimizationDate , -1 );
		}
		public GenomeRepresentation( double maxNumOfStdDevForNormalGap, Genome genome ,
			DateTime firstOptimizationDate , DateTime lastOptimizationDate ,
			int generationCounter )
		{
			this.genomeRepresentation( maxNumOfStdDevForNormalGap, genome , firstOptimizationDate ,
				lastOptimizationDate , generationCounter );
		}
	}
}
