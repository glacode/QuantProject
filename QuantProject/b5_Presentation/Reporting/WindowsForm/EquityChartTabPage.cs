using System;
using System.Windows.Forms;
using QuantProject.ADT.Histories;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Presentation.Charting;

namespace QuantProject.Presentation.Reporting.WindowsForm
{
	/// <summary>
	/// Summary description for ReportTabControl.
	/// </summary>
	public class EquityChartTabPage : TabPage
	{
		private AccountReport accountReport;
		private Chart equityChart;

		public EquityChartTabPage( AccountReport accountReport )
		{
			this.Text = "Equity Line";
			this.accountReport = accountReport;
			this.equityChart = new Chart();
			this.equityChart.Dock = DockStyle.Fill;
			History historyToBeCharted = new History();
			historyToBeCharted.Import( this.accountReport.Equity.DataTable ,
				QuantProject.Business.Financial.Accounting.Reporting.Tables.Equity.Date ,
				QuantProject.Business.Financial.Accounting.Reporting.Tables.Equity.AccountValue );
			this.equityChart.Add( historyToBeCharted );
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
