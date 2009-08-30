/*
QuantProject - Quantitative Finance Library

InefficiencyMovingBackEntryCondition.cs
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
	/// Description of InefficiencyMovingBackEntryCondition.
	/// </summary>
	[Serializable]
	public class InefficiencyMovingBackEntryCondition : IEntryCondition
	{
		private double coefficientForThresholdLevelComputationForMovingBackSignal;
		private HistoricalMarketValueProvider marketValueProvider;
		private Benchmark benchmark;
		
		public InefficiencyMovingBackEntryCondition(double coefficientForThresholdLevelComputationForMovingBackSignal,
																		HistoricalMarketValueProvider marketValueProvider,
																		Benchmark benchmark)
		{
			this.coefficientForThresholdLevelComputationForMovingBackSignal =
				coefficientForThresholdLevelComputationForMovingBackSignal;
			this.marketValueProvider = marketValueProvider;
			this.benchmark = benchmark;
		}
				
		public bool IsConditionSatisfiedByGivenPVOPositions(DateTime dateTime ,
																										 		PVOPositions pvoPositionsForOutOfSample )
		{
			bool returnValue = false;
			DateTime beginOfOscillatingPeriod = pvoPositionsForOutOfSample.LastInefficiencyDateTime;
			DateTime endOfOscillatingPeriod = dateTime;
			PVOPositionsStatus currentStatusForCurrentPositions =
				PVOPositionsStatus.InTheMiddle;
			if( pvoPositionsForOutOfSample != null )
				try{
				currentStatusForCurrentPositions =
					pvoPositionsForOutOfSample.GetStatus( beginOfOscillatingPeriod, endOfOscillatingPeriod,
					                                       this.benchmark.Ticker, this.marketValueProvider,
					                                       pvoPositionsForOutOfSample.OversoldThreshold * coefficientForThresholdLevelComputationForMovingBackSignal,
					                                       pvoPositionsForOutOfSample.OversoldThreshold,
					                                       pvoPositionsForOutOfSample.OverboughtThreshold * coefficientForThresholdLevelComputationForMovingBackSignal,
					                                       pvoPositionsForOutOfSample.OverboughtThreshold );
				
			}catch{}
			returnValue = ( (currentStatusForCurrentPositions == PVOPositionsStatus.Overbought &&
			                 pvoPositionsForOutOfSample.StatusAtLastInefficiencyTime == PVOPositionsStatus.Oversold) ||
			               (currentStatusForCurrentPositions == PVOPositionsStatus.Oversold  &&
			                pvoPositionsForOutOfSample.StatusAtLastInefficiencyTime == PVOPositionsStatus.Overbought) );
			return returnValue;
		}
	}
}
