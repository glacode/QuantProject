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
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using QuantProject.DataAccess.Tables;
using QuantProject.Data.Selectors;
using QuantProject.Data.DataTables;

namespace QuantProject.Data.Selectors
{
	/// <summary>
	/// Base class for advanced selections on tickers
	/// </summary>
	public class TickerSelector
	{
		protected DataTable setOfTickersToBeSelected = null;
		protected ITickerSelectorByDate tickerSelectorByDate = null;
		protected string groupID = "";
		protected DateTime firstQuoteDate = QuantProject.ADT.ConstantsProvider.InitialDateTimeForDownload;
		protected DateTime lastQuoteDate = DateTime.Now;
		protected long maxNumOfReturnedTickers = 0;
		protected bool isOrderedInASCMode;
		
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
		/// It gets / sets the order type for the ticker selector
		/// </summary>
		public bool IsOrderedInASCMode
		{
			get{return this.isOrderedInASCMode;}
			set{this.isOrderedInASCMode = value;}
		}

		
		#endregion

		public TickerSelector(DataTable setOfTickersToBeSelected,
		                      bool orderInASCmode,
		                      DateTime firstQuoteDate,
		                      DateTime lastQuoteDate,
		                      long maxNumOfReturnedTickers)
		{
			this.setOfTickersToBeSelected = setOfTickersToBeSelected;
			this.commonInitialization(orderInASCmode, firstQuoteDate,
			                          lastQuoteDate, maxNumOfReturnedTickers);
		}
		
		public TickerSelector(ITickerSelectorByDate tickerSelectorByDate,
		                      bool orderInASCmode,
		                      DateTime firstQuoteDate,
		                      DateTime lastQuoteDate,
		                      long maxNumOfReturnedTickers)
		{
			this.tickerSelectorByDate = tickerSelectorByDate;
			this.commonInitialization(orderInASCmode, firstQuoteDate,
			                          lastQuoteDate, maxNumOfReturnedTickers);
		}
		public TickerSelector(string groupID,
		                      bool orderInASCmode,
		                      DateTime firstQuoteDate,
		                      DateTime lastQuoteDate,
		                      long maxNumOfReturnedTickers)
		{
			this.groupID = groupID;
			this.commonInitialization(orderInASCmode, firstQuoteDate,
			                          lastQuoteDate, maxNumOfReturnedTickers);
		}
		
		private void commonInitialization(bool orderInASCmode,
		                                  DateTime firstQuoteDate,
		                                  DateTime lastQuoteDate,
		                                  long maxNumOfReturnedTickers)
		{
			this.isOrderedInASCMode = orderInASCmode;
			this.firstQuoteDate = firstQuoteDate;
			this.lastQuoteDate = lastQuoteDate;
			this.maxNumOfReturnedTickers = maxNumOfReturnedTickers;
		}
		
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
		
		#region GetDataTableForTickerSelectors
		private static DataTable getEmptyDataTable()
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("tiTicker", System.Type.GetType("System.String"));
			return dataTable;
		}
		/// <summary>
		/// returns a DataTable, suitable to be used by many selectors
		/// </summary>
		/// <param name="tickers"></param>
		/// <returns></returns>
		public static DataTable GetDataTableForTickerSelectors( IEnumerable<string> tickers )
		{
			DataTable dataTable = TickerSelector.getEmptyDataTable();
			foreach( string ticker in tickers )
			{
				DataRow newDataRow = dataTable.NewRow();
				newDataRow[ 0 ] = ticker;
				dataTable.Rows.Add( newDataRow );
			}
			return dataTable;
		}
		#endregion GetDataTableForTickerSelectors
		
		#region GetList
		/// <summary>
		/// transforms the DataTable in a strongly typed list of strings
		/// </summary>
		/// <param name="dataTableOfSelectedTickers"></param>
		/// <returns></returns>
		public static List<string> GetList( DataTable dataTableOfSelectedTickers )
		{
			List<string> tickerList = new List<string>();
			foreach ( DataRow dataRow in dataTableOfSelectedTickers.Rows )
				tickerList.Add( (string)( dataRow[ 0 ] ) );
			return tickerList;
		}
		#endregion GetList
	}
}
