/*
QuantProject - Quantitative Finance Library

OHLCuserControl.cs
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
using QuantProject.Business.Validation;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Content for the OHLC validation TabPage
	/// </summary>
	public class OHLCuserControl : System.Windows.Forms.UserControl
	{
    private OHLCdataGrid openHighLowCloseDataGrid;

    public ValidateDataTable ValidateDataTable;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public OHLCuserControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
      initializeOHLCdataGrid();
		}
    private void initializeOHLCdataGrid()
    {
      this.openHighLowCloseDataGrid = new OHLCdataGrid();
      this.Controls.Add( this.openHighLowCloseDataGrid );
      this.openHighLowCloseDataGrid.Width = this.Width - 4;
      this.openHighLowCloseDataGrid.Height = this.Height - 4;
    }

    public void PaintingHandler()
    {
      this.openHighLowCloseDataGrid.DataBind();
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
      // OHLCuserControl
      // 
      this.Name = "OHLCuserControl";
      this.Size = new System.Drawing.Size(648, 232);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.OHLCuserControl_Paint);

    }
		#endregion

    private void OHLCuserControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
      this.Width = this.Parent.Width - 10;
      this.Height = this.Parent.Height - 10;
      this.PaintingHandler();
    }
	}
}
