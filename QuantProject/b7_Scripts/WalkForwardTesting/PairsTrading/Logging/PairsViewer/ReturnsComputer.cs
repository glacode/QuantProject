/*
QuantProject - Quantitative Finance Library

ReturnsComputer.cs
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

using QuantProject.ADT.Histories;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Computes returns to be shown by the PairsViewer
	/// </summary>
	public class ReturnsComputer
	{
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		private IIntervalBeginFinder intervalBeginFinder;
		
		private DateTime lastComputedIntervalBeginDateTime;
		private string currentTicker;
		private double marketValueAtTheIntervalBegin;
		
		public ReturnsComputer(
			HistoricalMarketValueProvider historicalMarketValueProvider ,
			IIntervalBeginFinder intervalBeginFinder )
		{
			this.historicalMarketValueProvider = historicalMarketValueProvider;
			this.intervalBeginFinder = intervalBeginFinder;
			
			this.lastComputedIntervalBeginDateTime = DateTime.MinValue;
			this.currentTicker = "undefinedValue_itHasToBeInitialized";
		}
		
		#region getReturns
		
		#region addReturnsToHistory
		
		#region addReturnToHistory
		
		#region getMarketValueAtTheIntervalBegin
		
		#region updateCacheValuesForIntervalBeginIfTheCase
		
		#region updateCacheValueForIntervalBeginDateTime
//		protected virtual DateTime getIntervalBeginDateTime( DateTime dateTime )
//		{
//			DateTime yesterday = dateTime.AddDays( -1 );
//			DateTime yesterdayAtClose = HistoricalEndOfDayTimer.GetMarketClose( yesterday );
//			DateTime intervalBeginDateTime = yesterdayAtClose.AddMinutes( -1 );
//			return intervalBeginDateTime;
//		}
		private bool updateLastComputedIntervalBeginDateTime( DateTime dateTime )
		{
//			DateTime intervalBeginDateTime = this.getIntervalBeginDateTime( dateTime );
			DateTime intervalBeginDateTime =
				this.intervalBeginFinder.GetIntervalBeginDateTime( dateTime );
			bool isUpdated = ( intervalBeginDateTime != this.lastComputedIntervalBeginDateTime );
			if ( isUpdated )
				// the interval begin has changed since the last return calculation
				this.lastComputedIntervalBeginDateTime = intervalBeginDateTime;
			return isUpdated;
		}
		#endregion updateCacheValueForIntervalBeginDateTime
		
		private bool updateCurrentTicker( WeightedPosition weightedPosition )
		{
			bool isUpdated = ( this.currentTicker != weightedPosition.Ticker );
			if ( isUpdated )
				// the ticker has changed since the last return calculation
				this.currentTicker = weightedPosition.Ticker;
			return isUpdated;
		}

		private void updateCacheValuesForIntervalBeginIfTheCase(
			DateTime dateTime , WeightedPosition weightedPosition )
		{
			bool isIntervalBeginDateTimeUpdated =
				this.updateLastComputedIntervalBeginDateTime( dateTime );
			bool isCurrentTickerUpdated = this.updateCurrentTicker( weightedPosition );
			if ( isIntervalBeginDateTimeUpdated || isCurrentTickerUpdated )
				// either the interval begin or the ticker has been updated
				this.marketValueAtTheIntervalBegin =
					this.historicalMarketValueProvider.GetMarketValue(
						this.currentTicker ,
						this.lastComputedIntervalBeginDateTime );
		}
		#endregion updateCacheValuesForIntervalBeginIfTheCase
		
		private double getMarketValueAtTheIntervalBegin(
			DateTime dateTime , WeightedPosition weightedPosition )
		{
			this.updateCacheValuesForIntervalBeginIfTheCase( dateTime , weightedPosition );
			return this.marketValueAtTheIntervalBegin;
		}
		#endregion getMarketValueAtTheIntervalBegin
		
		private void addReturnToHistory(
			DateTime dateTime , WeightedPosition weightedPosition ,
			History marketValuesForTheWeightedPosition , History returns )
		{
			double marketValueAtTheIntervalBegin = this.getMarketValueAtTheIntervalBegin(
				dateTime , weightedPosition );
			double marketValueAtTheIntervalEnd =
				(double)marketValuesForTheWeightedPosition[ dateTime ];
			double currentReturn = marketValueAtTheIntervalEnd / marketValueAtTheIntervalBegin - 1;
			returns.Add( dateTime , currentReturn );
		}
		#endregion addReturnToHistory
		
		private void addReturnsToHistory(
			WeightedPosition weightedPosition , History marketValuesForTheWeightedPosition ,
			History returns )
		{
			foreach ( DateTime dateTime in marketValuesForTheWeightedPosition.Keys )
				this.addReturnToHistory(
					dateTime , weightedPosition , marketValuesForTheWeightedPosition , returns );
		}
		#endregion addReturnsToHistory
		
		/// <summary>
		/// returns the history of returns
		/// </summary>
		/// <param name="weightedPosition"></param>
		/// <param name="marketValuesForTheWeightedPosition"></param>
		/// <returns></returns>
		public History GetReturns(
			WeightedPosition weightedPosition , History marketValuesForTheWeightedPosition )
		{
			History returns = new History();
			this.addReturnsToHistory(
				weightedPosition , marketValuesForTheWeightedPosition , returns );
			return returns;
		}
		#endregion getReturns
	}
}
