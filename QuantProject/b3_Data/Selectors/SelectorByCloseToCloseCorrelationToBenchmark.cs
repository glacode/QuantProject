/*
QuantProject - Quantitative Finance Library

SelectorByCloseToCloseCorrelationToBenchmark.cs
Copyright (C) 2005 
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
using QuantProject.DataAccess.Tables;
using QuantProject.ADT.Statistics;

namespace QuantProject.Data.Selectors
{
  /// <summary>
  /// Class for selection on tickers by close to close absolute correlation to 
  /// a given benchmark
  /// </summary>
  [Serializable] 
  public class SelectorByCloseToCloseCorrelationToBenchmark : ITickerSelectorByDate
   																														 
  {
    private string benchmark;
    private bool orderInASCMode;
    private int maxNumOfReturnedTickers;
    private ITickerSelectorByDate tickerSelectorByDateForInitialTable;
    private int lengthInDaysOfCorrelationPeriod;
    private bool addBenchmarkToTheGivenSetOfTickers;
    private DateTime dateTimeOfCurrentSelection;
    private DataTable currentSelection;

    
    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="setOfTickersToBeSelected">The data table containing
    /// in the first column the tickers that have to be ordered by pearson correlation
    /// coefficient to a given benchmark</param>
    /// <param name="benchmark">Benchmark code</param>
    /// <param name="orderInASCmode">Ordering mode</param>
    /// <param name="maxNumOfReturnedTickers">Max number of tickers to be returned</param>
    public SelectorByCloseToCloseCorrelationToBenchmark(ITickerSelectorByDate tickerSelectorByDateForInitialTable,
                                                        int lengthInDaysOfCorrelationPeriod,
                                                        string benchmark,
                                                        bool orderInASCMode,
                                                        int maxNumOfReturnedTickers,
                                                        bool addBenchmarkToTheGivenSetOfTickers)
    {
      this.tickerSelectorByDateForInitialTable = tickerSelectorByDateForInitialTable;
      this.lengthInDaysOfCorrelationPeriod = lengthInDaysOfCorrelationPeriod;
    	this.benchmark = benchmark;
    	this.orderInASCMode = orderInASCMode;
    	this.maxNumOfReturnedTickers = maxNumOfReturnedTickers;
      this.addBenchmarkToTheGivenSetOfTickers = addBenchmarkToTheGivenSetOfTickers;
      this.dateTimeOfCurrentSelection = new DateTime(1900,1,1,16,0,0);
    }
    
    public DataTable GetTableOfSelectedTickers(DateTime dateTime)
    {
      if(dateTime != this.dateTimeOfCurrentSelection ||
    	   this.currentSelection == null)
    		this.getTableOfSelectedTickers_updateCurrentSelection(this.orderInASCMode,
    	    this.tickerSelectorByDateForInitialTable.GetTableOfSelectedTickers(dateTime),
    	    this.benchmark,
    	    dateTime.AddDays(-this.lengthInDaysOfCorrelationPeriod), dateTime,
          this.maxNumOfReturnedTickers);
    	
    	return this.currentSelection;
    }
    
    private void getTableOfSelectedTickers_updateCurrentSelection( bool orderByASC,
                                                DataTable setOfTickers, string benchmark,
                                                DateTime firstQuoteDate,
                                                DateTime lastQuoteDate,
                                                long maxNumOfReturnedTickers)
    {
      if(!setOfTickers.Columns.Contains("CloseToCloseCorrelationToBenchmark"))
        setOfTickers.Columns.Add("CloseToCloseCorrelationToBenchmark", System.Type.GetType("System.Double"));
      double[] benchmarkQuotes = QuantProject.Data.DataTables.Quotes.GetDoubleArrayOfAdjustedCloseQuotes(benchmark, firstQuoteDate, lastQuoteDate);
      double correlation;
      foreach(DataRow row in setOfTickers.Rows)
      {
        double[] tickerQuotes = QuantProject.Data.DataTables.Quotes.GetDoubleArrayOfAdjustedCloseQuotes((string)row[0], 
                                firstQuoteDate, lastQuoteDate);
        correlation = 0.0;
        if(benchmarkQuotes.Length == tickerQuotes.Length)
        	correlation = 
        		BasicFunctions.PearsonCorrelationCoefficient(benchmarkQuotes, tickerQuotes);

      	row["CloseToCloseCorrelationToBenchmark"] = correlation;
      }
      DataTable tableToReturn = ExtendedDataTable.CopyAndSort(setOfTickers,
                                                              "CloseToCloseCorrelationToBenchmark>0.0 OR " +
                                                              "CloseToCloseCorrelationToBenchmark<0.0",
                                                              "CloseToCloseCorrelationToBenchmark",
                                                              orderByASC);
      ExtendedDataTable.DeleteRows(tableToReturn, maxNumOfReturnedTickers);
      if(this.addBenchmarkToTheGivenSetOfTickers)
      {
        DataRow newRow = tableToReturn.NewRow();
        newRow[0] = benchmark;
        tableToReturn.Rows.Add(newRow);
      }
      this.currentSelection = tableToReturn;
      this.dateTimeOfCurrentSelection = lastQuoteDate;
			string[] currentSelectionForDebugging = 
				ExtendedDataTable.GetArrayOfStringFromRows(this.currentSelection);
    }
    
//    private DataTable getTickersByCloseToCloseCorrelationToBenchmark( bool orderByASC,
//																					      string groupID, string benchmark,
//																					      DateTime firstQuoteDate,
//																					      DateTime lastQuoteDate,
//																					      long maxNumOfReturnedTickers)
//    {
//      DataTable tickersOfGroup = Tickers_tickerGroups.GetTickers(groupID);
//      return this.getTableOfSelectedTickers_updateCurrentSelection(orderByASC,
//																					      tickersOfGroup, benchmark,
//																					      firstQuoteDate,
//																					      lastQuoteDate,
//																					      maxNumOfReturnedTickers);
//    }
	}
}
