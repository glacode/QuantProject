/*
QuantProject - Quantitative Finance Library

ReportShower.cs
Copyright (C) 2003 
Marco Milletti

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
using QuantProject.Business.Timing;

namespace QuantProject.Presentation.Reporting.WindowsForm
{
	/// <summary>
	/// Descrizione di riepilogo per ReportShower.
	/// </summary>
	public class ReportShower : System.Windows.Forms.Form
	{
    private System.Windows.Forms.Button buttonShowReport;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.NumericUpDown numDaysForEquityLine;
    private System.Windows.Forms.DateTimePicker endingDate;
    private System.Windows.Forms.TextBox reportName;
    private Report report;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox benchmark;

		/// <summary>
		/// Variabile di progettazione necessaria.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ReportShower(Report report)
		{
			//
			// Necessario per il supporto di Progettazione Windows Form
			//
			InitializeComponent();

			//
			  this.report = report;
        this.endingDate.Value = this.report.GetLastTradeDate();
			//
		}

		/// <summary>
		/// Pulire le risorse in uso.
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
		/// Metodo necessario per il supporto della finestra di progettazione. Non modificare
		/// il contenuto del metodo con l'editor di codice.
		/// </summary>
		private void InitializeComponent()
		{
      this.numDaysForEquityLine = new System.Windows.Forms.NumericUpDown();
      this.endingDate = new System.Windows.Forms.DateTimePicker();
      this.reportName = new System.Windows.Forms.TextBox();
      this.buttonShowReport = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.benchmark = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.numDaysForEquityLine)).BeginInit();
      this.SuspendLayout();
      // 
      // numDaysForEquityLine
      // 
      this.numDaysForEquityLine.Location = new System.Drawing.Point(144, 64);
      this.numDaysForEquityLine.Minimum = new System.Decimal(new int[] {
                                                                         1,
                                                                         0,
                                                                         0,
                                                                         0});
      this.numDaysForEquityLine.Name = "numDaysForEquityLine";
      this.numDaysForEquityLine.Size = new System.Drawing.Size(64, 20);
      this.numDaysForEquityLine.TabIndex = 0;
      this.numDaysForEquityLine.Value = new System.Decimal(new int[] {
                                                                       1,
                                                                       0,
                                                                       0,
                                                                       0});
      // 
      // endingDate
      // 
      this.endingDate.Location = new System.Drawing.Point(112, 112);
      this.endingDate.Name = "endingDate";
      this.endingDate.Size = new System.Drawing.Size(136, 20);
      this.endingDate.TabIndex = 1;
      // 
      // reportName
      // 
      this.reportName.Location = new System.Drawing.Point(104, 16);
      this.reportName.Name = "reportName";
      this.reportName.Size = new System.Drawing.Size(176, 20);
      this.reportName.TabIndex = 2;
      this.reportName.Text = "My report";
      // 
      // buttonShowReport
      // 
      this.buttonShowReport.Location = new System.Drawing.Point(80, 208);
      this.buttonShowReport.Name = "buttonShowReport";
      this.buttonShowReport.Size = new System.Drawing.Size(128, 40);
      this.buttonShowReport.TabIndex = 3;
      this.buttonShowReport.Text = "Show";
      this.buttonShowReport.Click += new System.EventHandler(this.buttonShowReport_Click);
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(8, 16);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(96, 16);
      this.label1.TabIndex = 4;
      this.label1.Text = "Report Name";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(8, 64);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(128, 32);
      this.label2.TabIndex = 5;
      this.label2.Text = "Num Days for equity line";
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(8, 112);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(96, 16);
      this.label3.TabIndex = 6;
      this.label3.Text = "Ending Date";
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(10, 160);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(96, 16);
      this.label4.TabIndex = 8;
      this.label4.Text = "Benchmark";
      // 
      // benchmark
      // 
      this.benchmark.Location = new System.Drawing.Point(106, 160);
      this.benchmark.Name = "benchmark";
      this.benchmark.Size = new System.Drawing.Size(176, 20);
      this.benchmark.TabIndex = 7;
      this.benchmark.Text = "";
      // 
      // ReportShower
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(292, 266);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.label4,
                                                                  this.benchmark,
                                                                  this.label3,
                                                                  this.label2,
                                                                  this.label1,
                                                                  this.buttonShowReport,
                                                                  this.reportName,
                                                                  this.endingDate,
                                                                  this.numDaysForEquityLine});
      this.Name = "ReportShower";
      this.Text = "ReportShower";
      ((System.ComponentModel.ISupportInitialize)(this.numDaysForEquityLine)).EndInit();
      this.ResumeLayout(false);

    }
		#endregion

    private void buttonShowReport_Click(object sender, System.EventArgs e)
    {
      try
      {
        if(this.benchmark.Text == "")
          throw new Exception("Benchmark symbol is requested!");
        this.report.Clear();
        this.report.Show(this.reportName.Text, (int)this.numDaysForEquityLine.Value,
                          new EndOfDayDateTime(this.endingDate.Value,
                                              EndOfDaySpecificTime.MarketClose),
                        this.benchmark.Text);
      }
      
      catch(Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }                  
    }
	}
}
