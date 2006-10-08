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
using QuantProject.ADT.Optimizing.BruteForce;
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

		protected WFLagEligibleTickers eligibleTickers;
		protected int numberOfDrivingPositions;
		protected int numberOfPositionsToBeChosen;
		protected int inSampleDays;
		protected IEndOfDayTimer endOfDayTimer;
		protected int generationNumberForGeneticOptimizer;
		protected int populationSizeForGeneticOptimizer;

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

		#region SetWeightedPositions
//		private void setSignedTickers_clearPositions()
//		{
//			this.drivingPositions.Clear();
//			this.portfolioPositions.Clear();
//		}
		#region setWeightedPositions_usingTheGeneticOptimizer
		private void newGenerationEventHandler(
			object sender , NewGenerationEventArgs e )
		{
			this.NewProgress( sender ,
				new NewProgressEventArgs( e.GenerationCounter , e.GenerationNumber ) );
		}
		private void setWeightedPositions(
			WFLagWeightedPositions wFLagWeightedPositions )
		{
			this.drivingWeightedPositions =
				wFLagWeightedPositions.DrivingWeightedPositions;
			this.portfolioWeightedPositions =
				wFLagWeightedPositions.PortfolioWeightedPositions;
		}
		private void setSignedTickers_setTickersFromGenome(
			IGenomeManager genomeManager ,
			Genome genome )
		{
			WFLagWeightedPositions wFLagWeightedPositions =
				( WFLagWeightedPositions )genomeManager.Decode( genome );
			this.setWeightedPositions( wFLagWeightedPositions );
//			this.drivingWeightedPositions =
//				wFLagWeightedPositions.DrivingWeightedPositions;
//			this.portfolioWeightedPositions =
//				wFLagWeightedPositions.PortfolioWeightedPositions;
		}
		public virtual void setWeightedPositions_usingTheGeneticOptimizer(
			WFLagEligibleTickers eligibleTickers )
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
				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator );

			GeneticOptimizer geneticOptimizer = new GeneticOptimizer(
				0.85 ,
				0.02 ,
				0.001 ,
				this.populationSizeForGeneticOptimizer ,
				this.generationNumberForGeneticOptimizer ,
				genomeManager ,
				ConstantsProvider.SeedForRandomGenerator );

			geneticOptimizer.NewGeneration +=
				new NewGenerationEventHandler( this.newGenerationEventHandler );

			geneticOptimizer.Run( false );

			this.setSignedTickers_setTickersFromGenome(
				genomeManager , geneticOptimizer.BestGenome );

			this.generation = geneticOptimizer.BestGenome.Generation;

		}
		#endregion
		#region setWeightedPositions_usingTheBruteForceOptimizer
		private void newBruteForceOptimizerProgressEventHandler(
			object sender , NewProgressEventArgs e )
		{
			this.NewProgress( sender , e );
		}
		public virtual void setWeightedPositions_usingTheBruteForceOptimizer(
			WFLagEligibleTickers eligibleTickers )
		{
			this.firstOptimizationDate =
				this.endOfDayTimer.GetCurrentTime().DateTime.AddDays(
				-( this.inSampleDays - 1 ) );
			this.lastOptimizationDate =
				this.endOfDayTimer.GetCurrentTime().DateTime;

			WFLagBruteForceOptimizableParametersManager
				wFLagBruteForceOptimizableItemManager= 
				new WFLagBruteForceOptimizableParametersManager(
				eligibleTickers.EligibleTickers ,
				eligibleTickers.EligibleTickers ,
				this.firstOptimizationDate ,
				this.lastOptimizationDate ,
				this.numberOfDrivingPositions ,
				this.numberOfPositionsToBeChosen );

			BruteForceOptimizer bruteForceOptimizer = new BruteForceOptimizer(
				wFLagBruteForceOptimizableItemManager );

			bruteForceOptimizer.NewProgress +=
				new NewProgressEventHandler(
				this.newBruteForceOptimizerProgressEventHandler );

			bruteForceOptimizer.Run();

			BruteForceOptimizableParameters bestParameters =
				bruteForceOptimizer.BestParameters;

			WFLagWeightedPositions wFLagWeightedPositions =
				( WFLagWeightedPositions )wFLagBruteForceOptimizableItemManager.Decode(
				bestParameters );

			this.setWeightedPositions( wFLagWeightedPositions );
		}
		#endregion
		#region setWeightedPositions_withFixedPortfolio
//		private void newBruteForceOptimizerProgressEventHandler(
//			object sender , NewProgressEventArgs e )
//		{
//			this.NewProgress( sender , e );
//		}
		public virtual void setWeightedPositions_withFixedPortfolio(
			WFLagEligibleTickers eligibleTickers ,
			string longPortfolioTicker , string shortPortfolioTicker )
		{
			this.firstOptimizationDate =
				this.endOfDayTimer.GetCurrentTime().DateTime.AddDays(
				-( this.inSampleDays - 1 ) );
			this.lastOptimizationDate =
				this.endOfDayTimer.GetCurrentTime().DateTime;

//			WFLagFixedPortfolioBruteForceOptimizableParametersManager
//				wFLagFixedPortfolioBruteForceOptimizableParametersManager= 
//				new WFLagFixedPortfolioBruteForceOptimizableParametersManager(
//				eligibleTickers.EligibleTickers ,
//				longPortfolioTicker ,
//				shortPortfolioTicker ,
//				this.firstOptimizationDate ,
//				this.lastOptimizationDate ,
//				this.numberOfDrivingPositions );
//
//			WFLagFixedPortfolioBruteForceOptParamManagerWithPortfolioNormalizedVolatility
//				wFLagFixedPortfolioBruteForceOptimizableParametersManager= 
//				new WFLagFixedPortfolioBruteForceOptParamManagerWithNormalizedVolatility(
//				eligibleTickers.EligibleTickers ,
//				longPortfolioTicker ,
//				shortPortfolioTicker ,
//				this.firstOptimizationDate ,
//				this.lastOptimizationDate ,
//				this.numberOfDrivingPositions );
			WFLagFixedPortfolioBruteForceOptParamManagerWithNormalizedVolatility
				wFLagFixedPortfolioBruteForceOptimizableParametersManager= 
				new WFLagFixedPortfolioBruteForceOptParamManagerWithNormalizedVolatility(
				eligibleTickers.EligibleTickers ,
				longPortfolioTicker ,
				shortPortfolioTicker ,
				this.firstOptimizationDate ,
				this.lastOptimizationDate ,
				this.numberOfDrivingPositions );

			BruteForceOptimizer bruteForceOptimizer = new BruteForceOptimizer(
				wFLagFixedPortfolioBruteForceOptimizableParametersManager );

			bruteForceOptimizer.NewProgress +=
				new NewProgressEventHandler(
				this.newBruteForceOptimizerProgressEventHandler );

			bruteForceOptimizer.Run( 100000 ,
				wFLagFixedPortfolioBruteForceOptimizableParametersManager.TotalIterations );

			BruteForceOptimizableParameters bestParameters =
				bruteForceOptimizer.BestParameters;

			WFLagWeightedPositions wFLagWeightedPositions =
				( WFLagWeightedPositions )wFLagFixedPortfolioBruteForceOptimizableParametersManager.Decode(
				bestParameters );

			this.setWeightedPositions( wFLagWeightedPositions );
		}
		#endregion
		public virtual void SetWeightedPositions(
			WFLagEligibleTickers eligibleTickers )
		{
//			this.setWeightedPositions_usingTheGeneticOptimizer(
//				eligibleTickers );
//			this.setWeightedPositions_usingTheBruteForceOptimizer(
//				eligibleTickers );
//			this.setWeightedPositions_withFixedPortfolio(
//				eligibleTickers , "SPY" , "IWM" );
//			this.setWeightedPositions_withFixedPortfolio(
//				eligibleTickers , "XLF" , "SMH" );
			this.setWeightedPositions_withFixedPortfolio(
				eligibleTickers , "QQQQ" , "SPY" );
		}
		#endregion
	}
}
