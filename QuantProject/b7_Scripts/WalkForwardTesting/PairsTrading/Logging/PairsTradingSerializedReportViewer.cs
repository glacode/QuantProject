/*
QuantProject - Quantitative Finance Library

PairsTradingSerializedReportViewer.cs
Copyright (C) 2009
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
using System.Windows.Forms;

using QuantProject.ADT.FileManaging;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Accounting.Reporting.Tables;
using QuantProject.Business.Strategies;
using QuantProject.Data.DataProviders.Bars.Caching;
using QuantProject.Presentation.Reporting.WindowsForm;
using QuantProject.Scripts.CallingReportsForRunScripts;

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Opens a serialized report and analyzes it
	/// </summary>
	public class PairsTradingSerializedReportViewer
	{
		private string fullPathFileNameForSerializedAccountReport;
		private AccountReport accountReport;
		private QuantProject.Business.DataProviders.HistoricalMarketValueProvider historicalMarketValueProvider;
		
		public PairsTradingSerializedReportViewer()
		{
			this.historicalMarketValueProvider =
				new HistoricalBarProvider( new SimpleBarCache( 60 ) );
		}
		public PairsTradingSerializedReportViewer(HistoricalMarketValueProvider historicalMarketValueProvider)
		{
			this.historicalMarketValueProvider = historicalMarketValueProvider;
		}
		
		#region Run
		
		#region createAccountReportFromSerialization
		private string getFullPathFileNameForSerializedAccountReport()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Select a serialized report please ...";
			openFileDialog.Multiselect = false;
			openFileDialog.CheckFileExists = true;
			openFileDialog.Filter = "Account Reports (*.qpr)|*.qpr";
			openFileDialog.ShowDialog();
			return openFileDialog.FileName;
		}
		private void getAccountReport( string fullPathFileNameForSerializedAccountReport )
		{
			this.accountReport =
				(AccountReport)ObjectArchiver.Extract(
					fullPathFileNameForSerializedAccountReport );
			Report report = new Report( accountReport );
			report.Text =
				fullPathFileNameForSerializedAccountReport.Substring(
					fullPathFileNameForSerializedAccountReport.LastIndexOf("\\") + 1 );
		}
		private void createAccountReportFromSerialization()
		{
			this.fullPathFileNameForSerializedAccountReport =
				this.getFullPathFileNameForSerializedAccountReport();
			this.accountReport = (AccountReport)ObjectArchiver.Extract(
				this.fullPathFileNameForSerializedAccountReport );
		}
		#endregion createAccountReportFromSerialization
		
		#region displayAccountReport
		private void displayAccountReport()
		{
			Report report = new Report( this.accountReport );
			report.Text =
				this.fullPathFileNameForSerializedAccountReport.Substring(
					this.fullPathFileNameForSerializedAccountReport.LastIndexOf("\\") + 1);
			report.Show();
		}
		#endregion displayAccountReport
		
		#region displayReport
		private Report displayReport()
		{
			string fullPathFileNameForSerializedAccountReport =
				this.getFullPathFileNameForSerializedAccountReport();
			Report report = ShowReportFromFile.ShowReportFromSerializedAccountReport(
				fullPathFileNameForSerializedAccountReport );
			return report;
		}
		#endregion displayReport
		
		private void addEventHandlerToReport( Report report )
		{
			report.TransactionGrid.RightClickedTransaction +=
				new RightClickedTransactionEventHandler(
					this.rightClickedTransactionEventHandler );
		}
		
		public void Run()
		{
			Report report = this.displayReport();
//			this.createAccountReportFromSerialization();
			this.addEventHandlerToReport( report );
		}
		#endregion Run
		
		#region rightClickedTransactionEventHandler
		
		#region getPairsViewerParameters
		private AccountReport getAccountReport( ReportGrid reportGrid )
		{
			Report report = (Report)reportGrid.FindForm();
			AccountReport accountReport = report.AccountReport;
			return accountReport;
		}
		
		#region getTransactionsDataRows
		
		#region getRowIndexForTheFirstTransactionDataRow
		private bool hasThePreviousDataRowTheSameDateTime( Transactions transactions , int rowIndex )
		{
			DataRow transactionForRowIndex = transactions.DataTable.Rows[ rowIndex ];
			DataRow transactionForPreviousIndex = transactions.DataTable.Rows[ rowIndex - 1 ];
			bool hasTheSameTime =
				( (DateTime)transactionForRowIndex[ Transactions.FieldNameForDateTime ] ==
				 (DateTime)transactionForPreviousIndex[ Transactions.FieldNameForDateTime ] );
			return hasTheSameTime;
		}
		private int getRowIndexForTheFirstTransactionDataRow(
			Transactions transactions , int rowIndex )
		{
			int rowIndexForTheFirstTransactionDataRow = rowIndex;
			if ( ( rowIndex > 0 ) &&
			    this.hasThePreviousDataRowTheSameDateTime( transactions , rowIndex ) )
				rowIndexForTheFirstTransactionDataRow -- ;
			return rowIndexForTheFirstTransactionDataRow;
		}
		#endregion getRowIndexForTheFirstTransactionDataRow
		
		private void getTransactionsDataRowsWithRowIndexForTheFirstTransactionDataRow(
			Transactions transactions , int rowIndexForTheFirstTransactionDataRow ,
			out DataRow firstTransactionDataRow , out DataRow secondTransactionDataRow )
		{
			firstTransactionDataRow =
				transactions.DataTable.Rows[ rowIndexForTheFirstTransactionDataRow ];
			secondTransactionDataRow =
				transactions.DataTable.Rows[ rowIndexForTheFirstTransactionDataRow + 1 ];
		}
		private void getTransactionsDataRows(
			Transactions transactions , int rowIndex ,
			out DataRow firstTransactionDataRow , out DataRow secondTransactionDataRow )
		{
			int rowIndexForTheFirstTransactionDataRow =
				this.getRowIndexForTheFirstTransactionDataRow(
					transactions , rowIndex );
			this.getTransactionsDataRowsWithRowIndexForTheFirstTransactionDataRow(
				transactions , rowIndexForTheFirstTransactionDataRow ,
				out firstTransactionDataRow , out secondTransactionDataRow );
		}
		#endregion getTransactionsDataRows
		
		private void getPairsViewerParameters(
			DataRow firstTransactionDataRow , DataRow secondTransactionDataRow ,
			out DateTime clickedDateTime , out string firstTicker , out string secondTicker )
		{
			clickedDateTime = (DateTime)firstTransactionDataRow[ Transactions.FieldNameForDateTime ];
			firstTicker = (string)firstTransactionDataRow[ Transactions.FieldNameForTicker ];
			secondTicker = (string)secondTransactionDataRow[ Transactions.FieldNameForTicker ];
		}		
		private void getPairsViewerParameters(
			AccountReport accountReport , int rowIndex ,
			out DateTime clickedDateTime , out string firstTicker , out string secondTicker )
		{
			DataRow firstTransactionDataRow;
			DataRow secondTransactionDataRow;
			this.getTransactionsDataRows(
				(Transactions)accountReport.TransactionTable , rowIndex ,
				out firstTransactionDataRow , out secondTransactionDataRow );
			this.getPairsViewerParameters(
				firstTransactionDataRow , secondTransactionDataRow ,
				out clickedDateTime , out firstTicker , out secondTicker );
		}
		private void getPairsViewerParameters(
			ReportGrid reportGrid , int rowIndex ,
			out DateTime clickedDateTime , out string firstTicker , out string secondTicker )
		{
			AccountReport accountReport = this.getAccountReport( reportGrid );
			this.getPairsViewerParameters(
				accountReport , rowIndex ,
				out clickedDateTime , out firstTicker , out secondTicker );
			
		}
		#endregion getPairsViewerParameters
		
		private void runPairsViewer(
			DateTime clickedDateTime , string firstTicker , string secondTicker )
		{
			WeightedPosition firstWeightedPosition = new WeightedPosition( 0.5 , firstTicker );
			WeightedPosition secondWeightedPosition = new WeightedPosition( 0.5 , secondTicker );
			PairsViewer pairsViewer = new PairsViewer(
				this.historicalMarketValueProvider ,
				firstWeightedPosition , secondWeightedPosition , clickedDateTime );
			pairsViewer.Show();
		}
		
		private void rightClickedTransactionEventHandler(
			ReportGrid reportGrid , int rowIndex )
		{
			DateTime clickedDateTime;
			string firstTicker;
			string secondTicker;
			this.getPairsViewerParameters(
				reportGrid , rowIndex ,
				out clickedDateTime , out firstTicker , out secondTicker );
			this.runPairsViewer( clickedDateTime , firstTicker , secondTicker );
		}
		#endregion rightClickedTransactionEventHandler
	}
}
