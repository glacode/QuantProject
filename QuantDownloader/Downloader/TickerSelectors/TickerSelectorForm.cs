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
		private System.Windows.Forms.DataGrid dataGrid1;
		private System.Windows.Forms.DateTimePicker dateTimePickerLastDate;
		private System.Windows.Forms.TextBox textBoxMarketIndex;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label labelMarketIndexKey;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxGroupID;
		private System.Windows.Forms.TextBox textBoxMaxStdDev;
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
      this.comboBoxAvailableSelectionRules.Items.Add("CloseToOpenVolatility");
      this.comboBoxAvailableSelectionRules.Items.Add("AverageCloseToClosePerformance");
      this.comboBoxAvailableSelectionRules.Items.Add("CloseToCloseLinearCorrelation");
      this.comboBoxAvailableSelectionRules.Items.Add("CloseToOpenLinearCorrelation");
      this.comboBoxAvailableSelectionRules.Items.Add("AverageCloseToOpenPerformance");
      this.comboBoxAvailableSelectionRules.Items.Add("QuotedAtEachMarketDay");
      this.comboBoxAvailableSelectionRules.Items.Add("AverageRawOpenPrice");

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
			this.labelMinStdDev = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.comboBoxAvailableSelectionRules = new System.Windows.Forms.ComboBox();
			this.textBoxMaxStdDev = new System.Windows.Forms.TextBox();
			this.textBoxGroupID = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labelMarketIndexKey = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.textBoxMarketIndex = new System.Windows.Forms.TextBox();
			this.dateTimePickerLastDate = new System.Windows.Forms.DateTimePicker();
			this.dataGrid1 = new System.Windows.Forms.DataGrid();
			this.dateTimePickerFirstDate = new System.Windows.Forms.DateTimePicker();
			this.groupBoxSelectionRule = new System.Windows.Forms.GroupBox();
			this.checkBoxASCMode = new System.Windows.Forms.CheckBox();
			this.buttonSelectTickers = new System.Windows.Forms.Button();
			this.labelMinPrice = new System.Windows.Forms.Label();
			this.textBoxMinStdDev = new System.Windows.Forms.TextBox();
			this.textBoxMinPrice = new System.Windows.Forms.TextBox();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.groupBoxSelectionRule.SuspendLayout();
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
			this.textBoxMaxPrice.Location = new System.Drawing.Point(72, 200);
			this.textBoxMaxPrice.Name = "textBoxMaxPrice";
			this.textBoxMaxPrice.Size = new System.Drawing.Size(56, 20);
			this.textBoxMaxPrice.TabIndex = 32;
			this.textBoxMaxPrice.Text = "";
			this.textBoxMaxPrice.Visible = false;
			// 
			// labelMaxPrice
			// 
			this.labelMaxPrice.Location = new System.Drawing.Point(16, 200);
			this.labelMaxPrice.Name = "labelMaxPrice";
			this.labelMaxPrice.Size = new System.Drawing.Size(56, 23);
			this.labelMaxPrice.TabIndex = 33;
			this.labelMaxPrice.Text = "Max Price";
			this.labelMaxPrice.Visible = false;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 24);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(64, 16);
			this.label5.TabIndex = 18;
			this.label5.Text = "First Date";
			// 
			// labelMaxStdDev
			// 
			this.labelMaxStdDev.Location = new System.Drawing.Point(152, 200);
			this.labelMaxStdDev.Name = "labelMaxStdDev";
			this.labelMaxStdDev.Size = new System.Drawing.Size(80, 23);
			this.labelMaxStdDev.TabIndex = 37;
			this.labelMaxStdDev.Text = "Max Std Dev";
			this.labelMaxStdDev.Visible = false;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(168, 24);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(64, 16);
			this.label7.TabIndex = 20;
			this.label7.Text = "Last Date";
			// 
			// textBoxMaxNumOfReturnedTickers
			// 
			this.textBoxMaxNumOfReturnedTickers.Location = new System.Drawing.Point(312, 64);
			this.textBoxMaxNumOfReturnedTickers.Name = "textBoxMaxNumOfReturnedTickers";
			this.textBoxMaxNumOfReturnedTickers.Size = new System.Drawing.Size(48, 20);
			this.textBoxMaxNumOfReturnedTickers.TabIndex = 23;
			this.textBoxMaxNumOfReturnedTickers.Text = "";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.groupBoxSelectionRule);
			this.panel2.Controls.Add(this.buttonSelectTickers);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.panel2.Location = new System.Drawing.Point(432, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(372, 478);
			this.panel2.TabIndex = 7;
			// 
			// labelMinStdDev
			// 
			this.labelMinStdDev.Location = new System.Drawing.Point(152, 152);
			this.labelMinStdDev.Name = "labelMinStdDev";
			this.labelMinStdDev.Size = new System.Drawing.Size(72, 23);
			this.labelMinStdDev.TabIndex = 35;
			this.labelMinStdDev.Text = "Min Std Dev";
			this.labelMinStdDev.Visible = false;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 96);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(120, 16);
			this.label3.TabIndex = 26;
			this.label3.Text = "Choose selection rule";
			// 
			// comboBoxAvailableSelectionRules
			// 
			this.comboBoxAvailableSelectionRules.Location = new System.Drawing.Point(8, 120);
			this.comboBoxAvailableSelectionRules.Name = "comboBoxAvailableSelectionRules";
			this.comboBoxAvailableSelectionRules.Size = new System.Drawing.Size(176, 21);
			this.comboBoxAvailableSelectionRules.TabIndex = 25;
			this.comboBoxAvailableSelectionRules.Text = "comboBox1";
			this.comboBoxAvailableSelectionRules.SelectedValueChanged += new System.EventHandler(this.comboBoxAvailableSelectionRules_SelectedValueChanged);
			// 
			// textBoxMaxStdDev
			// 
			this.textBoxMaxStdDev.Location = new System.Drawing.Point(232, 200);
			this.textBoxMaxStdDev.Name = "textBoxMaxStdDev";
			this.textBoxMaxStdDev.Size = new System.Drawing.Size(56, 20);
			this.textBoxMaxStdDev.TabIndex = 36;
			this.textBoxMaxStdDev.Text = "";
			this.textBoxMaxStdDev.Visible = false;
			// 
			// textBoxGroupID
			// 
			this.textBoxGroupID.Location = new System.Drawing.Point(72, 64);
			this.textBoxGroupID.Name = "textBoxGroupID";
			this.textBoxGroupID.Size = new System.Drawing.Size(88, 20);
			this.textBoxGroupID.TabIndex = 21;
			this.textBoxGroupID.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 16);
			this.label1.TabIndex = 22;
			this.label1.Text = "GroupID";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(160, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(152, 24);
			this.label2.TabIndex = 24;
			this.label2.Text = "Max Num of returned tickers";
			// 
			// labelMarketIndexKey
			// 
			this.labelMarketIndexKey.Location = new System.Drawing.Point(88, 176);
			this.labelMarketIndexKey.Name = "labelMarketIndexKey";
			this.labelMarketIndexKey.Size = new System.Drawing.Size(96, 23);
			this.labelMarketIndexKey.TabIndex = 29;
			this.labelMarketIndexKey.Text = "Market index key:";
			this.labelMarketIndexKey.Visible = false;
			// 
			// textBoxMarketIndex
			// 
			this.textBoxMarketIndex.Location = new System.Drawing.Point(192, 176);
			this.textBoxMarketIndex.Name = "textBoxMarketIndex";
			this.textBoxMarketIndex.Size = new System.Drawing.Size(88, 20);
			this.textBoxMarketIndex.TabIndex = 28;
			this.textBoxMarketIndex.Text = "";
			this.textBoxMarketIndex.Visible = false;
			// 
			// dateTimePickerLastDate
			// 
			this.dateTimePickerLastDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTimePickerLastDate.Location = new System.Drawing.Point(240, 24);
			this.dateTimePickerLastDate.Name = "dateTimePickerLastDate";
			this.dateTimePickerLastDate.Size = new System.Drawing.Size(88, 20);
			this.dateTimePickerLastDate.TabIndex = 15;
			// 
			// dataGrid1
			// 
			this.dataGrid1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.dataGrid1.DataMember = "";
			this.dataGrid1.Dock = System.Windows.Forms.DockStyle.Left;
			this.dataGrid1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid1.Location = new System.Drawing.Point(0, 0);
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.Size = new System.Drawing.Size(432, 478);
			this.dataGrid1.TabIndex = 2;
			// 
			// dateTimePickerFirstDate
			// 
			this.dateTimePickerFirstDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTimePickerFirstDate.Location = new System.Drawing.Point(72, 24);
			this.dateTimePickerFirstDate.Name = "dateTimePickerFirstDate";
			this.dateTimePickerFirstDate.Size = new System.Drawing.Size(88, 20);
			this.dateTimePickerFirstDate.TabIndex = 13;
			// 
			// groupBoxSelectionRule
			// 
			this.groupBoxSelectionRule.Controls.Add(this.labelMaxStdDev);
			this.groupBoxSelectionRule.Controls.Add(this.textBoxMaxStdDev);
			this.groupBoxSelectionRule.Controls.Add(this.labelMinStdDev);
			this.groupBoxSelectionRule.Controls.Add(this.textBoxMinStdDev);
			this.groupBoxSelectionRule.Controls.Add(this.labelMaxPrice);
			this.groupBoxSelectionRule.Controls.Add(this.textBoxMaxPrice);
			this.groupBoxSelectionRule.Controls.Add(this.labelMinPrice);
			this.groupBoxSelectionRule.Controls.Add(this.textBoxMinPrice);
			this.groupBoxSelectionRule.Controls.Add(this.labelMarketIndexKey);
			this.groupBoxSelectionRule.Controls.Add(this.textBoxMarketIndex);
			this.groupBoxSelectionRule.Controls.Add(this.checkBoxASCMode);
			this.groupBoxSelectionRule.Controls.Add(this.label3);
			this.groupBoxSelectionRule.Controls.Add(this.comboBoxAvailableSelectionRules);
			this.groupBoxSelectionRule.Controls.Add(this.label2);
			this.groupBoxSelectionRule.Controls.Add(this.textBoxMaxNumOfReturnedTickers);
			this.groupBoxSelectionRule.Controls.Add(this.label1);
			this.groupBoxSelectionRule.Controls.Add(this.textBoxGroupID);
			this.groupBoxSelectionRule.Controls.Add(this.label7);
			this.groupBoxSelectionRule.Controls.Add(this.label5);
			this.groupBoxSelectionRule.Controls.Add(this.dateTimePickerLastDate);
			this.groupBoxSelectionRule.Controls.Add(this.dateTimePickerFirstDate);
			this.groupBoxSelectionRule.Location = new System.Drawing.Point(8, 16);
			this.groupBoxSelectionRule.Name = "groupBoxSelectionRule";
			this.groupBoxSelectionRule.Size = new System.Drawing.Size(376, 248);
			this.groupBoxSelectionRule.TabIndex = 14;
			this.groupBoxSelectionRule.TabStop = false;
			this.groupBoxSelectionRule.Text = "Single Selection rule";
			// 
			// checkBoxASCMode
			// 
			this.checkBoxASCMode.Location = new System.Drawing.Point(200, 120);
			this.checkBoxASCMode.Name = "checkBoxASCMode";
			this.checkBoxASCMode.Size = new System.Drawing.Size(152, 24);
			this.checkBoxASCMode.TabIndex = 27;
			this.checkBoxASCMode.Text = "Order by ASC mode";
			// 
			// buttonSelectTickers
			// 
			this.buttonSelectTickers.Location = new System.Drawing.Point(136, 272);
			this.buttonSelectTickers.Name = "buttonSelectTickers";
			this.buttonSelectTickers.Size = new System.Drawing.Size(104, 24);
			this.buttonSelectTickers.TabIndex = 3;
			this.buttonSelectTickers.Text = "Select Tickers";
			this.buttonSelectTickers.Click += new System.EventHandler(this.buttonSelectTickers_Click);
			// 
			// labelMinPrice
			// 
			this.labelMinPrice.Location = new System.Drawing.Point(16, 152);
			this.labelMinPrice.Name = "labelMinPrice";
			this.labelMinPrice.Size = new System.Drawing.Size(56, 23);
			this.labelMinPrice.TabIndex = 31;
			this.labelMinPrice.Text = "Min Price";
			this.labelMinPrice.Visible = false;
			// 
			// textBoxMinStdDev
			// 
			this.textBoxMinStdDev.Location = new System.Drawing.Point(232, 152);
			this.textBoxMinStdDev.Name = "textBoxMinStdDev";
			this.textBoxMinStdDev.Size = new System.Drawing.Size(56, 20);
			this.textBoxMinStdDev.TabIndex = 34;
			this.textBoxMinStdDev.Text = "";
			this.textBoxMinStdDev.Visible = false;
			// 
			// textBoxMinPrice
			// 
			this.textBoxMinPrice.Location = new System.Drawing.Point(72, 152);
			this.textBoxMinPrice.Name = "textBoxMinPrice";
			this.textBoxMinPrice.Size = new System.Drawing.Size(56, 20);
			this.textBoxMinPrice.TabIndex = 30;
			this.textBoxMinPrice.Text = "";
			this.textBoxMinPrice.Visible = false;
			// 
			// TickerSelectorForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(804, 478);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.dataGrid1);
			this.Name = "TickerSelectorForm";
			this.Text = "Ticker Selector";
			this.panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			this.groupBoxSelectionRule.ResumeLayout(false);
			this.ResumeLayout(false);
		}
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
      ITickerSelector returnValue = new SelectorByLiquidity(this.textBoxGroupID.Text,
                                    this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
                                    this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text ));
      
      if(this.comboBoxAvailableSelectionRules.Text == "Liquidity")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByLiquidity(this.textBoxGroupID.Text,
                                                this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
                                                this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text ));
        else
          returnValue = new SelectorByLiquidity(this.tableOfSelectedTickers,
                                                this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
                                                this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
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
      else if (this.comboBoxAvailableSelectionRules.Text == "CloseToOpenVolatility")
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
      else if (this.comboBoxAvailableSelectionRules.Text == "CloseToOpenLinearCorrelation")
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
      else if (this.comboBoxAvailableSelectionRules.Text == "AverageCloseToOpenPerformance")
      { 
        if(this.textBoxGroupID.Text != "")
          returnValue = new SelectorByAverageOpenToClosePerformance(this.textBoxGroupID.Text,
            this.checkBoxASCMode.Checked, this.dateTimePickerFirstDate.Value,
            this.dateTimePickerLastDate.Value, Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
        else
          returnValue = new SelectorByAverageOpenToClosePerformance(this.tableOfSelectedTickers,
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
      return returnValue;  
    }

    private void setVisibilityForControls_QuotedAtEachMarketDay(bool showControls)
    {
      this.checkBoxASCMode.Enabled = showControls==false;
      this.labelMarketIndexKey.Visible = showControls;
      this.textBoxMarketIndex.Visible = showControls;
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
    private void comboBoxAvailableSelectionRules_SelectedValueChanged(object sender, System.EventArgs e)
    {
      if(this.comboBoxAvailableSelectionRules.Text == "QuotedAtEachMarketDay")
      {
      	this.setVisibilityForControls_QuotedAtEachMarketDay(true);
      }
      else if(this.comboBoxAvailableSelectionRules.Text == "AverageRawOpenPrice")
      {
        this.textBoxMarketIndex.Text = "";
        this.setVisibilityForControls_QuotedAtEachMarketDay(false);
        this.setVisibilityForControls_AverageRawOpenPrice(true);
      }
      else
      {
        this.textBoxMarketIndex.Text = "";
      	this.setVisibilityForControls_QuotedAtEachMarketDay(false);
        this.setVisibilityForControls_AverageRawOpenPrice(false);
      }
    }    

	}
}
