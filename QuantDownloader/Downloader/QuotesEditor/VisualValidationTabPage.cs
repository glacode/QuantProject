using System;
using System.Drawing;
using System.Windows.Forms;
using QuantProject.ADT;
using QuantProject.Presentation.Charting;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Generic TabPage containing a DataGrid and a Chart
	/// for visual validation
	/// </summary>
	public class VisualValidationTabPage : ValidationGridTabPage
	{
		public VisualValidationDataGrid VisualValidationDataGrid;
		public VisualValidationChart VisualValidationChart;
		protected int VisualValidationDataGridWidth = 150;

		public VisualValidationTabPage()
		{
		}

		#region initializeCloseToCloseDataGrid
//		private void initializeVisualValidationDataGrid_mouseUp(object sender, MouseEventArgs e)
//		{
//			Console.WriteLine( this.VisualValidationDataGrid[ this.VisualValidationDataGrid.CurrentRowIndex , 0 ] );
//			this.VisualValidationChart.SuspiciousDateTime =
//				(DateTime)this.VisualValidationDataGrid[ this.VisualValidationDataGrid.CurrentRowIndex , 0 ];
//			this.VisualValidationChart.Invalidate();
//		}
		private void initializeVisualValidationDataGrid()
		{
//			this.VisualValidationDataGrid = new CloseToCloseDataGrid();
			this.VisualValidationDataGrid.Location = new Point( 0 , 0 );
			this.VisualValidationDataGrid.Width = this.VisualValidationDataGridWidth;
//			this.VisualValidationDataGrid.MouseUp +=
//				new MouseEventHandler( this.initializeVisualValidationDataGrid_mouseUp );
		}
    #endregion

		private void initializeCloseToCloseChart()
		{
//			this.VisualValidationChart = new CloseToCloseChart();
			this.VisualValidationChart.Location = new Point( VisualValidationDataGridWidth + 5 , 0 );
			this.Controls.Add( this.VisualValidationChart );
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			initializeVisualValidationDataGrid();
			initializeCloseToCloseChart();
			this.VisualValidationDataGrid.DataBind();
			//			this.VisualValidationChart.Ticker = this.ticker;
			if ( this.VisualValidationDataGrid.CurrentRowIndex >= 0 )
				this.VisualValidationChart.SuspiciousDateTime =
					(DateTime)this.VisualValidationDataGrid[ this.VisualValidationDataGrid.CurrentRowIndex , 0 ];
			this.VisualValidationDataGrid.Height = this.Height - 10;
			this.VisualValidationChart.PrecedingDays = ConstantsProvider.PrecedingDaysForVisualValidation;
			this.VisualValidationChart.Width = this.Width - this.VisualValidationDataGridWidth - 5;
			this.VisualValidationChart.Height = this.Height - 10;
			base.OnPaint( e );
		}

		/// <summary>
		/// clears the contained objects
		/// </summary>
		public void Clear()
		{
			this.VisualValidationDataGrid.DataSource = null;
			this.Controls.Add( this.VisualValidationDataGrid );
		}

//		protected override void OnMouseUp( MouseEventArgs e )
//		{
//			Console.WriteLine( this.VisualValidationDataGrid[ this.VisualValidationDataGrid.CurrentRowIndex , 0 ] );
//			this.VisualValidationChart.SuspiciousDateTime =
//				(DateTime)this.VisualValidationDataGrid[ this.VisualValidationDataGrid.CurrentRowIndex , 0 ];
//			this.VisualValidationChart.Invalidate();
//		}
	}
}
