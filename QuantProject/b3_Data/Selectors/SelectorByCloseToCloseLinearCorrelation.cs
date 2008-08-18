/*
QuantProject - Quantitative Finance Library

SelectorByCloseToCloseLinearCorrelation.cs
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
using System.Collections;
using System.Data;
using System.Windows.Forms;

using QuantProject.ADT.Statistics;
using QuantProject.DataAccess.Tables;
using QuantProject.Data.DataTables;

namespace QuantProject.Data.Selectors
{
  /// <summary>
  /// Class for selection on tickers by close to close linear correlation
  /// </summary>
   public class SelectorByCloseToCloseLinearCorrelation : TickerSelector , ITickerSelector
  {
    
    public SelectorByCloseToCloseLinearCorrelation(DataTable setOfTickersToBeSelected, 
                               bool orderInASCmode,
                               DateTime firstQuoteDate,
                               DateTime lastQuoteDate,
                               long maxNumOfReturnedTickers):
                                    base(setOfTickersToBeSelected, 
                                         orderInASCmode,
                                         firstQuoteDate,
                                         lastQuoteDate,
                                         maxNumOfReturnedTickers)
    {

    }
    public SelectorByCloseToCloseLinearCorrelation(string groupID, 
                                bool orderInASCmode,
                                DateTime firstQuoteDate,
                                DateTime lastQuoteDate,
																long maxNumOfReturnedTickers):
                                  base(groupID, 
                                      orderInASCmode,
                                      firstQuoteDate,
                                      lastQuoteDate,
                                      maxNumOfReturnedTickers)
    {

    }


    public DataTable GetTableOfSelectedTickers()
    {
      if(this.setOfTickersToBeSelected == null)
        return this.getTickersByAdjCloseToClosePearsonCorrelationCoefficient(this.isOrderedInASCMode,
                                                    this.groupID,
                                                    this.firstQuoteDate,
                                                    this.lastQuoteDate); 
      else
        return this.getTickersByAdjCloseToClosePearsonCorrelationCoefficient(this.isOrderedInASCMode,
                                                    this.setOfTickersToBeSelected,
                                                    this.firstQuoteDate,
                                                    this.lastQuoteDate);
    }
    
		 private void getTickersByAdjCloseToClosePearsonCorrelationCoefficient_setTickersAdjCloses(out float[][]
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
		 private DataTable getTickersByAdjCloseToClosePearsonCorrelationCoefficient( bool orderByASC,
			 DataTable setOfTickers,
			 DateTime firstQuoteDate,
			 DateTime lastQuoteDate)
                                                              
		 {
			 if(!setOfTickers.Columns.Contains("CorrelatedTicker"))
				 setOfTickers.Columns.Add("CorrelatedTicker", System.Type.GetType("System.String"));
			 if(!setOfTickers.Columns.Contains("PearsonCorrelationCoefficient"))
				 setOfTickers.Columns.Add("PearsonCorrelationCoefficient", System.Type.GetType("System.Double"));
			 int initialNumberOfRows = setOfTickers.Rows.Count;
			 float[][] tickersAdjCloses;
			 getTickersByAdjCloseToClosePearsonCorrelationCoefficient_setTickersAdjCloses(out tickersAdjCloses, setOfTickers, firstQuoteDate, lastQuoteDate);
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
					 			tickersAdjCloses[j],tickersAdjCloses[i]);
					 }
					 catch(Exception ex)
					 {
					 	string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
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
		 private DataTable getTickersByAdjCloseToClosePearsonCorrelationCoefficient( bool orderByASC,
			 string groupID,
			 DateTime firstQuoteDate,
			 DateTime lastQuoteDate)
                                                              
		 {
      
			 DataTable tickersOfGroup = 
				 new QuantProject.Data.DataTables.Tickers_tickerGroups(groupID);
			 return this.getTickersByAdjCloseToClosePearsonCorrelationCoefficient(orderByASC,
				 tickersOfGroup,
				 firstQuoteDate,
				 lastQuoteDate);
		 }

    
    public void SelectAllTickers()
    {
      ;
    }	
   }
}
