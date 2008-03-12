/*
QuantProject - Quantitative Finance Library

FixedLevelOscillatorPVOStrategy.cs
Copyright (C) 2003 
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
using System.Collections;

using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator;

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Fixed Level Oscillators 
	/// Portfolio Value Oscillator strategy
	/// </summary>
	[Serializable]
	public class FixedLevelOscillatorPVOStrategy : EndOfDayTimerHandlerPVO, IEndOfDayStrategy
	{
				
		public FixedLevelOscillatorPVOStrategy( Account accountPVO ,
			                                     WeightedPositions chosenWeightedPositions,
                                           double oversoldThreshold,
                                           double overboughtThreshold,
                                           int numDaysForOscillatingPeriod):
                                          base("", 0, chosenWeightedPositions.Count, 0,
                                          accountPVO,                                
                                          0,0,
                                          "^GSPC",
                                          0, 0, 0, 0, 0, 0, false, false, 0, 
                                          PortfolioType.ShortAndLong, 0.5, 0.5)
		{
			this.chosenWeightedPositions = chosenWeightedPositions;
      this.currentOversoldThreshold = oversoldThreshold;
      this.currentOverboughtThreshold = overboughtThreshold;
      this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
		}
		
    public void FiveMinutesBeforeMarketCloseEventHandler( Object sender ,
      EndOfDayTimingEventArgs endOfDayTimingEventArgs)
    {
    }

    protected override double getCurrentChosenWeightedPositionsReturn(IndexBasedEndOfDayTimer timer)
    {
      double returnValue = 999.0;
      if(timer.CurrentDateArrayPosition - this.numDaysForOscillatingPeriod >= 0)
      //if there are sufficient data for computing currentChosenWeightedPositionsReturn
      //that's why the method has been overriden
      	returnValue = 
      		 base.getCurrentChosenWeightedPositionsReturn(timer);
      
      return returnValue;
    }   

    public override void OneHourAfterMarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs)
		{
      
		}
	}
}
