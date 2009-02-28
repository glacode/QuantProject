/*
QuantProject - Quantitative Finance Library

ConsecutiveMinutesOfCorrectionDetector.cs
Copyright (C) 2009
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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Detects if a the inefficiency gap has been closing for
	/// n times in the last n minutes
	/// </summary>
	[Serializable]
	public class ConsecutiveMinutesOfCorrectionDetector : IInefficiencyCorrectionDetector
	{
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		private int numberOfConsecutiveMinutesToSayTheInefficiencyIsClosing;
		
		public ConsecutiveMinutesOfCorrectionDetector(
			HistoricalMarketValueProvider historicalMarketValueProvider ,
			int numberOfConsecutiveMinutesToSayTheInefficiencyIsClosing )
		{
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.numberOfConsecutiveMinutesToSayTheInefficiencyIsClosing =
				numberOfConsecutiveMinutesToSayTheInefficiencyIsClosing;
		}
		
		#region IsInefficiencyCorrectionHappening
		
		#region getReturnsManagerToCheckForConsecutiveInefficiencyClosing
		
		#region getReturnIntervalsToCheckForConsecutiveInefficiencyClosing
		
		#region addReturnIntervalForMinute
		private ReturnInterval getReturnIntervalForCurrentMinute(
			int minuteIndex , DateTime currentDateTime )
		{
			DateTime returnIntervalBegin = currentDateTime.AddMinutes(
				minuteIndex - this.numberOfConsecutiveMinutesToSayTheInefficiencyIsClosing );
			DateTime returnIntervalEnd = returnIntervalBegin.AddMinutes( 1 );
			ReturnInterval returnIntervalForCurrentMinute =
				new ReturnInterval( returnIntervalBegin , returnIntervalEnd );
			return returnIntervalForCurrentMinute;
		}
		private void addReturnIntervalForMinute(
			int minuteIndex , DateTime currentDateTime ,
			ReturnIntervals returnIntervalsToCheckForConsecutiveInefficiencyClosing )
		{
			ReturnInterval returnInterval = this.getReturnIntervalForCurrentMinute(
				minuteIndex , currentDateTime );
			returnIntervalsToCheckForConsecutiveInefficiencyClosing.Add( returnInterval );
		}
		#endregion addReturnIntervalForMinute
		
		private ReturnIntervals getReturnIntervalsToCheckForConsecutiveInefficiencyClosing(
			DateTime currentDateTime )
		{
			ReturnIntervals returnIntervalsToCheckForConsecutiveInefficiencyClosing =
				new ReturnIntervals();
			for ( int minuteIndex = 0 ;
			     minuteIndex <
			     this.numberOfConsecutiveMinutesToSayTheInefficiencyIsClosing ;
			     minuteIndex++ )
				this.addReturnIntervalForMinute(
					minuteIndex , currentDateTime ,
					returnIntervalsToCheckForConsecutiveInefficiencyClosing );
			return returnIntervalsToCheckForConsecutiveInefficiencyClosing;
		}
		#endregion getReturnIntervalsToCheckForConsecutiveInefficiencyClosing
		
		private ReturnsManager getReturnsManagerToCheckForConsecutiveInefficiencyClosing(
			DateTime currentDateTime )
		{
			ReturnIntervals returnIntervalsToCheckForConsecutiveInefficiencyClosing
				= this.getReturnIntervalsToCheckForConsecutiveInefficiencyClosing(
					currentDateTime );
			ReturnsManager returnsManager = new ReturnsManager(
				returnIntervalsToCheckForConsecutiveInefficiencyClosing ,
				this.historicalMarketValueProvider );
			return returnsManager;
		}
		#endregion getReturnsManagerToCheckForConsecutiveInefficiencyClosing
		
		#region areAllPositiveReturns
		private bool areAllPositiveReturns( float[] returns )
		{
			bool areAllPositive = true;
			int currentReturnIndex = 0;
			while ( areAllPositive && ( currentReturnIndex < returns.Length ) )
			{
				float currentReturn = returns[ currentReturnIndex ];
				areAllPositive = ( currentReturn > 0 );
				currentReturnIndex++;
			}
			return areAllPositive;
		}
		private bool areAllPositiveReturns(
			ReturnsManager returnsManager , WeightedPositions weightedPositions )
		{
			bool areAllNegative = false;
			try
			{
				float[] returns = weightedPositions.GetReturns( returnsManager );
				areAllNegative = this.areAllPositiveReturns( returns );
			}
			catch( TickerNotExchangedException tickerNotExchangedException )
			{
				string toAvoidCompileWarning = tickerNotExchangedException.Message;
			}
			return areAllNegative;
		}
		#endregion areAllPositiveReturns
		
		public bool IsInefficiencyCorrectionHappening(
			DateTime currentDateTime ,
			ReturnInterval returnIntervalWhereInefficiencyWasFound ,
			WeightedPositions weightedPositions )
		{
			ReturnsManager returnManagerToCheckForConsecutiveInefficiencyClosing =
				this.getReturnsManagerToCheckForConsecutiveInefficiencyClosing(
					currentDateTime );
			bool isInefficiencyCorrectionHappening =
				this.areAllPositiveReturns(
					returnManagerToCheckForConsecutiveInefficiencyClosing ,
					weightedPositions );
			return isInefficiencyCorrectionHappening;
		}
		#endregion IsInefficiencyCorrectionHappening
	}
}
