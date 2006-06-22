/*
QuantProject - Quantitative Finance Library

WFLagDebugChosenPositionsCollection.cs
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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger
{
	/// <summary>
	/// Form to debug a chosen positions' collection
	/// </summary>
	public class WFLagDebugChosenPositionsCollection : System.Windows.Forms.Form
	{
		private System.Windows.Forms.DataGrid dataGridChosenPositionsWithAttributes;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private ICollection chosenPositionsCollection;

		public WFLagDebugChosenPositionsCollection( ICollection chosenPositionsCollection )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.chosenPositionsCollection = chosenPositionsCollection;
			this.updateGrid();
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
			this.dataGridChosenPositionsWithAttributes = new System.Windows.Forms.DataGrid();
			((System.ComponentModel.ISupportInitialize)(this.dataGridChosenPositionsWithAttributes)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridChosenPositionsWithAttributes
			// 
			this.dataGridChosenPositionsWithAttributes.DataMember = "";
			this.dataGridChosenPositionsWithAttributes.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGridChosenPositionsWithAttributes.Location = new System.Drawing.Point(24, 40);
			this.dataGridChosenPositionsWithAttributes.Name = "dataGridChosenPositionsWithAttributes";
			this.dataGridChosenPositionsWithAttributes.Size = new System.Drawing.Size(576, 240);
			this.dataGridChosenPositionsWithAttributes.TabIndex = 0;
			// 
			// WFLagDebugChosenPositionsCollection
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(640, 301);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.dataGridChosenPositionsWithAttributes});
			this.Name = "WFLagDebugChosenPositionsCollection";
			this.Text = "WFLagDebugChosenPositionsCollection";
			((System.ComponentModel.ISupportInitialize)(this.dataGridChosenPositionsWithAttributes)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion


		private void updateGrid()
		{
			this.dataGridChosenPositionsWithAttributes.DataSource =
				this.chosenPositionsCollection;
			this.dataGridChosenPositionsWithAttributes.Refresh();
		}
	}
}
