/*
QuantProject - Quantitative Finance Library

SelectorByIntradayQuotationAtEachMarketDay.cs
Copyright (C) 2009 
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

using QuantProject.ADT.Histories;
using QuantProject.DataAccess.Tables;
using QuantProject.Data.DataTables;

namespace QuantProject.Data.Selectors
{
  /// <summary>
  /// Class for selection on tickers with at least
  /// the given number of 60 seconds-bars 
  /// for each market day in the given marketDays History
  /// (or the days History the given market index is quoted)
  /// </summary>
   public class SelectorByIntradayQuotationAtEachMarketDay : TickerSelector , ITickerSelector
  {
    private string marketIndex;
    private History marketDays;
    private int minimumNumberOfBarsForEachMarketDay;
    
    public SelectorByIntradayQuotationAtEachMarketDay(DataTable setOfTickersToBeSelected, 
                               DateTime firstQuoteDate,
                               DateTime lastQuoteDate,
                               int minimumNumberOfBarsForEachMarketDay,
                               long maxNumOfReturnedTickers, string marketIndex):
                                    base(setOfTickersToBeSelected, 
                                         false,
                                         firstQuoteDate,
                                         lastQuoteDate,
                                         maxNumOfReturnedTickers)
    {
      this.marketIndex = marketIndex;
      this.minimumNumberOfBarsForEachMarketDay = 
      	minimumNumberOfBarsForEachMarketDay;
    }
     public SelectorByIntradayQuotationAtEachMarketDay(string groupID, 
                                DateTime firstQuoteDate,
                                DateTime lastQuoteDate,
                                int minimumNumberOfBarsForEachMarketDay,
                                long maxNumOfReturnedTickers, string marketIndex):
                                  base(groupID, 
                                      false,
                                      firstQuoteDate,
                                      lastQuoteDate,
                                      maxNumOfReturnedTickers)
    {
    	this.marketIndex = marketIndex;
    	this.minimumNumberOfBarsForEachMarketDay = 
      	minimumNumberOfBarsForEachMarketDay;
    }
    
    public SelectorByIntradayQuotationAtEachMarketDay(
    	DataTable setOfTickersToBeSelected ,
    	History marketDays , int minimumNumberOfBarsForEachMarketDay,
    	long maxNumOfReturnedTickers ):
    	base(setOfTickersToBeSelected ,
    	     false ,
    	     marketDays.FirstDateTime ,
    	     marketDays.LastDateTime ,
    	     maxNumOfReturnedTickers )
    {
    	this.marketIndex = "";
    	this.marketDays = marketDays;
    	this.minimumNumberOfBarsForEachMarketDay = 
      	minimumNumberOfBarsForEachMarketDay;
    }

		 #region GetTableOfSelectedTickers
		 
		 #region getTickersWithBarsInEachMarketDay
		 
		private void getTickersQuotedInEachMarketDay_addRow(DataRow rowToBeAdded, int numberOfTradingDays,
                               DataTable tableToWhichRowIsToBeAdded)
    {
      DataRow newRow = tableToWhichRowIsToBeAdded.NewRow();
      newRow[0]= rowToBeAdded[0];
      newRow["NumberOfDaysWithBars"] = numberOfTradingDays;
      tableToWhichRowIsToBeAdded.Rows.Add(newRow);
    }
		 
		private void getTickersQuotedInEachMarketDay_handleRow(
    	DataRow row , DataTable tableToWhichRowIsToBeAdded )
    {
    	History marketDaysForTicker = 
    		QuantProject.Data.DataTables.Bars.GetMarketDays( (string)row[0],
    	                      															this.firstQuoteDate , this.lastQuoteDate,
    	                      															60,
    	                      															this.minimumNumberOfBarsForEachMarketDay );
    	if( marketDaysForTicker.ContainsAllTheDatesIn( this.marketDays ) )
    		//the current ticker has been effectively traded in each market day
    		this.getTickersQuotedInEachMarketDay_addRow(
    			row , marketDaysForTicker.Count , tableToWhichRowIsToBeAdded );
    }
		 
		private DataTable getTickersWithBarsInEachMarketDay_givenMarketDaysAndGivenSetOfTickers()
		{
		 	if(!this.setOfTickersToBeSelected.Columns.Contains("NumberOfDaysWithBars"))
        this.setOfTickersToBeSelected.Columns.Add("NumberOfDaysWithBars", System.Type.GetType("System.Int32"));
			DataTable returnValue = this.setOfTickersToBeSelected.Clone();
			foreach(DataRow row in setOfTickersToBeSelected.Rows)
				this.getTickersQuotedInEachMarketDay_handleRow(	row , returnValue );
			ExtendedDataTable.DeleteRows( returnValue, maxNumOfReturnedTickers );
			return returnValue;
	 	}
		
		private DataTable getTickersWithBarsInEachMarketDay_givenGroupID()
		{
		 	this.setOfTickersToBeSelected =
    		QuantProject.DataAccess.Tables.Tickers_tickerGroups.GetTickers(this.groupID);
    	this.marketDays = 
    		QuantProject.Data.DataTables.Quotes.GetMarketDays(this.marketIndex,
    	                       															this.firstQuoteDate,
    	                       															this.lastQuoteDate);
		 	return getTickersWithBarsInEachMarketDay_givenMarketDaysAndGivenSetOfTickers();
	 	}
		
		private DataTable getTickersWithBarsInEachMarketDay_givenSetOfTickers()
		{
			this.marketDays = 
				QuantProject.Data.DataTables.Quotes.GetMarketDays(this.marketIndex,
    	                       															this.firstQuoteDate,
    	                       															this.lastQuoteDate);
			return getTickersWithBarsInEachMarketDay_givenMarketDaysAndGivenSetOfTickers();
	 	}
		#endregion
		 
		private DataTable getTableOfSelectedTickers_givenMarketDays()
		{
			DataTable dataTable = 
				this.getTickersWithBarsInEachMarketDay_givenMarketDaysAndGivenSetOfTickers();
			return dataTable;
		}
		 private DataTable getTableOfSelectedTickers_givenMarketIndex()
		 {
			 if(this.marketIndex == "")
				 throw new Exception("You first need to set TickerSelector's property <<MarketIndex>>!");
           
			 if(this.setOfTickersToBeSelected == null)
				 return this.getTickersWithBarsInEachMarketDay_givenGroupID();
			 else
				 return this.getTickersWithBarsInEachMarketDay_givenSetOfTickers();
		 }
		 public DataTable GetTableOfSelectedTickers()
		 {
			 if ( this.marketDays != null )
				 // marketDays has been passed to the constructor
				 return this.getTableOfSelectedTickers_givenMarketDays();
			 else
				 // marketIndex has been passed to the constructor
				 return this.getTableOfSelectedTickers_givenMarketIndex();
		 }
		 #endregion GetTableOfSelectedTickers
		 
		 public void SelectAllTickers()
		 {
			 ;
		 }	
	 }
}


