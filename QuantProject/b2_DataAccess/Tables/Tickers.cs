/*
QuantDownloader - Quantitative Finance Library

Tickers.cs
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
using QuantProject.DataAccess;


namespace QuantProject.DataAccess.Tables
{
	/// <summary>
	/// Class to access the Tickers table
	/// </summary>
	public class Tickers
	{

    // these static fields provide field name in the database table
    // They are intended to be used through intellisense when necessary
    public static string Ticker = "tiTicker";
    public static string CompanyName = "tiCompanyName";

    private DataTable tickers;
    private int count;
    
    public Tickers()
    {
      this.tickers = SqlExecutor.GetDataTable("SELECT * FROM tickers");
      this.count = this.tickers.Rows.Count;
    }

    /// <summary>
    /// Number of tickers in tickers table
    /// </summary>
    public int Count
    {
      get
      {
        return this.count;
      }
    }
    
    
    public static DataTable GetTableOfFilteredTickers(string tickerSymbolIsLike,
                                                      string tickerCompanyNameIsLike)
    {
      string sqlSelectString = Tickers.buildSqlSelectString(tickerSymbolIsLike, tickerCompanyNameIsLike);
      return SqlExecutor.GetDataTable(sqlSelectString);
    }
    
    public static DataTable GetTableOfFilteredTickers(string tickerSymbolIsLike,
                                                      string tickerCompanyNameIsLike,
                                                      string firstOperatorInHavingStatement,
                                                      DateTime firstQuoteDate,
                                                      string secondOperatorInHavingStatement,
                                                      DateTime lastQuoteDate)
    {
      string sqlSelectString = Tickers.buildSqlSelectString(tickerSymbolIsLike,
                                                            tickerCompanyNameIsLike,
                                                            firstOperatorInHavingStatement,
                                                            firstQuoteDate,
                                                            secondOperatorInHavingStatement,
                                                            lastQuoteDate);
      return SqlExecutor.GetDataTable(sqlSelectString);
    }

    #region buildSqlSelectString
    private static string buildSqlSelectString(string tickerSymbolIsLike,
                                               string tickerCompanyNameIsLike)
    {
      string sqlSelectString = "";
      sqlSelectString = "SELECT tiTicker, tiCompanyName " +
          "FROM tickers WHERE tiTicker LIKE '" +
          tickerSymbolIsLike + "' " +
          "AND tiCompanyName LIKE '" +
          tickerCompanyNameIsLike + "' " + 
          "ORDER BY tiTicker";
      return sqlSelectString;
    }

    private static string buildSqlSelectString(string tickerSymbolIsLike,
                                               string tickerCompanyNameIsLike,
                                               string firstOperatorInHavingStatement,
                                               DateTime firstQuoteDate,
                                               string secondOperatorInHavingStatement,
                                               DateTime lastQuoteDate)
    {
      string sqlSelectString = "";
      if(firstQuoteDate.CompareTo(lastQuoteDate)>0)
        throw new Exception("Last Date can't be previous of First date!");
      sqlSelectString = "SELECT tiTicker, tiCompanyName, Min(quotes.quDate) AS FirstQuote, Max(quotes.quDate) AS LastQuote " +
      "FROM quotes INNER JOIN tickers ON quotes.quTicker = tickers.tiTicker " +
      "WHERE tiTicker LIKE '" +
      tickerSymbolIsLike + "' " +
      "AND tiCompanyName LIKE '" +
      tickerCompanyNameIsLike + "' " + 
      "GROUP BY tickers.tiTicker, tickers.tiCompanyName " +
      "HAVING Min(quotes.quDate)" + firstOperatorInHavingStatement + 
      SQLBuilder.GetDateConstant(firstQuoteDate) +
      "AND Max(quotes.quDate)" + secondOperatorInHavingStatement + 
      SQLBuilder.GetDateConstant(lastQuoteDate);
      
      return sqlSelectString;
    }


    #endregion
  }
}
