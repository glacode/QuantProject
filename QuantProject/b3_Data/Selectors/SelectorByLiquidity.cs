/*
QuantProject - Quantitative Finance Library

SelectorByLiquidity.cs
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
using System.Data;
using System.Windows.Forms;
using QuantProject.DataAccess.Tables;
using QuantProject.Data.DataTables;

namespace QuantProject.Data.Selectors
{
  /// <summary>
  /// Class for selection on tickers by liquidity
  /// </summary>
   public class SelectorByLiquidity : TickerSelector, ITickerSelector 
  {
		 private long minVolume = long.MinValue;
        
    
    public SelectorByLiquidity(DataTable setOfTickersToBeSelected, 
                               bool orderInASCmode,
                               DateTime firstQuoteDate,
                               DateTime lastQuoteDate,
                               long maxNumOfReturnedTickers):
                                    base(setOfTickersToBeSelected, 
                                         orderInASCmode,
			firstQuoteDate,
			lastQuoteDate,
			maxNumOfReturnedTickers)
		{
     
		}
     public SelectorByLiquidity(DataTable setOfTickersToBeSelected, 
       bool orderInASCmode,
       DateTime firstQuoteDate,
       DateTime lastQuoteDate,
       long minVolume,
       long maxNumOfReturnedTickers):
                                    base(setOfTickersToBeSelected, 
                                    orderInASCmode,
                                    firstQuoteDate,
                                    lastQuoteDate,
                                    maxNumOfReturnedTickers)
     {
        this.minVolume = minVolume;
     }

     public SelectorByLiquidity(string groupID, 
			 bool orderInASCmode,
			 DateTime firstQuoteDate,
			 DateTime lastQuoteDate,
			 long maxNumOfReturnedTickers):
			 base(groupID, 
			 orderInASCmode,
			 firstQuoteDate,
			 lastQuoteDate,
			 maxNumOfReturnedTickers)
		 {
     
		 }

		 public SelectorByLiquidity(string groupID, 
			 bool orderInASCmode,
			 DateTime firstQuoteDate,
			 DateTime lastQuoteDate,
			 long minVolume ,
			 long maxNumOfReturnedTickers ):
			 base(groupID, 
			 orderInASCmode,
			 firstQuoteDate,
			 lastQuoteDate,
			 maxNumOfReturnedTickers)
		 {
			 this.minVolume = minVolume;
		 }


    public DataTable GetTableOfSelectedTickers()
    {
			DataTable returnTickers;
      if(this.setOfTickersToBeSelected == null)
      {
        if ( this.minVolume > long.MinValue )
          // a min volume value has been requested
          returnTickers =
            QuantProject.DataAccess.Tables.Quotes.GetTickersByLiquidity(this.isOrderedInASCMode,
            this.groupID,
            this.firstQuoteDate,
            this.lastQuoteDate,
            this.minVolume ,
            this.maxNumOfReturnedTickers);
        else
          // a min volume value has not been requested
          returnTickers =
            QuantProject.DataAccess.Tables.Quotes.GetTickersByLiquidity(this.isOrderedInASCMode,
            this.groupID,
            this.firstQuoteDate,
            this.lastQuoteDate,
            this.maxNumOfReturnedTickers);
      }
      else//a set of tickers, not a group ID, 
          //has been passed to the selector
      {  
        if ( this.minVolume > long.MinValue )
          // a min volume value has been requested
          returnTickers =
            QuantProject.Data.DataTables.Quotes.GetTickersByLiquidity(this.isOrderedInASCMode,
            this.setOfTickersToBeSelected, 
            this.firstQuoteDate,
            this.lastQuoteDate,
            this.minVolume,
            this.maxNumOfReturnedTickers);
        else
          returnTickers =
            QuantProject.Data.DataTables.Quotes.GetTickersByLiquidity(this.isOrderedInASCMode,
            this.setOfTickersToBeSelected, 
            this.firstQuoteDate,
            this.lastQuoteDate,
            this.maxNumOfReturnedTickers);
      }
      return returnTickers;
		}
		 public void SelectAllTickers()
		 {
			 ;
		 }	

	 }
}
