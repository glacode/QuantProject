/*
QuantProject - Quantitative Finance Library

PreviousPeriodsWereEfficientEntryCondition.cs
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
//using QuantProject.Business.Strategies.TickersRelationships;
using QuantProject.Business.Timing;
using QuantProject.Business.Timing.TimingManagement;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator;

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.EntryConditions
{
	/// <summary>
	/// Description of PreviousPeriodsWereEfficientEntryCondition.
	/// </summary>
	[Serializable]
	public class PreviousPeriodsWereEfficientEntryCondition : IEntryCondition
	{
		private int numberOfPreviousPeriods;
		private HistoricalMarketValueProvider marketValueProvider;
		private double numberOfIntervalsForEachPeriod;
		private MarketIntervalsManager marketIntervalsManager;
		private string tickerForBenchmark;
		
		public PreviousPeriodsWereEfficientEntryCondition(int numberOfPreviousPeriods,
																		HistoricalMarketValueProvider marketValueProvider,
																		double numberOfIntervalsForEachPeriod,
																		MarketIntervalsManager marketIntervalsManager)
		{
			this.numberOfPreviousPeriods = numberOfPreviousPeriods;
			this.marketValueProvider = marketValueProvider;
			this.numberOfIntervalsForEachPeriod = numberOfIntervalsForEachPeriod;
			this.marketIntervalsManager = marketIntervalsManager;
			this.tickerForBenchmark = marketIntervalsManager.Benchmark.Ticker;
		}
//		public PreviousPeriodsWereEfficientEntryCondition(int numberOfPreviousPeriods,
//																		HistoricalMarketValueProvider marketValueProvider,
//																		double numberOfIntervalsForEachPeriod,
//																	  Benchmark benchmark)
//		{
//			this.numberOfPreviousPeriods = numberOfPreviousPeriods;
//			this.marketValueProvider = marketValueProvider;
//			this.numberOfIntervalsForEachPeriod = numberOfIntervalsForEachPeriod;
//			this.tickerForBenchmark = benchmark.Ticker;
//			this.marketIntervalsManager = new MarketDaysManager(benchmark,
//		}
		private PVOPositionsStatus isConditionSatisfiedByGivenPVOPositions_getStatus( DateTime beginOfOscillatingPeriod,
		                                                                              DateTime endOfOscillatingPeriod,
		                                                                             	PVOPositions pvoPositionsForOutOfSample)
		{
			PVOPositionsStatus returnValue = PVOPositionsStatus.NotComputable;
			if(pvoPositionsForOutOfSample != null)
				try{
				returnValue =
					pvoPositionsForOutOfSample.GetStatus(beginOfOscillatingPeriod, endOfOscillatingPeriod,
					                                     this.tickerForBenchmark, this.marketValueProvider);
			}catch{}
			return returnValue;
		}
		public bool IsConditionSatisfiedByGivenPVOPositions(DateTime dateTime ,
																										 		PVOPositions pvoPositionsForOutOfSample )
		{
			bool returnValue = true;
			PVOPositionsStatus statusForCurrentPreviousPeriod;
			for( int i = 1; i <= this.numberOfPreviousPeriods; i++ )
			{
				DateTime beginOfOscillatingPeriod =
					this.marketIntervalsManager.AddMarketIntervals(dateTime, 
					                                       -(i+1)*(int)this.numberOfIntervalsForEachPeriod);
				DateTime endOfOscillatingPeriod =
					this.marketIntervalsManager.AddMarketIntervals (dateTime, 
					                                       -i*(int)this.numberOfIntervalsForEachPeriod);
				statusForCurrentPreviousPeriod =
					this.isConditionSatisfiedByGivenPVOPositions_getStatus( beginOfOscillatingPeriod,
					                                                        endOfOscillatingPeriod,
					                                                        pvoPositionsForOutOfSample );
				if( statusForCurrentPreviousPeriod != PVOPositionsStatus.InTheMiddle )
				{
					returnValue = false;
					i = this.numberOfPreviousPeriods;
				}
			}
			return returnValue;
		}
	}
}
