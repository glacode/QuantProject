/*
QuantProject - Quantitative Finance Library

BackTestForm.cs
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
using QuantProject.ADT;
using QuantProject.ADT.Optimizing;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Testing;
using QuantProject.Scripts;

namespace QuantProject.Principale
{
	/// <summary>
	/// Summary description for BackTestForm.
	/// </summary>
	public class BackTestForm : System.Windows.Forms.Form
	{
    private System.Windows.Forms.Button BackTestStartButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public BackTestForm()
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
      this.BackTestStartButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // BackTestStartButton
      // 
      this.BackTestStartButton.Location = new System.Drawing.Point(72, 40);
      this.BackTestStartButton.Name = "BackTestStartButton";
      this.BackTestStartButton.Size = new System.Drawing.Size(120, 23);
      this.BackTestStartButton.TabIndex = 0;
      this.BackTestStartButton.Text = "Start Backtest";
      this.BackTestStartButton.Click += new System.EventHandler(this.BackTestStartButton_Click);
      // 
      // BackTestForm
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(292, 273);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.BackTestStartButton});
      this.Name = "BackTestForm";
      this.Text = "BackTestForm";
      this.ResumeLayout(false);

    }
		#endregion

    private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
    {
    
    }

    private void BackTestStartButton_Click(object sender, System.EventArgs e)
    {
      MyTradingSystem tradingSystem = new MyTradingSystem();
      tradingSystem.Parameters.Add( new Parameter( "SMAdays" , 2 , 4 , 0.5 ) );
      tradingSystem.Parameters.Add( new Parameter( "CrossPercentage" , 0 , 5 , 1 ) );

      WalkForwardTester walkForwardTester = new WalkForwardTester();
      walkForwardTester.StartDateTime = new DateTime( 1995 , 1 , 1 );
      walkForwardTester.EndDateTime = DateTime.Today;
      walkForwardTester.InSampleWindowNumDays = 600;
      walkForwardTester.OutOfSampleWindowNumDays = 30;
      walkForwardTester.Add( tradingSystem );
      walkForwardTester.Account.AddCash(
        new ExtendedDateTime( new DateTime( 1995 , 1 , 1 ) , BarComponent.Open ) , 10000 );
      walkForwardTester.Test();
      walkForwardTester.Account.DrawReport();
    }  
	}
}
