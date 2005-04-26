/*
QuantDownloader - Quantitative Finance Library

GroupQuotes.cs
Copyright (C) 2004 
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
using System.Text;
using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.DataAccess;
using QuantProject.DataAccess.Tables;

namespace QuantProject.Data.DataTables
{
	/// <summary>
	/// DataTable for quotes of the tickers inside a given group
	/// </summary>
	public class GroupQuotes : DataTable
	{
    private string groupID;
    
    //Static method for computing close to close ratios
    //of all tickers belonging to a given group
    static public void ComputeCloseToCloseRatios(string groupID)
    {
    	DataTable tickers = 
    		QuantProject.DataAccess.Tables.Tickers_tickerGroups.GetTickers(groupID);
    	foreach(DataRow row in tickers.Rows)
    	{
    		QuantProject.DataAccess.Tables.Quotes.ComputeAndCommitCloseToCloseRatios((string)row[0]);
    	}
    }
    
    public GroupQuotes( string groupID, DateTime startDate, DateTime endDate)
    {
      this.groupID = groupID;
      this.fillDataTable( 
        groupID ,startDate, endDate);
				
    }
  
    private void setPrimaryKeys()
    {
      DataColumn[] columnPrimaryKeys = new DataColumn[2];
      columnPrimaryKeys[0] = this.Columns[Quotes.TickerFieldName];
      columnPrimaryKeys[1] = this.Columns[Quotes.Date];
      this.PrimaryKey = columnPrimaryKeys;
    }

    private void fillDataTable( string groupID , DateTime startDate , DateTime endDate )
    {
      QuantProject.DataAccess.Tables.Quotes.SetDataTable( 
        groupID , startDate , endDate , this );
      this.setPrimaryKeys(); 
    }
		
		/// <summary>
		/// Gets the groupID whose quotes are contained into the GroupQuotes object
		/// </summary>
		/// <returns></returns>
		public string GroupID
		{
			get{ return groupID; }
		}
    
    /// <summary>
    /// Gets the adjusted close for the given ticker at the given date
    /// </summary>
    /// <returns></returns>
    public float GetAdjustedClose( string ticker, DateTime date )
    {
      object[] keys = new object[2];
      keys[0] = ticker;
      keys[1] = date.Date;
      DataRow foundRow = this.Rows.Find(keys);
      if(foundRow==null)
        throw new Exception("No quote for such ticker and date!");
      return (float)foundRow[Quotes.AdjustedClose]; 
    }
    
    /// <summary>
    /// Gets the date of the first quote for the given ticker 
    /// </summary>
    /// <returns></returns>
    public DateTime GetStartDate(string ticker)
    {
      //if (!this.HasTicker(ticker))
        //throw new Exception("There's no such a ticker in the given group!");
      DataRow[] rows = this.Select(Quotes.TickerFieldName + "='" + ticker + "'", Quotes.Date);
      return (DateTime) rows[0][Quotes.Date];
    }
    
    /// <summary>
    /// Gets the date of the last quote for the given ticker
    /// </summary>
    /// <returns></returns>
    public DateTime GetEndDate(string ticker)
    {
      //if (!this.HasTicker(ticker))
          //throw new Exception("There's no such a ticker in the given group!");
      DataRow[] rows = this.Select(Quotes.TickerFieldName + "='" + ticker + "'", Quotes.Date);
      return (DateTime) rows[rows.Length - 1][Quotes.Date];
    }
   
		
    /// <summary>
    /// Returns true if a quote is available at the given date for a specific ticker
    /// </summary>
    /// <param name="ticker">ticker inside the group</param>
    /// <param name="date">date</param>
    /// <returns></returns>
    public bool HasDate( string ticker, DateTime date )
    {
      bool hasDate;
      object[] keys = new object[2];
      keys[0] = ticker;
      keys[1] = date.Date;
      hasDate = this.Rows.Contains(keys);
      return hasDate;
    }
    
    /// <summary>
    /// Returns true if the given ticker is contained into the groupQuotes object
    /// </summary>
    /// <param name="ticker">ticker inside the group</param>
    /// <param name="date">date</param>
    /// <returns></returns>
    public bool HasTicker(string ticker)
    {
      DataRow[] rows = this.Select(Quotes.TickerFieldName + "='" + ticker + "'");
      return rows.Length>0;
    }

    /// <summary>
    /// If the given ticker has a quote at the given date, then it returns the given date,
    /// else it returns the immediate following date at which a quote is available
    /// </summary>
    /// <param name="date">date</param>
    /// <returns></returns>
    public DateTime GetQuoteDateOrFollowing(string ticker, DateTime date )
    {
      if(this.HasDate(ticker, date))
      {
        return date;
      }
      else
      {
        return GetQuoteDateOrFollowing(ticker, date.AddDays(1));
      }
    }
    
    /// <summary>
    /// If the given ticker has a quote at the given date, then it returns the given date,
    /// else it returns the immediate preceding date at which a quote is available
    /// </summary>
    /// <param name="date">date</param>
    /// <returns></returns>
    public DateTime GetQuoteDateOrPreceding( string ticker, DateTime date )
    {
      if(this.HasDate(ticker, date))
      {
        return date;
      }
      else
      {
        return GetQuoteDateOrPreceding(ticker, date.AddDays(-1));
      }
    }

    /// <summary>
    /// Returns the number of quotes for the given ticker contained into the GroupQuotes object 
    /// </summary>
    /// <param name="ticker">ticker for which the number of quotes has to be returned</param>
    /// <returns></returns>
    public int GetNumberOfQuotes( string ticker )
    {
      DataRow[] rows = this.Select(Quotes.TickerFieldName + "='" + ticker + "'");
      return rows.Length;
    }
    
    /// <summary>
    /// Returns the number of days at which the given ticker has been effectively traded 
    /// </summary>
    /// <param name="ticker">ticker for which the number of days has to be returned</param>
    /// <returns></returns>
    public int GetNumberOfDaysWithEffectiveTrades( string ticker )
    {
      DataRow[] rows = this.Select(Quotes.TickerFieldName + "='" + ticker + "'" +
                                    " AND " + Quotes.Volume + ">0");
      return rows.Length;
    }

    /// <summary>
    /// If the given ticker has a quote at the given date, then it returns the given date,
    /// else it returns the first valid following date at which a quote is available
    /// (or the first valid preceding date, in case date is >= the last available quote) 
    /// </summary>
    /// <param name="date">date</param>
    /// <returns></returns>
    public DateTime GetFirstValidQuoteDate(string ticker, DateTime date)
    {
      DateTime startDate =  this.GetStartDate(ticker);
      DateTime endDate = this.GetEndDate(ticker);
      if(date<startDate || (date>=startDate && date<=endDate))
      {
        return this.GetQuoteDateOrFollowing(ticker, date);
      }
      else
      {
        return this.GetQuoteDateOrPreceding(ticker, date);
      }
    }


	}
}
