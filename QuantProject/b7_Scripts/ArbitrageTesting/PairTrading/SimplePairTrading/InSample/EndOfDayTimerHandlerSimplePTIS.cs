/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerSimplePTIS.cs
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
using System.Data;
using System.Collections;

using QuantProject.ADT;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Business.DataProviders;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.Data.DataTables;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.ArbitrageTesting.PairTrading.SimplePairTrading
{
	
  /// <summary>
  /// Implements MarketOpenEventHandler,
  /// MarketCloseEventHandler and OneHourAfterMarketCloseEventHandler
  /// for the implementation of the simple pair trading strategy for
  /// two given tickers for a given time interval
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerSimplePTIS : EndOfDayTimerHandlerSimplePT
  {
    private int numDaysForGap;
    private double averageGap;
    private double stdDevGap;

    private void endOfDayTimeHandlerSimplePTIS_setChosenTickers(string firstTicker,
                                                                string secondTicker,
                                                                double averageGap,
                                                                double stdDevGap)
    {
      for(int i = 0;i<this.averageGapsOfChosenTickers.Length;i++)
      {
        this.averageGapsOfChosenTickers[i] = averageGap;
        this.stdDevGapsOfChosenTickers[i] = stdDevGap;
        if(i%2 == 0)
          this.chosenTickers[i] = firstTicker;
        else
          this.chosenTickers[i] = secondTicker;
      }
    }

    public EndOfDayTimerHandlerSimplePTIS(double maxNumOfStdDevForNormalGap,
                                          int numDaysForGap,
                                          double averageGap,
                                          double stdDevGap,
                                          string firstTicker, string secondTicker,
                                          DateTime startDate, DateTime endDate,
                                          Account account) : 
                                          base("",0,0,0,0,
                                          "^GSPC",startDate, endDate,
                                          maxNumOfStdDevForNormalGap,
                                          0,0,0,0, account)
    {
    	this.numDaysForGap = numDaysForGap;
      this.averageGap = averageGap;
      this.stdDevGap = stdDevGap;
      this.minimumGainForClosingPositions = 0.002;
      this.maximumToleratedLoss = 0.02;
      this.endOfDayTimeHandlerSimplePTIS_setChosenTickers(firstTicker, secondTicker,
                                                          averageGap, stdDevGap);
    }
      
    /// <summary>
    /// Handles a "Market close" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    protected override void marketCloseEventHandler(
      Object sender , DateTime dateTime )
    {
    	if(((IndexBasedEndOfDayTimer)sender).GetPreviousDateTime() !=
          dateTime)
      //it is not the first date fired by the timer, so the
      // gap can be computed
          base.marketCloseEventHandler(sender, dateTime);
    }

    /// <summary>
    /// Handles a "One hour after market close" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    protected override void oneHourAfterMarketCloseEventHandler(
      Object sender , DateTime dateTime )
    {
    	
    }

  }
}
