using System;
using System.Collections;
using System.Data;
using System.Text;
using QuantProject.ADT;
using QuantProject.ADT.Statistics;
using QuantProject.ADT.Histories;
using QuantProject.DataAccess;
using QuantProject.DataAccess.Tables;
using System.Runtime.Serialization;

namespace QuantProject.Data.DataTables
{
	/// <summary>
	/// DataTable for quotes table data
	/// </summary>
	[Serializable]
	public class Quotes : DataTable
	{
		public static string TickerFieldName = "quTicker";	// Ticker cannot be simply used because
		// it is already used below
		public static string Date = "quDate";
		public static string Open = "quOpen";
		public static string High = "quHigh";
		public static string Low = "quLow";
		public static string Close = "quClose";
		public static string Volume = "quVolume";
		public static string AdjustedClose = "quAdjustedClose";
		public static string AdjustedCloseToCloseRatio = "quAdjustedCloseToCloseRatio";

		/// <summary>
		/// Builds a Quotes data table containing a row for each ticker in the
		/// collection, with the quotes for the given DateTime
		/// </summary>
		/// <param name="tickerCollection">Tickers whose quotes are to be fetched</param>
		/// <param name="dateTime">Date for the quotes to be fetched</param>
		public Quotes( ICollection tickerCollection , DateTime dateTime )
		{
			QuantProject.DataAccess.Tables.Quotes.SetDataTable( 
				tickerCollection , dateTime , this );
		}
		public Quotes( string ticker , DateTime startDate , DateTime endDate )
		{
			this.fillDataTable( ticker , startDate , endDate );
		}
		public Quotes( string ticker )
		{
			this.fillDataTable( 
				ticker ,
				QuantProject.DataAccess.Tables.Quotes.GetStartDate( ticker ) ,
				QuantProject.DataAccess.Tables.Quotes.GetEndDate( ticker ) );
		}
    public Quotes(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
		private void fillDataTable( string ticker , DateTime startDate , DateTime endDate )
		{
			QuantProject.DataAccess.Tables.Quotes.SetDataTable( 
				ticker , startDate , endDate , this );
			this.setPrimaryKeys(); 
		}
		private void setPrimaryKeys()
		{
			DataColumn[] columnPrimaryKeys = new DataColumn[1];
			columnPrimaryKeys[0] = this.Columns[Quotes.Date];
			this.PrimaryKey = columnPrimaryKeys;
		}

    /// <summary>
    /// returns most liquid tickers within the given set of tickers
    /// </summary>

    public static DataTable GetTickersByLiquidity( bool orderByASC,
                                                  DataTable setOfTickers,
                                                  DateTime firstQuoteDate,
                                                  DateTime lastQuoteDate,
                                                  long maxNumOfReturnedTickers)
    {
      if(!setOfTickers.Columns.Contains("AverageTradedValue"))
        setOfTickers.Columns.Add("AverageTradedValue", System.Type.GetType("System.Double"));
      foreach(DataRow row in setOfTickers.Rows)
      {
        row["AverageTradedValue"] = 
            QuantProject.DataAccess.Tables.Quotes.GetAverageTradedValue((string)row[0],
                                                                        firstQuoteDate,
                                                                        lastQuoteDate);
      }
      DataTable getMostLiquidTicker = ExtendedDataTable.CopyAndSort(setOfTickers,"AverageTradedValue", orderByASC);
      ExtendedDataTable.DeleteRows(getMostLiquidTicker, maxNumOfReturnedTickers);
      return getMostLiquidTicker;
    }

    /// <summary>
    /// returns tickers ordered by volatility computed with Standard deviation of adjusted 
    /// close to close ratio, within the given set of tickers
    /// </summary>
    public static DataTable GetTickersByCloseToCloseVolatility( bool orderByASC,
                                                              DataTable setOfTickers,
                                                              DateTime firstQuoteDate,
                                                              DateTime lastQuoteDate,
                                                              long maxNumOfReturnedTickers)
    {
      if(!setOfTickers.Columns.Contains("AdjCloseToCloseStandDev"))
        setOfTickers.Columns.Add("AdjCloseToCloseStandDev", System.Type.GetType("System.Double"));
      foreach(DataRow row in setOfTickers.Rows)
      {
        row["AdjCloseToCloseStandDev"] = 
          QuantProject.DataAccess.Tables.Quotes.GetAdjustedCloseToCloseStandardDeviation((string)row[0],
          firstQuoteDate,
          lastQuoteDate);
      }
      DataTable getTickersByVolatility = ExtendedDataTable.CopyAndSort(setOfTickers,"AdjCloseToCloseStandDev", orderByASC);
      ExtendedDataTable.DeleteRows(getTickersByVolatility, maxNumOfReturnedTickers);
      return getTickersByVolatility;
    }

    /// <summary>
    /// returns tickers ordered by volatility computed with Standard deviation of  
    /// open to close ratio, within the given set of tickers
    /// </summary>
    public static DataTable GetTickersByOpenToCloseVolatility( bool orderByASC,
      DataTable setOfTickers,
      DateTime firstQuoteDate,
      DateTime lastQuoteDate,
      long maxNumOfReturnedTickers)
    {
      if(!setOfTickers.Columns.Contains("OpenToCloseStandDev"))
        setOfTickers.Columns.Add("OpenToCloseStandDev", System.Type.GetType("System.Double"));
      foreach(DataRow row in setOfTickers.Rows)
      {
        try
        {
          row["OpenToCloseStandDev"] = -1000000.0;
          row["OpenToCloseStandDev"] = 
            QuantProject.DataAccess.Tables.Quotes.GetOpenToCloseStandardDeviation((string)row[0],
            firstQuoteDate,
            lastQuoteDate);
        }
        catch(Exception ex)
        {ex=ex;}
      }
      DataTable getTickersByVolatility = ExtendedDataTable.CopyAndSort(setOfTickers,
                                                    "OpenToCloseStandDev>-1000000.0",
                                                    "OpenToCloseStandDev",
                                                    orderByASC);
      ExtendedDataTable.DeleteRows(getTickersByVolatility, maxNumOfReturnedTickers);
      return getTickersByVolatility;
    }

//    /// <summary>
//    /// returns tickers ordered by volatility computed with Standard deviation of adjusted 
//    /// close to open ratio, within the given set of tickers
//    /// </summary>
//    public static DataTable GetTickersByCloseToOpenVolatility( bool orderByASC,
//      DataTable setOfTickers,
//      DateTime firstQuoteDate,
//      DateTime lastQuoteDate,
//      long maxNumOfReturnedTickers)
//    {
//      if(!setOfTickers.Columns.Contains("CloseToOpenStandDev"))
//        setOfTickers.Columns.Add("CloseToOpenStandDev", System.Type.GetType("System.Double"));
//      foreach(DataRow row in setOfTickers.Rows)
//      {
//        row["CloseToOpenStandDev"] = 
//          QuantProject.DataAccess.Tables.Quotes.GetCloseToOpenStandardDeviation((string)row[0],
//          firstQuoteDate,
//          lastQuoteDate);
//      }
//      DataTable getTickersByVolatility = ExtendedDataTable.CopyAndSort(setOfTickers,"CloseToOpenStandDev", orderByASC);
//      ExtendedDataTable.DeleteRows(getTickersByVolatility, maxNumOfReturnedTickers);
//      return getTickersByVolatility;
//    }


    /// <summary>
    /// returns tickers by average close to close performance within the given set of tickers
    /// </summary>
    public static DataTable GetTickersByAverageCloseToClosePerformance( bool orderByASC,
      DataTable setOfTickers,
      DateTime firstQuoteDate,
      DateTime lastQuoteDate,
      long maxNumOfReturnedTickers)
    {
      if(!setOfTickers.Columns.Contains("AverageCloseToClosePerformance"))
        setOfTickers.Columns.Add("AverageCloseToClosePerformance", System.Type.GetType("System.Double"));
      foreach(DataRow row in setOfTickers.Rows)
      {
        row["AverageCloseToClosePerformance"] = 
          QuantProject.DataAccess.Tables.Quotes.GetAverageCloseToClosePerformance((string)row[0],
          firstQuoteDate,
          lastQuoteDate);
      }
      DataTable tableToReturn = ExtendedDataTable.CopyAndSort(setOfTickers,"AverageCloseToClosePerformance", orderByASC);
      ExtendedDataTable.DeleteRows(tableToReturn, maxNumOfReturnedTickers);
      return tableToReturn;
    }

    /// <summary>
    /// returns tickers by average open to close performance
    /// within the given set of tickers
    /// </summary>

    public static DataTable GetTickersByAverageOpenToClosePerformance( bool orderByASC,
      DataTable setOfTickers,
      DateTime firstQuoteDate,
      DateTime lastQuoteDate,
      long maxNumOfReturnedTickers)
    {
      if(!setOfTickers.Columns.Contains("AverageOpenToClosePerformance"))
        setOfTickers.Columns.Add("AverageOpenToClosePerformance", System.Type.GetType("System.Double"));
      foreach(DataRow row in setOfTickers.Rows)
      {
        try
        {
          row["AverageOpenToClosePerformance"] = -1000000.0;
          row["AverageOpenToClosePerformance"] = 
            QuantProject.DataAccess.Tables.Quotes.GetAverageOpenToClosePerformance((string)row[0],
            firstQuoteDate,
            lastQuoteDate);
        }
        catch(Exception ex)
        {ex=ex;}
      }
      DataTable tableToReturn = ExtendedDataTable.CopyAndSort(setOfTickers,
                                                              "AverageOpenToClosePerformance>-1000000.0",
                                                              "AverageOpenToClosePerformance",
                                                              orderByASC);
      ExtendedDataTable.DeleteRows(tableToReturn, maxNumOfReturnedTickers);
      return tableToReturn;
    }
    
    private static void getTickersByAdjCloseToClosePearsonCorrelationCoefficient_setTickersReturns(out float[][]
      tickersReturns, DataTable setOfTickers, DateTime firstQuoteDate, DateTime lastQuoteDate) 
                                                              
    {
      tickersReturns = new float[setOfTickers.Rows.Count][];
      for(int i = 0; i<setOfTickers.Rows.Count; i++)
      {
        DataTable tickerQuoteTable = 
          QuantProject.DataAccess.Tables.Quotes.GetTickerQuotes((string)setOfTickers.Rows[i][0],
                                                                firstQuoteDate,
                                                                lastQuoteDate);
        tickersReturns[i] =
          ExtendedDataTable.GetArrayOfFloatFromColumn(tickerQuoteTable,"quAdjustedClose"); 

      }
    }

    /// <summary>
    /// Returns a table containing the Pearson correlation coefficient for the adjusted close values
    /// for any possible couple of tickers contained in the given table, for the specified interval
    /// </summary>
    public static DataTable GetTickersByAdjCloseToClosePearsonCorrelationCoefficient( bool orderByASC,
                                                                            DataTable setOfTickers,
                                                                            DateTime firstQuoteDate,
                                                                            DateTime lastQuoteDate)
                                                              
    {
      if(!setOfTickers.Columns.Contains("CorrelatedTicker"))
        setOfTickers.Columns.Add("CorrelatedTicker", System.Type.GetType("System.String"));
      if(!setOfTickers.Columns.Contains("PearsonCorrelationCoefficient"))
        setOfTickers.Columns.Add("PearsonCorrelationCoefficient", System.Type.GetType("System.Double"));
      int initialNumberOfRows = setOfTickers.Rows.Count;
      float[][] tickersReturns;
      getTickersByAdjCloseToClosePearsonCorrelationCoefficient_setTickersReturns(out tickersReturns, setOfTickers, firstQuoteDate, lastQuoteDate);
      for(int j=0; j!= initialNumberOfRows; j++)
      {
        string firstTicker = (string)setOfTickers.Rows[j][0];
        for(int i = j+1; i!= initialNumberOfRows; i++)
        {
          string secondTicker = (string)setOfTickers.Rows[i][0];
          DataRow rowToAdd = setOfTickers.NewRow();
          rowToAdd[0] = firstTicker;
          rowToAdd["PearsonCorrelationCoefficient"] = -2.0;
          //unassigned value for this column
          rowToAdd["CorrelatedTicker"] = secondTicker;
          try
          {
            rowToAdd["PearsonCorrelationCoefficient"] = 
              QuantProject.ADT.Statistics.BasicFunctions.PearsonCorrelationCoefficient(
              tickersReturns[j],tickersReturns[i]);
          }
          catch(Exception ex)
          {
            ex = ex;
          }
          finally
          {
            setOfTickers.Rows.Add(rowToAdd);
          }
        }
      }
      ExtendedDataTable.DeleteRows(setOfTickers, 0, initialNumberOfRows - 1);
      //delete initial rows that don't contain correlated ticker and Pearson coeff.
      return ExtendedDataTable.CopyAndSort(setOfTickers,"PearsonCorrelationCoefficient>-2.0",
                                          "PearsonCorrelationCoefficient", orderByASC);
    }

    /// <summary>
    /// Returns a table containing the Pearson correlation coefficient for the adjusted close values
    /// for any possible couple of tickers contained in the given group of tickers,
    /// for the specified interval
    /// </summary>
    public static DataTable GetTickersByAdjCloseToClosePearsonCorrelationCoefficient( bool orderByASC,
                                                      string groupID,
                                                      DateTime firstQuoteDate,
                                                      DateTime lastQuoteDate)
                                                              
    {
      
      DataTable tickersOfGroup = new Tickers_tickerGroups(groupID);
      return GetTickersByAdjCloseToClosePearsonCorrelationCoefficient(orderByASC,
                                                                tickersOfGroup,
                                                                firstQuoteDate,
                                                                lastQuoteDate);
    }

    /// <summary>
    /// Returns a table containing the Pearson correlation coefficient of the close to open ratios
    /// for any possible couple of tickers contained in the given table, for the specified interval
    /// </summary>
    public static DataTable GetTickersByCloseToOpenPearsonCorrelationCoefficient( bool orderByASC,
                                                              DataTable setOfTickers,
                                                              DateTime firstQuoteDate,
                                                              DateTime lastQuoteDate)
                                                              
    {
      if(!setOfTickers.Columns.Contains("CorrelatedTicker"))
        setOfTickers.Columns.Add("CorrelatedTicker", System.Type.GetType("System.String"));
      if(!setOfTickers.Columns.Contains("PearsonCorrelationCoefficient"))
        setOfTickers.Columns.Add("PearsonCorrelationCoefficient", System.Type.GetType("System.Double"));
      int initialNumberOfRows = setOfTickers.Rows.Count;
      for(int j=0; j!= initialNumberOfRows; j++)
      {
        string firstTicker = (string)setOfTickers.Rows[j][0];
        for(int i = j+1; i!= initialNumberOfRows; i++)
        {
          string secondTicker = (string)setOfTickers.Rows[i][0];
          DataTable dtFirstTicker = QuantProject.DataAccess.Tables.Quotes.GetTickerQuotes(firstTicker,
            firstQuoteDate,
            lastQuoteDate);
          DataTable dtSecondTicker = QuantProject.DataAccess.Tables.Quotes.GetTickerQuotes(secondTicker,
            firstQuoteDate,
            lastQuoteDate);
          DataRow rowToAdd = setOfTickers.NewRow();
          rowToAdd[0] = firstTicker;
          rowToAdd["CorrelatedTicker"] = secondTicker;
          try
          {
            rowToAdd["PearsonCorrelationCoefficient"] = 
              QuantProject.ADT.Statistics.BasicFunctions.PearsonCorrelationCoefficient(
              ExtendedDataTable.GetArrayOfFloatFromRatioOfColumns(dtFirstTicker, "quClose", "quOpen"),
              ExtendedDataTable.GetArrayOfFloatFromRatioOfColumns(dtSecondTicker, "quClose", "quOpen"));
            setOfTickers.Rows.Add(rowToAdd);
          }
          catch(Exception ex)
          {
            string notUsed = ex.ToString();
          }
        }
      }
      ExtendedDataTable.DeleteRows(setOfTickers, 0, initialNumberOfRows - 1);
      return ExtendedDataTable.CopyAndSort(setOfTickers,"PearsonCorrelationCoefficient", orderByASC);
    }

    /// <summary>
    /// returns tickers with the average raw open price
    /// belonging to the specified range within the given set of tickers
    /// </summary>

    public static DataTable GetTickersByAverageRawOpenPrice( bool orderByASC,
                                                  DataTable setOfTickers,
                                                  DateTime firstQuoteDate,
                                                  DateTime lastQuoteDate,
                                                  long maxNumOfReturnedTickers,
                                                  double minPrice, double maxPrice,
                                                  double minStdDeviation,
                                                  double maxStdDeviation)
    {
      if(!setOfTickers.Columns.Contains("AverageRawOpenPrice"))
        setOfTickers.Columns.Add("AverageRawOpenPrice", System.Type.GetType("System.Double"));
      if(!setOfTickers.Columns.Contains("RawOpenPriceStdDev"))
      	setOfTickers.Columns.Add("RawOpenPriceStdDev",System.Type.GetType("System.Double"));
      foreach(DataRow row in setOfTickers.Rows)
      {
        row["AverageRawOpenPrice"] = 
            QuantProject.DataAccess.Tables.Quotes.GetAverageRawOpenPrice((string)row[0],
                                                                        firstQuoteDate,
                                                                        lastQuoteDate);
        row["RawOpenPriceStdDev"] = 
            QuantProject.DataAccess.Tables.Quotes.GetRawOpenPriceStdDeviation((string)row[0],
                                                                        firstQuoteDate,
                                                                        lastQuoteDate);
      }
      getTickersByAverageRawOpenPrice_deleteRows(setOfTickers, minPrice, maxPrice,
                                                 minStdDeviation, maxStdDeviation);
      DataTable returnValue = ExtendedDataTable.CopyAndSort(setOfTickers,"AverageRawOpenPrice", orderByASC);
      ExtendedDataTable.DeleteRows(returnValue, maxNumOfReturnedTickers);
      return returnValue;
    }

	private static void getTickersByAverageRawOpenPrice_deleteRows( DataTable setOfTickers,
                                                                   double minPrice, double maxPrice,
                                                                  double minStdDeviation,
                                                                  double maxStdDeviation)
    {
      int currentNumRows = setOfTickers.Rows.Count;
      for(int i = 0;i<currentNumRows;i++)
      {
        if(setOfTickers.Rows[i].RowState != DataRowState.Deleted)
        {
          double averagePrice = (double)setOfTickers.Rows[i]["AverageRawOpenPrice"];
          double stdDeviation = (double)setOfTickers.Rows[i]["RawOpenPriceStdDev"];
          if (averagePrice < minPrice || averagePrice > maxPrice ||
            stdDeviation < minStdDeviation || stdDeviation > maxStdDeviation)
            //values of rows DON'T respect given criteria
          {
            setOfTickers.Rows[i].Delete();
            currentNumRows = setOfTickers.Rows.Count;
            i--;//deletion causes the new ID row
            //of the next row to be the ID row of the deleted Row
            //so, only in this way, all the rows are checked
          }
        }
      }
    }
		
    /// <summary>
    /// returns tickers counting how many times raw close is greater than raw open 
    /// for the given interval of days (within the given table of tickers).
    /// Tickers are ordered by the number of days raw open is greater than raw close
    /// </summary>
    public static DataTable GetTickersByOpenToCloseWinningDays( bool orderByASC,
                                                  DataTable setOfTickers,
                                                  DateTime firstQuoteDate,
                                                  DateTime lastQuoteDate,
                                                  long maxNumOfReturnedTickers,
                                                  bool onlyTickersWithAtLeastOneWinningDay)
    {
      if(!setOfTickers.Columns.Contains("NumOpenCloseWinningDays"))
        setOfTickers.Columns.Add("NumOpenCloseWinningDays", System.Type.GetType("System.Double"));
      foreach(DataRow row in setOfTickers.Rows)
      {
        row["NumOpenCloseWinningDays"] = 
            QuantProject.DataAccess.Tables.Quotes.GetNumberOfOpenToCloseWinningDays((string)row[0],
        	                                                                        firstQuoteDate, lastQuoteDate);
      }
      string filter_onlyTickersWithAtLeastOneWinningDay = "";
      if(onlyTickersWithAtLeastOneWinningDay)
        filter_onlyTickersWithAtLeastOneWinningDay = "NumOpenCloseWinningDays>0";
      DataTable returnValue = 
          ExtendedDataTable.CopyAndSort(setOfTickers,
                                        filter_onlyTickersWithAtLeastOneWinningDay,
                                        "NumOpenCloseWinningDays", orderByASC);
      ExtendedDataTable.DeleteRows(returnValue, maxNumOfReturnedTickers);
      return returnValue;
    }
    /// <summary>
    /// returns tickers counting how many times raw close is greater than raw open 
    /// for the given interval of days (within the given table of tickers).
    /// Tickers are ordered by the number of days raw open is greater than raw close
    /// </summary>
    public static DataTable GetTickersByOpenToCloseWinningDays( bool orderByASC,
                                                                string groupID,
                                                                DateTime firstQuoteDate,
                                                                DateTime lastQuoteDate,
                                                                long maxNumOfReturnedTickers,
                                                                bool onlyTickersWithAtLeastOneWinningDay)
    {
      return 
        GetTickersByOpenToCloseWinningDays(orderByASC,
                                           QuantProject.DataAccess.Tables.Tickers_tickerGroups.GetTickers(groupID),
                                           firstQuoteDate,
                                            lastQuoteDate,
                                           maxNumOfReturnedTickers,
                                            onlyTickersWithAtLeastOneWinningDay);
    }
    
    private static float[] getArrayOfCloseToOpenRatios(string ticker,
                                                      DateTime firstQuoteDate,
                                                      DateTime lastQuoteDate)
    {
      float[] returnValue;
      Quotes tickerQuotes = new Quotes(ticker, firstQuoteDate, lastQuoteDate);
      returnValue = ExtendedDataTable.GetArrayOfFloatFromRatioOfColumns(tickerQuotes, "quClose", "quOpen");
      return returnValue;

    }

    /// <summary>
    /// returns tickers of a given group ordered by open - close 
    /// correlation to a given benchmark
    /// </summary>
    public static DataTable GetTickersByOpenCloseCorrelationToBenchmark( bool orderByASC,
      string groupID, string benchmark,
      DateTime firstQuoteDate,
      DateTime lastQuoteDate,
      long maxNumOfReturnedTickers)
    {
      DataTable tickersOfGroup = new Tickers_tickerGroups(groupID);
      return GetTickersByOpenCloseCorrelationToBenchmark(orderByASC, tickersOfGroup, benchmark,
                                                  firstQuoteDate, lastQuoteDate,
                                                  maxNumOfReturnedTickers);
    }
    
    /// <summary>
    /// returns tickers of a given set of tickers ordered by open - close 
    /// correlation to a given benchmark
    /// </summary>
    public static DataTable GetTickersByOpenCloseCorrelationToBenchmark( bool orderByASC,
      DataTable setOfTickers, string benchmark,
      DateTime firstQuoteDate,
      DateTime lastQuoteDate,
      long maxNumOfReturnedTickers)
    {
      if(!setOfTickers.Columns.Contains("OpenCloseCorrelationToBenchmark"))
        setOfTickers.Columns.Add("OpenCloseCorrelationToBenchmark", System.Type.GetType("System.Double"));
      float[] benchmarkRatios = getArrayOfCloseToOpenRatios(benchmark, firstQuoteDate, lastQuoteDate);
      foreach(DataRow row in setOfTickers.Rows)
      {
        float[] tickerRatios = getArrayOfCloseToOpenRatios((string)row[0], 
          firstQuoteDate, lastQuoteDate);
      	if(tickerRatios.Length == benchmarkRatios.Length)
      		row["OpenCloseCorrelationToBenchmark"] =
          	Math.Abs(BasicFunctions.PearsonCorrelationCoefficient(benchmarkRatios, tickerRatios));
      }
      DataTable tableToReturn = ExtendedDataTable.CopyAndSort(setOfTickers,
                                                              "OpenCloseCorrelationToBenchmark>=0.0",
                                                              "OpenCloseCorrelationToBenchmark",
                                                              orderByASC);
      ExtendedDataTable.DeleteRows(tableToReturn, maxNumOfReturnedTickers);
      return tableToReturn;
    }
    
    private static float[] getArrayOfCloseToCloseRatios_getAdjustedValues(Quotes sourceQuotes,
                                                                          int numDaysForHalfPeriod,
                                                                          ref DateTime firstQuoteDate)
    {
      float[] returnValue = ExtendedDataTable.GetArrayOfFloatFromColumn(sourceQuotes, "quAdjustedClose");
      //in order to be alligned at the following market day,
      //allAdjValues has to be long n, where n is such that
      //n%2 * hp + 1 = 2 * hp (hp = half period)
      //if some quotes are deleted, first quote day has to be updated
      while(returnValue.Length % (2*numDaysForHalfPeriod) + 1 != 2*numDaysForHalfPeriod)
      {
        float[] newReturnValue = new float[returnValue.Length - 1];
        for(int k = 0;k<returnValue.Length - 1;k++)
            newReturnValue[k] = returnValue[k + 1];
        returnValue = newReturnValue;
        firstQuoteDate = firstQuoteDate.AddDays(1);
        firstQuoteDate = sourceQuotes.GetQuoteDateOrFollowing(firstQuoteDate);
      }
      return returnValue;
    }
    
    /// <summary>
    /// Gets an array containing close to close ratios
    /// </summary>
    /// <param name="ticker"></param>
    /// <param name="firstQuoteDate"></param>
    /// <param name="lastQuoteDate"></param>
    /// <param name="numDaysBetweenEachClose">Num of days the close to close ratio refers to</param>
    /// <param name="numOfInitialMarketDaysToJump">Num of initial market days that has not to be
    ///                                       considered in the calculation</param>
    /// <returns></returns>
    public static float[] GetArrayOfCloseToCloseRatios(string ticker,
                                                      ref DateTime firstQuoteDate,
                                                      DateTime lastQuoteDate,
                                                     	int numDaysBetweenEachClose,
                                                      int numOfInitialMarketDaysToJump)
    {
      float[] returnValue = null;
      Quotes tickerQuotes = new Quotes(ticker, firstQuoteDate, lastQuoteDate);
      //float[] allAdjValues = ExtendedDataTable.GetArrayOfFloatFromColumn(tickerQuotes, "quAdjustedClose");
      float[] allAdjValues = getArrayOfCloseToCloseRatios_getAdjustedValues(tickerQuotes, numDaysBetweenEachClose, ref firstQuoteDate);
      float[] adjValuesMinusInitialMarketDays = new float[allAdjValues.Length - numOfInitialMarketDaysToJump]; 
      //fill adjValuesMinusInitialMarketDays array
      for(int k = 0;k<allAdjValues.Length - numOfInitialMarketDaysToJump;k++)
            adjValuesMinusInitialMarketDays[k] = 
              allAdjValues[k + numOfInitialMarketDaysToJump]; 
      //
      returnValue = new float[adjValuesMinusInitialMarketDays.Length/(numDaysBetweenEachClose * 2)];
      int i = 0; //index for ratesOfReturns array
      int lastIdxAccessed = 0;
      for(int idx = 0;
          (idx + numDaysBetweenEachClose) < adjValuesMinusInitialMarketDays.Length && i<returnValue.Length;
          idx += numDaysBetweenEachClose )
      {
        if(idx-lastIdxAccessed>numDaysBetweenEachClose || idx == 0)
        //the current ratio is computed only if the first close, pointed by idx, is 
        //not the second close of the previous ratio
        {
          returnValue[i] = (adjValuesMinusInitialMarketDays[idx+numDaysBetweenEachClose]/
            adjValuesMinusInitialMarketDays[idx] - 1);
          lastIdxAccessed = idx;
          i++;
        }
      }	
      return returnValue;
    }
    
    public static float[] GetArrayOfCloseToCloseRatios(string ticker,
                                                        ref DateTime firstQuoteDate,
                                                        DateTime lastQuoteDate,
                                                        int numDaysBetweenEachClose)
    {
      return GetArrayOfCloseToCloseRatios(ticker, ref firstQuoteDate, lastQuoteDate, numDaysBetweenEachClose,
                                          0);
    }
    /// <summary>
    /// returns tickers of a given set of tickers ordered by close to close 
    /// correlation to a given benchmark
    /// </summary>
    public static DataTable GetTickersByCloseToCloseCorrelationToBenchmark( bool orderByASC,
      DataTable setOfTickers, string benchmark,
      DateTime firstQuoteDate,
      DateTime lastQuoteDate,
      long maxNumOfReturnedTickers, int numDaysBetweenEachClose)
    {
      if(!setOfTickers.Columns.Contains("CloseToCloseCorrelationToBenchmark"))
        setOfTickers.Columns.Add("CloseToCloseCorrelationToBenchmark", System.Type.GetType("System.Double"));
      float[] benchmarkRatios = GetArrayOfCloseToCloseRatios(benchmark, ref firstQuoteDate, lastQuoteDate, numDaysBetweenEachClose);
      foreach(DataRow row in setOfTickers.Rows)
      {
        float[] tickerRatios = GetArrayOfCloseToCloseRatios((string)row[0], 
          ref firstQuoteDate, lastQuoteDate, numDaysBetweenEachClose );
      	if(tickerRatios.Length == benchmarkRatios.Length)
      		row["CloseToCloseCorrelationToBenchmark"] =
          	Math.Abs(BasicFunctions.PearsonCorrelationCoefficient(benchmarkRatios, tickerRatios));
      }
      DataTable tableToReturn = ExtendedDataTable.CopyAndSort(setOfTickers,
                                                              "CloseToCloseCorrelationToBenchmark>=0.0",
                                                              "CloseToCloseCorrelationToBenchmark",
                                                              orderByASC);
      ExtendedDataTable.DeleteRows(tableToReturn, maxNumOfReturnedTickers);
      return tableToReturn;
    }
    
    /// <summary>
    /// returns tickers of a given set of tickers ordered by close to close 
    /// correlation to a given benchmark
    /// </summary>
    public static DataTable GetTickersByCloseToCloseCorrelationToBenchmark( bool orderByASC,
																					      string groupID, string benchmark,
																					      DateTime firstQuoteDate,
																					      DateTime lastQuoteDate,
																					      long maxNumOfReturnedTickers,
																					      int numDaysBetweenEachClose)
    {
      DataTable tickersOfGroup = new Tickers_tickerGroups(groupID);
      return GetTickersByCloseToCloseCorrelationToBenchmark(orderByASC,
																					      tickersOfGroup, benchmark,
																					      firstQuoteDate,
																					      lastQuoteDate,
																					      maxNumOfReturnedTickers,
																					      numDaysBetweenEachClose);
    }
    
    
    private History history;

		/// <summary>
		/// Gets the ticker whose quotes are contained into the Quotes object
		/// </summary>
		/// <returns></returns>
		public string Ticker
		{
			get{ return ((string)this.Rows[ 0 ][ Quotes.TickerFieldName ]); }
		}

    /// <summary>
    /// Gets the date of the first quote contained into the Quotes object
    /// </summary>
    /// <returns></returns>
    public DateTime StartDate
    {
      get{ return ((DateTime)this.Rows[ 0 ][ Quotes.Date ]); }
    }
    /// <summary>
    /// Gets the date of the last quote contained into the Quotes object
    /// </summary>
    /// <returns></returns>
    public DateTime EndDate
    {
      get{ return ((DateTime)this.Rows[ this.Rows.Count - 1 ][ Quotes.Date ]); }
    }
     
		#region GetHashValue
		private string getHashValue_getQuoteString_getRowString_getSingleValueString( Object value )
		{
			string returnValue;
			if ( value.GetType() == Type.GetType( "System.DateTime" ) )
				returnValue = ( (DateTime) value ).ToString();
			else
			{
				if ( value.GetType() == Type.GetType( "System.Double" ) )
					returnValue = ( (float) value ).ToString( "F2" );
				else
					returnValue = value.ToString();
			}

			return returnValue + ";";
		}
		/// <summary>
		/// Computes the string representing the concatenation for a single quote row
		/// </summary>
		/// <param name="dataRow"></param>
		/// <returns></returns>
		private StringBuilder getHashValue_getQuoteString_getRowString( DataRowView dataRow )
		{
			StringBuilder returnValue = new StringBuilder( "" );
			foreach ( DataColumn dataColumn in dataRow.DataView.Table.Columns )
				if ( dataColumn.ColumnName != "quTicker" )
					returnValue.Append( getHashValue_getQuoteString_getRowString_getSingleValueString(
						dataRow[ dataColumn.Ordinal ] ) );
			//					returnValue += "ggg";
			//					returnValue += getHashValue_getQuoteString_getRowString_getSingleValueString(
			//						dataRow[ dataColumn ] );
			return returnValue;
		}
		/// <summary>
		/// Computes the string representing the concatenation of all the quotes
		/// </summary>
		/// <param name="ticker"></param>
		/// <returns></returns>
		private string getHashValue_getQuoteString( DataView quotes )
		{
			StringBuilder returnValue = new StringBuilder( "" );
			foreach ( DataRowView dataRow in quotes )
				returnValue.Append( getHashValue_getQuoteString_getRowString( dataRow ) );
			return returnValue.ToString();
		}
		/// <summary>
		/// Computes the hash value for the contained quotes
		/// </summary>
		/// <returns>Hash value for all the quotes</returns>
		public string GetHashValue()
		{
			DataView quotes = new DataView( this );
			return HashProvider.GetHashValue( getHashValue_getQuoteString( quotes ) );
		}
		/// <summary>
		/// Computes the hash value for the contained quotes
		/// since startDate, to endDate
		/// </summary>
		/// <param name="startDate">date where hash begins being computed</param>
		/// <param name="endDate">date where hash ends being computed</param>
		/// <returns></returns>
		public string GetHashValue( DateTime startDate , DateTime endDate )
		{
			DataView quotes = new DataView( this );
			quotes.RowFilter = "( (quDate>=" + FilterBuilder.GetDateConstant( startDate ) +
				") and (quDate<=" + FilterBuilder.GetDateConstant( endDate ) + ") )";
			return HashProvider.GetHashValue( getHashValue_getQuoteString( quotes ) );
		}
		#endregion

		private void setHistory()
		{
			if ( this.history == null )
				// history has not been set, yet
			{
				this.history = new History();
				this.history.Import( this , "quDate" , "quAdjustedClose" );
			}
		}
		/// <summary>
		/// returns the Date for the quote that is precedingDays before
		/// quoteDate
		/// </summary>
		/// <param name="quoteDate"></param>
		/// <param name="precedingDays"></param>
		/// <returns></returns>
		public DateTime GetPrecedingDate( DateTime quoteDate , int precedingDays )
		{
			setHistory();
			return (DateTime) history.GetKey( Math.Max( 0 ,
				history.IndexOfKeyOrPrevious( quoteDate ) -
				precedingDays ) );
		}


		/// <summary>
		/// returns the Date for the quote that is followingDays after
		/// quoteDate
		/// </summary>
		/// <param name="quoteDate"></param>
		/// <param name="precedingDays"></param>
		/// <returns></returns>
		public DateTime GetFollowingDate( DateTime quoteDate , int followingDays )
		{
			setHistory();
			int indexOfKeyOrPrevious =
				history.IndexOfKeyOrPrevious( quoteDate );
			DateTime followingDate;
			try
			{
				followingDate =	(DateTime) history.GetKey( Math.Max( 0 ,
					indexOfKeyOrPrevious + followingDays ) );
			}
			catch ( ArgumentOutOfRangeException exception )
			{
				string message = exception.Message;
				throw new Exception( "Quotes.GetFollowingDate() error: there is not " +
					"a date for quoteDate=" + quoteDate.ToString() +
					" and followingDays=" + followingDays );
			}
			return followingDate;
		}
    
    /// <summary>
    /// Returns true if a quote is available at the given date
    /// </summary>
    /// <param name="date">date</param>
    /// <returns></returns>
    public bool HasDate( DateTime date )
    {
      /*alternative code, but primary keys need to be set first
      bool hasDate;
      hasDate = this.Rows.Contains(date.Date);
      return hasDate;*/
      setHistory();
      return this.history.ContainsKey(date.Date);
    }
    /// <summary>
    /// If the ticker has a quote at the given date, then it returns the given date,
    /// else it returns the immediate following date at which a quote is available
    /// </summary>
    /// <param name="date">date</param>
    /// <returns></returns>
    public DateTime GetQuoteDateOrFollowing(DateTime date )
    {
      if(this.HasDate(date))
      {
        return date;
      }
      else
      {
        return GetQuoteDateOrFollowing(date.AddDays(1));
      }
    }
    /// <summary>
    /// If the ticker has a quote at the given date, then it returns the given date,
    /// else it returns the immediate preceding date at which a quote is available
    /// </summary>
    /// <param name="date">date</param>
    /// <returns></returns>
    public DateTime GetQuoteDateOrPreceding( DateTime date )
    {
      if(this.HasDate(date))
      {
        return date;
      }
      else
      {
        return GetQuoteDateOrPreceding(date.AddDays(-1));
      }
    }

    /// <summary>
    /// If the ticker has a quote at the given date, then it returns the given date,
    /// else it returns the first valid following date at which a quote is available
    /// (or the first valid preceding date, in case date is >= the last available quote) 
    /// </summary>
    /// <param name="date">date</param>
    /// <returns></returns>
    public DateTime GetFirstValidQuoteDate(DateTime date)
    {
      DateTime startDate =  this.StartDate;
      DateTime endDate = this.EndDate;
      if(date<startDate || (date>=startDate && date<=endDate))
      {
        return this.GetQuoteDateOrFollowing(date);
      }
      else
      {
        return this.GetQuoteDateOrPreceding(date);
      }
    }

    /// <summary>
    /// Gets the adjusted close at the given date
    /// </summary>
    /// <returns></returns>
    public float GetAdjustedClose(DateTime date )
    {
      object[] keys = new object[1];
      keys[0] = date.Date;
      DataRow foundRow = this.Rows.Find(keys);
      if(foundRow==null)
        throw new Exception("No quote for such a date!");
      return (float)foundRow[Quotes.AdjustedClose]; 
    }

    /// <summary>
    /// Gets the first valid raw (not adjusted) close at the given date
    /// </summary>
    /// <returns></returns>
    public float GetFirstValidRawClose(DateTime date )
    {
      object[] keys = new object[1];
      keys[0] = this.GetFirstValidQuoteDate(date.Date);
      DataRow foundRow = this.Rows.Find(keys);
      if(foundRow==null)
        throw new Exception("No quote for such a date!");
      return (float)foundRow[Quotes.Close]; 
    }
    /// <summary>
    /// Gets the first valid adjusted close at the given date
    /// </summary>
    /// <returns></returns>
    public float GetFirstValidAdjustedClose(DateTime date )
    {
      object[] keys = new object[1];
      keys[0] = this.GetFirstValidQuoteDate(date.Date);
      DataRow foundRow = this.Rows.Find(keys);
      if(foundRow==null)
        throw new Exception("No quote for such a date!");
      return (float)foundRow[Quotes.AdjustedClose]; 
    }
    /// <summary>
    /// Gets the first valid raw (not adjusted) open at the given date
    /// </summary>
    /// <returns></returns>
    public float GetFirstValidRawOpen(DateTime date )
    {
      object[] keys = new object[1];
      keys[0] = this.GetFirstValidQuoteDate(date.Date);
      DataRow foundRow = this.Rows.Find(keys);
      if(foundRow==null)
        throw new Exception("No quote for such a date!");
      return (float)foundRow[Quotes.Open]; 
    }

    /// <summary>
    /// Gets the first valid adjusted open at the given date
    /// </summary>
    /// <returns></returns>
    public float GetFirstValidAdjustedOpen(DateTime date )
    {
      object[] keys = new object[1];
      keys[0] = this.GetFirstValidQuoteDate(date.Date);
      DataRow foundRow = this.Rows.Find(keys);
      if(foundRow==null)
        throw new Exception("No quote for such a date!");

      return (float)foundRow[Quotes.Open]*((float)foundRow[Quotes.AdjustedClose]/
                                           (float)foundRow[Quotes.Close]); 
    }

//		public DateTime GetPrecedingDate( DateTime quoteDate , int precedingDays )
//		{
//			History history = new History();
//			history.Import( this.quotes , "quDate" , "quAdjustedClose" );
//			return (DateTime) history.GetKey( Math.Max( 0 ,
//				history.IndexOfKeyOrPrevious( quoteDate ) -
//				precedingDays ) );
//		}
	}
}

