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
   public class SelectorByCloseToCloseCorrelationToBenchmark : TickerSelector, ITickerSelector
  {
    private string benchmark;
    private bool addBenchmarkToTheGivenSetOfTickers;
    /// <summary>
    /// Creates a new instance of the selector
    /// </summary>
    /// <param name="setOfTickersToBeSelected">The data table containing
    /// in the first column the tickers that have to be ordered by pearson correlation
    /// coefficient to a given benchmark</param>
    /// <param name="benchmark">Benchmark code</param>
    /// <param name="orderInASCmode">Ordering mode</param>
    /// <param name="firstQuoteDate">The first date for the interval</param>
    /// <param name="lastQuoteDate">The last date for the interval</param>
    /// <param name="maxNumOfReturnedTickers">Max number of tickers to be returned</param>
    public SelectorByCloseToCloseCorrelationToBenchmark(DataTable setOfTickersToBeSelected,
                                                        string benchmark,
                                                        bool orderInASCmode,
                                                        DateTime firstQuoteDate,
                                                        DateTime lastQuoteDate,
                                                        long maxNumOfReturnedTickers,
                                                        bool addBenchmarkToTheGivenSetOfTickers):
                                                        base(setOfTickersToBeSelected, 
                                                            orderInASCmode,
                                                            firstQuoteDate,
                                                            lastQuoteDate,
                                                            maxNumOfReturnedTickers)
    {
      this.benchmark = benchmark;
      this.addBenchmarkToTheGivenSetOfTickers = addBenchmarkToTheGivenSetOfTickers;
    }
     
     /// <summary>
     /// Creates a new instance of the selector
     /// </summary>
     /// <param name="groupID">The group ID containing the tickers that have to be ordered by Pearson
     /// 												correlation coefficient to a given benchmark</param>
     /// <param name="benchmark">Benchmark to be used for computation of correlation coefficient</param>
     /// <param name="orderInASCmode">Ordering mode</param>
     /// <param name="firstQuoteDate">The first date for the interval</param>
     /// <param name="lastQuoteDate">The last date for the interval</param>
     /// <param name="maxNumOfReturnedTickers">Max number of tickers to be returned</param>
     /// <param name="addBenchmarkToTheGivenSetOfTickers">If TRUE, the benchmark is added to
     /// 																									output table (with correlation
     /// 																									equal to 1) </param>
     public SelectorByCloseToCloseCorrelationToBenchmark(string groupID, 
                                                        string benchmark,
                                                        bool orderInASCmode,
                                                        DateTime firstQuoteDate,
                                                        DateTime lastQuoteDate,
                                                        long maxNumOfReturnedTickers,
                                                        bool addBenchmarkToTheGivenSetOfTickers):
                                                        base(groupID, 
                                                            orderInASCmode,
                                                            firstQuoteDate,
                                                            lastQuoteDate,
                                                            maxNumOfReturnedTickers)
     {
        this.benchmark = benchmark;
        this.addBenchmarkToTheGivenSetOfTickers = addBenchmarkToTheGivenSetOfTickers;
     }


    public DataTable GetTableOfSelectedTickers()
    {
      if(this.setOfTickersToBeSelected == null)
        return this.getTickersByCloseToCloseCorrelationToBenchmark(this.isOrderedInASCMode,
                                    this.groupID,this.benchmark,
                                    this.firstQuoteDate, this.lastQuoteDate,
                                    this.maxNumOfReturnedTickers);        

      else
        return this.getTickersByCloseToCloseCorrelationToBenchmark(this.isOrderedInASCMode,
          this.setOfTickersToBeSelected,this.benchmark,
          this.firstQuoteDate, this.lastQuoteDate,
          this.maxNumOfReturnedTickers);      
    }
    public void SelectAllTickers()
    {
      ;
    }
    
    private DataTable getTickersByCloseToCloseCorrelationToBenchmark( bool orderByASC,
                                                DataTable setOfTickers, string benchmark,
                                                DateTime firstQuoteDate,
                                                DateTime lastQuoteDate,
                                                long maxNumOfReturnedTickers)
    {
      if(!setOfTickers.Columns.Contains("CloseToCloseCorrelationToBenchmark"))
        setOfTickers.Columns.Add("CloseToCloseCorrelationToBenchmark", System.Type.GetType("System.Double"));
      float[] benchmarkQuotes = QuantProject.Data.DataTables.Quotes.GetArrayOfAdjustedCloseQuotes(benchmark, firstQuoteDate, lastQuoteDate);
      foreach(DataRow row in setOfTickers.Rows)
      {
        float[] tickerQuotes = QuantProject.Data.DataTables.Quotes.GetArrayOfAdjustedCloseQuotes((string)row[0], 
                                firstQuoteDate, lastQuoteDate);
        row["CloseToCloseCorrelationToBenchmark"] =
              BasicFunctions.PearsonCorrelationCoefficient(benchmarkQuotes, tickerQuotes);
      }
      DataTable tableToReturn = ExtendedDataTable.CopyAndSort(setOfTickers,
                                                              "CloseToCloseCorrelationToBenchmark>=0.0 OR " +
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
      return tableToReturn;
    }
    
    private DataTable getTickersByCloseToCloseCorrelationToBenchmark( bool orderByASC,
																					      string groupID, string benchmark,
																					      DateTime firstQuoteDate,
																					      DateTime lastQuoteDate,
																					      long maxNumOfReturnedTickers)
    {
      DataTable tickersOfGroup = Tickers_tickerGroups.GetTickers(groupID);
      return this.getTickersByCloseToCloseCorrelationToBenchmark(orderByASC,
																					      tickersOfGroup, benchmark,
																					      firstQuoteDate,
																					      lastQuoteDate,
																					      maxNumOfReturnedTickers);
    }
    
	
	}
}
