/*
QuantProject - Quantitative Finance Library

TestDisplayer.cs
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
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Summary description for TestDisplayer.
	/// </summary>
	public class TestDisplayer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.DataGrid dgBestGenomes;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.DateTimePicker dtpFirstDate;
		private System.Windows.Forms.DateTimePicker dtpLastDate;

		// Glauco code
		private ArrayList bestGenomes;

		private void testdisplayer()
		{
			this.dgBestGenomes.DataSource = this.bestGenomes;
		}
		public TestDisplayer( DateTime firstDate , DateTime lastDate ,
			ArrayList bestGenomes )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Glauco code
			this.dtpFirstDate.Value = firstDate;
			this.dtpLastDate.Value = lastDate;
      this.bestGenomes = bestGenomes;
			this.testdisplayer();
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgBestGenomes = new System.Windows.Forms.DataGrid();
			this.dtpFirstDate = new System.Windows.Forms.DateTimePicker();
			this.dtpLastDate = new System.Windows.Forms.DateTimePicker();
			((System.ComponentModel.ISupportInitialize)(this.dgBestGenomes)).BeginInit();
			this.SuspendLayout();
			// 
			// dgBestGenomes
			// 
			this.dgBestGenomes.DataMember = "";
			this.dgBestGenomes.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.dgBestGenomes.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgBestGenomes.Location = new System.Drawing.Point(0, 93);
			this.dgBestGenomes.Name = "dgBestGenomes";
			this.dgBestGenomes.Size = new System.Drawing.Size(496, 248);
			this.dgBestGenomes.TabIndex = 0;
			this.dgBestGenomes.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgBestGenomes_MouseUp);
			// 
			// dtpFirstDate
			// 
			this.dtpFirstDate.Location = new System.Drawing.Point(16, 24);
			this.dtpFirstDate.Name = "dtpFirstDate";
			this.dtpFirstDate.TabIndex = 1;
			// 
			// dtpLastDate
			// 
			this.dtpLastDate.Location = new System.Drawing.Point(264, 24);
			this.dtpLastDate.Name = "dtpLastDate";
			this.dtpLastDate.Size = new System.Drawing.Size(208, 20);
			this.dtpLastDate.TabIndex = 2;
			// 
			// TestDisplayer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(496, 341);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.dtpLastDate,
																																	this.dtpFirstDate,
																																	this.dgBestGenomes});
			this.Name = "TestDisplayer";
			this.Text = "TestDisplayer";
			((System.ComponentModel.ISupportInitialize)(this.dgBestGenomes)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void dgBestGenomes_MouseUp_actually(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			DataGrid dataGrid = (DataGrid)sender;
			Point point = new Point( e.X , e.Y );
			DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest( point );
			ArrayList bestGenomes = (ArrayList)dataGrid.DataSource;
//			DataRow dataRow = dataTable.Rows[ hitTestInfo.Row ];
			GenomeRepresentation genomeRepresentation =
				(GenomeRepresentation)bestGenomes[ hitTestInfo.Row ];
			string[] signedTickers = GenomeRepresentation.GetSignedTickers(
				genomeRepresentation.SignedTickers );
			LinearCombinationTest linearCombinationTest =
				new LinearCombinationTest( this.dtpFirstDate.Value ,
				this.dtpLastDate.Value , signedTickers );
			linearCombinationTest.Run();
		}
		private void dgBestGenomes_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( e.Button == MouseButtons.Right )
				this.dgBestGenomes_MouseUp_actually( sender , e );
		}
	}
}
