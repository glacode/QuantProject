using System;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// DataGrid to be displayed within a ValidationGridTabPage
	/// </summary>
	public abstract class ValidationDataGrid : QuotesDataGrid
	{
		public ValidationDataGrid()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public abstract void DataBind();
	}
}
