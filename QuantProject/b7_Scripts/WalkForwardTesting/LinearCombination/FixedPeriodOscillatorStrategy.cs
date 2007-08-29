/*
QuantProject - Quantitative Finance Library

FixedPeriodOscillatorStrategy.cs
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

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Close To Close Oscillator strategy
	/// </summary>
	[Serializable]
	public class FixedPeriodOscillatorStrategy : EndOfDayTimerHandler, IEndOfDayStrategy
	{
		private int daysForRightPeriod;
    private int daysForReversalPeriod;
    //length for movement upwards or downwards of the given tickers
    private int daysCounterWithRightPositions;
    private int daysCounterWithReversalPositions;
    private bool isReversalPeriodOn = false;
    private int numOfClosesElapsed = 0;
     

		public FixedPeriodOscillatorStrategy( Account account ,
			                                     WeightedPositions weightedPositions,
                                           int daysForRightPeriod,
                                           int daysForReversalPeriod):
    																		 base("", 0, 
                                weightedPositions.Count, 0, account,
                                0,
                                0,
                                "^GSPC", 0.0,
                                PortfolioType.ShortAndLong)
		{
			this.account = account;
			this.chosenWeightedPositions = weightedPositions;
      this.daysForRightPeriod = daysForRightPeriod;
      this.daysForReversalPeriod = daysForReversalPeriod;
		}
				
    public override void MarketOpenEventHandler(
			Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
		{
    	
		}
		    
    private void marketCloseEventHandler_updateCounters(bool isTheFirstClose)
    {
      if(this.account.Portfolio.Count > 0 && isTheFirstClose == false)
      {
        if(this.isReversalPeriodOn)
          this.daysCounterWithReversalPositions++ ;
        else
          this.daysCounterWithRightPositions++ ;
      }
    }

		public override void MarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs)
		{
      bool firstClose = false;
      if( (this.numOfClosesElapsed + 1) >= 
          (this.daysForRightPeriod + this.daysForReversalPeriod) )
      //strategy can now be applied because it is tuned with the optimization's results
      {
        if (this.account.Transactions.Count == 0)
          // it is the first close
        {
          firstClose = true;
          this.openPositions();
        }

        this.marketCloseEventHandler_updateCounters(firstClose);

        if(firstClose == false && this.isReversalPeriodOn == false &&
           this.daysCounterWithRightPositions == this.daysForRightPeriod)
      
        {
        	AccountManager.ReversePositions(this.account);
          this.isReversalPeriodOn = true;
        }
      
        if(this.isReversalPeriodOn == true &&
           this.daysCounterWithReversalPositions == this.daysForReversalPeriod)
      
        {
          AccountManager.ReversePositions(this.account);
          this.isReversalPeriodOn = false;
        }
      }
      
      this.numOfClosesElapsed++;

		}
		
    public override void OneHourAfterMarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs)
		{
      
		}
    
    public void FiveMinutesBeforeMarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs)
		{
      
		}
	}
}
