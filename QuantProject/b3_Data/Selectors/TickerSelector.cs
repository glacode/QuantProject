/*
QuantProject - Quantitative Finance Library

TickerSelector.cs
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
using QuantProject.DataAccess.Tables;
using QuantProject.Data.DataTables;

namespace QuantProject.Data.Selectors
{
  /// <summary>
  /// Class for advanced selections on tickers
  /// </summary>
  /// <remarks>
  /// Filter/selection results depend on the SelectionRule used for the instanciation 
  /// of a new TickerSelector
  /// </remarks>
	
  public class TickerSelector : ITickerSelector
  {
    private SelectionRule selectionRule;

    public TickerSelector(SelectionRule selectionRule)
    {
      this.selectionRule = selectionRule;
    }
    
    public DataTable GetSelectedTickers()
    {
       //TODO: implement switching code to the proper method of selection
       return Quotes.GetMostLiquidTickers(this.selectionRule.GroupID,
                                            this.selectionRule.FirstQuoteDate,
                                            this.selectionRule.LastQuoteDate,
                                            this.selectionRule.MaxNumOfReturnedTickers);
    }
    
    
    //implementation of ITickerSelector
    public TickerDataTable GetTableOfSelectedTickers()
    {
      return TickerDataTable.ConvertToTickerDataTable(this.GetSelectedTickers());
    }
    
    public void SelectAllTickers()
    {
      ;
    }
    // end of implementation of ITickerSelector

	}
}
