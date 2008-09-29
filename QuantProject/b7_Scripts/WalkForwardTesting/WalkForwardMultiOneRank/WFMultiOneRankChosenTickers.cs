/*
QuantProject - Quantitative Finance Library

WFMultiOneRankChosenTickers.cs
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
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.Timing;
using QuantProject.Scripts.WalkForwardTesting;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardMultiOneRank
{
	/// <summary>
	/// Chooses the best linear combination of positions, with respect
	/// to the one rank strategy
	/// </summary>
	public class WFMultiOneRankChosenTickers : Hashtable , IProgressNotifier
	{
		private WFMultiOneRankEligibleTickers eligibleTickers;
		private int numberOfTickersInPortfolio;
		private int inSampleDays;
		private Timer endOfDayTimer;
		private int generationNumberForGeneticOptimizer;
		private int populationSizeForGeneticOptimizer;

//		private ICollection chosenTickers;

		public WFMultiOneRankChosenTickers(
			WFMultiOneRankEligibleTickers eligibleTickers ,
			int numberOfTickersInPortfolio ,
			int inSampleDays ,
			Timer endOfDayTimer ,
			int generationNumberForGeneticOptimizer ,
			int populationSizeForGeneticOptimizer
			)
		{
			this.eligibleTickers = eligibleTickers;
			this.numberOfTickersInPortfolio = numberOfTickersInPortfolio;
			this.inSampleDays = inSampleDays;
			this.endOfDayTimer = endOfDayTimer;
			this.generationNumberForGeneticOptimizer =
				generationNumberForGeneticOptimizer;
			this.populationSizeForGeneticOptimizer =
				populationSizeForGeneticOptimizer;
		}

		public event NewProgressEventHandler NewProgress;

		#region SetTickers
		private void newGenerationEventHandler(
			object sender , NewGenerationEventArgs e )
		{
			this.NewProgress( sender ,
				new NewProgressEventArgs( e.GenerationCounter , e.GenerationNumber ) );
		}
		private void setTickers_setTickersFromGenome(
			WFMultiOneRankGenomeManager genomeManager ,
			Genome genome )
		{
			string[] genomeMeaning = ( string[] )genomeManager.Decode( genome );
			foreach ( string signedTicker in genomeMeaning )
				this.Add( signedTicker , signedTicker );
		}
		public void SetTickers( WFMultiOneRankEligibleTickers eligibleTickers )
		{
			this.Clear();

			DateTime firstDate =
				this.endOfDayTimer.GetCurrentDateTime().AddDays(
				-( this.inSampleDays - 1 ) );

			WFMultiOneRankGenomeManager genomeManager = 
				new WFMultiOneRankGenomeManager(
				eligibleTickers.EligibleTickers ,
				firstDate ,
				this.endOfDayTimer.GetCurrentDateTime() ,
				this.numberOfTickersInPortfolio ,
				0.0 );
        
			GeneticOptimizer geneticOptimizer = new GeneticOptimizer(
				genomeManager ,
				this.populationSizeForGeneticOptimizer ,
				this.generationNumberForGeneticOptimizer ,
				ConstantsProvider.SeedForRandomGenerator );

			geneticOptimizer.NewGeneration +=
				new NewGenerationEventHandler( this.newGenerationEventHandler );

			geneticOptimizer.Run( false );

			this.setTickers_setTickersFromGenome(
				genomeManager , geneticOptimizer.BestGenome );
		}
		#endregion

	}
}
