using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using QuantProject.ADT;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardOneRank
{
	/// <summary>
	/// Summary description for ProgressBarWindow.
	/// </summary>
	public class ProgressBarForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ProgressBar progressBarInSample;
		private System.Windows.Forms.ProgressBar progressBarOutOfSample;
		private System.Windows.Forms.Label labelOutOfSample;
		private System.Windows.Forms.Label labelInSample;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProgressBar ProgressBarOutOfSample
		{
			get { return this.progressBarOutOfSample; }
			set { this.progressBarOutOfSample = value; }
		}
			
		public ProgressBar ProgressBarInSample
		{
			get { return this.progressBarInSample; }
			set { this.progressBarInSample = value; }
		}
			
		public ProgressBarForm( IWalkForwardProgressNotifier walkForwardProgressNotifier )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.progressBarOutOfSample.Maximum = 100;
			this.progressBarInSample.Maximum = 100;
			walkForwardProgressNotifier.InSampleNewProgress +=
				new NewProgressEventHandler( this.inSampleNewProgressEventHandler );
			walkForwardProgressNotifier.OutOfSampleNewProgress +=
				new NewProgressEventHandler( this.outOfSampleNewProgressEventHandler );
		}

		private void inSampleNewProgressEventHandler(
			object sender , NewProgressEventArgs newProgressEventArgs )
		{
			if ( this.InvokeRequired )
			{
				// we're not in the UI thread, so we need to call BeginInvoke
				this.BeginInvoke( new NewProgressEventHandler( this.inSampleNewProgressEventHandler ) ,
					new object[]{ sender , newProgressEventArgs });
			}
			else
				// we are in the UI thread
			{
				this.progressBarInSample.Value = newProgressEventArgs.CurrentProgress;
			}
		}
		private void outOfSampleNewProgressEventHandler(
			object sender , NewProgressEventArgs newProgressEventArgs )
		{
			if ( this.InvokeRequired )
			{
				// we're not in the UI thread, so we need to call BeginInvoke
				this.BeginInvoke( new NewProgressEventHandler( this.outOfSampleNewProgressEventHandler ) ,
					new object[]{ sender , newProgressEventArgs });
			}
			else
				// we are in the UI thread
			{
				this.progressBarOutOfSample.Value = newProgressEventArgs.CurrentProgress;
			}
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
			this.progressBarInSample = new System.Windows.Forms.ProgressBar();
			this.progressBarOutOfSample = new System.Windows.Forms.ProgressBar();
			this.labelOutOfSample = new System.Windows.Forms.Label();
			this.labelInSample = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// progressBarInSample
			// 
			this.progressBarInSample.Location = new System.Drawing.Point(120, 24);
			this.progressBarInSample.Name = "progressBarInSample";
			this.progressBarInSample.Size = new System.Drawing.Size(200, 16);
			this.progressBarInSample.TabIndex = 0;
			// 
			// progressBarOutOfSample
			// 
			this.progressBarOutOfSample.Location = new System.Drawing.Point(120, 72);
			this.progressBarOutOfSample.Name = "progressBarOutOfSample";
			this.progressBarOutOfSample.Size = new System.Drawing.Size(200, 16);
			this.progressBarOutOfSample.TabIndex = 1;
			// 
			// labelOutOfSample
			// 
			this.labelOutOfSample.Location = new System.Drawing.Point(32, 72);
			this.labelOutOfSample.Name = "labelOutOfSample";
			this.labelOutOfSample.Size = new System.Drawing.Size(80, 16);
			this.labelOutOfSample.TabIndex = 2;
			this.labelOutOfSample.Text = "Out of Sample:";
			this.labelOutOfSample.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelOutOfSample.Click += new System.EventHandler(this.label1_Click);
			// 
			// labelInSample
			// 
			this.labelInSample.Location = new System.Drawing.Point(40, 24);
			this.labelInSample.Name = "labelInSample";
			this.labelInSample.Size = new System.Drawing.Size(72, 16);
			this.labelInSample.TabIndex = 3;
			this.labelInSample.Text = "In Sample:";
			this.labelInSample.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ProgressBarWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(424, 117);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.labelInSample,
																																	this.labelOutOfSample,
																																	this.progressBarOutOfSample,
																																	this.progressBarInSample});
			this.Name = "ProgressBarWindow";
			this.Text = "ProgressBarWindow";
			this.ResumeLayout(false);

		}
		#endregion

		private void label1_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}
