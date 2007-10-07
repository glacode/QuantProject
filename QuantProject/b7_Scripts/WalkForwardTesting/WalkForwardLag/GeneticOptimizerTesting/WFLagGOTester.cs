/*
QuantProject - Quantitative Finance Library

WFLagGOTester.cs
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
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Timing;


namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// Used to test the GeneticOptimizer and the genome manager
	/// effectiveness
	/// </summary>
	[Serializable]
	public class WFLagGOTester : WFLagChosenTickers
	{
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

		public WFLagGOTester(
			int numberOfDrivingPositions ,
			int numberOfPositionsToBeChosen ,
			int inSampleDays ,
			IEndOfDayTimer endOfDayTimer ,
			int generationNumberForGeneticOptimizer ,
			int populationSizeForGeneticOptimizer ,
			IEquityEvaluator equityEvaluator
			) : base( numberOfDrivingPositions ,
			numberOfPositionsToBeChosen , 
			inSampleDays ,
			endOfDayTimer ,
			generationNumberForGeneticOptimizer ,
			populationSizeForGeneticOptimizer ,
			equityEvaluator)
		{
		}
//		{
//			this.eligibleTickers = eligibleTickers;
//			this.numberOfDrivingPositions = numberOfDrivingPositions;
//			this.numberOfPositionsToBeChosen = numberOfPositionsToBeChosen;
//			this.inSampleDays = inSampleDays;
//			this.endOfDayTimer = endOfDayTimer;
//			this.generationNumberForGeneticOptimizer =
//				generationNumberForGeneticOptimizer;
//			this.populationSizeForGeneticOptimizer =
//				populationSizeForGeneticOptimizer;
//		}

		#region SetWeightedPositions
//		private void setSignedTickers_clearPositions()
//		{
//			this.drivingPositions.Clear();
//			this.portfolioPositions.Clear();
//		}
//		private void newGenerationEventHandler(
//			object sender , NewGenerationEventArgs e )
//		{
//			this.NewProgress( sender ,
//				new NewProgressEventArgs( e.GenerationCounter , e.GenerationNumber ) );
//		}
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
		private double getBestGenome(
			WFLagEligibleTickers eligibleTickers , int seed ,
			int populationSize , int generationNumber )
		{
			this.firstOptimizationDate =
				this.endOfDayTimer.GetCurrentTime().DateTime.AddDays(
				-( this.inSampleDays - 1 ) );
			this.lastOptimizationDate =
				this.endOfDayTimer.GetCurrentTime().DateTime;

			WFLagGenomeManager genomeManager = 
				new WFLagGenomeManager(
				eligibleTickers.EligibleTickers ,
				eligibleTickers.EligibleTickers ,
				this.firstOptimizationDate ,
				this.lastOptimizationDate ,
				this.numberOfDrivingPositions ,
				this.numberOfPositionsToBeChosen ,
				this.equityEvaluator ,
				seed * 100 );

			GeneticOptimizer geneticOptimizer = new GeneticOptimizer(
				0.85 ,
				0.02 ,
				0.10 ,
				populationSize ,
				generationNumber ,
				genomeManager ,
				ConstantsProvider.SeedForRandomGenerator );

//			geneticOptimizer.NewGeneration +=
//				new NewGenerationEventHandler( this.newGenerationEventHandler );

			geneticOptimizer.Run( false );

			this.setSignedTickers_setTickersFromGenome(
				genomeManager , geneticOptimizer.BestGenome );

			this.generation = geneticOptimizer.BestGenome.Generation;

			return geneticOptimizer.BestGenome.Fitness;
		}
		private double getBestGenomeForSingleGeneration(
			WFLagEligibleTickers eligibleTickers , int seed )
		{
			return this.getBestGenome(
				eligibleTickers , seed , 100000 , 0 );
		}
		private double getBestGenomeForMultipleGenerations(
			WFLagEligibleTickers eligibleTickers , int seed )
		{
			return this.getBestGenome(
				eligibleTickers , seed , 10000 , 9 );
		}
		private void setWeightedPositions(
			WFLagEligibleTickers eligibleTickers , int seed )
		{
			double bestGenomeForSingleGeneration =
				this.getBestGenomeForSingleGeneration( eligibleTickers , seed );
			double bestGenomeForMultipleGenerations =
				this.getBestGenomeForMultipleGenerations( eligibleTickers , seed );
			Console.WriteLine( "Seed=\t" + seed + "\t; bestForSingle:\t" +
				bestGenomeForSingleGeneration + "\t; bestForMultiple:\t" +
				bestGenomeForMultipleGenerations );
		}
//		public override void SetWeightedPositions(
//									WFLagEligibleTickers eligibleTickers )
//		{
//			for ( int i=10 ; i<80 ; i++ )
//				this.setWeightedPositions( eligibleTickers , i );
//			System.Windows.Forms.MessageBox.Show(
//				"Test is complete! Check the console output." );
//		}
		#endregion
	}
}
