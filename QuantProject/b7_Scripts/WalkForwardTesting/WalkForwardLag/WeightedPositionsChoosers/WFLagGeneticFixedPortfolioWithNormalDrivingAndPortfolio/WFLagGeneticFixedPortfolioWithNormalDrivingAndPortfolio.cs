/*
QuantProject - Quantitative Finance Library

WFLagGeneticFixedPortfolioWithNormalDrivingAndPortfolio.cs
Copyright (C) 2003 
Glauco Siliprandi

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

using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Timing;
using QuantProject.Data.DataTables;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WeightedPositionsChoosers
{
	/// <summary>
	/// Computes the best driving positions using the genetic
	/// optimizer. Portfolio positions are fixed. Only normal weights are considered,
	/// both for driving positions and for portfolio positions
	/// </summary>
	public class WFLagGeneticFixedPortfolioWithNormalDrivingAndPortfolio :
		IWFLagWeightedPositionsChooser
	{
		public event NewProgressEventHandler NewProgress;

		protected int numberOfDrivingPositions;
		private SignedTickers portfolioSignedTickers;
		protected int inSampleDays;
		protected string benchmark;
		protected IEquityEvaluator equityEvaluator;
		protected int populationSizeForGeneticOptimizer;
		protected int generationNumberForGeneticOptimizer;

		protected GeneticOptimizer geneticOptimizer;
		protected WFLagWeightedPositions wFLagChosenPositions;
		private EndOfDayDateTime timeWhenChosePositionsIsRequested;

//		// first in sample quote date for driving positions
//		protected DateTime firstInSampleDateForDrivingPositions;
//		// last in sample quote date for equity evaluation
//		protected DateTime lastInSampleOptimizationDate;

		public int NumberOfDrivingPositions
		{
			get
			{
				return this.numberOfDrivingPositions;
			}
		}

		public int NumberOfPortfolioPositions
		{
			get
			{
				return this.portfolioSignedTickers.Count;
			}
		}

		public int NumberDaysForInSampleOptimization
		{
			get
			{
				return this.inSampleDays;
			}
		}
		public int GenerationWhenChosenPositionsWereFound
		{
			get
			{
				return this.geneticOptimizer.BestGenome.Generation;
			}
		}


		public string Benchmark
		{
			get
			{
				return this.benchmark;
			}
		}

		public WFLagWeightedPositions WFLagChosenPositions
		{
			get
			{
				return this.wFLagChosenPositions;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="numberOfDrivingPositions"></param>
		/// <param name="numberOfPortfolioPositions"></param>
		/// <param name="inSampleDays">the number of in sample
		/// data required. Since one more driving day is needed, the max number of
		/// possible trades will be numberDaysForInSampleOptimization-1</param></param>
		public WFLagGeneticFixedPortfolioWithNormalDrivingAndPortfolio(
			int numberOfDrivingPositions ,
			SignedTickers portfolioSignedTickers ,
			int inSampleDays ,
			string benchmark ,
			IEquityEvaluator equityEvaluator ,
			int populationSizeForGeneticOptimizer ,
			int generationNumberForGeneticOptimizer
			)
		{
			this.numberOfDrivingPositions = numberOfDrivingPositions;
			this.portfolioSignedTickers = portfolioSignedTickers;
			this.inSampleDays =	inSampleDays;
			this.benchmark = benchmark;
			this.equityEvaluator = equityEvaluator;
			this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
			this.generationNumberForGeneticOptimizer =
				generationNumberForGeneticOptimizer;
		}
		#region setChosenPositions_usingTheGeneticOptimizer
		private ReturnIntervals getReturnIntervals( EndOfDayDateTime now )
		{
			DateTime firstInSampleDateForDrivingPositions =
				now.DateTime.AddDays(
				-( this.NumberDaysForInSampleOptimization - 1 ) );
			DateTime lastInSampleOptimizationDate =
				now.DateTime;
			ReturnIntervals returnIntervals =
				new CloseToCloseIntervals(
				new EndOfDayDateTime( firstInSampleDateForDrivingPositions ,
				EndOfDaySpecificTime.MarketClose ) ,
				new EndOfDayDateTime( lastInSampleOptimizationDate ,
				EndOfDaySpecificTime.MarketClose ) ,
				this.benchmark );
			return returnIntervals;
		}

		private void newGenerationEventHandler(
			object sender , NewGenerationEventArgs e )
		{
			// comment out this line if no debug is done
			WFLagGenerationDebugger wFLagGenerationDebugger =
				new WFLagGenerationDebugger(
				e.Generation ,
				this.timeWhenChosePositionsIsRequested.DateTime ,
				this.NumberDaysForInSampleOptimization ,
				this.benchmark );
			wFLagGenerationDebugger.Debug();
			this.NewProgress( sender ,
				new NewProgressEventArgs( e.GenerationCounter , e.GenerationNumber ) );
		}
//		private void setWeightedPositions(
//			WFLagWeightedPositions wFLagWeightedPositions )
//		{
//			this.drivingWeightedPositions =
//				wFLagWeightedPositions.DrivingWeightedPositions;
//			this.portfolioWeightedPositions =
//				wFLagWeightedPositions.PortfolioWeightedPositions;
//		}
		private WFLagWeightedPositions getWFLagWeightedPositions_FromDecodableGenome(
			IGenomeManager genomeManager , Genome genome )
		{
			object wFLagWeightedPositions =
				genomeManager.Decode( genome );
			if ( !(wFLagWeightedPositions is WFLagWeightedPositions) )
				throw new Exception( "The genome is not a WFLagWeightedPositions. " +
					"It should happen only if the genome is undecodable. This " +
					"should never happen for the best genome." );
			return (WFLagWeightedPositions)wFLagWeightedPositions;
		}
		private void setChosenPositions_FromDecodableGenome(
			IGenomeManager genomeManager , Genome genome ,
			EndOfDayDateTime now )
		{
			this.wFLagChosenPositions =
				this.getWFLagWeightedPositions_FromDecodableGenome(
				genomeManager , genome );
//			WFLagWeightedPositions wFLagWeightedPositions =
//				this.getWFLagWeightedPositions_FromDecodableGenome(
//				genomeManager , genome );
//			this.wFLagChosenPositions =
//				new WFLagChosenPositions(
//				wFLagWeightedPositions , genome.Generation , now.DateTime );
//			this.setWeightedPositions( wFLagWeightedPositions );
			//			this.drivingWeightedPositions =
			//				wFLagWeightedPositions.DrivingWeightedPositions;
			//			this.portfolioWeightedPositions =
			//				wFLagWeightedPositions.PortfolioWeightedPositions;
		}
		public virtual void setChosenPositions_usingTheGeneticOptimizer(
			WFLagEligibleTickers eligibleTickersForDrivingPositions ,
			EndOfDayDateTime now )
		{
			ReturnIntervals returnIntervals =
				this.getReturnIntervals( now );

			WFLagGenomeManagerForFixedPortfolioWithNormalDrivingAndPortfolio
				genomeManager = 
				new WFLagGenomeManagerForFixedPortfolioWithNormalDrivingAndPortfolio(
				this.numberOfDrivingPositions ,
				eligibleTickersForDrivingPositions.EligibleTickers ,
				this.portfolioSignedTickers ,
				returnIntervals ,
				this.equityEvaluator ,
				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator );

			this.geneticOptimizer = new GeneticOptimizer(
				0.85 ,
				0.02 ,
				0.001 ,
				this.populationSizeForGeneticOptimizer ,
				this.generationNumberForGeneticOptimizer ,
				genomeManager ,
				ConstantsProvider.SeedForRandomGenerator );

			this.geneticOptimizer.NewGeneration +=
				new NewGenerationEventHandler( this.newGenerationEventHandler );

			this.geneticOptimizer.Run( false );

			this.setChosenPositions_FromDecodableGenome(
				genomeManager , this.geneticOptimizer.BestGenome , now );

//			this.generation = geneticOptimizer.BestGenome.Generation;

		}
		#endregion //setChosenPositions_usingTheGeneticOptimizer
		private void chosePositions_checkParameters(
			WFLagEligibleTickers eligibleTickersForDrivingPositions ,
			EndOfDayDateTime now )
		{
			if ( now.EndOfDaySpecificTime != EndOfDaySpecificTime.OneHourAfterMarketClose )
				throw new Exception( "The 'now' parameter must be one hour after market " +
					"close. It is not." );
			if ( eligibleTickersForDrivingPositions.EligibleTickers.Rows.Count <
				this.NumberOfDrivingPositions )
				throw new Exception( "Eligilbe tickers for driving positions contains " +
					"only " + eligibleTickersForDrivingPositions.EligibleTickers.Rows.Count +
					" elements, while NumberOfDrivingPositions is " +
					this.NumberOfDrivingPositions );
		}
		/// <summary>
		/// Sets the best WFLagChosenPositions, with respecto to in sample data.
		/// </summary>
		/// <param name="eligibleTickersForDrivingPositions"></param>
		/// <param name="eligibleTickersForPortfolioPositions"></param>
		/// <param name="now">Last in sample EndOfDayDateTime. It is expected to be
		/// one hour after market close</param>
		public void ChosePositions(
			WFLagEligibleTickers eligibleTickersForDrivingPositions ,
			WFLagEligibleTickers eligibleTickersForPortfolioPositions ,
			EndOfDayDateTime now )
		{
			this.timeWhenChosePositionsIsRequested = now;
			this.chosePositions_checkParameters( eligibleTickersForDrivingPositions ,
				now );
			this.setChosenPositions_usingTheGeneticOptimizer(
				eligibleTickersForDrivingPositions ,
				now );
		}
	}
}
