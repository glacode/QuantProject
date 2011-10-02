/*
QuantDownloader - Quantitative Finance Library

TickerSelectorForm.cs
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
using System.Data;
using System.Data.OleDb;
using QuantProject.ADT;
using QuantProject.DataAccess.Tables;
using QuantProject.Data.DataTables;
using QuantProject.Data.Selectors;
using QuantProject.Data.Selectors.ByLinearIndipendence;

namespace QuantProject.Applications.Downloader.TickerSelectors
{
	/// <summary>
	/// It is the user interface for the ITickerSelector chosen by user
	/// </summary>
	public class TickerSelectorForm : System.Windows.Forms.Form, ITickerSelector
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.TextBox textBoxMinPrice;
		private System.Windows.Forms.TextBox textBoxMinStdDev;
		private System.Windows.Forms.Label labelMinPrice;
		private System.Windows.Forms.Button buttonSelectTickers;
		private System.Windows.Forms.CheckBox checkBoxASCMode;
		private System.Windows.Forms.GroupBox groupBoxSelectionRule;
		private System.Windows.Forms.DateTimePicker dateTimePickerFirstDate;
		private System.Windows.Forms.DateTimePicker dateTimePickerLastDate;
		private System.Windows.Forms.TextBox textBoxMaxStdDev;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label labelMarketIndexKey;
		private System.Windows.Forms.DataGrid dataGrid1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxGroupID;
		private System.Windows.Forms.TextBox textBoxMarketIndex;
		private System.Windows.Forms.ComboBox comboBoxAvailableSelectionRules;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labelMinStdDev;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.TextBox textBoxMaxNumOfReturnedTickers;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label labelMaxStdDev;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label labelMaxPrice;
		private System.Windows.Forms.TextBox textBoxMaxPrice;
		private System.Windows.Forms.Splitter splitter1;
    private System.Windows.Forms.CheckBox checkBoxOnlyWithAtLeastOneWinningDay;
    private System.Windows.Forms.TextBox textBoxMinVolume;
    private System.Windows.Forms.Label labelMinVolume;
    private DataTable tableOfSelectedTickers;

		public TickerSelectorForm()
		{
			InitializeComponent();
      this.dateTimePickerFirstDate.Value = ConstantsProvider.InitialDateTimeForDownload;
      this.dateTimePickerLastDate.Value = DateTime.Now;
      this.dataGrid1.ContextMenu = new TickerViewerMenu(this);
      //TODO: complete comboBox's code with all possible types of selections
      this.comboBoxAvailableSelectionRules.Text = "Liquidity";
      this.comboBoxAvailableSelectionRules.Items.Add("Liquidity");
      this.comboBoxAvailableSelectionRules.Items.Add("Performance");
      this.comboBoxAvailableSelectionRules.Items.Add("CloseToCloseVolatility");
      this.comboBoxAvailableSelectionRules.Items.Add("OpenToCloseVolatility");
      this.comboBoxAvailableSelectionRules.Items.Add("CloseToOpenVolatility");
      this.comboBoxAvailableSelectionRules.Items.Add("AverageCloseToClosePerformance");
      this.comboBoxAvailableSelectionRules.Items.Add("CloseToCloseLinearCorrelation");
      this.comboBoxAvailableSelectionRules.Items.Add("OpenToCloseLinearCorrelation");
      this.comboBoxAvailableSelectionRules.Items.Add("AverageOpenToClosePerformance");
      this.comboBoxAvailableSelectionRules.Items.Add("AverageCloseToOpenPerformance");
      this.comboBoxAvailableSelectionRules.Items.Add("QuotedAtEachMarketDay");
      this.comboBoxAvailableSelectionRules.Items.Add("QuotedNotAtEachMarketDay");
      this.comboBoxAvailableSelectionRules.Items.Add("AverageRawOpenPrice");
      this.comboBoxAvailableSelectionRules.Items.Add("WinningOpenToClose");
      this.comboBoxAvailableSelectionRules.Items.Add("OpenToCloseCorrelationToBenchmark");
      this.comboBoxAvailableSelectionRules.Items.Add("CloseToCloseCorrelationToBenchmark");
      this.comboBoxAvailableSelectionRules.Items.Add("MaxLinearIndipendence");

  	}
    public TickerSelectorForm(string groupID) : this()
    {
      this.Text = "Apply rule to all tickers of the selected group";
      this.textBoxGroupID.Text = groupID; 
    }
    
    public TickerSelectorForm(DataTable tableOfSelectedTickers) : this()
    {
      this.Text = "Apply rule to the selected tickers";
      this.tableOfSelectedTickers = tableOfSelectedTickers;
      this.textBoxGroupID.Enabled = false;
    }

		/// <summary>
		/// clean up 
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
		/// do not modify the content with the editor
		/// </summary>
		private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.splitter1 = new System.Windows.Forms.Splitter();
      this.textBoxMaxPrice = new System.Windows.Forms.TextBox();
      this.labelMaxPrice = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.labelMaxStdDev = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.textBoxMaxNumOfReturnedTickers = new System.Windows.Forms.TextBox();
      this.panel2 = new System.Windows.Forms.Panel();
      this.groupBoxSelectionRule = new System.Windows.Forms.GroupBox();
      this.labelTextBoxPopulationSize = new System.Windows.Forms.Label();
      this.labelTextBoxGenerations = new System.Windows.Forms.Label();
      this.textBoxGenerationsForGeneticOptimizer = new System.Windows.Forms.TextBox();
      this.textBoxPopulationSizeForGeneticOptimizer = new System.Windows.Forms.TextBox();
      this.checkBoxAddIndexToOutputTable = new System.Windows.Forms.CheckBox();
      this.checkBoxOnlyWithAtLeastOneWinningDay = new System.Windows.Forms.CheckBox();
      this.textBoxMaxStdDev = new System.Windows.Forms.TextBox();
      this.labelMinStdDev = new System.Windows.Forms.Label();
      this.textBoxMinStdDev = new System.Windows.Forms.TextBox();
      this.labelMinPrice = new System.Windows.Forms.Label();
      this.textBoxMinPrice = new System.Windows.Forms.TextBox();
      this.labelMarketIndexKey = new System.Windows.Forms.Label();
      this.textBoxMarketIndex = new System.Windows.Forms.TextBox();
      this.checkBoxASCMode = new System.Windows.Forms.CheckBox();
      this.label3 = new System.Windows.Forms.Label();
      this.comboBoxAvailableSelectionRules = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.textBoxGroupID = new System.Windows.Forms.TextBox();
      this.dateTimePickerLastDate = new System.Windows.Forms.DateTimePicker();
      this.dateTimePickerFirstDate = new System.Windows.Forms.DateTimePicker();
      this.buttonSelectTickers = new System.Windows.Forms.Button();
      this.dataGrid1 = new System.Windows.Forms.DataGrid();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.textBoxMinVolume = new System.Windows.Forms.TextBox();
      this.labelMinVolume = new System.Windows.Forms.Label();
      this.panel2.SuspendLayout();
      this.groupBoxSelectionRule.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
      this.SuspendLayout();
      // 
      // splitter1
      // 
      this.splitter1.BackColor = System.Drawing.SystemColors.Highlight;
      this.splitter1.Location = new System.Drawing.Point(432, 0);
      this.splitter1.Name = "splitter1";
      this.splitter1.Size = new System.Drawing.Size(3, 478);
      this.splitter1.TabIndex = 8;
      this.splitter1.TabStop = false;
      // 
      // textBoxMaxPrice
      // 
      this.textBoxMaxPrice.Location = new System.Drawing.Point(69, 202);
      this.textBoxMaxPrice.Name = "textBoxMaxPrice";
      this.textBoxMaxPrice.Size = new System.Drawing.Size(56, 20);
      this.textBoxMaxPrice.TabIndex = 32;
      this.textBoxMaxPrice.Text = "";
      this.textBoxMaxPrice.Visible = false;
      // 
      // labelMaxPrice
      // 
      this.labelMaxPrice.Location = new System.Drawing.Point(13, 202);
      this.labelMaxPrice.Name = "labelMaxPrice";
      this.labelMaxPrice.Size = new System.Drawing.Size(56, 14);
      this.labelMaxPrice.TabIndex = 33;
      this.labelMaxPrice.Text = "Max Price";
      this.labelMaxPrice.Visible = false;
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(8, 16);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(64, 21);
      this.label5.TabIndex = 18;
      this.label5.Text = "First Date";
      // 
      // labelMaxStdDev
      // 
      this.labelMaxStdDev.Location = new System.Drawing.Point(149, 202);
      this.labelMaxStdDev.Name = "labelMaxStdDev";
      this.labelMaxStdDev.Size = new System.Drawing.Size(80, 14);
      this.labelMaxStdDev.TabIndex = 37;
      this.labelMaxStdDev.Text = "Max Std Dev";
      this.labelMaxStdDev.Visible = false;
      // 
      // label7
      // 
      this.label7.Location = new System.Drawing.Point(168, 16);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(64, 21);
      this.label7.TabIndex = 20;
      this.label7.Text = "Last Date";
      // 
      // textBoxMaxNumOfReturnedTickers
      // 
      this.textBoxMaxNumOfReturnedTickers.Location = new System.Drawing.Point(312, 41);
      this.textBoxMaxNumOfReturnedTickers.Name = "textBoxMaxNumOfReturnedTickers";
      this.textBoxMaxNumOfReturnedTickers.Size = new System.Drawing.Size(48, 20);
      this.textBoxMaxNumOfReturnedTickers.TabIndex = 23;
      this.textBoxMaxNumOfReturnedTickers.Text = "";
      // 
      // panel2
      // 
      this.panel2.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                         this.groupBoxSelectionRule,
                                                                         this.buttonSelectTickers});
      this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel2.ForeColor = System.Drawing.SystemColors.ControlText;
      this.panel2.Location = new System.Drawing.Point(432, 0);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(372, 478);
      this.panel2.TabIndex = 7;
      // 
      // groupBoxSelectionRule
      // 
      this.groupBoxSelectionRule.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                        this.labelMinVolume,
                                                                                        this.textBoxMinVolume,
                                                                                        this.labelTextBoxPopulationSize,
                                                                                        this.labelTextBoxGenerations,
                                                                                        this.textBoxGenerationsForGeneticOptimizer,
                                                                                        this.textBoxPopulationSizeForGeneticOptimizer,
                                                                                        this.checkBoxAddIndexToOutputTable,
                                                                                        this.checkBoxOnlyWithAtLeastOneWinningDay,
                                                                                        this.labelMaxStdDev,
                                                                                        this.textBoxMaxStdDev,
                                                                                        this.labelMinStdDev,
                                                                                        this.textBoxMinStdDev,
                                                                                        this.labelMaxPrice,
                                                                                        this.textBoxMaxPrice,
                                                                                        this.labelMinPrice,
                                                                                        this.textBoxMinPrice,
                                                                                        this.labelMarketIndexKey,
                                                                                        this.textBoxMarketIndex,
                                                                                        this.checkBoxASCMode,
                                                                                        this.label3,
                                                                                        this.comboBoxAvailableSelectionRules,
                                                                                        this.label2,
                                                                                        this.textBoxMaxNumOfReturnedTickers,
                                                                                        this.label1,
                                                                                        this.textBoxGroupID,
                                                                                        this.label7,
                                                                                        this.label5,
                                                                                        this.dateTimePickerLastDate,
                                                                                        this.dateTimePickerFirstDate});
      this.groupBoxSelectionRule.Location = new System.Drawing.Point(8, 10);
      this.groupBoxSelectionRule.Name = "groupBoxSelectionRule";
      this.groupBoxSelectionRule.Size = new System.Drawing.Size(361, 272);
      this.groupBoxSelectionRule.TabIndex = 14;
      this.groupBoxSelectionRule.TabStop = false;
      this.groupBoxSelectionRule.Text = "Single Selection rule";
      // 
      // labelTextBoxPopulationSize
      // 
      this.labelTextBoxPopulationSize.Location = new System.Drawing.Point(192, 227);
      this.labelTextBoxPopulationSize.Name = "labelTextBoxPopulationSize";
      this.labelTextBoxPopulationSize.Size = new System.Drawing.Size(70, 14);
      this.labelTextBoxPopulationSize.TabIndex = 45;
      this.labelTextBoxPopulationSize.Text = "Pop. Size";
      this.labelTextBoxPopulationSize.Visible = false;
      // 
      // labelTextBoxGenerations
      // 
      this.labelTextBoxGenerations.Location = new System.Drawing.Point(16, 227);
      this.labelTextBoxGenerations.Name = "labelTextBoxGenerations";
      this.labelTextBoxGenerations.Size = new System.Drawing.Size(73, 15);
      this.labelTextBoxGenerations.TabIndex = 44;
      this.labelTextBoxGenerations.Text = "Generations";
      this.labelTextBoxGenerations.Visible = false;
      // 
      // textBoxGenerationsForGeneticOptimizer
      // 
      this.textBoxGenerationsForGeneticOptimizer.Location = new System.Drawing.Point(117, 228);
      this.textBoxGenerationsForGeneticOptimizer.Name = "textBoxGenerationsForGeneticOptimizer";
      this.textBoxGenerationsForGeneticOptimizer.Size = new System.Drawing.Size(56, 20);
      this.textBoxGenerationsForGeneticOptimizer.TabIndex = 43;
      this.textBoxGenerationsForGeneticOptimizer.Text = "";
      this.textBoxGenerationsForGeneticOptimizer.Visible = false;
      // 
      // textBoxPopulationSizeForGeneticOptimizer
      // 
      this.textBoxPopulationSizeForGeneticOptimizer.Location = new System.Drawing.Point(269, 227);
      this.textBoxPopulationSizeForGeneticOptimizer.Name = "textBoxPopulationSizeForGeneticOptimizer";
      this.textBoxPopulationSizeForGeneticOptimizer.Size = new System.Drawing.Size(56, 20);
      this.textBoxPopulationSizeForGeneticOptimizer.TabIndex = 42;
      this.textBoxPopulationSizeForGeneticOptimizer.Text = "";
      this.textBoxPopulationSizeForGeneticOptimizer.Visible = false;
      // 
      // checkBoxAddIndexToOutputTable
      // 
      this.checkBoxAddIndexToOutputTable.Location = new System.Drawing.Point(11, 131);
      this.checkBoxAddIndexToOutputTable.Name = "checkBoxAddIndexToOutputTable";
      this.checkBoxAddIndexToOutputTable.Size = new System.Drawing.Size(164, 15);
      this.checkBoxAddIndexToOutputTable.TabIndex = 41;
      this.checkBoxAddIndexToOutputTable.Text = "Add index to Output Table";
      this.toolTip1.SetToolTip(this.checkBoxAddIndexToOutputTable, "Check if you want to filter only tickers with at least one winning day");
      this.checkBoxAddIndexToOutputTable.Visible = false;
      // 
      // checkBoxOnlyWithAtLeastOneWinningDay
      // 
      this.checkBoxOnlyWithAtLeastOneWinningDay.Location = new System.Drawing.Point(195, 113);
      this.checkBoxOnlyWithAtLeastOneWinningDay.Name = "checkBoxOnlyWithAtLeastOneWinningDay";
      this.checkBoxOnlyWithAtLeastOneWinningDay.Size = new System.Drawing.Size(152, 16);
      this.checkBoxOnlyWithAtLeastOneWinningDay.TabIndex = 38;
      this.checkBoxOnlyWithAtLeastOneWinningDay.Text = "Exclude \"pure\" losers";
      this.toolTip1.SetToolTip(this.checkBoxOnlyWithAtLeastOneWinningDay, "Check if you want to filter only tickers with at least one winning day");
      this.checkBoxOnlyWithAtLeastOneWinningDay.Visible = false;
      // 
      // textBoxMaxStdDev
      // 
      this.textBoxMaxStdDev.Location = new System.Drawing.Point(229, 202);
      this.textBoxMaxStdDev.Name = "textBoxMaxStdDev";
      this.textBoxMaxStdDev.Size = new System.Drawing.Size(56, 20);
      this.textBoxMaxStdDev.TabIndex = 36;
      this.textBoxMaxStdDev.Text = "";
      this.textBoxMaxStdDev.Visible = false;
      // 
      // labelMinStdDev
      // 
      this.labelMinStdDev.Location = new System.Drawing.Point(152, 149);
      this.labelMinStdDev.Name = "labelMinStdDev";
      this.labelMinStdDev.Size = new System.Drawing.Size(72, 14);
      this.labelMinStdDev.TabIndex = 35;
      this.labelMinStdDev.Text = "Min Std Dev";
      this.labelMinStdDev.Visible = false;
      // 
      // textBoxMinStdDev
      // 
      this.textBoxMinStdDev.Location = new System.Drawing.Point(232, 149);
      this.textBoxMinStdDev.Name = "textBoxMinStdDev";
      this.textBoxMinStdDev.Size = new System.Drawing.Size(56, 20);
      this.textBoxMinStdDev.TabIndex = 34;
      this.textBoxMinStdDev.Text = "";
      this.textBoxMinStdDev.Visible = false;
      // 
      // labelMinPrice
      // 
      this.labelMinPrice.Location = new System.Drawing.Point(16, 149);
      this.labelMinPrice.Name = "labelMinPrice";
      this.labelMinPrice.Size = new System.Drawing.Size(56, 14);
      this.labelMinPrice.TabIndex = 31;
      this.labelMinPrice.Text = "Min Price";
      this.labelMinPrice.Visible = false;
      // 
      // textBoxMinPrice
      // 
      this.textBoxMinPrice.Location = new System.Drawing.Point(72, 149);
      this.textBoxMinPrice.Name = "textBoxMinPrice";
      this.textBoxMinPrice.Size = new System.Drawing.Size(56, 20);
      this.textBoxMinPrice.TabIndex = 30;
      this.textBoxMinPrice.Text = "";
      this.textBoxMinPrice.Visible = false;
      // 
      // labelMarketIndexKey
      // 
      this.labelMarketIndexKey.Location = new System.Drawing.Point(13, 176);
      this.labelMarketIndexKey.Name = "labelMarketIndexKey";
      this.labelMarketIndexKey.Size = new System.Drawing.Size(96, 16);
      this.labelMarketIndexKey.TabIndex = 29;
      this.labelMarketIndexKey.Text = "Market index key:";
      this.labelMarketIndexKey.Visible = false;
      // 
      // textBoxMarketIndex
      // 
      this.textBoxMarketIndex.Location = new System.Drawing.Point(117, 176);
      this.textBoxMarketIndex.Name = "textBoxMarketIndex";
      this.textBoxMarketIndex.Size = new System.Drawing.Size(56, 20);
      this.textBoxMarketIndex.TabIndex = 28;
      this.textBoxMarketIndex.Text = "";
      this.textBoxMarketIndex.Visible = false;
      // 
      // checkBoxASCMode
      // 
      this.checkBoxASCMode.Location = new System.Drawing.Point(11, 113);
      this.checkBoxASCMode.Name = "checkBoxASCMode";
      this.checkBoxASCMode.Size = new System.Drawing.Size(152, 16);
      this.checkBoxASCMode.TabIndex = 27;
      this.checkBoxASCMode.Text = "Order by ASC mode";
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(16, 68);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(120, 11);
      this.label3.TabIndex = 26;
      this.label3.Text = "Choose selection rule";
      // 
      // comboBoxAvailableSelectionRules
      // 
      this.comboBoxAvailableSelectionRules.Location = new System.Drawing.Point(11, 87);
      this.comboBoxAvailableSelectionRules.Name = "comboBoxAvailableSelectionRules";
      this.comboBoxAvailableSelectionRules.Size = new System.Drawing.Size(176, 21);
      this.comboBoxAvailableSelectionRules.TabIndex = 25;
      this.comboBoxAvailableSelectionRules.Text = "comboBox1";
      this.comboBoxAvailableSelectionRules.SelectedValueChanged += new System.EventHandler(this.comboBoxAvailableSelectionRules_SelectedValueChanged);
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(160, 45);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(152, 16);
      this.label2.TabIndex = 24;
      this.label2.Text = "Max Num of returned tickers";
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(8, 41);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(64, 21);
      this.label1.TabIndex = 22;
      this.label1.Text = "GroupID";
      // 
      // textBoxGroupID
      // 
      this.textBoxGroupID.Location = new System.Drawing.Point(72, 41);
      this.textBoxGroupID.Name = "textBoxGroupID";
      this.textBoxGroupID.Size = new System.Drawing.Size(88, 20);
      this.textBoxGroupID.TabIndex = 21;
      this.textBoxGroupID.Text = "";
      // 
      // dateTimePickerLastDate
      // 
      this.dateTimePickerLastDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.dateTimePickerLastDate.Location = new System.Drawing.Point(240, 16);
      this.dateTimePickerLastDate.Name = "dateTimePickerLastDate";
      this.dateTimePickerLastDate.Size = new System.Drawing.Size(88, 20);
      this.dateTimePickerLastDate.TabIndex = 15;
      // 
      // dateTimePickerFirstDate
      // 
      this.dateTimePickerFirstDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.dateTimePickerFirstDate.Location = new System.Drawing.Point(72, 16);
      this.dateTimePickerFirstDate.Name = "dateTimePickerFirstDate";
      this.dateTimePickerFirstDate.Size = new System.Drawing.Size(88, 20);
      this.dateTimePickerFirstDate.TabIndex = 13;
      // 
      // buttonSelectTickers
      // 
      this.buttonSelectTickers.Location = new System.Drawing.Point(148, 301);
      this.buttonSelectTickers.Name = "buttonSelectTickers";
      this.buttonSelectTickers.Size = new System.Drawing.Size(104, 31);
      this.buttonSelectTickers.TabIndex = 3;
      this.buttonSelectTickers.Text = "Select Tickers";
      this.buttonSelectTickers.Click += new System.EventHandler(this.buttonSelectTickers_Click);
      // 
      // dataGrid1
      // 
      this.dataGrid1.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.dataGrid1.DataMember = "";
      this.dataGrid1.Dock = System.Windows.Forms.DockStyle.Left;
      this.dataGrid1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
      this.dataGrid1.Name = "dataGrid1";
      this.dataGrid1.Size = new System.Drawing.Size(432, 478);
      this.dataGrid1.TabIndex = 2;
      // 
      // textBoxMinVolume
      // 
      this.textBoxMinVolume.Location = new System.Drawing.Point(264, 88);
      this.textBoxMinVolume.Name = "textBoxMinVolume";
      this.textBoxMinVolume.Size = new System.Drawing.Size(80, 20);
      this.textBoxMinVolume.TabIndex = 46;
      this.textBoxMinVolume.Text = "0";
      // 
      // labelMinVolume
      // 
      this.labelMinVolume.Location = new System.Drawing.Point(192, 92);
      this.labelMinVolume.Name = "labelMinVolume";
      this.labelMinVolume.Size = new System.Drawing.Size(64, 16);
      this.labelMinVolume.TabIndex = 47;
      this.labelMinVolume.Text = "Min Volume";
      // 
      // TickerSelectorForm
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(804, 478);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.splitter1,
                                                                  this.panel2,
                                                                  this.dataGrid1});
      this.Name = "TickerSelectorForm";
      this.Text = "Ticker Selector";
      this.panel2.ResumeLayout(false);
      this.groupBoxSelectionRule.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
      this.ResumeLayout(false);

    }
		private System.Windows.Forms.Label labelTextBoxPopulationSize;
		private System.Windows.Forms.Label labelTextBoxGenerations;
		private System.Windows.Forms.CheckBox checkBoxAddIndexToOutputTable;
		private System.Windows.Forms.TextBox textBoxPopulationSizeForGeneticOptimizer;
		private System.Windows.Forms.TextBox textBoxGenerationsForGeneticOptimizer;
		#endregion


    // implementation of ITickerSelector interface

		public DataTable GetTableOfSelectedTickers()
		{
			return TickerSelector.GetTableOfManuallySelectedTickers(this.dataGrid1);
		}

    public void SelectAllTickers()
    {
      DataTable dataTableOfDataGrid1 = (DataTable)this.dataGrid1.DataSource;
      int indexOfRow = 0;
      while(indexOfRow != dataTableOfDataGrid1.Rows.Count)
      {
        this.dataGrid1.Select(indexOfRow);
        indexOfRow++;
      }
    }    


    private void buttonSelectTickers_Click(object sender, System.EventArgs e)
    {
      try
      {
        Cursor.Current = Cursors.WaitCursor;  
        ITickerSelector selector;
        selector = this.getITickerSelector();
        this.dataGrid1.DataSource = selector.GetTableOfSelectedTickers();
        this.dataGrid1.Refresh();
        Cursor.Current = Cursors.Default;
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }

    private ITickerSelector getITickerSelector()
    {
      ITickerSelector returnValue = null;

      if(this.comboBoxAvailableSelectionRules.Text == "Liquidity")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByLiquidity(this.textBoxGroupID.Text,
                                                this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
                                                this.dateTimePickerLastDate.Value,
                                                Int64.Parse(this.textBoxMinVolume.Text),
                                                Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text ));
        else
          returnValue = new SelectorByLiquidity(this.tableOfSelectedTickers,
                                                this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
                                                this.dateTimePickerLastDate.Value,
                                                Int64.Parse(this.textBoxMinVolume.Text),
                                                Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
      }
      else if (this.comboBoxAvailableSelectionRules.Text == "Performance")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByAbsolutePerformance(this.textBoxGroupID.Text,
                                        this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
                                        this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
        else
          returnValue = new SelectorByAbsolutePerformance(this.tableOfSelectedTickers,
                      this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
                      this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
      }      
      else if (this.comboBoxAvailableSelectionRules.Text == "CloseToCloseVolatility")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByCloseToCloseVolatility(this.textBoxGroupID.Text,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
        else
          returnValue = new SelectorByCloseToCloseVolatility(this.tableOfSelectedTickers,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
      }            
      else if (this.comboBoxAvailableSelectionRules.Text == "OpenToCloseVolatility")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByOpenToCloseVolatility (this.textBoxGroupID.Text,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
        else
          returnValue = new SelectorByOpenToCloseVolatility (this.tableOfSelectedTickers,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
      }
      else if (this.comboBoxAvailableSelectionRules.Text == "CloseToOpenVolatility")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByCloseToOpenVolatility (this.textBoxGroupID.Text,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
        else
          returnValue = new SelectorByCloseToOpenVolatility (this.tableOfSelectedTickers,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
      }                   
      else if (this.comboBoxAvailableSelectionRules.Text == "AverageCloseToClosePerformance")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByAverageCloseToClosePerformance(this.textBoxGroupID.Text,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
        else
          returnValue = new SelectorByAverageCloseToClosePerformance(this.tableOfSelectedTickers,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
      }                  
      else if (this.comboBoxAvailableSelectionRules.Text == "CloseToCloseLinearCorrelation")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByCloseToCloseLinearCorrelation (this.textBoxGroupID.Text,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
        else
          returnValue = new SelectorByCloseToCloseLinearCorrelation(this.tableOfSelectedTickers,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
      }                 
      else if (this.comboBoxAvailableSelectionRules.Text == "OpenToCloseLinearCorrelation")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByOpenToCloseLinearCorrelation  (this.textBoxGroupID.Text,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
        else
          returnValue = new SelectorByOpenToCloseLinearCorrelation (this.tableOfSelectedTickers,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
      }                 
      else if (this.comboBoxAvailableSelectionRules.Text == "AverageOpenToClosePerformance")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByAverageOpenToClosePerformance(this.textBoxGroupID.Text,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, 0.08, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
        else
          returnValue = new SelectorByAverageOpenToClosePerformance(this.tableOfSelectedTickers,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, 0.08, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
      }
      else if (this.comboBoxAvailableSelectionRules.Text == "AverageCloseToOpenPerformance")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByAverageCloseToOpenPerformance(this.textBoxGroupID.Text,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
        else
          returnValue = new SelectorByAverageCloseToOpenPerformance(this.tableOfSelectedTickers,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
      }                      
      else if (this.comboBoxAvailableSelectionRules.Text == "QuotedAtEachMarketDay")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByQuotationAtEachMarketDay (this.textBoxGroupID.Text,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text),
            this.textBoxMarketIndex.Text);
        else
          returnValue = new SelectorByQuotationAtEachMarketDay(this.tableOfSelectedTickers,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text),
            this.textBoxMarketIndex.Text);
      } 
      else if (this.comboBoxAvailableSelectionRules.Text == "QuotedNotAtEachMarketDay")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByQuotationNotAtEachMarketDay (this.textBoxGroupID.Text,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text),
            this.textBoxMarketIndex.Text);
        else
          returnValue = new SelectorByQuotationNotAtEachMarketDay(this.tableOfSelectedTickers,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text),
            this.textBoxMarketIndex.Text);
      } 
      else if (this.comboBoxAvailableSelectionRules.Text == "AverageRawOpenPrice")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByAverageRawOpenPrice(this.textBoxGroupID.Text,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text),
            Double.Parse(this.textBoxMinPrice.Text),Double.Parse(this.textBoxMaxPrice.Text),
            Double.Parse(this.textBoxMinStdDev.Text), Double.Parse(this.textBoxMaxStdDev.Text));
        else
        	returnValue = new SelectorByAverageRawOpenPrice(this.tableOfSelectedTickers, this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text),
            Double.Parse(this.textBoxMinPrice.Text),Double.Parse(this.textBoxMaxPrice.Text),
            Double.Parse(this.textBoxMinStdDev.Text), Double.Parse(this.textBoxMaxStdDev.Text));
      } 
      else if (this.comboBoxAvailableSelectionRules.Text == "WinningOpenToClose")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByWinningOpenToClose(this.textBoxGroupID.Text,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text),
            this.checkBoxOnlyWithAtLeastOneWinningDay.Checked);
        else
        	returnValue = new SelectorByWinningOpenToClose(this.tableOfSelectedTickers,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text),
            this.checkBoxOnlyWithAtLeastOneWinningDay.Checked);
      }
      else if (this.comboBoxAvailableSelectionRules.Text == "OpenToCloseCorrelationToBenchmark")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByOpenCloseCorrelationToBenchmark(this.textBoxGroupID.Text, this.textBoxMarketIndex.Text,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
        else
          returnValue = new SelectorByOpenCloseCorrelationToBenchmark(this.tableOfSelectedTickers, this.textBoxMarketIndex.Text,
                              this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
                              this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
      } 
      else if (this.comboBoxAvailableSelectionRules.Text == "CloseToCloseCorrelationToBenchmark")
      { 
//        if(this.textBoxGroupID.Text != "")
//          returnValue = new SelectorByCloseToCloseCorrelationToBenchmark(this.textBoxGroupID.Text, this.textBoxMarketIndex.Text,
//            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
//            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text),
//            false);
//        else
//          returnValue = new SelectorByCloseToCloseCorrelationToBenchmark(this.tableOfSelectedTickers, this.textBoxMarketIndex.Text,
//            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
//            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text),
//            false);
      } 
      else if (this.comboBoxAvailableSelectionRules.Text == "MaxLinearIndipendence")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByMaxLinearIndipendence(this.textBoxGroupID.Text,
            this.dateTimePickerFirstDate.Value,this.dateTimePickerLastDate.Value,
            Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text),Int32.Parse(this.textBoxGenerationsForGeneticOptimizer.Text),
            Int32.Parse(this.textBoxPopulationSizeForGeneticOptimizer.Text),
            this.textBoxMarketIndex.Text);
        else
          returnValue = new SelectorByMaxLinearIndipendence(this.tableOfSelectedTickers,
            this.dateTimePickerFirstDate.Value,this.dateTimePickerLastDate.Value,
            Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text),Int32.Parse(this.textBoxGenerationsForGeneticOptimizer.Text),
            Int32.Parse(this.textBoxPopulationSizeForGeneticOptimizer.Text),
            this.textBoxMarketIndex.Text);
      } 
      return returnValue;  
    }
		   
    private void setVisibilityForControls_QuotedAtEachMarketDay(bool showControls)
    {
      this.checkBoxASCMode.Enabled = showControls;
      this.labelMarketIndexKey.Visible = showControls;
      this.textBoxMarketIndex.Visible = showControls;
    }    
    private void setVisibilityForControls_WinningOpenToClose(bool showControls)
    {
      this.checkBoxASCMode.Enabled = showControls;
      this.checkBoxOnlyWithAtLeastOneWinningDay.Enabled = showControls;
      
    }  
    private void setVisibilityForControls_AverageRawOpenPrice(bool showControls)
    {
      this.labelMinPrice.Visible = showControls;
      this.labelMaxPrice.Visible = showControls;
      this.textBoxMaxPrice.Visible = showControls;
      this.textBoxMinPrice.Visible = showControls;
      
      this.labelMinStdDev.Visible = showControls;
      this.labelMaxStdDev.Visible = showControls;
      this.textBoxMinStdDev.Visible = showControls;
      this.textBoxMaxStdDev.Visible = showControls;
      
    }
  
    private void setVisibilityForControls_CloseToCloseCorrelationToBenchmark(bool showControls)
    {
      this.labelMarketIndexKey.Visible = showControls;
      this.textBoxMarketIndex.Visible = showControls;
      this.checkBoxAddIndexToOutputTable.Visible = showControls;
    } 

    private void setVisibilityForControls_MaxLinearIndipendence(bool showControls)
    {
      this.textBoxMarketIndex.Visible = showControls;
      this.labelMarketIndexKey.Visible = showControls;
      this.labelTextBoxPopulationSize.Visible = showControls;
      this.textBoxPopulationSizeForGeneticOptimizer.Visible = showControls;
      this.labelTextBoxGenerations.Visible = showControls;
      this.textBoxGenerationsForGeneticOptimizer.Visible = showControls;
    }
    private void setVisibilityForControls_ByLiquidity(bool showControls)
    {
      this.textBoxMinVolume.Visible = showControls;
      this.labelMinVolume.Visible = showControls;
      this.checkBoxASCMode.Enabled = true;
    }
    private void comboBoxAvailableSelectionRules_SelectedValueChanged(object sender, System.EventArgs e)
    {
      if(this.comboBoxAvailableSelectionRules.Text == "QuotedAtEachMarketDay" ||
         this.comboBoxAvailableSelectionRules.Text == "QuotedNotAtEachMarketDay" ||
          this.comboBoxAvailableSelectionRules.Text == "OpenToCloseCorrelationToBenchmark")
      {
        this.setVisibilityForControls_CloseToCloseCorrelationToBenchmark(false);
        this.setVisibilityForControls_AverageRawOpenPrice(false);
        this.setVisibilityForControls_WinningOpenToClose(false);
        this.setVisibilityForControls_MaxLinearIndipendence(false);
        this.setVisibilityForControls_ByLiquidity(false);
        this.setVisibilityForControls_QuotedAtEachMarketDay(true);
      }
      else if(this.comboBoxAvailableSelectionRules.Text == "AverageRawOpenPrice")
      {
        this.textBoxMarketIndex.Text = "";
        this.setVisibilityForControls_CloseToCloseCorrelationToBenchmark(false);
        this.setVisibilityForControls_QuotedAtEachMarketDay(false);
        this.setVisibilityForControls_WinningOpenToClose(false);
        this.setVisibilityForControls_MaxLinearIndipendence(false);
        this.setVisibilityForControls_ByLiquidity(false);
        this.setVisibilityForControls_AverageRawOpenPrice(true);
      }
      else if(this.comboBoxAvailableSelectionRules.Text == "WinningOpenToClose")
      {
        this.textBoxMarketIndex.Text = "";
        this.setVisibilityForControls_CloseToCloseCorrelationToBenchmark(false);
        this.setVisibilityForControls_QuotedAtEachMarketDay(false);
        this.setVisibilityForControls_AverageRawOpenPrice(false);
        this.setVisibilityForControls_MaxLinearIndipendence(false);
        this.setVisibilityForControls_ByLiquidity(false);
        this.setVisibilityForControls_WinningOpenToClose(true);
      }
      else if(this.comboBoxAvailableSelectionRules.Text == "CloseToCloseCorrelationToBenchmark")
      {
        this.setVisibilityForControls_QuotedAtEachMarketDay(false);
        this.setVisibilityForControls_AverageRawOpenPrice(false);
        this.setVisibilityForControls_WinningOpenToClose(false);
        this.setVisibilityForControls_MaxLinearIndipendence(false);
        this.setVisibilityForControls_ByLiquidity(false);
        this.setVisibilityForControls_CloseToCloseCorrelationToBenchmark(true);
      }
      else if(this.comboBoxAvailableSelectionRules.Text == "MaxLinearIndipendence")
      {
        this.setVisibilityForControls_QuotedAtEachMarketDay(false);
        this.setVisibilityForControls_AverageRawOpenPrice(false);
        this.setVisibilityForControls_WinningOpenToClose(false);
        this.setVisibilityForControls_CloseToCloseCorrelationToBenchmark(false);
        this.setVisibilityForControls_ByLiquidity(false);
        this.setVisibilityForControls_MaxLinearIndipendence(true);
      }
      else if(this.comboBoxAvailableSelectionRules.Text == "Liquidity")
      {
        this.setVisibilityForControls_AverageRawOpenPrice(false);
        this.setVisibilityForControls_WinningOpenToClose(false);
        this.setVisibilityForControls_CloseToCloseCorrelationToBenchmark(false);
        this.setVisibilityForControls_MaxLinearIndipendence(false);
        this.setVisibilityForControls_QuotedAtEachMarketDay(false);
        this.setVisibilityForControls_ByLiquidity(true);
      }
      else
      {
        this.textBoxMarketIndex.Text = "";
      	this.setVisibilityForControls_AverageRawOpenPrice(false);
        this.setVisibilityForControls_WinningOpenToClose(false);
        this.setVisibilityForControls_CloseToCloseCorrelationToBenchmark(false);
        this.setVisibilityForControls_MaxLinearIndipendence(false);
        this.setVisibilityForControls_QuotedAtEachMarketDay(false);
        this.setVisibilityForControls_ByLiquidity(false);
        this.checkBoxASCMode.Enabled = true;
      }
    }    

	}
}
