/*
QuantProject - Quantitative Finance Library

WFLagChosenTickers.cs
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
using System.Collections;

using QuantProject.ADT;
using QuantProject.ADT.Collections;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;


namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// Best driving positions and tickers in portfolio,
	/// with respect to the lag strategy
	/// </summary>
	[Serializable]
	public class WFLagChosenTickers : IProgressNotifier
	{
		public event NewProgressEventHandler NewProgress;

		private WFLagEligibleTickers eligibleTickers;
		private int numberOfDrivingPositions;
		private int numberOfPositionsToBeChosen;
		private int inSampleDays;
		private IEndOfDayTimer endOfDayTimer;
		private int generationNumberForGeneticOptimizer;
		private int populationSizeForGeneticOptimizer;

		private WeightedPositions drivingWeightedPositions;
		private WeightedPositions portfolioWeightedPositions;
		private DateTime firstOptimizationDate;
		private DateTime lastOptimizationDate;
		private int generation;

//		public QPHashtable PortfolioPositions
//		{
//			get
//			{
//				return this.portfolioPositions;
//			}
//		}
//		public QPHashtable DrivingPositions
//		{
//			get
//			{
//				return this.drivingPositions;
//			}
//		}
		public WeightedPositions DrivingWeightedPositions
		{
			get
			{
				return this.drivingWeightedPositions;
			}
		}
		public WeightedPositions PortfolioWeightedPositions
		{
			get
			{
				return this.portfolioWeightedPositions;
			}
		}
		public DateTime FirstOptimizationDate
		{
			get
			{
				return this.firstOptimizationDate;
			}
		}
		public DateTime LastOptimizationDate
		{
			get
			{
				return this.lastOptimizationDate;
			}
		}
		/// <summary>
		/// First generation of the genetic optimizer, when the best genome was found
		/// </summary>
		public int Generation
		{
			get { return this.generation; }
		}
		public WFLagChosenTickers(
			int numberOfDrivingPositions ,
			int numberOfPositionsToBeChosen ,
			int inSampleDays ,
			IEndOfDayTimer endOfDayTimer ,
			int generationNumberForGeneticOptimizer ,
			int populationSizeForGeneticOptimizer
			)
		{
			this.eligibleTickers = eligibleTickers;
			this.numberOfDrivingPositions = numberOfDrivingPositions;
			this.numberOfPositionsToBeChosen = numberOfPositionsToBeChosen;
			this.inSampleDays = inSampleDays;
			this.endOfDayTimer = endOfDayTimer;
			this.generationNumberForGeneticOptimizer =
				generationNumberForGeneticOptimizer;
			this.populationSizeForGeneticOptimizer =
				populationSizeForGeneticOptimizer;
		}

		#region SetSignedTickers
//		private void setSignedTickers_clearPositions()
//		{
//			this.drivingPositions.Clear();
//			this.portfolioPositions.Clear();
//		}
		private void newGenerationEventHandler(
			object sender , NewGenerationEventArgs e )
		{
			this.NewProgress( sender ,
				new NewProgressEventArgs( e.GenerationCounter , e.GenerationNumber ) );
		}
		private void setSignedTickers_setTickersFromGenome(
			IGenomeManager genomeManager ,
			Genome genome )
		{
			WFLagWeightedPositions wFLagWeightedPositions =
				( WFLagWeightedPositions )genomeManager.Decode( genome );
			this.drivingWeightedPositions =
				wFLagWeightedPositions.DrivingWeightedPositions;
			this.portfolioWeightedPositions =
				wFLagWeightedPositions.PortfolioWeightedPositions;
		}
		public void SetWeightedPositions( WFLagEligibleTickers eligibleTickers )
		{
//			this.setSignedTickers_clearPositions();

			this.firstOptimizationDate =
				this.endOfDayTimer.GetCurrentTime().DateTime.AddDays(
				-( this.inSampleDays - 1 ) );
			this.lastOptimizationDate =
				this.endOfDayTimer.GetCurrentTime().DateTime;

			WFLagGenomeManagerWithWeights genomeManager = 
				new WFLagGenomeManagerWithWeights(
				eligibleTickers.EligibleTickers ,
				eligibleTickers.EligibleTickers ,
				this.firstOptimizationDate ,
				this.lastOptimizationDate ,
				this.numberOfDrivingPositions ,
				this.numberOfPositionsToBeChosen );

			GeneticOptimizer geneticOptimizer = new GeneticOptimizer(
				genomeManager ,
				this.populationSizeForGeneticOptimizer ,
				this.generationNumberForGeneticOptimizer ,
				ConstantsProvider.SeedForRandomGenerator );

			geneticOptimizer.NewGeneration +=
				new NewGenerationEventHandler( this.newGenerationEventHandler );

			geneticOptimizer.Run( false );

			this.setSignedTickers_setTickersFromGenome(
				genomeManager , geneticOptimizer.BestGenome );

			this.generation = geneticOptimizer.BestGenome.Generation;

		}
		#endregion
	}
}
