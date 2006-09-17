/*
QuantProject - Quantitative Finance Library

OTCBruteForceOptimizableParametersManager.cs
Copyright (C) 2006
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

using QuantProject.ADT.Optimizing.BruteForce;
using QuantProject.ADT.Statistics.Combinatorial;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TickerSelectionTesting.TestingOTCTypes.BruteForceOptimization
{
	/// <summary>
	/// This class implements IBruteForceOptimizableParametersManager,
	/// in order to find the best portfolio with respect 
	/// to the OTC - CTO (open to close - close to open) strategy.
	/// Weights are NOT used in this implementation
	/// </summary>
	public class OTCBruteForceOptimizableParametersManager :
		IBruteForceOptimizableParametersManager
	{
		private Combination portfolioCombination;
		private int numberOfPortfolioPositions;
		private GenomeManagerForEfficientOTCCTOPortfolio otcCtoGenomeManager;
		private DataTable eligibleTickersForPortfolioPositions;

		public OTCBruteForceOptimizableParametersManager(
			DataTable eligibleTickersForPortfolioPositions ,
			DateTime firstOptimizationDate ,
			DateTime lastOptimizationDate ,
			int numberOfPortfolioPositions )
		{
			this.eligibleTickersForPortfolioPositions =
				eligibleTickersForPortfolioPositions;
			this.numberOfPortfolioPositions = numberOfPortfolioPositions;
			this.portfolioCombination = new Combination(
				- eligibleTickersForPortfolioPositions.Rows.Count ,
				eligibleTickersForPortfolioPositions.Rows.Count - 1 ,
				numberOfPortfolioPositions );
			this.otcCtoGenomeManager = new GenomeManagerForEfficientOTCCTOPortfolio(
				eligibleTickersForPortfolioPositions ,
				firstOptimizationDate ,
				lastOptimizationDate ,
				numberOfPortfolioPositions , 0.0, PortfolioType.ShortAndLong);
				
		}
		public bool MoveNext()
		{
			return this.portfolioCombination.MoveNext();
		}
		
		public void Reset()
		{
			this.portfolioCombination.Reset();
		}
		#region Current
		public object Current
		{
			get
			{
				return this.getCurrent();
			}
		}
		private object getCurrent()
		{
			int[] currentValues = new int[ this.portfolioCombination.Length ];
			for ( int i = 0 ; i < this.portfolioCombination.Length ; i ++ )
				currentValues[ i ] = this.portfolioCombination.GetValue( i );
			BruteForceOptimizableParameters bruteForceOptimizableParameters =
				new BruteForceOptimizableParameters( currentValues ,
				this );
			return bruteForceOptimizableParameters;
		}
		#endregion
	
		public object Decode( BruteForceOptimizableParameters
			bruteForceOptimizableItem )
		{
			return this.otcCtoGenomeManager.Decode(bruteForceOptimizableItem);
		}
		
		public double GetFitnessValue(
			BruteForceOptimizableParameters bruteForceOptimizableItem )
		{
			return this.otcCtoGenomeManager.GetFitnessValue(bruteForceOptimizableItem);
		}
	}
}
