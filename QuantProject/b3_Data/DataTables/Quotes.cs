using System;
using System.Data;
using System.Text;
using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.DataAccess;
using QuantProject.DataAccess.Tables;

namespace QuantProject.Data.DataTables
{
	/// <summary>
	/// DataTable for quotes table data
	/// </summary>
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
    /// returns tickers ordered by volatility computed with Standard deviation of adjusted 
    /// close to open ratio, within the given set of tickers
    /// </summary>
    public static DataTable GetTickersByCloseToOpenVolatility( bool orderByASC,
      DataTable setOfTickers,
      DateTime firstQuoteDate,
      DateTime lastQuoteDate,
      long maxNumOfReturnedTickers)
    {
      if(!setOfTickers.Columns.Contains("CloseToOpenStandDev"))
        setOfTickers.Columns.Add("CloseToOpenStandDev", System.Type.GetType("System.Double"));
      foreach(DataRow row in setOfTickers.Rows)
      {
        row["CloseToOpenStandDev"] = 
          QuantProject.DataAccess.Tables.Quotes.GetCloseToOpenStandardDeviation((string)row[0],
          firstQuoteDate,
          lastQuoteDate);
      }
      DataTable getTickersByVolatility = ExtendedDataTable.CopyAndSort(setOfTickers,"CloseToOpenStandDev", orderByASC);
      ExtendedDataTable.DeleteRows(getTickersByVolatility, maxNumOfReturnedTickers);
      return getTickersByVolatility;
    }


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
    /// returns tickers by average close to open performance within the given set of tickers
    /// </summary>

    public static DataTable GetTickersByAverageCloseToOpenPerformance( bool orderByASC,
      DataTable setOfTickers,
      DateTime firstQuoteDate,
      DateTime lastQuoteDate,
      long maxNumOfReturnedTickers)
    {
      if(!setOfTickers.Columns.Contains("AverageCloseToOpenPerformance"))
        setOfTickers.Columns.Add("AverageCloseToOpenPerformance", System.Type.GetType("System.Double"));
      foreach(DataRow row in setOfTickers.Rows)
      {
        row["AverageCloseToOpenPerformance"] = 
          QuantProject.DataAccess.Tables.Quotes.GetAverageCloseToOpenPerformance((string)row[0],
          firstQuoteDate,
          lastQuoteDate);
      }
      DataTable tableToReturn = ExtendedDataTable.CopyAndSort(setOfTickers,"AverageCloseToOpenPerformance", orderByASC);
      ExtendedDataTable.DeleteRows(tableToReturn, maxNumOfReturnedTickers);
      return tableToReturn;
    }



    /// <summary>
    /// Returns a table containing the Pearson correlation coefficient of the adjusted close to close ratios
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
              ExtendedDataTable.GetArrayOfFloatFromColumn(dtFirstTicker, "quAdjustedCloseToCloseRatio"),
              ExtendedDataTable.GetArrayOfFloatFromColumn(dtSecondTicker, "quAdjustedCloseToCloseRatio"));
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
     
    private void setPrimaryKeys()
    {
      DataColumn[] columnPrimaryKeys = new DataColumn[1];
      columnPrimaryKeys[0] = this.Columns[Quotes.Date];
      this.PrimaryKey = columnPrimaryKeys;
    }
    

		private void fillDataTable( string ticker , DateTime startDate , DateTime endDate )
		{
			QuantProject.DataAccess.Tables.Quotes.SetDataTable( 
				ticker , startDate , endDate , this );
      this.setPrimaryKeys(); 
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
			return (DateTime) history.GetKey( Math.Max( 0 ,
				history.IndexOfKeyOrPrevious( quoteDate ) +
				followingDays ) );
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

