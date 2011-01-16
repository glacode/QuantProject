/*
QuantProject - Quantitative Finance Library

SelectorByPriceAndFairValueComparison.cs
Copyright (C) 2010 
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
using QuantProject.

namespace QuantProject.Data.Selectors
{
  /// <summary>
  /// Class for selection on tickers by the relative difference
  /// between price and fair value computed by the given
  /// IFairValueProvider
  /// </summary>
   public class SelectorByPriceAndFairValueComparison : TickerSelector, ITickerSelector 
  {
		private IFairValueProvider fairValueProvider;
    
		#region SelectorByPriceAndFairValueComparison Constructors
		
		/// <summary>
		/// ITickerSelector class for selecting tickers 
		/// by the relative difference
  	/// between price and fair value computed by the given
 		/// IFairValueProvider
		/// </summary>
		/// <param name="fairValueProvider">IFairValueProvider object,
		/// which provides fair value computation for a given
		/// tickers at a given date</param>
		/// <param name="setOfTickersToBeSelected">DataTable containing
		/// tickers that have to be selected</param>
		/// <param name="orderInASCmode">If true, returned tickers
		/// are ordered in Ascending mode - from less to most. If false
		/// , returned tickers are ordered in descending mode - from most to less</param>
		/// <param name="firstDateForFundamentalAnalysis"></param>
		/// <param name="lastDateForFundamentalAnalysis"></param>
		/// <param name="maxNumOfReturnedTickers">Max number of selected
		/// tickers to be returned</param>
		/// <returns></returns>
    public SelectorByPriceAndFairValueComparison(IFairValueProvider fairValueProvider,
      	                                         DataTable setOfTickersToBeSelected,
									                               bool orderInASCmode,
									                               DateTime firstDateForFundamentalAnalysis,
									                               DateTime lastDateForFundamentalAnalysis,
									                               long maxNumOfReturnedTickers):
                          base(setOfTickersToBeSelected, 
                               orderInASCmode,
															 firstDateForFundamentalAnalysis,
															 firstDateForFundamentalAnalysis,
															 maxNumOfReturnedTickers)
		{
			this.fairValueProvider = fairValueProvider;
		}
				
		#endregion SelectorByLiquidityConstructors
		
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
            this.maxNumOfReturnedTickers,
            this.numberOfTopRowsToDelete);
        else
          returnTickers =
            QuantProject.Data.DataTables.Quotes.GetTickersByLiquidity(this.isOrderedInASCMode,
            this.setOfTickersToBeSelected, 
            this.firstQuoteDate,
            this.lastQuoteDate,
            this.maxNumOfReturnedTickers,
            this.numberOfTopRowsToDelete);
      }
      return returnTickers;
		}
		public void SelectAllTickers()
		{
			;
		}	
	}
}
