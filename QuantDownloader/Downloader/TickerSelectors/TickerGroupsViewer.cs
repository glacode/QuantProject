/*
QuantDownloader - Quantitative Finance Library

TickerGroupsViewer.cs
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
//using System.Data.OleDb;
using System.Data;
using QuantProject.DataAccess;
using QuantProject.DataAccess.Tables;
using QuantProject.Data.DataTables;
using QuantProject.Data.Selectors;
using QuantProject.ADT;

namespace QuantProject.Applications.Downloader.TickerSelectors
{
	/// <summary>
	/// TickerGroupsViewer.
	/// </summary>
	public class TickerGroupsViewer : System.Windows.Forms.Form,
	ITickerSelector, ITickerReceiver, ITickerRemover
	{
//		private OleDbConnection oleDbConnection = ConnectionProvider.OleDbConnection;
//		private OleDbDataAdapter oleDbDataAdapter;
		private DataTable table;
		private System.Windows.Forms.TreeView treeViewGroups;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ListView listViewGroupsAndTickers;
		private System.Windows.Forms.ContextMenu contextMenuTickerGroupsTreeView;
		private System.Windows.Forms.MenuItem menuItemAddNewGroup;
		private System.Windows.Forms.MenuItem menuItemRemoveGroup;
		private System.Windows.Forms.MenuItem menuItemRenameGroup;
		private string selectedGroupID;
		private string selectedGroupDescription;
		private System.Windows.Forms.ImageList imageListGroupsAndTickers;
		private System.ComponentModel.IContainer components;
		private const int GROUP_IMAGE = 0;
		private const int TICKER_IMAGE = 1;
		private const string FIRST_COLUMN_NAME = "Element Name";
		private const string SECOND_COLUMN_NAME = "Element Type";
		private System.Windows.Forms.MenuItem menuItemTickerSelector;
		private const string THIRD_COLUMN_NAME = "Element Description";
		private SortOrder lastSorting = SortOrder.None;
		
		public TickerGroupsViewer()
		{
			InitializeComponent();

			//
			this.loadImagesInImageListGroupAndTickers();
			this.listViewGroupsAndTickers.ContextMenu = new TickerGroupsListViewMenu(this);
			this.listViewGroupsAndTickers.Columns.Add(FIRST_COLUMN_NAME,
			                                          this.listViewGroupsAndTickers.Width - 30,
			                                          HorizontalAlignment.Left);
			this.listViewGroupsAndTickers.Columns.Add(SECOND_COLUMN_NAME,
			                                          this.listViewGroupsAndTickers.Width - 60,
			                                          HorizontalAlignment.Left);
			this.listViewGroupsAndTickers.Columns.Add(THIRD_COLUMN_NAME,
			                                          this.listViewGroupsAndTickers.Width - 90,
			                                          HorizontalAlignment.Left);
			//
		}
		private void loadImagesInImageListGroupAndTickers()
		{
			try
			{
				string fileName = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf('\\'))
					+ @"\Group.bmp";
				Image image = Image.FromFile(fileName);
				this.imageListGroupsAndTickers.Images.Add(image);
				
				fileName = Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf('\\'))
					+ @"\Ticker.bmp";
				image = Image.FromFile(fileName);
				this.imageListGroupsAndTickers.Images.Add(image);

			}
			catch(Exception ex)
			{
				MessageBox.Show("Put all the bmp files in the dir <<TickerGroupsViewerImages>> inside the bin directory! ", "Files not found");
				string notUsed = ex.ToString();
			}

		}

		/// <summary>
		/// Clean all resources being used.
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
		/// <summary>
		/// It gets the groupID of the selected node in the treeView control of the object
		/// </summary>
		public string SelectedGroupID
		{
			get
			{
				return this.selectedGroupID;
			}
		}
		/// <summary>
		/// It gets the group Description of the selected node in the treeView control of the object
		/// </summary>
		public string SelectedGroupDescription
		{
			get
			{
				return this.selectedGroupDescription;
			}
		}
		
		/// <summary>
		/// It gets the selected items in the list view controls
		/// </summary>
		public ListView.SelectedListViewItemCollection SelectedListViewItems
		{
			get
			{
				return this.listViewGroupsAndTickers.SelectedItems;
			}
		}

		private void addTickersToTable(DataTable tableToFill)
		{
			try
			{
				;
			}
			catch(Exception ex)
			{
				string notUsed = ex.ToString();
			}
		}

		private void addTickerToTable(DataTable tableToFill, string tickerID,
		                              string tickerDescription)
		{
			try
			{
				DataRow newRow = tableToFill.NewRow();
				newRow["tiTicker"] = tickerID;
				newRow["tiCompanyName"] = tickerDescription;
				tableToFill.Rows.Add(newRow);
			}
			catch(Exception ex)
			{
				string notUsed = ex.ToString();
			}
		}
		
		private void addTickerToTable(DataTable tableToFill, string tickerID)
			
		{
			try
			{
				DataRow newRow = tableToFill.NewRow();
				newRow["tiTicker"] = tickerID;
				newRow["tiCompanyName"] = "-";
				tableToFill.Rows.Add(newRow);
			}
			catch(Exception ex)
			{
				string notUsed = ex.ToString();
			}
		}
		
		private void addTickersFromTableToTable(DataTable tableFrom, DataTable tableToFill)
			
		{
			DataColumn[] columnPrimaryKeys = new DataColumn[1];
			columnPrimaryKeys[0] = tableToFill.Columns[0];
			tableToFill.PrimaryKey = columnPrimaryKeys;
			
			foreach(DataRow row in tableFrom.Rows)
			{
				try
				{
					DataRow newRow = tableToFill.NewRow();
					newRow["tiTicker"] = (string)row[0];
					newRow["tiCompanyName"] = "-";
					tableToFill.Rows.Add(newRow);
				}
				catch(Exception ex)
				{
					string notUsed = ex.ToString();
				}
			}
		}

		// implementation of ITickerSelector interface
		public void SelectAllTickers()
		{
			foreach(ListViewItem item in this.listViewGroupsAndTickers.Items)
			{
				item.Selected = true;
			}
		}

		public DataTable GetTableOfSelectedTickers()
		{
			DataTable tableOfSelectedTickers = new DataTable();
			TickerDataTable.AddColumnsOfTickerTable(tableOfSelectedTickers);
			
			foreach(ListViewItem item in this.listViewGroupsAndTickers.SelectedItems)
			{
				if(item.Tag is System.String)
					// the item contains in Tag property the ticker ID
				{
					this.addTickerToTable(tableOfSelectedTickers,
					                      (string)item.Tag,
					                      item.SubItems[2].Text);
				}
				else
					// the item references to a node in the treeView :
					// so it stands for a group of tickers
				{
					TreeNode node = (TreeNode)item.Tag;
					DataTable tickersInsideGroup =
						QuantProject.DataAccess.Tables.Tickers_tickerGroups.GetTickers((string)node.Tag);
					this.addTickersFromTableToTable(tickersInsideGroup, tableOfSelectedTickers);
				}
			}
			return tableOfSelectedTickers;
		}

		// implementation of ITickerRemover interface
		public void RemoveTickers()
		{
			if(MessageBox.Show("Do you really want to delete the selected items?",
			                   "Confirm deletion",
			                   MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)
			   == DialogResult.No)
				return;
			//update database
			foreach(ListViewItem item in this.SelectedListViewItems)
			{
				if(item.Tag is System.String )
					// the item contains in Tag property the ticker ID
					QuantProject.DataAccess.Tables.Tickers_tickerGroups.Delete((string)item.Tag,
					                                                           (string)this.treeViewGroups.SelectedNode.Tag);
				else
					// the item references to a node in the treeView :
					// so it stands for a group of tickers
				{
					TreeNode node = (TreeNode)item.Tag;
					TickerGroups.DeleteGroup((string)node.Tag);
					node.Remove();
				}
			}
			
			//update list view
			for(int i = this.listViewGroupsAndTickers.SelectedItems.Count - 1;i>=0;
			    i=this.listViewGroupsAndTickers.SelectedItems.Count-1)
			{
				this.listViewGroupsAndTickers.SelectedItems[i].Remove();
			}
			//refresh
			this.treeViewGroups.Refresh();
			this.listViewGroupsAndTickers.Refresh();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Don't modify content with the code editor
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.treeViewGroups = new System.Windows.Forms.TreeView();
			this.contextMenuTickerGroupsTreeView = new System.Windows.Forms.ContextMenu();
			this.menuItemAddNewGroup = new System.Windows.Forms.MenuItem();
			this.menuItemRemoveGroup = new System.Windows.Forms.MenuItem();
			this.menuItemRenameGroup = new System.Windows.Forms.MenuItem();
			this.menuItemTickerSelector = new System.Windows.Forms.MenuItem();
			this.imageListGroupsAndTickers = new System.Windows.Forms.ImageList(this.components);
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.listViewGroupsAndTickers = new System.Windows.Forms.ListView();
			this.SuspendLayout();
			// 
			// treeViewGroups
			// 
			this.treeViewGroups.ContextMenu = this.contextMenuTickerGroupsTreeView;
			this.treeViewGroups.Dock = System.Windows.Forms.DockStyle.Left;
			this.treeViewGroups.ImageList = this.imageListGroupsAndTickers;
			this.treeViewGroups.Name = "treeViewGroups";
			this.treeViewGroups.Size = new System.Drawing.Size(120, 326);
			this.treeViewGroups.TabIndex = 0;
			this.treeViewGroups.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeViewGroups_AfterExpand);
			this.treeViewGroups.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewGroups_AfterSelect);
			// 
			// contextMenuTickerGroupsTreeView
			// 
			this.contextMenuTickerGroupsTreeView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			                                                        	this.menuItemAddNewGroup,
			                                                        	this.menuItemRemoveGroup,
			                                                        	this.menuItemRenameGroup,
			                                                        	this.menuItemTickerSelector});
			// 
			// menuItemAddNewGroup
			// 
			this.menuItemAddNewGroup.Index = 0;
			this.menuItemAddNewGroup.Text = "&Add New Group";
			this.menuItemAddNewGroup.Click += new System.EventHandler(this.menuItemAddNewGroup_Click);
			// 
			// menuItemRemoveGroup
			// 
			this.menuItemRemoveGroup.Index = 1;
			this.menuItemRemoveGroup.Text = "&Remove";
			this.menuItemRemoveGroup.Click += new System.EventHandler(this.menuItemRemoveGroup_Click);
			// 
			// menuItemRenameGroup
			// 
			this.menuItemRenameGroup.Index = 2;
			this.menuItemRenameGroup.Text = "&Modify";
			this.menuItemRenameGroup.Click += new System.EventHandler(this.menuItemRenameGroup_Click);
			// 
			// menuItemTickerSelector
			// 
			this.menuItemTickerSelector.Index = 3;
			this.menuItemTickerSelector.Text = "&Ticker Selector";
			this.menuItemTickerSelector.Click += new System.EventHandler(this.menuItemTickerSelector_Click);
			// 
			// imageListGroupsAndTickers
			// 
			this.imageListGroupsAndTickers.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageListGroupsAndTickers.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListGroupsAndTickers.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// splitter1
			// 
			this.splitter1.BackColor = System.Drawing.SystemColors.Highlight;
			this.splitter1.Location = new System.Drawing.Point(120, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 326);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// listViewGroupsAndTickers
			// 
			this.listViewGroupsAndTickers.Activation = System.Windows.Forms.ItemActivation.TwoClick;
			this.listViewGroupsAndTickers.AllowColumnReorder = true;
			this.listViewGroupsAndTickers.AllowDrop = true;
			this.listViewGroupsAndTickers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewGroupsAndTickers.Location = new System.Drawing.Point(123, 0);
			this.listViewGroupsAndTickers.Name = "listViewGroupsAndTickers";
			this.listViewGroupsAndTickers.Size = new System.Drawing.Size(397, 326);
			this.listViewGroupsAndTickers.SmallImageList = this.imageListGroupsAndTickers;
			this.listViewGroupsAndTickers.TabIndex = 2;
			this.listViewGroupsAndTickers.View = System.Windows.Forms.View.Details;
			this.listViewGroupsAndTickers.ItemActivate += new System.EventHandler(this.listViewGroupsAndTickers_ItemActivate);
			this.listViewGroupsAndTickers.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewGroupsAndTickers_DragDrop);
			this.listViewGroupsAndTickers.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewGroupsAndTickers_DragEnter);
			this.listViewGroupsAndTickers.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewGroupsAndTickers_ColumnClick);
			this.listViewGroupsAndTickers.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewGroupsAndTickers_ItemDrag);
			this.listViewGroupsAndTickers.DragLeave += new System.EventHandler(this.listViewGroupsAndTickers_DragLeave);
			// 
			// TickerGroupsViewer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(520, 326);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
			                       	this.listViewGroupsAndTickers,
			                       	this.splitter1,
			                       	this.treeViewGroups});
			this.Name = "TickerGroupsViewer";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Ticker-Groups Viewer";
			this.Load += new System.EventHandler(this.TickerGroupsViewer_Load);
			this.ResumeLayout(false);

		}
		#endregion

		#region Tree View Paintig on Load

		/// <summary>
		/// fill Tree View with existing groups (at root level)
		/// </summary>
		private void fillTreeViewAtRootLevel()
		{
//				this.oleDbDataAdapter = new OleDbDataAdapter("SELECT * FROM tickerGroups WHERE tgTgId IS NULL" ,
//															this.oleDbConnection);
//				this.table = new DataTable();
//				this.oleDbDataAdapter.Fill(this.table);
			this.table = SqlExecutor.GetDataTable(
				"SELECT * FROM tickerGroups WHERE tgTgId IS NULL" );
			TreeNode rootNode = new TreeNode("GROUPS", GROUP_IMAGE, GROUP_IMAGE);
			rootNode.Tag = "";
			this.treeViewGroups.Nodes.Add(rootNode);
			foreach (DataRow row in this.table.Rows)
			{
				
				TreeNode node = new TreeNode((string)row["tgDescription"], GROUP_IMAGE, GROUP_IMAGE);
				node.Tag = (string)row["tgId"];
				rootNode.Nodes.Add(node);
			}
			
		}
		/// <summary>
		/// fill the current node with existing groups from the DB
		/// </summary>
		private void addNodeChildsToCurrentNode(TreeNode currentNode)
		{
//				this.oleDbDataAdapter.SelectCommand.CommandText =
//							"SELECT * FROM tickerGroups WHERE tgTgId = '" +
//							(string)currentNode.Tag + "'";
//
//				DataTable groupsChild = new DataTable();
//				this.oleDbDataAdapter.Fill(groupsChild);
			
			DataTable groupsChild = SqlExecutor.GetDataTable(
				"SELECT * FROM tickerGroups WHERE tgTgId = '" +
				(string)currentNode.Tag + "'" );
			foreach (DataRow row in groupsChild.Rows)
			{
				TreeNode node = new TreeNode((string)row["tgDescription"], GROUP_IMAGE, GROUP_IMAGE);
				node.Tag = (string)row["tgId"];
				currentNode.Nodes.Add(node);
			}
			
		}

		private void TickerGroupsViewer_Load(object sender, System.EventArgs e)
		{
			try
			{
				Cursor.Current = Cursors.WaitCursor;
//				if(this.oleDbConnection.State != ConnectionState.Open)
//					this.oleDbConnection.Open();
				fillTreeViewAtRootLevel();
				foreach(TreeNode node in this.treeViewGroups.Nodes[0].Nodes)
				{
					addNodeChildsToCurrentNode(node);
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				Cursor.Current = Cursors.Default;
//				if(this.oleDbConnection != null)
//					this.oleDbConnection.Close();
			}
		}

		#endregion

		#region Paint Child Nodes During selection

		private void treeViewGroups_AfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			
			if((string)this.treeViewGroups.SelectedNode.Tag != "")
				return;
			
			try
			{
				Cursor.Current = Cursors.WaitCursor;
//				if(this.oleDbConnection.State != ConnectionState.Open)
//					this.oleDbConnection.Open();
				foreach(TreeNode node in this.treeViewGroups.SelectedNode.Nodes)
				{
					foreach(TreeNode childNode in node.Nodes)
					{
						if(childNode.Nodes.Count == 0)
							addNodeChildsToCurrentNode(childNode);
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				Cursor.Current = Cursors.Default;
//				this.oleDbConnection.Close();
			}
			
		}
		
		#endregion
		
		
		
		private void treeViewGroups_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			this.selectedGroupID = (string)this.treeViewGroups.SelectedNode.Tag;
			this.selectedGroupDescription = this.treeViewGroups.SelectedNode.Text;
			this.Text = "Ticker-Groups Viewer " + this.treeViewGroups.SelectedNode.FullPath;
			this.updateListView(this.treeViewGroups.SelectedNode);
		}


		#region Update List View

		private void updateGroupsInListView(TreeNode selectedNode)
		{
			foreach(TreeNode node in selectedNode.Nodes)
			{
				ListViewItem listViewItem = new ListViewItem(node.Text, GROUP_IMAGE);
				listViewItem.Tag = node;
				this.listViewGroupsAndTickers.Items.Add(listViewItem);
				listViewItem.SubItems.Add("Group");
			}
			
		}
		
		private void updateTickersInListView(TreeNode selectedNode)
		{
			try
			{
				Cursor.Current = Cursors.WaitCursor;
//				if(this.oleDbConnection.State != ConnectionState.Open)
//					this.oleDbConnection.Open();
//				this.oleDbDataAdapter.SelectCommand.CommandText =
//					"SELECT DISTINCT ttTiId, tiCompanyName, ttTgId FROM tickers INNER JOIN " +
//					"tickers_tickerGroups ON tickers.tiTicker = tickers_tickerGroups.ttTiId WHERE ttTgId ='" +
//					(string)selectedNode.Tag + "'";
//				DataTable tickers = new DataTable();
//				this.oleDbDataAdapter.Fill(tickers);
				
				DataTable tickers = SqlExecutor.GetDataTable(
					"SELECT DISTINCT ttTiId, tiCompanyName, ttTgId FROM tickers INNER JOIN " +
					"tickers_tickerGroups ON tickers.tiTicker = tickers_tickerGroups.ttTiId WHERE ttTgId ='" +
					(string)selectedNode.Tag + "'" );
				foreach (DataRow row in tickers.Rows)
				{
					ListViewItem listViewItem = new ListViewItem((string)row[0],
					                                             TICKER_IMAGE);
					listViewItem.Tag = (string)row[0];
					this.listViewGroupsAndTickers.Items.Add(listViewItem);
					listViewItem.SubItems.Add("Ticker");
					listViewItem.SubItems.Add((string)row[1]);
				}
				
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				Cursor.Current = Cursors.Default;
//				this.oleDbConnection.Close();
			}
		}

		private void updateListView(TreeNode selectedNode)
		{
			this.listViewGroupsAndTickers.Items.Clear();
			this.updateGroupsInListView(selectedNode);
			this.updateTickersInListView(selectedNode);
		}

		#endregion

		#region Add group
		private void addNodeGroupToCurrentNode(string groupID,
		                                       string groupDescription)
		{
			
			TreeNode node = new TreeNode(groupDescription);
			node.Tag = groupID;
			this.treeViewGroups.SelectedNode.Nodes.Add(node);
			this.updateListView(this.treeViewGroups.SelectedNode);
			
		}

		
		internal void AddNewGroupToDataBase(string groupID,
		                                    string groupDescription,
		                                    string parentGroupID)
		{
			try
			{
				Cursor.Current = Cursors.WaitCursor;
//				if(this.oleDbConnection.State != ConnectionState.Open)
//					this.oleDbConnection.Open();
//				this.oleDbDataAdapter.InsertCommand =
//					new OleDbCommand("INSERT INTO tickerGroups(tgId, tgDescription, tgTgId) " +
//					                 "VALUES('" +
//					                 groupID + "','" +
//					                 groupDescription + "','" +
//					                 parentGroupID + "')", this.oleDbConnection);
//				int numRowInserted =
//					this.oleDbDataAdapter.InsertCommand.ExecuteNonQuery();
				
				int numRowInserted =
					SqlExecutor.ExecuteNonQuery(
						"INSERT INTO tickerGroups(tgId, tgDescription, tgTgId) " +
						"VALUES('" +
						groupID + "','" +
						groupDescription + "','" +
						parentGroupID + "')" );
				if(numRowInserted > 0)
					this.addNodeGroupToCurrentNode(groupID, groupDescription);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				Cursor.Current = Cursors.Default;
//				this.oleDbConnection.Close();
			}
		}

		internal void AddNewGroupToDataBase(string groupID,
		                                    string groupDescription)
			
		{
			try
			{
				Cursor.Current = Cursors.WaitCursor;
//				if(this.oleDbConnection.State != ConnectionState.Open)
//					this.oleDbConnection.Open();
//				this.oleDbDataAdapter.InsertCommand =
//					new OleDbCommand("INSERT INTO tickerGroups(tgId, tgDescription) " +
//					                 "VALUES('" +
//					                 groupID + "','" +
//					                 groupDescription + "')", this.oleDbConnection);
//				this.oleDbDataAdapter.InsertCommand.ExecuteNonQuery();
				
				SqlExecutor.ExecuteNonQuery(
					"INSERT INTO tickerGroups(tgId, tgDescription) " +
					                 "VALUES('" +
					                 groupID + "','" +
					                 groupDescription + "')" );
				this.addNodeGroupToCurrentNode(groupID, groupDescription);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				Cursor.Current = Cursors.Default;
//				this.oleDbConnection.Close();
			}
		}
		#endregion

		#region Modify current Node

		private void modifyCurrentNode(string newTag, string newText)
		{
			this.treeViewGroups.SelectedNode.Text = newText;
			this.treeViewGroups.SelectedNode.Tag = newTag;
		}

		internal void ModifyGroup(string oldGroupID,
		                          string groupDescription,
		                          string newGroupID)
			
		{
			try
			{
				Cursor.Current = Cursors.WaitCursor;
//				this.oleDbConnection.Open();
//				this.oleDbDataAdapter.UpdateCommand =
//					new OleDbCommand("UPDATE tickerGroups SET tgId ='" +
//					                 newGroupID + "', tgDescription ='" +
//					                 groupDescription + "' WHERE tgId ='" +
//					                 oldGroupID + "'", this.oleDbConnection);
//				int numUpdatedRows =
//					this.oleDbDataAdapter.UpdateCommand.ExecuteNonQuery();
				
				int numUpdatedRows = SqlExecutor.ExecuteNonQuery(
					"UPDATE tickerGroups SET tgId ='" +
					newGroupID + "', tgDescription ='" +
					groupDescription + "' WHERE tgId ='" +
					oldGroupID + "'" );
				if(numUpdatedRows >0)
					this.modifyCurrentNode(newGroupID, groupDescription);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				Cursor.Current = Cursors.Default;
//				this.oleDbConnection.Close();
			}
		}
		#endregion

		#region Remove current Node
		
		private void removeCurrentNodeAndGroup()
		{
			if((string)this.treeViewGroups.SelectedNode.Tag == "")
			{
				MessageBox.Show("You can not delete the root node!", "Error message",
				                MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if(MessageBox.Show("Do you really want to delete the current group and " +
			                   "all its groups and tickers?", "Confirm deletion",
			                   MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)
			   == DialogResult.No)
			{
				return;
			}
			deleteCurrentGroup(this.treeViewGroups.SelectedNode);
		}
		
		private void deleteCurrentGroup(TreeNode nodeCorrespondingToGroup)
		{
			
			try
			{
				Cursor.Current = Cursors.WaitCursor;
//				this.oleDbConnection.Open();
//				this.oleDbDataAdapter.DeleteCommand =
//					new OleDbCommand("DELETE * FROM tickerGroups WHERE tgId = '" +
//					                 nodeCorrespondingToGroup.Tag + "'",
//					                 this.oleDbConnection);
//				int numDeletedRows =
//					this.oleDbDataAdapter.DeleteCommand.ExecuteNonQuery();
				
				int numDeletedRows =
					SqlExecutor.ExecuteNonQuery(
						"DELETE * FROM tickerGroups WHERE tgId = '" +
						nodeCorrespondingToGroup.Tag + "'" );
				if (numDeletedRows > 0)
					this.treeViewGroups.SelectedNode.Remove();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				Cursor.Current = Cursors.Default;
//				this.oleDbConnection.Close();
			}
			
		}

		#endregion

		private void menuItemAddNewGroup_Click(object sender, System.EventArgs e)
		{
			GroupEditor groupEditor = new GroupEditor(this);
			groupEditor.Show();
		}

		private void menuItemRemoveGroup_Click(object sender, System.EventArgs e)
		{
			this.removeCurrentNodeAndGroup();
		}

		private void menuItemRenameGroup_Click(object sender, System.EventArgs e)
		{
			if((string)this.treeViewGroups.SelectedNode.Tag == "")
				// it is the root node
			{
				MessageBox.Show("The root node can't be modified!");
				return;
			}
			GroupEditor groupEditor = new GroupEditor(this, this.treeViewGroups.SelectedNode);
			groupEditor.Show();
		}

		private void insertRow(DataRow row)
		{
			QuantProject.DataAccess.Tables.Tickers_tickerGroups.Add((string)row[0],
			                                                        (string)this.treeViewGroups.SelectedNode.Tag,
			                                                        EventType.Entry,
			                                                        ConstantsProvider.DefaultDateForTickersAddedToGroups);
		}
		


		private void listViewGroupsAndTickers_ItemActivate(object sender, System.EventArgs e)
		{
			if(this.listViewGroupsAndTickers.SelectedItems[0].Tag is TreeNode)
			{
				this.treeViewGroups.SelectedNode.Expand();
				foreach (TreeNode node in this.treeViewGroups.SelectedNode.Nodes)
				{
					if(node.Equals(this.listViewGroupsAndTickers.SelectedItems[0].Tag))
					{
						node.Expand();
						this.treeViewGroups.SelectedNode = node;
						this.updateListView(this.treeViewGroups.SelectedNode);
						break;
					}
				}

			}
		}
		//implementation of ITickerReceiver interface for this object

		public void ReceiveTickers(DataTable tickers)
		{
			try
			{
				if(tickers == null)
				{
					MessageBox.Show("No ticker has been copied to ClipBoard!\n\n" +
					                "Select some tickers before trying again!",
					                "Paste operation failure", MessageBoxButtons.OK,
					                MessageBoxIcon.Error);
					return;
				}
				if((string)this.treeViewGroups.SelectedNode.Tag == "")
				{
					MessageBox.Show("Selected tickers can't be copied inside " +
					                "the root node: change group and try again!",
					                "Paste operation failure", MessageBoxButtons.OK,
					                MessageBoxIcon.Error);
					return;
				}
				// end of checks
				Cursor.Current = Cursors.WaitCursor;
//				if(this.oleDbConnection.State != ConnectionState.Open)
//					this.oleDbConnection.Open();
				foreach (DataRow row in tickers.Rows)
					this.insertRow(row);
				this.updateListView(this.treeViewGroups.SelectedNode);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				Cursor.Current = Cursors.Default;
//				this.oleDbConnection.Close();
			}
		}

		private void menuItemTickerSelector_Click(object sender, System.EventArgs e)
		{
			if((string)this.treeViewGroups.SelectedNode.Tag == "")
				// it is the root node
			{
				MessageBox.Show("Choose a group containing some tickers!");
				return;
			}
			TickerSelectorForm tickerSelectorForm =
				new TickerSelectorForm((string)this.treeViewGroups.SelectedNode.Tag);
			tickerSelectorForm.Show();
		}

		private void listViewGroupsAndTickers_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
		{
			DoDragDrop(this.listViewGroupsAndTickers.SelectedItems, DragDropEffects.Copy);
		}

		private void listViewGroupsAndTickers_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			//MessageBox.Show("Finished drag and drop");
		}

		private void listViewGroupsAndTickers_DragLeave(object sender, System.EventArgs e)
		{
			//DragAction

			//MessageBox.Show("Try to leave data here");
		}

		private void listViewGroupsAndTickers_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			//MessageBox.Show("Drag here?");
		}

		private void listViewGroupsAndTickers_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
		{
			if(this.lastSorting == SortOrder.None ||
			   this.lastSorting == SortOrder.Descending)
				this.listViewGroupsAndTickers.Sorting = SortOrder.Ascending;
			else
				this.listViewGroupsAndTickers.Sorting = SortOrder.Descending;
			this.listViewGroupsAndTickers.Sort();
			this.lastSorting = this.listViewGroupsAndTickers.Sorting;
			this.listViewGroupsAndTickers.Sorting = SortOrder.None;
		}
		


	}
}
