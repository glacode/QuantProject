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
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

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
		// Millo code: lastSelectedGenomeRepresentation has been
		// changed into an array
		private ArrayList bestGenomes;
		private GenomeRepresentation[] lastSelectedGenomeRepresentations;
    private System.Windows.Forms.RadioButton radioButtonOTCCTODaily;
    private System.Windows.Forms.RadioButton radioButtonExtremeCounterTrend;
    private System.Windows.Forms.RadioButton radioButtonImmediateTrendFollower;
    private StrategyType selectedStrategyType = StrategyType.OpenToCloseDaily;
    private System.Windows.Forms.Label labelPortfolioType;
    private System.Windows.Forms.RadioButton radioButtonPVO;
    private System.Windows.Forms.RadioButton radioButtonPVOBiased;
    private System.Windows.Forms.Label labelStopLoss;
    private System.Windows.Forms.TextBox textBoxStopLoss;
    private System.Windows.Forms.Label labelTakeProfit;
    private System.Windows.Forms.TextBox textBoxTakeProfit;
    private System.Windows.Forms.RadioButton radioButtonPVOBiasedNoThresholds;
    private System.Windows.Forms.RadioButton radioButtonOTCPVOBiasedNoThresholds;
    private System.Windows.Forms.ComboBox comboBoxPortfolioType;
    
		private void testDisplayer()
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
      
      DataGridTextBoxColumn createdGen = new DataGridTextBoxColumn();
      createdGen.MappingName ="CreatedGenerations";
      createdGen.HeaderText = "TotGen";
      ts.GridColumnStyles.Add(createdGen);
      
      DataGridTextBoxColumn eligible = new DataGridTextBoxColumn();
      eligible.MappingName ="EligibleTickers";
      eligible.HeaderText = "EligibleTickers";
      ts.GridColumnStyles.Add(eligible);
      
      DataGridTextBoxColumn oversoldThreshold = new DataGridTextBoxColumn();
      oversoldThreshold.MappingName ="OversoldThreshold";
      oversoldThreshold.HeaderText = "OversoldThreshold";
      ts.GridColumnStyles.Add(oversoldThreshold);

      DataGridTextBoxColumn overboughtThreshold = new DataGridTextBoxColumn();
      overboughtThreshold.MappingName ="OverboughtThreshold";
      overboughtThreshold.HeaderText = "OverboughtThreshold";
      ts.GridColumnStyles.Add(overboughtThreshold);

      this.dgBestGenomes.TableStyles.Clear();
      ts.AllowSorting = true;
      this.dgBestGenomes.TableStyles.Add(ts);
      this.dgBestGenomes.AllowSorting = true;
		}
		
		private void testDisplayer_InitializeLastSelectedGenomeRepresentations(ArrayList bestGenomes)
		{
			//genomes with the same optimization's dates are grouped together
			GenomeRepresentation firstGenomeRepresentation =
				        ((GenomeRepresentation)bestGenomes[0]);
			DateTime firstDate = firstGenomeRepresentation.FirstOptimizationDate;
			int counterOfGenomesWithSameOptimizationDates = 0;
			foreach(Object item in bestGenomes)
				if(firstDate == ((GenomeRepresentation)item).FirstOptimizationDate)
						counterOfGenomesWithSameOptimizationDates++;
			this.lastSelectedGenomeRepresentations = new GenomeRepresentation[counterOfGenomesWithSameOptimizationDates];
			this.lastSelectedGenomeRepresentations[0]=
								((GenomeRepresentation)bestGenomes[0]);
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

			// Glauco code + Millo code
			this.testDisplayer_InitializeLastSelectedGenomeRepresentations(bestGenomes);
			this.dtpFirstDate.Value =
				this.lastSelectedGenomeRepresentations[0].FirstOptimizationDate;
			this.dtpLastDate.Value =
				this.lastSelectedGenomeRepresentations[0].LastOptimizationDate;
      this.bestGenomes = bestGenomes;
      this.comboBoxPortfolioType.Items.Add(PortfolioType.ShortAndLong);
      this.comboBoxPortfolioType.Items.Add(PortfolioType.OnlyLong);
      this.comboBoxPortfolioType.Items.Add(PortfolioType.OnlyShort);
      this.comboBoxPortfolioType.SelectedItem = PortfolioType.ShortAndLong;
      this.comboBoxPortfolioType.Text = this.comboBoxPortfolioType.SelectedItem.ToString();
			this.comboBoxPortfolioType.Visible = false;
      this.labelPortfolioType.Visible = false;
      this.testDisplayer();
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
      this.comboBoxPortfolioType = new System.Windows.Forms.ComboBox();
      this.labelPortfolioType = new System.Windows.Forms.Label();
      this.radioButtonPVO = new System.Windows.Forms.RadioButton();
      this.radioButtonPVOBiased = new System.Windows.Forms.RadioButton();
      this.labelStopLoss = new System.Windows.Forms.Label();
      this.textBoxStopLoss = new System.Windows.Forms.TextBox();
      this.labelTakeProfit = new System.Windows.Forms.Label();
      this.textBoxTakeProfit = new System.Windows.Forms.TextBox();
      this.radioButtonPVOBiasedNoThresholds = new System.Windows.Forms.RadioButton();
      this.radioButtonOTCPVOBiasedNoThresholds = new System.Windows.Forms.RadioButton();
      ((System.ComponentModel.ISupportInitialize)(this.dgBestGenomes)).BeginInit();
      this.SuspendLayout();
      // 
      // dgBestGenomes
      // 
      this.dgBestGenomes.DataMember = "";
      this.dgBestGenomes.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.dgBestGenomes.HeaderForeColor = System.Drawing.SystemColors.ControlText;
      this.dgBestGenomes.Location = new System.Drawing.Point(0, 238);
      this.dgBestGenomes.Name = "dgBestGenomes";
      this.dgBestGenomes.Size = new System.Drawing.Size(704, 176);
      this.dgBestGenomes.TabIndex = 0;
      this.dgBestGenomes.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgBestGenomes_MouseUp);
      // 
      // radioButtonCloseToOpenDaily
      // 
      this.radioButtonCloseToOpenDaily.Location = new System.Drawing.Point(64, 104);
      this.radioButtonCloseToOpenDaily.Name = "radioButtonCloseToOpenDaily";
      this.radioButtonCloseToOpenDaily.Size = new System.Drawing.Size(144, 24);
      this.radioButtonCloseToOpenDaily.TabIndex = 6;
      this.radioButtonCloseToOpenDaily.Text = "Close To Open Daily";
      this.radioButtonCloseToOpenDaily.CheckedChanged += new System.EventHandler(this.radioButtonCloseToOpenDaily_CheckedChanged);
      // 
      // radioButtonOpenToCloseWeekly
      // 
      this.radioButtonOpenToCloseWeekly.Location = new System.Drawing.Point(64, 128);
      this.radioButtonOpenToCloseWeekly.Name = "radioButtonOpenToCloseWeekly";
      this.radioButtonOpenToCloseWeekly.Size = new System.Drawing.Size(144, 24);
      this.radioButtonOpenToCloseWeekly.TabIndex = 5;
      this.radioButtonOpenToCloseWeekly.Text = "Open To Close Weekly";
      this.radioButtonOpenToCloseWeekly.CheckedChanged += new System.EventHandler(this.radioButtonOpenToCloseWeekly_CheckedChanged);
      // 
      // textBoxDaysFPOscillatorAndRevOneRank
      // 
      this.textBoxDaysFPOscillatorAndRevOneRank.Location = new System.Drawing.Point(480, 120);
      this.textBoxDaysFPOscillatorAndRevOneRank.Name = "textBoxDaysFPOscillatorAndRevOneRank";
      this.textBoxDaysFPOscillatorAndRevOneRank.Size = new System.Drawing.Size(56, 20);
      this.textBoxDaysFPOscillatorAndRevOneRank.TabIndex = 8;
      this.textBoxDaysFPOscillatorAndRevOneRank.Text = "1";
      // 
      // dtpFirstDate
      // 
      this.dtpFirstDate.Location = new System.Drawing.Point(48, 8);
      this.dtpFirstDate.Name = "dtpFirstDate";
      this.dtpFirstDate.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(32, 48);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(400, 40);
      this.label1.TabIndex = 3;
      this.label1.Text = "Left click data grid rows to reset dates to the optimization period. Right click " +
        "to preserve date displacements and backtest.";
      // 
      // dtpLastDate
      // 
      this.dtpLastDate.Location = new System.Drawing.Point(264, 8);
      this.dtpLastDate.Name = "dtpLastDate";
      this.dtpLastDate.Size = new System.Drawing.Size(208, 20);
      this.dtpLastDate.TabIndex = 2;
      // 
      // radioButtonExtremeCounterTrend
      // 
      this.radioButtonExtremeCounterTrend.Location = new System.Drawing.Point(64, 176);
      this.radioButtonExtremeCounterTrend.Name = "radioButtonExtremeCounterTrend";
      this.radioButtonExtremeCounterTrend.Size = new System.Drawing.Size(144, 24);
      this.radioButtonExtremeCounterTrend.TabIndex = 10;
      this.radioButtonExtremeCounterTrend.Text = "Extreme counter trend";
      this.radioButtonExtremeCounterTrend.CheckedChanged += new System.EventHandler(this.radioButtonExtremeCounterTrend_CheckedChanged);
      // 
      // radioButtonOpenToCloseDaily
      // 
      this.radioButtonOpenToCloseDaily.Location = new System.Drawing.Point(64, 80);
      this.radioButtonOpenToCloseDaily.Name = "radioButtonOpenToCloseDaily";
      this.radioButtonOpenToCloseDaily.Size = new System.Drawing.Size(144, 24);
      this.radioButtonOpenToCloseDaily.TabIndex = 4;
      this.radioButtonOpenToCloseDaily.Text = "Open To Close Daily";
      this.radioButtonOpenToCloseDaily.CheckedChanged += new System.EventHandler(this.radioButtonOpenToCloseDaily_CheckedChanged);
      // 
      // radioButtonFixedPeriodOscillator
      // 
      this.radioButtonFixedPeriodOscillator.Location = new System.Drawing.Point(232, 80);
      this.radioButtonFixedPeriodOscillator.Name = "radioButtonFixedPeriodOscillator";
      this.radioButtonFixedPeriodOscillator.Size = new System.Drawing.Size(184, 24);
      this.radioButtonFixedPeriodOscillator.TabIndex = 7;
      this.radioButtonFixedPeriodOscillator.Text = "Fixed Period n-days oscillator";
      this.radioButtonFixedPeriodOscillator.CheckedChanged += new System.EventHandler(this.radioButtonFixedPeriodOscillator_CheckedChanged);
      // 
      // labelDays
      // 
      this.labelDays.Location = new System.Drawing.Point(424, 120);
      this.labelDays.Name = "labelDays";
      this.labelDays.Size = new System.Drawing.Size(32, 16);
      this.labelDays.TabIndex = 9;
      this.labelDays.Text = "days";
      // 
      // radioButtonOTCCTODaily
      // 
      this.radioButtonOTCCTODaily.Location = new System.Drawing.Point(64, 152);
      this.radioButtonOTCCTODaily.Name = "radioButtonOTCCTODaily";
      this.radioButtonOTCCTODaily.Size = new System.Drawing.Size(144, 24);
      this.radioButtonOTCCTODaily.TabIndex = 11;
      this.radioButtonOTCCTODaily.Text = "OTC - CTO Daily";
      // 
      // radioButtonImmediateTrendFollower
      // 
      this.radioButtonImmediateTrendFollower.Location = new System.Drawing.Point(232, 104);
      this.radioButtonImmediateTrendFollower.Name = "radioButtonImmediateTrendFollower";
      this.radioButtonImmediateTrendFollower.Size = new System.Drawing.Size(192, 24);
      this.radioButtonImmediateTrendFollower.TabIndex = 12;
      this.radioButtonImmediateTrendFollower.Text = "Immediate Trend Follower";
      this.radioButtonImmediateTrendFollower.CheckedChanged += new System.EventHandler(this.radioButtonImmediateTrendFollower_CheckedChanged);
      // 
      // comboBoxPortfolioType
      // 
      this.comboBoxPortfolioType.Location = new System.Drawing.Point(472, 64);
      this.comboBoxPortfolioType.Name = "comboBoxPortfolioType";
      this.comboBoxPortfolioType.Size = new System.Drawing.Size(184, 21);
      this.comboBoxPortfolioType.TabIndex = 13;
      this.comboBoxPortfolioType.Text = "comboBoxPortfolioType";
      // 
      // labelPortfolioType
      // 
      this.labelPortfolioType.Location = new System.Drawing.Point(472, 48);
      this.labelPortfolioType.Name = "labelPortfolioType";
      this.labelPortfolioType.Size = new System.Drawing.Size(100, 16);
      this.labelPortfolioType.TabIndex = 14;
      this.labelPortfolioType.Text = "Type of portfolio";
      // 
      // radioButtonPVO
      // 
      this.radioButtonPVO.Location = new System.Drawing.Point(232, 128);
      this.radioButtonPVO.Name = "radioButtonPVO";
      this.radioButtonPVO.Size = new System.Drawing.Size(192, 24);
      this.radioButtonPVO.TabIndex = 15;
      this.radioButtonPVO.Text = "Portfolio Value Oscillator";
      this.radioButtonPVO.CheckedChanged += new System.EventHandler(this.radioButtonPVO_CheckedChanged);
      // 
      // radioButtonPVOBiased
      // 
      this.radioButtonPVOBiased.Location = new System.Drawing.Point(232, 152);
      this.radioButtonPVOBiased.Name = "radioButtonPVOBiased";
      this.radioButtonPVOBiased.Size = new System.Drawing.Size(88, 24);
      this.radioButtonPVOBiased.TabIndex = 16;
      this.radioButtonPVOBiased.Text = "PVO Biased";
      this.radioButtonPVOBiased.CheckedChanged += new System.EventHandler(this.radioButtonPVOBiased_CheckedChanged);
      // 
      // labelStopLoss
      // 
      this.labelStopLoss.Location = new System.Drawing.Point(424, 160);
      this.labelStopLoss.Name = "labelStopLoss";
      this.labelStopLoss.Size = new System.Drawing.Size(56, 16);
      this.labelStopLoss.TabIndex = 18;
      this.labelStopLoss.Text = "stop loss";
      // 
      // textBoxStopLoss
      // 
      this.textBoxStopLoss.Location = new System.Drawing.Point(480, 160);
      this.textBoxStopLoss.Name = "textBoxStopLoss";
      this.textBoxStopLoss.Size = new System.Drawing.Size(56, 20);
      this.textBoxStopLoss.TabIndex = 17;
      this.textBoxStopLoss.Text = "0.02";
      // 
      // labelTakeProfit
      // 
      this.labelTakeProfit.Location = new System.Drawing.Point(544, 160);
      this.labelTakeProfit.Name = "labelTakeProfit";
      this.labelTakeProfit.Size = new System.Drawing.Size(56, 16);
      this.labelTakeProfit.TabIndex = 20;
      this.labelTakeProfit.Text = "take profit";
      // 
      // textBoxTakeProfit
      // 
      this.textBoxTakeProfit.Location = new System.Drawing.Point(608, 160);
      this.textBoxTakeProfit.Name = "textBoxTakeProfit";
      this.textBoxTakeProfit.Size = new System.Drawing.Size(56, 20);
      this.textBoxTakeProfit.TabIndex = 19;
      this.textBoxTakeProfit.Text = "0.005";
      // 
      // radioButtonPVOBiasedNoThresholds
      // 
      this.radioButtonPVOBiasedNoThresholds.Location = new System.Drawing.Point(232, 176);
      this.radioButtonPVOBiasedNoThresholds.Name = "radioButtonPVOBiasedNoThresholds";
      this.radioButtonPVOBiasedNoThresholds.Size = new System.Drawing.Size(192, 24);
      this.radioButtonPVOBiasedNoThresholds.TabIndex = 21;
      this.radioButtonPVOBiasedNoThresholds.Text = "PVO Biased No Thresholds";
      this.radioButtonPVOBiasedNoThresholds.CheckedChanged += new System.EventHandler(this.radioButtonPVOBiasedNoThresholds_CheckedChanged);
      // 
      // radioButtonOTCPVOBiasedNoThresholds
      // 
      this.radioButtonOTCPVOBiasedNoThresholds.Location = new System.Drawing.Point(232, 200);
      this.radioButtonOTCPVOBiasedNoThresholds.Name = "radioButtonOTCPVOBiasedNoThresholds";
      this.radioButtonOTCPVOBiasedNoThresholds.Size = new System.Drawing.Size(192, 24);
      this.radioButtonOTCPVOBiasedNoThresholds.TabIndex = 22;
      this.radioButtonOTCPVOBiasedNoThresholds.Text = "OTC_PVO Biased No Thresholds";
      this.radioButtonOTCPVOBiasedNoThresholds.CheckedChanged += new System.EventHandler(this.radioButtonOTCPVOBiasedNoThresholds_CheckedChanged);
      // 
      // TestDisplayer
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(704, 414);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.radioButtonOTCPVOBiasedNoThresholds,
                                                                  this.radioButtonPVOBiasedNoThresholds,
                                                                  this.labelTakeProfit,
                                                                  this.textBoxTakeProfit,
                                                                  this.labelStopLoss,
                                                                  this.textBoxStopLoss,
                                                                  this.radioButtonPVOBiased,
                                                                  this.radioButtonPVO,
                                                                  this.labelPortfolioType,
                                                                  this.comboBoxPortfolioType,
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
		private GenomeRepresentation[] dgBestGenomes_MouseUp_getClickedGenomeRepresentation(
			object sender, System.Windows.Forms.MouseEventArgs e )
		{
			GenomeRepresentation[] genomeRepresentations = 
						new GenomeRepresentation[this.lastSelectedGenomeRepresentations.Length];
			DataGrid dataGrid = (DataGrid)sender;
			Point point = new Point( e.X , e.Y );
			DataGrid.HitTestInfo hitTestInfo = dataGrid.HitTest( point );
			ArrayList bestGenomes = (ArrayList)dataGrid.DataSource;
			//			DataRow dataRow = dataTable.Rows[ hitTestInfo.Row ];
			if ( hitTestInfo.Row >= 0 )
				// a grid row has been clicked, not the header
				// all best genomes with the same optimization's dates
				// are saved in an array
			{
				genomeRepresentations[0] =
						(GenomeRepresentation)bestGenomes[ hitTestInfo.Row ];
				for(int i = 1; i < bestGenomes.Count - hitTestInfo.Row; i++)
					if( genomeRepresentations[0].FirstOptimizationDate ==
					   ((GenomeRepresentation)bestGenomes[hitTestInfo.Row+i]).FirstOptimizationDate )
					// the next row has the same optimization's date of the
					// original selected best genome by the user
								genomeRepresentations[i] =
									(GenomeRepresentation)bestGenomes[ hitTestInfo.Row + i];
			}
				return genomeRepresentations;
		}
		private void dgBestGenomes_MouseUp_rightButton_updateDates(
			GenomeRepresentation newSelectedGenomeRepresentation )
		{
			TimeSpan currentFirstDateDisplacement =
				( this.dtpFirstDate.Value -
				 this.lastSelectedGenomeRepresentations[0].FirstOptimizationDate );
			TimeSpan currentLastDateDisplacement =
				( this.dtpLastDate.Value -
				 this.lastSelectedGenomeRepresentations[0].LastOptimizationDate );
			this.dtpFirstDate.Value = newSelectedGenomeRepresentation.FirstOptimizationDate +
				currentFirstDateDisplacement;
			this.dtpLastDate.Value = newSelectedGenomeRepresentation.LastOptimizationDate +
				currentLastDateDisplacement;
		}
		private void dgBestGenomes_MouseUp_rightButton(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			GenomeRepresentation[] genomeRepresentations =
				this.dgBestGenomes_MouseUp_getClickedGenomeRepresentation( sender , e );
			dgBestGenomes_MouseUp_rightButton_updateDates( genomeRepresentations[0] );
			//string[] signedTickers = genomeRepresentation.SignedTickers.Split(";".ToCharArray());
			LinearCombinationTest linearCombinationTest =
				new LinearCombinationTest( this.dtpFirstDate.Value ,
				this.dtpLastDate.Value , genomeRepresentations ,
				this.selectedStrategyType,
        (PortfolioType)this.comboBoxPortfolioType.SelectedItem, Convert.ToInt32(this.textBoxDaysFPOscillatorAndRevOneRank.Text),
        Convert.ToDouble(this.textBoxStopLoss.Text),Convert.ToDouble(this.textBoxTakeProfit.Text) );
			linearCombinationTest.Run();
			this.lastSelectedGenomeRepresentations = genomeRepresentations;
		}
		
    
    private void dgBestGenomes_MouseUp_leftButton_updateForm(
      GenomeRepresentation newSelectedGenomeRepresentation )
    {
      this.dtpFirstDate.Value =
        newSelectedGenomeRepresentation.FirstOptimizationDate;
      this.dtpLastDate.Value =
        newSelectedGenomeRepresentation.LastOptimizationDate;
      
      if(newSelectedGenomeRepresentation.HalfPeriodDays > 0)
      //genomeRepresentation derives from a strategy
      //based on HalfPeriodDays
        this.textBoxDaysFPOscillatorAndRevOneRank.Text =
          newSelectedGenomeRepresentation.HalfPeriodDays.ToString();

      this.comboBoxPortfolioType.SelectedItem = 
        newSelectedGenomeRepresentation.PortfolioType;
    }

		private void dgBestGenomes_MouseUp_leftButton(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( aRowHasBeenClicked( sender , e ) )
				// a grid row has been clicked, not the header
			{
				GenomeRepresentation[] newSelectedGenomeRepresentations =
					this.dgBestGenomes_MouseUp_getClickedGenomeRepresentation( sender , e );
        this.lastSelectedGenomeRepresentations = newSelectedGenomeRepresentations;
        dgBestGenomes_MouseUp_leftButton_updateForm( newSelectedGenomeRepresentations[0] );
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
      else if(this.radioButtonPVO.Checked)
        this.selectedStrategyType = StrategyType.PortfolioValueOscillator;
      else if(this.radioButtonPVOBiased.Checked)
        this.selectedStrategyType = StrategyType.PortfolioValueOscillatorBiased;
      else if(this.radioButtonPVOBiasedNoThresholds.Checked)
        this.selectedStrategyType = StrategyType.PortfolioValueOscillatorBiasedNoThresholds;
      else if(this.radioButtonOTCPVOBiasedNoThresholds.Checked)
        this.selectedStrategyType = StrategyType.OTC_PVOBiasedNoThresholds;
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
      this.comboBoxPortfolioType.Visible = this.radioButtonExtremeCounterTrend.Checked;
      this.labelPortfolioType.Visible = this.radioButtonExtremeCounterTrend.Checked;;
    }

    private void radioButtonImmediateTrendFollower_CheckedChanged(object sender, System.EventArgs e)
    {
      this.update_selectedStrategyType();
    }

    private void radioButtonPVO_CheckedChanged(object sender, System.EventArgs e)
    {
      this.update_selectedStrategyType();
    }

    private void radioButtonPVOBiased_CheckedChanged(object sender, System.EventArgs e)
    {
      this.update_selectedStrategyType();
      this.labelStopLoss.Visible = this.radioButtonPVOBiased.Checked;
      this.textBoxStopLoss.Visible = this.radioButtonPVOBiased.Checked;
      this.labelTakeProfit.Visible = this.radioButtonPVOBiased.Checked;
      this.textBoxTakeProfit.Visible = this.radioButtonPVOBiased.Checked;
    }

    private void radioButtonPVOBiasedNoThresholds_CheckedChanged(object sender, System.EventArgs e)
    {
      this.update_selectedStrategyType();
      this.labelStopLoss.Visible = this.radioButtonPVOBiasedNoThresholds.Checked;
      this.textBoxStopLoss.Visible = this.radioButtonPVOBiasedNoThresholds.Checked;
      this.labelTakeProfit.Visible = this.radioButtonPVOBiasedNoThresholds.Checked;
      this.textBoxTakeProfit.Visible = this.radioButtonPVOBiasedNoThresholds.Checked;
    }

    private void radioButtonOTCPVOBiasedNoThresholds_CheckedChanged(object sender, System.EventArgs e)
    {
      this.update_selectedStrategyType();
    }
    
	}
}
