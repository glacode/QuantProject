/*
QuantProject - Quantitative Finance Library

TestDisplayer.cs
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
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Summary description for TestDisplayer.
	/// </summary>
	public class TestDisplayer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelDays;
		private System.Windows.Forms.RadioButton radioButtonFixedPeriodOscillator;
		private System.Windows.Forms.RadioButton radioButtonOpenToCloseDaily;
		private System.Windows.Forms.DateTimePicker dtpLastDate;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DateTimePicker dtpFirstDate;
		private System.Windows.Forms.TextBox textBoxDaysFPOscillatorAndRevOneRank;
		private System.Windows.Forms.RadioButton radioButtonOpenToCloseWeekly;
		private System.Windows.Forms.RadioButton radioButtonCloseToOpenDaily;
		private System.Windows.Forms.DataGrid dgBestGenomes;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		// Glauco code
		private ArrayList bestGenomes;
		private GenomeRepresentation lastSelectedGenomeRepresentation;
    private System.Windows.Forms.RadioButton radioButtonOTCCTODaily;
    private System.Windows.Forms.RadioButton radioButtonExtremeCounterTrend;
    private System.Windows.Forms.RadioButton radioButtonImmediateTrendFollower;
    private StrategyType selectedStrategyType = StrategyType.OpenToCloseDaily;

		private void testdisplayer()
		{
			this.dgBestGenomes.DataSource = this.bestGenomes;
      DataGridTableStyle ts = new DataGridTableStyle();
      ts.MappingName = this.dgBestGenomes.DataSource.GetType().Name;
      
      DataGridTextBoxColumn signedTickersBox = new DataGridTextBoxColumn();
      signedTickersBox.MappingName ="SignedTickers";
      signedTickersBox.HeaderText = "Tickers";
      ts.GridColumnStyles.Add(signedTickersBox);
      
      DataGridTextBoxColumn weights = new DataGridTextBoxColumn();
      weights.MappingName ="WeightsForSignedTickers";
      weights.HeaderText = "Weights";
      ts.GridColumnStyles.Add(weights);
      
      DataGridTextBoxColumn fitness = new DataGridTextBoxColumn();
      fitness.MappingName ="Fitness";
      fitness.HeaderText = "Fitness";
      ts.GridColumnStyles.Add(fitness);

      DataGridTextBoxColumn firstDate = new DataGridTextBoxColumn();
      firstDate.MappingName ="FirstOptimizationDate";
      firstDate.HeaderText = "FirstOptimizationDate";
      ts.GridColumnStyles.Add(firstDate);

      DataGridTextBoxColumn lastDate = new DataGridTextBoxColumn();
      lastDate.MappingName ="LastOptimizationDate";
      lastDate.HeaderText = "LastOptimizationDate";
      ts.GridColumnStyles.Add(lastDate);
      
      DataGridTextBoxColumn genNumber = new DataGridTextBoxColumn();
      genNumber.MappingName ="GenerationCounter";
      genNumber.HeaderText = "GenNum";
      ts.GridColumnStyles.Add(genNumber);
      
      DataGridTextBoxColumn eligible = new DataGridTextBoxColumn();
      eligible.MappingName ="EligibleTickers";
      eligible.HeaderText = "EligibleTickers";
      ts.GridColumnStyles.Add(eligible);
      
      this.dgBestGenomes.TableStyles.Clear();
      ts.AllowSorting = true;
      this.dgBestGenomes.TableStyles.Add(ts);
      this.dgBestGenomes.AllowSorting = true;
		}
		public TestDisplayer( DateTime firstDate , DateTime lastDate ,
			ArrayList bestGenomes )
		{
			if ( bestGenomes.Count == 0 )
				throw new Exception( "bestGenomes is empty! It should contain " +
					"a genome, at least." );
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Glauco code
			this.lastSelectedGenomeRepresentation =
				((GenomeRepresentation)bestGenomes[0]);
			this.dtpFirstDate.Value =
				this.lastSelectedGenomeRepresentation.FirstOptimizationDate;
			this.dtpLastDate.Value =
				this.lastSelectedGenomeRepresentation.LastOptimizationDate;
      this.bestGenomes = bestGenomes;
			this.testdisplayer();
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
		private void InitializeComponent() {
      this.dgBestGenomes = new System.Windows.Forms.DataGrid();
      this.radioButtonCloseToOpenDaily = new System.Windows.Forms.RadioButton();
      this.radioButtonOpenToCloseWeekly = new System.Windows.Forms.RadioButton();
      this.textBoxDaysFPOscillatorAndRevOneRank = new System.Windows.Forms.TextBox();
      this.dtpFirstDate = new System.Windows.Forms.DateTimePicker();
      this.label1 = new System.Windows.Forms.Label();
      this.dtpLastDate = new System.Windows.Forms.DateTimePicker();
      this.radioButtonExtremeCounterTrend = new System.Windows.Forms.RadioButton();
      this.radioButtonOpenToCloseDaily = new System.Windows.Forms.RadioButton();
      this.radioButtonFixedPeriodOscillator = new System.Windows.Forms.RadioButton();
      this.labelDays = new System.Windows.Forms.Label();
      this.radioButtonOTCCTODaily = new System.Windows.Forms.RadioButton();
      this.radioButtonImmediateTrendFollower = new System.Windows.Forms.RadioButton();
      ((System.ComponentModel.ISupportInitialize)(this.dgBestGenomes)).BeginInit();
      this.SuspendLayout();
      // 
      // dgBestGenomes
      // 
      this.dgBestGenomes.DataMember = "";
      this.dgBestGenomes.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.dgBestGenomes.HeaderForeColor = System.Drawing.SystemColors.ControlText;
      this.dgBestGenomes.Location = new System.Drawing.Point(0, 205);
      this.dgBestGenomes.Name = "dgBestGenomes";
      this.dgBestGenomes.Size = new System.Drawing.Size(584, 168);
      this.dgBestGenomes.TabIndex = 0;
      this.dgBestGenomes.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgBestGenomes_MouseUp);
      // 
      // radioButtonCloseToOpenDaily
      // 
      this.radioButtonCloseToOpenDaily.Location = new System.Drawing.Point(64, 120);
      this.radioButtonCloseToOpenDaily.Name = "radioButtonCloseToOpenDaily";
      this.radioButtonCloseToOpenDaily.Size = new System.Drawing.Size(144, 24);
      this.radioButtonCloseToOpenDaily.TabIndex = 6;
      this.radioButtonCloseToOpenDaily.Text = "Close To Open Daily";
      this.radioButtonCloseToOpenDaily.CheckedChanged += new System.EventHandler(this.radioButtonCloseToOpenDaily_CheckedChanged);
      // 
      // radioButtonOpenToCloseWeekly
      // 
      this.radioButtonOpenToCloseWeekly.Location = new System.Drawing.Point(64, 144);
      this.radioButtonOpenToCloseWeekly.Name = "radioButtonOpenToCloseWeekly";
      this.radioButtonOpenToCloseWeekly.Size = new System.Drawing.Size(144, 24);
      this.radioButtonOpenToCloseWeekly.TabIndex = 5;
      this.radioButtonOpenToCloseWeekly.Text = "Open To Close Weekly";
      this.radioButtonOpenToCloseWeekly.CheckedChanged += new System.EventHandler(this.radioButtonOpenToCloseWeekly_CheckedChanged);
      // 
      // textBoxDaysFPOscillatorAndRevOneRank
      // 
      this.textBoxDaysFPOscillatorAndRevOneRank.Location = new System.Drawing.Point(320, 176);
      this.textBoxDaysFPOscillatorAndRevOneRank.Name = "textBoxDaysFPOscillatorAndRevOneRank";
      this.textBoxDaysFPOscillatorAndRevOneRank.Size = new System.Drawing.Size(56, 20);
      this.textBoxDaysFPOscillatorAndRevOneRank.TabIndex = 8;
      this.textBoxDaysFPOscillatorAndRevOneRank.Text = "1";
      // 
      // dtpFirstDate
      // 
      this.dtpFirstDate.Location = new System.Drawing.Point(16, 24);
      this.dtpFirstDate.Name = "dtpFirstDate";
      this.dtpFirstDate.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(32, 64);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(400, 40);
      this.label1.TabIndex = 3;
      this.label1.Text = "Left click data grid rows to reset dates to the optimization period. Right click " +
        "to preserve date displacements and backtest.";
      // 
      // dtpLastDate
      // 
      this.dtpLastDate.Location = new System.Drawing.Point(264, 24);
      this.dtpLastDate.Name = "dtpLastDate";
      this.dtpLastDate.Size = new System.Drawing.Size(208, 20);
      this.dtpLastDate.TabIndex = 2;
      // 
      // radioButtonExtremeCounterTrend
      // 
      this.radioButtonExtremeCounterTrend.Location = new System.Drawing.Point(232, 120);
      this.radioButtonExtremeCounterTrend.Name = "radioButtonExtremeCounterTrend";
      this.radioButtonExtremeCounterTrend.Size = new System.Drawing.Size(192, 24);
      this.radioButtonExtremeCounterTrend.TabIndex = 10;
      this.radioButtonExtremeCounterTrend.Text = "Extreme counter trend";
      this.radioButtonExtremeCounterTrend.CheckedChanged += new System.EventHandler(this.radioButtonExtremeCounterTrend_CheckedChanged);
      // 
      // radioButtonOpenToCloseDaily
      // 
      this.radioButtonOpenToCloseDaily.Location = new System.Drawing.Point(64, 96);
      this.radioButtonOpenToCloseDaily.Name = "radioButtonOpenToCloseDaily";
      this.radioButtonOpenToCloseDaily.Size = new System.Drawing.Size(144, 24);
      this.radioButtonOpenToCloseDaily.TabIndex = 4;
      this.radioButtonOpenToCloseDaily.Text = "Open To Close Daily";
      this.radioButtonOpenToCloseDaily.CheckedChanged += new System.EventHandler(this.radioButtonOpenToCloseDaily_CheckedChanged);
      // 
      // radioButtonFixedPeriodOscillator
      // 
      this.radioButtonFixedPeriodOscillator.Location = new System.Drawing.Point(232, 96);
      this.radioButtonFixedPeriodOscillator.Name = "radioButtonFixedPeriodOscillator";
      this.radioButtonFixedPeriodOscillator.Size = new System.Drawing.Size(192, 24);
      this.radioButtonFixedPeriodOscillator.TabIndex = 7;
      this.radioButtonFixedPeriodOscillator.Text = "Fixed Period n-days oscillator";
      this.radioButtonFixedPeriodOscillator.CheckedChanged += new System.EventHandler(this.radioButtonFixedPeriodOscillator_CheckedChanged);
      // 
      // labelDays
      // 
      this.labelDays.Location = new System.Drawing.Point(272, 176);
      this.labelDays.Name = "labelDays";
      this.labelDays.Size = new System.Drawing.Size(40, 16);
      this.labelDays.TabIndex = 9;
      this.labelDays.Text = "days";
      // 
      // radioButtonOTCCTODaily
      // 
      this.radioButtonOTCCTODaily.Location = new System.Drawing.Point(64, 168);
      this.radioButtonOTCCTODaily.Name = "radioButtonOTCCTODaily";
      this.radioButtonOTCCTODaily.Size = new System.Drawing.Size(144, 24);
      this.radioButtonOTCCTODaily.TabIndex = 11;
      this.radioButtonOTCCTODaily.Text = "OTC - CTO Daily";
      // 
      // radioButtonImmediateTrendFollower
      // 
      this.radioButtonImmediateTrendFollower.Location = new System.Drawing.Point(232, 144);
      this.radioButtonImmediateTrendFollower.Name = "radioButtonImmediateTrendFollower";
      this.radioButtonImmediateTrendFollower.Size = new System.Drawing.Size(192, 24);
      this.radioButtonImmediateTrendFollower.TabIndex = 12;
      this.radioButtonImmediateTrendFollower.Text = "Immediate Trend Follower";
      this.radioButtonImmediateTrendFollower.CheckedChanged += new System.EventHandler(this.radioButtonImmediateTrendFollower_CheckedChanged);
      // 
      // TestDisplayer
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(584, 373);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.radioButtonImmediateTrendFollower,
                                                                  this.radioButtonOTCCTODaily,
                                                                  this.radioButtonExtremeCounterTrend,
                                                                  this.labelDays,
                                                                  this.textBoxDaysFPOscillatorAndRevOneRank,
                                                                  this.radioButtonFixedPeriodOscillator,
                                                                  this.radioButtonCloseToOpenDaily,
                                                                  this.radioButtonOpenToCloseWeekly,
                                                                  this.radioButtonOpenToCloseDaily,
                                                                  this.label1,
                                                                  this.dtpLastDate,
                                                                  this.dtpFirstDate,
                                                                  this.dgBestGenomes});
      this.Name = "TestDisplayer";
      this.Text = "TestDisplayer";
      ((System.ComponentModel.ISupportInitialize)(this.dgBestGenomes)).EndInit();
      this.ResumeLayout(false);

    }
		#endregion

		private bool aRowHasBeenClicked(
			object sender, System.Windows.Forms.MouseEventArgs e )
		{
			DataGrid dataGrid = (DataGrid)sender;
			Point point = new Point( e.X , e.Y );
			DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest( point );
			return hitTestInfo.Row>=0;
		}
		private GenomeRepresentation dgBestGenomes_MouseUp_getClickedGenomeRepresentation(
			object sender, System.Windows.Forms.MouseEventArgs e )
		{
			GenomeRepresentation genomeRepresentation = null;
			DataGrid dataGrid = (DataGrid)sender;
			Point point = new Point( e.X , e.Y );
			DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest( point );
			ArrayList bestGenomes = (ArrayList)dataGrid.DataSource;
			//			DataRow dataRow = dataTable.Rows[ hitTestInfo.Row ];
			if ( hitTestInfo.Row >= 0 )
				// a grid row has been clicked, not the header
				genomeRepresentation =
				(GenomeRepresentation)bestGenomes[ hitTestInfo.Row ];
			return genomeRepresentation;
		}
		private void dgBestGenomes_MouseUp_rightButton_updateDates(
			GenomeRepresentation newSelectedGenomeRepresentation )
		{
			TimeSpan currentFirstDateDisplacement =
				( this.dtpFirstDate.Value -
				this.lastSelectedGenomeRepresentation.FirstOptimizationDate );
			TimeSpan currentLastDateDisplacement =
				( this.dtpLastDate.Value -
				this.lastSelectedGenomeRepresentation.LastOptimizationDate );
			this.dtpFirstDate.Value = newSelectedGenomeRepresentation.FirstOptimizationDate +
				currentFirstDateDisplacement;
			this.dtpLastDate.Value = newSelectedGenomeRepresentation.LastOptimizationDate +
				currentLastDateDisplacement;
		}
		private void dgBestGenomes_MouseUp_rightButton(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			GenomeRepresentation genomeRepresentation =
				this.dgBestGenomes_MouseUp_getClickedGenomeRepresentation( sender , e );
			dgBestGenomes_MouseUp_rightButton_updateDates( genomeRepresentation );
			string[] signedTickers = genomeRepresentation.SignedTickers.Split(";".ToCharArray());
			LinearCombinationTest linearCombinationTest =
				new LinearCombinationTest( this.dtpFirstDate.Value ,
				this.dtpLastDate.Value , genomeRepresentation ,
				this.selectedStrategyType, Convert.ToInt32(this.textBoxDaysFPOscillatorAndRevOneRank.Text));
			linearCombinationTest.Run();
			this.lastSelectedGenomeRepresentation = genomeRepresentation;
		}
		private void dgBestGenomes_MouseUp_leftButton_updateDates(
			GenomeRepresentation newSelectedGenomeRepresentation )
		{
			this.dtpFirstDate.Value =
				newSelectedGenomeRepresentation.FirstOptimizationDate;
			this.dtpLastDate.Value =
				newSelectedGenomeRepresentation.LastOptimizationDate;
		}
		private void dgBestGenomes_MouseUp_leftButton(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( aRowHasBeenClicked( sender , e ) )
				// a grid row has been clicked, not the header
			{
				GenomeRepresentation newSelectedGenomeRepresentation =
					this.dgBestGenomes_MouseUp_getClickedGenomeRepresentation( sender , e );
				dgBestGenomes_MouseUp_leftButton_updateDates( newSelectedGenomeRepresentation );
				this.lastSelectedGenomeRepresentation = newSelectedGenomeRepresentation;
			}
		}
		private void dgBestGenomes_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( e.Button == MouseButtons.Right )
				this.dgBestGenomes_MouseUp_rightButton( sender , e );
			if ( e.Button == MouseButtons.Left )
				this.dgBestGenomes_MouseUp_leftButton( sender , e );
		}
    
    private void update_selectedStrategyType()
    {
      if(this.radioButtonOpenToCloseDaily.Checked)
        this.selectedStrategyType = StrategyType.OpenToCloseDaily;
      else if(this.radioButtonOpenToCloseWeekly.Checked)
        this.selectedStrategyType = StrategyType.OpenToCloseWeekly;
      else if(this.radioButtonCloseToOpenDaily.Checked)
        this.selectedStrategyType = StrategyType.CloseToOpenDaily;
      else if(this.radioButtonFixedPeriodOscillator.Checked)
        this.selectedStrategyType = StrategyType.FixedPeriodOscillator;
      else if(this.radioButtonExtremeCounterTrend.Checked)
        this.selectedStrategyType = StrategyType.ExtremeCounterTrend;
      else if(this.radioButtonOTCCTODaily.Checked)
        this.selectedStrategyType = StrategyType.OpenToCloseCloseToOpenDaily;
      else if(this.radioButtonImmediateTrendFollower.Checked)
        this.selectedStrategyType = StrategyType.ImmediateTrendFollower;
    }
    
    private void radioButtonOpenToCloseDaily_CheckedChanged(object sender, System.EventArgs e)
    {
      this.update_selectedStrategyType();
    }

    private void radioButtonOpenToCloseWeekly_CheckedChanged(object sender, System.EventArgs e)
    {
      this.update_selectedStrategyType();
    }

    private void radioButtonCloseToOpenDaily_CheckedChanged(object sender, System.EventArgs e)
    {
      this.update_selectedStrategyType();
    }

    private void radioButtonFixedPeriodOscillator_CheckedChanged(object sender, System.EventArgs e)
    {
      this.update_selectedStrategyType();
    }

    private void radioButtonExtremeCounterTrend_CheckedChanged(object sender, System.EventArgs e)
    {
      this.update_selectedStrategyType();
    }

    private void radioButtonImmediateTrendFollower_CheckedChanged(object sender, System.EventArgs e)
    {
      this.update_selectedStrategyType();
    }
    
	}
}
