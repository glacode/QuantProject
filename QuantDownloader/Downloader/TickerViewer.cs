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
		private DataTable tableTickers;

		public TickerViewer()
		{
			InitializeComponent();
			this.oleDbConnection = ConnectionProvider.OleDbConnection;
			this.oleDbConnection.Open();
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
			this.dataGrid1 = new System.Windows.Forms.DataGrid();
			this.buttonFindTickers = new System.Windows.Forms.Button();
			this.contextMenuTickerViewer = new System.Windows.Forms.ContextMenu();
			this.menuItemValidateCurrentRows = new System.Windows.Forms.MenuItem();
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
			this.SuspendLayout();
			// 
			// textBoxStringToFind
			// 
			this.textBoxStringToFind.Location = new System.Drawing.Point(48, 8);
			this.textBoxStringToFind.Name = "textBoxStringToFind";
			this.textBoxStringToFind.Size = new System.Drawing.Size(160, 20);
			this.textBoxStringToFind.TabIndex = 0;
			this.textBoxStringToFind.Text = "";
			this.toolTip1.SetToolTip(this.textBoxStringToFind, "Type chars to filter tickers (you can use % and _ )  ");
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 24);
			this.label1.TabIndex = 1;
			this.label1.Text = "Find ...";
			// 
			// dataGrid1
			// 
			this.dataGrid1.ContextMenu = this.contextMenuTickerViewer;
			this.dataGrid1.DataMember = "";
			this.dataGrid1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.dataGrid1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dataGrid1.Location = new System.Drawing.Point(0, 46);
			this.dataGrid1.Name = "dataGrid1";
			this.dataGrid1.Size = new System.Drawing.Size(288, 432);
			this.dataGrid1.TabIndex = 2;
			// 
			// buttonFindTickers
			// 
			this.buttonFindTickers.Location = new System.Drawing.Point(216, 8);
			this.buttonFindTickers.Name = "buttonFindTickers";
			this.buttonFindTickers.Size = new System.Drawing.Size(64, 24);
			this.buttonFindTickers.TabIndex = 3;
			this.buttonFindTickers.Text = "Go";
			this.buttonFindTickers.Click += new System.EventHandler(this.buttonFindTickers_Click);
			// 
			// contextMenuTickerViewer
			// 
			this.contextMenuTickerViewer.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																									this.menuItemValidateCurrentRows});
			// 
			// menuItemValidateCurrentRows
			// 
			this.menuItemValidateCurrentRows.Index = 0;
			this.menuItemValidateCurrentRows.Text = "Validate current rows";
			this.menuItemValidateCurrentRows.Click += new System.EventHandler(this.menuItemValidateCurrentRows_Click);
			// 
			// TickerViewer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(288, 478);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonFindTickers,
																		  this.dataGrid1,
																		  this.label1,
																		  this.textBoxStringToFind});
			this.Name = "TickerViewer";
			this.Text = "TickerViewer";
			this.Closed += new System.EventHandler(this.TickerViewer_Closed);
			((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion


		#region Style for dataGrid1
		
		private void setStyle_dataGrid1()
		{
			DataGridTableStyle dataGrid1TableStyle = new DataGridTableStyle();
			dataGrid1TableStyle.MappingName = "tickers";
			dataGrid1TableStyle.ColumnHeadersVisible = true;
			dataGrid1TableStyle.ReadOnly = true;
			dataGrid1TableStyle.SelectionBackColor = Color.DimGray ;
			DataGridTextBoxColumn columnStyle = new DataGridTextBoxColumn();
			columnStyle.MappingName = "tiTicker";
			columnStyle.HeaderText = "Ticker";
			columnStyle.TextBox.Enabled = false;
			dataGrid1TableStyle.GridColumnStyles.Add(columnStyle);
			this.dataGrid1.TableStyles.Add(dataGrid1TableStyle);
			columnStyle.NullText = "";
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
				string criteria = "SELECT * FROM tickers WHERE tiTicker LIKE '" +
									this.textBoxStringToFind.Text + "'";
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
				;
			}
		
		}
	

		private DataTable getTableOfSelectedTickers()
		{
			DataTable dataTableOfDataGrid1 = (DataTable)this.dataGrid1.DataSource;
			DataTable tableOfSelectedTickers = dataTableOfDataGrid1.Copy();
			tableOfSelectedTickers.Clear();
			// so the two tables have the same structure
			int indexOfRow = 0;
			while(indexOfRow != dataTableOfDataGrid1.Rows.Count)
			{
				if(this.dataGrid1.IsSelected(indexOfRow))
				{
					DataRow dataRow = tableOfSelectedTickers.NewRow(); 
					dataRow[0] = (string)dataTableOfDataGrid1.Rows[indexOfRow][0];
					tableOfSelectedTickers.Rows.Add(dataRow);
				}
				indexOfRow++;
			}
			return tableOfSelectedTickers;
		}

		private void menuItemValidateCurrentRows_Click(object sender, System.EventArgs e)
		{
			QuantProject.Applications.Downloader.Validate.ValidateForm validateForm = 
				new Validate.ValidateForm(this.getTableOfSelectedTickers());
			validateForm.Show();
		}
		

	}
}
