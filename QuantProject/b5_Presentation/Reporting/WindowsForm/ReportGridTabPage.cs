using System;
using System.Windows.Forms;
using QuantProject.Business.Financial.Accounting.Reporting;

namespace QuantProject.Presentation.Reporting.WindowsForm
{
	/// <summary>
	/// Summary description for RoundTradesTabPage.
	/// </summary>
	public class ReportGridTabPage : TabPage
	{
		public ReportGridTabPage( string title , ReportTable reportTable )
		{
			this.Text = title;
			ReportGrid reportGrid = new ReportGrid( reportTable );
			reportGrid.Dock = DockStyle.Fill;
			this.Controls.Add( reportGrid );
		}
	}
}
