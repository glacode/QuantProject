/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerSimpleSelection.cs
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
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.Data.DataTables;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TickerSelectionTesting.SimpleSelection
{
	
  /// <summary>
  /// Base class for EndOfDayTimerHandlers for simple selection
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerSimpleSelection : EndOfDayTimerHandler
  {
    public EndOfDayTimerHandlerSimpleSelection(string tickerGroupID, int numberOfEligibleTickers,
                                int numberOfTickersToBeChosen, int numDaysForOptimizationPeriod, Account account,
                                string benchmark, double targetReturn,
                                PortfolioType portfolioType):
    														base(tickerGroupID, numberOfEligibleTickers, 
                                numberOfTickersToBeChosen, numDaysForOptimizationPeriod, account,
                                0,0,
                                benchmark, targetReturn,
                                portfolioType)
    {
    	
    }

    protected override void marketOpenEventHandler(
      Object sender , DateTime dateTime )
    {
      ;
    }
    protected override void marketCloseEventHandler(
      Object sender , DateTime dateTime )
    {
      ;
    }
    protected override void oneHourAfterMarketCloseEventHandler(
      Object sender , DateTime dateTime )
    {
      ;
    }
  } // end of class
}
