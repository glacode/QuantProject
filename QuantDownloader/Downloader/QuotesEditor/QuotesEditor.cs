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
		private double suspiciousRatio = 3;

		private ValidateDataTable validateDataTable;
		private CloseToCloseTabPage closeToCloseTabPage = new CloseToCloseTabPage();
		private ValidationTabPage validationTabPage = new ValidationTabPage();
		private ValidationTabControl validationTabControl = new ValidationTabControl();

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPageChart;
		private System.Windows.Forms.TabPage tabPageQuotes;
		private System.Windows.Forms.TextBox textBoxTicker;
		private QuantProject.Applications.Downloader.QuotesChart quotesChart;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private void updateValidateDataTable()
		{
			if ( this.validateDataTable == null )
			{
				this.validateDataTable = new ValidateDataTable();
			}
			this.validateDataTable.Clear();
			this.validateDataTable.Rows.Clear();
			this.validateDataTable.AddRows( this.textBoxTicker.Text ,
				this.suspiciousRatio );
		}

		public ValidateDataTable ValidateDataTable
		{
			get
			{
				if ( this.validateDataTable == null )
				{
					this.updateValidateDataTable();
				}
				return this.validateDataTable;
			}
		}

	public string Ticker
{
	get { return this.textBoxTicker.Text; }
}
	public QuotesEditor( string ticker )
{
	//
	// Required for Windows Form Designer support
	//
	InitializeComponent();


	this.tabControl1.Width = this.Width - 20;
	this.tabControl1.Height = this.Height - 80;

	this.validationTabPage.Controls.Add( this.validationTabControl );
	this.tabControl1.Controls.Add( this.validationTabPage );

	this.textBoxTicker.Text = ticker;
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
		this.textBoxTicker = new System.Windows.Forms.TextBox();
		this.tabControl1.SuspendLayout();
		this.tabPageChart.SuspendLayout();
		this.SuspendLayout();
		// 
		// tabControl1
		// 
		this.tabControl1.Controls.AddRange(new System.Windows.Forms.Control[] {
																																						this.tabPageChart,
																																						this.tabPageQuotes});
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
		this.quotesChart.Ticker = null;
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
		// textBoxTicker
		// 
		this.textBoxTicker.Location = new System.Drawing.Point(288, 8);
		this.textBoxTicker.Name = "textBoxTicker";
		this.textBoxTicker.TabIndex = 1;
		this.textBoxTicker.Text = "CCE";
		this.textBoxTicker.Leave += new System.EventHandler(this.textBoxTicker_Leave);
		// 
		// QuotesEditor
		// 
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		this.ClientSize = new System.Drawing.Size(736, 373);
		this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																this.textBoxTicker,
																																this.tabControl1});
		this.Name = "QuotesEditor";
		this.Text = "QuotesEditor";
		this.tabControl1.ResumeLayout(false);
		this.tabPageChart.ResumeLayout(false);
		this.ResumeLayout(false);

	}
		#endregion


    #region tabPageChart
    private void tabPageChart_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
      this.quotesChart.Ticker = this.textBoxTicker.Text;
			this.quotesChart.Invalidate();
//      quotesChart.PaintingHandler( this.textBoxTicker.Text );
    }

    #endregion
    #region tabPageValidation
    private void tabPageValidation_Click(object sender, System.EventArgs e)
    {
      this.validateDataTable.Rows.Clear();
      this.validateDataTable.AddRows( this.textBoxTicker.Text , this.suspiciousRatio );
//      this.openHighLowCloseUserControl.ValidateDataTable = this.validateDataTable;
//      this.closeToCloseUserControl.Ticker = this.textBoxTicker.Text;
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
//      this.openHighLowCloseUserControl.PaintingHandler();
    }

    private void tabPageCloseToClose_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
    {
//      this.closeToCloseUserControl.PaintingHandler();
    }

    private void tabPageValidation_Enter(object sender, System.EventArgs e)
    {
      this.validateDataTable.Rows.Clear();
      this.validateDataTable.AddRows( this.textBoxTicker.Text , this.suspiciousRatio );
//      this.openHighLowCloseUserControl.ValidateDataTable = this.validateDataTable;
//      this.closeToCloseUserControl.Ticker = this.textBoxTicker.Text;
    }

    private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
    {
      if ( this.tabControl1.SelectedIndex == 2 )
      {
//        this.validateDataTable.Rows.Clear();
//        this.validateDataTable.AddRows( this.textBoxTicker.Text , this.suspiciousRatio );
//        this.openHighLowCloseUserControl.ValidateDataTable = this.validateDataTable;
//        this.closeToCloseUserControl.Ticker = this.textBoxTicker.Text;
      }    
    }

		private void textBoxTicker_Leave(object sender, System.EventArgs e)
		{
			this.tabPageChart.Invalidate();
			this.validationTabControl.Clear();
			this.updateValidateDataTable();
			this.validationTabControl.Rebuild();
		}

	}
}
