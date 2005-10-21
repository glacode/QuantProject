/*
QuantProject - Quantitative Finance Library

BestTickersScreenerOpenToClose.cs
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
using System.Collections;
using QuantProject.ADT.Statistics;
using QuantProject.Data;
using QuantProject.Data.DataTables;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TickerSelectionTesting.SimpleSelection
{
  /// <summary>
  /// Class for tickers' selection without combination
  /// Open to close
  /// </summary>
  [Serializable]
  public class BestTickersScreenerOpenToClose : BestTickersScreener
  {
    
    public BestTickersScreenerOpenToClose(DataTable setOfInitialTickers,
																DateTime firstQuoteDate,
																DateTime lastQuoteDate,
																double targetPerformance,
                                PortfolioType portfolioType):
                                base(setOfInitialTickers,
                                    firstQuoteDate, lastQuoteDate,
                                    targetPerformance,
                                    portfolioType)
                          
    {
 			this.retrieveData();
    }
    
    protected override float[] getArrayOfRatesOfReturn(string ticker)
    {
      Quotes tickerQuotes = new Quotes(ticker, this.firstQuoteDate, this.lastQuoteDate);
      return ExtendedDataTable.GetRatesOfReturnsFromColumns(tickerQuotes, "quClose", "quOpen");
    }

  }

}
