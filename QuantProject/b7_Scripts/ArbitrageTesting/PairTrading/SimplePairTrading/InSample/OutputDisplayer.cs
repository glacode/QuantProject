/*
QuantProject - Quantitative Finance Library

OutputDisplayer.cs
Copyright (C) 2003 
Marco Milletti

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

namespace QuantProject.Scripts.ArbitrageTesting.PairTrading.SimplePairTrading.InSample
{
	/// <summary>
	/// Summary description for OutputDisplayer
	/// </summary>
	public class OutputDisplayer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonLoadFromFile;
		private System.Windows.Forms.DateTimePicker dtpLastDate;
		private System.Windows.Forms.DateTimePicker dtpFirstDate;
		private System.Windows.Forms.TextBox textBoxMaxNumOfStdDevForNormalGap;
		private System.Windows.Forms.DataGrid dgBestGenomes;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		// user code
		private ArrayList bestGenomes;
		private GenomeRepresentation lastSelectedGenomeRepresentation;

		private void outputDisplayer()
		{
			this.dgBestGenomes.DataSource = this.bestGenomes;
      DataGridTableStyle styleForGrid = 
          new DataGridTableStyle();
      styleForGrid.AllowSorting = true;
      this.dgBestGenomes.TableStyles.Add(styleForGrid);
		}
		public OutputDisplayer( DateTime firstDate , DateTime lastDate ,
			ArrayList bestGenomes )
		{
			if ( bestGenomes.Count == 0 )
				throw new Exception( "bestGenomes is empty! It should contain " +
					"a genome, at least." );
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Marco code
			this.lastSelectedGenomeRepresentation =
				((GenomeRepresentation)bestGenomes[0]);
			this.dtpFirstDate.Value =
				this.lastSelectedGenomeRepresentation.FirstOptimizationDate;
			this.dtpLastDate.Value =
				this.lastSelectedGenomeRepresentation.LastOptimizationDate;
      this.bestGenomes = bestGenomes;
			this.outputDisplayer();
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
		private void InitializeComponent() {
			this.dgBestGenomes = new System.Windows.Forms.DataGrid();
			this.textBoxMaxNumOfStdDevForNormalGap = new System.Windows.Forms.TextBox();
			this.dtpFirstDate = new System.Windows.Forms.DateTimePicker();
			this.dtpLastDate = new System.Windows.Forms.DateTimePicker();
			this.buttonLoadFromFile = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dgBestGenomes)).BeginInit();
			this.SuspendLayout();
			// 
			// dgBestGenomes
			// 
			this.dgBestGenomes.DataMember = "";
			this.dgBestGenomes.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.dgBestGenomes.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgBestGenomes.Location = new System.Drawing.Point(0, 173);
			this.dgBestGenomes.Name = "dgBestGenomes";
			this.dgBestGenomes.Size = new System.Drawing.Size(584, 200);
			this.dgBestGenomes.TabIndex = 0;
			this.dgBestGenomes.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgBestGenomes_MouseUp);
			// 
			// textBoxMaxNumOfStdDevForNormalGap
			// 
			this.textBoxMaxNumOfStdDevForNormalGap.Location = new System.Drawing.Point(264, 88);
			this.textBoxMaxNumOfStdDevForNormalGap.Name = "textBoxMaxNumOfStdDevForNormalGap";
			this.textBoxMaxNumOfStdDevForNormalGap.TabIndex = 5;
			this.textBoxMaxNumOfStdDevForNormalGap.Text = "1";
			// 
			// dtpFirstDate
			// 
			this.dtpFirstDate.Location = new System.Drawing.Point(16, 48);
			this.dtpFirstDate.Name = "dtpFirstDate";
			this.dtpFirstDate.TabIndex = 1;
			// 
			// dtpLastDate
			// 
			this.dtpLastDate.Location = new System.Drawing.Point(264, 48);
			this.dtpLastDate.Name = "dtpLastDate";
			this.dtpLastDate.Size = new System.Drawing.Size(208, 20);
			this.dtpLastDate.TabIndex = 2;
			// 
			// buttonLoadFromFile
			// 
			this.buttonLoadFromFile.Location = new System.Drawing.Point(168, 8);
			this.buttonLoadFromFile.Name = "buttonLoadFromFile";
			this.buttonLoadFromFile.Size = new System.Drawing.Size(136, 24);
			this.buttonLoadFromFile.TabIndex = 4;
			this.buttonLoadFromFile.Text = "Load genomes";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 128);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(400, 32);
			this.label1.TabIndex = 3;
			this.label1.Text = "Left click data grid rows to reset dates to the optimization period. Right click " +
"to preserve date displacements and backtest.";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(200, 23);
			this.label2.TabIndex = 6;
			this.label2.Text = "Max Num of StDev for normal Gap";
			// 
			// OutputDisplayer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(584, 373);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxMaxNumOfStdDevForNormalGap);
			this.Controls.Add(this.buttonLoadFromFile);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.dtpLastDate);
			this.Controls.Add(this.dtpFirstDate);
			this.Controls.Add(this.dgBestGenomes);
			this.Name = "OutputDisplayer";
			this.Text = "OutputDisplayer SimplePairTrading";
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
			RunSimplePairTradingIS runSimplePairTradingIS =
				new RunSimplePairTradingIS(Convert.ToDouble(this.textBoxMaxNumOfStdDevForNormalGap.Text),
                                   genomeRepresentation.NumDaysForGap,
                                   genomeRepresentation.AverageGap,
                                   genomeRepresentation.StdDevGap,
                                   genomeRepresentation.FirstTicker,
                                   genomeRepresentation.SecondTicker,
                                   this.dtpFirstDate.Value ,
				                           this.dtpLastDate.Value);
      runSimplePairTradingIS.Run();
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
