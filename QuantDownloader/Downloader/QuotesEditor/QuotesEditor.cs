using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace QuantProject.Applications.Downloader.QuotesEditor
{
	/// <summary>
	/// Summary description for QuotesEditor.
	/// </summary>
	public class QuotesEditor : System.Windows.Forms.Form
	{
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPageChart;
    private System.Windows.Forms.TabPage tabPageDataGrid;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public QuotesEditor()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tabPageChart = new System.Windows.Forms.TabPage();
      this.tabPageDataGrid = new System.Windows.Forms.TabPage();
      this.tabControl1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                              this.tabPageChart,
                                                                              this.tabPageDataGrid});
      this.tabControl1.Location = new System.Drawing.Point(8, 8);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(552, 264);
      this.tabControl1.TabIndex = 0;
      // 
      // tabPageChart
      // 
      this.tabPageChart.Location = new System.Drawing.Point(4, 22);
      this.tabPageChart.Name = "tabPageChart";
      this.tabPageChart.Size = new System.Drawing.Size(544, 238);
      this.tabPageChart.TabIndex = 0;
      this.tabPageChart.Text = "Chart";
      // 
      // tabPageDataGrid
      // 
      this.tabPageDataGrid.Location = new System.Drawing.Point(4, 22);
      this.tabPageDataGrid.Name = "tabPageDataGrid";
      this.tabPageDataGrid.Size = new System.Drawing.Size(544, 238);
      this.tabPageDataGrid.TabIndex = 1;
      this.tabPageDataGrid.Text = "Quotes";
      // 
      // QuotesEditor
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(576, 273);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.tabControl1});
      this.Name = "QuotesEditor";
      this.Text = "QuotesEditor";
      this.Load += new System.EventHandler(this.QuotesEditor_Load);
      this.tabControl1.ResumeLayout(false);
      this.ResumeLayout(false);

    }
		#endregion

    private void QuotesEditor_Load(object sender, System.EventArgs e)
    {
    
    }
	}
}
