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
	/// It is the user interface for the TickerSelector class
	/// </summary>
	public class TickerSelectorForm : System.Windows.Forms.Form, ITickerSelector
	{
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.DataGrid dataGrid1;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Splitter splitter1;
    private System.Windows.Forms.DateTimePicker dateTimePickerFirstDate;
    private System.Windows.Forms.DateTimePicker dateTimePickerLastDate;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Button buttonSelectTickers;
    private System.Windows.Forms.GroupBox groupBoxSelectionRule;
    private System.Windows.Forms.TextBox textBoxGroupID;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox comboBoxAvailableSelectionRules;
    private System.Windows.Forms.TextBox textBoxMaxNumOfReturnedTickers;
    private System.Windows.Forms.Label label3;
    private DataTable tableOfSelectedTickers;

		public TickerSelectorForm()
		{
			InitializeComponent();
      this.dateTimePickerFirstDate.Value = ConstantsProvider.InitialDateTimeForDownload;
      this.dateTimePickerLastDate.Value = DateTime.Now;
      this.dataGrid1.ContextMenu = new TickerViewerMenu(this);
      //TODO: complete comboBox's code with all possible types of selections
      this.comboBoxAvailableSelectionRules.Text = "Most liquid instrument";
      this.comboBoxAvailableSelectionRules.Items.Add("Most liquid instrument");

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
		private void InitializeComponent()
		{
      this.components = new System.ComponentModel.Container();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.dataGrid1 = new System.Windows.Forms.DataGrid();
      this.buttonSelectTickers = new System.Windows.Forms.Button();
      this.panel2 = new System.Windows.Forms.Panel();
      this.groupBoxSelectionRule = new System.Windows.Forms.GroupBox();
      this.label3 = new System.Windows.Forms.Label();
      this.comboBoxAvailableSelectionRules = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.textBoxMaxNumOfReturnedTickers = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.textBoxGroupID = new System.Windows.Forms.TextBox();
      this.label7 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.dateTimePickerLastDate = new System.Windows.Forms.DateTimePicker();
      this.dateTimePickerFirstDate = new System.Windows.Forms.DateTimePicker();
      this.splitter1 = new System.Windows.Forms.Splitter();
      ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
      this.panel2.SuspendLayout();
      this.groupBoxSelectionRule.SuspendLayout();
      this.SuspendLayout();
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
      // buttonSelectTickers
      // 
      this.buttonSelectTickers.Location = new System.Drawing.Point(136, 256);
      this.buttonSelectTickers.Name = "buttonSelectTickers";
      this.buttonSelectTickers.Size = new System.Drawing.Size(104, 24);
      this.buttonSelectTickers.TabIndex = 3;
      this.buttonSelectTickers.Text = "Select Tickers";
      this.buttonSelectTickers.Click += new System.EventHandler(this.buttonSelectTickers_Click);
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
      this.panel2.Size = new System.Drawing.Size(392, 478);
      this.panel2.TabIndex = 7;
      // 
      // groupBoxSelectionRule
      // 
      this.groupBoxSelectionRule.Controls.AddRange(new System.Windows.Forms.Control[] {
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
      this.groupBoxSelectionRule.Location = new System.Drawing.Point(8, 16);
      this.groupBoxSelectionRule.Name = "groupBoxSelectionRule";
      this.groupBoxSelectionRule.Size = new System.Drawing.Size(376, 144);
      this.groupBoxSelectionRule.TabIndex = 14;
      this.groupBoxSelectionRule.TabStop = false;
      this.groupBoxSelectionRule.Text = "Single Selection rule";
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(48, 112);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(120, 16);
      this.label3.TabIndex = 26;
      this.label3.Text = "Choose selection rule";
      // 
      // comboBoxAvailableSelectionRules
      // 
      this.comboBoxAvailableSelectionRules.Location = new System.Drawing.Point(176, 112);
      this.comboBoxAvailableSelectionRules.Name = "comboBoxAvailableSelectionRules";
      this.comboBoxAvailableSelectionRules.Size = new System.Drawing.Size(176, 21);
      this.comboBoxAvailableSelectionRules.TabIndex = 25;
      this.comboBoxAvailableSelectionRules.Text = "comboBox1";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(160, 64);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(152, 24);
      this.label2.TabIndex = 24;
      this.label2.Text = "Max Num of returned tickers";
      // 
      // textBoxMaxNumOfReturnedTickers
      // 
      this.textBoxMaxNumOfReturnedTickers.Location = new System.Drawing.Point(312, 64);
      this.textBoxMaxNumOfReturnedTickers.Name = "textBoxMaxNumOfReturnedTickers";
      this.textBoxMaxNumOfReturnedTickers.Size = new System.Drawing.Size(48, 20);
      this.textBoxMaxNumOfReturnedTickers.TabIndex = 23;
      this.textBoxMaxNumOfReturnedTickers.Text = "";
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(8, 64);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(64, 16);
      this.label1.TabIndex = 22;
      this.label1.Text = "GroupID";
      // 
      // textBoxGroupID
      // 
      this.textBoxGroupID.Location = new System.Drawing.Point(72, 64);
      this.textBoxGroupID.Name = "textBoxGroupID";
      this.textBoxGroupID.Size = new System.Drawing.Size(88, 20);
      this.textBoxGroupID.TabIndex = 21;
      this.textBoxGroupID.Text = "";
      // 
      // label7
      // 
      this.label7.Location = new System.Drawing.Point(168, 24);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(64, 16);
      this.label7.TabIndex = 20;
      this.label7.Text = "Last Date";
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(8, 24);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(64, 16);
      this.label5.TabIndex = 18;
      this.label5.Text = "First Date";
      // 
      // dateTimePickerLastDate
      // 
      this.dateTimePickerLastDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.dateTimePickerLastDate.Location = new System.Drawing.Point(240, 24);
      this.dateTimePickerLastDate.Name = "dateTimePickerLastDate";
      this.dateTimePickerLastDate.Size = new System.Drawing.Size(88, 20);
      this.dateTimePickerLastDate.TabIndex = 15;
      // 
      // dateTimePickerFirstDate
      // 
      this.dateTimePickerFirstDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.dateTimePickerFirstDate.Location = new System.Drawing.Point(72, 24);
      this.dateTimePickerFirstDate.Name = "dateTimePickerFirstDate";
      this.dateTimePickerFirstDate.Size = new System.Drawing.Size(88, 20);
      this.dateTimePickerFirstDate.TabIndex = 13;
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
      // TickerSelectorForm
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(824, 478);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.splitter1,
                                                                  this.panel2,
                                                                  this.dataGrid1});
      this.Name = "TickerSelectorForm";
      this.Text = "Ticker Selector";
      ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
      this.panel2.ResumeLayout(false);
      this.groupBoxSelectionRule.ResumeLayout(false);
      this.ResumeLayout(false);

    }
		#endregion






		
	
    // implementation of ITickerSelector interface

		public TickerDataTable GetTableOfSelectedTickers()
		{
			DataTable dataTableOfDataGrid1 = (DataTable)this.dataGrid1.DataSource;
			TickerDataTable tableOfSelectedTickers = new TickerDataTable();
			int indexOfRow = 0;
			while(indexOfRow != dataTableOfDataGrid1.Rows.Count)
			{
				if(this.dataGrid1.IsSelected(indexOfRow))
				{
					DataRow dataRow = tableOfSelectedTickers.NewRow(); 
					dataRow[0] = (string)dataTableOfDataGrid1.Rows[indexOfRow][0];
					dataRow[1] = (string)dataTableOfDataGrid1.Rows[indexOfRow][1];
					tableOfSelectedTickers.Rows.Add(dataRow);
				}
				indexOfRow++;
			}
			return tableOfSelectedTickers;
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
        //TODO: complete code finding a way to construct the right 
        // selection rule on the base of what selected by the user
        SelectionRule rule = new SelectionRule(SelectionType.MostLiquid, this.textBoxGroupID.Text, 
                                            this.dateTimePickerFirstDate.Value,
                                            this.dateTimePickerLastDate.Value,
                                            Int32.Parse(this.textBoxMaxNumOfReturnedTickers.Text));
        TickerSelector selector;
        if(this.textBoxGroupID.Text != "")
          selector= new TickerSelector(rule);
        else
          selector= new TickerSelector(this.tableOfSelectedTickers,rule);
        this.dataGrid1.DataSource = selector.GetTableOfSelectedTickers();
        this.dataGrid1.Refresh();
                        
    }

	}
}
