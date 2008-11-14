/*
QuantProject - Quantitative Finance Library

SelectorByQuotationAtAGivenPercentageOfDateTimes.cs
Copyright (C) 2008 
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
  /// Class for selection on tickers quoted at a given percentage of
  /// date times provided directly or through a given market index
  /// </summary>
   public class SelectorByQuotationAtAGivenPercentageOfDateTimes : TickerSelector , ITickerSelector
  {
    private string marketIndex;
    private History marketDateTimes;
    private double percentageOfDateTimes;
    private int intervalFrameInSeconds;
    
    public SelectorByQuotationAtAGivenPercentageOfDateTimes(DataTable setOfTickersToBeSelected, 
                               bool orderInASCmode,
                               DateTime firstBarDateTime,
                               DateTime lastBarDateTime,
                               int intervalFrameInSeconds,
                               long maxNumOfReturnedTickers, string marketIndex,
                               double percentageOfDateTimes):
                                    base(setOfTickersToBeSelected, 
                                         orderInASCmode,
                                         firstBarDateTime,
                                         lastBarDateTime,
                                         maxNumOfReturnedTickers)
    {
      this.marketIndex = marketIndex;
      this.percentageOfDateTimes = percentageOfDateTimes;
      this.intervalFrameInSeconds = intervalFrameInSeconds;
    }
     public SelectorByQuotationAtAGivenPercentageOfDateTimes(string groupID, 
                                bool orderInASCmode,
                                DateTime firstBarDateTime,
                               	DateTime lastBarDateTime,
                               	int intervalFrameInSeconds,
                                long maxNumOfReturnedTickers, string marketIndex,
                                double percentageOfDateTimes):
                                  base(groupID, 
                                      orderInASCmode,
                                      firstBarDateTime,
                                      lastBarDateTime,
                                      maxNumOfReturnedTickers)
    {
    	this.marketIndex = marketIndex;
    	this.percentageOfDateTimes = percentageOfDateTimes;
    	this.intervalFrameInSeconds = intervalFrameInSeconds;
    }
    
    public SelectorByQuotationAtAGivenPercentageOfDateTimes(
    	DataTable setOfTickersToBeSelected , bool orderInASCmode ,
    	History marketDateTimes , int intervalFrameInSeconds, long maxNumOfReturnedTickers,
      double percentageOfDateTimes):
    	base(setOfTickersToBeSelected ,
    	     orderInASCmode ,
    	     marketDateTimes.FirstDateTime ,
    	     marketDateTimes.LastDateTime ,
    	     maxNumOfReturnedTickers )
    {
    	this.marketIndex = "";
    	this.marketDateTimes = marketDateTimes;
    	this.percentageOfDateTimes = percentageOfDateTimes;
    	this.intervalFrameInSeconds = intervalFrameInSeconds;
    }

		 #region GetTableOfSelectedTickers
		 private DataTable getTableOfSelectedTickers_givenDateTimes()
		 {
			 DataTable dataTable =
				 QuantProject.Data.DataTables.TickerDataTable.GetTickersQuotedAtAGivenPercentageOfDateTimes(
				 this.marketDateTimes , this.percentageOfDateTimes , this.setOfTickersToBeSelected , this.firstQuoteDate ,
				 this.lastQuoteDate , this.intervalFrameInSeconds , this.maxNumOfReturnedTickers );
			 return dataTable;
		 }
		 private DataTable getTableOfSelectedTickers_givenMarketIndex()
		 {
			 if(this.marketIndex == "")
				 throw new Exception("You first need to set TickerSelector's property <<MarketIndex>>!");
           
			 if(this.setOfTickersToBeSelected == null)
				 return QuantProject.Data.DataTables.TickerDataTable.GetTickersQuotedAtAGivenPercentageOfDateTimes(
					 this.marketIndex, this.percentageOfDateTimes , this.groupID, this.firstQuoteDate, this.lastQuoteDate,
					 this.intervalFrameInSeconds, this.maxNumOfReturnedTickers);        

			 else
				 return QuantProject.Data.DataTables.TickerDataTable.GetTickersQuotedAtAGivenPercentageOfDateTimes(
					 this.marketIndex, this.percentageOfDateTimes , this.setOfTickersToBeSelected, this.firstQuoteDate, this.lastQuoteDate,
					 this.intervalFrameInSeconds, this.maxNumOfReturnedTickers);
		 }
		 public DataTable GetTableOfSelectedTickers()
		 {
			 if ( this.marketDateTimes != null )
				 // marketDateTimes has been passed to the constructor
				 return this.getTableOfSelectedTickers_givenDateTimes();
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
