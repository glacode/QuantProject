/*
QuantProject - Quantitative Finance Library

OutOfSampleChooserForAlreadyClosing.cs
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

using QuantProject.ADT.Timing;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Strategies.ReturnsManagement.Time;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Chooses two positions that seem to be an arbitrage opportunity.
	/// The measured inefficiency must be seen to have narrowed now, before
	/// giving the entry signal.
	/// Among the eligible couples, a couple is discarded if any of the two tickers
	/// have had a either split, or dividend, or any other event that may
	/// have caused an overnight price adjustment
	/// </summary>
	[Serializable]
	public class OutOfSampleChooserForAlreadyClosing :
		OutOfSampleChooserForSingleLongAndShort
	{
		private IInefficiencyCorrectionDetector inefficiencyCorrectionDetector;
		
		private PriceAdjustmentDetector adjustmentDetector;
		
		public OutOfSampleChooserForAlreadyClosing(
			double minThresholdForGoingLong ,
			double maxThresholdForGoingLong ,
			double minThresholdForGoingShort ,
			double maxThresholdForGoingShort ,
			IInefficiencyCorrectionDetector inefficiencyCorrectionDetector ) :
			base(
				new Time( 1 , 0 , 0 ) ,  // dummy parameter, not used by this derived class
				minThresholdForGoingLong ,
				maxThresholdForGoingLong ,
				minThresholdForGoingShort ,
				maxThresholdForGoingShort )
		{
			this.inefficiencyCorrectionDetector = inefficiencyCorrectionDetector;
			
			this.adjustmentDetector = new PriceAdjustmentDetector();
		}
		
		protected override DateTime getFirstDateTimeToTestInefficiency(DateTime currentDateTime)
		{
			DateTime yesterdayAtCurrentTime = currentDateTime.AddDays( -1 );
			DateTime yesterdayAtClose = HistoricalEndOfDayTimer.GetMarketClose(
				yesterdayAtCurrentTime ).AddMinutes( - 1 );  // I'm subtracting 1 minute
			// because the bar for 15.59 is there much more often
			// and I believe that bar open value to be really
			// next to the official close
			return yesterdayAtClose;
		}
		
		#region getPositionsFromCandidate
		
		#region hasThePriceBeenAdjustedWithinTheInterval
		
//		private double getAdjustedCloseToCloseReturn( string ticker , ReturnInterval returnInterval )
//		{
//			ReturnsManager returnManager = new ReturnsManager(
//				returnIntervals , this.historicalAdjustedQuoteProvider
//
//			}

		private bool doWeKnowThePriceHasNotBeenAdjustedWithinTheInterval(
			WeightedPositions candidate , ReturnInterval returnInterval	)
		{
			bool doWeKnowNotAdjusted = false;
			try
			{
				doWeKnowNotAdjusted =
					(
						( !this.adjustmentDetector.HasThePriceBeenAdjusted(
							candidate[ 0 ].Ticker , returnInterval.Begin , returnInterval.End ) )
						&&
						( !this.adjustmentDetector.HasThePriceBeenAdjusted(
							candidate[ 1 ].Ticker , returnInterval.Begin , returnInterval.End ) )
					);
			}
			catch( TickerNotExchangedException tickerNotExchangedException )
			{
				string toAvoidCompileWarning = tickerNotExchangedException.Message;
			}
			return doWeKnowNotAdjusted;
		}
		#endregion hasThePriceBeenAdjustedWithinTheInterval
		
		#region getWeightedPositionsFromCandidateWithoutAdjustment
		private WeightedPositions getWeightedPositionsFromCandidateWithoutAdjustment(
			DateTime currentDateTime ,
			ReturnsManager returnsManagerToTestInefficiency ,
			WeightedPositions candidate )
		{
			WeightedPositions weightedPositionsToBeReturned = null;
			WeightedPositions inefficientWeightedPositions =
				base.getWeightedPositionsIfThereIsInefficiency(
					currentDateTime , returnsManagerToTestInefficiency , candidate );
			if ( ( inefficientWeightedPositions != null ) &&
			    this.inefficiencyCorrectionDetector.IsInefficiencyCorrectionHappening(
			    	currentDateTime ,
			    	returnsManagerToTestInefficiency.ReturnIntervals[ 0 ] ,
			    	inefficientWeightedPositions ) )
				// the candidate is inefficient but the gap is closing
				weightedPositionsToBeReturned = inefficientWeightedPositions;
			return weightedPositionsToBeReturned;
		}
		#endregion getWeightedPositionsFromCandidateWithoutAdjustment
		
		protected override WeightedPositions getWeightedPositionsIfThereIsInefficiency(
			DateTime currentDateTime ,
			ReturnsManager returnsManagerForLastSecondPhaseInterval ,
			WeightedPositions candidate )
		{
			WeightedPositions weightedPositionsFromCandidate = null;
			if ( this.doWeKnowThePriceHasNotBeenAdjustedWithinTheInterval(
				candidate ,
				returnsManagerForLastSecondPhaseInterval.ReturnIntervals[ 0 ] ) )
				weightedPositionsFromCandidate =
					this.getWeightedPositionsFromCandidateWithoutAdjustment(
						currentDateTime , returnsManagerForLastSecondPhaseInterval , candidate );
			return weightedPositionsFromCandidate;
		}
		#endregion getPositionsFromCandidate
	}
}
