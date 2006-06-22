using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger
{
	/// <summary>
	/// Form to debug a WFLagLog: here the user can either chose to
	/// recreate the backtest final report or to analize each genome listed
	/// in a form grid
	/// </summary>
	public class WFLagLogDebugger : System.Windows.Forms.Form
	{
		private WFLagLog wFLagLog;

		private System.Windows.Forms.Button buttonDebugBacktest;
		private System.Windows.Forms.Button buttonDebugLog;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public WFLagLogDebugger( WFLagLog wFLagLog )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.wFLagLog = wFLagLog;
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
			this.buttonDebugBacktest = new System.Windows.Forms.Button();
			this.buttonDebugLog = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonDebugBacktest
			// 
			this.buttonDebugBacktest.Location = new System.Drawing.Point(32, 48);
			this.buttonDebugBacktest.Name = "buttonDebugBacktest";
			this.buttonDebugBacktest.Size = new System.Drawing.Size(104, 23);
			this.buttonDebugBacktest.TabIndex = 0;
			this.buttonDebugBacktest.Text = "Debug Backtest";
			this.buttonDebugBacktest.Click += new System.EventHandler(this.buttonDebugBacktest_Click);
			// 
			// buttonDebugLog
			// 
			this.buttonDebugLog.Location = new System.Drawing.Point(176, 48);
			this.buttonDebugLog.Name = "buttonDebugLog";
			this.buttonDebugLog.Size = new System.Drawing.Size(104, 23);
			this.buttonDebugLog.TabIndex = 1;
			this.buttonDebugLog.Text = "Debug Log";
			this.buttonDebugLog.Click += new System.EventHandler(this.buttonDebugLog_Click);
			// 
			// WFLagLogDebugger
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(328, 149);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.buttonDebugLog,
																																	this.buttonDebugBacktest});
			this.Name = "WFLagLogDebugger";
			this.Text = "WFLagLogDebugger";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonDebugBacktest_Click(object sender, System.EventArgs e)
		{
			new WFLagRunDebugger().Run( this.wFLagLog );
		}

		private void buttonDebugLog_Click(object sender, System.EventArgs e)
		{
			new WFLagRunGenomesDebugger().Run( this.wFLagLog );
		}
	}
}
