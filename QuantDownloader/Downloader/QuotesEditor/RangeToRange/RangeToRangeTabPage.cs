using System;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// VisualValidationTabPage to contain Range to Range
	/// suspicious ratios
	/// </summary>
	public class RangeToRangeTabPage : VisualValidationTabPage
	{
		public RangeToRangeTabPage()
		{
			this.Text = "Range To Range";
			this.VisualValidationDataGrid = new RangeToRangeDataGrid();
			this.dataGrid = this.VisualValidationDataGrid;
			this.VisualValidationChart = new RangeToRangeChart();
		}
	}
}
