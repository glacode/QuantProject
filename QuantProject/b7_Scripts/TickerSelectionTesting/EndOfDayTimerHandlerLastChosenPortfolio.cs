/*
QuantProject - Quantitative Finance Library

EndOfDayTimerHandlerLastChosenPortfolio.cs
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
using QuantProject.Business.Strategies;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;


namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	
  /// <summary>
  /// Implements MarketOpenEventHandler,
  /// MarketCloseEventHandler
  /// These handlers simply open positions - 
  /// for the given chosen tickers - at MarketOpen and close them
  /// at MarketClose
  /// </summary>
  [Serializable]
  public class EndOfDayTimerHandlerLastChosenPortfolio : EndOfDayTimerHandler
  {
  	private DateTime firstDate;
  	private DateTime lastDate;
  	public EndOfDayTimerHandlerLastChosenPortfolio(string[] chosenTickers,
  	                                               PortfolioType portfolioType,
  	                                              Account account,
  	                                             	string benchmark,
  	                                             	DateTime firstDate,
  	                                             	DateTime lastDate):
  		base(new WeightedPositions( new SignedTickers(chosenTickers) ),
  		     portfolioType, account, benchmark)
    {
    	this.firstDate = firstDate;
    	this.lastDate = lastDate;
    }
		 
    /// <summary>
    /// Handles a "Market Open" event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    protected override void marketOpenEventHandler(
      Object sender , DateTime dateTime )
    {
    	if( dateTime == this.firstDate )
    	{
    		this.openPositions();
    	}
    }
		
    protected override void marketCloseEventHandler(
      Object sender , DateTime dateTime )
    {
    	
    	//if(endOfDayTimingEventArgs.EndOfDayDateTime.CompareTo(this.lastDate) == 0)
      //	this.closePositions();
      if(dateTime.CompareTo(this.lastDate.AddDays(-1)) == 0)
      		AccountManager.ClosePositions(this.account);
    }
    
    protected override void oneHourAfterMarketCloseEventHandler(
      Object sender , DateTime dateTime )
    {
    	;
    }
		
  }
}
