/*
QuantDownloader - Quantitative Finance Library

TickerDataTable.cs
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
using System.Data;
using QuantProject.ADT;
using QuantProject.DataAccess.Tables;


namespace QuantProject.Data.DataTables
{
	/// <summary>
	/// The DataTable where to store tickers.
	/// </summary>
	
  public class TickerDataTable : DataTable
	{
    private static DataTable clipboard;
    public static DataTable Clipboard
    {
      get{ return TickerDataTable.clipboard; }
      set{ TickerDataTable.clipboard = value; }
    }

    public TickerDataTable()
    {
      
    }
    
    public static void AddColumnsOfTickerTable(DataTable table)
    {
     try
      {
      table.Columns.Add("tiTicker", System.Type.GetType("System.String"));
      table.Columns.Add("tiCompanyName", System.Type.GetType("System.String"));
			}
			catch(Exception ex)
			{
				string notUsed = ex.ToString();
			}
    }
    
    #region GetTickersByPerformance

    private static void addColumnsForPerformanceAnalysis(DataTable tableToAnalyze)
    {
      if(!tableToAnalyze.Columns.Contains("SimpleReturn"))
        tableToAnalyze.Columns.Add("SimpleReturn", System.Type.GetType("System.Double"));
      if(!tableToAnalyze.Columns.Contains("PeriodForSimpleReturn"))
        tableToAnalyze.Columns.Add("PeriodForSimpleReturn", System.Type.GetType("System.String"));
    }

    private static string getTickersByPerformance_getFilterExpression(float minAbsoluteSimpleReturn,
      float maxAbsoluteSimpleReturn)
    {
      string returnValue = "";
      if(minAbsoluteSimpleReturn > 0 &&
        maxAbsoluteSimpleReturn > 0)
        //both limits have to be greater than zero
      {
        returnValue = "(" + minAbsoluteSimpleReturn.ToString("#.##") + 
                      "<=SimpleReturn AND " +
                      maxAbsoluteSimpleReturn.ToString("#.##") + 
                      ">=SimpleReturn)" +
                      " OR " +
                      "(" + "-" + maxAbsoluteSimpleReturn.ToString("#.##") +
                      "<SimpleReturn AND " +
                      "-" + minAbsoluteSimpleReturn.ToString("#.##") +
                      ">SimpleReturn)";
      }
      return returnValue;              
    }

    public static DataTable GetTickersByPerformance(bool orderByASC, string groupID,
                                                    DateTime firstQuoteDate,
                                                    DateTime lastQuoteDate,
                                                    long maxNumOfReturnedTickers,
                                                    float minAbsoluteSimpleReturn,
                                                    float maxAbsoluteSimpleReturn)
    {
      DataTable groupOfTicker = QuantProject.DataAccess.Tables.Tickers_tickerGroups.GetTickers(groupID);
      
      //return TickerDataTable.GetTickersByPerformance(orderByASC, groupOfTicker, firstQuoteDate,
      //  lastQuoteDate, maxNumOfReturnedTickers, minAbsoluteSimpleReturn, maxAbsoluteSimpleReturn);
            
      TickerDataTable.addColumnsForPerformanceAnalysis(groupOfTicker);
      DateTime firstAvailableQuoteDate, lastAvailableQuoteDate;
      double firstQuote, lastQuote;
      QuantProject.Data.DataTables.GroupQuotes tickerQuotes = 
                                new QuantProject.Data.DataTables.GroupQuotes(
                                          groupID, new DateTime(1980,1,1), DateTime.Now);
      foreach(DataRow row in groupOfTicker.Rows)
      {
        if(tickerQuotes.GetNumberOfQuotes((string)row[0])>0)
        {
          firstAvailableQuoteDate = tickerQuotes.GetFirstValidQuoteDate((string)row[0], firstQuoteDate);
          lastAvailableQuoteDate =  tickerQuotes.GetFirstValidQuoteDate((string)row[0], lastQuoteDate);
          firstQuote = tickerQuotes.GetAdjustedClose((string)row[0],firstAvailableQuoteDate);
          lastQuote = tickerQuotes.GetAdjustedClose((string)row[0],lastAvailableQuoteDate);
          row["SimpleReturn"] = (lastQuote - firstQuote) / firstQuote;
          row["PeriodForSimpleReturn"] = "From " + firstAvailableQuoteDate.ToShortDateString() + " to " + lastAvailableQuoteDate.ToShortDateString();
        }
      }
      string filterExpression = 
            getTickersByPerformance_getFilterExpression(minAbsoluteSimpleReturn,
                                                        maxAbsoluteSimpleReturn);
      ExtendedDataTable.CopyAndSort(groupOfTicker, filterExpression, 
                                    "SimpleReturn", orderByASC);
      ExtendedDataTable.DeleteRows(groupOfTicker, maxNumOfReturnedTickers);
      return groupOfTicker;              
    }

    public static DataTable GetTickersByPerformance(bool orderByASC, string groupID,
                                                      DateTime firstQuoteDate,
                                                      DateTime lastQuoteDate,
                                                      long maxNumOfReturnedTickers)
    {
      
      return TickerDataTable.GetTickersByPerformance(orderByASC, groupID, firstQuoteDate,
                                                    lastQuoteDate, maxNumOfReturnedTickers,
                                                    -1, -1);
    
    }
    
    
    public static DataTable GetTickersByPerformance(bool orderByASC, DataTable setOfTickers,
                                                    DateTime firstQuoteDate,
                                                    DateTime lastQuoteDate,
                                                    long maxNumOfReturnedTickers,
                                                    float minAbsoluteSimpleReturn,
                                                    float maxAbsoluteSimpleReturn)
    {
      DataTable returnValue;
      TickerDataTable.addColumnsForPerformanceAnalysis(setOfTickers);
      DateTime firstAvailableQuoteDate, lastAvailableQuoteDate;
      double firstQuote, lastQuote;
      foreach(DataRow row in setOfTickers.Rows)
      {
        if(QuantProject.DataAccess.Tables.Quotes.GetNumberOfQuotes((string)row[0]) > 0)
        {
          QuantProject.Data.DataTables.Quotes quotesOfCurrentTicker =
                              new QuantProject.Data.DataTables.Quotes((string)row[0]);
          firstAvailableQuoteDate = quotesOfCurrentTicker.GetFirstValidQuoteDate(firstQuoteDate);
          lastAvailableQuoteDate =  quotesOfCurrentTicker.GetFirstValidQuoteDate(lastQuoteDate);
          firstQuote = quotesOfCurrentTicker.GetAdjustedClose(firstAvailableQuoteDate);
          lastQuote = quotesOfCurrentTicker.GetAdjustedClose(lastAvailableQuoteDate);
          row["SimpleReturn"] = (lastQuote - firstQuote) / firstQuote;
          row["PeriodForSimpleReturn"] = "From " + firstAvailableQuoteDate.ToShortDateString() + " to " + lastAvailableQuoteDate.ToShortDateString();
        }
      }
      string filterExpression = 
      	getTickersByPerformance_getFilterExpression(minAbsoluteSimpleReturn,
      	                                           	maxAbsoluteSimpleReturn);
      returnValue = ExtendedDataTable.CopyAndSort(setOfTickers,
                                                  filterExpression,
                                                  "SimpleReturn",
                                                  orderByASC);
      ExtendedDataTable.DeleteRows(returnValue, maxNumOfReturnedTickers);
      return returnValue;              
    }
    
    public static DataTable GetTickersByPerformance(bool orderByASC, DataTable setOfTickers,
      DateTime firstQuoteDate,
      DateTime lastQuoteDate,
      long maxNumOfReturnedTickers)
    {
      return GetTickersByPerformance(orderByASC, setOfTickers, firstQuoteDate,lastQuoteDate,
        maxNumOfReturnedTickers, -1, -1);              
    }
    #endregion

    
    private static void addColumnNumberOfQuotes(DataTable tableToAnalyze)
    {
      if(!tableToAnalyze.Columns.Contains("NumberOfQuotes"))
        tableToAnalyze.Columns.Add("NumberOfQuotes", System.Type.GetType("System.Int32"));
    }

    private static int getNumberOfTradingDays(string marketIndex,
                                              DateTime firstQuoteDate,
                                              DateTime lastQuoteDate)
    {
      QuantProject.Data.DataTables.Quotes marketQuotes = 
        new QuantProject.Data.DataTables.Quotes(marketIndex, firstQuoteDate, lastQuoteDate);
      return marketQuotes.Rows.Count;
    }

    private static void getTickersQuotedInEachMarketDay_addRow(DataRow rowToBeAdded, int numberOfTradingDays,
                               DataTable tableToWhichRowIsToBeAdded)
    {
      DataRow newRow = tableToWhichRowIsToBeAdded.NewRow();
      newRow[0]= rowToBeAdded[0];
      newRow["NumberOfQuotes"] = numberOfTradingDays;
      tableToWhichRowIsToBeAdded.Rows.Add(newRow);
    }
    public static DataTable GetTickersQuotedInEachMarketDay(string marketIndex, string groupID,
                                                            DateTime firstQuoteDate,
                                                            DateTime lastQuoteDate,
                                                            long maxNumOfReturnedTickers)
    {
      int marketDaysForTheGivenMarket = 
                    TickerDataTable.getNumberOfTradingDays(marketIndex, firstQuoteDate, lastQuoteDate);
      DataTable groupOfTicker = QuantProject.DataAccess.Tables.Tickers_tickerGroups.GetTickers(groupID);
      TickerDataTable.addColumnNumberOfQuotes(groupOfTicker);
      QuantProject.Data.DataTables.GroupQuotes tickerQuotes = 
                new QuantProject.Data.DataTables.GroupQuotes(
                groupID, firstQuoteDate, lastQuoteDate);
      DataTable returnValue = groupOfTicker.Clone(); 
      foreach(DataRow row in groupOfTicker.Rows)
      {
        if(tickerQuotes.GetNumberOfDaysWithEffectiveTrades((string)row[0]) == marketDaysForTheGivenMarket)
        //the current ticker has been effectively traded in each market day
          TickerDataTable.getTickersQuotedInEachMarketDay_addRow(row, marketDaysForTheGivenMarket,
                                                                 returnValue);
      }
      ExtendedDataTable.DeleteRows(returnValue, maxNumOfReturnedTickers);
      return returnValue;              
     
    }
    
    public static DataTable GetTickersQuotedInEachMarketDay(string marketIndex, DataTable setOfTickers,
                                                            DateTime firstQuoteDate,
                                                            DateTime lastQuoteDate,
                                                            long maxNumOfReturnedTickers)
    {
      int marketDaysForTheGivenMarket = 
        TickerDataTable.getNumberOfTradingDays(marketIndex, firstQuoteDate, lastQuoteDate);
      TickerDataTable.addColumnNumberOfQuotes(setOfTickers);
      DataTable returnValue = setOfTickers.Clone(); 
      foreach(DataRow row in setOfTickers.Rows)
      {
        if(QuantProject.DataAccess.Tables.Quotes.GetNumberOfDaysWithEffectiveTrades((string)row[0],firstQuoteDate,lastQuoteDate) == marketDaysForTheGivenMarket)
          //the current ticker has the same number of quotes as the market index
          TickerDataTable.getTickersQuotedInEachMarketDay_addRow(row, marketDaysForTheGivenMarket,
                                                                  returnValue);         
      }
      ExtendedDataTable.DeleteRows(returnValue, maxNumOfReturnedTickers);
      return returnValue;              
    }

  }
}
