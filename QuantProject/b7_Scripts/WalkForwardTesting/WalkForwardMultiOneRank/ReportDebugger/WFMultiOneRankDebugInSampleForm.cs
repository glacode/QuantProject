using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardMultiOneRank
{
	/// <summary>
	/// Form to easily test optimized genomes on different time windows
	/// (both in sample and out of sample)
	/// </summary>
	public class WFMultiOneRankDebugInSampleForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.DateTimePicker dateTimePickerFirstDate;
		private System.Windows.Forms.DateTimePicker dateTimePickerLastDate;
		private System.Windows.Forms.Button buttonGoTest;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private string[] signedTickers;
		private DateTime firstDateTime;
		private DateTime lastDateTime;
		private string benchmark;

		public WFMultiOneRankDebugInSampleForm(
			string[] signedTickers ,
			DateTime firstDateTime ,
			DateTime lastDateTime ,
			string benchmark )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.signedTickers = signedTickers;
			this.firstDateTime = firstDateTime;
			this.lastDateTime = lastDateTime;
			this.benchmark = benchmark;
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
			this.dateTimePickerFirstDate = new System.Windows.Forms.DateTimePicker();
			this.dateTimePickerLastDate = new System.Windows.Forms.DateTimePicker();
			this.buttonGoTest = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// dateTimePickerFirstDate
			// 
			this.dateTimePickerFirstDate.Location = new System.Drawing.Point(16, 32);
			this.dateTimePickerFirstDate.Name = "dateTimePickerFirstDate";
			this.dateTimePickerFirstDate.TabIndex = 0;
			// 
			// dateTimePickerLastDate
			// 
			this.dateTimePickerLastDate.Location = new System.Drawing.Point(248, 32);
			this.dateTimePickerLastDate.Name = "dateTimePickerLastDate";
			this.dateTimePickerLastDate.TabIndex = 1;
			// 
			// buttonGoTest
			// 
			this.buttonGoTest.Location = new System.Drawing.Point(200, 80);
			this.buttonGoTest.Name = "buttonGoTest";
			this.buttonGoTest.TabIndex = 2;
			this.buttonGoTest.Text = "Go Test";
			this.buttonGoTest.Click += new System.EventHandler(this.buttonGoTest_Click);
			// 
			// WFMultiOneRankDebugInSampleForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(472, 133);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.buttonGoTest,
																																	this.dateTimePickerLastDate,
																																	this.dateTimePickerFirstDate});
			this.Name = "WFMultiOneRankDebugInSampleForm";
			this.Text = "WFMultiOneRankDebugInSampleForm";
			this.Load += new System.EventHandler(this.WFMultiOneRankDebugInSampleForm_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonGoTest_Click(object sender, System.EventArgs e)
		{
			WFMultiOneRankDebugInSample wFMultiOneRankDebugInSample =
				new WFMultiOneRankDebugInSample(
				this.signedTickers ,
				this.dateTimePickerFirstDate.Value ,
				this.dateTimePickerLastDate.Value ,
				this.benchmark );
			wFMultiOneRankDebugInSample.Run();
		}

		private void WFMultiOneRankDebugInSampleForm_Load(object sender, System.EventArgs e)
		{
			this.dateTimePickerFirstDate.Value = this.firstDateTime;
			this.dateTimePickerLastDate.Value = this.lastDateTime;
		}
	}
}
