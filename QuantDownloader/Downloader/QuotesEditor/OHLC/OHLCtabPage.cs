using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuantProject.Applications.Downloader.Validate;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// This TabPage will contain the Open High Low Close inconsistencies
	/// </summary>
	public class OHLCtabPage : ValidationGridTabPage
	{
    private OHLCdataGrid ohlcDataGrid;
		public OHLCtabPage()
		{
      this.Text = "OHLC";
      initialize_ohlcDataGrid();
			this.dataGrid = this.ohlcDataGrid;
    }
    private void initialize_ohlcDataGrid()
    {
      this.ohlcDataGrid = new OHLCdataGrid();
      this.ohlcDataGrid.Location = new Point( 0 , 0 );
      this.Controls.Add( this.ohlcDataGrid );
    }
    protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {
      this.ohlcDataGrid.Width = this.Parent.Width - 10;
      this.ohlcDataGrid.DataBind();
      base.OnPaint( e );
    }
		/// <summary>
		/// clears the contained objects
		/// </summary>
		public void Clear()
		{
			this.ohlcDataGrid.DataSource = null;
		}
	}
}
