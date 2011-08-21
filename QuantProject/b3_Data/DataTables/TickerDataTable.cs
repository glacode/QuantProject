/*
QuantProject - Quantitative Finance Library

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
using QuantProject.ADT.Histories;
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
          firstAvailableQuoteDate = quotesOfCurrentTicker.GetQuoteDateOrPreceding(firstQuoteDate);
          lastAvailableQuoteDate =  quotesOfCurrentTicker.GetQuoteDateOrPreceding(lastQuoteDate);
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

    #region GetTickersQuotedInEachMarketDay
    private static void addColumnNumberOfValues(DataTable tableToAnalyze)
    {
      if(!tableToAnalyze.Columns.Contains("NumberOfValues"))
        tableToAnalyze.Columns.Add("NumberOfValues", System.Type.GetType("System.Int32"));
    }
    
    private static void getTickersQuotedInEachMarketDay_addRow(DataRow rowToBeAdded, int numberOfTradingDays,
                               DataTable tableToWhichRowIsToBeAdded)
    {
      DataRow newRow = tableToWhichRowIsToBeAdded.NewRow();
      newRow[0]= rowToBeAdded[0];
      newRow["NumberOfValues"] = numberOfTradingDays;
      tableToWhichRowIsToBeAdded.Rows.Add(newRow);
    }    
    
    private static void getTickersQuotedInEachMarketDay_handleRow(
    	DataRow row , History marketDays ,
    	DateTime firstQuoteDate , DateTime lastQuoteDate ,
    	DataTable tableToWhichRowIsToBeAdded )
    {
    	History marketDaysForTicker = Quotes.GetMarketDays( (string)row[0],
    	                                           firstQuoteDate , lastQuoteDate);
    	if( marketDaysForTicker.ContainsAllTheDatesIn( marketDays ) )
    		//the current ticker has been effectively traded in each market day
    		TickerDataTable.getTickersQuotedInEachMarketDay_addRow(
    			row , marketDaysForTicker.Count , tableToWhichRowIsToBeAdded );
    }
    
    public static DataTable GetTickersQuotedInEachMarketDay(
    	History marketDays, DataTable setOfTickers,
    	DateTime firstQuoteDate, DateTime lastQuoteDate,
    	long maxNumOfReturnedTickers)
    {
    	TickerDataTable.addColumnNumberOfValues(setOfTickers);
    	DataTable returnValue = setOfTickers.Clone();
    	foreach(DataRow row in setOfTickers.Rows)
    		getTickersQuotedInEachMarketDay_handleRow(
    			row , marketDays ,
    			firstQuoteDate , lastQuoteDate , returnValue );
    	ExtendedDataTable.DeleteRows(returnValue, maxNumOfReturnedTickers);
    	return returnValue;
    }
    #endregion GetTickersQuotedInEachMarketDay
    
    #region GetTickersQuotedAtEachDateTime
    
    private static void getTickersQuotedAtAGivenPercentageOfDateTimes_addRow(DataRow rowToBeAdded, int numberOfTradingDateTimes,
                               DataTable tableToWhichRowIsToBeAdded)
    {
      DataRow newRow = tableToWhichRowIsToBeAdded.NewRow();
      newRow[0]= rowToBeAdded[0];
      newRow["NumberOfValues"] = numberOfTradingDateTimes;
      tableToWhichRowIsToBeAdded.Rows.Add(newRow);
    }
    
    private static History getTickersQuotedAtAGivenPercentageOfDateTimes_handleRow_getDateTimesTickerHistory( string ticker ,
			DateTime firstDate , DateTime lastDate )
		{
    	Quotes quotes = new Quotes( ticker , firstDate , lastDate );
			History marketDaysOrDateTimes = new History();
			foreach ( DataRow dataRow in quotes.Rows )
			{
				DateTime dateTime = (DateTime)dataRow[ Quotes.Date ];
				DateTime dateTimeOpenOrClose = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
				                                            ConstantsProvider.OpenTime.Hour,
				                                            ConstantsProvider.OpenTime.Minute,
				                                            ConstantsProvider.OpenTime.Second);
				marketDaysOrDateTimes.Add( dateTimeOpenOrClose , dateTimeOpenOrClose );
				dateTimeOpenOrClose = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
	                                          ConstantsProvider.CloseTime.Hour,
	                                          ConstantsProvider.CloseTime.Minute,
	                                          ConstantsProvider.CloseTime.Second);
				marketDaysOrDateTimes.Add( dateTimeOpenOrClose , dateTimeOpenOrClose );
			}
			return marketDaysOrDateTimes;
		}
    
    
    private static void getTickersQuotedAtAGivenPercentageOfDateTimes_handleRow(
    	DataRow row , History marketDateTimes , double percentageOfDateTimes ,
    	DateTime firstQuoteDate , DateTime lastQuoteDate , 
    	DataTable tableToWhichRowIsToBeAdded )
    {
    	History dateTimesForTicker = 
    		TickerDataTable.getTickersQuotedAtAGivenPercentageOfDateTimes_handleRow_getDateTimesTickerHistory(
    			(string)row[0], firstQuoteDate, lastQuoteDate );	
    	if( dateTimesForTicker.ContainsAtAGivenPercentageDateTimesIn( marketDateTimes , percentageOfDateTimes ) )
    		//the current ticker has been effectively traded at the given percentage of times
    		//for the given market date times
    		TickerDataTable.getTickersQuotedAtAGivenPercentageOfDateTimes_addRow(
    			row , dateTimesForTicker.Count , tableToWhichRowIsToBeAdded );
    }
    
    private static void getTickersQuotedAtAGivenPercentageOfDateTimes_handleRow(
    	DataRow row , History marketDateTimes , double percentageOfDateTimes ,
    	DateTime firstQuoteDate , DateTime lastQuoteDate , int intervalFrameInSeconds,
    	DataTable tableToWhichRowIsToBeAdded )
    {
    	History dateTimesForTicker = Bars.GetMarketDateTimes( (string)row[0],
    	                                      firstQuoteDate , lastQuoteDate, intervalFrameInSeconds );
    	if( dateTimesForTicker.ContainsAtAGivenPercentageDateTimesIn( marketDateTimes , percentageOfDateTimes ) )
    		//the current ticker has been effectively traded at the given percentage of times
    		//for the given market date times
    		TickerDataTable.getTickersQuotedAtAGivenPercentageOfDateTimes_addRow(
    			row , dateTimesForTicker.Count , tableToWhichRowIsToBeAdded );
    }
      
    public static DataTable GetTickersQuotedAtAGivenPercentageOfDateTimes(string marketIndex, double percentageOfDateTimes,
                                                                          DataTable setOfTickers,
    																																			DateTime firstDateTime, DateTime lastDateTime,
    																																			long maxNumOfReturnedTickers)
    {
    	if(percentageOfDateTimes <= 0 || percentageOfDateTimes > 100)
    		throw new Exception ("invalid percentage");
    	
    	History marketDateTimesForIndex =
    		Quotes.GetMarketDays(marketIndex, firstDateTime, lastDateTime);
    	TickerDataTable.addColumnNumberOfValues(setOfTickers);
    	DataTable returnValue = setOfTickers.Clone();
    	foreach(DataRow row in setOfTickers.Rows)
    		getTickersQuotedAtAGivenPercentageOfDateTimes_handleRow(
    			row , marketDateTimesForIndex , percentageOfDateTimes ,
    			firstDateTime , lastDateTime , returnValue );
    	ExtendedDataTable.DeleteRows(returnValue, maxNumOfReturnedTickers);
    	return returnValue;
    }
    public static DataTable GetTickersQuotedAtAGivenPercentageOfDateTimes(string marketIndex, double percentageOfDateTimes,
                                                                          DataTable setOfTickers,
    																																			DateTime firstBarDateTime, DateTime lastBarDateTime,
    																																			int intervalFrameInSeconds,
    																																			long maxNumOfReturnedTickers)
    {
    	History marketDateTimesForIndex = Bars.GetMarketDateTimes(marketIndex,
    	                                                  firstBarDateTime , lastBarDateTime, intervalFrameInSeconds);
    	return GetTickersQuotedAtAGivenPercentageOfDateTimes(
    		marketDateTimesForIndex , percentageOfDateTimes , setOfTickers ,
    		firstBarDateTime , lastBarDateTime , intervalFrameInSeconds, maxNumOfReturnedTickers );
    }
    
    public static DataTable GetTickersQuotedAtAGivenPercentageOfDateTimes(string marketIndex, double percentageOfDateTimes,
                                                                          string groupID,
    																																			DateTime firstBarDateTime, DateTime lastBarDateTime,
    																																			int intervalFrameInSeconds,
    																																			long maxNumOfReturnedTickers)
    {
    	DataTable groupOfTicker =
    		QuantProject.DataAccess.Tables.Tickers_tickerGroups.GetTickers(groupID);
    	return GetTickersQuotedAtAGivenPercentageOfDateTimes(
    		marketIndex , percentageOfDateTimes , groupOfTicker ,
    		firstBarDateTime , lastBarDateTime , intervalFrameInSeconds, maxNumOfReturnedTickers );
    }
        
    /// <summary>
    /// Returns a dataTable containing tickers that are quoted
    /// at a given percentage of time with respect to
    /// a history of marketDateTimes
    /// </summary>
    /// <param name="percentageOfDateTimes">percentage (0 to 100) of times of quotation
    /// with respect to a given history of marketDateTimes. For example:
    /// with 50, it will be returned a dataTable with all the tickers 
    /// that have at least 0,5*(number of marketDateTimes) quoted dateTimes in common
    /// with marketDateTimes</param>
    /// <returns></returns>
    public static DataTable GetTickersQuotedAtAGivenPercentageOfDateTimes(
    	History marketDateTimes, double percentageOfDateTimes, 
    	DataTable setOfTickers,
    	DateTime firstBarDateTime, DateTime lastBarDateTime,
    	int intervalFrameInSeconds,
    	long maxNumOfReturnedTickers)
    {
    	if(percentageOfDateTimes <= 0 || percentageOfDateTimes > 100)
    		throw new Exception ("invalid percentage");
    	
    	TickerDataTable.addColumnNumberOfValues(setOfTickers);
    	DataTable returnValue = setOfTickers.Clone();
    	foreach(DataRow row in setOfTickers.Rows)
    		getTickersQuotedAtAGivenPercentageOfDateTimes_handleRow(
    			row , marketDateTimes , percentageOfDateTimes ,
    			firstBarDateTime , lastBarDateTime , intervalFrameInSeconds , returnValue );
    	ExtendedDataTable.DeleteRows(returnValue, maxNumOfReturnedTickers);
    	return returnValue;
    }
     public static DataTable GetTickersQuotedAtAGivenPercentageOfDateTimes(History marketDateTimes, double percentageOfDateTimes,
                                                                          DataTable setOfTickers,
    																																			DateTime firstDateTime, DateTime lastDateTime,
    																																			long maxNumOfReturnedTickers)
    {
    	if(percentageOfDateTimes <= 0 || percentageOfDateTimes > 100)
    		throw new Exception ("invalid percentage");
    	
    	TickerDataTable.addColumnNumberOfValues(setOfTickers);
    	DataTable returnValue = setOfTickers.Clone();
    	foreach(DataRow row in setOfTickers.Rows)
    		getTickersQuotedAtAGivenPercentageOfDateTimes_handleRow(
    			row , marketDateTimes , percentageOfDateTimes ,
    			firstDateTime , lastDateTime , returnValue );
    	ExtendedDataTable.DeleteRows(returnValue, maxNumOfReturnedTickers);
    	return returnValue;
    }
    #endregion GetTickersQuotedAtAGivenPercentageOfDateTimes
    
    public static DataTable GetTickersQuotedInEachMarketDay(string marketIndex, DataTable setOfTickers,
                                                            DateTime firstQuoteDate,
                                                            DateTime lastQuoteDate,
                                                            long maxNumOfReturnedTickers)
    {
    	History marketDaysForIndex = Quotes.GetMarketDays(marketIndex,
    	                                                  firstQuoteDate , lastQuoteDate);
    	return GetTickersQuotedInEachMarketDay(
    		marketDaysForIndex , setOfTickers ,
    		firstQuoteDate , lastQuoteDate , maxNumOfReturnedTickers );
    }
    public static DataTable GetTickersQuotedInEachMarketDay(string marketIndex, string groupID,
                                                            DateTime firstQuoteDate,
                                                            DateTime lastQuoteDate,
                                                            long maxNumOfReturnedTickers)
    {
    	DataTable groupOfTicker =
    		QuantProject.DataAccess.Tables.Tickers_tickerGroups.GetTickers(groupID);
    	return GetTickersQuotedInEachMarketDay(
    		marketIndex , groupOfTicker ,
    		firstQuoteDate , lastQuoteDate , maxNumOfReturnedTickers );
    }

    
    private static void getTickersQuotedNotAtEachMarketDay_addRow(DataRow rowToBeAdded,
                                                                  int numberOfTradingDays,
                                                                  int numberOfMissingQuotes,
                               																		DataTable tableToWhichRowIsToBeAdded)
    {
      DataRow newRow = tableToWhichRowIsToBeAdded.NewRow();
      newRow[0]= rowToBeAdded[0];
      newRow["NumberOfTradingDays"] = numberOfTradingDays;
      newRow["NumberOfMissingQuotes"] = numberOfMissingQuotes;
      tableToWhichRowIsToBeAdded.Rows.Add(newRow);
    }
    
    private static void getTickersQuotedNotAtEachMarketDay_addColumns(DataTable tableToAnalyze)
    {
      if(!tableToAnalyze.Columns.Contains("NumberOfTradingDays"))
        tableToAnalyze.Columns.Add("NumberOfTradingDays", System.Type.GetType("System.Int32"));
      if(!tableToAnalyze.Columns.Contains("NumberOfMissingQuotes"))
        tableToAnalyze.Columns.Add("NumberOfMissingQuotes", System.Type.GetType("System.Int32"));
    }
    
    public static DataTable GetTickersNotQuotedAtEachMarketDay(string marketIndex, string groupID,
                                                            DateTime firstQuoteDate,
                                                            DateTime lastQuoteDate,
                                                            long maxNumOfReturnedTickers)
    {
      
      DataTable groupOfTickers = QuantProject.DataAccess.Tables.Tickers_tickerGroups.GetTickers(groupID);
      
      return GetTickersNotQuotedAtEachMarketDay(marketIndex,groupOfTickers,firstQuoteDate,lastQuoteDate,
                                                maxNumOfReturnedTickers);
    }
    
    private static int getNumberOfTradingDays(string marketIndex,
                                              DateTime firstQuoteDate,
                                              DateTime lastQuoteDate)
    {
    	QuantProject.Data.DataTables.Quotes marketQuotes =
    		new QuantProject.Data.DataTables.Quotes(marketIndex, firstQuoteDate, lastQuoteDate);
    	return marketQuotes.Rows.Count;
    }
    public static DataTable GetTickersNotQuotedAtEachMarketDay(string marketIndex, DataTable setOfTickers,
                                                               DateTime firstQuoteDate,
                                                               DateTime lastQuoteDate,
                                                            long maxNumOfReturnedTickers)
    {
			int marketDaysForTheGivenMarket = 
                    TickerDataTable.getNumberOfTradingDays(marketIndex, firstQuoteDate, lastQuoteDate);
      TickerDataTable.getTickersQuotedNotAtEachMarketDay_addColumns(setOfTickers);
      DataTable returnValue = setOfTickers.Clone();
      int numberOfMissingQuotes = 0;
      foreach(DataRow row in setOfTickers.Rows)
      {
      	DataTable tickerQuotes = new Quotes((string)row[0],firstQuoteDate,lastQuoteDate);
      	numberOfMissingQuotes = marketDaysForTheGivenMarket - tickerQuotes.Rows.Count;
      	if( numberOfMissingQuotes > 0 )
        //the current ticker has NOT been effectively traded at each market day
          TickerDataTable.getTickersQuotedNotAtEachMarketDay_addRow(row, marketDaysForTheGivenMarket,
        	                                                          numberOfMissingQuotes,
                                                                 		returnValue);
      }
      ExtendedDataTable.DeleteRows(returnValue, maxNumOfReturnedTickers);
      
      return returnValue;
   }

  }
}
