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

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// It finds tickers that match criteria entered by the user
	/// </summary>
	public class TickerViewer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBoxStringToFind;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.DataGrid dataGrid1;
		private System.Windows.Forms.Button buttonFindTickers;
		private System.ComponentModel.IContainer components;
		private OleDbConnection oleDbConnection;
		private OleDbDataAdapter oleDbDataAdapter;
		private System.Windows.Forms.ContextMenu contextMenuTickerViewer;
		private System.Windows.Forms.MenuItem menuItemValidateCurrentRows;
		private System.Windows.Forms.MenuItem menuItemDownloadCurrentRows;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox textBoxStringToFindInName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.MenuItem menuItemCopySelectedTickersToClipboard;
		private DataTable tableTickers;

		public TickerViewer()
		{
			InitializeComponent();
			this.oleDbConnection = ConnectionProvider.OleDbConnection;
			this.tableTickers = new DataTable("tickers");
			this.dataGrid1.DataSource = this.tableTickers;
			this.setStyle_dataGrid1();
			this.dataGrid1.Visible = false;
			//the datagrid is still empty at this point
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
			this.contextMenuTickerViewer = new System.Windows.Forms.ContextMenu();
			this.menuItemValidateCurrentRows = new System.Windows.Forms.MenuItem();
			this.menuItemDownloadCurrentRows = new System.Windows.Forms.MenuItem();
			this.buttonFindTickers = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.menuItemCopySelectedTickersToClipboard = new System.Windows.Forms.MenuItem();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBoxStringToFind
			// 
			this.textBoxStringToFind.Location = new System.Drawing.Point(16, 32);
			this.textBoxStringToFind.Name = "textBoxStringToFind";
			this.textBoxStringToFind.Size = new System.Drawing.Size(136, 20);
			this.textBoxStringToFind.TabIndex = 0;
			this.textBoxStringToFind.Text = "%";
			this.toolTip1.SetToolTip(this.textBoxStringToFind, "Type chars to filter tickers (you can use % and _ )  ");
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(112, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Find Ticker is like";
			// 
			// textBoxStringToFindInName
			// 
			this.textBoxStringToFindInName.AllowDrop = true;
			this.textBoxStringToFindInName.Location = new System.Drawing.Point(16, 112);
			this.textBoxStringToFindInName.Name = "textBoxStringToFindInName";
			this.textBoxStringToFindInName.Size = new System.Drawing.Size(136, 20);
			this.textBoxStringToFindInName.TabIndex = 4;
			this.textBoxStringToFindInName.Text = "%";
			this.toolTip1.SetToolTip(this.textBoxStringToFindInName, "Type chars to filter companies (you can use % and _ )  ");
			// 
			// dataGrid1
			// 
			this.dataGrid1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.dataGrid1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.dataGrid1.ContextMenu = this.contextMenuTickerViewer;
			this.dataGrid1.DataMember = "";
			this.dataGrid1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.Size = new System.Drawing.Size(270, 478);
			this.dataGrid1.TabIndex = 2;
			// 
			// contextMenuTickerViewer
			// 
			this.contextMenuTickerViewer.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																									this.menuItemValidateCurrentRows,
																									this.menuItemDownloadCurrentRows,
																									this.menuItemCopySelectedTickersToClipboard});
			// 
			// menuItemValidateCurrentRows
			// 
			this.menuItemValidateCurrentRows.Index = 0;
			this.menuItemValidateCurrentRows.Text = "Validate selected tickers";
			this.menuItemValidateCurrentRows.Click += new System.EventHandler(this.menuItemValidateCurrentRows_Click);
			// 
			// menuItemDownloadCurrentRows
			// 
			this.menuItemDownloadCurrentRows.Index = 1;
			this.menuItemDownloadCurrentRows.Text = "Download quotes of selected tickers";
			this.menuItemDownloadCurrentRows.Click += new System.EventHandler(this.menuItemDownloadCurrentRows_Click);
			// 
			// buttonFindTickers
			// 
			this.buttonFindTickers.Location = new System.Drawing.Point(56, 200);
			this.buttonFindTickers.Name = "buttonFindTickers";
			this.buttonFindTickers.Size = new System.Drawing.Size(64, 24);
			this.buttonFindTickers.TabIndex = 3;
			this.buttonFindTickers.Text = "Go";
			this.buttonFindTickers.Click += new System.EventHandler(this.buttonFindTickers_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.dataGrid1});
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(288, 478);
			this.panel1.TabIndex = 4;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.Location = new System.Drawing.Point(72, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 16);
			this.label3.TabIndex = 6;
			this.label3.Text = "AND";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(136, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "Company Name is like";
			// 
			// panel2
			// 
			this.panel2.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.textBoxStringToFind,
																				 this.buttonFindTickers,
																				 this.textBoxStringToFindInName,
																				 this.label2,
																				 this.label3,
																				 this.label1});
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(288, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(184, 478);
			this.panel2.TabIndex = 7;
			// 
			// splitter1
			// 
			this.splitter1.BackColor = System.Drawing.SystemColors.Highlight;
			this.splitter1.Location = new System.Drawing.Point(288, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 478);
			this.splitter1.TabIndex = 8;
			this.splitter1.TabStop = false;
			// 
			// menuItemCopySelectedTickersToClipboard
			// 
			this.menuItemCopySelectedTickersToClipboard.Index = 2;
			this.menuItemCopySelectedTickersToClipboard.Text = "Copy selected tickers to clipboard";
			this.menuItemCopySelectedTickersToClipboard.Click += new System.EventHandler(this.menuItemCopySelectedTickersToClipboard_Click);
			// 
			// TickerViewer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(472, 478);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.splitter1,
																		  this.panel2,
																		  this.panel1});
			this.Name = "TickerViewer";
			this.Text = "Ticker Viewer";
			this.Closed += new System.EventHandler(this.TickerViewer_Closed);
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		#region Style for dataGrid1
		
		private void setStyle_dataGrid1()
		{
			DataGridTableStyle dataGrid1TableStyle = new DataGridTableStyle();
			//this.dataGrid1.Width = 300;
			dataGrid1TableStyle.MappingName = "tickers";
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
			columnStyle_tiCompanyName.Width = 150;
			dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_tiTicker);
			dataGrid1TableStyle.GridColumnStyles.Add(columnStyle_tiCompanyName);
			this.dataGrid1.TableStyles.Add(dataGrid1TableStyle);
			
		}
		#endregion


		private void TickerViewer_Closed(object sender, System.EventArgs e)
		{
			this.oleDbConnection.Close();
		}

		private void buttonFindTickers_Click(object sender, System.EventArgs e)
		{

			try
			{
				Cursor.Current = Cursors.WaitCursor;
				if(this.oleDbConnection.State != ConnectionState.Open)
					this.oleDbConnection.Open();
				string criteria = "SELECT * FROM tickers WHERE tiTicker LIKE '" +
									this.textBoxStringToFind.Text + "'" +
									" AND tiCompanyName LIKE '" +
									this.textBoxStringToFindInName.Text + "'";
				oleDbDataAdapter  = new OleDbDataAdapter(criteria, this.oleDbConnection);
				this.tableTickers.Clear();
				oleDbDataAdapter.Fill(this.tableTickers);
				this.dataGrid1.Refresh();
				
				if (this.dataGrid1.Visible == false && 
					this.tableTickers.Rows.Count != 0)
				//there are some rows to show and the grid is not visible
				{
					this.dataGrid1.Visible = true; 
				}
				else if(this.tableTickers.Rows.Count == 0)
				// there aren't rows to show
				{
                    this.dataGrid1.Visible = false; 
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				Cursor.Current = Cursors.Default;
				this.oleDbConnection.Close();
			}
		
		}
	

		internal DataTable GetTableOfSelectedTickers()
		{
			DataTable dataTableOfDataGrid1 = (DataTable)this.dataGrid1.DataSource;
			DataTable tableOfSelectedTickers = dataTableOfDataGrid1.Copy();
			tableOfSelectedTickers.Clear();
			// doing so, the two tables have the same structure
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

		private void menuItemValidateCurrentRows_Click(object sender, System.EventArgs e)
		{
			DataTable tableOfSelectedTickers = this.GetTableOfSelectedTickers();
			if(tableOfSelectedTickers.Rows.Count == 0)
			{
				MessageBox.Show("No ticker has been selected!\n\n" + 
								"Click on the grey area on the left to " +
								"select a ticker", "QuantDownloader error message",
								MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			QuantProject.Applications.Downloader.Validate.ValidateForm validateForm = 
				new Validate.ValidateForm(this.GetTableOfSelectedTickers());
			validateForm.Show();
		}

		private void menuItemDownloadCurrentRows_Click(object sender, System.EventArgs e)
		{
			DataTable tableOfSelectedTickers = this.GetTableOfSelectedTickers();
			if(tableOfSelectedTickers.Rows.Count == 0)
			{
				MessageBox.Show("No ticker has been selected!\n\n" + 
					"Click on the grey area on the left to " +
					"select a ticker", "QuantDownloader error message",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			WebDownloader webDownloader = new WebDownloader(this.GetTableOfSelectedTickers());
			webDownloader.Show();
		}

		private void dataGrid1_DragLeave(object sender, System.EventArgs e)
		{
			MessageBox.Show("Calcolo object data!");
		}

		private void menuItemCopySelectedTickersToClipboard_Click(object sender, System.EventArgs e)
		{
			Clipboard.SetDataObject(this.GetTableOfSelectedTickers());
		}

		

	}
}
