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
using QuantProject.Applications.Downloader.TickerSelectors;

namespace QuantProject.Applications.Downloader
{
	/// <summary>
	/// It has to be used for editing group names and ID in the
	/// TickerGroupsViewer passed as the actual parameter in the constructor
	/// </summary>
	public class GroupEditor : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelGroupID;
		private System.Windows.Forms.Label labelGroupDescription;
		private System.Windows.Forms.TextBox textBoxGroupDescription;
		private System.Windows.Forms.TextBox textBoxGroupID;
		private TickerGroupsViewer tickerGroupsViewer;
		private TreeNode selectedNodeInTickerGroupsViewer;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonModify;
		/// <summary>
		/// components
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GroupEditor(TickerGroupsViewer tickerGroupsViewer)
							
		{
			
			InitializeComponent();
			//
			this.tickerGroupsViewer = tickerGroupsViewer;
			this.Text = "Add new group inside: " + 
												this.tickerGroupsViewer.SelectedGroupDescription;
			this.buttonModify.Visible = false;
			//
		}
		public GroupEditor(TickerGroupsViewer tickerGroupsViewer, 
						   TreeNode nodeToBeModified)
							
		{
			
			InitializeComponent();
			//
			this.tickerGroupsViewer = tickerGroupsViewer;
			this.selectedNodeInTickerGroupsViewer = nodeToBeModified;
			this.Text = "Modify group";
			this.textBoxGroupDescription.Text = nodeToBeModified.Text;
			this.textBoxGroupID.Text = (string)nodeToBeModified.Tag;
			this.buttonAdd.Visible = false;
			this.buttonModify.Location = new System.Drawing.Point(144,112);
			//
		}

		/// <summary>
		/// Clean up resources being used
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
		/// Metodo necessario per il supporto della finestra di progettazione. Non modificare
		/// il contenuto del metodo con l'editor di codice.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonAdd = new System.Windows.Forms.Button();
			this.textBoxGroupID = new System.Windows.Forms.TextBox();
			this.labelGroupID = new System.Windows.Forms.Label();
			this.labelGroupDescription = new System.Windows.Forms.Label();
			this.textBoxGroupDescription = new System.Windows.Forms.TextBox();
			this.buttonModify = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonAdd
			// 
			this.buttonAdd.Location = new System.Drawing.Point(144, 112);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.TabIndex = 0;
			this.buttonAdd.Text = "&ADD";
			this.buttonAdd.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// textBoxGroupID
			// 
			this.textBoxGroupID.Location = new System.Drawing.Point(144, 24);
			this.textBoxGroupID.Name = "textBoxGroupID";
			this.textBoxGroupID.Size = new System.Drawing.Size(112, 20);
			this.textBoxGroupID.TabIndex = 1;
			this.textBoxGroupID.Text = "GroupID";
			// 
			// labelGroupID
			// 
			this.labelGroupID.Location = new System.Drawing.Point(8, 24);
			this.labelGroupID.Name = "labelGroupID";
			this.labelGroupID.Size = new System.Drawing.Size(128, 23);
			this.labelGroupID.TabIndex = 2;
			this.labelGroupID.Text = "Group ID (max 8 chr)";
			// 
			// labelGroupDescription
			// 
			this.labelGroupDescription.Location = new System.Drawing.Point(8, 64);
			this.labelGroupDescription.Name = "labelGroupDescription";
			this.labelGroupDescription.TabIndex = 4;
			this.labelGroupDescription.Text = "Group Description";
			// 
			// textBoxGroupDescription
			// 
			this.textBoxGroupDescription.Location = new System.Drawing.Point(144, 64);
			this.textBoxGroupDescription.Name = "textBoxGroupDescription";
			this.textBoxGroupDescription.Size = new System.Drawing.Size(252, 20);
			this.textBoxGroupDescription.TabIndex = 3;
			this.textBoxGroupDescription.Text = "Group Description";
			// 
			// buttonModify
			// 
			this.buttonModify.Location = new System.Drawing.Point(232, 112);
			this.buttonModify.Name = "buttonModify";
			this.buttonModify.TabIndex = 5;
			this.buttonModify.Text = "&MODIFY";
			this.buttonModify.Click += new System.EventHandler(this.buttonModify_Click);
			// 
			// GroupEditor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(418, 152);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonModify,
																		  this.labelGroupDescription,
																		  this.textBoxGroupDescription,
																		  this.labelGroupID,
																		  this.textBoxGroupID,
																		  this.buttonAdd});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "GroupEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Group Editor";
			this.ResumeLayout(false);

		}
		#endregion

		private bool FormIsNotComplete()
		{
			if(this.textBoxGroupID.Text == "" ||
				this.textBoxGroupDescription.Text == "")
			{
				MessageBox.Show("Type both Group ID and Group Description!",
								"Updating or Adding group not possible",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error);
				return true;
			}
			else
			{
				return false;
			}
		}
		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			if(this.FormIsNotComplete())
				return;
			
			if(this.tickerGroupsViewer.SelectedGroupID == "")
			// it is a group to be added under the root node
			{
				this.tickerGroupsViewer.AddNewGroupToDataBase(this.textBoxGroupID.Text,
																this.textBoxGroupDescription.Text);
			}
			else
			//// it is a group to be added under the selected group
			{
				this.tickerGroupsViewer.AddNewGroupToDataBase(this.textBoxGroupID.Text,
															  this.textBoxGroupDescription.Text,
															  this.tickerGroupsViewer.SelectedGroupID);
			}
			this.Close();
			
		}

		private void buttonModify_Click(object sender, System.EventArgs e)
		{
			if(this.FormIsNotComplete())
				return;
			
			this.tickerGroupsViewer.ModifyGroup((string)this.selectedNodeInTickerGroupsViewer.Tag,
												this.textBoxGroupDescription.Text,
												this.textBoxGroupID.Text);
			this.Close();
		}

		
	}
}
