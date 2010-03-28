/*
QuantProject - Quantitative Finance Library

ReturnIntervalsBuilderForTradingAndForSignaling.cs
Copyright (C) 2010
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
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// Builds (in sample) return intervals for trading and return intervals
	/// for signaling
	/// </summary>
	[Serializable]
	public class ReturnIntervalsBuilderForTradingAndForSignaling :
		IReturnIntervalsBuilderForTradingAndForSignaling
	{
//		private IReturnIntervalFilter returnIntervalFilterForTrading;
//		private IReturnIntervalFilter returnIntervalFilterForSignaling;
		
		public ReturnIntervalsBuilderForTradingAndForSignaling()
//			IReturnIntervalFilter returnIntervalFilterForTrading ,
//			IReturnIntervalFilter returnIntervalFilterForSignaling )
		{
//			this.returnIntervalFilterForTrading = returnIntervalFilterForTrading;
//			this.returnIntervalFilterForSignaling = returnIntervalFilterForSignaling;
		}
		
		#region BuildIntervals
		
		#region addReturnIntervalForTradingAndReturnIntervalForSignalingIfTheCase
		
		private bool wereAllTickersExchanged(
			string[] tickers ,
			ReturnInterval returnInterval ,
			IHistoricalMarketValueProvider historicalMarketValueProvider )
		{
			bool wereAllExchanged = true;
			int tickerIndex = 0;
			while ( wereAllExchanged && ( tickerIndex < tickers.Length ) )
			{
				string ticker = tickers[ tickerIndex ];
				bool wasExchanged = (
					historicalMarketValueProvider.WasExchanged( ticker , returnInterval.Begin ) &&
					historicalMarketValueProvider.WasExchanged( ticker , returnInterval.End ) );
				wereAllExchanged = wasExchanged;
				tickerIndex++;
			}
			return wereAllExchanged;
		}
		
		/// <summary>
		/// A return interval is added to returnIntervalsForTrading only if
		/// all eligibles for trading are traded on that interval, and the
		/// interval is a trading interval.
		/// A return interval is added to returnIntervalsForSignaling only if
		/// all eligibles for signaling are traded on that interval, and the
		/// interval is a signaling interval.
		/// </summary>
		/// <param name="returnInterval"></param>
		/// <param name="historicalMarketValueProvider"></param>
		/// <param name="eligibleTickersForTrading"></param>
		/// <param name="eligibleTickersForSignaling"></param>
		/// <param name="returnIntervalsForTrading"></param>
		/// <param name="returnIntervalsForSignaling"></param>
		private void addReturnIntervalForTradingAndReturnIntervalForSignalingIfTheCase(
			ReturnInterval returnIntervalCandidateForTrading ,
			IHistoricalMarketValueProvider historicalMarketValueProvider ,
			IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling ,
			string[] eligibleTickersForTrading ,
			string[] eligibleTickersForSignaling ,
			ReturnIntervals returnIntervalsForTrading ,
			ReturnIntervals returnIntervalsForSignaling )
		{
			ReturnInterval returnIntervalForSignaling =
				returnIntervalSelectorForSignaling.GetReturnIntervalUsedForSignaling(
					returnIntervalCandidateForTrading );
			if ( this.wereAllTickersExchanged(
				eligibleTickersForTrading ,
				returnIntervalCandidateForTrading , historicalMarketValueProvider ) &&
			    this.wereAllTickersExchanged(
			    	eligibleTickersForSignaling ,
			    	returnIntervalForSignaling , historicalMarketValueProvider ) )
			{
				// all eligible tickers for trading were exchanged in the current
				// return interval candidate for trading, and all eligible tickers
				// for signaling were exchanged in the current return interval candidate
				// for signaling
				returnIntervalsForTrading.Add( returnIntervalCandidateForTrading );
				returnIntervalsForSignaling.Add( returnIntervalForSignaling );
			}				
//				bool wereAllEligibleTickersForTradingExchanged =
//					this.wereAllTickersExchanged(
//						eligibleTickersForTrading ,
//						returnIntervalCandidateForTrading , historicalMarketValueProvider );
//			bool isReturnIntervalValidForTrading =
//				this.returnIntervalFilterForTrading.IsValid( returnIntervalCandidateForTrading );
//			bool isReturnIntervalToBeAddedToReturnIntervalsForTrading = (
//				wereAllEligibleTickersForTradingExchanged &&
//				isReturnIntervalValidForTrading );
//			if ( isReturnIntervalToBeAddedToReturnIntervalsForTrading )
//				returnIntervalsForTrading.Add( returnIntervalCandidateForTrading );
//			
//			bool wereAllEligibleTickersForSignalingExchanged =
//				this.wereAllTickersExchanged(
//					eligibleTickersForSignaling ,
//					returnIntervalCandidateForTrading , historicalMarketValueProvider );
//			bool isReturnIntervalValidForSignaling =
//				this.returnIntervalFilterForSignaling.IsValid( returnIntervalCandidateForTrading );
//			bool isReturnIntervalToBeAddedToReturnIntervalsForSignaling = (
//				wereAllEligibleTickersForSignalingExchanged &&
//				isReturnIntervalValidForSignaling );
//			if ( isReturnIntervalToBeAddedToReturnIntervalsForSignaling )
//				returnIntervalsForSignaling.Add( returnIntervalCandidateForTrading );
		}
		#endregion addReturnIntervalForTradingAndReturnIntervalForSignalingIfTheCase
		
		
		#region removeReturnIntervalsFromSignalingIfTheCorrespondingIsNotInTradingAndViceversa
		private void
			removeReturnIntervalsFromSignalingIfTheCorrespondingIsNotInTradingAndViceversa(
				ReturnIntervals returnIntervalsCandidates ,
				ReturnIntervals returnIntervalsForTrading ,
				ReturnIntervals returnIntervalsForSignaling )
		{
//			int indexForCurrentCandidate = 0;
//			int indexForTradingThatCouldBeRemoved = 0;
//			int indexForSignalingThatCouldBeRemoved = 0;
//			while ( indexForCurrentCandidate < returnIntervalsCandidates.Count )
//			{
//				ReturnInterval currentCandidate =
//					returnIntervalsCandidates[ indexForCurrentCandidate ];
//				if ( this.isToBeRemovedFromTrading( currentCandidate
//			}
		}
		#endregion removeReturnIntervalsFromSignalingIfTheCorrespondingIsNotInTradingAndViceversa
		
		/// <summary>
		/// Builds returnIntervalsForTrading and returnIntervalsForSignaling
		/// </summary>
		/// <param name="returnsManager">contains the ReturnIntervals for the
		/// trading tickers; it also contains the IHistoricalMarketValueProvider
		/// to be used to see if eligibleTickersForTrading and eligibleTickersForSignaling
		/// were exchanged at given DateTime(s)</param></param>
		/// <param name="eligibleTickersForTrading"></param>
		/// <param name="eligibleTickersForSignaling"></param>
		/// <param name="returnIntervalsForTrading"></param>
		/// <param name="returnIntervalsForSignaling"></param>
		public void BuildIntervals(
			IReturnsManager returnsManager ,
			IReturnIntervalSelectorForSignaling returnIntervalSelectorForSignaling ,
			string[] eligibleTickersForTrading ,
			string[] eligibleTickersForSignaling ,
			out ReturnIntervals returnIntervalsForTrading ,
			out ReturnIntervals returnIntervalsForSignaling )
		{
			ReturnIntervals returnIntervalsCandidatesForTrading = returnsManager.ReturnIntervals;
			returnIntervalsForTrading = new ReturnIntervals();
			returnIntervalsForSignaling = new ReturnIntervals();
			foreach ( ReturnInterval returnIntervalCandidateForTrading in
			         returnIntervalsCandidatesForTrading )
				this.addReturnIntervalForTradingAndReturnIntervalForSignalingIfTheCase(
					returnIntervalCandidateForTrading , returnsManager.HistoricalMarketValueProvider ,
					returnIntervalSelectorForSignaling ,
					eligibleTickersForTrading , eligibleTickersForSignaling ,
					returnIntervalsForTrading , returnIntervalsForSignaling );
			this.removeReturnIntervalsFromSignalingIfTheCorrespondingIsNotInTradingAndViceversa(
				returnIntervalsCandidatesForTrading , returnIntervalsForTrading , returnIntervalsForSignaling );
		}
		#endregion BuildIntervals
	}
}
