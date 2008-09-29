/*
QuantProject - Quantitative Finance Library

GenomeManagerForEfficientOTCCTOPortfolio.cs
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
	/// portfolios based on the OTC and CTO strategy, using the
	/// GeneticOptimizer
	/// </summary>
	[Serializable]
	public class GenomeManagerForEfficientOTCCTOPortfolio : GenomeManagerForEfficientPortfolio
	{
		private ReturnsManager returnsManager;
		
		public GenomeManagerForEfficientOTCCTOPortfolio(DataTable setOfInitialTickers,
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
		//if the genome manager derives from genome manager without weights,
		//delete override key word
		protected ReturnIntervals getReturnIntervals(
			DateTime firstDateTime,
			DateTime lastDateTime)
		{
			return
				new OpenToCloseCloseToOpenIntervals(
					firstDateTime,
					lastDateTime,
					this.benchmark);
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
				new ReturnsManager( this.getReturnIntervals(firstDateTime,
				                                            lastDateTime),
				                   new HistoricalAdjustedQuoteProvider() );
		}
		
		protected override float[] getStrategyReturns()
		{
			float[] returnValue;
			returnValue = this.weightedPositionsFromGenome.GetReturns(
				this.returnsManager ) ;
			for(int i = 0; i<returnValue.Length; i++)
				if(i%2 != 0)
				//returnValue[i] is a CloseToOpen return:
				//the strategy implies to reverse positions at night
				returnValue[i] = - returnValue[i];
			
			return returnValue;
		}
	}
}
