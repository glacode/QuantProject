/*
QuantProject - Quantitative Finance Library

CloseToCloseUserControl.cs
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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using QuantProject.Applications.Downloader.Validate;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Content for the close to close validation TabPage
	/// </summary>
	public class CloseToCloseUserControl : System.Windows.Forms.UserControl
	{
    private CloseToCloseDataGrid closeToCloseDataGrid;
    private CloseToCloseChart closeToCloseChart;

    private string ticker;

    public string Ticker
    {
      get { return this.ticker; }
      set { this.ticker = value; }
    }

    public ValidateDataTable ValidateDataTable;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public CloseToCloseUserControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
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
      this.closeToCloseDataGrid.Width = 150;
      this.closeToCloseDataGrid.Height = this.Height - 4;
      this.closeToCloseDataGrid.MouseUp +=
        new MouseEventHandler( this.closeToCloseDataGrid_mouseUp );
      this.Controls.Add( this.closeToCloseDataGrid );
    }
    #endregion

    private void initializeCloseToCloseChart()
    {
      this.closeToCloseChart = new CloseToCloseChart();
      this.closeToCloseChart.Location = new Point( 155 , 0 );
      this.closeToCloseChart.Height = this.Height - 4;
      this.closeToCloseChart.Width = this.Width - 150;
      this.Controls.Add( this.closeToCloseChart );
    }

    public void PaintingHandler()
    {
      this.Width = this.Parent.Width - 10;
      this.Height = this.Parent.Height - 10;
      this.closeToCloseDataGrid.DataBind( this.ValidateDataTable );
      this.closeToCloseChart.Ticker = this.ticker;
    }

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
      // 
      // CloseToCloseUserControl
      // 
      this.Name = "CloseToCloseUserControl";
      this.Size = new System.Drawing.Size(648, 232);
//      this.Paint += new System.Windows.Forms.PaintEventHandler(this.CloseToCloseUserControl_Paint);
      this.closeToCloseChart = new CloseToCloseChart();
    }
		#endregion

    protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {
      this.closeToCloseDataGrid.DataBind( this.ValidateDataTable );
      this.closeToCloseChart.Ticker = this.ticker;
      this.closeToCloseChart.SuspiciousDateTime =
        (DateTime)this.closeToCloseDataGrid[ this.closeToCloseDataGrid.CurrentRowIndex , 0 ];
      base.OnPaint( e );
    }
	}
}
