/*
QuantProject - Quantitative Finance Library

EquityChartTabPage.cs
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
using System.Drawing;
using System.Windows.Forms;
using QuantProject.ADT.Histories;
//using QuantProject.Data.DataProviders;
using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Timing;
using QuantProject.Presentation.Charting;

namespace QuantProject.Presentation.Reporting.WindowsForm
{
	/// <summary>
	/// TabPage for the equity chart for the report form
	/// </summary>
	public class EquityChartTabPage : TabPage
	{
		private AccountReport accountReport;
		private Chart equityChart;
		private History equity = new History();
		private History benchmark;
		
		/// <summary>
		/// Returns the history for the BuyAndHoldTicker, normalized for
		/// a proper display, that is, to be displayed overlapped to
		/// the equity line
		/// </summary>
		/// <returns></returns>
		private History getBenchmark()
		{
//			History buyAndHoldTickerEquityLine =
//				HistoricalDataProvider.GetAdjustedCloseHistory(
//				this.accountReport.BuyAndHoldTicker );
//			HistoricalAdjustedQuoteProvider quoteProvider =
//				new HistoricalAdjustedQuoteProvider();
//			DateTime firstDate = (DateTime)this.equity.GetKey( 0 );
//			double normalizingFactor =
//				( double )this.equity[ firstDate ] /
//				( double )quoteProvider.GetMarketValue( this.accountReport.BuyAndHoldTicker ,ù
			DateTime firstDate = (DateTime)this.equity.GetKey( 0 );
			double normalizingFactor =
				( double )this.equity[ firstDate ] /
				Convert.ToDouble( this.accountReport.BenchmarkEquityLine[ firstDate ] );
			return this.accountReport.BenchmarkEquityLine.MultiplyBy( normalizingFactor );
		}
		public EquityChartTabPage( AccountReport accountReport )
		{
			this.Text = "Equity Line";
			this.accountReport = accountReport;
			this.equityChart = new Chart();
			this.equityChart.Dock = DockStyle.Fill;
			this.equity.Import( this.accountReport.Equity.DataTable ,
				QuantProject.Business.Financial.Accounting.Reporting.Tables.Equity.Date ,
				QuantProject.Business.Financial.Accounting.Reporting.Tables.Equity.AccountValue );
			this.equityChart.Add( equity , Color.Red );
			this.benchmark = this.getBenchmark();
			this.equityChart.Add( benchmark , Color.Blue , (DateTime)this.equity.GetKey( 0 ) ,
				(DateTime)this.equity.GetKey( this.equity.Count - 1 ) );
			this.Controls.Add( this.equityChart );
		}
//		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
//		{
//			this.VisualValidationDataGrid.Height = this.Height - 10;
//			this.VisualValidationChart.PrecedingDays = ConstantsProvider.PrecedingDaysForVisualValidation;
//			this.VisualValidationChart.Width = this.Width - this.VisualValidationDataGridWidth - 5;
//			this.VisualValidationChart.Height = this.Height - 10;
//			base.OnPaint( e );
//		}	
	}
}
