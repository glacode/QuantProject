/*
QuantProject - Quantitative Finance Library

WFMultiOneRankReportDebugger.cs
Copyright (C) 2003 
Glauco Siliprandi

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
using System.Drawing;
using System.Windows.Forms;

using QuantProject.Business.Financial.Accounting;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardMultiOneRank
{
	/// <summary>
	/// Script to debug the walk forward one rank strategy backtest
	/// </summary>
	[Serializable]
	public class WFMultiOneRankReportDebugger
	{
		private int numberOfPortfolioPositions;
		private int numberDaysForInSampleOptimization;
		private string benchmark;

		public WFMultiOneRankReportDebugger(
			int numberOfPortfolioPositions ,
			int numberDaysForInSampleOptimization ,
			string benchmark )
		{
			this.numberOfPortfolioPositions = numberOfPortfolioPositions;
			this.numberDaysForInSampleOptimization =
				numberDaysForInSampleOptimization;
			this.benchmark = benchmark;
		}
		#region MouseClickEventHandler
		private int rightClickEventHandler_getRowNumber(  object sender ,
			MouseEventArgs eventArgs )
		{
			DataGrid dataGrid = (DataGrid)sender;
			Point point = new Point( eventArgs.X , eventArgs.Y );
			DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest( point );
			DataTable dataTable = (DataTable)dataGrid.DataSource;
			return hitTestInfo.Row;
		}
		private void rightClickEventHandler_checkExceptions( object sender ,
			MouseEventArgs eventArgs )
		{
			int rowNumber = rightClickEventHandler_getRowNumber(
				sender , eventArgs );
			DataGrid dataGrid = (DataGrid)sender;
			DataTable dataTable = (DataTable)dataGrid.DataSource;
			DataRow dataRow = dataTable.Rows[ rowNumber ];
			string transactionType = (string)dataRow[ "TransactionType" ];
			if ( ( transactionType != TransactionType.BuyLong.ToString() ) &&
				( transactionType != TransactionType.SellShort.ToString() ) )
				throw new Exception( "The right button of the mouse have been " +
					"clicked on a closing position area. It has to be clicked on " +
					"an opening position area!" );
		}

		private int rightClickEventHandler_withDataRow_getSignedTickers_getFirstRowNumberForTheGroup(
			DataTable dataTable , int rowNumber )
		{			
			DateTime tradingDateTime = (DateTime)dataTable.Rows[ rowNumber ][ "DateTime" ];
			int currentRowIndex = rowNumber;
			while ( ( currentRowIndex > 0 ) &&
				( (DateTime)dataTable.Rows[ currentRowIndex - 1 ][ "DateTime" ] ==
				tradingDateTime ) )
				// current row is still in the same transaction group as the mouse clicked row
				currentRowIndex -- ;
			int returnValue = currentRowIndex;
			if ( currentRowIndex >= this.numberOfPortfolioPositions )
				returnValue += this.numberOfPortfolioPositions;
			return returnValue;
		}
		private string rightClickEventHandler_withDataRow_getSignedTicker(
			DataRow dataRow )
		{
			string returnValue = (string)dataRow[ "InstrumentKey" ];
			string transactionType = (string)dataRow[ "TransactionType" ];
			if ( transactionType == TransactionType.SellShort.ToString() ) 
				// current position is short in the group
				returnValue = "-" + returnValue;
			return returnValue;
		}
		private string[] rightClickEventHandler_withDataRow_getSignedTickers(
			DataTable dataTable , int rowNumber )
		{
			string[] signedTickers = new string[ this.numberOfPortfolioPositions ];
			int firstRowNumberForTheGroup =
				rightClickEventHandler_withDataRow_getSignedTickers_getFirstRowNumberForTheGroup(
				dataTable , rowNumber );
			int currentRowIndex = firstRowNumberForTheGroup;
			while ( dataTable.Rows[ currentRowIndex ][ "DateTime" ]
				== dataTable.Rows[ rowNumber ][ "DateTime" ] )
			{
				// the current row is still in the group
				signedTickers[ currentRowIndex - firstRowNumberForTheGroup ] =
					rightClickEventHandler_withDataRow_getSignedTicker(
					dataTable.Rows[ currentRowIndex ] );
				currentRowIndex ++ ;
			}
			return signedTickers;
		}
		private void rightClickEventHandler_withDataRow_getNeededDataFromGrid(
			DataGrid dataGrid , int rowNumber , out string[] signedTickers ,
			out DateTime dateTime )
		{
			DataTable dataTable = (DataTable)dataGrid.DataSource;
			signedTickers =	rightClickEventHandler_withDataRow_getSignedTickers(
				dataTable , rowNumber );
			DataRow dataRow = dataTable.Rows[ rowNumber ];
			dateTime = (DateTime)dataRow[ "DateTime" ];
		}
		private void rightClickEventHandler_withRowNumber( DataGrid dataGrid ,
			int rowNumber )
		{
			string[] signedTickers;
			DateTime dateTime;

			rightClickEventHandler_withDataRow_getNeededDataFromGrid(
				dataGrid , rowNumber , out signedTickers , out dateTime );
			WFMultiOneRankDebugInSample wFMultiOneRankDebugInSample =
				new WFMultiOneRankDebugInSample( signedTickers , dateTime ,
				this.numberDaysForInSampleOptimization ,
				this.benchmark );
			wFMultiOneRankDebugInSample.Run();
		}
		private void rightClickEventHandler( object sender ,
			MouseEventArgs eventArgs )
		{
			rightClickEventHandler_checkExceptions( sender ,
				eventArgs );
			int rowNumber = rightClickEventHandler_getRowNumber(
				sender , eventArgs );
			rightClickEventHandler_withRowNumber(
				(DataGrid)sender , rowNumber );
			//			MessageBox.Show( dataRow[ "DateTime" ].ToString() );
		}
		public void MouseClickEventHandler( object sender ,
			MouseEventArgs eventArgs )
		{
			if ( eventArgs.Button == MouseButtons.Right )
				this.rightClickEventHandler( sender , eventArgs );
		}
		#endregion
	}
}
