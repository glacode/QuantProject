/*
QuantProject - Quantitative Finance Library

ConfigManager.cs
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
	partial class MySqlConnectionForm
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
			this.labelServerHost = new System.Windows.Forms.Label();
			this.textBoxServerHost = new System.Windows.Forms.TextBox();
			this.labelUsername = new System.Windows.Forms.Label();
			this.labelPassword = new System.Windows.Forms.Label();
			this.textBoxUsername = new System.Windows.Forms.TextBox();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.buttonOk = new System.Windows.Forms.Button();
			this.textBoxDataBase = new System.Windows.Forms.TextBox();
			this.labelDataBase = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelServerHost
			// 
			this.labelServerHost.Location = new System.Drawing.Point(95, 53);
			this.labelServerHost.Name = "labelServerHost";
			this.labelServerHost.Size = new System.Drawing.Size(100, 23);
			this.labelServerHost.TabIndex = 0;
			this.labelServerHost.Text = "Server Host:";
			this.labelServerHost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxServerHost
			// 
			this.textBoxServerHost.Location = new System.Drawing.Point(197, 55);
			this.textBoxServerHost.Name = "textBoxServerHost";
			this.textBoxServerHost.Size = new System.Drawing.Size(100, 20);
			this.textBoxServerHost.TabIndex = 0;
			this.textBoxServerHost.Text = "localhost";
			// 
			// labelUsername
			// 
			this.labelUsername.Location = new System.Drawing.Point(95, 125);
			this.labelUsername.Name = "labelUsername";
			this.labelUsername.Size = new System.Drawing.Size(100, 23);
			this.labelUsername.TabIndex = 2;
			this.labelUsername.Text = "Username:";
			this.labelUsername.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPassword
			// 
			this.labelPassword.Location = new System.Drawing.Point(95, 161);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(100, 23);
			this.labelPassword.TabIndex = 3;
			this.labelPassword.Text = "Password:";
			this.labelPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxUsername
			// 
			this.textBoxUsername.Location = new System.Drawing.Point(197, 127);
			this.textBoxUsername.Name = "textBoxUsername";
			this.textBoxUsername.Size = new System.Drawing.Size(100, 20);
			this.textBoxUsername.TabIndex = 2;
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Location = new System.Drawing.Point(197, 163);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.Size = new System.Drawing.Size(100, 20);
			this.textBoxPassword.TabIndex = 3;
			this.textBoxPassword.UseSystemPasswordChar = true;
			// 
			// buttonOk
			// 
			this.buttonOk.Location = new System.Drawing.Point(197, 215);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(85, 23);
			this.buttonOk.TabIndex = 4;
			this.buttonOk.Text = "Ok";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.Click += new System.EventHandler(this.ButtonOkClick);
			// 
			// textBoxDataBase
			// 
			this.textBoxDataBase.Location = new System.Drawing.Point(197, 91);
			this.textBoxDataBase.Name = "textBoxDataBase";
			this.textBoxDataBase.Size = new System.Drawing.Size(100, 20);
			this.textBoxDataBase.TabIndex = 1;
			// 
			// labelDataBase
			// 
			this.labelDataBase.Location = new System.Drawing.Point(95, 88);
			this.labelDataBase.Name = "labelDataBase";
			this.labelDataBase.Size = new System.Drawing.Size(100, 23);
			this.labelDataBase.TabIndex = 7;
			this.labelDataBase.Text = "Database:";
			this.labelDataBase.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// MySqlConnectionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(473, 299);
			this.Controls.Add(this.textBoxDataBase);
			this.Controls.Add(this.labelDataBase);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.textBoxPassword);
			this.Controls.Add(this.textBoxUsername);
			this.Controls.Add(this.labelPassword);
			this.Controls.Add(this.labelUsername);
			this.Controls.Add(this.textBoxServerHost);
			this.Controls.Add(this.labelServerHost);
			this.Name = "MySqlConnectionForm";
			this.Text = "MySqlConnectionForm";
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Label labelDataBase;
		private System.Windows.Forms.TextBox textBoxDataBase;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.TextBox textBoxUsername;
		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.Label labelUsername;
		private System.Windows.Forms.TextBox textBoxServerHost;
		private System.Windows.Forms.Label labelServerHost;
	}
}
