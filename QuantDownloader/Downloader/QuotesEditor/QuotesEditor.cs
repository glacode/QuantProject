using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using scpl;
using QuantProject.ADT.Histories;
using QuantProject.Data;
using QuantProject.Applications.Downloader.Validate;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// Windows form editor for quotes of a single ticker
	/// </summary>
	public class QuotesEditor : System.Windows.Forms.Form
	{
    private double suspiciousRatio = 8;

    private string ticker;
    private ValidateDataTable validateDataTable = new ValidateDataTable();

    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPageChart;
    private System.Windows.Forms.TabPage tabPageQuotes;
    private System.Windows.Forms.TabPage tabPageValidation;
    private System.Windows.Forms.TextBox textBoxTicker;
    private System.Windows.Forms.TabControl tabControl2;
    private System.Windows.Forms.TabPage tabPageOHLC;
    private System.Windows.Forms.TabPage tabPageCloseToClose;
    private QuantProject.Applications.Downloader.QuotesChart quotesChart;
    private QuantProject.Applications.Downloader.OHLCuserControl openHighLowCloseUserControl;
    private QuantProject.Applications.Downloader.CloseToCloseUserControl closeToCloseUserControl;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public QuotesEditor( string ticker )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

      this.ticker = ticker;
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
      this.quotesChart = new QuantProject.Applications.Downloader.QuotesChart();
      this.tabPageQuotes = new System.Windows.Forms.TabPage();
      this.tabPageValidation = new System.Windows.Forms.TabPage();
      this.tabControl2 = new System.Windows.Forms.TabControl();
      this.tabPageOHLC = new System.Windows.Forms.TabPage();
      this.openHighLowCloseUserControl = new QuantProject.Applications.Downloader.OHLCuserControl();
      this.tabPageCloseToClose = new System.Windows.Forms.TabPage();
      this.closeToCloseUserControl = new QuantProject.Applications.Downloader.CloseToCloseUserControl();
      this.textBoxTicker = new System.Windows.Forms.TextBox();
      this.tabControl1.SuspendLayout();
      this.tabPageChart.SuspendLayout();
      this.tabPageValidation.SuspendLayout();
      this.tabControl2.SuspendLayout();
      this.tabPageOHLC.SuspendLayout();
      this.tabPageCloseToClose.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                              this.tabPageChart,
                                                                              this.tabPageQuotes,
                                                                              this.tabPageValidation});
      this.tabControl1.Location = new System.Drawing.Point(8, 40);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(688, 320);
      this.tabControl1.TabIndex = 0;
      // 
      // tabPageChart
      // 
      this.tabPageChart.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                               this.quotesChart});
      this.tabPageChart.Location = new System.Drawing.Point(4, 22);
      this.tabPageChart.Name = "tabPageChart";
      this.tabPageChart.Size = new System.Drawing.Size(680, 294);
      this.tabPageChart.TabIndex = 2;
      this.tabPageChart.Text = "Chart";
      this.tabPageChart.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPageChart_Paint);
      // 
      // quotesChart
      // 
      this.quotesChart.AllowSelection = false;
      this.quotesChart.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.quotesChart.HorizontalEdgeLegendPlacement = scpl.Legend.Placement.Inside;
      this.quotesChart.LegendBorderStyle = scpl.Legend.BorderType.Shadow;
      this.quotesChart.LegendXOffset = 10F;
      this.quotesChart.LegendYOffset = 1F;
      this.quotesChart.Location = new System.Drawing.Point(8, 8);
      this.quotesChart.Name = "quotesChart";
      this.quotesChart.Padding = 10;
      this.quotesChart.PlotBackColor = System.Drawing.Color.White;
      this.quotesChart.ShowLegend = false;
      this.quotesChart.Size = new System.Drawing.Size(664, 288);
      this.quotesChart.TabIndex = 0;
      this.quotesChart.Title = "";
      this.quotesChart.TitleFont = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
      this.quotesChart.VerticalEdgeLegendPlacement = scpl.Legend.Placement.Outside;
      this.quotesChart.XAxis1 = null;
      this.quotesChart.XAxis2 = null;
      this.quotesChart.YAxis1 = null;
      this.quotesChart.YAxis2 = null;
      // 
      // tabPageQuotes
      // 
      this.tabPageQuotes.Location = new System.Drawing.Point(4, 22);
      this.tabPageQuotes.Name = "tabPageQuotes";
      this.tabPageQuotes.Size = new System.Drawing.Size(680, 294);
      this.tabPageQuotes.TabIndex = 0;
      this.tabPageQuotes.Text = "Quotes";
      this.tabPageQuotes.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPageChart_Paint);
      // 
      // tabPageValidation
      // 
      this.tabPageValidation.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                    this.tabControl2});
      this.tabPageValidation.Location = new System.Drawing.Point(4, 22);
      this.tabPageValidation.Name = "tabPageValidation";
      this.tabPageValidation.Size = new System.Drawing.Size(680, 294);
      this.tabPageValidation.TabIndex = 1;
      this.tabPageValidation.Text = "Validation";
      this.tabPageValidation.Click += new System.EventHandler(this.tabPageValidation_Click);
      this.tabPageValidation.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPageValidation_Paint);
      // 
      // tabControl2
      // 
      this.tabControl2.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                              this.tabPageOHLC,
                                                                              this.tabPageCloseToClose});
      this.tabControl2.Location = new System.Drawing.Point(8, 8);
      this.tabControl2.Multiline = true;
      this.tabControl2.Name = "tabControl2";
      this.tabControl2.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.tabControl2.SelectedIndex = 0;
      this.tabControl2.Size = new System.Drawing.Size(664, 280);
      this.tabControl2.TabIndex = 0;
      // 
      // tabPageOHLC
      // 
      this.tabPageOHLC.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                              this.openHighLowCloseUserControl});
      this.tabPageOHLC.Location = new System.Drawing.Point(4, 22);
      this.tabPageOHLC.Name = "tabPageOHLC";
      this.tabPageOHLC.Size = new System.Drawing.Size(656, 254);
      this.tabPageOHLC.TabIndex = 0;
      this.tabPageOHLC.Text = "OHLC";
      this.tabPageOHLC.Click += new System.EventHandler(this.tabPageOHLC_Click);
      this.tabPageOHLC.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPageOHLC_Paint);
      // 
      // openHighLowCloseUserControl
      // 
      this.openHighLowCloseUserControl.Location = new System.Drawing.Point(8, 8);
      this.openHighLowCloseUserControl.Name = "openHighLowCloseUserControl";
      this.openHighLowCloseUserControl.Size = new System.Drawing.Size(646, 244);
      this.openHighLowCloseUserControl.TabIndex = 0;
      // 
      // tabPageCloseToClose
      // 
      this.tabPageCloseToClose.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                      this.closeToCloseUserControl});
      this.tabPageCloseToClose.Location = new System.Drawing.Point(4, 22);
      this.tabPageCloseToClose.Name = "tabPageCloseToClose";
      this.tabPageCloseToClose.Size = new System.Drawing.Size(656, 254);
      this.tabPageCloseToClose.TabIndex = 1;
      this.tabPageCloseToClose.Text = "Close To Close";
      this.tabPageCloseToClose.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPageCloseToClose_Paint);
      // 
      // closeToCloseUserControl
      // 
      this.closeToCloseUserControl.Location = new System.Drawing.Point(8, 8);
      this.closeToCloseUserControl.Name = "closeToCloseUserControl";
      this.closeToCloseUserControl.Size = new System.Drawing.Size(646, 244);
      this.closeToCloseUserControl.TabIndex = 0;
      // 
      // textBoxTicker
      // 
      this.textBoxTicker.Location = new System.Drawing.Point(288, 8);
      this.textBoxTicker.Name = "textBoxTicker";
      this.textBoxTicker.TabIndex = 1;
      this.textBoxTicker.Text = "CCE";
      // 
      // QuotesEditor
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(704, 365);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.textBoxTicker,
                                                                  this.tabControl1});
      this.Name = "QuotesEditor";
      this.Text = "QuotesEditor";
      this.tabControl1.ResumeLayout(false);
      this.tabPageChart.ResumeLayout(false);
      this.tabPageValidation.ResumeLayout(false);
      this.tabControl2.ResumeLayout(false);
      this.tabPageOHLC.ResumeLayout(false);
      this.tabPageCloseToClose.ResumeLayout(false);
      this.ResumeLayout(false);

    }
		#endregion


    #region tabPageChart
    private void tabPageChart_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
      this.quotesChart.Ticker = this.textBoxTicker.Text;
//      quotesChart.PaintingHandler( this.textBoxTicker.Text );
    }

    #endregion
    #region tabPageValidation
    private void tabPageValidation_Click(object sender, System.EventArgs e)
    {
    }

    private void tabPageValidation_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
      this.validateDataTable.Clear();
      this.validateDataTable.AddRows( this.textBoxTicker.Text , this.suspiciousRatio );
      this.openHighLowCloseUserControl.ValidateDataTable = this.validateDataTable;
      this.closeToCloseUserControl.ValidateDataTable = this.validateDataTable;
      this.closeToCloseUserControl.Ticker = this.textBoxTicker.Text;
    }

    #endregion
    #region tabPageOHLC
    private void tabPageOHLC_Click(object sender, System.EventArgs e)
    {
    
    }

    private void tabPageOHLC_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
//      this.openHighLowCloseDataGrid.DataBind( this.validateDataTable );
    }
    #endregion

    private void openHighLowCloseUserControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
      this.openHighLowCloseUserControl.PaintingHandler();
    }

    private void tabPageCloseToClose_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
      this.closeToCloseUserControl.PaintingHandler();
    }

	}
}
