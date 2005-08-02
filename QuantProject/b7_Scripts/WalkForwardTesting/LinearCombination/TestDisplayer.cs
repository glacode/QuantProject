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
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton radioButtonOpenToCloseDaily;
		private System.Windows.Forms.RadioButton radioButtonOpenToCloseWeekly;
		private GenomeRepresentation lastSelectedGenomeRepresentation;

		private void testdisplayer()
		{
			this.dgBestGenomes.DataSource = this.bestGenomes;
		}
		public TestDisplayer( DateTime firstDate , DateTime lastDate ,
			ArrayList bestGenomes )
		{
			if ( bestGenomes.Count == 0 )
				throw new Exception( "bestGenomes is empty! It should contain " +
					"a genome, at least." );
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Glauco code
			this.lastSelectedGenomeRepresentation =
				((GenomeRepresentation)bestGenomes[0]);
			this.dtpFirstDate.Value =
				this.lastSelectedGenomeRepresentation.FirstOptimizationDate;
			this.dtpLastDate.Value =
				this.lastSelectedGenomeRepresentation.LastOptimizationDate;
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
			this.label1 = new System.Windows.Forms.Label();
			this.radioButtonOpenToCloseDaily = new System.Windows.Forms.RadioButton();
			this.radioButtonOpenToCloseWeekly = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.dgBestGenomes)).BeginInit();
			this.SuspendLayout();
			// 
			// dgBestGenomes
			// 
			this.dgBestGenomes.DataMember = "";
			this.dgBestGenomes.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.dgBestGenomes.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgBestGenomes.Location = new System.Drawing.Point(0, 125);
			this.dgBestGenomes.Name = "dgBestGenomes";
			this.dgBestGenomes.Size = new System.Drawing.Size(520, 248);
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
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(400, 40);
			this.label1.TabIndex = 3;
			this.label1.Text = "Left click data grid rows to reset dates to the optimization period. Right click " +
				"to preserve date displacements and backtest.";
			// 
			// radioButtonOpenToCloseDaily
			// 
			this.radioButtonOpenToCloseDaily.Checked = true;
			this.radioButtonOpenToCloseDaily.Location = new System.Drawing.Point(64, 96);
			this.radioButtonOpenToCloseDaily.Name = "radioButtonOpenToCloseDaily";
			this.radioButtonOpenToCloseDaily.Size = new System.Drawing.Size(144, 24);
			this.radioButtonOpenToCloseDaily.TabIndex = 4;
			this.radioButtonOpenToCloseDaily.TabStop = true;
			this.radioButtonOpenToCloseDaily.Text = "Open To Close Daily";
			// 
			// radioButtonOpenToCloseWeekly
			// 
			this.radioButtonOpenToCloseWeekly.Location = new System.Drawing.Point(224, 96);
			this.radioButtonOpenToCloseWeekly.Name = "radioButtonOpenToCloseWeekly";
			this.radioButtonOpenToCloseWeekly.Size = new System.Drawing.Size(144, 24);
			this.radioButtonOpenToCloseWeekly.TabIndex = 5;
			this.radioButtonOpenToCloseWeekly.Text = "Open To Close Weekly";
			// 
			// TestDisplayer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(520, 373);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.radioButtonOpenToCloseWeekly,
																																	this.radioButtonOpenToCloseDaily,
																																	this.label1,
																																	this.dtpLastDate,
																																	this.dtpFirstDate,
																																	this.dgBestGenomes});
			this.Name = "TestDisplayer";
			this.Text = "TestDisplayer";
			((System.ComponentModel.ISupportInitialize)(this.dgBestGenomes)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private bool aRowHasBeenClicked(
			object sender, System.Windows.Forms.MouseEventArgs e )
		{
			DataGrid dataGrid = (DataGrid)sender;
			Point point = new Point( e.X , e.Y );
			DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest( point );
			return hitTestInfo.Row>=0;
		}
		private GenomeRepresentation dgBestGenomes_MouseUp_getClickedGenomeRepresentation(
			object sender, System.Windows.Forms.MouseEventArgs e )
		{
			GenomeRepresentation genomeRepresentation = null;
			DataGrid dataGrid = (DataGrid)sender;
			Point point = new Point( e.X , e.Y );
			DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest( point );
			ArrayList bestGenomes = (ArrayList)dataGrid.DataSource;
			//			DataRow dataRow = dataTable.Rows[ hitTestInfo.Row ];
			if ( hitTestInfo.Row >= 0 )
				// a grid row has been clicked, not the header
				genomeRepresentation =
				(GenomeRepresentation)bestGenomes[ hitTestInfo.Row ];
			return genomeRepresentation;
		}
		private void dgBestGenomes_MouseUp_rightButton_updateDates(
			GenomeRepresentation newSelectedGenomeRepresentation )
		{
			TimeSpan currentFirstDateDisplacement =
				( this.dtpFirstDate.Value -
				this.lastSelectedGenomeRepresentation.FirstOptimizationDate );
			TimeSpan currentLastDateDisplacement =
				( this.dtpLastDate.Value -
				this.lastSelectedGenomeRepresentation.LastOptimizationDate );
			this.dtpFirstDate.Value = newSelectedGenomeRepresentation.FirstOptimizationDate +
				currentFirstDateDisplacement;
			this.dtpLastDate.Value = newSelectedGenomeRepresentation.LastOptimizationDate +
				currentLastDateDisplacement;
		}
		private void dgBestGenomes_MouseUp_rightButton(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			GenomeRepresentation genomeRepresentation =
				this.dgBestGenomes_MouseUp_getClickedGenomeRepresentation( sender , e );
			dgBestGenomes_MouseUp_rightButton_updateDates( genomeRepresentation );
			string[] signedTickers = GenomeRepresentation.GetSignedTickers(
				genomeRepresentation.SignedTickers );
			LinearCombinationTest linearCombinationTest =
				new LinearCombinationTest( this.dtpFirstDate.Value ,
				this.dtpLastDate.Value , signedTickers ,
				this.radioButtonOpenToCloseDaily.Checked );
			linearCombinationTest.Run();
			this.lastSelectedGenomeRepresentation = genomeRepresentation;
		}
		private void dgBestGenomes_MouseUp_leftButton_updateDates(
			GenomeRepresentation newSelectedGenomeRepresentation )
		{
			this.dtpFirstDate.Value =
				newSelectedGenomeRepresentation.FirstOptimizationDate;
			this.dtpLastDate.Value =
				newSelectedGenomeRepresentation.LastOptimizationDate;
		}
		private void dgBestGenomes_MouseUp_leftButton(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( aRowHasBeenClicked( sender , e ) )
				// a grid row has been clicked, not the header
			{
				GenomeRepresentation newSelectedGenomeRepresentation =
					this.dgBestGenomes_MouseUp_getClickedGenomeRepresentation( sender , e );
				dgBestGenomes_MouseUp_leftButton_updateDates( newSelectedGenomeRepresentation );
				this.lastSelectedGenomeRepresentation = newSelectedGenomeRepresentation;
			}
		}
		private void dgBestGenomes_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( e.Button == MouseButtons.Right )
				this.dgBestGenomes_MouseUp_rightButton( sender , e );
			if ( e.Button == MouseButtons.Left )
				this.dgBestGenomes_MouseUp_leftButton( sender , e );
		}
	}
}
