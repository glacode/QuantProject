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

using QuantProject.Business.Strategies;
using QuantProject.Presentation;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WeightedPositionsChoosers;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WeightedPositionsChoosers.Decoding;
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
			this.NewBacktest.Location = new System.Drawing.Point(24, 56);
			this.NewBacktest.Name = "NewBacktest";
			this.NewBacktest.Size = new System.Drawing.Size(88, 23);
			this.NewBacktest.TabIndex = 0;
			this.NewBacktest.Text = "New Backtest";
			this.NewBacktest.Click += new System.EventHandler(this.NewBacktest_Click);
			// 
			// debugOldBacktest
			// 
			this.debugOldBacktest.Location = new System.Drawing.Point(200, 56);
			this.debugOldBacktest.Name = "debugOldBacktest";
			this.debugOldBacktest.TabIndex = 1;
			this.debugOldBacktest.Text = "Debug Log";
			this.debugOldBacktest.Click += new System.EventHandler(this.debugOldBacktest_Click);
			// 
			// WFLagMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(312, 141);
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
//			new RunWalkForwardLag( "ib_etf" , 500 ,
//				4 , 2 , 200 , 3 , 4500 , 1000 , "EWQ" ,
//				new DateTime( 2003 , 8 , 13 ) ,
//				new DateTime( 2003 , 12 , 31 ) ,
//				8 ).Run();
			//			new RunWalkForwardLag( "millo" , 500 ,
//				3 , 2 , 90 , 20 , 3 , 5000 , "MSFT" ,
//				new DateTime( 2003 , 1 , 1 ) ,
//				new DateTime( 2003 , 1 , 8 ) ,
//				1 ).Run();

			// fixed portfolio
//			new RunWalkForwardLag( "DrvPstns" , 500 ,
//				4 , 4 , 100 , 13 , 9 , 100 , "EWQ" ,
//				new DateTime( 2001 , 1 , 1 ) ,
//				new DateTime( 2003 , 12 , 31 ) ,
//				7 ).Run();
			// XLF vs SMH
			// QQQQ vs SPY
			IWFLagWeightedPositionsChooser wFLagWeightedPositionsChooser =
//				new WFLagBruteForceFixedPortfolioWeightedPositionsChooser(
//				4 , new string[]{ "IWM" , "-SPY" } , 100 , "EWQ" ,
//				new QuantProject.Business.Strategies.EquityEvaluation.WinningPeriods() );
//			wFLagWeightedPositionsChooser =
				new WFLagGeneticFixedPortfolioWithNormalDrivingAndPortfolio(
				4 , new SignedTickers( "IWM;-SPY" ) , 100 , "EWQ" ,
				new QuantProject.Business.Strategies.EquityEvaluation.WinningPeriods() ,
				10000 , 30 );
//			wFLagWeightedPositionsChooser =
//				new WFLagGeneticFixedPortfolioWithNormalDrivingAndPortfolio(
//				4 , new SignedTickers( "IWM;-SPY" ) , 100 , "EWQ" ,
//				new QuantProject.Business.Strategies.EquityEvaluation.SharpeRatio() ,
//				10000 , 30 );
			wFLagWeightedPositionsChooser =
				new WFLagBruteForceWeightedPositionsChooserForBalancedFixedPortfolioAndBalancedDriving(
				3 , new SignedTickers( "IWM;-SPY" ) , 100 , "EWQ" ,
				new QuantProject.Business.Strategies.EquityEvaluation.SharpeRatio() );
//			new RunWalkForwardLag( "DrvPstns" , 200 ,
//				wFLagWeightedPositionsChooser ,	7 ,
//				new DateTime( 2001 , 1 , 1 ) ,
//				new DateTime( 2001 , 1 , 8 ) ,
//				0.5 ).Run();
			new RunWalkForwardLag( "DrvPstns" , 200 ,
				wFLagWeightedPositionsChooser ,	7 ,
				new DateTime( 2001 , 1 , 4 ) ,
				new DateTime( 2001 , 1 , 9 ) ,
				0.2 ).Run();

//			new RunWalkForwardLag( "DrvPstns" , 500 ,
//				4 , 2 , 365 , 13 , 9 , 100 , "EWQ" ,
//				new DateTime( 2002 , 1 , 1 ) ,
//				new DateTime( 2003 , 12 , 31 ) ,
//				new QuantProject.Business.Strategies.EquityEvaluation.WinningPeriods() ,
//				13 ).Run();
//
			//			new RunWalkForwardLag( "ib_etf" , 500 ,
//				4 , 4 , 250 , 2 , 15 , 30000 , "EWQ" ,
//				new DateTime( 2003 , 1 , 1 ) ,
//				new DateTime( 2003 , 12 , 31 ) ,
//				13 ).Run();
//						new RunWalkForwardLag( "SP500" , 500 ,
//							4 , 4 , 400 , 2 , 15 , 30000 , "MSFT" ,
//							new DateTime( 2002 , 6 , 30 ) ,
//							new DateTime( 2003 , 6 , 30 ) ,
//							8 ).Run();
//						new RunWalkForwardLag( "SP500" , 500 ,
//							1 , 1 , 400 , 2 , 5 , 5000 , "MSFT" ,
//							new DateTime( 2002 , 6 , 30 ) ,
//							new DateTime( 2003 , 6 , 30 ) ,
//							3 ).Run();
		}

		private void debugOldBacktest_Click(object sender, System.EventArgs e)
		{
			VisualObjectArchiver visualObjectArchiver =
				new VisualObjectArchiver();
			WFLagLog wFLagLog =
				( WFLagLog )visualObjectArchiver.Load(
				"Load WFLag backtest log" , "qPWFLagLog" , "Load transactions" );
			WFLagLogDebugger wFLagLogDebugger = new WFLagLogDebugger( wFLagLog );
			wFLagLogDebugger.ShowDialog();
		}

		private void debugOldGenomes_Click(object sender, System.EventArgs e)
		{
//			new WFLagGenomesDebugger.TestPerDeserializzazioneInAltraClasse();
//			new WFLagRunGenomesDebugger().Run();
		}
	}
}
