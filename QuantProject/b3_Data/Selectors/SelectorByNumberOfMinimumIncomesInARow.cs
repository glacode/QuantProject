/*
QuantProject - Quantitative Finance Library

SelectorByNumberOfMinimumIncomesInARow.cs
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

using QuantProject.ADT.Histories;
using QuantProject.DataAccess.Tables;
using QuantProject.Data.DataTables;

namespace QuantProject.Data.Selectors
{
  /// <summary>
  /// Class for selection on tickers which have a given number of
  /// minimum incomes previously than a given date
  /// </summary>
   public class SelectorByNumberOfMinimumIncomesInARow : TickerSelector , ITickerSelector
  {
    private double minimumIncome;
   	private int numberOfIncomes;
    private int incomePeriodLengthInMonths;
    private int numDaysForFundamentalDataAvailability;
    
    public SelectorByNumberOfMinimumIncomesInARow(DataTable setOfTickersToBeSelected, 
                               DateTime firstQuoteDate,
                               DateTime lastQuoteDate,
                               double minimumIncome,
                               int numberOfIncomes,
                               int incomePeriodLengthInMonths,
                               int numDaysForFundamentalDataAvailability):
                                    base(setOfTickersToBeSelected, 
                                         true,
                                         firstQuoteDate,
                                         lastQuoteDate,
                                         500)
    {
      this.minimumIncome = minimumIncome;
    	this.numberOfIncomes = numberOfIncomes;
      this.incomePeriodLengthInMonths = incomePeriodLengthInMonths;
      this.numDaysForFundamentalDataAvailability = 
      	numDaysForFundamentalDataAvailability;
    }

		 #region GetTableOfSelectedTickers
		 
		 private bool getTableOfSelectedTickers_areIncomesAllGreaterThanMinimum(DataTable incomesForTicker)
		 {
		 		bool returnValue = true;
		 		double currentIncome;
			 	for(int i = 0; i < incomesForTicker.Rows.Count; i++)
			 	{
			 		currentIncome = 
			 			(double)incomesForTicker.Rows[i]["fvValue"];
			 		if( currentIncome < this.minimumIncome )
				 		returnValue = false;
			 	}
			 	return returnValue;
		 }
		 
		 public DataTable GetTableOfSelectedTickers()
		 {
		 		DataTable returnValue = new DataTable();
		 		returnValue.Columns.Add("Ticker", System.Type.GetType("System.String"));
		 		object[] values = new object[1];
		 		string currentTicker;
			 	DataTable incomesForCurrentTicker;
			 	for(int i = 0; i < this.setOfTickersToBeSelected.Rows.Count; i++)
			 	{
			 		currentTicker = 
			 			(string)this.setOfTickersToBeSelected.Rows[i][0];
			 		incomesForCurrentTicker =
			 			FinancialValues.GetLastFinancialValuesForTicker(currentTicker,
			 			  40, 12, this.lastQuoteDate.AddDays(-this.numDaysForFundamentalDataAvailability));
			 		if( this.getTableOfSelectedTickers_areIncomesAllGreaterThanMinimum(incomesForCurrentTicker) )
			 		{
			 			values[0] = currentTicker;
			 			returnValue.Rows.Add(values);
			 		}
			 	}
			 	return returnValue;
		 }
		 #endregion GetTableOfSelectedTickers
		 
		 public void SelectAllTickers()
		 {
			 ;
		 }	
	 }
}
