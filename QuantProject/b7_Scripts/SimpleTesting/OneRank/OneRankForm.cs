using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using QuantProject.Business.DataProviders;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Presentation.Reporting.WindowsForm;

namespace QuantProject.Scripts.SimpleTesting
{
	/// <summary>
	/// Summary description for OneRankForm.
	/// </summary>
	public class OneRankForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox ticker;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.DateTimePicker firstDateTime;
		private System.Windows.Forms.DateTimePicker lastDateTime;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button start;
		private System.ComponentModel.IContainer components;

		private Account account;
		private HistoricalAdjustedQuoteProvider historicalQuoteProvider =
			new HistoricalAdjustedQuoteProvider();

		public string Ticker
		{
			set { this.ticker.Text = value; }
		}
		public DateTime FirstDateTime
		{
			set { this.firstDateTime.Value = value; }
		}
		public DateTime LastDateTime
		{
			set { this.lastDateTime.Value = value; }
		}

		public OneRankForm()
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
			this.components = new System.ComponentModel.Container();
			this.start = new System.Windows.Forms.Button();
			this.ticker = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.firstDateTime = new System.Windows.Forms.DateTimePicker();
			this.lastDateTime = new System.Windows.Forms.DateTimePicker();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// start
			// 
			this.start.Location = new System.Drawing.Point(160, 136);
			this.start.Name = "start";
			this.start.TabIndex = 0;
			this.start.Text = "button1";
			this.start.Click += new System.EventHandler(this.start_Click);
			// 
			// ticker
			// 
			this.ticker.Location = new System.Drawing.Point(120, 16);
			this.ticker.Name = "ticker";
			this.ticker.TabIndex = 1;
			this.ticker.Text = "";
			this.ticker.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.TabIndex = 2;
			this.label1.Text = "Ticker:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// firstDateTime
			// 
			this.firstDateTime.Location = new System.Drawing.Point(128, 56);
			this.firstDateTime.Name = "firstDateTime";
			this.firstDateTime.TabIndex = 3;
			// 
			// lastDateTime
			// 
			this.lastDateTime.Location = new System.Drawing.Point(128, 88);
			this.lastDateTime.Name = "lastDateTime";
			this.lastDateTime.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 56);
			this.label2.Name = "label2";
			this.label2.TabIndex = 5;
			this.label2.Text = "First Date:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 88);
			this.label3.Name = "label3";
			this.label3.TabIndex = 6;
			this.label3.Text = "Last Date:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// OneRankForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(424, 273);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.label3,
																																	this.label2,
																																	this.lastDateTime,
																																	this.firstDateTime,
																																	this.label1,
																																	this.ticker,
																																	this.start});
			this.Name = "OneRankForm";
			this.Text = "OneRankForm";
			this.ResumeLayout(false);

		}
		#endregion

		private void textBox1_TextChanged(object sender, System.EventArgs e)
		{
		
		}

		private void label1_Click(object sender, System.EventArgs e)
		{
		
		}

		#region start_Click
		private void start_Click_initializeAccount()
		{
			HistoricalEndOfDayTimer historicalEndOfDayTimer =
				new IndexBasedEndOfDayTimer(
				new EndOfDayDateTime( this.firstDateTime.Value ,
				EndOfDaySpecificTime.MarketOpen ) , this.ticker.Text );

			//			with IB commission
			//			this.account = new Account( "MSFT" , historicalEndOfDayTimer ,
			//				new HistoricalEndOfDayDataStreamer( historicalEndOfDayTimer ,
			//				this.historicalQuoteProvider ) ,
			//				new HistoricalEndOfDayOrderExecutor( historicalEndOfDayTimer ,
			//				this.historicalQuoteProvider ) ,
			//				new IBCommissionManager() );

			//			with no commission
			this.account = new Account( this.ticker.Text , historicalEndOfDayTimer ,
				new HistoricalEndOfDayDataStreamer( historicalEndOfDayTimer ,
				this.historicalQuoteProvider ) ,
				new HistoricalEndOfDayOrderExecutor( historicalEndOfDayTimer ,
				this.historicalQuoteProvider ) );
		}
		private void start_Click(object sender, System.EventArgs e)
		{
			start_Click_initializeAccount();
			OneRank oneRank = new OneRank( this.account , this.lastDateTime.Value );
			Report report = new Report( this.account , this.historicalQuoteProvider );
			report.Create( "WFT One Rank" , 1 ,
				new EndOfDayDateTime( this.lastDateTime.Value ,
				EndOfDaySpecificTime.MarketClose ) ,
				this.ticker.Text );
			report.Show();
		}
		#endregion
	}
}
