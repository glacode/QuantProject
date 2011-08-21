/*
QuantProject - Quantitative Finance Library

SelectorByGroupLiquidityAndPrice.cs
Copyright (C) 2011 
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
using QuantProject.Data.Selectors;

namespace QuantProject.Data.Selectors
{
  /// <summary>
  /// Class for selection on tickers by group, liquidity and price,
  /// implementing ITickerSelectorByDate interface
  /// </summary>
   [Serializable]
   public class SelectorByGroupLiquidityAndPrice : ITickerSelectorByDate 
  {
		private string groupID;
		private bool temporizedGroup;
		private bool orderInASCmode;
		private int lengthInDaysOfPreviousPeriodFromCurrentDate;
		private int maxNumOfReturnedTickers;
		private int numberOfTopRowsToDelete;
    private int numDaysForAveragePriceComputation;
    private double minPrice;
    private DateTime dateTimeOfCurrentSelection;
    private DataTable currentSelection;
		
		/// <summary>
		/// ITickerSelectorByDate class for selecting tickers 
		/// by group, liquidity and price
		/// </summary>
		/// <param name="groupID">Group containing
		/// tickers that have to be selected</param>
		/// <param name="temporizedGroup">choose true if the set of tickers belonging
		/// to the group depends on time </param>
		/// <param name="orderInASCmode">In each step of the selecting process,
		/// if true, returned tickers
		/// are ordered in Ascending mode - from less to most. If false
		/// , returned tickers are ordered in descending mode - from most to less.</param>
		/// <param name="lengthInDaysOfPreviousPeriodFromCurrentDate">The lenght in days
		/// of the time window - before current time - the selection
		/// process takes into account </param>
		/// <param name="maxNumOfReturnedTickers">Max number of selected
		/// tickers to be returned</param>
		/// <returns></returns>
    public SelectorByGroupLiquidityAndPrice(string groupID, bool temporizedGroup, 
																						bool orderInASCmode,
																						int lengthInDaysOfPreviousPeriodFromCurrentDate,
																						int maxNumOfReturnedTickers,
																						int numberOfTopRowsToDelete,
																						int numDaysForAveragePriceComputation,
																						double minPrice)
		{
			this.groupID = groupID;
			this.temporizedGroup = temporizedGroup;
			this.orderInASCmode = orderInASCmode;
			this.lengthInDaysOfPreviousPeriodFromCurrentDate = 
				lengthInDaysOfPreviousPeriodFromCurrentDate;
			this.maxNumOfReturnedTickers = maxNumOfReturnedTickers;
			this.numberOfTopRowsToDelete = numberOfTopRowsToDelete;
			this.numDaysForAveragePriceComputation = numDaysForAveragePriceComputation;
			this.minPrice = minPrice;
			this.dateTimeOfCurrentSelection = new DateTime(1900,1,1,16,0,0);
		}
		
		private void getTableOfSelectedTickers_updateCurrentSelection(DateTime currentDate)
		{
			SelectorByGroup group;
			if(this.temporizedGroup)
			//the group is "temporized": returned set of tickers
			// depends on time
				group = new SelectorByGroup(this.groupID,
				                            currentDate);
      else//the group is not temporized
      	group = new SelectorByGroup(this.groupID);
      DataTable tickersFromGroup = group.GetTableOfSelectedTickers();
      string[] tickersFromGroupForDebugging =
      	ExtendedDataTable.GetArrayOfStringFromRows(tickersFromGroup);
      SelectorByLiquidity mostLiquidSelector =
				new SelectorByLiquidity( tickersFromGroup, false,
      	                         currentDate.AddDays(-this.lengthInDaysOfPreviousPeriodFromCurrentDate),
      	                         currentDate, 10, this.maxNumOfReturnedTickers, this.numberOfTopRowsToDelete);
      DataTable dataTableMostLiquid =	mostLiquidSelector.GetTableOfSelectedTickers();
      string[] tickersFromMostLiquidForDebugging = 
      	ExtendedDataTable.GetArrayOfStringFromRows(dataTableMostLiquid);
      SelectorByAverageRawOpenPrice byPrice =
      		new SelectorByAverageRawOpenPrice(dataTableMostLiquid, false,
      	                                  currentDate.AddDays(-this.lengthInDaysOfPreviousPeriodFromCurrentDate),
      	                                  currentDate,
      	                                  this.maxNumOfReturnedTickers,
      	                                  this.minPrice, 10000, 0.00001, 10000);
     	this.currentSelection = byPrice.GetTableOfSelectedTickers();
     	this.dateTimeOfCurrentSelection = currentDate;
			string[] tickersFromMinPriceForDebugging = 
				ExtendedDataTable.GetArrayOfStringFromRows(this.currentSelection);
		}
		
    public DataTable GetTableOfSelectedTickers(DateTime currentDate)
    {
			if(currentDate != this.dateTimeOfCurrentSelection ||
    	   this.currentSelection == null)
    		this.getTableOfSelectedTickers_updateCurrentSelection(currentDate);
    	return this.currentSelection;
		}
	}
}
