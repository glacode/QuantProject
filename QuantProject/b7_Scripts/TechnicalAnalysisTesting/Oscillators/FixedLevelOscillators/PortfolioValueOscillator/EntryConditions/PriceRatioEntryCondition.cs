/*
QuantProject - Quantitative Finance Library

PriceRatioEntryCondition.cs
Copyright (C) 2009
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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.TickersRelationships;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.EntryConditions
{
	/// <summary>
	/// Description of PriceRatioEntryCondition.
	/// </summary>
	[Serializable]
	public class PriceRatioEntryCondition : IEntryCondition
	{
		private int lengthInDaysForAveragePriceRatioCalculation;
		private HistoricalMarketValueProvider marketValueProvider;
		private double numOfStdDevForSignificantPriceMovements;
		
		public PriceRatioEntryCondition(int lengthInDaysForAveragePriceRatioCalculation,
																		HistoricalMarketValueProvider marketValueProvider,
																		double numOfStdDevForSignificantPriceMovements)
		{
			this.lengthInDaysForAveragePriceRatioCalculation = 
				lengthInDaysForAveragePriceRatioCalculation;
			this.marketValueProvider = marketValueProvider;
			this.numOfStdDevForSignificantPriceMovements = 
				numOfStdDevForSignificantPriceMovements;
		}
		//da rifare usando marketIntervalsManager
		public bool IsConditionSatisfiedByGivenPVOPositions(DateTime dateTime ,
																										 PVOPositions pvoPositionsForOutOfSample )
		{
			bool returnValue = false;
			try{
				if( pvoPositionsForOutOfSample.WeightedPositions.Count != 2 )
					throw new Exception("Price ratio condition can be verified only by pairs");
				SignedTickers signedTickers =
					pvoPositionsForOutOfSample.WeightedPositions.SignedTickers;
				double priceRatioAverage =
					PriceRatioProvider.GetPriceRatioAverage(signedTickers[0].Ticker, signedTickers[1].Ticker,
			                                            dateTime.AddDays(-this.lengthInDaysForAveragePriceRatioCalculation),
			                                            dateTime.AddDays(-1) );
				double priceRatioStdDev = 
					PriceRatioProvider.GetPriceRatioStandardDeviation(signedTickers[0].Ticker, signedTickers[1].Ticker,
			                                            dateTime.AddDays(-this.lengthInDaysForAveragePriceRatioCalculation),
			                                            dateTime.AddDays(-1) );
				double currentPriceRatio =
					this.marketValueProvider.GetMarketValue(signedTickers[0].Ticker, dateTime ) /
					this.marketValueProvider.GetMarketValue(signedTickers[1].Ticker, dateTime );
				if(currentPriceRatio > priceRatioAverage + this.numOfStdDevForSignificantPriceMovements * priceRatioStdDev ||
			   	currentPriceRatio < priceRatioAverage - this.numOfStdDevForSignificantPriceMovements * priceRatioStdDev )
						returnValue = true;
			}
			catch(Exception ex){
				string s = ex.ToString();
			}
			return returnValue;
		}
	}
}
