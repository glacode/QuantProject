/*
QuantProject - Quantitative Finance Library

WFLagGenerationDebugger.cs
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

using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.Strategies;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WeightedPositionsChoosers
{
	/// <summary>
	/// Debugs a generation produced by a genetic optimizer
	/// </summary>
	public class WFLagGenerationDebugger
	{
		private ArrayList generation;
		private DateTime timeWhenOptimizationWasRequested;
		private int numberDaysForInSampleOptimization;
		private string benchmark;

		public WFLagGenerationDebugger(
			ArrayList generation ,
			DateTime timeWhenOptimizationWasRequested ,
      int numberDaysForInSampleOptimization ,
			string benchmark )
		{
			this.generation = generation;
			this.timeWhenOptimizationWasRequested =
				timeWhenOptimizationWasRequested;
			this.numberDaysForInSampleOptimization =
				numberDaysForInSampleOptimization;
			this.benchmark = benchmark;
		}
		#region debugCurrentGeneration
		private bool isPromising( Genome genome )
		{
			int optimalPositionsContained = 0;
			object meaning = genome.Meaning;
			WFLagWeightedPositions wFLagWeightedPositions;
			if ( meaning is WFLagWeightedPositions )
			{
				wFLagWeightedPositions = ( WFLagWeightedPositions )meaning;
				WeightedPositions drivingWeightedPositions =
					wFLagWeightedPositions.DrivingWeightedPositions;
				if ( drivingWeightedPositions.ContainsTicker( "^OEX" ) &&
					drivingWeightedPositions.GetWeightedPosition( "^OEX" ).IsLong )
					optimalPositionsContained++;
				if ( drivingWeightedPositions.ContainsTicker( "BDH" ) &&
					drivingWeightedPositions.GetWeightedPosition( "BDH" ).IsShort )
					optimalPositionsContained++;
				if ( drivingWeightedPositions.ContainsTicker( "HHH" ) &&
					drivingWeightedPositions.GetWeightedPosition( "HHH" ).IsShort )
					optimalPositionsContained++;
				if ( drivingWeightedPositions.ContainsTicker( "IIH" ) &&
					drivingWeightedPositions.GetWeightedPosition( "IIH" ).IsLong )
					optimalPositionsContained++;
										
			}
			bool isPromising = ( optimalPositionsContained >= 2 );
			return isPromising;				
		}
		private void showBacktest(
			WFLagWeightedPositions wFLagWeightedPositions )
		{
			WFLagDebugPositions wFLagDebugPositions =
				new WFLagDebugPositions( wFLagWeightedPositions ,
				this.timeWhenOptimizationWasRequested ,
				0 ,
				this.numberDaysForInSampleOptimization ,
				0 ,
				this.benchmark );
			wFLagDebugPositions.Run();
			//			HistoricalEndOfDayTimer historicalEndOfDayTimer =
			//				new HistoricalEndOfDayTimer( new EndOfDayDateTime(
			//				new DateTime( 2000 , 1 , 1 ) , EndOfDaySpecificTime.MarketClose ) );
			//			WFLagChosenTickers wFLagChosenTickers =
			//				new WFLagChosenTickers( this.NumberOfDrivingPositions ,
			//				this.NumberOfPortfolioPositions , this.inSampleDays ,
			//				historicalEndOfDayTimer , -999 , this.populationSizeForGeneticOptimizer ,
			//				this.equityEvaluator );
			//			wFLagChosenTickers.DrivingWeightedPositions =
			//				wFLagWeightedPositions.DrivingWeightedPositions;
			//			wFLagChosenTickers.PortfolioWeightedPositions =
			//				wFLagWeightedPositions.PortfolioWeightedPositions;
			//			WFLagChosenPositions wFLagChosenPositions =
			//				new WFLagChosenPositions( wFLagChosenTickers ,
			//				new DateTime( 2001 , 1 , 4 ) );
			//			WFLagDebugGenome wFLagDebugGenome =
			//				new WFLagDebugGenome( wFLagChosenPositions  ,
			//				this.inSampleDays , this.benchmark );
			//			wFLagDebugGenome.Show();
		}
		public void Debug()
		{
			int numberOfDifferentFitness = 1;
			System.Collections.ArrayList indexesForPromisingGenomes =
				new System.Collections.ArrayList();
			for ( int i = 1 ; i < this.generation.Count ; i++ )
			{
				Genome genome = (Genome)this.generation[ i ];
				if ( genome.Fitness >
					((Genome)this.generation[ i - 1 ]).Fitness )
					numberOfDifferentFitness++;
				if ( this.isPromising( genome ) )
				{
					indexesForPromisingGenomes.Add( i );
					this.showBacktest(
						(WFLagWeightedPositions)genome.Meaning );
				}
			}
			numberOfDifferentFitness += 23 - 3 - 20;
		}
		#endregion debugCurrentGeneration

	}
}
