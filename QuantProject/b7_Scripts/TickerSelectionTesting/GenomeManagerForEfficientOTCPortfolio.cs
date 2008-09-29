/*
QuantProject - Quantitative Finance Library

GenomeManagerForEfficientOTCPortfolio.cs
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
using System.Data;

using QuantProject.Business.DataProviders;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;

namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	/// <summary>
	/// This class implements IGenomeManager, in order to find efficient
	/// portfolios based on tickers' OpenToClose rates, using the
	/// GeneticOptimizer
	/// </summary>
	[Serializable]
	public class GenomeManagerForEfficientOTCPortfolio : GenomeManagerForEfficientPortfolio
	{
		private ReturnsManager returnsManager;
		public GenomeManagerForEfficientOTCPortfolio(DataTable setOfInitialTickers,
		                                             DateTime firstQuoteDate,
		                                             DateTime lastQuoteDate,
		                                             int numberOfTickersInPortfolio,
		                                             double targetPerformance,
		                                             PortfolioType portfolioType,
		                                             string benchmark)
			:base(setOfInitialTickers,
			      firstQuoteDate,
			      lastQuoteDate,
			      numberOfTickersInPortfolio,
			      targetPerformance,
			      portfolioType,
			      benchmark)
			
		{
			this.setReturnsManager(firstQuoteDate, lastQuoteDate);
		}
		
		private void setReturnsManager(DateTime firstQuoteDate,
		                               DateTime lastQuoteDate)
		{
			DateTime firstDateTime =
				HistoricalEndOfDayTimer.GetMarketOpen( firstQuoteDate );
//				new EndOfDayDateTime(firstQuoteDate, EndOfDaySpecificTime.MarketOpen);
			DateTime lastDateTime =
				HistoricalEndOfDayTimer.GetMarketClose( lastQuoteDate );
//				new EndOfDayDateTime(lastQuoteDate, EndOfDaySpecificTime.MarketClose);
			this.returnsManager =
				new ReturnsManager(
					new DailyOpenToCloseIntervals(
						firstDateTime,
						lastDateTime,
						this.benchmark ),
					new HistoricalRawQuoteProvider() );
		}
		
		protected override float[] getStrategyReturns()
		{
			return
				this.weightedPositionsFromGenome.GetReturns(this.returnsManager);
		}
	}
}
