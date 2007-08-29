/*
QuantProject - Quantitative Finance Library

ImmediateTrendFollowerStrategy.cs
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
using QuantProject.Data;
using QuantProject.Data.DataTables;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Strategies;
using QuantProject.Business.Timing;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Immediate Trend Follower Strategy (it's just the
	/// reversal of the ExtremeCounterTrend strategy 
	/// </summary>
	[Serializable]
	public class ImmediateTrendFollowerStrategy : EndOfDayTimerHandler, IEndOfDayStrategy
	{
		private int numDaysForReturnCalculation;
    private int numOfClosesElapsed = 0;
    private int numOfDaysWithOpenPosition = 0;

		public ImmediateTrendFollowerStrategy( Account account ,
			                              WeightedPositions weightedPositions,
			                              int numDaysForReturnCalculation) :
    																base("", 0, 
                                weightedPositions.Count, 0, account,
                                0,
                                0,
                                "^GSPC", 0.0,
                                PortfolioType.ShortAndLong)
		{
			this.account = account;
			this.chosenWeightedPositions = weightedPositions;
      this.numDaysForReturnCalculation = numDaysForReturnCalculation;
		}
    
    public override void MarketOpenEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
    }
		
    public void FiveMinutesBeforeMarketCloseEventHandler( Object sender ,
      EndOfDayTimingEventArgs endOfDayTimingEventArgs)
    {
    }

    private double marketCloseEventHandler_openPositions_getLastHalfPeriodGain(IndexBasedEndOfDayTimer timer)
    {
      double returnValue = 999.0;
      try
      {
		    DateTime initialDateForHalfPeriod = 
	          (DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition - this.numDaysForReturnCalculation + 1]["quDate"];
	      DateTime finalDateForHalfPeriod = 
	        (DateTime)timer.IndexQuotes.Rows[timer.CurrentDateArrayPosition]["quDate"];
      	returnValue =
	      	 this.chosenWeightedPositions.GetCloseToCloseReturn(initialDateForHalfPeriod,finalDateForHalfPeriod);
      }
    	catch(MissingQuotesException ex)
    	{
    		ex = ex;
    	}
    	return returnValue;
    }   

    private void marketCloseEventHandler_openPositions(IndexBasedEndOfDayTimer timer)
    {
      double lastHalfPeriodGain = 
      		this.marketCloseEventHandler_openPositions_getLastHalfPeriodGain(timer);
    	if(lastHalfPeriodGain != 999.0)
    	//last half period gain has been properly computed
    	{
    		if(lastHalfPeriodGain > 0.0)
          //the portfolio had a gain for the last half period
    			this.openPositions();
    		else//the portfolio had a loss for the last half period
    		{
    			this.chosenWeightedPositions.Reverse();
    			//short the portfolio
    			try{this.openPositions();}
    			catch(Exception ex){ex = ex;}
    			finally{this.chosenWeightedPositions.Reverse();}
    		}
    	}
    }
        
    public override void MarketCloseEventHandler(
      Object sender , EndOfDayTimingEventArgs endOfDayTimingEventArgs )
    {
      if(this.account.Portfolio.Count > 0)
        this.numOfDaysWithOpenPosition++;
      
      if(this.numOfDaysWithOpenPosition == this.numDaysForReturnCalculation)
      	AccountManager.ClosePositions(this.account);
      
      if(this.account.Portfolio.Count == 0 &&
          (this.numOfClosesElapsed + 1) >= this.numDaysForReturnCalculation)
        //portfolio is empty and previous closes can be now checked
      	if(this.account.Portfolio.Count == 0)
      		this.marketCloseEventHandler_openPositions((IndexBasedEndOfDayTimer)sender);
      
      this.numOfClosesElapsed++;
    }
		
    public override void OneHourAfterMarketCloseEventHandler( Object sender ,
			EndOfDayTimingEventArgs endOfDayTimingEventArgs)
		{
      
		}
	}
}
