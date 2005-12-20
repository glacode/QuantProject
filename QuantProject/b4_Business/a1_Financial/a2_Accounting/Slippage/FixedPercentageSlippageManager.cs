/*
QuantProject - Quantitative Finance Library

FixedPercentageSlippageManager.cs
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
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Timing;

namespace QuantProject.Business.Financial.Accounting.Slippage
{
	/// <summary>
	/// Slippage Manager for a slippage computed as a fixed percentage of price.
	/// </summary>
	[Serializable]
  public class FixedPercentageSlippageManager : ISlippageManager
	{
		private double percentage;
    private IHistoricalQuoteProvider quoteProvider;
    private IEndOfDayTimer endOfDayTimer;
    public FixedPercentageSlippageManager(IHistoricalQuoteProvider quoteProvider,
                                          IEndOfDayTimer endOfDayTimer,
                                          double percentage)
		{
      this.quoteProvider = quoteProvider;
      this.endOfDayTimer = endOfDayTimer;
      this.percentage = percentage;
		}
		
    public double GetSlippage(Order order)
		{
      double returnValue = 0.0;
      double marketPrice;
      if(order.Type == OrderType.LimitBuy ||
          order.Type == OrderType.LimitCover ||
          order.Type == OrderType.MarketBuy ||
          order.Type == OrderType.MarketCover)
      {
        //it should be GetCurrentBid
        marketPrice = this.quoteProvider.GetMarketValue(order.Instrument.Key,
                                                        endOfDayTimer.GetCurrentTime()); 
      	returnValue = percentage * marketPrice / 100.00;
      }
      else//sell type or sellShort type, limit or not
      {
        //it should be GetCurrentAsk
        marketPrice = this.quoteProvider.GetMarketValue(order.Instrument.Key,
                                                        endOfDayTimer.GetCurrentTime()); 
      	returnValue = -percentage * marketPrice / 100.00;
      }
      return returnValue;
    }
  }
}
