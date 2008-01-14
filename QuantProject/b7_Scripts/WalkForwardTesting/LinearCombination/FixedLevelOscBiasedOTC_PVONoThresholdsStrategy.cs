/*
QuantProject - Quantitative Finance Library

FixedLevelOscBiasedOTC_PVONoThresholdsStrategy.cs
Copyright (C) 2007 
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
using QuantProject.Data;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.ReturnsManagement;
using QuantProject.Business.Timing;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.BiasedPVO;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.BiasedPVO.BiasedPVONoThresholds;
using QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.BiasedPVO.BiasedOTC_PVONoThresholds;

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Fixed Level Oscillators 
	/// Open To Close Portfolio Value Oscillator Biased strategy:
	/// from a group of non-moving OTC - CTO genomes, the strategy indicates
	/// to buy the portfolio that presents at open the highest gain or loss
	/// closing all positions at close
	/// </summary>
	[Serializable]
	public class FixedLevelOscBiasedOTC_PVONoThresholdsStrategy : EndOfDayTimerHandlerBiasedOTC_PVONoThresholds, IEndOfDayStrategy
	{
    
		public FixedLevelOscBiasedOTC_PVONoThresholdsStrategy( Account accountPVO ,
		                                                      WeightedPositions[] weightedPositionsToEvaluateOutOfSample,
                                           int numOfDifferentGenomesToEvaluateOutOfSample):
                                        base("", 0, 
                                        weightedPositionsToEvaluateOutOfSample[0].Count, 0, accountPVO, 0, 0,
                                        "", numOfDifferentGenomesToEvaluateOutOfSample,0,
                                        PortfolioType.ShortAndLong)
                                       
		{
			this.weightedPositionsToEvaluateOutOfSample = weightedPositionsToEvaluateOutOfSample;
    }
		
    public override void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      if(this.account.Portfolio.Count == 0)
        this.openPositions((IndexBasedEndOfDayTimer)sender);
    }

    public void FiveMinutesBeforeMarketCloseEventHandler( Object sender ,
      EndOfDayTimingEventArgs endOfDayTimingEventArgs)
    {
    }
    		
    protected override double getCurrentWeightedPositionsGainOrLoss(
			IndexBasedEndOfDayTimer timer,
			ReturnsManager returnsManager,
      int indexForChosenWeightedPositions )
    {
      double returnValue = 999.0;
      if(timer.CurrentDateArrayPosition >= 1)
      //if there are sufficient data for computing currentChosenWeightedPositionsValue
      //that's why the method has been overriden
      {
	      try
	      {
          returnValue = 
          	this.weightedPositionsToEvaluateOutOfSample[indexForChosenWeightedPositions].GetReturn(
		    		0 , returnsManager);
	      }
	    	catch(MissingQuotesException ex)
	    	{
	    		ex = ex;
	    	}
      }
      return returnValue;
    }   

    public override void OneHourAfterMarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs)
		{
		}
	}
}
