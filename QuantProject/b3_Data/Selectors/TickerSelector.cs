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
using System.Collections;
using System.Data;
using System.Windows.Forms;
using QuantProject.DataAccess.Tables;
using QuantProject.Data.DataTables;

namespace QuantProject.Data.Selectors
{
  /// <summary>
  /// Class for advanced selections on tickers
  /// </summary>
  /// <remarks>
  /// Selection depends on the parameters used in the construction of a new TickerSelector object
  /// </remarks>
  public class TickerSelector : ITickerSelector
  {
    private DataTable setOfTickersToBeSelected = null;
    private SelectionType typeOfSelection;
    private string groupID = "";
    private DateTime firstQuoteDate = QuantProject.ADT.ConstantsProvider.InitialDateTimeForDownload;
    private DateTime lastQuoteDate = DateTime.Now;
    private long maxNumOfReturnedTickers = 0;
    private bool isOrderedInASCMode; 
    
    #region properties
    /// <summary>
    /// It gets the GroupID from which tickers have to be selected
    /// </summary>
    public string GroupID
    {
      get{return this.groupID;}
    }
    /// <summary>
    /// It gets the first date of selection for the quotes
    /// </summary>
    public DateTime FirstQuoteDate
    {
      get{return this.firstQuoteDate;}
    }
    /// <summary>
    /// It gets the last date of selection for the quotes
    /// </summary>
    public DateTime LastQuoteDate
    {
      get{return this.lastQuoteDate;}
    }   
    
    /// <summary>
    /// It gets the max number of tickers to be returned
    /// </summary>
    public long MaxNumOfReturnedTickers
    {
      get{return this.maxNumOfReturnedTickers;}
    }
    /// <summary>
    /// It gets the type of selection provided by the Ticker Selector
    /// </summary>
    public SelectionType TypeOfSelection
    {
      get{return this.typeOfSelection;}
    }

    /// <summary>
    /// It gets / sets the order type for the ticker selector
    /// </summary>
    public bool IsOrderedInASCMode
    {
      get{return this.isOrderedInASCMode;}
      set{this.isOrderedInASCMode = value;}
    }
    
    #endregion

    public TickerSelector(DataTable setOfTickersToBeSelected, SelectionType typeOfSelection,
                          bool orderInASCmode,
                          string groupID,
                          DateTime firstQuoteDate,
                          DateTime lastQuoteDate,
                          long maxNumOfReturnedTickers)
    {
      this.setOfTickersToBeSelected = setOfTickersToBeSelected;
      this.commonInitialization(typeOfSelection, orderInASCmode, groupID, firstQuoteDate,lastQuoteDate,maxNumOfReturnedTickers);
    }
    
    public TickerSelector(SelectionType typeOfSelection,
                          bool orderInASCmode,
                          string groupID,
                          DateTime firstQuoteDate,
                          DateTime lastQuoteDate,
                          long maxNumOfReturnedTickers)
    {
      this.commonInitialization(typeOfSelection, orderInASCmode, groupID, firstQuoteDate,lastQuoteDate,maxNumOfReturnedTickers);
    }
    
    private void commonInitialization(SelectionType typeOfSelection,
                                      bool orderInASCmode,
                                      string groupID,
                                      DateTime firstQuoteDate,
                                      DateTime lastQuoteDate,
                                      long maxNumOfReturnedTickers)
    {
      this.typeOfSelection = typeOfSelection;
      this.isOrderedInASCMode = orderInASCmode;
      this.groupID = groupID;
      this.firstQuoteDate = firstQuoteDate;
      this.lastQuoteDate = lastQuoteDate;
      this.maxNumOfReturnedTickers = maxNumOfReturnedTickers;
    }    
  

    //implementation of ITickerSelector
    public DataTable GetTableOfSelectedTickers()
    {
        switch (this.typeOfSelection)
        {
          case SelectionType.Liquidity:
            return this.getTickersByLiquidity();
          case SelectionType.Performance:
            return this.getTickersByPerformance();
          case SelectionType.CloseToCloseVolatility:
            return this.getTickersByCloseToCloseVolatility();
          case SelectionType.CloseToOpenVolatility:
            return this.getTickersByCloseToOpenVolatility();
          case SelectionType.AverageCloseToClosePerformance:
            return this.getTickersByAverageCloseToClosePerformance();
          case SelectionType.AverageCloseToOpenPerformance:
            return this.getTickersByAverageCloseToOpenPerformance();
          case SelectionType.CloseToCloseLinearCorrelation:
            return this.getTickersByCloseToCloseLinearCorrelation();
          case SelectionType.CloseToOpenLinearCorrelation:
            return this.getTickersByCloseToOpenLinearCorrelation();
            //this line should never be reached!
          default:
            return new DataTable();
        }
      
    }
    
    private DataTable getTickersByLiquidity()
    {
      if(this.setOfTickersToBeSelected == null)
        return QuantProject.DataAccess.Tables.Quotes.GetTickersByLiquidity(this.isOrderedInASCMode,
                                                                        this.groupID,
                                                                        this.firstQuoteDate,
                                                                        this.lastQuoteDate,
                                                                        this.maxNumOfReturnedTickers);        

      else
        return QuantProject.Data.DataTables.Quotes.GetTickersByLiquidity(this.isOrderedInASCMode,
                                                                        this.setOfTickersToBeSelected, 
                                                                        this.firstQuoteDate,
                                                                        this.lastQuoteDate,
                                                                        this.maxNumOfReturnedTickers);
    }
    
    private DataTable getTickersByPerformance()
    {
      if(this.setOfTickersToBeSelected == null)
         return TickerDataTable.GetTickersByPerformance(this.isOrderedInASCMode,
                                                        this.groupID, 
                                                        this.firstQuoteDate,
                                                        this.lastQuoteDate,
                                                        this.maxNumOfReturnedTickers);
      else
        return TickerDataTable.GetTickersByPerformance(this.isOrderedInASCMode,
                                                        this.setOfTickersToBeSelected,
                                                        this.firstQuoteDate,
                                                        this.lastQuoteDate,
                                                        this.maxNumOfReturnedTickers);
      
    }

    private DataTable getTickersByAverageCloseToClosePerformance()
    {
      if(this.setOfTickersToBeSelected == null)
        return QuantProject.DataAccess.Tables.Quotes.GetTickersByAverageCloseToClosePerformance(this.isOrderedInASCMode,
                                                          this.groupID, this.firstQuoteDate,
                                                          this.lastQuoteDate,
                                                          this.maxNumOfReturnedTickers);
      else
        return QuantProject.Data.DataTables.Quotes.GetTickersByAverageCloseToClosePerformance(this.isOrderedInASCMode,
                                                          this.setOfTickersToBeSelected,
                                                          this.firstQuoteDate,
                                                          this.lastQuoteDate,
                                                          this.maxNumOfReturnedTickers);
      
    }

    private DataTable getTickersByAverageCloseToOpenPerformance()
    {
      if(this.setOfTickersToBeSelected == null)
        return QuantProject.DataAccess.Tables.Quotes.GetTickersByAverageCloseToOpenPerformance(this.isOrderedInASCMode,
          this.groupID, this.firstQuoteDate,
          this.lastQuoteDate,
          this.maxNumOfReturnedTickers);
      else
        return QuantProject.Data.DataTables.Quotes.GetTickersByAverageCloseToOpenPerformance(this.isOrderedInASCMode,
          this.setOfTickersToBeSelected,
          this.firstQuoteDate,
          this.lastQuoteDate,
          this.maxNumOfReturnedTickers);
      
    }

    private DataTable getTickersByCloseToCloseVolatility()
    {
      if(this.setOfTickersToBeSelected == null)
        return QuantProject.DataAccess.Tables.Quotes.GetTickersByCloseToCloseVolatility(this.isOrderedInASCMode,
                                                                    this.groupID,
                                                                    this.firstQuoteDate,
                                                                    this.lastQuoteDate,
                                                                    this.maxNumOfReturnedTickers);        

      else
        return QuantProject.Data.DataTables.Quotes.GetTickersByCloseToCloseVolatility(this.isOrderedInASCMode,
          this.setOfTickersToBeSelected, 
          this.firstQuoteDate,
          this.lastQuoteDate,
          this.maxNumOfReturnedTickers);
    }

    private DataTable getTickersByCloseToOpenVolatility()
    {
      if(this.setOfTickersToBeSelected == null)
        return QuantProject.DataAccess.Tables.Quotes.GetTickersByCloseToOpenVolatility(this.isOrderedInASCMode,
          this.groupID,
          this.firstQuoteDate,
          this.lastQuoteDate,
          this.maxNumOfReturnedTickers);        

      else
        return QuantProject.Data.DataTables.Quotes.GetTickersByCloseToOpenVolatility(this.isOrderedInASCMode,
          this.setOfTickersToBeSelected, 
          this.firstQuoteDate,
          this.lastQuoteDate,
          this.maxNumOfReturnedTickers);
    }

    private DataTable getTickersByCloseToCloseLinearCorrelation()
    {
      this.launchExceptionIfGroupIDIsNotEmpty();
      return QuantProject.Data.DataTables.Quotes.GetTickersByAdjCloseToClosePearsonCorrelationCoefficient(this.isOrderedInASCMode,
                                                          this.setOfTickersToBeSelected,
                                                          this.firstQuoteDate,
                                                          this.lastQuoteDate);
    }

    private DataTable getTickersByCloseToOpenLinearCorrelation()
    {
      
      this.launchExceptionIfGroupIDIsNotEmpty();
      
      return QuantProject.Data.DataTables.Quotes.GetTickersByCloseToOpenPearsonCorrelationCoefficient(this.isOrderedInASCMode,
        this.setOfTickersToBeSelected,
        this.firstQuoteDate,
        this.lastQuoteDate);
    }

    private void launchExceptionIfGroupIDIsNotEmpty()
    {
      if(this.groupID!="")
      {
        throw new Exception("Not implemented: this type of selection works only with few tickers, at the moment");
      }
    }

    public void SelectAllTickers()
    {
      ;
    }
    // end of implementation of ITickerSelector
    
    /// <summary>
	  /// It returns a dataTable containing tickers selected by the user
	  /// </summary>
	  /// <param name="dataGrid">The data grid from which the user has selected tickers</param>
    public static DataTable GetTableOfManuallySelectedTickers(DataGrid dataGrid)
    {
      DataTable dataTableOfDataGrid = (DataTable)dataGrid.DataSource;
      DataTable tableOfSelectedTickers = dataTableOfDataGrid.Clone();
      int indexOfRow = 0;
      while(indexOfRow != dataTableOfDataGrid.Rows.Count)
      {
        if(dataGrid.IsSelected(indexOfRow))
        {
          tableOfSelectedTickers.ImportRow(dataTableOfDataGrid.Rows[indexOfRow]);
        }
        indexOfRow++;
      }
      return tableOfSelectedTickers;
    }

    /// <summary>
    /// It returns a DataTable, with only one column, containing tickers
    /// that are contained in both the two tables passed as parameters
    /// </summary>
    /// <param name="firstDataTable">The first data table in which the column indexed 0 contains the first set of tickers' symbols</param>
    /// <param name="secondDataTable">The second data table in which the column indexed 0 contains the second set of tickers' symbols</param>
    public static DataTable GetTableOfCommonTickers(DataTable firstDataTable, DataTable secondDataTable)
    {
      DataTable commonTickers = new DataTable();
      commonTickers.Columns.Add("tiTicker", System.Type.GetType("System.String"));
      Hashtable hashTable = ExtendedDataTable.GetCommonValues(firstDataTable, 
                                                              secondDataTable,0,0);
      object[] values = new object[1];
      foreach(DictionaryEntry element in hashTable )
      {
        values[0]=element.Value;
        commonTickers.Rows.Add(values);
      }
      return commonTickers;
    }


	}
}
