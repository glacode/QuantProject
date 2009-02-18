/*
QuantProject - Quantitative Finance Library

OutOfSampleChooserForAlreadyClosing.cs
Copyright (C) 2008
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

using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Chooses two positions that seem to be an arbitrage opportunity.
	/// Among the eligible couples, a couple is discard if any of the two tickers
	/// have had a either split, or dividend, or any other event that may
	/// have caused an overnight price adjustment
	/// </summary>
	[Serializable]
	public class OutOfSampleChooserForAlreadyClosing :
		OutOfSampleChooserForSingleLongAndShort
	{
		private int numberOfConsecutiveMinutesToSayTheInefficiencyIsClosing;
		
		public OutOfSampleChooserForAlreadyClosing(
			double minThresholdForGoingLong ,
			double maxThresholdForGoingLong ,
			double minThresholdForGoingShort ,
			double maxThresholdForGoingShort ,
			int numberOfConsecutiveMinutesToSayTheInefficiencyIsClosing ) :
			base(
				minThresholdForGoingLong ,
				maxThresholdForGoingLong ,
				minThresholdForGoingShort ,
				maxThresholdForGoingShort )
		{
			this.numberOfConsecutiveMinutesToSayTheInefficiencyIsClosing =
				numberOfConsecutiveMinutesToSayTheInefficiencyIsClosing;
		}
		
		#region getPositionsFromCandidate
		
		#region hasThePriceBeenAdjustedLastNight
		private bool hasThePriceBeenAdjustedLastNight( WeightedPositions candidate )
		{
			// qui!!!
			return false;
		}
		#endregion hasThePriceBeenAdjustedLastNight
		
		#region getWeightedPositionsFromCandidateWithoutAdjustment
		private WeightedPositions getWeightedPositionsFromCandidateWithoutAdjustment(
			ReturnsManager returnsManagerForLastSecondPhaseInterval ,
			WeightedPositions candidate )
		{
//			qui!!!!
//			vedi OutOfSampleChooser.getWeightedPositionsFromCandidate(),
//			testa l'inefficienza allo stesso modo, ma poi aggiungi il test
//				per vedere se c'e' l'inizio di chiusura; devi considerare
//				il passaggio dei tempi: probabilmente ti converra' modificare
//				OutOfSampleChooserForSingleLongAndShort e passargli
//				firstTimeToTestInefficiency nel costruttore per poi modificare
//				OutOfSampleChooser.GetPositionsToBeOpened() togliendo i
//				parametri firstDateTimeToTestInefficiency e dateTimeToClosePositions
			// qui!!!
			return candidate;
		}
		#endregion getWeightedPositionsFromCandidateWithoutAdjustment
		
		protected override WeightedPositions getWeightedPositionsFromCandidate(
			ReturnsManager returnsManagerForLastSecondPhaseInterval ,
			WeightedPositions candidate )
		{
			WeightedPositions weightedPositionsFromCandidate = null;
			if ( !this.hasThePriceBeenAdjustedLastNight( candidate ) )
				weightedPositionsFromCandidate =
					this.getWeightedPositionsFromCandidateWithoutAdjustment(
						returnsManagerForLastSecondPhaseInterval , candidate );
			return weightedPositionsFromCandidate;
		}
		#endregion getPositionsFromCandidate
	}
}
