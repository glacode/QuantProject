/*
QuantProject - Quantitative Finance Library

SelectionRule.cs
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

namespace QuantProject.Data.Selectors
{
  /// <summary>
  /// Selection rule used by the TickerSelector
  /// </summary>
  /// <remarks>
  /// Selection process is based on quotes
  /// </remarks>
	
  public class SelectionRule
  {
    private SelectionType typeOfSelection;
    private string groupID = "";
    private DateTime firstQuoteDate = QuantProject.ADT.ConstantsProvider.InitialDateTimeForDownload;
    private DateTime lastQuoteDate = DateTime.Now;
    private long maxNumOfReturnedTickers = 0;
    
    /// <summary>
	  /// SelectionRule constructor 
	  /// </summary>
    /// <param name="typeOfSelection">Type of selection rule to be applied</param>
    /// <param name="groupID">GroupID to which ticker to be selected has to belong</param>
    /// <param name="firstQuoteDate">First date of selection for the quotes</param>
    /// <param name="lastQuoteDate">Last date of selection for the quotes</param>
    /// <param name="maxNumOfReturnedTickers">Max number of tickers to be returned</param>
    
    public SelectionRule(SelectionType typeOfSelection,
                         string groupID,
                         DateTime firstQuoteDate,
                         DateTime lastQuoteDate,
                         long maxNumOfReturnedTickers)



    {
       this.typeOfSelection = typeOfSelection;
       this.groupID = groupID;
       this.firstQuoteDate = firstQuoteDate;
       this.lastQuoteDate = lastQuoteDate;
       this.maxNumOfReturnedTickers = maxNumOfReturnedTickers;
      
    }

    /// <summary>
    /// GroupID from which tickers have to be selected
    /// </summary>
    public string GroupID
    {
      get{return this.groupID;}
    }
    /// <summary>
    /// First date of selection for the quotes
    /// </summary>
    public DateTime FirstQuoteDate
    {
      get{return this.firstQuoteDate;}
    }
    /// <summary>
    /// Last date of selection for the quotes
    /// </summary>
    public DateTime LastQuoteDate
    {
      get{return this.lastQuoteDate;}
    }   
    
    /// <summary>
    /// Max number of tickers to be returned
    /// </summary>
    public long MaxNumOfReturnedTickers
    {
      get{return this.maxNumOfReturnedTickers;}
    }
    /// <summary>
    /// Type of selection provided by the rule
    /// </summary>
    public SelectionType TypeOfSelection
    {
      get{return this.typeOfSelection;}
    }

	}
}
