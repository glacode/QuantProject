/*
QuantProject - Quantitative Finance Library

WFLagMain.cs
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

using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// Main form to test the walk forward lag strategy: you
	/// can chose either to backtest the strategy or to
	/// debug a previous backtest
	/// </summary>
	public class WFLagMain : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button NewBacktest;
		private System.Windows.Forms.Button debugOldBacktest;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public WFLagMain()
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
			this.NewBacktest = new System.Windows.Forms.Button();
			this.debugOldBacktest = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// NewBacktest
			// 
			this.NewBacktest.Location = new System.Drawing.Point(64, 56);
			this.NewBacktest.Name = "NewBacktest";
			this.NewBacktest.Size = new System.Drawing.Size(88, 23);
			this.NewBacktest.TabIndex = 0;
			this.NewBacktest.Text = "New Backtest";
			this.NewBacktest.Click += new System.EventHandler(this.NewBacktest_Click);
			// 
			// debugOldBacktest
			// 
			this.debugOldBacktest.Location = new System.Drawing.Point(216, 56);
			this.debugOldBacktest.Name = "debugOldBacktest";
			this.debugOldBacktest.TabIndex = 1;
			this.debugOldBacktest.Text = "Debug Log";
			this.debugOldBacktest.Click += new System.EventHandler(this.debugOldBacktest_Click);
			// 
			// WFLagMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(352, 149);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.debugOldBacktest,
																																	this.NewBacktest});
			this.Name = "WFLagMain";
			this.Text = "WFLagMain";
			this.ResumeLayout(false);

		}
		#endregion

		private void NewBacktest_Click(object sender, System.EventArgs e)
		{
//			new RunWalkForwardLag( "millo" , 500 ,
//				3 , 3 , 90 , 7 , 15 , 30000 , "MSFT" ,
//				new DateTime( 2002 , 1 , 1 ) ,
//				new DateTime( 2002 , 3 , 31 ) ,
//				7 ).Run();
			new RunWalkForwardLag( "SP500" , 500 ,
				3 , 3 , 90 , 7 , 1 , 30000 , "MSFT" ,
				new DateTime( 2002 , 1 , 30 ) ,
				new DateTime( 2002 , 2 , 3 ) ,
				1 ).Run();
		}

		private void debugOldBacktest_Click(object sender, System.EventArgs e)
		{
			new WFLagRunDebugger().Run();
		}
	}
}
