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
using System.Data.OleDb;
using System.Data;
using QuantProject.DataAccess;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// TickerGroupsViewer.
	/// </summary>
	public class TickerGroupsViewer : System.Windows.Forms.Form
	{
		private OleDbConnection oleDbConnection = ConnectionProvider.OleDbConnection;
		private OleDbDataAdapter oleDbDataAdapter;
		private DataTable table;
		private System.Windows.Forms.TreeView treeViewGroups;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.ListView listViewGroupsAndTickers;
		private System.Windows.Forms.ContextMenu contextMenuTickerGroupsTreeView;
		private System.Windows.Forms.MenuItem menuItemAddNewGroup;
		private System.Windows.Forms.MenuItem menuItemRemoveGroup;
		private System.Windows.Forms.MenuItem menuItemRenameGroup;
		private System.Windows.Forms.ContextMenu contextMenuTickerGroupsListView;
		private System.Windows.Forms.MenuItem menuItemValidateGroup;
		private System.Windows.Forms.MenuItem menuItemDownloadQuotesOfCurrentGroup;
		private System.Windows.Forms.MenuItem menuItemOpenTickerSelector;
		private string selectedGroupID;

		/// <summary>
		/// Components
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TickerGroupsViewer()
		{
			
			InitializeComponent();

			//
			// TODO: 
			//
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
		/// It gets the groupID of the selected node in the treeView control of the class
		/// </summary>
		public string SelectedGroupID
		{
			get
			{
				return this.selectedGroupID;
			}
		}


		#region Windows Form Designer generated code
		/// <summary>
		/// Don't modify content with the code editor
		/// </summary>
		private void InitializeComponent()
		{
			this.treeViewGroups = new System.Windows.Forms.TreeView();
			this.contextMenuTickerGroupsTreeView = new System.Windows.Forms.ContextMenu();
			this.menuItemAddNewGroup = new System.Windows.Forms.MenuItem();
			this.menuItemRemoveGroup = new System.Windows.Forms.MenuItem();
			this.menuItemRenameGroup = new System.Windows.Forms.MenuItem();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.listViewGroupsAndTickers = new System.Windows.Forms.ListView();
			this.contextMenuTickerGroupsListView = new System.Windows.Forms.ContextMenu();
			this.menuItemValidateGroup = new System.Windows.Forms.MenuItem();
			this.menuItemDownloadQuotesOfCurrentGroup = new System.Windows.Forms.MenuItem();
			this.menuItemOpenTickerSelector = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// treeViewGroups
			// 
			this.treeViewGroups.ContextMenu = this.contextMenuTickerGroupsTreeView;
			this.treeViewGroups.Dock = System.Windows.Forms.DockStyle.Left;
			this.treeViewGroups.ImageIndex = -1;
			this.treeViewGroups.Name = "treeViewGroups";
			this.treeViewGroups.SelectedImageIndex = -1;
			this.treeViewGroups.Size = new System.Drawing.Size(120, 326);
			this.treeViewGroups.TabIndex = 0;
			this.treeViewGroups.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewGroups_AfterSelect);
			// 
			// contextMenuTickerGroupsTreeView
			// 
			this.contextMenuTickerGroupsTreeView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																											this.menuItemAddNewGroup,
																											this.menuItemRemoveGroup,
																											this.menuItemRenameGroup});
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
			this.listViewGroupsAndTickers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listViewGroupsAndTickers.Location = new System.Drawing.Point(123, 0);
			this.listViewGroupsAndTickers.Name = "listViewGroupsAndTickers";
			this.listViewGroupsAndTickers.Size = new System.Drawing.Size(301, 326);
			this.listViewGroupsAndTickers.TabIndex = 2;
			// 
			// contextMenuTickerGroupsListView
			// 
			this.contextMenuTickerGroupsListView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																											this.menuItemValidateGroup,
																											this.menuItemDownloadQuotesOfCurrentGroup,
																											this.menuItemOpenTickerSelector});
			// 
			// menuItemValidateGroup
			// 
			this.menuItemValidateGroup.Index = 0;
			this.menuItemValidateGroup.Text = "&Validate";
			// 
			// menuItemDownloadQuotesOfCurrentGroup
			// 
			this.menuItemDownloadQuotesOfCurrentGroup.Index = 1;
			this.menuItemDownloadQuotesOfCurrentGroup.Text = "&Download quotes";
			// 
			// menuItemOpenTickerSelector
			// 
			this.menuItemOpenTickerSelector.Index = 2;
			this.menuItemOpenTickerSelector.Text = "&Open Ticker Selector";
			// 
			// TickerGroupsViewer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(424, 326);
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
				this.oleDbDataAdapter = new OleDbDataAdapter("SELECT * FROM tickerGroups WHERE tgTgId IS NULL" ,
															this.oleDbConnection);
				this.table = new DataTable();
				this.oleDbDataAdapter.Fill(this.table);
				TreeNode rootNode = new TreeNode("GROUPS");
				rootNode.Tag = "";
				this.treeViewGroups.Nodes.Add(rootNode);
				foreach (DataRow row in this.table.Rows)
				{
					
					TreeNode node = new TreeNode((string)row["tgDescription"]);
					node.Tag = (string)row["tgId"];
					rootNode.Nodes.Add(node);
				}
			
		}
		/// <summary>
		/// fill the current node with existing groups from the DB
		/// </summary>
		private void addNodeChildsToCurrentNode(TreeNode currentNode)
		{
				this.oleDbDataAdapter.SelectCommand.CommandText = 
							"SELECT * FROM tickerGroups WHERE tgTgId = '" +
							(string)currentNode.Tag + "'";
				
				DataTable groupsChild = new DataTable();
				this.oleDbDataAdapter.Fill(groupsChild);
				foreach (DataRow row in groupsChild.Rows)
				{
					TreeNode node = new TreeNode((string)row["tgDescription"]);
					node.Tag = (string)row["tgId"];
					currentNode.Nodes.Add(node);
				}
						
		}

		private void TickerGroupsViewer_Load(object sender, System.EventArgs e)
		{
			try
			{
				Cursor.Current = Cursors.WaitCursor;
				this.oleDbConnection.Open();
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
				this.oleDbConnection.Close();
			}
		}

		#endregion

		#region Paint Child Nodes During selection

		private void treeViewGroups_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			this.selectedGroupID = (string)this.treeViewGroups.SelectedNode.Tag; 
			// It updates the member used by the property SelectedGroupID 
			if((string)this.treeViewGroups.SelectedNode.Tag != "" && 
				this.treeViewGroups.SelectedNode.Nodes.Count == 0)
			// if the selected node is not the root node and it hasn't got child nodes
			// it has to be filled with its child nodes (if any)	
			{
				try
				{
					Cursor.Current = Cursors.WaitCursor;
					this.oleDbConnection.Open();
					foreach(TreeNode node in this.treeViewGroups.SelectedNode.Nodes)
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
					this.oleDbConnection.Close();
				}
			}
		}
		#endregion

		
		#region Add group
		private void addNodeGroupToCurrentNode(string groupID,
												string groupDescription)
		{
			
			TreeNode node = new TreeNode(groupDescription);
			node.Tag = groupID;
			this.treeViewGroups.SelectedNode.Nodes.Add(node);
						
		}

		
		internal void AddNewGroupToDataBase(string groupID,
											string groupDescription,
											string parentGroupID)
		{
			try
			{
				Cursor.Current = Cursors.WaitCursor;
				this.oleDbConnection.Open();
				this.oleDbDataAdapter.InsertCommand = 
					new OleDbCommand("INSERT INTO tickerGroups(tgId, tgDescription, tgTgId) " +
									 "VALUES('" +
									 groupID + "','" +
									 groupDescription + "','" +
									 parentGroupID + "')", this.oleDbConnection);
				this.oleDbDataAdapter.InsertCommand.ExecuteNonQuery();
				this.addNodeGroupToCurrentNode(groupID, groupDescription);
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

		internal void AddNewGroupToDataBase(string groupID,
											string groupDescription)
											
		{
			try
			{
				Cursor.Current = Cursors.WaitCursor;
				this.oleDbConnection.Open();
				this.oleDbDataAdapter.InsertCommand = 
					new OleDbCommand("INSERT INTO tickerGroups(tgId, tgDescription) " +
										"VALUES('" +
										groupID + "','" +
										groupDescription + "')", this.oleDbConnection);
				this.oleDbDataAdapter.InsertCommand.ExecuteNonQuery();
				this.addNodeGroupToCurrentNode(groupID, groupDescription);
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
				this.oleDbConnection.Open();
				this.oleDbDataAdapter.UpdateCommand = 
					new OleDbCommand("UPDATE tickerGroups SET tgId =' " +
									 newGroupID + "', tgDescription =' " + 
									 groupDescription + "' WHERE tgId =' " +
									 oldGroupID + "'", this.oleDbConnection);
				this.oleDbDataAdapter.UpdateCommand.ExecuteNonQuery();
				this.modifyCurrentNode(newGroupID, groupDescription);
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
			this.treeViewGroups.SelectedNode.Remove(); 
		}
		
		private void deleteCurrentGroup(TreeNode nodeCorrespondingToGroup)
		{
			
			try
			{
				Cursor.Current = Cursors.WaitCursor;
				this.oleDbConnection.Open();
				this.oleDbDataAdapter.DeleteCommand = 
					new OleDbCommand("DELETE * FROM tickerGroups WHERE tgId = '" +
					nodeCorrespondingToGroup.Tag + "'",
					this.oleDbConnection);
				this.oleDbDataAdapter.DeleteCommand.ExecuteNonQuery();
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


		
	}
}
