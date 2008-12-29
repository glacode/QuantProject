/*
QuantProject - Quantitative Finance Library

DbTypeChooser.cs
Copyright (C) 2008
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

namespace QuantProject.DataAccess
{
	partial class DbTypeChooser
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBoxForRadioButtons = new System.Windows.Forms.GroupBox();
			this.radioButtonInvisible = new System.Windows.Forms.RadioButton();
			this.radioButtonMySql = new System.Windows.Forms.RadioButton();
			this.radioButtonAccess = new System.Windows.Forms.RadioButton();
			this.buttonSelect = new System.Windows.Forms.Button();
			this.groupBoxForRadioButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxForRadioButtons
			// 
			this.groupBoxForRadioButtons.Controls.Add(this.radioButtonInvisible);
			this.groupBoxForRadioButtons.Controls.Add(this.radioButtonMySql);
			this.groupBoxForRadioButtons.Controls.Add(this.radioButtonAccess);
			this.groupBoxForRadioButtons.Location = new System.Drawing.Point(127, 66);
			this.groupBoxForRadioButtons.Name = "groupBoxForRadioButtons";
			this.groupBoxForRadioButtons.Size = new System.Drawing.Size(241, 93);
			this.groupBoxForRadioButtons.TabIndex = 1;
			this.groupBoxForRadioButtons.TabStop = false;
			this.groupBoxForRadioButtons.Text = "Which database type do you want to use?";
			// 
			// radioButtonInvisible
			// 
			this.radioButtonInvisible.Checked = true;
			this.radioButtonInvisible.Location = new System.Drawing.Point(76, 19);
			this.radioButtonInvisible.Name = "radioButtonInvisible";
			this.radioButtonInvisible.Size = new System.Drawing.Size(150, 24);
			this.radioButtonInvisible.TabIndex = 0;
			this.radioButtonInvisible.TabStop = true;
			this.radioButtonInvisible.Text = "radioButtonInvisible";
			this.radioButtonInvisible.UseVisualStyleBackColor = true;
			this.radioButtonInvisible.Visible = false;
			// 
			// radioButtonMySql
			// 
			this.radioButtonMySql.Location = new System.Drawing.Point(158, 46);
			this.radioButtonMySql.Name = "radioButtonMySql";
			this.radioButtonMySql.Size = new System.Drawing.Size(68, 24);
			this.radioButtonMySql.TabIndex = 2;
			this.radioButtonMySql.TabStop = true;
			this.radioButtonMySql.Text = "MySQL";
			this.radioButtonMySql.UseVisualStyleBackColor = true;
			// 
			// radioButtonAccess
			// 
			this.radioButtonAccess.Location = new System.Drawing.Point(26, 46);
			this.radioButtonAccess.Name = "radioButtonAccess";
			this.radioButtonAccess.Size = new System.Drawing.Size(83, 24);
			this.radioButtonAccess.TabIndex = 1;
			this.radioButtonAccess.TabStop = true;
			this.radioButtonAccess.Text = "Access";
			this.radioButtonAccess.UseVisualStyleBackColor = true;
			// 
			// buttonSelect
			// 
			this.buttonSelect.Location = new System.Drawing.Point(174, 203);
			this.buttonSelect.Name = "buttonSelect";
			this.buttonSelect.Size = new System.Drawing.Size(147, 23);
			this.buttonSelect.TabIndex = 2;
			this.buttonSelect.Text = "Select";
			this.buttonSelect.UseVisualStyleBackColor = true;
			this.buttonSelect.Click += new System.EventHandler(this.ButtonSelectClick);
			// 
			// DbTypeChooser
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(494, 293);
			this.Controls.Add(this.buttonSelect);
			this.Controls.Add(this.groupBoxForRadioButtons);
			this.Name = "DbTypeChooser";
			this.Text = "Select the database type";
			this.Load += new System.EventHandler(this.DbTypeChooserLoad);
			this.groupBoxForRadioButtons.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.RadioButton radioButtonInvisible;
		private System.Windows.Forms.Button buttonSelect;
		private System.Windows.Forms.RadioButton radioButtonAccess;
		private System.Windows.Forms.RadioButton radioButtonMySql;
		private System.Windows.Forms.GroupBox groupBoxForRadioButtons;
	}
}
