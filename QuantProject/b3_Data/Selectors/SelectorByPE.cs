/*
QuantProject - Quantitative Finance Library

SelectorByPE.cs
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

using QuantProject.ADT.Histories;
using QuantProject.DataAccess.Tables;
using QuantProject.Data.DataTables;

namespace QuantProject.Data.Selectors
{
  /// <summary>
  /// Class for selection on tickers by using the popular PE ratio 
  /// </summary>
   public class SelectorByPE : TickerSelector , ITickerSelector
  {
   	private double minPE;
   	private double maxPE;
    private int numDaysForFundamentalDataAvailability;
    
    public SelectorByPE(DataTable setOfTickersToBeSelected, 
	                       DateTime firstQuoteDate,
	                       DateTime lastQuoteDate,
	                       double minPE,
	                       double maxPE,
	                       int maxNumberOfEligibleTickersToBeChosen,
	                       bool orderInASCmode):
                          base(setOfTickersToBeSelected, 
                               orderInASCmode,
                               firstQuoteDate,
                               lastQuoteDate,
                               maxNumberOfEligibleTickersToBeChosen)
    {
      this.minPE = minPE;
    	this.maxPE = maxPE;
      this.numDaysForFundamentalDataAvailability = 45;
    }

		 #region GetTableOfSelectedTickers
		 
		 public DataTable GetTableOfSelectedTickers()
		 {
		 		if(!this.setOfTickersToBeSelected.Columns.Contains("LastAvailableAveragePE"))
		 			this.setOfTickersToBeSelected.Columns.Add("LastAvailableAveragePE", System.Type.GetType("System.Double"));
//		 		returnValue.Columns.Add("AverageOpenPrice", System.Type.GetType("System.Double"));
//		 		returnValue.Columns.Add("LastAvailableEarnings", System.Type.GetType("System.Double"));
		 		
			 	foreach(DataRow row in this.setOfTickersToBeSelected.Rows)
	      {
			 		try
			 		{
			 			row["LastAvailableAveragePE"] = FinancialValues.GetLastFinancialValueForTicker((string)row[0],
			 			  FinancialValueType.AveragePriceEarnings, 12, this.lastQuoteDate.AddDays(-this.numDaysForFundamentalDataAvailability));
			 		}
			 		catch(Exception ex){string str = ex.Message;}
			 	}
	      string filterString = 
	      	"LastAvailableAveragePE is not null AND " + 
	      	"LastAvailableAveragePE >= " + this.minPE.ToString() +
	      	" AND LastAvailableAveragePE <= " + this.maxPE.ToString();
	      
	      DataTable getTickersByPE =
	      	ExtendedDataTable.CopyAndSort(this.setOfTickersToBeSelected,
			 		                              filterString, "LastAvailableAveragePE", this.isOrderedInASCMode);
	      ExtendedDataTable.DeleteRows(getTickersByPE, maxNumOfReturnedTickers);
	      
	      string[] getTickersByPEForDebugging =
	      	ExtendedDataTable.GetArrayOfStringFromRows(getTickersByPE);
	      
	      return getTickersByPE;
		 }
		 #endregion GetTableOfSelectedTickers
		 
		 public void SelectAllTickers()
		 {
			 ;
		 }	
	 }
}
