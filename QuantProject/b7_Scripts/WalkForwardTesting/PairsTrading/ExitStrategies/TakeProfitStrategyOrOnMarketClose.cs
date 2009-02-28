/*
QuantProject - Quantitative Finance Library

TakeProfitStrategyOrOnMarketClose.cs
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
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// This exit strategy closes positions if the gain is above the given
	/// threshold; in any case, positions are closed at the end of the market day
	/// </summary>
	[Serializable]
	public class TakeProfitStrategyOrOnMarketClose : IExitStrategy
	{
		private double minGainToTakeProfitOn;
		private HistoricalMarketValueProvider historicalMarketValueProvider;
		
		/// <summary>
		/// This exit strategy closes positions if the gain is above the given
		/// threshold; in any case, positions are closed at the end of the market day
		/// </summary>
		/// <param name="minGainToTakeProfitOn">percentage gain above wich
		/// positions are closed</param>
		/// <param name="historicalBarProvider"></param>
		public TakeProfitStrategyOrOnMarketClose(
			double minGainToTakeProfitOn ,
			HistoricalMarketValueProvider historicalMarketValueProvider )
		{
			this.minGainToTakeProfitOn = minGainToTakeProfitOn;
			this.historicalMarketValueProvider = historicalMarketValueProvider;
		}
		
		#region DoWeClosePositions
		
		private void arePositionsToBeClosed_checkParameters(
			DateTime currentDateTime, Account account )
		{
			if ( account.Portfolio.Count == 0 )
				throw new Exception(
					"the method DoWeClosePositions() cannot be invoked if the " +
					"account has no open positions" );
		}
		
		#region arePositionsToBeClosed_actually
		
		#region arePositionsToBeClosed_evenThoughItsNotMarketClose
		
		#region getCurrentGain
		
		#region getReturnsManagerForCurrentGain
		
		#region getReturnIntervalsForCurrentGain
		private ReturnInterval getReturnIntervalForCurrentGain(
			DateTime currentDateTime , Account account )
		{
			DateTime intervalBegin = account.Transactions.LastDateTime;
			DateTime intervalEnd = currentDateTime;
			ReturnInterval returnIntervalForCurrentGain =
				new ReturnInterval( intervalBegin , intervalEnd );
			return returnIntervalForCurrentGain;
		}
		private ReturnIntervals getReturnIntervalsForCurrentGain(
			DateTime currentDateTime , Account account )
		{
			ReturnInterval returnIntervalForCurrentGain =
				this.getReturnIntervalForCurrentGain(
					currentDateTime , account );
			ReturnIntervals returnIntervalsForCurrentGain =
				new ReturnIntervals( returnIntervalForCurrentGain );
			return returnIntervalsForCurrentGain;
		}
		#endregion getReturnIntervalsForCurrentGain
		
		private ReturnsManager getReturnsManagerForCurrentGain(
			DateTime currentDateTime , Account account )
		{
			ReturnIntervals returnIntervalsForCurrentGain =
				this.getReturnIntervalsForCurrentGain( currentDateTime , account );
			ReturnsManager returnsManagerForCurrentGain = new ReturnsManager(
				returnIntervalsForCurrentGain , this.historicalMarketValueProvider );
			return returnsManagerForCurrentGain;
		}
		#endregion getReturnsManagerForCurrentGain
		
		private float getCurrentGain(
			ReturnsManager returnsManagerForCurrentGain , Account account )
		{
			WeightedPositions weightedPositions =
				AccountManager.GetWeightedPositions( account );
			float currentGain = weightedPositions.GetReturn(
				0 , returnsManagerForCurrentGain ) ;
			return currentGain;
		}
		private float getCurrentGain( DateTime currentDateTime , Account account )
		{
			ReturnsManager returnsManagerForCurrentGain = this.getReturnsManagerForCurrentGain(
				currentDateTime, account );
			float currentGain = this.getCurrentGain( returnsManagerForCurrentGain , account );
			return currentGain;
		}
		#endregion getCurrentGain
		
		private bool arePositionsToBeClosed_evenThoughItsNotMarketClose(
			DateTime currentDateTime, Account account )
		{
			bool arePositionsToBeClosed = false;
			try
			{
				float currentGain = this.getCurrentGain( currentDateTime , account );
				arePositionsToBeClosed =
					arePositionsToBeClosed || ( currentGain >= this.minGainToTakeProfitOn );
			}
			catch( TickerNotExchangedException tickerNotExchangedException )
			{
				string toAvoidCompileWarning = tickerNotExchangedException.Message;
			}
			return arePositionsToBeClosed;
		}
		
		#endregion arePositionsToBeClosed_evenThoughItsNotMarketClose
		
		private bool arePositionsToBeClosed_actually( DateTime currentDateTime, Account account )
		{
			bool arePositionsToBeClosed =
				HistoricalEndOfDayTimer.IsMarketClose( currentDateTime );
			arePositionsToBeClosed =
				arePositionsToBeClosed ||
				this.arePositionsToBeClosed_evenThoughItsNotMarketClose(
					currentDateTime, account );
			return arePositionsToBeClosed;
		}
		#endregion arePositionsToBeClosed_actually
		
		public bool ArePositionsToBeClosed( DateTime currentDateTime, Account account )
		{
			this.arePositionsToBeClosed_checkParameters( currentDateTime , account );
			bool arePositionsToBeClosed = this.arePositionsToBeClosed_actually(
				currentDateTime , account );
			return arePositionsToBeClosed;
		}
		#endregion DoWeClosePositions
	}
}
