/*
QuantProject - Quantitative Finance Library

WFLagBruteForceWeightedPositionsChooserForBalancedFixedPortfolioAndBalancedDriving.cs
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
//using QuantProject.ADT.Collections;
using QuantProject.ADT.Optimizing.BruteForce;
//using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.EquityEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Timing;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WeightedPositionsChoosers.Decoding;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger;


namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WeightedPositionsChoosers
{
	/// <summary>
	/// Computes the best driving positions, for the given portfolio
	/// tickers, with respect to the lag strategy, using
	/// the brute force optimization method.
	/// Weights for driving positions and for portfolio positions are searched in
	/// such a way to balance each position with respect to its volatility.
	/// </summary>
	[Serializable]
	public class WFLagBruteForceWeightedPositionsChooserForBalancedFixedPortfolioAndBalancedDriving :
		IWFLagWeightedPositionsChooser
	{
		public event NewProgressEventHandler NewProgress;

		protected int numberOfDrivingPositions;
		protected SignedTickers portfolioSignedTickers;
		protected int inSampleDays;
		protected string benchmark;
		protected IEquityEvaluator equityEvaluator;

		private IWFLagDecoder wFLagDecoder;
		private ReturnsManager returnsManager;
		private WFLagChosenPositions wFLagChosenPositions;
		private WeightedPositions drivingWeightedPositions;
		private WeightedPositions portfolioWeightedPositions;
		private DateTime firstOptimizationDate;
		private DateTime lastOptimizationDate;

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
		public string Benchmark
		{
			get
			{
				return this.benchmark;
			}
		}
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
		public WFLagChosenPositions WFLagChosenPositions
		{
			get
			{
				return this.wFLagChosenPositions;
			}
		}
		public WFLagBruteForceWeightedPositionsChooserForBalancedFixedPortfolioAndBalancedDriving(
			int numberOfDrivingPositions ,
			SignedTickers portfolioSignedTickers ,
			int inSampleDays ,
			string benchmark ,
			IEquityEvaluator equityEvaluator
			)
		{
			this.numberOfDrivingPositions = numberOfDrivingPositions;
			this.portfolioSignedTickers = portfolioSignedTickers;
			this.inSampleDays = inSampleDays;
			this.benchmark = benchmark;
			this.equityEvaluator = equityEvaluator;
		}

		#region SetWeightedPositions
//		private void setSignedTickers_clearPositions()
//		{
//			this.drivingPositions.Clear();
//			this.portfolioPositions.Clear();
//		}
//		#region setWeightedPositions_usingTheGeneticOptimizer
//		private void newGenerationEventHandler(
//			object sender , NewGenerationEventArgs e )
//		{
//			this.NewProgress( sender ,
//				new NewProgressEventArgs( e.GenerationCounter , e.GenerationNumber ) );
//		}
		private void setWeightedPositions(
			WFLagWeightedPositions wFLagWeightedPositions )
		{
			this.drivingWeightedPositions =
				wFLagWeightedPositions.DrivingWeightedPositions;
			this.portfolioWeightedPositions =
				wFLagWeightedPositions.PortfolioWeightedPositions;
		}
//		private void setSignedTickers_setTickersFromGenome(
//			IGenomeManager genomeManager ,
//			Genome genome )
//		{
//			WFLagWeightedPositions wFLagWeightedPositions =
//				( WFLagWeightedPositions )genomeManager.Decode( genome );
//			this.setWeightedPositions( wFLagWeightedPositions );
////			this.drivingWeightedPositions =
////				wFLagWeightedPositions.DrivingWeightedPositions;
////			this.portfolioWeightedPositions =
////				wFLagWeightedPositions.PortfolioWeightedPositions;
//		}
//		public virtual void setWeightedPositions_usingTheGeneticOptimizer(
//			WFLagEligibleTickers eligibleTickers )
//		{
//			this.firstOptimizationDate =
//				this.endOfDayTimer.GetCurrentTime().DateTime.AddDays(
//				-( this.inSampleDays - 1 ) );
//			this.lastOptimizationDate =
//				this.endOfDayTimer.GetCurrentTime().DateTime;
//
//			WFLagGenomeManager genomeManager = 
//				new WFLagGenomeManager(
//				eligibleTickers.EligibleTickers ,
//				eligibleTickers.EligibleTickers ,
//				this.firstOptimizationDate ,
//				this.lastOptimizationDate ,
//				this.numberOfDrivingPositions ,
//				this.numberOfPositionsToBeChosen ,
//				this.equityEvaluator ,
//				QuantProject.ADT.ConstantsProvider.SeedForRandomGenerator );
//
//			GeneticOptimizer geneticOptimizer = new GeneticOptimizer(
//				0.85 ,
//				0.02 ,
//				0.001 ,
//				this.populationSizeForGeneticOptimizer ,
//				this.generationNumberForGeneticOptimizer ,
//				genomeManager ,
//				ConstantsProvider.SeedForRandomGenerator );
//
//			geneticOptimizer.NewGeneration +=
//				new NewGenerationEventHandler( this.newGenerationEventHandler );
//
//			geneticOptimizer.Run( false );
//
//			this.setSignedTickers_setTickersFromGenome(
//				genomeManager , geneticOptimizer.BestGenome );
//
//			this.generation = geneticOptimizer.BestGenome.Generation;
//
//		}
//		#endregion
//		#region setWeightedPositions_usingTheBruteForceOptimizer
//		private void newBruteForceOptimizerProgressEventHandler(
//			object sender , NewProgressEventArgs e )
//		{
//			this.NewProgress( sender , e );
//		}
//		public virtual void setWeightedPositions_usingTheBruteForceOptimizer(
//			WFLagEligibleTickers eligibleTickers )
//		{
//			this.firstOptimizationDate =
//				this.endOfDayTimer.GetCurrentTime().DateTime.AddDays(
//				-( this.inSampleDays - 1 ) );
//			this.lastOptimizationDate =
//				this.endOfDayTimer.GetCurrentTime().DateTime;
//
//			WFLagBruteForceOptimizableParametersManager
//				wFLagBruteForceOptimizableItemManager= 
//				new WFLagBruteForceOptimizableParametersManager(
//				eligibleTickers.EligibleTickers ,
//				eligibleTickers.EligibleTickers ,
//				this.firstOptimizationDate ,
//				this.lastOptimizationDate ,
//				this.numberOfDrivingPositions ,
//				this.numberOfPositionsToBeChosen ,
//				this.equityEvaluator );
//
//			BruteForceOptimizer bruteForceOptimizer = new BruteForceOptimizer(
//				wFLagBruteForceOptimizableItemManager );
//
//			bruteForceOptimizer.NewProgress +=
//				new NewProgressEventHandler(
//				this.newBruteForceOptimizerProgressEventHandler );
//
//			bruteForceOptimizer.Run();
//
//			BruteForceOptimizableParameters bestParameters =
//				bruteForceOptimizer.BestParameters;
//
//			WFLagWeightedPositions wFLagWeightedPositions =
//				( WFLagWeightedPositions )wFLagBruteForceOptimizableItemManager.Decode(
//				bestParameters );
//
//			this.setWeightedPositions( wFLagWeightedPositions );
//		}
//		#endregion
		#region setDecoder
		#region setReturnsManager
		protected virtual ReturnIntervals getReturnIntervals(
			EndOfDayDateTime now )
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
		private void setReturnsManager( EndOfDayDateTime now )
		{
			ReturnIntervals returnIntervals =
				this.getReturnIntervals( now );
			IHistoricalQuoteProvider historicalQuoteProvider =
				new HistoricalAdjustedQuoteProvider();
			this.returnsManager =
				new ReturnsManager( returnIntervals ,
				historicalQuoteProvider );
		}
		#endregion setReturnsManager
		protected virtual void setDecoder(
			WFLagEligibleTickers eligibleTickersForDrivingPositions ,	EndOfDayDateTime now )
		{
			this.setReturnsManager( now );
			this.wFLagDecoder = new
				WFLagDecoderForFixedPortfolioWithBalancedVolatilityAndDrivingWithBalancedVolatility(
				eligibleTickersForDrivingPositions , this.numberOfDrivingPositions ,
				this.portfolioSignedTickers , returnsManager );
		}
		#endregion setDecoder
		#region setWeightedPositions_withFixedPortfolio
//		private void newBruteForceOptimizerProgressEventHandler(
//			object sender , NewProgressEventArgs e )
//		{
//			this.NewProgress( sender , e );
//		}
		private void newBruteForceOptimizerProgressEventHandler(
			object sender , NewProgressEventArgs e )
		{
			this.NewProgress( sender , e );
		}
		public virtual void setWeightedPositions_withFixedPortfolio(
			WFLagEligibleTickers eligibleTickers ,
			string longPortfolioTicker , string shortPortfolioTicker ,
			EndOfDayDateTime now )
		{
			this.firstOptimizationDate = now.DateTime.AddDays(
				-( this.inSampleDays - 1 ) );
			this.lastOptimizationDate =	now.DateTime;

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
			WFLagBruteForceOptimizableParametersManagerForBalancedFixedPortfolioAndBalancedDriving
				wFLagFixedPortfolioBruteForceOptimizableParametersManager= 
				new WFLagBruteForceOptimizableParametersManagerForBalancedFixedPortfolioAndBalancedDriving(
				eligibleTickers.EligibleTickers ,
				longPortfolioTicker ,
				shortPortfolioTicker ,
				this.firstOptimizationDate ,
				this.lastOptimizationDate ,
				this.numberOfDrivingPositions ,
				this.wFLagDecoder ,
				this.equityEvaluator ,
				this.returnsManager );

			BruteForceOptimizer bruteForceOptimizer = new BruteForceOptimizer(
				wFLagFixedPortfolioBruteForceOptimizableParametersManager );

			bruteForceOptimizer.NewProgress +=
				new NewProgressEventHandler(
				this.newBruteForceOptimizerProgressEventHandler );

			bruteForceOptimizer.Run( 10000 ,
				wFLagFixedPortfolioBruteForceOptimizableParametersManager.TotalIterations );

			BruteForceOptimizableParameters bestParameters =
				bruteForceOptimizer.BestParameters;

			WFLagWeightedPositions wFLagWeightedPositions =
				( WFLagWeightedPositions )wFLagFixedPortfolioBruteForceOptimizableParametersManager.Decode(
				bestParameters );

			this.setWeightedPositions( wFLagWeightedPositions );
		}
		#endregion
		public virtual void ChosePositions(
			WFLagEligibleTickers eligibleTickersForDrivingPositions ,
			WFLagEligibleTickers eligibleTickersForPortfolioPositions ,
			EndOfDayDateTime now )
		{
			this.setDecoder( eligibleTickersForDrivingPositions , now );
			this.setWeightedPositions_withFixedPortfolio(
				eligibleTickersForDrivingPositions , "SPY" , "IWM" , now );
			this.wFLagChosenPositions = new WFLagChosenPositions(
				this.DrivingWeightedPositions ,
				this.PortfolioWeightedPositions ,
				now.DateTime );
		}
		#endregion
	}
}
