using System;
using System.Data;
using System.Windows.Forms;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Validation TabPage that will contain a DataGrid containing
	/// suspicious quotes.
	/// </summary>
	public class ValidationGridTabPage : TabPage
	{
		protected ValidationDataGrid dataGrid;

		public ValidationGridTabPage()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public int SetGrid()
		{
			this.dataGrid.DataBind();
			return ((DataView)this.dataGrid.DataSource).Count;
		}
	}
}
