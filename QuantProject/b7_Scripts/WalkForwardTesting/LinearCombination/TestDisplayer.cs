using System;
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

		// Glauco code
		private ArrayList bestGenomes;

		private void testdisplayer()
		{
			this.dgBestGenomes.DataSource = this.bestGenomes;
		}
		public TestDisplayer( ArrayList bestGenomes )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Glauco code
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
			((System.ComponentModel.ISupportInitialize)(this.dgBestGenomes)).BeginInit();
			this.SuspendLayout();
			// 
			// dgBestGenomes
			// 
			this.dgBestGenomes.DataMember = "";
			this.dgBestGenomes.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgBestGenomes.Location = new System.Drawing.Point(48, 32);
			this.dgBestGenomes.Name = "dgBestGenomes";
			this.dgBestGenomes.Size = new System.Drawing.Size(208, 216);
			this.dgBestGenomes.TabIndex = 0;
			// 
			// TestDisplayer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.dgBestGenomes});
			this.Name = "TestDisplayer";
			this.Text = "TestDisplayer";
			((System.ComponentModel.ISupportInitialize)(this.dgBestGenomes)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion
	}
}
