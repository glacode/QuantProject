using System;
using System.Drawing;
using System.Windows.Forms;
using QuantProject.Applications.Downloader.Validate;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// This TabPage will contain the Close to
	/// Close visual validation controls
	/// </summary>
	public class CloseToCloseTabPage : ValidationGridTabPage
	{
    private CloseToCloseDataGrid closeToCloseDataGrid;
    private CloseToCloseChart closeToCloseChart;
    private int closeToCloseDataGridWidth = 150;
//    private ValidateDataTable validateDataTable;

    private string ticker;

    public string Ticker
    {
      get { return this.ticker; }
      set { this.ticker = value; }
    }

		public CloseToCloseTabPage()
		{
      this.Text = "Close To Close";
      initializeCloseToCloseDataGrid();
      initializeCloseToCloseChart();
    }

    #region initializeCloseToCloseDataGrid
    private void closeToCloseDataGrid_mouseUp(object sender, MouseEventArgs e)
    {
      Console.WriteLine( this.closeToCloseDataGrid[ this.closeToCloseDataGrid.CurrentRowIndex , 0 ] );
      this.closeToCloseChart.SuspiciousDateTime =
        (DateTime)this.closeToCloseDataGrid[ this.closeToCloseDataGrid.CurrentRowIndex , 0 ];
      this.closeToCloseChart.Invalidate();
    }
    private void initializeCloseToCloseDataGrid()
    {
      this.closeToCloseDataGrid = new CloseToCloseDataGrid();
			this.dataGrid = this.closeToCloseDataGrid;
      this.closeToCloseDataGrid.Location = new Point( 0 , 0 );
      this.closeToCloseDataGrid.Width = this.closeToCloseDataGridWidth;
      this.closeToCloseDataGrid.MouseUp +=
        new MouseEventHandler( this.closeToCloseDataGrid_mouseUp );
      this.Controls.Add( this.closeToCloseDataGrid );
    }
    #endregion

    private void initializeCloseToCloseChart()
    {
      this.closeToCloseChart = new CloseToCloseChart();
      this.closeToCloseChart.Location = new Point( closeToCloseDataGridWidth + 5 , 0 );
      this.Controls.Add( this.closeToCloseChart );
    }


    protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {
			Console.WriteLine( "CloseToClosePageTab.OnPaint()" );
      this.closeToCloseDataGrid.DataBind();
			if ( this.closeToCloseDataGrid.VisibleRowCount > 0 )
			{
				this.closeToCloseChart.Ticker = this.ticker;
				this.closeToCloseChart.SuspiciousDateTime =
					(DateTime)this.closeToCloseDataGrid[ this.closeToCloseDataGrid.CurrentRowIndex , 0 ];
				this.closeToCloseDataGrid.Height = this.Height - 10;
				this.closeToCloseChart.Width = this.Width - this.closeToCloseDataGridWidth - 5;
				this.closeToCloseChart.Height = this.Height - 10;
			}
      base.OnPaint( e );
    }

		/// <summary>
		/// clears the contained objects
		/// </summary>
		public void Clear()
		{
			this.closeToCloseDataGrid.DataSource = null;
		}
	}
}
