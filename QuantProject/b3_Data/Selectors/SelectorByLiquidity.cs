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
		private long minVolume;
		private long numberOfTopRowsToDelete;
    
		#region SelectorByLiquidity Constructors
		
		private void selectorByLiquidity_checkParameters(DataTable setOfTickersToBeSelected,
		                                                 long maxNumOfReturnedTickers,
                               											 long numberOfTopRowsToDelete)
		{
			if( (int)maxNumOfReturnedTickers+(int)numberOfTopRowsToDelete > 
			   setOfTickersToBeSelected.Rows.Count )
				throw new Exception("parameters maxNumOfReturnedTickers=" + maxNumOfReturnedTickers.ToString() +
				                    " and numberOfTopRowsToDelete=" + numberOfTopRowsToDelete.ToString() +
				                    " are too high " +
				                    "for the given setOfTickersToBeSelected (num of rows: " +
				                    setOfTickersToBeSelected.Rows.Count.ToString() + ")");
		}
		
		/// <summary>
		/// ITickerSelector class for selecting tickers 
		/// by liquidity
		/// </summary>
		/// <param name="setOfTickersToBeSelected">DataTable containing
		/// tickers that have to be selected</param>
		/// <param name="orderInASCmode">If true, returned tickers
		/// are ordered in Ascending mode - from less to most. If false
		/// , returned tickers are ordered in descending mode - from most to less</param>
		/// <param name="firstQuoteDate"></param>
		/// <param name="lastQuoteDate"></param>
		/// <param name="maxNumOfReturnedTickers">Max number of selected
		/// tickers to be returned</param>
		/// <param name="numberOfTopRowsToDelete">Number of top rows that have to be
		/// deleted from the returned table. In other words,
		/// max number of selected tickers are chosen after skipping the
		/// first "numberOfTopRowsToDelete" rows from the tickers' table
		/// resulting from selection by liquidity on the given initial
		/// set of tickers 
		/// </param>
		/// <returns></returns>
    public SelectorByLiquidity(DataTable setOfTickersToBeSelected, 
                               bool orderInASCmode,
                               DateTime firstQuoteDate,
                               DateTime lastQuoteDate,
                               long maxNumOfReturnedTickers,
                               long numberOfTopRowsToDelete):
                          base(setOfTickersToBeSelected, 
                               orderInASCmode,
															 firstQuoteDate,
															 lastQuoteDate,
															 maxNumOfReturnedTickers)
		{
    	this.selectorByLiquidity_checkParameters(setOfTickersToBeSelected,
		 	                                         maxNumOfReturnedTickers,
		 	                                         numberOfTopRowsToDelete);
			this.minVolume = long.MinValue;
		 	this.numberOfTopRowsToDelete = numberOfTopRowsToDelete;
		}
    /// <summary>
		/// ITickerSelector class for selecting tickers 
		/// by liquidity
		/// </summary>
		/// <param name="setOfTickersToBeSelected">DataTable containing
		/// tickers that have to be selected</param>
		/// <param name="orderInASCmode">If true, returned tickers
		/// are ordered in Ascending mode - from less to most. If false
		/// , returned tickers are ordered in descending mode - from most to less</param>
		/// <param name="firstQuoteDate"></param>
		/// <param name="lastQuoteDate"></param>
		/// <param name="minVolume">Filter by volume: only tickers having
		/// average volume >= minVolume in the given period are selected</param>
		/// <param name="maxNumOfReturnedTickers">Max number of selected
		/// tickers to be returned</param>
		/// <param name="numberOfTopRowsToDelete">Number of top rows that have to be
		/// deleted from the returned table. In other words,
		/// max number of selected tickers are chosen after skipping the
		/// first "numberOfTopRowsToDelete" rows from the tickers' table
		/// resulting from selection by liquidity on the given initial
		/// set of tickers 
		/// </param>
		/// <returns></returns>
		public SelectorByLiquidity(DataTable setOfTickersToBeSelected,
													     bool orderInASCmode,
													     DateTime firstQuoteDate,
													     DateTime lastQuoteDate,
													     long minVolume,
													     long maxNumOfReturnedTickers,
													     long numberOfTopRowsToDelete):
                          base(setOfTickersToBeSelected, 
		                           orderInASCmode,
		                           firstQuoteDate,
		                           lastQuoteDate,
		                           maxNumOfReturnedTickers)
    {
    	this.selectorByLiquidity_checkParameters(setOfTickersToBeSelected,
		 	                                         maxNumOfReturnedTickers,
		 	                                         numberOfTopRowsToDelete);
			this.minVolume = minVolume;
      this.numberOfTopRowsToDelete = numberOfTopRowsToDelete;
    }
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
    	this.selectorByLiquidity_checkParameters(setOfTickersToBeSelected,
		 	                                         maxNumOfReturnedTickers,
		 	                                         0);
			this.minVolume = long.MinValue;
		 	this.numberOfTopRowsToDelete = 0;
		}
    public SelectorByLiquidity(DataTable setOfTickersToBeSelected, 
												       bool orderInASCmode,
												       long minVolume,
												       DateTime firstQuoteDate,
												       DateTime lastQuoteDate,
												       long maxNumOfReturnedTickers):
                          base(setOfTickersToBeSelected, 
                          		 orderInASCmode,
                          		 firstQuoteDate,
                          		 lastQuoteDate,
                          		 maxNumOfReturnedTickers)
    {
    	this.selectorByLiquidity_checkParameters(setOfTickersToBeSelected,
		 	                                         maxNumOfReturnedTickers,
		 	                                         0);
			this.minVolume = minVolume;
      this.numberOfTopRowsToDelete = 0;
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
    	this.minVolume = long.MinValue;
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
