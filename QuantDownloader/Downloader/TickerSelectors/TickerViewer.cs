/*
QuantDownloader - Quantitative Finance Library

TickerViewer.cs
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
using QuantProject.DataAccess;
using QuantProject.DataAccess.Tables;
using QuantProject.Data.DataTables;
using QuantProject.Data.Selectors;
using QuantProject.ADT;

namespace QuantProject.Applications.Downloader.TickerSelectors
{
	/// <summary>
	/// It finds tickers that match criteria entered by the user
	/// </summary>
	public class TickerViewer : System.Windows.Forms.Form, ITickerSelector
	{
		private System.Windows.Forms.TextBox textBoxStringToFind;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.DataGrid dataGrid1;
		private System.Windows.Forms.Button buttonFindTickers;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.TextBox textBoxStringToFindInName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Splitter splitter1;
    private System.Windows.Forms.GroupBox groupBoxDateQuoteFilter;
    private System.Windows.Forms.RadioButton radioButtonDateQuoteFilter;
    private System.Windows.Forms.RadioButton radioButtonAnyTicker;
    private System.Windows.Forms.DateTimePicker dateTimePickerFirstDate;
    private System.Windows.Forms.DateTimePicker dateTimePickerLastDate;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.ComboBox comboBoxFirstOperator;
    private System.Windows.Forms.ComboBox comboBoxSecondOperator;
		private DataTable tableTickers;
    private System.Windows.Forms.Button buttonViewFaultyTickers;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.ComboBox comboBoxBelongingGroup;
    private bool skipRowChangedEvent;// event must be launched only by
                                     // user's changes
   		
    public TickerViewer()
		{
			InitializeComponent();
      this.dataGrid1.ContextMenu = new TickerViewerMenu(this);
      this.tableTickers = new DataTable("tickers");
      this.dataGrid1.DataSource = this.tableTickers;
      this.AcceptButton = this.buttonFindTickers;
      
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
      this.textBoxStringToFind = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.textBoxStringToFindInName = new System.Windows.Forms.TextBox();
      this.dataGrid1 = new System.Windows.Forms.DataGrid();
      this.buttonFindTickers = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.panel2 = new System.Windows.Forms.Panel();
      this.label4 = new System.Windows.Forms.Label();
      this.buttonViewFaultyTickers = new System.Windows.Forms.Button();
      this.groupBoxDateQuoteFilter = new System.Windows.Forms.GroupBox();
      this.comboBoxBelongingGroup = new System.Windows.Forms.ComboBox();
      this.label8 = new System.Windows.Forms.Label();
      this.comboBoxSecondOperator = new System.Windows.Forms.ComboBox();
      this.comboBoxFirstOperator = new System.Windows.Forms.ComboBox();
      this.label7 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.dateTimePickerLastDate = new System.Windows.Forms.DateTimePicker();
      this.dateTimePickerFirstDate = new System.Windows.Forms.DateTimePicker();
      this.radioButtonDateQuoteFilter = new System.Windows.Forms.RadioButton();
      this.radioButtonAnyTicker = new System.Windows.Forms.RadioButton();
      this.splitter1 = new System.Windows.Forms.Splitter();
      ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
      this.panel2.SuspendLayout();
      this.groupBoxDateQuoteFilter.SuspendLayout();
      this.SuspendLayout();
      // 
      // textBoxStringToFind
      // 
      this.textBoxStringToFind.Location = new System.Drawing.Point(48, 40);
      this.textBoxStringToFind.Name = "textBoxStringToFind";
      this.textBoxStringToFind.Size = new System.Drawing.Size(88, 20);
      this.textBoxStringToFind.TabIndex = 0;
      this.textBoxStringToFind.Text = "%";
      this.toolTip1.SetToolTip(this.textBoxStringToFind, "Type chars to filter tickers (you can use % and _ )  ");
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(48, 16);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(96, 16);
      this.label1.TabIndex = 1;
      this.label1.Text = "Ticker is like";
      // 
      // textBoxStringToFindInName
      // 
      this.textBoxStringToFindInName.AllowDrop = true;
      this.textBoxStringToFindInName.Location = new System.Drawing.Point(216, 40);
      this.textBoxStringToFindInName.Name = "textBoxStringToFindInName";
      this.textBoxStringToFindInName.Size = new System.Drawing.Size(120, 20);
      this.textBoxStringToFindInName.TabIndex = 4;
      this.textBoxStringToFindInName.Text = "%";
      this.toolTip1.SetToolTip(this.textBoxStringToFindInName, "Type chars to filter companies (you can use % and _ )  ");
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
      // buttonFindTickers
      // 
      this.buttonFindTickers.Location = new System.Drawing.Point(152, 264);
      this.buttonFindTickers.Name = "buttonFindTickers";
      this.buttonFindTickers.Size = new System.Drawing.Size(104, 24);
      this.buttonFindTickers.TabIndex = 0;
      this.buttonFindTickers.Text = "&Find Tickers";
      this.buttonFindTickers.Click += new System.EventHandler(this.buttonFindTickers_Click);
      // 
      // label3
      // 
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.label3.Location = new System.Drawing.Point(160, 40);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(32, 16);
      this.label3.TabIndex = 6;
      this.label3.Text = "AND";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(216, 16);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(120, 16);
      this.label2.TabIndex = 5;
      this.label2.Text = "Company Name is like";
      // 
      // panel2
      // 
      this.panel2.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                         this.label4,
                                                                         this.buttonViewFaultyTickers,
                                                                         this.groupBoxDateQuoteFilter,
                                                                         this.textBoxStringToFind,
                                                                         this.buttonFindTickers,
                                                                         this.textBoxStringToFindInName,
                                                                         this.label2,
                                                                         this.label3,
                                                                         this.label1});
      this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel2.ForeColor = System.Drawing.SystemColors.ControlText;
      this.panel2.Location = new System.Drawing.Point(432, 0);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(400, 478);
      this.panel2.TabIndex = 7;
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(24, 304);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(352, 16);
      this.label4.TabIndex = 16;
      this.label4.Text = "________________________________________";
      this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      // 
      // buttonViewFaultyTickers
      // 
      this.buttonViewFaultyTickers.Location = new System.Drawing.Point(144, 336);
      this.buttonViewFaultyTickers.Name = "buttonViewFaultyTickers";
      this.buttonViewFaultyTickers.Size = new System.Drawing.Size(120, 24);
      this.buttonViewFaultyTickers.TabIndex = 15;
      this.buttonViewFaultyTickers.Text = "&View faulty Tickers";
      this.buttonViewFaultyTickers.Click += new System.EventHandler(this.buttonViewFaultyTickers_Click);
      // 
      // groupBoxDateQuoteFilter
      // 
      this.groupBoxDateQuoteFilter.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                                          this.comboBoxBelongingGroup,
                                                                                          this.label8,
                                                                                          this.comboBoxSecondOperator,
                                                                                          this.comboBoxFirstOperator,
                                                                                          this.label7,
                                                                                          this.label5,
                                                                                          this.label6,
                                                                                          this.dateTimePickerLastDate,
                                                                                          this.dateTimePickerFirstDate,
                                                                                          this.radioButtonDateQuoteFilter,
                                                                                          this.radioButtonAnyTicker});
      this.groupBoxDateQuoteFilter.Location = new System.Drawing.Point(8, 80);
      this.groupBoxDateQuoteFilter.Name = "groupBoxDateQuoteFilter";
      this.groupBoxDateQuoteFilter.Size = new System.Drawing.Size(384, 168);
      this.groupBoxDateQuoteFilter.TabIndex = 14;
      this.groupBoxDateQuoteFilter.TabStop = false;
      this.groupBoxDateQuoteFilter.Text = "Quote filter";
      // 
      // comboBoxBelongingGroup
      // 
      this.comboBoxBelongingGroup.Enabled = false;
      this.comboBoxBelongingGroup.Location = new System.Drawing.Point(200, 128);
      this.comboBoxBelongingGroup.Name = "comboBoxBelongingGroup";
      this.comboBoxBelongingGroup.Size = new System.Drawing.Size(176, 21);
      this.comboBoxBelongingGroup.TabIndex = 24;
      this.comboBoxBelongingGroup.Text = "comboBox1";
      // 
      // label8
      // 
      this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.label8.Location = new System.Drawing.Point(24, 136);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(120, 16);
      this.label8.TabIndex = 23;
      this.label8.Text = "belonging to group";
      // 
      // comboBoxSecondOperator
      // 
      this.comboBoxSecondOperator.Enabled = false;
      this.comboBoxSecondOperator.Items.AddRange(new object[] {
                                                                "",
                                                                "<=",
                                                                ">="});
      this.comboBoxSecondOperator.Location = new System.Drawing.Point(200, 96);
      this.comboBoxSecondOperator.Name = "comboBoxSecondOperator";
      this.comboBoxSecondOperator.Size = new System.Drawing.Size(48, 21);
      this.comboBoxSecondOperator.TabIndex = 22;
      this.comboBoxSecondOperator.SelectedValueChanged += new System.EventHandler(this.comboBoxSecondOperator_SelectedValueChanged);
      // 
      // comboBoxFirstOperator
      // 
      this.comboBoxFirstOperator.Enabled = false;
      this.comboBoxFirstOperator.Items.AddRange(new object[] {
                                                               "",
                                                               "<=",
                                                               ">="});
      this.comboBoxFirstOperator.Location = new System.Drawing.Point(200, 64);
      this.comboBoxFirstOperator.Name = "comboBoxFirstOperator";
      this.comboBoxFirstOperator.Size = new System.Drawing.Size(48, 21);
      this.comboBoxFirstOperator.TabIndex = 21;
      this.comboBoxFirstOperator.SelectedValueChanged += new System.EventHandler(this.comboBoxFirstOperator_SelectedValueChanged);
      // 
      // label7
      // 
      this.label7.Location = new System.Drawing.Point(256, 96);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(32, 16);
      this.label7.TabIndex = 20;
      this.label7.Text = "than";
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(256, 64);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(32, 16);
      this.label5.TabIndex = 18;
      this.label5.Text = "than";
      // 
      // label6
      // 
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
      this.label6.Location = new System.Drawing.Point(24, 96);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(152, 16);
      this.label6.TabIndex = 17;
      this.label6.Text = "and last available quote is";
      // 
      // dateTimePickerLastDate
      // 
      this.dateTimePickerLastDate.Enabled = false;
      this.dateTimePickerLastDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.dateTimePickerLastDate.Location = new System.Drawing.Point(288, 96);
      this.dateTimePickerLastDate.Name = "dateTimePickerLastDate";
      this.dateTimePickerLastDate.Size = new System.Drawing.Size(88, 20);
      this.dateTimePickerLastDate.TabIndex = 15;
      // 
      // dateTimePickerFirstDate
      // 
      this.dateTimePickerFirstDate.Enabled = false;
      this.dateTimePickerFirstDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.dateTimePickerFirstDate.Location = new System.Drawing.Point(288, 64);
      this.dateTimePickerFirstDate.Name = "dateTimePickerFirstDate";
      this.dateTimePickerFirstDate.Size = new System.Drawing.Size(88, 20);
      this.dateTimePickerFirstDate.TabIndex = 13;
      // 
      // radioButtonDateQuoteFilter
      // 
      this.radioButtonDateQuoteFilter.Location = new System.Drawing.Point(8, 56);
      this.radioButtonDateQuoteFilter.Name = "radioButtonDateQuoteFilter";
      this.radioButtonDateQuoteFilter.Size = new System.Drawing.Size(184, 24);
      this.radioButtonDateQuoteFilter.TabIndex = 12;
      this.radioButtonDateQuoteFilter.Text = "Only tickers whose first quote is";
      this.radioButtonDateQuoteFilter.CheckedChanged += new System.EventHandler(this.radioButtonDateQuoteFilter_CheckedChanged);
      // 
      // radioButtonAnyTicker
      // 
      this.radioButtonAnyTicker.Checked = true;
      this.radioButtonAnyTicker.Location = new System.Drawing.Point(8, 24);
      this.radioButtonAnyTicker.Name = "radioButtonAnyTicker";
      this.radioButtonAnyTicker.Size = new System.Drawing.Size(248, 24);
      this.radioButtonAnyTicker.TabIndex = 10;
      this.radioButtonAnyTicker.TabStop = true;
      this.radioButtonAnyTicker.Text = "Any ticker, with or without quotes";
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
      // TickerViewer
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(832, 478);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.splitter1,
                                                                  this.panel2,
                                                                  this.dataGrid1});
      this.Name = "TickerViewer";
      this.Text = "Ticker Viewer";
      ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
      this.panel2.ResumeLayout(false);
      this.groupBoxDateQuoteFilter.ResumeLayout(false);
      this.ResumeLayout(false);

    }
		#endregion


		#region Style for dataGrid
		
		private void setStyle_dataGridWithQuoteInfo()
		{
			DataGridTableStyle dataGrid1TableStyle = new DataGridTableStyle();
			dataGrid1TableStyle.MappingName = this.tableTickers.TableName;
			dataGrid1TableStyle.ColumnHeadersVisible = true;
			dataGrid1TableStyle.ReadOnly = true;
			dataGrid1TableStyle.SelectionBackColor = Color.DimGray ;
			
      DataGridTextBoxColumn columnStyle_tiTicker = new DataGridTextBoxColumn();
			columnStyle_tiTicker.MappingName = "tiTicker";
			columnStyle_tiTicker.HeaderText = "Ticker";
			columnStyle_tiTicker.TextBox.Enabled = false;
			columnStyle_tiTicker.NullText = "";
			columnStyle_tiTicker.Width = 60;
			DataGridTextBoxColumn columnStyle_tiCompanyName = new DataGridTextBoxColumn();
			columnStyle_tiCompanyName.MappingName = "tiCompanyName";
			columnStyle_tiCompanyName.HeaderText = "Company Name";
			columnStyle_tiCompanyName.TextBox.Enabled = false;
			columnStyle_tiCompanyName.NullText = "";
			columnStyle_tiCompanyName.Width = 120;
      DataGridTextBoxColumn columnStyle_FirstQuote = new DataGridTextBoxColumn();
      columnStyle_FirstQuote.MappingName = "FirstQuote";
      columnStyle_FirstQuote.HeaderText = "First Quote";
      columnStyle_FirstQuote.TextBox.Enabled = false;
      columnStyle_FirstQuote.NullText = "";
      columnStyle_FirstQuote.Width = 60;
      DataGridTextBoxColumn columnStyle_LastQuote = new DataGridTextBoxColumn();
      columnStyle_LastQuote.MappingName = "LastQuote";
      columnStyle_LastQuote.HeaderText = "Last Quote";
      columnStyle_LastQuote.TextBox.Enabled = false;
      columnStyle_LastQuote.NullText = "";
      columnStyle_LastQuote.Width = 60;
      
      DataGridTextBoxColumn columnStyle_NumberOfQuotes = new DataGridTextBoxColumn();
      columnStyle_NumberOfQuotes.MappingName = "NumberOfQuotes";
      columnStyle_NumberOfQuotes.HeaderText = "# Quotes";
      columnStyle_NumberOfQuotes.TextBox.Enabled = false;
      columnStyle_NumberOfQuotes.NullText = "";
      columnStyle_NumberOfQuotes.Width = 60;

      dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_tiTicker);
			dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_tiCompanyName);
      dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_FirstQuote);
      dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_LastQuote);
      dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_NumberOfQuotes); 
 
			this.dataGrid1.TableStyles.Add(dataGrid1TableStyle);

      
			
		}

    private void setStyle_dataGridWithoutQuoteInfo()
    {
      DataGridTableStyle dataGrid1TableStyle = new DataGridTableStyle();
      dataGrid1TableStyle.MappingName = this.tableTickers.TableName;
      dataGrid1TableStyle.ColumnHeadersVisible = true;
      dataGrid1TableStyle.ReadOnly = false;
      dataGrid1TableStyle.SelectionBackColor = Color.DimGray;
			
      DataGridTextBoxColumn columnStyle_tiTicker = new DataGridTextBoxColumn();
      columnStyle_tiTicker.MappingName = "tiTicker";
      columnStyle_tiTicker.HeaderText = "Ticker";
      columnStyle_tiTicker.TextBox.Enabled = true;
      columnStyle_tiTicker.NullText = "";
      columnStyle_tiTicker.Width = 60;
      DataGridTextBoxColumn columnStyle_tiCompanyName = new DataGridTextBoxColumn();
      columnStyle_tiCompanyName.MappingName = "tiCompanyName";
      columnStyle_tiCompanyName.HeaderText = "Company Name";
      columnStyle_tiCompanyName.TextBox.Enabled = true;
      columnStyle_tiCompanyName.NullText = "";
      columnStyle_tiCompanyName.Width = 120;
  
      dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_tiTicker);
      dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_tiCompanyName);

      this.dataGrid1.TableStyles.Add(dataGrid1TableStyle);
			
    }

    private void setStyle_faultyTickersDataGrid()
    {
      DataGridTableStyle dataGrid1TableStyle = new DataGridTableStyle();
      dataGrid1TableStyle.MappingName = this.tableTickers.TableName;
      dataGrid1TableStyle.ColumnHeadersVisible = true;
      dataGrid1TableStyle.ReadOnly = true;
      dataGrid1TableStyle.SelectionBackColor = Color.DimGray ;
			
      DataGridTextBoxColumn columnStyle_ftTicker = new DataGridTextBoxColumn();
      columnStyle_ftTicker.MappingName = "ftTicker";
      columnStyle_ftTicker.HeaderText = "Ticker not downloaded";
      columnStyle_ftTicker.TextBox.Enabled = false;
      columnStyle_ftTicker.NullText = "";
      columnStyle_ftTicker.Width = 100;
      DataGridTextBoxColumn columnStyle_ftDate = new DataGridTextBoxColumn();
      columnStyle_ftDate.MappingName = "ftDate";
      columnStyle_ftDate.HeaderText = "Last attempt";
      columnStyle_ftDate.TextBox.Enabled = false;
      columnStyle_ftDate.NullText = "";
      columnStyle_ftDate.Width = 80;
  
      dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_ftTicker);
      dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_ftDate);

      this.dataGrid1.TableStyles.Add(dataGrid1TableStyle);
			
    }
		#endregion



		private void buttonFindTickers_Click(object sender, System.EventArgs e)
		{

			try
			{
        Cursor.Current = Cursors.WaitCursor;
        if(this.radioButtonAnyTicker.Checked)
        //all tickers with or without quotes
        {
          this.tableTickers = Tickers.GetTableOfFilteredTickers(this.textBoxStringToFind.Text,
                                                                this.textBoxStringToFindInName.Text);
          
          this.tableTickers.Columns[Tickers.CompanyName].AllowDBNull = false;
          this.tableTickers.Columns[Tickers.CompanyName].DefaultValue = "-";
          this.tableTickers.Columns[Tickers.Ticker].AllowDBNull = false;
          this.tableTickers.Columns[Tickers.Ticker].DefaultValue = "tickerSymbol";

          this.tableTickers.RowChanged += new DataRowChangeEventHandler(this.row_Changed);
          this.tableTickers.RowDeleted += new DataRowChangeEventHandler(this.row_Deleted);
          this.dataGrid1.DataSource = this.tableTickers;
          this.setStyle_dataGridWithoutQuoteInfo();
          this.dataGrid1.ReadOnly = false;
        }
        else
        //all tickers with first date quote and last date quote within a specified interval
        //(and, if specified, belonging to a given group)
        {
          if(((string)this.comboBoxBelongingGroup.SelectedValue).Length == 0)
          //no group has been selected
            this.tableTickers = Tickers.GetTableOfFilteredTickers(this.textBoxStringToFind.Text,
                                                                this.textBoxStringToFindInName.Text,
                                                                this.comboBoxFirstOperator.Text,
                                                                this.dateTimePickerFirstDate.Value,
                                                                this.comboBoxSecondOperator.Text,
                                                                this.dateTimePickerLastDate.Value);
          else
            this.tableTickers = Tickers.GetTableOfFilteredTickers(this.textBoxStringToFind.Text,
                                                                this.textBoxStringToFindInName.Text,
                                                                this.comboBoxFirstOperator.Text,
                                                                this.dateTimePickerFirstDate.Value,
                                                                this.comboBoxSecondOperator.Text,
                                                                this.dateTimePickerLastDate.Value,
                                                                (string)this.comboBoxBelongingGroup.SelectedValue);
          this.dataGrid1.DataSource = this.tableTickers;
          this.setStyle_dataGridWithQuoteInfo();
          this.dataGrid1.ReadOnly = true;
        }        
      }
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		
		}
	
    // implementation of ITickerSelector interface

		public DataTable GetTableOfSelectedTickers()
		{
			/*
      DataTable dataTableOfDataGrid1 = (DataTable)this.dataGrid1.DataSource;
			DataTable tableOfSelectedTickers = new DataTable();
      TickerDataTable.AddColumnsOfTickerTable(tableOfSelectedTickers);
			int indexOfRow = 0;
			while(indexOfRow != dataTableOfDataGrid1.Rows.Count)
			{
				if(this.dataGrid1.IsSelected(indexOfRow))
				{
					DataRow dataRow = tableOfSelectedTickers.NewRow(); 
					dataRow["Ticker"] = (string)dataTableOfDataGrid1.Rows[indexOfRow][0];
					dataRow["CompanyName"] = (string)dataTableOfDataGrid1.Rows[indexOfRow][1];
					tableOfSelectedTickers.Rows.Add(dataRow);
				}
				indexOfRow++;
			}*/
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
    private void radioButtonDateQuoteFilter_checkedChanged_fillComboBoxBelongingGroup()
    {
      if(this.comboBoxBelongingGroup.DataSource == null)
        //data source not set yet
      {
        DataTable groups = SqlExecutor.GetDataTable("SELECT tgId AS GroupID, tgDescription AS GroupDescription FROM tickerGroups " +
                                                    "UNION " +
                                                    "SELECT '' AS GroupID, '' AS GroupDescription FROM tickerGroups " +
                                                    "ORDER BY GroupDescription");
        this.comboBoxBelongingGroup.DataSource = groups;
        this.comboBoxBelongingGroup.DisplayMember = "GroupDescription";
        this.comboBoxBelongingGroup.ValueMember = "GroupID";

      }
      
    }
    private void radioButtonDateQuoteFilter_CheckedChanged(object sender, System.EventArgs e)
    {
      if(this.radioButtonDateQuoteFilter.Checked == true)
      {
        this.comboBoxFirstOperator.Enabled = true;
        this.comboBoxFirstOperator.Text = "";
        this.comboBoxSecondOperator.Enabled = true;
        this.comboBoxSecondOperator.Text = "";
        this.comboBoxBelongingGroup.Enabled = true;
        this.radioButtonDateQuoteFilter_checkedChanged_fillComboBoxBelongingGroup();
        this.comboBoxBelongingGroup.Text = "";
        
  
      }
      else
      {
        this.dateTimePickerFirstDate.Enabled = false;
        this.dateTimePickerLastDate.Enabled = false;
        this.comboBoxFirstOperator.Enabled = false;
        this.comboBoxSecondOperator.Enabled = false;
        this.comboBoxBelongingGroup.Enabled = false;
      }
    }
        
    private void row_Changed( object sender, DataRowChangeEventArgs e )
    {
      this.rowModified_Manager(e);
    }
    
    private void row_Deleted( object sender, DataRowChangeEventArgs e )
    {
      this.rowModified_Manager(e);
    }
    
    private void rowModified_Manager(DataRowChangeEventArgs rowChangeEventArgs)
    {
      if(this.skipRowChangedEvent)
        return;
      DialogResult userAnswer = MessageBox.Show( "Do you want to commit these changes permanently to the database?",
        "Confirmation for permanent commit",
        MessageBoxButtons.YesNo);

      if(userAnswer == DialogResult.Yes)
      {
        this.saveChangesToCurrentRow(rowChangeEventArgs);
      }
      else
      {
        this.skipRowChangedEvent = true;
        this.tableTickers.RejectChanges();
        this.skipRowChangedEvent = false;
      }
    }
    
    private void saveChangesToCurrentRow(DataRowChangeEventArgs rowChangeEventArgs)
    {
      try
      {
        if(rowChangeEventArgs.Action == DataRowAction.Add)
        {
          string sqlInsertString = 
              "INSERT INTO tickers(tiTicker, tiCompanyName) VALUES('" +
              (string)rowChangeEventArgs.Row["tiTicker"] + "', '" +
              (string)rowChangeEventArgs.Row["tiCompanyName"] + "')";
          SqlExecutor.ExecuteNonQuery(sqlInsertString);
          this.skipRowChangedEvent = true;
        }
        else
        {
          DataTable changedData = this.tableTickers.GetChanges();
          this.skipRowChangedEvent = true;
          OleDbSingleTableAdapter adapter = new OleDbSingleTableAdapter();
          adapter.SetAdapter("tickers");
          adapter.OleDbDataAdapter.Update(changedData);
          this.tableTickers.AcceptChanges();
        }
       }
      catch(Exception ex)
      {
        MessageBox.Show(ex.ToString());
        this.tableTickers.RejectChanges();
      }
      finally
      {
        this.skipRowChangedEvent = false;
      }
    }

    private void buttonViewFaultyTickers_Click(object sender, System.EventArgs e)
    {
      try
      {
        Cursor.Current = Cursors.WaitCursor;
      
        this.tableTickers = new FaultyTickers().Table;

        this.dataGrid1.DataSource = this.tableTickers;
        this.setStyle_faultyTickersDataGrid();
        this.dataGrid1.ReadOnly = false;
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
    
    }

    private void comboBoxFirstOperator_SelectedValueChanged(object sender, System.EventArgs e)
    {
      if(this.comboBoxFirstOperator.Text.Length > 0)
        this.dateTimePickerFirstDate.Enabled = true;
      else
        this.dateTimePickerFirstDate.Enabled = false;
    }

    private void comboBoxSecondOperator_SelectedValueChanged(object sender, System.EventArgs e)
    {
      if(this.comboBoxSecondOperator.Text.Length > 0)
        this.dateTimePickerLastDate.Enabled = true;
      else
        this.dateTimePickerLastDate.Enabled = false;
    }


	}
}
