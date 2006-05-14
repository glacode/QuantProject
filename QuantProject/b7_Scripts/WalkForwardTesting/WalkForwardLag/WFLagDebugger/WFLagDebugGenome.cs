using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using QuantProject.Data.DataProviders.Caching;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger
{
	/// <summary>
	/// Form to select different parameters and debug
	/// a WFLag strategy genome
	/// </summary>
	public class WFLagDebugGenome : System.Windows.Forms.Form
	{
		DateTime transactionDateTime;
		WFLagLog wFLagLog;
		WFLagChosenPositions wFLagChosenPositions;

		private System.Windows.Forms.Button TestPreInSampleAndPost;
		private System.Windows.Forms.Button TestInSample;
		private System.Windows.Forms.Button TestPostSample;
		private System.Windows.Forms.TextBox PostSampleDays;
		private System.Windows.Forms.Button testPreSample;
		private System.Windows.Forms.TextBox PreSampleDays;
		private System.Windows.Forms.Label labelDrivingPositions;
		private System.Windows.Forms.Label labelPortfolioPositions;
		private System.Windows.Forms.Label labelDrivingPositionsContent;
		private System.Windows.Forms.Label labelPortfolioPositionsContent;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public WFLagDebugGenome( DateTime transactionDateTime ,
			WFLagLog wFLagLog )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.transactionDateTime = transactionDateTime;
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
			this.TestPreInSampleAndPost = new System.Windows.Forms.Button();
			this.TestInSample = new System.Windows.Forms.Button();
			this.TestPostSample = new System.Windows.Forms.Button();
			this.PostSampleDays = new System.Windows.Forms.TextBox();
			this.testPreSample = new System.Windows.Forms.Button();
			this.PreSampleDays = new System.Windows.Forms.TextBox();
			this.labelDrivingPositions = new System.Windows.Forms.Label();
			this.labelPortfolioPositions = new System.Windows.Forms.Label();
			this.labelDrivingPositionsContent = new System.Windows.Forms.Label();
			this.labelPortfolioPositionsContent = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// TestPreInSampleAndPost
			// 
			this.TestPreInSampleAndPost.Location = new System.Drawing.Point(32, 64);
			this.TestPreInSampleAndPost.Name = "TestPreInSampleAndPost";
			this.TestPreInSampleAndPost.Size = new System.Drawing.Size(160, 40);
			this.TestPreInSampleAndPost.TabIndex = 0;
			this.TestPreInSampleAndPost.Text = "Test pre, in sample and post";
			this.TestPreInSampleAndPost.Click += new System.EventHandler(this.TestPreInSampleAndPost_Click);
			// 
			// TestInSample
			// 
			this.TestInSample.Location = new System.Drawing.Point(32, 8);
			this.TestInSample.Name = "TestInSample";
			this.TestInSample.Size = new System.Drawing.Size(160, 32);
			this.TestInSample.TabIndex = 1;
			this.TestInSample.Text = "Test in Sample";
			this.TestInSample.Click += new System.EventHandler(this.TestInSample_Click);
			// 
			// TestPostSample
			// 
			this.TestPostSample.Location = new System.Drawing.Point(32, 144);
			this.TestPostSample.Name = "TestPostSample";
			this.TestPostSample.Size = new System.Drawing.Size(160, 32);
			this.TestPostSample.TabIndex = 2;
			this.TestPostSample.Text = "Test post sample";
			this.TestPostSample.Click += new System.EventHandler(this.TestPostSample_Click);
			// 
			// PostSampleDays
			// 
			this.PostSampleDays.Location = new System.Drawing.Point(256, 152);
			this.PostSampleDays.Name = "PostSampleDays";
			this.PostSampleDays.TabIndex = 3;
			this.PostSampleDays.Text = "30";
			// 
			// testPreSample
			// 
			this.testPreSample.Location = new System.Drawing.Point(32, 216);
			this.testPreSample.Name = "testPreSample";
			this.testPreSample.Size = new System.Drawing.Size(160, 32);
			this.testPreSample.TabIndex = 4;
			this.testPreSample.Text = "Test pre sample";
			this.testPreSample.Click += new System.EventHandler(this.testPreSample_Click);
			// 
			// PreSampleDays
			// 
			this.PreSampleDays.Location = new System.Drawing.Point(256, 224);
			this.PreSampleDays.Name = "PreSampleDays";
			this.PreSampleDays.TabIndex = 5;
			this.PreSampleDays.Text = "30";
			// 
			// labelDrivingPositions
			// 
			this.labelDrivingPositions.Location = new System.Drawing.Point(272, 16);
			this.labelDrivingPositions.Name = "labelDrivingPositions";
			this.labelDrivingPositions.TabIndex = 6;
			this.labelDrivingPositions.Text = "Driving Pos.";
			this.labelDrivingPositions.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPortfolioPositions
			// 
			this.labelPortfolioPositions.Location = new System.Drawing.Point(272, 64);
			this.labelPortfolioPositions.Name = "labelPortfolioPositions";
			this.labelPortfolioPositions.TabIndex = 7;
			this.labelPortfolioPositions.Text = "Portfolio Pos.";
			this.labelPortfolioPositions.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDrivingPositionsContent
			// 
			this.labelDrivingPositionsContent.Location = new System.Drawing.Point(384, 16);
			this.labelDrivingPositionsContent.Name = "labelDrivingPositionsContent";
			this.labelDrivingPositionsContent.TabIndex = 8;
			this.labelDrivingPositionsContent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPortfolioPositionsContent
			// 
			this.labelPortfolioPositionsContent.Location = new System.Drawing.Point(384, 64);
			this.labelPortfolioPositionsContent.Name = "labelPortfolioPositionsContent";
			this.labelPortfolioPositionsContent.TabIndex = 9;
			this.labelPortfolioPositionsContent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// WFLagDebugGenome
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(616, 365);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.labelPortfolioPositionsContent,
																																	this.labelDrivingPositionsContent,
																																	this.labelPortfolioPositions,
																																	this.labelDrivingPositions,
																																	this.PreSampleDays,
																																	this.testPreSample,
																																	this.PostSampleDays,
																																	this.TestPostSample,
																																	this.TestInSample,
																																	this.TestPreInSampleAndPost});
			this.Name = "WFLagDebugGenome";
			this.Text = "WFLagDebugGenome";
			this.Load += new System.EventHandler(this.WFLagDebugGenome_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private DateTime getInSampleLastDateTime()
		{
			DateTime inSampleLastDateTime =
				this.transactionDateTime.AddDays( - 1 );
			return inSampleLastDateTime;
		}
		private DateTime getPreSampleLastDateTime()
		{
			DateTime preSampleLastDateTime =
				this.getInSampleLastDateTime().AddDays( -this.wFLagLog.InSampleDays );
			return preSampleLastDateTime;
		}
		private int getPostSampleDays()
		{
			int postSampleDays =
				Convert.ToInt32(  this.PostSampleDays.Text );
			return postSampleDays;
		}
		private int getPreSampleDays()
		{
			int preSampleDays =
				Convert.ToInt32(  this.PreSampleDays.Text );
			return preSampleDays;
		}
		private void run( DateTime inSampleLastDateTime ,
			int preSampleDays , int inSampleDays , int postSampleDays )
		{
//			WFLagChosenPositions wFLagChosenPositions =
//				this.wFLagLog.GetChosenPositions(
//				this.transactionDateTime );
			WFLagDebugPositions wFLagDebugPositions =
				new WFLagDebugPositions( this.wFLagChosenPositions ,
				inSampleLastDateTime , preSampleDays ,
				inSampleDays , postSampleDays ,
				this.wFLagLog.Benchmark );
			wFLagDebugPositions.Run();
		}
		private void TestPreInSampleAndPost_Click(object sender, System.EventArgs e)
		{
			WFLagChosenPositions wFLagChosenPositions =
				this.wFLagLog.GetChosenPositions(
				this.transactionDateTime );
			WFLagDebugPositions wFLagDebugPositions =
				new WFLagDebugPositions( wFLagChosenPositions ,
				this.transactionDateTime.AddDays( - 1 ) , 30 ,
				this.wFLagLog.InSampleDays ,
				Convert.ToInt32(  this.PostSampleDays.Text ) ,
				this.wFLagLog.Benchmark );
			wFLagDebugPositions.Run();
		}

		private void TestInSample_Click(object sender, System.EventArgs e)
		{		
			WFLagChosenPositions wFLagChosenPositions =
				this.wFLagLog.GetChosenPositions(
				this.transactionDateTime );
			WFLagDebugPositions wFLagDebugPositions =
				new WFLagDebugPositions( wFLagChosenPositions ,
				this.transactionDateTime.AddDays( - 1 ) , 0 ,
				this.wFLagLog.InSampleDays , 0 ,
				this.wFLagLog.Benchmark );
			wFLagDebugPositions.Run();
		}
		private void TestPostSample_Click(object sender, System.EventArgs e)
		{
			this.run( this.getInSampleLastDateTime() ,
				0 , 0 , this.getPostSampleDays() );
		}

		private void testPreSample_Click(object sender, System.EventArgs e)
		{
			try
			{
				this.run( this.getPreSampleLastDateTime() ,
					0 , this.getPreSampleDays() , 0 );
			}
			catch( MissingQuoteException ex )
			{
				MessageBox.Show( "The pre sample backtest cannot be " +
					"performed, because there are missing quotes.\n" +
					ex.Message );
			}
		}

		private void WFLagDebugGenome_Load(object sender, System.EventArgs e)
		{
			this.wFLagChosenPositions =
				this.wFLagLog.GetChosenPositions(
				this.transactionDateTime );
			this.labelDrivingPositionsContent.Text =
				this.wFLagChosenPositions.DrivingPositions.KeyConcat;
			this.labelPortfolioPositionsContent.Text =
				this.wFLagChosenPositions.PortfolioPositions.KeyConcat;
		}
	}
}
