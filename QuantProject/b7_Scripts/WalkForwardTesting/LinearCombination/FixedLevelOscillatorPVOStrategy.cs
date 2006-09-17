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
			                                     string[] tickers,
			                                     double[] tickersPortfolioWeights,
                                           double oversoldThreshold,
                                           double overboughtThreshold,
                                           int numDaysForOscillatingPeriod):
                                          base("", 0, tickers.Length, 0,
                                          accountPVO,                                
                                          0,0,
                                          "^GSPC",
                                          0, 0, 0, 0, 0, 0, 0,
                                          PortfolioType.ShortAndLong, 0.5)
		{
			this.chosenTickers = tickers;
			this.chosenTickersPortfolioWeights = tickersPortfolioWeights;
      this.currentOversoldThreshold = oversoldThreshold;
      this.currentOverboughtThreshold = overboughtThreshold;
      this.numDaysForOscillatingPeriod = numDaysForOscillatingPeriod;
		}
		
    public void FiveMinutesBeforeMarketCloseEventHandler( Object sender ,
      EndOfDayTimingEventArgs endOfDayTimingEventArgs)
    {
    }

    protected override void marketCloseEventHandler_closePositionsIfNeeded()
    {
      
    }    
		
    protected override double getCurrentChosenTickersValue(IndexBasedEndOfDayTimer timer)
    {
      double returnValue = 999.0;
      if(timer.CurrentDateArrayPosition + 2 >= this.numDaysForOscillatingPeriod)
      {
        try
        {
          DateTime initialDate = 
            (DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition - this.numDaysForOscillatingPeriod + 2]["quDate"];
          //so to replicate exactly in sample scheme, where only numOscillatingDay - 1 returns
          //are computed
          DateTime finalDate = 
            (DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition]["quDate"];
          returnValue =
            SignedTicker.GetCloseToClosePortfolioReturn(
            this.chosenTickers, this.chosenTickersPortfolioWeights,
            initialDate,finalDate) + 1.0;
        }
        catch(Exception ex)
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
