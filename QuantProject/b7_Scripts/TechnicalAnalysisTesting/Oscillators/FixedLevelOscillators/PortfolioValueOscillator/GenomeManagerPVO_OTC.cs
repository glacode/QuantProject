/*
QuantProject - Quantitative Finance Library

GenomeManagerPVO_OTC.cs
Copyright (C) 2008
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
using System.Collections;

using QuantProject.ADT;
using QuantProject.ADT.Statistics;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Data;
using QuantProject.Data.DataTables;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Strategies.TickersRelationships;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator
{
	/// <summary>
	/// Implements what needed to use the Genetic Optimizer
	/// for finding the portfolio that best suites
	/// the Portfolio Value Oscillator strategy, based on
	/// open to close returns
	/// </summary>
	[Serializable]
	public class GenomeManagerPVO_OTC : GenomeManagerPVO
	{
		
		public GenomeManagerPVO_OTC(DataTable setOfInitialTickers,
		                            DateTime firstQuoteDate,
		                            DateTime lastQuoteDate,
		                            int numberOfTickersInPortfolio,
		                            int minLevelForOversoldThreshold,
		                            int maxLevelForOversoldThreshold,
		                            int minLevelForOverboughtThreshold,
		                            int maxLevelForOverboughtThreshold,
		                            int divisorForThresholdComputation,
		                            bool symmetricalThresholds,
		                            bool overboughtMoreThanOversoldForFixedPortfolio,
		                            PortfolioType inSamplePortfolioType,
		                            string benchmark)
			:
			base(setOfInitialTickers,
			     firstQuoteDate,
			     lastQuoteDate,
			     numberOfTickersInPortfolio,
			     1,
			     minLevelForOversoldThreshold,
			     maxLevelForOversoldThreshold,
			     minLevelForOverboughtThreshold,
			     maxLevelForOverboughtThreshold,
			     divisorForThresholdComputation,
			     symmetricalThresholds,
			     overboughtMoreThanOversoldForFixedPortfolio,
			     inSamplePortfolioType,
			     benchmark)
			
			
		{
			
		}
		
		protected override void setReturnsManager(DateTime firstQuoteDate,
		                                          DateTime lastQuoteDate)
		{
			DateTime firstDateTime =
				HistoricalEndOfDayTimer.GetMarketOpen( firstQuoteDate );
//				new EndOfDayDateTime(firstQuoteDate,
			//    		EndOfDaySpecificTime.MarketOpen);
			DateTime lastDateTime =
				HistoricalEndOfDayTimer.GetMarketClose( lastQuoteDate );
//				new EndOfDayDateTime(lastQuoteDate,
//				EndOfDaySpecificTime.MarketClose);
			this.returnsManager =
				new ReturnsManager( new DailyOpenToCloseIntervals(
					firstDateTime,
					lastDateTime,
					this.benchmark),
				                   new HistoricalRawQuoteProvider() );
		}
		
		
		//fitness is a sharpe-ratio based indicator for the equity line resulting
		//from applying the strategy
		public override double GetFitnessValue(Genome genome)
		{
			//NEW implementation: fitness is just the pearson correlation
			//applied to two tickers. This kind of fitness is only valid
			//for tests with 2-tickers portfolios
			double returnValue = -2.0;
			if(this.correlationProvider == null)
				this.correlationProvider = new OpenToCloseCorrelationProvider(
					QuantProject.ADT.ExtendedDataTable.GetArrayOfStringFromColumn(this.setOfTickers, 0),
					this.returnsManager, 0.0001f, 0.5f);
			string firstTicker = this.getFitnessValue_getFirstTickerFromGenome(genome);
			string secondTicker = this.getFitnessValue_getSecondTickerFromGenome(genome);
			if(  ( firstTicker.StartsWith("-") && !secondTicker.StartsWith("-") ) ||
			   ( secondTicker.StartsWith("-") && !firstTicker.StartsWith("-") )     )
				//tickers have to be opposite in sign
			{
				double correlationIndex = correlationProvider.GetPearsonCorrelation(
					SignedTicker.GetTicker(firstTicker),
					SignedTicker.GetTicker(secondTicker) );
				if(correlationIndex < 0.96)
					//	if correlation index is not too high to be
					//  probably originated by the same instrument
					returnValue = correlationIndex;
			}
			return returnValue;
		}
	}
}
