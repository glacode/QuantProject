using System;
using System.Drawing;
using System.Windows.Forms;
using QuantProject.Business.Validation;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// This TabPage will contain the Close to
	/// Close visual validation controls
	/// </summary>
	public class CloseToCloseTabPage : VisualValidationTabPage
	{
		public CloseToCloseTabPage()
		{
			this.Text = "Close To Close";
			this.VisualValidationDataGrid = new CloseToCloseDataGrid();
			this.dataGrid = this.VisualValidationDataGrid;
			this.VisualValidationChart = new CloseToCloseChart();
    }
	}
}
