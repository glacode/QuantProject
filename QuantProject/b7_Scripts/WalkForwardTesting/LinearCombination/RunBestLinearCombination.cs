/*
QuantProject - Quantitative Finance Library

RunBestLinearCombination.cs
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

using QuantProject.Business.Scripting;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Script to buy at open and sell at close 
	/// the best in sample linear combination.
	/// The generation rules
	/// (contained in the EndOfDayTimerHandler) are:
	/// - choose the most liquid tickers;
	/// - choose the best linear combination among these tickers
	/// </summary>
	[Serializable]
	public class RunBestLinearCombination : Script
	{
		private string tickerGroupID;
		private int numberOfEligibleTickers;
		private int numberOfTickersToBeChosen;
		private int numDaysForLiquidity;
		private int generationNumberForGeneticOptimizer;
		private int populationSizeForGeneticOptimizer;
		private string benchmark;
		private DateTime firstDate;
		private DateTime lastDate;
		private double targetReturn;
		private PortfolioType portfolioType;

		public RunBestLinearCombination(string tickerGroupID, int numberOfEligibleTickers, 
			int numberOfTickersToBeChosen, int numDaysForLiquidity, 
			int generationNumberForGeneticOptimizer,
			int populationSizeForGeneticOptimizer, string benchmark,
			DateTime firstDate, DateTime lastDate, double targetReturn,
			PortfolioType portfolioType )
		{
			this.tickerGroupID = tickerGroupID;
			this.numberOfEligibleTickers = numberOfEligibleTickers;
			this.numberOfTickersToBeChosen = numberOfTickersToBeChosen;
			this.numDaysForLiquidity = numDaysForLiquidity;
			this.generationNumberForGeneticOptimizer = generationNumberForGeneticOptimizer;
			this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
			this.benchmark = benchmark;
			this.firstDate = firstDate;
			this.lastDate = lastDate;
			this.targetReturn = targetReturn;
			this.portfolioType = portfolioType;
		}

		public override void Run()
    {
			MainForm mainForm = new MainForm( tickerGroupID, numberOfEligibleTickers, 
			numberOfTickersToBeChosen, numDaysForLiquidity, 
			generationNumberForGeneticOptimizer,
			populationSizeForGeneticOptimizer, benchmark,
			firstDate, lastDate, targetReturn,
			portfolioType);
			mainForm.ShowDialog();
		}
	}
}
