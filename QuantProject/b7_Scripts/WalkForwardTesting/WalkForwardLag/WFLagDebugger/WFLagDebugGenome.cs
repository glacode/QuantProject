/*
QuantProject - Quantitative Finance Library

WFLagDebugGenome.cs
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

using QuantProject.Data.DataProviders.Caching;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger
{
	/// <summary>
	/// Form to select different parameters and debug
	/// a WFLag strategy genome
	/// </summary>
	public class WFLagDebugGenome : System.Windows.Forms.Form
	{
		DateTime lastOptimizationDate;
		int inSampleDays;
		string benchmark;
		WFLagWeightedPositions wFLagWeightedPositions;
		
		private WFLagChosenPositionsDebugInfo wFLagChosenPositionsDebugInfo;

		public WFLagChosenPositionsDebugInfo WFLagChosenPositionsDebugInfo
		{
			set { this.wFLagChosenPositionsDebugInfo = value; }
			get { return this.wFLagChosenPositionsDebugInfo; }
		}

		private System.Windows.Forms.Button TestPreInSampleAndPost;
		private System.Windows.Forms.Button TestInSample;
		private System.Windows.Forms.Button TestPostSample;
		private System.Windows.Forms.TextBox PostSampleDays;
		private System.Windows.Forms.Button testPreSample;
		private System.Windows.Forms.TextBox PreSampleDays;
		private System.Windows.Forms.Label labelDrivingPositions;
		private System.Windows.Forms.Label labelPortfolioPositions;
		private System.Windows.Forms.DataGrid dataGridDrivingPositions;
		private System.Windows.Forms.DataGrid dataGridPortfolioPositions;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public WFLagDebugGenome( WFLagWeightedPositions wFLagWeightedPositions ,
		                        DateTime lastOptimizationDate ,
		                        int inSampleDays , string benchmark )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.lastOptimizationDate = lastOptimizationDate;
			this.inSampleDays = inSampleDays;
			this.benchmark = benchmark;
			this.wFLagWeightedPositions = wFLagWeightedPositions;
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
			this.dataGridDrivingPositions = new System.Windows.Forms.DataGrid();
			this.dataGridPortfolioPositions = new System.Windows.Forms.DataGrid();
			((System.ComponentModel.ISupportInitialize)(this.dataGridDrivingPositions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridPortfolioPositions)).BeginInit();
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
			this.TestPostSample.Location = new System.Drawing.Point(32, 304);
			this.TestPostSample.Name = "TestPostSample";
			this.TestPostSample.Size = new System.Drawing.Size(160, 32);
			this.TestPostSample.TabIndex = 2;
			this.TestPostSample.Text = "Test post sample";
			this.TestPostSample.Click += new System.EventHandler(this.TestPostSample_Click);
			// 
			// PostSampleDays
			// 
			this.PostSampleDays.Location = new System.Drawing.Point(256, 312);
			this.PostSampleDays.Name = "PostSampleDays";
			this.PostSampleDays.TabIndex = 3;
			this.PostSampleDays.Text = "30";
			// 
			// testPreSample
			// 
			this.testPreSample.Location = new System.Drawing.Point(32, 376);
			this.testPreSample.Name = "testPreSample";
			this.testPreSample.Size = new System.Drawing.Size(160, 32);
			this.testPreSample.TabIndex = 4;
			this.testPreSample.Text = "Test pre sample";
			this.testPreSample.Click += new System.EventHandler(this.testPreSample_Click);
			// 
			// PreSampleDays
			// 
			this.PreSampleDays.Location = new System.Drawing.Point(256, 384);
			this.PreSampleDays.Name = "PreSampleDays";
			this.PreSampleDays.TabIndex = 5;
			this.PreSampleDays.Text = "30";
			// 
			// labelDrivingPositions
			// 
			this.labelDrivingPositions.Location = new System.Drawing.Point(208, 16);
			this.labelDrivingPositions.Name = "labelDrivingPositions";
			this.labelDrivingPositions.Size = new System.Drawing.Size(72, 23);
			this.labelDrivingPositions.TabIndex = 6;
			this.labelDrivingPositions.Text = "Driving Pos.";
			this.labelDrivingPositions.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPortfolioPositions
			// 
			this.labelPortfolioPositions.Location = new System.Drawing.Point(208, 160);
			this.labelPortfolioPositions.Name = "labelPortfolioPositions";
			this.labelPortfolioPositions.Size = new System.Drawing.Size(72, 23);
			this.labelPortfolioPositions.TabIndex = 7;
			this.labelPortfolioPositions.Text = "Portfolio Pos.";
			this.labelPortfolioPositions.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// dataGridDrivingPositions
			// 
			this.dataGridDrivingPositions.DataMember = "";
			this.dataGridDrivingPositions.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGridDrivingPositions.Location = new System.Drawing.Point(296, 16);
			this.dataGridDrivingPositions.Name = "dataGridDrivingPositions";
			this.dataGridDrivingPositions.Size = new System.Drawing.Size(312, 128);
			this.dataGridDrivingPositions.TabIndex = 10;
			// 
			// dataGridPortfolioPositions
			// 
			this.dataGridPortfolioPositions.DataMember = "";
			this.dataGridPortfolioPositions.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGridPortfolioPositions.Location = new System.Drawing.Point(296, 160);
			this.dataGridPortfolioPositions.Name = "dataGridPortfolioPositions";
			this.dataGridPortfolioPositions.Size = new System.Drawing.Size(312, 128);
			this.dataGridPortfolioPositions.TabIndex = 11;
			// 
			// WFLagDebugGenome
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(616, 437);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
			                       	this.dataGridPortfolioPositions,
			                       	this.dataGridDrivingPositions,
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
			((System.ComponentModel.ISupportInitialize)(this.dataGridDrivingPositions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridPortfolioPositions)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

//		private DateTime getInSampleLastDateTime()
//		{
//			DateTime inSampleLastDateTime =
//				this.transactionDateTime.AddDays( - 1 );
//			return inSampleLastDateTime;
//		}
		private DateTime getPreSampleLastDateTime()
		{
			DateTime preSampleLastDateTime =
				this.lastOptimizationDate.AddDays(
					-this.inSampleDays );
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
		
//		private void setChosenPositions_actually()
//		{
//			if ( this.wFLagChosenPositionsDebugInfo != null )
//			{
//				// the form was given a valid WFLagChosenPositionsDebugInfo object
//				this.wFLagChosenPositions = this.wFLagChosenPositionsDebugInfo.GetChosenPositions();
//			}
//			else
//			{
//				// the form was given a valid WFLagLog object
//				this.wFLagChosenPositions = this.wflagl
//				}
//		}
//		private void setChosenPositions()
//		{
//			if ( this.wFLagChosenPositions == null )
//			{
//				this.setChosenPositions_actually();
//			}
//		}
		private void run( DateTime inSampleLastDateTime ,
		                 int preSampleDays , int inSampleDays , int postSampleDays )
		{
//			WFLagChosenPositions wFLagChosenPositions =
//				this.wFLagLog.GetChosenPositions(
//				this.transactionDateTime );
			WFLagDebugPositions wFLagDebugPositions =
				new WFLagDebugPositions( this.wFLagWeightedPositions ,
				                        inSampleLastDateTime , preSampleDays ,
				                        inSampleDays , postSampleDays ,
				                        this.benchmark );
			wFLagDebugPositions.Run();
		}
		private void TestPreInSampleAndPost_Click(object sender, System.EventArgs e)
		{
			WFLagDebugPositions wFLagDebugPositions =
				new WFLagDebugPositions( this.wFLagWeightedPositions ,
				                        this.lastOptimizationDate , 30 ,
				                        this.inSampleDays ,
				                        Convert.ToInt32(  this.PostSampleDays.Text ) ,
				                        this.benchmark );
			wFLagDebugPositions.Run();
		}

		private void TestInSample_Click(object sender, System.EventArgs e)
		{
			WFLagDebugPositions wFLagDebugPositions =
				new WFLagDebugPositions( this.wFLagWeightedPositions ,
				                        this.lastOptimizationDate , 0 ,
				                        this.inSampleDays , 0 ,
				                        this.benchmark );
			wFLagDebugPositions.Run();
		}
		private void TestPostSample_Click(object sender, System.EventArgs e)
		{
			this.run( this.lastOptimizationDate ,
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

		private void setDataGridDrivingPositions()
		{
			ArrayList drivingWeightedPositions =
				new ArrayList( this.wFLagWeightedPositions.DrivingWeightedPositions );
			this.dataGridDrivingPositions.DataSource = drivingWeightedPositions;
		}
		private void setDataGridPortfolioPositions()
		{
			ArrayList portfolioWeightedPositions =
				new ArrayList( this.wFLagWeightedPositions.PortfolioWeightedPositions );
			this.dataGridPortfolioPositions.DataSource = portfolioWeightedPositions;
		}
		private void WFLagDebugGenome_Load(object sender, System.EventArgs e)
		{
//			this.labelDrivingPositionsContent.Text =
//				this.wFLagChosenPositions.DrivingPositions.KeyConcat;
//			this.labelPortfolioPositionsContent.Text =
//				this.wFLagChosenPositions.PortfolioPositions.KeyConcat;
			this.setDataGridDrivingPositions();
			this.setDataGridPortfolioPositions();
		}
	}
}
